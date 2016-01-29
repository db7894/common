package com.bashwork.commons.testing.supplier;

import com.bashwork.commons.producer.SpecificProducer;
import com.bashwork.commons.supplier.ConfirmedSupplier;
import com.bashwork.commons.supplier.testing.SupplierProducerPair;
import org.junit.Test;

import java.util.Optional;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.Future;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;

public class ConfirmedSupplierProducerPairTest {

    @Test
    public void default_producer_supplier() throws InterruptedException, ExecutionException {
        final ConfirmedSupplierProducerPair<String> pair = new ConfirmedSupplierProducerPair.Default<String>();
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
