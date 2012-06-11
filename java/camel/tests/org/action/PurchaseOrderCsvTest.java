package org.action;

import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.component.mock.MockEndpoint;
import org.apache.camel.model.dataformat.JsonLibrary;
import org.apache.camel.test.junit4.CamelTestSupport;
import org.junit.Test;

import java.util.List;

/**
 * An example of using the csv parser to split
 */
public class PurchaseOrderCsvTest extends CamelTestSupport {

	@Test
	public void testUnmarshal() throws Exception {
		MockEndpoint mock = getMockEndpoint("mock:queue.csv");

		String message = "title,cost,price";
		template.sendBody("direct:start", message);

		List line = mock.getReceivedExchanges().get(0).getIn().getBody(List.class);

		assertEquals("title", line.get(0));
		assertEquals("cost", line.get(1));
		assertEquals("price", line.get(2));
	}

	@Override
	protected RouteBuilder createRouteBuilder() throws Exception {
		return new RouteBuilder() {
			@Override
			public void configure() throws Exception {
				context().setTracing(true);
				from("direct:start")
					.unmarshal().csv().split(body())
					.to("mock:queue.csv");
			}
		};
	}
}
