using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileSplit
{
	public class CalculateFilesWorker
	{
		public class WorkerArgs
		{
			public string InputFile { get; private set; }
			public bool HasHeaders { get; private set; }
			public int NumberOfLinesPerFile { get; private set; }
			public WorkerArgs ( string inputFile, bool hasHeaders , int numberOfLinesPerFile )
			{
				InputFile = inputFile;
				HasHeaders = hasHeaders;
				NumberOfLinesPerFile = numberOfLinesPerFile;
			}
		}
		public class WorkerReturn
		{
			public int NumberOfFiles { get; set; }
			public int NumberOfInputLines { get; set; }
		}

		public WorkerReturn NumberOfFiles ( WorkerArgs args, BackgroundWorker worker )
		{
			long bytes = 0;
			long byteCount = 0;

			var retVal = new WorkerReturn();
			StreamReader reader = null;
			try
			{
				var fi = new FileInfo(args.InputFile);
				bytes = fi.Length;
				reader = fi.OpenText();
				if ( args.HasHeaders )
				{
					byteCount += reader.ReadLine().Length;
				}
				while ( !worker.CancellationPending && !reader.EndOfStream )
				{
					byteCount += reader.ReadLine().Length;
					retVal.NumberOfInputLines++;
					if ( retVal.NumberOfInputLines % 10000 == 0 )
					{
						double percent = byteCount * 100.0 / bytes;
						worker.ReportProgress((int)percent);
					}
				}
			}
			finally
			{
				if ( reader != null )
				{
					reader.Close();
					reader.Dispose();
				}
				retVal.NumberOfFiles = (int)Math.Ceiling((double)retVal.NumberOfInputLines/args.NumberOfLinesPerFile);
			}
			return retVal;
		}

	}
}
