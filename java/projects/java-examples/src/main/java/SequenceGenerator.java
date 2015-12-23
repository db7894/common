import java.util.stream.LongStream;
import java.util.Iterator;
import java.util.concurrent.atomic.AtomicLong;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.Executors;
import java.util.concurrent.ExecutorService;

/**
 * The idea here is to let threads block off a region of values that only
 * they can pull from. When that range is exhausted, they claim the next
 * available region of values:
 *
 * 1. [1, 2, 3], [7, 8, 9]
 * 2. [4, 5, 6], [10, 11, 12]
 *
 * Another idea is to have each thread choose a multiple, so that the threads
 * will do the following:
 *
 * 1. 10, 20, 30
 * 2. 11, 21, 31
 */
public final class SequenceGenerator implements Iterator<Long>, Iterable<Long> {

    private static final long range = 32;
    private final AtomicLong offset;
    private final ThreadLocal<Shard> shards = new ThreadLocal<Shard>() {
        @Override
        protected Shard initialValue() {
            return new Shard(offset.getAndAdd(range));
        }
    };

    /**
     * @param start Where to start the count at
     */
    public SequenceGenerator(final long start) {
        this.offset = new AtomicLong(start);
    }

    public Iterator<Long> iterator() {
        return this;
    }

    public Long next() {
        if (!shards.get().hasNext()) {
            shards.set(new Shard(offset.getAndAdd(range)));
        }
        return shards.get().next();
    }

    public boolean hasNext() {
        return true;
    }

    public LongStream stream() {
        return LongStream.generate(() -> this.next());
    }

    public static void main(String[] args) {
        final SequenceGenerator generator = new SequenceGenerator(0);
        final ExecutorService service = Executors.newFixedThreadPool(4);

        for (int i = 0; i < 4; ++i) {
            service.execute(new Printer(generator));
        }
    }

    /**
     * Simple runnable to exercise the generator
     */
    private static final class Printer implements Runnable {
        private final SequenceGenerator generator;

        public Printer(SequenceGenerator generator) {
            this.generator = generator;
        }

        public void run() {
            final long id = Thread.currentThread().getId();
            generator.stream().forEach(value ->
                System.out.println("thread[" + id + "]: " + value));
        }
    }

    /**
     * A single shard that can be used to generate a stream of
     * longs.
     */
    private static final class Shard implements Iterator<Long>, Iterable<Long> {
        private final long stop;
        private final AtomicLong counter;

        public Shard(final long start) {
            this.counter = new AtomicLong(start);
            this.stop = start + range;
        }

        public Iterator<Long> iterator() { return this; }
        public Long next() {
            return counter.getAndIncrement();
        }

        public boolean hasNext() {
            return counter.get() < stop;
        }
    }
}
