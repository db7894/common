package com.bashwork.commons.balancer;

import java.util.Objects;

/**
 * Represents a single server instances
 */
public final class Server {

    private final String name;
    private final String host;
    private final int port;
    private final String id;
    private final String groupId;

    private Server(Builder builder) {
        name = builder.name;
        host = builder.host;
        port = builder.port;
        id = builder.id;
        groupId = builder.groupId;
    }

    public String getName() { return name; }
    public String getHost() { return host; }
    public int getPort() { return port; }
    public String getId() { return id; }
    public String getGroupId() { return groupId; }

    /**
     * @see Object#equals(Object)
     */
    @Override
    public boolean equals(final Object other) {
        if (this == other) {
            return true;
        }

        if (other == null || getClass() != other.getClass()) {
            return false;
        }

        final Server that = (Server) other;
        return Objects.equals(this.port, that.port)
            && Objects.equals(this.name, that.name)
            && Objects.equals(this.host, that.host)
            && Objects.equals(this.id, that.id)
            && Objects.equals(this.groupId, that.groupId);
    }

    /**
     * @see Object#hashCode()
     */
    @Override
    public int hashCode() {
        return Objects.hash(name, host, port, id, groupId);
    }

    /**
     * Return an initialized type builder for this class.
     *
     * @return The builder for this class.
     */
    public static Builder builder() {
        return new Builder();
    }

    /**
     * The type builder for this class.
     */
    public static final class Builder {
        private Builder() { }

        private String name;
        private String host;
        private int port;
        private String id;
        private String groupId;

        public Server build() {
            return new Server(this);
        }
    }
}
