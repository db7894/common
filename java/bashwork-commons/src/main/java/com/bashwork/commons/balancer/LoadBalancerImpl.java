package com.bashwork.commons.balancer;

import java.util.List;
import java.util.concurrent.ConcurrentMap;
import java.util.stream.Collectors;
import java.util.concurrent.ConcurrentHashMap;

/**
 * Implementation of a simple load balancer that will route
 * a new connection to one of the supplied servers.
 */
public class LoadBalancerImpl implements LoadBalancer {

    private final ConcurrentMap<Server, Integer> servers;
    private final ConcurrentMap<Connection, Server> connections = new ConcurrentHashMap<>();
    private final int maxConnections;

    /**
     * @param maxConnections The maximum allowed connections per server
     */
    public LoadBalancerImpl(List<Server> servers, int maxConnections) {
        this.maxConnections = maxConnections;
        this.servers = servers.stream().collect(Collectors.toConcurrentMap(s -> s, s -> 0));
    }

    /**
     * @see LoadBalancer#assign(Connection)
     */
    public synchronized Server assign(Connection connection) {
        final Server server = getMinimimumConnections(servers);
        if (servers.get(server) >= maxConnections) {
            throw new RuntimeException("too many active connections");
        }

        servers.put(server, servers.get(server) + 1);
        connections.put(connection, server);
        return server;
    }

    /**
     * @see LoadBalancer#release(Connection)
     */
    public void release(Connection connection) {
        final Server server = connections.remove(connection);
        if (server == null) {
            throw new RuntimeException("connection removed that doesn't exist");
        }

        boolean finished = false;
        while (!finished) {
            Integer count = servers.get(server);
            finished = servers.replace(server, count, count - 1);
        }
    }

    /**
     * The strategy to choose which server to assign next.
     *
     * @param servers The collection of servers to choose from.
     * @return The server chosen for the next client.
     */
    private static Server getMinimimumConnections(ConcurrentMap<Server, Integer> servers) {
        return servers.entrySet().stream()
            .min((s1, s2) -> s1.getValue().compareTo(s2.getValue()))
            .get().getKey();
    }
}
