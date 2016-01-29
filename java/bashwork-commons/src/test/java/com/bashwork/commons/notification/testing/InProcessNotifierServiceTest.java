package com.bashwork.commons.notification.testing;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;

import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;

import org.junit.Before;
import org.junit.Test;

import com.bashwork.commons.notification.NotificationException;

/**
 * The JDK8 CompletableFuture API is confusing at best. Since I have to
 * constantly look up the documentation to remember what operation does
 * what, here is a JDK8 to Scala reference:
 *
 * - CompletableFuture  => Future + Promise
 * - future.thenApply   => future.map(f: (T) => U)
 * - future.thenRun     => future.map(f: ()  => Unit)
 * - future.thenAccept  => future.map(f: (T) => Unit)
 * - future.thenCombine => List[Future] -> Future[List]
 * - future.thenCompose => future.flatMap *
 */
public class InProcessNotifierServiceTest {

    private InProcessNotifierService service;

    @Before
    public void setup() {
        service = new InProcessNotifierService();
    }

    @Test(expected=NotificationException.class)
    public void notify_without_registration() {
        service.notify("endpoint", "message");
    }

    @Test
    public void registered_callback_is_notified() throws InterruptedException, ExecutionException {
        CompletableFuture<Void> future = service.register("endpoint")
            .thenAccept(response -> assertThat(response, equalTo("message")));

        service.notify("endpoint", "message");

        future.get();
    }

    @Test
    public void with_chained_callback() throws InterruptedException, ExecutionException {
        CompletableFuture<Integer> future = service.register("endpoint1")
            .thenCombine(service.register("endpoint2"), (m1, m2) ->
            Integer.parseInt(m1) + Integer.parseInt(m2));

        service.notify("endpoint1", "5");
        service.notify("endpoint2", "6");

        assertThat(future.get(), equalTo(11));
    }
}
