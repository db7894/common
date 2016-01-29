package com.bashwork.commons.utility;

import static org.junit.Assert.assertThat;
import static org.hamcrest.Matchers.equalTo;

import java.io.IOException;
import java.net.URI;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Optional;

import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;

public class DownloadUtilityTest {

    private static final Path SOURCE_PATH = getTemporaryFile();
    private static final URI URL = SOURCE_PATH.toUri();
    private static final String URL_STRING = URL.toString();
    private static final String BAD_URL = "http://localhost/invalid";

    private static final String DESTINATION = "/tmp/testing/test-file.jpeg";
    private static final Path DESTINATION_PATH = Paths.get(DESTINATION);
    private static final Path DESTINATION_COPY = DESTINATION_PATH.getParent().resolve(SOURCE_PATH.getFileName());
    private static final Optional<Path> DESTINATION_COPY_EXISTS = Optional.of(DESTINATION_COPY);
    private static final Optional<Path> DESTINATION_EXISTS = Optional.of(DESTINATION_PATH);
    private static final Optional<Path> DESTINATION_MISSING = Optional.empty();
    private static final String BAD_DESTINATION = "/nowhere/testing/test-file.jpeg";

    @After
    public void test_cleanup() throws IOException {
        Files.deleteIfExists(DESTINATION_COPY);
        Files.deleteIfExists(DESTINATION_PATH);
    }

    @AfterClass
    public static void suite_cleanup() throws IOException {
        Files.deleteIfExists(SOURCE_PATH);
    }

    // TODO test with a space in the URL

    @Test
    public void test_downloading_to_file() {
        Optional<Path> path = DownloadUtility.create()
            .withSource(URL_STRING)
            .withDestination(DESTINATION)
            .download();

        assertThat(path, equalTo(DESTINATION_EXISTS));
    }

    @Test
    public void test_downloading_in_directory() {
        final Path destination = DESTINATION_PATH.getParent();

        Optional<Path> path = DownloadUtility.create()
            .withSource(URL)
            .withDestinationDirectory(destination)
            .download();

        assertThat(path, equalTo(DESTINATION_COPY_EXISTS));
    }

    @Test
    public void test_downloading_in_directory_strings() {
        final String source = URL.toString();
        final String destination = DESTINATION_PATH.getParent().toString();

        Optional<Path> path = DownloadUtility.create()
            .withSource(source)
            .withDestinationDirectory(destination)
            .download();

        assertThat(path, equalTo(DESTINATION_COPY_EXISTS));
    }
    
    @Test
    public void test_downloading_existing_file() {
        Optional<Path> path1 = DownloadUtility.create()
            .withSource(URL_STRING)
            .withDestination(DESTINATION)
            .download();

        Optional<Path> path2 = DownloadUtility.create()
            .withSource(URL_STRING)
            .withDestination(DESTINATION)
            .download();

        assertThat(path1, equalTo(DESTINATION_EXISTS));
        assertThat(path2, equalTo(DESTINATION_EXISTS));
    }    

    @Test
    public void test_download_missing_file() {
        Optional<Path> path =DownloadUtility.create()
            .withSource(BAD_URL)
            .withDestination(DESTINATION)
            .download();

        assertThat(path, equalTo(DESTINATION_MISSING));
    }
    
    @Test
    public void test_download_to_invalid_path() {
        Optional<Path> path =DownloadUtility.create()
            .withSource(URL_STRING)
            .withDestination(BAD_DESTINATION)
            .download();

        assertThat(path, equalTo(DESTINATION_MISSING));
    }

    /**
     * Helper method to create a temporary file to "download"
     * using the download utility.
     *
     * @return The file to download.
     */
    private static Path getTemporaryFile() {
        try {
            Path path = Files.createTempFile("test-file", ".jpeg");
            Files.write(path, "hello world".getBytes("UTF-8"));
            return path;
        } catch (IOException ex) {
            return Paths.get("/sys/kernel/notes");
        }
    }
}
