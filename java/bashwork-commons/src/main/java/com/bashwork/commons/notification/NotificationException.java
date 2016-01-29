package com.bashwork.commons.notification;

/**
 * An exception thrown to indicate an error occurred in a notification
 * action where attempts to retry will always result in further failures.
 */
public class NotificationException extends RuntimeException {

    /**
     * Creates a new instance of NotificationException.
     */
    public NotificationException() {
        super();
    }

    /**
     * Creates a new instance of NotificationException.
     *
     * @param message The exception message.
     */
    public NotificationException(String message) {
        super(message);
    }

    /**
     * Creates a new instance of NotificationException.
     *
     * @param cause The root cause.
     */
    public NotificationException(Throwable cause) {
        super(cause);
    }

    /**
     * Creates a new instance of NotificationException.
     *
     * @param message The exception message.
     * @param cause The root cause.
     */
    public NotificationException(String message, Throwable cause) {
        super(message, cause);
    }
}
