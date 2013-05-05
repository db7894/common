using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using log4net;

namespace InstrumentationLib
{
	/// <summary>
	/// Abstraction of a memory mapped file
	/// </summary>
	public class MemoryFile<T> : IPostProcessor<T> where T : IOutputRecord
	{
		/// <summary>
		/// The logger
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(typeof(MemoryFile<T>));

		/// <summary>
		/// The memory mapped file.
		/// </summary>
		private MemoryMappedFile _mmFile;

		/// <summary>
		/// The current memory view of the file.
		/// </summary>
		private MemoryMappedViewStream _mmStream;

		/// <summary>
		/// Keeps the current view position to determine when to create the next view.
		/// </summary>
		private long _curViewPosition;

		/// <summary>
		/// The current view number.
		/// </summary>
		private long _currentView;

		/// <summary>
		/// The current physical file position to determine when full.
		/// </summary>
		private long _curFilePosition;

		/// <summary>
		/// View size obtained through the constructor.
		/// </summary>
		private readonly long _memoryViewSize;

		/// <summary>
		/// Physical file size obtained through the constructor.
		/// </summary>
		private readonly long _mappedFileSize;

		/// <summary>
		/// The system page size.
		/// </summary>
		private static int _pageSize = Environment.SystemPageSize;

		/// <summary>
		/// The full physical file path.
		/// </summary>
		public string FilePath { get; private set; }

		/// <summary>
		/// Constructor for the memory mapped file abstraction.
		/// </summary>
		/// <param name="filePath">The full physical file path.</param>
		/// <param name="mappedFileSize">The physical file size.</param>
		/// <param name="memoryViewSize">The memory view size.</param>
		public MemoryFile(string filePath, long mappedFileSize, long memoryViewSize)
		{
			if (mappedFileSize < _pageSize)
			{
				throw new ArgumentException(string.Format("File size s/b >= {0}", _pageSize), "mappedFileSize");
			}
			if (memoryViewSize < _pageSize)
			{
				throw new ArgumentException(string.Format("View size s/b >= {0}", _pageSize), "memoryViewSize");
			}
			if (memoryViewSize % _pageSize != 0)
			{
				throw new ArgumentException(string.Format("View size must be a multiple of {0}", _pageSize), "memoryViewSize");
			}
			if (mappedFileSize < memoryViewSize ||
				 mappedFileSize % memoryViewSize != 0)
			{
				throw new ArgumentException("Mapped file size must be a multiple of memory view size");
			}
			FilePath = filePath;
			_mappedFileSize = mappedFileSize;
			_memoryViewSize = memoryViewSize;
		}

		/// <summary>
		/// Creates the mm file and accessor
		/// </summary>
		/// <returns>True on success</returns>
		public bool Open()
		{
			bool retVal = false;
			try
			{
				_curFilePosition = 0;
				_currentView = 0;
				FileStream stream = File.Create(FilePath);
				stream.Close();
				_mmFile = MemoryMappedFile.CreateFromFile(FilePath, FileMode.Open, "StreamingServiceInstrMM", _mappedFileSize);

				CreateAccessor();

				_log.InfoFormat("Created MMFile \'{0}\', file size = {1}, view size = {2}", FilePath, _mappedFileSize, _memoryViewSize);
				retVal = true;
			}
			catch (Exception ex)
			{
				_log.ErrorFormat("Exception creating Memmory mapped file/view: {0}", ex.Message);
			}
			return retVal;
		}

		/// <summary>
		/// Writes to the memory view.
		/// </summary>
		/// <param name="rec">The IOutputRec to write.</param>
		/// <returns>true on success</returns>
		public bool Write(T rec)
		{
			bool retVal = true;
			if (_curFilePosition + rec.Length > _mappedFileSize)
			{
				retVal = false;
			}
			else
			{
				int length = rec.Length;
				int offset = 0;
				byte[] bytes = rec.ToByteArray();
				try
				{
					if (_curViewPosition + length > _memoryViewSize)
					{
						// write what you can
						var writeLength = (int)(_memoryViewSize - _curViewPosition);
						if (writeLength > 0)
						{
							_mmStream.Write(bytes, offset, writeLength);
							_curViewPosition += writeLength;
							_curFilePosition += writeLength;
						}
						// create the new view
						CreateAccessor();
						// write the balance
						offset += writeLength;
						length -= writeLength;
						_mmStream.Write(bytes, offset, length);
						_curViewPosition += length;
						_curFilePosition += length;
					}
					else
					{
						_mmStream.Write(bytes, offset, length);
						_curViewPosition += length;
						_curFilePosition += length;
					}
				}
				catch (Exception ex)
				{
					// don't want the app to crash on instrumentations dime
					_log.Error("Exception writing to Memory Mapped File", ex);
					retVal = false;
				}
			}
			return retVal;
		}

		/// <summary>
		/// Disposes the accessor and file.
		/// </summary>
		public void Close()
		{
			try
			{
				// We're going to sleep for 2 seconds to avoid syncronization.
				// Token Service is responsible to stop writting before operating on Close.
				System.Threading.Thread.Sleep(1000 * 2);
				if (_mmFile != null)
				{
					if (_mmStream != null)
					{
						_mmStream.Flush();
						_mmStream.Dispose();
						_mmStream = null;
					}
					_mmFile.Dispose();
					_mmFile = null;
				}
			}
			catch (Exception ex)
			{
				_log.ErrorFormat("Exception disposing/closing Memmory mapped file/view: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Creates the memory view.
		/// </summary>
		private void CreateAccessor()
		{
			if (_mmStream != null)
			{
				_mmStream.Flush();
				_mmStream.Dispose();
			}
			_mmStream = _mmFile.CreateViewStream(_currentView * _memoryViewSize, _memoryViewSize);
			_curViewPosition = 0;
			++_currentView;
		}

	}
}
