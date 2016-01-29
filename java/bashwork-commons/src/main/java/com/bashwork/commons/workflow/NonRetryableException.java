package com.bashwork.commons.workflow;

/**
 * An exception that indicates something has happened in the
 * service that has caused a error to occur. However, this
 * error is believed to be permanent and thus further retries
 * by the client would result in the same error occurring
 * again.
 */
public class NonRetryableException extends Exception {

    private static final long serialVersionUID = 1L;

    /**
     * Creates a new instance of NonRetryableServiceException.
     *
     * @param message The exception message.
     */
    public NonRetryableException(String message) {
        super(message);
    }

    /**
     * Creates a new instance of NonRetryableServiceException.
     *
     * @param cause The root cause.
     */
    public NonRetryableException(Throwable cause) {
        super(cause);
    }

    /**
     * Creates a new instance of NonRetryableServiceException.
     *
     * @param message The exception message.
     * @param cause The root cause.
     */
    public NonRetryableException(String message, Throwable cause) {
        super(message, cause);
    }
}
