package com.bashwork.akka.config

import com.bashwork.akka.config.actor.ActorServiceMainModule
import com.bashwork.akka.config.akka.AkkaSystemModule
import com.bashwork.akka.config.service.ApiServiceMainModule
import net.codingwell.scalaguice.ScalaModule

/**
 * The main collection of configuration modules for
 * the indexing service.
 */
class MainModule extends ScalaModule {
  
   override def configure() {
    installCore
    installActors
    installService
  }
   
  private def installCore {
    install(new ConfigModule)
    install(new AkkaSystemModule)
  }
  
  private def installActors {
    install(new ActorServiceMainModule)
    install(new BashworkWorkflowModule)
  }
  
  private def installService {
    install(new ApiServiceMainModule)
  }
}
