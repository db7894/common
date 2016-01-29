package com.bashwork.commons.testing.model;

import com.google.common.base.MoreObjects;

import java.util.Objects;

/**
 * A simple type that can be used as a test event for
 * various messaging systems / serialization systems.
 */
public class TestPojo {

    private String name;
    private Integer age;

    public TestPojo() { }
    public TestPojo(String name, Integer age) {
        this.name = name;
        this.age = age;
    }

    public String getName() {
        return name;
    }

    public void setName(String value) {
        name = value;
    }

    public Integer getAge() {
        return age;
    }

    public void setAge(Integer value) {
        age = value;
    }

    @Override
    public int hashCode() {
        return Objects.hash(age, name);
    }

    @Override
    public boolean equals(Object object) {
        if (this == object) {
            return true;
        }
        if (object instanceof TestPojo) {
            TestPojo that = (TestPojo) object;
            return Objects.equals(this.age, that.age)
                && Objects.equals(this.name, that.name);
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
