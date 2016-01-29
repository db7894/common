package com.bashwork.commons.testing.utility;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Optional;

/**
 * A collection of utilities for dealing with the filesystem
 * for testing.
 */
public final class TestFilesystem {
    private TestFilesystem() { }

    /**
     * Helper method to create a temporary file to "download"
     * using the download utility.
     *
     * @param content The content of the temporary file.
     * @return The file to download.
     */
    public static Optional<Path> getTempFile(String content) {
        try {
            Path path = Files.createTempFile("test-file", ".jpeg");
            Files.write(path, content.getBytes("UTF-8"));
            return Optional.of(path);
        } catch (IOException ex) {
            return Optional.empty();
        }
    }
}
