package org.action;

import org.apache.activemq.ActiveMQConnectionFactory;
import org.apache.camel.CamelContext;
import org.apache.camel.Exchange;
import org.apache.camel.Processor;
import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.component.jms.JmsComponent;
import org.apache.camel.impl.DefaultCamelContext;

import javax.jms.ConnectionFactory;

/**
 * An example for using camel to connect
 * two file directories
 *
 */
public final class DownloadLogger implements Processor {

    @Override
    public void process(Exchange exchange) throws Exception {
        System.out.println("Downloading " + exchange.getIn().getHeader("CamelFileName"));
    }
}
