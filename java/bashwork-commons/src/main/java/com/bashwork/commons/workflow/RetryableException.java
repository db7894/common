package com.bashwork.commons.workflow;

/**
 * An exception that indicates something has happened in the
 * service that has caused a error to occur. However, this
 * error is transient and the user should try the previous
 * action again after waiting for a short while as the error
 * may be resolved.
 */
public class RetryableException extends Exception {

    private static final long serialVersionUID = 1L;

    /**
     * Creates a new instance of RetryableServiceException.
     *
     * @param message The exception message.
     */
    public RetryableException(String message) {
        super(message);
    }

    /**
     * Creates a new instance of RetryableServiceException.
     *
     * @param cause The root cause.
     */
    public RetryableException(Throwable cause) {
        super(cause);
    }

    /**
     * Creates a new instance of RetryableServiceException.
     *
     * @param message The exception message.
     * @param cause The root cause.
     */
    public RetryableException(String message, Throwable cause) {
        super(message, cause);
    }
}
