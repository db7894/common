package org.bashwork.hqs.database;

import java.text.MessageFormat;
import java.util.Objects;

/**
 */
public class HqsMessage {

    private final String body;
    private final String md5HashOfBody;
    private final String identifier;
    private final int receiveCount;
    // sent timestamp
    // sender of message
    // date first touched
    // md5 of message, md5 of attributes
    // message id and receipt handle
    // attributes key value

    private HqsMessage(Builder builder) {
        this.body = builder.body;
        this.md5HashOfBody = builder.md5HashOfBody;
        this.identifier = builder.identifier;
        this.receiveCount = builder.receiveCount;
    }

    public String getBody() {
        return body;
    }

    public String getMd5HashOfBody() {
        return md5HashOfBody;
    }

    public String getIdentifier() {
        return identifier;
    }

    public int getReceiveCount() {
        return receiveCount;
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
            && Objects.equals(this.md5HashOfBody, that.md5HashOfBody)
            && Objects.equals(this.identifier, that.identifier);
    }

    @Override
    public int hashCode() {
        return Objects.hash(body, md5HashOfBody, identifier, receiveCount);
    }


    @Override
    public String toString() {
        return MessageFormat.format("( body={0}, hash={1}, identifier={2} )",
            body, md5HashOfBody, identifier);
    }

    public static Builder newBuilder() {
        return new Builder();
    }

    public final static class Builder {
        private Builder() { }

        private String body;
        private String md5HashOfBody;
        private String identifier;
        private int receiveCount;

        public Builder setBody(String body) {
            this.body = body;
            return this;
        }

        public Builder setMd5HashOfBody(String md5HashOfBody) {
            this.md5HashOfBody = md5HashOfBody;
            return this;
        }

        public Builder setIdentifier(String identifier) {
            this.identifier = identifier;
            return this;
        }

        public Builder setReceiveCount(int receiveCount) {
            this.receiveCount = receiveCount;
            return this;
        }

        public HqsMessage build() {
            Objects.requireNonNull(body, "Message body is required");
            Objects.requireNonNull(identifier, "Message identifier is required");
            return new HqsMessage(this);
        }
    }
}