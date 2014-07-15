package com.bashwork.akka.config

import com.google.inject.Provides
import com.google.inject.Singleton
import com.google.inject.{AbstractModule, Provider}
import com.typesafe.config.{ConfigFactory, Config}
import net.codingwell.scalaguice.ScalaModule


/**
 * Binds the application configuration to the Config interface.
 *
 * The configuration is bound as an eager singleton so that errors
 * in the configuration are detected as early as possible.
 */
class ConfigModule extends AbstractModule with ScalaModule {

  override def configure() {
  }
  
  @Provides
  @Singleton
  def providesConfig: Config = ConfigFactory.load()

}
