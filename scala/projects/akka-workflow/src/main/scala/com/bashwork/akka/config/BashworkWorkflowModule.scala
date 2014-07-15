package com.bashwork.akka.config

import com.bashwork.akka.dao.MessageQueue
import com.bashwork.akka.dao.SqsMessageQueue
import com.google.inject.AbstractModule
import net.codingwell.scalaguice.ScalaModule

/**
 * The data dependencies for the various web services, aws services,
 * databases, etc.
 */
class BashworkWorkflowModule extends AbstractModule with ScalaModule {
  
  def configure {
    bind[MessageQueue].to[SqsMessageQueue].asEagerSingleton
  }
}
