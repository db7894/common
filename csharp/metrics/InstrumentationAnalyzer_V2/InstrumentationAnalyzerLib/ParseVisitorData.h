#ifndef __PARSE_VISITOR_DATA_H__
#define __PARSE_VISITOR_DATA_H__

#include <list>
#include "TokenPairRec.h"

namespace NativeInstrumentation {

  /// <summary>
  /// The data used by the ParseGraphVisitor and ParseGraphProcessor.
  /// </summary>
  class ParseVisitorData_t
  {
  public:
    typedef std::list<TokenPairRec> ParseData_t;

    ParseVisitorData_t ( void ){}

    ~ParseVisitorData_t ( void ) {}

    void Clear ( void ) {
      _parseData.clear();
    }

    void Add ( TokenPairRec & rec ) {
      _parseData.push_back(rec);
    }

    const ParseData_t * GetData ( void ) const { return &_parseData; }

  private:
    ParseData_t _parseData;
  };

}; // namespace NativeInstrumentation

#endif // __PARSE_VISITOR_DATA_H__
