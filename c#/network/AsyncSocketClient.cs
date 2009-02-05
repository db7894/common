// SocketClient.cs created with MonoDevelop
// User: lansing at 7:10 PM 2/4/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

//using NLog;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Xml.Serialization; 

namespace Common.Networking
{
	public class SocketClient
	{
		private string _hostName;
		private Int32 _portNumber;
		//private static Logger logger = CommunicationLogManager.Instance.GetLogger("VIM.Communication.SocketClient");
	
		public SocketClient(string hostName, Int32 portNumber)
		{
			_hostName = hostName;
			_port = portNumber;
		}
	
		/// <summary>
		/// Sends a communication block asynchronously
		/// </summary>
		/// <param name="commBlock">communication block to send</param>
		public void Send(String buffer)
		{
			//logger.Trace("Sending data async.");
			try {
				if (String.IsNullOrEmpty(_hostName))
					throw new ApplicationException("Config Entry 'hostName' is missing or invalid");
	
				if (_portNumber == 0)
					throw new ApplicationException("Config Entry 'hostPortNumber' is missing or invalid");
	
				IPHostEntry host = Dns.GetHostEntry(_hostName);
				IPAddress[] addressList = host.AddressList;
	
				IPEndPoint hostEndPoint = new IPEndPoint(
					addressList[addressList.Length - 1], _portNumber);
				Socket clientSocket = new Socket(
					hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				SendSocketState sss = new SendSocketState(clientSocket, buffer);
				
				SocketAsyncEventArgs asyncSockectArgs = new SocketAsyncEventArgs();
				asyncSockectArgs.UserToken = sss;
				asyncSockectArgs.RemoteEndPoint = hostEndPoint;
				asyncSockectArgs.DisconnectReuseSocket = true;
				asyncSockectArgs.Completed +=
					new EventHandler<SocketAsyncEventArgs>(OnConnect);
	
				Boolean willRaiseEvent = sss.TheSocket.ConnectAsync(asyncSockectArgs);
				if (!willRaiseEvent)
					ProcessSendMessage(asyncSockectArgs);
				//logger.Trace(formatProvider, asyncSockectArgs);
			}
			catch (Exception ex) {
			    //logger.ErrorException("SocketClient.SendDataAsync", ex);
			}
		}
	
		/// <summary>
		/// On connect event 
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">SocketAsyncEventArgs</param>
		private void OnConnect(object sender, SocketAsyncEventArgs e)
		{
			//logger.Trace(formatProvider, e);
			if (e.SocketError == SocketError.Success)
				ProcessSendMessage(e);
			else {
				e.Completed -= new EventHandler<SocketAsyncEventArgs>(OnConnect);
				//string action = string.Format(" connecting to {0} at port {1}", hostName, zPortNumber);
				//LogError(action, e);
				ProcessError(e);
			}
		}
	
		/// <summary>
		/// Processes the message to send
		/// </summary>
		/// <param name="e">SocketAsyncEventArgs</param>
		private void ProcessSendMessage(SocketAsyncEventArgs e)
		{
			//logger.Trace("Processing send message.");
			//logger.Trace(formatProvider, e);
			e.Completed -= new EventHandler<SocketAsyncEventArgs>(OnConnect);
			e.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendComplete);
			
			SendSocketState sss = (SendSocketState)e.UserToken;
			try {
			    Byte[] sendBuffer = Encoding.UTF8.GetBytes(SerializeMessage(sss.TheMessage));
			    e.SetBuffer(sendBuffer, 0, sendBuffer.Length);
			    Socket s = sss.TheSocket;
			    Boolean willRaiseEvent = s.SendAsync(e);
			    if (!willRaiseEvent)
			        ProcessSendIsComplete(e);
			}
			catch (Exception ex) {
			    //logger.ErrorException("SocketClient.ProcessSendMessage", ex);
			}
			//logger.Trace("Finished processing send message.");
		}
	
		/// <summary>
		/// Serializes the communication block containing the message
		/// </summary>
		/// <param name="obj">CommunicationBlock</param>
		/// <returns>string</returns>
		private string SerializeMessage(String obj)
		{
		    //logger.Trace("Serializing communication block message.");
		    //logger.Trace(formatProvider, obj);
		    try {
				//using (MemoryStream stream = new MemoryStream()) {
				//    obj.GenericStuffer = STUFFER;
				//    objSerializer.Serialize(stream, obj);
				//	//logger.Trace("Finished serializng the message.");
				//	return stream
				//}
				return obj;
			}
			catch (Exception ex) {
				//logger.ErrorException("SocketClient.SerializeMessage", ex);
			}
			//finally {
			//}
			return null;
		}
	
		/// <summary>
		/// On send complete event
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">SocketAsyncEventArgs</param>
		private void OnSendComplete(object sender, SocketAsyncEventArgs e)
		{
		    //logger.Trace("Send complete.");
		    //logger.Trace(formatProvider, e);
		    if (e.SocketError == SocketError.Success)
		        ProcessSendIsComplete(e);
		    else {
		        e.Completed -= new EventHandler<SocketAsyncEventArgs>(OnSendComplete);
		        //string action = string.Format(" sending data to {0} at port {1}", hostName, zPortNumber);
		        //LogError(action, e);
		        ProcessError(e);
		    }
		    //logger.Trace("Finished send complete.");
		}
	
		/// <summary>
		/// Processes the completed send
		/// </summary>
		/// <param name="e">SocketAsyncEventArgs</param>
		private void ProcessSendIsComplete(SocketAsyncEventArgs e)
		{
			//logger.Trace("Processing send complete.");
			try {
				if (e.LastOperation == SocketAsyncOperation.Send) {
					SendSocketState sss = (SendSocketState)e.UserToken;
					Socket s = sss.TheSocket;
					e.Completed -= new EventHandler<SocketAsyncEventArgs>(OnSendComplete);
					s.Shutdown(SocketShutdown.Both);
					s.Close();
					(s as IDisposable).Dispose();
					s = null;
					sss.CleanUp();
					e.Dispose();
				}
			}
			catch (Exception ex) {
				//logger.ErrorException("SocketClient.ProcessSendIsComplete", ex);
			}
			//logger.Trace("Finished processing send complete.");
		}
	
		/// <summary>
		/// On disconnect event
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">SocketAsyncEventArgs</param>
		private void OnDisconnectComplete(object sender, SocketAsyncEventArgs e)
		{
			//logger.Trace("On disconnect complete.");
			//logger.Trace(formatProvider, e);
			if (e.SocketError == SocketError.Success)
				ProcessDisconnectIsComplete(e);
			else
				ProcessError(e);
			//logger.Trace("Finished on disconnect complete.");
		}
	
		/// <summary>
		/// Processes the completed disconnect
		/// </summary>
		/// <param name="e">SocketAsyncEventArgs</param>
		private void ProcessDisconnectIsComplete(SocketAsyncEventArgs e)
		{
			if (e.LastOperation == SocketAsyncOperation.Disconnect)
				e.Dispose();
		}
		
		/// <summary>
		/// Processes errors
		/// </summary>
		/// <param name="e">SocketAsyncEventArgs</param>
		private void ProcessError(SocketAsyncEventArgs e)
		{
		    //logger.Trace("Processing error.");
		    //logger.Trace(formatProvider, e);
		    SendSocketState sss = (SendSocketState)e.UserToken;
		    Socket s = sss.TheSocket as Socket;
		    if (s != null) {
		        try {
		            if (s.Connected) {
		                s.Shutdown(SocketShutdown.Both);
		                s.Close();
		            }
		        }
		        catch(Exception ex) {
		            //logger.ErrorException("SocketClient.ProcessError", ex);
		        }
		        finally {
		            (s as IDisposable).Dispose();
		            s = null;
		        }
		    }
		    sss.CleanUp();
		    e.Dispose();
		    //logger.Trace("Finished processing error.");
		}
	
		private static void LogError(string action, SocketAsyncEventArgs e)
		{
			//logger.Error(String.Format("'{0}' while {1}", e.SocketError.ToString(), action));
		}
	      
		private sealed class SendSocketState
		{
		    private Socket socket = null;
		    private String message = null;            
		
		    /// <summary>
		    /// Constructor
		    /// </summary>
		    /// <param name="socket">Socket</param>
		    /// <param name="message">CommunicationBlock</param>
		    internal SendSocketState(Socket socket, String message)
		    {
		        this.socket = socket;
		        this.message = message;
		    } 
		
		    /// <summary>
		    /// Socket
		    /// </summary>
		    internal Socket TheSocket
		    {
		        get { return socket; }
		    }
		
		    /// <summary>
		    /// Communication block
		    /// </summary>
		    internal String TheMessage
		    {
		        get { return message; }
		    } 
		
		    /// <summary>
		    /// Clears the object
		    /// </summary>
		    internal void CleanUp()
		    {
		        message = null;
		        if (socket != null)
		        {
		            (socket as IDisposable).Dispose();
		            socket = null;
		        }
		    } 
		}
	}
}
