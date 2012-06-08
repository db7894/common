package org.action;

import org.apache.camel.CamelContext;
import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.impl.DefaultCamelContext;

/**
 * An example for using camel to connect
 * two file directories
 *
 */
public final class CamelFileCopier {

    private CamelFileCopier() {
    }

    public static void main(String args[]) throws Exception {
        CamelContext context = new DefaultCamelContext();
        context.addRoutes(new RouteBuilder() {
            public void configure() {
            from("file:data/input?noop=true")
                .to("file://data/outbox");
            }
        });

        context.start();
        Thread.sleep(10000);
        context.stop();
    }
}
