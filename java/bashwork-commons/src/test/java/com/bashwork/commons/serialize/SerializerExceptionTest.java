package com.bashwork.commons.serialize;

import org.junit.Test;

public class SerializerExceptionTest {

    @Test
    public void test_all_exception_constructors() {
        final String message = "there was an error serializing";
        final Throwable throwable = new Throwable();

        new SerializerException();
        new SerializerException(message);
        new SerializerException(throwable);
        new SerializerException(message, throwable);
    }
}
