package com.bashwork.commons.filesystem;

import static com.bashwork.commons.utility.Validate.notBlank;
import static com.bashwork.commons.utility.Validate.notNull;

import com.amazonaws.services.s3.model.S3Object;
import com.amazonaws.services.s3.model.ObjectMetadata;
import com.amazonaws.services.s3.model.GetObjectRequest;
import com.amazonaws.services.s3.model.GetObjectMetadataRequest;
import com.amazonaws.services.s3.model.PutObjectRequest;
import com.amazonaws.services.s3.model.PutObjectResult;
import com.amazonaws.services.s3.model.DeleteObjectRequest;
import org.apache.commons.io.FilenameUtils;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import com.bashwork.commons.utility.Validate;
import com.amazonaws.AmazonServiceException;
import com.amazonaws.services.s3.AmazonS3;
import java.util.Optional;

/**
 * Implementation of the file-system interface that uses
 * S3 as its storage mechanism.
 */
public class S3Filesystem implements Filesystem {

    static final Logger logger = LogManager.getLogger(S3Filesystem.class);

    private final AmazonS3 client;
    private final String bucket;

    /**
     * Initialize a new instance of the S3Filesystem class.
     *
     * @param client The S3 client to operate with.
     * @param bucket The bucket to operate out of.
     */
    public S3Filesystem(AmazonS3 client, String bucket) {
        this.client = notNull(client, "AmazonS3");
        this.bucket = notBlank(bucket, "Bucket Name");
    }

    /**
     * @see Filesystem#exists(String, FileType)
     */
    @Override
    public boolean exists(String path, FileType type) {

        notNull(path, "Path");
        notNull(type, "Type");

        try {
            GetObjectMetadataRequest request = new GetObjectMetadataRequest(bucket, createPath(path, type));
            client.getObjectMetadata(request);
        } catch (AmazonServiceException ex) {
            return false;
        }
        return true;
    }

    /**
     * @see Filesystem#get(String, FileType)
     */
    @Override
    public Optional<FileObject> get(String path, FileType type) {

        notNull(path, "Path");
        notNull(type, "Type");

        try {
            GetObjectRequest request = new GetObjectRequest(bucket, createPath(path, type));
            S3Object response = client.getObject(request);
            return Optional.of(adapt(response));
        } catch (AmazonServiceException ex) {
            // TODO: differentiate between FileDoesntExist and other exceptions
            return Optional.empty();
        }
    }

    /**
     * @see Filesystem#put(String, FileType, FileObject)
     */
    @Override
    public void put(String path, FileType type, FileObject object) {

        notNull(path, "Path");
        notNull(type, "Type");
        notNull(object, "Object");

        try {
            ObjectMetadata metadata = new ObjectMetadata();
            metadata.setContentType(type.getMimeType());
            if (object.getSize() >= 0) { // -1 is 'unknown'
                metadata.setContentLength(object.getSize());
            }

            String pathWithExtension = createPath(path, type);
            PutObjectRequest request = new PutObjectRequest(
                bucket, pathWithExtension, object.getData(), metadata);
            PutObjectResult result = client.putObject(request);
            logger.debug("result of storing file {}: {}", pathWithExtension, result);
        } catch (AmazonServiceException ex) {
            throw new FilesystemException("unable to store the given file object", ex);
        }
    }

    /**
     * @see Filesystem#delete(String, FileType)
     */
    @Override
    public void delete(String path, FileType type) {

        notNull(path, "Path");
        notNull(type, "Type");

        try {
            String pathWithExtension = createPath(path, type);
            DeleteObjectRequest request = new DeleteObjectRequest(bucket, pathWithExtension);
            client.deleteObject(request);
            logger.debug("result of deleting file {}: {}", pathWithExtension, "success");
        } catch (AmazonServiceException ex) {
            throw new FilesystemException("unable to delete the given file object", ex);
        }
    }

    /**
     * Adapt a S3Object to a FileObject class.
     *
     * @param object The S3Object to adapt.
     * @return The adapted FileObject.
     */
    private static FileObject adapt(S3Object object) {
        return FileObject.builder()
            .withData(object.getObjectContent())
            .withType(adapt(object.getObjectMetadata()))
            .build();
    }

    /**
     * Adapt an ObjectMetadata to the FileObject meta-data
     *
     * @param metadata The ObjectMetadata to convert
     * @return The adapted FileObject meta-data.
     */
    private static FileType adapt(ObjectMetadata metadata) {
        return FileType.getByMimeType(metadata.getContentType());
    }

    /**
     * Generates a String path that includes a file extension based on FileType.
     *
     * Current implementation requires that the path already have the file extension.
     *
     * @param path The base path to use
     * @param type The type to use to determine extension
     * @return The full path
     */
    private static String createPath(String path, FileType type) {
        String extension = FilenameUtils.getExtension(path);
        Validate.areEqual(type.getExtension(), extension);
        return path;
    }
}
