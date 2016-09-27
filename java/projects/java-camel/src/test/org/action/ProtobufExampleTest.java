/*
package org.action;

import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.dataformat.protobuf.ProtobufDataFormat;
import org.apache.camel.test.junit4.CamelTestSupport;
import org.junit.Test;

public class ProtobufExampleTest extends CamelTestSupport {

	@Test
	public void testMarshaling() throws Exception {
		ProtobufDataFormat format = new ProtobufDataFormat(Person.getDefaultInstance());

	}

	@Override
	protected RouteBuilder createRouteBuilder() throws Exception {
		return new RouteBuilder() {
			@Override
			public void configure() throws Exception {
				from("direct:start")
					.bean(new OrderToCsvBean())
					.to("file://out/workspace/orders/received?fileName=report-${header.Date}.csv");
			}
		};
	}
}
*/
