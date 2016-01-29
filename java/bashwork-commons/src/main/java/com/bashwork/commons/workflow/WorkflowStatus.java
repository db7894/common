package com.bashwork.commons.workflow;

/**
 * An enumeration that can be used to specify the resulting
 * state of a given workflow.
 */
public enum WorkflowStatus {

    /**
     * The workflow has completely succeeded with no failures.
     */
    SUCCESS,

    /**
     * The workflow has completely failed with no successes.
     */
    FAILURE;
}
