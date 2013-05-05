#ifndef __EVENT_ID_H__
#define __EVENT_ID_H__


namespace StreamingInstrumentation {
  struct EventId_t {
  public:
    enum ValuesE {
		  EventNull					    = 0,
		  EventQuoteReceived			= 1,
		  EventQuoteNormal			    = 2,
		  EventQuoteCharting			= 3,
		  EventTickReceived			    = 4,
		  EventTickNormal				= 5,
		  EventTickCharting			    = 6,
		  EventRefreshReceived		    = 7,
		  EventRefreshNormal			= 8,
		  EventRefreshCharting			= 9,
		  EventOptionRefreshReceived	= 10,
		  EventOptionRefreshNormal		= 11,
		  EventOptionRefreshCharting	= 12,
		  EventSysMonMsgReceived		= 13,
		  EventSysMonMsgUpdate			= 14,
		  EventShareImbalanceReceived	= 15,
		  EventShareImbalanceUpdate		= 16,
		  EventLevel2Received			= 17,
		  EventLevel2Update				= 18,
		  EventConflationStored		    = 19,
		  EventConflationCombined		= 20,
		  EventConflationPublishing		= 21,
		  EventWriteStart				= 22,
		  EventConnectionQueued		    = 23,
		  EventConnectionStartBuffer	= 24,
		  EventConnectionBuffered		= 25,
		  EventConnectionSent			= 26,
		  EventWriteEnd				    = 27,
		  EventConnectionReject		    = 28
    };
    EventId_t ( void ) : _value(EventNull) {}
    ValuesE Value ( void ) const { return _value; }
    void Value ( ValuesE value ) { _value = value; }
  private:
    ValuesE _value;
  };

}

#endif // __EVENT_ID_H__