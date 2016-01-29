package com.bashwork.commons.filesystem;

import static org.junit.Assert.assertEquals;

import org.junit.Test;

/**
 * Code to validate the EmailNotifierService class.
 */
public class FileTypeTest {

    @Test
    public void lookup_mime_type() {
        for (FileType type : FileType.values()) {
            assertEquals(FileType.getByMimeType(type.getMimeType()), type);
        }
    }

    @Test(expected=IllegalArgumentException.class)
    public void lookup_mime_type_null() {
        FileType.getByMimeType(null);
    }

    @Test(expected=IllegalArgumentException.class)
    public void lookup_mime_type_invalid() {
        FileType.getByMimeType("bad name");
    }

    @Test
    public void lookup_extension() {
        for (FileType type : FileType.values()) {
            assertEquals(FileType.getByExtension(type.getExtension()), type);
        }
    }

    @Test(expected=IllegalArgumentException.class)
    public void lookup_extension_null() {
        FileType.getByExtension(null);
    }

    @Test(expected=IllegalArgumentException.class)
    public void lookup_extension_invalid() {
        FileType.getByExtension("bad extension");
    }
}
