package com.bashwork.commons.worker.source;

import static org.apache.commons.lang3.Validate.notBlank;
import static org.apache.commons.lang3.Validate.notNull;

import com.bashwork.commons.worker.model.AmazonSnsDTO;
import com.bashwork.commons.serialize.JsonStringSerializer;
import com.bashwork.commons.serialize.StringSerializer;
import com.bashwork.commons.worker.EventSource;
import com.bashwork.commons.worker.TaggedType;
import com.amazonaws.services.sqs.AmazonSQS;
import com.amazonaws.services.sqs.model.DeleteMessageRequest;
import com.amazonaws.services.sqs.model.Message;
import com.amazonaws.services.sqs.model.ReceiveMessageRequest;
import com.amazonaws.services.sqs.model.ReceiveMessageResult;
import com.google.common.base.Optional;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * A supplier that backs up to SNS messages published to SQS.
 * 
 * @param <TEvent> The type of event this supplier will generate
 */
public class SnsEventSource<TEvent> implements EventSource<TaggedType<TEvent>> {
    
    static final Logger logger = LogManager.getLogger(SnsEventSource.class);
    private final Class<TEvent> eventClass; 
    private final AmazonSQS client;
    private final String queueUrl;
    private final StringSerializer serializer;
    
    /**
     * Initializes a new instance of the the SnsEventSupplier.
     * 
     * @param client The client to operate with.
     * @param queueUrl The queue to query from.
     * @param eventClass The class to deserialize with
     */
    public SnsEventSource(AmazonSQS client, String queueUrl, Class<TEvent> eventClass) {
        this(client, queueUrl, eventClass, new JsonStringSerializer());
    }
    
    /**
     * Initializes a new instance of the the SnsEventSupplier.
     * 
     * @param client The client to operate with.
     * @param queueUrl The queue to query from.
     * @param eventClass The class to deserialize with
     * @param serializer The serializer to operate with.
     */
    public SnsEventSource(AmazonSQS client, String queueUrl,
        Class<TEvent> eventClass, StringSerializer serializer) {
        
        this.client = notNull(client, "Must supply a valid SQS client");
        this.queueUrl = notBlank(queueUrl, "Must supply a valid SQS url");
        this.eventClass = notNull(eventClass, "Must supply a valid event class");
        this.serializer = notNull(serializer, "Must supply a valid serializer");
    }    

    /**
     * @see EventSource#get()
     */
    @Override
    public Optional<TaggedType<TEvent>> get() {
        return decode(poll());
    }
    
    /**
     * @see EventSource#confirm(Object)
     */
    @Override
    public void confirm(TaggedType<TEvent> message) {
        delete(message.getTag());        
    }    
    
    /**
     * Retrieves a single message from SQS if one
     * exists, otherwise it returns null if no message is
     * received in a timely manner.
     * 
     * @return The next available message or null.
     */
    private Optional<Message> poll() {
        ReceiveMessageRequest request = new ReceiveMessageRequest()
            .withQueueUrl(queueUrl)
            .withMaxNumberOfMessages(1)
            .withWaitTimeSeconds(20);

        ReceiveMessageResult result = client.receiveMessage(request);

        if (result.getMessages().isEmpty()) {
            return Optional.absent();
        }
        
        return Optional.of(result.getMessages().get(0));
    }
    
    /**
     * Deletes the message with the supplied message handle from
     * the underlying SQS queue.
     * 
     * @param id The identifier of the message to delete.
     */
    private void delete(String id) {
        logger.trace("Deleting sqs message {}", id);
        DeleteMessageRequest request = new DeleteMessageRequest(queueUrl, id);
        client.deleteMessage(request);
    }
    
    /**
     * Given a wrapped SNS message, decode it to the underlying
     * event message.
     * 
     * @param handle The SNS message that might exist
     * @return The decoded message if available.
     */
    protected Optional<TaggedType<TEvent>> decode(Optional<Message> handle) {
        
        if (handle.isPresent()) {       
            try {
                Message message = handle.get();        
                String id = message.getReceiptHandle();
                String strPayload = message.getBody();
                AmazonSnsDTO payload = serializer.deserialize(strPayload, AmazonSnsDTO.class);
                TEvent decoded = serializer.deserialize(payload.getMessage(), eventClass);
                
                return Optional.of(TaggedType.create(decoded, id));
            } catch (Exception ex) {
                logger.error("Failed to decode the supplied message", ex);
            }
        }
        return Optional.absent();
    }
}
