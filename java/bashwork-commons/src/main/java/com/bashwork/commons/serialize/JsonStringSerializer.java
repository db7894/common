package com.bashwork.commons.serialize;

import java.io.IOException;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectMapper.DefaultTyping;
import com.fasterxml.jackson.databind.SerializationFeature;

/**
 * A utility that can serialize from some type to and from a
 * JSON string.
 */
public class JsonStringSerializer implements StringSerializer {
    
    private final ObjectMapper mapper;
    
    /**
     * Initialize a new instance of the JsonStringSerializer
     * 
     * @param mapper The overloaded mapper object to use.
     */
    public JsonStringSerializer(ObjectMapper mapper) {
        this.mapper = mapper;
    }
    
    /**
     * Initialize a new instance of the JsonStringSerializer
     */
    public JsonStringSerializer() {
        this(new ObjectMapper());
        // ignoring unknown properties makes us more robust to changes in the schema
        mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
        mapper.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, false);

        // This will allow including type information all non-final types.  This allows correct 
        // serialization/deserialization of generic collections, for example List<MyType>. 
        mapper.enableDefaultTyping(DefaultTyping.NON_FINAL);
    }

    /**
     * @see Serializer#serialize(Object)
     */
    @Override
    public <TValue> String serialize(TValue value) {
        try {
            return mapper.writeValueAsString(value);
        } catch(IOException ex) {
            throwException(ex, value);
        }
        throw new IllegalStateException("this cannot be reached");
    }

    /**
     * @see Serializer#deserialize(Object, Class)
     */
    @Override
    public <TValue> TValue deserialize(String from, Class<TValue> klass) {
        try {
            return mapper.readValue(from, klass);
        } catch(IOException ex) {
            throw new SerializerException(ex);            
        }
    }
    
    /**
     * A Helper method to throw a useful error message.
     * 
     * @param ex The exception to re-throw.
     * @param value The value that we attempted to deserialize.
     */
    private void throwException(Throwable ex, Object value) {
        if (value == null) {
            throw new SerializerException("Failure serializing null value", ex);
        }
        
        String message = "Failure serializing \"" + value + "\" of type \"" + value.getClass() + "\"";
        throw new SerializerException(message, ex);        
    }    
}
