package com.bashwork.commons.serialize;

/**
 * Exception thrown when a serialization error occurs.
 */
public class SerializerException extends RuntimeException {

    public SerializerException() {
        super();
    }

    public SerializerException(String arg0, Throwable arg1) {
        super(arg0, arg1);
    }

    public SerializerException(String arg0) {
        super(arg0);
    }

    public SerializerException(Throwable arg0) {
        super(arg0);
    }
}
