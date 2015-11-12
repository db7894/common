package org.bashwork.hqs;

import java.io.IOException;

import com.google.inject.Guice;
import io.grpc.netty.NettyServerBuilder;
import org.bashwork.hqs.configuration.HqsServerModule;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import io.grpc.Server;

import javax.inject.Inject;
import javax.inject.Named;

/**
 * An example of a simple server for the Hqs service.
 */
public class ServerMain {

    private static final Logger logger = LoggerFactory.getLogger(ServerMain.class);
    private final HqsService service;
    private final Integer port;
    private Server server;

    @Inject
    public ServerMain(HqsService service, @Named("ServerPort") Integer port) {
        this.service = service;
        this.port = port;
    }

    public static void main(String[] args) throws Exception {
        Guice.createInjector(new HqsServerModule())
            .getInstance(ServerMain.class)
            .start();
    }

    public void start() throws Exception {
        startServer();
        logger.info("Listening on " + port);
        startShutdownHook();
    }

    public void stop() {
        if (server != null) {
            logger.info("Shutting down " + port);
            server.shutdown();
        }
    }

    private void startServer() throws IOException {
        server = NettyServerBuilder.forPort(port)
            .addService(HqsGrpc.bindService(service))
            .build()
            .start();
    }

    private void startShutdownHook() throws Exception {
        Runtime.getRuntime().addShutdownHook(new Thread() {
            @Override
            public void run() {
                System.err.println("*** stopping the running server***");
                ServerMain.this.stop();
            }
        });
        server.awaitTermination();
    }
}
