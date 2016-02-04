package org.bashwork.guava;

import static org.junit.Assert.assertThat;
import static org.junit.Assert.assertTrue;
import static org.junit.Assert.assertFalse;
import static org.hamcrest.Matchers.is;


import org.junit.Test;
import java.util.Comparator;
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
}
