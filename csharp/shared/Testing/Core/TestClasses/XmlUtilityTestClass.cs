
namespace SharedAssemblies.Core.UnitTests.TestClasses
{
    /// <summary>
    /// Test class simply for XmlUtilityFixture
    /// </summary>
    public class XmlUtilityTestClass
    {
		/// <summary>Handle to an int field</summary>
		public int MyInt { get; set; }

		/// <summary>Handle to a double field</summary>
		public double MyDouble { get; set; }

		/// <summary>Handle to a string field</summary>
		public string MyString { get; set; }

		/// <summary>
		/// Initializes a new instance of the XmlUtilityTestClass
		/// </summary>
        public XmlUtilityTestClass()
        {
        }

		/// <summary>
		/// Initializes a new instance of the XmlUtilityTestClass
		/// </summary>
		/// <param name="myInt">Value for the int field</param>
		/// <param name="myDouble">Value for the double field</param>
		/// <param name="myString">Value for the string field</param>
        public XmlUtilityTestClass(int myInt, double myDouble, string myString)
        {
            MyInt = myInt;
            MyDouble = myDouble;
            MyString = myString;
        }

		/// <summary>
		/// Override for equality tests
		/// </summary>
		/// <param name="obj">the object to compare to</param>
		/// <returns>true if the objects are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            XmlUtilityTestClass other = obj as XmlUtilityTestClass;

            return other != null &&
                   MyInt == other.MyInt &&
                   MyDouble == other.MyDouble &&
                   MyString == other.MyString;
        }

		/// <summary>
		/// Override for GetHashCode
		/// </summary>
		/// <returns>A hash of the object</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
    }
}