using System.ServiceModel;
using System.ServiceModel.Web;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health
{
	/// <summary>
	/// A service contract that can be used to retrieve the current status
	/// of a given application.
	/// </summary>
	[ServiceContract(Namespace = "http://service.bashwork.com/8/2010/", Name = "IHealthServiceContract")]
	public interface IHealthServiceContract
	{
		/// <summary>
		/// Get the current status of the application and its dependencies.
		/// </summary>
		/// <returns>The result of the operation</returns>
		[OperationContract(Name = "IsSystemHealthy", IsOneWay = false)]
		[WebInvoke(Method = "GET")]
		bool IsSystemHealthy();

		/// <summary>
		/// Retrieve the current health of the application and its dependencies.
		/// </summary>
		/// <returns>The result of the operation</returns>
		[OperationContract(Name = "GetApplicationHealth", IsOneWay = false)]
		[WebInvoke(Method = "GET")]
		GetApplicationHealthResponse GetApplicationHealth();
	}
}

