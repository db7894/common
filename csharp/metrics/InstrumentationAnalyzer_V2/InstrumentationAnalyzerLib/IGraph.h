#ifndef __IGRAPH_H__
#define __IGRAPH_H__

#include "TokenPairRec.h"

namespace NativeInstrumentation {

  /// <summary>
  /// Public interface for graph processing.
  /// </summary>
  class IGraph
  {
  public:

    IGraph(void)
    {
    }

    virtual ~IGraph(void)
    {
    }

    virtual void Clear ( void ) = 0;
    virtual void Put ( const TokenPairRec & rec ) = 0;
    virtual void Process ( void ) = 0;
  };

}; // namespace NativeInstrumentation

#endif // __IGRAPH_H__
