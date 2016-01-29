package com.bashwork.commons.testing.supplier;

import com.bashwork.commons.producer.SpecificProducer;
import org.junit.Test;

import java.util.concurrent.ExecutionException;
import java.util.concurrent.Future;
import java.util.function.Supplier;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;

public class SupplierProducerPairTest {

    @Test
    public void default_producer_supplier() throws InterruptedException, ExecutionException {
        final SupplierProducerPair<String> pair = new SupplierProducerPair.Default<String>();
        final SpecificProducer<String> producer = pair.getProducer();
        final Supplier<String> supplier = pair.getSupplier();

        final String message = "test message";
        final String tracker = producer.produce(message);
        final String result = supplier.get();
        final String missing = supplier.get();
        final Future<String> future = pair.getFuture(tracker);

        assertThat(missing, equalTo(null));
        assertThat(result, equalTo(message));
        assertThat(future.get(), equalTo(message));
    }
}
