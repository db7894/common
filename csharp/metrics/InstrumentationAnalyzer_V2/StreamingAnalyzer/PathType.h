#ifndef __PATH_TYPE_H__
#define __PATH_TYPE_H__

#include <vector>

namespace StreamingInstrumentation {

  enum FilterValueE {
    Unknown,
    Normal,
    Chart
  };
  typedef std::vector<FilterValueE> FilterValueColl_t;
  template <class FilterMapT>
  struct PathTypeT {
    PathTypeT ( void ) {}
    PathTypeT ( FilterMapT * pFilterMap , FilterValueColl_t * pFilterValueColl) : _pFilterMap(pFilterMap), _pFilterValueColl(pFilterValueColl) {}
    template <class VertexT>
    bool operator()(const VertexT v) const {
      FilterValueColl_t::const_iterator itor = _pFilterValueColl->cbegin();
      for ( ; itor != _pFilterValueColl->end(); ++itor ) {
        if ( *itor == static_cast<FilterValueE>(get(*_pFilterMap, v)) ) {
          return true;
        }
      }
      return false;
    }
    FilterMapT * _pFilterMap;
    FilterValueColl_t * _pFilterValueColl;
  };

  struct TruePathTypeT {
    TruePathTypeT ( void ) {}
    template <class EdgeT>
    bool operator()(const EdgeT v) const {
      return true;
    }
  };

}; // namespace StreamingInstrumentation

#endif // __PATH_TYPE_H__