using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace MessagingManagementService.DataAccessLayer.VirusScanner.Helpers
{
	/// <summary>
	/// A collection of methods to simplify using rest web services
	/// </summary>
	public class HttpClient : IHttpClient
	{
		/// <summary>
		/// Perform an HTTP GET on the supplied resource
		/// </summary>
		/// <param name="uri">The resource to perform a GET on</param>
		/// <returns>The result of the operation</returns>
		public string Get(string uri)
		{
			var request = WebRequest.Create(uri);
			request.Method = "GET";

			using (var stream = request.GetResponse().GetResponseStream())
			using (var reader = new StreamReader(stream))
			{
				var content = reader.ReadToEnd();
				return content;
			}
		}

		/// <summary>
		/// Perform an HTTP POST on the supplied resource
		/// </summary>
		/// <param name="uri">The resource to perform a POST on</param>
		/// <param name="data">The form data to post to the service</param>
		/// <returns>The response from the web service</returns>
		public string Post(string uri, IDictionary<string, string> data)
		{
			var parameters = data.Aggregate(new StringBuilder(), (builder, pair) =>
				builder.AppendFormat("{0}={1}&", pair.Key, WebUtility.HtmlEncode(pair.Value)));
			var values = parameters.ToString(0, parameters.Length - 1);

			var request = WebRequest.Create(uri);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = values.Length;

			using (var stream = request.GetRequestStream())
			using (var writer = new StreamWriter(stream))
			{
				writer.Write(values);
			}

			using (var stream = request.GetResponse().GetResponseStream())
			using (var reader = new StreamReader(stream))
			{
				var content = reader.ReadToEnd();
				return content;
			}
		}

		/// <summary>
		/// Perform an HTTP POST on the supplied resource
		/// </summary>
		/// <param name="uri">The resource to perform a POST on</param>
		/// <param name="data">The binary data to post to the service</param>
		/// <returns>The response from the web service</returns>
		public string Upload(string uri, byte[] data)
		{
			var request = WebRequest.Create(uri);
			request.Method = "POST";

			using (var stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
				stream.Flush();
			}

			using (var stream = request.GetResponse().GetResponseStream())
			using (var reader = new StreamReader(stream))
			{
				var content = reader.ReadToEnd();
				return content;
			}
		}
	}
}
