package org.action;

import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.component.mock.MockEndpoint;
import org.apache.camel.model.dataformat.JsonLibrary;
import org.apache.camel.test.junit4.CamelTestSupport;
import org.junit.Test;

public class PurchaseOrderJsonTest extends CamelTestSupport {

	@Test
	public void testUnmarshal() throws Exception {
		MockEndpoint mock = getMockEndpoint("mock:queue.csv");

		String message = "{ \"title\":\"title\", \"cost\":\"22\", \"count\":\"1\" }";
		template.sendBody("direct:start", message);

		ExampleBook book = mock.getReceivedExchanges().get(0).getIn().getBody(ExampleBook.class);

		assertEquals("title", book.title);
		assertEquals(22, book.cost);
		assertEquals(1, book.count);
	}

	@Override
	protected RouteBuilder createRouteBuilder() throws Exception {
		return new RouteBuilder() {
			@Override
			public void configure() throws Exception {
				context().setTracing(true);
				from("direct:start")
					.unmarshal().json(JsonLibrary.Jackson, ExampleBook.class)
					.to("mock:queue.csv");
			}
		};
	}
}
