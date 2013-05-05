#ifndef __PARSE_GRAPH_VISITOR_H__
#define __PARSE_GRAPH_VISITOR_H__

#include "boost/graph/depth_first_search.hpp"

#include "Token.h"
#include "ParseVisitorData.h"

namespace NativeInstrumentation {

  /// <summary>
  /// A depth first visitor implementation that just walks the graph and
  /// rebuilds the token pair record.
  /// </summary>
  template <typename TokMap_t , typename TikMap_t , typename EMap_t , typename FMap_t >
  class ParseGraphVisitor : public default_dfs_visitor
  {
  public:
    ParseGraphVisitor ( ParseVisitorData_t & parseData )
      :_pTokenPropMap(NULL)
      ,_pTicksPropMap(NULL)
      ,_pEdgePropMap(NULL)
      ,_pFilterPropMap(NULL)
      ,_parseData(parseData)
    {
    }

    ~ParseGraphVisitor ( void )
    {
    }

    void InitMaps ( TokMap_t * pTokenPropMap ,
                    TikMap_t * pTicksPropMap ,
                    EMap_t * pEdgePropMap ,
                    FMap_t * pFilterPropMap ) {
      _pTokenPropMap = pTokenPropMap;
      _pTicksPropMap = pTicksPropMap;
      _pEdgePropMap = pEdgePropMap;
      _pFilterPropMap = pFilterPropMap;
    }


    template < typename V, typename G >
    void start_vertex(V u, const G & g) const {
    }
     
    template < typename V, typename G >
    void discover_vertex(V u, const G & g) const {
    }

    template < typename E, typename G >
    void forward_or_cross_edge(E e, const G & g) const {
    }

    template < typename E, typename G >
    void examine_edge(E e, const G & g) const {
      typedef graph_traits<G>::vertex_descriptor VertexDescriptor_t;
      VertexDescriptor_t u = source(e, g);
      VertexDescriptor_t v = target(e, g);

      property_traits<typename TokMap_t>::value_type tokenValue = get(*_pTokenPropMap, u);
      Token sourceToken(tokenValue);

      tokenValue = get(*_pTokenPropMap, v);
      Token targetToken(tokenValue);

      property_traits<typename EMap_t>::value_type edgeTicksValue = get(*_pEdgePropMap, e);

      TokenPairRec tokenRec(sourceToken, targetToken, edgeTicksValue);
      _parseData.Add(tokenRec);
    }

    template < typename Vertex, typename Graph >
    void finish_vertex(Vertex u, const Graph & g) const {
    }

  private:
    TokMap_t * _pTokenPropMap;
    TikMap_t * _pTicksPropMap;
    EMap_t * _pEdgePropMap;
    FMap_t * _pFilterPropMap;
    ParseVisitorData_t & _parseData;
  };

}; // namespace NativeInstrumentation
#endif // __PARSE_GRAPH_VISITOR_H__
