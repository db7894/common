package com.bashwork.akka.config.service

import akka.actor.ActorRef
import akka.actor.ActorSystem
import akka.io.IO
import com.bashwork.akka.config.akka.GuiceAkkaExtension
import com.bashwork.akka.service.BashworkServiceActor
import com.google.inject.Inject
import com.google.inject.Provides
import com.google.inject.Singleton
import com.google.inject.name.Named
import com.typesafe.config.Config
import net.codingwell.scalaguice.ScalaModule
import spray.can.Http

/**
 * 
 */
class ApiServiceMain @Inject()(config: Config, @Named("BashworkServiceActor") service: ActorRef)
  (implicit actorSystem: ActorSystem) {
  
  val host = config.getString("server.host")
  val port = config.getInt("server.port") 
  
  def start {
    IO(Http) ! Http.Bind(service, interface = host, port = port)
  }
}

/**
 * 
 */
class ApiServiceMainModule extends ScalaModule {
  
  def configure {
    bind[ApiServiceMain]
  }
  
  @Provides
  @Singleton
  @Named("BashworkServiceActor")
  def provideApiRouterActorRef(system: ActorSystem): ActorRef = {
    system.actorOf(GuiceAkkaExtension(system).props(classOf[BashworkServiceActor]), "service")
  }
}
