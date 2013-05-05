#include "AccumProcessor.h"

using namespace NativeInstrumentation;
using namespace StreamingInstrumentation;

std::ofstream * AccumProcessor::_pOutStream = NULL;
AccumProcessor::AccumGraph_t * AccumProcessor::_pGraph = NULL;
AccumProcessor::AccumGraphVisitor_t * AccumProcessor::_pVis = NULL;
VisitorTimestamps_t * AccumProcessor::_pTimestamps = NULL;

bool AccumProcessor::Init ( std::ofstream * pOutStream , int startEvent ) {
  _pTimestamps = new VisitorTimestamps_t();
  _pVis = new AccumGraphVisitor_t(*_pTimestamps);
  _pGraph = new AccumGraph_t(&GraphProcessor, *_pVis, *_pTimestamps, startEvent);
  _pOutStream = pOutStream;
  WriteAccumHeader();
  return _pOutStream->good();
}

void AccumProcessor::Dispose ( void ) {
  if ( _pGraph != NULL ) {
    delete _pGraph;
    _pGraph = NULL;
  }
  if ( _pVis != NULL ) {
    delete _pVis;
    _pVis = NULL;
  }
  if ( _pTimestamps != NULL ) {
    delete _pTimestamps;
    _pTimestamps = NULL;
  }
}

void AccumProcessor::WriteAccumHeader ( void ) {
  *_pOutStream << "Path Type,";
  *_pOutStream << "Start Time,";
  *_pOutStream << "Total Time,";
  *_pOutStream << "Max Time,";
  *_pOutStream << "Total Count,";
  
  *_pOutStream << "Bucket Time,";
  *_pOutStream << "Bucket Count,";
  
  *_pOutStream << "Conflation Time,";
  *_pOutStream << "Conflation Count,";
  *_pOutStream << "Combined Count,";

  *_pOutStream << "Queued Time,";
  *_pOutStream << "Max Queued Time,";
  *_pOutStream << "Queued Count,";

  *_pOutStream << "Buffered Time,";
  *_pOutStream << "Max Buffered Time,";
  *_pOutStream << "Buffered Count,";
  *_pOutStream << "Buffered Combined Count,";

  *_pOutStream << "Write Time,";
  *_pOutStream << "Max Write Time,";
  *_pOutStream << "Write Count" << std::endl;
}

void AccumProcessor::GraphProcessor ( VisitorTimestamps_t & data ) {
  switch ( data.FilterValue ) {
    case Normal:
      *_pOutStream << "Normal,";
      break;
    case Chart:
      *_pOutStream << "Chart,";
      break;
    default:
      *_pOutStream << "Unknown,";
      break;
  }
  *_pOutStream << data.StartTime << ",";
  *_pOutStream << data.TotalTime << ",";
  *_pOutStream << data.MaxTime << ",";
  *_pOutStream << data.TotalCount << ","; 

  *_pOutStream << data.BucketTime << ","; 
  *_pOutStream << data.BucketCount << ","; 

  *_pOutStream << data.ConflationTime << ","; 
  *_pOutStream << data.ConflationCount << ","; 
  *_pOutStream << data.CombinedCount << ",";

  *_pOutStream << data.QueueTime << ",";
  *_pOutStream << data.MaxQueueTime << ",";
  *_pOutStream << data.QueueCount << ",";

  *_pOutStream << data.BufferedTime << ",";
  *_pOutStream << data.MaxBufferedTime << ",";
  *_pOutStream << data.BufferedCount << ",";
  *_pOutStream << data.BufferedCombinedCount << ",";

  *_pOutStream << data.ConnectionWriteTime << ",";
  *_pOutStream << data.MaxConnectionWriteTime << ",";
  *_pOutStream << data.ConnectionWriteCount << std::endl; 
}

