using System;
using System.IO;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Health;

namespace SharedAssemblies.Monitoring.HealthCheck.Utilities.Health
{
	/// <summary>
	/// A helper factory to generate health checks for trivial tasks
	/// </summary>
	public static class HealthCheckUtility
	{
		/// <summary>
		/// A single instance of the success result to prevent constant
		/// creation for the common path.
		/// </summary>
		private static readonly Tuple<bool, string> _successResult
			= Tuple.Create(true, string.Empty);

		/// <summary>
		/// Factory that create a new instance of the generic health check
		/// </summary>
		/// <param name="predicate">The test to execute</param>
		/// <param name="name">The name of the generic health check</param>
		/// <returns>A new instance of a GenericHealthCheck</returns>
		public static IHealthCheck CreateGenericCheck(Func<bool> predicate, string name)
		{
			return new GenericHealthCheck(predicate, name);
		}

		/// <summary>
		/// Factory that create a new instance of the generic health check
		/// </summary>
		/// <param name="predicate">The test to execute</param>
		/// <param name="name">The name of the generic health check</param>
		/// <returns>A new instance of a GenericHealthCheck</returns>
		public static IHealthCheck CreateGenericCheck(Func<Tuple<bool, string>> predicate, string name)
		{
			return new GenericHealthCheck(predicate, name);
		}

		/// <summary>
		/// Factory that creates an IHealthCheck instance to see if the given
		/// file exists.
		/// </summary>
		/// <param name="filename">The filename to check for existence for</param>
		/// <returns>An initialized health check</returns>
		public static IHealthCheck CreateFileExistsCheck(string filename)
		{
			var name = string.Format("File Exists Check({0})", Path.GetFileName(filename));

			return CreateGenericCheck(() =>
			{
				if (!File.Exists(filename))
				{
					var message = string.Format("The requested file({0}) does not exist", filename);
					return Tuple.Create(false, message);
				}
				return _successResult;
			}, name);
		}

		/// <summary>
		/// Factory that creates an IHealthCheck instance to see if the given host
		/// exists on the network and is up.
		/// </summary>
		/// <param name="hostname">The hostname to check for existence for</param>
		/// <param name="timeout">The amount of time to wait for a reply</param>
		/// <returns>An initialized health check</returns>
		public static IHealthCheck CreateHostExistsCheck(string hostname, TimeSpan timeout)
		{
			var name = string.Format("Host Exists Check({0})", hostname);

			return CreateGenericCheck(() =>
			{
				var pinger = new Ping();
				var result = pinger.Send(hostname, (int)timeout.TotalMilliseconds);
				
				if ((result == null) || (result.Status != IPStatus.Success))
				{
					var message = string.Format("The requested host({0}) appears to be down", hostname);
					return Tuple.Create(false, message);
				}
				return _successResult;
			}, name);
		}

		/// <summary>
		/// Factory that creates an IHealthCheck instance to see if the specified
		/// port is open on the supplied host.
		/// </summary>
		/// <param name="hostname">The hostname to check for connection to</param>
		/// <param name="port">The port to attempt to connect to on the host</param>
		/// <returns>An initialized health check</returns>
		public static IHealthCheck CreatePortOpenCheck(string hostname, int port)
		{
			var name = string.Format("Port Open Check({0}:{1})", hostname, port);

			return CreateGenericCheck(() =>
			{
				var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect(hostname, port);

				if (!socket.Connected)
				{	// socket is disconnected on dispose
					var message = string.Format("The requested port ({0}:{1}) appears to be closed", hostname, port);
					return Tuple.Create(false, message);
				}
				return _successResult;
			}, name);
		}
	}
}
