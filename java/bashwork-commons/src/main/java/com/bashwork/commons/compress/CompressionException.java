package com.bashwork.commons.compress;

/**
 * An exception thrown to indicate there was a problem with
 * compression/decompression.
 */
public class CompressionException extends RuntimeException {

	private static final long serialVersionUID = 8050951217236060244L;

	/**
     * Creates a new instance of CompressionException.
     */
    public CompressionException() {
        super();
    }

    /**
     * Creates a new instance of CompressionException.
     * 
     * @param message The exception message.
     */
    public CompressionException(String message) {
        super(message);
    }

    /**
     * Creates a new instance of CompressionException.
     * 
     * @param cause The root cause.
     */
    public CompressionException(Throwable cause) {
        super(cause);
    }

    /**
     * Creates a new instance of CompressionException.
     * 
     * @param message The exception message.
     * @param cause The root cause.
     */
    public CompressionException(String message, Throwable cause) {
        super(message, cause);
    }

}
