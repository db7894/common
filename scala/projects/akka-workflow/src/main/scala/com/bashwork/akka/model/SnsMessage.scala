package com.bashwork.akka.model

/**
 * Represents a single SNS message that comes over an SQS queue
 * to be processed.
 */
case class SnsMessage(kind: String, id: String, arn: String, message: String) {
  val source = kind.split(':').last
}

/**
 * Represents a single SQS message that is embedded in an SNS
 * message and tracked with a unique identifier.
 */
case class TrackedMessage(id: String, message: String)
