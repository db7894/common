name := """akka-workflow"""

version := "1.0"

scalaVersion := "2.11.1"

resolvers += "spray" at "http://repo.spray.io/"

libraryDependencies ++= Seq(
  "ch.qos.logback"     % "logback-classic" % "1.0.10",
  "io.spray"          %% "spray-json"       % "1.2.6",
  "io.spray"          %% "spray-can"        % "1.3.1",
  "io.spray"          %% "spray-routing"    % "1.3.1",
  "io.spray"          %% "spray-client"     % "1.3.1",
  "com.typesafe.akka" %% "akka-actor"       % "2.3.4",
  "com.typesafe.akka" %% "akka-testkit"     % "2.3.4",
  "com.amazonaws"      % "aws-java-sdk"     % "1.7.12",
  "net.codingwell"    %% "scala-guice"      % "4.0.0-beta4",
  "org.scalatest"     %% "scalatest"        % "2.2.0" % "test",
  "junit"              % "junit"            % "4.11" % "test",
  "com.novocode"       % "junit-interface"  % "0.10" % "test",
  "org.scalamock"     %% "scalamock-scalatest-support" % "3.1.1" % "test"
)

testOptions += Tests.Argument(TestFrameworks.JUnit, "-v")

scalacOptions ++= Seq("-deprecation", "-feature")

fork := true

// disable 64k xml processing limit
javaOptions += "-Djdk.xml.entityExpansionLimit=0"
