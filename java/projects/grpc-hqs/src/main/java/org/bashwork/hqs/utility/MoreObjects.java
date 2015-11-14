package org.bashwork.hqs.utility;

import java.util.function.Supplier;

/**
 * A collection of utility methods that are not available in
 * the BCL.
 */
public final class MoreObjects {
    private MoreObjects() { }

    /**
     * Given two numbers, return the first that is positive.
     *
     * @param first The first number to check if it is positive.
     * @param second The second number to check if it is positive.
     * @return The first number that is positive.
     */
    public static int firstPositive(final int first, final int second) {
        return (first <= 0) ? second : first;
    }

    /**
     * Given two numbers, return the first that is positive..
     *
     * @param first The first number to check if it is positive.
     * @param second The second number to check if it is positive.
     * @return The first number that is positive.
     */
    public static long firstPositive(final long first, final long second) {
        return (first <= 0) ? second : first;
    }

    /**
     * Given two objects, return the first that is not null.
     *
     * @param first The first object to check for null.
     * @param second The second object to check for null.
     * @param <T> The type of object to return.
     * @return The first non null object.
     */
    public static <T> T firstNonNull(final T first, final T second) {
        return (first == null) ? second : first;
    }

    /**
     * Given two objects, return the first that is not null or generate
     * the new second one.
     *
     * @param first The first object to check for null.
     * @param second The supplier of the second object in case of null.
     * @param <T> The type of object to return.
     * @return The first non null object.
     */
    public static <T> T firstNonNull(final T first, final Supplier<T> second) {
        return (first == null) ? second.get() : first;
    }
}
