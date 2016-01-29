package com.bashwork.commons.notification;

import org.junit.Test;

/**
 * Code to validate the NotificationException class
 */
public class NotificationExceptionTest {

    @Test
    public void create_without_arguments() {
        new NotificationException();
    }

    @Test
    public void create_with_message() {
        new NotificationException("some exception");
    }

    @Test
    public void create_with_throwable() {
        new NotificationException(new Throwable());
    }

    @Test
    public void create_with_message_and_throwable() {
        new NotificationException("some exception", new Throwable());
    }
}
