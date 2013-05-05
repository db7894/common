#ifndef __STREAMING_GRAPH_H__
#define __STREAMING_GRAPH_H__

#include <vector>
#include <iostream>
#include "boost/graph/filtered_graph.hpp"
#include "graph.h"
#include "PathType.h"
#include "MarkGraphVisitor.h"

using namespace boost;
using namespace NativeInstrumentation;

namespace StreamingInstrumentation {

  /// <summary>
  /// A class that specializes the Graph::Process method.
  /// </summary>
  template<class DFSVisitor, class DataType>
  class StreamingGraph : public Graph<DFSVisitor, DataType>
  {
  public:

    StreamingGraph ( void (*pGraphProcessor)(DataType & data) , DFSVisitor & visitor , DataType & data , int startEvent )
      : Graph(pGraphProcessor, visitor, data, startEvent)
    {
    }
    virtual ~StreamingGraph ( void ) {}

    virtual void Process ( void ) {
      GraphTypes::PropMaps_t propMaps;
      GetPropMaps(propMaps, _graph);
      std::cout << "Starting Mark Phase ...." << std::endl;
      MarkPhase(propMaps);
      std::cout << "Mark Phase Complete." << std::endl;

      _visitor.InitMaps(&(propMaps.tokenPropMap), &(propMaps.ticksPropMap),
                        &(propMaps.edgeTicksPropMap), &(propMaps.filterPropMap));

      FilterValueColl_t coll;
      coll.push_back(Unknown);
      coll.push_back(Normal);
      ProcessPhase(propMaps, coll);
      coll.clear();
      coll.push_back(Unknown);
      coll.push_back(Chart);
      ProcessPhase(propMaps, coll);
    }
    
  private:
    void MarkPhase ( GraphTypes::PropMaps_t & propMaps ) {
      MarkGraphVisitor<GraphTypes::TokenMap_t,
                       GraphTypes::TicksMap_t,
                       GraphTypes::EdgeTicksMap_t,
                       GraphTypes::FilterVertexMap_t> visitor;
      visitor.InitMaps(&(propMaps.tokenPropMap), &(propMaps.ticksPropMap),
                       &(propMaps.edgeTicksPropMap), &(propMaps.filterPropMap));

      InitializeVertexIndexProp(_graph);

      std::map<GraphTypes::VertexDescriptor_t, default_color_type> colorMap;
      associative_property_map<std::map<GraphTypes::VertexDescriptor_t, default_color_type> > assocColorMap =
        make_assoc_property_map(colorMap);

      // initialize the color map
      graph_traits<GraphTypes::Graph_t>::vertex_iterator vi, vend;
      for ( tie(vi, vend) = vertices(_graph); vi != vend; ++vi ) {
        put(assocColorMap, *vi, white_color);
      }
      //

      long count = 0;
      TokenColl_t::iterator itor = _tokenColl.begin();
      for ( ; itor != _tokenColl.end(); ++itor ) {
        if ( itor->second->IsStartToken ) {
          depth_first_visit(_graph, itor->second->StartDescriptor, visitor, assocColorMap);
          ++count;
        }
      }
      std::cout << "Start tokens visited " << count << "." << std::endl;
    }

    void ProcessPhase ( GraphTypes::PropMaps_t & propMaps , FilterValueColl_t & coll ) {
      std::cout << "Process phase started for: " << std::endl;
      FilterValueColl_t::const_iterator citor = coll.begin();
      for ( ; citor != coll.end(); ++citor ) {
        switch ( *citor ) {
        case Normal:
          std::cout << "  Normal" << std::endl;
          break;
        case Chart:
          std::cout << "  Charting" << std::endl;
          break;
        default:
          std::cout << "  Unknown" << std::endl;
          break;
        }
      }
      TruePathTypeT alwaysTrue;
      GraphTypes::FilterVertexMap_t filterMap = get(GraphTypes::FilterVertex_t(), _graph);
      PathTypeT<GraphTypes::FilterVertexMap_t> filter(&filterMap, &coll);

      typedef filtered_graph<GraphTypes::Graph_t, TruePathTypeT, PathTypeT<GraphTypes::FilterVertexMap_t> > filteredGraph_t;
      filteredGraph_t fg(_graph, alwaysTrue, filter);

      std::map<GraphTypes::VertexDescriptor_t, default_color_type> colorMap;
      associative_property_map<std::map<GraphTypes::VertexDescriptor_t, default_color_type> > assocColorMap =
        make_assoc_property_map(colorMap);

      // initialize the color map
      graph_traits<filteredGraph_t>::vertex_iterator vi, vend;
      for ( tie(vi, vend) = vertices(fg); vi != vend; ++vi ) {
        put(assocColorMap, *vi, white_color);
      }
      //

      long count = 0;
      TokenColl_t::iterator itor = _tokenColl.begin();
      for ( ; itor != _tokenColl.end(); ++itor ) {
        if ( itor->second->IsStartToken ) {
          _data.Clear();
          depth_first_visit(fg, itor->second->StartDescriptor, _visitor, assocColorMap);
          if ( _pGraphProcessor != NULL ) {
            _pGraphProcessor(_data);
          }
          ++count;
        }
      }
      std::cout << "Start tokens visited " << count << "." << std::endl;
      std::cout << "Process phase ended." << std::endl;
    }

  };

}; // namespace StreamingInstrumentation

#endif // __STREAMING_GRAPH_H__