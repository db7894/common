package com.bashwork.commons.filesystem;

import java.util.Optional;

/**
 * An abstraction to a simple file-system with basic
 * jailed operations to prevent freely interacting with
 * the larger system.
 */
public interface Filesystem {

    /**
     * Check if the given file-system object exists
     *
     * @param path The path to the file-system object
     * @param type The type of the file-system object
     * @return true if it exists, false otherwise
     */
    boolean exists(String path, FileType type);

    /**
     * Retrieve the file-system object at the given path
     *
     * @param path The path to the file-system object
     * @param type The type of the file-system object
     * @return An optional of the file-system object
     */
    Optional<FileObject> get(String path, FileType type);

    /**
     * Store the file-system object at the given path. This
     * will store in a last write wins so any existing data
     * will simply be overwritten.
     *
     * @param path The path to the file-system object
     * @param type The type of the file-system object
     * @param object The object to store
     */
    void put(String path, FileType type, FileObject object);

    /**
     * Delete the supplied file if it exists.
     *
     * @param path The path to the file to delete.
     * @param type The type of file to delete.
     */
    void delete(String path, FileType type);
}
