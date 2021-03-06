package com.bashwork.commons.serialize.string;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotEquals;

import org.junit.Test;

import com.bashwork.commons.serialize.Serializer;
import com.bashwork.commons.serialize.SerializerException;
import com.bashwork.commons.serialize.Serializers;
import com.bashwork.commons.serialize.SpecificSerializer;
import com.bashwork.commons.serialize.TestPojo;

public class XmlStringSerializerTest {

    private static final Serializer<String> serializer = new XmlStringSerializer();
    private static final SpecificSerializer<String, TestPojo> specificSerializer =
        Serializers.specificXml(TestPojo.class);

    @Test
    public void test_specific_serialization() {
        TestPojo instance = new TestPojo("username", 29);
        String serialized = specificSerializer.serialize(instance);
        TestPojo deserialized = specificSerializer.deserialize(serialized);

        assertEquals(instance, deserialized);
    }

    @Test
    public void test_null_specific_serialization() {
        TestPojo instance = null;
        String serialized = specificSerializer.serialize(instance);
        TestPojo deserialized = specificSerializer.deserialize(serialized);

        assertEquals(instance, deserialized);
    }

    @Test
    public void test_serialization() {
        TestPojo instance = new TestPojo("username", 29);
        String serialized = serializer.serialize(instance);
        TestPojo deserialized = serializer.deserialize(serialized, TestPojo.class);

        assertEquals(instance, deserialized);
    }

    /**
     * JAXB will make a best effort to attempt to deserialize to a
     * given type. So it will generate an empty instance and then attempt
     * to fill it in with the supplied data. Thus, deserializing to
     * an incorrect type will succeed, but be invalid.
     */
    @Test
    public void test_invalid_serialization() {
        String instance = "username";
        String serialized = serializer.serialize(instance);
        TestPojo deserialized = serializer.deserialize(serialized, TestPojo.class);

        assertNotEquals(instance, deserialized);
    }

    @Test
    public void test_null_serialization() {
        TestPojo instance = null;
        String serialized = serializer.serialize(instance);
        TestPojo deserialized = serializer.deserialize(serialized, TestPojo.class);

        assertEquals(instance, deserialized);
    }

    @Test
    public void test_null_class_serialization() {
        Class<TestPojo> klass = null;
        TestPojo instance = new TestPojo("username", 29);
        String serialized = serializer.serialize(instance);
        TestPojo deserialized = serializer.deserialize(serialized, klass);

        assertEquals(null, deserialized);
    }

    @Test(expected=SerializerException.class)
    public void test_invalid_xml_deserialization() {
        TestPojo instance = new TestPojo("username", 29);
        String serialized = "<TestPojo><value>23</TestPojo>";
        TestPojo deserialized = serializer.deserialize(serialized, TestPojo.class);

        assertEquals(instance, deserialized);
    }
}
