package com.bashwork.commons.supplier.sqs;

import static com.bashwork.commons.utility.Validate.notBlank;
import static com.bashwork.commons.utility.Validate.notNull;

import java.util.Map;
import java.util.Optional;

import com.bashwork.commons.supplier.ConfirmedSupplier;
import com.bashwork.commons.workflow.WorkflowRequest;
import com.amazonaws.services.sqs.AmazonSQSAsync;
import com.amazonaws.services.sqs.model.DeleteMessageRequest;
import com.amazonaws.services.sqs.model.Message;
import com.amazonaws.services.sqs.model.MessageAttributeValue;
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
 *
 * TODO can we make this generic possibly with the template pattern
 * - delete on retry count too high
 * - return a generic type with an adapt method (message, tracker)
 */
public final class SqsConfirmedSupplier implements ConfirmedSupplier<WorkflowRequest> {

    private static final int SQS_POLL_SECONDS = 10;
    private static final Logger logger = LogManager.getLogger(SqsSupplier.class);

    private final AmazonSQSAsync client;
    private final String queueUrl;
    private final ReceiveMessageRequest request; /* hoisted to save garbage */

    /**
     * Initialize a new instance of the ContentResponseSerializer class.
     *
     * @param client The AmazonSQS client to operate with.
     * @param queueUrl The URL of the queue to operate with.
     */
    public SqsConfirmedSupplier(AmazonSQSAsync client, String queueUrl) {
        this.client = notNull(client, "client");
        this.queueUrl = notBlank(queueUrl, "queueUrl");
        this.request = new ReceiveMessageRequest()
            .withMaxNumberOfMessages(1)
            .withMessageAttributeNames(SqsAttribute.ALL)
            .withQueueUrl(queueUrl)
            .withWaitTimeSeconds(SQS_POLL_SECONDS);
    }

    @Override
    public Optional<WorkflowRequest> get() {
        ReceiveMessageResult response = client.receiveMessage(request);
        if (response.getMessages().isEmpty()) {
            return Optional.empty();
        }

        Message message = response.getMessages().get(0);
        return Optional.of(adapt(message));
    }

    /**
     * Delete the supplied message from the queue
     * indicating that it has been handled.
     *
     * @param message The message to delete.
     */
    @Override
    public void confirm(WorkflowRequest message) {
        DeleteMessageRequest request = new DeleteMessageRequest()
            .withQueueUrl(queueUrl)
            .withReceiptHandle(message.getTracker());

        logger.debug("deleting new message: {}", request);
        client.deleteMessageAsync(request);
    }

    /**
     * Given an SQS message, convert this to a WorkflowRequest.
     *
     * @param message The SQS message to convert to a workflow request.
     * @return The adapted workflow request.
     */
    private WorkflowRequest adapt(Message message) {
        Map<String, MessageAttributeValue> attributes = message.getMessageAttributes();

        return WorkflowRequest.builder()
            .withMessage(message.getBody())
            .withTracker(message.getReceiptHandle())
            .withType(attributes.get(SqsAttribute.WORKFLOW_TYPE).getStringValue())
            .build();
    }
}
