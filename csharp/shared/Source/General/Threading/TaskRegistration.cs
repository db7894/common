using System;


namespace SharedAssemblies.General.Threading
{
    /// <summary>
    /// The registration for a task to be run.  These tasks operate on a Task that is a
    /// method that takes a parameter of type T and returns void.
    /// </summary>
    /// <typeparam name="T">The type of parameter the task takes</typeparam>
	/// <remarks>Obsolete as error in 2.0, redundant with Task in TPL.</remarks>
	[Obsolete("This class is completely redundant with the TPL.", true)]
	public class TaskRegistration<T>
    {
        /// <summary>
        /// The action that will be performed on the task
        /// </summary>
        public Action<T> Task { get; set; }


        /// <summary>
        /// The parameter to pass to the task (if any)
        /// </summary>
        public T TaskParameter { get; set; }
    }
}
