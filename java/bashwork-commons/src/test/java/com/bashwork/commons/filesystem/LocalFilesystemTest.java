package com.bashwork.commons.filesystem;

import static com.bashwork.commons.activity.UnitTestHelper.getPathForImage;
import static org.junit.Assert.assertArrayEquals;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertTrue;

import java.io.ByteArrayInputStream;
import java.io.InputStream;

import org.junit.Before;
import org.junit.Test;

import com.google.common.base.Optional;
import com.google.common.io.ByteStreams;

/**
 * Code to validate the LocalFilesystem class.
 */
public class LocalFilesystemTest {

    private static final String ROOT = "/tmp/";

    private static final String PATH = getPathForImage("Warehouse", "LocalFilesystemTest");
    private static final FileType TYPE = FileType.IMAGE;

    private static final byte[] DATA_BYTES = "payload".getBytes();

    private Filesystem filesystem;
    private FileObject object;

    @Before
    public void setup() {
        filesystem = new LocalFilesystem(ROOT);
        filesystem.delete(PATH, TYPE); // Ensure previous data doesn't screw with us
        object = FileObject.builder()
            .withData(new ByteArrayInputStream(DATA_BYTES))
            .withType(TYPE)
            .build();
    }

    // ------------------------------------------------------------------------
    // test methods
    // ------------------------------------------------------------------------

    /*
     * Exists
     */

    @Test(expected=IllegalArgumentException.class)
    public void exists_null_path() {
        filesystem.exists(null, TYPE);
    }

    @Test(expected=IllegalArgumentException.class)
    public void exists_null_type() {
        filesystem.exists(PATH, null);
    }

    @Test
    public void exists_false() {
        assertFalse(filesystem.exists(PATH, TYPE));
    }

    @Test
    public void exists_true() {
        filesystem.put(PATH, TYPE, object);
        assertTrue(filesystem.exists(PATH, TYPE));
    }

    /*
     * Put
     */

    @Test(expected=IllegalArgumentException.class)
    public void put_null_path() {
        filesystem.put(null, TYPE, object);
    }

    @Test(expected=IllegalArgumentException.class)
    public void put_null_type() {
        filesystem.put(PATH, null, object);
    }

    @Test(expected=IllegalArgumentException.class)
    public void put_null_object() {
        filesystem.put(PATH, TYPE, null);
    }

    @Test
    public void put_replaces() throws Exception {
        InputStream data2 = new ByteArrayInputStream("payload2".getBytes());
        FileObject object2 = FileObject.builder().withData(data2).withType(TYPE).build();

        filesystem.put(PATH, TYPE, object2);
        filesystem.put(PATH, TYPE, object);

        Optional<FileObject> object = filesystem.get(PATH, TYPE);
        assertTrue(object.isPresent());
        assertFileObjectEqualsBytes(object.get());
    }

    @Test
    public void put_delete() {
        filesystem.put(PATH, TYPE, object);
        assertTrue(filesystem.exists(PATH, TYPE));

        filesystem.delete(PATH, TYPE);
        assertFalse(filesystem.exists(PATH, TYPE));
    }

    /*
     * Get
     */

    @Test
    public void put_get() throws Exception {
        filesystem.put(PATH, TYPE, object);

        Optional<FileObject> object = filesystem.get(PATH, TYPE);
        assertTrue(object.isPresent());
        assertFileObjectEqualsBytes(object.get());
    }

    @Test
    public void get_nothing() {
        Optional<FileObject> object = filesystem.get(PATH, TYPE);
        assertFalse(object.isPresent());
    }

    @Test(expected=IllegalArgumentException.class)
    public void get_null_path() {
        filesystem.get(null, TYPE);
    }

    @Test(expected=IllegalArgumentException.class)
    public void get_null_type() {
        filesystem.get(PATH, null);
    }

    /*
     * Delete
     */

    @Test(expected=IllegalArgumentException.class)
    public void delete_null_path() {
        filesystem.delete(null, TYPE);
    }

    @Test(expected=IllegalArgumentException.class)
    public void delete_null_type() {
        filesystem.delete(PATH, null);
    }

    /*
     * (other)
     */

    @Test
    public void no_arg_constructor() {
        LocalFilesystem filesystem = new LocalFilesystem();
        assertEquals("/", filesystem.getRoot());
    }

    protected void assertFileObjectEqualsBytes(FileObject object) throws Exception {
        byte[] result = ByteStreams.toByteArray(object.getData());
        assertArrayEquals(DATA_BYTES, result);
    }

}
