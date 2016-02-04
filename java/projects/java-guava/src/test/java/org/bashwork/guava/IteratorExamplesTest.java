package org.bashwork.guava;

import static org.junit.Assert.assertThat;
import static org.hamcrest.Matchers.is;

import org.junit.Test;
import java.util.Comparator;
import java.util.List;
import java.util.Iterator;
import com.google.common.collect.Lists;
import com.google.common.collect.Iterators;

public final class IteratorExamplesTest {

    @Test
    public void test_skip_duplicates() {
        final List<Integer> list = Lists.newArrayList(1, 1, 2, 2, 3, 3);
        final Iterator<Integer> iterator = IteratorExamples.skipDuplicates(list.iterator());
        final List<Integer> expect = Lists.newArrayList(1, 2, 3);
        final List<Integer> actual = Lists.newArrayList(iterator); 

        assertThat(actual, is(expect));
    }

    @Test
    public void test_order_checking() {
        final List<Integer> list = Lists.newArrayList(1, 1, 2, 2, 3, 3);
        final Iterator<Integer> iterator = IteratorExamples.orderChecking(list.iterator());
        final List<Integer> actual = Lists.newArrayList(iterator); 
    }

    @Test(expected=AssertionError.class)
    public void test_order_checking_fails() {
        final List<Integer> list = Lists.newArrayList(1, 1, 2, 4, 3, 2);
        final Iterator<Integer> iterator = IteratorExamples.orderChecking(list.iterator());
        final List<Integer> actual = Lists.newArrayList(iterator); 
    }

    @Test
    public void test_order_checking_comparator() {
        final Comparator<Integer> comparator = (a, b) -> a.compareTo(b);
        final List<Integer> list = Lists.newArrayList(1, 1, 2, 2, 3, 3);
        final Iterator<Integer> iterator = IteratorExamples.orderChecking(list.iterator(), comparator);
        final List<Integer> actual = Lists.newArrayList(iterator); 
    }

    @Test(expected=AssertionError.class)
    public void test_order_checking_fails_comparator() {
        final Comparator<Integer> comparator = (a, b) -> a.compareTo(b);
        final List<Integer> list = Lists.newArrayList(1, 1, 2, 4, 3, 2);
        final Iterator<Integer> iterator = IteratorExamples.orderChecking(list.iterator(), comparator);
        final List<Integer> actual = Lists.newArrayList(iterator); 
    }

    @Test
    public void test_count_up() {
        final Iterator<Integer> iterator = Iterators.limit(IteratorExamples.countUp(0), 6);
        final List<Integer> expect = Lists.newArrayList(0, 1, 2, 3, 4, 5);
        final List<Integer> actual = Lists.newArrayList(iterator); 

        assertThat(actual, is(expect));
    }

    @Test
    public void test_count_down() {
        final Iterator<Integer> iterator = Iterators.limit(IteratorExamples.countDown(5), 6);
        final List<Integer> expect = Lists.newArrayList(5, 4, 3, 2, 1, 0);
        final List<Integer> actual = Lists.newArrayList(iterator); 

        assertThat(actual, is(expect));
    }
}
