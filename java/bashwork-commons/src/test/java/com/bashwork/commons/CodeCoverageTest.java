package com.bashwork.commons;

import static org.junit.Assert.assertNotNull;

import java.lang.reflect.Constructor;
import java.util.ArrayList;
import java.util.List;

import com.bashwork.commons.serialize.Serializers;
import org.junit.Test;

import com.bashwork.commons.filesystem.Filesystems;
import com.bashwork.commons.notification.NotifierServices;
import com.bashwork.commons.producer.Producers;
import com.bashwork.commons.supplier.Suppliers;
import com.bashwork.commons.supplier.sqs.SqsAttribute;
import com.bashwork.commons.utility.Validate;

public class CodeCoverageTest {

    private static final List<Class<?>> classes = new ArrayList<>();
    static {
        classes.add(Producers.class);
        classes.add(Serializers.class);
        classes.add(Suppliers.class);
        classes.add(NotifierServices.class);
        classes.add(Filesystems.class);
        classes.add(SqsAttribute.class);
        classes.add(Validate.class);
    }

    @Test
    public void test_private_constructors() {
        classes.forEach(CodeCoverageTest::create_private_instance_of);
    }

    /**
     * This reflectively creates a private instance of a given type
     * by finding the constructor and making it accessible.
     *
     * @param klass The class to create an instance of.
     */
    private static <T> void create_private_instance_of(Class<T> klass) {
        try {
            Constructor<T> constructor = klass.getDeclaredConstructor();
            constructor.setAccessible(true);
            T instance = constructor.newInstance();

            assertNotNull(instance);
        } catch (Exception ex) {
            throw new RuntimeException(ex);
        }
    }
}
