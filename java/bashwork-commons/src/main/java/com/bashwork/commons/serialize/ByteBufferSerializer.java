package com.bashwork.commons.serialize;

import java.nio.ByteBuffer;

/**
 * An interface that describes a utility that can serialize
 * a type to and from a {@link java.nio.ByteBuffer}.
 */
public interface ByteBufferSerializer extends Serializer<ByteBuffer> {
}
