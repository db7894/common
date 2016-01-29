package com.bashwork.commons.testing.model;

import org.junit.Test;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;

/**
 * A collection of tests for the TestPojo class.
 */
public class TestPojoTest {

    public static final String NAME = "User Name";
    public static final int AGE = 21;
    public static final TestPojo POJO = new TestPojo(NAME, AGE);

    @Test
    public void test_pojo_methods() {
        TestPojo copy = new TestPojo();
        copy.setAge(AGE);
        copy.setName(NAME);

        assertThat(POJO, equalTo(copy));
        assertThat(POJO.getAge(), equalTo(copy.getAge()));
        assertThat(POJO.getName(), equalTo(copy.getName()));
        assertThat(POJO.hashCode(), equalTo(copy.hashCode()));
        assertThat(POJO.toString(), equalTo(copy.toString()));
    }
}
