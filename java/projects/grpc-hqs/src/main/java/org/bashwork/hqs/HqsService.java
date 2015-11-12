package org.bashwork.hqs;

import java.util.Optional;
import java.util.List;
import java.util.UUID;
import java.util.stream.Collectors;

import org.bashwork.hqs.database.HqsMessage;
import org.bashwork.hqs.database.HqsQueue;
import org.bashwork.hqs.database.HqsDatabase;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import io.grpc.stub.StreamObserver;

import javax.inject.Inject;

/**
 * Implementation of the Hqs service that backs up to an
 * instance of the HqsDatabase store.
 */
public final class HqsService implements HqsGrpc.Hqs {

    private static final Logger logger = LoggerFactory.getLogger(HqsService.class);
    private final HqsDatabase store;

    @Inject
    public HqsService(final HqsDatabase store) {
        this.store = store;
    }

    @Override
    public void createQueue(final CreateQueueRequest request,
        final StreamObserver<CreateQueueResponse> observer) {

        final Optional<HqsQueue> queue = store.createQueue(request.getQueueName());
        final CreateQueueResponse response = CreateQueueResponse.newBuilder()
            .setQueueUrl(queue.get().getUrl())
            .setRequestId(getRequestId())
            .build();

        logger.debug("created queue {}: {}", request.getQueueName(), queue);

        observer.onNext(response);
        observer.onCompleted();
    }

    @Override
    public void deleteQueue(final DeleteQueueRequest request,
        final StreamObserver<DeleteQueueResponse> observer) {

        final boolean isDeleted = store.deleteQueue(request.getQueueUrl());
        final DeleteQueueResponse response = DeleteQueueResponse.newBuilder()
            .setRequestId(getRequestId())
            .build();

        logger.debug("deleted queue {}: {}", request.getQueueUrl(), isDeleted);

        observer.onNext(response);
        observer.onCompleted();
    }

    @Override
    public void listQueues(final ListQueuesRequest request,
        final StreamObserver<ListQueuesResponse> observer) {

        final List<HqsQueue> queues = store.listQueues();
        final ListQueuesResponse response = ListQueuesResponse.newBuilder()
            .addAllQueues(queues.stream()
                .map(queue -> Queue.newBuilder()
                    .setQueueName(queue.getName())
                    .setQueueUrl(queue.getUrl())
                    .build())
                .collect(Collectors.toList()))
            .setRequestId(getRequestId())
            .build();

        logger.debug("listed {} queues", queues.size());

        observer.onNext(response);
        observer.onCompleted();
    }

    @Override
    public void purgeQueue(final PurgeQueueRequest request,
        final StreamObserver<PurgeQueueResponse> observer) {

        final boolean isPurged = store.purgeQueue(request.getQueueUrl());
        final PurgeQueueResponse response = PurgeQueueResponse.newBuilder()
            .setRequestId(getRequestId())
            .build();

        logger.debug("purged messages from {}: {}", request.getQueueUrl(), isPurged);

        observer.onNext(response);
        observer.onCompleted();
    }

    @Override
    public void getQueueUrl(final GetQueueUrlRequest request,
        final StreamObserver<GetQueueUrlResponse> observer) {

        final Optional<HqsQueue> queue = store.getQueue(request.getQueueName());
        final GetQueueUrlResponse response = GetQueueUrlResponse.newBuilder()
            .setQueueUrl(queue.get().getUrl()) // TODO
            .setRequestId(getRequestId())
            .build();

        logger.debug("got the queue url for {}: {}", request.getQueueName(), queue);

        observer.onNext(response);
        observer.onCompleted();
    }

    @Override
    public void sendMessage(final SendMessageRequest request,
        final StreamObserver<SendMessageResponse> observer) {

        final Optional<HqsMessage> message = store.sendMessage(request.getQueueUrl(), request.getMessageBody());
        final SendMessageResponse response = SendMessageResponse.newBuilder()
            .setMetadata(MessageMetadata.newBuilder()
                .setId(message.get().getIdentifier())
                .setMd5OfMessageBody(message.get().getMd5HashOfBody())
                .build())
            .setRequestId(getRequestId())
            .build();

        logger.debug("sent the message to queue {}: {}", request.getQueueUrl(), message);

        observer.onNext(response);
        observer.onCompleted();
    }

    @Override
    public void sendMessageBatch(final SendMessageBatchRequest request,
        final StreamObserver<SendMessageBatchResponse> observer) {

        final List<HqsMessage> messages = store.sendMessageBatch(request.getQueueUrl(), request.getMessageBodiesList());
        final SendMessageBatchResponse response = SendMessageBatchResponse.newBuilder()
            // TODO failed
            .addAllSuccessful(messages.stream()
                .map(message -> MessageMetadata.newBuilder()
                    .setId(message.getIdentifier())
                    .setMd5OfMessageBody(message.getMd5HashOfBody())
                    .build())
                .collect(Collectors.toList()))
            .setRequestId(getRequestId())
            .build();

        logger.debug("sent {} messages to queue {}", messages.size(), request.getQueueUrl());

        observer.onNext(response);
        observer.onCompleted();
    }

    @Override
    public void receiveMessage(final ReceiveMessageRequest request,
        final StreamObserver<ReceiveMessageResponse> observer) {

        final List<HqsMessage> messages = store.receiveMessages(request.getQueueUrl(), request.getMaxNumberOfMessages());
        final ReceiveMessageResponse response = ReceiveMessageResponse.newBuilder()
            .addAllMessage(messages.stream()
                .map(message -> Message.newBuilder()
                    .setBody(message.getBody())
                    .setId(message.getIdentifier())
                    .setMd5OfMessageBody(message.getMd5HashOfBody())
                    .setReceiptHandle(message.getIdentifier())
                    .build())
                .collect(Collectors.toList()))
            .setRequestId(getRequestId())
            .build();

        logger.debug("received {} messages from queue {}", messages.size(), request.getQueueUrl());

        observer.onNext(response);
        observer.onCompleted();

    }

    @Override
    public void deleteMessage(final DeleteMessageRequest request,
        final StreamObserver<DeleteMessageResponse> observer) {

        final Optional<HqsMessage> message = store.deleteMessage(request.getReceiptHandle());
        final DeleteMessageResponse response = DeleteMessageResponse.newBuilder()
            .setRequestId(getRequestId())
            .build();

        logger.debug("deleted message from queue {}: {}", request.getQueueUrl(), message);

        observer.onNext(response);
        observer.onCompleted();
    }

    @Override
    public void deleteMessageBatch(final DeleteMessageBatchRequest request,
        final StreamObserver<DeleteMessageBatchResponse> observer) {

        final List<HqsMessage> messages = store.deleteMessageBatch(request.getReceiptHandlesList());
        final DeleteMessageBatchResponse response = DeleteMessageBatchResponse.newBuilder()
            // TODO failed
            .addAllSuccessful(messages.stream()
                .map(HqsMessage::getIdentifier)
                .collect(Collectors.toList()))
            .setRequestId(getRequestId())
            .build();

        logger.debug("deleted {} messages from queue {}", messages.size(), request.getQueueUrl());

        observer.onNext(response);
        observer.onCompleted();
    }

    /**
     * Generates a new unique identifier that can be used to reference
     * all the operations associated with this request.
     *
     * @return The unique request identifier.
     */
    private static String getRequestId() {
        return UUID.randomUUID().toString();
    }
}
