package org.bashwork.hqs.database;

import static java.util.Objects.requireNonNull;

import org.bashwork.hqs.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import javax.inject.Inject;
import java.time.Duration;
import java.time.Instant;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.Optional;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentLinkedDeque;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

/**
 * Implementation of the HqsDatabase that backs up to an
 * in memory collection of structures.
 */
public final class HqsDatabaseImpl implements HqsDatabase {

    private static final Logger logger = LoggerFactory.getLogger(HqsDatabaseImpl.class);

    private final ConcurrentHashMap<String, String> queueUrlToName;
    private final ConcurrentHashMap<String, HqsQueue> queueMetadata;
    private final ConcurrentHashMap<String, HqsMessage> messagesInTransit;
    private final ConcurrentHashMap<String, ConcurrentLinkedDeque<HqsMessage>> queueMessages;
    private final ScheduledExecutorService executor;

    /**
     * Initializes a new instance of the HqsDatabaseImpl class.
     *
     * @param executor The executor to run asynchronous operations on.
     */
    @Inject
    public HqsDatabaseImpl(ScheduledExecutorService executor) {
        this.queueMetadata = new ConcurrentHashMap<>();
        this.queueUrlToName = new ConcurrentHashMap<>();
        this.messagesInTransit = new ConcurrentHashMap<>();
        this.queueMessages = new ConcurrentHashMap<>();
        this.executor = requireNonNull(executor, "HqsDatabase requires an Executor");
    }

    /**
     * @see HqsDatabase#createQueue(CreateQueueRequest)
     */
    @Override
    public Optional<HqsQueue> createQueue(final CreateQueueRequest request) {

        /**
         * If the queue name has already been created, then we will simply use
         * the same values when we update the remaining structures. As such the
         * first created queue will always continue to exist.
         */

        final int messageDelay = request.getDelayInSeconds();
        final int visibilityTimeout = request.getVisibilityTimeoutInSeconds();
        final String queueName = request.getQueueName();
        final HqsQueue queue = queueMetadata.computeIfAbsent(queueName, name -> HqsQueue.newBuilder()
            .setName(name)
            .setUrl(HqsDatabaseAdapter.generateQueueUrl(name))
            .setCreatedTime(Instant.now())
            .setMessageDelay(Duration.ofSeconds(messageDelay))
            .setVisibilityTimeout(Duration.ofSeconds(visibilityTimeout))
            .build());

        logger.debug("created queue {} -> {}", queue.getName(), queue.getUrl());
        queueUrlToName.putIfAbsent(queue.getUrl(), queue.getName());
        queueMessages.putIfAbsent(queue.getUrl(), new ConcurrentLinkedDeque<>());

        return Optional.of(queue);
    }

    /**
     * @see HqsDatabase#getQueue(GetQueueUrlRequest)
     */
    @Override
    public Optional<HqsQueue> getQueue(final GetQueueUrlRequest request) {
        final String queueName = request.getQueueName();
        final HqsQueue queue = queueMetadata.get(queueName);
        return Optional.ofNullable(queue);
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

        final String queueUrl = request.getQueueUrl();
        final String queueName = queueUrlToName.remove(queueUrl);
        final boolean isQueueAvailable = (queueName != null);

        if (isQueueAvailable) {
            queueMetadata.remove(queueName);
            queueMessages.remove(queueUrl);
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

        final String queueUrl = request.getQueueUrl();
        final ConcurrentLinkedDeque<HqsMessage> newQueue = new ConcurrentLinkedDeque<>();
        final ConcurrentLinkedDeque<HqsMessage> queue = queueMessages.replace(queueUrl, newQueue);
        final boolean isQueueAvailable = (queue != null);

        if (isQueueAvailable) {
            logger.debug("purged {} messages from {}", queue.size(), queueUrl);
        }

        return isQueueAvailable;
    }

    /**
     * @see HqsDatabase#listQueues(ListQueuesRequest)
     */
    @Override
    public List<HqsQueue> listQueues(final ListQueuesRequest request) {
        final String namePrefix = request.getQueueNamePrefix();

        if (!namePrefix.isEmpty()) {
            return queueMetadata.values().stream()
                .filter(queue -> queue.getName().startsWith(namePrefix))
                .collect(Collectors.toList());
        }
        return new ArrayList<>(queueMetadata.values());
    }

    /**
     * @see HqsDatabase#sendMessage(SendMessageRequest)
     */
    @Override
    public Optional<HqsMessage> sendMessage(final SendMessageRequest request) {

        final String queueUrl = request.getQueueUrl();
        final String messageBody = request.getMessageBody();
        final Map<String, String> attributes = request.getAttributes();
        final HqsQueue metadata = queueMetadata.get(queueUrl);
        final ConcurrentLinkedDeque<HqsMessage> messages = queueMessages.get(queueUrl);
        final boolean isQueueNotAvailable = (messages == null) || (metadata == null);

        if (isQueueNotAvailable) {
            return Optional.empty();
        }

        final HqsMessage message = HqsDatabaseAdapter.adaptMessage(messageBody, attributes);
        final long delayInSeconds = (request.getDelayInSeconds() <= 0)
            ? metadata.getMessageDelay()
            : request.getDelayInSeconds();

        offerWithDelay(message, delayInSeconds, messages);

        return Optional.ofNullable(message);
    }

    /**
     * @see HqsDatabase#sendMessageBatch(SendMessageBatchRequest)
     */
    @Override
    public List<HqsMessage> sendMessageBatch(final SendMessageBatchRequest request) {

        final String queueUrl = request.getQueueUrl();
        final List<HqsMessage> sentMessages = new ArrayList<>();
        final HqsQueue metadata = queueMetadata.get(queueUrl);
        final ConcurrentLinkedDeque<HqsMessage> messages = queueMessages.get(queueUrl);
        final boolean isQueueNotAvailable = (metadata == null) || (messages == null);

        if (isQueueNotAvailable) {
            return new ArrayList<>();
        }

        for (SendMessageEntry entry : request.getEntriesList()) {
            final HqsMessage message = HqsDatabaseAdapter.adaptMessage(entry);
            final long delayInSeconds = (entry.getDelayInSeconds() <= 0)
                ? metadata.getMessageDelay()
                : entry.getDelayInSeconds();

            offerWithDelay(message, delayInSeconds, messages);
            sentMessages.add(message);
        }

        return sentMessages;
    }

    /**
     * @see HqsDatabase#receiveMessages(ReceiveMessageRequest)
     */
    @Override
    public List<HqsMessage> receiveMessages(final ReceiveMessageRequest request) {
        final String queueUrl = request.getQueueUrl();
        final int maxNumberOfMessages = Math.max(1, request.getMaxNumberOfMessages());
        final List<HqsMessage> receivedMessages = new ArrayList<>();
        final HqsQueue metadata = queueMetadata.get(queueUrl);
        final ConcurrentLinkedDeque<HqsMessage> messages = queueMessages.get(queueUrl);
        final boolean isQueueAvailable = (metadata != null) && (messages != null);

        if (isQueueAvailable) {
            final long visibilityTimeout = (request.getVisibilityTimeoutInSeconds() == 0)
                ? metadata.getVisibilityTimeout()
                : request.getVisibilityTimeoutInSeconds();

            for (int index = maxNumberOfMessages; index > 0; --index) {

                // make a best effort to get as many messages as possible
                final HqsMessage message = messages.poll();
                if (message == null) {
                    break;                // TODO add wait timeout here
                }

                // update all the relevant fields for the touched message
                final HqsMessage messageInTransit = HqsMessage.buildFrom(message)
                    .setReceiveCount(message.getReceiveCount() + 1)
                    .setRecieptHandle(HqsDatabaseAdapter.generateMessageId())
                    .build();

                messagesInTransit.put(messageInTransit.getReceiptHandle(), messageInTransit);
                executor.schedule(() -> addBackToQueue(queueUrl, messageInTransit), visibilityTimeout, TimeUnit.SECONDS);
                receivedMessages.add(messageInTransit); // TODO add the future so we can change visibility an stop
            }
        }

        return receivedMessages;
    }

    /**
     * @see HqsDatabase#deleteMessage(DeleteMessageRequest)
     */
    @Override
    public Optional<HqsMessage> deleteMessage(final DeleteMessageRequest request) {
        final String receiptHandle = request.getReceiptHandle();
        final HqsMessage messageInTransit = messagesInTransit.remove(receiptHandle);
        // TODO cancel future
        return Optional.ofNullable(messageInTransit);
    }

    /**
     * @see HqsDatabase#deleteMessageBatch(DeleteMessageBatchRequest)
     */
    @Override
    public List<HqsMessage> deleteMessageBatch(final DeleteMessageBatchRequest request) {
        final List<String> receiptHandles = request.getReceiptHandlesList();
        // TODO cancel futures
        return receiptHandles.stream()
            .map(messagesInTransit::remove)
            .filter(Objects::nonNull)
            .collect(Collectors.toList());
    }

    /**
     * Callback to add the message back to the queue after if has been timed
     * out from not being deleted in time.
     *
     * @param queueUrl The queue to add the message back to.
     * @param message The message to add back to the queue.
     */
    private void addBackToQueue(final String queueUrl, final HqsMessage message) {
        final HqsMessage messageInProgress = messagesInTransit.remove(message.getReceiptHandle());
        final ConcurrentLinkedDeque<HqsMessage> messages = queueMessages.get(queueUrl);
        final boolean isQueueAvailable = (messageInProgress != null) && (messages != null);

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
            messages.addFirst(message);
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
        if (delayInSeconds > 0) {
            executor.schedule(() -> messages.offer(message), delayInSeconds, TimeUnit.SECONDS);
        } else {
            messages.offer(message);
        }
    }
}
