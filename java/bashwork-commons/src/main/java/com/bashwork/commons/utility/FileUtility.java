package com.bashwork.commons.utility;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

/**
 * A collection of file utilities that are reused throughout
 * the AVM stack.
 */
public final class FileUtility {
    private FileUtility() { }
    static final Logger logger = LogManager.getLogger(FileUtility.class);


    /**
     * Recursively delete the supplied file and all its children.
     *
     * @param path The path to delete recursively.
     */
    public static void delete(String path) {
        delete(Paths.get(path));
    }

    /**
     * Recursively delete the supplied file and all its children.
     *
     * @param path The path to delete recursively.
     */
    public static void delete(File path) {
        delete(path.getPath());
    }

    /**
     * Recursively delete the supplied file and all its children.
     *
     * @param path The path to delete recursively.
     */
    public static void delete(Path path) {
        try {
            if (Files.isDirectory(path)) {
                Files.list(path).forEach(FileUtility::delete);
            }
            Files.deleteIfExists(path);
        } catch (IOException ex) {
            // we don't care if this fails or not, just keep trying
            logger.debug("failed to delete the file {}", path, ex);
        }
    }
}
