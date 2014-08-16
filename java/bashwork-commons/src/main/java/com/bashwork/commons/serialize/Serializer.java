package com.bashwork.commons.serialize;

/**
 * Used by the framework to serialize/deserialize method parameters
 * that need to be sent over the wire. 
 */
public interface Serializer {

    /**
     * Implements conversion of the single value.
     * 
     * @param value The java value to convert to a String.
     * @return The converted String value.
     * @throws SerializerException If conversion fails for any reason
     */
    public<T> String serialize(T value) throws SerializerException;

    /**
     * Implements conversion of the single value.
     * 
     * @param content The data value to convert to a Java object.
     * @param klass The class of the Java object to convert to.
     * @return converted Java object
     * @throws SerializerException If conversion fails for any reason
     */
    public <T> T deserialize(String content, Class<T> klass) throws SerializerException;

}
