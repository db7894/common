package org.bashwork.hqs.configuration;

import com.google.inject.AbstractModule;
import com.google.inject.name.Names;
import org.bashwork.hqs.ClientMain;

/**
 * The configuration for the Hqs Server
 */
public class HqsClientModule extends AbstractModule {

    @Override
    protected void configure() {
        bind(Integer.class).annotatedWith(Names.named("ServerPort")).toInstance(8081);
        bind(String.class).annotatedWith(Names.named("ServerHost")).toInstance("localhost");
        bind(ClientMain.class).asEagerSingleton();
    }
}
