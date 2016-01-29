package com.bashwork.commons.utility;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.IOException;
import java.io.InputStream;
import java.net.URI;
import java.net.URLConnection;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;
import java.util.Optional;

import static com.bashwork.commons.utility.Validate.notNull;

/**
 * A collection of common utilities for working with returned media.
 */
public final class DownloadUtility {
    private DownloadUtility() { }
    static final Logger logger = LogManager.getLogger(FileUtility.class);

    /**
     * A collection of options governing how the DownloadUtility
     * operates.
     */
    public enum Option {
        REPLACE_EXISTING,
        KEEP_EXISTING
    }

    /**
     * Download the supplied resources to the supplied destination. This
     * means given a url of "http://.../image.jpeg" and a destination of
     * "/a/b/c/another.jpeg", this will produce "/a/b/c/another.jpeg".
     *
     * @param source The URL of the resource to download.
     * @param destination The destination path to store the file to.
     * @return The size of the downloaded file.
     */
    private static Optional<Path> download(URI source, Path destination, Option option) {

        /**
         * If we are going to use the file that already exists,
         * short circuit here.
         */

        if (Option.KEEP_EXISTING.equals(option) && Files.exists(destination)) {
            return Optional.of(destination);
        }

        /**
         * Make sure a directory exists to store the file in or
         * the copy operation will fail. This also tests if we
         * have permissions to copy to the filesystem.
         */

        final Path parent = destination.getParent();
        if (!Files.isDirectory(parent)) {
            try {
                Files.createDirectories(parent);
            } catch (IOException ex) {
                logger.error("failed to create directory {}", parent, ex);
                return Optional.empty();
            }
        }

        /**
         * We need to set a few parameters on the HTTP connection so it
         * will work correctly with Cloudfront.
         */

        final URLConnection connection;

        try {
            connection = source.toURL().openConnection();
            connection.addRequestProperty("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0)");
        } catch (IOException ex) {
            logger.error("failed to open connection to {}", source, ex);
            return Optional.empty();
        }

        /**
         * Copy and overwrite anything that may be written to at
         * the moment.
         */

        try (InputStream stream = connection.getInputStream()) {
            Files.copy(stream, destination, StandardCopyOption.REPLACE_EXISTING);
            return Optional.of(destination);

        } catch (IOException ex) {
            logger.error("failed to download {} to {}", source, destination, ex);
            return Optional.empty();
        }
    }

    /**
     * Create a new instance of the DownloadUtility.Builder.
     *
     * @return The new builder instance.
     */
    public static Builder create() {
        return new Builder();
    }

    /**
     * A builder to abstract away the details of building a
     * download request.
     */
    public static final class Builder {
        private Builder() { }

        private Option option = Option.REPLACE_EXISTING;
        private URI source;
        private Path destination;

        public Builder withOption(Option option) {
            this.option = option;
            return this;
        }

        public Builder withSource(String source) {
            return withSource(URIEncoder.encode(source));
        }

        public Builder withSource(URI source) {
            this.source = source;
            return this;
        }

        public Builder withDestination(String destination) {
            return withDestination(Paths.get(destination));
        }

        public Builder withDestination(Path destination) {
            this.destination = destination;
            return this;
        }

        public Builder withDestinationDirectory(String directory) {
            return withDestinationDirectory(Paths.get(directory));
        }

        public Builder withDestinationDirectory(Path directory) {
            notNull(source, "Must supply a valid source first");
            final Path filename = Paths.get(source.getPath()).getFileName();
            this.destination = directory.resolve(filename);
            return this;
        }

        /**
         * Downloads the supplied file to the supplied destination with
         * the optional arguments.
         * @return The path of the downloaded file if it was downloaded.
         */
        public Optional<Path> download() {
            notNull(source, "Must supply a valid source");
            notNull(destination, "Must supply a valid destination");
            return DownloadUtility.download(source, destination, option);
        }
    }
}
