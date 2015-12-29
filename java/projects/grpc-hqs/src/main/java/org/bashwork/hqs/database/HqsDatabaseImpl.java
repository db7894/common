package org.bashwork.hqs.database;

import static org.bashwork.hqs.service.HqsConstraints.*;
import static java.util.Objects.requireNonNull;
import static org.bashwork.hqs.database.HqsDatabaseAdapter.*;
import static org.bashwork.hqs.utility.MoreObjects.*;

import org.bashwork.hqs.protocol.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import com.codahale.metrics.MetricRegistry;
import com.codahale.metrics.Gauge;

import javax.inject.Inject;
import java.time.Duration;
import java.time.Instant;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentLinkedDeque;
import java.util.concurrent.Future;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

/**
 * Implementation of the HqsDatabase that backs up to an
 * in memory collection of structures.
 */
public final class HqsDatabaseImpl implements HqsDatabase {

    //
    // TODO validate constraints set by AWS
    // - maximum messages per queue (120000) + O(N) check
    // - maximum delay per message (12 hours) + visibility is additive
    //

    private static final String MISSING = "";
    private static final boolean MAY_INTERRUPT = true;
    private static final Logger logger = LoggerFactory.getLogger(HqsDatabaseImpl.class);

    private final MetricRegistry metrics;
    private final ConcurrentHashMap<String, String> queueUrlToName;
    private final ConcurrentHashMap<String, HqsQueue> queueMetadata;
    private final ConcurrentHashMap<String, MessageTask> messageTasks;
    private final ConcurrentHashMap<String, ConcurrentLinkedDeque<HqsMessage>> queueMessages;
    private final ScheduledExecutorService executor;

    /**
     * A collection of common metrics used in the database
     */
    private final class Metric {
        private static final String Namespace = "org.bashwork.hqs.database";

        static final String QueuesCreated = Namespace + ".queues-created";
        static final String QueuesDeleted = Namespace + ".queues-deleted";
        static final String QueueCount = Namespace + ".queue-count";
        static final String MessagesSent = Namespace + ".messages-sent";
        static final String MessageCount = Namespace + ".messages-count";
        static final String MessagesReceived = Namespace + ".messages-received";
        static final String MessagesDeleted = Namespace + ".messages-deleted";
    }

    /**
     * Initializes a new instance of the HqsDatabaseImpl class.
     *
     * @param executor The executor to run asynchronous operations on.
     * @param metrics The metrics registry to send metrics to.
     */
    @Inject
    public HqsDatabaseImpl(ScheduledExecutorService executor, MetricRegistry metrics) {
        this.queueMetadata = new ConcurrentHashMap<>();
        this.queueUrlToName = new ConcurrentHashMap<>();
        this.messageTasks = new ConcurrentHashMap<>();
        this.queueMessages = new ConcurrentHashMap<>();
        this.executor = requireNonNull(executor, "HqsDatabase requires an Executor");
        this.metrics = requireNonNull(metrics, "HqsDatabase requires a MetricsRegistry");

        registerMetrics();
    }

    /**
     * @see HqsDatabase#createQueue(CreateQueueRequest)
     */
    @Override
    public Optional<HqsQueue> createQueue(final CreateQueueRequest request) {

        /**
         * This is the locking operation. The first client to create the queue
         * will win and the resulting clients will simply perform the same work
         * using the winning queue. We separate the queueUrl generation so we can
         * check if _we_ won the creation.
         */

        final String queueUrl = generateQueueUrl(request.getQueueName());
        final HqsQueue queue = queueMetadata.computeIfAbsent(request.getQueueName(),
            (key) -> HqsQueue.newBuilder()
                .setName(request.getQueueName())
                .setUrl(queueUrl)
                .setCreatedTime(Instant.now())
                .setMessageDelay(Duration.ofSeconds(request.getDelayInSeconds()))
                .setVisibilityTimeout(Duration.ofSeconds(request.getVisibilityInSeconds()))
                .setMaxMessageSize(firstPositive(request.getMaxMessageSize(), MAX_MESSAGE_SIZE))
                .build());

        final boolean isCreated = queue.getName().equals(queueUrlToName.get(queueUrl));

        if (!isCreated) {
            logger.debug("attempted to create existing queue {}", queue.getName());
            return Optional.empty();
        }

        /**
         * These will be no-ops if the queue already exists, however
         * they will not be reached if the queue was not just created.
         */

        queueUrlToName.putIfAbsent(queue.getUrl(), queue.getName());
        queueMessages.putIfAbsent(queue.getUrl(), new ConcurrentLinkedDeque<>());

        logger.debug("created queue {} -> {}", queue.getName(), queue.getUrl());
        metrics.counter(Metric.QueuesCreated).inc();
        metrics.counter(Metric.QueueCount).inc();

        return Optional.of(queue);
    }

    /**
     * @see HqsDatabase#getQueue(GetQueueUrlRequest)
     */
    @Override
    public Optional<HqsQueue> getQueue(final GetQueueUrlRequest request) {
        final HqsQueue queue = queueMetadata.get(request.getQueueName());
        return Optional.ofNullable(queue);
    }

    /**
     * @see HqsDatabase#getQueueAttributes(GetQueueAttributesRequest)
     */
    @Override
    public Optional<HqsQueue> getQueueAttributes(final GetQueueAttributesRequest request) {
        final String queueName = queueUrlToName.getOrDefault(request.getQueueUrl(), MISSING);
        final HqsQueue queue = queueMetadata.get(queueName);

        return Optional.ofNullable(queue);
    }

    /**
     * @see HqsDatabase#setQueueAttributes(SetQueueAttributesRequest)
     */
    @Override
    public Optional<HqsQueue> setQueueAttributes(final SetQueueAttributesRequest request) {
        final String queueName = queueUrlToName.getOrDefault(request.getQueueUrl(), MISSING);
        final HqsQueue oldQueue = queueMetadata.get(queueName);
        final boolean isQueueAvailable = (oldQueue != null);

        if (isQueueAvailable) {
            // TODO get largest value
            final HqsQueue newQueue = HqsQueue.buildFrom(oldQueue)
                .setMaxMessageSize(request.getMaxMessageSize())
                .setMessageDelay(Duration.ofSeconds(request.getDelayInSeconds()))
                .setVisibilityTimeout(Duration.ofSeconds(request.getVisibilityInSeconds()))
                .build();

            /**
             * We only try to replace the metadata with the new values once
             * and if someone beat us to it, we abort and return the current
             * values to the customer so they can try again.
             */

            return queueMetadata.replace(queueName, oldQueue, newQueue)
                ? Optional.of(newQueue)
                : Optional.of(queueMetadata.get(queueName));
        }

        return Optional.empty();
    }

    /**
     * @see HqsDatabase#deleteQueue(DeleteQueueRequest)
     */
    @Override
    public boolean deleteQueue(final DeleteQueueRequest request) {

        /**
         * If another user has beaten us to deleting the url to name
         * mapping, then it will be null and the remaining structures
         * will be safe from mutation.
         */

        final String queueName = queueUrlToName.remove(request.getQueueUrl());
        final boolean isQueueAvailable = (queueName != null);

        if (isQueueAvailable) {
            metrics.counter(Metric.QueuesDeleted).inc();
            metrics.counter(Metric.QueueCount).dec();

            queueMetadata.remove(queueName);
            queueMessages.remove(request.getQueueUrl());
        }

        return isQueueAvailable;
    }

    /**
     * @see HqsDatabase#purgeQueue(PurgeQueueRequest)
     */
    @Override
    public boolean purgeQueue(final PurgeQueueRequest request) {

        /**
         * We simple replace the existing deque with a new one which is atomic
         * and will 'clear' all messages. If there is a current operation with
         * the existing queue, we simple take the last write wins and thus it
         * is purged as well.
         */

        final ConcurrentLinkedDeque<HqsMessage> newQueue = new ConcurrentLinkedDeque<>();
        final ConcurrentLinkedDeque<HqsMessage> queue = queueMessages.replace(request.getQueueUrl(), newQueue);
        final boolean isQueueAvailable = (queue != null);

        if (isQueueAvailable) {
            logger.debug("purged {} messages from {}", queue.size(), request.getQueueUrl());
        }

        return isQueueAvailable;
    }

    /**
     * @see HqsDatabase#listQueues(ListQueuesRequest)
     */
    @Override
    public List<HqsQueue> listQueues(final ListQueuesRequest request) {
        final String prefix = request.getQueueNamePrefix();
        final boolean isPrefixValid = (prefix != null);

        if (isPrefixValid) {
            return queueMetadata.values().stream()
                .filter(queue -> queue.getName().startsWith(prefix))
                .limit(MAX_LISTED_QUEUE_NAMES)
                .collect(Collectors.toList());
        }
        return new ArrayList<>(queueMetadata.values());
    }

    /**
     * @see HqsDatabase#sendMessage(SendMessageRequest)
     */
    @Override
    public Optional<HqsMessage> sendMessage(final SendMessageRequest request) {
        final String queueName = queueUrlToName.getOrDefault(request.getQueueUrl(), MISSING);
        final HqsQueue metadata = queueMetadata.get(queueName);
        final ConcurrentLinkedDeque<HqsMessage> messages = queueMessages.get(request.getQueueUrl());
        final boolean isQueueNotAvailable = (messages == null) || (metadata == null);

        if (isQueueNotAvailable) {
            logger.debug("requested queue {} does not exist", request.getQueueUrl());
            return Optional.empty();
        }

        final HqsMessage message = adaptMessage(request.getMessageBody(), request.getAttributes());
        final long delayInSeconds = firstPositive(request.getDelayInSeconds(), metadata.getMessageDelay());

        offerWithDelay(message, delayInSeconds, messages);

        metrics.counter(Metric.MessagesSent).inc();
        metrics.counter(Metric.MessageCount).inc();

        return Optional.ofNullable(message);
    }

    /**
     * @see HqsDatabase#sendMessageBatch(SendMessageBatchRequest)
     */
    @Override
    public List<HqsMessage> sendMessageBatch(final SendMessageBatchRequest request) {
        final String queueName = queueUrlToName.getOrDefault(request.getQueueUrl(), MISSING);
        final HqsQueue metadata = queueMetadata.get(queueName);
        final ConcurrentLinkedDeque<HqsMessage> messages = queueMessages.get(request.getQueueUrl());
        final List<HqsMessage> sentMessages = new ArrayList<>();
        final boolean isQueueNotAvailable = (metadata == null) || (messages == null);

        if (isQueueNotAvailable) {
            logger.debug("requested queue {} does not exist", request.getQueueUrl());
            return sentMessages;
        }

        for (SendMessageEntry messageEntry : request.getEntriesList()) {
            final HqsMessage newMessage = adaptMessage(messageEntry);
            final long delayInSeconds = firstPositive(messageEntry.getDelayInSeconds(), metadata.getMessageDelay());

            offerWithDelay(newMessage, delayInSeconds, messages);
            sentMessages.add(newMessage);
        }

        metrics.counter(Metric.MessagesSent).inc(sentMessages.size());
        metrics.counter(Metric.MessageCount).inc(sentMessages.size());

        return sentMessages;
    }

    /**
     * @see HqsDatabase#receiveMessages(ReceiveMessageRequest)
     */
    @Override
    public List<HqsMessage> receiveMessages(final ReceiveMessageRequest request) {
        final int maxNumberOfMessages = Math.max(1, request.getMaxNumberOfMessages());
        final String queueName = queueUrlToName.getOrDefault(request.getQueueUrl(), MISSING);
        final HqsQueue metadata = queueMetadata.get(queueName);
        final ConcurrentLinkedDeque<HqsMessage> messages = queueMessages.get(request.getQueueUrl());
        final List<HqsMessage> receivedMessages = new ArrayList<>();
        final boolean isQueueNotAvailable = (metadata == null) || (messages == null);

        if (isQueueNotAvailable) {
            logger.debug("requested queue {} does not exist", request.getQueueUrl());
            return receivedMessages;
        }

        final long visibilityTimeout = firstPositive(request.getVisibilityInSeconds(), metadata.getVisibilityTimeout());

        for (int index = maxNumberOfMessages; index > 0; --index) {

            // make a best effort to get as many messages as possible
            final HqsMessage oldMessage = messages.poll();
            final boolean isQueueEmpty = (oldMessage == null);

            // if there are no more messages and we will not wait anymore
            if (isQueueEmpty) {
                break;                // TODO add wait timeout here
            }

            // update all the relevant fields for the touched message
            final String receiptHandle = generateMessageId();
            final Instant firstReceived = firstNonNull(oldMessage.getFirstReceived(), Instant::now);
            final HqsMessage newMessage = HqsMessage.buildFrom(oldMessage)
                .setReceiveCount(oldMessage.getReceiveCount() + 1)
                .setRecieptHandle(receiptHandle)
                .setFirstReceived(firstReceived)
                .build();

            // schedule the job to add back to the queue
            final Runnable runnable = () -> addBackToQueue(request.getQueueUrl(), receiptHandle);
            final Future<?> future = executor.schedule(runnable, visibilityTimeout, TimeUnit.SECONDS);
            final MessageTask task = new MessageTask(future, newMessage, runnable);

            messageTasks.put(receiptHandle, task);
            receivedMessages.add(newMessage);
        }

        metrics.counter(Metric.MessagesReceived).inc(receivedMessages.size());

        return receivedMessages;
    }

    /**
     * @see HqsDatabase#changeMessageVisibility(ChangeMessageVisibilityRequest)
     */
    @Override
    public Optional<HqsMessage> changeMessageVisibility(ChangeMessageVisibilityRequest request) {
        final MessageTask task = messageTasks.remove(request.getReceiptHandle());
        final boolean isTaskNotAvailable = (task == null);

        if (isTaskNotAvailable) {
            logger.debug("requested message {} does not exist", request.getReceiptHandle());
            return Optional.empty();
        }

        task.getFuture().cancel(MAY_INTERRUPT);
        final Future<?> future = executor.schedule(task.getRunnable(), request.getVisibilityInSeconds(), TimeUnit.SECONDS);
        final MessageTask newTask = new MessageTask(future, task.getMessage(), task.getRunnable());

        messageTasks.put(request.getReceiptHandle(), newTask);
        return Optional.ofNullable(task.getMessage());
    }

    /**
     * @see HqsDatabase#changeMessageVisibilityBatch(ChangeMessageVisibilityBatchRequest)
     */
    @Override
    public List<HqsMessage> changeMessageVisibilityBatch(ChangeMessageVisibilityBatchRequest request) {
        final List<HqsMessage> messages = new ArrayList<>();

        for (ChangeMessageVisibilityEntry entry : request.getEntriesList()) {
            final MessageTask task = messageTasks.remove(entry.getReceiptHandle());
            final boolean isTaskAvailable = (task != null);

            if (isTaskAvailable) {
                task.getFuture().cancel(MAY_INTERRUPT);
                final Future<?> future = executor.schedule(task.getRunnable(), entry.getVisibilityInSeconds(), TimeUnit.SECONDS);
                final MessageTask newTask = new MessageTask(future, task.getMessage(), task.getRunnable());

                messageTasks.put(entry.getReceiptHandle(), newTask);
                messages.add(task.getMessage());
            }
        }

        return messages;
    }

    /**
     * @see HqsDatabase#deleteMessage(DeleteMessageRequest)
     */
    @Override
    public Optional<HqsMessage> deleteMessage(final DeleteMessageRequest request) {
        final String receiptHandle = request.getReceiptHandle();
        final MessageTask task = messageTasks.remove(receiptHandle);
        final boolean isTaskMissing = (task == null);
        
        if (isTaskMissing) {
            return Optional.empty();
        }

        task.getFuture().cancel(MAY_INTERRUPT);

        metrics.counter(Metric.MessagesDeleted).inc();
        metrics.counter(Metric.MessageCount).dec();

        return Optional.ofNullable(task.getMessage());
    }

    /**
     * @see HqsDatabase#deleteMessageBatch(DeleteMessageBatchRequest)
     */
    @Override
    public List<HqsMessage> deleteMessageBatch(final DeleteMessageBatchRequest request) {
        final List<HqsMessage> messages = new ArrayList<>();

        for (final String receiptHandle : request.getReceiptHandlesList()) {
            final MessageTask task = messageTasks.remove(receiptHandle);
            final boolean isTaskAvailable = (task != null);

            if (isTaskAvailable) {
                task.getFuture().cancel(MAY_INTERRUPT);
                messages.add(task.getMessage());
            }
        }

        metrics.counter("org.bashwork.hqs.database.messages-deleted").inc(messages.size());
        metrics.counter("org.bashwork.hqs.database.messages-count").dec(messages.size());

        return messages;
    }

    /**
     * Callback to add the message back to the queue after if has been timed
     * out from not being deleted in time.
     *
     * @param queueUrl The queue to add the message back to.
     * @param receiptHandle The receiptHandle for the message to add back.
     */
    private void addBackToQueue(final String queueUrl, final String receiptHandle) {
        final MessageTask task = messageTasks.remove(receiptHandle);
        final ConcurrentLinkedDeque<HqsMessage> messages = queueMessages.get(queueUrl);
        final boolean isQueueAvailable = (task != null) && (messages != null);

        /**
         * If the message still exists in the transit map and the queue still exists,
         * then we can simple add it back to the queue. If another thread attempts to
         * remove the message later, it will simple fail as we already got it here. If
         * the queue is removed while we are adding to it, then our operation will
         * succeed, however as soon as we finish the GC will simply mark the reference
         * as available for cleaning.
         *
         * If the message still exists, but the queue does not exist, then we will delete
         * the message anyways.
         */

        if (isQueueAvailable) {
            messages.addFirst(task.getMessage());
        }
    }

    /**
     * Helper method to offer with delay for a given queue message.
     *
     * @param message The message to offer to the queue.
     * @param delayInSeconds The amount of time to delay before sending.
     * @param messages The queue to offer the message on.
     */
    private void offerWithDelay(final HqsMessage message, final long delayInSeconds,
        final ConcurrentLinkedDeque<HqsMessage> messages) {
        final boolean isDelayed = (delayInSeconds > 0);

        if (isDelayed) {
            executor.schedule(() -> messages.offer(message), delayInSeconds, TimeUnit.SECONDS);
        } else {
            messages.offer(message);
        }
    }

    private void registerMetrics() {
        metrics.register("org.bashwork.hqs.database.task-count",
            (Gauge<Integer>) messageTasks::size);
    }

    /**
     * A helper class that can be used to keep a message and
     * its task together.
     */
    private static final class MessageTask {
        private final Future<?> future;
        private final HqsMessage message;
        private final Runnable runnable;

        public MessageTask(Future<?> future, HqsMessage message, Runnable runnable) {
            this.future = future;
            this.message = message;
            this.runnable = runnable;
        }

        public HqsMessage getMessage() {
            return message;
        }

        public Future<?> getFuture() {
            return future;
        }

        public Runnable getRunnable() {
            return runnable;
        }
    }
}
