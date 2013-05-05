namespace SharedAssemblies.Core.Extensions
{
    /// <summary>
    /// The type of the comparisson for IsBetween.
    /// </summary>
    public enum BetweenComparison   
    {
        /// <summary>
        /// Checks if value is between two values including those two values.
        /// I.e.  low &lt;= value &lt;= high.
        /// </summary>
        Inclusive, 

        /// <summary>
        /// Checks to see if a value is strictly between two values but not including
        /// those two values.  I.e.  low &lt; value &lt; high.
        /// </summary>
        Exclusive,
    }
}
