package org.bashwork.hqs.database;

import org.bashwork.hqs.protocol.*;

import java.util.List;
import java.util.Optional;

/**
 * Interface that implements a backing store for the message
 * queue system.
 */
public interface HqsDatabase {

    /**
     * Create a new queue with the supplied parameters.
     *
     * @param request The request to handle.
     * @return The optional created queue if it was able to be created.
     */
    Optional<HqsQueue> createQueue(CreateQueueRequest request);

    /**
     * Get the queue specified by the given queue name.
     *
     * @param request The request to handle.
     * @return The optional queue that is mapped to the supplied name.
     */
    Optional<HqsQueue> getQueue(GetQueueUrlRequest request);

    /**
     * Delete the supplied queue and all its messages.
     *
     * @param request The request to handle.
     * @return true if the queue was deleted, false otherwise.
     */
    boolean deleteQueue(DeleteQueueRequest request);

    /**
     * Remove all the current messages from the supplied queue.
     *
     * @param request The request to handle.
     * @return true if the queue was purged, false otherwise.
     */
    boolean purgeQueue(PurgeQueueRequest request);

    /**
     * List all the queueMetadata matching the request.
     *
     * @param request The request to handle.
     * @return The collection of existing queueMetadata.
     */
    List<HqsQueue> listQueues(ListQueuesRequest request);

    /**
     * Send the supplied message to the queue.
     *
     * @param request The request to fulfill.
     * @return The optional message that was sent to the queue.
     */
    Optional<HqsMessage> sendMessage(SendMessageRequest request);

    /**
     * Send the batch of supplied messages to the queue.
     *
     * @param request The request to fulfill.
     * @return The collection of messages that were sent to the queue.
     */
    List<HqsMessage> sendMessageBatch(SendMessageBatchRequest request);

    /**
     * Receive the requested number of available messages.
     *
     * @param request The request to fulfill.
     * @return The collection of received messages.
     */
    List<HqsMessage> receiveMessages(ReceiveMessageRequest request);

    /**
     * Change the visibility timeout before some messages are added back to the queue.
     *
     * @param request The request to fulfill.
     * @return The optional updated message.
     */
    List<HqsMessage> changeMessageVisibilityBatch(ChangeMessageVisibilityBatchRequest request);

    /**
     * Change the visibility timeout before a message is added back to the queue.
     *
     * @param request The request to fulfill.
     * @return The optional updated message.
     */
    Optional<HqsMessage> changeMessageVisibility(ChangeMessageVisibilityRequest request);

    /**
     * Delete a single message that has been handled.
     *
     * @param request The request to fulfill.
     * @return The optional deleted message if it was successful.
     */
    Optional<HqsMessage> deleteMessage(DeleteMessageRequest request);

    /**
     * Delete a batch of supplied messages that have been handled.
     *
     * @param request The request to fulfill.
     * @return The number of messages that were deleted.
     */
    List<HqsMessage> deleteMessageBatch(DeleteMessageBatchRequest request);
}
