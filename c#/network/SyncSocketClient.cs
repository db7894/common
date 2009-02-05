// SyncSocketClient.cs created with MonoDevelop
// User: lansing at 7:41 PM 2/4/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Async
{
	/// <summary>
	/// Wrapper around windows sync socket
	/// </summary>	
	public class SyncSocketClient
	{
		string _hostName;
		int _portNumber;
		Socket _sender;
	
		/// <summary>
		/// Constructor for a socket client
		/// </summary>
		/// <param name="hostName">The host to connect to</param>
		/// <param name="portNumber">The port to connect on</param>
		public SyncSocketClient(string hostName, int portNumber)
		{
			_hostName = hostName;
			_portNumber = portNumber;
		}
		
		/// <summary>
		/// Wrapper to open a socket
		/// </summary>
		public void Open()
		{
			try {
				IPHostEntry ipHostInfo = Dns.GetHostEntry(_hostName);
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				IPEndPoint remoteEP = new IPEndPoint(ipAddress,_portNumber);

            	_sender = new Socket(AddressFamily.InterNetwork, 
					SocketType.Stream, ProtocolType.Tcp );
				_sender.Connect(remoteEP);
			}
			catch (SocketException se) {
				Console.WriteLine("SocketException : {0}",se.ToString());
			}
		}

		/// <summary>
		/// A wrapper to send data in buffer
		/// </summary>
		/// <param name="buffer">The data to send </param>
		public void Send(byte[] buffer)
		{
			try {
				if (buffer.Length != 0) {
					_sender.Send(buffer);
				}
			}
			catch (SocketException se) {
				Console.WriteLine("SocketException : {0}",se.ToString());
			}
		}
		
		/// <summary>
		/// A wrapper to send data in buffer
		/// </summary>
		/// <param name="buffer">The data to send </param>
		/// <param name="size">The amount of data to send</param>
		public void SendAll(byte[] buffer, int size)
		{
			int result = 0;
			
			do {
				try {
					if ((buffer.Length != 0) && (size != 0)) {
						result += _sender.Send(buffer, result, 
							size - result, SocketFlags.None);
					}
				}
				catch (SocketException se) {
					Console.WriteLine("SocketException : {0}",se.ToString());
				}
			} while (result < size);
		}

		
		/// <summary>
		/// Performs a one shot receive with no guarantees to retrieve size
		/// </summary>
		/// <param name="buffer">The preallocated storage buffer</param>
		/// <param name="size">The max amount to receive</param>
		/// <returns>The number of bytes read</returns>
		public int Receive(byte[] buffer, int size)
		{
			int result = 0;
			try {
				result = _sender.Receive(buffer, size, SocketFlags.None);
			} 
			catch (SocketException se) {
				Console.WriteLine("SocketException : {0}",se.ToString());
			}
			return result;
		}

		/// <summary>
		/// Performs a receive and guarantees to retrieve size
		/// </summary>
		/// <param name="buffer">A preallocated buffer to store results</param>
		/// <param name="size">The size to retrieve</param>
		public void ReceiveAll(byte[] buffer, int size)
		{
			int result = 0;
			
			do {
				try {
					result += _sender.Receive(buffer, result,
						size - result, SocketFlags.None);
				}
				catch (SocketException se) {
					Console.WriteLine("SocketException : {0}",se.ToString());
				}
			} while (result < size);
		}
		
		/// <summary>
		/// A wrapper to shutdown a socket
		/// </summary>
		public void Close()
		{
			try {
				_sender.Shutdown(SocketShutdown.Both);
				_sender.Close();
			}
			catch (SocketException se) {
				Console.WriteLine("SocketException : {0}",se.ToString());
			}

		}
	}
}