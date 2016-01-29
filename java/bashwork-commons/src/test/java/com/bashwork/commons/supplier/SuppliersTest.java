package com.bashwork.commons.supplier;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;

import java.util.Collections;
import java.util.List;
import java.util.Optional;

import org.junit.Test;

public class SuppliersTest {

    private static final String value = "value";
    private static final Optional<String> optional = Optional.of(value);
    private static final Optional<String> empty = Optional.empty();
    private static final List<String> values = Collections.singletonList(value);

    @Test
    public void supplier_of_constant() {
        ConfirmedSupplier<String> supplier = Suppliers.of(value);
        assertThat(supplier.get(), equalTo(optional));
        assertThat(supplier.get(), equalTo(optional));
    }

    @Test
    public void supplier_of_iterable() {
        ConfirmedSupplier<String> supplier = Suppliers.of(values);
        assertThat(supplier.get(), equalTo(optional));
        assertThat(supplier.get(), equalTo(empty));
    }

    @Test
    public void supplier_of_iterator() {
        ConfirmedSupplier<String> supplier = Suppliers.of(values.iterator());
        assertThat(supplier.get(), equalTo(optional));
        assertThat(supplier.get(), equalTo(empty));
    }

    @Test
    public void supplier_of_supplier() {
        ConfirmedSupplier<String> supplier = Suppliers.toConfirmed(() -> value);
        supplier.confirm("this does nothing");
        assertThat(supplier.get(), equalTo(optional));
    }
}
