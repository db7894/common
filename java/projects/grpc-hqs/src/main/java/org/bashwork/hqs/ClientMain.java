package org.bashwork.hqs;

import com.google.inject.Guice;
import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;
import org.bashwork.hqs.configuration.HqsClientModule;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import javax.inject.Inject;
import javax.inject.Named;
import java.util.List;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

/**
 * An example of a simple client for the Hqs service.
 */
public class ClientMain {

    private static final Logger logger = LoggerFactory.getLogger(ClientMain.class);
    private final ManagedChannel channel;
    private final HqsGrpc.HqsBlockingStub client;

    @Inject
    public ClientMain(@Named("ServerHost") String host, @Named("ServerPort") Integer port) {
        channel = ManagedChannelBuilder
            .forAddress(host, port)
            .usePlaintext(true)
            .build();
        client = HqsGrpc.newBlockingStub(channel);
    }

    public static void main(String[] args) throws Exception {
        Guice.createInjector(new HqsClientModule())
            .getInstance(ClientMain.class)
            .start();
    }

    /**
     * Performs a simple workflow test that exercises all the methods
     * of the service.
     *
     * @throws Exception If any exception occurs.
     */
    public void start() throws Exception {
        logger.info("starting client workflow");

        final String queueName = "example-queue-name";
        final String queueUrl  = createQueue(queueName);

        logger.info("get_queue_test: {}", getQueueUrl(queueName).equals(queueUrl));
        logger.info("list_queues_test: {}", listQueues().contains(queueUrl));
        logger.info("delete_queue_test: {}", deleteQueue(queueUrl));
        logger.info("finished client workflow");
        channel.shutdown().awaitTermination(5, TimeUnit.SECONDS);
    }

    private String createQueue(final String queueName) {
        logger.info("creating queue {}", queueName);
        return client.createQueue(CreateQueueRequest.newBuilder()
            .setQueueName(queueName)
            .build())
            .getQueueUrl();
    }

    private String getQueueUrl(final String queueName) {
        logger.info("checking for queue {}", queueName);
        return client.getQueueUrl(GetQueueUrlRequest.newBuilder()
            .setQueueName(queueName)
            .build())
            .getQueueUrl();
    }

    private boolean deleteQueue(final String queueUrl) {
        logger.info("deleting queue {}", queueUrl);
        return client.deleteQueue(DeleteQueueRequest.newBuilder()
            .setQueueUrl(queueUrl)
            .build())
            .getRequestId() != null;
    }

    private List<String> listQueues() {
        logger.info("listing all queues");
        return client.listQueues(ListQueuesRequest.newBuilder()
            .build())
            .getQueuesList().stream()
            .map(Queue::getQueueUrl)
            .collect(Collectors.toList());
    }
}
