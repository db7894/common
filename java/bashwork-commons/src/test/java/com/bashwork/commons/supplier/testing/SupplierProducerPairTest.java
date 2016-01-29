package com.bashwork.commons.supplier.testing;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;

import java.util.Optional;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.Future;

import org.junit.Test;

import com.bashwork.commons.producer.SpecificProducer;
import com.bashwork.commons.supplier.ConfirmedSupplier;

public class SupplierProducerPairTest {

    @Test
    public void default_producer_supplier() throws InterruptedException, ExecutionException {
        final SupplierProducerPair<String> pair = new SupplierProducerPair.Default<String>();
        final SpecificProducer<String> producer = pair.getProducer(String.class);
        final ConfirmedSupplier<String> supplier = pair.getSupplier();

        final String message = "test message";
        final String tracker = producer.produce(message);
        final Optional<String> result = supplier.get();
        final Optional<String> missing = supplier.get();
        final Future<String> future = pair.getFuture(tracker);

        supplier.confirm(result.get());
        supplier.confirm(message); // double confirm is okay

        assertThat(missing.isPresent(), equalTo(false));
        assertThat(result.isPresent(), equalTo(true));
        assertThat(result.get(), equalTo(message));
        assertThat(future.get(), equalTo(message));
    }
}
