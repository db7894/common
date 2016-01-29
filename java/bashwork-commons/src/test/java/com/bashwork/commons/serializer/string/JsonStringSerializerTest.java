package com.bashwork.commons.serializer.string;

import static org.junit.Assert.assertEquals;

import org.junit.Test;

import com.bashwork.commons.serializer.Serializer;
import com.bashwork.commons.serializer.SerializerException;
import com.bashwork.commons.serializer.Serializers;
import com.bashwork.commons.serializer.SpecificSerializer;
import com.bashwork.commons.serializer.TestPojo;

public class JsonStringSerializerTest {

    private static final Serializer<String> serializer = new JsonStringSerializer();
    private static final SpecificSerializer<String, TestPojo> specificSerializer =
        Serializers.specificJson(TestPojo.class);

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

    @Test(expected=SerializerException.class)
    public void test_invalid_serialization() {
        String instance = "username";
        String serialized = serializer.serialize(instance);
        TestPojo deserialized = serializer.deserialize(serialized, TestPojo.class);

        assertEquals(instance, deserialized);
    }

    @Test
    public void test_null_serialization() {
        TestPojo instance = null;
        String serialized = serializer.serialize(instance);
        TestPojo deserialized = serializer.deserialize(serialized, TestPojo.class);

        assertEquals(instance, deserialized);
    }
}
