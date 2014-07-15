package com.bashwork.akka.actor

import akka.actor.ActorRef
import akka.actor.Actor
import akka.actor.ActorLogging
import akka.actor.Terminated

class SystemShutdownActor(actor: ActorRef) extends Actor with ActorLogging {
  context watch actor
    
  def receive = {
    case Terminated(_) => terminated()
  }
  
  /**
   * This occurs when the main actor has shut down, as
   * a result we simply shut down the entire akka system.
   */
  def terminated() {  
    log.info(s"${actor.path} has terminated, shutting down system")
    context.system.shutdown()
  }
}
