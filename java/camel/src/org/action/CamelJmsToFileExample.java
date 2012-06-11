package org.action;

import javax.jms.ConnectionFactory;

import org.apache.activemq.ActiveMQConnectionFactory;
import org.apache.camel.CamelContext;
import org.apache.camel.ProducerTemplate;
import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.component.jms.JmsComponent;
import org.apache.camel.impl.DefaultCamelContext;

/**
 * An example for using camel to connect
 * JMS Queue to consuming to Text File
 *
 */
public final class CamelJmsToFileExample {

    private CamelJmsToFileExample() {
    }

    public static void main(String args[]) throws Exception {
        CamelContext context = new DefaultCamelContext();
        ConnectionFactory factory = new ActiveMQConnectionFactory("vm://localhost?broker.persistent=false");
        context.addComponent("test-jms", JmsComponent.jmsComponentAutoAcknowledge(factory));
        context.addRoutes(new RouteBuilder() {
            public void configure() {
                from("test-jms:queue:test.queue")
                    .to("file://data/queue");
            }
        });

        ProducerTemplate producer = context.createProducerTemplate();
        context.start();

        for (int i = 0; i < 10; ++i) {
            producer.sendBody("test-jms:queue:test.queue", "Test Message: " + i);
        }

        Thread.sleep(1000);
        context.stop();
    }
}
