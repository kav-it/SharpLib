// ****************************************************************************
//
// Имя файла    : 'NativeMethods.cs'
// Заголовок    : Реализация работы с WinApi-функциями
// Автор        : Крыцкий А.В./Тихомиров В.С.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.Security;

namespace SharpLib.Native.Windows
{
    [SuppressUnmanagedCodeSecurity]
    public sealed partial class NativeMethods
    {
        #region Константы

        public const UInt16 CREATEPROCESS_MANIFEST_RESOURCE_ID = 1;

        public const UInt32 CREATE_ALWAYS = 2;

        public const UInt32 CREATE_NEW = 1;

        internal const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;

        internal const int ERROR_ACCESS_DENIED = 5;

        internal const int ERROR_CALL_NOT_IMPLEMENTED = 120;

        internal const int ERROR_DIR_NOT_EMPTY = 145;

        internal const int ERROR_DISK_FULL = 112;

        internal const int ERROR_DUP_NAME = 52;

        internal const int ERROR_FILE_EXISTS = 80;

        internal const int ERROR_FILE_NOT_FOUND = 2;

        internal const int ERROR_INVALID_HANDLE = 6;

        internal const int ERROR_INVALID_PARAMETER = 87;

        internal const int ERROR_IO_PENDING = 997;

        internal const int ERROR_NOT_READY = 21;

        internal const int ERROR_NOT_SUPPORTED = 50;

        internal const int ERROR_NO_MORE_FILES = 18;

        internal const int ERROR_PATH_NOT_FOUND = 3;

        internal const int ERROR_SHARING_VIOLATION = 32;

        public const UInt32 FILE_ACTION_ADDED = 0x00000001;

        public const UInt32 FILE_ACTION_MODIFIED = 0x00000003;

        public const UInt32 FILE_ACTION_REMOVED = 0x00000002;

        public const UInt32 FILE_ACTION_RENAMED_NEW_NAME = 0x00000005;

        public const UInt32 FILE_ACTION_RENAMED_OLD_NAME = 0x00000004;

        public const UInt32 FILE_ATTRIBUTE_ARCHIVE = 0x00000020;

        public const UInt32 FILE_ATTRIBUTE_COMPRESSED = 0x00000800;

        public const UInt32 FILE_ATTRIBUTE_DEVICE = 0x00000040;

        public const UInt32 FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        public const UInt32 FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;

        public const UInt32 FILE_ATTRIBUTE_HIDDEN = 0x00000002;

        public const UInt32 FILE_ATTRIBUTE_NORMAL = 0x00000080;

        public const UInt32 FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;

        public const UInt32 FILE_ATTRIBUTE_OFFLINE = 0x00001000;

        public const UInt32 FILE_ATTRIBUTE_READONLY = 0x00000001;

        public const UInt32 FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;

        public const UInt32 FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;

        public const UInt32 FILE_ATTRIBUTE_SYSTEM = 0x00000004;

        public const UInt32 FILE_ATTRIBUTE_TEMPORARY = 0x00000100;

        public const UInt32 FILE_ATTRIBUTE_VIRTUAL = 0x00010000;

        public const UInt32 FILE_CASE_PRESERVED_NAMES = 0x00000002;

        public const UInt32 FILE_CASE_SENSITIVE_SEARCH = 0x00000001;

        public const UInt32 FILE_FILE_COMPRESSION = 0x00000010;

        public const UInt32 FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

        public const UInt32 FILE_FLAG_DELETE_ON_CLOSE = 0x04000000;

        public const UInt32 FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000;

        public const UInt32 FILE_FLAG_NO_BUFFERING = 0x20000000;

        public const UInt32 FILE_FLAG_OPEN_NO_RECALL = 0x00100000;

        public const UInt32 FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;

        public const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;

        public const UInt32 FILE_FLAG_POSIX_SEMANTICS = 0x01000000;

        public const UInt32 FILE_FLAG_RANDOM_ACCESS = 0x10000000;

        public const UInt32 FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000;

        public const UInt32 FILE_FLAG_WRITE_THROUGH = 0x80000000;

        public const UInt32 FILE_NAMED_STREAMS = 0x00040000;

        public const UInt32 FILE_NOTIFY_CHANGE_ATTRIBUTES = 0x00000004;

        public const UInt32 FILE_NOTIFY_CHANGE_CREATION = 0x00000040;

        public const UInt32 FILE_NOTIFY_CHANGE_DIR_NAME = 0x00000002;

        public const UInt32 FILE_NOTIFY_CHANGE_FILE_NAME = 0x00000001;

        public const UInt32 FILE_NOTIFY_CHANGE_LAST_ACCESS = 0x00000020;

        public const UInt32 FILE_NOTIFY_CHANGE_LAST_WRITE = 0x00000010;

        public const UInt32 FILE_NOTIFY_CHANGE_SECURITY = 0x00000100;

        public const UInt32 FILE_NOTIFY_CHANGE_SIZE = 0x00000008;

        public const UInt32 FILE_PERSISTENT_ACLS = 0x00000008;

        public const UInt32 FILE_READ_ONLY_VOLUME = 0x00080000;

        public const UInt32 FILE_SEQUENTIAL_WRITE_ONCE = 0x00100000;

        public const UInt32 FILE_SHARE_DELETE = 0x00000004;

        public const UInt32 FILE_SHARE_READ = 0x00000001;

        public const UInt32 FILE_SHARE_WRITE = 0x00000002;

        public const UInt32 FILE_SUPPORTS_ENCRYPTION = 0x00020000;

        public const UInt32 FILE_SUPPORTS_OBJECT_IDS = 0x00010000;

        public const UInt32 FILE_SUPPORTS_REMOTE_STORAGE = 0x00000100;

        public const UInt32 FILE_SUPPORTS_REPARSE_POINTS = 0x00000080;

        public const UInt32 FILE_SUPPORTS_SPARSE_FILES = 0x00000040;

        public const UInt32 FILE_SUPPORTS_TRANSACTIONS = 0x00200000;

        public const UInt32 FILE_UNICODE_ON_DISK = 0x00000004;

        public const UInt32 FILE_VOLUME_IS_COMPRESSED = 0x00008000;

        public const UInt32 FILE_VOLUME_QUOTAS = 0x00000020;

        public const UInt32 GENERIC_ALL = 0x10000000;

        public const UInt32 GENERIC_EXECUTE = 0x20000000;

        public const UInt32 GENERIC_READ = 0x80000000;

        public const UInt32 GENERIC_WRITE = 0x40000000;

        public const UInt16 ISOLATIONAWARE_MANIFEST_RESOURCE_ID = 2;

        public const UInt16 ISOLATIONAWARE_NOSTATICIMPORT_MANIFEST_RESOURCE_ID = 3;

        public const UInt16 LANG_ENGLISH = 9;

        public const UInt16 LANG_NEUTRAL = 0;

        internal const uint LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010;

        internal const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        internal const uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;

        public const UInt32 MAILSLOT_NO_MESSAGE = 0xffffffff;

        public const UInt32 MAILSLOT_WAIT_FOREVER = 0xffffffff;

        public const UInt32 OPEN_ALWAYS = 4;

        public const UInt32 OPEN_EXISTING = 3;

        public const UInt16 SUBLANG_ENGLISH_US = 1;

        public const UInt16 SUBLANG_NEUTRAL = 0;

        public const UInt32 TRUNCATE_EXISTING = 5;

        public const UInt32 WAIT_ABANDONED = 0x00000080;

        public const UInt32 WAIT_FAILED = 0xFFFFFFFF;

        public const UInt32 WAIT_OBJECT_0 = 0x00000000;

        public const UInt32 WAIT_TIMEOUT = 0x00000102;

        public const int WM_CHAR = 0x102;

        public const int WM_KEYDOWN = 0x100;

        public const int WM_KEYUP = 0x101;

        #endregion
    }
}