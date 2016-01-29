package com.bashwork.commons.workflow.testing;

import java.util.Collections;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.Future;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;

import com.bashwork.commons.workflow.Workflow;
import com.bashwork.commons.workflow.WorkflowManager;

/**
 * This is the start of a base framework for testing workflows.
 */
public abstract class BaseWorkflowTest {

    /**
     * Given a request and a workflow, create a workflow manager that will
     * run the supplied workflow with the given request. This will run the
     * workflow until the message is supplied and then shut down the workflow
     * system.
     *
     * @param request The request to execute.
     * @param workflow The workflow to execute the request with.
     * @param verifier The verification step to run on the workflow result.
     * @param <T> The underlying type of the workflow.
     */
    protected static <T> void executeWorkflow(T request, Workflow<T> workflow,
        Future<Void> verifier) {

        final long minutes = 1;
        final TimeUnit unit = TimeUnit.MINUTES;
        final WorkflowRequestSupplier supplier = new WorkflowRequestSupplier();
        final String tracker = supplier.produce(request);
        final WorkflowManager manager = WorkflowManager.builder()
            .withDefaultSerializer()
            .withWorkers(1)
            .withSupplier(supplier)
            .withWorkflows(Collections.singletonList(workflow))
            .build();

        try {
            manager.start();

            /**
             * This checks that the message that was confirmed was indeed the message
             * that we put on the queue. This should execute almost immediately unless
             * something is wrong, hence the timeout to prevent an infinite stall.
             */
            final String tracked = supplier.getFuture(tracker)
                .get(minutes, unit)
                .getTracker();

            assert (tracked.equals((tracker)));

            /**
             * This actually runs the underlying unit test asynchronously. Since we
             * registered against the notifier's future, it will be run when the
             * workflow performs the notification operation. Like the check before,
             * this should execute immediately unless something is wrong.
             */
            verifier.get(minutes, unit);

        } catch (InterruptedException ex) {

            /**
             * If waiting for the future is interrupted, restore the interrupted
             * status and throw to let the test fail.
             */
            Thread.currentThread().interrupt();
            throw new RuntimeException(ex.getMessage());

        } catch (ExecutionException | TimeoutException ex) {

            /**
             * If anything about the framework goes wrong, we simply fail the test.
             * This includes: workflow exception, deadlock, race-conditions, timeouts,
             * etc.
             */
            throw new RuntimeException(ex.getMessage());

        } finally {

            /**
             * This will always eventually stop all the threads. The only thing
             * that can prevent this from completing is an infinite workflow loop.
             */
            manager.stop();
        }
    }
}
