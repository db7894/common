package com.bashwork.akka.actor.utility

import akka.actor.ActorRef
import akka.event.EventBus
import akka.event.LookupClassification
import com.bashwork.akka.model.SnsMessage

/**
 * Publishes the payload of the SnsMessage when the topic of the
 * SnsMessage equals the String specified when subscribing.
 */
class RawMessageBus extends EventBus with LookupClassification {
  type Event      = SnsMessage
  type Classifier = String
  type Subscriber = ActorRef
 
  // is used for extracting the classifier from the incoming events  
  override protected def classify(event: Event): Classifier =
    event.source 
 
  // will be invoked for each event for all subscribers which registered themselves
  // for the event’s classifier
  override protected def publish(event: Event, subscriber: Subscriber): Unit = {
    subscriber ! event.message
  }
 
  // must define a full order over the subscribers, expressed as expected from
  // `java.lang.Comparable.compare`
  override protected def compareSubscribers(a: Subscriber, b: Subscriber): Int =
    a.compareTo(b)
 
  // determines the initial size of the index data structure
  // used internally (i.e. the expected number of different classifiers)
  override protected def mapSize: Int = 16
}

/**
 * Publishes the payload of the SnsMessage when the topic of the
 * SnsMessage equals the String specified when subscribing. Also,
 * when subscribing, the subscriber supplies a message type payload
 * converter to generate a typed message.
 */
class TypedMessageBus extends EventBus with LookupClassification {
  type Event      = (String, SnsMessage)      // message handle, message
  type Classifier = String                    // SnsMessage.source
  type Subscriber = (ActorRef, String => Any) // (actor, Conversion) 
 
  // is used for extracting the classifier from the incoming events  
  override protected def classify(event: Event): Classifier =
    event._2.source 
 
  // will be invoked for each event for all subscribers which registered themselves
  // for the event’s classifier
  override protected def publish(event: Event, subscriber: Subscriber): Unit = {
    val (actor, factory) = subscriber
    val (handle, message) = event
    
    actor ! (handle, factory(message.message))
  }
 
  // must define a full order over the subscribers, expressed as expected from
  // `java.lang.Comparable.compare`
  override protected def compareSubscribers(a: Subscriber, b: Subscriber): Int =
    a._1.compareTo(b._1)
 
  // determines the initial size of the index data structure
  // used internally (i.e. the expected number of different classifiers)
  override protected def mapSize: Int = 16
}
