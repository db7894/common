package org.bashwork.hqs.service.errors;

import io.grpc.Status;
import io.grpc.StatusRuntimeException;

/**
 * An Error that indicates there was a problem with the request.
 */
public final class InvalidRequestError extends StatusRuntimeException {

    public InvalidRequestError(final String field) {
        super(Status.INVALID_ARGUMENT.withDescription("the value for " + field + " is invalid"));
    }
}
