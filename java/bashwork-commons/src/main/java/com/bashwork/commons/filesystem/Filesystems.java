package com.bashwork.commons.filesystem;

import static com.bashwork.commons.utility.Validate.notNull;

import java.util.Collection;
import java.util.Optional;

/**
 * A collection of higher-order filesystem abstractions.
 */
public final class Filesystems {
    private Filesystems() { } // block instantiation

    /**
     * Build a filesystem that will return the first successful
     * response from a collection of supplied filesystems.
     *
     * @param filesystems The filesystems to operate with.
     * @return The composite filesystem wrapper.
     */
    public Filesystem oneOf(Collection<Filesystem> filesystems) {
        return new BaseFilesystem(filesystems) {

            @Override
            public void put(String path, FileType type, FileObject object) {
                for (Filesystem filesystem : filesystems) {
                    try {
                        filesystem.put(path, type, object);
                        return;
                    } catch (FilesystemException ex) {
                        // if the save was invalid, then we will simply
                        // ignore it as the next might succeed.
                    }
                }

            }
        };
    }

    /**
     * Build a filesystem that will write to all the supplied
     * filesystems and return the first successful response from
     * a filesystem read.
     *
     * @param filesystems The filesystems to operate with.
     * @return The composite filesystem wrapper.
     */
    public Filesystem allOf(Collection<Filesystem> filesystems) {
        return new BaseFilesystem(filesystems) {

            @Override
            public void put(String path, FileType type, FileObject object) {
                for (Filesystem filesystem : filesystems) {
                    try {
                        filesystem.put(path, type, object);
                    } catch (FilesystemException ex) {
                        // if the save was invalid, then we will simply
                        // ignore it as the next might succeed.
                    }
                }

            }
        };
    }

    /**
     * A base composite filesystem that can be used to create
     * higher order filesystems.
     */
    private abstract class BaseFilesystem implements Filesystem {

        protected final Collection<Filesystem> filesystems;

        /**
         * Initialize a new instance of the BaseFilesystem
         *
         * @param filesystems The filesystems to operate with.
         */
        public BaseFilesystem(Collection<Filesystem> filesystems) {
            this.filesystems = notNull(filesystems, "Must supply a valid collection of filesystems");
        }

        /**
         * @see BaseFilesystem#exists
         */
        @Override
        public boolean exists(String path, FileType type) {
            return filesystems.stream().anyMatch(filesystem ->
            filesystem.exists(path, type));
        }

        /**
         * @see BaseFilesystem#get
         */
        @Override
        public Optional<FileObject> get(String path, FileType type) {
            for (Filesystem filesystem : filesystems) {
                Optional<FileObject> object = filesystem.get(path, type);
                if (object.isPresent()) {
                    return object;
                }
            }
            return Optional.empty();
        }

        /**
         * @see Filesystem#delete(String, FileType)
         */
        @Override
        public void delete(String path, FileType type) {
            for (Filesystem filesystem : filesystems) {
                filesystem.delete(path, type);
            }
        }
    }
}
