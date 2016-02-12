package com.bashwork.commons.supplier.sqs;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;
import static org.mockito.Matchers.any;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.runners.MockitoJUnitRunner;

import com.bashwork.commons.serialize.Serializer;
import com.bashwork.commons.serialize.TestPojo;
import com.bashwork.commons.serialize.string.JsonStringSerializer;
import com.amazonaws.services.sqs.AmazonSQSAsync;
import com.amazonaws.services.sqs.model.DeleteMessageRequest;
import com.amazonaws.services.sqs.model.Message;
import com.amazonaws.services.sqs.model.ReceiveMessageRequest;
import com.amazonaws.services.sqs.model.ReceiveMessageResult;

@RunWith(MockitoJUnitRunner.class)
public class SqsSupplierTest {

    private final Serializer<String> serializer = new JsonStringSerializer();
    private final TestPojo message = new TestPojo("Mark", 22);
    private final Class<TestPojo> klass = TestPojo.class;

    private final String serialized = serializer.serialize(message);
    private final String queueUrl = "producer-message-queue";
    private final String messageHandle = "message-handle";

    @Mock
    private AmazonSQSAsync client;

    private SqsSupplier<TestPojo> supplier;

    @Before
    public void setup() {

        when(client.receiveMessage(any(ReceiveMessageRequest.class)))
        .thenReturn(new ReceiveMessageResult()
        .withMessages(new Message()
        .withReceiptHandle(messageHandle)
        .withBody(serialized)));

        supplier = new SqsSupplier<TestPojo>(client, queueUrl, serializer, klass);
    }

    @Test
    public void supplier_with_messages() {
        TestPojo supplied = supplier.get();

        assertThat(supplied, equalTo(message));

        verify(client).deleteMessageAsync(any(DeleteMessageRequest.class));
    }

    @Test
    public void supplier_without_messages() {
        when(client.receiveMessage(any(ReceiveMessageRequest.class)))
        .thenReturn(new ReceiveMessageResult());

        TestPojo supplied = supplier.get();

        assertThat(supplied, equalTo(null));

        verify(client, never()).deleteMessageAsync(any(DeleteMessageRequest.class));
    }
}
