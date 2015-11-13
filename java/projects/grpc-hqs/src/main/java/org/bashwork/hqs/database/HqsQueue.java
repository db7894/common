package org.bashwork.hqs.database;

import java.time.Duration;
import java.time.Instant;
import java.util.Objects;

/**
 * Represents an internal queue and its attribute.
 */
public class HqsQueue {

    private final String name;
    private final String url;
    private final Instant createdTime;
    private final Long visibilityTimeout;
    private final Long messageDelay;
    // TODO message retention policy
    // TODO max message size
    // TODO re-drive policy
    // TODO dead letter queue

    private HqsQueue(Builder builder) {
        this.name = builder.name;
        this.url = builder.url;
        this.createdTime = builder.createdTime;
        this.visibilityTimeout = builder.visibilityTimeout;
        this.messageDelay = builder.messageDelay;
    }

    public String getName() {
        return name;
    }

    public String getUrl() {
        return url;
    }

    public Instant getCreatedTime() {
        return createdTime;
    }

    public Long getVisibilityTimeout() {
        return visibilityTimeout;
    }

    public Long getMessageDelay() {
        return messageDelay;
    }

    @Override
    public boolean equals(final Object other) {
        if (this == other) {
            return true;
        }
        if (other == null || getClass() != other.getClass()) {
            return false;
        }
        final HqsQueue that = (HqsQueue) other;
        return Objects.equals(this.name, that.name)
            && Objects.equals(this.url, that.url)
            && Objects.equals(this.createdTime, that.createdTime)
            && Objects.equals(this.visibilityTimeout, that.visibilityTimeout)
            && Objects.equals(this.messageDelay, that.messageDelay);
    }

    @Override
    public int hashCode() {
        return Objects.hash(url, name, createdTime, visibilityTimeout, messageDelay);
    }

    @Override
    public String toString() {
        return new StringBuilder()
            .append("HqsQueue{ ")
            .append("url=").append(url)
            .append(", name=").append(name)
            .append(", createdTime=").append(createdTime)
            .append(", visibilityTimeout=").append(visibilityTimeout)
            .append(", messageDelay=").append(messageDelay)
            .append(" }")
            .toString();
    }

    public static Builder newBuilder() {
        return new Builder();
    }

    public final static class Builder {
        private Builder() { }

        private String name;
        private String url;
        private Instant createdTime;
        private Long visibilityTimeout;
        private Long messageDelay;

        public Builder setName(String name) {
            this.name = name;
            return this;
        }

        public Builder setUrl(String url) {
            this.url = url;
            return this;
        }

        public Builder setCreatedTime(Instant createdTime) {
            this.createdTime = createdTime;
            return this;
        }

        public Builder setVisibilityTimeout(Duration visibilityTimeout) {
            this.visibilityTimeout = visibilityTimeout.getSeconds();
            return this;
        }

        public Builder setMessageDelay(Duration messageDelay) {
            this.messageDelay = messageDelay.getSeconds();
            return this;
        }

        public HqsQueue build() {
            Objects.requireNonNull(name, "Queue name is required");
            Objects.requireNonNull(url, "Queue url is required");
            Objects.requireNonNull(createdTime, "Queue created time is required");
            Objects.requireNonNull(visibilityTimeout, "Queue visibility timeout is required");
            Objects.requireNonNull(messageDelay, "Queue message delay is required");
            return new HqsQueue(this);
        }
    }
}