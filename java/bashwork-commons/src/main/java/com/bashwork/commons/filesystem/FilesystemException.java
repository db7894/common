package com.bashwork.commons.filesystem;

/**
 * An exception thrown to indicate an error occurred in a
 * file-system operation.
 */
@SuppressWarnings("serial")
public class FilesystemException extends RuntimeException {

    /**
     * Creates a new instance of NotificationException.
     */
    public FilesystemException() {
        super();
    }

    /**
     * Creates a new instance of NotificationException.
     *
     * @param message The exception message.
     */
    public FilesystemException(String message) {
        super(message);
    }

    /**
     * Creates a new instance of NotificationException.
     *
     * @param cause The root cause.
     */
    public FilesystemException(Throwable cause) {
        super(cause);
    }

    /**
     * Creates a new instance of NotificationException.
     *
     * @param message The exception message.
     * @param cause The root cause.
     */
    public FilesystemException(String message, Throwable cause) {
        super(message, cause);
    }
}
