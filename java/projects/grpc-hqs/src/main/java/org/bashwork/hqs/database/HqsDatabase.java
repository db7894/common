package org.bashwork.hqs.database;

import java.util.List;
import java.util.Map;
import java.util.Optional;

/**
 * Interface that implements a backing store for the message
 * queue system.
 */
public interface HqsDatabase {

    Optional<HqsQueue> createQueue(String queueName);

    Optional<HqsQueue> getQueue(String queueName);

    boolean deleteQueue(String queueUrl);

    boolean purgeQueue(String queueUrl);

    List<HqsQueue> listQueues();

    Optional<HqsMessage> sendMessage(String queueUrl, String payload, Map<String, String> attributes);

    List<HqsMessage> sendMessageBatch(String queueUrl, List<String> payload);

    List<HqsMessage> receiveMessages(String queueUrl, int count);

    Optional<HqsMessage> deleteMessage(String identifier);

    List<HqsMessage> deleteMessageBatch(List<String> identifiers);
}
