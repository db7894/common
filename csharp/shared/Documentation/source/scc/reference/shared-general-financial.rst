=========================================================================
Financial --- Financial Business Logic Helpers
=========================================================================
:Assembly: SharedAssemblies.General.Financial.dll
:Namespace: SharedAssemblies.General.Financial
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. module:: SharedAssemblies.General.Financial
   :platform: Windows, .Net
   :synopsis: Financial - Financial Business Logic Helpers

.. highlight:: csharp

.. index:: 
    pair: Financial; DateTime
    pair: BusinessLogic; Financial

Introduction
------------------------------------------------------------

The **SharedAssemblies.General.Financial** assembly is a library of standard financial business 
logic.  Currently all the shared functionality in this library is the **FinanceDateTime** class.
This class encapsulates a standard .Net *System.DateTime* instance and enhances it with several
common Date and Time calculations related to Bashwork's financial days, hours, and holidays.

Usage
--------------------------------------------------------------

To use the **FinanceDateTime** class, you simply instantiate it and then call the desired
methods from it.  There are several ways to instantiates a *FinanceDateTime* including:

    * **Default** - *Constructs a FinanceDateTime instance using the current date and time.*
    * **With a DateTime** - *Constructs a FinanceDateTime which wraps the given date and time.*
    * **With Delta times** - *Constructs a FinanceDateTime that is before/after market open/close.*
    
Once it is instantiated, you can call any of the number of methods to determine how the DateTime fits into
Bashwork's business day::

    // constructs the FinanceDateTime instance with the current DateTime
    var now = new FinanceDateTime();

    // constructs a FinanceDateTime instance with 12/01/1970 08:30:00
    var then = new FinanceDateTime(new DateTime(1970, 12, 01, 8, 30, 0));

    // constructs a FinanceDateTime 15 minutes before open today
    var fifteenBeforeOpen = new FinanceDateTime(0, 15, MarketTimeType.MarketOpen);

    // is it a holiday today?
    Console.WriteLine("Is it a holiday now? " + now.IsHoliday());

    // is it a half holiday today?
    Console.WriteLine("Is it a half-holiday now? " + now.IsHolidayHalfDay());

    // is it a market day today?
    Console.WriteLine("Is it a market day today? " + now.IsMarketDay());

    // is the market currently open?
    Console.WriteLine("Is the market currently open right now? " + now.IsMarketOpen());
    
Configuration
---------------------------------------------------------------------------------

The holidays in **FinanceDateTime** need to be configured.  This is accomplished through
an xml configuration file.  The filename must be FinanceDateTime.config.xml and must be
in the working directory of the application::

    <?xml version="1.0" encoding="utf-8"?>
    <FinanceDateTimeConfiguration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
            xmlns:xsd="http://www.w3.org/2001/XMLSchema">
      <HalfDays>
        <Date>
          <Year>2009</Year>
          <Month>11</Month>
          <Day>27</Day>
        </Date>
        <Date>
          <Year>2009</Year>
          <Month>12</Month>
          <Day>24</Day>
        </Date>
        <Date>
          <Year>2010</Year>
          <Month>11</Month>
          <Day>26</Day>
        </Date>
        <Date>
          <Year>2011</Year>
          <Month>11</Month>
          <Day>25</Day>
        </Date>
      </HalfDays>
      <SpecialHolidays>
        <Date>
          <Year>2007</Year>
          <Month>1</Month>
          <Day>2</Day>
        </Date>
      </SpecialHolidays>
    </FinanceDateTimeConfiguration>
    
Class
---------------------------------------------------------------------------------

The following are a summary of the key classes in the **SharedAssemblies.General.Financial** library.

.. class:: FinanceDateTime

The **FinanceDateTime** class is a business logic class that checks a given *DateTime* against Bashwork's market hours, holidays, etc.

    .. attribute:: FinanceDateTime.DateTimeStamp
    
        :returns: The current *DateTime* this instance represents.
        :rtype: DateTime
        
        This property is used to get or set the current *DateTime* of this *FinanceDateTime* instance.
        
    .. attribute:: FinanceDateTime.Configuration
    
        :returns: The configuration of holidays and hours.
        :rypte: FinanceDateTime.HoursConfiguration
        
        This property is used to get the configuration data for what special holidays, half-holidays, and hours settings
        are to be used in the *FinanceDateTime* hours and market days calculations.
        
        This property is read-only publicly.  To set this value, you must change the configuration value as specified in the
        Configuration section above.

    .. method:: FinanceDateTime.GetNewYearsDay(year)
    
        :param year: The year of the holiday to calculate.
        :type year: int
        :returns: The date of the holiday for *year* specified.
        :rtype: DateTime
        
        This method simply gets the date of the New Year's Day on the *year* specified.
    
    .. method:: FinanceDateTime.GetMartinLutherKingDay(year)
    
        :param year: The year of the holiday to calculate.
        :type year: int
        :returns: The date of the holiday for *year* specified.
        :rtype: DateTime
        
        This method simply gets the date of Martin Luther King, Jr. Day on the *year* specified.
        
    .. method:: FinanceDateTime.GetPresidentsDay(year)
    
        :param year: The year of the holiday to calculate.
        :type year: int
        :returns: The date of the holiday for *year* specified.
        :rtype: DateTime
        
        This method simply gets the date of Presidents' Day for the *year* specified.
        
    .. method:: FinanceDateTime.GetGoodFriday(year)
    
        :param year: The year of the holiday to calculate.
        :type year: int
        :returns: The date of the holiday for *year* specified.
        :rtype: DateTime
        
        This method returns the date of Good Friday for the *year* specified.
        
    .. method:: FinanceDateTime.GetMemorialDay(year)
    
        :param year: The year of the holiday to calculate.
        :type year: int
        :returns: The date of the holiday for *year* specified.
        :rtype: DateTime
        
        This method returns the date of Memorial Day for the *year* specified.
        
    .. method:: FinanceDateTime.GetIndependenceDay(year)
    
        :param year: The year of the holiday to calculate.
        :type year: int
        :returns: The date of the holiday for *year* specified.
        :rtype: DateTime
        
        This method returns the date of Independence Day (4th of July) for the *year* specified.
        
    .. method:: FinanceDateTime.GetLaborDay(year)

        :param year: The year of the holiday to calculate.
        :type year: int
        :returns: The date of the holiday for *year* specified.
        :rtype: DateTime
        
        This method returns the date of Labor Day for the *year* specified.
        
    .. method:: FinanceDateTime.GetThanksgiving(year)
    
        :param year: The year of the holiday to calculate.
        :type year: int
        :returns: The date of the holiday for *year* specified.
        :rtype: DateTime
        
        This method returns the date of Thanksgiving Day for the *year* specified.
        
    .. method:: FinanceDateTime.GetChristmas(year)
    
        :param year: The year of the holiday to calculate.
        :type year: int
        :returns: The date of the holiday for *year* specified.
        :rtype: DateTime
        
        This method returns the date of Christmas Day for the *year* specified.
        
    .. method:: FinanceDateTime.GetNextMarketOpen()
    
        :returns: The date and time of the next market open.
        :rtype: DateTime
        
    .. method:: FinanceDateTime.GetDaysSinceLastMarketDay([date])
    
        :param date: The date to calculate last market open from.
        :type date: DateTime
        :returns: number of days that have elapsed.
        :rtype: int
        
        Returns the number of days that have passed from last market open to *date* specified.  
        If *date* is not specified, uses the internal date of the instance.
        
    .. method:: FinanceDateTime.GetGtcExpirationFromOrderDate(orderDate, securityType)
    
        :param orderDate: The date the order was placed.
        :type orderDate: DateTime
        :param securityType: The type of security on the order.
        :type securityType: MarketCloseType
        :returns: DateTime for GTC expiration of the order.
        :rtype: DateTime
        
        Given an order's date and security type, determine the date and time a GTC order will expire.
        
    .. method:: FinanceDateTime.GetMarketCloseForDate(dateTime, securityType)
    
        :param dateTime: The date to determine market close for.
        :type dateTime: DateTime
        :param securityType: The type of security to use for market close determination.
        :type securityType: MarketCloseType
        :returns: DateTime for market close on that day.
        :rtype: DateTime
        
        Given a date, determine for a type of security at what time the market will close on that day.
        
    .. method:: FinanceDateTime.IsLastMarketDay(then)
    
        :param then: The date to check.
        :type then: DateTime
        :returns: True if *then* was last market day.
        :rtype: bool
        
    .. method:: FinanceDateTime.SetToMarketClose(deltaHours, deltaMinutes)
    
        :param deltaHours: Hours to add.
        :type deltaHours: int
        :param deltaMinutes: Minutes to add.
        :type deltaMinutes: int
        :rtype: void
        
        This method sets the *DateTimeStamp* property of the instance to market close plus the delta hours and minutes specified.
        
    .. method:: FinanceDateTime.SetToMarketOpen(deltaHours, deltaMinutes)
    
        :param deltaHours: Hours to add.
        :type deltaHours: int
        :param deltaMinutes: Minutes to add.
        :type deltaMinutes: int
        :rtype: void
        
        This method sets the *DateTimeStamp* property of the instance to market open plus the delta hours and minutes specified.
        
    .. method:: FinanceDateTime.IsMarketDay()
    
        :returns: True if it is a market day.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of this instance is a market day.
        
    .. method:: FinanceDateTime.IsNearMarketClose([closeType])
    
        :param closeType: Type of market close time.
        :type closeType: MarketCloseType
        :returns: True if near market close.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of this instance is within a 
        three-second lag period before the market close for the given security market type.
        This returns false if *DateTimeStamp* is not a market day.
        
    .. method:: FinanceDateTime.IsMarketOpen([closeType])
        
        :param closeType: Type of market close time.
        :type closeType: MarketCloseType
        :returns: True if market is still open.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of this instance is within the market hours of 
        the given day.  This returns false if *DateTimeStamp* is not a market day.
        
    .. method:: FinanceDateTime.IsPastMarketOpen(minutesPast)
    
        :param minutesPast: Number of minutes past market open to check.
        :type minutesPast: int
        :returns: True if at least *minutesPast* past market open.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of this instance is at least the specified
        number of minutes past market open time.  This returns false if *DateTimeStamp* is not a market day.
        
    .. method:: FinanceDateTime.IsExtendedHoursMarketOpen()
    
        :returns: True if in the extended market hours.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of this instance is in the 
        extended hours period of the market day.  This method returns false if *DateTimeStamp* is not a market day.
        
    .. method:: FinanceDateTime.IsHoliday()
    
        :returns: True if a holiday.
        :rtype: bool
        
        This method returns true
        if the *DateTimeStamp* property of the instance is a holiday.
    
    .. method:: FinanceDateTime.IsHalfDayHoliday()
    
        :returns: True if a holiday.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of the instance is a half-day holiday.
    
    .. method:: FinanceDateTime.IsDuringWideSpread()

        :returns: True if in wide spread.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of this instance is a market day
        and between the pre-market open and pre-market close.  This method returns false otherwise including
        if the day is not a market day.
        
    .. method:: FinanceDateTime.IsValidGtdExpiration()
    
        :returns: True if valid date-time stamp for a GTD order.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of the instance is valid for a GTD order expiration time.
        This does not check any date constraints on GTD.
    
    .. method:: FinanceDateTime.HasMarketOpened()
        
        :returns: True if it is past market open.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of the instance is past market open, but is irrespective
        of the market close.  
        
        This is in contrast to *MarketOpen* which checks to see if the market is currently open and not closed.
        
    .. method:: FinanceDateTime.HasMarketClosed([closeType])
    
        :param closeType: The type of market to check close time against.
        :type closeType: MarketCloseType
        :returns: True if past market close.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of the instance is past market close for the given market type. 
        If no market type is specified, *MarketCloseType.Equity* is assumed.
        
    .. method:: FinanceDateTime.HasExtendedHoursClosed()
        
        :returns: True if it is extended hours has closed
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of the instance is past the market
        extended hours close.
            
    .. method:: FinanceDateTime.HasExtendedOpened()
        
        :returns: True if it is extended hours has opened
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of the instance is past the market
        extended hours open, but is irrespective of the extended hours close.  
        
        This is in contrast to *IsExtendedHoursOpen* which checks to see if we are past open
        of extended hours but not before close of extended hours.
            
    .. method:: FinanceDateTime.HasPreMarketClosed()
    
        :returns: True if it is pre-market hours has closed
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of the instance is past the market
        pre-market hours close.
            
    .. method:: FinanceDateTime.HasPreMarketHoursOpened()
    
        :returns: True if it is pre-market hours has opened
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of the instance is past the market
        pre-market hours open.  
        
        This is in contrast to *IsPreMarketOpen()* which checks to see if we are past pre-market open
        but not past pre-market close.
            
    .. method:: FinanceDateTime.IsPreMarketOpen()
    
        :return: True if pre-market is currently open.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of the instance is currently within
        the open period of the pre-market session.
        
    .. method:: FinanceDateTime.IsWithinFifteenMinutesToOpen()
    
        :return: True if time is within 15 minutes to market open.
        :rtype: bool
        
        This method returns true if the *DateTimeStamp* property of the instance is within the 15 minutes
        before market opens.
        
    .. method:: FinanceDateTime.GetMinutesToMarketOpen()
    
        :return: Number of minutes until market open.
        :rtype: int
        
        This method returns the number of minutes from the *DateTimeStamp* property of the instance to the 
        market open time.

    .. method:: FinanceDateTime.GetMinutesToMarketClose()
    
        :return: Number of minutes until market close.
        :rtype: int
        
        This method returns the number of minutes from the *DateTimeStamp* property of the instance to the 
        market close time.
        
For more information, see the `API Reference <../../../../Api/index.html>`_.