package com.bashwork.commons.utility;

import org.junit.Test;

import java.io.IOException;
import java.util.function.Consumer;
import java.util.function.Function;
import java.util.function.Predicate;
import java.util.function.Supplier;

import static org.hamcrest.Matchers.is;
import static org.junit.Assert.assertThat;
import static com.bashwork.commons.utility.Unchecked.unchecked;

public class UncheckedTest {

    private static final String STRING = "string";

    @Test(expected = IOException.class)
    public void test_unchecked_predicate_throws() {
        unchecked((Unchecked.UncheckedPredicate<String>)(value) -> {
            throw new IOException();
        }).test(STRING);
    }

    @Test(expected = IOException.class)
    public void test_unchecked_predicate_throws_interface() {
        final Unchecked.UncheckedPredicate<String> unchecked = (value) -> {
            throw new IOException();
        };
        final Predicate<String> function = unchecked;
        function.test(STRING);
    }

    @Test
    public void test_unchecked_predicate_does_not_throw() {
        final boolean result = unchecked((String value) -> {
            return new String(value.getBytes("UTF-8")).equals(value); // throws UnsupportedEncodingException
        }).test(STRING);

        assertThat(result, is(true));
    }

    @Test(expected = IOException.class)
    public void test_unchecked_supplier_throws() {
        unchecked((Unchecked.UncheckedSupplier<String>)() -> {
            throw new IOException();
        }).get();
    }

    @Test(expected = IOException.class)
    public void test_unchecked_supplier_throws_interface() {
        final Unchecked.UncheckedSupplier<String> unchecked = () -> {
            throw new IOException();
        };
        final Supplier<String> function = unchecked;
        function.get();
    }

    @Test
    public void test_unchecked_supplier_does_not_throw() {
        final String result = unchecked(() -> {
            return new String(STRING.getBytes("UTF-8")); // throws UnsupportedEncodingException
        }).get();

        assertThat(result, is(STRING));
    }

    @Test(expected = IOException.class)
    public void test_unchecked_function_throws() {
        unchecked((Unchecked.UncheckedFunction<String, String>)(value) -> {
            throw new IOException();
        }).apply(STRING);
    }

    @Test(expected = IOException.class)
    public void test_unchecked_function_throws_interface() {
        final Unchecked.UncheckedFunction<String, String> unchecked = (value) -> {
            throw new IOException();
        };
        final Function<String, String> function = unchecked;
        function.apply(STRING);
    }

    @Test
    public void test_unchecked_function_does_not_throw() {
        final String result = unchecked((String value) -> {
            return new String(value.getBytes("UTF-8")); // throws UnsupportedEncodingException
        }).apply(STRING);

        assertThat(result, is(STRING));
    }

    @Test(expected = IOException.class)
    public void test_unchecked_consumer_throws() {
        unchecked((Unchecked.UncheckedConsumer<String>)(value) -> {
            throw new IOException();
        }).accept(STRING);
    }

    @Test(expected = IOException.class)
    public void test_unchecked_consumer_throws_interface() {
        final Unchecked.UncheckedConsumer<String> unchecked = (value) -> {
            throw new IOException();
        };
        final Consumer<String> function = unchecked;
        function.accept(STRING);
    }

    @Test
    public void test_unchecked_consumer_does_not_throw() {
        unchecked((String value) -> {
            new String(value.getBytes("UTF-8")); // throws UnsupportedEncodingException
        }).accept(STRING);
    }
}
