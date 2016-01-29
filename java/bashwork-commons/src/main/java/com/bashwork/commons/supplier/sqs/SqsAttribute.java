package com.bashwork.commons.supplier.sqs;

/**
 * A collection of SQS attribute names that are stored
 * with each sent message.
 */
public final class SqsAttribute {
    private SqsAttribute() { }

    public static final String STRING = "String";
    public static final String NUMBER = "Number";
    public static final String BINARY = "Binary";

    public static final String WORKFLOW_TYPE = "workflow-type";
    public static final String ALL = "All";
    public static final String RETRY_COUNT = "ApproximateReceiveCount";
    public static final String SENT_TIMESTAMP = "SentTimestamp";
    public static final String RECEIVED_TIMESTAMP = "ApproximateFirstReceiveTimestamp";
}
