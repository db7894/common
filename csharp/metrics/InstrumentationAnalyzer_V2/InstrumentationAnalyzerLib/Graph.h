#ifndef __GRAPH_H__
#define __GRAPH_H__

#include <map>

#include "TokenPairRec.h"
#include "GraphTypes.h"
#include "IGraph.h"

using namespace boost;

namespace NativeInstrumentation {

  /// <summary>
  /// Implementation of IGraph.  Implements the boost Adjacency List processing given a depth first search visitor.
  /// Class will callback a supplied static void func(DataType &) function after visiting each vertex in a path starting from 
  /// each start vertex.
  /// </summary>
  template<class DFSVisitor, class DataType>
  class Graph : public IGraph
	{
	public:
		
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pGraphProcessor">Static callback function pointer.</param>
    /// <param name="visitor">The depth first visitor to use.</param>
    /// <param name="data">An instance of data that your visitor and callback function will use.</param>
    /// <param name="startEvent">The start event number.</param>
    Graph ( void (*pGraphProcessor)(DataType & data) , DFSVisitor & visitor , DataType & data , int startEvent )
      :_startEvent(startEvent)
      ,_pGraphProcessor(pGraphProcessor)
      ,_visitor(visitor)
      ,_data(data)
    {
    }

    /// <summary>
    /// Destructor
    /// </summary>
    virtual ~Graph ( void ) {
      TokenColl_t::iterator itor = _tokenColl.begin();
      for ( ; itor != _tokenColl.end(); ++itor ) {
        delete itor->second;
      }
    }

    /// <summary>
    /// Clears the underlying graph and token collection.
    /// </summary>
    virtual void Clear ( void ) {
      _graph.clear();
      TokenColl_t::iterator itor = _tokenColl.begin();
      for ( ; itor != _tokenColl.end(); ++itor ) {
        delete itor->second;
      }
      _tokenColl.clear();
    }
    
    /// <summary>
    /// Added the token pair record to the graph.
    /// </summary>
    /// <param name="rec">The token pair record to add.</param>
    virtual void Put ( const TokenPairRec & rec ) {
      static long testCount = 0;

      if ( rec.T2().IsNull() ) {
        std::cout << "Invalid TokenPairRec, T2 is null" << std::endl;
        return;
      }

      if ( rec.T2().EventId() == _startEvent && rec.T1().IsNull() ) {
        TokenColl_t::const_iterator itor = _tokenColl.find(rec.T2().Id());
        if ( itor == _tokenColl.end() ) {
          _tokenColl[rec.T2().Id()] =
            new TokenCollValue_t(add_vertex(
              GraphTypes::TokenProp_t(rec.T2().Value(),
                                      GraphTypes::TicksProp_t(rec.Timestamp() )), _graph), true);
        }
        else {
          std::cout << "Duplicate start event, token id = " << rec.T2().Value() << std::endl;
        }
      }
      else if ( rec.T1().IsNull() ) {
          return; // this is a start token we aren't interested in
      }
      else {
        TokenColl_t::iterator itor1 = _tokenColl.find(rec.T1().Id());
        if ( itor1 == _tokenColl.end() ) {
          return; // this is a token we aren't interested in
        }
        if ( itor1->first == rec.T2().Id() ) {
          // this is an event change only
          GraphTypes::VertexDescriptor_t descriptor = add_vertex(
            GraphTypes::TokenProp_t(rec.T2().Value(),
                                    GraphTypes::TicksProp_t(rec.Timestamp() )), _graph);
          add_edge(itor1->second->CurDescriptor, descriptor, GraphTypes::EdgeTicksProp_t(rec.Timestamp()), _graph);
          // update the descriptor
          itor1->second->CurDescriptor = descriptor;
        }
        else {
          TokenColl_t::iterator itor2 = _tokenColl.find(rec.T2().Id());
          if ( itor2 == _tokenColl.end() ) {
            // this is a split leg
            GraphTypes::VertexDescriptor_t descriptor = add_vertex(
              GraphTypes::TokenProp_t(rec.T2().Value(),
                                      GraphTypes::TicksProp_t(rec.Timestamp() )), _graph);
            add_edge(itor1->second->CurDescriptor, descriptor, GraphTypes::EdgeTicksProp_t(rec.Timestamp()), _graph);
            _tokenColl[rec.T2().Id()] = new TokenCollValue_t(descriptor, false);
          }
          else {
            // this is a join
            // create the joing vertex and edge for t1
            GraphTypes::VertexDescriptor_t descriptor = add_vertex(
              GraphTypes::TokenProp_t(rec.T1().Value(),
                                      GraphTypes::TicksProp_t(rec.Timestamp() )), _graph);
            add_edge(itor1->second->CurDescriptor, descriptor, GraphTypes::EdgeTicksProp_t(rec.Timestamp()), _graph);
            // update the descriptor
            itor1->second->CurDescriptor = descriptor;

            // need to join t2 into t1 since t1 is later in time
            add_edge(itor2->second->CurDescriptor, itor1->second->CurDescriptor, GraphTypes::EdgeTicksProp_t(rec.Timestamp()), _graph);
          }
        }
      }
    }

    /// <summary>
    /// Performs a depth first search for each start token.
    /// </summary>
    virtual void Process ( void ) {
      GraphTypes::PropMaps_t propMaps;
      GetPropMaps(propMaps, _graph);

      _visitor.InitMaps(&(propMaps.tokenPropMap), &(propMaps.ticksPropMap),
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

      TokenColl_t::iterator itor = _tokenColl.begin();
      for ( ; itor != _tokenColl.end(); ++itor ) {
        if ( itor->second->IsStartToken ) {
          _data.Clear();
          depth_first_visit(_graph, itor->second->StartDescriptor, _visitor, assocColorMap);
          if ( _pGraphProcessor != NULL ) {
            _pGraphProcessor(_data);
          }
        }
      }
    }

  protected:
    /// <summary>
    /// Structure to add to the token collection.
    /// </summary>
    struct TokenCollValue_t {
      TokenCollValue_t ( void ) 
        :StartDescriptor(0)
        ,CurDescriptor(0)
        ,IsStartToken(false)
      {
      }
      TokenCollValue_t ( GraphTypes::VertexDescriptor_t curDescriptor ) 
        :StartDescriptor(0)
        ,CurDescriptor(curDescriptor)
        ,IsStartToken(false)
      {
      }
      TokenCollValue_t ( GraphTypes::VertexDescriptor_t descriptor , bool isStartToken ) 
        :StartDescriptor(isStartToken ? descriptor : 0)
        ,CurDescriptor(descriptor)
        ,IsStartToken(isStartToken)
      {
      }
      GraphTypes::VertexDescriptor_t StartDescriptor;
      GraphTypes::VertexDescriptor_t CurDescriptor;
      bool IsStartToken;
    };

    typedef std::map<unsigned long long, TokenCollValue_t *> TokenColl_t;

    /// <summary>
    /// The boost adjacency list graph.
    /// </summary>
    GraphTypes::Graph_t _graph;

    /// <summary>
    /// The collection of TokenCollValues.
    /// </summary>
    TokenColl_t _tokenColl;

    /// <summary>
    /// The start events we're interested in.
    /// </summary>
    int _startEvent;

    /// <summary>
    /// The callback function.
    /// </summary>
    void (*_pGraphProcessor)(DataType & data);

    /// <summary>
    /// The depth first visitor.
    /// </summary>
    DFSVisitor & _visitor;

    /// <summary>
    /// The data reference.
    /// </summary>
    DataType & _data;

    /// <summary>
    /// Loads up the property maps structure with the property maps from the graph.
    /// </summary>
    /// <param name="propMaps">A reference to the maps structure.</param>
    /// <param name="graph">The graph that contains the maps.</param>
    void GetPropMaps ( GraphTypes::PropMaps_t & propMaps , GraphTypes::Graph_t & graph ) {
      propMaps.tokenPropMap = get(GraphTypes::Token_t(), graph);
      propMaps.ticksPropMap = get(GraphTypes::Ticks_t(), graph);
      propMaps.edgeTicksPropMap = get(GraphTypes::EdgeTicks_t(), graph);
      propMaps.filterPropMap = get(GraphTypes::FilterVertex_t(), graph);
    }

    /// <summary>
    /// Initializes the vertex index property.
    /// Only necessary when vertices are listS (not vecS)
    /// </summary>
    /// <param name="graph">The graph whos map is initialized.</param>
    void InitializeVertexIndexProp ( GraphTypes::Graph_t graph ) {
      property_map<GraphTypes::Graph_t, vertex_index_t>::type index = get(vertex_index, graph);
      graph_traits<GraphTypes::Graph_t>::vertex_iterator vi, vend;
      graph_traits<GraphTypes::Graph_t>::vertices_size_type count = 0;
      for ( tie(vi, vend) = vertices(graph); vi != vend; ++vi ) {
        put(index, *vi, count++);
      }
    }

	};

}; // namespace NativeInstrumentation

#endif // __GRAPH_H__