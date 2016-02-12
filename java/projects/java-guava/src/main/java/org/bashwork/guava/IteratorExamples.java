package org.bashwork.guava;

import java.util.Comparator;
import java.util.Iterator;
import java.util.function.Function;

import com.google.common.collect.Iterators;
import com.google.common.collect.AbstractIterator;
import com.google.common.collect.AbstractSequentialIterator;
import com.google.common.collect.PeekingIterator;

/**
 * Iterator.addAll
 * Iterator.advance
 * Iterator.any
 * Iterator.asEnumeration
 * Iterator.concat
 * Iterator.consumingIterator
 * Iterator.contains
 * Iterator.cycle
 * Iterator.elementsEqual
 * Iterator.filter
 * Iterator.find
 * Iterator.forArray
 * Iterator.frequency
 * Iterator.transform
 */
public final class IteratorExamples {
    private IteratorExamples() { }

    /**
     * Given a sorted iterator, return a new iterator where the sequantial
     * duplicates are skipped.
     *
     * @param iterator The iterator to skip duplicates on.
     * @return An iterator where the duplicates are skipped.
     */
    public static <T> Iterator<T> skipDuplicates(final Iterator<T> iterator) {
        final PeekingIterator<T> peeker = Iterators.peekingIterator(iterator);
        return new AbstractIterator<T>() {
            protected T computeNext() {
                if (peeker.hasNext()) {
                    final T next = peeker.next();
                    while (peeker.hasNext()
                        && peeker.peek().equals(next)) {
                        peeker.next();
                    }
                    return next;
                }
                return endOfData();
            }
        };
    }

    public static <T> Iterator<T> of(final T value) {
        return new AbstractIterator<T>() {
            protected T computeNext() {
                return value;
            }
        };
    }

    public static <T> Iterator<T> infinite(final T initial, final Function<T, T> mapper) {
        return new AbstractSequentialIterator<T>(initial) {
            protected T computeNext(T previous) {
                return mapper.apply(previous);
            }
        };
    }

    /**
     * Create an iterator that will start counting up from
     * the supplied value.
     *
     * @param start The number to start counting from
     * @return A counting up iterator.
     */
    public static Iterator<Integer> countUp(int start) {
        return infinite(start, next -> next + 1);
    }

    /**
     * Create an iterator that will start counting down from
     * the supplied value.
     *
     * @param start The number to start counting from
     * @return A counting down iterator.
     */
    public static Iterator<Integer> countDown(int start) {
        return infinite(start, next -> next - 1);
    }

    /**
     * Given an iterator, check during usage that it indeed is in order,
     * otherwise throw an {@link AssertionError}.
     *
     * @param iterator The iterator to check for order.
     * @param comparator The comparator to use for checking order.
     * @param <E> The type of the value being iterated.
     * @return The order checking iterator.
     */
    public static <E> Iterator<E> orderChecking(final Iterator<E> iterator,
        final Comparator<E> comparator) {

        final PeekingIterator<E> peeker = Iterators.peekingIterator(iterator);
        return new AbstractIterator<E>() {
            @Override
            protected E computeNext() {
                if (peeker.hasNext()) {
                    E next = peeker.next();
                    if (peeker.hasNext()) {
                        assert comparator.compare(next, peeker.peek()) <= 0;
                    }
                    return next;
                }
                return endOfData();
            }
        };
    }

    /**
     * Given an iterator, check during usage that it indeed is in order,
     * otherwise throw an {@link AssertionError}.
     *
     * @param iterator The iterator to check for order.
     * @param <E> The type of the value being iterated (must be Comparable).
     * @return The order checking iterator.
     */
    public static <E extends Comparable<E>> Iterator<E> orderChecking(final Iterator<E> iterator) {
        final PeekingIterator<E> peeker = Iterators.peekingIterator(iterator);
        return new AbstractIterator<E>() {
            @Override
            protected E computeNext() {
                if (peeker.hasNext()) {
                    E next = peeker.next();
                    if (peeker.hasNext()) {
                        assert (next.compareTo(peeker.peek()) <= 0);
                    }
                    return next;
                }
                return endOfData();
            }
        };
    }
}
