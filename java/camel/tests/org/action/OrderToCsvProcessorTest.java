package org.action;

import java.io.File;
import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.test.junit4.CamelTestSupport;
import org.junit.Test;

public class OrderToCsvProcessorTest extends CamelTestSupport {

	@Test
	public void testProcess() throws Exception {
		String message = "0000004444000001212320091208  1217@1478@2132";
		template.sendBodyAndHeader("direct:start", message, "Date", "20091208");

		File file = new File("out/workspace/orders/received/report-20091208.csv");
		assertTrue("File should exist", file.exists());

		String body = context.getTypeConverter().convertTo(String.class, file);
		assertEquals("000000444,20091208,000001212,1217,1478,2132", body);
	}

	@Override
	protected RouteBuilder createRouteBuilder() throws Exception {
		return new RouteBuilder() {
			@Override
			public void configure() throws Exception {
				from("direct:start")
					.process(new OrderToCsvProcessor())
					.to("file://out/workspace/orders/received?fileName=report-${header.Date}.csv");
			}
		};
	}
}
