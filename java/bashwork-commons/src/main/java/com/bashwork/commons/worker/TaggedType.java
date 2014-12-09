package com.bashwork.commons.worker;

import java.util.Objects;

/**
 * A simple value that is tagged with an identifier.
 * 
 * @param <TValue> The type of the value that is tagged.
 */
public class TaggedType<TValue> {
    
    private final String tag;
    private final TValue value;
    
    /**
     * Initializes a new instance of the TaggedType class.
     * 
     * @param value The value to maintain a reference to.
     * @param tag The current tag of the value.
     */
    public TaggedType(TValue value, String tag) {
        this.value = value;
        this.tag = tag;
    }
    
    /**
     * Create a new instance of the TaggedType.
     * 
     * @param value The value to be tagged.
     * @param tag The tag of the suppled value
     * @return A new instance of the TaggedType.
     */
    public static <T> TaggedType<T> create(T value, String tag) {
        return new TaggedType<T>(value, tag);
    }
    
    public String getTag() { return tag; }
    public TValue getValue() { return value; }
    
    @Override
    public int hashCode() {
        return Objects.hash(value, tag);
    }
    
    @Override
    @SuppressWarnings("unchecked")
    public boolean equals(Object object) {
        if (this == object)
            return true;

        if (object == null)
            return false;

        if (getClass() != object.getClass())
            return false;
        
        final TaggedType<TValue> that = (TaggedType<TValue>)object;
        return Objects.equals(this.value, that.value)
            && Objects.equals(this.tag, that.tag);
    }
    
    @Override
    public String toString() {
        return String.format("%s @ %s", value, tag);
    }
}
