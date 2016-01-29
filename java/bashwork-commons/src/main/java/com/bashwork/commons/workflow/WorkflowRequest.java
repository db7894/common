package com.bashwork.commons.workflow;

import static com.bashwork.commons.utility.Validate.notEmpty;

import java.util.Objects;

import com.google.common.base.MoreObjects;

/**
 * Represents a single workflow request to be started. This
 * tracks the raw message, the type of workflow request, and
 * a tracker for the message that can be used to confirm that
 * the workflow has been handled.
 */
public final class WorkflowRequest {

    private final String message;
    private final String type;
    private final String tracker;

    private WorkflowRequest(Builder builder) {
        this.message = builder.message;
        this.type = builder.type;
        this.tracker = builder.tracker;
    }

    public String getMessage() {
        return message;
    }

    public String getType() {
        return type;
    }

    public String getTracker() {
        return tracker;
    }

    /**
     * Initialize a new builder used to create a new
     * WorkflowRequest instance.
     *
     * @return The initialized builder to build with.
     */
    public static Builder builder() {
        return new Builder();
    }

    public static final class Builder {
        private Builder() { }

        private String message;
        private String type;
        private String tracker;

        public Builder withMessage(String message) {
            this.message = message;
            return this;
        }

        public Builder withType(String type) {
            this.type = type;
            return this;
        }

        public Builder withTracker(String tracker) {
            this.tracker = tracker;
            return this;
        }

        public WorkflowRequest build() {
            notEmpty(message, "message");
            notEmpty(message, "type");
            notEmpty(message, "tracker");
            return new WorkflowRequest(this);
        }
    }

    @Override
    public int hashCode() {
        return Objects.hash(message, type, tracker);
    }

    @Override
    public String toString() {
        return MoreObjects.toStringHelper(this)
            .add("message", message)
            .add("tracker", tracker)
            .add("type", type)
            .toString();
    }

    @Override
    public boolean equals(Object other) {
        if (this == other) {
            return true;
        }
        if (other instanceof WorkflowRequest) {
            WorkflowRequest that = (WorkflowRequest) other;
            return Objects.equals(this.message, that.message)
                && Objects.equals(this.type, that.type)
                && Objects.equals(this.tracker, that.tracker);
        }

        return false;
    }
}
