package com.bashwork.akka.actor

import akka.actor.Actor
import akka.actor.ActorLogging
import akka.actor.ActorRef
import akka.actor.Props
import akka.event.EventBus
import akka.event.LookupClassification
import com.amazonaws.services.sqs.model.Message
import com.bashwork.akka.config.akka.GuiceActorCreator
import com.bashwork.akka.dao.MessageQueue
import com.bashwork.akka.model.CustomJsonProtocol._
import com.bashwork.akka.model.SnsMessage
import com.bashwork.akka.model.TrackedMessage
import com.google.inject.Inject
import scala.concurrent.duration._
import spray.json._

/**
 * Messages used by the MessageQueueActor
 */
object MessageRouterActor {
  case class HandleMessage(message: Message)
  case class DeleteMessage(handle: String)
  
  def props(queue: MessageQueue) =
    Props(new MessageRouterActor(queue))
}

/**
 * Actor used to handle interacting with the message
 * queue and working with message handles.
 */
class MessageRouterActor @Inject() (queue: MessageQueue)
  extends Actor with ActorLogging with GuiceActorCreator {
  
  // pull necessary actor messages into scope
  import MessageRouterActor._
  import BashworkWorkflowActor._
  
  // used to provide unique names to workflows
  var count = 0L
    
  def receive = {
    case HandleMessage(message) => handle(message)
    case DeleteMessage(handle)  => delete(handle)
  }

  /**
   * When we get a new message, we forward it to
   * the correct handling workflow.
   */
  def handle(message: Message) {
    log.debug(s"Handling new message: ${message}")
    count += 1
    val handle  = message.getReceiptHandle()
    val decoded = message.getBody().parseJson.convertTo[SnsMessage]
    val tracked = new TrackedMessage(handle, decoded.message)

    val actor = actorOf(classOf[BashworkWorkflowActor], s"workflow-${count}")
    actor ! StartWorkflow(tracked)
  }
  
  /**
   * After we have handled the message in question,
   * we delete it by the supplied message handle.
   */
  def delete(handle: String) {
    log.debug(s"Deleting handled message: ${handle}")
    queue -= handle
  }
}
