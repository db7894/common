package com.bashwork.commons.serialize.string;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.UnsupportedEncodingException;
import java.nio.charset.Charset;

import javax.xml.bind.DataBindingException;
import javax.xml.bind.JAXB;

import com.bashwork.commons.serialize.Serializer;
import com.bashwork.commons.serialize.SerializerException;

/**
 * A utility that can serialize from any type to and from a
 * XML string:
 *
 * <pre>
 * {@code Serializer<String>} serializer = new XmlStringSerializer();
 * String serialized = serializer.serialize(new Example());
 * Example deserialized = serializer.deserialize(serialized, Example.class);
 * </pre>
 */
public class XmlStringSerializer implements Serializer<String> {

    private static final String UTF8 = "UTF-8";
    private static final Charset CHARSET = Charset.forName(UTF8);

    /**
     * @see Serializer#serialize(Object)
     */
    @Override
    public <TValue> String serialize(TValue value) {
        if (value == null) {
            return null;
        }

        try {
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            JAXB.marshal(value, stream);
            return stream.toString(UTF8);
        } catch (UnsupportedEncodingException ex) {
            throw new SerializerException(ex);
        }
    }

    /**
     * @see Serializer#deserialize(Object, Class)
     */
    @Override
    public <TValue> TValue deserialize(String from, Class<TValue> klass) {
        if (from == null || klass == null) {
            return null;
        }

        try {
            byte[] bytes = from.getBytes(CHARSET);
            ByteArrayInputStream stream = new ByteArrayInputStream(bytes);
            return JAXB.unmarshal(stream, klass);
        } catch (DataBindingException ex) {
            throw new SerializerException(ex);
        }
    }
}
