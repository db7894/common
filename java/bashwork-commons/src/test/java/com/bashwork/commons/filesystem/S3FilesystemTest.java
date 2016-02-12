package com.bashwork.commons.filesystem;

import static org.junit.Assert.assertArrayEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertTrue;
import static org.mockito.Matchers.any;
import static org.mockito.Mockito.when;
import static org.mockito.Mockito.doThrow;

import java.io.ByteArrayInputStream;
import java.io.InputStream;
import java.util.Optional;

import com.amazonaws.services.s3.model.*;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.runners.MockitoJUnitRunner;

import com.amazonaws.AmazonServiceException;
import com.amazonaws.services.s3.AmazonS3;
import com.google.common.io.ByteStreams;

/**
 * Code to validate the S3Filesystem class.
 */
@RunWith(MockitoJUnitRunner.class)
public class S3FilesystemTest {

    private static final String BUCKET = "ImageBucket";
    private static final String PATH = "/folder/picture.jpeg";
    private static final FileType TYPE = FileType.IMAGE;
    private static final byte[] DATA_BYTES = "payload".getBytes();

    private static final ObjectMetadata METADATA;
    static {
        METADATA = new ObjectMetadata();
        METADATA.setContentType(TYPE.getMimeType());
    }

    private static final PutObjectResult PUT_RESULT;
    static {
        PUT_RESULT = new PutObjectResult();
        PUT_RESULT.setVersionId("1");
    }

    @Mock private AmazonS3 client;
    private S3Filesystem filesystem;

    // ------------------------------------------------------------------------
    // test methods
    // ------------------------------------------------------------------------

    private InputStream data;
    private FileObject object;

    @Before
    public void setup() {
        filesystem = new S3Filesystem(client, BUCKET);

        data = new ByteArrayInputStream(DATA_BYTES);

        object = FileObject.builder().withData(data).withType(TYPE).build();
    }

    @Test(expected=IllegalArgumentException.class)
    public void create_with_bad_client() {
        AmazonS3 client = null;
        new S3Filesystem(client, BUCKET);
    }

    @Test(expected=IllegalArgumentException.class)
    public void create_with_bad_bucket() {
        String bucket = null;
        new S3Filesystem(client, bucket);
    }

    @Test
    public void check_that_file_exists() {
        when(client.getObjectMetadata(any(GetObjectMetadataRequest.class)))
            .thenReturn(METADATA);

        assertTrue(filesystem.exists(PATH, TYPE));
    }

    @Test
    public void check_that_file_does_not_exist() {
        when(client.getObjectMetadata(any(GetObjectMetadataRequest.class)))
            .thenThrow(new AmazonServiceException("boom"));

        assertFalse(filesystem.exists(PATH, TYPE));
    }

    @Test
    public void successfully_put_a_file_object() {
        when(client.putObject(any(PutObjectRequest.class)))
            .thenReturn(PUT_RESULT);

        filesystem.put(PATH, TYPE, object);
    }

    @Test(expected=FilesystemException.class)
    public void fail_to_put_a_file_object() {
        when(client.putObject(any(PutObjectRequest.class)))
            .thenThrow(new AmazonServiceException("boom"));

        filesystem.put(PATH, TYPE, object);
    }

    @Test
    public void successfully_get_a_file_object() throws Exception {
        S3Object response = new S3Object();
        response.setBucketName(BUCKET);
        response.setKey(PATH);
        response.setObjectContent(data);
        response.setObjectMetadata(METADATA);

        when(client.getObject(any(GetObjectRequest.class)))
            .thenReturn(response);

        Optional<FileObject> result = filesystem.get(PATH, TYPE);
        assertTrue(result.isPresent());
        assertFileObjectEqualsBytes(result.get());
    }

    @Test
    public void fail_to_get_a_file_object() {
        when(client.getObject(any(GetObjectRequest.class)))
            .thenThrow(new AmazonServiceException("boom"));

        Optional<FileObject> object = filesystem.get(PATH, TYPE);
        assertFalse(object.isPresent());
    }

    @Test(expected=FilesystemException.class)
    public void fail_to_delete_a_file_object() {
        doThrow(new AmazonServiceException("boom"))
            .when(client).deleteObject(any(DeleteObjectRequest.class));

        filesystem.delete(PATH, TYPE);
    }

    private void assertFileObjectEqualsBytes(FileObject object) throws Exception {
        byte[] result = ByteStreams.toByteArray(object.getData());
        assertArrayEquals(DATA_BYTES, result);
    }
}
