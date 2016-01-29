package com.bashwork.commons.serialize.string;

import static com.bashwork.commons.utility.Validate.notNull;

import com.bashwork.commons.serialize.Serializer;
import com.bashwork.commons.serialize.SerializerException;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonSyntaxException;

/**
 * A utility that can serialize from any type to and from a
 * JSON string:
 *
 * <pre>
 * {@code Serializer<String>} serializer = new JsonStringSerializer();
 * String serialized = serializer.serialize(new Example());
 * Example deserialized = serializer.deserialize(serialized, Example.class);
 * </pre>
 */
public class JsonStringSerializer implements Serializer<String> {

    private final Gson serializer;

    /**
     * Initialize a new instance of the JsonStringSerializer
     */
    public JsonStringSerializer() {
        this(new GsonBuilder().create());
    }

    /**
     * Initialize a new instance of the JsonStringSerializer
     *
     * @param serializer The overloaded serializer object to use.
     */
    public JsonStringSerializer(Gson serializer) {
        this.serializer = notNull(serializer, "serializer");
    }

    /**
     * @see Serializer#serialize(Object)
     */
    @Override
    public <TValue> String serialize(TValue value) {
        return serializer.toJson(value);
    }

    /**
     * @see Serializer#deserialize(Object, Class)
     */
    @Override
    public <TValue> TValue deserialize(String from, Class<TValue> klass) {
        try {
            return serializer.fromJson(from, klass);
        } catch (JsonSyntaxException ex) {
            throw new SerializerException(ex);
        }
    }
}
