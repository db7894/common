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

    /**
     * Initialize a new instance of the ServerMain class.
     *
     * @param service The service instance to host.
     * @param port The port to host the service on.
     */
    @Inject
    public ServerMain(HqsService service, @Named("ServerPort") Integer port) {
        this.service = service;
        this.port = port;
    }

    /**
     * The main runner point for the server main. This simply starts
     * an instance that is created by Guice.
     *
     * @param args Any command line arguments to the program.
     * @throws Exception
     */
    public static void main(String[] args) throws Exception {
        Guice.createInjector(new HqsServerModule())
            .getInstance(ServerMain.class)
            .start();
    }

    /**
     * Starts the server and attaches any callback methods.
     *
     * @throws Exception
     */
    public void start() throws Exception {
        startServer();
        logger.info("Listening on " + port);
        startShutdownHook();
    }

    /**
     * Stops the underlying GRPC server.
     */
    public void stop() {
        if (server != null) {
            logger.info("Shutting down " + port);
            server.shutdown();
        }
    }

    /**
     * Creates and starts the underlying GRPC server.
     *
     * @throws IOException
     */
    private void startServer() throws IOException {
        server = NettyServerBuilder
            .forPort(port)
            .addService(HqsGrpc.bindService(service))
            .build()
            .start();
    }

    /**
     * Attaches a shutdown runtime hook to stop the server
     * cleanly on JVM exit.
     *
     * @throws Exception
     */
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
