using System;
using System.Threading;


namespace SharedAssemblies.General.Threading
{
    /// <summary>
    /// This class represents a consumer that will process discrete tasks from a bucket of
    /// Action delegates that take an object and return nothing.
    /// </summary>
    /// <typeparam name="T">The type of items the tasks operate on.</typeparam>
	/// <remarks>Obsolete as error in 2.0, redundant with Task in TPL.</remarks>
	[Obsolete("This class is completely redundant with the TPL.", true)]
	public class TaskConsumer<T> : DiscreteBucketConsumer<TaskRegistration<T>>
    {
        /// <summary>
        /// Construct a task consumer with normal thread priority and no bucket.
        /// </summary>
        public TaskConsumer()
            : this(null, ThreadPriority.Normal)
        {            
        }


        /// <summary>
        /// Construct a task consumer with the given thread priority and no bucket.
        /// </summary>
        /// <param name="priority">The priority at which the consuming thread should run.</param>
        public TaskConsumer(ThreadPriority priority)
            : this(null, priority)
        {
        }


        /// <summary>
        /// Construct a task consumer with the given bucket and normal thread priority.
        /// </summary>
        /// <param name="sourceBucket">The bucket to consume items from.</param>
        public TaskConsumer(IBucket<TaskRegistration<T>> sourceBucket)
            : this(sourceBucket, ThreadPriority.Normal)
        {
        }


        /// <summary>
        /// Construct a task consumer with the given thread priority and bucket.
        /// </summary>
        /// <param name="sourceBucket">The bucket to consume items from.</param>
        /// <param name="priority">The priority at which the consuming thread should run.</param>
        public TaskConsumer(IBucket<TaskRegistration<T>> sourceBucket, ThreadPriority priority)
            : base(PerformTask, sourceBucket, priority)
        {            
        }



        /// <summary>
        /// The only task this consumer performs is call the delegate and pass the parameter.
        /// </summary>
        /// <param name="taskRegistration">The task and parameter pair to run</param>
        private static void PerformTask(TaskRegistration<T> taskRegistration)
        {
            if (taskRegistration != null && taskRegistration.Task != null)
            {
                taskRegistration.Task(taskRegistration.TaskParameter);
            }         
        }
    }
}
