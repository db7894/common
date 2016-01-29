package com.bashwork.commons.utility;

import com.google.common.collect.ImmutableMap;
import org.junit.Test;

import java.util.Map;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;

/**
 * Code to test the functionality of the URIEncoder utility.
 */
public class URIEncoderTest {

    private static Map<String, String> CASES = ImmutableMap.<String, String>builder()
        .put("http://www.google.com/", "http://www.google.com/")
        .put("http://www.google.com/this and that", "http://www.google.com/this%20and%20that")
        .put("http://www.google.com/this+and+that", "http://www.google.com/this%2Band%2Bthat")
        .put("http://www.google.com/this<and>that", "http://www.google.com/this%3Cand%3Ethat")
        .put("http://www.google.com/this-and_that", "http://www.google.com/this-and_that")
        .put("http://www.google.com/GOOGLE", "http://www.google.com/GOOGLE")
        .put("http://www.google.com/0123456789", "http://www.google.com/0123456789")
        .put("http://www.google.com/file.jpg?expires=123", "http://www.google.com/file.jpg?expires%3D123")
        .build();

    @Test
    public void run_test_cases() {
        for (Map.Entry<String, String> entry : CASES.entrySet()) {
            String expect = entry.getValue();
            String actual = URIEncoder.encode(entry.getKey()).toString();

            assertThat(actual, equalTo(expect));
        }
    }
}
