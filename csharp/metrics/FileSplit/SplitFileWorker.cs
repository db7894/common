using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileSplit
{
	public class SplitFileWorker
	{
		public class WorkerArgs
		{
			public string InputFile { get; private set; }
			public string OutputFile { get; private set; }
			public int LinesPerFile { get; private set; }
			public int NumberOfFiles { get; private set; }
			public long TotalLines { get; private set; }
			public bool FirstLineIsHeaders { get; private set; }
			public WorkerArgs ( string inputFile, string outputFile,
								int linesPerFile, int numberOfFiles,
								long totalLines, bool firstLineIsHeaders )
			{
				InputFile = inputFile;
				OutputFile = outputFile;
				LinesPerFile = linesPerFile;
				NumberOfFiles = numberOfFiles;
				TotalLines = totalLines;
				FirstLineIsHeaders = firstLineIsHeaders;
			}
		}

		public int SplitFile ( WorkerArgs args, BackgroundWorker worker )
		{
			StreamReader reader = null;
			int result = 0;
			int lineCount = 0;
			try
			{
				reader = File.OpenText(args.InputFile);
				StreamWriter writer = null;
				string openedFileName = null;
				string headers = null;
				while ( !worker.CancellationPending && !reader.EndOfStream )
				{
					if ( lineCount % args.LinesPerFile == 0 )
					{
						CloseOutputFile(writer);
						writer = OpenOutputFile(args.OutputFile, ++result, out openedFileName);
						if ( headers != null )
						{
							writer.WriteLine(headers);
						}
					}
					if ( lineCount % 10000 == 0 )
					{
						double percentage = lineCount * 100.0 / args.TotalLines;
						worker.ReportProgress((int)percentage);
					}
					var line = reader.ReadLine();
					if ( lineCount == 0 )
					{
						if ( args.FirstLineIsHeaders )
						{
							headers = line;
						}
					}
					writer.WriteLine(line);
					lineCount++;
				}
				if ( writer != null )
				{
					CloseOutputFile(writer);
					if ( lineCount == 0 && openedFileName != null )
					{
						File.Delete(openedFileName);
					}
				}
			}
			catch
			{
				result = 0;
			}
			finally
			{
				if ( reader != null )
				{
					reader.Close();
					reader.Dispose();
				}
			}
			return result;
		}

		private StreamWriter OpenOutputFile ( string filename, int fileNum, out string openedFileName )
		{
			StreamWriter writer = null;
			openedFileName = string.Format("{0}\\{1}_{2}{3}",
										Path.GetDirectoryName(filename),
										Path.GetFileNameWithoutExtension(filename),
										fileNum.ToString(),
										Path.GetExtension(filename));
			writer = File.CreateText(openedFileName);
			return writer;
		}

		private void CloseOutputFile ( StreamWriter writer )
		{
			if ( writer != null )
			{
				writer.Flush();
				writer.Close();
				writer.Dispose();
			}
		}

	}
}
