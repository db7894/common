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
      libraryDependencies += "com.amazonaws" % "aws-java-sdk" % "1.3.32"
    )
  )
}
