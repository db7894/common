using System;

namespace Bashwork.Validation.Attributes
{
	/// <summary>
	/// Validator attribute to define the a method that can validate the given type
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class ValidatorMethodAttribute : Attribute
	{
		/// <summary>
		/// The type that this method validates
		/// </summary>
		public Type ValidatorType { get; private set; }

		/// <summary>
		/// Construct a new instance of the <see cref="ValidatorMethodAttribute"/>
		/// </summary>
		/// <param name="validatorType">The type the method validates</param>
		public ValidatorMethodAttribute(Type validatorType)
		{
			ValidatorType = validatorType;
		}
	}
}
