using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Core.UnitTests.TestClasses;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// ConvertUtility test fixture
    /// </summary>
    [TestClass]
    public class ConverterTest
    {
        /// <summary>
        /// The test context property
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
            IConvertible actual = DBNull.Value;
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
            IConvertible boxedZero = 0;
            IConvertible boxedSeven = 7;

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
            Assert.IsTrue(((IConvertible)actual).IsNullOrDbNull());
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
        /// Testing ToEnum successes from string -> enum
        /// </summary>
        [TestMethod]
        public void ToEnum_CorrectlyConvertsEnum_FromString()
        {
            Assert.AreEqual(TestEnum.Zero, "Zero".ToEnum(TestEnum.One));
            Assert.AreEqual(TestEnum.One, "One".ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Two, "Two".ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Three, "Three".ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Four, "Four".ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Five, "Five".ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Six, "Six".ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Seven, "Seven".ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Eight, "Eight".ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Nine, "Nine".ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Ten, "Ten".ToEnum(TestEnum.Zero));
        }


        /// <summary>
        /// Testing ToEnum successes from number -> enum
        /// </summary>
        [TestMethod]
        public void ToEnum_CorrectlyConvertsEnum_FromNumeric()
        {
            const short shortSeven = 7;
            const ushort ushortEight = 8;
            const byte byteNine = 9;
            const double doubleTen = 10.2;

            Assert.AreEqual(TestEnum.Zero, 0.ToEnum(TestEnum.One));
            Assert.AreEqual(TestEnum.One, 1.ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Two, 2.ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Three, 3.ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Four, 4.ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Five, 5.ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Six, 6.ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Seven, shortSeven.ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Eight, ushortEight.ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Nine, byteNine.ToEnum(TestEnum.Zero));
            Assert.AreEqual(TestEnum.Ten, doubleTen.ToEnum(TestEnum.Zero));
        }


        /// <summary>
        /// Testing ToEnum only works on enums
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToEnum_Throws_IfToTypeNotEnum()
        {
            int x = 6.ToEnum<int>(0);
        }


        /// <summary>
        /// Testing ToEnum defaults for nulls
        /// </summary>
        [TestMethod]
        public void ToEnum_ReturnsDefault_IfNull()
        {
            const IConvertible actualNull = null;
            IConvertible actualDbNull = DBNull.Value;

            Assert.AreEqual(TestEnum.One, actualNull.ToEnum(TestEnum.One));
            Assert.AreEqual(TestEnum.Two, actualDbNull.ToEnum(TestEnum.Two));
        }


        /// <summary>
        /// Testing ToEnum defaults for nulls
        /// </summary>
        [TestMethod]
        public void ToEnum_Suceeds_IfIntOutOfRange()
        {
            Assert.AreEqual((TestEnum)13, 13.ToEnum(TestEnum.One));
        }


        /// <summary>
        /// Testing ToEnum defaults for nulls
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToEnum_Throws_IfStringOutOfRange()
        {
            string s = "Twenty";
            s.ToEnum(TestEnum.One);
        }


        /// <summary>
        /// Testing ToNullableEnum successes from string -> enum
        /// </summary>
        [TestMethod]
        public void ToNullableEnum_CorrectlyConvertsEnum_FromString()
        {
            Assert.AreEqual(TestEnum.Zero, "Zero".ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.One, "One".ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Two, "Two".ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Three, "Three".ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Four, "Four".ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Five, "Five".ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Six, "Six".ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Seven, "Seven".ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Eight, "Eight".ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Nine, "Nine".ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Ten, "Ten".ToNullableEnum<TestEnum>());
        }


        /// <summary>
        /// Testing ToNullableEnum successes from number -> enum
        /// </summary>
        [TestMethod]
        public void ToNullableEnum_CorrectlyConvertsEnum_FromNumeric()
        {
            const short shortSeven = 7;
            const ushort ushortEight = 8;
            const byte byteNine = 9;
            const double doubleTen = 10.2;

            Assert.AreEqual(TestEnum.Zero, 0.ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.One, 1.ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Two, 2.ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Three, 3.ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Four, 4.ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Five, 5.ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Six, 6.ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Seven, shortSeven.ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Eight, ushortEight.ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Nine, byteNine.ToNullableEnum<TestEnum>());
            Assert.AreEqual(TestEnum.Ten, doubleTen.ToNullableEnum<TestEnum>());
        }


        /// <summary>
        /// Testing ToNullableEnum only works on enums
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToNullableEnum_Throws_IfToTypeNotEnum()
        {
            int i = 6;
            i.ToNullableEnum<int>();
        }


        /// <summary>
        /// Testing ToNullableEnum defaults for nulls
        /// </summary>
        [TestMethod]
        public void ToNullableEnum_ReturnsDefault_IfNull()
        {
            const IConvertible actualNull = null;
            IConvertible actualDbNull = DBNull.Value;

            Assert.AreEqual(null, actualNull.ToNullableEnum<TestEnum>());
            Assert.AreEqual(null, actualDbNull.ToNullableEnum<TestEnum>());
        }


        /// <summary>
        /// Testing ToNullableEnum defaults for nulls
        /// </summary>
        [TestMethod]
        public void ToNullableEnum_Suceeds_IfIntOutOfRange()
        {
            Assert.AreEqual((TestEnum)13, 13.ToNullableEnum<TestEnum>());
        }


        /// <summary>
        /// Testing ToNullableEnum defaults for nulls
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToNullableEnum_Throws_IfStringOutOfRange()
        {
            string s = "Twenty";
            s.ToNullableEnum<TestEnum>();
        }


        /// <summary>
        /// ConvertTo defaults on dbnull or null
        /// </summary>
        [TestMethod]
        public void ConvertTo_Defaults_IfNullOrDbNull()
        {
            IConvertible actual = null;
            Assert.AreEqual(TestEnum.Seven, actual.ConvertTo<TestEnum>(TestEnum.Seven));

            actual = DBNull.Value;
            Assert.AreEqual(TestEnum.Eight, actual.ConvertTo<TestEnum>(TestEnum.Eight));
        }


        /// <summary>
        /// ConvertTo suceeds between IConvertibles
        /// </summary>
        [TestMethod]
        public void ConvertTo_Succeeds_FromOneConvertibleToAnother()
        {
            int intThree = 3;
            double doubleThree = 3.0;
            string stringThree = "3";
            string stringThreeLiteral = "Three";
            TestEnum enumThree = TestEnum.Three;

            Assert.AreEqual(intThree, doubleThree.ConvertTo<int>(0));
            Assert.AreEqual(intThree, stringThree.ConvertTo<int>(0));
            Assert.AreEqual(intThree, intThree.ConvertTo<int>(0));
            Assert.AreEqual(intThree, enumThree.ConvertTo<int>(0));

            Assert.AreEqual(doubleThree, doubleThree.ConvertTo<double>(0.0));
            Assert.AreEqual(doubleThree, stringThree.ConvertTo<double>(0.0));
            Assert.AreEqual(doubleThree, intThree.ConvertTo<double>(0.0));
            Assert.AreEqual(doubleThree, enumThree.ConvertTo<double>(0.0));

            // all primatives -> string are the stringified version of the primative
            Assert.AreEqual(stringThree, doubleThree.ConvertTo<string>(string.Empty));
            Assert.AreEqual(stringThree, stringThree.ConvertTo<string>(string.Empty));
            Assert.AreEqual(stringThree, intThree.ConvertTo<string>(string.Empty));

            // enum -> string will be the literal translation of the text of the enum
            Assert.AreEqual(stringThreeLiteral, enumThree.ConvertTo<string>(string.Empty));
        }


        /// <summary>
        /// Test if not IConvertible from IConvertible
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ConvertTo_Throws_IfNotIConvertibleToType()
        {
            InstantTime time = new InstantTime();

            double d = 3.14;
            d.ConvertTo<InstantTime>(time);
        }


        /// <summary>
        /// ConvertToNullable defaults on dbnull or null
        /// </summary>
        [TestMethod]
        public void ConvertToNullable_Defaults_IfNullOrDbNull()
        {
            IConvertible actual = null;
            Assert.AreEqual(null, actual.ConvertToNullable<TestEnum>());

            actual = DBNull.Value;
            Assert.AreEqual(null, actual.ConvertToNullable<TestEnum>());
        }


        /// <summary>
        /// ConvertToNullable suceeds between IConvertibles
        /// </summary>
        [TestMethod]
        public void ConvertToNullable_Succeeds_FromOneConvertibleToAnother()
        {
            int intThree = 3;
            double doubleThree = 3.0;
            string stringThree = "3";
            TestEnum enumThree = TestEnum.Three;

            Assert.AreEqual(intThree, doubleThree.ConvertToNullable<int>());
            Assert.AreEqual(intThree, stringThree.ConvertToNullable<int>());
            Assert.AreEqual(intThree, intThree.ConvertToNullable<int>());
            Assert.AreEqual(intThree, enumThree.ConvertToNullable<int>());

            Assert.AreEqual(doubleThree, doubleThree.ConvertToNullable<double>());
            Assert.AreEqual(doubleThree, stringThree.ConvertToNullable<double>());
            Assert.AreEqual(doubleThree, intThree.ConvertToNullable<double>());
            Assert.AreEqual(doubleThree, enumThree.ConvertToNullable<double>());
        }


        /// <summary>
        /// Test if not IConvertible from IConvertible
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ConvertToNullable_Throws_IfNotIConvertibleToType()
        {
            double d = 3.14;
            d.ConvertToNullable<TimeSpan>();
        }
    }
}