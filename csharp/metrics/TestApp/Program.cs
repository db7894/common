using System.Configuration;
using InstrumentationLib;

namespace TestApp
{
	class Program
	{
		static void Main ( string[] args )
		{
			int appId = 0;
			int.TryParse(ConfigurationManager.AppSettings.Get("Instrumentation.AppId"), out appId);
			string filePath;
			string temp = ConfigurationManager.AppSettings.Get("Instrumentation.FilePath");
			if ( temp == null )
			{
				filePath = string.Format("Instrumentation_{0}.dat", appId);
			}
			else
			{
				if ( !temp.EndsWith("\\") )
				{
					temp += "\\";
				}
				filePath = string.Format("{0}Instrumentation_{1}.dat", temp, appId);
			}

			int fileSize = 0;
			int.TryParse(ConfigurationManager.AppSettings.Get("Instrumentation.MMFileSize"), out fileSize);
			int viewSize = 0;
			int.TryParse(ConfigurationManager.AppSettings.Get("Instrumentation.MMViewSize"), out viewSize);
			var mmfile = new MemoryFile<ITokenPairRecord>(filePath, fileSize, viewSize);
			var tokenService = new TokenService(1001, mmfile, 0.01);
			tokenService.Enabled = true;
			for ( int index = 0; index < 2000; ++index )
			{
				ITokenPairRecord tokenRecord = tokenService.GetToken(4);
			}
			System.Threading.Thread.Sleep(10000);
			System.Console.WriteLine("Press any key....");
			System.Console.ReadKey();
			tokenService.Enabled = false;
			System.Threading.Thread.Sleep(10000);
			mmfile.Close();
		}
	}
}
