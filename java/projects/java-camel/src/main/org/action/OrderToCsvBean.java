package org.action;

/**
 * Can schedule this with quartz as follows::
 *
 * from(:quartz://report?cron=0+0+6+*+*+?")
 * 	.to("http://riders.com/orders/cmd=received")
 * 	.bean(new OrderToCsvBean())
 * 	.to("file://riders/orders?fileName=report-${header.Date}.csv");
 */
public class OrderToCsvBean {

	public String map(String custom) throws Exception {
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

		return csv.toString();
	}
}
