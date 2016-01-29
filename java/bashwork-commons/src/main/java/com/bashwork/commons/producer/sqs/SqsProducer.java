package com.bashwork.commons.producer.sqs;

import static com.bashwork.commons.utility.Validate.notNull;

import com.bashwork.commons.producer.Producer;
import com.bashwork.commons.serialize.Serializer;
import com.bashwork.commons.supplier.sqs.SqsAttribute;
import com.amazonaws.services.sqs.AmazonSQSAsync;
import com.amazonaws.services.sqs.model.MessageAttributeValue;
import com.amazonaws.services.sqs.model.SendMessageRequest;
import com.amazonaws.services.sqs.model.SendMessageResult;
import com.bashwork.commons.supplier.sqs.SqsSupplier;
import com.google.common.collect.ImmutableMap;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.time.Duration;

/**
 * A supplied for a message from SQS. If you are publishing a message
 * from SNS to SQS, make sure that when you attach the queue to the
 * topic you mark the configuration option "Send Raw." Otherwise, the
 * message will be a serialized SNS message that contains the original
 * serialized message.
 */
public final class SqsProducer implements Producer {

    private static final Logger logger = LogManager.getLogger(SqsSupplier.class);

    private final AmazonSQSAsync client;
    private final String queueUrl;
    private final Serializer<String> serializer;

    /**
     * Initialize a new instance of the ContentResponseSerializer class.
     *
     * @param client The AmazonSQS client to operate with.
     * @param queueUrl The URL of the queue to operate with.
     * @param serializer The serializer to serialize messages with.
     */
    public SqsProducer(AmazonSQSAsync client, String queueUrl, Serializer<String> serializer) {
        this.client = notNull(client, "client");
        this.queueUrl = notNull(queueUrl, "queueUrl");
        this.serializer = notNull(serializer, "serializer");
    }

    /**
     * @see Producer#produce(Object)
     */
    @Override
    public <TProduct> String produce(TProduct message) {
        return produce(message, new SendMessageRequest());
    }

    /**
     * @see Producer#produce(Object, Duration)
     */
    @Override
    public <TProduct> String produce(TProduct message, Duration delay) {
        return produce(message, new SendMessageRequest()
            .withDelaySeconds((int) delay.getSeconds()));
    }

    /**
     * Helper method to produce a given message.
     *
     * @param message The message to produce.
     * @param request The current request to produce with.
     * @param <TProduct> The type of the message to produce.
     * @return The result og producing a message.
     */
    private <TProduct> String produce(TProduct message, SendMessageRequest request) {
        request
            .withQueueUrl(queueUrl)
            .withMessageBody(serializer.serialize(message))
            .withMessageAttributes(ImmutableMap.of(
                SqsAttribute.WORKFLOW_TYPE, new MessageAttributeValue()
                    .withDataType(SqsAttribute.STRING)
                    .withStringValue(message.getClass().getName())));

        logger.debug("producing new message: {}", request);
        SendMessageResult response = client.sendMessage(request);
        return response.getMessageId();
    }
}
