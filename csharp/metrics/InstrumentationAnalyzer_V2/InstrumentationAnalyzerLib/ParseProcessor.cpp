#include "ParseProcessor.h"

using namespace NativeInstrumentation;

std::ofstream * ParseProcessor::_pOutStream = NULL;
ParseProcessor::ParseGraph_t * ParseProcessor::_pGraph = NULL;
ParseProcessor::ParseGraphVisitor_t * ParseProcessor::_pVis = NULL;
ParseVisitorData_t * ParseProcessor::_pParseData = NULL;

bool ParseProcessor::Init ( std::ofstream * pOutStream , int startEvent ) {
  _pParseData = new ParseVisitorData_t();
  _pVis = new ParseGraphVisitor_t(*_pParseData);
  _pGraph = new ParseGraph_t(&GraphProcessor, *_pVis, *_pParseData, startEvent);
  _pOutStream = pOutStream;
  WriteParseHeader();
  return _pOutStream->good();
}

void ParseProcessor::Dispose ( void ) {
  if ( _pGraph != NULL ) {
    delete _pGraph;
    _pGraph = NULL;
  }
  if ( _pVis != NULL ) {
    delete _pVis;
    _pVis = NULL;
  }
  if ( _pParseData != NULL ) {
    delete _pParseData;
    _pParseData = NULL;
  }
}

void ParseProcessor::OutputToken ( Token token ) {
  *_pOutStream << token.AppId() << ",";
  *_pOutStream << token.EventId() << ",";
  *_pOutStream << token.Id();
}

void ParseProcessor::WriteParseHeader ( void ) {
  *_pOutStream << "App Id,";
  *_pOutStream << "Event Id,";
  *_pOutStream << "TokenId,";
  *_pOutStream << "App Id,";
  *_pOutStream << "Event Id,";
  *_pOutStream << "TokenId,";
  *_pOutStream << "Timestamp," << std::endl;

}

void ParseProcessor::GraphProcessor ( ParseVisitorData_t & data ) {
  const ParseVisitorData_t::ParseData_t * pParseData = data.GetData();
  ParseVisitorData_t::ParseData_t::const_iterator itor = pParseData->begin();
  for ( ; itor != pParseData->end(); ++itor ) {
    OutputToken(itor->T1());
    *_pOutStream << ",";
    OutputToken(itor->T2());
    *_pOutStream << ",";
    *_pOutStream << itor->Timestamp() << std::endl;
  }
}



