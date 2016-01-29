package com.bashwork.commons.filesystem;

import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;

/**
 * A class encapsulating a set of time-based paths.
 *
 * @author mbarrows
 */
public final class PathBuilder {
    private PathBuilder() { }

    public enum Type {

        IMAGE("image", FileType.IMAGE),
        VIDEO("video", FileType.VIDEO);

        private final String pathKey;
        private final FileType fileType;

        public String getPathKey() {
            return pathKey;
        }

        public FileType getFileType() {
            return fileType;
        }

        private Type(String pathKey, FileType fileType) {
            this.pathKey = pathKey;
            this.fileType = fileType;
        }
    }

    /*
     * Note: We changed this from "YYYY/MM/dd/HH/'{identifier}_{timestamp}'" because
     *       we want to be able to find the latest image for a given identifier, and
     *       so we filter by prefix and sort the remainder
     */
    private static final String PATH_PATTERN = "'{type}/{warehouse}/{identifier}'/YYYY/MM/dd/'{timestamp}'";
    private static final DateTimeFormatter DATE_FORMATTER = DateTimeFormat.forPattern(PATH_PATTERN);

    private static final String TYPE = "{type}";
    private static final String WAREHOUSE = "{warehouse}";
    /** Camera identifier */
    private static final String IDENTIFIER = "{identifier}";
    private static final String TIMESTAMP = "{timestamp}";

    /**
     * Longest path this class will possibly produce.
     *
     * Dictated by our most restrictive file-system, S3.
     * "The name for a key is a sequence of Unicode characters whose UTF-8 encoding is at most 1024
     * bytes long." http://docs.aws.amazon.com/AmazonS3/latest/dev/UsingMetadata.html
     *
     * We reduce that by 24 as a buffer for other file-system-specific additions.
     */
    private static final int LONGEST_S3_PATH = 1024;
    private static final int LONGEST_METADATA_SUFFIX = 24;
    private static final int LONGEST_PATH = LONGEST_S3_PATH - LONGEST_METADATA_SUFFIX;

    public static String forImage(String warehouse, String identifier, long timestamp) {
        String result = getBasePath(Type.IMAGE, warehouse, identifier, timestamp);
        result = result.replace(TIMESTAMP, timeToString(timestamp));
        return validatePathLength(result);
    }

    public static String forVideo(String warehouse, String identifier, long start, long end) {
        String result = getBasePath(Type.VIDEO, warehouse, identifier, start);
        String timestamp = timeToString(start) + "-" + timeToString(end);
        result = result.replace(TIMESTAMP, timestamp);
        return validatePathLength(result);
    }

    private static String getBasePath(Type type, String warehouse,  String identifier, long timestamp) {
        DateTime dateTime = new DateTime(timestamp, DateTimeZone.UTC);
        String result = DATE_FORMATTER.print(dateTime);
        result = result.replace(TYPE, type.getPathKey());
        result = result.replace(WAREHOUSE, warehouse);
        result = result.replace(IDENTIFIER, identifier);
        result = result + "." + type.getFileType().getExtension();
        return result;
    }

    protected static String timeToString(long timestamp) {
        return String.format("%019d", timestamp);
    }

    protected static String validatePathLength(String path) {
        if (path.length() > LONGEST_PATH) {
            throw new IllegalArgumentException("Resulting path is longer than maxiumum length: "
                + path);
        }
        return path;
    }
}
