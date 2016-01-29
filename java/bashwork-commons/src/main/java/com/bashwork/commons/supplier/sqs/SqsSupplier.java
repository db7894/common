package com.bashwork.commons.supplier.sqs;

import static com.bashwork.commons.utility.Validate.notBlank;
import static com.bashwork.commons.utility.Validate.notNull;

import java.util.function.Supplier;

import com.bashwork.commons.serialize.Serializer;
import com.amazonaws.services.sqs.AmazonSQSAsync;
import com.amazonaws.services.sqs.model.DeleteMessageRequest;
import com.amazonaws.services.sqs.model.Message;
import com.amazonaws.services.sqs.model.ReceiveMessageRequest;
import com.amazonaws.services.sqs.model.ReceiveMessageResult;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * A supplied for a message from SQS. If you are publishing a message
 * from SNS to SQS, make sure that when you attach the queue to the
 * topic you mark the configuration option "Send Raw." Otherwise, the
 * message will be a serialized SNS message that contains the original
 * serialized message.
 */
public class SqsSupplier<TSupply> implements Supplier<TSupply> {

    private static final int SQS_POLL_SECONDS = 10;
    private static final Logger logger = LogManager.getLogger(SqsSupplier.class);

    private final AmazonSQSAsync client;
    private final String queueUrl;
    private final ReceiveMessageRequest request; /* hoisted to save garbage */
    private final Serializer<String> serializer;
    private final Class<TSupply> klass;

    /**
     * Initialize a new instance of the ContentResponseSerializer class.
     *
     * @param client The AmazonSQS client to operate with.
     * @param queueUrl The URL of the queue to operate with.
     * @param serializer The serializer to deserialize a message with.
     * @param klass The underlying class to convert the message to.
     */
    public SqsSupplier(AmazonSQSAsync client, String queueUrl,
        Serializer<String> serializer, Class<TSupply> klass) {

        this.client = notNull(client, "client");
        this.queueUrl = notBlank(queueUrl, "queueUrl");
        this.serializer = notNull(serializer, "serializer");
        this.klass = notNull(klass, "class");
        this.request = new ReceiveMessageRequest()
            .withMaxNumberOfMessages(1)
            .withQueueUrl(queueUrl)
            .withWaitTimeSeconds(SQS_POLL_SECONDS);
    }

    @Override
    public TSupply get() {
        ReceiveMessageResult response = client.receiveMessage(request);
        if (response.getMessages().isEmpty()) {
            return null;
        }

        Message message = response.getMessages().get(0);
        delete(message);
        return serializer.deserialize(message.getBody(), klass);
    }

    /**
     * Delete the supplied message from the queue
     * indicating that it has been handled.
     *
     * @param message The message to delete.
     */
    public void delete(Message message) {
        DeleteMessageRequest request = new DeleteMessageRequest()
            .withQueueUrl(queueUrl)
            .withReceiptHandle(message.getReceiptHandle());

        logger.debug("deleting new message: {}", request);
        client.deleteMessageAsync(request);
    }
}
