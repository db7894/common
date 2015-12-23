package org.bashwork.hqs.service;

import static java.util.Objects.requireNonNull;
import static org.bashwork.hqs.service.HqsServiceValidator.validate;

import java.util.Optional;
import java.util.List;
import java.util.Set;

import org.bashwork.hqs.protocol.*;
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

        validate(request);
        final Optional<HqsQueue> queue = database.createQueue(request);

        if (!queue.isPresent()) {
            logger.debug("failed to create queue {}", request.getQueueName());
            throw new QueueDoesNotExistError(request.getQueueName());
        }

        final CreateQueueResponse response = CreateQueueResponse.newBuilder()
            .setQueueUrl(queue.get().getUrl())
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("created queue {}: {}", request.getQueueName(), queue);
        observer.onNext(response);
        observer.onCompleted();
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void deleteQueue(final DeleteQueueRequest request,
        final StreamObserver<DeleteQueueResponse> observer) {

        validate(request);
        final boolean isDeleted = database.deleteQueue(request);

        if (!isDeleted) {
            logger.error("failed to delete queue {}", request.getQueueUrl());
            throw new QueueDoesNotExistError(request.getQueueUrl());
        }

        final DeleteQueueResponse response = DeleteQueueResponse.newBuilder()
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("deleted queue {}", request.getQueueUrl());
        observer.onNext(response);
        observer.onCompleted();
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void listQueues(final ListQueuesRequest request,
        final StreamObserver<ListQueuesResponse> observer) {

        validate(request);
        final List<HqsQueue> queues = database.listQueues(request);
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

        validate(request);
        final boolean isPurged = database.purgeQueue(request);

        if (!isPurged) {
            logger.error("failed to purge messages from queue {}", request.getQueueUrl());
            throw new QueueDoesNotExistError(request.getQueueUrl());
        }

        final PurgeQueueResponse response = PurgeQueueResponse.newBuilder()
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("purged messages from {}", request.getQueueUrl());
        observer.onNext(response);
        observer.onCompleted();
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void getQueueUrl(final GetQueueUrlRequest request,
        final StreamObserver<GetQueueUrlResponse> observer) {

        validate(request);
        final Optional<HqsQueue> queue = database.getQueue(request);

        if (!queue.isPresent()) {
            logger.error("failed to retrieve queue {}", request.getQueueName());
            throw new QueueDoesNotExistError(request.getQueueName());
        }

        final GetQueueUrlResponse response = GetQueueUrlResponse.newBuilder()
            .setQueueUrl(queue.get().getUrl())
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("got the queue url for {}: {}", request.getQueueName(), queue);
        observer.onNext(response);
        observer.onCompleted();
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void sendMessage(final SendMessageRequest request,
        final StreamObserver<SendMessageResponse> observer) {

        validate(request);
        final Optional<HqsMessage> message = database.sendMessage(request);

        if (!message.isPresent()) {
            logger.error("failed to send message to queue {}", request.getQueueUrl());
            throw new CouldNotSendMessageError(request.getMessageBody());
        }

        final SendMessageResponse response = SendMessageResponse.newBuilder()
            .setMetadata(HqsServiceAdapter.adaptMetadata(message.get()))
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("sent the message to queue {}: {}", request.getQueueUrl(), message);
        observer.onNext(response);
        observer.onCompleted();
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void sendMessageBatch(final SendMessageBatchRequest request,
        final StreamObserver<SendMessageBatchResponse> observer) {

        validate(request);
        final List<HqsMessage> messages = database.sendMessageBatch(request);
        final Set<String> possible = Streams.extract(request.getEntriesList(), SendMessageEntry::getId);
        final Set<String> successful = Streams.extract(messages, HqsMessage::getIdentifier);
        final SendMessageBatchResponse response = SendMessageBatchResponse.newBuilder()
            .addAllFailed(Streams.disjunction(possible, successful))
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

        validate(request);
        final List<HqsMessage> messages = database.receiveMessages(request);
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
    public void changeMessageVisibility(final ChangeMessageVisibilityRequest request,
        final StreamObserver<ChangeMessageVisibilityResponse> observer) {

        validate(request);
        final Optional<HqsMessage> message = database.changeMessageVisibility(request);

        if (!message.isPresent()) {
            logger.error("failed to change message visibility: {}", request.getReceiptHandle());
            throw new CouldNotDeleteMessageError(request.getReceiptHandle());
        }

        final ChangeMessageVisibilityResponse response = ChangeMessageVisibilityResponse.newBuilder()
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("changed message visibility for message: {}", request.getReceiptHandle());

        observer.onNext(response);
        observer.onCompleted();
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void changeMessageVisibilityBatch(final ChangeMessageVisibilityBatchRequest request,
        final StreamObserver<ChangeMessageVisibilityResponse> observer) {

        validate(request);

        final ChangeMessageVisibilityResponse response = ChangeMessageVisibilityResponse.newBuilder()
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("changed message visibility for {} messages", request.getEntriesCount());

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

        validate(request);
        final Optional<HqsMessage> message = database.deleteMessage(request);

        if (!message.isPresent()) {
            logger.error("failed to delete message from queue {}", request.getQueueUrl());
            throw new CouldNotDeleteMessageError(request.getReceiptHandle());
        }

        final DeleteMessageResponse response = DeleteMessageResponse.newBuilder()
            .setRequestId(HqsServiceAdapter.generateRequestId())
            .build();

        logger.debug("deleted message from queue {}: {}", request.getQueueUrl(), message);
        observer.onNext(response);
        observer.onCompleted();
    }

    /**
     *
     * @param request The request to handle.
     * @param observer The observer to write the response to.
     */
    @Override
    public void deleteMessageBatch(final DeleteMessageBatchRequest request,
        final StreamObserver<DeleteMessageBatchResponse> observer) {

        validate(request);
        final List<HqsMessage> messages = database.deleteMessageBatch(request);
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
