package org.bashwork

import com.amazonaws.services.sqs.AmazonSQSClient
import com.amazonaws.auth.ClasspathPropertiesFileCredentialsProvider
import com.amazonaws.services.sqs.model._
import scala.collection.JavaConversions._

/**
 * A trait representing an interface to a message
 * queue (with a bias towards SQS).
 */
trait MessageQueue {
  def createQueue(name: String): String
  def deleteQueue(name: String): String
  def listQueues: List[String]
  def get(count: Int): List[Message]
  def get(count: Int, wait: Int): List[Message]
  def +=(message: String): String
  def -=(handle: String)
}

/**
 *
 * @param queue
 */
class SQSMessageBusClient(var queue: String) {
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

  def listQueues: java.util.List[String] =
    client.listQueues().getQueueUrls

  def get(count: Int): java.util.List[Message] = {
    val request = new ReceiveMessageRequest(queue)
      .withMaxNumberOfMessages(count)

    client.receiveMessage(request).getMessages
  }

  def get(count: Int, wait: Int): java.util.List[Message] = {
    val request = new ReceiveMessageRequest(queue)
      .withMaxNumberOfMessages(count)
      .withWaitTimeSeconds(wait)

    client.receiveMessage(request).getMessages
  }

  def +=(message: String): String = {
    val request = new SendMessageRequest(queue, message)
    client.sendMessage(request).getMessageId
  }

  def -=(message: Message) {
    val handle  = message.getReceiptHandle
    val request = new DeleteMessageRequest(queue, handle)
    client.deleteMessage(request)
  }
}

/**
 * A simple example of using the amazon SQS service
 */
object Sqsrouter extends App {

  val queueName = "example-queue"
  val queue = new SQSMessageBusClient(queueName)
  queue.createQueue(queueName)

  queue += "hello"
  queue += "world"

  for (name <- queue.listQueues) {
    println(name)
  }

  for (msg <- queue.get(2, 20)) {
    println(msg.getBody)
    queue -= msg
  }

  queue.deleteQueue(queueName)
}
