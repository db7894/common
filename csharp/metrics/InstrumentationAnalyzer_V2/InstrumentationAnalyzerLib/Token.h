#ifndef __TOKEN_H__
#define __TOKEN_H__

namespace NativeInstrumentation {

  /// <summary>
  /// A C++ implementation of the Managed C# class
  /// </summary>
  class Token {
  public:

    Token ( void ) : _baseToken(0) {}

    Token ( unsigned long long tokenValue ) : _baseToken(tokenValue) {}

		Token ( int appId, short eventId, unsigned long long id )
		{
			_baseToken = ( (unsigned long long)appId << 54 ) | ( (unsigned long long)eventId << 44 ) | ( id & 0x00000fffffffffff );
		}

		Token ( unsigned char * pBytes , int offset )
		{
		  _baseToken = 0;
		  // little Endian
		  for ( int index = 0; index < TokenSize; ++index )
		  {
			  _baseToken |= (((unsigned long long)pBytes[offset + index]) << (index * 8));
		  }
		}

    short AppId ( void ) const { return (short)(( _baseToken & 0xffC0000000000000 ) >> 54); }

    short EventId ( void ) const { return (short)(( _baseToken & 0x003ff00000000000 ) >> 44); }

    void EventId ( short value ) {
				_baseToken &= 0xffc00fffffffffff;
				_baseToken |= ( ( (unsigned long long)value & 0x3ff ) << 44 );
    }

    unsigned long long Id ( void ) const { return  ( _baseToken & 0x00000fffffffffff ) ; }

    unsigned long long Value ( void ) const { return _baseToken; }

    static Token Null () {
      Token token;
      token._baseToken = 0;
      return token;
    }

    bool IsNull ( void ) const {
      return ( _baseToken == 0 );
    }

    static const int TokenSize;
  private:
    unsigned long long _baseToken;

  };

};

#endif // __TOKEN_H__