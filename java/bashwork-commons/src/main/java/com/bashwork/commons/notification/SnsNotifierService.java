package com.bashwork.commons.notification;

import static com.bashwork.commons.utility.Validate.notNull;

import com.amazonaws.services.sns.AmazonSNS;
import com.amazonaws.services.sns.model.PublishRequest;
import com.amazonaws.services.sns.model.PublishResult;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * A notifier service that notifies vis SNS topics.
 */
public class SnsNotifierService implements NotifierService {

    private static final Logger logger = LogManager.getLogger(SnsNotifierService.class);
    private static final String PREFIX = "arn:aws:sns:";

    private final AmazonSNS client;


    /**
     * Initialize a new instance of the SnsNotifierService.
     *
     * @param client The SNS client to notify with
     */
    public SnsNotifierService(AmazonSNS client) {
        this.client = notNull(client, "AmazonSNSClient");
    }

    /**
     * @see NotifierService#notify(String, String)
     */
    @Override
    public String notify(String endpoint, String message) throws NotificationException {

        notNull(message, "notification message");
        if (!isEndpointValid(endpoint)) {
            throw new NotificationException("invalid topic supplied: " + endpoint);
        }

        try {
            PublishRequest request = new PublishRequest(endpoint, message);
            logger.trace("AmazonSNS notify request:\n{}", request);

            PublishResult response = client.publish(request);
            logger.trace("AmazonSNS notify response:\n{}", response);

            String identifier = response.getMessageId();
            logger.debug("Published sns message with resulting tracker: {}", identifier);

            return identifier;
        } catch (Exception ex) {
            throw new NotificationException("Failed to send notification message", ex);
        }
    }

    /**
     * @see NotifierService#isEndpointValid(String)
     */
    @Override
    public boolean isEndpointValid(String topic) {
        return topic.startsWith(PREFIX);
    }
}
