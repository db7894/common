package com.bashwork.akka.config.akka

import akka.actor.ActorRefFactory
import akka.actor.ActorSystem
import com.google.inject.AbstractModule
import com.google.inject.Inject
import com.google.inject.Injector
import com.google.inject.Provider
import com.google.inject.Provides
import com.google.inject.Singleton
import com.typesafe.config.Config
import net.codingwell.scalaguice.ScalaModule

/**
 * Singleton wrapper for the various dependency providers
 * for the akka actor system. 
 * TODO, drop this below.
 */
object AkkaSystemModule {
  
  class ActorSystemProvider @Inject() (val config: Config, val injector: Injector) extends Provider[ActorSystem] {
    override def get() = {
      val system = ActorSystem("main-actor-system", config)
      GuiceAkkaExtension(system).initialize(injector)
      system
    }
  }
}

/**
 * A module providing an Akka ActorSystem.
 */
class AkkaSystemModule extends AbstractModule with ScalaModule {

  import AkkaSystemModule._
  
  override def configure() {
    bind[ActorSystem].toProvider[ActorSystemProvider].asEagerSingleton()
  }
  
  /**
   * Binds ActorRefFactory to the root ActorSystem.  Note the the single binding
   * to ActorSystem is not enough because Guice will only inject exact type matches
   */
  @Provides @Singleton
  def provideActorRefFactory(provider: Provider[ActorSystem]): ActorRefFactory = {
    provider.get
  }
}
