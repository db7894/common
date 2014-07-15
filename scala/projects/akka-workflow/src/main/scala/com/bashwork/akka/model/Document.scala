package com.bashwork.akka.model

/**
 * Represents a single event
 */
case class Event(key: String, value: String)

/**
 * Represents a collection of event data that has
 * been aggregated and can be used for further inspection
 */
case class Document(event1: Option[Event], event2: Option[Event]) {
  def withEvent1(event: Event) = copy(event1 = Some(event))
  def withEvent2(event: Event) = copy(event2 = Some(event))
}

object Document {
  def apply = new Document(None, None)
}
