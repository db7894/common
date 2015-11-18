package org.action;

import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.test.junit4.CamelTestSupport;
import org.junit.Test;

public class SimpleBeanTest extends CamelTestSupport {

	@Test
	public void testProcess() throws Exception {
		String message = template.requestBody("direct:hello", "World", String.class);
		assertEquals("Hello World", message);
	}

	@Override
	protected RouteBuilder createRouteBuilder() throws Exception {
		return new RouteBuilder() {
			@Override
			public void configure() throws Exception {
				from("direct:hello")
					.beanRef("helloBean", "respond")
					.to("mock:result");
			}
		};
	}
}
