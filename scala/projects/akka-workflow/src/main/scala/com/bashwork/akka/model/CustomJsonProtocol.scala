package com.bashwork.akka.model

import spray.json.DefaultJsonProtocol
import spray.json.JsObject
import spray.json.RootJsonFormat
import spray.json.JsString
import spray.json.JsValue
import spray.json.DeserializationException

object CustomJsonProtocol extends DefaultJsonProtocol {

  implicit val eventFormat = jsonFormat2(Event)
  
  implicit object SnsMessageJsonFormat extends RootJsonFormat[SnsMessage] {
    def write(m: SnsMessage) = JsObject(
      "Type"      -> JsString(m.kind),
      "MessageId" -> JsString(m.id),
      "TopicArn"  -> JsString(m.arn),
      "Message"   -> JsString(m.message)
    )
    
    def read(value: JsValue) = {
      value.asJsObject.getFields("Type", "MessageId", "TopicArn", "Message") match {
        case Seq(JsString(kind), JsString(id), JsString(arn), JsString(message)) =>
          new SnsMessage(kind, id, arn, message)
        case _ => throw new DeserializationException("Color expected")
      }
    }
  }
}
