package com.bashwork.commons.compress;

import static org.junit.Assert.*;

import org.junit.Test;

public class CompressionExceptionTest {

    // *************************************************************************
    // Constants
    // *************************************************************************
    
    private static final String MESSAGE = "Sample message.";
    private static final Exception CAUSE = new Exception();

    // *************************************************************************
    // Test methods
    // *************************************************************************
    
    @Test
    public void testConstructor() {
        new CompressionException();
    }

    @Test
    public void testConstructor_Message() {
        CompressionException exception = new CompressionException(MESSAGE);
        assertNotNull(exception.getMessage());
        assertNull(exception.getCause());
    }

    @Test
    public void testConstructor_Cause() {
        CompressionException exception = new CompressionException(CAUSE);
        assertNotNull(exception.getCause());
        assertNotNull(exception.getMessage());
    }

    @Test
    public void testConstructor_MessageCause() {
        CompressionException exception = new CompressionException(MESSAGE, CAUSE);
        assertNotNull(exception.getMessage());
        assertNotNull(exception.getCause());
    }
    
}
