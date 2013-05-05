using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SharedAssemblies.General.Validation
{
    /// <summary>
    /// This static class starts the validation chain required for the fluent design pattern. 
    /// Thus, you will always start your validation with the following: Demand.That
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Specify the objects you want to validate using this method.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="targets">The list of objects to validate</param>
        /// <returns>ValidationChain containing all validation exceptions (if any)</returns>
        public static ValidationChain<T> That<T>(params T[] targets)
        {
            if (targets == null || targets.Length == 0)
            {
                throw new ArgumentException("Missing parameters to validate!");
            }

            return new ValidationChain<T>(targets);
        }
    }
}
