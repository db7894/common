using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;


namespace SharedAssemblies.General.Testing
{
	/// <summary>
	/// A helper class to simplify working with private data. The only use case
	/// this should have is for testing, debugging, or system tools.
	/// </summary>
	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
		"ST5005:NoEmptyCatchBlocks",
		Justification = "This class is a unit tester helper to see if two types are equal.")]
	public static class Reflector
	{
		#region Get Field / Property

		/// <summary>
		/// Retrieves the value of an internal field
		/// </summary>
		/// <typeparam name="TInput">The type of class</typeparam>
		/// <param name="name">The field to retrieve</param>
		/// <returns>Returns the field from the object</returns>
		public static object GetField<TInput>(string name)
			where TInput : new()
		{
			return GetField(new TInput(), name);
		}

		/// <summary>
		/// Retrieves the value of an internal field
		/// </summary>
		/// <typeparam name="TInput">The type of class</typeparam>
		/// <param name="instance">An instance of the class</param>
		/// <param name="name">The field to retrieve</param>
        /// <returns>Returns the field from the object</returns>
        public static object GetField<TInput>(TInput instance, string name)
		{
			object result = null;

			try
			{
				var handle = ReflectField(typeof(TInput), name);
				result = handle.GetValue(instance);
			}
			catch (ArgumentNullException)
			{
			}

		    return result;
		}

		/// <summary>
		/// Retrieves the value of an internal property
		/// </summary>
		/// <typeparam name="TInput">the type of class</typeparam>
		/// <param name="name">The property to retrieve</param>
        /// <returns>Returns the property from the object</returns>
        public static object GetProperty<TInput>(string name)
			where TInput : new()
		{
			return GetProperty(new TInput(), name);
		}

		/// <summary>
		/// Retrieves the value of an internal property
		/// </summary>
		/// <typeparam name="TInput">The type of the class</typeparam>
		/// <param name="instance">An instance of the class</param>
		/// <param name="name">The property to retrieve</param>
        /// <returns>Returns the property from the object</returns>
        public static object GetProperty<TInput>(TInput instance, string name)
		{
			object result = null;

			try
			{
				var handle = ReflectProperty(typeof(TInput), name);
				result = handle.GetValue(instance, null);
			}
			catch (ArgumentNullException)
			{
			}

		    return result;
		}

		/// <summary>
		/// Retrieves the value of an internal field from a static type
		/// </summary>
		/// <param name="type">The type of the static class</param>
		/// <param name="name">The field to retrieve</param>
        /// <returns>Returns the property from the object</returns>
        public static object GetField(Type type, string name)
		{
			object result = null;

			try
			{
				var handle = ReflectField(type, name);
				result = handle.GetValue(null);
			}
			catch (ArgumentNullException)
			{			    
			}

			return result;
		}

		/// <summary>
		/// Retrieves the value of an internal property from a static type
		/// </summary>
		/// <param name="type">The type of the static class</param>
		/// <param name="name">The property to retrieve</param>
        /// <returns>Returns the property from the object</returns>
        public static object GetProperty(Type type, string name)
		{
			object result = null;

			try
			{
				var handle = ReflectProperty(type, name);
				result = handle.GetValue(null, null);
			}
			catch (ArgumentNullException)
			{
			}

		    return result;
		}

		/// <summary>
		/// Retrieves the value of an internal field from the given internal type.
		/// For this to work, you will need to know another public type from that
		/// assembly or the full assembly name.
		/// </summary>
		/// <param name="name">The field to retrieve</param>
		/// <param name="friend">Another type in the requested assembly</param>
		/// <param name="type">The internal type to retrieve</param>
		/// <returns>Returns the property from the object</returns>
		public static object GetField(string name, Type friend, string type)
		{
			object result = null;

			try
			{
				var handle = friend.Assembly.GetType(type, false, true);
				result = GetField(handle, name);
			}
			catch (ArgumentNullException)
			{
			}

			return result;
		}

		/// <summary>
		/// Retrieves the value of an internal property from the given internal type.
		/// For this to work, you will need to know another public type from that
		/// assembly or the full assembly name.
		/// </summary>
		/// <param name="name">The property to retrieve</param>
		/// <param name="friend">Another type in the requested assembly</param>
		/// <param name="type">The internal type to retrieve</param>
		/// <returns>Returns the property from the object</returns>
		public static object GetProperty(string name, Type friend, string type)
		{
			object result = null;

			try
			{
				var handle = friend.Assembly.GetType(type, false, true);
				result = GetProperty(handle, name);
			}
			catch (ArgumentNullException)
			{
			}

			return result;
		}

		#endregion

		#region Set Field / Property

		/// <summary>
		/// Retrieves the value of an internal property
		/// </summary>
		/// <typeparam name="TInput">The type of the class</typeparam>
		/// <param name="instance">An instance of the class</param>
		/// <param name="name">The property to retrieve</param>
		/// <param name="value">What to set the property to</param>
		public static void SetProperty<TInput>(TInput instance, string name, object value)
		{
			try
			{
				var handle = ReflectProperty(typeof(TInput), name);
				handle.SetValue(instance, value, null);
			}
			catch (ArgumentNullException)
			{
			}
		}

	    /// <summary>
		/// Retrieves the value of an internal field
		/// </summary>
		/// <typeparam name="TInput">The type of class</typeparam>
		/// <param name="instance">An instance of the class</param>
		/// <param name="name">The field to retrieve</param>
		/// <param name="value">What to set the property to</param>
		public static void SetField<TInput>(TInput instance, string name, object value)
		{
			try
			{
				var handle = ReflectField(typeof(TInput), name);
				handle.SetValue(instance, value);
			}
			catch (ArgumentNullException)
			{
			}
		}

		#endregion

		#region Execute Method

		/// <summary>
		/// Call the specified method from the given type
		/// </summary>
		/// <typeparam name="TInput">The type of class</typeparam>
		/// <param name="instance">An instance of the class</param>
		/// <param name="name">The method to execute</param>
		/// <param name="parameters">The parameters to pass to the method</param>
		/// <returns>The result of the operation</returns>
		public static object ExecuteMethod<TInput>(TInput instance, string name, object[] parameters)
		{
			object result = null;

			try
			{
				result = ReflectMethod(typeof(TInput), name, instance, parameters);
			}
			catch (ArgumentNullException)
			{
			}

			return result;
		}

		/// <summary>
		/// Call the specified method from the given static type
		/// </summary>
		/// <typeparam name="TInput">The type of class</typeparam>
		/// <param name="name">The method to execute</param>
		/// <param name="parameters">The parameters to pass to the method</param>
		/// <returns>The result of the operation</returns>
		public static object ExecuteMethod<TInput>(string name, object[] parameters)
		{
			object result = null;

			try
			{
				result = ReflectMethod(typeof(TInput), name, null, parameters);
			}
			catch (ArgumentNullException)
			{
			}

			return result;
		}

		/// <summary>
		/// Call the specified method from the given internal type. For this
		/// to work, you will need to know another public type from that assembly
		/// or the full assembly name.
		/// </summary>
		/// <param name="name">The method to execute</param>
		/// <param name="parameters">The parameters to pass to the method</param>
		/// <param name="friend">Another type in the requested assembly</param>
		/// <param name="type">The internal type to retrieve</param>
		/// <returns>The result of the operation</returns>
		public static object ExecuteMethod(string name, object[] parameters, Type friend, string type)
		{
			object result = null;

			try
			{
				var handle = friend.Assembly.GetType(type, false, true);
				result = ReflectMethod(handle, name, null, parameters);
			}
			catch (ArgumentNullException)
			{
			}

			return result;
		}

		#endregion

		#region Private Helper Methods

		/// <summary>
		/// Helper method to abstract away the Type.InvokeMember method
		/// </summary>
		/// <param name="type">The type to call a method on</param>
		/// <param name="name">The method to execute</param>
		/// <param name="instance">An instance of the class</param>
		/// <param name="parameters">The parameters to pass to the method</param>
		/// <returns>The result of the operation</returns>
		private static object ReflectMethod(Type type, string name,
			object instance, object[] parameters)
		{
			return type.InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
				| BindingFlags.Instance | BindingFlags.Static
				| BindingFlags.Public | BindingFlags.NonPublic,
				Type.DefaultBinder, instance, parameters);
		}

		/// <summary>
		/// Helper method to abstract away the Type.GetField method
		/// </summary>
		/// <param name="type">The type to call a method on</param>
		/// <param name="name">The method to execute</param>
		/// <returns>The result of the operation</returns>
		private static FieldInfo ReflectField(Type type, string name)
		{
			return type.GetField(name, BindingFlags.IgnoreCase
				| BindingFlags.Public | BindingFlags.NonPublic
				| BindingFlags.Static | BindingFlags.Instance);
		}

		/// <summary>
		/// Helper method to abstract away the Type.GetProperty method
		/// </summary>
		/// <param name="type">The type to call a method on</param>
		/// <param name="name">The method to execute</param>
		/// <returns>The result of the operation</returns>
		private static PropertyInfo ReflectProperty(Type type, string name)
		{
			return type.GetProperty(name, BindingFlags.IgnoreCase
				| BindingFlags.Public | BindingFlags.NonPublic
				| BindingFlags.Static | BindingFlags.Instance);
		}

		#endregion
	}
}
