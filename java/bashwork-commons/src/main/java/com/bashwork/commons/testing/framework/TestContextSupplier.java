package com.bashwork.commons.testing.framework;

import com.amazonaws.services.s3.AmazonS3;
import com.amazonaws.services.s3.model.ObjectListing;
import com.amazonaws.services.s3.model.S3Object;
import com.amazonaws.services.s3.model.S3ObjectSummary;
import org.apache.logging.log4j.LogManager;

import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.function.Supplier;
import java.util.stream.Collectors;

import static com.bashwork.commons.utility.Validate.notBlank;
import static com.bashwork.commons.utility.Validate.notNull;

/**
 * A supplier of test context value from the backing data
 * store.
 */
public final class TestContextSupplier implements Supplier<List<TestContext>> {

    static final org.apache.logging.log4j.Logger logger = LogManager.getLogger(TestContextSupplier.class);

    private final AmazonS3 s3Client;
    private final String s3Bucket;
    private final String s3Prefix;
    private final Path directory;

    private TestContextSupplier(Builder builder) {
        this.s3Client = builder.s3Client;
        this.s3Bucket = builder.s3Bucket;
        this.s3Prefix = builder.s3Prefix;
        this.directory = builder.directory;
    }

    /**
     * @see Supplier#get()
     */
    @Override
    public List<TestContext> get() {
        Set<Path> downloaded = getDownloadedTests();
        Set<String> available = getAvailableTests();

        available.removeAll(downloaded.stream()
            .map(Path::toString)
            .collect(Collectors.toSet()));

        downloaded.addAll(download(available));

        return adapt(downloaded);
    }

    /**
     * Given a collection of file paths, convert them to a collection
     * of TestContext instances by using the conventional configuration.
     *
     * TODO in the future replace this with the S3 metadata stored in another file.
     *
     * @param paths The paths to adapt.
     * @return The adapted paths.
     */
    private List<TestContext> adapt(Set<Path> paths) {
        return paths.stream()
            .map(path -> TestContext.builder()
                .withPath(path)
                .withDescription(path.toString())
                .withSuccess(path.startsWith("pass"))
                .build())
            .collect(Collectors.toList());
    }

    /**
     * Given a collection of S3 object keys, download the files
     * associated with those keys to the local test directory.
     *
     * @param keys The S3 keys to download to disk.
     * @return The paths to the downloaded files.
     */
    private Set<Path> download(Set<String> keys) {
        Set<Path> paths = new HashSet<>();

        for (String key : keys) {
            S3Object object = s3Client.getObject(s3Bucket, key);

            try (InputStream stream = object.getObjectContent()) {
                Path path = directory.resolve(key);
                Files.copy(stream, path);
                paths.add(path);
            } catch (IOException ex) {
                logger.error("problem downloading test context", ex);
            }
        }

        return paths;
    }

    /**
     * Retrieve the current set of previously downloaded test files.
     *
     * @return The current set of downloaded test files.
     */
    private Set<Path> getDownloadedTests() {
        setupTestDirectory();

        try {
            return Files.list(directory)
                .collect(Collectors.toSet());
        } catch (IOException ex) {
            return Collections.emptySet();
        }
    }

    /**
     * Retrieve the current set of available tests stored in the
     * remote test store database.
     *
     * @return The current set of all available tests.
     */
    private Set<String> getAvailableTests() {
        ObjectListing listing = s3Client.listObjects(s3Bucket, s3Prefix);
        Set<String> results = listing.getObjectSummaries()
            .stream()
            .map(S3ObjectSummary::getKey)
            .collect(Collectors.toSet());

        do {
            listing = s3Client.listNextBatchOfObjects(listing);
            listing.getObjectSummaries()
                .stream()
                .map(S3ObjectSummary::getKey)
                .forEach(results::add);
        } while (listing.isTruncated());

        return results;
    }

    /**
     * Perform an first time setup on the local test directory
     * used to store the test files.
     */
    private void setupTestDirectory() {
        if (!Files.isDirectory(directory)) {
            try {
                Files.createDirectory(directory);
            } catch (IOException ex) {
                logger.error("problem setting up test context directory", ex);
                throw new RuntimeException(ex);
            }
        }
    }

    /**
     * Create a new builder instance for the TestContextSupplier.
     *
     * @return The initialized builder object.
     */
    public static Builder builder() {
        return new Builder();
    }

    /**
     * The builder object used to create an instance of a
     * TestContextSupplier.
     */
    public static final class Builder {
        private Builder() { }

        private AmazonS3 s3Client;
        private String s3Bucket;
        private Path directory;
        private String s3Prefix;

        public Builder withS3Client(final AmazonS3 s3Client) {
            this.s3Client = s3Client;
            return this;
        }

        public Builder withS3Bucket(final String s3Bucket) {
            this.s3Bucket = s3Bucket;
            return this;
        }

        public Builder withS3Prefix(final String s3Prefix) {
            this.s3Prefix = s3Prefix;
            return this;
        }

        public Builder withDirectoryt(final Path directory) {
            this.directory = directory;
            return this;
        }

        public TestContextSupplier createTestContextSupplier() {
            notNull(s3Client, "S3Client");
            notBlank(s3Bucket, "Bucket");
            notBlank(s3Prefix, "Bucket");
            notNull(directory, "directory");
            return new TestContextSupplier(this);
        }
    }
}
