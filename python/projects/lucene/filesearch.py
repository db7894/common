#!/usr/bin/env python
import re
import sys
import os
import time
import fnmatch
import lucene
import operator
from optparse import OptionParser
from collections import defaultdict

from java.io import File
from org.apache.lucene.analysis.miscellaneous import LimitTokenCountAnalyzer
from org.apache.lucene.analysis.standard import StandardAnalyzer
from org.apache.lucene.document import Document, Field, FieldType
from org.apache.lucene.index import FieldInfo, IndexWriter, IndexWriterConfig
from org.apache.lucene.store import SimpleFSDirectory
from org.apache.lucene.util import Version
from org.apache.lucene.index import DirectoryReader
from org.apache.lucene.search import IndexSearcher
from org.apache.lucene.queryparser.classic import QueryParser

#------------------------------------------------------------
# Logger Configuration
#------------------------------------------------------------
import logging
logger = logging.getLogger(__name__)

#------------------------------------------------------------
# Providers
#------------------------------------------------------------
class FilesystemProvider(object):

    def __init__(self, **kwargs):
        ''' Initialize a new filesystem provider
        '''
        self.path    = kwargs.get('path', '.')
        self.path    = os.path.abspath(self.path)
        self.pattern = kwargs.get('pattern', '*')

    def get_paths(self):
        ''' Returns a generator around the the filesystem
        walker that matches the supplied filters.
        '''
        for path, dirs, names in os.walk(self.path):
            for name in fnmatch.filter(names, self.pattern):
                yield os.path.join(path, name)

#------------------------------------------------------------
# Indexing / Searching Classes
#------------------------------------------------------------
class Searcher(object):

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the Searcher

        :param count: The number of counts to return from a query
        :param output: The output directory of the underlying index
        '''
        self.count    = kwargs.get('count', 100)
        self.output   = kwargs.get('root', 'index')
        self.store    = SimpleFSDirectory(File(self.output))
        self.analyzer = StandardAnalyzer(Version.LUCENE_30)
        self.searcher = IndexSearcher(DirectoryReader.open(self.store))

    def search(self, query):
        ''' Given a query, apply it against the existing index.

        :param query: The query to apply to the index
        :returns: A generator of the matching documents
        '''
        query = QueryParser(Version.LUCENE_30, "data", self.analyzer).parse(query)
        results = self.searcher.search(query, self.count)
        for result in results.scoreDocs or []:
            #logger.debug("%s %s %s", hit.score, hit.doc, hit.toString())
            document = self.searcher.doc(result.doc)
            yield document.get('path'), result.score

class Indexer(object):

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the Indexer

        :param output: The output directory of the underlying index
        :param anaylzer: The overloaded analyzer to work with
        '''
        self.output   = kwargs.get('root', 'index')
        if not os.path.exists(self.output):
            os.mkdir(self.output)

        self.analyzer = kwargs.get('analyzer', StandardAnalyzer(Version.LUCENE_CURRENT))
        self.analyzer = LimitTokenCountAnalyzer(self.analyzer, 1048576)
        self.config   = IndexWriterConfig(Version.LUCENE_CURRENT, self.analyzer)
        self.config.setOpenMode(IndexWriterConfig.OpenMode.CREATE)
        self.store    = SimpleFSDirectory(File(self.output))
        self.writer   = IndexWriter(self.store, self.config)
        self.create_field_types()

    def index(self, document):
        ''' Given a new document, add it to the index.

        :param document: The document to add to the indexer
        '''
        try:
            self.writer.addDocument(document)
        except Exception:
            logger.exception("Failed to index the supplied document")

    def shutdown(self):
        ''' Shutdown the currently processing indexer.
        '''
        try:
            #self.writer.optimize()
            self.writer.close()
        except Exception:
            logger.exception("Failed to shutdown the indexer correctly")

    def create_field_types(self):
        ''' Create the field types that will be used to specify
        what actions lucene should take on the various fields
        supplied to index.
        '''
        self.field_clean = FieldType()
        self.field_clean.setIndexed(True)
        self.field_clean.setStored(True)
        self.field_clean.setTokenized(False)
        self.field_clean.setIndexOptions(FieldInfo.IndexOptions.DOCS_AND_FREQS)

        self.field_dirty = FieldType()
        self.field_dirty.setIndexed(True)
        self.field_dirty.setStored(False)
        self.field_dirty.setTokenized(True)
        self.field_dirty.setIndexOptions(FieldInfo.IndexOptions.DOCS_AND_FREQS_AND_POSITIONS)

#------------------------------------------------------------
# Utility Index Methods
#------------------------------------------------------------
FILE_TYPES = {
    'py'   : 'python',
    'rb'   : 'ruby',
    'scala': 'scala',
    'java' : 'java',
}

def generate_document(path, indexer):
    ''' Given a file, convert it into a lucene document that
    is ready to be indexed.

    :param path: The file to add to the search index
    :param indexer: The indexer to operate with
    :returns: The index document for the specified camera
    '''
    name = FILE_TYPES.get(path.rsplit('.', 1)[-1], '')
    data = open(path, 'r').read()

    document = Document()
    document.add(Field('path', path, indexer.field_clean))
    document.add(Field('type', name, indexer.field_clean))
    document.add(Field('data', data, indexer.field_dirty))
    return document

def generate_index(provider, indexer):
    ''' Given a document provider and an indexer, index
    all the available documents to the index.

    :param provider: The provider of documents
    :param indexer: The indexer to add the cameras to
    '''
    for path in provider.get_paths():
        logger.debug("indexing the supplied path: %s", path)
        indexer.index(generate_document(path, indexer))
    indexer.shutdown()

#------------------------------------------------------------
# Main Script Runner
#------------------------------------------------------------

def parse_options():
    ''' Parse the command line arguments

    :returns: The command line arguments
    '''
    parser = OptionParser()
    parser.add_option("-o", "--output",
        help="The output directory to write the index to",
        dest="output", default="index")
    parser.add_option("-s", "--source",
        help="The source directory to index from",
        dest="source", default=".")
    parser.add_option("-m", "--mode",
        help="The mode to operate as (index|search)",
        type="string", default="search")
    parser.add_option("-d", "--debug",
        help="To enable debugging or not",
        action="store_true", dest="debug", default=False)
    options, _ = parser.parse_args()
    return options

def main():
    ''' A test driver for the indexer
    '''

    lucene.initVM(vmargs=['-Djava.awt.headless=true'])
    options = parse_options()

    if options.debug:
        try:
            logger.setLevel(logging.DEBUG)
            logging.basicConfig()
        except Exception, e:
    	    print "Logging is not supported on this system"

    if options.mode == "index":
        logger.debug('lucene [%s] starting', lucene.VERSION)
        indexer  = Indexer()
        provider = FilesystemProvider(path=options.source, pattern="*.py")
        generate_index(provider, indexer)
        logger.debug('lucene [%s] finished', lucene.VERSION)

    elif options.mode == "search":
        searcher = Searcher()
        while True:
            query = raw_input('enter your query: ')
            for path, score in searcher.search(query.strip()):
                print "{} -> {}".format(score, path)

if __name__ == "__main__":
    main()

