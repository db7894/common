using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace SharedAssemblies.Core.Text
{
	/// <summary>
	/// Reads a data source line by line. The source can be a file, a stream,
	/// or a text reader. In any case, the source is only opened when the
	/// enumerator is fetched, and is closed when the iterator is disposed.
	/// </summary>
	public sealed class LineReader : IEnumerable<string>
	{
		/// <summary>
		/// Means of creating a TextReader to read from.
		/// </summary>
		private readonly Func<TextReader> _dataSource;

		/// <summary>
		/// A collection of web based resources
		/// </summary>
		private static readonly List<string> _webPrefixes = new List<string>
		{
			@"http:", @"https:", @"ftp:", @"ftps:", @"file:",
		};

		/// <summary>
		/// A collection of web based resources
		/// </summary>
		private static readonly List<string> _socketPrefixes = new List<string>
		{
			@"tcp:", @"udp:",
		};

		/// <summary>
		/// Creates a LineReader from a socket source.
		/// </summary>
		/// <param name="socket">The data source</param>
		public LineReader(Socket socket)
			: this(new NetworkStream(socket))
		{
		}

		/// <summary>
		/// Creates a LineReader from a socket source. The delegate is only
		/// called when the enumerator is fetched.
		/// </summary>
		/// <param name="socket">The data source</param>
		public LineReader(Func<Socket> socket)
			: this(() => new NetworkStream(socket()))
		{
		}

		/// <summary>
		/// Creates a LineReader from a stream source. UTF-8 is used to decode
		/// the stream into text.
		/// </summary>
		/// <param name="stream">The data source</param>
		public LineReader(Stream stream)
			: this(stream, Encoding.UTF8)
		{
		}

		/// <summary>
		/// Creates a LineReader from a stream source.
		/// </summary>
		/// <param name="stream">The data source</param>
		/// <param name="encoding">Encoding to use to decode the stream into text</param>
		public LineReader(Stream stream, Encoding encoding)
			: this(() => new StreamReader(stream, encoding))
		{
		}

		/// <summary>
		/// Creates a LineReader from a stream source. The delegate is only
		/// called when the enumerator is fetched. UTF-8 is used to decode
		/// the stream into text.
		/// </summary>
		/// <param name="stream">Data source</param>
		public LineReader(Func<Stream> stream)
			: this(stream, Encoding.UTF8)
		{
		}

		/// <summary>
		/// Creates a LineReader from a stream source. The delegate is only
		/// called when the enumerator is fetched.
		/// </summary>
		/// <param name="stream">The data source</param>
		/// <param name="encoding">Encoding to use to decode the stream into text</param>
		public LineReader(Func<Stream> stream, Encoding encoding)
			: this(() => new StreamReader(stream(), encoding))
		{
		}

		/// <summary>
		/// Creates a LineReader from a filename. The file is only opened
		/// (or even checked for existence) when the enumerator is fetched.
		/// UTF8 is used to decode the file into text.
		/// </summary>
		/// <param name="filename">File to read from</param>
		public LineReader(string filename)
			: this(filename, Encoding.UTF8)
		{
		}

		/// <summary>
		/// Creates a LineReader from a filename. The file is only opened
		/// (or even checked for existence) when the enumerator is fetched.
		/// </summary>
		/// <param name="filename">File to read from</param>
		/// <param name="encoding">Encoding to use to decode the file into text</param>
		public LineReader(string filename, Encoding encoding)
			: this(() => BuildUriStream(filename, encoding))
		{
		}

		/// <summary>
		/// Creates a LineReader from a string source. The delegate is only
		/// called when the enumerator is fetched.
		/// </summary>
		/// <param name="stream">The data source</param>
		public LineReader(Func<string> stream)
			: this(() => new StringReader(stream()))
		{
		}

		/// <summary>
		/// Creates a LineReader from a TextReader.
		/// </summary>
		/// <param name="stream">The data source</param>
		public LineReader(TextReader stream)
			: this(() => stream)
		{
		}

		/// <summary>
		/// Creates a LineReader from a TextReader source. The delegate
		/// is only called when the enumerator is fetched
		/// </summary>
		/// <param name="stream">The data source</param>
		public LineReader(Func<TextReader> stream)
		{
			_dataSource = stream;
		}

		/// <summary>
		/// Enumerates the data source line by line.
		/// </summary>
		/// <returns>An generator for the lines coming out of the resource</returns>
		public IEnumerator<string> GetEnumerator()
		{
			using (TextReader reader = _dataSource())
			{
				string line;

				while ((line = reader.ReadLine()) != null)
				{
					yield return line;
				}
			}
		}

		/// <summary>
		/// Enumerates the data source line by line.
		/// </summary>
		/// <returns>An generator for the lines coming out of the resource</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Helper method to build the correct stream reader around
		/// the specified URI.
		/// </summary>
		/// <param name="uri">The uri to create a stream for</param>
		/// <param name="encoding">The encoding to use with the stream</param>
		/// <returns>A populated stream reader</returns>
		private static StreamReader BuildUriStream(string uri, Encoding encoding)
		{
			StreamReader result = null;

			if (_webPrefixes.Any(prefix =>
				uri.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)))
			{
				result = new StreamReader(new WebClient().OpenRead(uri), encoding);
			}
			else if (_socketPrefixes.Any(prefix =>
				uri.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)))
			{
				result = BuildSocketStream(uri, encoding);
			}
			else
			{
				result = new StreamReader(uri, encoding);
			}

			return result;
		}

		/// <summary>
		/// Helper method to build the correct stream reader around a tcp or
		/// udp uri (ex: tcp://10.0.0.2:21).
		/// </summary>
		/// <param name="uri">The uri to create a stream for</param>
		/// <param name="encoding">The encoding to use with the stream</param>
		/// <returns>A populated stream reader</returns>
		private static StreamReader BuildSocketStream(string uri, Encoding encoding)
		{
			NetworkStream stream = null;
			var pieces = uri.Split(':');

			if (pieces[0].Equals("tcp", StringComparison.CurrentCultureIgnoreCase))
			{
				var client = new TcpClient(pieces[1].Substring(2), int.Parse(pieces[2]));
				stream = client.GetStream();
			}
			else if (pieces[0].Equals("udp", StringComparison.CurrentCultureIgnoreCase))
			{
				throw new NotSupportedException("UDP streams are not available");
			}

			return new StreamReader(stream, encoding);
		}
	}
}

