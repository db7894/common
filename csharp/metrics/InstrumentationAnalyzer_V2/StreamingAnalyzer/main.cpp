#include <iostream>
#include <fstream>
#include <string>
#include "time.h"

#include "ParseProcessor.h"
#include "AccumProcessor.h"
#include "EventId.h"

using namespace NativeInstrumentation;
using namespace StreamingInstrumentation;

static std::string _inFile;
static std::string _outFile;

static std::basic_ifstream<unsigned char> _inStream;
static std::ofstream _outStream;

static EventId_t::ValuesE _startEvent;

static bool _parseProcessor = false;

static long long _chunkSize = 1000000;

struct TokenElement {
  TokenElement ( Token token , unsigned long long timestamp )
    :Token(token), Timestamp(timestamp)
  {
  }
  Token Token;
  unsigned long long Timestamp;
};

void Usage ( void ) {
  std::cout << "Usage: InstrumentationAnalyzer -i=inputfile -o=outputfile -e=startEventNum [-p=1] [-c=1000000]" << std::endl;
}

bool ValidateInFile ( const char * pInFile ) {
  if ( strlen(pInFile) < 4 ) {
    std::cout << "Invalid argument \'" << pInFile << "\'" << std::endl;
    return false;
  }
  _inFile = &(pInFile[3]);
  return true;
}

bool ValidateOutFile ( const char * pOutFile ) {
  if ( strlen(pOutFile) < 4 ) {
    std::cout << "Invalid argument \'" << pOutFile << "\'" << std::endl;
    return false;
  }
  _outFile = &(pOutFile[3]);
  return true;
}

bool ValidateStartEvent ( const char * pStartEvent ) {
  if ( strlen(pStartEvent) < 4 ) {
    std::cout << "Invalid argument \'" << pStartEvent << "\'" << std::endl;
    return false;
  }
  int i = atoi(&(pStartEvent[3]));
  if ( i <= EventId_t::EventNull || i > EventId_t::EventConnectionReject ) {
    std::cout << "Invalid argument \'" << pStartEvent << "\'" << std::endl;
    return false;
  }
  _startEvent = (EventId_t::ValuesE)i;
  return true;
}

bool ValidateParseProcessor ( const char * pParse ) {
  if ( strlen(pParse) != 4 ) {
    std::cout << "Invalid argument \'" << pParse << "\'" << std::endl;
    return false;
  }
  int i = atoi(&(pParse[3]));
  if ( i != 0 && i != 1 ) {
    std::cout << "Invalid argument \'" << pParse << "\'" << std::endl;
    return false;
  }
  _parseProcessor = ( i == 1 ? true : false );
  return true;
}

bool ValidateChunkSize ( const char * pChunkSize ) {
  if ( strlen(pChunkSize) < 4 ) {
    std::cout << "Invalid argument \'" << pChunkSize << "\'" << std::endl;
    return false;
  }
  _chunkSize = std::stoll(&(pChunkSize[3]));
  if ( _chunkSize <= 100 ) {
    std::cout << "Invalid argument \'" << pChunkSize << "\'" << std::endl;
    return false;
  }
  return true;
}

bool OpenInFile ( void ) {
  _inStream.open(_inFile.c_str(), std::ios_base::in|std::ios_base::binary);
  if ( !_inStream.is_open() ) {
    std::cout << "Unable to open input file \'" << _inFile << "\'" << std::endl;
    return false;
  }
  return true;
}

bool OpenOutFile ( void ) {
  _outStream.open(_outFile.c_str());
  if ( !_outStream.is_open() ) {
    std::cout << "Unable to open output file \'" << _outFile << "\'" << std::endl;
    return false;
  }
  return true;
}

bool ParseArgs ( int argc , char * argv[] ) {
  bool retVal = true;
  
  if ( argc < 4 ) {
    std::cout << "Invalid arguments" << std::endl;
    retVal = false;
  }
  else {
    int index = 0;
    while ( retVal && ++index < argc ) {
      if ( argv[index][0] == '-' ) {
        switch ( argv[index][1] ) {
			    case 'i':
				    if ( !ValidateInFile(argv[index]) )
				    {
					    retVal = false;
				    }
				    break;
			    case 'o':
				    if ( !ValidateOutFile(argv[index]) )
				    {
					    retVal = false;
				    }
				    break;
			    case 'e':
				    if ( !ValidateStartEvent(argv[index]) )
				    {
					    retVal = false;
				    }
				    break;
          case 'p':
            if ( !ValidateParseProcessor(argv[index]) ) {
              retVal = false;
            }
            break;
          case 'c':
            if ( !ValidateChunkSize(argv[index]) ) {
              retVal = false;
            }
            break;
			    default:
            std::cout << "Invalid argument \'" << argv[index] << "\'" << std::endl;;
				    retVal = false;
				    break;
          }
      }
      else {
        std::cout << "Invalid argument \'" << argv[index] << "\'" << std::endl;;
				retVal = false;
      }
    }
  }
  if ( !retVal ) {
    Usage();
  }
  return retVal;
}

void DoWork ( IGraph & graph ) {
  unsigned char pBytes[100];
  bool done = false;
  long long recCount = 0;
  bool dirty = false;
  int chunkCount = 0;

  while ( !done ) {
    _inStream.read(pBytes, TokenPairRec::TokenPairRecSize);
    if ( _inStream.fail() ) {
      done = true;
      std::cout << "Invalid input file." << std::endl;
      std::cout << "Read " << recCount << " records." << std::endl;
    }
    else if ( _inStream.eof() ) {
      done = true;
      std::cout << "Read " << recCount << " records." << std::endl;
    }
    else {
      TokenPairRec rec ( pBytes, 0);
      if ( rec.T2().AppId() > 0 ) {
        graph.Put(rec);
        dirty = true;
        if ( (++recCount % _chunkSize) == 0 ) {
          struct tm * t;
          time_t clock;
          time(&clock);
          t = localtime(&clock);
          std::cout << "-- " << asctime(t) << "Processing chunk " << ++chunkCount << ", through record " << recCount << " ...." << std::endl;
          graph.Process();
          graph.Clear();
          dirty = false;
        }
      }
    }
  }
  if ( dirty ) {
    graph.Process();
  }
}

int main ( int argc , char * argv[] ) {
  if ( ParseArgs(argc, argv) ) {
    std::cout << "Instrumentation Analyzer starting ...." << std::endl;
    if ( OpenInFile() && OpenOutFile() ) {
      if ( _parseProcessor ) {
        ParseProcessor::Init(&_outStream, _startEvent);
        DoWork(ParseProcessor::GetGraph());
        ParseProcessor::Dispose();
      }
      else {
        AccumProcessor::Init(&_outStream, _startEvent);
        DoWork(AccumProcessor::GetGraph());
        AccumProcessor::Dispose();
      }
    }
    if ( _inStream.is_open() ) {
      _inStream.close();
    }
    if ( _outStream.is_open() ) {
      _outStream.flush();
      _outStream.close();
    }
  }
  
  return 0;
}