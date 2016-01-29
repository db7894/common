package com.bashwork.commons.producer;

import java.time.Duration;

import static com.bashwork.commons.utility.Validate.notNull;

/**
 * A collection of common producers and utility methods
 * for working with them.
 */
public final class Producers {
    private Producers() { }

    /**
     * Given a generic producer, create a new producer that is specialized
     * to produce a single type of message.
     *
     * @param producer The producer to specialize.
     * @param <TProduct> The type to specialize to producer to.
     * @return The new specialized producer.
     */
    public static <TProduct> SpecificProducer<TProduct> toSpecific(Producer producer) {
        notNull(producer, "producer");

        return new SpecificProducer<TProduct>() {
            @Override
            public String produce(TProduct message) {
                return producer.produce(message);
            }

            @Override
            public String produce(TProduct message, Duration delay) {
                return producer.produce(message, delay);
            }
        };
    }
}
