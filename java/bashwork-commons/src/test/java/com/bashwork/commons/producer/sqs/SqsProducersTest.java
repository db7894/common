package com.bashwork.commons.producer.sqs;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;
import static org.mockito.Matchers.any;
import static org.mockito.Mockito.when;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.runners.MockitoJUnitRunner;

import com.bashwork.commons.serializer.Serializer;
import com.bashwork.commons.serializer.TestPojo;
import com.bashwork.commons.serializer.string.JsonStringSerializer;
import com.amazonaws.services.sqs.AmazonSQSAsync;
import com.amazonaws.services.sqs.model.SendMessageRequest;
import com.amazonaws.services.sqs.model.SendMessageResult;

import java.time.Duration;

@RunWith(MockitoJUnitRunner.class)
public class SqsProducersTest {

    private final String queueUrl = "producer-message-queue";
    private final String messageId = "messageID";
    private final TestPojo message = new TestPojo("Mark", 22);

    @Mock private AmazonSQSAsync client;
    private final Serializer<String> serializer = new JsonStringSerializer();

    private SqsProducer producer;

    @Before
    public void setup() {

        when(client.sendMessage(any(SendMessageRequest.class)))
            .thenReturn(new SendMessageResult()
                .withMessageId(messageId));

        producer = new SqsProducer(client, queueUrl, serializer);
    }

    @Test
    public void produce_new_message() {
        String tracker = producer.produce(message);

        assertThat(tracker, equalTo(messageId));
    }

    @Test
    public void produce_new_message_with_delay() {
        String tracker = producer.produce(message, Duration.ofDays(1));

        assertThat(tracker, equalTo(messageId));
    }
}
