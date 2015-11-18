package org.action;

import org.apache.camel.Exchange;
import org.apache.camel.Processor;

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
