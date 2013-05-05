using System;
using SharedAssemblies.Core.Conversions;

namespace SharedAssemblies.Core.Extensions
{
    /// <summary>
    /// Utility class that converts objects from one type to another.
    /// </summary>
    public static class ConvertibleExtensions
    {
        /// <summary>
        /// Converts the given value type to the type specified.  If the value is of dbnull 
        /// then assign the passed default value.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="value">Value of the object.</param>
        /// <param name="defaultValue">Default value to be used in case the value is dbnull.</param>
        /// <returns>Converted field type.</returns>
        /// <exception>InvalidCastException if cannot convert between the types.</exception>
        public static T ConvertTo<T>(this IConvertible value, T defaultValue)
        {
            return TypeConverter.ConvertTo<T>(value, defaultValue);
        }

        /// <summary>
        /// Converts the given value type to the nullable type.  If the value is of dbnull 
        /// then assign the passed default value.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="value">Value of the object.</param>
        /// <returns>Converted field type.</returns>
        /// <exception>InvalidCastException if cannot convert between the types.</exception>
        public static T? ConvertToNullable<T>(this IConvertible value)
            where T : struct
        {
            return TypeConverter.ConvertToNullable<T>(value);
        }


        /// <summary>
        /// Converts the given value type to the specified enum.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="value">Value of object.</param>
        /// <param name="defaultValue">Default value to be used in case the value is dbnull.</param>
        /// <returns>Converted field type.</returns>
        /// <exception>System.ArgumentException if argument does not match enum.</exception>
        public static T ToEnum<T>(this IConvertible value, T defaultValue)
        {
            return TypeConverter.ConvertToEnum<T>(value, defaultValue);
        }

        /// <summary>
        /// Converts the given value type to the specified enum.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="value">Value of object.</param>
        /// <returns>Converted field type.</returns>
        /// <exception>System.ArgumentException if argument does not match enum.</exception>
        public static T? ToNullableEnum<T>(this IConvertible value)
            where T : struct
        {
            return TypeConverter.ConvertToNullableEnum<T>(value);
        }


        /// <summary>
        /// Returns true if the value given is defined in the enum T, where T
        /// must be a valid enumerated type.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="value">The value to check against the enum.</param>
        /// <returns>True if the value is in the enum's range.</returns>
        public static bool IsDefinedInEnum<T>(this IConvertible value)
            where T : struct
        {
            return Enum.IsDefined(typeof(T), value);
        }
    }
}