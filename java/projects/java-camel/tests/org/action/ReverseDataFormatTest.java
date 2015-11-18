package org.action;

import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.component.mock.MockEndpoint;
import org.apache.camel.test.junit4.CamelTestSupport;
import org.junit.Test;

import java.io.File;
import java.util.List;

public class ReverseDataFormatTest extends CamelTestSupport {

	@Test
	public void testMarshal() throws Exception {
		String message = template.requestBody("direct:marshal", "abcdefghijklmnop", String.class);
		assertEquals("ponmlkjihgfedcba", message);
	}

	@Test
	public void testUnmarshal() throws Exception {
		String message = template.requestBody("direct:unmarshal", "ponmlkjihgfedcba", String.class);
		assertEquals("abcdefghijklmnop", message);
	}

	@Override
	protected RouteBuilder createRouteBuilder() throws Exception {
		return new RouteBuilder() {
			@Override
			public void configure() throws Exception {
				from("direct:marshal")
					.marshal(new ReverseDataFormat())
						.to("mock:marshalOut");

				from("direct:unmarshal")
					.marshal(new ReverseDataFormat())
					.to("mock:unmarshalOut");
			}
		};
	}
}
