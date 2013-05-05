#ifndef __TOKEN_PAIR_REC_H__
#define __TOKEN_PAIR_REC_H__

#include "Token.h"

namespace NativeInstrumentation {

  /// <summary>
  /// A C++ implementation of the managed C# class.
  /// </summary>
  class TokenPairRec {
  public:
    TokenPairRec ( void ) : _timestamp(0) {}
    TokenPairRec ( Token t1 , Token t2 , unsigned long long timestamp )
      :_t1(t1), _t2(t2), _timestamp(timestamp) {}
    TokenPairRec ( unsigned char * pBytes , int offset )
      :_t1(Token(pBytes, offset))
      ,_t2(Token(pBytes, offset + Token::TokenSize))
			,_timestamp(0)
    {
      offset += (2 * Token::TokenSize);
			// little Endian
			for ( int index = 0; index < sizeof(long); ++index )
			{
				_timestamp |= ( (long)pBytes[offset + index] << ( index * 8) );
			}
    }

    Token T1 ( void ) const { return _t1; }
    Token T2 ( void ) const { return _t2; }
    unsigned long long Timestamp ( void ) const { return _timestamp; }

    static const int TokenPairRecSize;

  private:
    Token _t1;
    Token _t2;
    unsigned long long _timestamp;
  };

};


#endif // __TOKEN_PAIR_REC_H__