package org.bashwork.commons

import com.google.common.base.Stopwatch;
import com.google.common.collect.AbstractIterator;
import com.google.common.collect.Iterators;
import com.google.common.collect.PeekingIterator;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Comparator;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.PriorityQueue;
import java.util.TreeMap;
import java.util.UUID;
import java.util.concurrent.TimeUnit;
import java.util.function.Predicate;
import java.util.stream.Collectors;

/**
 */
public class PerformanceTest {

    public interface Serializer<T> {
        String serialize(T value);
    }

    public interface Deserializer<T> {
        T deserialize(String value);
    }

    /**
     *
     * @param <K> The type of the key
     * @param <V> The type of the value
     */
    public static final class OrderedAggregator<K, V> {

        private final Map<K, V> values = new TreeMap<>();
        private final Predicate<Map<K, V>> validator;
        private final Serializer<Map.Entry<K, V>> serializer;

        public OrderedAggregator(final Predicate<Map<K, V>> validator,
            final Serializer<Map.Entry<K, V>> serializer) {

            this.validator = validator;
            this.serializer = serializer;
        }

        public boolean put(final K key, final V value) {
            values.put(key, value);
            return validator.test(values);
        }

        public void write(final BufferedWriter writer)
            throws IOException {

            for (Map.Entry<K, V> entry : values.entrySet()) {
                writer.write(serializer.serialize(entry));
            }
            values.clear();
        }
    }

    /**
     * Represents a simple pair of two values along with a number of utility methods
     * for operating on their values.
     *
     * @param <K> The type of the key
     * @param <V> The type of the value
     */
    public static final class Pair<K extends Comparable<K>, V>
        implements Comparable<Pair<K, V>> {

        private final K key;
        private final V value;

        public Pair(final K key, final V value) {
            this.key = key;
            this.value = value;
        }

        public static <K extends Comparable<K>, V> Pair<K, V> create(Map.Entry<K, V> entry) {
            return new Pair<>(entry.getKey(), entry.getValue());
        }

        public static <T extends Comparable<T>> Pair<T, T> create(T[] values) {
            return new Pair<>(values[0], values[1]);
        }

        public static <K extends Comparable<K>, V> Pair<K, V> create(K key, V value) {
            return new Pair<>(key, value);
        }

        public static <K extends Comparable<K>, V> Deserializer<Pair<K, V>> deserializeWith(
            final Deserializer<K> kd, final Deserializer<V> vd) {
            return (string) -> {
                final String[] parts = string.substring(1, string.length() - 2).split(",");
                if (parts.length != 2) {
                    throw new IllegalArgumentException("invalid serialized Pair");
                }
                return Pair.create(kd.deserialize(parts[0]), vd.deserialize(parts[1]));

            };
        }

        public K getKey() { return key; }
        public V getValue() { return value; }

        @Override
        public boolean equals(final Object other) {
            if (other instanceof Pair) {
                Pair<K, V> that = (Pair<K, V>)other;
                return Objects.equals(this.key, that.key)
                    && Objects.equals(this.value, that.value);
            }
            return false;
        }

        @Override
        public int hashCode() {
            return Objects.hash(key, value);
        }

        @Override
        public String toString() {
            return "(" + key + "," + value + ")";
        }

        @Override
        public int compareTo(final Pair<K, V> that) {
            return this.getKey().compareTo(that.getKey());
        }
    }

    /**
     *
     * @param <K> The type of the pair key
     * @param <V> The type of the pair value
     */
    public static final class MultiPriorityQueue<K extends Comparable<K>, V>
        implements Iterator<Pair<K, V>> {

        private final PriorityQueue<PeekingIterator<Pair<K, V>>> heap;
        private final Comparator<PeekingIterator<Pair<K, V>>> comparator = (a, b) -> {
            if (a.hasNext() && b.hasNext()) {
                return a.peek().compareTo(b.peek());
            }
            return a.hasNext() ? 1 : -1;
        };

        public MultiPriorityQueue(List<Iterator<Pair<K, V>>> iterators) {
            this.heap = new PriorityQueue<>(comparator);
            for (Iterator<Pair<K, V>> iterator : iterators) {
                heap.add(Iterators.peekingIterator(iterator));
            }
        }

        @Override
        public boolean hasNext() {
            return !heap.isEmpty();
        }

        @Override
        public Pair<K, V> next() {
            final PeekingIterator<Pair<K, V>> iterator = heap.remove();
            final Pair<K, V> value = iterator.next();

            if (iterator.hasNext()) {
                heap.add(iterator);
            }

            return value;
        }
    }

    /**
     *
     */
    public static final class Pairs {
        private Pairs() { }

        /**
         * @param files
         * @param deserializer
         * @param <K> The type of the pair key
         * @param <V> The type of the pair value
         * @return
         */
        public static <K extends Comparable<K>, V> MultiPriorityQueue<K, V> toPriorityQueue(
            final List<Path> files, final Deserializer<Pair<K, V>> deserializer) {

            return new MultiPriorityQueue<>(files.stream()
                .map(file -> toIterator(file, deserializer))
                .collect(Collectors.toList()));
        }

        /**
         * @param file
         * @param deserializer
         * @param <K> The type of the pair key
         * @param <V> The type of the pair value
         * @return
         */
        public static <K extends Comparable<K>, V> Iterator<Pair<K, V>> toIterator(
            final Path file, final Deserializer<Pair<K, V>> deserializer) {
            final BufferedReader reader;
            try {
                reader = Files.newBufferedReader(file);
            } catch (IOException ex) {
                throw new RuntimeException(ex);
            }

            return new AbstractIterator<Pair<K, V>>() {
                @Override
                protected Pair<K, V> computeNext() {
                    try {
                        final String line = reader.readLine();
                        if (line == null) {
                            reader.close();
                            return endOfData();
                        }
                        return deserializer.deserialize(line);
                    } catch (IOException e) {
                        return endOfData();
                    }
                }
            };
        }
    }

    /**
     * A collection of extra methods that would be nice in the {@link Files}
     * utility class.
     */
    public static final class MoreFiles {
        private MoreFiles() { }

        /**
         * @param root The path to remove all the files at.
         */
        public static void deleteAll(Path root) {
            try {
                if (Files.isDirectory(root)) {
                    Files.list(root).forEach(MoreFiles::deleteAll);
                }
                Files.deleteIfExists(root);
            } catch (IOException ex) {
                // ignored for the stream to work
            }
        }

        /**
         * @param root The path to create the fresh directory at.
         * @return The new fresh directory path.
         * @throws IOException If there was an error cleaning or creating the directory.
         */
        public static Path createFresh(final String root)
            throws IOException {

            final Path path = Paths.get(root);
            if (Files.isDirectory(path)) {
                deleteAll(path);
            }
            return Files.createDirectory(path);
        }
    }

    /**
     * Run the performance test of the heap sort.
     *
     * @param arguments The command line arguments to operate with
     * @throws IOException If there was an error working with files.
     */
    public static void main(String[] arguments) throws IOException {
        final int numberOfRecords = 12_500_000;
        final Stopwatch stopwatch = Stopwatch.createStarted();
        final Path parent = MoreFiles.createFresh("/tmp/heap");
        final Predicate<Map<String, Integer>> predicate = map -> map.size() >= (numberOfRecords / 10);
        final Serializer<Map.Entry<String, Integer>> serializer = e -> "(" + e.getKey() + "," + e.getValue() + ")";
        final OrderedAggregator<String, Integer> aggregator = new OrderedAggregator<>(predicate, serializer);

        System.out.println("initializing: " + stopwatch.elapsed(TimeUnit.SECONDS));
        for (int number = 0; number < numberOfRecords; ++number) {
            if (aggregator.put(UUID.randomUUID().toString(), number)) {
                try (final BufferedWriter writer = Files.newBufferedWriter(parent.resolve("chunk" + number))) {
                    aggregator.write(writer);
                }
            }
        }

        System.out.println("created splits: " + stopwatch.elapsed(TimeUnit.SECONDS));
        final Deserializer<Pair<String, Integer>> deserializer = Pair.deserializeWith(s -> s, Integer::parseInt);
        final List<Path> files = Files.list(parent).collect(Collectors.toList());
        final MultiPriorityQueue<String, Integer> queue = Pairs.toPriorityQueue(files, deserializer);

        System.out.println("initializing: " + stopwatch.elapsed(TimeUnit.SECONDS));
        try (BufferedWriter writer = Files.newBufferedWriter(parent.resolve("final"))) {
            while (queue.hasNext()) {
                final Pair<String, Integer> pair = queue.next();
                writer.write(pair.toString());
                writer.newLine();
            }
        }

        System.out.println("completed: " + stopwatch.elapsed(TimeUnit.SECONDS));
    }
}
