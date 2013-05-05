using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Testing.UnitTests.Types;


namespace SharedAssemblies.General.Testing.UnitTests
{
	/// <summary>
	/// ReflectorTests test fixture
	/// </summary>
	[TestClass]
	public class ReflectorTests
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }


		/// <summary>
		/// Test that we can retrieve all permutations of class fields from
		/// a supplied generic type.
		/// </summary>
		[TestMethod]
		public void Refelctor_GetField_Generic()
		{
			Assert.AreEqual(Reflector.GetField<SimplePublicType>("InternalStaticField"),
				"InternalStaticField");
			Assert.AreEqual(Reflector.GetField<SimplePublicType>("InternalConstField"),
				"InternalConstField");
			Assert.AreEqual(Reflector.GetField<SimplePublicType>("PublicConstField"),
				"PublicConstField");
			Assert.AreEqual(Reflector.GetField<SimplePublicType>("PrivateConstField"),
				"PrivateConstField");
			Assert.AreEqual(Reflector.GetField<SimplePublicType>("PrivateStaticField"),
				"PrivateStaticField");
		}

		/// <summary>
		/// Test that we can retrieve all permutations of class fields from
		/// a supplied type instance.
		/// </summary>
		[TestMethod]
		public void Refelctor_GetField_Instance()
		{
			var instance = new SimplePublicType();

			Assert.AreEqual(Reflector.GetField(instance, "InternalStaticField"),
				"InternalStaticField");
			Assert.AreEqual(Reflector.GetField(instance, "InternalConstField"),
				"InternalConstField");
			Assert.AreEqual(Reflector.GetField(instance, "PublicConstField"),
				"PublicConstField");
			Assert.AreEqual(Reflector.GetField(instance, "PrivateConstField"),
				"PrivateConstField");
			Assert.AreEqual(Reflector.GetField(instance, "PrivateStaticField"),
				"PrivateStaticField");
		}

		/// <summary>
		/// Test that we can retrieve all permutations of class fields from
		/// supplied type metadata.
		/// </summary>
		[TestMethod]
		public void Refelctor_GetField_Type()
		{
			var instance = typeof(SimplePublicType);

			Assert.AreEqual(Reflector.GetField(instance, "InternalStaticField"),
				"InternalStaticField");
			Assert.AreEqual(Reflector.GetField(instance, "InternalConstField"),
				"InternalConstField");
			Assert.AreEqual(Reflector.GetField(instance, "PublicConstField"),
				"PublicConstField");
			Assert.AreEqual(Reflector.GetField(instance, "PrivateConstField"),
				"PrivateConstField");
			Assert.AreEqual(Reflector.GetField(instance, "PrivateStaticField"),
				"PrivateStaticField");
		}


		/// <summary>
		/// This tests that we cannot use GetProperty to retrieve or fail to retrieve
		/// properties. Notice that we cannot retrieve properties from a non-instance
		/// as they have not been populated yet.
		/// </summary>
		[TestMethod]
		public void Refelctor_GetProperty_Type()
		{
			var instance = typeof(SimplePublicType);

			AssertEx.Throws(() =>
				Reflector.GetProperty(instance, "PublicField"));
			AssertEx.Throws(() =>
				Reflector.GetProperty(instance, "PrivateField"));
			AssertEx.Throws(() =>
				Reflector.GetProperty(instance, "PopulatedPublicField"));
			AssertEx.Throws(() =>
				Reflector.GetProperty(instance, "PopulatedPrivateField"));
			Assert.AreEqual(Reflector.GetProperty(instance, "StaticPopulatedPublicField"),
				"StaticPopulatedPublicField");
			Assert.AreEqual(Reflector.GetProperty(instance, "StaticPopulatedPrivateField"),
				"StaticPopulatedPrivateField");
		}

		/// <summary>
		/// Test that we can retrieve all permutations of class properties from
		/// a supplied type instance.
		/// </summary>
		[TestMethod]
		public void Refelctor_GetProperty_Instance()
		{
			var instance = new SimplePublicType();

			Assert.AreEqual(Reflector.GetProperty(instance, "PublicField"),
				"PublicField");
			Assert.AreEqual(Reflector.GetProperty(instance, "PrivateField"),
				"PrivateField");
			Assert.AreEqual(Reflector.GetProperty(instance, "PopulatedPublicField"),
				"PopulatedPublicField");
			Assert.AreEqual(Reflector.GetProperty(instance, "PopulatedPrivateField"),
				"PopulatedPrivateField");
			Assert.AreEqual(Reflector.GetProperty(instance, "StaticPopulatedPublicField"),
				"StaticPopulatedPublicField");
			Assert.AreEqual(Reflector.GetProperty(instance, "StaticPopulatedPrivateField"),
				"StaticPopulatedPrivateField");
		}

		/// <summary>
		/// Test that we can retrieve all permutations of class properties from
		/// a supplied generic type.
		/// </summary>
		[TestMethod]
		public void Refelctor_GetProperty_Generic()
		{
			Assert.AreEqual(Reflector.GetProperty<SimplePublicType>("PublicField"),
				"PublicField");
			Assert.AreEqual(Reflector.GetProperty<SimplePublicType>("PrivateField"),
				"PrivateField");
			Assert.AreEqual(Reflector.GetProperty<SimplePublicType>("PopulatedPublicField"),
				"PopulatedPublicField");
			Assert.AreEqual(Reflector.GetProperty<SimplePublicType>("PopulatedPrivateField"),
				"PopulatedPrivateField");
			Assert.AreEqual(Reflector.GetProperty<SimplePublicType>("StaticPopulatedPublicField"),
				"StaticPopulatedPublicField");
			Assert.AreEqual(Reflector.GetProperty<SimplePublicType>("StaticPopulatedPrivateField"),
				"StaticPopulatedPrivateField");
		}


		/// <summary>
		/// Test that we can set most permutations of class fields from
		/// a supplied type instance.
		/// </summary>
		/// <remarks>The set of static fields actually change the value, however, that breaks the
		/// other tests. So we just show that if the method doesn't throw, it succeeded.</remarks>
		[TestMethod]
		public void Refelctor_SetField_Instance()
		{
			var instance = new SimplePublicType();

			Reflector.SetField(instance, "InternalStaticField", "InternalStaticField");
			Reflector.SetField(instance, "PrivateStaticField", "PrivateStaticField");

			AssertEx.Throws(() => 
				Reflector.SetField(instance, "InternalConstField", "2InternalConstField"));
			AssertEx.Throws(() => 
				Reflector.SetField(instance, "PublicConstField", "2PublicConstField"));
			AssertEx.Throws(() => 
				Reflector.SetField(instance, "PrivateConstField", "2PrivateConstField"));

			Assert.AreEqual(Reflector.GetField(instance, "InternalStaticField"),
				"InternalStaticField");
			Assert.AreEqual(Reflector.GetField(instance, "PrivateStaticField"),
				"PrivateStaticField");
			Assert.AreEqual(Reflector.GetField(instance, "InternalConstField"),
				"InternalConstField");
			Assert.AreEqual(Reflector.GetField(instance, "PublicConstField"),
				"PublicConstField");
			Assert.AreEqual(Reflector.GetField(instance, "PrivateConstField"),
				"PrivateConstField");
		}

		/// <summary>
		/// Test that we can set most permutations of class properties from
		/// a supplied type instance.
		/// </summary>
		[TestMethod]
		public void Refelctor_SetProperty_Instance()
		{
			var instance = new SimplePublicType();

			Reflector.SetProperty(instance, "PublicField", "PublicField");
			Reflector.SetProperty(instance, "PrivateField", "PrivateField");
			AssertEx.Throws(() =>
				Reflector.SetProperty(instance, "PopulatedPublicField", "PopulatedPublicField"));
			AssertEx.Throws(() =>
				Reflector.SetProperty(instance, "PopulatedPrivateField", "PopulatedPrivateField"));
			AssertEx.Throws(() =>
				Reflector.SetProperty(instance, "StaticPopulatedPublicField", "StaticPopulatedPublicField"));
			AssertEx.Throws(() =>
				Reflector.SetProperty(instance, "StaticPopulatedPrivateField", 
					"StaticPopulatedPrivateField"));

			Assert.AreEqual(Reflector.GetProperty(instance, "PublicField"),
				"PublicField");
			Assert.AreEqual(Reflector.GetProperty(instance, "PrivateField"),
				"PrivateField");
			Assert.AreEqual(Reflector.GetProperty(instance, "PopulatedPublicField"),
				"PopulatedPublicField");
			Assert.AreEqual(Reflector.GetProperty(instance, "PopulatedPrivateField"),
				"PopulatedPrivateField");
			Assert.AreEqual(Reflector.GetProperty(instance, "StaticPopulatedPublicField"),
				"StaticPopulatedPublicField");
			Assert.AreEqual(Reflector.GetProperty(instance, "StaticPopulatedPrivateField"),
				"StaticPopulatedPrivateField");
		}

		/// <summary>
		/// Test that we can call every permutation of a method from 
		/// a static type.
		/// </summary>
		[TestMethod]
		public void Refelctor_CallMethods_Static()
		{	
			var input = new object[] { "test input data" };
			Assert.IsNull(Reflector.ExecuteMethod<SimplePublicType>("StaticPublicVoidMethod", null));
			Assert.IsNull(Reflector.ExecuteMethod<SimplePublicType>("StaticPrivateVoidMethod", null));
			Assert.IsNull(Reflector.ExecuteMethod<SimplePublicType>("StaticInternalVoidMethod", null));

			Assert.IsNull(Reflector.ExecuteMethod<SimplePublicType>("StaticPublicVoidMethod", input));
			Assert.IsNull(Reflector.ExecuteMethod<SimplePublicType>("StaticPrivateVoidMethod", input));
			Assert.IsNull(Reflector.ExecuteMethod<SimplePublicType>("StaticInternalVoidMethod", input));

			Assert.AreEqual("StaticPublicStringMethod", 
				Reflector.ExecuteMethod<SimplePublicType>("StaticPublicStringMethod", null));
			Assert.AreEqual("StaticPrivateStringMethod", 
				Reflector.ExecuteMethod<SimplePublicType>("StaticPrivateStringMethod", null));
			Assert.AreEqual("StaticInternalStringMethod", 
				Reflector.ExecuteMethod<SimplePublicType>("StaticInternalStringMethod", null));

			Assert.AreEqual("StaticPublicStringMethod", 
				Reflector.ExecuteMethod<SimplePublicType>("StaticPublicStringMethod", input));
			Assert.AreEqual("StaticPrivateStringMethod", 
				Reflector.ExecuteMethod<SimplePublicType>("StaticPrivateStringMethod", input));
			Assert.AreEqual("StaticInternalStringMethod", 
				Reflector.ExecuteMethod<SimplePublicType>("StaticInternalStringMethod", input));
		}

		/// <summary>
		/// Test that we can call every permutation of a method from 
		/// a static type.
		/// </summary>
		[TestMethod]
		public void Refelctor_CallMethods_Instance()
		{
			var instance = new SimplePublicType();
			var input = new object[] { "test input data" };

			Assert.IsNull(Reflector.ExecuteMethod(instance, "InstancePublicVoidMethod", null));
			Assert.IsNull(Reflector.ExecuteMethod(instance, "InstancePrivateVoidMethod", null));
			Assert.IsNull(Reflector.ExecuteMethod(instance, "InstanceInternalVoidMethod", null));

			Assert.IsNull(Reflector.ExecuteMethod(instance, "InstancePublicVoidMethod", input));
			Assert.IsNull(Reflector.ExecuteMethod(instance, "InstancePrivateVoidMethod", input));
			Assert.IsNull(Reflector.ExecuteMethod(instance, "InstanceInternalVoidMethod", input));

			Assert.AreEqual("InstancePublicStringMethod", 
				Reflector.ExecuteMethod(instance, "InstancePublicStringMethod", null));
			Assert.AreEqual("InstancePrivateStringMethod", 
				Reflector.ExecuteMethod(instance, "InstancePrivateStringMethod", null));
			Assert.AreEqual("InstanceInternalStringMethod", 
				Reflector.ExecuteMethod(instance, "InstanceInternalStringMethod", null));

			Assert.AreEqual("InstancePublicStringMethod", 
				Reflector.ExecuteMethod(instance, "InstancePublicStringMethod", input));
			Assert.AreEqual("InstancePrivateStringMethod", 
				Reflector.ExecuteMethod(instance, "InstancePrivateStringMethod", input));
			Assert.AreEqual("InstanceInternalStringMethod", 
				Reflector.ExecuteMethod(instance, "InstanceInternalStringMethod", input));
		}

		/// <summary>
		/// Test that we can call every permutation of a method from 
		/// a static type.
		/// </summary>
		[TestMethod]
		public void Refelctor_CallMethods_Overloaded()
		{
			var instance = new SimplePublicType();
			
			Assert.AreEqual(0, Reflector.ExecuteMethod(instance, "ParameterMethod", null));
			Assert.AreEqual(1, Reflector.ExecuteMethod(instance, "ParameterMethod", 
				new object[] { "1" }));
			Assert.AreEqual(2, Reflector.ExecuteMethod(instance, "ParameterMethod", 
				new object[] { "1", "2" }));
			Assert.AreEqual(3, Reflector.ExecuteMethod(instance, "ParameterMethod", 
				new object[] { "1", "2", "3" }));
			Assert.AreEqual(4, Reflector.ExecuteMethod(instance, "ParameterMethod", 
				new object[] { "1", "2", "3", "4" }));
		}

		/// <summary>
		/// Test that we can call internal methods from another assembly.
		/// </summary>
		[TestMethod]
		public void Refelctor_CallMethods_Internal()
		{
			var type = "System.ThrowHelper";
			var method = "ThrowKeyNotFoundException";

			// because reflection will wrap the inner exception
			AssertEx.Throws<TargetInvocationException>(() =>
				Reflector.ExecuteMethod(method, null, typeof(System.DateTime), type));
		}
	}
}