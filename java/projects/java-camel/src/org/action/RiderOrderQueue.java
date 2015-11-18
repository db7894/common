package org.action;

import javax.jms.ConnectionFactory;

import org.apache.activemq.ActiveMQConnectionFactory;
import org.apache.camel.CamelContext;
import org.apache.camel.Exchange;
import org.apache.camel.Processor;
import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.impl.DefaultCamelContext;
import org.apache.camel.component.jms.JmsComponent;
import org.apache.camel.processor.aggregate.AggregationStrategy;

/**
 * A quick summery of a number of camels' features
 */
public final class RiderOrderQueue {

	private RiderOrderQueue() {
	}

	public static void main(String args[]) throws Exception {
		CamelContext context = new DefaultCamelContext();
		ConnectionFactory factory = new ActiveMQConnectionFactory("vm://localhost?broker.persistent=false");
		context.addComponent("jms", JmsComponent.jmsComponentAutoAcknowledge(factory));
		context.addRoutes(new RouteBuilder() {
			public void configure() {

			/**
			 * How to change the source of the messages
			 */
			//from("ftp://localhost/orders?username=testing&password=testing&noop=true")
			from("file:data?noop=true")
				.to("jms:incomingOrders");

			/**
			 * How to conditionally route messages
			 * We can also continue to send these messages to another route
			 * with the chain.end() keyword (and we can block messages from further processing
			 * with the chain.stop() method)
			 * wireTap sends the message to the wireTap EIP
			 */
			from("jms:incomingOrders")
				.wireTap("jms:orderAudit")
				.choice()
				.when(header("CamelFileName").endsWith(".xml")).to("jms:xmlOrders")
				.when(header("CamelFileName").endsWith(".csv")).to("jms:csvOrders")
				.otherwise().to("jms:badOrders").stop()
				.end()
				.to("jms:furtherProcessing");


			/**
			 * How to filter messages (with xpath in this case)
			 */
/*			from("jms:xmlOrders").filter(xpath("/order[not(@test)]"))
				.process(new Processor() {
					@Override
					public void process(Exchange exchange) throws Exception {
						System.out.println("Downloading XML order " + exchange.getIn().getHeader("CamelFileName"));
					}
				});*/

			/**
			 * How to route with recipient lists determined dynamically
			 * from the message.
			 */
/*			from("jms:xmlOrders")
				.setHeader("customer", xpath("/order/@customer"))
				.process(new Processor() {
					@Override
					public void process(Exchange exchange) throws Exception {
						String recipients = "jms:accounting";
						String customer = exchange.getIn().getHeader("customer", String.class);
						if (customer.equals("honda")) {
							recipients += ",jms:production";
						}
						exchange.getIn().setHeader("recipients", recipients);
					}
				}).recipientList(header("recipients"));*/

			/**
			 * This could have been achieved with the following
			 */
			from("jms:xmlOrders").bean(RecipientListBean.class);

			/**
			 * How to multi-cast messages sequentially
			 */
			from("jms:csvOrders")
				.multicast()
				.to("jms:accounting", "jms:production");

			/**
			 * How to multi-cast messages in parallel
			 * This uses a default thread pool size of 10. To change, supply
			 * an initialized ExecutorService like: chain.executorService(executor)
			 *
			 * To stop all processors on an exception: chain.stopOnException()
			 */
/*			from("jms:csvOrders")
				.multicast().parallelProcessing()
				.to("jms:accounting", "jms:production");*/

			/**
			 * A bad message queue example
			 */
			from("jms:badOrders")
				.process(new Processor() {
					@Override
					public void process(Exchange exchange) throws Exception {
						System.out.println("Downloading BAD order " + exchange.getIn().getHeader("CamelFileName"));
					}
				});

			/**
			 * An example of an in-place transformer and an in place enricher
			 */
/*			from("jms:accounting")
				.transform(body().regexReplaceAll("\n", "<br/>"))
				.pollEnrich("ftp://example", new AggregationStrategy() {
					@Override
					public Exchange aggregate(Exchange exchange, Exchange nexchange) {
						if (nexchange == null) {
							return exchange;
						}

						String first = exchange.getIn().getBody(String.class);
						String second = nexchange.getIn().getBody(String.class);
						exchange.getIn().setBody(first + "\n" + second);

						return exchange;
					}
				}).to("mock:result");*/

			from("jms:production")
				.process(new Processor() {
					@Override
					public void process(Exchange exchange) throws Exception {
						System.out.println("Gold Level order " + exchange.getIn().getHeader("CamelFileName"));
					}
				});
			}
		});

		context.start();
		Thread.sleep(10000);
		context.stop();
	}
}
