using System;
using System.Linq;
using System.Collections.Generic;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation
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
		private static readonly IEnumerable<ValidationFailure> _emptyFailures
			= Enumerable.Empty<ValidationFailure>().ToList();

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
			Descriptions = descriptions ?? _emptyFailures;
		}
	}
}
