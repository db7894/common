using System;
using System.Runtime.ExceptionServices;
using System.Security.Permissions;
using log4net;

namespace ArmTrap
{
	/// <summary>
	/// Class to catch unhandled exceptions and act on it.
	/// NOTE 1: Add 'legacyCorruptedStateExceptionsPolicy=true' to config file to handle
	/// NOTE  corrupted state exceptions (see HandleProcessCorruptedStateExceptionsAttribute Class).
	/// NOTE 2: Does not currently support WinForms (need Forms.Application.Exit and Application.ThreadException)
	/// </summary>
	public class UnhandledExceptions : IDisposable
	{
		private bool _disposed = false;
		private bool _enabled = false;
		private readonly ILog _log;
		private readonly FeatureFlags _featureFlags;
		private readonly DumpType _dumpType;

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if ( value != _enabled )
				{
					HandlerSetup(value);
				}
			}
		}

		public UnhandledExceptions ( FeatureFlags featureFlags , ILog log )
		{
			_featureFlags = featureFlags;
			_log = log;

			if ( ( _featureFlags & FeatureFlags.Logging ) == FeatureFlags.Logging && _log == null )
			{
				throw new ArgumentNullException("log", "Logging feature is enabled with no log provided");
			}
			if ( ( _featureFlags & FeatureFlags.Dump ) == FeatureFlags.Dump )
			{
				throw new ArgumentException("Dump feature is enabled with no DumpType provided", "featureFlags");
			}
		}

		public UnhandledExceptions ( FeatureFlags featureFlags , DumpType dumpType )
		{
			_featureFlags = featureFlags;
			_dumpType = dumpType;
			if ( ( _featureFlags & FeatureFlags.Logging ) == FeatureFlags.Logging )
			{
				throw new ArgumentException("Logging feature is enabled with no log provided", "featureFlags");
			}
			if ( ( _featureFlags & FeatureFlags.Dump ) == FeatureFlags.Dump && _dumpType == DumpType.None )
			{
				throw new ArgumentException("Dump feature is enabled with DumpType.None provided", "dumpType");
			}
		}

		public UnhandledExceptions ( FeatureFlags featureFlags , DumpType dumpType , ILog log )
		{
			_featureFlags = featureFlags;
			_dumpType = dumpType;
			_log = log;
			if ( ( _featureFlags & FeatureFlags.Logging ) == FeatureFlags.Logging && _log == null )
			{
				throw new ArgumentNullException("log", "Logging feature is enabled with null log provided");
			}
			if ( ( _featureFlags & FeatureFlags.Dump ) == FeatureFlags.Dump && _dumpType == DumpType.None )
			{
				throw new ArgumentException("Dump feature is enabled with DumpType.None provided", "dumpType");
			}
		}

		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)
		,HandleProcessCorruptedStateExceptions()]
		private void HandlerSetup ( bool enabled )
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			if ( enabled )
			{
				currentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
			}
			else
			{
				currentDomain.UnhandledException -= new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
			}
			_enabled = enabled;
		}

		private void CurrentDomainUnhandledException ( object sender, UnhandledExceptionEventArgs e )
		{
			if ( (_featureFlags & FeatureFlags.Logging) == FeatureFlags.Logging )
			{
				_log.Error("Unhandled Exception caught in ArmTrap.", (Exception)e.ExceptionObject);
			}
			if ( ( _featureFlags & FeatureFlags.Dump ) == FeatureFlags.Dump )
			{
				// TODO
				switch ( _dumpType )
				{
					case DumpType.Full:
						break;
					case DumpType.Mini:
						break;
					default:
						break;
				}
			}
			if ( !e.IsTerminating && ( _featureFlags & FeatureFlags.ForceTermination ) == FeatureFlags.ForceTermination )
			{
				ForceExit();
			}
		}

		private static void ForceExit ()
		{
			Environment.Exit(4);
		}

		#region IDisposable Members

		public void Dispose ()
		{
			if ( !_disposed )
			{
				_disposed = true;
				Enabled = false;
			}
		}

		#endregion
	}
}
