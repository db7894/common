package org.bashwork.hqs.utility;

import static org.junit.Assert.assertThat;
import static org.hamcrest.CoreMatchers.*;

import org.junit.Test;

import java.util.function.Supplier;

/**
 * Code to test the functionality of the MoreObjects class.
 */
public class MoreObjectsTest {

    @Test
    public void first_positive_int_is_valid() {
        assertThat(MoreObjects.firstPositive(0, 2), is(2));
        assertThat(MoreObjects.firstPositive(-1, 2), is(2));
        assertThat(MoreObjects.firstPositive(1, 2), is(1));
    }

    @Test
    public void first_positive_long_is_valid() {
        assertThat(MoreObjects.firstPositive(0L, 2L), is(2L));
        assertThat(MoreObjects.firstPositive(-1L, 2L), is(2L));
        assertThat(MoreObjects.firstPositive(1L, 2L), is(1L));
    }

    @Test
    public void first_non_null_is_valid() {
        Object a = new Object();
        Object b = null;
        Object c = new Object();

        assertThat(MoreObjects.firstNonNull(a, b), is(a));
        assertThat(MoreObjects.firstNonNull(b, a), is(a));
        assertThat(MoreObjects.firstNonNull(a, c), is(a));
    }

    @Test
    public void first_non_null_supplier_is_valid() {
        Object a = new Object();
        Object b = null;
        Supplier<Object> c = () -> a;

        assertThat(MoreObjects.firstNonNull(a, c), is(a));
        assertThat(MoreObjects.firstNonNull(b, c), is(a));
    }
}
