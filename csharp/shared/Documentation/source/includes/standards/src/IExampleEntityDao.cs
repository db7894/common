
using System.Collections.Generic;
using SharedAssemblies.Entities.ExampleDomain;

namespace SharedAssemblies.DataAccess.ExampleDomain
{
	/// <summary>
	/// Interface for a some entity data access object
	/// </summary>
	public interface IExampleEntityDao
	{
		#region Examples of a CRUD DAO

		/// <summary>
		/// Retrieve an <see cref="ExampleEntity"> associated with the identifier
		/// </summary>
		/// <param name="identifier">The id associated with the entity</param>
		/// <returns>A hydrated <see cref="ExampleEntity"></returns>       
		ExampleEntity GetEntityById(string identifier);

		/// <summary>
		/// Get all <see cref="ExampleEntity"> associated with a given branch
		/// </summary>
		/// <param name="branchCode">The branch code to retrieve entities from</param>
		/// <returns>A collection of <see cref="ExampleEntity"></returns>       
		List<ExampleEntity> GetExampleEntityByBranch(string branchCode);

		/// <summary>
		/// Insert the <see cref="ExampleEntity"> into the database
		/// </summary>
		/// <param name="entity">The entity to insert</param>
		/// <returns>A unique id to reference the <see cref="ExampleEntity"></returns>       
		string Insert(ExampleEntity entity);

		/// <summary>
		/// Update the requested <see cref="ExampleEntity">
		/// </summary>
		/// <param name="entity">The entity to update</param>
		/// <returns>true if the operation succeeded, false otherwise</returns>       
		bool Update(ExampleEntity entity);

		/// <summary>
		/// Remove the requested <see cref="ExampleEntity">
		/// </summary>
		/// <param name="entity">The entity to delete</param>
		/// <returns>true if the operation succeeded, false otherwise</returns>       
		bool Delete(ExampleEntity entity);

		#endregion

		#region Examples of Other Database Utility Methods

		/// <summary>
		/// Check the database for the current status of a system variable
		/// </summary>
		/// <param name="system">The system to get the status of</param>
		/// <returns>true if the flush can proceed, false otherwise</returns>
		bool GetSystemStatus(string system);

		/// <summary>
		/// Set the current status in the database of a system variable
		/// </summary>
		/// <param name="system">The system to get the status of</param>
		/// <param name="status">The value to set the flag to</param>
		/// <returns>The result of the operation</returns>       
		bool SetSystemStatus(string system, bool status);

		#endregion
	}
}

