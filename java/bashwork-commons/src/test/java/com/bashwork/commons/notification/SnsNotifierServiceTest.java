package com.bashwork.commons.notification;

import static org.junit.Assert.assertEquals;
import static org.mockito.Matchers.any;
import static org.mockito.Mockito.when;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.runners.MockitoJUnitRunner;

import com.bashwork.commons.metrics.MetricFactory;
import com.bashwork.commons.metrics.NullMetricFactory;
import com.amazonaws.services.sns.AmazonSNS;
import com.amazonaws.services.sns.model.PublishRequest;
import com.amazonaws.services.sns.model.PublishResult;

/**
 * Code to validate the SnsNotifierService class.
 */
@RunWith(MockitoJUnitRunner.class)
public class SnsNotifierServiceTest {

    private static final MetricFactory METRICS = new NullMetricFactory();
    private static final String IDENTIFIER = "63a3f6b6-d533-4a47-aef9-fcf5cf758c76";
    private static final String MESSAGE = "s3://bucket/filename.png";
    private static final String NULL_MESSAGE = null;
    private static final String ENDPOINT = "arn:aws:sns:us-east-1:123456789012:MyTopic";
    private static final String INVALID_ENDPOINT = "topic@address.com";

    private static final PublishResult PUBLISH_RESULT;
    static {
        PUBLISH_RESULT = new PublishResult();
        PUBLISH_RESULT.setMessageId(IDENTIFIER);
    }

    @Mock private AmazonSNS client;
    private SnsNotifierService service;

    @Before
    public void setup() {
        service = new SnsNotifierService(client, METRICS);
    }

    @Test(expected=IllegalArgumentException.class)
    public void create_with_bad_client() {
        AmazonSNS client = null;
        new SnsNotifierService(client, METRICS);
    }

    @Test(expected=IllegalArgumentException.class)
    public void create_with_bad_metrics() {
        MetricFactory metrics = null;
        new SnsNotifierService(client, metrics);
    }

    @Test(expected=NotificationException.class)
    public void notify_with_bad_endpoint() {
        service.notify(INVALID_ENDPOINT, MESSAGE);
    }

    @Test(expected=IllegalArgumentException.class)
    public void notify_with_bad_message() {
        service.notify(ENDPOINT, NULL_MESSAGE);
    }

    @Test(expected=NotificationException.class)
    public void notify_with_bad_client() {
        when(client.publish(any(PublishRequest.class)))
        .thenThrow(new NotificationException("boom!"));

        service.notify(ENDPOINT, MESSAGE);
    }

    @Test
    public void notify_with_good_client() {
        when(client.publish(any(PublishRequest.class)))
        .thenReturn(PUBLISH_RESULT);

        String tracker = service.notify(ENDPOINT, MESSAGE);
        assertEquals(tracker, IDENTIFIER);
    }
}
