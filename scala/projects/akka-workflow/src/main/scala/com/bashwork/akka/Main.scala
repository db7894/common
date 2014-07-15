package com.bashwork.akka

import com.google.inject.Guice
import com.bashwork.akka.config.MainModule
import com.bashwork.akka.config.actor.ActorServiceMain
import com.bashwork.akka.config.service.ApiServiceMain

object Main {
  
  def main(args: Array[String]): Unit = {
    val injector = Guice.createInjector(new MainModule)
    
    injector.getInstance(classOf[ActorServiceMain]).start
    injector.getInstance(classOf[ApiServiceMain]).start
  }
}
