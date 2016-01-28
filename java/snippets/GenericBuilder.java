package org.bashwork.commons;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.function.Function;
import java.util.stream.Collectors;

/**
 * Trying to make a DSL for a generic builder for any complex
 * type. To get generic `with[Field]` methods, we (hack) use
 * the getters for the immutable created type. For the backing
 * fields in the builder, we just store them in a map keyed off
 * by the (hack) getter methods (hopefully they will hash to the
 * same location).
 */
public class GenericBuilder {

    /**
     * The type used to construct a builder which can then be used
     * to create an instance of the requested type.
     *
     * @param <T> The type to create
     */
    static class BuilderBuilder<T> {
        private final List<Function<T, ?>> setters = new ArrayList<>();
        private final Function<Constructor<T>, T> constructor;

        /**
         * Reflection based BuilderBuilder
         * @param klass
         * @param <T>
         * @return
         */
        public static <T> BuilderBuilder<T> of(Class<T> klass) {
            try {
                java.lang.reflect.Constructor<T> constructor = klass.getConstructor(Constructor.class);
                constructor.setAccessible(true);
                return new BuilderBuilder<>(builder -> {
                    try {
                        return (T)constructor.newInstance(builder);
                    } catch (Exception ex) {
                        throw new RuntimeException(ex);
                    }
                });
            } catch (NoSuchMethodException ex) {
                throw new RuntimeException(ex);
            }
        }

        /**
         * Supplier based BuilderBuilder
         * @param constructor
         * @param <T>
         * @return
         */
        public static <T> BuilderBuilder<T> of(Function<Constructor<T>, T> constructor) {
            return new BuilderBuilder<>(constructor);
        }

        private BuilderBuilder(Function<Constructor<T>, T> constructor) {
            this.constructor = constructor;
        }

        public <V> BuilderBuilder<T> with(Function<T, V> getter) {
            // TODO add type validation composition / stacking / adapting
            setters.add(getter);
            return this;
        }

        public FrozenBuilder<T> create() {
            return FrozenBuilder.create(this);
        }
    }

    /**
     * The class used as a static instance field for a type
     * that can be used to create a new builder instance.
     *
     * @param <T> The type to create
     */
    static class FrozenBuilder<T> {
        private final List<Function<T, ?>> setters;
        private final Function<Constructor<T>, T> constructor;

        private FrozenBuilder(Function<Constructor<T>, T> constructor, List<Function<T, ?>> setters) {
            this.constructor = constructor;
            this.setters = setters;
        }

        public static <T> FrozenBuilder<T> create(BuilderBuilder<T> builder) {
            return new FrozenBuilder<>(builder.constructor, builder.setters);
        }

        public Builder<T> newBuilder() {
            return Builder.create(this);
        }
    }

    /**
     * The class used by the client to initialize the values for
     * the type they want to create.
     *
     * @param <T> The type to create
     */
    static class Builder<T> {

        private Map<Function<T, ?>, Object> setters;
        private Function<Constructor<T>, T> constructor;

        private Builder(Function<Constructor<T>, T> constructor, List<Function<T, ?>> setters) {
            this.constructor = constructor;
            // TODO ability to supply default values / composite setter / validator
            this.setters = setters.stream().collect(Collectors.toMap(x -> x, x -> null));
        }

        public static <T> Builder<T> create(FrozenBuilder<T> builder) {
            return new Builder(builder.constructor, builder.setters);
        }

        public <V> Builder<T> with(Function<T, V> getter, V value) {
            setters.put(getter, value);
            return this;
        }

        public T build() {
            return constructor.apply(Constructor.create(this));
        }
    }

    /**
     * The class used by the type to be created to get
     * the fields to initialize with.
     *
     * @param <T> The type to create
     */
    static class Constructor<T> {
        private Map<Function<T, ?>, Object> setters;

        private Constructor(Map<Function<T, ?>, Object> setters) {
            this.setters = setters;
        }

        public static <T> Constructor<T> create(Builder<T> builder) {
            return new Constructor<>(builder.setters);
        }

        public <V> V get(Function<T, V> getter) {
            return (V)setters.get(getter);
        }
    }

    /**
     * A marker type for the field in question
     */
    static final class Field<T> {
        private final String name;
        private Field(String name) {
            this.name = name;
        }

        public static <T> Field<T> create(String name) {
            return new Field<>(name);
        }

        @Override public String toString() { return name; }
        @Override public int hashCode() { return name.hashCode(); }
        @Override public boolean equals(Object other) {
            if (other instanceof Field) {
                final Field<T> that (Field<T>)other;
                return Objects.equals(that.name, this.name);
            }
            return false;
        }
    }

    /**
     * An example type that supplies clients with a type builder
     */
    static class Something {
        private static FrozenBuilder<Something> BUILDER = BuilderBuilder.of(Something::new)
            .with(Something::getAge)
            .with(Something::getName)
            .create();

        public static Builder<Something> newBuilder() {
            return BUILDER.newBuilder();
        }

        private final String name;
        private final int age;

        private Something(Constructor<Something> builder) {
            this.name = builder.get(Something::getName);
            this.age = builder.get(Something::getAge);
        }

        public String getName() { return name; }
        public int getAge() { return age; }
    }

    /**
     * An example usage of the created builder to construct a new type
     */
    static void test() {
        final Something something = Something.newBuilder()
            .with(Something::getAge, 12)
            .with(Something::getName, "galen")
            .build();
    }
}
