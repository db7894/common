using System.Diagnostics.CodeAnalysis;

namespace SharedAssemblies.General.Testing.UnitTests.Types
{
#pragma warning disable 0414 // since we don't use the defined constants here
	/// <summary>
	/// An example of a public type that can be reflected upon
	/// </summary>
	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.NamingAnalyzer",
		"ST3001:NonPublicFieldsMustStartWithUnderscore",
		Justification = "Purely for relfection unit test.")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
		Justification = "Purely for relfection unit test.")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess",
		Justification = "Purely for relfection unit test.")]
	public class SimplePublicType
	{
		#region Fields and Properties

		/// <summary>Just a simple test field that's public constant</summary>
		public const string PublicConstField = "PublicConstField";

		/// <summary>Just a simple test field that's internal static</summary>
		internal static readonly string InternalStaticField = "InternalStaticField";

		/// <summary>Just a simple test field that's internal constant</summary>
		internal const string InternalConstField = "InternalConstField";

		/// <summary>Just a simple test field that's private constant</summary>
		private const string PrivateConstField = "PrivateConstField";

		/// <summary>Just a simple test field that's private constant</summary>
		private static readonly string PrivateStaticField = "PrivateStaticField";

		/// <summary>Just a simple test field that's public property</summary>        
		public string PublicField { get; set; }

		/// <summary>Just a simple test field that's public populated</summary>
		public string PopulatedPublicField
		{
			get { return "PopulatedPublicField"; }
		}

		/// <summary>Just a simple test field that's private property</summary>
		private string PrivateField { get; set; }

		/// <summary>Just a simple test field that's static populated public constant</summary>
		public static string StaticPopulatedPublicField
		{
			get { return "StaticPopulatedPublicField"; }
		}

		/// <summary>Just a simple test field that's private populated</summary>
		private string PopulatedPrivateField
		{
			get { return "PopulatedPrivateField"; }
		}

		/// <summary>Just a simple test field that's static populated private constant</summary>
		private static string StaticPopulatedPrivateField
		{
			get { return "StaticPopulatedPrivateField"; }
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the SimplePublicType
		/// </summary>
		public SimplePublicType()
		{
			PublicField = "PublicField";
			PrivateField = "PrivateField";
		}

		#region All Permutations of Test Methods

		#region Static

		#region Without Parameters

		/// <summary>
		/// Simple test method.
		/// </summary>
		public static void StaticPublicVoidMethod()
		{
			// nothing
		}

		/// <summary>
		/// Simple test method.
		/// </summary>
		/// <returns>Returns nothing at all.</returns>
		public static string StaticPublicStringMethod()
		{
			return "StaticPublicStringMethod";
		}

		private static void StaticPrivateVoidMethod()
		{
			// nothing
		}

		private static string StaticPrivateStringMethod()
		{
			return "StaticPrivateStringMethod";
		}

		private static void StaticInternalVoidMethod()
		{
			// nothing
		}

		private static string StaticInternalStringMethod()
		{
			return "StaticInternalStringMethod";
		}

		#endregion

		#region With Parameters

		public static void StaticPublicVoidMethod(string input)
		{
			// nothing
		}

		public static string StaticPublicStringMethod(string input)
		{
			return "StaticPublicStringMethod";
		}

		private static void StaticPrivateVoidMethod(string input)
		{
			// nothing
		}

		private static string StaticPrivateStringMethod(string input)
		{
			return "StaticPrivateStringMethod";
		}

		private static void StaticInternalVoidMethod(string input)
		{
			// nothing
		}

		private static string StaticInternalStringMethod(string input)
		{
			return "StaticInternalStringMethod";
		}

		#endregion

		#endregion

		#region Instance

		#region Without Parameters

		public void InstancePublicVoidMethod()
		{
			// nothing
		}

		public string InstancePublicStringMethod()
		{
			return "InstancePublicStringMethod";
		}

		private void InstancePrivateVoidMethod()
		{
			// nothing
		}

		private string InstancePrivateStringMethod()
		{
			return "InstancePrivateStringMethod";
		}

		internal void InstanceInternalVoidMethod()
		{
			// nothing
		}

		internal string InstanceInternalStringMethod()
		{
			return "InstanceInternalStringMethod";
		}

		#endregion

		#region With Parameters

		public void InstancePublicVoidMethod(string input)
		{
			// nothing
		}

		public string InstancePublicStringMethod(string input)
		{
			return "InstancePublicStringMethod";
		}

		private void InstancePrivateVoidMethod(string input)
		{
			// nothing
		}

		private string InstancePrivateStringMethod(string input)
		{
			return "InstancePrivateStringMethod";
		}

		internal void InstanceInternalVoidMethod(string input)
		{
			// nothing
		}

		internal string InstanceInternalStringMethod(string input)
		{
			return "InstanceInternalStringMethod";
		}

		#endregion

		#endregion

		#endregion

		#region Number of Parameters Tests

		public static int ParameterMethod()
		{
			return 0;
		}

		public static int ParameterMethod(string a)
		{
			return 1;
		}

		public static int ParameterMethod(string a, string b)
		{
			return 2;
		}

		public static int ParameterMethod(string a, string b, string c)
		{
			return 3;
		}

		public static int ParameterMethod(string a, string b, string c, string d)
		{
			return 4;
		}

		#endregion
	}
#pragma warning restore 0414
}