using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Core.UnitTests.TestClasses;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// Unit test fixture for type extensions
    /// </summary>
    [TestClass]
    public class TypeExtensionsTest
    {
        /// <summary>
        /// The MSTest testing context
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// Test that System.Nullable types return true.
        /// </summary>
        [TestMethod]
        public void IsNullable_ReturnsTrue_OnNullableTypes()
        {
            Assert.IsTrue(typeof(int?).IsNullable());
            Assert.IsTrue(typeof(double?).IsNullable());
            Assert.IsTrue(typeof(char?).IsNullable());
            Assert.IsTrue(typeof(short?).IsNullable());
            Assert.IsTrue(typeof(uint?).IsNullable());
            Assert.IsTrue(typeof(ushort?).IsNullable());
            Assert.IsTrue(typeof(byte?).IsNullable());
            Assert.IsTrue(typeof(TestEnum?).IsNullable());
            Assert.IsTrue(typeof(decimal?).IsNullable());
            Assert.IsTrue(typeof(DateTime?).IsNullable());
            Assert.IsTrue(typeof(TimeSpan?).IsNullable());
        }


        /// <summary>
        /// test that reference types return false.
        /// </summary>
        [TestMethod]
        public void IsNullable_ReturnsFalse_OnReferenceTypes()
        {
            Assert.IsFalse(typeof(string).IsNullable());
            Assert.IsFalse(typeof(StringBuilder).IsNullable());
            Assert.IsFalse(typeof(InstantTime).IsNullable());
            Assert.IsFalse(typeof(XmlUtilityTestClass).IsNullable());
        }


        /// <summary>
        /// Test that raw primitives (not wrapped in System.Nullable) return false.
        /// </summary>
        [TestMethod]
        public void IsNullable_ReturnsFalse_OnNonNullablePrimitiveTypes()
        {
            Assert.IsFalse(typeof(int).IsNullable());
            Assert.IsFalse(typeof(double).IsNullable());
            Assert.IsFalse(typeof(char).IsNullable());
            Assert.IsFalse(typeof(short).IsNullable());
            Assert.IsFalse(typeof(uint).IsNullable());
            Assert.IsFalse(typeof(ushort).IsNullable());
            Assert.IsFalse(typeof(byte).IsNullable());
            Assert.IsFalse(typeof(TestEnum).IsNullable());
            Assert.IsFalse(typeof(decimal).IsNullable());
            Assert.IsFalse(typeof(DateTime).IsNullable());
            Assert.IsFalse(typeof(TimeSpan).IsNullable());            
        }
    }
}