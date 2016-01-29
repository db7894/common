package com.bashwork.commons.producer;

import java.time.Duration;

/**
 * Represents a producer that can produce a specific type
 * of message to a future consumer.
 *
 * @param <TProduct> The type of message to be produced.
 */
public interface SpecificProducer<TProduct> {

    /**
     * Produce a new message to be consumed.
     * @param message The message to be consumed
     * @return A unique identifier for the produced message.
     */
    String produce(TProduct message);

    /**
     * Produce a new message to be consumed.
     * @param message The message to be consumed
     * @param delay The amount of time to delay delivery of the message.
     * @return A unique identifier for the produced message.
     */
    String produce(TProduct message, Duration delay);
}
