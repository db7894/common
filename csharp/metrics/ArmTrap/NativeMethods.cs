﻿/****************************** Module Header ******************************\
 * Module Name:  MiniDumpCreator.cs
 * Project:      CSCreateMiniDump
 * Copyright (c) Microsoft Corporation.
 * 
 * This class wraps the extern method MiniDumpWriteDump in dbghelp.dll.
 *  
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ArmTrap
{
	enum MiniDumpType
	{
		MiniDumpWithDataSegs = 0x00000001,
		MiniDumpWithFullMemory = 0x00000002,
		MiniDumpWithHandleData = 0x00000004,
		MiniDumpFilterMemory = 0x00000008,
		MiniDumpScanMemory = 0x00000010,
		MiniDumpWithUnloadedModules = 0x00000020,
		MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
		MiniDumpFilterModulePaths = 0x00000080,
		MiniDumpWithProcessThreadData = 0x00000100,
		MiniDumpWithPrivateReadWriteMemory = 0x00000200,
		MiniDumpWithoutOptionalData = 0x00000400,
		MiniDumpWithFullMemoryInfo = 0x00000800,
		MiniDumpWithThreadInfo = 0x00001000,
		MiniDumpWithCodeSegs = 0x00002000,
		MiniDumpWithoutAuxiliaryState = 0x00004000,
		MiniDumpWithFullAuxiliaryState = 0x00008000,
		MiniDumpValidTypeFlags = 0x0000ffff,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct MiniDumpExceptionInformation
	{
		/// <summary> 
		/// The identifier of the thread throwing the exception. 
		/// </summary> 
		public int ThreadId;

		/// <summary> 
		/// A pointer to an EXCEPTION_POINTERS structure specifying a computer-independent 
		/// description of the exception and the processor context at the time of the  
		/// exception. 
		/// </summary> 
		public IntPtr ExceptionPointers;

		/// <summary> 
		/// Determines where to get the memory regions pointed to by the ExceptionPointers member.  
		/// Set to TRUE if the memory resides in the process being debugged (the target process  
		/// of the debugger).  
		/// Otherwise, set to FALSE if the memory resides in the address space of the calling  
		/// program (the debugger process). If you are accessing local memory (in the calling 
		/// process) you should not set this member to TRUE. 
		/// </summary> 
		[MarshalAs(UnmanagedType.Bool)]
		public bool ClientPointers;
	}

	internal static class NativeMethods
	{
		/// <summary>
		/// Write MiniDump.
		/// </summary>
		/// <param name="hProcess">
		/// A handle to the process for which the information is to be generated.
		/// </param>
		/// <param name="processId">
		/// The identifier of the process for which the information is to be generated.
		/// </param>
		/// <param name="hFile">
		/// A handle to the file in which the information is to be written
		/// </param>
		/// <param name="dumpType">
		/// The type of information to be generated. This parameter can be one or more of
		/// the values from the MiniDumpType enumeration.
		/// </param>
		/// <param name="exceptionParam">
		/// A pointer to a MiniDumpExceptionInformation structure describing the client
		/// exception that caused the minidump to be generated. If the value of this 
		/// parameter is IntPtr.Zero, no exception information is included in the minidump
		/// file.
		/// </param>
		[DllImport("dbghelp.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool MiniDumpWriteDump ( IntPtr hProcess,
			int processId,
			SafeFileHandle hFile,
			MiniDumpType dumpType,
			IntPtr exceptionParam,
			IntPtr userStreamParam,
			IntPtr callbackParam );
	}
}