#ifndef __VISITOR_TIMESTAMPS_H__
#define __VISITOR_TIMESTAMPS_H__

#include "PathType.h"

namespace StreamingInstrumentation {

  struct VisitorTimestamps_t
  {
  public:
    VisitorTimestamps_t ( void ) {
      Clear();
    }

    ~VisitorTimestamps_t ( void ) {
    }

    void Clear ( void ) {
      FilterValue = Unknown;
      StartTime = 0;
      TotalTime = 0;
      MaxTime = 0;
      TotalCount = 0;

      BucketTime = 0;
      BucketCount = 0;

      CombinedCount = 0;
      ConflationTime = 0;
      ConflationCount = 0;
    
      QueueTime = 0;
      MaxQueueTime = 0;
      QueueCount = 0;

      BufferedCombinedCount = 0;
      BufferedTime = 0;
      MaxBufferedTime = 0;
      BufferedCount = 0;

      ConnectionWriteTime = 0;
      MaxConnectionWriteTime = 0;
      ConnectionWriteCount = 0;
    }

    FilterValueE FilterValue;
    unsigned long long StartTime;
    unsigned long long TotalTime;
    unsigned long long MaxTime;
    unsigned long TotalCount;

    unsigned long long BucketTime;
    unsigned long BucketCount;
    
    unsigned long CombinedCount;
    unsigned long long ConflationTime;
    unsigned long ConflationCount;

    unsigned long long QueueTime;
    unsigned long long MaxQueueTime;
    unsigned long QueueCount;

    unsigned long BufferedCombinedCount;
    unsigned long long BufferedTime;
    unsigned long long MaxBufferedTime;
    unsigned long BufferedCount;

    unsigned long long ConnectionWriteTime;
    unsigned long long MaxConnectionWriteTime;
    unsigned long ConnectionWriteCount;
  };

}; // namespace NativeInstrumentation

#endif // __VISITOR_TIMESTAMPS_H__