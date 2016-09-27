package com.bashwork.commons.workflow2;

import java.time.Duration;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.UUID;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.ThreadLocalRandom;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.function.Consumer;
import java.util.function.Function;
import java.util.function.Predicate;
import java.util.function.Supplier;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

public final class WorkflowTester {

    // ------------------------------------------------------------------------
    // Example Runner
    // ------------------------------------------------------------------------

    final static AtomicInteger identifiers = new AtomicInteger(1);
    final ScheduledExecutorService scheduler = Executors.newScheduledThreadPool(1);
    final ExecutorService executor = Executors.newFixedThreadPool(4);

    public static void main(String[] arguments) throws Exception {
        final Random random = new Random();
        final WorkflowTester engine = new WorkflowTester();
        final List<CompletableFuture<State<String>>> futures = new ArrayList<>();

        IntStream.range(0, 5)
            .mapToObj(value -> engine.run(new Workflow<String>(){{
                request = random.nextBoolean() ? "success" : "failure";
                id = String.valueOf(identifiers.incrementAndGet());
            }})).collect(Collectors.toCollection(() -> futures));

        flatten(futures).join();
        System.out.println("completely finished!");
    }

    // ------------------------------------------------------------------------
    // State Machine Interfaces
    // ------------------------------------------------------------------------

    /**
     * Represents a single state in the state machine. This will take in the
     * current workflow context and then is required to return the next state
     * in the state machine.
     *
     * @param <T> The original request in the state machine.
     */
    @FunctionalInterface
    public interface State<T> {
        State<T> run(Workflow<T> workflow);

        static <T> State<T> from(Function<Workflow<T>, State<T>> function) {
            return function::apply;
        }
    }

    /**
     * A state in the state machine that needs to be run until some predicate
     * evaluates to true. This will run the state task passing the result to
     * the predicate while delaying the supplied amount between task runs. Here
     * one can supply a back off strategy. After the predicate evaluates to true,
     * the result of onFinish is used to supply the next state.
     *
     * @param <T> The original request in the state machine.
     */
    public interface WaitState<T> extends State<T> {
        Duration getWait();
        boolean isFinished(State<T> state);
        State<T> onFinish();
    }

    /**
     * A collection of common states that are used in the system. The finished
     * state is a special state used in the system to indicate that the workflow
     * is complete (can be used wherever in the graph).
     *
     * Other ideas for common states are:
     *
     * - delay(predicate, nextState) -> performs exponential backoff
     * - branch(predicate, left, right)
     * - retry(times, task, failure)
     * - split(values, task) -> values.each(v -> task(v)(state))) -> List[State]
     * - split -> allOf, oneOf, etc (what does splitting mean here though)
     */
    static final class States {
        private States(){ }

        // this will not work if we cross JVM boundaries...override equals
        static final State<Object> FINISHED = new State<Object>() {
            @Override
            public State<Object> run(Workflow<Object> workflow) {
                return this;
            }
        };

        /**
         * Mark a workflow as complete. This is a special value used by
         * the workflow engine.
         *
         * @param <T> The original request in the state machine.
         * @return The delay decorated next state.
         */
        @SuppressWarnings("unchecked")
        public static <T> State<T> finished() {
            return (State<T>)FINISHED;
        }

        /**
         * Decorate the next step with a delayed transition.
         *
         * @param duration The amount to delay before continuing.
         * @param nextState The next state to transition to.
         * @param <T> The original request in the state machine.
         * @return The delay decorated next state.
         */
        public static <T> State<T> delayed(Duration duration, State<T> nextState) {
            return new WaitState<T>() {
                private volatile boolean hasFinished = false;
                @Override
                public Duration getWait() {
                    // another common state could be delayed with a predicate and a delay
                    // policy that automatically does the exponential backoff with jitter,
                    // backoff, clamped higher bound, etc.
                    return duration;
                }

                @Override
                public boolean isFinished(State<T> state) {
                    return hasFinished;
                }

                @Override
                public State<T> onFinish() {
                    return nextState;
                }

                @Override
                public State<T> run(Workflow<T> workflow) {
                    // another common state could be supplied that provides a delegate
                    // here to perform during the wait state.
                    System.out.println("delaying work for: " + workflow);
                    hasFinished = true;
                    return this;
                }
            };
        }
    }

    /**
     * A typed key that can be used to store a value by a name along with
     * a type capture that can be used to convert it back from an untyped
     * array.
     *
     * @param <T> The underlying type of the key.
     */
    public interface Key<T> {
        String getName();
        Class<T> getType();

        static <T> Key<T> of(Class<T> type) {
            return of(type.getName(), type);
        }

        static <T> Key<T> of(String name, Class<T> type) {
            return new Key<T>() {
                @Override
                public String getName() {
                    return name;
                }

                @Override
                public Class<T> getType() {
                    return type;
                }
            };
        }
    }

    /**
     * A simple scratch space used to pass data around through the workflow.
     * This should be able to be serialized to a json blob that can be
     * persisted. It should also be able to be merged (CRDT latest wins):
     *
     * <code>
     * {
     *   key-name : {
     *     "key-type" : "com.some.type", "value" : value // json safe serialization
     *   },
     *   ...
     * }
     * </code>
     */
    public static final class Scratch {
        private Map<Key<?>, Object> database = new HashMap<>();

        @SuppressWarnings("unchecked")
        public <T> T get(Key<T> key) {
            return (T)database.get(key);
        }

        public <T> Key<T> put(Key<T> key, T value) {
            database.put(key, value);
            return key;
        }
    }

    /**
     * Represents the context of the current running workflow. It includes
     * the immutable original request, a unique identifier (supplied by the
     * creator), and a scratch space where data can be passed between states.
     *
     * This should also be able to be serialized to json and persisted:
     *
     * <code>
     * {
     *   id : "unique-identifier",
     *   scratch : { ... },
     *   request : {
     *     "type" : "com.some.type", "value" : value // json safe serialization
     *   },
     * }
     * </code>
     *
     * @param <T> The original request in the state machine.
     */
    public static class Workflow<T> {
        private final Scratch scratch = new Scratch();
        String id;
        T request;

        public T getRequest() { return request; }
        public String getIdentifier() { return id; }
        public Scratch getScratch() { return scratch; }

        @Override
        public String toString() { return id; }
    }

    // ------------------------------------------------------------------------
    // States
    // ------------------------------------------------------------------------

    /**
     * The common collection of keys used in this workflow.
     */
    static final class Keys {
        private Keys() { }

        public static final Key<UUID> Tracker = Key.of("tracker", UUID.class);
    }

    static final class StateStart implements State<String> {

        @Override
        public State<String> run(Workflow<String> workflow) {
            System.out.println("starting work for: " + workflow);
            if (workflow.getRequest().equals("success")) {
                workflow.getScratch().put(Keys.Tracker, UUID.randomUUID());
                return States.delayed(Duration.ofSeconds(2), new StateNext());
            } else {
                return new StateFail();
            }
        }
    }

    static final class StateNext implements WaitState<String> {
        final CountDownLatch latch = new CountDownLatch(5);

        @Override
        public Duration getWait() {
            return Duration.ofSeconds(
                ThreadLocalRandom.current().nextLong(0, 2));
        }

        @Override
        public boolean isFinished(State<String> state) {
            return (latch.getCount() == 0);
        }

        @Override
        public State<String> onFinish() {
            return new StateDone();
        }

        @Override
        public State<String> run(Workflow<String> workflow) {
            latch.countDown();
            System.out.println("doing the next work for: " + workflow);
            return this;
        }
    }

    static final class StateDone implements State<String> {

        @Override
        public State<String> run(Workflow<String> workflow) {
            System.out.println(
                "finished the work for: " + workflow +
                " tracker: " + workflow.getScratch().get(Keys.Tracker));
            return States.finished();
        }
    }

    static final class StateFail implements State<String> {

        @Override
        public State<String> run(Workflow<String> workflow) {
            System.out.println("failing the work for: " + workflow);
            return new StateDone();
        }
    }

    // ------------------------------------------------------------------------
    // State Machinery
    // ------------------------------------------------------------------------

    private CompletableFuture<State<String>> run(Workflow<String> workflow) {
        return execute(new StateStart(), workflow);
    }

    private static <T> CompletableFuture<List<T>> flatten(List<CompletableFuture<T>> futures) {
        // stupid array based api...
        return CompletableFuture
            .allOf(futures.toArray(new CompletableFuture[futures.size()]))
            .thenApply(value -> futures.stream()
                .map(CompletableFuture::join)
                .collect(Collectors.toList()));
    }

    private <T> CompletableFuture<State<T>> execute(State<T> state, Workflow<T> workflow) {
        final CompletableFuture<State<T>> future = (state instanceof WaitState)
            ? execute((WaitState<T>)state, workflow)
            : execute(() -> state.run(workflow));

        return (state == States.FINISHED)
            ? future
            : flatMap(nextState -> execute(nextState, workflow), future);
    }

    private <T> CompletableFuture<T> execute(Supplier<T> supplier) {
        return CompletableFuture.supplyAsync(supplier, executor);
    }

    private <T> CompletableFuture<State<T>> execute(WaitState<T> state, Workflow<T> workflow) {
        return poll(() -> state.run(workflow), state::isFinished, state::onFinish, state.getWait(), scheduler);
    }

    private <T, U> CompletableFuture<U> flatMap(Function<T, CompletableFuture<U>> function, CompletableFuture<T> future) {
        return future.thenComposeAsync(function, executor);
    }

    private static <T> CompletableFuture<T> poll(
        final Supplier<T> task,
        final Predicate<T> predicate,
        final Supplier<T> supplier,
        final Duration frequency,
        final ScheduledExecutorService executor) {

        final TimeUnit unit = TimeUnit.MILLISECONDS;
        final long jitter = ThreadLocalRandom.current().nextLong(1, 10); // at least 1 for delay
        final CompletableFuture<T> result = new CompletableFuture<>();
        final ScheduledFuture<?> scheduled = executor.scheduleAtFixedRate(() -> tryOrFail(() -> {
            if (predicate.test(task.get())) {
                result.complete(supplier.get());
            }
        }, result::completeExceptionally), 0, frequency.toMillis() + jitter, unit);
        result.whenComplete((r, ex) -> scheduled.cancel(true));
        return result;
    }

    private static void tryOrFail(Runnable runnable, Consumer<Throwable> onFailure) {
        try {
            runnable.run();
        } catch (Exception ex) {
            onFailure.accept(ex);
        }
    }
}
