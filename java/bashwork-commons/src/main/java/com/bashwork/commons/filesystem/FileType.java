package com.bashwork.commons.filesystem;

import static com.bashwork.commons.utility.Validate.notNull;

import java.util.HashMap;
import java.util.Map;

/**
 * Represents the file-type of a given file object.
 */
public enum FileType {

    ZIP("application/zip", "zip"),
    JSON("application/json", "json"),
    IMAGE("image/jpeg", "jpeg"),
    VIDEO("video/mp4", "mp4");

    private final String mimeType;
    private final String extension;

    public String getMimeType() {
        return mimeType;
    }

    public String getExtension() {
        return extension;
    }

    FileType(String mimeType, String extension) {
        this.mimeType = mimeType;
        this.extension = extension;
    }

    /**
     * Retrieve a FileType from the given MIME type.
     *
     * @param mimeType The MIME type string for the FileType
     * @return The matching FileType if it exists.
     * @throws IllegalArgumentException if the given name or resulting FileType are null
     */
    public static FileType getByMimeType(String mimeType) {
        notNull(mimeType, "name");
        return notNull(NAME_TO_TYPE.get(mimeType), "FileType for name: " + mimeType);
    }

    /**
     * Retrieve a FileType from the given extension string.
     *
     * @param extension The extension string for the FileType
     * @return The matching FileType if it exists.
     * @throws IllegalArgumentException if the given extension or resulting FileType are null
     */
    public static FileType getByExtension(String extension) {
        notNull(extension, "extension");
        return notNull(EXTENSION_TO_TYPE.get(extension), "FileType for ext: " + extension);
    }

    /**
     * Pre-computed map of mime-type to type enumeration.
     */
    private static final Map<String, FileType> NAME_TO_TYPE;
    static {
        NAME_TO_TYPE = new HashMap<String, FileType>();
        for (FileType type : FileType.values()) {
            NAME_TO_TYPE.put(type.mimeType, type);
        }
    }

    /**
     * Pre-computed map of type-extension to type enumeration.
     */
    private static final Map<String, FileType> EXTENSION_TO_TYPE;
    static {
        EXTENSION_TO_TYPE = new HashMap<String, FileType>();
        for (FileType type : FileType.values()) {
            EXTENSION_TO_TYPE.put(type.extension, type);
        }
    }
}
