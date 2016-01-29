package com.bashwork.commons.workflow;

import static com.bashwork.commons.utility.Validate.notNull;

import com.bashwork.commons.serialize.SpecificSerializer;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * A workflow wrapper that if initialized can take a
 * collection of strongly typed utilities and generate
 * a workflow executor that can run a given message.
 *
 * @param <TRequest> The request that the workflow is driven with.
 */
public class WorkflowExecutor<TRequest> {

    private static final Logger logger = LogManager.getLogger(WorkflowExecutor.class);

    private final Workflow<TRequest> workflow;
    private final SpecificSerializer<String, TRequest> serializer;

    /**
     * Create a new instance of the WorkflowExecutor
     *
     * @param workflow The workflow to execute until completion.
     * @param serializer The serializer used to deserialize new messages.
     */
    public WorkflowExecutor(Workflow<TRequest> workflow,
        SpecificSerializer<String, TRequest> serializer) {

        this.workflow = notNull(workflow, "Workflow");
        this.serializer = notNull(serializer, "Serializer");
    }

    /**
     * Given a new message, run the workflow until it completes.
     *
     * @param message The message that the workflow should handle.
     */
    public void execute(String message) {

        final TRequest request = serializer.deserialize(message);
        logger.debug("starting {} workflow with: {}", workflow.getName(), request);

        /**
         * As of now we will simply run the request directly in the worker thread
         * that is being used to poll the supplier. If we switch to using the blocking
         * IO executor, we MUST be careful not to create a situation where all the worker
         * threads are used to wait on child tasks.
         */
        workflow.execute(request);
    }
}
