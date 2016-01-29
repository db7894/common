package com.bashwork.commons.workflow;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;

import org.junit.Test;

import com.bashwork.commons.serializer.Serializers;
import com.bashwork.commons.serializer.SpecificSerializer;

public class WorkflowRequestTest {

    private static final SpecificSerializer<String, WorkflowRequest> serializer
    = Serializers.specificJson(WorkflowRequest.class);

    @Test
    public void serialize_round_trip() {
        WorkflowRequest expected = WorkflowRequest.builder()
            .withMessage("message")
            .withTracker("tracker")
            .withType("type")
            .build();

        String serialized = serializer.serialize(expected);
        WorkflowRequest deserialized = serializer.deserialize(serialized);

        assertThat(deserialized, equalTo(expected));
        assertThat(deserialized.hashCode(), equalTo(expected.hashCode()));
        assertThat(deserialized.toString(), equalTo(expected.toString()));
    }
}
