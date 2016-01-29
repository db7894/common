package com.bashwork.commons.filesystem;

import static com.bashwork.commons.utility.Validate.notNull;

import java.io.InputStream;

/**
 * An immutable wrapper for a file system object. It should be noted
 * that the underlying data is not used for hashing or equality as it
 * could very well be megabytes or gigabytes in size.
 */
public final class FileObject {

    private final FileType type;
    private final InputStream data;
    private final long size;

    private FileObject(Builder builder) {
        this.type = builder.type;
        this.data = builder.data;
        this.size = builder.size;
    }

    public FileType getType() {
        return type;
    }

    public InputStream getData() {
        return data;
    }

    public long getSize() {
        return size;
    }

    /**
     * Create a new builder to create a FileObject with.
     *
     * @return A new builder to operate with.
     */
    public static Builder builder() {
        return new Builder();
    }

    /**
     * A builder for the FileObject class.
     */
    public static final class Builder {

        private FileType type;
        private InputStream data;
        private long size = -1; // -1 is 'uknown'

        public Builder withType(FileType value) {
            this.type = value;
            return this;
        }

        public Builder withData(InputStream value) {
            this.data = value;
            return this;
        }

        public Builder withSize(long value) {
            this.size = value;
            return this;
        }

        public FileObject build() {
            notNull(this.type, "filetype");
            notNull(this.data, "filedata");
            notNull(this.size, "filesize");
            return new FileObject(this);
        }
    }
}
