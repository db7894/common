package com.bashwork.commons.serialize;

import static org.apache.commons.lang3.Validate.notNull;

import java.io.IOException;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectMapper.DefaultTyping;
import com.fasterxml.jackson.databind.SerializationFeature;


/**
 * Implements conversion through Jackson JSON processor. Consult its
 * documentation on how to ensure that classes are serializable, configure their
 * serialization through annotations and {@link ObjectMapper} parameters.
 * 
 * <p>
 * Note that default configuration used by this class includes class name of the
 * every serialized value into the produced JSON. It is done to support
 * polymorphic types out of the box. But in some cases it might be beneficial to
 * disable polymorphic support as it produces much more concise and portable
 * output.
 */
public class JsonSerializer implements Serializer {

    protected final ObjectMapper mapper;

    /**
     * Create instance of the converter that uses ObjectMapper with
     * {@link Feature#FAIL_ON_UNKNOWN_PROPERTIES} set to <code>false</code> and
     * default typing set to {@link DefaultTyping#NON_FINAL}.
     */
    public JsonSerializer() {
        this(new ObjectMapper());
        // ignoring unknown properties makes us more robust to changes in the schema
        mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
        mapper.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, false);

        // This will allow including type information all non-final types.  This allows correct 
        // serialization/deserialization of generic collections, for example List<MyType>. 
        mapper.enableDefaultTyping(DefaultTyping.NON_FINAL);
    }

    /**
     * Create instance of the converter that uses {@link ObjectMapper}
     * configured externally.
     */
    public JsonSerializer(ObjectMapper mapper) {
        this.mapper = notNull(mapper, "Must provide an object mapper.");
    }

    @Override
    public<T> String serialize(T value) throws SerializerException {
        try {
            return mapper.writeValueAsString(value);
        } catch (IOException ex) {
            throwSerializerException(ex, value);
        }
        throw new IllegalStateException("not reachable");
    }

    @Override
    public <T> T deserialize(String serialized, Class<T> klass) throws SerializerException {
        try {
            return mapper.readValue(serialized, klass);
        } catch (IOException ex) {
            throw new SerializerException(ex);
        }
    }
    
    /**
     * Helper method to throw a data converter exception
     * 
     * @param ex The exception to throw.
     * @param value The value of the object to throw about.
     */
    private void throwSerializerException(Throwable ex, Object value) {
        if (value == null) {
            throw new SerializerException("Failure serializing null value", ex);
        }
        throw new SerializerException("Failure serializing \"" + value + "\" of type \"" + value.getClass() + "\"", ex);
    }  
}
