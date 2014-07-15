package com.bashwork.akka.config.actor

import akka.actor.ActorSystem
import akka.actor.Props
import com.bashwork.akka.actor.MessagePumpActor
import com.bashwork.akka.actor.SystemShutdownActor
import com.bashwork.akka.config.akka.GuiceAkkaExtension
import com.google.inject.Inject
import net.codingwell.scalaguice.ScalaModule

/**
 * The logic required to initialize and start the actor
 * processing system.
 */
class ActorServiceMain @Inject()(system: ActorSystem) {
  
  import com.bashwork.akka.actor.MessagePumpActor._
  
  val pumper = system.actorOf(GuiceAkkaExtension(system).props(classOf[MessagePumpActor]), "pumper")
  
  /**
   * The main starting point for the actor message pump
   * service.
   */
  def start {

    system.actorOf(Props(classOf[SystemShutdownActor], pumper), "pumper-watcher")
    pumper ! StartPump // start the long poll
  }
}

/**
 * The main dependencies for the actor processing system.
 */
class ActorServiceMainModule extends ScalaModule {
  
  override def configure() {
    bind[ActorServiceMain]
  }
}
