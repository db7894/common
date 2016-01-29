package com.bashwork.commons.serialize;

/**
 * An interface that describes a utility that can serialize
 * a specific type to and from some encoding.
 */
public interface SpecificSerializer<TSerialized, TDeserialized> {

    TSerialized serialize(TDeserialized value);
    TDeserialized deserialize(TSerialized value);
}
