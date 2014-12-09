package com.bashwork.commons.worker;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.Matchers.is;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import static org.mockito.Matchers.any;

import java.util.Arrays;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.runners.MockitoJUnitRunner;

import com.bashwork.commons.worker.source.SnsEventSource;
import com.amazonaws.services.sqs.AmazonSQS;
import com.amazonaws.services.sqs.model.DeleteMessageRequest;
import com.amazonaws.services.sqs.model.Message;
import com.amazonaws.services.sqs.model.ReceiveMessageRequest;
import com.amazonaws.services.sqs.model.ReceiveMessageResult;
import com.google.common.base.Optional;


@RunWith(MockitoJUnitRunner.class)
public class SnsEventSourceTest {
    
    private static final String PAYLOAD = "hello world";
    private static final String WRAPPER = "{"
    + "   \"Type\" : \"Notification\","
    + "   \"MessageId\" : \"cb012975-b52f-517b-a132-65f24a1cc931\","
    + "   \"TopicArn\" : \"arn:aws:sns:us-east-1:258343978509:VBIChangeDetectionTmp\","
    + "   \"Message\" : \"\\\"" + PAYLOAD + "\\\"\","
    + "   \"Timestamp\" : \"2014-09-08T19:01:07.669Z\","
    + "   \"SignatureVersion\" : \"1\","
    + "   \"Signature\" : \"blah\","
    + "   \"SigningCertURL\" : \"https://sns.us-east-1.amazonaws.com/\","
    + "   \"UnsubscribeURL\" : \"https://sns.us-east-1.amazonaws.com/\""
    + "}";
    
    private static final String TAG = "message tag";
    private static final ReceiveMessageResult MESSAGE;
    static {
        Message message = new Message();
        message.setBody(WRAPPER);
        message.setMessageId(TAG);
        MESSAGE = new ReceiveMessageResult();
        MESSAGE.setMessages(Arrays.asList(message));
    }    
    private static final ReceiveMessageResult EMPTY = new ReceiveMessageResult();

    @Mock private AmazonSQS client;
    private SnsEventSource<String> source;
    
    @Before
    public void test_setup() {
        source = new SnsEventSource<String>(client, "queue", String.class);
    }
    
    @Test
    public void message_can_be_confirmed() {
        when(client.receiveMessage(any(ReceiveMessageRequest.class)))
            .thenReturn(MESSAGE);
        
        Optional<TaggedType<String>> message = source.get();
        source.confirm(message.get());
        
        verify(client).deleteMessage(any(DeleteMessageRequest.class));
    }    
    
    @Test
    public void works_without_any_message() {
        when(client.receiveMessage(any(ReceiveMessageRequest.class)))
            .thenReturn(EMPTY);
        
        Optional<TaggedType<String>> message = source.get();
        assertThat(message.isPresent(), is(false));
    }
    
    @Test
    public void works_with_one_message() {
        when(client.receiveMessage(any(ReceiveMessageRequest.class)))
            .thenReturn(MESSAGE);
        
        Optional<TaggedType<String>> message = source.get();
        assertThat(message.isPresent(), is(true));
        assertThat(message.get().getValue(), is(PAYLOAD));
    }
}
