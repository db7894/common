using System;
using System.Collections.Generic;
using System.IO;
using log4net.Appender;
using log4net.Util;

namespace SharedAssemblies.General.Logging
{
	/// <summary>
	/// Custom log4net appender which will delete log files older than a configurable
	/// number of days
	/// </summary>
	public class RollingMaxDaysFileAppender : RollingFileAppender
	{
		/// <summary>
		/// Maximum number of days worth of log files to keep.  Defaults to zero
		/// which translates to no file deletions.
		/// </summary>
		/// <remarks>This property name matches appender config item key</remarks>
		public int MaxDaysRollBackups { get; set; } 

		/// <summary>
		/// Initialize the appender
		/// </summary>
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			PurgeOldLogFiles();
		}

		/// <summary>
		/// Delete all log files older than the maximum number of days to keep
		/// </summary>
		private void PurgeOldLogFiles()
		{
			if (MaxDaysRollBackups > 0)  // zero means no old file removal
			{
				using (SecurityContext.Impersonate(this))
				{
					string fullPath = Path.GetFullPath(base.File);

					var existingFiles = GetExistingFiles(fullPath);

					foreach (var file in existingFiles)
					{
						DateTime fileModifyDateTime = System.IO.File.GetLastWriteTime(file);
						if (fileModifyDateTime < DateTime.Now.AddDays(-1 * MaxDaysRollBackups))
						{
							DeleteFile(file);
						}
					}
				}
			}
		}


		/// <summary>
		/// Builds a list of filenames (with full absolute path) for all files matching the 
		/// base filename plus a file pattern.
		/// </summary>
		/// <param name="baseFilePath">full absolute path of base (no date suffix) filename</param>
		/// <returns>list of filenames</returns>
		private List<string> GetExistingFiles(string baseFilePath)
		{
			var existingFileList = new List<string>();

			using (SecurityContext.Impersonate(this))
			{
				string fullPath = Path.GetFullPath(baseFilePath);  // Get absolute path

				string directory = Path.GetDirectoryName(fullPath);
				if (Directory.Exists(directory))
				{
					string baseFileName = Path.GetFileName(fullPath);

					string[] filesWithFullPath = Directory.GetFiles(directory, GetWildcardPatternForFile(baseFileName));

					if (filesWithFullPath != null)
					{
						existingFileList.AddRange(filesWithFullPath);
					}
				}
				// Log to log4net log
				if (LogLog.IsDebugEnabled)
				{
					LogLog.Debug(ToString() + ": Searched for existing files in [" + directory + "]");
				}
			}

			return existingFileList;
		}

		/// <summary>
		/// Generates a wildcard pattern that can be used to find all files
		/// that are similar to the base file name.
		/// </summary>
		/// <param name="baseFileName">base name for all relevant files</param>
		/// <returns>wildcard pattern</returns>
		private static string GetWildcardPatternForFile(string baseFileName)
		{
			return baseFileName + '*';
		}
	}
}
