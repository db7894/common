using System.Collections.Generic;
using System.Reflection;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
	/// <summary>
	/// Class that loads an AutoCounterRegistry from an assembly.
	/// </summary>
	internal static class AutoCounterAssemblyLoader
	{
		/// <summary>
		/// <para>
		/// Loads the default action to perform if an auto counter cannot be created in the operating
		/// system.  This is defined by the AutoCounterCreateFailedDefaultActionAttribute assembly
		/// level attribute in the target assembly specified.
		/// </para>
		/// <para>
		/// If the attribute cannot be found, it will return Default.
		/// </para>
		/// </summary>
		/// <param name="targetAssembly">The assembly to search for the default create failed action.</param>
		/// <returns>The default create failed action, if any.</returns>
		internal static CreateFailedAction LoadDefaultCreateFailedAction(Assembly targetAssembly)
		{
			var result = CreateFailedAction.Default;

			var attribute = targetAssembly.GetAttribute<AutoCounterCreateFailedDefaultActionAttribute>();

			if (attribute != null)
			{
				result = attribute.DefaultCreateFailedAction;
			}

			return result;
		}


		/// <summary>
		/// <para>
		/// Will create a new auto counter registry from a given assembly by analyzing the assembly
		/// for AutoCounterCategoryAttributes, AutoCounterAttributes, and AutoCounterCollection 
		/// attributes.
		/// </para>
		/// <para>
		/// Any duplicated attributes will be ignored, the first attribute found with the given 
		/// category, counter, or unique name will win in the appropriate indicies.
		/// </para>
		/// </summary>
		/// <param name="targetAssembly">The assembly to analyze for AutoCounter attributes.</param>
		/// <param name="createFailedAction">Action to take if counter does not exist.</param>
		/// <param name="interval">Time in milliseconds between flushes of non-locking
		/// auto counters.  A value of null indicates that the counters should use locks instead.
		/// </param>
		/// <returns>A registry containing a hierarchical list of registrations.</returns>
		internal static AutoCounterRegistry LoadCounterRegistry(Assembly targetAssembly,
																CreateFailedAction createFailedAction, 
																NonLockingAutoCountersAttribute interval)
		{
			var registry = new AutoCounterRegistry();

			// load the categories and counters first
			LoadCategoriesIntoRegistry(targetAssembly, registry);
			LoadCountersIntoRegistry(targetAssembly, registry, createFailedAction, interval);
			LoadCollectionsIntoRegistry(targetAssembly, registry, createFailedAction, interval);
			LinkAndValidateCollectionParentsInRegistry(targetAssembly, registry);

			return registry;
		}


		/// <summary>
		/// May be an easier way to do this, but this seems to work at the moment.  Can't link the
		/// ParentCollection on the first pass, because we may not have read the ParentCollection yet
		/// since the assembly attributes are never in any particular order when Reflecting.
		/// </summary>
		/// <param name="targetAssembly">The assembly to load from</param>
		/// <param name="registry">The registry of auto counters</param>
		private static void LinkAndValidateCollectionParentsInRegistry(Assembly targetAssembly, 
			AutoCounterRegistry registry)
		{
			// loop through each collection attribute
			foreach (var attribute in targetAssembly.GetAttributesReflectively<AutoCounterCollectionAttribute>())
			{
				// if parent is null or empty string, no parent and that's fine as is.
				if (!string.IsNullOrEmpty(attribute.ParentCollection))
				{
					// if the parent is non-null/non-empty, must exist
					var parentCollection = registry.Get<AutoCounterCollectionRegistration>(
						attribute.ParentCollection);

					if (parentCollection == null)
					{
						throw new AutoCounterException(string.Format("An attempt was made to declare an " +
							 "AutoCounterCollection \"{0}\" with an unknown parent collection {1} in " + 
							 "assembly {2}.",
							 attribute.Name, attribute.ParentCollection, targetAssembly.FullName));
					}

					// validate that the collection is valid in respect to its ancestor collections
					var collection = registry.Collections[attribute.Name];
					collection.ParentCollection = parentCollection;

					while (parentCollection != null)
					{
						// a collection cannot be an ancestor of itself, circular reference
						if (parentCollection == collection)
						{
							throw new AutoCounterException(string.Format("AutoCounterCollection {0} has a " + 
								"circular reference in the ParentCollection chain in assembly {1}.", 
								attribute.Name, targetAssembly));
						}

						// a single-instance collection can't have a multi-instance ancestor
						if (collection.InstanceType == InstanceType.SingleInstance &&
							parentCollection.InstanceType == InstanceType.MultiInstance)
						{
							throw new AutoCounterException(string.Format("AutoCounterCollection {0} is " + 
								"SingleInstance and cannot have MultiInstance parent {1} in assembly {2}.",
								  attribute.Name, parentCollection.UniqueName, targetAssembly));
						}

						parentCollection = parentCollection.ParentCollection;
					}
				}
			}
		}


		/// <summary>
		/// Loads the AutoCounterCollections into the registry by scanning the assembly for the
		/// AutoCounterCollectionAttributes and constructing them appropriately.
		/// </summary>
		/// <param name="targetAssembly">The assembly to load from</param>
		/// <param name="registry">The registry of auto counters</param>
		/// <param name="createFailedAction">Action to take if counter does not exist.</param>
		/// <param name="interval">Time in milliseconds between flushes of non-locking
		/// auto counters.  A value of null indicates that the counters should use locks instead.
		/// </param>
		private static void LoadCollectionsIntoRegistry(Assembly targetAssembly, 
														AutoCounterRegistry registry, 
														CreateFailedAction createFailedAction, 
														NonLockingAutoCountersAttribute interval)
		{
			// get all collection attributes from the assembly
			foreach (var attribute in targetAssembly.GetAttributesReflectively<AutoCounterCollectionAttribute>())
			{
				var counterMap = new Dictionary<string, AutoCounterRegistration>();
				
				// make sure all the counters referenced exist and are of a compatible type
				foreach (var counterName in attribute.AutoCounters)
				{
					var counter = registry.Get<AutoCounterRegistration>(counterName);

					// counter must exist
					if (counter == null)
					{
						throw new AutoCounterException(
							string.Format("Could not find counter with unique name \"{0}\" referenced " +
										  "by AutoCounterCollection \"{1}\" in assembly {2}.",
										  counterName, attribute.Name, targetAssembly.FullName));
					}

					// counter must be of same instance type as the collection containing it
					if (counter.InstanceType != attribute.InstanceType)
					{
						throw new AutoCounterException(string.Format("Cannot have AutoCounter {0} of " + 
							"type {1} in AutoCounterCollection {2} of type {3} in assembly {4}.",
							 counterName, counter.InstanceType, attribute.Name,
							 attribute.InstanceType, targetAssembly.FullName));
					}

					// add counter to collection
					counterMap.Add(counter.UniqueName, counter);
				}

				// defer linking parents until all collections are loaded
				var collection = new AutoCounterCollectionRegistration(attribute.InstanceType,
										attribute.Name, counterMap.Values, createFailedAction, interval);

				// if we get this far with no errors, we have a good collection
				RegisterUniqueName(registry, collection);
				registry.Collections[collection.UniqueName] = collection;
			}
		}


		/// <summary>
		/// Attempts to load the auto counters tagged in the assembly into the AutoCounterRegistry.
		/// </summary>
		/// <param name="targetAssembly">The assembly to load from</param>
		/// <param name="registry">The registry of auto counters</param>
		/// <param name="createFailedAction">Action to take if counter does not exist.</param>
		/// <param name="interval">Time in milliseconds between flushes of non-locking
		/// auto counters.  A value of null indicates that the counters should use locks instead.
		/// </param>
		private static void LoadCountersIntoRegistry(Assembly targetAssembly, AutoCounterRegistry registry, 
							CreateFailedAction createFailedAction, NonLockingAutoCountersAttribute interval)
		{
			// get all the individual auto counter attributes
			foreach (var attribute in targetAssembly.GetAttributesReflectively<AutoCounterAttribute>())
			{
				LoadCounterIntoRegistry(attribute, targetAssembly, registry, createFailedAction, interval);
			}

			// get all auto heartbeat counters
			foreach (var attribute in targetAssembly.GetAttributesReflectively<AutoHeartbeatAttribute>())
			{
				LoadCounterIntoRegistry(attribute, targetAssembly, registry, createFailedAction, interval);
			}

			// and get all of the heartbeat counter attributes which are also auto
			// counters with a little candy added.
			foreach (var attribute in targetAssembly.GetAttributesReflectively<AutoHeartbeatAttribute>())
			{
				var counter = registry.Get<AutoCounterRegistration>(attribute.UniqueName);

				if (counter != null)
				{
					if (!attribute.IsReadOnly && attribute.HeartbeatIntervalInMs > 0)
					{
						registry.Heartbeats.Add(
							new AutoHeartbeatRegistration(counter, attribute.HeartbeatIntervalInMs));
					}
				}
				else
				{
					throw new AutoCounterException(string.Format("A heartbeat installation was requested " + 
						"for counter \"{0}\" that does not exist in assembly {1}.",
						 attribute.UniqueName, targetAssembly.FullName));                    
				}
			}
		}


		/// <summary>
		/// Loads a single auto counter registration into the registry
		/// </summary>
		/// <param name="attribute">The attribute to load</param>
		/// <param name="targetAssembly">The assembly to load from</param>
		/// <param name="registry">The registry of auto counters</param>
		/// <param name="createFailedAction">Action to take if counter does not exist.</param>
		/// <param name="interval">Time in milliseconds between flushes of non-locking
		/// auto counters.  A value of null indicates that the counters should use locks instead.
		/// </param>
		/// <returns>A registration for the attribute</returns>
		private static AutoCounterRegistration LoadCounterIntoRegistry(AutoCounterAttribute attribute,
			Assembly targetAssembly, AutoCounterRegistry registry, CreateFailedAction createFailedAction,
			NonLockingAutoCountersAttribute interval)
		{
			AutoCounterRegistration counter = null;
 
			var category = registry.Get<AutoCounterCategoryRegistration>(attribute.Category);

			// the category must already exist, and the counter name must be unique
			if (category != null)
			{
				counter = new AutoCounterRegistration(category.InstanceType, attribute.UniqueName, 
																		createFailedAction, interval)
							  {
								  Category = category,
								  Name = attribute.Name,
								  Description = attribute.Description,
								  Type = attribute.AutoCounterType,
								  IsReadOnly = attribute.IsReadOnly,
							  };

				RegisterUniqueName(registry, counter);
				registry.Counters[attribute.UniqueName] = counter;
				category.AutoCounters[attribute.UniqueName] = counter;
			}
			else
			{
				throw new AutoCounterException(string.Format("A counter installation was requested for " +
					 "category \"{0}\" that does not exist in assembly {1}.",
					 attribute.Category, targetAssembly.FullName));
			}

			return counter;
		}


		/// <summary>
		/// Loads the categories from the assembly into the registry
		/// </summary>
		/// <param name="targetAssembly">The assembly to load from</param>
		/// <param name="registry">The registry of auto counters</param>
		private static void LoadCategoriesIntoRegistry(Assembly targetAssembly, AutoCounterRegistry registry)
		{
			// get all categories from the assembly and register them, ensuring the names are unique
			foreach (var attribute in targetAssembly.GetAttributesReflectively<AutoCounterCategoryAttribute>())
			{
				var category = new AutoCounterCategoryRegistration(attribute.InstanceType, attribute.Name)
								   {
									   Description = attribute.Description
								   };

				RegisterUniqueName(registry, category);
				registry.Categories[attribute.Name] = category;
			}
		}


		/// <summary>
		/// add a unique name to the unique name registry
		/// </summary>
		/// <param name="registry">The registry of auto counters</param>
		/// <param name="registration">The registration to register</param>
		private static void RegisterUniqueName(AutoCounterRegistry registry, 
			ICounterRegistration registration)
		{
			if (!registry.UniqueNames.ContainsKey(registration.UniqueName))
			{
				registry.UniqueNames.Add(registration.UniqueName, registration);    
			}
			else
			{
				throw new AutoCounterException(
					string.Format("The unique name {0} already exists and cannot be duplicated.", 
						registration.UniqueName));
			}
		}
	}
}
