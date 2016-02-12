package com.bashwork.commons.serialize;

import java.util.Objects;

import com.google.common.base.MoreObjects;

public class TestPojo {

    private String name;
    private Integer age;

    public TestPojo() {}
    public TestPojo(String name, Integer age) {
        this.name = name;
        this.age = age;
    }

    public String getName() { return name; }
    public void setName(String value) {
        name = value;
    }

    public Integer getAge() { return age; }
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
            TestPojo that = (TestPojo)object;
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
