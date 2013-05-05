namespace SharedAssemblies.General.Logging.UnitTests.TestClasses
{
    /// <summary>
    /// A simple representation of a two-dimensional point
    /// </summary>
    public class Point
    {
        /// <summary>
        /// The X-coordinate
        /// </summary>
        public int X { get; set; }


        /// <summary>
        /// The Y-coordinate
        /// </summary>
        public int Y { get; set; }


        /// <summary>
        /// Formats the point to a string in the format [x,y].
        /// </summary>
        /// <returns>A string version of a point in the format [x,y]</returns>
        public override string ToString()
        {
            return string.Format("[{0},{1}]", X, Y);
        }
    }
}
