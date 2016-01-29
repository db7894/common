package com.bashwork.commons.testing.framework;

import java.nio.file.Path;
import java.util.Collections;
import java.util.Map;

import static com.bashwork.commons.utility.Validate.notBlank;
import static com.bashwork.commons.utility.Validate.notNull;

/**
 * Manages the test context for a single data drive test.
 */
public final class TestContext {

    private final Path path;
    private final String description;
    private final boolean success;
    private final Map<String, String> features;

    private TestContext(Builder builder) {
        this.path = builder.path;
        this.description = builder.description;
        this.success = builder.success;
        this.features = builder.features;
    }

    public Path getPath() {
        return path;
    }

    public String getDescription() {
        return description;
    }

    public boolean isSuccess() {
        return success;
    }

    public Map<String, String> getFeatures() {
        return features;
    }

    public static Builder builder() {
        return new Builder();
    }

    public static final class Builder {
        private Builder() { }

        private Path path;
        private String description;
        private boolean success;
        private Map<String, String> features;

        public Builder withPath(Path path) {
            this.path = path;
            return this;
        }

        public Builder withDescription(String description) {
            this.description = description;
            return this;
        }

        public Builder withSuccess(boolean success) {
            this.success = success;
            return this;
        }

        public Builder withFeatures(Map<String, String> features) {
            this.features = features;
            return this;
        }

        public TestContext build() {
            notNull(path, "Path");
            notBlank(description, "Description");
            if (features == null) {
                this.features = Collections.emptyMap();
            }
            return new TestContext(this);
        }
    }
}
