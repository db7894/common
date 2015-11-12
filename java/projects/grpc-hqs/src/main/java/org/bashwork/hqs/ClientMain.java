package org.bashwork.hqs;

import com.google.inject.Guice;
import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;
import org.bashwork.hqs.configuration.HqsClientModule;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import javax.inject.Inject;
import javax.inject.Named;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

/**
 * An example of a simple client for the Hqs service.
 */
public class ClientMain {

    private static final Logger logger = LoggerFactory.getLogger(ClientMain.class);
    private final ManagedChannel channel;
    private final HqsGrpc.HqsBlockingStub client;

    /**
     * Initialize a new instance of the ClientMain class.
     *
     * @param host The host to send requests to.
     * @param port The port to send requests to.
     */
    @Inject
    public ClientMain(
        @Named("ServerHost") String host,
        @Named("ServerPort") Integer port) {

        channel = ManagedChannelBuilder
            .forAddress(host, port)
            .usePlaintext(true)
            .build();
        client = HqsGrpc.newBlockingStub(channel);
    }

    /**
     * The main runner method for the client. This simply starts
     * an instance of this class using Guice.
     *
     * @param args Any command line arguments
     * @throws Exception In case something goes wrong.
     */
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

        try {
            final String messageBody = "example message";
            final String queueName = "example-queue-name";
            final String queueUrl = createQueue(queueName);
            final String receiptHandle = sendMessage(queueUrl, messageBody);
            final Message message = receiveMessage(queueUrl).get(0);

            //stressTest(queueUrl);

            logger.info("get_queue_test: {}", getQueueUrl(queueName).equals(queueUrl));
            logger.info("list_queues_test: {}", listQueues().contains(queueUrl));
            logger.info("send_message_test: {}", receiptHandle.equals(message.getReceiptHandle()));
            logger.info("receive_message_test: {}", messageBody.equals(message.getBody()));
            logger.info("delete_message_test: {}", deleteMessage(queueUrl, message.getReceiptHandle()));
            logger.info("delete_queue_test: {}", deleteQueue(queueUrl));
            //error logger.info("delete_queue_test: {}", deleteQueue(queueUrl));
            logger.info("finished client workflow");

        } catch (Exception ex) {
            logger.error("an error occurred during the test", ex);
        }
        channel.shutdown().awaitTermination(5, TimeUnit.SECONDS);
    }

    /**
     * Example of a simple stress test to see how much can be pushed
     * through the client / server.
     *
     * @param queueUrl The queue to run the stress test on.
     * @throws InterruptedException
     * @throws ExecutionException
     */
    private void stressTest(final String queueUrl)
        throws InterruptedException, ExecutionException {

        final ExecutorService service = Executors.newFixedThreadPool(4);
        final List<Future<?>> futures = new ArrayList<>();

        logger.info("running the stress test");
        futures.add(service.submit(() -> publisher(queueUrl, 30000)));
        //futures.add(service.submit(() -> publisher(queueUrl, 1000)));
        //futures.add(service.submit(() -> publisher(queueUrl, 1000)));
        futures.add(service.submit(() -> subscriber(queueUrl, 30000)));

        for (Future<?> future : futures) {
            future.get();
        }

        logger.info("tearing down the stress test");
        service.shutdown();
        service.awaitTermination(5, TimeUnit.SECONDS);
    }

    /**
     * Example of a high throughput publisher
     *
     * @param queueUrl The queue to publish messages on.
     * @param count The number of messages to publish.
     */
    private void publisher(final String queueUrl, final int count) {
        final long start = System.currentTimeMillis();

        for (int index = 0; index < count; ++index) {
            final String receiptHandle = sendMessage(queueUrl, Integer.toHexString(index));
            logger.debug("published {} to {}", receiptHandle, queueUrl);
        }

        final long stop = System.currentTimeMillis();
        logger.info("publish rate for {} messages is {}", count, (1000.0 * (stop - start)) / count);
    }

    /**
     * An example of a high throughput subscriber.
     *
     * @param queueUrl The queue to read messages from.
     * @param count The number of messages to read.
     */
    private void subscriber(final String queueUrl, final int count) {
        final long start = System.currentTimeMillis();

        for (int index = 0; index < count; ++index) {
            for (Message message : receiveMessage(queueUrl)) {
                logger.debug("received {} from {}", message.getBody(), queueUrl);
                deleteMessage(queueUrl, message.getReceiptHandle());
                logger.debug("deleted {} from {}", message.getReceiptHandle(), queueUrl);
            }
        }

        final long stop = System.currentTimeMillis();
        logger.info("subscribe rate for {} messages is {}", count, (1000.0 * (stop - start)) / count);
    }

    /*
     * A collection of wrappers for the client API.
     */

    private String sendMessage(final String queueUrl, final String message) {
        return client.sendMessage(SendMessageRequest
            .newBuilder()
            .setMessageBody(message)
            .setQueueUrl(queueUrl)
            .putAllAttributes(Collections.singletonMap("example-key", "example-value"))
            .build())
            .getMetadata().getId();
    }

    private List<Message> receiveMessage(final String queueUrl) {
        return client.receiveMessage(ReceiveMessageRequest
            .newBuilder()
            .setMaxNumberOfMessages(1)
            .setQueueUrl(queueUrl)
            .build())
            .getMessageList();
    }

    private boolean deleteMessage(final String queueUrl, final String receiptHandle) {
        return client.deleteMessage(DeleteMessageRequest
            .newBuilder()
            .setQueueUrl(queueUrl)
            .setReceiptHandle(receiptHandle)
            .build())
            .getRequestId() != null;
    }

    private String createQueue(final String queueName) {
        return client.createQueue(CreateQueueRequest
            .newBuilder()
            .setQueueName(queueName)
            .build())
            .getQueueUrl();
    }

    private String getQueueUrl(final String queueName) {
        return client.getQueueUrl(GetQueueUrlRequest
            .newBuilder()
            .setQueueName(queueName)
            .build())
            .getQueueUrl();
    }

    private boolean deleteQueue(final String queueUrl) {
        return client.deleteQueue(DeleteQueueRequest
            .newBuilder()
            .setQueueUrl(queueUrl)
            .build())
            .getRequestId() != null;
    }

    private List<String> listQueues() {
        return client.listQueues(ListQueuesRequest
            .newBuilder()
            .build())
            .getQueuesList().stream()
            .map(Queue::getQueueUrl)
            .collect(Collectors.toList());
    }
}
