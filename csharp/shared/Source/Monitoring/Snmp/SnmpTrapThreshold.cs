using System;
using System.Diagnostics.CodeAnalysis;

namespace SharedAssemblies.Monitoring.Snmp
{
    /// <summary>
    /// This class holds all the threshold information and
    /// evaluate if the threshold is exceeded.
    /// </summary>
	/// <remarks>This is now completely deprecated as an error in version 2.0</remarks>
	[Obsolete("Use $/SharedAssemblies/ThirdParty/SharpSnmp instead", true)]
	public class SnmpTrapThreshold
    {
        /// <summary>
        /// The name of the SNMP threshold.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The determined threshold value.
        /// </summary>
        public double Threshold { get; private set; }

        /// <summary>
        /// The predefined trap number.
        /// </summary>
        public int TrapNumber { get; private set; }

        /// <summary>
        /// The description of the trap.
        /// </summary>
        public string TrapMessage { get; private set; }

        /// <summary>
        /// The delegate for lambda
        /// </summary>
        public Func<bool> ThresholdEvaluator { get; private set; }

        /// <summary>
        /// Construct the trap threshold using separate extraction and comparer predicates
        /// </summary>
        /// <param name="name">Name of the threshold</param>
        /// <param name="threshold">Amount of the threshold</param>
        /// <param name="trapNumber">SNMP trap number</param>
        /// <param name="trapMessage">SNMP trap message</param>
        /// <param name="extractor">Method to extract value to compare</param>
        /// <param name="comparer">Method to compare extracted value to threshold</param>
        public SnmpTrapThreshold(string name, double threshold, int trapNumber, string trapMessage,
            Func<double> extractor, Func<double, double, bool> comparer)
        {
            if (extractor == null)
            {
                throw new ArgumentNullException("extractor", "Cannot have a null extractor delegate.");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer", "Cannot have a null comparer delegate.");
            }

            // set members
            Name = name;
            Threshold = threshold;
            TrapNumber = trapNumber;
            TrapMessage = trapMessage;

            // build delegate
            ThresholdEvaluator = () => comparer(extractor(), Threshold);
        }

        /// <summary>
        /// Construct the trap threshold using a single evaluation predicate
        /// </summary>
        /// <param name="name">Name of the threshold</param>
        /// <param name="threshold">Amount of the threshold</param>
        /// <param name="trapNumber">SNMP trap number</param>
        /// <param name="trapMessage">SNMP trap message</param>
        /// <param name="evaluator">Method to evaluate threshold</param>
        public SnmpTrapThreshold(string name, double threshold, int trapNumber, string trapMessage,
            Predicate<double> evaluator)
        {
            if (evaluator == null)
            {
                throw new ArgumentNullException("evaluator", "Cannot have a null evaluator delegate.");
            }

            // set members
            Name = name;
            Threshold = threshold;
            TrapNumber = trapNumber;
            TrapMessage = trapMessage;

            // build delegate
            ThresholdEvaluator = () => evaluator(Threshold);
        }

        /// <summary>
        /// This method evaluates if the threshold gets exceeded or not
        /// </summary>
        /// <returns>True if threshold gets exceeded, otherwise false</returns>
        public bool IsThresholdExceeded()
        {
            return ThresholdEvaluator();
        }

        /// <summary>
        /// Sends a trap
        /// </summary>
        /// <returns>True if trap sent successfully</returns>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException",
			Justification = "Since we're calling Win32, don't want anything to bleed through.")]
		public bool SendTrap()
        {
            try
            {
                return SnmpTraps.SendTrap(TrapNumber, TrapMessage);
            }
            catch (Exception ex)
            {
                throw new SnmpTrapException(string.Format("Exception while evaluating threshold: "
					+ " threshold={0}, trap={1}, text={2}, exception={3}",
					Threshold, TrapNumber, TrapMessage, ex.Message), ex);
            }            
        }
    }
}