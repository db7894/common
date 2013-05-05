using System;

namespace SharedAssemblies.General.Financial
{
	/// <summary>
	/// Defined as a partial class to allow us to separate the FinanceDateTime.Configuration 
	/// into its own file for readability.
	/// </summary>
	public partial class FinanceDateTime
	{
		/// <summary>
		/// Defined as a partial class to allow us to separate FinanceDateTime.Configuration.Time
		/// into its own file for readability.
		/// </summary>
		public partial class HoursConfiguration
		{
			/// <summary>
			/// Class for hour/minute pairs
			/// </summary>
			public struct Time
			{
				/// <summary>
				/// Hour property
				/// </summary>
				public int Hour { get; set; }


				/// <summary>
				/// Minute property
				/// </summary>
				public int Minute { get; set; }

				/// <summary>
				/// Constructs a simple time structure for storing open/close hours
				/// </summary>
				/// <param name="hour">Hour of the simple time</param>
				/// <param name="minute">Minute of the simple time</param>
				public Time(int hour, int minute)
					: this()
				{
					Hour = hour;
					Minute = minute;
				}

				/// <summary>
				/// Converts the Time to a string representation of the time.
				/// </summary>
				/// <returns>Returns the time in ShortTimeString format.</returns>
				public override string ToString()
				{
					// actual date is irrelevant, but can't be zero
					return new DateTime(2010, 12, 30, Hour, Minute, 0).ToShortTimeString();
				}
			}
		}
	}
}