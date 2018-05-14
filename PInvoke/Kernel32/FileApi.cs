using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Vanara.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Vanara.PInvoke
{
	/// <summary>Fields, enums, functions and structures for kernel32.dll.</summary>
	public static partial class Kernel32
	{
		/// <summary>A value returned when invalid file attributes are found.</summary>
		[PInvokeData("fileapi.h")] public const int INVALID_FILE_ATTRIBUTES = -2;
		/// <summary>A value returned by <see cref="GetCompressedFileSize(string, ref uint)"/> when the function fails.</summary>
		[PInvokeData("fileapi.h")] public const uint INVALID_FILE_SIZE = 0xFFFFFFFF;
		/// <summary>A value returned then a file pointer cannot be set.</summary>
		[PInvokeData("fileapi.h")] public const int INVALID_SET_FILE_POINTER = -1;

		/// <summary>
		/// An application-defined callback function used with the ReadFileEx and WriteFileEx functions. It is called when the asynchronous input and output
		/// (I/O) operation is completed or canceled and the calling thread is in an alertable state (by using the SleepEx, MsgWaitForMultipleObjectsEx,
		/// WaitForSingleObjectEx, or WaitForMultipleObjectsEx function with the fAlertable parameter set to TRUE).
		/// </summary>
		/// <param name="dwErrorCode">The I/O completion status. This parameter can be one of the system error codes.</param>
		/// <param name="dwNumberOfBytesTransfered">The number of bytes transferred. If an error occurs, this parameter is zero.</param>
		/// <param name="lpOverlapped">
		/// A pointer to the OVERLAPPED structure specified by the asynchronous I/O function.
		/// <para>
		/// The system does not use the OVERLAPPED structure after the completion routine is called, so the completion routine can deallocate the memory used by
		/// the overlapped structure.
		/// </para>
		/// </param>
		public unsafe delegate void FileIOCompletionRoutine(uint dwErrorCode, uint dwNumberOfBytesTransfered, NativeOverlapped* lpOverlapped);

		/// <summary>The controllable aspects of the DefineDosDevice function.</summary>
		[Flags]
		public enum DDD
		{
			/// <summary>Uses the lpTargetPath string as is. Otherwise, it is converted from an MS-DOS path to a path.</summary>
			DDD_RAW_TARGET_PATH = 0x00000001,
			/// <summary>
			/// Removes the specified definition for the specified device. To determine which definition to remove, the function walks the list of mappings for
			/// the device, looking for a match of lpTargetPath against a prefix of each mapping associated with this device. The first mapping that matches is
			/// the one removed, and then the function returns.
			/// <para>
			/// If lpTargetPath is NULL or a pointer to a NULL string, the function will remove the first mapping associated with the device and pop the most
			/// recent one pushed. If there is nothing left to pop, the device name will be removed.
			/// </para>
			/// <para>If this value is not specified, the string pointed to by the lpTargetPath parameter will become the new mapping for this device.</para>
			/// </summary>
			DDD_REMOVE_DEFINITION = 0x00000002,
			/// <summary>
			/// If this value is specified along with DDD_REMOVE_DEFINITION, the function will use an exact match to determine which mapping to remove. Use this
			/// value to ensure that you do not delete something that you did not define.
			/// </summary>
			DDD_EXACT_MATCH_ON_REMOVE = 0x00000004,
			/// <summary>
			/// Do not broadcast the WM_SETTINGCHANGE message. By default, this message is broadcast to notify the shell and applications of the change.
			/// </summary>
			DDD_NO_BROADCAST_SYSTEM = 0x00000008,
			/// <summary>Undocumented.</summary>
			DDD_LUID_BROADCAST_DRIVE = 0x00000010
		}

		/// <summary>Specifies the type of drive.</summary>
		public enum DRIVE_TYPE
		{
			/// <summary>The drive type cannot be determined.</summary>
			DRIVE_UNKNOWN = 0,

			/// <summary>The root path is invalid; for example, there is no volume mounted at the specified path.</summary>
			DRIVE_NO_ROOT_DIR = 1,

			/// <summary>The drive has removable media; for example, a floppy drive, thumb drive, or flash card reader.</summary>
			DRIVE_REMOVABLE = 2,

			/// <summary>The drive has fixed media; for example, a hard disk drive or flash drive.</summary>
			DRIVE_FIXED = 3,

			/// <summary>The drive is a remote (network) drive.</summary>
			DRIVE_REMOTE = 4,

			/// <summary>The drive is a CD-ROM drive.</summary>
			DRIVE_CDROM = 5,

			/// <summary>The drive is a RAM disk.</summary>
			DRIVE_RAMDISK = 6,
		}

		/// <summary>The filter conditions that satisfy a change notification wait.</summary>
		[Flags]
		public enum FILE_NOTIFY_CHANGE
		{
			/// <summary>
			/// Any file name change in the watched directory or subtree causes a change notification wait operation to return. Changes include renaming,
			/// creating, or deleting a file name.
			/// </summary>
			FILE_NOTIFY_CHANGE_FILE_NAME = 0x00000001,
			/// <summary>
			/// Any directory-name change in the watched directory or subtree causes a change notification wait operation to return. Changes include creating or
			/// deleting a directory.
			/// </summary>
			FILE_NOTIFY_CHANGE_DIR_NAME = 0x00000002,
			/// <summary>Any attribute change in the watched directory or subtree causes a change notification wait operation to return.</summary>
			FILE_NOTIFY_CHANGE_ATTRIBUTES = 0x00000004,
			/// <summary>
			/// Any file-size change in the watched directory or subtree causes a change notification wait operation to return. The operating system detects a
			/// change in file size only when the file is written to the disk. For operating systems that use extensive caching, detection occurs only when the
			/// cache is sufficiently flushed.
			/// </summary>
			FILE_NOTIFY_CHANGE_SIZE = 0x00000008,
			/// <summary>
			/// Any change to the last write-time of files in the watched directory or subtree causes a change notification wait operation to return. The
			/// operating system detects a change to the last write-time only when the file is written to the disk. For operating systems that use extensive
			/// caching, detection occurs only when the cache is sufficiently flushed.
			/// </summary>
			FILE_NOTIFY_CHANGE_LAST_WRITE = 0x00000010,
			/// <summary>Any change to the last access time of files in the watched directory or subtree causes a change notification wait operation to return.</summary>
			FILE_NOTIFY_CHANGE_LAST_ACCESS = 0x00000020,
			/// <summary>Any change to the creation time of files in the watched directory or subtree causes a change notification wait operation to return.</summary>
			FILE_NOTIFY_CHANGE_CREATION = 0x00000040,
			/// <summary>Any security-descriptor change in the watched directory or subtree causes a change notification wait operation to return.</summary>
			FILE_NOTIFY_CHANGE_SECURITY = 0x00000100,
		}

		/// <summary>Specifies additional flags that control the search.</summary>
		[Flags]
		public enum FIND_FIRST
		{
			/// <summary>Searches are case-sensitive.</summary>
			FIND_FIRST_EX_CASE_SENSITIVE = 1,
			/// <summary>
			/// Uses a larger buffer for directory queries, which can increase performance of the find operation. This value is not supported until Windows
			/// Server 2008 R2 and Windows 7.
			/// </summary>
			FIND_FIRST_EX_LARGE_FETCH = 2,
			/// <summary>Limits the results to files that are physically on disk. This flag is only relevant when a file virtualization filter is present.</summary>
			FIND_FIRST_EX_ON_DISK_ENTRIES_ONLY = 4
		}

		/// <summary>Defines values that are used with the FindFirstFileEx function to specify the information level of the returned data.</summary>
		public enum FINDEX_INFO_LEVELS
		{
			/// <summary>The FindFirstFileEx function retrieves a standard set of attribute information. The data is returned in a WIN32_FIND_DATA structure.</summary>
			FindExInfoStandard,

			/// <summary>
			/// The FindFirstFileEx function does not query the short file name, improving overall enumeration speed. The data is returned in a WIN32_FIND_DATA
			/// structure, and the cAlternateFileName member is always a NULL string.
			/// </summary>
			FindExInfoMaxInfoLevel,
		}

		/// <summary>Defines values that are used with the FindFirstFileEx function to specify the type of filtering to perform.</summary>
		public enum FINDEX_SEARCH_OPS
		{
			/// <summary>
			/// The search for a file that matches a specified file name. The lpSearchFilter parameter of FindFirstFileEx must be NULL when this search operation
			/// is used.
			/// </summary>
			FindExSearchNameMatch,

			/// <summary>
			/// This is an advisory flag. If the file system supports directory filtering, the function searches for a file that matches the specified name and
			/// is also a directory. If the file system does not support directory filtering, this flag is silently ignored.
			/// <para>The lpSearchFilter parameter of the FindFirstFileEx function must be NULL when this search value is used.</para>
			/// <para>
			/// If directory filtering is desired, this flag can be used on all file systems, but because it is an advisory flag and only affects file systems
			/// that support it, the application must examine the file attribute data stored in the lpFindFileData parameter of the FindFirstFileEx function to
			/// determine whether the function has returned a handle to a directory.
			/// </para>
			/// </summary>
			FindExSearchLimitToDirectories,

			/// <summary>This filtering type is not available.</summary>
			FindExSearchLimitToDevices
		}

		/// <summary>
		/// Defines values that are used with the GetFileAttributesEx and GetFileAttributesTransacted functions to specify the information level of the returned data.
		/// </summary>
		public enum GET_FILEEX_INFO_LEVELS
		{
			/// <summary>
			/// The GetFileAttributesEx or GetFileAttributesTransacted function retrieves a standard set of attribute information. The data is returned in a
			/// WIN32_FILE_ATTRIBUTE_DATA structure.
			/// </summary>
			GetFileExInfoStandard,
		}

		/// <summary>Options for <see cref="LockFileEx"/>.</summary>
		[PInvokeData("minwinbase.h", MSDNShortId = "aa365203")]
		[Flags]
		public enum LOCKFILE
		{
			/// <summary>The function returns immediately if it is unable to acquire the requested lock. Otherwise, it waits.</summary>
			LOCKFILE_FAIL_IMMEDIATELY = 0x00000001,
			/// <summary>The function requests an exclusive lock. Otherwise, it requests a shared lock.</summary>
			LOCKFILE_EXCLUSIVE_LOCK = 0x00000002
		}

		/// <summary>
		/// Reads data from the specified file or input/output (I/O) device. Reads occur at the position specified by the file pointer if supported by the device.
		/// </summary>
		/// <param name="hFile">
		/// A handle to the device (for example, a file, file stream, physical disk, volume, console buffer, tape drive, socket, communications resource,
		/// mailslot, or pipe). The hFile parameter must have been created with read access.
		/// </param>
		/// <param name="buffer">A pointer to the buffer that receives the data read from a file or device.</param>
		/// <param name="numberOfBytesToRead">The maximum number of bytes to be read.</param>
		/// <param name="requestCallback">An AsyncCallback delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="stateObject">
		/// A user-defined object that contains information about the operation. This object is passed to the requestCallback delegate when the operation is complete.
		/// </param>
		/// <returns>An IAsyncResult instance that references the asynchronous request.</returns>
		public static unsafe IAsyncResult BeginReadFile(SafeFileHandle hFile, byte[] buffer, uint numberOfBytesToRead, AsyncCallback requestCallback, object stateObject)
		{
			var ar = OverlappedAsync.SetupOverlappedFunction(hFile, requestCallback, stateObject);
			fixed (byte* pIn = buffer)
			{
				var ret = ReadFile(hFile, pIn, numberOfBytesToRead, IntPtr.Zero, ar.Overlapped);
				return OverlappedAsync.EvaluateOverlappedFunction(ar, ret);
			}
		}

		/// <summary>
		/// Writes data to the specified file or input/output (I/O) device.
		/// <para>
		/// This function is designed for both synchronous and asynchronous operation. For a similar function designed solely for asynchronous operation, see WriteFileEx.
		/// </para>
		/// </summary>
		/// <param name="hFile">
		/// A handle to the file or I/O device (for example, a file, file stream, physical disk, volume, console buffer, tape drive, socket, communications
		/// resource, mailslot, or pipe).
		/// <para>
		/// The hFile parameter must have been created with the write access. For more information, see Generic Access Rights and File Security and Access Rights.
		/// </para>
		/// <para>
		/// For asynchronous write operations, hFile can be any handle opened with the CreateFile function using the FILE_FLAG_OVERLAPPED flag or a socket handle
		/// returned by the socket or accept function.
		/// </para>
		/// </param>
		/// <param name="buffer">
		/// A pointer to the buffer containing the data to be written to the file or device.
		/// <para>This buffer must remain valid for the duration of the write operation. The caller must not use this buffer until the write operation is completed.</para>
		/// </param>
		/// <param name="numberOfBytesToWrite">
		/// The number of bytes to be written to the file or device.
		/// <para>
		/// A value of zero specifies a null write operation. The behavior of a null write operation depends on the underlying file system or communications technology.
		/// </para>
		/// <para>
		/// Windows Server 2003 and Windows XP: Pipe write operations across a network are limited in size per write. The amount varies per platform. For x86
		/// platforms it's 63.97 MB. For x64 platforms it's 31.97 MB. For Itanium it's 63.95 MB. For more information regarding pipes, see the Remarks section.
		/// </para>
		/// </param>
		/// <param name="requestCallback">An AsyncCallback delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="stateObject">
		/// A user-defined object that contains information about the operation. This object is passed to the requestCallback delegate when the operation is complete.
		/// </param>
		/// <returns>An IAsyncResult instance that references the asynchronous request.</returns>
		public static unsafe IAsyncResult BeginWriteFile(SafeFileHandle hFile, byte[] buffer, uint numberOfBytesToWrite, AsyncCallback requestCallback, object stateObject)
		{
			var ar = OverlappedAsync.SetupOverlappedFunction(hFile, requestCallback, stateObject);
			fixed (byte* pIn = buffer)
			{
				var ret = WriteFile(hFile, pIn, numberOfBytesToWrite, IntPtr.Zero, ar.Overlapped);
				return OverlappedAsync.EvaluateOverlappedFunction(ar, ret);
			}
		}

		/// <summary>
		/// <para>Compares two file times.</para>
		/// </summary>
		/// <param name="lpFileTime1">
		/// <para>A pointer to a <c>FILETIME</c> structure that specifies the first file time.</para>
		/// </param>
		/// <param name="lpFileTime2">
		/// <para>A pointer to a <c>FILETIME</c> structure that specifies the second file time.</para>
		/// </param>
		/// <returns>
		/// <para>The return value is one of the following values.</para>
		/// <list type="table">
		/// <listheader>
		/// <term>Return value</term>
		/// <term>Description</term>
		/// </listheader>
		/// <item>
		/// <term>-1</term>
		/// <term>First file time is earlier than second file time.</term>
		/// </item>
		/// <item>
		/// <term>0</term>
		/// <term>First file time is equal to second file time.</term>
		/// </item>
		/// <item>
		/// <term>1</term>
		/// <term>First file time is later than second file time.</term>
		/// </item>
		/// </list>
		/// </returns>
		// LONG WINAPI CompareFileTime( _In_ const FILETIME *lpFileTime1, _In_ const FILETIME *lpFileTime2);
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "ms724214")]
		public static extern int CompareFileTime([In, MarshalAs(UnmanagedType.LPStruct)] FILETIME lpFileTime1, [In, MarshalAs(UnmanagedType.LPStruct)] FILETIME lpFileTime2);

		/// <summary>
		/// Creates a new directory. If the underlying file system supports security on files and directories, the function applies a specified security
		/// descriptor to the new directory.
		/// </summary>
		/// <param name="lpPathName">The path of the directory to be created.</param>
		/// <param name="lpSecurityAttributes">
		/// A pointer to a SECURITY_ATTRIBUTES structure. The lpSecurityDescriptor member of the structure specifies a security descriptor for the new directory.
		/// If lpSecurityAttributes is NULL, the directory gets a default security descriptor. The ACLs in the default security descriptor for a directory are
		/// inherited from its parent directory.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa363855")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CreateDirectory(string lpPathName, [In] SECURITY_ATTRIBUTES lpSecurityAttributes);

		/// <summary>
		/// Creates or opens a file or I/O device. The most commonly used I/O devices are as follows: file, file stream, directory, physical disk, volume,
		/// console buffer, tape drive, communications resource, mailslot, and pipe. The function returns a handle that can be used to access the file or device
		/// for various types of I/O depending on the file or device and the flags and attributes specified.
		/// </summary>
		/// <param name="lpFileName">
		/// The name of the file or device to be created or opened. You may use either forward slashes (/) or backslashes (\) in this name.
		/// <para>
		/// In the ANSI version of this function, the name is limited to MAX_PATH characters. To extend this limit to 32,767 wide characters, call the Unicode
		/// version of the function and prepend "\\?\" to the path. For more information, see Naming Files, Paths, and Namespaces.
		/// </para>
		/// <para>For information on special device names, see Defining an MS-DOS Device Name.</para>
		/// <para>To create a file stream, specify the name of the file, a colon, and then the name of the stream. For more information, see File Streams.</para>
		/// <note type="tip">Starting with Windows 10, version 1607, for the Unicode version of this function (CreateFileW), you can opt-in to remove the
		/// MAX_PATH limitation without prepending "\\?\". See the "Maximum Path Length Limitation" section of Naming Files, Paths, and Namespaces for details.</note>
		/// </param>
		/// <param name="dwDesiredAccess">
		/// The requested access to the file or device, which can be summarized as read, write, both or neither zero).
		/// <para>
		/// The most commonly used values are GENERIC_READ, GENERIC_WRITE, or both (GENERIC_READ | GENERIC_WRITE). For more information, see Generic Access
		/// Rights, File Security and Access Rights, File Access Rights Constants, and ACCESS_MASK.
		/// </para>
		/// <para>
		/// If this parameter is zero, the application can query certain metadata such as file, directory, or device attributes without accessing that file or
		/// device, even if GENERIC_READ access would have been denied.
		/// </para>
		/// <para>
		/// You cannot request an access mode that conflicts with the sharing mode that is specified by the dwShareMode parameter in an open request that already
		/// has an open handle.
		/// </para>
		/// </param>
		/// <param name="dwShareMode">
		/// The requested sharing mode of the file or device, which can be read, write, both, delete, all of these, or none (refer to the following table).
		/// Access requests to attributes or extended attributes are not affected by this flag.
		/// <para>
		/// If this parameter is zero and CreateFile succeeds, the file or device cannot be shared and cannot be opened again until the handle to the file or
		/// device is closed. For more information, see the Remarks section.
		/// </para>
		/// <para>
		/// You cannot request a sharing mode that conflicts with the access mode that is specified in an existing request that has an open handle. CreateFile
		/// would fail and the GetLastError function would return ERROR_SHARING_VIOLATION.
		/// </para>
		/// <para>
		/// To enable a process to share a file or device while another process has the file or device open, use a compatible combination of one or more of the
		/// following values. For more information about valid combinations of this parameter with the dwDesiredAccess parameter, see Creating and Opening Files.
		/// </para>
		/// <note>The sharing options for each open handle remain in effect until that handle is closed, regardless of process context.</note>
		/// </param>
		/// <param name="lpSecurityAttributes">
		/// A pointer to a SECURITY_ATTRIBUTES structure that contains two separate but related data members: an optional security descriptor, and a Boolean
		/// value that determines whether the returned handle can be inherited by child processes.
		/// <para>This parameter can be NULL.</para>
		/// <para>
		/// If this parameter is NULL, the handle returned by CreateFile cannot be inherited by any child processes the application may create and the file or
		/// device associated with the returned handle gets a default security descriptor.
		/// </para>
		/// <para>
		/// The lpSecurityDescriptor member of the structure specifies a SECURITY_DESCRIPTOR for a file or device. If this member is NULL, the file or device
		/// associated with the returned handle is assigned a default security descriptor.
		/// </para>
		/// <para>CreateFile ignores the lpSecurityDescriptor member when opening an existing file or device, but continues to use the bInheritHandle member.</para>
		/// <para>The bInheritHandlemember of the structure specifies whether the returned handle can be inherited.</para>
		/// </param>
		/// <param name="dwCreationDisposition">
		/// An action to take on a file or device that exists or does not exist.
		/// <para>For devices other than files, this parameter is usually set to OPEN_EXISTING.</para>
		/// </param>
		/// <param name="dwFlagsAndAttributes">
		/// The file or device attributes and flags, FILE_ATTRIBUTE_NORMAL being the most common default value for files.
		/// <para>This parameter can include any combination of the available file attributes (FILE_ATTRIBUTE_*). All other file attributes override FILE_ATTRIBUTE_NORMAL.</para>
		/// <para>
		/// This parameter can also contain combinations of flags (FILE_FLAG_*) for control of file or device caching behavior, access modes, and other
		/// special-purpose flags. These combine with any FILE_ATTRIBUTE_* values.
		/// </para>
		/// <para>
		/// This parameter can also contain Security Quality of Service (SQOS) information by specifying the SECURITY_SQOS_PRESENT flag. Additional SQOS-related
		/// flags information is presented in the table following the attributes and flags tables.
		/// </para>
		/// <note>When CreateFile opens an existing file, it generally combines the file flags with the file attributes of the existing file, and ignores any
		/// file attributes supplied as part of dwFlagsAndAttributes. Special cases are detailed in Creating and Opening Files.</note>
		/// <para>
		/// Some of the following file attributes and flags may only apply to files and not necessarily all other types of devices that CreateFile can open. For
		/// additional information, see the Remarks section of this topic and Creating and Opening Files.
		/// </para>
		/// <para>
		/// For more advanced access to file attributes, see SetFileAttributes. For a complete list of all file attributes with their values and descriptions,
		/// see File Attribute Constants.
		/// </para>
		/// </param>
		/// <param name="hTemplateFile">
		/// A valid handle to a template file with the GENERIC_READ access right. The template file supplies file attributes and extended attributes for the file
		/// that is being created.
		/// <para>This parameter can be NULL.</para>
		/// <para>When opening an existing file, CreateFile ignores this parameter.</para>
		/// <para>
		/// When opening a new encrypted file, the file inherits the discretionary access control list from its parent directory. For additional information, see
		/// File Encryption.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is an open handle to the specified file, device, named pipe, or mail slot.
		/// <para>If the function fails, the return value is INVALID_HANDLE_VALUE. To get extended error information, call GetLastError.</para>
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa363858")]
		public static extern SafeFileHandle CreateFile(string lpFileName, FileAccess dwDesiredAccess, FileShare dwShareMode,
			[Optional] SECURITY_ATTRIBUTES lpSecurityAttributes, FileMode dwCreationDisposition, FileFlagsAndAttributes dwFlagsAndAttributes,
			SafeFileHandle hTemplateFile);

		/// <summary>
		/// Creates or opens a file or I/O device. The most commonly used I/O devices are as follows: file, file stream, directory, physical disk, volume,
		/// console buffer, tape drive, communications resource, mailslot, and pipe. The function returns a handle that can be used to access the file or device
		/// for various types of I/O depending on the file or device and the flags and attributes specified.
		/// </summary>
		/// <param name="lpFileName">
		/// The name of the file or device to be created or opened. You may use either forward slashes (/) or backslashes (\) in this name.
		/// <para>
		/// In the ANSI version of this function, the name is limited to MAX_PATH characters. To extend this limit to 32,767 wide characters, call the Unicode
		/// version of the function and prepend "\\?\" to the path. For more information, see Naming Files, Paths, and Namespaces.
		/// </para>
		/// <para>For information on special device names, see Defining an MS-DOS Device Name.</para>
		/// <para>To create a file stream, specify the name of the file, a colon, and then the name of the stream. For more information, see File Streams.</para>
		/// <note type="tip">Starting with Windows 10, version 1607, for the Unicode version of this function (CreateFileW), you can opt-in to remove the
		/// MAX_PATH limitation without prepending "\\?\". See the "Maximum Path Length Limitation" section of Naming Files, Paths, and Namespaces for details.</note>
		/// </param>
		/// <param name="dwDesiredAccess">
		/// The requested access to the file or device, which can be summarized as read, write, both or neither zero).
		/// <para>
		/// The most commonly used values are GENERIC_READ, GENERIC_WRITE, or both (GENERIC_READ | GENERIC_WRITE). For more information, see Generic Access
		/// Rights, File Security and Access Rights, File Access Rights Constants, and ACCESS_MASK.
		/// </para>
		/// <para>
		/// If this parameter is zero, the application can query certain metadata such as file, directory, or device attributes without accessing that file or
		/// device, even if GENERIC_READ access would have been denied.
		/// </para>
		/// <para>
		/// You cannot request an access mode that conflicts with the sharing mode that is specified by the dwShareMode parameter in an open request that already
		/// has an open handle.
		/// </para>
		/// </param>
		/// <param name="dwShareMode">
		/// The requested sharing mode of the file or device, which can be read, write, both, delete, all of these, or none (refer to the following table).
		/// Access requests to attributes or extended attributes are not affected by this flag.
		/// <para>
		/// If this parameter is zero and CreateFile succeeds, the file or device cannot be shared and cannot be opened again until the handle to the file or
		/// device is closed. For more information, see the Remarks section.
		/// </para>
		/// <para>
		/// You cannot request a sharing mode that conflicts with the access mode that is specified in an existing request that has an open handle. CreateFile
		/// would fail and the GetLastError function would return ERROR_SHARING_VIOLATION.
		/// </para>
		/// <para>
		/// To enable a process to share a file or device while another process has the file or device open, use a compatible combination of one or more of the
		/// following values. For more information about valid combinations of this parameter with the dwDesiredAccess parameter, see Creating and Opening Files.
		/// </para>
		/// <note>The sharing options for each open handle remain in effect until that handle is closed, regardless of process context.</note>
		/// </param>
		/// <param name="lpSecurityAttributes">
		/// A pointer to a SECURITY_ATTRIBUTES structure that contains two separate but related data members: an optional security descriptor, and a Boolean
		/// value that determines whether the returned handle can be inherited by child processes.
		/// <para>This parameter can be NULL.</para>
		/// <para>
		/// If this parameter is NULL, the handle returned by CreateFile cannot be inherited by any child processes the application may create and the file or
		/// device associated with the returned handle gets a default security descriptor.
		/// </para>
		/// <para>
		/// The lpSecurityDescriptor member of the structure specifies a SECURITY_DESCRIPTOR for a file or device. If this member is NULL, the file or device
		/// associated with the returned handle is assigned a default security descriptor.
		/// </para>
		/// <para>CreateFile ignores the lpSecurityDescriptor member when opening an existing file or device, but continues to use the bInheritHandle member.</para>
		/// <para>The bInheritHandlemember of the structure specifies whether the returned handle can be inherited.</para>
		/// </param>
		/// <param name="dwCreationDisposition">
		/// An action to take on a file or device that exists or does not exist.
		/// <para>For devices other than files, this parameter is usually set to OPEN_EXISTING.</para>
		/// </param>
		/// <param name="dwFlagsAndAttributes">
		/// The file or device attributes and flags, FILE_ATTRIBUTE_NORMAL being the most common default value for files.
		/// <para>This parameter can include any combination of the available file attributes (FILE_ATTRIBUTE_*). All other file attributes override FILE_ATTRIBUTE_NORMAL.</para>
		/// <para>
		/// This parameter can also contain combinations of flags (FILE_FLAG_*) for control of file or device caching behavior, access modes, and other
		/// special-purpose flags. These combine with any FILE_ATTRIBUTE_* values.
		/// </para>
		/// <para>
		/// This parameter can also contain Security Quality of Service (SQOS) information by specifying the SECURITY_SQOS_PRESENT flag. Additional SQOS-related
		/// flags information is presented in the table following the attributes and flags tables.
		/// </para>
		/// <note>When CreateFile opens an existing file, it generally combines the file flags with the file attributes of the existing file, and ignores any
		/// file attributes supplied as part of dwFlagsAndAttributes. Special cases are detailed in Creating and Opening Files.</note>
		/// <para>
		/// Some of the following file attributes and flags may only apply to files and not necessarily all other types of devices that CreateFile can open. For
		/// additional information, see the Remarks section of this topic and Creating and Opening Files.
		/// </para>
		/// <para>
		/// For more advanced access to file attributes, see SetFileAttributes. For a complete list of all file attributes with their values and descriptions,
		/// see File Attribute Constants.
		/// </para>
		/// </param>
		/// <param name="hTemplateFile">
		/// A valid handle to a template file with the GENERIC_READ access right. The template file supplies file attributes and extended attributes for the file
		/// that is being created.
		/// <para>This parameter can be NULL.</para>
		/// <para>When opening an existing file, CreateFile ignores this parameter.</para>
		/// <para>
		/// When opening a new encrypted file, the file inherits the discretionary access control list from its parent directory. For additional information, see
		/// File Encryption.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is an open handle to the specified file, device, named pipe, or mail slot.
		/// <para>If the function fails, the return value is INVALID_HANDLE_VALUE. To get extended error information, call GetLastError.</para>
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa363858")]
		public static extern SafeFileHandle CreateFile(string lpFileName, FileAccess dwDesiredAccess, FileShare dwShareMode,
			[Optional] SECURITY_ATTRIBUTES lpSecurityAttributes, FileMode dwCreationDisposition, FileFlagsAndAttributes dwFlagsAndAttributes,
			[Optional] IntPtr hTemplateFile);

		/// <summary>
		/// <para>
		/// Creates or opens a file or I/O device. The most commonly used I/O devices are as follows: file, file stream, directory, physical disk, volume,
		/// console buffer, tape drive, communications resource, mailslot, and pipe. The function returns a handle that can be used to access the file or device
		/// for various types of I/O depending on the file or device and the flags and attributes specified.
		/// </para>
		/// <para>
		/// When called from a Windows Store app, <c>CreateFile2</c> is simplified. You can open only files or directories inside the
		/// <c>ApplicationData.LocalFolder</c> or <c>Package.InstalledLocation</c> directories. You can't open named pipes or mailslots or create encrypted files
		/// ( <c>FILE_ATTRIBUTE_ENCRYPTED</c>).
		/// </para>
		/// <para>
		/// To perform this operation as a transacted operation, which results in a handle that can be used for transacted I/O, use the
		/// <c>CreateFileTransacted</c> function.
		/// </para>
		/// </summary>
		/// <param name="lpFileName">
		/// <para>The name of the file or device to be created or opened.</para>
		/// <para>For information on special device names, see Defining an MS-DOS Device Name.</para>
		/// <para>To create a file stream, specify the name of the file, a colon, and then the name of the stream. For more information, see File Streams.</para>
		/// </param>
		/// <param name="dwDesiredAccess">
		/// <para>The requested access to the file or device, which can be summarized as read, write, both or neither zero).</para>
		/// <para>
		/// The most commonly used values are <c>GENERIC_READ</c>, <c>GENERIC_WRITE</c>, or both (). For more information, see Generic Access Rights, File
		/// Security and Access Rights, <c>File Access Rights Constants</c>, and <c>ACCESS_MASK</c>.
		/// </para>
		/// <para>
		/// If this parameter is zero, the application can query certain metadata such as file, directory, or device attributes without accessing that file or
		/// device, even if <c>GENERIC_READ</c> access would have been denied.
		/// </para>
		/// <para>
		/// You cannot request an access mode that conflicts with the sharing mode that is specified by the dwShareMode parameter in an open request that already
		/// has an open handle.
		/// </para>
		/// <para>For more information, see the Remarks section of this topic and Creating and Opening Files.</para>
		/// </param>
		/// <param name="dwShareMode">
		/// <para>
		/// The requested sharing mode of the file or device, which can be read, write, both, delete, all of these, or none (refer to the following table).
		/// Access requests to attributes or extended attributes are not affected by this flag.
		/// </para>
		/// <para>
		/// If this parameter is zero and <c>CreateFile2</c> succeeds, the file or device cannot be shared and cannot be opened again until the handle to the
		/// file or device is closed. For more information, see the Remarks section.
		/// </para>
		/// <para>
		/// You cannot request a sharing mode that conflicts with the access mode that is specified in an existing request that has an open handle.
		/// <c>CreateFile2</c> would fail and the <c>GetLastError</c> function would return <c>ERROR_SHARING_VIOLATION</c>.
		/// </para>
		/// <para>
		/// To enable a process to share a file or device while another process has the file or device open, use a compatible combination of one or more of the
		/// following values. For more information about valid combinations of this parameter with the dwDesiredAccess parameter, see Creating and Opening Files.
		/// </para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <term>Meaning</term>
		/// </listheader>
		/// <item>
		/// <term>0 0x00000000</term>
		/// <term>
		/// Prevents other processes from opening a file or device if they request delete, read, or write access. Exclusive access to a file or directory is only
		/// granted if the application has write access to the file.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_SHARE_DELETE 0x00000004</term>
		/// <term>
		/// Enables subsequent open operations on a file or device to request delete access.Otherwise, other processes cannot open the file or device if they
		/// request delete access.If this flag is not specified, but the file or device has been opened for delete access, the function fails.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_SHARE_READ 0x00000001</term>
		/// <term>
		/// Enables subsequent open operations on a file or device to request read access.Otherwise, other processes cannot open the file or device if they
		/// request read access.If this flag is not specified, but the file or device has been opened for read access, the function fails.If a file or directory
		/// is being opened and this flag is not specified, and the caller does not have write access to the file or directory, the function fails.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_SHARE_WRITE 0x00000002</term>
		/// <term>
		/// Enables subsequent open operations on a file or device to request write access.Otherwise, other processes cannot open the file or device if they
		/// request write access.If this flag is not specified, but the file or device has been opened for write access or has a file mapping with write access,
		/// the function fails.
		/// </term>
		/// </item>
		/// </list>
		/// </para>
		/// </param>
		/// <param name="dwCreationDisposition">
		/// <para>An action to take on a file or device that exists or does not exist.</para>
		/// <para>For devices other than files, this parameter is usually set to <c>OPEN_EXISTING</c>.</para>
		/// <para>For more information, see the Remarks section.</para>
		/// <para>This parameter must be one of the following values, which cannot be combined:</para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <term>Meaning</term>
		/// </listheader>
		/// <item>
		/// <term>CREATE_ALWAYS 2</term>
		/// <term>
		/// Creates a new file, always.If the specified file exists and is writable, the function overwrites the file, the function succeeds, and last-error code
		/// is set to ERROR_ALREADY_EXISTS (183).If the specified file does not exist and is a valid path, a new file is created, the function succeeds, and the
		/// last-error code is set to zero.For more information, see the Remarks section of this topic.
		/// </term>
		/// </item>
		/// <item>
		/// <term>CREATE_NEW 1</term>
		/// <term>
		/// Creates a new file, only if it does not already exist.If the specified file exists, the function fails and the last-error code is set to
		/// ERROR_FILE_EXISTS (80).If the specified file does not exist and is a valid path to a writable location, a new file is created.
		/// </term>
		/// </item>
		/// <item>
		/// <term>OPEN_ALWAYS 4</term>
		/// <term>
		/// Opens a file, always.If the specified file exists, the function succeeds and the last-error code is set to ERROR_ALREADY_EXISTS (183).If the
		/// specified file does not exist and is a valid path to a writable location, the function creates a file and the last-error code is set to zero.
		/// </term>
		/// </item>
		/// <item>
		/// <term>OPEN_EXISTING 3</term>
		/// <term>
		/// Opens a file or device, only if it exists.If the specified file or device does not exist, the function fails and the last-error code is set to
		/// ERROR_FILE_NOT_FOUND (2).For more information about devices, see the Remarks section.
		/// </term>
		/// </item>
		/// <item>
		/// <term>TRUNCATE_EXISTING 5</term>
		/// <term>
		/// Opens a file and truncates it so that its size is zero bytes, only if it exists.If the specified file does not exist, the function fails and the
		/// last-error code is set to ERROR_FILE_NOT_FOUND (2).The calling process must open the file with the GENERIC_WRITE bit set as part of the
		/// dwDesiredAccess parameter.
		/// </term>
		/// </item>
		/// </list>
		/// </para>
		/// </param>
		/// <param name="pCreateExParams">
		/// <para>Pointer to an optional <c>CREATEFILE2_EXTENDED_PARAMETERS</c> structure.</para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is an open handle to the specified file, device, named pipe, or mail slot.</para>
		/// <para>If the function fails, the return value is <c>INVALID_HANDLE_VALUE</c>. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// HANDLE WINAPI CreateFile2( _In_ LPCWSTR lpFileName, _In_ DWORD dwDesiredAccess, _In_ DWORD dwShareMode, _In_ DWORD dwCreationDisposition, _In_opt_
		// LPCREATEFILE2_EXTENDED_PARAMETERS pCreateExParams);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
		[PInvokeData("FileAPI.h", MSDNShortId = "hh449422")]
		public static extern SafeFileHandle CreateFile2(string lpFileName, FileAccess dwDesiredAccess, FileShare dwShareMode, FileMode dwCreationDisposition, ref CREATEFILE2_EXTENDED_PARAMETERS pCreateExParams);

		/// <summary>Defines, redefines, or deletes MS-DOS device names.</summary>
		/// <param name="dwFlags">The controllable aspects of the DefineDosDevice function.</param>
		/// <param name="lpDeviceName">
		/// A pointer to an MS-DOS device name string specifying the device the function is defining, redefining, or deleting. The device name string must not
		/// have a colon as the last character, unless a drive letter is being defined, redefined, or deleted. For example, drive C would be the string "C:". In
		/// no case is a trailing backslash ("\") allowed.
		/// </param>
		/// <param name="lpTargetPath">
		/// A pointer to a path string that will implement this device. The string is an MS-DOS path string unless the DDD_RAW_TARGET_PATH flag is specified, in
		/// which case this string is a path string.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa363904")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DefineDosDevice(DDD dwFlags, string lpDeviceName, string lpTargetPath);

		/// <summary>Deletes an existing file.</summary>
		/// <param name="lpFileName">The name of the file to be deleted.</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa363915")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteFile([In] string lpFileName);

		/// <summary>Deletes a drive letter or mounted folder.</summary>
		/// <param name="lpszVolumeMountPoint">The drive letter or mounted folder to be deleted. A trailing backslash is required, for example, "X:\" or "Y:\MountX\".</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa363927")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteVolumeMountPoint(string lpszVolumeMountPoint);

		/// <summary>Ends an asynchronous request for ReadFile.</summary>
		/// <param name="asyncResult">An IAsyncResult instance returned by a call to the BeginReadFile method.</param>
		public static void EndReadFile(IAsyncResult asyncResult)
		{
			OverlappedAsync.EndOverlappedFunction(asyncResult);
		}

		/// <summary>Ends an asynchronous request for WriteFile.</summary>
		/// <param name="asyncResult">An IAsyncResult instance returned by a call to the BeginWriteFile method.</param>
		public static void EndWriteFile(IAsyncResult asyncResult)
		{
			OverlappedAsync.EndOverlappedFunction(asyncResult);
		}

		/// <summary>Converts a file time to a local file time.</summary>
		/// <param name="lpFileTime">A pointer to a FILETIME structure containing the UTC-based file time to be converted into a local file time.</param>
		/// <param name="lpLocalFileTime">
		/// A pointer to a FILETIME structure to receive the converted local file time. This parameter cannot be the same as the lpFileTime parameter.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "ms724277")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FileTimeToLocalFileTime([In] ref FILETIME lpFileTime, [Out] out FILETIME lpLocalFileTime);

		/// <summary>
		/// Closes a file search handle opened by the FindFirstFile, FindFirstFileEx, FindFirstFileNameW, FindFirstFileNameTransactedW, FindFirstFileTransacted,
		/// FindFirstStreamTransactedW, or FindFirstStreamW functions.
		/// </summary>
		/// <param name="hFindFile">The file search handle.</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364413")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FindClose(IntPtr hFindFile);

		/// <summary>Stops change notification handle monitoring.</summary>
		/// <param name="hChangeHandle">A handle to a change notification handle created by the FindFirstChangeNotification function.</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364414")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FindCloseChangeNotification([In] IntPtr hChangeHandle);

		/// <summary>
		/// Creates a change notification handle and sets up initial change notification filter conditions. A wait on a notification handle succeeds when a
		/// change matching the filter conditions occurs in the specified directory or subtree. The function does not report changes to the specified directory itself.
		/// <para>
		/// This function does not indicate the change that satisfied the wait condition.To retrieve information about the specific change as part of the
		/// notification, use the ReadDirectoryChangesW function.
		/// </para>
		/// </summary>
		/// <param name="lpPathName">The full path of the directory to be watched. This cannot be a relative path or an empty string.</param>
		/// <param name="bWatchSubtree">
		/// If this parameter is TRUE, the function monitors the directory tree rooted at the specified directory; if it is FALSE, it monitors only the specified directory.
		/// </param>
		/// <param name="dwNotifyFilter">The filter conditions that satisfy a change notification wait.</param>
		/// <returns>
		/// If the function succeeds, the return value is a handle to a find change notification object. If the function fails, the return value is
		/// INVALID_HANDLE_VALUE.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364417")]
		public static extern SafeFindChangeNotificationHandle FindFirstChangeNotification(
			[In] string lpPathName, [MarshalAs(UnmanagedType.Bool)] bool bWatchSubtree, FILE_NOTIFY_CHANGE dwNotifyFilter);

		/// <summary>Searches a directory for a file or subdirectory with a name that matches a specific name (or partial name if wildcards are used).</summary>
		/// <param name="lpFileName">
		/// The directory or path, and the file name. The file name can include wildcard characters, for example, an asterisk (*) or a question mark (?).
		/// <para>
		/// This parameter should not be NULL, an invalid string (for example, an empty string or a string that is missing the terminating null character), or
		/// end in a trailing backslash (\).
		/// </para>
		/// <para>
		/// If the string ends with a wildcard, period (.), or directory name, the user must have access permissions to the root and all subdirectories on the path.
		/// </para>
		/// </param>
		/// <param name="lpFindFileData">A pointer to the WIN32_FIND_DATA structure that receives information about a found file or directory.</param>
		/// <returns>
		/// If the function succeeds, the return value is a search handle used in a subsequent call to FindNextFile or FindClose, and the lpFindFileData
		/// parameter contains information about the first file or directory found.
		/// <para>
		/// If the function fails or fails to locate files from the search string in the lpFileName parameter, the return value is INVALID_HANDLE_VALUE and the
		/// contents of lpFindFileData are indeterminate. To get extended error information, call the GetLastError function.
		/// </para>
		/// <para>If the function fails because no matching files can be found, the GetLastError function returns ERROR_FILE_NOT_FOUND.</para>
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364418")]
		public static extern SafeSearchHandle FindFirstFile([In] string lpFileName, [Out] out WIN32_FIND_DATA lpFindFileData);

		/// <summary>Searches a directory for a file or subdirectory with a name that matches a specific name (or partial name if wildcards are used).</summary>
		/// <param name="lpFileName">
		/// The directory or path, and the file name. The file name can include wildcard characters, for example, an asterisk (*) or a question mark (?).
		/// <para>
		/// This parameter should not be NULL, an invalid string (for example, an empty string or a string that is missing the terminating null character), or
		/// end in a trailing backslash (\).
		/// </para>
		/// <para>
		/// If the string ends with a wildcard, period (.), or directory name, the user must have access permissions to the root and all subdirectories on the path.
		/// </para>
		/// </param>
		/// <param name="fInfoLevelId">The information level of the returned data.</param>
		/// <param name="lpFindFileData">A pointer to the WIN32_FIND_DATA structure that receives information about a found file or directory.</param>
		/// <param name="fSearchOp">The type of filtering to perform that is different from wildcard matching.</param>
		/// <param name="lpSearchFilter">
		/// A pointer to the search criteria if the specified fSearchOp needs structured search information.
		/// <para>At this time, none of the supported fSearchOp values require extended search information. Therefore, this pointer must be NULL.</para>
		/// </param>
		/// <param name="dwAdditionalFlags">Specifies additional flags that control the search.</param>
		/// <returns>
		/// If the function succeeds, the return value is a search handle used in a subsequent call to FindNextFile or FindClose, and the lpFindFileData
		/// parameter contains information about the first file or directory found.
		/// <para>
		/// If the function fails or fails to locate files from the search string in the lpFileName parameter, the return value is INVALID_HANDLE_VALUE and the
		/// contents of lpFindFileData are indeterminate.To get extended error information, call the GetLastError function.
		/// </para>
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364419")]
		public static extern SafeSearchHandle FindFirstFileEx(
			[In] string lpFileName, FINDEX_INFO_LEVELS fInfoLevelId, [Out] out WIN32_FIND_DATA lpFindFileData, FINDEX_SEARCH_OPS fSearchOp,
			IntPtr lpSearchFilter, FIND_FIRST dwAdditionalFlags);

		/// <summary>Retrieves the name of a volume on a computer. FindFirstVolume is used to begin scanning the volumes of a computer.</summary>
		/// <param name="lpszVolumeName">
		/// A pointer to a buffer that receives a null-terminated string that specifies a volume GUID path for the first volume that is found.
		/// </param>
		/// <param name="cchBufferLength">The length of the buffer to receive the volume GUID path, in TCHARs.</param>
		/// <returns>
		/// If the function succeeds, the return value is a search handle used in a subsequent call to the FindNextVolume and FindVolumeClose functions.
		/// <para>
		/// If the function fails to find any volumes, the return value is the INVALID_HANDLE_VALUE error code. To get extended error information, call GetLastError.
		/// </para>
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364419")]
		public static extern SafeVolumeSearchHandle FindFirstVolume([In, Out] StringBuilder lpszVolumeName, uint cchBufferLength);

		/// <summary>Requests that the operating system signal a change notification handle the next time it detects an appropriate change.</summary>
		/// <param name="hChangeHandle">A handle to a change notification handle created by the FindFirstChangeNotification function.</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364427")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FindNextChangeNotification([In] SafeFindChangeNotificationHandle hChangeHandle);

		/// <summary>Continues a file search from a previous call to the FindFirstFile, FindFirstFileEx, or FindFirstFileTransacted functions.</summary>
		/// <param name="hFindFile">The search handle returned by a previous call to the FindFirstFile or FindFirstFileEx function.</param>
		/// <param name="lpFindFileData">A pointer to the WIN32_FIND_DATA structure that receives information about the found file or subdirectory.</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero and the lpFindFileData parameter contains information about the next file or directory found.
		/// <para>
		/// If the function fails, the return value is zero and the contents of lpFindFileData are indeterminate. To get extended error information, call the
		/// GetLastError function.
		/// </para>
		/// <para>If the function fails because no more matching files can be found, the GetLastError function returns ERROR_NO_MORE_FILES.</para>
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364428")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FindNextFile([In] SafeSearchHandle hFindFile, [Out] out WIN32_FIND_DATA lpFindFileData);

		/// <summary>Continues a volume search started by a call to the FindFirstVolume function. FindNextVolume finds one volume per call.</summary>
		/// <param name="hFindVolume">The volume search handle returned by a previous call to the FindFirstVolume function.</param>
		/// <param name="lpszVolumeName">A pointer to a string that receives the volume GUID path that is found.</param>
		/// <param name="cchBufferLength">The length of the buffer that receives the volume GUID path, in TCHARs.</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call
		/// GetLastError. If no matching files can be found, the GetLastError function returns the ERROR_NO_MORE_FILES error code. In that case, close the search
		/// with the FindVolumeClose function.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364431")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FindNextVolume(SafeVolumeSearchHandle hFindVolume, [In, Out] StringBuilder lpszVolumeName, uint cchBufferLength);

		/// <summary>Closes the specified volume search handle. The FindFirstVolume and FindNextVolume functions use this search handle to locate volumes.</summary>
		/// <param name="hFindVolume">The volume search handle to be closed. This handle must have been previously opened by the FindFirstVolume function.</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364433")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FindVolumeClose([In] IntPtr hFindVolume);

		/// <summary>Flushes the buffers of a specified file and causes all buffered data to be written to a file.</summary>
		/// <param name="hFile">
		/// A handle to the open file.
		/// <para>The file handle must have the GENERIC_WRITE access right. For more information, see File Security and Access Rights.</para>
		/// <para>If hFile is a handle to a communications device, the function only flushes the transmit buffer.</para>
		/// <para>
		/// If hFile is a handle to the server end of a named pipe, the function does not return until the client has read all buffered data from the pipe.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// <para>If the function fails, the return value is zero. To get extended error information, call GetLastError.</para>
		/// <para>
		/// The function fails if hFile is a handle to the console output. That is because the console output is not buffered. The function returns FALSE, and
		/// GetLastError returns ERROR_INVALID_HANDLE.
		/// </para>
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364433")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FlushFileBuffers([In] SafeFileHandle hFile);

		/// <summary>Retrieves information about the specified disk, including the amount of free space on the disk.</summary>
		/// <param name="lpRootPathName">
		/// The root directory of the disk for which information is to be returned. If this parameter is NULL, the function uses the root of the current disk. If
		/// this parameter is a UNC name, it must include a trailing backslash (for example, "\\MyServer\MyShare\"). Furthermore, a drive specification must have
		/// a trailing backslash (for example, "C:\"). The calling application must have FILE_LIST_DIRECTORY access rights for this directory.
		/// </param>
		/// <param name="lpSectorsPerCluster">A pointer to a variable that receives the number of sectors per cluster.</param>
		/// <param name="lpBytesPerSector">A pointer to a variable that receives the number of bytes per sector.</param>
		/// <param name="lpNumberOfFreeClusters">
		/// A pointer to a variable that receives the total number of free clusters on the disk that are available to the user who is associated with the calling thread.
		/// <para>If per-user disk quotas are in use, this value may be less than the total number of free clusters on the disk.</para>
		/// </param>
		/// <param name="lpTotalNumberOfClusters">
		/// A pointer to a variable that receives the total number of clusters on the disk that are available to the user who is associated with the calling thread.
		/// <para>If per-user disk quotas are in use, this value may be less than the total number of clusters on the disk.</para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364935")]
		public static extern bool GetDiskFreeSpace(string lpRootPathName, out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters, out uint lpTotalNumberOfClusters);

		/// <summary>
		/// Retrieves information about the amount of space that is available on a disk volume, which is the total amount of space, the total amount of free
		/// space, and the total amount of free space available to the user that is associated with the calling thread.
		/// </summary>
		/// <param name="lpDirectoryName">
		/// A directory on the disk.
		/// <para>If this parameter is NULL, the function uses the root of the current disk.</para>
		/// <para>If this parameter is a UNC name, it must include a trailing backslash, for example, "\\MyServer\MyShare\".</para>
		/// <para>This parameter does not have to specify the root directory on a disk. The function accepts any directory on a disk.</para>
		/// <para>The calling application must have FILE_LIST_DIRECTORY access rights for this directory.</para>
		/// </param>
		/// <param name="lpFreeBytesAvailable">
		/// A pointer to a variable that receives the total number of free bytes on a disk that are available to the user who is associated with the calling thread.
		/// <para>This parameter can be NULL.</para>
		/// <para>If per-user quotas are being used, this value may be less than the total number of free bytes on a disk.</para>
		/// </param>
		/// <param name="lpTotalNumberOfBytes">
		/// A pointer to a variable that receives the total number of bytes on a disk that are available to the user who is associated with the calling thread.
		/// <para>This parameter can be NULL.</para>
		/// <para>If per-user quotas are being used, this value may be less than the total number of bytes on a disk.</para>
		/// <para>To determine the total number of bytes on a disk or volume, use IOCTL_DISK_GET_LENGTH_INFO.</para>
		/// </param>
		/// <param name="lpTotalNumberOfFreeBytes">
		/// A pointer to a variable that receives the total number of free bytes on a disk.
		/// <para>This parameter can be NULL.</para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364937")]
		public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

		/// <summary>Determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive.</summary>
		/// <param name="lpRootPathName">
		/// The root directory for the drive. A trailing backslash is required.If this parameter is NULL, the function uses the root of the current directory.
		/// </param>
		/// <returns>The return value specifies the type of drive, which can be one of the following values.</returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364937")]
		public static extern DRIVE_TYPE GetDriveType([In] string lpRootPathName);

		/// <summary>Retrieves file system attributes for a specified file or directory.</summary>
		/// <param name="lpFileName">The name of the file or directory.</param>
		/// <returns>
		/// If the function succeeds, the return value contains the attributes of the specified file or directory. For a list of attribute values and their
		/// descriptions, see File Attribute Constants. If the function fails, the return value is INVALID_FILE_ATTRIBUTES.To get extended error information,
		/// call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364944")]
		public static extern FileFlagsAndAttributes GetFileAttributes([In] string lpFileName);

		/// <summary>Retrieves attributes for a specified file or directory.</summary>
		/// <param name="lpFileName">The name of the file or directory.</param>
		/// <param name="fInfoLevelId">A class of attribute information to retrieve.</param>
		/// <param name="lpFileInformation">
		/// A pointer to a buffer that receives the attribute information. The type of attribute information that is stored into this buffer is determined by the
		/// value of fInfoLevelId.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is a nonzero value. If the function fails, the return value is zero(0). To get extended error information,
		/// call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364946")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetFileAttributesEx(
			[In] string lpFileName, GET_FILEEX_INFO_LEVELS fInfoLevelId, out WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

		/// <summary>
		/// <para>Retrieves file information for the specified file.</para>
		/// <para>For a more advanced version of this function, see <c>GetFileInformationByHandleEx</c>.</para>
		/// <para>To set file information using a file handle, see <c>SetFileInformationByHandle</c>.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>A handle to the file that contains the information to be retrieved.</para>
		/// <para>This handle should not be a pipe handle.</para>
		/// </param>
		/// <param name="lpFileInformation">
		/// <para>A pointer to a <c>BY_HANDLE_FILE_INFORMATION</c> structure that receives the file information.</para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, the return value is nonzero and file information data is contained in the buffer pointed to by the lpFileInformation parameter.
		/// </para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI GetFileInformationByHandle( _In_ HANDLE hFile, _Out_ LPBY_HANDLE_FILE_INFORMATION lpFileInformation);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364952")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetFileInformationByHandle([In] SafeFileHandle hFile, [Out] out BY_HANDLE_FILE_INFORMATION lpFileInformation);

		/// <summary>
		/// <para>Retrieves the size of the specified file, in bytes.</para>
		/// <para>It is recommended that you use <c>GetFileSizeEx</c>.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>A handle to the file.</para>
		/// </param>
		/// <param name="lpFileSizeHigh">
		/// <para>
		/// A pointer to the variable where the high-order doubleword of the file size is returned. This parameter can be <c>NULL</c> if the application does not
		/// require the high-order doubleword.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, the return value is the low-order doubleword of the file size, and, if lpFileSizeHigh is non- <c>NULL</c>, the function
		/// puts the high-order doubleword of the file size into the variable pointed to by that parameter.
		/// </para>
		/// <para>
		/// If the function fails and lpFileSizeHigh is <c>NULL</c>, the return value is <c>INVALID_FILE_SIZE</c>. To get extended error information, call
		/// <c>GetLastError</c>. When lpFileSizeHigh is <c>NULL</c>, the results returned for large files are ambiguous, and you will not be able to determine
		/// the actual size of the file. It is recommended that you use <c>GetFileSizeEx</c> instead.
		/// </para>
		/// <para>
		/// If the function fails and lpFileSizeHigh is non- <c>NULL</c>, the return value is <c>INVALID_FILE_SIZE</c> and <c>GetLastError</c> will return a
		/// value other than <c>NO_ERROR</c>.
		/// </para>
		/// </returns>
		// DWORD WINAPI GetFileSize( _In_ HANDLE hFile, _Out_opt_ LPDWORD lpFileSizeHigh);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364955")]
		public static extern uint GetFileSize([In] SafeFileHandle hFile, out uint lpFileSizeHigh);

		/// <summary>
		/// <para>Retrieves the size of the specified file.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file. The handle must have been created with the <c>FILE_READ_ATTRIBUTES</c> access right or equivalent, or the caller must have
		/// sufficient permission on the directory that contains the file. For more information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="lpFileSize">
		/// <para>A pointer to a <c>LARGE_INTEGER</c> structure that receives the file size, in bytes.</para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI GetFileSizeEx( _In_ HANDLE hFile, _Out_ PLARGE_INTEGER lpFileSize);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364957")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetFileSizeEx([In] SafeFileHandle hFile, [Out] out long lpFileSize);

		/// <summary>
		/// <para>Retrieves the date and time that a file or directory was created, last accessed, and last modified.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file or directory for which dates and times are to be retrieved. The handle must have been created using the <c>CreateFile</c>
		/// function with the <c>GENERIC_READ</c> access right. For more information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="lpCreationTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure to receive the date and time the file or directory was created. This parameter can be <c>NULL</c> if the
		/// application does not require this information.
		/// </para>
		/// </param>
		/// <param name="lpLastAccessTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure to receive the date and time the file or directory was last accessed. The last access time includes the last
		/// time the file or directory was written to, read from, or, in the case of executable files, run. This parameter can be <c>NULL</c> if the application
		/// does not require this information.
		/// </para>
		/// </param>
		/// <param name="lpLastWriteTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure to receive the date and time the file or directory was last written to, truncated, or overwritten (for
		/// example, with <c>WriteFile</c> or <c>SetEndOfFile</c>). This date and time is not updated when file attributes or security descriptors are changed.
		/// This parameter can be <c>NULL</c> if the application does not require this information.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI GetFileTime( _In_ HANDLE hFile, _Out_opt_ LPFILETIME lpCreationTime, _Out_opt_ LPFILETIME lpLastAccessTime, _Out_opt_ LPFILETIME lpLastWriteTime);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "ms724320")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetFileTime([In] SafeFileHandle hFile, out FILETIME lpCreationTime, out FILETIME lpLastAccessTime, out FILETIME lpLastWriteTime);

		/// <summary>
		/// <para>Retrieves the file type of the specified file.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>A handle to the file.</para>
		/// </param>
		/// <returns>
		/// <para>The function returns one of the following values.</para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Return code/value</term>
		/// <term>Description</term>
		/// </listheader>
		/// <item>
		/// <term>FILE_TYPE_CHAR = 0x0002</term>
		/// <term>The specified file is a character file, typically an LPT device or a console.</term>
		/// </item>
		/// <item>
		/// <term>FILE_TYPE_DISK = 0x0001</term>
		/// <term>The specified file is a disk file.</term>
		/// </item>
		/// <item>
		/// <term>FILE_TYPE_PIPE = 0x0003</term>
		/// <term>The specified file is a socket, a named pipe, or an anonymous pipe.</term>
		/// </item>
		/// <item>
		/// <term>FILE_TYPE_REMOTE = 0x8000</term>
		/// <term>Unused.</term>
		/// </item>
		/// <item>
		/// <term>FILE_TYPE_UNKNOWN = 0x0000</term>
		/// <term>Either the type of the specified file is unknown, or the function failed.</term>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// You can distinguish between a "valid" return of <c>FILE_TYPE_UNKNOWN</c> and its return due to a calling error (for example, passing an invalid
		/// handle to <c>GetFileType</c>) by calling <c>GetLastError</c>.
		/// </para>
		/// <para>If the function worked properly and <c>FILE_TYPE_UNKNOWN</c> was returned, a call to <c>GetLastError</c> will return <c>NO_ERROR</c>.</para>
		/// <para>
		/// If the function returned <c>FILE_TYPE_UNKNOWN</c> due to an error in calling <c>GetFileType</c>, <c>GetLastError</c> will return the error code.
		/// </para>
		/// </returns>
		// DWORD WINAPI GetFileType( _In_ HANDLE hFile);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364960")]
		public static extern uint GetFileType([In] SafeFileHandle hFile);

		/// <summary>
		/// <para>Retrieves the final path for the specified file.</para>
		/// <para>For more information about file and path names, see Naming a File.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>A handle to a file or directory.</para>
		/// </param>
		/// <param name="lpszFilePath">
		/// <para>A pointer to a buffer that receives the path of hFile.</para>
		/// </param>
		/// <param name="cchFilePath">
		/// <para>The size of lpszFilePath, in <c>TCHAR</c> s. This value does not include a <c>NULL</c> termination character.</para>
		/// </param>
		/// <param name="dwFlags">
		/// <para>The type of result to return. This parameter can be one of the following values.</para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <term>Meaning</term>
		/// </listheader>
		/// <item>
		/// <term>FILE_NAME_NORMALIZED = 0x0</term>
		/// <term>Return the normalized drive name. This is the default.</term>
		/// </item>
		/// <item>
		/// <term>FILE_NAME_OPENED = 0x8</term>
		/// <term>Return the opened file name (not normalized).</term>
		/// </item>
		/// </list>
		/// </para>
		/// <para>This parameter can also include one of the following values.</para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <term>Meaning</term>
		/// </listheader>
		/// <item>
		/// <term>VOLUME_NAME_DOS = 0x0</term>
		/// <term>Return the path with the drive letter. This is the default.</term>
		/// </item>
		/// <item>
		/// <term>VOLUME_NAME_GUID = 0x1</term>
		/// <term>Return the path with a volume GUID path instead of the drive name.</term>
		/// </item>
		/// <item>
		/// <term>VOLUME_NAME_NONE = 0x4</term>
		/// <term>Return the path with no drive information.</term>
		/// </item>
		/// <item>
		/// <term>VOLUME_NAME_NT = 0x2</term>
		/// <term>Return the path with the volume device path.</term>
		/// </item>
		/// </list>
		/// </para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, the return value is the length of the string received by lpszFilePath, in <c>TCHAR</c> s. This value does not include the
		/// size of the terminating null character.
		/// </para>
		/// <para>
		/// <c>Windows Server 2008 and Windows Vista:</c> For the ANSI version of this function, <c>GetFinalPathNameByHandleA</c>, the return value includes the
		/// size of the terminating null character.
		/// </para>
		/// <para>
		/// If the function fails because lpszFilePath is too small to hold the string plus the terminating null character, the return value is the required
		/// buffer size, in <c>TCHAR</c> s. This value includes the size of the terminating null character.
		/// </para>
		/// <para>If the function fails for any other reason, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Return code</term>
		/// <term>Description</term>
		/// </listheader>
		/// <item>
		/// <term>ERROR_PATH_NOT_FOUND</term>
		/// <term>
		/// Can be returned if you are searching for a drive letter and one does not exist. For example, the handle was opened on a drive that is not currently
		/// mounted, or if you create a volume and do not assign it a drive letter. If a volume has no drive letter, you can use the volume GUID path to identify
		/// it.This return value can also be returned if you are searching for a volume GUID path on a network share. Volume GUID paths are not created for
		/// network shares.
		/// </term>
		/// </item>
		/// <item>
		/// <term>ERROR_NOT_ENOUGH_MEMORY</term>
		/// <term>Insufficient memory to complete the operation.</term>
		/// </item>
		/// <item>
		/// <term>ERROR_INVALID_PARAMETER</term>
		/// <term>Invalid flags were specified for dwFlags.</term>
		/// </item>
		/// </list>
		/// </para>
		/// </returns>
		// DWORD WINAPI GetFinalPathNameByHandle( _In_ HANDLE hFile, _Out_ LPTSTR lpszFilePath, _In_ DWORD cchFilePath, _In_ DWORD dwFlags);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364962")]
		public static extern uint GetFinalPathNameByHandle([In] SafeFileHandle hFile, StringBuilder lpszFilePath, uint cchFilePath, FinalPathNameOptions dwFlags);

		/// <summary>
		/// <para>Retrieves the full path and file name of the specified file.</para>
		/// <para>To perform this operation as a transacted operation, use the <c>GetFullPathNameTransacted</c> function.</para>
		/// <para>For more information about file and path names, see File Names, Paths, and Namespaces.</para>
		/// </summary>
		/// <param name="lpFileName">
		/// <para>The name of the file.</para>
		/// <para>This parameter can be a short (the 8.3 form) or long file name. This string can also be a share or volume name.</para>
		/// <para>
		/// In the ANSI version of this function, the name is limited to <c>MAX_PATH</c> characters. To extend this limit to 32,767 wide characters, call the
		/// Unicode version of the function ( <c>GetFullPathNameW</c>), and prepend "\\?\" to the path. For more information, see Naming a File.
		/// </para>
		/// </param>
		/// <param name="nBufferLength">
		/// <para>The size of the buffer to receive the null-terminated string for the drive and path, in <c>TCHARs</c>.</para>
		/// </param>
		/// <param name="lpBuffer">
		/// <para>A pointer to a buffer that receives the null-terminated string for the drive and path.</para>
		/// </param>
		/// <param name="lpFilePart">
		/// <para>A pointer to a buffer that receives the address (within lpBuffer) of the final file name component in the path.</para>
		/// <para>This parameter can be <c>NULL</c>.</para>
		/// <para>If lpBuffer refers to a directory and not a file, lpFilePart receives zero.</para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, the return value is the length, in <c>TCHARs</c>, of the string copied to lpBuffer, not including the terminating null character.
		/// </para>
		/// <para>
		/// If the lpBuffer buffer is too small to contain the path, the return value is the size, in <c>TCHARs</c>, of the buffer that is required to hold the
		/// path and the terminating null character.
		/// </para>
		/// <para>If the function fails for any other reason, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// DWORD WINAPI GetFullPathName( _In_ LPCTSTR lpFileName, _In_ DWORD nBufferLength, _Out_ LPTSTR lpBuffer, _Out_ LPTSTR *lpFilePart);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364963")]
		public static extern uint GetFullPathName([In] string lpFileName, uint nBufferLength, [Out] StringBuilder lpBuffer, out IntPtr lpFilePart);

		/// <summary>
		/// <para>Retrieves a bitmask representing the currently available disk drives.</para>
		/// </summary>
		/// <returns>
		/// <para>
		/// If the function succeeds, the return value is a bitmask representing the currently available disk drives. Bit position 0 (the least-significant bit)
		/// is drive A, bit position 1 is drive B, bit position 2 is drive C, and so on.
		/// </para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// DWORD WINAPI GetLogicalDrives(void);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364972")]
		public static extern uint GetLogicalDrives();

		/// <summary>
		/// <para>Fills a buffer with strings that specify valid drives in the system.</para>
		/// </summary>
		/// <param name="nBufferLength">
		/// <para>
		/// The maximum size of the buffer pointed to by lpBuffer, in <c>TCHARs</c>. This size does not include the terminating null character. If this parameter
		/// is zero, lpBuffer is not used.
		/// </para>
		/// </param>
		/// <param name="lpBuffer">
		/// <para>
		/// A pointer to a buffer that receives a series of null-terminated strings, one for each valid drive in the system, plus with an additional null
		/// character. Each string is a device name.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, the return value is the length, in characters, of the strings copied to the buffer, not including the terminating null
		/// character. Note that an ANSI-ASCII null character uses one byte, but a Unicode (UTF-16) null character uses two bytes.
		/// </para>
		/// <para>
		/// If the buffer is not large enough, the return value is greater than nBufferLength. It is the size of the buffer required to hold the drive strings.
		/// </para>
		/// <para>If the function fails, the return value is zero. To get extended error information, use the <c>GetLastError</c> function.</para>
		/// </returns>
		// DWORD WINAPI GetLogicalDriveStrings( _In_ DWORD nBufferLength, _Out_ LPTSTR lpBuffer);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364975")]
		public static extern uint GetLogicalDriveStrings(uint nBufferLength, [Out] StringBuilder lpBuffer);

		/// <summary>
		/// <para>Converts the specified path to its long form.</para>
		/// <para>To perform this operation as a transacted operation, use the <c>GetLongPathNameTransacted</c> function.</para>
		/// <para>For more information about file and path names, see Naming Files, Paths, and Namespaces.</para>
		/// </summary>
		/// <param name="lpszShortPath">
		/// <para>The path to be converted.</para>
		/// <para>
		/// In the ANSI version of this function, <c>GetLongPathNameA</c>, the name is limited to <c>MAX_PATH</c> (260) characters. To extend this limit to
		/// 32,767 wide characters, call the Unicode version of the function, <c>GetLongPathNameW</c>, and prepend "\\?\" to the path. For more information, see
		/// Naming Files, Paths, and Namespaces.
		/// </para>
		/// </param>
		/// <param name="lpszLongPath">
		/// <para>A pointer to the buffer to receive the long path.</para>
		/// <para>You can use the same buffer you used for the lpszShortPath parameter.</para>
		/// </param>
		/// <param name="cchBuffer">
		/// <para>The size of the buffer lpszLongPath points to, in <c>TCHARs</c>.</para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, the return value is the length, in <c>TCHARs</c>, of the string copied to lpszLongPath, not including the terminating null character.
		/// </para>
		/// <para>
		/// If the lpBuffer buffer is too small to contain the path, the return value is the size, in <c>TCHARs</c>, of the buffer that is required to hold the
		/// path and the terminating null character.
		/// </para>
		/// <para>
		/// If the function fails for any other reason, such as if the file does not exist, the return value is zero. To get extended error information, call <c>GetLastError</c>.
		/// </para>
		/// </returns>
		// DWORD WINAPI GetLongPathName( _In_ LPCTSTR lpszShortPath, _Out_ LPTSTR lpszLongPath, _In_ DWORD cchBuffer);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364980")]
		public static extern uint GetLongPathName([In] string lpszShortPath, [Out] StringBuilder lpszLongPath, uint cchBuffer);

		/// <summary>
		/// <para>Retrieves the short path form of the specified path.</para>
		/// <para>For more information about file and path names, see Naming Files, Paths, and Namespaces.</para>
		/// </summary>
		/// <param name="lpszLongPath">
		/// <para>The path string.</para>
		/// <para>
		/// In the ANSI version of this function, the name is limited to <c>MAX_PATH</c> characters. To extend this limit to 32,767 wide characters, call the
		/// Unicode version of the function and prepend "\\?\" to the path. For more information, see Naming Files, Paths, and Namespaces.
		/// </para>
		/// </param>
		/// <param name="lpszShortPath">
		/// <para>A pointer to a buffer to receive the null-terminated short form of the path that lpszLongPath specifies.</para>
		/// <para>Passing <c>NULL</c> for this parameter and zero for cchBuffer will always return the required buffer size for a specified lpszLongPath.</para>
		/// </param>
		/// <param name="cchBuffer">
		/// <para>The size of the buffer that lpszShortPath points to, in <c>TCHARs</c>.</para>
		/// <para>Set this parameter to zero if lpszShortPath is set to <c>NULL</c>.</para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, the return value is the length, in <c>TCHARs</c>, of the string that is copied to lpszShortPath, not including the
		/// terminating null character.
		/// </para>
		/// <para>
		/// If the lpszShortPath buffer is too small to contain the path, the return value is the size of the buffer, in <c>TCHARs</c>, that is required to hold
		/// the path and the terminating null character.
		/// </para>
		/// <para>If the function fails for any other reason, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// DWORD WINAPI GetShortPathName( _In_ LPCTSTR lpszLongPath, _Out_ LPTSTR lpszShortPath, _In_ DWORD cchBuffer);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364989")]
		public static extern uint GetShortPathName([In] string lpszLongPath, [Out] StringBuilder lpszShortPath, uint cchBuffer);

		/// <summary>
		/// <para>
		/// Creates a name for a temporary file. If a unique file name is generated, an empty file is created and the handle to it is released; otherwise, only a
		/// file name is generated.
		/// </para>
		/// </summary>
		/// <param name="lpPathName">
		/// <para>
		/// The directory path for the file name. Applications typically specify a period (.) for the current directory or the result of the <c>GetTempPath</c>
		/// function. The string cannot be longer than <c>MAX_PATH</c>�14 characters or <c>GetTempFileName</c> will fail. If this parameter is <c>NULL</c>, the
		/// function fails.
		/// </para>
		/// </param>
		/// <param name="lpPrefixString">
		/// <para>
		/// The null-terminated prefix string. The function uses up to the first three characters of this string as the prefix of the file name. This string must
		/// consist of characters in the OEM-defined character set.
		/// </para>
		/// </param>
		/// <param name="uUnique">
		/// <para>An unsigned integer to be used in creating the temporary file name. For more information, see Remarks.</para>
		/// <para>
		/// If uUnique is zero, the function attempts to form a unique file name using the current system time. If the file already exists, the number is
		/// increased by one and the functions tests if this file already exists. This continues until a unique filename is found; the function creates a file by
		/// that name and closes it. Note that the function does not attempt to verify the uniqueness of the file name when uUnique is nonzero.
		/// </para>
		/// </param>
		/// <param name="lpTempFileName">
		/// <para>
		/// A pointer to the buffer that receives the temporary file name. This buffer should be <c>MAX_PATH</c> characters to accommodate the path plus the
		/// terminating null character.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, the return value specifies the unique numeric value used in the temporary file name. If the uUnique parameter is nonzero,
		/// the return value specifies that same number.
		/// </para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// <para>The following is a possible return value.</para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Return value</term>
		/// <term>Description</term>
		/// </listheader>
		/// <item>
		/// <term>ERROR_BUFFER_OVERFLOW</term>
		/// <term>The length of the string pointed to by the lpPathName parameter is more than MAX_PATH�14 characters.</term>
		/// </item>
		/// </list>
		/// </para>
		/// </returns>
		// UINT WINAPI GetTempFileName( _In_ LPCTSTR lpPathName, _In_ LPCTSTR lpPrefixString, _In_ UINT uUnique, _Out_ LPTSTR lpTempFileName);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364991")]
		public static extern uint GetTempFileName([In] string lpPathName, [In] string lpPrefixString, uint uUnique, [Out, MarshalAs(UnmanagedType.LPTStr, SizeConst = MAX_PATH)] string lpTempFileName);

		/// <summary>
		/// <para>Retrieves the path of the directory designated for temporary files.</para>
		/// </summary>
		/// <param name="nBufferLength">
		/// <para>The size of the string buffer identified by lpBuffer, in <c>TCHARs</c>.</para>
		/// </param>
		/// <param name="lpBuffer">
		/// <para>
		/// A pointer to a string buffer that receives the null-terminated string specifying the temporary file path. The returned string ends with a backslash,
		/// for example, "C:\TEMP\".
		/// </para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, the return value is the length, in <c>TCHARs</c>, of the string copied to lpBuffer, not including the terminating null
		/// character. If the return value is greater than nBufferLength, the return value is the length, in <c>TCHARs</c>, of the buffer required to hold the path.
		/// </para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// <para>The maximum possible return value is <c>MAX_PATH</c>+1 (261).</para>
		/// </returns>
		// DWORD WINAPI GetTempPath( _In_ DWORD nBufferLength, _Out_ LPTSTR lpBuffer);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364992")]
		public static extern uint GetTempPath(uint nBufferLength, [Out] StringBuilder lpBuffer);

		/// <summary>Retrieves information about the file system and volume associated with the specified root directory.</summary>
		/// <param name="lpRootPathName">
		/// A pointer to a string that contains the root directory of the volume to be described.
		/// <para>
		/// If this parameter is NULL, the root of the current directory is used. A trailing backslash is required. For example, you specify \\MyServer\MyShare
		/// as "\\MyServer\MyShare\", or the C drive as "C:\".
		/// </para>
		/// </param>
		/// <param name="lpVolumeNameBuffer">
		/// A pointer to a buffer that receives the name of a specified volume. The buffer size is specified by the <paramref name="nVolumeNameSize"/> parameter.
		/// </param>
		/// <param name="nVolumeNameSize">
		/// The length of a volume name buffer, in TCHARs. The maximum buffer size is MAX_PATH+1.
		/// <para>This parameter is ignored if the volume name buffer is not supplied.</para>
		/// </param>
		/// <param name="lpVolumeSerialNumber">
		/// A pointer to a variable that receives the volume serial number.
		/// <para>This parameter can be NULL if the serial number is not required.</para>
		/// <para>
		/// This function returns the volume serial number that the operating system assigns when a hard disk is formatted. To programmatically obtain the hard
		/// disk's serial number that the manufacturer assigns, use the Windows Management Instrumentation (WMI) Win32_PhysicalMedia property SerialNumber.
		/// </para>
		/// </param>
		/// <param name="lpMaximumComponentLength">
		/// A pointer to a variable that receives the maximum length, in TCHARs, of a file name component that a specified file system supports.
		/// <para>A file name component is the portion of a file name between backslashes.</para>
		/// <para>
		/// The value that is stored in the variable that <paramref name="lpMaximumComponentLength"/> points to is used to indicate that a specified file system
		/// supports long names. For example, for a FAT file system that supports long names, the function stores the value 255, rather than the previous 8.3
		/// indicator. Long names can also be supported on systems that use the NTFS file system.
		/// </para>
		/// </param>
		/// <param name="lpFileSystemFlags">
		/// A pointer to a variable that receives flags associated with the specified file system.
		/// <para>
		/// This parameter can be one or more of the <c>FileSystemFlags</c> values. However, FILE_FILE_COMPRESSION and FILE_VOL_IS_COMPRESSED are mutually exclusive.
		/// </para>
		/// </param>
		/// <param name="lpFileSystemNameBuffer">
		/// A pointer to a buffer that receives the name of the file system, for example, the FAT file system or the NTFS file system. The buffer size is
		/// specified by the <paramref name="nFileSystemNameSize"/> parameter.
		/// </param>
		/// <param name="nFileSystemNameSize">
		/// The length of the file system name buffer, in TCHARs. The maximum buffer size is MAX_PATH+1.
		/// <para>This parameter is ignored if the file system name buffer is not supplied.</para>
		/// </param>
		/// <returns>
		/// If all the requested information is retrieved, the return value is nonzero.
		/// <para>If not all the requested information is retrieved, the return value is zero. To get extended error information, call GetLastError.</para>
		/// </returns>
		/// <remarks>
		/// When a user attempts to get information about a floppy drive that does not have a floppy disk, or a CD-ROM drive that does not have a compact disc,
		/// the system displays a message box for the user to insert a floppy disk or a compact disc, respectively. To prevent the system from displaying this
		/// message box, call the SetErrorMode function with SEM_FAILCRITICALERRORS.
		/// <para>
		/// The FILE_VOL_IS_COMPRESSED flag is the only indicator of volume-based compression. The file system name is not altered to indicate compression, for
		/// example, this flag is returned set on a DoubleSpace volume. When compression is volume-based, an entire volume is compressed or not compressed.
		/// </para>
		/// <para>
		/// The FILE_FILE_COMPRESSION flag indicates whether a file system supports file-based compression. When compression is file-based, individual files can
		/// be compressed or not compressed.
		/// </para>
		/// <para>The FILE_FILE_COMPRESSION and FILE_VOL_IS_COMPRESSED flags are mutually exclusive. Both bits cannot be returned set.</para>
		/// <para>
		/// The maximum component length value that is stored in lpMaximumComponentLength is the only indicator that a volume supports longer-than-normal FAT
		/// file system (or other file system) file names. The file system name is not altered to indicate support for long file names.
		/// </para>
		/// <para>
		/// The GetCompressedFileSize function obtains the compressed size of a file. The GetFileAttributes function can determine whether an individual file is compressed.
		/// </para>
		/// <para>Symbolic link behavior�</para>
		/// <para>If the path points to a symbolic link, the function returns volume information for the target.</para>
		/// </remarks>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364993")]
		public static extern bool GetVolumeInformation(string lpRootPathName, StringBuilder lpVolumeNameBuffer, int nVolumeNameSize, ref uint lpVolumeSerialNumber, ref uint lpMaximumComponentLength, ref FileSystemFlags lpFileSystemFlags, StringBuilder lpFileSystemNameBuffer, int nFileSystemNameSize);

		/// <summary>Retrieves information about the file system and volume associated with the specified root directory.</summary>
		/// <param name="rootPathName">
		/// A string that contains the root directory of the volume to be described.
		/// <para>
		/// If this parameter is NULL, the root of the current directory is used. A trailing backslash is required. For example, you specify \\MyServer\MyShare
		/// as "\\MyServer\MyShare\", or the C drive as "C:\".
		/// </para>
		/// </param>
		/// <param name="volumeName">Receives the name of a specified volume.</param>
		/// <param name="volumeSerialNumber">
		/// Receives the volume serial number.
		/// <para>
		/// This function returns the volume serial number that the operating system assigns when a hard disk is formatted. To programmatically obtain the hard
		/// disk's serial number that the manufacturer assigns, use the Windows Management Instrumentation (WMI) Win32_PhysicalMedia property SerialNumber.
		/// </para>
		/// </param>
		/// <param name="maximumComponentLength">
		/// Receives the maximum length, in characters, of a file name component that a specified file system supports.
		/// <para>A file name component is the portion of a file name between backslashes.</para>
		/// <para>
		/// The value that is stored in the variable that <paramref name="maximumComponentLength"/> returns is used to indicate that a specified file system
		/// supports long names. For example, for a FAT file system that supports long names, the function stores the value 255, rather than the previous 8.3
		/// indicator. Long names can also be supported on systems that use the NTFS file system.
		/// </para>
		/// </param>
		/// <param name="fileSystemFlags">
		/// Receives the flags associated with the specified file system.
		/// <para>
		/// This parameter can be one or more of the <c>FileSystemFlags</c> values. However, FILE_FILE_COMPRESSION and FILE_VOL_IS_COMPRESSED are mutually exclusive.
		/// </para>
		/// </param>
		/// <param name="fileSystemName">Receives the name of the file system, for example, the FAT file system or the NTFS file system.</param>
		/// <returns>
		/// If all the requested information is retrieved, the return value is nonzero.
		/// <para>If not all the requested information is retrieved, the return value is zero. To get extended error information, call GetLastError.</para>
		/// </returns>
		/// <remarks>
		/// When a user attempts to get information about a floppy drive that does not have a floppy disk, or a CD-ROM drive that does not have a compact disc,
		/// the system displays a message box for the user to insert a floppy disk or a compact disc, respectively. To prevent the system from displaying this
		/// message box, call the SetErrorMode function with SEM_FAILCRITICALERRORS.
		/// <para>
		/// The FILE_VOL_IS_COMPRESSED flag is the only indicator of volume-based compression. The file system name is not altered to indicate compression, for
		/// example, this flag is returned set on a DoubleSpace volume. When compression is volume-based, an entire volume is compressed or not compressed.
		/// </para>
		/// <para>
		/// The FILE_FILE_COMPRESSION flag indicates whether a file system supports file-based compression. When compression is file-based, individual files can
		/// be compressed or not compressed.
		/// </para>
		/// <para>The FILE_FILE_COMPRESSION and FILE_VOL_IS_COMPRESSED flags are mutually exclusive. Both bits cannot be returned set.</para>
		/// <para>
		/// The maximum component length value that is stored in lpMaximumComponentLength is the only indicator that a volume supports longer-than-normal FAT
		/// file system (or other file system) file names. The file system name is not altered to indicate support for long file names.
		/// </para>
		/// <para>
		/// The GetCompressedFileSize function obtains the compressed size of a file. The GetFileAttributes function can determine whether an individual file is compressed.
		/// </para>
		/// <para>Symbolic link behavior�</para>
		/// <para>If the path points to a symbolic link, the function returns volume information for the target.</para>
		/// </remarks>
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364993")]
		public static bool GetVolumeInformation(string rootPathName, out string volumeName, out uint volumeSerialNumber,
			out uint maximumComponentLength, out FileSystemFlags fileSystemFlags, out string fileSystemName)
		{
			var sb1 = new StringBuilder(MAX_PATH + 1);
			var sn = 0U;
			var cl = 0U;
			FileSystemFlags flags = 0;
			var sb2 = new StringBuilder(MAX_PATH + 1);
			var ret = GetVolumeInformation(rootPathName, sb1, sb1.Capacity, ref sn, ref cl, ref flags, sb2, sb2.Capacity);
			volumeName = sb1.ToString();
			volumeSerialNumber = sn;
			maximumComponentLength = cl;
			fileSystemFlags = flags;
			fileSystemName = sb2.ToString();
			return ret;
		}

		/// <summary>
		/// <para>Retrieves information about the file system and volume associated with the specified file.</para>
		/// <para>To retrieve the current compression state of a file or directory, use <c>FSCTL_GET_COMPRESSION</c>.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>A handle to the file.</para>
		/// </param>
		/// <param name="lpVolumeNameBuffer">
		/// <para>A pointer to a buffer that receives the name of a specified volume. The maximum buffer size is .</para>
		/// </param>
		/// <param name="nVolumeNameSize">
		/// <para>The length of a volume name buffer, in <c>WCHAR</c> s. The maximum buffer size is .</para>
		/// <para>This parameter is ignored if the volume name buffer is not supplied.</para>
		/// </param>
		/// <param name="lpVolumeSerialNumber">
		/// <para>A pointer to a variable that receives the volume serial number.</para>
		/// <para>This parameter can be <c>NULL</c> if the serial number is not required.</para>
		/// <para>
		/// This function returns the volume serial number that the operating system assigns when a hard disk is formatted. To programmatically obtain the hard
		/// disk's serial number that the manufacturer assigns, use the Windows Management Instrumentation (WMI) <c>Win32_PhysicalMedia</c> property <c>SerialNumber</c>.
		/// </para>
		/// </param>
		/// <param name="lpMaximumComponentLength">
		/// <para>A pointer to a variable that receives the maximum length, in <c>WCHAR</c> s, of a file name component that a specified file system supports.</para>
		/// <para>A file name component is the portion of a file name between backslashes.</para>
		/// <para>
		/// The value that is stored in the variable that *lpMaximumComponentLength points to is used to indicate that a specified file system supports long
		/// names. For example, for a FAT file system that supports long names, the function stores the value 255, rather than the previous 8.3 indicator. Long
		/// names can also be supported on systems that use the NTFS file system.
		/// </para>
		/// </param>
		/// <param name="lpFileSystemFlags">
		/// <para>A pointer to a variable that receives flags associated with the specified file system.</para>
		/// <para>
		/// This parameter can be one or more of the following flags. However, <c>FILE_FILE_COMPRESSION</c> and <c>FILE_VOL_IS_COMPRESSED</c> are mutually exclusive.
		/// </para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <term>Meaning</term>
		/// </listheader>
		/// <item>
		/// <term>FILE_CASE_PRESERVED_NAMES0x00000002</term>
		/// <term>The specified volume supports preserved case of file names when it places a name on disk.</term>
		/// </item>
		/// <item>
		/// <term>FILE_CASE_SENSITIVE_SEARCH0x00000001</term>
		/// <term>The specified volume supports case-sensitive file names.</term>
		/// </item>
		/// <item>
		/// <term>FILE_FILE_COMPRESSION0x00000010</term>
		/// <term>The specified volume supports file-based compression.</term>
		/// </item>
		/// <item>
		/// <term>FILE_NAMED_STREAMS0x00040000</term>
		/// <term>The specified volume supports named streams.</term>
		/// </item>
		/// <item>
		/// <term>FILE_PERSISTENT_ACLS0x00000008</term>
		/// <term>
		/// The specified volume preserves and enforces access control lists (ACL). For example, the NTFS file system preserves and enforces ACLs, and the FAT
		/// file system does not.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_READ_ONLY_VOLUME0x00080000</term>
		/// <term>The specified volume is read-only.</term>
		/// </item>
		/// <item>
		/// <term>FILE_SEQUENTIAL_WRITE_ONCE0x00100000</term>
		/// <term>The specified volume supports a single sequential write.</term>
		/// </item>
		/// <item>
		/// <term>FILE_SUPPORTS_ENCRYPTION0x00020000</term>
		/// <term>The specified volume supports the Encrypted File System (EFS). For more information, see File Encryption.</term>
		/// </item>
		/// <item>
		/// <term>FILE_SUPPORTS_EXTENDED_ATTRIBUTES0x00800000</term>
		/// <term>
		/// The specified volume supports extended attributes. An extended attribute is a piece of application-specific metadata that an application can
		/// associate with a file and is not part of the file&amp;#39;s data.Windows Vista and Windows Server 2008: This value is not supported.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_SUPPORTS_HARD_LINKS0x00400000</term>
		/// <term>
		/// The specified volume supports hard links. For more information, see Hard Links and Junctions.Windows Vista and Windows Server 2008: This value is not supported.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_SUPPORTS_OBJECT_IDS0x00010000</term>
		/// <term>The specified volume supports object identifiers.</term>
		/// </item>
		/// <item>
		/// <term>FILE_SUPPORTS_OPEN_BY_FILE_ID0x01000000</term>
		/// <term>
		/// The file system supports open by FileID. For more information, see FILE_ID_BOTH_DIR_INFO.Windows Vista and Windows Server 2008: This value is not supported.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_SUPPORTS_REPARSE_POINTS0x00000080</term>
		/// <term>The specified volume supports re-parse points.</term>
		/// </item>
		/// <item>
		/// <term>FILE_SUPPORTS_SPARSE_FILES0x00000040</term>
		/// <term>The specified volume supports sparse files.</term>
		/// </item>
		/// <item>
		/// <term>FILE_SUPPORTS_TRANSACTIONS0x00200000</term>
		/// <term>The specified volume supports transactions. For more information, see About KTM.</term>
		/// </item>
		/// <item>
		/// <term>FILE_SUPPORTS_USN_JOURNAL0x02000000</term>
		/// <term>
		/// The specified volume supports update sequence number (USN) journals. For more information, see Change Journal Records.Windows Vista and Windows
		/// Server 2008: This value is not supported.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_UNICODE_ON_DISK0x00000004</term>
		/// <term>The specified volume supports Unicode in file names as they appear on disk.</term>
		/// </item>
		/// <item>
		/// <term>FILE_VOLUME_IS_COMPRESSED0x00008000</term>
		/// <term>The specified volume is a compressed volume.</term>
		/// </item>
		/// <item>
		/// <term>FILE_VOLUME_QUOTAS0x00000020</term>
		/// <term>The specified volume supports disk quotas.</term>
		/// </item>
		/// </list>
		/// </para>
		/// </param>
		/// <param name="lpFileSystemNameBuffer">
		/// <para>
		/// A pointer to a buffer that receives the name of the file system, for example, the FAT file system or the NTFS file system. The buffer size is
		/// specified by the nFileSystemNameSize parameter.
		/// </para>
		/// </param>
		/// <param name="nFileSystemNameSize">
		/// <para>The length of the file system name buffer, in <c>WCHAR</c> s. The maximum buffer size is .</para>
		/// <para>This parameter is ignored if the file system name buffer is not supplied.</para>
		/// </param>
		/// <returns>
		/// <para>If all the requested information is retrieved, the return value is nonzero.</para>
		/// <para>If not all the requested information is retrieved, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI GetVolumeInformationByHandleW( _In_ HANDLE hFile, _Out_opt_ LPWSTR lpVolumeNameBuffer, _In_ DWORD nVolumeNameSize, _Out_opt_ LPDWORD
		// lpVolumeSerialNumber, _Out_opt_ LPDWORD lpMaximumComponentLength, _Out_opt_ LPDWORD lpFileSystemFlags, _Out_opt_ LPWSTR lpFileSystemNameBuffer, _In_
		// DWORD nFileSystemNameSize);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa964920")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern uint GetVolumeInformationByHandle([In] SafeFileHandle hFile, [Out] StringBuilder lpVolumeNameBuffer, uint nVolumeNameSize, out uint lpVolumeSerialNumber, out uint lpMaximumComponentLength, out FileSystemFlags lpFileSystemFlags, [Out] StringBuilder lpFileSystemNameBuffer, uint nFileSystemNameSize);

		/// <summary>
		/// <para>
		/// Retrieves a volume <c>GUID</c> path for the volume that is associated with the specified volume mount point ( drive letter, volume <c>GUID</c> path,
		/// or mounted folder).
		/// </para>
		/// </summary>
		/// <param name="lpszVolumeMountPoint">
		/// <para>
		/// A pointer to a string that contains the path of a mounted folder (for example, "Y:\MountX\") or a drive letter (for example, "X:\"). The string must
		/// end with a trailing backslash ('\').
		/// </para>
		/// </param>
		/// <param name="lpszVolumeName">
		/// <para>
		/// A pointer to a string that receives the volume <c>GUID</c> path. This path is of the form "\\?\Volume{GUID}\" where GUID is a <c>GUID</c> that
		/// identifies the volume. If there is more than one volume <c>GUID</c> path for the volume, only the first one in the mount manager's cache is returned.
		/// </para>
		/// </param>
		/// <param name="cchBufferLength">
		/// <para>
		/// The length of the output buffer, in <c>TCHARs</c>. A reasonable size for the buffer to accommodate the largest possible volume <c>GUID</c> path is 50 characters.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI GetVolumeNameForVolumeMountPoint( _In_ LPCTSTR lpszVolumeMountPoint, _Out_ LPTSTR lpszVolumeName, _In_ DWORD cchBufferLength);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364994")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetVolumeNameForVolumeMountPoint([In] string lpszVolumeMountPoint, [Out] StringBuilder lpszVolumeName, uint cchBufferLength);

		/// <summary>
		/// <para>Retrieves the volume mount point where the specified path is mounted.</para>
		/// </summary>
		/// <param name="lpszFileName">
		/// <para>A pointer to the input path string. Both absolute and relative file and directory names, for example "..", are acceptable in this path.</para>
		/// <para>
		/// If you specify a relative directory or file name without a volume qualifier, <c>GetVolumePathName</c> returns the drive letter of the boot volume.
		/// </para>
		/// <para>If this parameter is an empty string, "", the function fails but the last error is set to <c>ERROR_SUCCESS</c>.</para>
		/// </param>
		/// <param name="lpszVolumePathName">
		/// <para>A pointer to a string that receives the volume mount point for the input path.</para>
		/// </param>
		/// <param name="cchBufferLength">
		/// <para>The length of the output buffer, in <c>TCHARs</c>.</para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI GetVolumePathName( _In_ LPCTSTR lpszFileName, _Out_ LPTSTR lpszVolumePathName, _In_ DWORD cchBufferLength);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364996")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetVolumePathName([In] string lpszFileName, [Out] StringBuilder lpszVolumePathName, uint cchBufferLength);

		/// <summary>
		/// <para>Retrieves a list of drive letters and mounted folder paths for the specified volume.</para>
		/// </summary>
		/// <param name="lpszVolumeName">
		/// <para>A volume <c>GUID</c> path for the volume. A volume <c>GUID</c> path is of the form "\\?\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\".</para>
		/// </param>
		/// <param name="lpszVolumePathNames">
		/// <para>
		/// A pointer to a buffer that receives the list of drive letters and mounted folder paths. The list is an array of null-terminated strings terminated by
		/// an additional <c>NULL</c> character. If the buffer is not large enough to hold the complete list, the buffer holds as much of the list as possible.
		/// </para>
		/// </param>
		/// <param name="cchBufferLength">
		/// <para>The length of the lpszVolumePathNames buffer, in <c>TCHARs</c>, including all <c>NULL</c> characters.</para>
		/// </param>
		/// <param name="lpcchReturnLength">
		/// <para>
		/// If the call is successful, this parameter is the number of <c>TCHARs</c> copied to the lpszVolumePathNames buffer. Otherwise, this parameter is the
		/// size of the buffer required to hold the complete list, in <c>TCHARs</c>.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>
		/// If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>. If the buffer is not large enough to
		/// hold the complete list, the error code is <c>ERROR_MORE_DATA</c> and the lpcchReturnLength parameter receives the required buffer size.
		/// </para>
		/// </returns>
		// BOOL WINAPI GetVolumePathNamesForVolumeName( _In_ LPCTSTR lpszVolumeName, _Out_ LPTSTR lpszVolumePathNames, _In_ DWORD cchBufferLength, _Out_ PDWORD lpcchReturnLength);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364998")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetVolumePathNamesForVolumeName([In] string lpszVolumeName, [Out] StringBuilder lpszVolumePathNames, uint cchBufferLength, [Out] out uint lpcchReturnLength);

		/// <summary>
		/// <para>Converts a local file time to a file time based on the Coordinated Universal Time (UTC).</para>
		/// </summary>
		/// <param name="lpLocalFileTime">
		/// <para>A pointer to a <c>FILETIME</c> structure that specifies the local file time to be converted into a UTC-based file time.</para>
		/// </param>
		/// <param name="lpFileTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure to receive the converted UTC-based file time. This parameter cannot be the same as the lpLocalFileTime parameter.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, use the <c>GetLastError</c> function.</para>
		/// </returns>
		// BOOL WINAPI LocalFileTimeToFileTime( _In_ const FILETIME *lpLocalFileTime, _Out_ LPFILETIME lpFileTime);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "ms724490")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LocalFileTimeToFileTime([In] ref FILETIME lpLocalFileTime, [Out] out FILETIME lpFileTime);

		/// <summary>
		/// <para>Locks the specified file for exclusive access by the calling process.</para>
		/// <para>To specify additional options, for example creating a shared lock or for block-on-fail operation, use the <c>LockFileEx</c> function.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file. The file handle must have been created with the <c>GENERIC_READ</c> or <c>GENERIC_WRITE</c> access right. For more information,
		/// see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="dwFileOffsetLow">
		/// <para>The low-order 32 bits of the starting byte offset in the file where the lock should begin.</para>
		/// </param>
		/// <param name="dwFileOffsetHigh">
		/// <para>The high-order 32 bits of the starting byte offset in the file where the lock should begin.</para>
		/// </param>
		/// <param name="nNumberOfBytesToLockLow">
		/// <para>The low-order 32 bits of the length of the byte range to be locked.</para>
		/// </param>
		/// <param name="nNumberOfBytesToLockHigh">
		/// <para>The high-order 32 bits of the length of the byte range to be locked.</para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero ( <c>TRUE</c>).</para>
		/// <para>If the function fails, the return value is zero ( <c>FALSE</c>). To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI LockFile( _In_ HANDLE hFile, _In_ DWORD dwFileOffsetLow, _In_ DWORD dwFileOffsetHigh, _In_ DWORD nNumberOfBytesToLockLow, _In_ DWORD nNumberOfBytesToLockHigh);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365202")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LockFile([In] SafeFileHandle hFile, uint dwFileOffsetLow, uint dwFileOffsetHigh, uint nNumberOfBytesToLockLow, uint nNumberOfBytesToLockHigh);

		/// <summary>
		/// <para>
		/// Locks the specified file for exclusive access by the calling process. This function can operate either synchronously or asynchronously and can
		/// request either an exclusive or a shared lock.
		/// </para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file. The handle must have been created with either the <c>GENERIC_READ</c> or <c>GENERIC_WRITE</c> access right. For more
		/// information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="dwFlags">
		/// <para>This parameter may be one or more of the following values.</para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <term>Meaning</term>
		/// </listheader>
		/// <item>
		/// <term>LOCKFILE_EXCLUSIVE_LOCK = 0x00000002</term>
		/// <term>The function requests an exclusive lock. Otherwise, it requests a shared lock.</term>
		/// </item>
		/// <item>
		/// <term>LOCKFILE_FAIL_IMMEDIATELY = 0x00000001</term>
		/// <term>The function returns immediately if it is unable to acquire the requested lock. Otherwise, it waits.</term>
		/// </item>
		/// </list>
		/// </para>
		/// </param>
		/// <param name="dwReserved">
		/// <para>Reserved parameter; must be set to zero.</para>
		/// </param>
		/// <param name="nNumberOfBytesToLockLow">
		/// <para>The low-order 32 bits of the length of the byte range to lock.</para>
		/// </param>
		/// <param name="nNumberOfBytesToLockHigh">
		/// <para>The high-order 32 bits of the length of the byte range to lock.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// <para>
		/// A pointer to an <c>OVERLAPPED</c> structure that the function uses with the locking request. This structure, which is required, contains the file
		/// offset of the beginning of the lock range. You must initialize the <c>hEvent</c> member to a valid handle or zero.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero ( <c>TRUE</c>).</para>
		/// <para>If the function fails, the return value is zero ( <c>FALSE</c>). To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI LockFileEx( _In_ HANDLE hFile, _In_ DWORD dwFlags, _Reserved_ DWORD dwReserved, _In_ DWORD nNumberOfBytesToLockLow, _In_ DWORD
		// nNumberOfBytesToLockHigh, _Inout_ LPOVERLAPPED lpOverlapped);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365203")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern unsafe bool LockFileEx([In] IntPtr hFile, LOCKFILE dwFlags, uint dwReserved, uint nNumberOfBytesToLockLow, uint nNumberOfBytesToLockHigh, NativeOverlapped* lpOverlapped);

		/// <summary>
		/// Retrieves information about MS-DOS device names. The function can obtain the current mapping for a particular MS-DOS device name. The function can
		/// also obtain a list of all existing MS-DOS device names.
		/// <para>
		/// MS-DOS device names are stored as junctions in the object namespace. The code that converts an MS-DOS path into a corresponding path uses these
		/// junctions to map MS-DOS devices and drive letters. The QueryDosDevice function enables an application to query the names of the junctions used to
		/// implement the MS-DOS device namespace as well as the value of each specific junction.
		/// </para>
		/// </summary>
		/// <param name="lpDeviceName">
		/// An MS-DOS device name string specifying the target of the query. The device name cannot have a trailing backslash; for example, use "C:", not "C:\".
		/// <para>
		/// This parameter can be NULL. In that case, the QueryDosDevice function will store a list of all existing MS-DOS device names into the buffer pointed
		/// to by <paramref name="lpTargetPath"/>.
		/// </para>
		/// </param>
		/// <param name="lpTargetPath">
		/// A pointer to a buffer that will receive the result of the query. The function fills this buffer with one or more null-terminated strings. The final
		/// null-terminated string is followed by an additional NULL.
		/// <para>
		/// If <paramref name="lpDeviceName"/> is non-NULL, the function retrieves information about the particular MS-DOS device specified by
		/// <paramref name="lpDeviceName"/>. The first null-terminated string stored into the buffer is the current mapping for the device. The other
		/// null-terminated strings represent undeleted prior mappings for the device.
		/// </para>
		/// <para>
		/// If <paramref name="lpDeviceName"/> is NULL, the function retrieves a list of all existing MS-DOS device names. Each null-terminated string stored
		/// into the buffer is the name of an existing MS-DOS device, for example, \Device\HarddiskVolume1 or \Device\Floppy0.
		/// </para>
		/// </param>
		/// <param name="ucchMax">The maximum number of TCHARs that can be stored into the buffer pointed to by <paramref name="lpTargetPath"/>.</param>
		/// <returns>
		/// If the function succeeds, the return value is the number of TCHARs stored into the buffer pointed to by <paramref name="lpTargetPath"/>.
		/// <para>If the function fails, the return value is zero. To get extended error information, call GetLastError.</para>
		/// <para>If the buffer is too small, the function fails and the last error code is ERROR_INSUFFICIENT_BUFFER.</para>
		/// </returns>
		/// <remarks>
		/// The DefineDosDevice function enables an application to create and modify the junctions used to implement the MS-DOS device namespace.
		/// <para>
		/// <c>Windows Server 2003 and Windows XP:</c><c>QueryDosDevice</c> first searches the Local MS-DOS Device namespace for the specified device name. If
		/// the device name is not found, the function will then search the Global MS-DOS Device namespace.
		/// </para>
		/// <para>
		/// When all existing MS-DOS device names are queried, the list of device names that are returned is dependent on whether it is running in the
		/// "LocalSystem" context. If so, only the device names included in the Global MS-DOS Device namespace will be returned. If not, a concatenation of the
		/// device names in the Global and Local MS-DOS Device namespaces will be returned. If a device name exists in both namespaces, <c>QueryDosDevice</c>
		/// will return the entry in the Local MS-DOS Device namespace.
		/// </para>
		/// </remarks>
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365461")]
		public static extern int QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

		/// <summary>
		/// Retrieves information about MS-DOS device names. The function can obtain the current mapping for a particular MS-DOS device name. The function can
		/// also obtain a list of all existing MS-DOS device names.
		/// <para>
		/// MS-DOS device names are stored as junctions in the object namespace. The code that converts an MS-DOS path into a corresponding path uses these
		/// junctions to map MS-DOS devices and drive letters. The QueryDosDevice function enables an application to query the names of the junctions used to
		/// implement the MS-DOS device namespace as well as the value of each specific junction.
		/// </para>
		/// </summary>
		/// <param name="deviceName">
		/// An MS-DOS device name string specifying the target of the query. The device name cannot have a trailing backslash; for example, use "C:", not "C:\".
		/// <para>This parameter can be NULL. In that case, the QueryDosDevice function will return a list of all existing MS-DOS device names.</para>
		/// </param>
		/// <returns>
		/// <para>
		/// If <paramref name="deviceName"/> is non-NULL, the function returns information about the particular MS-DOS device specified by
		/// <paramref name="deviceName"/>. The first string returned is the current mapping for the device. The other strings represent undeleted prior mappings
		/// for the device.
		/// </para>
		/// <para>
		/// If <paramref name="deviceName"/> is NULL, the function returns a list of all existing MS-DOS device names. Each string returned is the name of an
		/// existing MS-DOS device, for example, \Device\HarddiskVolume1 or \Device\Floppy0.
		/// </para>
		/// </returns>
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365461")]
		public static IEnumerable<string> QueryDosDevice(string deviceName)
		{
			deviceName = deviceName?.TrimEnd('\\');
			var bytes = 16;
			var retLen = 0U;
			using (var mem = new SafeHGlobalHandle(0))
			{
				do
				{
					mem.Size = (bytes *= 4);
					retLen = QueryDosDevice(deviceName, (IntPtr)mem, mem.Size / Marshal.SystemDefaultCharSize);
				} while (retLen == 0 && Win32Error.GetLastError() == Win32Error.ERROR_INSUFFICIENT_BUFFER);
				if (retLen == 0) throw new Win32Exception();
				return mem.ToStringEnum().ToArray();
			}
		}

		/// <summary>
		/// Reads data from the specified file or input/output (I/O) device. Reads occur at the position specified by the file pointer if supported by the device.
		/// </summary>
		/// <param name="hFile">
		/// A handle to the device (for example, a file, file stream, physical disk, volume, console buffer, tape drive, socket, communications resource,
		/// mailslot, or pipe). The hFile parameter must have been created with read access.
		/// </param>
		/// <param name="lpBuffer">A pointer to the buffer that receives the data read from a file or device.</param>
		/// <param name="nNumberOfBytesToRead">The maximum number of bytes to be read.</param>
		/// <param name="lpNumberOfBytesRead">
		/// A pointer to the variable that receives the number of bytes read when using a synchronous hFile parameter. ReadFile sets this value to zero before
		/// doing any work or error checking. Use NULL for this parameter if this is an asynchronous operation to avoid potentially erroneous results.
		/// <para>This parameter can be NULL only when the lpOverlapped parameter is not NULL.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// A pointer to an OVERLAPPED structure is required if the hFile parameter was opened with FILE_FLAG_OVERLAPPED, otherwise it can be NULL.
		/// <para>
		/// If hFile is opened with FILE_FLAG_OVERLAPPED, the lpOverlapped parameter must point to a valid and unique OVERLAPPED structure, otherwise the
		/// function can incorrectly report that the read operation is complete.
		/// </para>
		/// <para>
		/// For an hFile that supports byte offsets, if you use this parameter you must specify a byte offset at which to start reading from the file or device.
		/// This offset is specified by setting the Offset and OffsetHigh members of the OVERLAPPED structure. For an hFile that does not support byte offsets,
		/// Offset and OffsetHigh are ignored.
		/// </para>
		/// <para>
		/// For more information about different combinations of lpOverlapped and FILE_FLAG_OVERLAPPED, see the Remarks section and the Synchronization and File
		/// Position section.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero (TRUE). If the function fails, or is completing asynchronously, the return value is
		/// zero(FALSE). To get extended error information, call the GetLastError function.
		/// </returns>
		[DllImport(Lib.Kernel32, ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365467")]
		public static extern bool ReadFile(SafeFileHandle hFile, IntPtr lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

		/// <summary>
		/// Reads data from the specified file or input/output (I/O) device. Reads occur at the position specified by the file pointer if supported by the device.
		/// </summary>
		/// <param name="hFile">
		/// A handle to the device (for example, a file, file stream, physical disk, volume, console buffer, tape drive, socket, communications resource,
		/// mailslot, or pipe). The hFile parameter must have been created with read access.
		/// </param>
		/// <param name="lpBuffer">A pointer to the buffer that receives the data read from a file or device.</param>
		/// <param name="nNumberOfBytesToRead">The maximum number of bytes to be read.</param>
		/// <param name="lpNumberOfBytesRead">
		/// A pointer to the variable that receives the number of bytes read when using a synchronous hFile parameter. ReadFile sets this value to zero before
		/// doing any work or error checking. Use NULL for this parameter if this is an asynchronous operation to avoid potentially erroneous results.
		/// <para>This parameter can be NULL only when the lpOverlapped parameter is not NULL.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// A pointer to an OVERLAPPED structure is required if the hFile parameter was opened with FILE_FLAG_OVERLAPPED, otherwise it can be NULL.
		/// <para>
		/// If hFile is opened with FILE_FLAG_OVERLAPPED, the lpOverlapped parameter must point to a valid and unique OVERLAPPED structure, otherwise the
		/// function can incorrectly report that the read operation is complete.
		/// </para>
		/// <para>
		/// For an hFile that supports byte offsets, if you use this parameter you must specify a byte offset at which to start reading from the file or device.
		/// This offset is specified by setting the Offset and OffsetHigh members of the OVERLAPPED structure. For an hFile that does not support byte offsets,
		/// Offset and OffsetHigh are ignored.
		/// </para>
		/// <para>
		/// For more information about different combinations of lpOverlapped and FILE_FLAG_OVERLAPPED, see the Remarks section and the Synchronization and File
		/// Position section.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero (TRUE). If the function fails, or is completing asynchronously, the return value is
		/// zero(FALSE). To get extended error information, call the GetLastError function.
		/// </returns>
		[DllImport(Lib.Kernel32, ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365467")]
		public static extern unsafe bool ReadFile(SafeFileHandle hFile, byte* lpBuffer, uint nNumberOfBytesToRead, IntPtr lpNumberOfBytesRead, NativeOverlapped* lpOverlapped);

		/// <summary>
		/// Reads data from the specified file or input/output (I/O) device. Reads occur at the position specified by the file pointer if supported by the device.
		/// </summary>
		/// <param name="hFile">
		/// A handle to the device (for example, a file, file stream, physical disk, volume, console buffer, tape drive, socket, communications resource,
		/// mailslot, or pipe). The hFile parameter must have been created with read access.
		/// </param>
		/// <param name="lpBuffer">A pointer to the buffer that receives the data read from a file or device.</param>
		/// <param name="nNumberOfBytesToRead">The maximum number of bytes to be read.</param>
		/// <param name="lpNumberOfBytesRead">
		/// A pointer to the variable that receives the number of bytes read when using a synchronous hFile parameter. ReadFile sets this value to zero before
		/// doing any work or error checking. Use NULL for this parameter if this is an asynchronous operation to avoid potentially erroneous results.
		/// <para>This parameter can be NULL only when the lpOverlapped parameter is not NULL.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// A pointer to an OVERLAPPED structure is required if the hFile parameter was opened with FILE_FLAG_OVERLAPPED, otherwise it can be NULL.
		/// <para>
		/// If hFile is opened with FILE_FLAG_OVERLAPPED, the lpOverlapped parameter must point to a valid and unique OVERLAPPED structure, otherwise the
		/// function can incorrectly report that the read operation is complete.
		/// </para>
		/// <para>
		/// For an hFile that supports byte offsets, if you use this parameter you must specify a byte offset at which to start reading from the file or device.
		/// This offset is specified by setting the Offset and OffsetHigh members of the OVERLAPPED structure. For an hFile that does not support byte offsets,
		/// Offset and OffsetHigh are ignored.
		/// </para>
		/// <para>
		/// For more information about different combinations of lpOverlapped and FILE_FLAG_OVERLAPPED, see the Remarks section and the Synchronization and File
		/// Position section.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero (TRUE). If the function fails, or is completing asynchronously, the return value is
		/// zero(FALSE). To get extended error information, call the GetLastError function.
		/// </returns>
		[DllImport(Lib.Kernel32, ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365467")]
		public static extern bool ReadFile(SafeFileHandle hFile, byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

		/// <summary>
		/// <para>
		/// Reads data from the specified file or input/output (I/O) device. It reports its completion status asynchronously, calling the specified completion
		/// routine when reading is completed or canceled and the calling thread is in an alertable wait state.
		/// </para>
		/// <para>To read data from a file or device synchronously, use the <c>ReadFile</c> function.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file or I/O device (for example, a file, file stream, physical disk, volume, console buffer, tape drive, socket, communications
		/// resource, mailslot, or pipe).
		/// </para>
		/// <para>
		/// This parameter can be any handle opened with the <c>FILE_FLAG_OVERLAPPED</c> flag by the <c>CreateFile</c> function, or a socket handle returned by
		/// the <c>socket</c> or <c>accept</c> function.
		/// </para>
		/// <para>This handle also must have the <c>GENERIC_READ</c> access right. For more information on access rights, see File Security and Access Rights.</para>
		/// </param>
		/// <param name="lpBuffer">
		/// <para>A pointer to a buffer that receives the data read from the file or device.</para>
		/// <para>
		/// This buffer must remain valid for the duration of the read operation. The application should not use this buffer until the read operation is completed.
		/// </para>
		/// </param>
		/// <param name="nNumberOfBytesToRead">
		/// <para>The number of bytes to be read.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// <para>A pointer to an <c>OVERLAPPED</c> data structure that supplies data to be used during the asynchronous (overlapped) file read operation.</para>
		/// <para>
		/// For files that support byte offsets, you must specify a byte offset at which to start reading from the file. You specify this offset by setting the
		/// <c>Offset</c> and <c>OffsetHigh</c> members of the <c>OVERLAPPED</c> structure. For files or devices that do not support byte offsets, <c>Offset</c>
		/// and <c>OffsetHigh</c> are ignored.
		/// </para>
		/// <para>
		/// The <c>ReadFileEx</c> function ignores the <c>OVERLAPPED</c> structure's <c>hEvent</c> member. An application is free to use that member for its own
		/// purposes in the context of a <c>ReadFileEx</c> call. <c>ReadFileEx</c> signals completion of its read operation by calling, or queuing a call to, the
		/// completion routine pointed to by lpCompletionRoutine, so it does not need an event handle.
		/// </para>
		/// <para>
		/// The <c>ReadFileEx</c> function does use the <c>OVERLAPPED</c> structure's <c>Internal</c> and <c>InternalHigh</c> members. An application should not
		/// set these members.
		/// </para>
		/// <para>
		/// The <c>OVERLAPPED</c> data structure must remain valid for the duration of the read operation. It should not be a variable that can go out of scope
		/// while the read operation is pending completion.
		/// </para>
		/// </param>
		/// <param name="lpCompletionRoutine">
		/// <para>
		/// A pointer to the completion routine to be called when the read operation is complete and the calling thread is in an alertable wait state. For more
		/// information about the completion routine, see <c>FileIOCompletionRoutine</c>.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// <para>
		/// If the function succeeds, the calling thread has an asynchronous I/O operation pending: the overlapped read operation from the file. When this I/O
		/// operation completes, and the calling thread is blocked in an alertable wait state, the system calls the function pointed to by lpCompletionRoutine,
		/// and the wait state completes with a return code of <c>WAIT_IO_COMPLETION</c>.
		/// </para>
		/// <para>
		/// If the function succeeds, and the file reading operation completes, but the calling thread is not in an alertable wait state, the system queues the
		/// completion routine call, holding the call until the calling thread enters an alertable wait state. For information about alertable waits and
		/// overlapped input/output operations, see About Synchronization.
		/// </para>
		/// <para>
		/// If <c>ReadFileEx</c> attempts to read past the end-of-file (EOF), the call to <c>GetOverlappedResult</c> for that operation returns <c>FALSE</c> and
		/// <c>GetLastError</c> returns <c>ERROR_HANDLE_EOF</c>.
		/// </para>
		/// </returns>
		// BOOL WINAPI ReadFileEx( _In_ HANDLE hFile, _Out_opt_ LPVOID lpBuffer, _In_ DWORD nNumberOfBytesToRead, _Inout_ LPOVERLAPPED lpOverlapped, _In_
		// LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365468")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern unsafe bool ReadFileEx([In] SafeFileHandle hFile, byte* lpBuffer, uint nNumberOfBytesToRead, NativeOverlapped* lpOverlapped, FileIOCompletionRoutine lpCompletionRoutine);

		/// <summary>
		/// <para>Reads data from a file and stores it in an array of buffers.</para>
		/// <para>
		/// The function starts reading data from the file at a position that is specified by an <c>OVERLAPPED</c> structure. The <c>ReadFileScatter</c> function
		/// operates asynchronously.
		/// </para>
		/// </summary>
		/// <param name="hFile">
		/// <para>A handle to the file to be read.</para>
		/// <para>
		/// The file handle must be created with the <c>GENERIC_READ</c> right, and the <c>FILE_FLAG_OVERLAPPED</c> and <c>FILE_FLAG_NO_BUFFERING</c> flags. For
		/// more information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="aSegmentArray">
		/// <para>A pointer to an array of <c>FILE_SEGMENT_ELEMENT</c> buffers that receives the data. For a description of this union, see Remarks.</para>
		/// <para>Each element can receive one page of data.</para>
		/// <para>
		/// The array must contain enough elements to store nNumberOfBytesToRead bytes of data, plus one element for the terminating <c>NULL</c>. For example, if
		/// there are 40 KB to be read and the page size is 4 KB, the array must have 11 elements that includes 10 for the data and one for the <c>NULL</c>.
		/// </para>
		/// <para>
		/// Each buffer must be at least the size of a system memory page and must be aligned on a system memory page size boundary. The system reads one system
		/// memory page of data into each buffer.
		/// </para>
		/// <para>
		/// The function stores the data in the buffers in sequential order. For example, it stores data into the first buffer, then into the second buffer, and
		/// so on until each buffer is filled and all the data is stored, or there are no more buffers.
		/// </para>
		/// </param>
		/// <param name="nNumberOfBytesToRead">
		/// <para>
		/// The total number of bytes to be read from the file. Each element of aSegmentArray contains a one-page chunk of this total. Because the file must be
		/// opened with <c>FILE_FLAG_NO_BUFFERING</c>, the number of bytes must be a multiple of the sector size of the file system where the file is located.
		/// </para>
		/// </param>
		/// <param name="lpReserved">
		/// <para>This parameter is reserved for future use and must be <c>NULL</c>.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// <para>A pointer to an <c>OVERLAPPED</c> data structure.</para>
		/// <para>The <c>ReadFileScatter</c> function requires a valid <c>OVERLAPPED</c> structure. The lpOverlapped parameter cannot be <c>NULL</c>.</para>
		/// <para>
		/// The <c>ReadFileScatter</c> function starts reading data from the file at a position that is specified by the <c>Offset</c> and <c>OffsetHigh</c>
		/// members of the <c>OVERLAPPED</c> structure.
		/// </para>
		/// <para>
		/// The <c>ReadFileScatter</c> function may return before the read operation is complete. In that scenario, the <c>ReadFileScatter</c> function returns
		/// the value 0 (zero), and the <c>GetLastError</c> function returns the value <c>ERROR_IO_PENDING</c>. This asynchronous operation of
		/// <c>ReadFileScatter</c> lets the calling process continue while the read operation completes. You can call the <c>GetOverlappedResult</c>,
		/// <c>HasOverlappedIoCompleted</c>, or <c>GetQueuedCompletionStatus</c> functions to obtain information about the completion of the read operation. For
		/// more information, see Synchronous and Asynchronous I/O.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero (0). To get extended error information, call the <c>GetLastError</c> function.</para>
		/// <para>
		/// If <c>ReadFileScatter</c> attempts to read past the end-of-file (EOF), the call to <c>GetOverlappedResult</c> for that operation returns <c>FALSE</c>
		/// and <c>GetLastError</c> returns <c>ERROR_HANDLE_EOF</c>.
		/// </para>
		/// <para>If the function returns before the read operation is complete, the function returns zero (0), and <c>GetLastError</c> returns <c>ERROR_IO_PENDING</c>.</para>
		/// </returns>
		// BOOL WINAPI ReadFileScatter( _In_ HANDLE hFile, _In_ FILE_SEGMENT_ELEMENT aSegmentArray[], _In_ DWORD nNumberOfBytesToRead, _Reserved_ LPDWORD
		// lpReserved, _Inout_ LPOVERLAPPED lpOverlapped);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365469")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern unsafe bool ReadFileScatter([In] SafeFileHandle hFile, [In] IntPtr aSegmentArray, uint nNumberOfBytesToRead, IntPtr lpReserved, NativeOverlapped* lpOverlapped);

		/// <summary>
		/// <para>Deletes an existing empty directory.</para>
		/// <para>To perform this operation as a transacted operation, use the <c>RemoveDirectoryTransacted</c> function.</para>
		/// </summary>
		/// <param name="lpPathName">
		/// <para>
		/// The path of the directory to be removed. This path must specify an empty directory, and the calling process must have delete access to the directory.
		/// </para>
		/// <para>
		/// In the ANSI version of this function, the name is limited to <c>MAX_PATH</c> characters. To extend this limit to 32,767 wide characters, call the
		/// Unicode version of the function and prepend "\\?\" to the path. For more information, see Naming a File.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI RemoveDirectory( _In_ LPCTSTR lpPathName);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365488")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RemoveDirectory([In] string lpPathName);

		/// <summary>
		/// <para>Sets the physical file size for the specified file to the current position of the file pointer.</para>
		/// <para>
		/// The physical file size is also referred to as the end of the file. The <c>SetEndOfFile</c> function can be used to truncate or extend a file. To set
		/// the logical end of a file, use the <c>SetFileValidData</c> function.
		/// </para>
		/// </summary>
		/// <param name="hFile">
		/// <para>A handle to the file to be extended or truncated.</para>
		/// <para>The file handle must be created with the <c>GENERIC_WRITE</c> access right. For more information, see File Security and Access Rights.</para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero (0). To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI SetEndOfFile( _In_ HANDLE hFile);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365531")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetEndOfFile([In] SafeFileHandle hFile);

		/// <summary>
		/// <para>Sets the attributes for a file or directory.</para>
		/// <para>To perform this operation as a transacted operation, use the <c>SetFileAttributesTransacted</c> function.</para>
		/// </summary>
		/// <param name="lpFileName">
		/// <para>The name of the file whose attributes are to be set.</para>
		/// <para>
		/// In the ANSI version of this function, the name is limited to <c>MAX_PATH</c> characters. To extend this limit to 32,767 wide characters, call the
		/// Unicode version of the function ( <c>SetFileAttributesW</c>) and prepend "\\?\" to the path. For more information, see File Names, Paths, and Namespaces.
		/// </para>
		/// </param>
		/// <param name="dwFileAttributes">
		/// <para>The file attributes to set for the file.</para>
		/// <para>This parameter can be one or more values, combined using the bitwise-OR operator. However, all other values override <c>FILE_ATTRIBUTE_NORMAL</c>.</para>
		/// <para>Not all attributes are supported by this function. For more information, see the Remarks section.</para>
		/// <para>The following is a list of supported attribute values.</para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <term>Meaning</term>
		/// </listheader>
		/// <item>
		/// <term>FILE_ATTRIBUTE_ARCHIVE = 32 (0x20)</term>
		/// <term>A file or directory that is an archive file or directory. Applications typically use this attribute to mark files for backup or removal.</term>
		/// </item>
		/// <item>
		/// <term>FILE_ATTRIBUTE_HIDDEN = 2 (0x2)</term>
		/// <term>The file or directory is hidden. It is not included in an ordinary directory listing.</term>
		/// </item>
		/// <item>
		/// <term>FILE_ATTRIBUTE_NORMAL = 128 (0x80)</term>
		/// <term>A file that does not have other attributes set. This attribute is valid only when used alone.</term>
		/// </item>
		/// <item>
		/// <term>FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 8192 (0x2000)</term>
		/// <term>The file or directory is not to be indexed by the content indexing service.</term>
		/// </item>
		/// <item>
		/// <term>FILE_ATTRIBUTE_OFFLINE = 4096 (0x1000)</term>
		/// <term>
		/// The data of a file is not available immediately. This attribute indicates that the file data is physically moved to offline storage. This attribute
		/// is used by Remote Storage, which is the hierarchical storage management software. Applications should not arbitrarily change this attribute.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_ATTRIBUTE_READONLY = 1 (0x1)</term>
		/// <term>
		/// A file that is read-only. Applications can read the file, but cannot write to it or delete it. This attribute is not honored on directories. For more
		/// information, see &amp;quot;You cannot view or change the Read-only or the System attributes of folders in Windows Server 2003, in Windows XP, or in
		/// Windows Vista.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_ATTRIBUTE_SYSTEM = 4 (0x4)</term>
		/// <term>A file or directory that the operating system uses a part of, or uses exclusively.</term>
		/// </item>
		/// <item>
		/// <term>FILE_ATTRIBUTE_TEMPORARY = 256 (0x100)</term>
		/// <term>
		/// A file that is being used for temporary storage. File systems avoid writing data back to mass storage if sufficient cache memory is available,
		/// because typically, an application deletes a temporary file after the handle is closed. In that scenario, the system can entirely avoid writing the
		/// data. Otherwise, the data is written after the handle is closed.
		/// </term>
		/// </item>
		/// </list>
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI SetFileAttributes( _In_ LPCTSTR lpFileName, _In_ DWORD dwFileAttributes);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365535")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetFileAttributes([In] string lpFileName, FileFlagsAndAttributes dwFileAttributes);

		/// <summary>
		/// <para>Sets the file information for the specified file.</para>
		/// <para>To retrieve file information using a file handle, see <c>GetFileInformationByHandle</c> or <c>GetFileInformationByHandleEx</c>.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>A handle to the file for which to change information.</para>
		/// <para>
		/// This handle must be opened with the appropriate permissions for the requested change. For more information, see the Remarks and Example Code sections.
		/// </para>
		/// <para>This handle should not be a pipe handle.</para>
		/// </param>
		/// <param name="FileInformationClass">
		/// <para>A <c>FILE_INFO_BY_HANDLE_CLASS</c> enumeration value that specifies the type of information to be changed.</para>
		/// <para>For a table of valid values, see the Remarks section.</para>
		/// </param>
		/// <param name="lpFileInformation">
		/// <para>
		/// A pointer to the buffer that contains the information to change for the specified file information class. The structure that this parameter points to
		/// corresponds to the class that is specified by FileInformationClass.
		/// </para>
		/// <para>For a table of valid structure types, see the Remarks section.</para>
		/// </param>
		/// <param name="dwBufferSize">
		/// <para>The size of lpFileInformation, in bytes.</para>
		/// </param>
		/// <returns>
		/// <para>Returns nonzero if successful or zero otherwise.</para>
		/// <para>To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI SetFileInformationByHandle( _In_ HANDLE hFile, _In_ FILE_INFO_BY_HANDLE_CLASS FileInformationClass, _In_ LPVOID lpFileInformation, _In_
		// DWORD dwBufferSize);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365539")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetFileInformationByHandle([In] SafeFileHandle hFile, FILE_INFO_BY_HANDLE_CLASS FileInformationClass, IntPtr lpFileInformation, uint dwBufferSize);

		/// <summary>
		/// <para>Moves the file pointer of the specified file.</para>
		/// <para>
		/// This function stores the file pointer in two <c>LONG</c> values. To work with file pointers that are larger than a single <c>LONG</c> value, it is
		/// easier to use the <c>SetFilePointerEx</c> function.
		/// </para>
		/// </summary>
		/// <param name="hFile">
		/// <para>A handle to the file.</para>
		/// <para>
		/// The file handle must be created with the <c>GENERIC_READ</c> or <c>GENERIC_WRITE</c> access right. For more information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="lDistanceToMove">
		/// <para>The low order 32-bits of a signed value that specifies the number of bytes to move the file pointer.</para>
		/// <para>
		/// If lpDistanceToMoveHigh is not <c>NULL</c>, lpDistanceToMoveHigh and lDistanceToMove form a single 64-bit signed value that specifies the distance to move.
		/// </para>
		/// <para>
		/// If lpDistanceToMoveHigh is <c>NULL</c>, lDistanceToMove is a 32-bit signed value. A positive value for lDistanceToMove moves the file pointer forward
		/// in the file, and a negative value moves the file pointer back.
		/// </para>
		/// </param>
		/// <param name="lpDistanceToMoveHigh">
		/// <para>A pointer to the high order 32-bits of the signed 64-bit distance to move.</para>
		/// <para>If you do not need the high order 32-bits, this pointer must be set to <c>NULL</c>.</para>
		/// <para>
		/// When not <c>NULL</c>, this parameter also receives the high order <c>DWORD</c> of the new value of the file pointer. For more information, see the
		/// Remarks section in this topic.
		/// </para>
		/// </param>
		/// <param name="dwMoveMethod">
		/// <para>The starting point for the file pointer move.</para>
		/// <para>This parameter can be one of the following values.</para>
		/// <para>
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <term>Meaning</term>
		/// </listheader>
		/// <item>
		/// <term>FILE_BEGIN = 0</term>
		/// <term>The starting point is zero or the beginning of the file.</term>
		/// </item>
		/// <item>
		/// <term>FILE_CURRENT = 1</term>
		/// <term>The starting point is the current value of the file pointer.</term>
		/// </item>
		/// <item>
		/// <term>FILE_END = 2</term>
		/// <term>The starting point is the current end-of-file position.</term>
		/// </item>
		/// </list>
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds and lpDistanceToMoveHigh is <c>NULL</c>, the return value is the low-order <c>DWORD</c> of the new file pointer.</para>
		/// <para>
		/// <c>Note</c> If the function returns a value other than <c>INVALID_SET_FILE_POINTER</c>, the call to <c>SetFilePointer</c> has succeeded. You do not
		/// need to call <c>GetLastError</c>.
		/// </para>
		/// <para>
		/// If function succeeds and lpDistanceToMoveHigh is not <c>NULL</c>, the return value is the low-order <c>DWORD</c> of the new file pointer and
		/// lpDistanceToMoveHigh contains the high order <c>DWORD</c> of the new file pointer.
		/// </para>
		/// <para>If the function fails, the return value is <c>INVALID_SET_FILE_POINTER</c>. To get extended error information, call <c>GetLastError</c>.</para>
		/// <para>
		/// If a new file pointer is a negative value, the function fails, the file pointer is not moved, and the code returned by <c>GetLastError</c> is <c>ERROR_NEGATIVE_SEEK</c>.
		/// </para>
		/// <para>If lpDistanceToMoveHigh is <c>NULL</c> and the new file position does not fit in a 32-bit value, the function fails and returns <c>INVALID_SET_FILE_POINTER</c>.</para>
		/// </returns>
		// DWORD WINAPI SetFilePointer( _In_ HANDLE hFile, _In_ LONG lDistanceToMove, _Inout_opt_ PLONG lpDistanceToMoveHigh, _In_ DWORD dwMoveMethod);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365541")]
		public static extern uint SetFilePointer([In] SafeFileHandle hFile, int lDistanceToMove, ref int lpDistanceToMoveHigh, SeekOrigin dwMoveMethod);

		/// <summary>
		/// <para>Moves the file pointer of the specified file.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file. The file handle must have been created with the <c>GENERIC_READ</c> or <c>GENERIC_WRITE</c> access right. For more information,
		/// see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="liDistanceToMove">
		/// <para>
		/// The number of bytes to move the file pointer. A positive value moves the pointer forward in the file and a negative value moves the file pointer backward.
		/// </para>
		/// </param>
		/// <param name="lpNewFilePointer">
		/// <para>A pointer to a variable to receive the new file pointer. If this parameter is <c>NULL</c>, the new file pointer is not returned.</para>
		/// </param>
		/// <param name="dwMoveMethod">
		/// <para>The starting point for the file pointer move. This parameter can be one of the following values.</para>
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <term>Meaning</term>
		/// </listheader>
		/// <item>
		/// <term>FILE_BEGIN = 0</term>
		/// <term>
		/// The starting point is zero or the beginning of the file. If this flag is specified, then the liDistanceToMove parameter is interpreted as an unsigned value.
		/// </term>
		/// </item>
		/// <item>
		/// <term>FILE_CURRENT = 1</term>
		/// <term>The start point is the current value of the file pointer.</term>
		/// </item>
		/// <item>
		/// <term>FILE_END = 2</term>
		/// <term>The starting point is the current end-of-file position.</term>
		/// </item>
		/// </list>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI SetFilePointerEx( _In_ HANDLE hFile, _In_ LARGE_INTEGER liDistanceToMove, _Out_opt_ PLARGE_INTEGER lpNewFilePointer, _In_ DWORD dwMoveMethod);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365542")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetFilePointerEx([In] SafeFileHandle hFile, long liDistanceToMove, out long lpNewFilePointer, SeekOrigin dwMoveMethod);

		/// <summary>
		/// <para>Sets the date and time that the specified file or directory was created, last accessed, or last modified.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file or directory. The handle must have been created using the <c>CreateFile</c> function with the <c>FILE_WRITE_ATTRIBUTES</c>
		/// access right. For more information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="lpCreationTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure that contains the new creation date and time for the file or directory. If the application does not need to
		/// change this information, set this parameter either to <c>NULL</c> or to a pointer to a <c>FILETIME</c> structure that has both the
		/// <c>dwLowDateTime</c> and <c>dwHighDateTime</c> members set to 0.
		/// </para>
		/// </param>
		/// <param name="lpLastAccessTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure that contains the new last access date and time for the file or directory. The last access time includes the
		/// last time the file or directory was written to, read from, or (in the case of executable files) run. If the application does not need to change this
		/// information, set this parameter either to <c>NULL</c> or to a pointer to a <c>FILETIME</c> structure that has both the <c>dwLowDateTime</c> and
		/// <c>dwHighDateTime</c> members set to 0.
		/// </para>
		/// <para>
		/// To prevent file operations using the given handle from modifying the last access time, call <c>SetFileTime</c> immediately after opening the file
		/// handle and pass a <c>FILETIME</c> structure that has both the <c>dwLowDateTime</c> and <c>dwHighDateTime</c> members set to 0xFFFFFFFF.
		/// </para>
		/// </param>
		/// <param name="lpLastWriteTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure that contains the new last modified date and time for the file or directory. If the application does not
		/// need to change this information, set this parameter either to <c>NULL</c> or to a pointer to a <c>FILETIME</c> structure that has both the
		/// <c>dwLowDateTime</c> and <c>dwHighDateTime</c> members set to 0.
		/// </para>
		/// <para>
		/// To prevent file operations using the given handle from modifying the last access time, call <c>SetFileTime</c> immediately after opening the file
		/// handle and pass a <c>FILETIME</c> structure that has both the <c>dwLowDateTime</c> and <c>dwHighDateTime</c> members set to 0xFFFFFFFF.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI SetFileTime( _In_ HANDLE hFile, _In_opt_ const FILETIME *lpCreationTime, _In_opt_ const FILETIME *lpLastAccessTime, _In_opt_ const
		// FILETIME *lpLastWriteTime);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "ms724933")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetFileTime([In] SafeFileHandle hFile, [In, MarshalAs(UnmanagedType.LPStruct)] FILETIME lpCreationTime, [In, MarshalAs(UnmanagedType.LPStruct)] FILETIME lpLastAccessTime, [In, MarshalAs(UnmanagedType.LPStruct)] FILETIME lpLastWriteTime);

		/// <summary>
		/// <para>Sets the date and time that the specified file or directory was created, last accessed, or last modified.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file or directory. The handle must have been created using the <c>CreateFile</c> function with the <c>FILE_WRITE_ATTRIBUTES</c>
		/// access right. For more information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="lpCreationTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure that contains the new creation date and time for the file or directory. If the application does not need to
		/// change this information, set this parameter either to <c>NULL</c> or to a pointer to a <c>FILETIME</c> structure that has both the
		/// <c>dwLowDateTime</c> and <c>dwHighDateTime</c> members set to 0.
		/// </para>
		/// </param>
		/// <param name="lpLastAccessTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure that contains the new last access date and time for the file or directory. The last access time includes the
		/// last time the file or directory was written to, read from, or (in the case of executable files) run. If the application does not need to change this
		/// information, set this parameter either to <c>NULL</c> or to a pointer to a <c>FILETIME</c> structure that has both the <c>dwLowDateTime</c> and
		/// <c>dwHighDateTime</c> members set to 0.
		/// </para>
		/// <para>
		/// To prevent file operations using the given handle from modifying the last access time, call <c>SetFileTime</c> immediately after opening the file
		/// handle and pass a <c>FILETIME</c> structure that has both the <c>dwLowDateTime</c> and <c>dwHighDateTime</c> members set to 0xFFFFFFFF.
		/// </para>
		/// </param>
		/// <param name="lpLastWriteTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure that contains the new last modified date and time for the file or directory. If the application does not
		/// need to change this information, set this parameter either to <c>NULL</c> or to a pointer to a <c>FILETIME</c> structure that has both the
		/// <c>dwLowDateTime</c> and <c>dwHighDateTime</c> members set to 0.
		/// </para>
		/// <para>
		/// To prevent file operations using the given handle from modifying the last access time, call <c>SetFileTime</c> immediately after opening the file
		/// handle and pass a <c>FILETIME</c> structure that has both the <c>dwLowDateTime</c> and <c>dwHighDateTime</c> members set to 0xFFFFFFFF.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI SetFileTime( _In_ HANDLE hFile, _In_opt_ const FILETIME *lpCreationTime, _In_opt_ const FILETIME *lpLastAccessTime, _In_opt_ const
		// FILETIME *lpLastWriteTime);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "ms724933")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern unsafe bool SetFileTime([In] SafeFileHandle hFile, [In] FILETIME* lpCreationTime, [In] FILETIME* lpLastAccessTime, [In] FILETIME* lpLastWriteTime);

		/// <summary>
		/// <para>
		/// Sets the valid data length of the specified file. This function is useful in very limited scenarios. For more information, see the Remarks section.
		/// </para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file. The file must have been opened with the <c>GENERIC_WRITE</c> access right, and the <c>SE_MANAGE_VOLUME_NAME</c> privilege
		/// enabled. For more information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="ValidDataLength">
		/// <para>The new valid data length.</para>
		/// <para>This parameter must be a positive value that is greater than the current valid data length, but less than the current file size.</para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is 0. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI SetFileValidData( _In_ HANDLE hFile, _In_ LONGLONG ValidDataLength);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365544")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetFileValidData([In] SafeFileHandle hFile, long ValidDataLength);

		/// <summary>Sets the label of a file system volume.</summary>
		/// <param name="lpRootPathName">
		/// A pointer to a string that contains the volume's drive letter (for example, X:\) or the path of a mounted folder that is associated with the volume
		/// (for example, Y:\MountX\). The string must end with a trailing backslash ('\'). If this parameter is <c>NULL</c>, the root of the current directory
		/// is used.
		/// </param>
		/// <param name="lpVolumeName">
		/// A pointer to a string that contains the new label for the volume. If this parameter is <c>NULL</c>, the function deletes any existing label from the
		/// specified volume and does not assign a new label.
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI SetVolumeLabel( _In_opt_ LPCTSTR lpRootPathName, _In_opt_ LPCTSTR lpVolumeName); https://msdn.microsoft.com/en-us/library/windows/desktop/aa365560(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("WinBase.h", MSDNShortId = "aa365560")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetVolumeLabel([In] string lpRootPathName, [In] string lpVolumeName);

		/// <summary>Associates a volume with a drive letter or a directory on another volume.</summary>
		/// <param name="lpszVolumeMountPoint">
		/// The user-mode path to be associated with the volume. This may be a drive letter (for example, "X:\") or a directory on another volume (for example,
		/// "Y:\MountX\"). The string must end with a trailing backslash ('\').
		/// </param>
		/// <param name="lpszVolumeName">
		/// A volume <c>GUID</c> path for the volume. This string must be of the form "\\?\Volume{GUID}\" where GUID is a <c>GUID</c> that identifies the volume.
		/// The "\\?\" turns off path parsing and is ignored as part of the path, as discussed in Naming a Volume.
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// <para>
		/// If the lpszVolumeMountPoint parameter contains a path to a mounted folder, <c>GetLastError</c> returns <c>ERROR_DIR_NOT_EMPTY</c>, even if the
		/// directory is empty.
		/// </para>
		/// </returns>
		// BOOL WINAPI SetVolumeMountPoint( _In_ LPCTSTR lpszVolumeMountPoint, _In_ LPCTSTR lpszVolumeName); https://msdn.microsoft.com/en-us/library/windows/desktop/aa365561(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("WinBase.h", MSDNShortId = "aa365561")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetVolumeMountPoint([In] string lpszVolumeMountPoint, [In] string lpszVolumeName);

		/// <summary>
		/// <para>Unlocks a region in an open file. Unlocking a region enables other processes to access the region.</para>
		/// <para>For an alternate way to specify the region, use the <c>UnlockFileEx</c> function.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file that contains a region locked with <c>LockFile</c>. The file handle must have been created with either the <c>GENERIC_READ</c>
		/// or <c>GENERIC_WRITE</c> access right. For more information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="dwFileOffsetLow">
		/// <para>The low-order word of the starting byte offset in the file where the locked region begins.</para>
		/// </param>
		/// <param name="dwFileOffsetHigh">
		/// <para>The high-order word of the starting byte offset in the file where the locked region begins.</para>
		/// </param>
		/// <param name="nNumberOfBytesToUnlockLow">
		/// <para>The low-order word of the length of the byte range to be unlocked.</para>
		/// </param>
		/// <param name="nNumberOfBytesToUnlockHigh">
		/// <para>The high-order word of the length of the byte range to be unlocked.</para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI UnlockFile( _In_ HANDLE hFile, _In_ DWORD dwFileOffsetLow, _In_ DWORD dwFileOffsetHigh, _In_ DWORD nNumberOfBytesToUnlockLow, _In_ DWORD nNumberOfBytesToUnlockHigh);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365715")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnlockFile([In] SafeFileHandle hFile, uint dwFileOffsetLow, uint dwFileOffsetHigh, uint nNumberOfBytesToUnlockLow, uint nNumberOfBytesToUnlockHigh);

		/// <summary>
		/// <para>Unlocks a region in the specified file. This function can operate either synchronously or asynchronously.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file. The handle must have been created with either the <c>GENERIC_READ</c> or <c>GENERIC_WRITE</c> access right. For more
		/// information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="dwReserved">
		/// <para>Reserved parameter; must be zero.</para>
		/// </param>
		/// <param name="nNumberOfBytesToUnlockLow">
		/// <para>The low-order part of the length of the byte range to unlock.</para>
		/// </param>
		/// <param name="nNumberOfBytesToUnlockHigh">
		/// <para>The high-order part of the length of the byte range to unlock.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// <para>
		/// A pointer to an <c>OVERLAPPED</c> structure that the function uses with the unlocking request. This structure contains the file offset of the
		/// beginning of the unlock range. You must initialize the <c>hEvent</c> member to a valid handle or zero. For more information, see Synchronous and
		/// Asynchronous I/O.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero or <c>NULL</c>. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI UnlockFileEx( _In_ HANDLE hFile, _Reserved_ DWORD dwReserved, _In_ DWORD nNumberOfBytesToUnlockLow, _In_ DWORD nNumberOfBytesToUnlockHigh,
		// _Inout_ LPOVERLAPPED lpOverlapped);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365716")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern unsafe bool UnlockFileEx([In] SafeFileHandle hFile, uint dwReserved, uint nNumberOfBytesToUnlockLow, uint nNumberOfBytesToUnlockHigh, NativeOverlapped* lpOverlapped);

		/// <summary>
		/// Writes data to the specified file or input/output (I/O) device.
		/// <para>
		/// This function is designed for both synchronous and asynchronous operation. For a similar function designed solely for asynchronous operation, see WriteFileEx.
		/// </para>
		/// </summary>
		/// <param name="hFile">
		/// A handle to the file or I/O device (for example, a file, file stream, physical disk, volume, console buffer, tape drive, socket, communications
		/// resource, mailslot, or pipe).
		/// <para>
		/// The hFile parameter must have been created with the write access. For more information, see Generic Access Rights and File Security and Access Rights.
		/// </para>
		/// <para>
		/// For asynchronous write operations, hFile can be any handle opened with the CreateFile function using the FILE_FLAG_OVERLAPPED flag or a socket handle
		/// returned by the socket or accept function.
		/// </para>
		/// </param>
		/// <param name="lpBuffer">
		/// A pointer to the buffer containing the data to be written to the file or device.
		/// <para>This buffer must remain valid for the duration of the write operation. The caller must not use this buffer until the write operation is completed.</para>
		/// </param>
		/// <param name="nNumberOfBytesToWrite">
		/// The number of bytes to be written to the file or device.
		/// <para>
		/// A value of zero specifies a null write operation. The behavior of a null write operation depends on the underlying file system or communications technology.
		/// </para>
		/// <para>
		/// Windows Server 2003 and Windows XP: Pipe write operations across a network are limited in size per write. The amount varies per platform. For x86
		/// platforms it's 63.97 MB. For x64 platforms it's 31.97 MB. For Itanium it's 63.95 MB. For more information regarding pipes, see the Remarks section.
		/// </para>
		/// </param>
		/// <param name="lpNumberOfBytesWritten">
		/// A pointer to the variable that receives the number of bytes written when using a synchronous hFile parameter. WriteFile sets this value to zero
		/// before doing any work or error checking. Use NULL for this parameter if this is an asynchronous operation to avoid potentially erroneous results.
		/// <para>This parameter can be NULL only when the lpOverlapped parameter is not NULL.</para>
		/// <para>For more information, see the Remarks section.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// A pointer to an OVERLAPPED structure is required if the hFile parameter was opened with FILE_FLAG_OVERLAPPED, otherwise this parameter can be NULL.
		/// <para>
		/// For an hFile that supports byte offsets, if you use this parameter you must specify a byte offset at which to start writing to the file or device.
		/// This offset is specified by setting the Offset and OffsetHigh members of the OVERLAPPED structure. For an hFile that does not support byte offsets,
		/// Offset and OffsetHigh are ignored.
		/// </para>
		/// <para>
		/// To write to the end of file, specify both the Offset and OffsetHigh members of the OVERLAPPED structure as 0xFFFFFFFF. This is functionally
		/// equivalent to previously calling the CreateFile function to open hFile using FILE_APPEND_DATA access.
		/// </para>
		/// <para>
		/// For more information about different combinations of lpOverlapped and FILE_FLAG_OVERLAPPED, see the Remarks section and the Synchronization and File
		/// Position section.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero (TRUE). If the function fails, or is completing asynchronously, the return value is
		/// zero(FALSE). To get extended error information, call the GetLastError function.
		/// </returns>
		[DllImport(Lib.Kernel32, ExactSpelling = true, SetLastError = true), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365747")]
		public static extern bool WriteFile(SafeFileHandle hFile, IntPtr lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

		/// <summary>
		/// Writes data to the specified file or input/output (I/O) device.
		/// <para>
		/// This function is designed for both synchronous and asynchronous operation. For a similar function designed solely for asynchronous operation, see WriteFileEx.
		/// </para>
		/// </summary>
		/// <param name="hFile">
		/// A handle to the file or I/O device (for example, a file, file stream, physical disk, volume, console buffer, tape drive, socket, communications
		/// resource, mailslot, or pipe).
		/// <para>
		/// The hFile parameter must have been created with the write access. For more information, see Generic Access Rights and File Security and Access Rights.
		/// </para>
		/// <para>
		/// For asynchronous write operations, hFile can be any handle opened with the CreateFile function using the FILE_FLAG_OVERLAPPED flag or a socket handle
		/// returned by the socket or accept function.
		/// </para>
		/// </param>
		/// <param name="lpBuffer">
		/// A pointer to the buffer containing the data to be written to the file or device.
		/// <para>This buffer must remain valid for the duration of the write operation. The caller must not use this buffer until the write operation is completed.</para>
		/// </param>
		/// <param name="nNumberOfBytesToWrite">
		/// The number of bytes to be written to the file or device.
		/// <para>
		/// A value of zero specifies a null write operation. The behavior of a null write operation depends on the underlying file system or communications technology.
		/// </para>
		/// <para>
		/// Windows Server 2003 and Windows XP: Pipe write operations across a network are limited in size per write. The amount varies per platform. For x86
		/// platforms it's 63.97 MB. For x64 platforms it's 31.97 MB. For Itanium it's 63.95 MB. For more information regarding pipes, see the Remarks section.
		/// </para>
		/// </param>
		/// <param name="lpNumberOfBytesWritten">
		/// A pointer to the variable that receives the number of bytes written when using a synchronous hFile parameter. WriteFile sets this value to zero
		/// before doing any work or error checking. Use NULL for this parameter if this is an asynchronous operation to avoid potentially erroneous results.
		/// <para>This parameter can be NULL only when the lpOverlapped parameter is not NULL.</para>
		/// <para>For more information, see the Remarks section.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// A pointer to an OVERLAPPED structure is required if the hFile parameter was opened with FILE_FLAG_OVERLAPPED, otherwise this parameter can be NULL.
		/// <para>
		/// For an hFile that supports byte offsets, if you use this parameter you must specify a byte offset at which to start writing to the file or device.
		/// This offset is specified by setting the Offset and OffsetHigh members of the OVERLAPPED structure. For an hFile that does not support byte offsets,
		/// Offset and OffsetHigh are ignored.
		/// </para>
		/// <para>
		/// To write to the end of file, specify both the Offset and OffsetHigh members of the OVERLAPPED structure as 0xFFFFFFFF. This is functionally
		/// equivalent to previously calling the CreateFile function to open hFile using FILE_APPEND_DATA access.
		/// </para>
		/// <para>
		/// For more information about different combinations of lpOverlapped and FILE_FLAG_OVERLAPPED, see the Remarks section and the Synchronization and File
		/// Position section.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero (TRUE). If the function fails, or is completing asynchronously, the return value is
		/// zero(FALSE). To get extended error information, call the GetLastError function.
		/// </returns>
		[DllImport(Lib.Kernel32, ExactSpelling = true, SetLastError = true), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365747")]
		public static extern unsafe bool WriteFile(SafeFileHandle hFile, byte* lpBuffer, uint nNumberOfBytesToWrite, IntPtr lpNumberOfBytesWritten, NativeOverlapped* lpOverlapped);

		/// <summary>
		/// Writes data to the specified file or input/output (I/O) device.
		/// <para>
		/// This function is designed for both synchronous and asynchronous operation. For a similar function designed solely for asynchronous operation, see WriteFileEx.
		/// </para>
		/// </summary>
		/// <param name="hFile">
		/// A handle to the file or I/O device (for example, a file, file stream, physical disk, volume, console buffer, tape drive, socket, communications
		/// resource, mailslot, or pipe).
		/// <para>
		/// The hFile parameter must have been created with the write access. For more information, see Generic Access Rights and File Security and Access Rights.
		/// </para>
		/// <para>
		/// For asynchronous write operations, hFile can be any handle opened with the CreateFile function using the FILE_FLAG_OVERLAPPED flag or a socket handle
		/// returned by the socket or accept function.
		/// </para>
		/// </param>
		/// <param name="lpBuffer">
		/// A pointer to the buffer containing the data to be written to the file or device.
		/// <para>This buffer must remain valid for the duration of the write operation. The caller must not use this buffer until the write operation is completed.</para>
		/// </param>
		/// <param name="nNumberOfBytesToWrite">
		/// The number of bytes to be written to the file or device.
		/// <para>
		/// A value of zero specifies a null write operation. The behavior of a null write operation depends on the underlying file system or communications technology.
		/// </para>
		/// <para>
		/// Windows Server 2003 and Windows XP: Pipe write operations across a network are limited in size per write. The amount varies per platform. For x86
		/// platforms it's 63.97 MB. For x64 platforms it's 31.97 MB. For Itanium it's 63.95 MB. For more information regarding pipes, see the Remarks section.
		/// </para>
		/// </param>
		/// <param name="lpNumberOfBytesWritten">
		/// A pointer to the variable that receives the number of bytes written when using a synchronous hFile parameter. WriteFile sets this value to zero
		/// before doing any work or error checking. Use NULL for this parameter if this is an asynchronous operation to avoid potentially erroneous results.
		/// <para>This parameter can be NULL only when the lpOverlapped parameter is not NULL.</para>
		/// <para>For more information, see the Remarks section.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// A pointer to an OVERLAPPED structure is required if the hFile parameter was opened with FILE_FLAG_OVERLAPPED, otherwise this parameter can be NULL.
		/// <para>
		/// For an hFile that supports byte offsets, if you use this parameter you must specify a byte offset at which to start writing to the file or device.
		/// This offset is specified by setting the Offset and OffsetHigh members of the OVERLAPPED structure. For an hFile that does not support byte offsets,
		/// Offset and OffsetHigh are ignored.
		/// </para>
		/// <para>
		/// To write to the end of file, specify both the Offset and OffsetHigh members of the OVERLAPPED structure as 0xFFFFFFFF. This is functionally
		/// equivalent to previously calling the CreateFile function to open hFile using FILE_APPEND_DATA access.
		/// </para>
		/// <para>
		/// For more information about different combinations of lpOverlapped and FILE_FLAG_OVERLAPPED, see the Remarks section and the Synchronization and File
		/// Position section.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero (TRUE). If the function fails, or is completing asynchronously, the return value is
		/// zero(FALSE). To get extended error information, call the GetLastError function.
		/// </returns>
		[DllImport(Lib.Kernel32, ExactSpelling = true, SetLastError = true), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365747")]
		public static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

		/// <summary>
		/// <para>
		/// Writes data to the specified file or input/output (I/O) device. It reports its completion status asynchronously, calling the specified completion
		/// routine when writing is completed or canceled and the calling thread is in an alertable wait state.
		/// </para>
		/// <para>To write data to a file or device synchronously, use the <c>WriteFile</c> function.</para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file or I/O device (for example, a file, file stream, physical disk, volume, console buffer, tape drive, socket, communications
		/// resource, mailslot, or pipe).
		/// </para>
		/// <para>
		/// This parameter can be any handle opened with the <c>FILE_FLAG_OVERLAPPED</c> flag by the <c>CreateFile</c> function, or a socket handle returned by
		/// the <c>socket</c> or <c>accept</c> function.
		/// </para>
		/// <para>Do not associate an I/O completion port with this handle. For more information, see the Remarks section.</para>
		/// <para>This handle also must have the <c>GENERIC_WRITE</c> access right. For more information on access rights, see File Security and Access Rights.</para>
		/// </param>
		/// <param name="lpBuffer">
		/// <para>A pointer to the buffer containing the data to be written to the file or device.</para>
		/// <para>This buffer must remain valid for the duration of the write operation. The caller must not use this buffer until the write operation is completed.</para>
		/// </param>
		/// <param name="nNumberOfBytesToWrite">
		/// <para>The number of bytes to be written to the file or device.</para>
		/// <para>A value of zero specifies a null write operation. The behavior of a null write operation depends on the underlying file system.</para>
		/// <para>Pipe write operations across a network are limited to 65,535 bytes per write. For more information regarding pipes, see the Remarks section.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// <para>A pointer to an <c>OVERLAPPED</c> data structure that supplies data to be used during the overlapped (asynchronous) write operation.</para>
		/// <para>
		/// For files that support byte offsets, you must specify a byte offset at which to start writing to the file. You specify this offset by setting the
		/// <c>Offset</c> and <c>OffsetHigh</c> members of the <c>OVERLAPPED</c> structure. For files or devices that do not support byte offsets, <c>Offset</c>
		/// and <c>OffsetHigh</c> are ignored.
		/// </para>
		/// <para>
		/// To write to the end of file, specify both the <c>Offset</c> and <c>OffsetHigh</c> members of the <c>OVERLAPPED</c> structure as 0xFFFFFFFF. This is
		/// functionally equivalent to previously calling the <c>CreateFile</c> function to open hFile using <c>FILE_APPEND_DATA</c> access.
		/// </para>
		/// <para>
		/// The <c>WriteFileEx</c> function ignores the <c>OVERLAPPED</c> structure's <c>hEvent</c> member. An application is free to use that member for its own
		/// purposes in the context of a <c>WriteFileEx</c> call. <c>WriteFileEx</c> signals completion of its writing operation by calling, or queuing a call
		/// to, the completion routine pointed to by lpCompletionRoutine, so it does not need an event handle.
		/// </para>
		/// <para>
		/// The <c>WriteFileEx</c> function does use the <c>Internal</c> and <c>InternalHigh</c> members of the <c>OVERLAPPED</c> structure. You should not
		/// change the value of these members.
		/// </para>
		/// <para>
		/// The <c>OVERLAPPED</c> data structure must remain valid for the duration of the write operation. It should not be a variable that can go out of scope
		/// while the write operation is pending completion.
		/// </para>
		/// </param>
		/// <param name="lpCompletionRoutine">
		/// <para>
		/// A pointer to a completion routine to be called when the write operation has been completed and the calling thread is in an alertable wait state. For
		/// more information about this completion routine, see <c>FileIOCompletionRoutine</c>.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// <para>
		/// If the <c>WriteFileEx</c> function succeeds, the calling thread has an asynchronous I/O operation pending: the overlapped write operation to the
		/// file. When this I/O operation finishes, and the calling thread is blocked in an alertable wait state, the operating system calls the function pointed
		/// to by lpCompletionRoutine, and the wait completes with a return code of <c>WAIT_IO_COMPLETION</c>.
		/// </para>
		/// <para>
		/// If the function succeeds and the file-writing operation finishes, but the calling thread is not in an alertable wait state, the system queues the
		/// call to *lpCompletionRoutine, holding the call until the calling thread enters an alertable wait state. For more information about alertable wait
		/// states and overlapped input/output operations, see About Synchronization.
		/// </para>
		/// </returns>
		// BOOL WINAPI WriteFileEx( _In_ HANDLE hFile, _In_opt_ LPCVOID lpBuffer, _In_ DWORD nNumberOfBytesToWrite, _Inout_ LPOVERLAPPED lpOverlapped, _In_
		// LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365748")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern unsafe bool WriteFileEx([In] SafeFileHandle hFile, [In] byte* lpBuffer, uint nNumberOfBytesToWrite, NativeOverlapped* lpOverlapped, FileIOCompletionRoutine lpCompletionRoutine);

		/// <summary>
		/// <para>Retrieves data from an array of buffers and writes the data to a file.</para>
		/// <para>
		/// The function starts writing data to the file at a position that is specified by an <c>OVERLAPPED</c> structure. The <c>WriteFileGather</c> function
		/// operates asynchronously.
		/// </para>
		/// </summary>
		/// <param name="hFile">
		/// <para>
		/// A handle to the file. The file handle must be created with the <c>GENERIC_WRITE</c> access right, and the <c>FILE_FLAG_OVERLAPPED</c> and
		/// <c>FILE_FLAG_NO_BUFFERING</c> flags. For more information, see File Security and Access Rights.
		/// </para>
		/// </param>
		/// <param name="aSegmentArray">
		/// <para>A pointer to an array of <c>FILE_SEGMENT_ELEMENT</c> buffers that contain the data. For a description of this union, see Remarks.</para>
		/// <para>Each element contains the address of one page of data.</para>
		/// <para>
		/// Each buffer must be at least the size of a system memory page and must be aligned on a system memory page size boundary. The system writes one system
		/// memory page of data from each buffer.
		/// </para>
		/// <para>
		/// The function gathers the data from the buffers in a sequential order. For example, it writes data to the file from the first buffer, then the second
		/// buffer, and so on until there is no more data.
		/// </para>
		/// <para>
		/// Due to the asynchronous operation of this function, precautions must be taken to ensure that this parameter always references valid memory for the
		/// lifetime of the asynchronous writes. For instance, a common programming error is to use local stack storage and then allow execution to run out of scope.
		/// </para>
		/// </param>
		/// <param name="nNumberOfBytesToWrite">
		/// <para>
		/// The total number of bytes to be written. Each element of aSegmentArray contains a one-page chunk of this total. Because the file must be opened with
		/// <c>FILE_FLAG_NO_BUFFERING</c>, the number of bytes must be a multiple of the sector size of the file system where the file is located.
		/// </para>
		/// <para>
		/// If nNumberOfBytesToWrite is zero (0), the function performs a null write operation. The behavior of a null write operation depends on the underlying
		/// file system. If nNumberOfBytesToWrite is not zero (0) and the offset and length of the write place data beyond the current end of the file, the
		/// <c>WriteFileGather</c> function extends the file.
		/// </para>
		/// </param>
		/// <param name="lpReserved">
		/// <para>This parameter is reserved for future use and must be <c>NULL</c>.</para>
		/// </param>
		/// <param name="lpOverlapped">
		/// <para>A pointer to an <c>OVERLAPPED</c> data structure.</para>
		/// <para>The <c>WriteFileGather</c> function requires a valid <c>OVERLAPPED</c> structure. The lpOverlapped parameter cannot be <c>NULL</c>.</para>
		/// <para>
		/// The <c>WriteFileGather</c> function starts writing data to the file at a position that is specified by the <c>Offset</c> and <c>OffsetHigh</c>
		/// members of the <c>OVERLAPPED</c> structure.
		/// </para>
		/// <para>
		/// The <c>WriteFileGather</c> function may return before the write operation is complete. In that scenario, the <c>WriteFileGather</c> function returns
		/// the value zero (0), and the <c>GetLastError</c> function returns the value <c>ERROR_IO_PENDING</c>. This asynchronous operation of the
		/// <c>WriteFileGather</c> function lets the calling process continue while the write operation completes. You can call the <c>GetOverlappedResult</c>,
		/// <c>HasOverlappedIoCompleted</c>, or <c>GetQueuedCompletionStatus</c> function to obtain information about the completion of the write operation. For
		/// more information, see Synchronous and Asynchronous I/O.
		/// </para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero (0). To get extended error information, call the <c>GetLastError</c> function.</para>
		/// <para>
		/// If the function returns before the write operation is complete, the function returns zero (0), and the <c>GetLastError</c> function returns <c>ERROR_IO_PENDING</c>.
		/// </para>
		/// </returns>
		// BOOL WINAPI WriteFileGather( _In_ HANDLE hFile, _In_ FILE_SEGMENT_ELEMENT aSegmentArray[], _In_ DWORD nNumberOfBytesToWrite, _Reserved_ LPDWORD
		// lpReserved, _Inout_ LPOVERLAPPED lpOverlapped);
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365749")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern unsafe bool WriteFileGather([In] SafeFileHandle hFile, [In] IntPtr aSegmentArray, uint nNumberOfBytesToWrite, IntPtr lpReserved, NativeOverlapped* lpOverlapped);

		/// <summary>
		/// <para>
		/// Retrieves information about MS-DOS device names. The function can obtain the current mapping for a particular MS-DOS device name. The function can
		/// also obtain a list of all existing MS-DOS device names.
		/// </para>
		/// <para>
		/// MS-DOS device names are stored as junctions in the object namespace. The code that converts an MS-DOS path into a corresponding path uses these
		/// junctions to map MS-DOS devices and drive letters. The <c>QueryDosDevice</c> function enables an application to query the names of the junctions used
		/// to implement the MS-DOS device namespace as well as the value of each specific junction.
		/// </para>
		/// </summary>
		/// <param name="lpDeviceName">
		/// <para>
		/// An MS-DOS device name string specifying the target of the query. The device name cannot have a trailing backslash; for example, use "C:", not "C:\".
		/// </para>
		/// <para>
		/// This parameter can be <c>NULL</c>. In that case, the <c>QueryDosDevice</c> function will store a list of all existing MS-DOS device names into the
		/// buffer pointed to by lpTargetPath.
		/// </para>
		/// </param>
		/// <param name="lpTargetPath">
		/// <para>
		/// A pointer to a buffer that will receive the result of the query. The function fills this buffer with one or more null-terminated strings. The final
		/// null-terminated string is followed by an additional <c>NULL</c>.
		/// </para>
		/// <para>
		/// If lpDeviceName is non- <c>NULL</c>, the function retrieves information about the particular MS-DOS device specified by lpDeviceName. The first
		/// null-terminated string stored into the buffer is the current mapping for the device. The other null-terminated strings represent undeleted prior
		/// mappings for the device.
		/// </para>
		/// <para>
		/// If lpDeviceName is <c>NULL</c>, the function retrieves a list of all existing MS-DOS device names. Each null-terminated string stored into the buffer
		/// is the name of an existing MS-DOS device, for example, \Device\HarddiskVolume1 or \Device\Floppy0.
		/// </para>
		/// </param>
		/// <param name="ucchMax">
		/// <para>The maximum number of <c>TCHARs</c> that can be stored into the buffer pointed to by lpTargetPath.</para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, the return value is the number of <c>TCHARs</c> stored into the buffer pointed to by lpTargetPath.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// <para>If the buffer is too small, the function fails and the last error code is <c>ERROR_INSUFFICIENT_BUFFER</c>.</para>
		/// </returns>
		// DWORD WINAPI QueryDosDevice( _In_opt_ LPCTSTR lpDeviceName, _Out_ LPTSTR lpTargetPath, _In_ DWORD ucchMax);
		[DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa365461")]
		private static extern uint QueryDosDevice(string lpDeviceName, IntPtr lpTargetPath, int ucchMax);

		/// <summary>Contains information that the GetFileInformationByHandle function retrieves.</summary>
		[StructLayout(LayoutKind.Sequential)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa363788")]
		public struct BY_HANDLE_FILE_INFORMATION
		{
			/// <summary>The file attributes. For possible values and their descriptions, see File Attribute Constants.</summary>
			public FileFlagsAndAttributes dwFileAttributes;
			/// <summary>
			/// A FILETIME structure that specifies when a file or directory is created. If the underlying file system does not support creation time, this
			/// member is zero (0).
			/// </summary>
			public FILETIME ftCreationTime;
			/// <summary>
			/// A FILETIME structure. For a file, the structure specifies the last time that a file is read from or written to. For a directory, the structure
			/// specifies when the directory is created. For both files and directories, the specified date is correct, but the time of day is always set to
			/// midnight. If the underlying file system does not support the last access time, this member is zero (0).
			/// </summary>
			public FILETIME ftLastAccessTime;
			/// <summary>
			/// A FILETIME structure. For a file, the structure specifies the last time that a file is written to. For a directory, the structure specifies when
			/// the directory is created. If the underlying file system does not support the last write time, this member is zero (0).
			/// </summary>
			public FILETIME ftLastWriteTime;
			/// <summary>The serial number of the volume that contains a file.</summary>
			public uint dwVolumeSerialNumber;
			/// <summary>The high-order part of the file size.</summary>
			public uint nFileSizeHigh;
			/// <summary>The low-order part of the file size.</summary>
			public uint nFileSizeLow;
			/// <summary>
			/// The number of links to this file. For the FAT file system this member is always 1. For the NTFS file system, it can be more than 1.
			/// </summary>
			public uint nNumberOfLinks;
			/// <summary>The high-order part of a unique identifier that is associated with a file. For more information, see nFileIndexLow.</summary>
			public uint nFileIndexHigh;
			/// <summary>
			/// The low-order part of a unique identifier that is associated with a file.
			/// <para>
			/// The identifier (low and high parts) and the volume serial number uniquely identify a file on a single computer. To determine whether two open
			/// handles represent the same file, combine the identifier and the volume serial number for each file and compare them.
			/// </para>
			/// <para>
			/// The ReFS file system, introduced with Windows Server 2012, includes 128-bit file identifiers. To retrieve the 128-bit file identifier use the
			/// GetFileInformationByHandleEx function with FileIdInfo to retrieve the FILE_ID_INFO structure. The 64-bit identifier in this structure is not
			/// guaranteed to be unique on ReFS.
			/// </para>
			/// </summary>
			public uint nFileIndexLow;
		}

		/// <summary>
		/// <para>Contains optional extended parameters for <c>CreateFile2</c>.</para>
		/// </summary>
		// typedef struct _CREATEFILE2_EXTENDED_PARAMETERS { DWORD dwSize; DWORD dwFileAttributes; DWORD dwFileFlags; DWORD dwSecurityQosFlags;
		// LPSECURITY_ATTRIBUTES lpSecurityAttributes; HANDLE hTemplateFile;} CREATEFILE2_EXTENDED_PARAMETERS, *PCREATEFILE2_EXTENDED_PARAMETERS, *LPCREATEFILE2_EXTENDED_PARAMETERS;
		[PInvokeData("FileAPI.h", MSDNShortId = "hh449426")]
		[StructLayout(LayoutKind.Sequential)]
		public struct CREATEFILE2_EXTENDED_PARAMETERS
		{
			/// <summary>
			/// <para>Contains the size of this structure, .</para>
			/// </summary>
			public uint dwSize;
			/// <summary>
			/// <para>The file or device attributes and flags, <c>FILE_ATTRIBUTE_NORMAL</c> being the most common default value for files.</para>
			/// <para>
			/// This parameter can include any combination of the available file attributes ( <c>FILE_ATTRIBUTE_*</c>). All other file attributes override <c>FILE_ATTRIBUTE_NORMAL</c>.
			/// </para>
			/// <para>
			/// Some of the following file attributes and flags may only apply to files and not necessarily all other types of devices that <c>CreateFile2</c>
			/// can open. For additional information, see the Remarks section of the <c>CreateFile2</c> reference page and Creating and Opening Files.
			/// </para>
			/// <para>
			/// For more advanced access to file attributes, see <c>SetFileAttributes</c>. For a complete list of all file attributes with their values and
			/// descriptions, see <c>File Attribute Constants</c>.
			/// </para>
			/// <para>
			/// <list type="table">
			/// <listheader>
			/// <term>Attribute</term>
			/// <term>Meaning</term>
			/// </listheader>
			/// <item>
			/// <term>FILE_ATTRIBUTE_ARCHIVE32 (0x20)</term>
			/// <term>The file should be archived. Applications use this attribute to mark files for backup or removal.</term>
			/// </item>
			/// <item>
			/// <term>FILE_ATTRIBUTE_ENCRYPTED16384 (0x4000)</term>
			/// <term>
			/// The file or directory is encrypted. For a file, this means that all data in the file is encrypted. For a directory, this means that encryption is
			/// the default for newly created files and subdirectories. For more information, see File Encryption.This flag has no effect if
			/// FILE_ATTRIBUTE_SYSTEM is also specified.This flag is not supported on Home, Home Premium, Starter, or ARM editions of Windows.This flag is not
			/// supported when called from a Windows Store app.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_ATTRIBUTE_HIDDEN2 (0x2)</term>
			/// <term>The file is hidden. Do not include it in an ordinary directory listing.</term>
			/// </item>
			/// <item>
			/// <term>FILE_ATTRIBUTE_INTEGRITY_STREAM32768 (0x8000)</term>
			/// <term>
			/// A file or directory that is configured with integrity. For a file, all data streams in the file have integrity. For a directory, integrity is the
			/// default for newly created files and subdirectories, unless the caller specifies otherwise.This flag is only supported on the ReFS file system.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_ATTRIBUTE_NORMAL128 (0x80)</term>
			/// <term>The file does not have other attributes set. This attribute is valid only if used alone.</term>
			/// </item>
			/// <item>
			/// <term>FILE_ATTRIBUTE_OFFLINE4096 (0x1000)</term>
			/// <term>
			/// The data of a file is not immediately available. This attribute indicates that file data is physically moved to offline storage. This attribute
			/// is used by Remote Storage, the hierarchical storage management software. Applications should not arbitrarily change this attribute.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_ATTRIBUTE_READONLY1 (0x1)</term>
			/// <term>The file is read only. Applications can read the file, but cannot write to or delete it.</term>
			/// </item>
			/// <item>
			/// <term>FILE_ATTRIBUTE_SYSTEM4 (0x4)</term>
			/// <term>The file is part of or used exclusively by an operating system.</term>
			/// </item>
			/// <item>
			/// <term>FILE_ATTRIBUTE_TEMPORARY256 (0x100)</term>
			/// <term>The file is being used for temporary storage.For more information, see the Caching Behavior section of this topic.</term>
			/// </item>
			/// </list>
			/// </para>
			/// </summary>
			public FileFlagsAndAttributes dwFileAttributes;
			/// <summary>
			/// <para>
			/// This parameter can contain combinations of flags ( <c>FILE_FLAG_*</c>) for control of file or device caching behavior, access modes, and other
			/// special-purpose flags.
			/// </para>
			/// <para>
			/// <list type="table">
			/// <listheader>
			/// <term>Flag</term>
			/// <term>Meaning</term>
			/// </listheader>
			/// <item>
			/// <term>FILE_FLAG_BACKUP_SEMANTICS0x02000000</term>
			/// <term>
			/// The file is being opened or created for a backup or restore operation. The system ensures that the calling process overrides file security checks
			/// when the process has SE_BACKUP_NAME and SE_RESTORE_NAME privileges. For more information, see Changing Privileges in a Token.You must set this
			/// flag to obtain a handle to a directory. A directory handle can be passed to some functions instead of a file handle. For more information, see
			/// the Remarks section.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_DELETE_ON_CLOSE0x04000000</term>
			/// <term>
			/// The file is to be deleted immediately after all of its handles are closed, which includes the specified handle and any other open or duplicated
			/// handles.If there are existing open handles to a file, the call fails unless they were all opened with the FILE_SHARE_DELETE share mode.Subsequent
			/// open requests for the file fail, unless the FILE_SHARE_DELETE share mode is specified.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_NO_BUFFERING0x20000000</term>
			/// <term>
			/// The file or device is being opened with no system caching for data reads and writes. This flag does not affect hard disk caching or memory mapped
			/// files.There are strict requirements for successfully working with files opened with CreateFile2 using the FILE_FLAG_NO_BUFFERING flag, for
			/// details see File Buffering.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_OPEN_NO_RECALL0x00100000</term>
			/// <term>
			/// The file data is requested, but it should continue to be located in remote storage. It should not be transported back to local storage. This flag
			/// is for use by remote storage systems.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_OPEN_REPARSE_POINT0x00200000</term>
			/// <term>
			/// Normal reparse point processing will not occur; CreateFile2 will attempt to open the reparse point. When a file is opened, a file handle is
			/// returned, whether or not the filter that controls the reparse point is operational.This flag cannot be used with the CREATE_ALWAYS flag.If the
			/// file is not a reparse point, then this flag is ignored.For more information, see the Remarks section.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_OPEN_REQUIRING_OPLOCK0x00040000</term>
			/// <term>
			/// The file is being opened and an opportunistic lock (oplock) on the file is being requested as a single atomic operation. The file system checks
			/// for oplocks before it performs the create operation, and will fail the create with a last error code of ERROR_CANNOT_BREAK_OPLOCK if the result
			/// would be to break an existing oplock.If you use this flag and your call to the CreateFile2 function successfully returns, the first operation you
			/// should perform on the file handle is to request an oplock by calling the DeviceIOControl function and then pass in FSCTL_REQUEST_OPLOCK or one of
			/// the other Opportunistic Lock Operations. If you perform other file system operations with the file handle before requesting an oplock, a deadlock
			/// might occur.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_OVERLAPPED0x40000000</term>
			/// <term>
			/// The file or device is being opened or created for asynchronous I/O.When subsequent I/O operations are completed on this handle, the event
			/// specified in the OVERLAPPED structure will be set to the signaled state.If this flag is specified, the file can be used for simultaneous read and
			/// write operations.If this flag is not specified, then I/O operations are serialized, even if the calls to the read and write functions specify an
			/// OVERLAPPED structure.For information about considerations when using a file handle created with this flag, see the Synchronous and Asynchronous
			/// I/O Handles section of this topic.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_POSIX_SEMANTICS0x0100000</term>
			/// <term>
			/// Access will occur according to POSIX rules. This includes allowing multiple files with names, differing only in case, for file systems that
			/// support that naming. Use care when using this option, because files created with this flag may not be accessible by applications that are written
			/// for MS-DOS or 16-bit Windows.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_RANDOM_ACCESS0x10000000</term>
			/// <term>
			/// Access is intended to be random. The system can use this as a hint to optimize file caching.This flag has no effect if the file system does not
			/// support cached I/O and FILE_FLAG_NO_BUFFERING.For more information, see the Caching Behavior section of this topic.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_SESSION_AWARE0x00800000</term>
			/// <term>
			/// The file or device is being opened with session awareness. If this flag is not specified, then per-session devices (such as a device using
			/// RemoteFX USB Redirection) cannot be opened by processes running in session 0. This flag has no effect for callers not in session 0. This flag is
			/// supported only on server editions of Windows.Windows Server 2008 R2 and Windows Server 2008: This flag is not supported before Windows Server 2012.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_SEQUENTIAL_SCAN0x08000000</term>
			/// <term>
			/// Access is intended to be sequential from beginning to end. The system can use this as a hint to optimize file caching.This flag should not be
			/// used if read-behind (that is, backwards scans) will be used.This flag has no effect if the file system does not support cached I/O and
			/// FILE_FLAG_NO_BUFFERING.For more information, see the Caching Behavior section of this topic.
			/// </term>
			/// </item>
			/// <item>
			/// <term>FILE_FLAG_WRITE_THROUGH0x80000000</term>
			/// <term>
			/// Write operations will not go through any intermediate cache, they will go directly to disk.For additional information, see the Caching Behavior
			/// section of this topic.
			/// </term>
			/// </item>
			/// </list>
			/// </para>
			/// </summary>
			public FileFlagsAndAttributes dwFileFlags;
			/// <summary>
			/// <para>The dwSecurityQosFlags parameter specifies SQOS information. For more information, see Impersonation Levels.</para>
			/// <para>
			/// <list type="table">
			/// <listheader>
			/// <term>Security flag</term>
			/// <term>Meaning</term>
			/// </listheader>
			/// <item>
			/// <term>SECURITY_ANONYMOUS</term>
			/// <term>Impersonates a client at the Anonymous impersonation level.</term>
			/// </item>
			/// <item>
			/// <term>SECURITY_CONTEXT_TRACKING</term>
			/// <term>The security tracking mode is dynamic. If this flag is not specified, the security tracking mode is static.</term>
			/// </item>
			/// <item>
			/// <term>SECURITY_DELEGATION</term>
			/// <term>Impersonates a client at the Delegation impersonation level.</term>
			/// </item>
			/// <item>
			/// <term>SECURITY_EFFECTIVE_ONLY</term>
			/// <term>
			/// Only the enabled aspects of the client&amp;#39;s security context are available to the server. If you do not specify this flag, all aspects of
			/// the client&amp;#39;s security context are available.This allows the client to limit the groups and privileges that a server can use while
			/// impersonating the client.
			/// </term>
			/// </item>
			/// <item>
			/// <term>SECURITY_IDENTIFICATION</term>
			/// <term>Impersonates a client at the Identification impersonation level.</term>
			/// </item>
			/// <item>
			/// <term>SECURITY_IMPERSONATION</term>
			/// <term>Impersonate a client at the impersonation level. This is the default behavior if no other flags are specified.</term>
			/// </item>
			/// </list>
			/// </para>
			/// </summary>
			public FileFlagsAndAttributes dwSecurityQosFlags;
			/// <summary>
			/// <para>
			/// A pointer to a <c>SECURITY_ATTRIBUTES</c> structure that contains two separate but related data members: an optional security descriptor, and a
			/// Boolean value that determines whether the returned handle can be inherited by child processes.
			/// </para>
			/// <para>This parameter can be <c>NULL</c>.</para>
			/// <para>
			/// If this parameter is <c>NULL</c>, the handle returned by <c>CreateFile2</c> cannot be inherited by any child processes the application may create
			/// and the file or device associated with the returned handle gets a default security descriptor.
			/// </para>
			/// <para>
			/// The <c>lpSecurityDescriptor</c> member of the structure specifies a <c>SECURITY_DESCRIPTOR</c> for a file or device. If this member is
			/// <c>NULL</c>, the file or device associated with the returned handle is assigned a default security descriptor.
			/// </para>
			/// <para>
			/// <c>CreateFile2</c> ignores the <c>lpSecurityDescriptor</c> member when opening an existing file or device, but continues to use the
			/// <c>bInheritHandle</c> member.
			/// </para>
			/// <para>The <c>bInheritHandle</c> member of the structure specifies whether the returned handle can be inherited.</para>
			/// <para>For more information, see the Remarks section of the <c>CreateFile2</c> topic.</para>
			/// </summary>
			public IntPtr lpSecurityAttributes;
			/// <summary>
			/// <para>
			/// A valid handle to a template file with the <c>GENERIC_READ</c> access right. The template file supplies file attributes and extended attributes
			/// for the file that is being created.
			/// </para>
			/// <para>This parameter can be <c>NULL</c>.</para>
			/// <para>When opening an existing file, <c>CreateFile2</c> ignores this parameter.</para>
			/// <para>
			/// When opening a new encrypted file, the file inherits the discretionary access control list from its parent directory. For additional information,
			/// see File Encryption.
			/// </para>
			/// </summary>
			public IntPtr hTemplateFile;
		}

		/*/// <summary>Retrieves file information for the specified file.</summary>
		/// <param name="hFile">A handle to the file that contains the information to be retrieved. This handle should not be a pipe handle.</param>
		/// <param name="lpFileInformation">A pointer to a BY_HANDLE_FILE_INFORMATION structure that receives the file information.</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero and file information data is contained in the buffer pointed to by the lpFileInformation
		/// parameter. If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// </returns>
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("FileAPI.h", MSDNShortId = "aa364952")]*/

		/// <summary>Represents a self-closing change notification handle created by the FindFirstChangeNotification function.</summary>
		/// <seealso cref="Vanara.InteropServices.GenericSafeHandle"/>
		public class SafeFindChangeNotificationHandle : GenericSafeHandle
		{
			/// <summary>Initializes a new instance of the <see cref="SafeFindChangeNotificationHandle"/> class.</summary>
			public SafeFindChangeNotificationHandle() : this(IntPtr.Zero) { }

			/// <summary>Initializes a new instance of the <see cref="SafeFindChangeNotificationHandle"/> class.</summary>
			/// <param name="handle">The handle.</param>
			public SafeFindChangeNotificationHandle(IntPtr handle) : base(handle, FindCloseChangeNotification) { }
		}

		/// <summary>
		/// Represents a self-closing file search handle opened by the FindFirstFile, FindFirstFileEx, FindFirstFileNameW, FindFirstFileNameTransactedW,
		/// FindFirstFileTransacted, FindFirstStreamTransactedW, or FindFirstStreamW functions.
		/// </summary>
		/// <seealso cref="Vanara.InteropServices.GenericSafeHandle"/>
		public class SafeSearchHandle : GenericSafeHandle
		{
			/// <summary>Initializes a new instance of the <see cref="SafeSearchHandle"/> class.</summary>
			public SafeSearchHandle() : this(IntPtr.Zero) { }

			/// <summary>Initializes a new instance of the <see cref="SafeSearchHandle"/> class.</summary>
			/// <param name="handle">The handle.</param>
			public SafeSearchHandle(IntPtr handle) : base(handle, FindClose) { }
		}

		/// <summary>Represents a self-closing volume search handle opened by the FindFirstVolume.</summary>
		/// <seealso cref="Vanara.InteropServices.GenericSafeHandle"/>
		public class SafeVolumeSearchHandle : GenericSafeHandle
		{
			/// <summary>Initializes a new instance of the <see cref="SafeVolumeSearchHandle"/> class.</summary>
			public SafeVolumeSearchHandle() : this(IntPtr.Zero) { }

			/// <summary>Initializes a new instance of the <see cref="SafeVolumeSearchHandle"/> class.</summary>
			/// <param name="handle">The handle.</param>
			public SafeVolumeSearchHandle(IntPtr handle) : base(handle, FindVolumeClose) { }
		}
	}
}