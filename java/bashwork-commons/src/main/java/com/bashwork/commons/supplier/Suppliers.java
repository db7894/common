package com.bashwork.commons.supplier;

import static com.bashwork.commons.utility.Validate.notNull;

import java.util.Iterator;
import java.util.Optional;
import java.util.concurrent.ThreadLocalRandom;
import java.util.function.Supplier;

/**
 * A collection of common suppliers and supplier utility
 * methods.
 */
public final class Suppliers {
    private Suppliers() { }

    /**
     * Given a constant value, return a supplier that will
     * always return that value.
     *
     * @param value The value to build a supplier with.
     * @param <T> The underlying type of the supplier
     * @return An upgraded ConfirmedSupplier.
     */
    public static <T> ConfirmedSupplier<T> of(T value) {
        return toConfirmed(() -> value);
    }

    /**
     * Given an iterable, upgrade it to a ConfirmedSupplier.
     *
     * @param iterable The iterable to upgrade.
     * @param <T> The underlying type of the supplier
     * @return An upgraded ConfirmedSupplier.
     */
    public static <T> ConfirmedSupplier<T> of(Iterable<T> iterable) {
        notNull(iterable, "iterable");
        return of(iterable.iterator());
    }

    /**
     * Given an iterator, upgrade it to a ConfirmedSupplier.
     *
     * @param iterator The iterator to upgrade.
     * @param <T> The underlying type of the supplier
     * @return An upgraded ConfirmedSupplier.
     */
    public static <T> ConfirmedSupplier<T> of(Iterator<T> iterator) {
        notNull(iterator, "iterator");
        return toConfirmed(() -> iterator.hasNext() ? iterator.next() : null);
    }

    /**
     * Given an existing supplier, upgrade it to a confirmed supplier.
     *
     * @param supplier The original supplier to upgrade.
     * @param <T> The underlying type of the supplier
     * @return An upgraded ConfirmedSupplier.
     */
    public static <T> ConfirmedSupplier<T> toConfirmed(Supplier<T> supplier) {

        notNull(supplier, "supplier");

        return new ConfirmedSupplier<T>() {
            @Override
            public Optional<T> get() {
                return Optional.ofNullable(supplier.get());
            }

            @Override
            public void confirm(T supplied) {
                // is not confirmed using a regular supplier
            }
        };
    }

    /**
     * Given a supplier, sample that supplier at the supplied rate.
     *
     * @param supplier The supplier to sample from.
     * @param rate The rate at which to sample the supplier at.
     * @param <T> The underlying type of the supplier
     * @return An supplier that is sampled at the supplied rate.
     */
    public static <T> ConfirmedSupplier<T> sampled(final ConfirmedSupplier<T> supplier,
        final float rate) {

        notNull(supplier, "supplier");

        return new ConfirmedSupplier<T>() {
            @Override
            public Optional<T> get() {
                Optional<T> message = supplier.get();
                boolean shouldDrop = (ThreadLocalRandom.current().nextFloat() < rate);

                if (message.isPresent() && shouldDrop) {
                    confirm(message.get());
                    message = Optional.empty();
                }

                return message;
            }

            @Override
            public void confirm(T supplied) {
                supplier.confirm(supplied);
            }
        };
    }
}
