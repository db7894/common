scalaVersion := "2.11.0"

val scalazVersion = "7.0.6"

libraryDependencies ++= Seq(
  "com.scalarx" %% "scalarx" % "0.2.5",
  "org.scalaz"  %% "scalaz-core" % scalazVersion,
  "org.scalaz"  %% "scalaz-effect" % scalazVersion,
  "org.scalaz"  %% "scalaz-typelevel" % scalazVersion,
  "org.scalaz"  %% "scalaz-scalacheck-binding" % scalazVersion % "test"
)

scalacOptions += "-feature"

// sbt console
initialCommands in console := "import scalaz._, Scalaz._"

// sbt test:console
initialCommands in console in Test := "import scalaz._, Scalaz._, scalacheck.ScalazProperties._, scalacheck.ScalazArbitrary._,scalacheck.ScalaCheckBinding._"
