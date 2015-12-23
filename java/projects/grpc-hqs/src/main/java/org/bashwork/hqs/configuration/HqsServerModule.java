package org.bashwork.hqs.configuration;

import com.google.inject.AbstractModule;
import com.google.inject.name.Names;
import org.bashwork.hqs.protocol.HqsGrpc;
import org.bashwork.hqs.service.HqsService;
import org.bashwork.hqs.ServerMain;
import org.bashwork.hqs.database.HqsDatabase;
import org.bashwork.hqs.database.HqsDatabaseImpl;

import com.codahale.metrics.MetricRegistry;
import com.codahale.metrics.JmxReporter;

import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;

/**
 * The configuration for the Hqs Server
 */
public class HqsServerModule extends AbstractModule {
    private static final int cores = Runtime.getRuntime().availableProcessors();

    @Override
    protected void configure() {
        bind(Integer.class).annotatedWith(Names.named("ServerPort")).toInstance(8081);
        bind(String.class).annotatedWith(Names.named("ServerHost")).toInstance("localhost");
        bind(HqsDatabase.class).to(HqsDatabaseImpl.class);
        bind(HqsGrpc.Hqs.class).to(HqsService.class);
        bind(ScheduledExecutorService.class).toInstance(Executors.newScheduledThreadPool(cores));
        bind(MetricRegistry.class).toInstance(getMetrics());
        bind(ServerMain.class).asEagerSingleton();
    }

    private MetricRegistry getMetrics() {
        final MetricRegistry registry = new MetricRegistry();

        JmxReporter.forRegistry(registry)
            .build().start();

        return registry;
    }
}
