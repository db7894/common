using System;
using Microsoft.FxCop.Sdk;
using log4net;
using Microsoft.VisualStudio.CodeAnalysis.Extensibility;

namespace SharedAssemblies.Tools.FxCopRules
{
	/// <summary>
	/// An example custom rule that looks for methods with > 7 parameters
	/// </summary>
	public class NoMoreThanSevenParametersToMethod : BaseIntrospectionRule
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(NoMoreThanSevenParametersToMethod));
		private const int _maxArguments = 7;


		/// <summary>
		/// Make sure all targets are visible, even obsolete, etc.
		/// </summary>
		public override TargetVisibilities TargetVisibility
		{
			get { return TargetVisibilities.All; }
		}


		/// <summary>
		/// Construct the custom rule, passing in name, resource, and assembly.
		/// </summary>
		public NoMoreThanSevenParametersToMethod()
			: base("NoMoreThanSevenParametersToMethod",				// name of the rule
				"SharedAssemblies.Tools.FxCopRules.Rules",			// location of the xml file
				typeof(NoMoreThanSevenParametersToMethod).Assembly)	// assembly that contains xml file
		{			
		}

		/// <summary>
		/// Check each member of the assembly for this problem
		/// </summary>
		/// <param name="member">The member to check</param>
		/// <returns>Any problems found in the check process.</returns>
		public override ProblemCollection Check(Member member)
		{
			var results = new ProblemCollection();

			try
			{
				var method = member as Method;

				// check to see if the member is a method, if so, let's check parameter count!
				if (method != null && method.Parameters.Count > _maxArguments)
				{
					var resolution = GetResolution(method.FullName, _maxArguments);

					results.Add(new Problem(resolution) 
						{ 
							Certainty = 75,
							FixCategory = FixCategories.Breaking,
							MessageLevel = MessageLevel.Warning
						});
				}
			}

			catch (Exception ex)
			{
				_log.Warn("Error checking member.", ex);
			}

			return results;
		}
	}
}
