package org.bashwork.hqs.service;

import static org.bashwork.hqs.service.HqsConstraints.*;

import org.bashwork.hqs.protocol.*;
import org.bashwork.hqs.service.errors.InvalidRequestError;

import java.util.List;
import java.util.Map;

/**
 * Collection of validators that can be used to check incoming
 * requests for validity.
 */
public final class HqsServiceValidator {
    private HqsServiceValidator() { }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final CreateQueueRequest request) {
        isNotEmpty(request.getQueueName(), "QueueName");
        isPositive(request.getDelayInSeconds(), "DelayInSeconds");
        isPositive(request.getVisibilityInSeconds(), "VisibilityTimeoutInSeconds");
        isPositive(request.getMaxMessageSize(), "MaxMessageSize");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final GetQueueAttributesRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final SetQueueAttributesRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
        isPositive(request.getDelayInSeconds(), "DelayInSeconds");
        isPositive(request.getVisibilityInSeconds(), "VisibilityTimeoutInSeconds");
        isPositive(request.getMaxMessageSize(), "MaxMessageSize");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final DeleteQueueRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final ListQueuesRequest request) {
        isNotNull(request.getQueueNamePrefix(), "QueueNamePrefix");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final PurgeQueueRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final GetQueueUrlRequest request) {
        isNotEmpty(request.getQueueName(), "QueueName");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final SendMessageRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
        isNotEmpty(request.getMessageBody(), "MessageBody");
        isNotEmpty(request.getAttributes(), "Attributes");
        isPositive(request.getDelayInSeconds(), "DelayInSeconds");
        isLessThan(request.getDelayInSeconds(), MAX_VISIBILITY, "DelayInSeconds");
        isLessThan(request.getMessageBody().length(), MAX_MESSAGE_SIZE, "MessageBody Size");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final SendMessageBatchRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
        isValidSendEntries(request.getEntriesList(), "Entries");
        isLessThan(request.getEntriesCount(), MAX_MESSAGE_COUNT, "Entries Count");
    }

    /**
     * Validate the supplied request.
     *
     * @param values The values to validate.
     * @param name The name of the field to validate.
     */
    private static void isValidSendEntries(final List<SendMessageEntry> values, final String name) {
        if (values == null) {
            throw new InvalidRequestError(name);
        }
        values.forEach(HqsServiceValidator::validate);
    }

    /**
     * Validate the supplied request.
     *
     * @param entry The request to validate.
     */
    private static void validate(final SendMessageEntry entry) {
        isNotEmpty(entry.getAttributes(), "Attributes");
        isNotEmpty(entry.getId(), "Identifier");
        isNotEmpty(entry.getMessageBody(), "MessageBody");
        isPositive(entry.getDelayInSeconds(), "DelayInSeconds");
        isLessThan(entry.getDelayInSeconds(), MAX_VISIBILITY, "DelayInSeconds");
        isLessThan(entry.getMessageBody().length(), MAX_MESSAGE_SIZE, "MessageBody Size");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final ReceiveMessageRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
        isPositive(request.getMaxNumberOfMessages(), "MaxNumberOfMessages");
        isPositive(request.getVisibilityInSeconds(), "VisibilityTimeoutInSeconds");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final ChangeMessageVisibilityRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
        isNotEmpty(request.getReceiptHandle(), "ReceiptHandle");
        isPositive(request.getVisibilityInSeconds(), "VisibilityTimeoutInSeconds");
        isLessThan(request.getVisibilityInSeconds(), MAX_VISIBILITY, "VisibilityTimeoutInSeconds");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final ChangeMessageVisibilityBatchRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
        isValidChangeEntries(request.getEntriesList(), "Entries");
        isLessThan(request.getEntriesCount(), MAX_MESSAGE_COUNT, "Entries Count");
    }

    /**
     * Validate the supplied request.
     *
     * @param entry The request to validate.
     */
    private static void validate(final ChangeMessageVisibilityEntry entry) {

        isNotEmpty(entry.getReceiptHandle(), "ReceiptHandle");
        isPositive(entry.getVisibilityInSeconds(), "VisibilityTimeoutInSeconds");
        isLessThan(entry.getVisibilityInSeconds(), MAX_VISIBILITY, "VisibilityTimeoutInSeconds");
    }

    /**
     * Validate the supplied request.
     *
     * @param values The values to validate.
     * @param name The name of the field to validate.
     */
    private static void isValidChangeEntries(final List<ChangeMessageVisibilityEntry> values, final String name) {
        if (values == null) {
            throw new InvalidRequestError(name);
        }
        values.forEach(HqsServiceValidator::validate);
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final DeleteMessageRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
        isNotEmpty(request.getReceiptHandle(), "ReceiptHandle");
    }

    /**
     * Validate the supplied request.
     *
     * @param request The request to validate.
     */
    public static void validate(final DeleteMessageBatchRequest request) {
        isNotEmpty(request.getQueueUrl(), "QueueUrl");
        isNotEmpty(request.getReceiptHandlesList(), "ReceiptHandles");
    }

    /**
     * Validate that the supplied string is not null.
     *
     * @param value The value of the string to validate.
     * @param name The name of the field to validate.
     */
    private static void isNotNull(final String value, final String name) {
        if (value == null) {
            throw new InvalidRequestError(name);
        }
    }

    /**
     * Validate that the supplied string is valid.
     *
     * @param value The value of the string to validate.
     * @param name The name of the field to validate.
     */
    private static void isNotEmpty(final String value, final String name) {
        if (value == null || value.isEmpty()) {
            throw new InvalidRequestError(name);
        }
    }

    /**
     * Validate that the supplied string is valid.
     *
     * @param values The value of the string to validate.
     * @param name The name of the field to validate.
     */
    private static void isNotEmpty(final List<String> values, final String name) {
        if (values == null) {
            throw new InvalidRequestError(name);
        }
        values.forEach(value -> isNotEmpty(value, name));
    }

    /**
     * Validate that the supplied string is valid.
     *
     * @param values The value of the string to validate.
     * @param name The name of the field to validate.
     */
    private static void isNotEmpty(final Map<String, String> values, final String name) {
        if (values == null) {
            throw new InvalidRequestError(name);
        }

        values.keySet().forEach(value -> isNotEmpty(value, name));
        values.values().forEach(value -> isNotEmpty(value, name));
    }

    /**
     * Validate that the supplied value is positive.
     *
     * @param value The value to test for being positive.
     * @param name The name of the value to test.
     */
    private static void isPositive(final int value, final String name) {
        if (value < 0) {
            throw new InvalidRequestError(name);
        }
    }

    /**
     * Validate that the supplied value is less than its maximum limit.
     *
     * @param value The field value to validate.
     * @param limit The maximum value for this field
     * @param name The name of the value to test.
     */
    private static void isLessThan(final int value, final int limit, final String name) {
        if (value > limit) {
            throw new InvalidRequestError(name);
        }
    }
}
