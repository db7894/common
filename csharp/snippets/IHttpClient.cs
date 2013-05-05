using System.Collections.Generic;

namespace MessagingManagementService.DataAccessLayer.VirusScanner.Helpers
{
	/// <summary>
	/// An interface to a http client implementation.
	/// </summary>
	public interface IHttpClient
	{
		/// <summary>
		/// Perform an HTTP GET on the supplied resource
		/// </summary>
		/// <param name="uri">The resource to perform a GET on</param>
		/// <returns>The result of the operation</returns>
		string Get(string uri);

		/// <summary>
		/// Perform an HTTP POST on the supplied resource
		/// </summary>
		/// <param name="uri">The resource to perform a POST on</param>
		/// <param name="data">The form data to post to the service</param>
		/// <returns>The response from the web service</returns>
		string Post(string uri, IDictionary<string, string> data);

		/// <summary>
		/// Perform an HTTP POST on the supplied resource
		/// </summary>
		/// <param name="uri">The resource to perform a POST on</param>
		/// <param name="data">The binary data to post to the service</param>
		/// <returns>The response from the web service</returns>
		string Upload(string uri, byte[] data);
	}
}
