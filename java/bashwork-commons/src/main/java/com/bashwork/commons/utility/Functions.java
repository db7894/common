package com.bashwork.commons.utility;

import java.util.function.Function;

/**
 * A collection of common functions that can be used as
 * lambda expressions.
 */
public final class Functions {
    private Functions() { }

    /**
     * Returns a function that always returns its input argument.
     *
     * @param <T> the type of the input and output objects to the function
     * @return a function that always returns its input argument
     */
    public static <T> Function<T, T> identity() {
        return t -> t;
    }

    /**
     * Returns a function that always returns its input argument.
     *
     * @param <U> the type of the output objects to the function
     * @return a function that always returns its input argument
     */
    public static <T, U> Function<T, U> constant(final U value) {
        return unused -> value;
    }
}
