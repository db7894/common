package org.bashwork.guava;

import static org.junit.Assert.assertThat;
import static org.junit.Assert.assertTrue;
import static org.junit.Assert.assertFalse;
import static org.hamcrest.Matchers.is;


import com.google.common.collect.Lists;
import com.google.common.collect.Ordering;
import org.junit.Test;
import java.util.Objects;
import com.google.common.base.Preconditions;
import com.google.common.base.MoreObjects;
import com.google.common.collect.ComparisonChain;

public final class MoreObjectsTest {

    public static final class Person implements Comparable<Person> {
        private final String name;
        private final Integer age;

        public Person(final String name, final Integer age) {
            this.name = Preconditions.checkNotNull(name);
            this.age  = Preconditions.checkNotNull(age);
        }

        @Override
        public int compareTo(final Person that) {
            return ComparisonChain.start()
                .compare(this.name, that.name)
                .compare(this.age, that.age)
                .result();
        }

        @Override
        public int hashCode() {
            return Objects.hash(name, age);
        }

        @Override
        public boolean equals(final Object other) {
            if (other instanceof Person) {
                final Person that = (Person)other;
                return Objects.equals(this.name, that.name)
                    && Objects.equals(this.age, that.age);
            }
            return false;
        }

        @Override
        public String toString() {
            return MoreObjects.toStringHelper(this)
                .add("name", name)
                .add("age", age)
                .toString();
        }
    }

    @Test
    public void test_object_methods() {
        final Person person = new Person("Ricky", 25);
        final Person person2 = new Person("Ricky", 26);

        assertThat(person.toString(), is("Person{name=Ricky, age=25}"));
        assertThat(person.compareTo(person), is(0));
        assertThat(person.compareTo(person2), is(-1));
        assertThat(person2.compareTo(person), is(1));

        assertTrue(person.equals(person));
        assertFalse(person.equals(null));
        assertFalse(person.equals(person2));
    }

    @Test
    public void test_ordering_methods() {
        final Ordering<Person> ordering = Ordering.natural();
        final Person p1 = new Person("Ricky", 25);
        final Person p2 = new Person("Ricky", 26);

        assertThat(ordering.max(p1, p2), is(p2));
        assertThat(ordering.min(p1, p2), is(p1));
        assertThat(ordering.reversed().compare(p1, p2), is(1));
        assertThat(ordering.compare(p1, p2), is(-1));
        assertThat(ordering.isOrdered(Lists.newArrayList(p1, p2)), is(true));
    }

    /**
     * Works by using an index map such that { key -> counter } and then it simply
     * subtracts the looked up values of each key.
     */
    @Test
    public void test_explicit_ordering() {
        final Ordering<Integer> ordering = Ordering.explicit(1, 5, 3, 6, 7);
        assertThat(ordering.isOrdered(Lists.newArrayList(1, 5, 3, 6, 7)), is(true));
        assertThat(ordering.isOrdered(Lists.newArrayList(1, 3, 5, 6, 7)), is(false));
    }
}
