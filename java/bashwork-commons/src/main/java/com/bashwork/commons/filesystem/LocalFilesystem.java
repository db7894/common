package com.bashwork.commons.filesystem;

import static com.bashwork.commons.utility.Validate.notNull;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.Files;
import java.nio.file.StandardCopyOption;
import java.nio.file.AtomicMoveNotSupportedException;

import org.apache.commons.io.FilenameUtils;
import org.apache.logging.log4j.LogManager;

import com.bashwork.commons.utility.Validate;
import java.util.Optional;

/**
 * Implementation of the {@link Filesystem} interface that
 * uses the local file-system as its storage mechanism.
 *
 * @author mbarrows
 */
public class LocalFilesystem implements Filesystem {

    static final org.apache.logging.log4j.Logger logger = LogManager.getLogger(LocalFilesystem.class);
    private final String root;

    /**
     * Initialize a new instance of the LocalFilesystem class.
     */
    public LocalFilesystem() {
        this("/");
    }

    /**
     * Shim to retrieve the current root value for this filesystem.
     *
     * @return The current filesystem root.
     */
    public String getRoot() {
        return root;
    }

    /**
     * Initialize a new instance of the LocalFilesystem class.
     *
     * @param root The root path to jail the filesystem at.
     */
    public LocalFilesystem(String root) {
        notNull(root, "Root");
        this.root = root;
    }

    /**
     * @see Filesystem#exists(String, FileType)
     */
    @Override
    public boolean exists(String path, FileType type) {

        notNull(path, "Path");
        notNull(type, "Type");

        final File file = fileOf(path, type);
        return file.exists() && file.isFile();
    }

    /**
     * @see Filesystem#get(String, FileType)
     */
    @Override
    public Optional<FileObject> get(String path, FileType type) {

        notNull(path, "Path");
        notNull(type, "Type");

        final File file = fileOf(path, type);
        final InputStream inputStream;

        try {
            inputStream = new FileInputStream(file);
        } catch (FileNotFoundException e) {
            return Optional.empty();
        }

        final FileObject object = FileObject.builder()
            .withData(inputStream)
            .withType(type)
            .build();

        return Optional.of(object);
    }

    /**
     * @see Filesystem#put(String, FileType, FileObject)
     */
    @Override
    public void put(String path, FileType type, FileObject object) {

        notNull(path, "Path");
        notNull(type, "Type");
        notNull(object, "Object");

        final Path tempPath;
        try {
            final String prefix = null;
            final String suffix = null;
            tempPath = Files.createTempFile(prefix, suffix);
            logger.debug("created empty temporary file: {}", tempPath);
        } catch (IOException e) {
            throw new FilesystemException("failure creating empty temporary file", e);
        }

        final InputStream stream = object.getData();
        try {
            Files.copy(stream, tempPath, StandardCopyOption.REPLACE_EXISTING);
            logger.debug("wrote data to file: {}", tempPath);
        } catch (IOException e) {
            throw new FilesystemException("unable to write to temporary file object", e);
        }

        final Path pathObj = fileOf(path, type).toPath();
        try {
            final Path parent = pathObj.getParent();
            Files.createDirectories(parent);
            logger.debug("created directories: {}", parent);
        } catch (IOException e) {
            throw new FilesystemException("unable to create the given directories", e);
        }

        try {
            try {
                Files.move(tempPath, pathObj, StandardCopyOption.ATOMIC_MOVE);
                logger.debug("moved tmp to file: {}", pathObj);
            } catch (AtomicMoveNotSupportedException ex) {
                /**
                 * If we are crossing file-systems or devices, then we cannot perform
                 * an atomic move. In this case, this is the best we can do.
                 */
                Files.move(tempPath, pathObj, StandardCopyOption.REPLACE_EXISTING);
            }
        } catch (IOException e) {
            throw new FilesystemException("unable to store the given file object", e);
        }
    }

    /**
     * @see Filesystem#delete(String, FileType)
     */
    @Override
    public void delete(String path, FileType type) {
        notNull(path, "Path");
        notNull(type, "Type");

        final Path pathObj = fileOf(path, type).toPath();
        try {
            Files.deleteIfExists(pathObj);
        } catch (IOException e) {
            throw new FilesystemException("unable to delete the given file object", e);
        }
    }

    /**
     * Delete the supplied file if it exists.
     *
     * @param directory The path to the file to delete.
     */
    public void deleteDirectory(String directory) {
        deleteDirectory(Paths.get(directory));
    }

    /**
     * Helper method to delete a given directory recursively.
     * @param path The path to delete.
     */
    private static void deleteDirectory(Path path) {
        try {
            if (Files.isDirectory(path)) {
                Files.list(path).forEach(LocalFilesystem::deleteDirectory);
            }
            Files.deleteIfExists(path);
        } catch (IOException ex) {
            throw new FilesystemException("unable to delete the given file object", ex);
        }
    }

    /**
     * Helper method to get a File handle instance given a path and a
     * filetype.
     *
     * @param path The path of the file object.
     * @param type The type of the file object.
     * @return The File handle for the given path.
     */
    private File fileOf(String path, FileType type) {
        String extension = FilenameUtils.getExtension(path);
        Validate.areEqual(type.getExtension(), extension);
        return new File(root, path);
    }
}
