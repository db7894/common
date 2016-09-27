package org.bashwork.guava;

import java.time.Duration;
import java.util.Optional;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;
import java.util.function.Supplier;

/**
 *
 */
public final class CompletableFutures {
    private CompletableFutures() { }

    /**
     * Schedule a polling task to wait for a result to complete.
     *
     * @param pollingTask The task to poll for a result on.
     * @param frequency The duration to delay between polling calls.
     * @param executor The executor to schedule the polling action on.
     * @param <T> The type of the result.
     * @return A future wrapping the final result.
     */
    public static <T> CompletableFuture<T> poll(
        final Supplier<Optional<T>> pollingTask,
        final Duration frequency,
        final ScheduledExecutorService executor) {

        final CompletableFuture<T> result = new CompletableFuture<>();
        final ScheduledFuture<?> scheduled = executor.scheduleAtFixedRate(() -> {
            try {
                    pollingTask.get().ifPresent(result::complete);
                } catch (Exception ex) {
                    result.completeExceptionally(ex);
                }
            }, 0, frequency.toMillis(), TimeUnit.MILLISECONDS);
        result.whenComplete((r, ex) -> scheduled.cancel(true));
        return result;
    }
}
