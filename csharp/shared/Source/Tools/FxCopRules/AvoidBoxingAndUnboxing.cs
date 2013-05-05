using System;
using Microsoft.FxCop.Sdk;

namespace SharedAssemblies.Tools.FxCopRules
{
	/// <summary>Rule to check for excessive boxing/unboxing.</summary>
	public class AvoidBoxingAndUnboxing : BaseIntrospectionRule
	{
		private int _boxInstuctCount = 0;         // # of BOX instructions 
		private int _unboxInstructCount = 0;      // # of UNBOX instructions
		private static int _maxBoxInstruct = 3;   // max # BOX instructions allowed
		private static int _maxUnBoxInstruct = 0; // max # UNBOX instructions allowed

		/// <summary>Gets the base class hooked up.</summary>
		public AvoidBoxingAndUnboxing() : base("AvoidBoxingAndUnboxing",
			"SharedAssemblies.Tools.FxCopRules.Rules",
			typeof(AvoidBoxingAndUnboxing).Assembly)
		{
		}

		/// <summary>Check for excessive box/unbox instructions.</summary>
		/// <param name="member">The method to check.</param>
		/// <returns>null if no problems; otherwise, !null.</returns>
		public override ProblemCollection Check(Member member)
		{
			var result = new ProblemCollection();
			var memberMethod = member as Method;

			if (memberMethod != null)
			{
				// Always set the counters to a known value before starting.
				_boxInstuctCount = 0;
				_unboxInstructCount = 0;

				// Walk this method's code.  I'm only interested in the 
				// body of the method, not all the attributes.
				VisitBlock(memberMethod.Body);

				if ((_boxInstuctCount > _maxBoxInstruct) ||
				    (_unboxInstructCount > _maxUnBoxInstruct))
				{
					// Got an error.
					result.Add(new Problem(GetResolution(memberMethod.Name.Name)));
				}
			}

			return result;
		}

		/// <summary>Called for binary expressions in the IL.</summary>
		/// <param name="expression">The binary expression being evaluated. </param>
		public override void VisitExpression(Expression expression)
		{
			if (expression != null)
			{
				// NodeType contains the instruction I'm interested in 
				switch (expression.NodeType)
				{
					case NodeType.Box:
						_boxInstuctCount++;
						break;
					case NodeType.Unbox:
					case NodeType.UnboxAny:
						_unboxInstructCount++;
						break;
				}
			}
		}
	}
}