
namespace Bashwork.General.Validation
{
	/// <summary>
	/// A collection of options that define the behavior of the
	/// current validation context
	/// </summary>
	public sealed class ValidationOptions
	{
		/// <summary>
		/// Specifies the behavior when the first validation error is found
		/// </summary>
		public CascadeMode CascadeBehavior { get; set; }

		/// <summary>
		/// Specifies the behavior of how the validation errors are reported
		/// </summary>
		public ReportingMode ReportingBehavior { get; set; }

		/// <summary>
		/// Specifies if the library should build error messages or just report
		/// that an error occured validating the given property.
		/// </summary>
		public MessageLevel MessageBehavior { get; set; }

		/// <summary>
		/// Helper factory to return the default options for the validation
		/// context.
		/// </summary>
		public static ValidationOptions Default
		{
			get
			{
				return new ValidationOptions
				{					
					MessageBehavior = MessageLevel.Normal,
					CascadeBehavior = CascadeMode.Continue,
					ReportingBehavior = ReportingMode.ReturnValidationResults,
				};
			}
		}
	}
}
