#ifndef __ACCUM_PROCESSOR_H__
#define __ACCUM_PROCESSOR_H__

#include <iostream>
#include <fstream>

#include "StreamingGraph.h"
#include "AccumGraphVisitor.h"

using namespace NativeInstrumentation;

namespace StreamingInstrumentation {

  /// <summary>
  /// A static class that provides the callback function to pair with the AccumGraphVisitor
  /// </summary>
  class AccumProcessor
  {
  public:
    static bool Init ( std::ofstream * pOutStream , int startEvent );
    static IGraph & GetGraph ( void ) { return *_pGraph; }
    static void Dispose ( void );

  private:
    typedef AccumGraphVisitor<GraphTypes::TokenMap_t, GraphTypes::TicksMap_t,
                              GraphTypes::EdgeTicksMap_t, GraphTypes::FilterVertexMap_t> AccumGraphVisitor_t;
    typedef StreamingGraph<AccumGraphVisitor_t, VisitorTimestamps_t> AccumGraph_t;

    AccumProcessor ( void ) {}
    static void WriteAccumHeader ( void );
    static void GraphProcessor ( VisitorTimestamps_t & data );

    static std::ofstream * _pOutStream;
    static AccumGraph_t * _pGraph;
    static AccumGraphVisitor_t * _pVis;
    static VisitorTimestamps_t * _pTimestamps;
  };

}; // namespace NativeInstrumentation

#endif // __ACCUM_PROCESSOR_H__