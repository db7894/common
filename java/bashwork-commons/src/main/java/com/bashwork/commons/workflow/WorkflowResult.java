package com.bashwork.commons.workflow;

/**
 * Represents the result of a workflow operation along with
 * the request that caused it to be executed.
 *
 * @param <TRequest> The type of the original request.
 */
public final class WorkflowResult<TRequest> {

    private final TRequest request;
    private final WorkflowStatus status;
    private final String message;

    private WorkflowResult(TRequest request, WorkflowStatus status, String message) {
        this.request = request;
        this.status = status;
        this.message = message;
    }

    /**
     * Generate a success result for the workflow.
     *
     * @param request The original request this responds to.
     * @param <T> The type of the original request.
     * @return The newly created WorkflowResult.
     */
    public static <T> WorkflowResult<T> success(T request) {
        return new WorkflowResult<T>(request, WorkflowStatus.SUCCESS, "success");
    }

    /**
     * Generate a failures result for the workflow.
     *
     * @param request The original request this responds to.
     * @param ex The exception that caused the workflow to fail.
     * @param <T> The type of the original request.
     * @return The newly created WorkflowResult.
     */
    public static <T> WorkflowResult<T> failure(T request, Throwable ex) {
        return new WorkflowResult<T>(request, WorkflowStatus.FAILURE, ex.getMessage());
    }

    public TRequest getRequest() {
        return request;
    }

    public WorkflowStatus getStatus() {
        return status;
    }

    public String getMessage() {
        return message;
    }
}
