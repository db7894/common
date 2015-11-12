package org.bashwork.hqs.database;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import javax.inject.Inject;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.Optional;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentLinkedDeque;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

/**
 *
 */
public final class HqsDatabaseImpl implements HqsDatabase {

    private static final Logger logger = LoggerFactory.getLogger(HqsDatabaseImpl.class);

    private final ConcurrentHashMap<String, String> queueUrlToName;
    private final ConcurrentHashMap<String, HqsQueue> queues;
    private final ConcurrentHashMap<String, HqsMessage> messagesInTransit;
    private final ConcurrentHashMap<String, ConcurrentLinkedDeque<HqsMessage>> queueMessages;
    private final ScheduledExecutorService executor;

    /**
     *
     * @param executor
     */
    @Inject
    public HqsDatabaseImpl(ScheduledExecutorService executor) {
        this.queues = new ConcurrentHashMap<>();
        this.queueUrlToName = new ConcurrentHashMap<>();
        this.messagesInTransit = new ConcurrentHashMap<>();
        this.queueMessages = new ConcurrentHashMap<>();
        this.executor = Objects.requireNonNull(executor, "Store executor is required");
    }

    /**
     *
     * @param queueName The name of the queue to create.
     * @return The optional created queue if it was able to be created.
     */
    @Override
    public Optional<HqsQueue> createQueue(final String queueName) {

        /**
         * If the queue name has already been created, then we will simply use
         * the same values when we update the remaining structures. As such the
         * first created queue will always continue to exist.
         */

        final HqsQueue queue = queues.computeIfAbsent(queueName, name -> HqsQueue.newBuilder()
            .setName(name)
            .setUrl(HqsFactory.generateQueueUrl(name))
            .build());

        logger.debug("created queue {} -> {}", queue.getName(), queue.getUrl());
        queueUrlToName.putIfAbsent(queue.getUrl(), queue.getName());
        queueMessages.putIfAbsent(queue.getUrl(), new ConcurrentLinkedDeque<>());

        return Optional.ofNullable(queue);
    }

    /**
     *
     * @param queueName The name of the queue to retrieve.
     * @return The optional queue that is mapped to the supplied name.
     */
    @Override
    public Optional<HqsQueue> getQueue(String queueName) {
        final HqsQueue queue = queues.get(queueName);
        return Optional.ofNullable(queue);
    }

    /**
     *
     * @param queueUrl
     * @return
     */
    @Override
    public boolean deleteQueue(String queueUrl) {

        /**
         * If another user has beaten us to deleting the url to name
         * mapping, then it will be null and the remaining structures
         * will be safe from mutation.
         */

        final String queueName = queueUrlToName.remove(queueUrl);

        if (queueName != null) {
            queues.remove(queueName);
            queueMessages.remove(queueUrl);
        }

        return true;
    }

    /**
     *
     * @param queueUrl The queue to purge all the messages from.
     * @return
     */
    @Override
    public boolean purgeQueue(String queueUrl) {

        /**
         * We simple replace the existing deque with a new one which is atomic
         * and will 'clear' all messages. If there is a current operation with
         * the existing queue, we simple take the last write wins and thus it
         * is purged as well.
         */

        final ConcurrentLinkedDeque<HqsMessage> messages = queueMessages
            .replace(queueUrl, new ConcurrentLinkedDeque<>());

        if (messages != null) {
            logger.debug("purged {} messages from {}", messages.size(), queueUrl);
        }

        return true;
    }

    /**
     *
     * @return The collection of existing queues.
     */
    @Override
    public List<HqsQueue> listQueues() {
        return new ArrayList<>(queues.values());
    }

    /**
     *
     * @param queueUrl The queue to send the message to.
     * @param messageBody The message to send to the queue.
     * @return The optional message that was sent to the queue.
     */
    @Override
    public Optional<HqsMessage> sendMessage(String queueUrl, String messageBody) {

        final ConcurrentLinkedDeque<HqsMessage> queue = queueMessages.get(queueUrl);
        if (queue == null) {
            return Optional.empty();
        }

        final HqsMessage message = HqsFactory.generateMessage(messageBody);
        queue.offer(message);

        return Optional.ofNullable(message);
    }

    /**
     *
     * @param queueUrl The queue to send messages to.
     * @param messageBodies The messages to send to the queue.
     * @return The collection of messages that were sent to the queue.
     */
    @Override
    public List<HqsMessage> sendMessageBatch(String queueUrl, List<String> messageBodies) {

        final ConcurrentLinkedDeque<HqsMessage> queue = queueMessages.get(queueUrl);
        if (queue == null) {
            return new ArrayList<>();
        }

        final List<HqsMessage> messages = messageBodies.stream()
            .map(HqsFactory::generateMessage)
            .collect(Collectors.toList());

        queue.addAll(messages); // TODO check order

        return messages;
    }

    /**
     *
     * @param queueUrl The queue to receive messages from.
     * @param count The number of message to receive.
     * @return The collection of received messages.
     */
    @Override
    public List<HqsMessage> receiveMessages(final String queueUrl, int count) {
        final List<HqsMessage> messages = new ArrayList<>();
        final ConcurrentLinkedDeque<HqsMessage> queue = queueMessages.get(queueUrl);

        if (queue != null) {
            for (int i = Math.max(1, count); i > 0; --i) {
                final HqsMessage message = queue.poll();
                // TODO add wait timeout here
                if (message == null) {
                    break;
                }

                messagesInTransit.put(message.getIdentifier(), message);
                // TODO configure this visibility here
                // TODO add message handle here
                executor.schedule(() -> addBackToQueue(queueUrl, message), 5, TimeUnit.MINUTES);
                messages.add(message);
            }
        }

        return messages;
    }

    /**
     *
     * @param receiptHandle The identifier to remove the message for.
     * @return The optional deleted message if it was successful.
     */
    @Override
    public Optional<HqsMessage> deleteMessage(String receiptHandle) {
        final HqsMessage message = messagesInTransit.remove(receiptHandle);
        return Optional.ofNullable(message);
    }

    /**
     *
     * @param receiptHandles The identifiers to remove messages for.
     * @return The number of messages that were deleted.
     */
    @Override
    public List<HqsMessage> deleteMessageBatch(List<String> receiptHandles) {
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
        final HqsMessage messageInProgress = messagesInTransit.remove(message.getIdentifier());
        final ConcurrentLinkedDeque<HqsMessage> queue = queueMessages.get(queueUrl);

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

        if ((messageInProgress != null) && (queue != null)) {
            queue.addFirst(message);
        }
    }
}
