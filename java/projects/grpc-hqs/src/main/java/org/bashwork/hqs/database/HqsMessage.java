package org.bashwork.hqs.database;

import java.time.Instant;
import java.util.Map;
import java.util.Objects;

/**
 */
public class HqsMessage {

    private final String body;
    private final String md5HashOfBody;
    private final String md5HashOfAttributes;
    private final String identifier;
    private final String receiptHandle;
    private final Map<String, String> attributes;
    private final int receiveCount;
    private final Instant sentTime;

    // TODO sender of message
    // TODO date first touched

    private HqsMessage(Builder builder) {
        this.body = builder.body;
        this.md5HashOfBody = builder.md5HashOfBody;
        this.md5HashOfAttributes = builder.md5HashOfAttributes;
        this.attributes = builder.attributes;
        this.identifier = builder.identifier;
        this.receiptHandle = builder.receiptHandle;
        this.receiveCount = builder.receiveCount;
        this.sentTime = builder.sentTime;
    }

    public String getBody() {
        return body;
    }

    public String getMd5HashOfBody() {
        return md5HashOfBody;
    }

    public String getMd5HashOfAttributes() {
        return md5HashOfAttributes;
    }

    public Map<String, String> getAttributes() {
        return attributes;
    }

    public String getIdentifier() {
        return identifier;
    }

    public String getReceiptHandle() {
        return receiptHandle;
    }

    public int getReceiveCount() {
        return receiveCount;
    }

    public Instant getSentTime() {
        return sentTime;
    }

    @Override
    public boolean equals(final Object other) {
        if (this == other) {
            return true;
        }
        if (other == null || getClass() != other.getClass()) {
            return false;
        }
        final HqsMessage that = (HqsMessage) other;
        return Objects.equals(this.receiveCount, that.receiveCount)
            && Objects.equals(this.body, that.body)
            && Objects.equals(this.attributes, that.attributes)
            && Objects.equals(this.md5HashOfBody, that.md5HashOfBody)
            && Objects.equals(this.md5HashOfAttributes, that.md5HashOfAttributes)
            && Objects.equals(this.identifier, that.identifier)
            && Objects.equals(this.receiptHandle, that.receiptHandle)
            && Objects.equals(this.sentTime, that.sentTime);
    }

    @Override
    public int hashCode() {
        return Objects.hash(body, md5HashOfBody, md5HashOfAttributes,
            attributes, identifier, receiveCount, sentTime, receiptHandle);
    }


    @Override
    public String toString() {
        return new StringBuilder()
            .append("HqsMessage{ ")
            .append("body=").append(body)
            .append(", md5HashOfBody=").append(md5HashOfBody)
            .append(", md5HashOfAttributes=").append(md5HashOfAttributes)
            .append(", identifier=").append(identifier)
            .append(", receiptHandle=").append(receiptHandle)
            .append(", receiveCount=").append(receiveCount)
            .append(", attributes=").append(attributes)
            .append(", sentTime=").append(sentTime)
            .append(" }")
            .toString();
    }

    /**
     * Build a new instance of the HqsMessage using an existing message
     * as a template.
     *
     * @param message The message to build from.
     * @return The cloned message builder.
     */
    public static Builder buildFrom(final HqsMessage message) {
        return new Builder()
            .setAttributes(message.getAttributes())
            .setBody(message.getBody())
            .setIdentifier(message.getIdentifier())
            .setMd5HashOfAttributes(message.getMd5HashOfAttributes())
            .setMd5HashOfBody(message.getMd5HashOfBody())
            .setReceiveCount(message.getReceiveCount())
            .setSentTime(message.getSentTime());
    }

    public static Builder newBuilder() {
        return new Builder();
    }

    public final static class Builder {
        private Builder() { }

        private String body;
        private String md5HashOfBody;
        private String md5HashOfAttributes;
        private String identifier;
        private String receiptHandle;
        private int receiveCount;
        private Map<String, String> attributes;
        private Instant sentTime;

        public Builder setBody(String body) {
            this.body = body;
            return this;
        }

        public Builder setMd5HashOfBody(String md5HashOfBody) {
            this.md5HashOfBody = md5HashOfBody;
            return this;
        }

        public Builder setMd5HashOfAttributes(String md5HashOfAttributes) {
            this.md5HashOfAttributes = md5HashOfAttributes;
            return this;
        }

        public Builder setAttributes(Map<String, String> attributes) {
            this.attributes = attributes;
            return this;
        }

        public Builder setIdentifier(String identifier) {
            this.identifier = identifier;
            return this;
        }

        public Builder setRecieptHandle(String receiptHandle) {
            this.receiptHandle = receiptHandle;
            return this;
        }

        public Builder setReceiveCount(int receiveCount) {
            this.receiveCount = receiveCount;
            return this;
        }

        public Builder setSentTime(Instant sentTime) {
            this.sentTime = sentTime;
            return this;
        }

        public HqsMessage build() {
            Objects.requireNonNull(body, "Message body is required");
            Objects.requireNonNull(identifier, "Message identifier is required");
            Objects.requireNonNull(attributes, "Message attributes are required");
            Objects.requireNonNull(md5HashOfBody, "Message body hash is required");
            Objects.requireNonNull(md5HashOfAttributes, "Message attributes hash is required");
            Objects.requireNonNull(sentTime, "Message sent time is required");
            return new HqsMessage(this);
        }
    }
}