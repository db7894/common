package org.bashwork.hqs.service.errors;

import io.grpc.Status;
import io.grpc.StatusException;

/**
 * An Error that indicates that the requested queue does not exist.
 */
public final class QueueDoesNotExistError extends StatusException {

    public QueueDoesNotExistError(final String queueUrl) {
        super(Status.NOT_FOUND.withDescription("the requested queue does not exist: " + queueUrl));
    }
}