package com.bashwork.commons.testing.supplier;

import com.bashwork.commons.producer.SpecificProducer;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.UnsupportedEncodingException;
import java.time.Duration;
import java.util.Deque;
import java.util.UUID;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentLinkedDeque;
import java.util.concurrent.Future;
import java.util.function.Supplier;

import static com.bashwork.commons.utility.Validate.notNull;

/**
 * This is a simple type that implements the producer and supplier
 * by using a local concurrent queue. This allows them to easily be
 * used for unit testing. It should be noted that completed futures
 * are not removed so that the idempotence of the service can be
 * guaranteed (producing of the same message twice will complete once).
 *
 * @param <TSupply> The underlying type of the supplier
 */
public abstract class SupplierProducerPair<TSupply>
    implements SpecificProducer<TSupply>, Supplier<TSupply> {

    private static final Logger logger = LogManager.getLogger(SupplierProducerPair.class);
    private static final String UTF8 = "UTF8";

    private Deque<TSupply> queue = new ConcurrentLinkedDeque<>();
    private ConcurrentHashMap<String, CompletableFuture<TSupply>> futures
        = new ConcurrentHashMap<>();

    /*
     * @see ConfirmedSupplier#get()
     */
    @Override
    public TSupply get() {
        return confirm(queue.poll());
    }

    /**
     * @see com.bashwork.commons.producer.Producer#produce(Object)
     */
    @Override
    public String produce(TSupply message) {
        final String tracker = toTracker(message);

        //
        // If the supplied message already exists in the queue, then
        // we will simply let the new and existing consumers block on
        // the same future and complete when it completes. We do not
        // add the same message to the queue twice as the second run
        // will complete as soon the producer subscribes to the future.
        //
        // The work that it actually completes will be thrown away as
        // it cannot complete an already completed future.
        //
        CompletableFuture<TSupply> produced = new CompletableFuture<>();
        CompletableFuture<TSupply> existing = futures.putIfAbsent(tracker, produced);

        if (produced != existing) {
            logger.debug("produced {} with the supplied message: {}", tracker, message);
            queue.offer(message);
        }

        return tracker;
    }

    /**
     * @see com.bashwork.commons.producer.Producer#produce(Object, Duration)
     *
     * This does not actually delay at the moment as this type
     * is mostly use for local testing. If we would like to add
     * the delay, it can be added later with a ScheduledExecutorService.
     */
    @Override
    public String produce(TSupply message, Duration delay) {
        return produce(message);
    }

    /**
     * A helper method to confirm a given message's future if it exists.
     *
     * @param supplied The message to confirm the future for.
     */
    private TSupply confirm(TSupply supplied) {
        if (supplied != null) {
            final String tracker = toTracker(supplied);
            final CompletableFuture<TSupply> future = futures.get(tracker);

            if (future != null) {
                logger.debug("confirmed {} with the supplied message: {}", tracker, supplied);
                future.complete(supplied);
            }
        }

        return supplied;
    }

    // ------------------------------------------------------------------------
    // Generic Interface
    // ------------------------------------------------------------------------

    /**
     * Given an existing message, generate the unique tracking identifier
     * for that message.
     *
     * @param message The message to get a tracking identifier for.
     * @return The tracking identifier.
     */
    protected abstract String toTracker(TSupply message);

    // ------------------------------------------------------------------------
    // Hidden Alias Interface
    // ------------------------------------------------------------------------

    /**
     * Quick alias to convert this to a producer.
     *
     * @return The producer interface.
     */
    public SpecificProducer<TSupply> getProducer() {
        return this;
    }

    /**
     * Quick alias to convert this to a supplier.
     *
     * @return The supplier interface.
     */
    public Supplier<TSupply> getSupplier() {
        return this;
    }

    // ------------------------------------------------------------------------
    // Hidden Callback Interface
    // ------------------------------------------------------------------------

    /**
     * A helper method to get the underlying future for
     * the registered workflow message. The resulting value
     * in the future will be the original request if it is
     * indeed confirmed.
     *
     * @param tracker The tracker to get a workflow message for.
     * @return The future that is attached to that tracker.
     */
    public Future<TSupply> getFuture(String tracker) {
        return futures.get(tracker);
    }

    // ------------------------------------------------------------------------
    // Factory Methods
    // ------------------------------------------------------------------------

    /**
     * A SupplierProducerPair that supplies and produces the same value.
     *
     * @param <TSupply> The type that is produced and supplied.
     */
    public static class Default<TSupply> extends SupplierProducerPair<TSupply> {

        @Override
        protected String toTracker(TSupply message) {
            //
            // This is an attempt to make an idempotent identifier for
            // each type that comes through. We require that the type
            // generate a consistent toString method (not the runtime version)
            // for this to work correctly.
            //
            notNull(message, "message");

            try {
                byte[] bytes = message.toString().getBytes(UTF8);
                return UUID.nameUUIDFromBytes(bytes).toString();
            } catch (UnsupportedEncodingException ex) {
                // this should never happen
                return UUID.randomUUID().toString();
            }
        }
    }
}
