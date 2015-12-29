package org.bashwork.hqs.service;

import java.time.Duration;

/**
 * A collection of constraints for the system based on the
 * interface setup by the AWS implementation.
 */
public final class HqsConstraints {
    private HqsConstraints() { }

    /**
     * The maximum size in bytes for a given message payload
     */
    public static final int MAX_MESSAGE_SIZE = 262144;

    /**
     * The maximum number of messages that can be sent in a single batch
     * request.
     */
    public static final int MAX_MESSAGE_COUNT = 10;

    /**
     * The maximum amount of time that a message can be delayed or
     * be made invisible.
     */
    public static final int MAX_VISIBILITY = (int) Duration.ofHours(12).getSeconds();

    /**
     * The maximum number of messages that can be enqueued on a
     * single queue at one time.
     */
    public static final int MAX_MESSAGES_ENQUEUED = 120000;

    /**
     * The maximum number of messages that can be returned on a
     * list queue request.
     */
    public static final int MAX_LISTED_QUEUE_NAMES = 1000;
}
