package org.action;

import javax.jms.ConnectionFactory;

import org.apache.activemq.ActiveMQConnectionFactory;
import org.apache.camel.CamelContext;
import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.impl.DefaultCamelContext;
import org.apache.camel.component.jms.JmsComponent;

/**
 * An example for using camel to connect
 * two file directories
 *
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
                from("ftp://localhost/orders?username=testing&password=testing")
                    .to("jms:queue:incomingOrders");
            }
        });

        context.start();
        Thread.sleep(10000);
        context.stop();
    }
}
