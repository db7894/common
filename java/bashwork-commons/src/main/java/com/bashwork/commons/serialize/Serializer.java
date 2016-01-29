package com.bashwork.commons.serialize;

/**
 * An interface that describes a utility that can serialize
 * any type to and from some encoding.
 */
public interface Serializer<TSerialized> {

    /**
     * Given a value, serializer to the current serialization type.
     *
     * @param value The value to serialize.
     * @param <TDeserialized> The type to serialize from.
     * @return The serialized version of that value.
     */
    <TDeserialized> TSerialized serialize(TDeserialized value);

    /**
     * Given a serialized value, deserialize it back to its original type.
     *
     * @param value The serialized value.
     * @param klass The type to deserialize to.
     * @param <TDeserialized> The type to deserialize to.
     * @return The deserialized value.
     */
    <TDeserialized> TDeserialized deserialize(TSerialized value, Class<TDeserialized> klass);
}
