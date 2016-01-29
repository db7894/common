package com.bashwork.commons.notification;

import static org.junit.Assert.assertEquals;

import org.junit.Test;

/**
 * Code to validate the NullNotifierService class.
 */
public class NullNotifierServiceTest {

    private static final String IDENTIFIER = "noop:notifition:identifier";
    private static final String MESSAGE = "s3://bucket/filename.png";
    private static final String ENDPOINT = "arn:aws:sns:us-east-1:123456789012:MyTopic";
    private NullNotifierService service = new NullNotifierService();

    @Test
    public void notify_with_any_endpoint() {
        String tracker = service.notify(ENDPOINT, MESSAGE);
        assertEquals(tracker, IDENTIFIER);
    }
}
