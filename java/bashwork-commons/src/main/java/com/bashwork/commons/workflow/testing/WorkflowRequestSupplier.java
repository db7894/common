package com.bashwork.commons.workflow.testing;

import java.time.Duration;
import java.util.Optional;
import java.util.UUID;

import com.bashwork.commons.serialize.Serializer;
import com.bashwork.commons.serialize.string.JsonStringSerializer;
import com.bashwork.commons.supplier.testing.SupplierProducerPair;
import com.bashwork.commons.workflow.WorkflowRequest;

/**
 * This is a simple producer / supplier pair that takes in some
 * message type and converts it into a valid WorkflowRequest.
 */
public class WorkflowRequestSupplier extends SupplierProducerPair<WorkflowRequest> {

    private static final long POLL_DELAY = Duration.ofSeconds(1).toMillis();
    private static final Serializer<String> SERIALIZER = new JsonStringSerializer();

    @Override
    public <T> WorkflowRequest toSupply(T message) {
        return WorkflowRequest.builder()
            .withMessage(SERIALIZER.serialize(message))
            .withTracker(UUID.randomUUID().toString())
            .withType(message.getClass().getName())
            .build();
    }

    @Override
    public Optional<WorkflowRequest> get() {
        Optional<WorkflowRequest> supply = super.get();

        /**
         * This is overridden as the poller can run through thousands
         * of requests before the first test method is called. This
         * just slows it down a bit and prevents the logs from being
         * swamped.
         */
        if (!supply.isPresent()) {
            try {
                Thread.sleep(POLL_DELAY);
            } catch (InterruptedException ex) {
                Thread.interrupted();
            }
        }
        return supply;
    }

    @Override
    protected String toTracker(WorkflowRequest message) {
        return message.getTracker();
    }
}
