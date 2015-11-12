package org.bashwork.hqs.service;

import static java.util.Objects.requireNonNull;

import java.util.Map;
import java.util.Optional;
import java.util.List;
import java.util.Set;

import org.bashwork.hqs.*;
import org.bashwork.hqs.database.HqsMessage;
import org.bashwork.hqs.database.HqsQueue;
import org.bashwork.hqs.database.HqsDatabase;
import org.bashwork.hqs.service.errors.CouldNotDeleteMessageError;
import org.bashwork.hqs.service.errors.CouldNotSendMessageError;
import org.bashwork.hqs.service.errors.QueueDoesNotExistError;
import org.bashwork.hqs.utility.Streams;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import io.grpc.stub.StreamObserver;

import javax.inject.Inject;

/**
 * Implementation of the Hqs service that backs up to an
 * instance of the HqsDatabase database.
 */
public final class HqsService implements HqsGrpc.Hqs {

    private static final Logger logger = LoggerFactory.getLogger(HqsService.class);
    private final HqsDatabase database;

    /**
     * Initialize a new instance of the HqsService class.
     *
     * @param database The database to operate with.
     */
    @Inject
    public HqsService(final HqsDatabase database) {
        this.database = requireNonNull(database, "HqsService requires an HqsDatabase");
    }

    // TODO change message visibility
    // TODO change message visibility batch
    // TODO get queue attributes
    // TODO set queue attributes

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void createQueue(final CreateQueueRequest request,
        final StreamObserver<CreateQueueResponse> observer) {

        final String queueName = request.getQueueName();
        final Optional<HqsQueue> queue = database.createQueue(queueName);

        if (queue.isPresent()) {
            final CreateQueueResponse response = CreateQueueResponse.newBuilder()
                .setQueueUrl(queue.get()
                    .getUrl())
                .setRequestId(HqsServiceAdapter.generateRequestId())
                .build();

            logger.debug("created queue {}: {}", queueName, queue);
            observer.onNext(response);
            observer.onCompleted();

        } else {
            logger.debug("failed to create queue {}", queueName);
            observer.onError(new QueueDoesNotExistError(queueName));
        }
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void deleteQueue(final DeleteQueueRequest request,
        final StreamObserver<DeleteQueueResponse> observer) {

        final String queueUrl = request.getQueueUrl();
        final boolean isDeleted = database.deleteQueue(queueUrl);

        if (isDeleted) {
            final DeleteQueueResponse response = DeleteQueueResponse.newBuilder()
                .setRequestId(HqsServiceAdapter.generateRequestId())
                .build();

            logger.debug("deleted queue {}: {}", queueUrl, isDeleted);
            observer.onNext(response);
            observer.onCompleted();

        } else {
            logger.debug("failed to delete queue {}", queueUrl);
            observer.onError(new QueueDoesNotExistError(queueUrl));
        }
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void listQueues(final ListQueuesRequest request,
        final StreamObserver<ListQueuesResponse> observer) {

        final List<HqsQueue> queues = database.listQueues();
        final ListQueuesResponse response = ListQueuesResponse.newBuilder()
            .addAllQueues(HqsServiceAdapter.adaptQueues(queues))
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("listed {} queues", queues.size());

        observer.onNext(response);
        observer.onCompleted();
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void purgeQueue(final PurgeQueueRequest request,
        final StreamObserver<PurgeQueueResponse> observer) {

        final String queueUrl = request.getQueueUrl();
        final boolean isPurged = database.purgeQueue(queueUrl);

        if (isPurged) {
            final PurgeQueueResponse response = PurgeQueueResponse.newBuilder()
                .setRequestId(HqsServiceAdapter.generateRequestId())
                .build();

            logger.debug("purged messages from {}: {}", queueUrl, isPurged);
            observer.onNext(response);
            observer.onCompleted();

        } else {
            logger.debug("failed to purge messages from queue {}", queueUrl);
            observer.onError(new QueueDoesNotExistError(queueUrl));
        }
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void getQueueUrl(final GetQueueUrlRequest request,
        final StreamObserver<GetQueueUrlResponse> observer) {

        final String queueName = request.getQueueName();
        final Optional<HqsQueue> queue = database.getQueue(queueName);

        if (queue.isPresent()) {
            final GetQueueUrlResponse response = GetQueueUrlResponse.newBuilder()
                .setQueueUrl(queue.get().getUrl())
                .setRequestId(HqsServiceAdapter.generateRequestId())
                .build();

            logger.debug("got the queue url for {}: {}", queueName, queue);
            observer.onNext(response);
            observer.onCompleted();

        } else {
            logger.debug("failed to retrieve queue {}", queueName);
            observer.onError(new QueueDoesNotExistError(queueName));
        }
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void sendMessage(final SendMessageRequest request,
        final StreamObserver<SendMessageResponse> observer) {

        final String queueUrl = request.getQueueUrl();
        final String messageBody = request.getMessageBody();
        final Map<String, String> attributes = request.getAttributes();
        final Optional<HqsMessage> message = database.sendMessage(queueUrl, messageBody, attributes);

        if (message.isPresent()) {
            final SendMessageResponse response = SendMessageResponse.newBuilder()
                .setMetadata(HqsServiceAdapter.adaptMetadata(message.get()))
                .setRequestId(HqsServiceAdapter.generateRequestId())
                .build();

            logger.debug("sent the message to queue {}: {}", request.getQueueUrl(), message);
            observer.onNext(response);
            observer.onCompleted();

        } else {
            logger.debug("failed to sent message to queue {}", request.getQueueUrl());
            observer.onError(new CouldNotSendMessageError(messageBody));
        }
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void sendMessageBatch(final SendMessageBatchRequest request,
        final StreamObserver<SendMessageBatchResponse> observer) {

        final String queueUrl = request.getQueueUrl();
        final List<String> messageBodies = request.getMessageBodiesList();
        final List<HqsMessage> messages = database.sendMessageBatch(queueUrl, messageBodies);
        final SendMessageBatchResponse response = SendMessageBatchResponse.newBuilder()
            // TODO .addAllFailed()
            .addAllSuccessful(HqsServiceAdapter.adaptMetadatas(messages))
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("sent {} messages to queue {}", messages.size(), request.getQueueUrl());

        observer.onNext(response);
        observer.onCompleted();
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void receiveMessage(final ReceiveMessageRequest request,
        final StreamObserver<ReceiveMessageResponse> observer) {

        final String queueUrl = request.getQueueUrl();
        final int messageCount = Math.max(1, request.getMaxNumberOfMessages());
        final List<HqsMessage> messages = database.receiveMessages(queueUrl, messageCount);
        final ReceiveMessageResponse response = ReceiveMessageResponse.newBuilder()
            .addAllMessage(HqsServiceAdapter.adaptMessages(messages))
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("received {} messages from queue {}", messages.size(), request.getQueueUrl());

        observer.onNext(response);
        observer.onCompleted();

    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void deleteMessage(final DeleteMessageRequest request,
        final StreamObserver<DeleteMessageResponse> observer) {

        final String receiptHandle = request.getReceiptHandle();
        final Optional<HqsMessage> message = database.deleteMessage(receiptHandle);

        if (message.isPresent()) {
            final DeleteMessageResponse response = DeleteMessageResponse.newBuilder()
                .setRequestId(HqsServiceAdapter.generateRequestId())
                .build();

            logger.debug("deleted message from queue {}: {}", request.getQueueUrl(), message);
            observer.onNext(response);
            observer.onCompleted();

        } else {
            logger.debug("failed to delete message from queue {}", request.getQueueUrl());
            observer.onError(new CouldNotDeleteMessageError(receiptHandle));
        }
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void deleteMessageBatch(final DeleteMessageBatchRequest request,
        final StreamObserver<DeleteMessageBatchResponse> observer) {

        final List<HqsMessage> messages = database.deleteMessageBatch(request.getReceiptHandlesList());
        final Set<String> successful = Streams.extract(messages, HqsMessage::getIdentifier);
        final DeleteMessageBatchResponse response = DeleteMessageBatchResponse.newBuilder()
            .addAllFailed(Streams.disjunction(request.getReceiptHandlesList(), successful))
            .addAllSuccessful(successful)
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("deleted {} messages from queue {}", messages.size(), request.getQueueUrl());

        observer.onNext(response);
        observer.onCompleted();
    }
}
