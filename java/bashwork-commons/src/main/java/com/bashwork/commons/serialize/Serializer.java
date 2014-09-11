package com.bashwork.commons.serialize;

/**
 * An interface that describes a utility that can serialize
 * a type to and from some encoding.
 */
public interface Serializer<TSerialized> {

    public <TValue> TSerialized serialize(TValue value);
    public <TValue> TValue deserialize(TSerialized value, Class<TValue> klass);
}
