package com.bashwork.akka.dao

import com.amazonaws.auth.ClasspathPropertiesFileCredentialsProvider
import com.amazonaws.services.sqs.AmazonSQSClient
import com.amazonaws.services.sqs.model._
import com.google.inject.Inject
import com.typesafe.config.Config
import scala.collection.JavaConversions._
import scala.concurrent.duration._


/**
 * A trait representing an interface to a message
 * queue (with a bias towards SQS).
 */
trait MessageQueue {
  def createQueue(name: String): String
  def deleteQueue(name: String): Unit
  def listQueues: Seq[String]
  def get(count: Int): Seq[Message]
  def get(count: Int, wait: FiniteDuration): Seq[Message]
  def +=(message: String): String
  def -=(message: Message): Unit
  def -=(handle: String): Unit
}

class MemoryMessageQueue(queueName: String="default") extends MessageQueue {
  var queue = Map[String, String]()
    
  def createQueue(name: String) = name
  def deleteQueue(name: String) {}
  def listQueues: Seq[String] = List(queueName)
  def get(count: Int): Seq[Message] = get(count, 0.seconds)
  def get(count: Int, wait: FiniteDuration): Seq[Message] =
    queue.take(count).map { case (k, v) =>
      new Message().withBody(v).withReceiptHandle(k)
    }.toList
  def +=(message: String): String = {
    val id = java.util.UUID.randomUUID.toString
    queue += id -> message
    id
  }
  def -=(message: Message) {
    queue -= message.getReceiptHandle()
  }
  def -=(handle: String) {
    queue -= handle
  }
}

/**
 *
 * @param queue The name of the queue to work with
 */
class SqsMessageQueue @Inject() (val config: Config) extends MessageQueue {
  
  val queue = config.getString("sqs.queue");
  val provider = new ClasspathPropertiesFileCredentialsProvider
  val client = new AmazonSQSClient(provider)

  def createQueue(name: String): String = {
    val request = new CreateQueueRequest(name)
    client.createQueue(request).getQueueUrl
  }

  def deleteQueue(name: String) {
    val request = new DeleteQueueRequest(name)
    client.deleteQueue(request)
  }

  def listQueues: Seq[String] =
    client.listQueues().getQueueUrls
    
  def get(count: Int): Seq[Message] = {
    val request = new ReceiveMessageRequest(queue)
      .withMaxNumberOfMessages(count)

    client.receiveMessage(request).getMessages
  }

  def get(count: Int, wait: FiniteDuration): Seq[Message] = {
    val request = new ReceiveMessageRequest(queue)
      .withMaxNumberOfMessages(count)
      .withWaitTimeSeconds(wait.toSeconds.toInt)

    client.receiveMessage(request).getMessages
  }

  def +=(message: String): String = {
    val request = new SendMessageRequest(queue, message)
    client.sendMessage(request).getMessageId
  }
  
  def -=(handle: String) {
    val request = new DeleteMessageRequest(queue, handle)
    client.deleteMessage(request)
  }

  def -=(message: Message) {
    val handle  = message.getReceiptHandle
    val request = new DeleteMessageRequest(queue, handle)
    client.deleteMessage(request)
  }
}
