package com.bashwork.commons.filesystem;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertTrue;

import java.io.ByteArrayInputStream;
import java.io.InputStream;

import org.junit.Test;

/**
 * Code to validate the EmailNotifierService class.
 */
public class FileObjectTest {

    private static final FileType TYPE = FileType.IMAGE;
    private static final FileType NULL_TYPE = null;
    private static final InputStream DATA = new ByteArrayInputStream("payload".getBytes());
    private static final InputStream NULL_DATA = null;

    @Test
    public void create_valid_object() {
        FileObject object = FileObject.builder()
            .withData(DATA)
            .withType(TYPE)
            .build();

        assertEquals(object.toString(), object.toString());
        assertEquals(object.hashCode(), object.hashCode());
        assertEquals(TYPE, object.getType());
        assertTrue(object.equals(object));
        assertFalse(object.equals(null));
    }

    @Test(expected=IllegalArgumentException.class)
    public void create_with_invalid_data() {
        FileObject.builder()
        .withData(NULL_DATA)
        .withType(TYPE)
        .build();

    }

    @Test(expected=IllegalArgumentException.class)
    public void create_with_invalid_type() {
        FileObject.builder()
        .withData(DATA)
        .withType(NULL_TYPE)
        .build();
    }
}
