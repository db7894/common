package com.bashwork.commons.workflow;

import static com.bashwork.commons.utility.Validate.notNull;

import java.util.Map;
import java.util.Optional;

import com.bashwork.commons.supplier.ConfirmedSupplier;
import com.google.common.util.concurrent.AbstractExecutionThreadService;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * This is a service that can be run as a thread. It safely
 * pulls new requests from the supplier and then dispatches
 * the workflow steps to an underlying executor.
 */
public class WorkflowService extends AbstractExecutionThreadService {

    private static final Logger logger = LogManager.getLogger(WorkflowService.class);

    private final ConfirmedSupplier<WorkflowRequest> supplier;
    private final Map<String, WorkflowExecutor<?>> workflows;

    /**
     * Initialize a new instance of the WorkflowService class.
     *
     * @param supplier The supplier to pull new work item requests from.
     * @param workflows The map of workflows to handle.
     */
    public WorkflowService(ConfirmedSupplier<WorkflowRequest> supplier,
        Map<String, WorkflowExecutor<?>> workflows) {

        this.workflows = notNull(workflows, "Workflows");
        this.supplier = notNull(supplier, "supplier");
    }

    /**
     * @see Thread#run()
     */
    @Override
    protected void run() throws Exception {
        logger.debug("workflow service starting polling loop");
        while (isRunning()) {
            Optional<WorkflowRequest> request = supplier.get();
            if (handle(request)) {
                supplier.confirm(request.get());
            }
        }
    }

    /**
     * Handle the newly supplied message from the supplier.
     * This tries to find a valid workflow and then run the
     * execution.
     *
     * @param message The new message to attempt to handle
     * @return true if the message was handled successfully, false otherwise
     */
    private boolean handle(Optional<WorkflowRequest> message) {
        boolean isSuccessful = false;

        if (message.isPresent()) {
            final WorkflowRequest request = message.get();
            final WorkflowExecutor<?> workflow = workflows.get(request.getType());

            if (workflow != null) {
                try {
                    logger.debug("handling new workflow message: {}", request);
                    workflow.execute(request.getMessage());
                    isSuccessful = true;
                } catch (Exception ex) {

                    /**
                     * We need to prevent the worker service from going down, as
                     * such we will swallow all exceptions up to this point which
                     * will simply force the current workflow item to be retried.
                     */
                    logger.error("exception in the workflow executor", ex);
                }
            } else {
                logger.error("recieved invalid workflow message: {}", request);
            }
        } else {
            logger.trace("workflow service received empty message");
        }

        return isSuccessful;
    }
}
