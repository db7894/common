package org.action;

import org.apache.camel.Exchange;
import org.apache.camel.Processor;

/**
 * Can schedule this with quartz as follows::
 *
 * from(:quartz://report?cron=0+0+6+*+*+?")
 * 	.to("http://riders.com/orders/cmd=received")
 * 	.process(new OrderToCsvProcessor())
 * 	.to("file://riders/orders?fileName=report-${header.Date}.csv");
 */
public class OrderToCsvProcessor implements Processor {

	@Override
	public void process(Exchange exchange) throws Exception {
		String custom = exchange.getIn().getBody(String.class);
		String id = custom.substring(0, 9);
		String cid = custom.substring(10, 19);
		String date = custom.substring(20, 29);
		String items = custom.substring(30);
		String[] itemids = items.split("@");

		StringBuilder csv = new StringBuilder();
		csv.append(id.trim());
		csv.append(",").append(date.trim());
		csv.append(",").append(cid.trim());
		for (String item : itemids) {
			csv.append(",").append(item.trim());
		}

		exchange.getIn().setBody(csv.toString());
	}
}
