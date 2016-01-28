package com.bashwork.commons.utility;

import java.util.function.BooleanSupplier;

/**
 * Class for validating input/output variables etc.
 *
 * @author mbarrows
 */
public final class Validate {
    private Validate() { }

    /**
     * Throws IllegalArgumentException iff predicate is false.
     *
     * @param predicate The predicate to check for truth.
     * @param message The message to return on failure
     * @return The original object passed in.
     */
    public static void check(boolean predicate, String message) {
        if (!predicate) {
            throw new IllegalArgumentException(message);
        }
    }

    /**
     * Throws IllegalArgumentException iff predicate is false.
     *
     * @param predicate The predicate to check for truth.
     * @param message The message to return on failure
     * @return The original object passed in.
     */
    public static void check(BooleanSupplier predicate, String message) {
        check(predicate.getAsBoolean(), message);
    }

    /**
     * Throws IllegalArgumentException iff object is null.
     * Throws with the message "Must supply a non-null Object"
     *
     * @param object the object to check.
     * @param <T> the type of object to validate.
     * @return The original object passed in.
     */
    public static <T> T notNull(T object) {
        return notNull(object, "Object");
    }

    /**
     * Throws IllegalArgumentException iff object is null.
     * Throws with the message "Must supply a non-null [name supplied]"
     *
     * @param object the object to check
     * @param name name of the object to report in the exception
     * @param <T> the type of object to validate.
     * @return The original object passed in.
     */
    public static <T> T notNull(T object, String name) {
        if (object == null) {
            throw new IllegalArgumentException("Must supply a non-null " + name);
        }
        return object;
    }

    /**
     * Throws IllegalArgumentException iff String is null or empty.
     * Throws with the message "Must supply a non-empty String"
     *
     * @param string the string to check
     * @return The original object passed in.
     */
    public static String notEmpty(String string) {
        return notEmpty(string, "String");
    }

    /**
     * Throws IllegalArgumentException iff String is null or empty.
     * Throws with the message "Must supply a non-empty [name supplied]"
     *
     * @param string the string to check
     * @param name name of the string to report in the exception
     * @return The original object passed in.
     */
    public static String notEmpty(String string, String name) {
        notNull(string, name);
        if (string.isEmpty()) {
            throw new IllegalArgumentException("Must supply a non-empty " + name);
        }
        return string;
    }

    /**
     * Throws IllegalArgumentException iff String is null, empty, or blank.
     * Throws with the message "Must supply a non-blank String"
     *
     * @param string the string to check
     * @return The original object passed in.
     */
    public static String notBlank(String string) {
        return notBlank(string, "String");
    }

    /**
     * Throws IllegalArgumentException iff String is null, empty, or blank.
     * Throws with the message "Must supply a non-blank [name supplied]"
     *
     * @param string the string to check.
     * @param name name of the string to report in the exception.
     * @return The original object passed in.
     */
    public static String notBlank(String string, String name) {
        notEmpty(string, name);
        for (int i = 0; i < string.length(); i++) {
            if (!Character.isWhitespace(string.charAt(i))) {
                // At least one character is not whitespace
                return string;
            }
        }
        throw new IllegalArgumentException("Must supply a non-blank " + name);
    }

    /**
     * Check if the supplied value is non negative.
     *
     * @param value The value to check
     * @return The original object passed in.
     */
    public static long isNonNegative(long value) {
        return isNonNegative(value, "Long");
    }

    /**
     * Check if the supplied value is non negative.
     *
     * @param value The value to check
     * @param name name of the value to report in the exception.
     * @return The original object passed in.
     */
    public static long isNonNegative(long value, String name) {
        if (value < 0) {
            throw new IllegalArgumentException("Must supply a non-negative " + name + ": " + value);
        }
        return value;
    }

    /**
     * Check if the supplied object is positive.
     *
     * @param value The value to check
     * @return The original object passed in.
     */
    public static long isPositive(long value) {
        return isPositive(value, "Long");
    }

    /**
     * Check if the supplied object is positive.
     *
     * @param value The value to check
     * @param name name of the value to report in the exception.
     * @return The original object passed in.
     */
    public static long isPositive(long value, String name) {
        if (value <= 0) {
            throw new IllegalArgumentException("Must supply a positive " + name + ": " + value);
        }
        return value;
    }

    /**
     * Check if two objects are equal according to the equals method of the first.
     *
     * @param expected The first value to check
     * @param actual The second value to check
     */
    public static void areEqual(Object expected, Object actual) {
        if (expected == actual) {
            return;
        }
        if (expected == null || !expected.equals(actual)) {
            throw new IllegalArgumentException("Expected " + expected + " but got " + actual);
        }
    }
}
