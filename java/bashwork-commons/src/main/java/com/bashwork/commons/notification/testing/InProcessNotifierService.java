package com.bashwork.commons.notification.testing;

import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.CompletableFuture;

import com.bashwork.commons.notification.NotificationException;
import com.bashwork.commons.notification.NotifierService;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * This is a notifier service that can be used for in process
 * unit or integration tests. The general idea is that we create
 * a map of futures which the test will block on. When the notify
 * comes through, the worker thread will trigger the future to
 * be complete.
 */
public class InProcessNotifierService implements NotifierService {

    private static final Logger logger = LogManager.getLogger(InProcessNotifierService.class);
    private final ConcurrentHashMap<String, CompletableFuture<String>> futures = new ConcurrentHashMap<>();

    /**
     * @see NotifierService#notify(String, String)
     */
    @Override
    public String notify(String endpoint, String message) throws NotificationException {
        logger.info("notification from({}): {}", endpoint, message);
        CompletableFuture<String> future = futures.remove(endpoint);
        if (future == null) {
            throw new NotificationException("No available endpoints to notify on");
        }

        future.complete(message);
        return "future:" + endpoint;
    }

    /**
     * @see NotifierService#isEndpointValid(String)
     */
    @Override
    public boolean isEndpointValid(String endpoint) {
        return true;
    }

    /**
     * Register an endpoint to be notified on. It should be noted that this
     * will only create a new future if it does not exist. Multiple notifiers
     * will use the same future until they are notified to, and then they will
     * be removed.
     *
     * @param endpoint The endpoint to notify on.
     * @return The future to block on that endpoint with.
     */
    public CompletableFuture<String> register(String endpoint) {
        logger.info("registration for({})", endpoint);
        return futures.computeIfAbsent(endpoint,
            key -> new CompletableFuture<String>());
    }
}
