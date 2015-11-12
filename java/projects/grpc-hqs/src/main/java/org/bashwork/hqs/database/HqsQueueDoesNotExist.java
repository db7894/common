package org.bashwork.hqs.database;

/**
 * Indicates that the supplied queue does not exist.
 */
public class HqsQueueDoesNotExist extends Exception {
    public HqsQueueDoesNotExist(final String message) {
        super(message);
    }

    public HqsQueueDoesNotExist(final String message, final Throwable cause) {
        super(message, cause);
    }

    public HqsQueueDoesNotExist(final Throwable cause) {
        super(cause);
    }

    public HqsQueueDoesNotExist(final String message, final Throwable cause, final boolean enableSuppression,
        final boolean writableStackTrace) {
        super(message, cause, enableSuppression, writableStackTrace);
    }
}
