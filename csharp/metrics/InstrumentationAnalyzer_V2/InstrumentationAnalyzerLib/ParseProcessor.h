#ifndef __PARSE_PROCESSOR_H__
#define __PARSE_PROCESSOR_H__

#include <iostream>
#include <fstream>

#include "ParseGraphVisitor.h"
#include "Graph.h"
#include "Token.h"

namespace NativeInstrumentation {

  /// <summary>
  /// A static class that provides the callback function to pair with the ParseGraphVisitor
  /// </summary>
  class ParseProcessor
  {
  public:
    static bool Init ( std::ofstream * pOutStream , int startEvent );
    static IGraph & GetGraph ( void ) { return *_pGraph; }
    static void Dispose ( void );
  private:
    typedef ParseGraphVisitor<GraphTypes::TokenMap_t, GraphTypes::TicksMap_t,
                              GraphTypes::EdgeTicksMap_t, GraphTypes::FilterVertexMap_t> ParseGraphVisitor_t;
    typedef Graph<ParseGraphVisitor_t, ParseVisitorData_t> ParseGraph_t;

    ParseProcessor ( void ) {}
    static void OutputToken ( Token token );
    static void WriteParseHeader ( void );
    static void GraphProcessor ( ParseVisitorData_t & data );

    static std::ofstream * _pOutStream;
    static ParseGraph_t * _pGraph;
    static ParseGraphVisitor_t * _pVis;
    static ParseVisitorData_t * _pParseData;
  };
  
}; // namespace NativeInstrumentation

#endif // __PARSE_PROCESSOR_H__
