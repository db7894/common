package org.bashwork.hqs.service.errors;

import io.grpc.Status;
import io.grpc.StatusRuntimeException;

/**
 * An Error that indicates a message was unable to be sent.
 */
public final class CouldNotSendMessageError extends StatusRuntimeException {

    public CouldNotSendMessageError(final String messageBody) {
        super(Status.INTERNAL.withDescription("could not send the message with the body: " + messageBody));
    }
}
