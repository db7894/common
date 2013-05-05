using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharedAssemblies.Core.Containers;
using SharedAssemblies.General.Threading.Extensions;


namespace SharedAssemblies.General.Threading
{
	/// <summary>
	/// This is the base collection consumer behavior
	/// </summary>
	/// <typeparam name="T">The type of items in the collection to consume.</typeparam>
	/// <typeparam name="TCollection">The type of collection to use for consumption.</typeparam>
	public abstract class AbstractConsumer<T, TCollection> : IConsumer<T>, IDisposable
		where TCollection : IProducerConsumerCollection<T>, new()
	{
		private readonly Task[] _consumers;
		private readonly BlockingCollection<T>[] _collections;
		private readonly Func<T, object> _partitioner;
		private readonly object _startStopLock = new object();
		private readonly CancellationTokenSource _cancellationTokenSource;

		/// <summary>
		/// Gets/sets the TimeSpan to wait for a join on Stop() and Dispose()
		/// </summary>
		public TimeSpan TimeoutOnStop { get; set; }

		/// <summary>
		/// Gets/sets the TimeSpan to wait for items to consume before checking stop status.
		/// </summary>
		public TimeSpan TimeoutOnConsume { get; set; }

		/// <summary>
		/// Gets the number of consumer processes.
		/// </summary>
		public int ConsumerCount { get; private set; }

		/// <summary>
		/// Gets the number of underlying collections.
		/// </summary>
		public int CollectionCount { get; private set; }

		/// <summary>
		/// Returns true if currently in a consuming mode.  
		/// </summary>
		/// <returns>True if consumer is consuming.</returns>
		public bool IsConsuming { get; private set; }

		/// <summary>
		/// Returns true if the consumer has been stopped.
		/// </summary>
		/// <returns>True if consumer has been stopped.</returns>
		public bool IsStopped { get; private set; }

		/// <summary>
		/// Gets the total depth of the consumer collections
		/// </summary>
		public int Depth
		{
			get { return _collections.Sum(c => c.Count); }
		}

		/// <summary>
		/// Gets the total depth of the consumer collections
		/// </summary>
		public IEnumerable<int> Depths
		{
			get { return _collections.Select(c => c.Count).ToArray(); }
		}

		/// <summary>
		/// <para>
		/// Constructor which creates a consumer pointed to the given Collection.  This consumer can have multiple collections and multiple
		/// consumers.  If a partitioning projection is provided, there will be one collection per consumer thread and the projection will decide
		/// which consumer thread gets the data (this keeps like data processed by same consumer).  If the partitioning projection is null,
		/// then there will be one collection shared by all consumers.
		/// </para>
		/// <para>
		/// Note: This creates all tasks and collections, but does not start consumption until Start() is called.
		/// </para>
		/// </summary>
		/// <param name="consumerCount">The number of consumer threads to process data.</param>
		/// <param name="partitionerProjection">A project that provides a field to decide which collection to place data in.</param>
		protected AbstractConsumer(int consumerCount, Func<T, object> partitionerProjection)
		{
			if (consumerCount < 1)
			{
				throw new ArgumentOutOfRangeException("consumerCount", "The number of consumer threads must be positive");
			}

			// set default timeouts
			TimeoutOnStop = TimeSpan.FromMilliseconds(1.0);
			TimeoutOnConsume = TimeSpan.FromSeconds(1.0);

			// set the counts.  The collection count is 1 if the discriminator is null
			ConsumerCount = consumerCount;
			CollectionCount = partitionerProjection == null ? 1 : consumerCount;

			// create source for cancellation tokens
			_cancellationTokenSource = new CancellationTokenSource();

			// set the discriminator, and consumer/collection arrays
			_partitioner = partitionerProjection;
			_collections = ArrayFactory.Create(CollectionCount, () => new BlockingCollection<T>(new TCollection()));
			_consumers = ArrayFactory.Create(ConsumerCount, i => 
				new Task(StartConsumingUntilCancelled, Tuple.Create(i % CollectionCount, _cancellationTokenSource.Token),
					_cancellationTokenSource.Token, TaskCreationOptions.LongRunning));
		}

		/// <summary>
		/// Begins the consuming process, cannot be called if already consuming
		/// </summary>
		public void Start()
		{
			lock (_startStopLock)
			{
				if (IsConsuming)
				{
					throw new InvalidOperationException("The Consumer has already been started.");
				}

				if (IsStopped)
				{
					throw new InvalidOperationException("Cannot start a Consumer after it has been stopped.");
				}

				// set IsConsuming to true and start all the consumers
				IsConsuming = true;
				Array.ForEach(_consumers, c => c.Start());
			}
		}

		/// <summary>
		/// Dispose the instance by stopping the thread
		/// </summary>
		public void Dispose()
		{
			lock (_startStopLock)
			{
				Stop();

				// perform disposes, Dispose() is multi-call safe.
				Array.ForEach(_consumers, c => c.TryDispose());
				Array.ForEach(_collections, c => c.Dispose());
				_cancellationTokenSource.Dispose();
			}
		}

		/// <summary>
		/// Adds an item to be consumed.
		/// </summary>
		/// <param name="item">The item to add to the collection to be consumed.</param>
		public void Add(T item)
		{
			// if only one collection, index is zero, otherwise, use the partitioner to choose the correct collection
			var index = (CollectionCount == 1) ? 0 : _partitioner(item).GetHashCode() % CollectionCount;

			try
			{
				_collections[index].Add(item, _cancellationTokenSource.Token);
			}

			catch (ObjectDisposedException ex)
			{
				// just wrap with helper text appropriate for this class and nest the inner exception
				throw new ObjectDisposedException("Cannot Add() to Consumer after Dispose() has been called.", ex);
			}

			catch (InvalidOperationException ex)
			{
				// just wrap with helper text appropriate for this class and nest the inner exception
				throw new InvalidOperationException("Cannot Add() to Consumer after Stop() has been called.", ex);
			}
		}

		/// <summary>
		/// Stops the consumer given the TimeSpan (as specified by the TimeoutOnStop property) to wait for the thread to join.
		/// </summary>
		/// <returns>Returns true if the process has stopped (or was stopped).</returns>
		public bool Stop()
		{
			return Stop(TimeoutOnStop);
		}

		/// <summary>
		/// Stops the consumer given a specific TimeSpan to wait for the thread to join.
		/// </summary>
		/// <param name="timeoutToWaitForJoin">The TimeSpan to wait for a join.</param>
		/// <returns>Returns true if the process has stopped (or was stopped).</returns>
		public bool Stop(TimeSpan timeoutToWaitForJoin)
		{
			lock (_startStopLock)
			{
				IsStopped = true;

				// only stop if currently consuming
				if (IsConsuming)
				{
					IsConsuming = false;

					// set all collections to complete adding to give consumers chance to complete.
					Array.ForEach(_collections, c => c.CompleteAdding());

					// Trigger the cancel token, the consumers will fall out of loop eventually asynchronously.
					_cancellationTokenSource.Cancel();

					// attempt to wait for all consumer to join.
					return _consumers.TryWaitAll(timeoutToWaitForJoin);
				}
			}

			return true;
		}

		/// <summary>
		/// Method to override to consume items until the cancellation token is set.
		/// </summary>
		/// <param name="collection">The collection to consume from.</param>
		/// <param name="token">The cancellation token to halt consumption.</param>
		protected abstract void ConsumeUntilCancelled(BlockingCollection<T> collection, CancellationToken token);

		/// <summary>
		/// This private method is the consumer's thread function.
		/// </summary>
		/// <param name="state">The cancellation token as state.</param>
		private void StartConsumingUntilCancelled(object state)
		{
			var tuple = (Tuple<int, CancellationToken>)state;

			// launch our consume method override with the collection to consume from and the cancellation token to watch
			ConsumeUntilCancelled(_collections[tuple.Item1], tuple.Item2);
		}
	}
}