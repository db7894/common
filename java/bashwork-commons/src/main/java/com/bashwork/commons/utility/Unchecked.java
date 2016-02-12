package com.bashwork.commons.utility;

import java.util.function.Consumer;
import java.util.function.Function;
import java.util.function.Predicate;
import java.util.function.Supplier;

/**
 * A collection of utility methods that can be used to perform
 * lambda operations without having to worry about wrapping
 * everything in a try-catch closure.
 */
public final class Unchecked {
    private Unchecked() { }

    /**
     * Checked wrapper around a {@link Predicate}.
     *
     * @param <T> The type accepted by the {@link Predicate}.
     */
    @FunctionalInterface
    public interface UncheckedPredicate<T> extends Predicate<T> {

        @Override
        default boolean test(T t) {
            try {
                return testThrows(t);
            } catch (Exception ex) {
                return rechecked(ex);
            }
        }

        boolean testThrows(T t) throws Exception;
    }

    /**
     * Checked wrapper around a {@link Function}.
     *
     * @param <T> The type accepted by the {@link Function}.
     * @param <R> The type returned by the {@link Function}.
     */
    @FunctionalInterface
    public interface UncheckedFunction<T, R> extends Function<T, R> {

        @Override
        default R apply(T t) {
            try {
                return applyThrows(t);
            } catch (Exception ex) {
                return rechecked(ex);
            }
        }

        R applyThrows(T t) throws Exception;
    }

    /**
     * Checked wrapper around a {@link Supplier}.
     *
     * @param <T> The type supplied by the {@link Supplier}.
     */
    @FunctionalInterface
    public interface UncheckedSupplier<T> extends Supplier<T> {

        @Override
        default T get() {
            try {
                return getThrows();
            } catch (Exception ex) {
                return rechecked(ex);
            }
        }

        T getThrows() throws Exception;
    }

    /**
     * Checked wrapper around a {@link Consumer}.
     *
     * @param <T> The type accepted by the {@link Consumer}.
     */
    @FunctionalInterface
    public interface UncheckedConsumer<T> extends Consumer<T> {

        @Override
        default void accept(T t) {
            try {
                acceptThrows(t);
            } catch (Exception ex) {
                rechecked(ex);
            }
        }

        void acceptThrows(T t) throws Exception;
    }

    /**
     * Wraps the supplied {@link Predicate} in an unchecked wrapper.
     *
     * @param unchecked The {@link Predicate} to wrap.
     * @param <T> The type accepted by the {@link Predicate}.
     * @return The wrapped {@link Predicate}.
     */
    public static <T> Predicate<T> unchecked(final UncheckedPredicate<T> unchecked) {
        return unchecked;
    }

    /**
     * Wraps the supplied {@link Function} in an unchecked wrapper.
     *
     * @param unchecked The function to wrap.
     * @param <T> The type accepted by the {@link Function}.
     * @param <R> The type returned by the {@link Function}.
     * @return The wrapped {@link Function}.
     */
    public static <T, R> Function<T, R> unchecked(final UncheckedFunction<T, R> unchecked) {
        return unchecked;
    }

    /**
     * Wraps the supplied {@link Supplier} in an unchecked wrapper.
     *
     * @param unchecked The {@link Supplier} to wrap.
     * @param <T> The type supplied by the {@link Supplier}.
     * @return The wrapped {@link Supplier}.
     */
    public static <T> Supplier<T> unchecked(final UncheckedSupplier<T> unchecked) {
        return unchecked;
    }

    /**
     * Wraps the supplied {@link Consumer} in an unchecked wrapper.
     *
     * @param unchecked The {@link Consumer} to wrap.
     * @param <T> The type accepted by the {@link Consumer}.
     * @return The wrapped {@link Consumer}.
     */
    public static <T> Consumer<T> unchecked(final UncheckedConsumer<T> unchecked) {
        return unchecked;
    }

    /**
     * Helper utility to make sure the correct exception is thrown.
     *
     * @param exception The caught exception to rechecked.
     * @param <T> The return type of the original function
     * @param <E> The type of the original exception.
     * @return This will never return; just a helper to fulfill the type contract.
     * @throws E The type of original exception.
     */
    @SuppressWarnings("unchecked")
    private static <T, E extends Throwable> T rechecked(final Exception exception)
        throws E {
        throw (E)exception;
    }
}
