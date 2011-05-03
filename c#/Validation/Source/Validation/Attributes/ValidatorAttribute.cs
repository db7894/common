using System;

namespace Bashwork.Validation.Attributes
{
	/// <summary>
	/// Validator attribute to define the a method that can validate the given type
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ValidatorAttribute : Attribute
	{
	}
}