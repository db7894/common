package com.bashwork.commons.workflow;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Callable;
import java.util.concurrent.CompletionService;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorCompletionService;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Future;

import org.apache.logging.log4j.LogManager;

/**
 * A base class for helping to write workflows where the original
 * request should be split into a number of smaller requests.
 */
public abstract class SplitWorkflow<TRequest> implements Workflow<TRequest> {

    static final org.apache.logging.log4j.Logger logger = LogManager.getLogger(SplitWorkflow.class);
    private static final Void UNUSED_CONTEXT = null;

    private final ExecutorService executor;

    /**
     * Create a new instance of the SplitWorkflow class.
     *
     * @param executor The executor to run the child tasks on.
     */
    public SplitWorkflow(ExecutorService executor) {
        this.executor = executor;
    }

    /**
     * Callback for creating child requests from the parent request.
     *
     * @param request The original request to possibly split.
     * @return The split child requests.
     */
    public abstract List<TRequest> onSplit(TRequest request);

    /**
     * Callback for when all the child tasks have completed.
     *
     * @param request The original request.
     * @param results The resulting completed child tasks.
     * @throws NonRetryableException If this task should be retried.
     * @throws RetryableException If this task should fail immediately.
     */
    public abstract void onComplete(TRequest request, List<WorkflowResult<TRequest>> results)
        throws NonRetryableException, RetryableException;;

        /**
         * Callback for when a single child task is to be executed.
         *
         * @param request The child task to execute.
         * @throws NonRetryableException If this task should be retried.
         * @throws RetryableException If this task should fail immediately.
         */
        public abstract void onExecute(TRequest request)
            throws NonRetryableException, RetryableException;

        /**
         * @see Workflow#execute(Object)
         */
        @Override
        public void execute(TRequest request) {
            final List<TRequest> splits = onSplit(request);
            final List<Future<WorkflowResult<TRequest>>> futures = new ArrayList<>(splits.size());
            final List<WorkflowResult<TRequest>> results = new ArrayList<>(splits.size());
            final CompletionService<WorkflowResult<TRequest>> service = new ExecutorCompletionService<>(executor);

            try {
                for (TRequest split : splits) {
                    futures.add(service.submit(new BaseWorkflowCallable(split)));
                }

                for (int count = 0; count < splits.size(); ++count) {
                    try {
                        results.add(service.take().get());
                    } catch (InterruptedException ex) {
                        logger.error("interruption thrown in the workflow child task", ex);
                        Thread.currentThread().interrupt(); // reset the interrupt status
                    } catch (ExecutionException ex) {
                        logger.error("exception thrown in the workflow child task", ex);
                    } catch (RuntimeException | Error er) {
                        logger.error("unhandled error/exception thrown in the workflow child task", er);
                        throw er;
                    }
                }

                /**
                 * TODO should we retry N times
                 * The complete call should ideally always work. If it does not, there
                 * is very little that will make it immediately work again as this task
                 * should be little more than an SNS publish.
                 */
                try {
                    onComplete(request, results);
                } catch (NonRetryableException | RetryableException ex) {
                    logger.error("execption thrown in workflow complete", ex);
                } catch (RuntimeException | Error er) {
                    logger.error("unhandled error/exception thrown in the workflow child task", er);
                    throw er;
                }

            } finally {
                for (Future<WorkflowResult<TRequest>> future : futures) {
                    future.cancel(true);
                }
            }
        }

        /**
         * @see Workflow#getName()
         */
        @Override
        public String getName() {
            return getType().getName();
        }

        /**
         * A wrapper around calling a single workflow instance. This takes
         * care of handling the retry policy for the workflow and correctly
         * generating the response messages.
         */
        private class BaseWorkflowCallable implements Callable<WorkflowResult<TRequest>> {

            private final TRequest request;

            public BaseWorkflowCallable(TRequest request) {
                this.request = request;
            }

            @Override
            public WorkflowResult<TRequest> call() {
                int failures = 0;
                long started = System.currentTimeMillis();

                while (true) {
                    try {
                        onExecute(request);
                        return WorkflowResult.success(request);
                    } catch (RuntimeException | RetryableException | NonRetryableException ex) {
                        // TODO add retry logic here
//                        long delay = strategy.nextDelay(started, ++failures, ex, UNUSED_CONTEXT);
//                        if (delay < 0) {
//                            return WorkflowResult.failure(request, ex);
//                        }
//
//                        try {
//                            Thread.sleep(delay);
//                        } catch (InterruptedException ex2) {
//                            Thread.currentThread().interrupt();
//                            return WorkflowResult.failure(request, ex2);
//                        }
                    }
                }
            }
        }
}
