package com.bashwork.akka.service

import akka.actor._
import com.bashwork.akka.dao.MessageQueue
import com.google.inject.Inject
import spray.http.StatusCodes.InternalServerError
import spray.httpx.SprayJsonSupport._
import spray.routing.{ExceptionHandler, HttpService}
import spray.util.LoggingContext


/**
 * The main stacked Avid service implementation that can
 * be installed and run.
 */
class BashworkServiceActor @Inject() (queue: MessageQueue) extends Actor
  with BashworkService {

  def actorRefFactory = context
  def receive = runRoute(route)
}


/**
 * The BashworkService API which is separated to be used in
 * unit tests as well as a production service.
 */
trait BashworkService extends HttpService {
  
  // The main collection of routes for the service
  def route = healthCheckRoute
  
  /**
   * The collection of routes that implement the required health
   * check for the load balancer to work correctly.
   */
  def healthCheckRoute = {
    path("ping") {
      get {
        complete("pong")
      }
    }
  }
      
  /**
   * The implicit exception handler to deal with failures.
   */
  implicit def BashworkExceptionHandler(implicit log: LoggingContext) =
    ExceptionHandler {
      case ex: Exception => requestUri { uri =>
        log.error(s"request to ${uri} could not be handled")
        complete(InternalServerError, s"Exception: ${ex.getMessage}" )
      }
  }
}
