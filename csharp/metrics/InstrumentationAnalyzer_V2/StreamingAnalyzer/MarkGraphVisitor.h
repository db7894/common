#ifndef __MARK_GRAPH_VISITOR_H__
#define __MARK_GRAPH_VISITOR_H__

#include "boost/graph/depth_first_search.hpp"

#include "Token.h"
#include "EventId.h"
#include "PathType.h"

namespace StreamingInstrumentation {

  /// <summary>
  /// A depth first visitor implementation that just walks the graph and
  /// records the path type for each vertex.
  /// </summary>
  template <typename TokMap_t , typename TikMap_t , typename EMap_t , typename FMap_t >
  class MarkGraphVisitor : public default_dfs_visitor
  {
  public:
    MarkGraphVisitor ( void )
      :_pTokenPropMap(NULL)
      ,_pTicksPropMap(NULL)
      ,_pEdgePropMap(NULL)
      ,_pFilterPropMap(NULL)
    {
    }

    MarkGraphVisitor ( FilterValueColl_t & coll )
      :_pTokenPropMap(NULL)
      ,_pTicksPropMap(NULL)
      ,_pEdgePropMap(NULL)
      ,_pFilterPropMap(NULL)
      ,_filterValueColl(coll)
    {
    }

    ~MarkGraphVisitor ( void )
    {
    }

    void InitMaps ( TokMap_t * pTokenPropMap , TikMap_t * pTicksPropMap ,
                    EMap_t * pEdgePropMap , FMap_t * pFilterPropMap ) {
      _pTokenPropMap = pTokenPropMap;
      _pTicksPropMap = pTicksPropMap;
      _pEdgePropMap = pEdgePropMap;
      _pFilterPropMap = pFilterPropMap;
    }


    template < typename V, typename G >
    void start_vertex(V u, const G & g) const {
      put(*_pFilterPropMap, u, (long)Unknown);
    }
     
    template < typename V, typename G >
    void discover_vertex(V u, const G & g) const {
      GraphTypes::TokenValue_t tokenValue = get(*_pTokenPropMap, u);
      Token token = Token(tokenValue);

      switch ( token.EventId() ) {
        case EventId_t::EventQuoteNormal:
        case EventId_t::EventTickNormal:
        case EventId_t::EventRefreshNormal:
        case EventId_t::EventOptionRefreshNormal:
          put(*_pFilterPropMap, u, (long)Normal);
          break;
        case EventId_t::EventQuoteCharting:
        case EventId_t::EventTickCharting:
        case EventId_t::EventRefreshCharting:
        case EventId_t::EventOptionRefreshCharting:
          put(*_pFilterPropMap, u, (long)Chart);
          break;
        default:
          break;
      }
    }

    template < typename E, typename G >
    void examine_edge(E e, const G & g) const {
      typedef graph_traits<G>::vertex_descriptor VertexDescriptor_t;
      VertexDescriptor_t u = source(e, g);
      VertexDescriptor_t v = target(e, g);

      put(*_pFilterPropMap, v, get(*_pFilterPropMap, u));
    }

  private:
    TokMap_t * _pTokenPropMap;
    TikMap_t * _pTicksPropMap;
    EMap_t * _pEdgePropMap;
    FMap_t * _pFilterPropMap;
    FilterValueColl_t _filterValueColl;
  };

}; // namespace NativeInstrumentation
#endif // __MARK_GRAPH_VISITOR_H__
