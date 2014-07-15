package com.bashwork.akka.actor

import akka.actor.Actor
import akka.actor.ActorLogging
import akka.actor.ActorRef
import akka.actor.PoisonPill
import akka.actor.Props
import com.amazonaws.services.sqs.model.Message
import com.bashwork.akka.config.akka.GuiceActorCreator
import com.bashwork.akka.dao.MessageQueue
import com.bashwork.akka.model.CustomJsonProtocol._
import com.google.inject.Inject
import com.google.inject.name.Named
import com.typesafe.config.Config
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors
import java.util.concurrent.atomic.AtomicBoolean
import scala.annotation.tailrec
import scala.concurrent.duration._
import spray.json._

/**
 * Messages used by the MessagePumpActor
 */
object MessagePumpActor {
  case object StartPump
  case object StopPump
  
  def props(queue: MessageQueue, config: Config) =
    Props(new MessagePumpActor(queue, config))
}

/**
 * Actor used to handle interacting with the message
 * queue and working with message handles.
 * 
 * @param queue The queue to read messages from
 */
class MessagePumpActor @Inject() (queue: MessageQueue, config: Config)
  extends Actor with ActorLogging with GuiceActorCreator {
 
  val cores    = Runtime.getRuntime().availableProcessors()
  val threads  = config.getInt("sqs.threads").ensuring(x => x > 0)
  val executor = Executors.newFixedThreadPool(cores)
  
  // pull necessary actor messages into scope
  import MessagePumpActor._
  import MessageRouterActor._
  
  def receive = {
    case StartPump => startup()
  }
  
  def running(flag: AtomicBoolean): Receive = {
    case StopPump  => shutdown(flag)
  }

  /**
   * This starts the system running to process
   * messages.
   */
  def startup() {
    val flag = new AtomicBoolean(true)
    val router = actorOf(classOf[MessageRouterActor], "router")
    (1 to threads) foreach { id =>
      executor.submit(new QueuePoller(router, queue, flag))
    }
    context.become(running(flag))
  }
  
  /**
   * This is called when we want to shut the 
   * system down and return to waiting to start
   * again.
   * 
   * @param flag The flag to control the running threads
   */
  def shutdown(flag: AtomicBoolean) {
    flag.set(false)
    context.unbecome()
  }
  
  /**
   * A simple queue poller to read messages from the queue and inject them
   * into the router.
   * 
   * @param router The routing actor to pump messages into
   * @param queue The queue to read messages from
   * @param running The flag that controls the message pump thread
   */
  class QueuePoller(router: ActorRef, queue: MessageQueue, running: AtomicBoolean)
    extends Runnable {
    
    /**
     * This recursively calls itself checking each time if it should continue
     * to pump messages into the router.
     */
    @tailrec private def poll() {
      if (running.get()) {
        queue.get(1, 10.seconds).map(router ! HandleMessage(_))
        poll
      } else {
        router ! PoisonPill // allow the router to finish current work
      }
    }
    
    def run = poll
  }
}
