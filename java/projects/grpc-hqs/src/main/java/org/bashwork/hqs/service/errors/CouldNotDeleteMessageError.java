package org.bashwork.hqs.service.errors;

import io.grpc.Status;
import io.grpc.StatusException;

/**
 * An Error that indicates a message was unable to be deleted.
 */
public final class CouldNotDeleteMessageError extends StatusException {

    public CouldNotDeleteMessageError(final String receiptHandle) {
        super(Status.NOT_FOUND.withDescription("could not delete the message associated with: " + receiptHandle));
    }
}
