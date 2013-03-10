import sbt._
import sbt.Keys._

object SqsrouterBuild extends Build {

  lazy val sqsrouter = Project(
    id = "sqsrouter",
    base = file("."),
    settings = Project.defaultSettings ++ Seq(
      name := "SQSRouter",
      organization := "org.bashwork",
      version := "1.0",
      scalaVersion := "2.9.2",
      resolvers += "Typesafe Repository" at "http://repo.typesafe.com/typesafe/releases/",
      libraryDependencies ++= Seq(
        "com.amazonaws" % "aws-java-sdk" % "1.3.32",
        "com.typesafe.akka" % "akka-actor" % "2.0.5"
      )
    )
  )
}
