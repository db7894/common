using System;

namespace SharedAssemblies.Monitoring.Snmp.Win32.Types
{
    /// <summary>
    /// Different attributes a file can have
    /// </summary>
    [Flags]
    internal enum FileAttributes : uint
    {
        /// <summary>
        /// No attributes
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Cannot write to file
        /// </summary>
        Readonly = 0x00000001,

        /// <summary>
        /// File not visible
        /// </summary>
        Hidden = 0x00000002,

        /// <summary>
        /// Special system file
        /// </summary>
        System = 0x00000004,

        /// <summary>
        /// File is a directory
        /// </summary>
        Directory = 0x00000010,

        /// <summary>
        /// File is archived
        /// </summary>
        Archive = 0x00000020,

        /// <summary>
        /// File is a device
        /// </summary>
        Device = 0x00000040,

        /// <summary>
        /// File is normal
        /// </summary>
        Normal = 0x00000080,

        /// <summary>
        /// File is temporary
        /// </summary>
        Temporary = 0x00000100,

        /// <summary>
        /// File has sparse access
        /// </summary>
        SparseFile = 0x00000200,

        /// <summary>
        /// No freaking idea
        /// </summary>
        ReparsePoint = 0x00000400,

        /// <summary>
        /// File is compressed
        /// </summary>
        Compressed = 0x00000800,

        /// <summary>
        /// File is not always online
        /// </summary>
        Offline = 0x00001000,

        /// <summary>
        /// File is not indexed
        /// </summary>
        NotContentIndexed = 0x00002000,

        /// <summary>
        /// File is encrypted
        /// </summary>
        Encrypted = 0x00004000,

        /// <summary>
        /// File has write-through
        /// </summary>
        WriteThrough = 0x80000000,

        /// <summary>
        /// Even lesss freaking idea
        /// </summary>
        Overlapped = 0x40000000,

        /// <summary>
        /// File is not buffered
        /// </summary>
        NoBuffering = 0x20000000,

        /// <summary>
        /// File is random access
        /// </summary>
        RandomAccess = 0x10000000,

        /// <summary>
        /// File is sequentially accessed
        /// </summary>
        SequentialScan = 0x08000000,

        /// <summary>
        /// File deletes when closed
        /// </summary>
        DeleteOnClose = 0x04000000,

        /// <summary>
        /// File has backup semantics
        /// </summary>
        BackupSemantics = 0x02000000,

        /// <summary>
        /// File has Posix Semantics
        /// </summary>
        PosixSemantics = 0x01000000,

        /// <summary>
        /// File has open reparse point
        /// </summary>
        OpenReparsePoint = 0x00200000,

        /// <summary>
        /// File has open no recall
        /// </summary>
        OpenNoRecall = 0x00100000,

        /// <summary>
        /// File has first pipe instance
        /// </summary>
        FirstPipeInstance = 0x00080000
    }
}
