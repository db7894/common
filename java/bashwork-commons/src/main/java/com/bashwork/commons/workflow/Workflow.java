package com.bashwork.commons.workflow;

/**
 * Represents a single workflow that should be executed in response
 * to a new request.
 *
 * @param <TRequest> The request that the workflow is driven with.
 */
public interface Workflow<TRequest> {

    /**
     * Returns the request type of this workflow.
     *
     * @return The type of the workflow.
     */
    Class<TRequest> getType();

    /**
     * Returns the identifying name of this workflow.
     *
     * @return The name of the workflow
     */
    String getName();

    /**
     * Executes the workflow with the supplied request.
     *
     * @param request The request object the workflow runs with.
     */
    void execute(TRequest request);
}
