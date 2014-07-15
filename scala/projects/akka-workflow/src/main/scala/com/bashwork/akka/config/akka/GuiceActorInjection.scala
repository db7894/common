package com.bashwork.akka.config.akka

import akka.actor.Actor
import akka.actor.ActorSystem
import akka.actor.ExtendedActorSystem
import akka.actor.Extension
import akka.actor.ExtensionId
import akka.actor.ExtensionIdProvider
import akka.actor.IndirectActorProducer
import akka.actor.Props
import com.google.inject.Injector

/**
 * A producer of actor references via the guice dependency
 * injection system.
 * 
 * @param injector The guice injector to operate with.
 * @param klass The class of the actor to create.
 */
class GuiceActorProducer(injector: Injector, klass: Class[_ <: Actor])
  extends IndirectActorProducer {
  
  def actorClass() = klass
  def produce() = injector.getInstance(klass)
}

/**
 * An akka extension implementation for guice based injection.
 * The Extension provides Akka access to dependencies defined
 * in Guice.
 */
class GuiceAkkaExtensionImpl extends Extension {

  private var injector: Injector = _

  def initialize(injector: Injector) {
    this.injector = injector
  }

  def props(klass: Class[_ <: Actor]) = Props(classOf[GuiceActorProducer], injector, klass)
//  def props[A <: Actor: ClassTag] = {
//    val guice = classTag[GuiceActorProducer[A]].runtimeClass
//    val actor = classTag[A].runtimeClass
//    Props(guice, injector, actor)
//  }

}

/**
 * The singleton akka extension for tying everything together.
 */
object GuiceAkkaExtension extends ExtensionId[GuiceAkkaExtensionImpl] with ExtensionIdProvider {
  
  override def lookup() = GuiceAkkaExtension
  override def createExtension(system: ExtendedActorSystem) = new GuiceAkkaExtensionImpl
  override def get(system: ActorSystem): GuiceAkkaExtensionImpl = super.get(system)
}

/**
 * Helper trait to reduce the boilerplate of calling the
 * extension in the actor provider.
 */
trait GuiceActorCreator { self: Actor =>
  
  def actorOf(klass: Class[_ <: Actor]) =
    context.actorOf(GuiceAkkaExtension(context.system).props(klass))
    
  def actorOf(klass: Class[_ <: Actor], name: String) =
    context.actorOf(GuiceAkkaExtension(context.system).props(klass), name)
}
