package com.bashwork.commons.conversion;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

import java.io.IOException;

import org.junit.Before;
import org.junit.Test;
import org.mockito.Mock;

import com.bashwork.commons.serialize.Serializer;
import com.bashwork.commons.serialize.SerializerException;
import com.bashwork.commons.serialize.JsonSerializer;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.common.base.Objects;

import static org.mockito.Mockito.*;
import static org.mockito.MockitoAnnotations.*;

public class JsonDataConverterTest {
    
	// ------------------------------------------------------------------------
	// constants
	// ------------------------------------------------------------------------
	
	public static class Person {
		public String name;
		public Integer age;
		
		public Person() {}
		public Person(String name, Integer age) {
			this.name = name;
			this.age = age;
		}
		
	    @Override
	    public boolean equals(Object object) {
	        if (object == null) {
	            return false;
	        }
	        if (object.getClass() != getClass()) {
	            return false;
	        }

	        final Person that = (Person)object;
	        return Objects.equal(this.name, that.name)
	            && Objects.equal(this.age, that.age);
	    }

	    @Override
	    public int hashCode() {
	        return Objects.hashCode(name, age);
	    }
	}
    
    private final String name = "Foo Bar";
    private final int age = 30;
    private final String converted = "mock";
    
	// ------------------------------------------------------------------------
	// test setup
	// ------------------------------------------------------------------------
    
    @Mock private ObjectMapper mapper;
    
    @Before
    public void before() throws Exception {
        initMocks(this);
    }

	// ------------------------------------------------------------------------
	// tests
	// ------------------------------------------------------------------------
    
    @Test(expected = NullPointerException.class)
    public void test_converter_with_null_mapper() {
        new JsonSerializer(null);
    }
    
    @Test
    public void test_converter() {
        
        Person person = new Person(name, age);
        
        Serializer converter = new JsonSerializer();
        
        String marshalled = converter.serialize(person);
        assertNotNull(marshalled);
        
        Person unmarshalled = converter.deserialize(marshalled, Person.class);
        assertNotNull(unmarshalled);
        
        assertEquals(person, unmarshalled);    
    }
    
    @Test(expected = SerializerException.class)
    public void test_serialize()
    throws Exception {
        
        when(mapper.writeValueAsString(any()))
        	.thenThrow(new IOException("blah"));
        
        Serializer converter = new JsonSerializer(mapper);
        
        converter.serialize(new Person(name, age));   
    }
    
    @Test(expected = SerializerException.class)
    @SuppressWarnings("unchecked")
    public void test_deserialize() throws Exception {
        
        when(mapper.writeValueAsString(any())).thenReturn(converted);
        when(mapper.readValue(anyString(), any(Class.class))).thenThrow(new IOException());
        
        Serializer converter = new JsonSerializer(mapper);
        
        String marshalled = converter.serialize(new Person(name, age));
        converter.deserialize(marshalled, Person.class);   
    }
}
