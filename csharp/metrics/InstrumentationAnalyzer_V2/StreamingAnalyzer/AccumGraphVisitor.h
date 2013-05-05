#ifndef __ACCUM_GRAPH_VISITOR_H__
#define __ACCUM_GRAPH_VISITOR_H__

#include "boost/graph/depth_first_search.hpp"

#include "Token.h"
#include "EventId.h"
#include "VisitorTimestamps.h"

namespace StreamingInstrumentation {

  /// <summary>
  /// A depth first visitor implementation that just walks the graph and
  /// calculates the instrumentation times and counts.
  /// </summary>
  template <typename TokMap_t , typename TikMap_t , typename EMap_t , typename FMap_t >
  class AccumGraphVisitor : public default_dfs_visitor
  {
  public:
    AccumGraphVisitor ( VisitorTimestamps_t & timestamps )
      :_pTokenPropMap(NULL)
      ,_pTicksPropMap(NULL)
      ,_pEdgePropMap(NULL)
      ,_pFilterPropMap(NULL)
      ,_timestamps(timestamps)
    {
    }

    ~AccumGraphVisitor ( void )
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
      _timestamps.StartTime = get(*_pTicksPropMap, u);
    }
     
    template < typename V, typename G >
    void discover_vertex(V u, const G & g) const {
      property_traits<typename FMap_t>::value_type filterValue = get(*_pFilterPropMap, u);
      _timestamps.FilterValue = (FilterValueE)filterValue;

      graph_traits<G>::degree_size_type degree = out_degree(u, g);
      if ( degree == 0 ) {
        GraphTypes::TokenValue_t tokenValue = get(*_pTokenPropMap, u);
        Token token = Token(tokenValue);
        property_traits<typename TikMap_t>::value_type ticksValue = get(*_pTicksPropMap, u);

        switch ( token.EventId() ) {
          case EventId_t::EventConflationCombined:
            _timestamps.CombinedCount++;
            break;
          case EventId_t::EventConnectionBuffered:
            _timestamps.BufferedCombinedCount++;
            break;
          case EventId_t::EventWriteEnd:
            {
              _timestamps.TotalCount++;
              unsigned long long totalTime = (ticksValue - _timestamps.StartTime);
              if ( totalTime > _timestamps.MaxTime ) {
                _timestamps.MaxTime = totalTime;
              }
              _timestamps.TotalTime += totalTime;
            }
            break;
          default:
            break;
        }
      }
    }

    template < typename E, typename G >
    void examine_edge(E e, const G & g) const {
      typedef graph_traits<G>::vertex_descriptor VertexDescriptor_t;
      VertexDescriptor_t u = source(e, g);
      VertexDescriptor_t v = target(e, g);

      property_traits<typename TikMap_t>::value_type sourceTicksValue = get(*_pTicksPropMap, u);

      GraphTypes::TokenValue_t targetTokenValue = get(*_pTokenPropMap, v);
      Token targetToken = Token(targetTokenValue);

      property_traits<typename EMap_t>::value_type edgeTicksValue = get(*_pEdgePropMap, e);

      switch ( targetToken.EventId() ) {
        case EventId_t::EventQuoteNormal:
        case EventId_t::EventQuoteCharting:
        case EventId_t::EventTickNormal:
        case EventId_t::EventTickCharting:
        case EventId_t::EventRefreshNormal:
        case EventId_t::EventRefreshCharting:
        case EventId_t::EventOptionRefreshNormal:
        case EventId_t::EventOptionRefreshCharting:
          _timestamps.BucketCount++;
          _timestamps.BucketTime += (edgeTicksValue - sourceTicksValue);
          break;
        case EventId_t::EventConflationStored:
          break;
        case EventId_t::EventConflationCombined:
          break;
        // out of conflation
        case EventId_t::EventConflationPublishing:
          _timestamps.ConflationCount++;
          _timestamps.ConflationTime += (edgeTicksValue - sourceTicksValue);
          break;

        // out of queue
        case EventId_t::EventConnectionStartBuffer:
        case EventId_t::EventConnectionBuffered:
          {
            _timestamps.QueueCount++;
            unsigned long long maxTime = (edgeTicksValue - sourceTicksValue);
            if ( maxTime > _timestamps.MaxQueueTime ) {
              _timestamps.MaxQueueTime = maxTime;
            }
            _timestamps.QueueTime += maxTime;
          }
          break;

        // out of buffer
        case EventId_t::EventConnectionSent:
          {
            _timestamps.BufferedCount++;
            unsigned long long maxTime = (edgeTicksValue - sourceTicksValue);
            if ( maxTime > _timestamps.MaxBufferedTime ) {
              _timestamps.MaxBufferedTime = maxTime;
            }
            _timestamps.BufferedTime += maxTime;
          }
          break;

        // Encrypt/Serialization done
        case EventId_t::EventWriteStart:
          break;

        //I/O completed
        case EventId_t::EventWriteEnd:
          {
            _timestamps.ConnectionWriteCount++;
            unsigned long long maxTime = (edgeTicksValue - sourceTicksValue);
            if ( maxTime > _timestamps.MaxConnectionWriteTime ) {
              _timestamps.MaxConnectionWriteTime = maxTime;
            }
            _timestamps.ConnectionWriteTime += maxTime;
          }
          break;
        default:
          break;
      }
    }

    template < typename Vertex, typename Graph >
    void finish_vertex(Vertex u, const Graph & g) const {
    }

  private:
    TokMap_t * _pTokenPropMap;
    TikMap_t * _pTicksPropMap;
    EMap_t * _pEdgePropMap;
    FMap_t * _pFilterPropMap;
    VisitorTimestamps_t & _timestamps;
  };

}; // namespace NativeInstrumentation
#endif // __ACCUM_GRAPH_VISITOR_H__
