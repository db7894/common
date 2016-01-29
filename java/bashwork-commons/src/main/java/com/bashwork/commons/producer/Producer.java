package com.bashwork.commons.producer;

import java.time.Duration;

/**
 * Represents a producer that can produce any type of message
 * that can be consumed by some future consumer.
 */
public interface Producer {

    /**
     * Produce a new message to be consumed.
     * @param message The message to be consumed.
     * @param <TProduct> The type of message to be produced.
     * @return A unique identifier for the produced message.
     */
    <TProduct> String produce(TProduct message);

    /**
     * Produce a new message to be consumed.
     * @param message The message to be consumed.
     * @param delay The amount of time to delay delivery of the message.
     * @param <TProduct> The type of message to be produced.
     * @return A unique identifier for the produced message.
     */
    <TProduct> String produce(TProduct message, Duration delay);
}
