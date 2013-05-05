using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.UnitTests.TestClasses;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// Object extensions unit test fixture
	/// </summary>
	[TestClass]
	public class ObjectExtensionsTest
	{
		/// <summary>
		/// The ms test context
		/// </summary>
		public TestContext TestContext { get; set; }


		/// <summary>
		/// Testing IsDefault()
		/// </summary>
		[TestMethod]
		public void IsDefault_ReturnsTrue_IfNullOnReferenceTypes()
		{
			string s = null;
			Assert.IsTrue(s.IsDefault());
		}


		/// <summary>
		/// Testing IsDefault()
		/// </summary>
		[TestMethod]
		public void IsDefault_ReturnsFalse_IfNotNullOnReferenceTypes()
		{
			string s = "Hiya";
			Assert.IsFalse(s.IsDefault());
		}


		/// <summary>
		/// Testing IsDefault()
		/// </summary>
		[TestMethod]
		public void IsDefault_ReturnsTrue_IfNullOnNullableTypes()
		{
			int? s = null;
			Assert.IsTrue(s.IsDefault());
		}


		/// <summary>
		/// Testing IsDefault()
		/// </summary>
		[TestMethod]
		public void IsDefault_ReturnsFalse_IfNotNullOnNullableTypes()
		{
			int? s = 0;
			Assert.IsFalse(s.IsDefault());
		}


		/// <summary>
		/// Testing IsDefault()
		/// </summary>
		[TestMethod]
		public void IsDefault_ReturnsTrue_IfZeroOnPrimitiveTypes()
		{
			int s = 0;
			Assert.IsTrue(s.IsDefault());
		}


		/// <summary>
		/// Testing IsDefault()
		/// </summary>
		[TestMethod]
		public void IsDefault_ReturnsFalse_IfNotZeroOnPrimitiveTypes()
		{
			int s = -42;
			Assert.IsFalse(s.IsDefault());
		}


		/// <summary>
		/// Testing IsDefault()
		/// </summary>
		[TestMethod]
		public void IsDefault_ReturnsTrue_IfDefaultOnStruct()
		{
			TimeSpan s = new TimeSpan();
			Assert.IsTrue(s.IsDefault());
		}


		/// <summary>
		/// Testing IsDefault()
		/// </summary>
		[TestMethod]
		public void IsDefault_ReturnsFalse_IfNotDefaultOnStruct()
		{
			TimeSpan s = new TimeSpan(7, 30, 0);
			Assert.IsFalse(s.IsDefault());
		}


		/// <summary>
		/// Testing IsNullOrDbNull() which checks if value null or DBNull.Value
		/// </summary>
		[TestMethod]
		public void IsNullOrDbNull_ReturnsTrue_OnNullReference()
		{
			string actual = null;
			Assert.IsTrue(actual.IsNullOrDbNull());
		}


		/// <summary>
		/// Testing IsNullOrDbNull() which checks if value null or DBNull.Value
		/// </summary>
		[TestMethod]
		public void IsNullOrDbNull_ReturnsTrue_OnDbNull()
		{
			object actual = DBNull.Value;
			Assert.IsTrue(actual.IsNullOrDbNull());
		}


		/// <summary>
		/// Testing IsNullOrDbNull() which checks if value null or DBNull.Value
		/// </summary>
		[TestMethod]
		public void IsNullOrDbNull_ReturnsFalse_OnNonNullReference()
		{
			string actual = "Hiya";
			Assert.IsFalse(actual.IsNullOrDbNull());
		}

		
		/// <summary>
		/// Testing IsNullOrDbNull() which checks if value null or DBNull.Value
		/// </summary>
		[TestMethod]
		public void IsNullOrDbNull_ReturnsFalse_OnPrimitiveRegardlessOfValue()
		{
			object boxedZero = 0;
			object boxedSeven = 7;

			Assert.IsFalse(7.IsNullOrDbNull());
			Assert.IsFalse((-1).IsNullOrDbNull());
			Assert.IsFalse(42.IsNullOrDbNull());
			Assert.IsFalse(0.IsNullOrDbNull());
			Assert.IsFalse(boxedSeven.IsNullOrDbNull());
			Assert.IsFalse(boxedZero.IsNullOrDbNull());
		}

		
		/// <summary>
		/// Testing IsNullOrDbNull() which checks if value null or DBNull.Value
		/// </summary>
		[TestMethod]
		public void IsNullOrDbNull_ReturnsTrue_OnNullNullablePrimatives()
		{
			int? actual = null;

			Assert.IsTrue(actual.IsNullOrDbNull());
		}

		
		/// <summary>
		/// Testing IsNullOrDbNull() which checks if value null or DBNull.Value
		/// </summary>
		[TestMethod]
		public void IsNullOrDbNull_ReturnsFalse_OnNonNullNullablePrimatives()
		{
			int? actualZero = 0;
			int? actualSeven = 7;

			Assert.IsFalse(actualZero.IsNullOrDbNull());
			Assert.IsFalse(actualSeven.IsNullOrDbNull());
		}

		
		/// <summary>
		/// Testing IsNullOrDbNull() which checks if value null or DBNull.Value
		/// </summary>
		[TestMethod]
		public void IsNullOrDbNull_ReturnsTrue_OnNullNullableStructures()
		{
			DateTime? actual = null;

			Assert.IsTrue(actual.IsNullOrDbNull());
			Assert.IsTrue(((object)actual).IsNullOrDbNull());
		}


		/// <summary>
		/// Testing IsNullOrDbNull() which checks if value null or DBNull.Value
		/// </summary>
		[TestMethod]
		public void IsNullOrDbNull_ReturnsFalse_OnNonNullNullableStructures()
		{
			DateTime? actual = DateTime.Now;

			Assert.IsFalse(actual.IsNullOrDbNull());
		}


		/// <summary>
		/// Testing ToType successes from string -> enum
		/// </summary>
		[TestMethod]
		public void ToType_CorrectlyConvertsEnum_FromString()
		{
			Assert.AreEqual(TestEnum.Zero, "Zero".ToType(TestEnum.One));
			Assert.AreEqual(TestEnum.One, "One".ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Two, "Two".ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Three, "Three".ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Four, "Four".ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Five, "Five".ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Six, "Six".ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Seven, "Seven".ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Eight, "Eight".ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Nine, "Nine".ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Ten, "Ten".ToType(TestEnum.Zero));
		}


		/// <summary>
		/// Testing ToType successes from string -> enum
		/// </summary>
		[TestMethod]
		public void ToType_CorrectlyConvertsEnum_FromStringWithDefaults()
		{
			Assert.AreEqual(TestEnum.Zero, "Zero".ToType<TestEnum>());
			Assert.AreEqual(TestEnum.One, "One".ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Two, "Two".ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Three, "Three".ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Four, "Four".ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Five, "Five".ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Six, "Six".ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Seven, "Seven".ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Eight, "Eight".ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Nine, "Nine".ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Ten, "Ten".ToType<TestEnum>());
		}


		/// <summary>
		/// Testing ToType successes from enum -> int
		/// </summary>
		[TestMethod]
		public void ToType_CorrectlyConverts_FromEnumToInt()
		{
			Assert.AreEqual(0, TestEnum.Zero.ToType(0));
			Assert.AreEqual(1, TestEnum.One.ToType(0));
			Assert.AreEqual(2, TestEnum.Two.ToType(0));
			Assert.AreEqual(3, TestEnum.Three.ToType(0));
			Assert.AreEqual(4, TestEnum.Four.ToType(0));
			Assert.AreEqual(5, TestEnum.Five.ToType(0));
			Assert.AreEqual(6, TestEnum.Six.ToType(0));
			Assert.AreEqual(7, TestEnum.Seven.ToType(0));
			Assert.AreEqual(8, TestEnum.Eight.ToType(0));
			Assert.AreEqual(9, TestEnum.Nine.ToType(0));
			Assert.AreEqual(10, TestEnum.Ten.ToType(0));
		}


		/// <summary>
		/// Testing ToType successes from enum -> int
		/// </summary>
		[TestMethod]
		public void ToType_CorrectlyConverts_FromEnumToIntWithoutDefault()
		{
			Assert.AreEqual(0, TestEnum.Zero.ToType<int>());
			Assert.AreEqual(1, TestEnum.One.ToType<int>());
			Assert.AreEqual(2, TestEnum.Two.ToType<int>());
			Assert.AreEqual(3, TestEnum.Three.ToType<int>());
			Assert.AreEqual(4, TestEnum.Four.ToType<int>());
			Assert.AreEqual(5, TestEnum.Five.ToType<int>());
			Assert.AreEqual(6, TestEnum.Six.ToType<int>());
			Assert.AreEqual(7, TestEnum.Seven.ToType<int>());
			Assert.AreEqual(8, TestEnum.Eight.ToType<int>());
			Assert.AreEqual(9, TestEnum.Nine.ToType<int>());
			Assert.AreEqual(10, TestEnum.Ten.ToType<int>());
		}


		/// <summary>
		/// Testing ToType successes from enum -> int
		/// </summary>
		[TestMethod]
		public void ToType_CorrectlyConverts_FromEnumToString()
		{
			Assert.AreEqual("Zero", TestEnum.Zero.ToType(string.Empty));
			Assert.AreEqual("One", TestEnum.One.ToType(string.Empty));
			Assert.AreEqual("Two", TestEnum.Two.ToType(string.Empty));
			Assert.AreEqual("Three", TestEnum.Three.ToType(string.Empty));
			Assert.AreEqual("Four", TestEnum.Four.ToType(string.Empty));
			Assert.AreEqual("Five", TestEnum.Five.ToType(string.Empty));
			Assert.AreEqual("Six", TestEnum.Six.ToType(string.Empty));
			Assert.AreEqual("Seven", TestEnum.Seven.ToType(string.Empty));
			Assert.AreEqual("Eight", TestEnum.Eight.ToType(string.Empty));
			Assert.AreEqual("Nine", TestEnum.Nine.ToType(string.Empty));
			Assert.AreEqual("Ten", TestEnum.Ten.ToType(string.Empty));
		}


		/// <summary>
		/// Testing ToType successes from enum -> int
		/// </summary>
		[TestMethod]
		public void ToType_CorrectlyConverts_FromEnumToStringWithoutDefault()
		{
			Assert.AreEqual("Zero", TestEnum.Zero.ToType<string>());
			Assert.AreEqual("One", TestEnum.One.ToType<string>());
			Assert.AreEqual("Two", TestEnum.Two.ToType<string>());
			Assert.AreEqual("Three", TestEnum.Three.ToType<string>());
			Assert.AreEqual("Four", TestEnum.Four.ToType<string>());
			Assert.AreEqual("Five", TestEnum.Five.ToType<string>());
			Assert.AreEqual("Six", TestEnum.Six.ToType<string>());
			Assert.AreEqual("Seven", TestEnum.Seven.ToType<string>());
			Assert.AreEqual("Eight", TestEnum.Eight.ToType<string>());
			Assert.AreEqual("Nine", TestEnum.Nine.ToType<string>());
			Assert.AreEqual("Ten", TestEnum.Ten.ToType<string>());
		}


		/// <summary>
		/// Testing ToType successes from enum -> double
		/// </summary>
		[TestMethod]
		public void ToType_CorrectlyConverts_FromEnumToDouble()
		{
			Assert.AreEqual(0.0, TestEnum.Zero.ToType(0.0));
			Assert.AreEqual(1.0, TestEnum.One.ToType(0.0));
			Assert.AreEqual(2.0, TestEnum.Two.ToType(0.0));
			Assert.AreEqual(3.0, TestEnum.Three.ToType(0.0));
			Assert.AreEqual(4.0, TestEnum.Four.ToType(0.0));
			Assert.AreEqual(5.0, TestEnum.Five.ToType(0.0));
			Assert.AreEqual(6.0, TestEnum.Six.ToType(0.0));
			Assert.AreEqual(7.0, TestEnum.Seven.ToType(0.0));
			Assert.AreEqual(8.0, TestEnum.Eight.ToType(0.0));
			Assert.AreEqual(9.0, TestEnum.Nine.ToType(0.0));
			Assert.AreEqual(10.0, TestEnum.Ten.ToType(0.0));
		}


		/// <summary>
		/// Testing ToType successes from enum -> double
		/// </summary>
		[TestMethod]
		public void ToType_CorrectlyConverts_FromEnumToDoubleWithoutDefault()
		{
			Assert.AreEqual(0.0, TestEnum.Zero.ToType<double>());
			Assert.AreEqual(1.0, TestEnum.One.ToType<double>());
			Assert.AreEqual(2.0, TestEnum.Two.ToType<double>());
			Assert.AreEqual(3.0, TestEnum.Three.ToType<double>());
			Assert.AreEqual(4.0, TestEnum.Four.ToType<double>());
			Assert.AreEqual(5.0, TestEnum.Five.ToType<double>());
			Assert.AreEqual(6.0, TestEnum.Six.ToType<double>());
			Assert.AreEqual(7.0, TestEnum.Seven.ToType<double>());
			Assert.AreEqual(8.0, TestEnum.Eight.ToType<double>());
			Assert.AreEqual(9.0, TestEnum.Nine.ToType<double>());
			Assert.AreEqual(10.0, TestEnum.Ten.ToType<double>());
		}


		/// <summary>
		/// Testing ToType successes from number -> enum
		/// </summary>
		[TestMethod]
		public void ToType_CorrectlyConvertsEnum_FromNumeric()
		{
			const short shortSeven = 7;
			const ushort ushortEight = 8;
			const byte byteNine = 9;
			const double doubleTen = 10.2;

			Assert.AreEqual(TestEnum.Zero, 0.ToType(TestEnum.One));
			Assert.AreEqual(TestEnum.One, 1.ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Two, 2.ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Three, 3.ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Four, 4.ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Five, 5.ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Six, 6.ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Seven, shortSeven.ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Eight, ushortEight.ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Nine, byteNine.ToType(TestEnum.Zero));
			Assert.AreEqual(TestEnum.Ten, doubleTen.ToType(TestEnum.Zero));
		}


		/// <summary>
		/// Testing ToType successes from number -> enum
		/// </summary>
		[TestMethod]
		public void ToType_CorrectlyConvertsEnum_FromNumericWithoutDefault()
		{
			const short shortSeven = 7;
			const ushort ushortEight = 8;
			const byte byteNine = 9;
			const double doubleTen = 10.2;

			Assert.AreEqual(TestEnum.Zero, 0.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.One, 1.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Two, 2.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Three, 3.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Four, 4.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Five, 5.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Six, 6.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Seven, shortSeven.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Eight, ushortEight.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Nine, byteNine.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Ten, doubleTen.ToType<TestEnum>());
		}


		/// <summary>
		/// Testing ToType defaults for nulls
		/// </summary>
		[TestMethod]
		public void ToType_ReturnsDefault_IfNull()
		{
			const object actualNull = null;
			object actualDbNull = DBNull.Value;

			Assert.AreEqual(TestEnum.One, actualNull.ToType(TestEnum.One));
			Assert.AreEqual(TestEnum.Two, actualDbNull.ToType(TestEnum.Two));
			Assert.AreEqual(TestEnum.Zero, actualNull.ToType<TestEnum>());
			Assert.AreEqual(TestEnum.Zero, actualDbNull.ToType<TestEnum>());
			Assert.AreEqual(0, actualNull.ToType<int>());
			Assert.AreEqual(0, actualDbNull.ToType<int>());
			Assert.AreEqual(0, actualDbNull.ToType<double>());
			Assert.AreEqual(0, actualNull.ToType<double>());
			Assert.AreEqual(0, actualDbNull.ToType<short>());
			Assert.AreEqual(0, actualNull.ToType<short>());
			Assert.AreEqual(0, actualDbNull.ToType<char>());
			Assert.AreEqual(0, actualNull.ToType<char>());
			Assert.AreEqual(null, actualDbNull.ToType<string>());
			Assert.AreEqual(null, actualNull.ToType<string>());
			Assert.AreEqual(null, actualDbNull.ToType<int?>());
			Assert.AreEqual(null, actualNull.ToType<int?>());
		}


		/// <summary>
		/// Testing ToType defaults for nulls
		/// </summary>
		[TestMethod]
		public void ToType_Suceeds_IfIntOutOfRange()
		{
			Assert.AreEqual((TestEnum)13, 13.ToType(TestEnum.One));
		}


		/// <summary>
		/// Testing ToType defaults for nulls
		/// </summary>
		[TestMethod]
		public void ToType_Suceeds_IfIntOutOfRangeAndWithoutDefault()
		{
			Assert.AreEqual((TestEnum)13, 13.ToType<TestEnum>());
		}


		/// <summary>
		/// Testing ToType defaults for nulls
		/// </summary>
		[TestMethod]
		public void TryToType_Defaults_IfIntOutOfRange()
		{
			Assert.AreEqual((TestEnum)13, 13.TryToType(TestEnum.One));
		}


		/// <summary>
		/// Testing ToType defaults for nulls
		/// </summary>
		[TestMethod]
		public void TryToType_Defaults_IfIntOutOfRangeWithoutDefault()
		{
			Assert.AreEqual((TestEnum)13, 13.TryToType<TestEnum>());
		}


		/// <summary>
		/// Testing ToType defaults for nulls
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ToType_Throws_IfStringOutOfRange()
		{
			string s = "Twenty"; 
			s.ToType(TestEnum.One);
		}


		/// <summary>
		/// Testing ToType defaults for nulls
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ToType_Throws_IfStringOutOfRangeWithoutDefault()
		{
			string s = "Twenty"; 
			s.ToType<TestEnum>();
		}


		/// <summary>
		/// Testing ToType defaults for nulls
		/// </summary>
		[TestMethod]
		public void TryToType_Defaults_IfStringOutOfRange()
		{
			string s = "Twenty"; 
			Assert.AreEqual(TestEnum.One, s.TryToType(TestEnum.One));
		}


		/// <summary>
		/// Testing ToType defaults for nulls
		/// </summary>
		[TestMethod]
		public void TryToType_Defaults_IfStringOutOfRangeWithoutDefault()
		{
			string s = "Twenty"; 
			Assert.AreEqual(TestEnum.Zero, s.TryToType<TestEnum>());
		}


		/// <summary>
		/// Testing ToNullableType successes from string -> enum
		/// </summary>
		[TestMethod]
		public void ToNullableType_CorrectlyConvertsEnum_FromString()
		{
			Assert.AreEqual(TestEnum.Zero, "Zero".ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.One, "One".ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Two, "Two".ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Three, "Three".ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Four, "Four".ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Five, "Five".ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Six, "Six".ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Seven, "Seven".ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Eight, "Eight".ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Nine, "Nine".ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Ten, "Ten".ToNullableType<TestEnum>());
		}


		/// <summary>
		/// Testing ToNullableType successes from number -> enum
		/// </summary>
		[TestMethod]
		public void ToNullableType_CorrectlyConvertsEnum_FromNumeric()
		{
			const short shortSeven = 7;
			const ushort ushortEight = 8;
			const byte byteNine = 9;
			const double doubleTen = 10.2;

			Assert.AreEqual(TestEnum.Zero, 0.ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.One, 1.ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Two, 2.ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Three, 3.ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Four, 4.ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Five, 5.ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Six, 6.ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Seven, shortSeven.ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Eight, ushortEight.ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Nine, byteNine.ToNullableType<TestEnum>());
			Assert.AreEqual(TestEnum.Ten, doubleTen.ToNullableType<TestEnum>());
		}


		/// <summary>
		/// Testing ToNullableType defaults for nulls
		/// </summary>
		[TestMethod]
		public void ToNullableType_ReturnsDefault_IfNull()
		{
			const object actualNull = null;
			object actualDbNull = DBNull.Value;

			Assert.AreEqual(null, actualNull.ToNullableType<TestEnum>());
			Assert.AreEqual(null, actualDbNull.ToNullableType<TestEnum>());
		}


		/// <summary>
		/// Testing ToNullableType defaults for nulls
		/// </summary>
		[TestMethod]
		public void ToNullableType_Throws_IfIntOutOfRange()
		{
			Assert.AreEqual((TestEnum)13, 13.ToNullableType<TestEnum>());
		}


		/// <summary>
		/// Testing ToNullableType defaults for nulls
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ToNullableType_Throws_IfStringOutOfRange()
		{
			string s = "Twenty"; 
			s.ToNullableType<TestEnum>();
		}


		/// <summary>
		/// ToType throws if types aren't same
		/// </summary>
		[TestMethod]
		public void ToType_Converts_IfIConvertible()
		{
			short actual = 7;

			Assert.IsInstanceOfType(actual.ToType<int>(0), typeof(int));
			Assert.AreEqual(7, actual.ToType<int>(0));
		}


		/// <summary>
		/// ToType throws if types aren't same
		/// </summary>
		[TestMethod]
		public void ToType_Converts_IfIConvertibleWithoutDefault()
		{
			short actual = 7;

			Assert.IsInstanceOfType(actual.ToType<int>(), typeof(int));
			Assert.AreEqual(7, actual.ToType<int>());
		}


		/// <summary>
		/// ToType succeeds if types are same
		/// </summary>
		[TestMethod]
		public void ToType_Suceeds_IfGoodCast()
		{
			short actualShort = 7;
			string actualString = "Hiya";
			int actualInt = 9;
			DateTime actualDateTime = DateTime.Now;
			TestEnum actualEnum = TestEnum.Three;
			object actual;

			actual = actualShort;
			Assert.AreEqual(actualShort, actual.ToType<short>(0));

			actual = actualString;
			Assert.AreEqual(actualString, actual.ToType<string>(string.Empty));

			actual = actualInt;
			Assert.AreEqual(actualInt, actual.ToType<int>(0));

			actual = actualDateTime;
			Assert.AreEqual(actualDateTime, actual.ToType<DateTime>(new DateTime()));

			actual = actualEnum;
			Assert.AreEqual(actualEnum, actual.ToType<TestEnum>(TestEnum.Zero));
		}


		/// <summary>
		/// ToType succeeds if types are same
		/// </summary>
		[TestMethod]
		public void ToType_Suceeds_IfGoodCastWithoutDefault()
		{
			short actualShort = 7;
			string actualString = "Hiya";
			int actualInt = 9;
			DateTime actualDateTime = DateTime.Now;
			TestEnum actualEnum = TestEnum.Three;
			object actual;

			actual = actualShort;
			Assert.AreEqual(actualShort, actual.ToType<short>());

			actual = actualString;
			Assert.AreEqual(actualString, actual.ToType<string>());

			actual = actualInt;
			Assert.AreEqual(actualInt, actual.ToType<int>());

			actual = actualDateTime;
			Assert.AreEqual(actualDateTime, actual.ToType<DateTime>());

			actual = actualEnum;
			Assert.AreEqual(actualEnum, actual.ToType<TestEnum>());
		}


		/// <summary>
		/// ToNullableType succeeds if types are same
		/// </summary>
		[TestMethod]
		public void ToNullableType_Suceeds_IfGoodCast()
		{
			short actualShort = 7;
			int actualInt = 9;
			DateTime actualDateTime = DateTime.Now;
			TestEnum actualEnum = TestEnum.Three;
			object actual;

			actual = actualShort;
			Assert.AreEqual(actualShort, actual.ToNullableType<short>());

			actual = actualInt;
			Assert.AreEqual(actualInt, actual.ToNullableType<int>());

			actual = actualDateTime;
			Assert.AreEqual(actualDateTime, actual.ToNullableType<DateTime>());

			actual = actualEnum;
			Assert.AreEqual(actualEnum, actual.ToNullableType<TestEnum>());
		}


		/// <summary>
		/// ToType defaults on dbnull or null
		/// </summary>
		[TestMethod]
		public void ToType_Defaults_IfNullOrDbNull()
		{
			object actual = null;
			Assert.AreEqual(TestEnum.Seven, actual.ToType<TestEnum>(TestEnum.Seven));
			Assert.AreEqual(TestEnum.Zero, actual.ToType<TestEnum>());
			Assert.AreEqual(1, actual.ToType<int>(1));
			Assert.AreEqual(0, actual.ToType<int>());
			Assert.AreEqual(null, actual.ToType<string>());
			Assert.AreEqual(string.Empty, actual.ToType<string>(string.Empty));
				
			actual = DBNull.Value;
			Assert.AreEqual(TestEnum.Eight, actual.ToType<TestEnum>(TestEnum.Eight));
			Assert.AreEqual(TestEnum.Zero, actual.ToType<TestEnum>());
			Assert.AreEqual(1, actual.ToType<int>(1));
			Assert.AreEqual(0, actual.ToType<int>());
			Assert.AreEqual(null, actual.ToType<string>());
			Assert.AreEqual(string.Empty, actual.ToType<string>(string.Empty));
		}


		/// <summary>
		/// ToType suceeds between IConvertibles
		/// </summary>
		[TestMethod]
		public void ToType_Succeeds_FromOneConvertibleToAnother()
		{
			int intThree = 3;
			double doubleThree = 3.0;
			string stringThree = "3";
			string stringThreeLiteral = "Three";
			TestEnum enumThree = TestEnum.Three;

			Assert.AreEqual(intThree, doubleThree.ToType<int>(0));
			Assert.AreEqual(intThree, stringThree.ToType<int>(0));
			Assert.AreEqual(intThree, intThree.ToType<int>(0));
			Assert.AreEqual(intThree, enumThree.ToType<int>(0));

			Assert.AreEqual(doubleThree, doubleThree.ToType<double>(0.0));
			Assert.AreEqual(doubleThree, stringThree.ToType<double>(0.0));
			Assert.AreEqual(doubleThree, intThree.ToType<double>(0.0));
			Assert.AreEqual(doubleThree, enumThree.ToType<double>(0.0));

			// all primatives -> string are the stringified version of the primative
			Assert.AreEqual(stringThree, doubleThree.ToType<string>(string.Empty));
			Assert.AreEqual(stringThree, stringThree.ToType<string>(string.Empty));
			Assert.AreEqual(stringThree, intThree.ToType<string>(string.Empty));

			// enum -> string will be the literal translation of the text of the enum
			Assert.AreEqual(stringThreeLiteral, enumThree.ToType<string>(string.Empty));
		}


		/// <summary>
		/// ToType suceeds between IConvertibles
		/// </summary>
		[TestMethod]
		public void ToType_Succeeds_FromOneConvertibleToAnotherWithoutDefaults()
		{
			int intThree = 3;
			double doubleThree = 3.0;
			string stringThree = "3";
			string stringThreeLiteral = "Three";
			TestEnum enumThree = TestEnum.Three;

			Assert.AreEqual(intThree, doubleThree.ToType<int>());
			Assert.AreEqual(intThree, stringThree.ToType<int>());
			Assert.AreEqual(intThree, intThree.ToType<int>());
			Assert.AreEqual(intThree, enumThree.ToType<int>());

			Assert.AreEqual(doubleThree, doubleThree.ToType<double>());
			Assert.AreEqual(doubleThree, stringThree.ToType<double>());
			Assert.AreEqual(doubleThree, intThree.ToType<double>());
			Assert.AreEqual(doubleThree, enumThree.ToType<double>());

			// all primatives -> string are the stringified version of the primative
			Assert.AreEqual(stringThree, doubleThree.ToType<string>());
			Assert.AreEqual(stringThree, stringThree.ToType<string>());
			Assert.AreEqual(stringThree, intThree.ToType<string>());

			// enum -> string will be the literal translation of the text of the enum
			Assert.AreEqual(stringThreeLiteral, enumThree.ToType<string>());
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ToType_Throws_IfNotIConvertibleFromType()
		{
			InstantTime time = new InstantTime();

			time.ToType<double>(0.0);
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ToType_Throws_IfNotIConvertibleFromTypeWithoutDefault()
		{
			InstantTime time = new InstantTime();

			time.ToType<double>();
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void TryToType_Throws_IfNotIConvertibleFromType()
		{
			InstantTime time = new InstantTime();

			time.TryToType<double>(0.0);
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void TryToType_Throws_IfNotIConvertibleFromTypeWithoutDefault()
		{
			InstantTime time = new InstantTime();

			time.TryToType<double>();
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void TryToType_Throws_IfNotIConvertibleToType()
		{
			InstantTime time = new InstantTime();

			double d = 3.14;
			d.TryToType<InstantTime>(time);
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void TryToType_Throws_IfNotIConvertibleToTypeWithoutDefault()
		{
			InstantTime time = new InstantTime();

			double d = 3.14;
			d.TryToType<InstantTime>();
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ToType_Throws_IfNotIConvertibleToType()
		{
			InstantTime time = new InstantTime();

			double d = 3.14;
			d.ToType<InstantTime>(time);
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ToType_Throws_IfNotIConvertibleToTypeWithoutDefault()
		{
			InstantTime time = new InstantTime();

			double d = 3.14;
			d.ToType<InstantTime>();
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		public void ToType_Succeeds_ToSelfTypeEvenIfNotIConvertible()
		{
			InstantTime time = new InstantTime();

			time.ToType<InstantTime>(time);
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		public void ToType_Succeeds_ToSelfTypeEvenIfNotIConvertibleWithoutDefault()
		{
			InstantTime time = new InstantTime();

			time.ToType<InstantTime>();
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		public void ToType_Suceeds_IfCastingToSuperTypeAndNotIConvertible()
		{
			InstantTime time = new SubInstantTime();

			Assert.IsInstanceOfType(time.ToType<InstantTime>(time), typeof(InstantTime));
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		public void ToType_Suceeds_IfCastingToSuperTypeAndNotIConvertibleWithoutDefault()
		{
			InstantTime time = new SubInstantTime();

			Assert.IsInstanceOfType(time.ToType<InstantTime>(), typeof(InstantTime));
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		public void ToType_Suceeds_IfCastingToSubTypeIfActualType()
		{
			SubInstantTime time = new SubInstantTime();

			time.ToType<SubInstantTime>(time);
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		public void ToType_Suceeds_IfCastingToSubTypeIfActualTypeWithoutDefault()
		{
			SubInstantTime time = new SubInstantTime();

			time.ToType<SubInstantTime>();
		}


		/// <summary>
		/// ToNullableType defaults on dbnull or null
		/// </summary>
		[TestMethod]
		public void ToNullableType_Defaults_IfNullOrDbNull()
		{
			object actual = null;
			Assert.AreEqual(null, actual.ToNullableType<TestEnum>());
			Assert.AreEqual(null, actual.ToNullableType<int>());

			actual = DBNull.Value;
			Assert.AreEqual(null, actual.ToNullableType<TestEnum>());
			Assert.AreEqual(null, actual.ToNullableType<int>());
		}


		/// <summary>
		/// ToNullableType suceeds between IConvertibles
		/// </summary>
		[TestMethod]
		public void ToNullableType_Succeeds_FromOneConvertibleToAnother()
		{
			int intThree = 3;
			double doubleThree = 3.0;
			string stringThree = "3";
			TestEnum enumThree = TestEnum.Three;

			Assert.AreEqual(intThree, doubleThree.ToNullableType<int>());
			Assert.AreEqual(intThree, stringThree.ToNullableType<int>());
			Assert.AreEqual(intThree, intThree.ToNullableType<int>());
			Assert.AreEqual(intThree, enumThree.ToNullableType<int>());

			Assert.AreEqual(doubleThree, doubleThree.ToNullableType<double>());
			Assert.AreEqual(doubleThree, stringThree.ToNullableType<double>());
			Assert.AreEqual(doubleThree, intThree.ToNullableType<double>());
			Assert.AreEqual(doubleThree, enumThree.ToNullableType<double>());
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ToNullableType_Throws_IfNotIConvertibleFromType()
		{
			TimeSpan time = new TimeSpan();

			time.ToNullableType<double>();
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ToNullableType_Throws_IfNotIConvertibleToType()
		{
			double d = 3.14;
			d.ToNullableType<TimeSpan>();
		}


		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		public void ToNullableType_Succeeds_ToSelfTypeEvenIfNotIConvertible()
		{
			TimeSpan time = new TimeSpan();

			time.ToNullableType<TimeSpan>();
		}

		/// <summary>
		/// Test if not IConvertible from object
		/// </summary>
		[TestMethod]
		public void ToType_Succeeds_WithDecimalConversion()
		{
			var expected = 22.34;
			var actual = new Decimal(expected);
			var actualString = "22.34";

			Assert.AreEqual(expected, actual.ToType<double>());
			Assert.AreEqual(actual, expected.ToType<decimal>());
			Assert.AreEqual(actual, expected.ToType<decimal>());
			Assert.AreEqual(expected, actualString.ToType<double>());
			Assert.AreEqual(actual, actualString.ToType<decimal>());

			double? expected2 = null;
			decimal? actual2 = null;
			var actualString2 = string.Empty;

			Assert.AreEqual(expected2, actual2.ToNullableType<double>());
			Assert.AreEqual(actual2, expected2.ToNullableType<decimal>());
			Assert.AreEqual(actual2, expected2.ToNullableType<decimal>());
			Assert.AreEqual(expected2, actualString2.TryToNullableType<double>());
			Assert.AreEqual(actual2, actualString2.TryToNullableType<decimal>());
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when With() removed", false)]
		public void With_ReturnsValue_IfNonNull()
		{
			Address address = new Address();
			Person person = new Person { HomeAddress = address };

			Address actual = person.With(x => x.HomeAddress);

			Assert.IsNotNull(actual);
			Assert.AreEqual(address, actual);
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when With() removed", false)]
		public void With_ReturnsNullValue_IfNull()
		{
			Person person = null;

			Address actual = person.With(x => x.HomeAddress);

			Assert.IsNull(actual);
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when With() removed", false)]
		public void ChainedWith_ReturnsValue_IfNonNull()
		{
			Address address = new Address { City = "Parsons" };
			Person person = new Person { HomeAddress = address };

			string actual = person.With(x => x.HomeAddress).With(x => x.City);

			Assert.IsNotNull(actual);
			Assert.AreEqual(person.HomeAddress.City, actual);
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when With() removed", false)]
		public void ChainedWith_ReturnsNullValue_IfFirstNull()
		{
			Person person = null;

			string actual = person.With(x => x.HomeAddress).With(x => x.City);

			Assert.IsNull(actual);
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when With() removed", false)]
		public void ChainedWith_ReturnsNullValue_IfSecondNull()
		{
			Person person = new Person { HomeAddress = null };

			string actual = person.With(x => x.HomeAddress).With(x => x.City);

			Assert.IsNull(actual);
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when Return() removed", false)]
		public void Return_ReturnsValue_IfNonNull()
		{
			Address address = new Address();
			Person person = new Person { HomeAddress = address };

			Address actual = person.Return(x => x.HomeAddress, new Address {});

			Assert.IsNotNull(actual);
			Assert.AreEqual(address, actual);
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when Return() removed", false)]
		public void Return_ReturnsNullValue_IfNull()
		{
			Person person = null;
			Address defaultValue = new Address();

			Address actual = person.Return(x => x.HomeAddress, defaultValue);

			Assert.IsNotNull(actual);
			Assert.AreEqual(defaultValue, actual);
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when Return() removed", false)]
		public void ChainedReturn_ReturnsValue_IfNonNull()
		{
			Address address = new Address { City = "Parsons" };
			Person person = new Person { HomeAddress = address };

			string actual = person.With(x => x.HomeAddress).Return(x => x.City, string.Empty);

			Assert.IsNotNull(actual);
			Assert.AreEqual(person.HomeAddress.City, actual);
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when Return() removed", false)]
		public void ChaineReturn_ReturnsDefaultValue_IfFirstNull()
		{
			Person person = null;

			string actual = person.With(x => x.HomeAddress).Return(x => x.City, string.Empty);

			Assert.AreEqual(string.Empty, actual);
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when Return() removed", false)]
		public void ChainedReturn_ReturnsDefaultValue_IfSecondNull()
		{
			Person person = new Person { HomeAddress = null };

			string actual = person.With(x => x.HomeAddress).Return(x => x.City, string.Empty);

			Assert.AreEqual(string.Empty, actual);
		}

		/// <summary>
		/// Test the With extension method
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when Return() removed", false)]
		public void ChainedReturn_ReturnsNullValue_IfReturnEvaluationNull()
		{
			Person person = new Person { HomeAddress = new Address() };

			string actual = person.With(x => x.HomeAddress).Return(x => x.City, string.Empty);

			// yes, this will be null and not empty!  This is because the LHS itself ws null and not the evaluation.
			Assert.IsNull(actual);
		}

		/// <summary>
		/// Test the Return overload that takes a failure value and evaluation failure value
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when Return() removed", false)]
		public void ReturnGivesValueIfNotNull()
		{
			const string expected = "Smith";
			var person = new Person { Name = expected };

			var actual = person.Return(p => p.Name, "No Person", "No Name");

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Test the Return overload that takes a failure value and evaluation failure value
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when Return() removed", false)]
		public void ReturnGivesFailureValueIfNull()
		{
			const string expected = "No Person";
			Person person = null;

			var actual = person.Return(p => p.Name, expected, "No Name");

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Test the Return overload that takes a failure value and evaluation failure value
		/// </summary>
		[TestMethod]
		[Obsolete("Remove when Return() removed", false)]
		public void ReturnGivesEvaluationFailureValueIfNotNullButEvaluatesNull()
		{
			const string expected = "No Name";
			var person = new Person { Name = null };

			var actual = person.Return(p => p.Name, "No Person", expected);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void Maybe_ReturnsValue_IfNonNullAndReferenceType()
		{
			Address address = new Address();
			Person person = new Person { HomeAddress = address };

			Address actual = person.Maybe(x => x.HomeAddress);

			Assert.IsNotNull(actual);
			Assert.AreEqual(address, actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void Maybe_ReturnsNullValue_IfNull()
		{
			Person person = null;

			Address actual = person.Maybe(x => x.HomeAddress);

			Assert.IsNull(actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void ChainedMaybe_ReturnsValue_IfNonNullAndReferenceType()
		{
			Address address = new Address { City = "Parsons" };
			Person person = new Person { HomeAddress = address };

			string actual = person.Maybe(x => x.HomeAddress).Maybe(x => x.City);

			Assert.IsNotNull(actual);
			Assert.AreEqual(person.HomeAddress.City, actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void ChainedMaybe_ReturnsNullValue_IfFirstNull()
		{
			Person person = null;

			string actual = person.Maybe(x => x.HomeAddress).Maybe(x => x.City);

			Assert.IsNull(actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void ChainedMaybe_ReturnsNullValue_IfSecondNull()
		{
			Person person = new Person { HomeAddress = null };

			string actual = person.Maybe(x => x.HomeAddress).Maybe(x => x.City);

			Assert.IsNull(actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void Maybe_ReturnsValue_IfNonNull()
		{
			Address address = new Address();
			Person person = new Person { HomeAddress = address };

			Address actual = person.Maybe(x => x.HomeAddress, new Address { });

			Assert.IsNotNull(actual);
			Assert.AreEqual(address, actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void REturn_ReturnsNullValue_IfNull()
		{
			Person person = null;
			Address defaultValue = new Address();

			Address actual = person.Maybe(x => x.HomeAddress, defaultValue);

			Assert.IsNotNull(actual);
			Assert.AreEqual(defaultValue, actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void ChainedMaybe_ReturnsValue_IfNonNull()
		{
			Address address = new Address { City = "Parsons" };
			Person person = new Person { HomeAddress = address };

			string actual = person.Maybe(x => x.HomeAddress).Maybe(x => x.City, string.Empty);

			Assert.IsNotNull(actual);
			Assert.AreEqual(person.HomeAddress.City, actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void ChaineMaybe_ReturnsDefaultValue_IfFirstNull()
		{
			Person person = null;

			string actual = person.Maybe(x => x.HomeAddress).Maybe(x => x.City, string.Empty);

			Assert.AreEqual(string.Empty, actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void ChainedMaybe_ReturnsDefaultValue_IfSecondNull()
		{
			Person person = new Person { HomeAddress = null };

			string actual = person.Maybe(x => x.HomeAddress).Maybe(x => x.City, string.Empty);

			Assert.AreEqual(string.Empty, actual);
		}

		/// <summary>
		/// Test the Maybe extension method
		/// </summary>
		[TestMethod]
		public void ChainedMaybe_ReturnsNullValue_IfMaybeEvaluationNull()
		{
			Person person = new Person { HomeAddress = new Address() };

			string actual = person.Maybe(x => x.HomeAddress).Maybe(x => x.City, string.Empty);

			// yes, this will be null and not empty!  This is because the LHS itself ws null and not the evaluation.
			Assert.IsNull(actual);
		}

		/// <summary>
		/// Test the In extension method
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InThrowsIfValueNull()
		{
			string s = null;

			s.In("X", "Y", "Z");
		}

		/// <summary>
		/// Test the In extension method
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InThrowsIfParametersNull()
		{
			string s = "A";

			s.In(null);
		}

		/// <summary>
		/// Test the In extension method
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void InThrowsIfParametersLengthLessThanOne()
		{
			string s = "A";

			s.In();
		}

		/// <summary>
		/// Test the In extension method
		/// </summary>
		[TestMethod]
		public void InSucceedsIfInSet()
		{
			string s = "A";

			Assert.IsTrue(s.In("A", "B", "C", "D"));
			Assert.IsTrue(s.In("B", "A", "C", "D"));
			Assert.IsTrue(s.In("B", "C", "A", "D"));
			Assert.IsTrue(s.In("B", "C", "D", "A"));
			Assert.IsFalse(s.In("B", "C", "D", "E"));
		}

		/// <summary>
		/// Test the EqualsAny extension method
		/// </summary>
		[TestMethod]
		public void EqualsAnySucceedsIfValueNull()
		{
			string s = null;

			Assert.IsFalse(s.EqualsAny("X", "Y", "Z"));
			Assert.IsTrue(s.EqualsAny("X", "Y", null));
		}

		/// <summary>
		/// Test the EqualsAny extension method
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void EqualsAnyThrowsIfParametersNull()
		{
			string s = "A";

			s.EqualsAny(null);
		}

		/// <summary>
		/// Test the EqualsAny extension method
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void EqualsAnyThrowsIfParametersLengthLessThanOne()
		{
			string s = "A";

			s.EqualsAny();
		}

		/// <summary>
		/// Test the EqualsAny extension method
		/// </summary>
		[TestMethod]
		public void EqualsAnySucceedsIfInSet()
		{
			string s = "A";

			Assert.IsTrue(s.EqualsAny("A", "B", "C", "D"));
			Assert.IsTrue(s.EqualsAny("B", "A", "C", "D"));
			Assert.IsTrue(s.EqualsAny("B", "C", "A", "D"));
			Assert.IsTrue(s.EqualsAny("B", "C", "D", "A"));
			Assert.IsFalse(s.EqualsAny("B", "C", "D", "E"));
		}

        /// <summary>
        /// Test the Maybe extension method
        /// </summary>
        [TestMethod]
        public void MaybeValueReturnsDefaultIfNull()
        {
            double? salary = null;

            var actual = salary.MaybeValue();

            Assert.AreEqual(default(double), actual);
        }

        /// <summary>
        /// Test the Maybe extension method
        /// </summary>
        [TestMethod]
        public void MaybeValueWithReplacementReturnsReplacementIfNull()
        {
            double? salary = null;

            var actual = salary.MaybeValue(3.14);

            Assert.AreEqual(3.14, actual);
        }

        /// <summary>
        /// Test the Maybe extension method
        /// </summary>
        [TestMethod]
        public void MaybeValueReturnsValueIfNotNull()
        {
            double? salary = 3.14;

            var actual = salary.MaybeValue();

            Assert.AreEqual(salary.Value, actual);
        }

        /// <summary>
        /// Test the Maybe extension method
        /// </summary>
        [TestMethod]
        public void MaybeValueWithReplacementReturnsValueIfNotNull()
        {
            double? salary = 3.14;

            var actual = salary.MaybeValue(42.0);

            Assert.AreEqual(salary.Value, actual);
        }

        /// <summary>
        /// Test the Maybe extension method
        /// </summary>
        [TestMethod]
        public void MaybeValueWithProjectionReturnsDefaultIfNull()
        {
            double? salary = null;

            var actual = salary.MaybeValue(d => (int)(d * 2.0));

            Assert.AreEqual(default(int), actual);
        }

        /// <summary>
        /// Test the Maybe extension method
        /// </summary>
        [TestMethod]
        public void MaybeValueWithProjectionWithReplacementReturnsReplacementIfNull()
        {
            double? salary = null;

            var actual = salary.MaybeValue(d => (int)(d * 2.0), 42);

            Assert.AreEqual(42, actual);
        }

        /// <summary>
        /// Test the Maybe extension method
        /// </summary>
        [TestMethod]
        public void MaybeValueWithProjectionReturnsValueIfNotNull()
        {
            double? salary = 3.14;

            var actual = salary.MaybeValue(d => (int)(d * 2.0));

            Assert.AreEqual(6, actual);
        }

        /// <summary>
        /// Test the Maybe extension method
        /// </summary>
        [TestMethod]
        public void MaybeValueWithProjectionWithReplacementReturnsValueIfNotNull()
        {
            double? salary = 3.14;

            var actual = salary.MaybeValue(d => (int)(d * 2.0));

            Assert.AreEqual(6, actual);
        }
    }
}