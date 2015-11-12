package org.bashwork.hqs.utility;

import java.util.Collection;
import java.util.List;
import java.util.Set;
import java.util.function.Function;
import java.util.stream.Collectors;

/**
 * A collection of utility methods for working with Java 8
 * streams.
 */
public final class Streams {
    private Streams() { }

    /**
     * Given a collection of some type, extract just the field that
     * is necessary from that collection.
     *
     * @param collection The collection to extract the value from.
     * @param <T> The original collection type.
     * @param <U> The resulting collection type.
     * @return The resulting extracted set.
     */
    public static <T, U> Set<U> extract(List<T> collection, Function<T, U> extractor) {
        return collection.stream()
            .map(extractor)
            .collect(Collectors.toSet());
    }

    /**
     * Given a collection of elements and a disjunction set, return the
     * set disjunction of the two collections.
     *
     * @param collection The original collection.
     * @param disjoint The set to filter with.
     * @param <T> The type of the collection.
     * @return The set disjunction of the two collections.
     */
    public static <T> List<T> disjunction(Collection<T> collection, Set<T> disjoint) {
        return collection.stream()
            .filter(entry -> !disjoint.contains(entry))
            .collect(Collectors.toList());
    }
}
