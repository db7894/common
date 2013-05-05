using System;
using System.Diagnostics.CodeAnalysis;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Core.Conversions
{
    /// <summary>
    /// utility class that converts objects from one type to another
    /// </summary>
    public static class TypeConverter
    {
        /// <summary>
        /// Converts the given value type to the type specified.  If the value is of dbnull 
        /// then assign the passed default value.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="value">Value of the object</param>
        /// <param name="defaultValue">default value to be used in case the value is dbnull</param>
        /// <returns> converted field type </returns>
        /// <exception>InvalidCastException if cannot convert between the types</exception>
        public static T ConvertTo<T>(IConvertible value, T defaultValue)
        {
            return value.IsNullOrDbNull()
                       ? defaultValue
                       : (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Converts the given value type to the nullable type.  If the value is of dbnull 
        /// then assign the passed default value
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="value">Value of the object</param>
        /// <returns> converted field type </returns>
        /// <exception>InvalidCastException if cannot convert between the types</exception>
        public static T? ConvertToNullable<T>(IConvertible value)
            where T : struct
        {
            T? result = null;

            if (!value.IsNullOrDbNull())
            {
                result = (T)Convert.ChangeType(value, typeof(T));
            }

            return result;
        }

        /// <summary>
        /// Converts the given value type to the specified enum
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="value">value of object</param>
        /// <param name="defaultValue">default value to be used in case the value is dbnull</param>
        /// <returns>converted field type</returns>
        /// <exception>System.ArgumentException if argument does not match enum.</exception>
        public static T ConvertToEnum<T>(IConvertible value, T defaultValue)
        {
            T result = defaultValue;

            if (!value.IsNullOrDbNull())
            {
                result = ConvertEnum<T>(value);
            }

            return result;
        }

        /// <summary>
        /// Converts the given value type to the specified enum
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="value">value of object</param>
        /// <returns>converted field type</returns>
        /// <exception>System.ArgumentException if argument does not match enum.</exception>
        public static T? ConvertToNullableEnum<T>(IConvertible value)
            where T : struct
        {
            T? result = null;

            if (!value.IsNullOrDbNull())
            {
                result = ConvertEnum<T>(value);
            }

            return result;
        }

        /// <summary>
        /// Generically converts from one type to another type.  If the to type (T) is a string,
        /// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
        /// if the type converting to is an enum, it will attempt to parse the enum, otherwise, 
        /// it will attempt a cast.  It will return the defaultValue if the value
        /// is a null reference or the DBNull.Value object.
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="value">The value to convert</param>
        /// <param name="defaultValue">The default value to use if null/DBNull.Value</param>
        /// <returns>The converted value in the converted type</returns>
        /// <exception>InvalidCastException if no conversion possible between the types</exception>
        public static T ToType<T>(object value, T defaultValue)
        {
            T result = defaultValue;

            if (!value.IsNullOrDbNull())
            {
                Type toType = typeof(T);

                if (toType == typeof(string))
                {
                    // you'd think this would be obvious, but compiler barfs...
                    result = (T)(object)value.ToString();
                }
                else
                {
                    IConvertible convertibleValue = value as IConvertible;

                    if (convertibleValue != null)
                    {
                        result = toType.IsEnum
                                     ? convertibleValue.ToEnum<T>(defaultValue)
                                     : convertibleValue.ConvertTo<T>(defaultValue);
                    }
                    else
                    {
                        result = (T)value;
                    }
                }
            }

            return result;
        }

        /// <summary>
		/// <para>
        /// Generically converts from one type to another type.  If the to type (T) is a string,
        /// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
        /// otherwise, it will attempt a cast.  It will return the default(T) if the value
        /// is a null reference or the DBNull.Value object.
		/// </para>
		/// <para> 
        /// Warning!  If T is a value type, then 
        /// that means it will have the default value, not NULL!  If you wnat a value type to 
        /// default to null, use ToNullableType<typeparamref name="T"/>.
		/// </para>
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value in the converted type</returns>
        /// <exception>InvalidCastException if no conversion possible between the types</exception>
        public static T ToType<T>(object value)
        {
            return ToType<T>(value, default(T));
        }

        /// <summary>
		/// <para>
        /// Attempts to convert from one type to another type.  If the to type (T) is a string,
        /// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
        /// if the type converting to is an enum, it will attempt to parse the enum, otherwise, 
        /// it will attempt a cast.  It will return the defaultValue if the value
        /// is a null reference or the DBNull.Value object.
		/// </para>
		/// <para>
        /// If the parse fails, it will absorb the exception and return the default value.  
        /// However, it will NOT absorb an InvalidCastException which means the types were
        /// completely irreconcilable.
		/// </para>
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="value">The value to convert</param>
        /// <param name="defaultValue">The default value to use if null/DBNull.Value</param>
        /// <returns>The converted value in the converted type</returns>
        /// <exception>InvalidCastException if no conversion possible between the types</exception>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException",
			Justification = "Yes, it's a catch-all, but the original intent was for all " +
			    "non-cast exceptions to be handled by the try and don't want to break existing code.")]
		public static T TryToType<T>(object value, T defaultValue)
        {
            try
            {
                return ToType(value, defaultValue);
            }
            catch (InvalidCastException) 
            {
                throw;
            }
            catch (Exception) 
			{
                return defaultValue;
            }
        }

        /// <summary>
        /// <para>
        /// Attempts to convert from one type to another type.  If the to type (T) is a string,
        /// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
        /// if the type converting to is an enum, it will attempt to parse the enum, otherwise, 
        /// it will attempt a cast.  It will return the defaultValue if the value
        /// is a null reference or the DBNull.Value object.
        /// </para>
        /// <para>
        /// If the parse fails, it will absorb the exception and return the default value.  
        /// However, it will NOT absorb an InvalidCastException which means the types were
        /// completely irreconcilable.
        /// </para>
        /// <para>
        /// Warning!  If T is a value type, then 
        /// that means it will have the default value, not NULL!  If you wnat a value type to 
        /// default to null, use ToNullableType<typeparamref name="T"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value in the converted type</returns>
        /// <exception>InvalidCastException if no conversion possible between the types</exception>
        public static T TryToType<T>(object value)
        {
            return TryToType<T>(value, default(T));
        }

        /// <summary>
        /// Generically converts from one type to a nullable type.  If the to type (T) is a string,
        /// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
        /// if the type converting to is an enum, it will attempt to parse the enum, otherwise, 
        /// it will attempt a cast.  It will return the defaultValue if the value
        /// is a null reference or the DBNull.Value object.
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value in the converted type</returns>
        /// <exception>InvalidCastException if no conversion possible between the types</exception>
        public static T? ToNullableType<T>(object value)
            where T : struct
        {
            T? result = null;

            if (!value.IsNullOrDbNull())
            {
                Type toType = typeof(T);

                if (toType == typeof(string))
                {
                    // you'd think this would be obvious, but compiler barfs...
                    result = (T)(object)value.ToString();
                }
                else
                {
                    var convertibleValue = value as IConvertible;

                    if (convertibleValue != null)
                    {
                        result = toType.IsEnum
                                     ? convertibleValue.ToNullableEnum<T>()
                                     : convertibleValue.ConvertToNullable<T>();
                    }
                    else
                    {
                        result = (T)value;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// <para>
        /// Generically converts from one type to a nullable type.  If the to type (T) is a string,
        /// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
        /// if the type converting to is an enum, it will attempt to parse the enum, otherwise, 
        /// it will attempt a cast.  It will return the defaultValue if the value
        /// is a null reference or the DBNull.Value object.
        /// </para>
        /// <para>
        /// If the parse fails, it will absorb the exception and return the default value.  
        /// However, it will NOT absorb an InvalidCastException which means the types were
        /// completely irreconcilable.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value in the converted type</returns>
        /// <exception>InvalidCastException if no conversion possible between the types</exception>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException",
			Justification = "Yes, it's a catch-all, but the original intent was for all " +
				"non-cast exceptions to be handled by the try and don't want to break existing code.")]
		public static T? TryToNullableType<T>(object value)
            where T : struct
        {
            try
            {
                return ToNullableType<T>(value);
            }
            catch (InvalidCastException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Helper method to convert the enum from one type to another
        /// </summary>
        /// <typeparam name="T">The type to convert to, must be enumerated.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        private static T ConvertEnum<T>(IConvertible value)
        {
            Type toType = typeof(T);

            // make sure type is a true enum
            if (!toType.IsEnum)
            {
                throw new ArgumentException("Cannot invoke ToEnum if type T is not an enum type.");
            }

            T result;

            // if value is convertible and a primitive, cast to int and cast to enum
            if (value.GetType().IsPrimitive)
            {
                int intValue = Convert.ToInt32(value);

                result = (T)(object)intValue;
            }

            // if it's not a numeric type, then parse it from a string
            else
            {
                result = (T)Enum.Parse(toType, value.ToString(), true);
            }

            return result;
        }
    }
}