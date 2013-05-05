﻿using System;
using System.Reflection;

namespace SharedAssemblies.Core.Resources
{
	/// <summary>
	/// Exception thrown when AssemblyResourceManager cannot retrieve specified resource
	/// </summary>
	public class AssemblyResourceNotFoundException : ApplicationException
	{
		private const string _defaultMessageFormat = "The resource '{0}' was not found in assembly {1}.";

		/// <summary>
		/// Gets the assembly from which the embedded resource was requested.
		/// </summary>
		public Assembly Assembly { get; private set; }

		/// <summary>
		/// Gets the name of the requested embedded resource.
		/// </summary>
		public string ResourceName { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyResourceNotFoundException"/> class.
		/// </summary>
		/// <param name="assembly">The assembly from which the embedded resource was requested.</param>
		/// <param name="resourceName">The name of the requested embedded resource.</param>
		public AssemblyResourceNotFoundException(Assembly assembly, string resourceName)
			: base(string.Format(_defaultMessageFormat, resourceName, assembly.FullName))
		{
			Assembly = assembly;
			ResourceName = resourceName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyResourceNotFoundException"/> class.
		/// </summary>
		/// <param name="assembly">The assembly from which the embedded resource was requested.</param>
		/// <param name="resourceName">The name of the requested embedded resource.</param>
		/// <param name="inner">Inner Exception</param>
		public AssemblyResourceNotFoundException(
			Assembly assembly, string resourceName, Exception inner)
			: base(string.Format(_defaultMessageFormat, resourceName, assembly.FullName), inner)
		{
			Assembly = assembly;
			ResourceName = resourceName;
		}
	}
}