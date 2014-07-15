package com.bashwork.akka.dao

import com.bashwork.akka.model.Document

/**
 * Interface to a document search service
 */
trait Enricher {
  /**
   * Given a single document, index it into the 
   * underlying document store.
   */
  def enrich(document: Document)
}

class ExampleEventEnricher extends Enricher {
  def enrich(document: Document) {
    println(document)
  }
}
