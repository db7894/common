namespace SharedAssemblies.Core.Extensions
{
    /// <summary>
    /// An enumeration that is a collection of the different styles of serialization that can be
    /// applied to a DateTime using the ToString(Xxx) extension method.
    /// </summary>
    public enum DateTimeFormat
    {
        /// <summary>
        /// ODBC format = yyyy-MM-dd HH:mm:ss.nnn
        /// </summary>
        DateTime,

        /// <summary>
        /// ODBC format = yyyy-MM-dd
        /// </summary>
        Date,

        /// <summary>
        /// ODBC format = HH:mm:ss.nnn
        /// </summary>
        Time,

        /// <summary>
        /// HTML format = MM/dd/yy hh:mm:ss AM
        /// </summary>
        HtmlDateTime,

        /// <summary>
        /// HTML format = MM/dd/yy
        /// </summary>
        HtmlDate,

        /// <summary>
        /// HTML format = hh:mm:ss AM
        /// </summary>
        HtmlTime,

        /// <summary>
        /// HTML format = hh:mm AM
        /// </summary>
        HtmlTimeNoSeconds,

        /// <summary>
        /// Numeric format = yyyyMMdd
        /// </summary>
        NumericDate,

        /// <summary>
        /// Numeric format = HHmmssnnn
        /// </summary>
        NumericTime,

        /// <summary>
        /// Numeric format = HHmmss
        /// </summary>
        NumericTimeNoMilliseconds,

        /// <summary>
        /// Displayable format = Month dd, yyyy
        /// </summary>
        DisplayableDate,

        /// <summary>
        /// Full name for month = August, June, etc.
        /// </summary>
        MonthName,

        /// <summary>
        /// Abbreviation for month = Aug, Jun, etc.
        /// </summary>
        AbreviatedMonthName,

        /// <summary>
        /// 2 digit number for month of year
        /// </summary>
        Month,

        /// <summary>
        /// 2 digit number for day of month
        /// </summary>
        Day,

        /// <summary>
        /// 4 digit year
        /// </summary>
        Year
    }
}