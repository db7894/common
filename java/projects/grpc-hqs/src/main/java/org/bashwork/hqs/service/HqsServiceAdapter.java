package org.bashwork.hqs.service;

import org.bashwork.hqs.protocol.*;
import org.bashwork.hqs.database.HqsMessage;
import org.bashwork.hqs.database.HqsQueue;

import java.util.List;
import java.util.UUID;
import java.util.stream.Collectors;

/**
 * A collection of common factory methods for generating
 * data for the Hqs service.
 */
public final class HqsServiceAdapter {
    private HqsServiceAdapter() { }

    /**
     * Generates a new unique identifier for the new message.
     *
     * @return The unique message identifier for the new message.
     */
    public static String generateRequestId() {
        return UUID.randomUUID().toString();
    }

    /**
     * Adapt from the database layer to the service layer.
     *
     * @param message The database layer to convert.
     * @return The converted service layer.
     */
    public static Message adaptMessage(final HqsMessage message) {
        return Message.newBuilder()
            .setBody(message.getBody())
            .setId(message.getIdentifier())
            .setMd5OfMessageBody(message.getMd5HashOfBody())
            .setMd5OfMessageAttributes(message.getMd5HashOfAttributes())
            .setReceiptHandle(message.getReceiptHandle())
            .putAllAttributes(message.getAttributes())
            .build();
    }

    /**
     * Adapt from the database layer to the service layer.
     *
     * @param messages The database layer to convert.
     * @return The converted service layer.
     */
    public static List<Message> adaptMessages(final List<HqsMessage> messages) {
        return messages.stream()
            .map(HqsServiceAdapter::adaptMessage)
            .collect(Collectors.toList());
    }

    /**
     * Adapt from the database layer to the service layer.
     *
     * @param message The database layer to convert.
     * @return The converted service layer.
     */
    public static MessageMetadata adaptMetadata(final HqsMessage message) {
        return MessageMetadata.newBuilder()
            .setId(message.getIdentifier())
            .setMd5OfMessageBody(message.getMd5HashOfBody())
            .setMd5OfMessageAttributes(message.getMd5HashOfAttributes())
            .build();
    }

    /**
     * Adapt from the database layer to the service layer.
     *
     * @param messages The database layer to convert.
     * @return The converted service layer.
     */
    public static List<MessageMetadata> adaptMetadatas(final List<HqsMessage> messages) {
        return messages.stream()
            .map(HqsServiceAdapter::adaptMetadata)
            .collect(Collectors.toList());
    }

    /**
     * Adapt from the database layer to the service layer.
     *
     * @param queue The database layer to convert.
     * @return The converted service layer.
     */
    public static Queue adaptQueue(final HqsQueue queue) {
        return Queue.newBuilder()
            .setQueueName(queue.getName())
            .setQueueUrl(queue.getUrl())
            .setCreatedTime(queue.getCreatedTime().toString())
            .build();
    }

    /**
     * Adapt from the database layer to the service layer.
     *
     * @param queues The database layer to convert.
     * @return The converted service layer.
     */
    public static List<Queue> adaptQueues(final List<HqsQueue> queues) {
        return queues.stream()
            .map(HqsServiceAdapter::adaptQueue)
            .collect(Collectors.toList());
    }
}
