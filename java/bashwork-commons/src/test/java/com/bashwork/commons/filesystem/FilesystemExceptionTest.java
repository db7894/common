package com.bashwork.commons.filesystem;

import org.junit.Test;

/**
 * Code to validate the FilesystemException class
 */
public class FilesystemExceptionTest {

    @Test
    public void create_without_arguments() {
        new FilesystemException();
    }

    @Test
    public void create_with_message() {
        new FilesystemException("some exception");
    }

    @Test
    public void create_with_throwable() {
        new FilesystemException(new Throwable());
    }

    @Test
    public void create_with_message_and_throwable() {
        new FilesystemException("some exception", new Throwable());
    }
}
