using System;
using System.Collections.Generic;
using System.Threading;


namespace SharedAssemblies.General.Threading
{
    /// <summary>
    /// A thread pool that executes tasks on a group of threads (consumers) from a bucket of tasks.
    /// This is the generic version where you can strongly type the Action to take a parameter of
    /// type T.
    /// </summary>
    /// <typeparam name="T">The type of parameter each action will take.</typeparam>
	/// <remarks>Obsolete as error in 2.0, redundant with Task in TPL.</remarks>
	[Obsolete("This class is completely redundant with the TPL.", true)]
	public class TaskThreadPool<T> : IDisposable
    {
        /// <summary>The bucket of tasks to consume.</summary>
        private readonly IBucket<TaskRegistration<T>> _taskBucket;

        /// <summary>A list of consumers for the tasks.</summary>
        private readonly List<TaskConsumer<T>> _taskConsumers;


        /// <summary>
        /// Property to control the number of threads to divvy up the work.
        /// </summary>
        public int ThreadCount { get; private set; }


        /// <summary>
        /// Constructs a task thread pool with the given number of threads.  It will use the 
        /// Normal thread priority and will construct its own bucket.
        /// </summary>
        /// <param name="threadCount">Number of threads to execute tasks on</param>
        public TaskThreadPool(int threadCount)
            : this(threadCount, null, ThreadPriority.Normal)
        {
        }


        /// <summary>
        /// Constructs a task thread pool with the given number of threads and a given bucket.
        /// If the bucket is null, it will be constructed.  The thread priority will be 
        /// Normal.
        /// </summary>
        /// <param name="threadCount">Number of threads to execute tasks on</param>
        /// <param name="taskBucket">The bucket to consume tasks from</param>
        public TaskThreadPool(int threadCount, IBucket<TaskRegistration<T>> taskBucket)
            : this(threadCount, taskBucket, ThreadPriority.Normal)
        {
        }


        /// <summary>
        /// Constructs a task thread pool with the given number of threads and a priority.
        /// This constructor will construct its own bucket and will consume with a Normal
        /// thread priority.
        /// </summary>
        /// <param name="threadCount">Number of threads to execute tasks on</param>
        /// <param name="priority">The priority of the consumer thread</param>
        public TaskThreadPool(int threadCount, ThreadPriority priority)
            : this(threadCount, null, priority)
        {            
        }


        /// <summary>
        /// Constructs a task thread pool with the given number of threads, given priority, and a
        /// given bucket.  If the bucket is null, it will construct a bucket.
        /// </summary>
        /// <param name="threadCount">Number of threads to execute tasks</param>
        /// <param name="taskBucket">The bucket to consume tasks from (creates if null)</param>
        /// <param name="priority">The priority of the consumer thread</param>
        public TaskThreadPool(int threadCount, IBucket<TaskRegistration<T>> taskBucket,
                              ThreadPriority priority)
        {
            if(threadCount > 0)
            {
                ThreadCount = threadCount;

                // create bucket if one not passed in
                _taskBucket = taskBucket ?? new Bucket<TaskRegistration<T>>();

                // create and initialize all consumers
                _taskConsumers = new List<TaskConsumer<T>>(ThreadCount);

                for(int i = 0; i < ThreadCount; ++i)
                {
                    var consumer = new TaskConsumer<T>(_taskBucket, priority);
                    _taskConsumers.Add(consumer);
                    consumer.Start();
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("threadCount", threadCount,
                    "The number of threads specified must be larger than zero.");
            }
        }


        /// <summary>
        /// Queues up a task to be worked on another thread
        /// </summary>
        /// <param name="action">The task to be run.</param>
        /// <param name="parameter">The parameter to be passed to the task.</param>
        public void QueueTask(Action<T> action, T parameter)
        {
            // add to the task bucket
            _taskBucket.Add(new TaskRegistration<T>
                                {
                                    Task = action,
                                    TaskParameter = parameter
                                });
        }


        /// <summary>
        /// Dispose of all resources
        /// </summary>
        public void Dispose()
        {
            _taskBucket.Clear();
            _taskConsumers.ForEach(c => c.Dispose());
        }
    }
}
