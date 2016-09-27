package org.bashwork.guava;

import com.google.common.base.Function;
import com.google.common.base.Supplier;
import com.google.common.base.Suppliers;
import org.junit.Test;

import java.util.Random;
import java.util.concurrent.atomic.AtomicInteger;

import static org.hamcrest.Matchers.is;
import static org.junit.Assert.assertThat;

/**
 *
 */
public class SuppliersTest {

    @Test
    public void test_memoize_supplier() {
        final AtomicInteger counter = new AtomicInteger(5);
        final Supplier<Integer> supplier = counter::getAndIncrement;
        final Supplier<Integer> memoized = Suppliers.memoize(supplier);

        assertThat(memoized.get(), is(5));
        assertThat(memoized.get(), is(5));
        assertThat(supplier.get(), is(6));
        assertThat(memoized.get(), is(5));
    }

    @Test
    public void test_comopse_supplier() {
        final Supplier<Integer> supplier = Suppliers.ofInstance(5);
        final Supplier<String> composed = Suppliers.compose(Object::toString, supplier);
        assertThat(supplier.get(), is(5));
        assertThat(composed.get(), is("5"));
    }

    @Test
    public void test_supplier_function() {
        final Supplier<Integer> supplier = Suppliers.ofInstance(5);
        final Function<Supplier<Integer>, Integer> function = Suppliers.supplierFunction();

        assertThat(function.apply(supplier), is(5));
    }

    @Test
    public void test_synchronized_supplier() {
        final Random random = new Random(12345L);
        final Supplier<Integer> supplier = random::nextInt;
        final Supplier<Integer> threadsafe = Suppliers.synchronizedSupplier(supplier);

        assertThat(threadsafe.get(), is(1553932502));
    }
}
