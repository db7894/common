package org.bashwork.hqs.database;

import java.time.Instant;
import java.util.Objects;

/**
 * Represents a
 */
public class HqsQueue {

    private final String name;
    private final String url;
    private final Instant createdTime;
    // TODO visibility timeout
    // TODO message retention policy
    // TODO max message size
    // TODO default delay
    // TODO redrive policy
    // TODO dead letter queue

    private HqsQueue(Builder builder) {
        this.name = builder.name;
        this.url = builder.url;
        this.createdTime = builder.createdTime;
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
            && Objects.equals(this.createdTime, that.createdTime);
    }

    @Override
    public int hashCode() {
        return Objects.hash(url, name, createdTime);
    }

    @Override
    public String toString() {
        return new StringBuilder()
            .append("HqsQueue{ ")
            .append("url=").append(url)
            .append(", name=").append(name)
            .append(", createdTime=").append(createdTime)
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

        public HqsQueue build() {
            Objects.requireNonNull(name, "Queue name is required");
            Objects.requireNonNull(url, "Queue url is required");
            Objects.requireNonNull(createdTime, "Queue created time is required");
            return new HqsQueue(this);
        }
    }
}