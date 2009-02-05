// Main.cs created with MonoDevelop
// User: lansing at 7:08 PM 2/4/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using System.Text;
using System.Collections.Generic;

namespace Common.Networking
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			byte[] result = new byte[1024];
			string buffer = "This is a test example";
			SyncSocketClient sock = new SyncSocketClient("localhost", 7000);
			
			Console.WriteLine("Starting Test");			
			sock.Open();
			try {
				sock.Send(Encoding.UTF8.GetBytes(buffer));
				int size = sock.Receive(result, result.Length);
				
				Console.WriteLine("Test Result = {0}",
					Encoding.ASCII.GetString(result, 0, size));
			} catch (Exception ex) {
				Console.WriteLine("SocketException : {0}",ex.ToString());
			} finally {
				sock.Close();
				Console.WriteLine("Ending Test");
			}
		}
	}
}
