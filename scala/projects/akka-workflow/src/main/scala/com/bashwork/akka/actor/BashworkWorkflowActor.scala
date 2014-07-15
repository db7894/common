package com.bashwork.akka.actor

import akka.actor.Actor
import akka.actor.ActorLogging
import akka.actor.ActorRef
import akka.actor.Props
import akka.event.EventBus
import akka.event.LookupClassification
import com.amazonaws.services.sqs.model.Message
import com.bashwork.akka.dao.MessageQueue
import com.bashwork.akka.model.CustomJsonProtocol._
import com.bashwork.akka.model.TrackedMessage
import com.google.inject.Inject
import scala.concurrent.duration._
import spray.json._

/**
 * Messages used by the BashworkWorkflowActor
 */
object BashworkWorkflowActor {
  case class StartWorkflow(message: TrackedMessage)
  
  def props() =
    Props(new BashworkWorkflowActor())
}

/**
 * Actor used to handle interacting with the message
 * queue and working with message handles.
 */
class BashworkWorkflowActor @Inject() ()
  extends Actor with ActorLogging {
  
  import MessageRouterActor._
  import BashworkWorkflowActor._
      
  def receive = {
    case StartWorkflow(message) => startup(message)
  }
  
  def running(id: String): Receive = {
    case StartWorkflow(message) => finish(id)
  }
  
  /**
   * This starts the Bashwork workflow and puts it in the
   * running state.
   * 
   * @param message The message to handle in this workflow
   */
  def startup(message: TrackedMessage) {
    log.debug(s"Handling new message: ${message}")

    finish(message.id)
    // start enrichers with message.message
    //context.become(running(message.id))
  }
  
  /**
   * When we are finished with the workflow, we respond
   * to our parent with the message id that we handled.
   * 
   * @param id The unique identifier for the handled message
   */
  def finish(id: String) {
    log.debug(s"Finished Bashwork workflow associated with: ${id}")
    context.parent ! DeleteMessage(id)
    context.stop(self)
  }
}
