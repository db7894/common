scalaVersion := "2.11.3"

resolvers ++= Seq(
  "Sonatype Releases" at "https://oss.sonatype.org/content/repositories/releases/"
)

libraryDependencies ++= Seq(
  "org.scalanlp" %% "breeze" % "0.10",
  "org.scalanlp" %% "breeze-natives" % "0.10",
  "org.scalanlp"  % "nak" % "1.2.1"
)

scalacOptions += "-feature"

// sbt console
initialCommands in console := "import scalaz._, Scalaz._"

// sbt test:console
initialCommands in console in Test := "import scalaz._, Scalaz._, scalacheck.ScalazProperties._, scalacheck.ScalazArbitrary._,scalacheck.ScalaCheckBinding._"
