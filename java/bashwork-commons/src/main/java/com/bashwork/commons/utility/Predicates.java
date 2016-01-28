package com.bashwork.commons.utility;

import java.util.Collection;
import java.util.function.Function;
import java.util.function.Predicate;

/**
 * A collection of common predicates that can be used
 * in tests.
 */
public final class Predicates {
    private Predicates() { }

    public static <T, R> Predicate<T> notNull(Function<T, R> mapper) {
        return o -> (o != null) ? (mapper.apply(o) != null) : false;
    }

    public static <T, R> Predicate<T> isEmpty(Function<T, Collection<R>> mapper) {
        return o -> mapper.apply(o).isEmpty();
    }

    public static <T, R> Predicate<T> notEmpty(Function<T, Collection<R>> mapper) {
        return o -> !mapper.apply(o).isEmpty();
    }

    public static <T> Predicate<T> isEmptyString(Function<T, String> mapper) {
        return o -> mapper.apply(o).isEmpty();
    }

    public static <T> Predicate<T> notEmptyString(Function<T, String> mapper) {
        return o -> !mapper.apply(o).isEmpty();
    }
}
