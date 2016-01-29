package com.bashwork.commons.serialize;

import static com.bashwork.commons.utility.Validate.notNull;

import com.bashwork.commons.serialize.string.JsonStringSerializer;
import com.bashwork.commons.serialize.string.XmlStringSerializer;

/**
 * A collection of common serializers and utilities.
 */
public final class Serializers {
    private Serializers() { }

    /**
     * Given a generalized serializer, return a new constrained serializer
     * that only works on the supplied input type.
     *
     * @param serializer The generic serializer to constrain.
     * @param klass The class to constrain the serializer to.
     * @param <TSerialized> The type to serialize to.
     * @param <TDeserialized> The type to deserialize from.
     * @return The constrained serializer.
     */
    public static <TSerialized, TDeserialized> SpecificSerializer<TSerialized, TDeserialized> toSpecific(
        final Serializer<TSerialized> serializer, final Class<TDeserialized> klass) {

        notNull(serializer, "serializer");
        notNull(klass, "class");

        return new SpecificSerializer<TSerialized, TDeserialized>() {

            @Override
            public TSerialized serialize(TDeserialized message) {
                return serializer.serialize(message);
            }

            @Override
            public TDeserialized deserialize(TSerialized message) {
                return serializer.deserialize(message, klass);
            }
        };
    }

    /**
     * Create a strongly typed XML string serializer from the supplied type.
     *
     * @param klass The type to build a serializer for.
     * @param <TDeserialized> The type to deserialize from.
     * @return The specific XML string serializer.
     */
    public static <TDeserialized> SpecificSerializer<String, TDeserialized> specificXml(
        final Class<TDeserialized> klass) {
        return toSpecific(new XmlStringSerializer(), klass);
    }

    /**
     * Create a strongly typed JSON string serializer from the supplied type.
     *
     * @param klass The type to build a serializer for.
     * @param <TDeserialized> The type to deserialize from.
     * @return The specific JSON string serializer.
     */
    public static <TDeserialized> SpecificSerializer<String, TDeserialized> specificJson(
        final Class<TDeserialized> klass) {
        return toSpecific(new JsonStringSerializer(), klass);
    }
}
