import java.util.Optional;
import java.util.function.Supplier;
import java.time.Instant;
import java.time.Duration;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.ForkJoinPool;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ScheduledExecutorService;

import com.squareup.okhttp.MediaType;
import com.squareup.okhttp.OkHttpClient;
import com.squareup.okhttp.Request;
import com.squareup.okhttp.RequestBody;
import com.squareup.okhttp.Response;
import java.io.IOException;

/**
 * http://www.nurkiewicz.com/2013/05/java-8-definitive-guide-to.html
 * http://download.java.net/lambda/b88/docs/api/java/util/concurrent/CompletableFuture.html
 */
public final class CompletableFutures {
    private final static ScheduledExecutorService executor =
        Executors.newScheduledThreadPool(2);

    /**
     * An example of creating your own future that is completed
     * and then returned from your own executor.
     */
    public void wait_on_complete_result() throws InterruptedException, ExecutionException {
        final CompletableFuture<String> future = new CompletableFuture<>();
        executor.execute((ThrowingRunnable)() -> {
            System.out.println("[3] performing long operation");
            Thread.sleep(1000);
            future.complete("[5] result of future");
            System.out.println("[4] finished long operation");
        });
        System.out.println("[1] waiting for future");
        System.out.println(future.getNow("[2] future is not finished yet"));
        ThrowingSupplier.ignoreExceptions(future::get).ifPresent(System.out::println);
        System.out.println("[6] finished getting future\n");
    }

    /**
     * An example of creating your own future that is completed
     * with a failure immediately
     */
    public void wait_on_exception_result() {
        final CompletableFuture<String> future = new CompletableFuture<>();
        executor.execute((ThrowingRunnable)() -> {
            System.out.println("[2] failing the computation");
            Thread.sleep(1000);
            future.completeExceptionally(new RuntimeException("boom"));
            System.out.println("[3] finished failed operation");
        });
        System.out.println("[1] waiting for future");
        ThrowingSupplier.ignoreExceptions(future::get).ifPresent(System.out::println);
        System.out.println("[4] finished getting future\n");
    }

    public void wait_on_time_instant() throws InterruptedException, ExecutionException {
        supplyDelayed(Instant::now, Duration.ofSeconds(1))
            .thenApply(instant -> instant.toString())
            .thenAccept(System.out::println)
            .get();
    }

    public void wait_on_current_ip_address() throws InterruptedException, ExecutionException {
        final String url = "http://checkip.dyn.com";
        CompletableFuture
            .supplyAsync((ThrowingSupplier<String>)() -> WebClient.get(url))
            .thenAccept(System.out::println)
            .get();
    }
    // CompletableFuture.runAsync(supplier)
    // CompletableFuture.runAsync(supplier, executor)

    /**
     * Shutdown all the currently running operations
     */
    public void close() {
        try {
            System.out.println("shutting down the executors");
            executor.shutdown();
            executor.awaitTermination(5, TimeUnit.SECONDS);
        } catch (Exception ex) { }
    }

    /**
     *
     */
    public static void main(String args[]) throws Exception {
        final CompletableFutures futures = new CompletableFutures();
        futures.wait_on_complete_result();
        futures.wait_on_exception_result();
        futures.wait_on_time_instant();
        futures.wait_on_current_ip_address();
        futures.close();
    }

    /**
     * An example of creating a future that is completed immediately.
     */
    public static <T> CompletableFuture<T> supplyDelayed(final Supplier<T> supplier,
        final Duration duration) {

        if (duration == null || duration.isZero()) {
            return CompletableFuture.completedFuture(supplier.get());
        }

        final CompletableFuture<T> future = new CompletableFuture<>();
        final ScheduledFuture<Boolean> scheduled = executor.schedule(() ->
            future.complete(supplier.get()), duration.toMillis(), TimeUnit.MILLISECONDS);

        future.whenComplete((t, ex) -> scheduled.cancel(true));

        return future;
    }

    /**
     * A simple http client to make requests with
     */
    public static final class WebClient {

        private static final MediaType JSON = MediaType.parse("application/json; charset=utf-8");
        private static final OkHttpClient client = new OkHttpClient();

        public static String get(final String url) throws IOException {
            final Request request = new Request.Builder()
                .url(url).build();
            
            final Response response = client.newCall(request).execute();
            return response.body().string();
        }

        public static String post(final String url, final String json) throws IOException {
            final Request request = new Request.Builder()
                .url(url)
                .post(RequestBody.create(JSON, json))
                .build();

            final Response response = client.newCall(request).execute();
            return response.body().string();
        }
    }

    /**
     * Converts a Runnable to a Runnable that will block
     * checked exceptions and throw a RuntimeException instead.
     */
    @FunctionalInterface
    public interface ThrowingRunnable extends Runnable {
    
        @Override
        default void run() {
            try {
                runThrows();
            } catch (final Exception ex) {
                throw new RuntimeException(ex);
            }
        }

        void runThrows() throws Exception;
    }

    /**
     * Converts a Supplier<T> to a Supplier<T> that will block
     * checked exceptions and throw a RuntimeException instead.
     */
    @FunctionalInterface
    public interface ThrowingSupplier<T> extends Supplier<T> {
    
        @Override
        default T get() {
            try {
                return getThrows();
            } catch (final Exception ex) {
                throw new RuntimeException(ex);
            }
        }

        T getThrows() throws Exception;

        /**
         * A helper method that will ignore all exceptions that
         * are thrown from the given supplier.
         */
        public static <T> Optional<T> ignoreExceptions(final ThrowingSupplier<T> supplier) {
            try {
                return Optional.ofNullable(supplier.get());
            } catch (Exception ex) {
                return Optional.empty();
            }
        }
    }
}
