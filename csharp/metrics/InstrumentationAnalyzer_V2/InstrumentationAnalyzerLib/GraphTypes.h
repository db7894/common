#ifndef __GRAPH_TYPES_H__
#define __GRAPH_TYPES_H__

#include "boost/graph/adjacency_list.hpp"

using namespace boost;

namespace NativeInstrumentation {

  /// <summary>
  /// A container for the commonly used typedefs.
  /// </summary>
  class GraphTypes
  {
  public:

    struct Token_t {
      typedef vertex_property_tag kind;
    };
    struct Ticks_t {
      typedef vertex_property_tag kind;
    };
    struct FilterVertex_t {
      typedef vertex_property_tag kind;
    };
    struct EdgeTicks_t {
      typedef edge_property_tag kind;
    };
    typedef property<FilterVertex_t, long, property<vertex_index_t, std::size_t> > FilterVertexProp_t;
    typedef property<Ticks_t, unsigned long long, FilterVertexProp_t> TicksProp_t;
    typedef property<Token_t, unsigned long long, TicksProp_t> TokenProp_t;

    typedef property<EdgeTicks_t, unsigned long long> EdgeTicksProp_t;
    
    typedef adjacency_list<listS, listS, directedS, TokenProp_t, EdgeTicksProp_t> Graph_t;

    typedef graph_traits<Graph_t>::vertex_descriptor VertexDescriptor_t;
    typedef property_map<Graph_t, Token_t>::type TokenMap_t;
    typedef property_traits<TokenMap_t>::value_type TokenValue_t;
    typedef property_map<Graph_t, Ticks_t>::type TicksMap_t;
    typedef property_traits<TicksMap_t>::value_type TicksValue_t;
    typedef property_map<Graph_t, EdgeTicks_t>::type EdgeTicksMap_t;
    typedef property_traits<EdgeTicksMap_t>::value_type EdgeTicksValue_t;
    typedef property_map<Graph_t, FilterVertex_t>::type FilterVertexMap_t;
    typedef property_traits<FilterVertexMap_t>::value_type FilterVertexValue_t;
   
    struct PropMaps_t {
      TokenMap_t tokenPropMap;
      TicksMap_t ticksPropMap;
      EdgeTicksMap_t edgeTicksPropMap;
      FilterVertexMap_t filterPropMap;
    };

  };

}; // namespace NativeInstrumentation

#endif // __GRAPH_TYPES_H__
