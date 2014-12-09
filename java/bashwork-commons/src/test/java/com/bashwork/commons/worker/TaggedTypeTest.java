package com.bashwork.commons.worker;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.Matchers.*;

import org.junit.Test;

public class TaggedTypeTest {
    
    @Test
    public void create_tagged_type() {
        String value = "value";
        String tag = "tag";
        TaggedType<String> type = TaggedType.create(value, tag);
        TaggedType<String> that = TaggedType.create(value, tag);
        
        assertThat(type.getValue(), is(value));
        assertThat(type.getTag(), is(tag));
        assertThat(type, is(that));
        assertThat(type.toString(), is(that.toString()));
        assertThat(type.hashCode(), is(that.hashCode()));
    }
}
