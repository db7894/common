package com.bashwork.commons.conversion;

import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertNull;
import static org.junit.Assert.assertTrue;

import org.junit.Test;

import com.bashwork.commons.serialize.SerializerException;

public class DataConverterExceptionTest {
    
    // *************************************************************************
    // Constants
    // *************************************************************************
    
    private static final String KEY = "foo";
    private static final String MESSAGE = "Sample message.";
    private static final Exception CAUSE = new Exception();

    // *************************************************************************
    // Test methods
    // *************************************************************************
    
    @Test
    public void testConstructor() {
        new SerializerException();
    }

    @Test
    public void testConstructor_Message() {
        SerializerException exception = new SerializerException(MESSAGE);
        assertNotNull(exception.getMessage());
        assertNull(exception.getCause());
    }

    @Test
    public void testConstructor_Cause() {
        SerializerException exception = new SerializerException(CAUSE);
        assertNotNull(exception.getCause());
        assertNotNull(exception.getMessage());
    }

    @Test
    public void testConstructor_MessageCause() {
        SerializerException exception = new SerializerException(MESSAGE, CAUSE);
        assertNotNull(exception.getMessage());
        assertNotNull(exception.getCause());
    }
    
    @Test
    public void testDataConverterException() {
        SerializerException exception = new SerializerException(MESSAGE, CAUSE);
        exception.setKey(KEY);
        assertTrue(exception.getMessage().contains(KEY));
    }
    
}
