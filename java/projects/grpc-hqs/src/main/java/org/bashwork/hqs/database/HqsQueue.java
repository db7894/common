package org.bashwork.hqs.database;

import java.text.MessageFormat;
import java.util.Objects;

/**
 * Represents a
 */
public class HqsQueue {

    private final String name;
    private final String url;
    // created time

    private HqsQueue(Builder builder) {
        this.name = builder.name;
        this.url = builder.url;
    }

    public String getName() {
        return name;
    }

    public String getUrl() {
        return url;
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
            && Objects.equals(this.url, that.url);
    }

    @Override
    public int hashCode() {
        return Objects.hash(url, name);
    }

    @Override
    public String toString() {
        return MessageFormat.format("{0} -> {1}", name, url);
    }

    public static Builder newBuilder() {
        return new Builder();
    }

    public final static class Builder {
        private Builder() { }

        private String name;
        private String url;

        public Builder setName(String name) {
            this.name = name;
            return this;
        }

        public Builder setUrl(String url) {
            this.url = url;
            return this;
        }

        public HqsQueue build() {
            Objects.requireNonNull(name, "Queue name is required");
            Objects.requireNonNull(url, "Queue url is required");
            return new HqsQueue(this);
        }
    }
}