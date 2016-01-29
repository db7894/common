package com.bashwork.commons.utility;

import java.util.concurrent.Future;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

import com.google.common.util.concurrent.ListenableFuture;
import com.google.common.util.concurrent.ListeningScheduledExecutorService;

/**
 * A collection of utility methods that make it easier to work with
 * asynchronous futures.
 */
public final class AsyncFutures {
    private AsyncFutures() { }

    /**
     * Cancel the future if it hasn't completed within the given timeout.
     *
     * @param executor service suitable for scheduling frequently-canceled tasks
     * @param future future to cancel
     * @param delay wait this long before canceling
     * @param unit units for timeout
     * @param <TResult> The result type of the underlying future.
     */
    public static <TResult> void scheduleTimeout(ScheduledExecutorService executor,
        final ListenableFuture<TResult> future, long delay, TimeUnit unit) {

        final Future<?> timeoutFuture = executor.schedule(new Runnable() {

            @Override
            public void run() {
                future.cancel(true);
            }
        }, delay, unit);

        future.addListener(new Runnable() {

            @Override
            public void run() {
                timeoutFuture.cancel(false);

            }
        }, executor);
    }

    /**
     * Cancel the future if it hasn't completed within the given timeout.
     *
     * @param executor service suitable for scheduling frequently-canceled tasks
     * @param runnable The runnable to cancel
     * @param delay wait this long before canceling
     * @param unit units for timeout
     */
    public static void scheduleTimeout(ListeningScheduledExecutorService executor,
        Runnable runnable, long delay, TimeUnit unit) {
        scheduleTimeout(executor, executor.submit(runnable), delay, unit);
    }
}
