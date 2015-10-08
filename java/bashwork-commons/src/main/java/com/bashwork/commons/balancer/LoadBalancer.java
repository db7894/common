package com.bashwork.commons.balancer;

/**
 * An interface to a software load balancer.
 */
public interface LoadBalancer {

    /**
     * Assign the supplied connection to a server based on the internal
     * load balancing policy.
     *
     * @param connection The new connection to assign to a server
     * @return The server the connection was assigned to.
     */
    Server assign(Connection connection);

    /**
     * Release the server that the connection is connected to
     * back into the balancer pool.
     *
     * @param connection The existing connection to release
     */
    void release(Connection connection);
}
