using System;
using System.Collections.Generic;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation
{
	/// <summary>
	/// Represents an exception that is thrown by this library.
	/// </summary>
	[Serializable]
	public sealed class ValidationException : Exception
	{
		/// <summary>
		/// A handle to the descriptions of all the validation failures
		/// </summary>
		public IEnumerable<ValidationFailure> Descriptions { get; private set;  }

		/// <summary>
		/// Initialize a new instance of the ValidationException class
		/// </summary>
		/// <param name="description">A single validation failures</param>
		public ValidationException(ValidationFailure description)
			: this(new[] { description })
		{
		}

		/// <summary>
		/// Initialize a new instance of the ValidationException class
		/// </summary>
		/// <param name="descriptions">A collection of the validation failures</param>
		public ValidationException(IEnumerable<ValidationFailure> descriptions)
			: base(MessageResources.ValidationException)
		{
			Descriptions = descriptions;
		}
	}
}
