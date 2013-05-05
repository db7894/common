using System;
using System.Threading;
using System.Threading.Tasks;


namespace SharedAssemblies.General.Threading.UnitTests.TestClasses
{
    /// <summary>
    /// Safe Task so that don't get errors during finalization from Task
    /// </summary>
    public class SafeTask : Task
    {
        /// <summary>
        /// Constructs an instance of a task
        /// </summary>
        /// <param name="action">The action to perform</param>
        public SafeTask(Action action)
            : base(action)
        {
        }

        /// <summary>
        /// Constructs an instance of a task
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="cancellationToken">The token to cancel stuff</param>
        public SafeTask(Action action, CancellationToken cancellationToken)
            : base(action, cancellationToken)
        {
        }

        /// <summary>
        /// Constructs an instance of a task
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="creationOptions">The options to create the task with</param>
        public SafeTask(Action action, TaskCreationOptions creationOptions)
            : base(action, creationOptions)
        {
        }

        /// <summary>
        /// Constructs an instance of a task
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="cancellationToken">The token to cancel stuff</param>
        /// <param name="creationOptions">The options to create the task with</param>
        public SafeTask(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : base(action, cancellationToken, creationOptions)
        {
        }

        /// <summary>
        /// Constructs an instance of a task
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="state">The state data</param>
        public SafeTask(Action<object> action, object state)
            : base(action, state)
        {
        }

        /// <summary>
        /// Constructs an instance of a task
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="state">The state data</param>
        /// <param name="cancellationToken">The token to cancel stuff</param>
        public SafeTask(Action<object> action, object state, CancellationToken cancellationToken)
            : base(action, state, cancellationToken)
        {
        }

        /// <summary>
        /// Constructs an instance of a task
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="state">The state data</param>
        /// <param name="creationOptions">The options to create the task with</param>
        public SafeTask(Action<object> action, object state, TaskCreationOptions creationOptions)
            : base(action, state, creationOptions)
        {
        }

        /// <summary>
        /// Constructs an instance of a task
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="state">The state data</param>
        /// <param name="cancellationToken">The token to cancel stuff</param>
        /// <param name="creationOptions">The options to create the task with</param>
        public SafeTask(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : base(action, state, cancellationToken, creationOptions)
        {
        }

        /// <summary>
        /// Overriding this so we don't throw during unit tests.
        /// </summary>
        /// <param name="disposing">True if we are disposing managed resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }

            catch (Exception)
            {   
                // catch and consume so we don't throw during finalization in unit tests.
            }
        }
    }
}
