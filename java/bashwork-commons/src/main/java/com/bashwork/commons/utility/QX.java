package ql.api.itemdb;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.Comparator;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.Set;
import java.util.function.Function;
import java.util.function.Predicate;
import java.util.stream.Collectors;
import java.util.stream.Stream;
import java.util.stream.StreamSupport;

/**
 * A collection of simple queries that can be applied to any java collection
 * type without having to deal with any of the java Streams API.
 */
public final class QX {
    private QX() { }

    /**
     * Given a collection of values, convert it into a Java8 Stream.
     *
     * @param values The collection of values to convert into a stream.
     * @param <T> The type of the values in the collection.
     * @return The resulting stream of the supplied collection.
     */
    public static <T> Stream<T> streamOf(Iterable<T> values) {
        return StreamSupport.stream(values.spliterator(), false);
    }

    /**
     * Given a collection of values, convert it into a Java8 Stream.
     *
     * @param values The collection of values to convert into a stream.
     * @param <T> The type of the values in the collection.
     * @return The resulting stream of the supplied collection.
     */
    public static <T> Stream<T> streamOf(T[] values) {
        return Arrays.stream(values);
    }

    /**
     * Given a collection of values, convert it into a Java8 Stream.
     *
     * @param values The collection of values to convert into a stream.
     * @param <T> The type of the values in the collection.
     * @return The resulting stream of the supplied collection.
     */
    public static <T> Stream<T> streamOfVargs(T... values) {
        return Arrays.stream(values);
    }

    /**
     * Given a collection of values, convert it into a Java8 Stream.
     *
     * @param values The collection of values to convert into a stream.
     * @param <T> The type of the values in the collection.
     * @return The resulting stream of the supplied collection.
     */
    public static <T> Stream<T> streamOf(Collection<T> values) {
        return values.stream();
    }

    public static <T> long count(Iterable<T> values, Predicate<T> predicate) {
        return count(streamOf(values), predicate);
    }

    public static <T> long count(T[] values, Predicate<T> predicate) {
        return count(streamOf(values), predicate);
    }

    public static <T> long countVargs(Predicate<T> predicate, T... values) {
        return count(streamOf(values), predicate);
    }

    public static <T> long count(Collection<T> values, Predicate<T> predicate) {
        return count(streamOf(values), predicate);
    }

    public static <T> long count(Stream<T> values, Predicate<T> predicate) {
        return values.filter(predicate).count();
    }

    public static <T> long countNot(Iterable<T> values, Predicate<T> predicate) {
        return countNot(streamOf(values), predicate);
    }

    public static <T> long countNot(T[] values, Predicate<T> predicate) {
        return countNot(streamOf(values), predicate);
    }

    public static <T> long countNotVargs(Predicate<T> predicate, T... values) {
        return countNot(streamOf(values), predicate);
    }

    public static <T> long countNot(Collection<T> values, Predicate<T> predicate) {
        return countNot(streamOf(values), predicate);
    }

    public static <T> long countNot(Stream<T> values, Predicate<T> predicate) {
        return values.filter(predicate.negate()).count();
    }

    public static <T> List<T> filter(Iterable<T> values, Predicate<T> predicate) {
        return filter(streamOf(values), predicate);
    }

    public static <T> List<T> filter(T[] values, Predicate<T> predicate) {
        return filter(streamOf(values), predicate);
    }

    public static <T> List<T> filterVargs(Predicate<T> predicate, T... values) {
        return filter(streamOf(values), predicate);
    }

    public static <T> List<T> filter(Collection<T> values, Predicate<T> predicate) {
        return filter(streamOf(values), predicate);
    }

    public static <T> List<T> filter(Stream<T> values, Predicate<T> predicate) {
        return values.filter(predicate).collect(Collectors.toList());
    }

    public static <T> List<T> filterNot(Iterable<T> values, Predicate<T> predicate) {
        return filterNot(streamOf(values), predicate);
    }

    public static <T> List<T> filterNot(T[] values, Predicate<T> predicate) {
        return filterNot(streamOf(values), predicate);
    }

    public static <T> List<T> filterNotVargs(Predicate<T> predicate, T... values) {
        return filterNot(streamOf(values), predicate);
    }

    public static <T> List<T> filterNot(Collection<T> values, Predicate<T> predicate) {
        return filterNot(streamOf(values), predicate);
    }

    public static <T> List<T> filterNot(Stream<T> values, Predicate<T> predicate) {
        return values.filter(predicate.negate()).collect(Collectors.toList());
    }

    public static <T, R> List<R> map(Iterable<T> values, Function<T, R> mapper) {
        return map(streamOf(values), mapper);
    }

    public static <T, R> List<R> map(T[] values, Function<T, R> mapper) {
        return map(streamOf(values), mapper);
    }

    public static <T, R> List<R> mapVargs(Function<T, R> mapper, T... values) {
        return map(streamOf(values), mapper);
    }

    public static <T, R> List<R> map(Collection<T> values, Function<T, R> mapper) {
        return map(streamOf(values), mapper);
    }

    public static <T, R> List<R> map(Stream<T> values, Function<T, R> mapper) {
        return values.map(mapper).collect(Collectors.toList());
    }

    public static <T, R> Map<R, T> groupBy(Iterable<T> values, Function<T, R> grouper) {
        return groupBy(streamOf(values), grouper);
    }

    public static <T, R> Map<R, T> groupBy(T[] values, Function<T, R> grouper) {
        return groupBy(streamOf(values), grouper);
    }

    public static <T, R> Map<R, T> groupByVargs(Function<T, R> grouper, T... values) {
        return groupBy(streamOf(values), grouper);
    }

    public static <T, R> Map<R, T> groupBy(Collection<T> values, Function<T, R> grouper) {
        return groupBy(streamOf(values), grouper);
    }

    public static <T, R> Map<R, T> groupBy(Stream<T> values, Function<T, R> grouper) {
        return values.collect(Collectors.toMap(grouper, x -> x));
    }

    public static <T> boolean any(Iterable<T> values, Predicate<T> predicate) {
        return any(streamOf(values), predicate);
    }

    public static <T> boolean any(T[] values, Predicate<T> predicate) {
        return any(streamOf(values), predicate);
    }

    public static <T> boolean anyVargs(Predicate<T> predicate, T... values) {
        return any(streamOf(values), predicate);
    }

    public static <T> boolean any(Collection<T> values, Predicate<T> predicate) {
        return any(streamOf(values), predicate);
    }

    public static <T> boolean any(Stream<T> values, Predicate<T> predicate) {
        return values.anyMatch(predicate);
    }

    public static <T> boolean all(Iterable<T> values, Predicate<T> predicate) {
        return all(streamOf(values), predicate);
    }

    public static <T> boolean all(T[] values, Predicate<T> predicate) {
        return all(streamOf(values), predicate);
    }

    public static <T> boolean allVargs(Predicate<T> predicate, T... values) {
        return all(streamOf(values), predicate);
    }

    public static <T> boolean all(Collection<T> values, Predicate<T> predicate) {
        return all(values.stream(), predicate);
    }

    public static <T> boolean all(Stream<T> values, Predicate<T> predicate) {
        return values.allMatch(predicate);
    }

    public static <T, R> Set<R> unique(Iterable<T> values, Function<T, R> mapper) {
        return unique(streamOf(values), mapper);
    }

    public static <T, R> Set<R> unique(T[] values, Function<T, R> mapper) {
        return unique(streamOf(values), mapper);
    }

    public static <T, R> Set<R> uniqueVargs(Function<T, R> mapper, T... values) {
        return unique(streamOf(values), mapper);
    }

    public static <T, R> Set<R> unique(Collection<T> values, Function<T, R> mapper) {
        return unique(streamOf(values), mapper);
    }

    public static <T, R> Set<R> unique(Stream<T> values, Function<T, R> mapper) {
        return values.map(mapper).collect(Collectors.toSet());
    }

    public static <T, R extends Comparable<? super R>> Optional<T> max(Iterable<T> values, Function<T, R> mapper) {
        return max(streamOf(values), mapper);
    }

    public static <T, R extends Comparable<? super R>> Optional<T> max(T[] values, Function<T, R> mapper) {
        return max(streamOf(values), mapper);
    }

    public static <T, R extends Comparable<? super R>> Optional<T> maxVargs(Function<T, R> mapper, T... values) {
        return max(streamOf(values), mapper);
    }

    public static <T, R extends Comparable<? super R>> Optional<T> max(Collection<T> values, Function<T, R> mapper) {
        return max(values.stream(), mapper);
    }

    public static <T, R extends Comparable<? super R>> Optional<T> max(Stream<T> values, Function<T, R> mapper) {
        return max(values, Comparator.comparing(mapper));
    }

    public static <T> Optional<T> max(Iterable<T> values, Comparator<T> comparator) {
        return max(streamOf(values), comparator);
    }

    public static <T> Optional<T> max(T[] values, Comparator<T> comparator) {
        return max(streamOf(values), comparator);
    }

    public static <T> Optional<T> maxVargs(Comparator<T> comparator, T... values) {
        return max(streamOf(values), comparator);
    }

    public static <T> Optional<T> max(Collection<T> values, Comparator<T> comparator) {
        return max(values.stream(), comparator);
    }

    public static <T> Optional<T> max(Stream<T> values, Comparator<T> comparator) {
        return values.max(comparator);
    }

    public static <T, R extends Comparable<? super R>> Optional<T> min(Iterable<T> values, Function<T, R> mapper) {
        return min(streamOf(values), mapper);
    }

    public static <T, R extends Comparable<? super R>> Optional<T> min(T[] values, Function<T, R> mapper) {
        return min(streamOf(values), mapper);
    }

    public static <T, R extends Comparable<? super R>> Optional<T> minVargs(Function<T, R> mapper, T... values) {
        return min(streamOf(values), mapper);
    }

    public static <T, R extends Comparable<? super R>> Optional<T> min(Collection<T> values, Function<T, R> mapper) {
        return min(values.stream(), mapper);
    }

    public static <T, R extends Comparable<? super R>> Optional<T> min(Stream<T> values, Function<T, R> mapper) {
        return min(values, Comparator.comparing(mapper));
    }

    public static <T> Optional<T> min(Iterable<T> values, Comparator<T> comparator) {
        return min(streamOf(values), comparator);
    }

    public static <T> Optional<T> min(T[] values, Comparator<T> comparator) {
        return min(streamOf(values), comparator);
    }

    public static <T> Optional<T> minVargs(Comparator<T> comparator, T... values) {
        return min(streamOf(values), comparator);
    }

    public static <T> Optional<T> min(Collection<T> values, Comparator<T> comparator) {
        return min(values.stream(), comparator);
    }

    public static <T> Optional<T> min(Stream<T> values, Comparator<T> comparator) {
        return values.min(comparator);
    }

    /**
     * Sort the supplied collection of objects by the supplied comparator.
     *
     * @param values The values to sort
     * @param mapper The field or value to sort by
     * @param <T> The type of the objects to sort.
     * @return The sorted list of objects.
     */
    public static <T, R extends Comparable<? super R>> List<T> sort(Iterable<T> values, Function<T, R> mapper) {
        return sort(streamOf(values), mapper);
    }

    /**
     * Sort the supplied collection of objects by the supplied comparator.
     *
     * @param values The values to sort
     * @param mapper The field or value to sort by
     * @param <T> The type of the objects to sort.
     * @return The sorted list of objects.
     */
    public static <T, R extends Comparable<? super R>> List<T> sort(T[] values, Function<T, R> mapper) {
        return sort(streamOf(values), mapper);
    }

    /**
     * Sort the supplied collection of objects by the supplied comparator.
     *
     * @param values The values to sort
     * @param mapper The field or value to sort by
     * @param <T> The type of the objects to sort.
     * @return The sorted list of objects.
     */
    public static <T, R extends Comparable<? super R>> List<T> sortVargs(Function<T, R> mapper, T... values) {
        return sort(streamOf(values), mapper);
    }

    /**
     * Sort the supplied collection of objects by the supplied comparator.
     *
     * @param values The values to sort
     * @param mapper The field or value to sort by
     * @param <T> The type of the objects to sort.
     * @return The sorted list of objects.
     */
    public static <T, R extends Comparable<? super R>> List<T> sort(Collection<T> values, Function<T, R> mapper) {
        return sort(values.stream(), mapper);
    }

    /**
     * Sort the supplied collection of objects by the supplied comparator.
     *
     * @param values The values to sort
     * @param mapper The field or value to sort by
     * @param <T> The type of the objects to sort.
     * @return The sorted list of objects.
     */
    public static <T, R extends Comparable<? super R>> List<T> sort(Stream<T> values, Function<T, R> mapper) {
        return sort(values, Comparator.comparing(mapper));
    }

    /**
     * Sort the supplied collection of objects by the supplied comparator.
     *
     * @param values The values to sort
     * @param comparator The comparator to sort by
     * @param <T> The type of the objects to sort.
     * @return The sorted list of objects.
     */
    public static <T> List<T> sort(Iterable<T> values, Comparator<T> comparator) {
        return sort(streamOf(values), comparator);
    }

    /**
     * Sort the supplied collection of objects by the supplied comparator.
     *
     * @param values The values to sort
     * @param comparator The comparator to sort by
     * @param <T> The type of the objects to sort.
     * @return The sorted list of objects.
     */
    public static <T> List<T> sort(T[] values, Comparator<T> comparator) {
        return sort(streamOf(values), comparator);
    }

    /**
     * Sort the supplied collection of objects by the supplied comparator.
     *
     * @param values The values to sort
     * @param comparator The comparator to sort by
     * @param <T> The type of the objects to sort.
     * @return The sorted list of objects.
     */
    public static <T> List<T> sortVargs(Comparator<T> comparator, T... values) {
        return sort(streamOf(values), comparator);
    }

    /**
     * Sort the supplied collection of objects by the supplied comparator.
     *
     * @param values The values to sort
     * @param comparator The comparator to sort by
     * @param <T> The type of the objects to sort.
     * @return The sorted list of objects.
     */
    public static <T> List<T> sort(Collection<T> values, Comparator<T> comparator) {
        return sort(values.stream(), comparator);
    }

    /**
     * Sort the supplied collection of objects by the supplied comparator.
     *
     * @param values The values to sort
     * @param comparator The comparator to sort by
     * @param <T> The type of the objects to sort.
     * @return The sorted list of objects.
     */
    public static <T> List<T> sort(Stream<T> values, Comparator<T> comparator) {
        return values.sorted(comparator).collect(Collectors.toList());
    }

    public static <T> Optional<T> firstOf(Iterable<T> values, Predicate<T> predicate) {
        return firstOf(streamOf(values), predicate);
    }

    public static <T> Optional<T> firstOf(T[] values, Predicate<T> predicate) {
        return firstOf(streamOf(values), predicate);
    }

    public static <T> Optional<T> firstOfVargs(Predicate<T> predicate, T... values) {
        return firstOf(streamOf(values), predicate);
    }

    public static <T> Optional<T> firstOf(Collection<T> values, Predicate<T> predicate) {
        return firstOf(streamOf(values), predicate);
    }

    public static <T> Optional<T> firstOf(Stream<T> values, Predicate<T> predicate) {
        return values.filter(predicate).findFirst();
    }

    public static <T> Optional<T> lastOf(Iterable<T> values, Predicate<T> predicate) {
        return lastOf(streamOf(values), predicate);
    }

    public static <T> Optional<T> lastOf(T[] values, Predicate<T> predicate) {
        return lastOf(streamOf(values), predicate);
    }

    public static <T> Optional<T> lastOfVargs(Predicate<T> predicate, T... values) {
        return lastOf(streamOf(values), predicate);
    }

    public static <T> Optional<T> lastOf(Collection<T> values, Predicate<T> predicate) {
        return lastOf(streamOf(values), predicate);
    }

    public static <T> Optional<T> lastOf(Stream<T> values, Predicate<T> predicate) {
        return values.filter(predicate).reduce((prev, next) -> next);
    }

    public final class Customer {
        private final String name;
        private final int group;

        public Customer(String name, int group) {
            this.name = name;
            this.group = group;
        }

        String getName() { return name; }
        int getGroup() { return group; }

    }
}

public final class PX {
    private PX() { }

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

// validate argument methods
// predicate methods used by validate argument methods
