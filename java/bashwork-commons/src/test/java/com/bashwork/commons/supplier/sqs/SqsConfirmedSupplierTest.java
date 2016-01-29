package com.bashwork.commons.supplier.sqs;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;
import static org.mockito.Matchers.any;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import java.util.Optional;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.runners.MockitoJUnitRunner;

import com.bashwork.commons.serializer.Serializer;
import com.bashwork.commons.serializer.TestPojo;
import com.bashwork.commons.serializer.string.JsonStringSerializer;
import com.bashwork.commons.workflow.WorkflowRequest;
import com.amazonaws.services.sqs.AmazonSQSAsync;
import com.amazonaws.services.sqs.model.DeleteMessageRequest;
import com.amazonaws.services.sqs.model.Message;
import com.amazonaws.services.sqs.model.MessageAttributeValue;
import com.amazonaws.services.sqs.model.ReceiveMessageRequest;
import com.amazonaws.services.sqs.model.ReceiveMessageResult;
import com.google.common.collect.ImmutableMap;

@RunWith(MockitoJUnitRunner.class)
public class SqsConfirmedSupplierTest {

    private final Serializer<String> serializer = new JsonStringSerializer();
    private final TestPojo message = new TestPojo("Mark", 22);

    private final String type = TestPojo.class.getName();
    private final String serialized = serializer.serialize(message);
    private final String queueUrl = "producer-message-queue";
    private final String messageHandle = "message-handle";

    private final WorkflowRequest request = WorkflowRequest.builder()
        .withMessage(serialized)
        .withTracker(messageHandle)
        .withType(type)
        .build();

    private final ReceiveMessageResult result = new ReceiveMessageResult()
    .withMessages(new Message()
    .withReceiptHandle(messageHandle)
    .withMessageAttributes(ImmutableMap.<String, MessageAttributeValue>builder()
        .put(SqsAttribute.WORKFLOW_TYPE, new MessageAttributeValue()
        .withStringValue(type)
        .withDataType(SqsAttribute.STRING))
        .build())
        .withBody(serialized));

    @Mock
    private AmazonSQSAsync client;

    private SqsConfirmedSupplier supplier;

    @Before
    public void setup() {

        when(client.receiveMessage(any(ReceiveMessageRequest.class)))
        .thenReturn(result);

        supplier = new SqsConfirmedSupplier(client, queueUrl);
    }

    @Test
    public void supplier_with_message() {
        Optional<WorkflowRequest> supplied = supplier.get();

        assertThat(supplied.get(), equalTo(request));

        verify(client, never()).deleteMessageAsync(any(DeleteMessageRequest.class));

        supplier.confirm(supplied.get());

        verify(client).deleteMessageAsync(any(DeleteMessageRequest.class));
    }

    @Test
    public void supplier_without_messages() {
        when(client.receiveMessage(any(ReceiveMessageRequest.class)))
        .thenReturn(new ReceiveMessageResult());

        Optional<WorkflowRequest> supplied = supplier.get();

        assertThat(supplied, equalTo(Optional.empty()));
    }
}
