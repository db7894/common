using System;


namespace SharedAssemblies.Core.Conversions
{
	/// <summary>
	/// A helper class that allows you to cast generic TEnum enum types to int and back without incurring boxing costs.
	/// </summary>
	/// <typeparam name="TEnum">The type of enum to cast, MUST be an enum with int backing type.</typeparam>
	public static class EnumConverter<TEnum> where TEnum : struct
	{
		// Identity function, needed to spoof the int <--> TEnum delegates
		private static readonly Func<int, int> _identity = x => x;

		// the reverse cast delegate from TEnum --> int
		private static readonly Func<int, TEnum> _enumProjection = CreateEnumProjection();

		// the forward cast delegate from int --> TEnum
		private static readonly Func<TEnum, int> _intProjection = CreateIntProjection();

		/// <summary>
		/// Gets the delegate that converts the int to a TEnum.
		/// </summary>
		/// <remarks>
		/// By returning the delegate as a property instead of creating a method to invoke, we can take advantage of some JIT
		/// optimizations that improve the call performance (in-lining, etc).
		/// </remarks>
		public static Func<int, TEnum> ToEnum
		{
			get { return _enumProjection; }
		}

		/// <summary>
		/// Gets the delegate that converts the TEnum to an int.  
		/// </summary>
		/// <remarks>
		/// By returning the delegate as a property instead of creating a method to invoke, we can take advantage of some JIT
		/// optimizations that improve the call performance (in-lining, etc).
		/// </remarks>
		public static Func<TEnum, int> ToInt
		{
			get { return _intProjection; }
		}

		// Helper method to check compatibility and make the delegate
		private static Func<int, TEnum> CreateEnumProjection()
		{
			var type = typeof(TEnum);

			if (!type.IsEnum)
			{
				throw new InvalidOperationException("GenericEnumCaster can only be used on enum types.");
			}

			if (type.GetEnumUnderlyingType() != typeof(int))
			{
				throw new InvalidOperationException("GenericEnumCaster can only be used on enum types with int underlying type.");
			}

			return (Func<int,TEnum>)Delegate.CreateDelegate(typeof(Func<int, TEnum>), _identity.Method);
		}

		// Helper method to check compatibility and make the delegate
		private static Func<TEnum, int> CreateIntProjection()
		{
			var type = typeof(TEnum);

			if (!type.IsEnum)
			{
				throw new InvalidOperationException("GenericEnumCaster can only be used on enum types.");
			}

			if (type.GetEnumUnderlyingType() != typeof(int))
			{
				throw new InvalidOperationException("GenericEnumCaster can only be used on enum types with int underlying type.");
			}

			return (Func<TEnum, int>) Delegate.CreateDelegate(typeof(Func<TEnum, int>), _identity.Method);
		}
	}
}
