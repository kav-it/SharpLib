//*****************************************************************************
//
// Имя файла    : 'Zip.cs'
// Заголовок    : Архивирование Zip
// Автор        : Крыцкий А.В. (на базе DotNetZip v1.9.1.8)
// Контакты     : kav.it@mail.ru
// Дата         : 13/11/2012
//
//*****************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading;

using Interop = System.Runtime.InteropServices;

namespace SharpLib.Zip
{

    #region Перечисления

    #region Перечисление ZipErrorAction

    public enum ZipErrorAction
    {
        Throw,

        Skip,

        Retry,

        InvokeErrorEvent,
    }

    #endregion Перечисление ZipErrorAction

    #region Перечисление ZipCompressionMethod

    public enum ZipCompressionMethod
    {
        None = 0,

        Deflate = 8,
    }

    #endregion Перечисление ZipCompressionMethod

    #region Перечисление ZipCompressionLevel

    public enum ZipCompressionLevel
    {
        None = 0,

        Level0 = 0,

        BestSpeed = 1,

        Level1 = 1,

        Level2 = 2,

        Level3 = 3,

        Level4 = 4,

        Level5 = 5,

        Default = 6,

        Level6 = 6,

        Level7 = 7,

        Level8 = 8,

        BestCompression = 9,

        Level9 = 9,
    }

    #endregion Перечисление ZipCompressionLevel

    #region Перечисление ZipProgressEventType

    public enum ZipProgressEventType
    {
        Adding_Started,

        Adding_AfterAddEntry,

        Adding_Completed,

        Reading_Started,

        Reading_BeforeReadEntry,

        Reading_AfterReadEntry,

        Reading_Completed,

        Reading_ArchiveBytesRead,

        Saving_Started,

        Saving_BeforeWriteEntry,

        Saving_AfterWriteEntry,

        Saving_Completed,

        Saving_AfterSaveTempArchive,

        Saving_BeforeRenameTempArchive,

        Saving_AfterRenameTempArchive,

        Saving_AfterCompileSelfExtractor,

        Saving_EntryBytesRead,

        Extracting_BeforeExtractEntry,

        Extracting_AfterExtractEntry,

        Extracting_ExtractEntryWouldOverwrite,

        Extracting_EntryBytesWritten,

        Extracting_BeforeExtractAll,

        Extracting_AfterExtractAll,

        Error_Saving,
    }

    #endregion Перечисление ZipProgressEventType

    #region Перечисление ZipOption

    public enum ZipOption
    {
        Default = 0,

        Never = 0,

        AsNecessary = 1,

        Always
    }

    #endregion Перечисление ZipOption

    #region Перечисление Zip64Option

    public enum Zip64Option
    {
        Default = 0,

        Never = 0,

        AsNecessary = 1,

        Always
    }

    #endregion Перечисление Zip64Option

    #region Перечисление ZipFlushType

    public enum ZipFlushType
    {
        None = 0,

        Partial,

        Sync,

        Full,

        Finish,
    }

    #endregion Перечисление ZipFlushType

    #region Перечисление ZipBlockState

    internal enum ZipBlockState
    {
        NeedMore = 0,

        BlockDone,

        FinishStarted,

        FinishDone
    }

    #endregion Перечисление ZipBlockState

    #region Перечисление ZipDeflateFlavor

    internal enum ZipDeflateFlavor
    {
        Store,

        Fast,

        Slow
    }

    #endregion Перечисление ZipDeflateFlavor

    #region Перечисление ZlibStreamFlavor

    internal enum ZlibStreamFlavor
    {
        ZLIB = 1950,

        DEFLATE = 1951,

        GZIP = 1952
    }

    #endregion Перечисление ZlibStreamFlavor

    #region Перечисление ZipAddOrUpdateAction

    internal enum ZipAddOrUpdateAction
    {
        AddOnly = 0,

        AddOrUpdate
    }

    #endregion Перечисление ZipAddOrUpdateAction

    #region Перечисление ZipEncryptionAlgorithm

    public enum ZipEncryptionAlgorithm
    {
        None = 0,

        PkzipWeak,

        WinZipAes128,

        WinZipAes256,

        Unsupported = 4,
    }

    #endregion Перечисление ZipEncryptionAlgorithm

    #region Перечисление ZipCompressionStrategy

    public enum ZipCompressionStrategy
    {
        Default = 0,

        Filtered = 1,

        HuffmanOnly = 2,
    }

    #endregion Перечисление ZipCompressionStrategy

    #region Перечисление ZipCompressionMode

    public enum ZipCompressionMode
    {
        Compress = 0,

        Decompress = 1,
    }

    #endregion Перечисление ZipCompressionMode

    #region Перечисление ZipExtractExistingFileAction

    public enum ZipExtractExistingFileAction
    {
        Throw,

        OverwriteSilently,

        DoNotOverwrite,

        InvokeExtractProgressEvent,
    }

    #endregion Перечисление ZipExtractExistingFileAction

    #region Перечисление ZipEntrySource

    public enum ZipEntrySource
    {
        None = 0,

        FileSystem,

        Stream,

        ZipFile,

        WriteDelegate,

        JitStream,

        ZipOutputStream,
    }

    #endregion Перечисление ZipEntrySource

    #region Перечисление ZipEntryTimestamp

    [Flags]
    public enum ZipEntryTimestamp
    {
        None = 0,

        DOS = 1,

        Windows = 2,

        Unix = 4,

        InfoZip1 = 8,
    }

    #endregion Перечисление ZipEntryTimestamp

    #region Перечисление ZipCryptoMode

    internal enum ZipCryptoMode
    {
        Encrypt,

        Decrypt
    }

    #endregion Перечисление ZipCryptoMode

    #endregion Перечисления

    #region Делегаты

    #region Делегат ZipSetCompressionCallback

    public delegate ZipCompressionLevel ZipSetCompressionCallback(String localFileName, String fileNameInArchive);

    #endregion Делегат ZipSetCompressionCallback

    #region Делегат ZipWriteDelegate

    public delegate void ZipWriteDelegate(String entryName, System.IO.Stream stream);

    #endregion Делегат ZipWriteDelegate

    #region Делегат ZipOpenDelegate

    public delegate System.IO.Stream ZipOpenDelegate(String entryName);

    #endregion Делегат ZipOpenDelegate

    #region Делегат ZipCloseDelegate

    public delegate void ZipCloseDelegate(String entryName, System.IO.Stream stream);

    #endregion Делегат ZipCloseDelegate

    #endregion Делегаты

    #region Классы

    #region Класс ZipFile

    [Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d00005")]
    [Interop.ComVisible(true)]
    [Interop.ClassInterface(Interop.ClassInterfaceType.AutoDispatch)]
    public class ZipFile : IEnumerable, IEnumerable<ZipEntry>, IDisposable
    {
        #region Поля

        private TextWriter _statusMessageTextWriter;

        private Boolean _caseSensitiveRetrieval;

        private Stream _readstream;

        private Stream _writestream;

        private UInt32 _diskNumberWithCd;

        private Int32 _maxOutputSegmentSize;

        private UInt32 _numberOfSegmentsForMostRecentSave;

        private ZipErrorAction _zipErrorAction;

        private Boolean _disposed;

        private Dictionary<String, ZipEntry> _entries;

        private List<ZipEntry> _zipEntriesAsList;

        private String _name;

        private String _readName;

        private String _Comment;

        private Boolean _emitNtfsTimes;

        private Boolean _emitUnixTimes;

        private Boolean _fileAlreadyExists;

        private String _temporaryFileName;

        private Boolean _contentsChanged;

        private Boolean _hasBeenSaved;

        private String _TempFileFolder;

        private Boolean _readStreamIsOurs;

        private Object _lock;

        private Boolean _saveOperationCanceled;

        private Boolean _extractOperationCanceled;

        private Boolean _addOperationCanceled;

        private ZipEncryptionAlgorithm _encryption;

        private Boolean _justSaved;

        private long _locEndOfCDS;

        private uint _offsetOfCentralDirectory;

        private Int64 _offsetOfCentralDirectory64;

        private bool? _outputUsesZip64;

        private Encoding _alternateEncoding;

        private ZipOption _alternateEncodingUsage;

        private Int64 _lengthOfReadStream;

        private long _ParallelDeflateThreshold;

        private int _maxBufferPairs;
#pragma warning disable 649
        private Boolean _savingSfx;
#pragma warning restore 649
        internal String _password;

        internal Boolean _inExtractAll;

        internal ZipParallelDeflateOutputStream ParallelDeflater;

        internal Zip64Option _zip64;

        private static Encoding _defaultEncoding;

        public static readonly int BufferSizeDefault;

        #endregion Поля

        #region Свойства

        private List<ZipEntry> ZipEntriesAsList
        {
            get
            {
                if (_zipEntriesAsList == null)
                    _zipEntriesAsList = new List<ZipEntry>(_entries.Values);
                return _zipEntriesAsList;
            }
        }

        private Stream WriteStream
        {
            // workitem 9763
            get
            {
                if (_writestream != null) return _writestream;
                if (_name == null) return _writestream;

                if (_maxOutputSegmentSize != 0)
                {
                    _writestream = ZipSegmentedStream.ForWriting(_name, _maxOutputSegmentSize);
                    return _writestream;
                }

                ZipSharedUtilities.CreateAndOpenUniqueTempFile(TempFileFolder ?? Path.GetDirectoryName(_name),
                                                               out _writestream,
                                                               out _temporaryFileName);
                return _writestream;
            }
        }

        private String ArchiveNameForEvent
        {
            get { return (_name != null) ? _name : "(stream)"; }
        }

        private Int64 LengthOfReadStream
        {
            get
            {
                if (_lengthOfReadStream == -99)
                {
                    _lengthOfReadStream = (_readStreamIsOurs)
                                              ? ZipSharedUtilities.GetFileLength(_name)
                                              : -1L;
                }
                return _lengthOfReadStream;
            }
        }

        internal Stream ReadStream
        {
            get
            {
                if (_readstream == null)
                {
                    if (_readName != null || _name != null)
                    {
                        _readstream = File.Open(_readName ?? _name,
                                                FileMode.Open,
                                                FileAccess.Read,
                                                FileShare.Read | FileShare.Write);
                        _readStreamIsOurs = true;
                    }
                }
                return _readstream;
            }
        }

        internal Boolean Verbose
        {
            get { return (_statusMessageTextWriter != null); }
        }

        public Boolean FullScan { get; set; }

        public Boolean SortEntriesBeforeSaving { get; set; }

        public Boolean AddDirectoryWillTraverseReparsePoints { get; set; }

        public int BufferSize { get; set; }

        public int CodecBufferSize { get; set; }

        public Boolean FlattenFoldersOnExtract { get; set; }

        public ZipCompressionStrategy Strategy { get; set; }

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ZipCompressionLevel CompressionLevel { get; set; }

        public ZipCompressionMethod CompressionMethod { get; set; }

        public String Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                _contentsChanged = true;
            }
        }

        public Boolean EmitTimesInWindowsFormatWhenSaving
        {
            get { return _emitNtfsTimes; }
            set { _emitNtfsTimes = value; }
        }

        public Boolean EmitTimesInUnixFormatWhenSaving
        {
            get { return _emitUnixTimes; }
            set { _emitUnixTimes = value; }
        }

        public Boolean CaseSensitiveRetrieval
        {
            get { return _caseSensitiveRetrieval; }
            set
            {
                if (value != _caseSensitiveRetrieval)
                {
                    _caseSensitiveRetrieval = value;
                    _initEntriesDictionary();
                }
            }
        }

        public Zip64Option UseZip64WhenSaving
        {
            get { return _zip64; }
            set { _zip64 = value; }
        }

        public bool? RequiresZip64
        {
            get
            {
                if (_entries.Count > 65534)
                    return true;

                // If the <c>ZipFile</c> has not been saved or if the contents have changed, then
                // it is not known if ZIP64 is required.
                if (!_hasBeenSaved || _contentsChanged) return null;

                // Whether ZIP64 is required is knowable.
                foreach (ZipEntry e in _entries.Values)
                    if (e.RequiresZip64.Value) return true;

                return false;
            }
        }

        public bool? OutputUsedZip64
        {
            get { return _outputUsesZip64; }
        }

        public bool? InputUsesZip64
        {
            get
            {
                if (_entries.Count > 65534)
                    return true;

                foreach (ZipEntry e in this)
                {
                    // if any entry was added after reading the zip file, then the result is null
                    if (e.Source != ZipEntrySource.ZipFile) return null;

                    // if any entry read from the zip used zip64, then the result is true
                    if (e._InputUsesZip64) return true;
                }
                return false;
            }
        }

        public System.Text.Encoding AlternateEncoding
        {
            get { return _alternateEncoding; }
            set { _alternateEncoding = value; }
        }

        public ZipOption AlternateEncodingUsage
        {
            get { return _alternateEncodingUsage; }
            set { _alternateEncodingUsage = value; }
        }

        public static Encoding DefaultEncoding
        {
            get { return _defaultEncoding; }
        }

        public TextWriter StatusMessageTextWriter
        {
            get { return _statusMessageTextWriter; }
            set { _statusMessageTextWriter = value; }
        }

        public String TempFileFolder
        {
            get { return _TempFileFolder; }
            set
            {
                _TempFileFolder = value;
                if (value == null) return;

                if (!Directory.Exists(value))
                    throw new FileNotFoundException(String.Format("That directory ({0}) does not exist.", value));
            }
        }

        public String Password
        {
            set
            {
                _password = value;
                if (_password == null)
                    Encryption = ZipEncryptionAlgorithm.None;
                else if (Encryption == ZipEncryptionAlgorithm.None)
                    Encryption = ZipEncryptionAlgorithm.PkzipWeak;
            }
        }

        public ZipExtractExistingFileAction ExtractExistingFile { get; set; }

        public ZipErrorAction ZipErrorAction
        {
            get
            {
                if (ZipError != null)
                    _zipErrorAction = ZipErrorAction.InvokeErrorEvent;
                return _zipErrorAction;
            }
            set
            {
                _zipErrorAction = value;
                if (_zipErrorAction != ZipErrorAction.InvokeErrorEvent && ZipError != null)
                    ZipError = null;
            }
        }

        public ZipEncryptionAlgorithm Encryption
        {
            get { return _encryption; }
            set
            {
                if (value == ZipEncryptionAlgorithm.Unsupported)
                    throw new InvalidOperationException("You may not set Encryption to that value.");
                _encryption = value;
            }
        }

        public ZipSetCompressionCallback SetCompression { get; set; }

        public Int32 MaxOutputSegmentSize
        {
            get { return _maxOutputSegmentSize; }
            set
            {
                if (value < 65536 && value != 0)
                    throw new ZipException("The minimum acceptable segment size is 65536.");
                _maxOutputSegmentSize = value;
            }
        }

        public Int32 NumberOfSegmentsForMostRecentSave
        {
            get { return unchecked((Int32)_numberOfSegmentsForMostRecentSave + 1); }
        }

        public long ParallelDeflateThreshold
        {
            get { return _ParallelDeflateThreshold; }
            set
            {
                if ((value != 0) && (value != -1) && (value < 64 * 1024))
                    throw new ArgumentOutOfRangeException("ParallelDeflateThreshold should be -1, 0, or > 65536");
                _ParallelDeflateThreshold = value;
            }
        }

        public int ParallelDeflateMaxBufferPairs
        {
            get { return _maxBufferPairs; }
            set
            {
                if (value < 4)
                {
                    throw new ArgumentOutOfRangeException("ParallelDeflateMaxBufferPairs",
                                                          "Value must be 4 or greater.");
                }
                _maxBufferPairs = value;
            }
        }

        public static System.Version LibraryVersion
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public ZipEntry this[int ix]
        {
            // workitem 6402
            get { return ZipEntriesAsList[ix]; }
        }

        public ZipEntry this[String fileName]
        {
            get
            {
                var key = ZipSharedUtilities.NormalizePathForUseInZipFile(fileName);
                if (_entries.ContainsKey(key))
                    return _entries[key];
                // workitem 11056
                key = key.Replace("/", "\\");
                if (_entries.ContainsKey(key))
                    return _entries[key];
                return null;
            }
        }

        public ICollection<String> EntryFileNames
        {
            get { return _entries.Keys; }
        }

        public ICollection<ZipEntry> Entries
        {
            get { return _entries.Values; }
        }

        public ICollection<ZipEntry> EntriesSorted
        {
            get
            {
                var coll = new System.Collections.Generic.List<ZipEntry>();
                foreach (var e in Entries)
                    coll.Add(e);
                StringComparison sc = (CaseSensitiveRetrieval) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                coll.Sort((x, y) => { return String.Compare(x.FileName, y.FileName, sc); });
                return coll.AsReadOnly();
            }
        }

        public int Count
        {
            get { return _entries.Count; }
        }

        public List<String> Filters { get; set; }

        public String Filter
        {
            get
            {
                if (Filter != null && Filters.Count > 0)
                    return Filters[0];

                return "";
            }
            set
            {
                Filters = new List<String>
                    {
                        value
                    };
            }
        }

        #endregion Свойства

        #region События

        public event EventHandler<ZipSaveProgressEventArgs> SaveProgress;

        public event EventHandler<ZipReadProgressEventArgs> ReadProgress;

        public event EventHandler<ZipExtractProgressEventArgs> ExtractProgress;

        public event EventHandler<ZipAddProgressEventArgs> AddProgress;

        public event EventHandler<ZipErrorEventArgs> ZipError;

        #endregion События

        #region Конструктор

        static ZipFile()
        {
            BufferSizeDefault = 32768;
        }

        public ZipFile()
        {
            _emitNtfsTimes = true;
            Strategy = ZipCompressionStrategy.Default;
            CompressionMethod = ZipCompressionMethod.Deflate;
            _readStreamIsOurs = true;
            _lock = new object();
            _locEndOfCDS = -1;
            _alternateEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            _alternateEncodingUsage = ZipOption.Always;
            _lengthOfReadStream = -99;
            BufferSize = BufferSizeDefault;
            _maxBufferPairs = 16;
            _defaultEncoding = System.Text.Encoding.GetEncoding("IBM437");
            _zip64 = Zip64Option.Default;
            Filters = new List<String>();

            _InitInstance(null, null);
        }

        //public ZipFile(String fileName)
        //{
        //    try
        //    {
        //        _InitInstance(fileName, null);
        //    }
        //    catch (Exception e1)
        //    {
        //        throw new ZipException(String.Format("Could not read {0} as a zip file", fileName), e1);
        //    }
        //}
        //public ZipFile(String fileName, System.Text.Encoding encoding)
        //{
        //    try
        //    {
        //        AlternateEncoding = encoding;
        //        AlternateEncodingUsage = ZipOption.Always;
        //        _InitInstance(fileName, null);
        //    }
        //    catch (Exception e1)
        //    {
        //        throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
        //    }
        //}
        //public ZipFile(System.Text.Encoding encoding)
        //{
        //    AlternateEncoding = encoding;
        //    AlternateEncodingUsage = ZipOption.Always;
        //    _InitInstance(null, null);
        //}
        //public ZipFile(String fileName, TextWriter statusMessageWriter)
        //{
        //    try
        //    {
        //        _InitInstance(fileName, statusMessageWriter);
        //    }
        //    catch (Exception e1)
        //    {
        //        throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
        //    }
        //}
        //public ZipFile(String fileName, TextWriter statusMessageWriter, System.Text.Encoding encoding)
        //{
        //    try
        //    {
        //        AlternateEncoding = encoding;
        //        AlternateEncodingUsage = ZipOption.Always;
        //        _InitInstance(fileName, statusMessageWriter);
        //    }
        //    catch (Exception e1)
        //    {
        //        throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
        //    }
        //}
        public void Dispose()
        {
            // dispose of the managed and unmanaged resources
            Dispose(true);

            // tell the GC that the Finalize process no longer needs
            // to be run for this object.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposeManagedResources)
        {
            if (!_disposed)
            {
                if (disposeManagedResources)
                {
                    // dispose managed resources
                    if (_readStreamIsOurs)
                    {
                        if (_readstream != null)
                        {
                            // workitem 7704
                            _readstream.Dispose();
                            _readstream = null;
                        }
                    }
                    // only dispose the writestream if there is a backing file
                    if ((_temporaryFileName != null) && (_name != null))
                    {
                        if (_writestream != null)
                        {
                            // workitem 7704
                            _writestream.Dispose();
                            _writestream = null;
                        }
                    }

                    // workitem 10030
                    if (ParallelDeflater != null)
                    {
                        ParallelDeflater.Dispose();
                        ParallelDeflater = null;
                    }
                }
                _disposed = true;
            }
        }

        #endregion Конструктор

        #region Методы

        #region Инициализация

        private void _initEntriesDictionary()
        {
            // workitem 9868
            StringComparer sc = (CaseSensitiveRetrieval) ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            _entries = (_entries == null)
                           ? new Dictionary<String, ZipEntry>(sc)
                           : new Dictionary<String, ZipEntry>(_entries, sc);
        }

        private void _InitInstance(String zipFileName, TextWriter statusMessageWriter)
        {
            // create a new zipfile
            _name = zipFileName;
            _statusMessageTextWriter = statusMessageWriter;
            _contentsChanged = true;
            AddDirectoryWillTraverseReparsePoints = true; // workitem 8617
            CompressionLevel = ZipCompressionLevel.Default;
            ParallelDeflateThreshold = 512 * 1024;
            // workitem 7685, 9868
            _initEntriesDictionary();

            if (File.Exists(_name))
            {
                if (FullScan)
                    ReadIntoInstance_Orig(this);
                else
                    ReadIntoInstance(this);
                _fileAlreadyExists = true;
            }

            return;
        }

        internal void NotifyEntryChanged()
        {
            _contentsChanged = true;
        }

        internal Stream StreamForDiskNumber(uint diskNumber)
        {
            if (diskNumber + 1 == _diskNumberWithCd ||
                (diskNumber == 0 && _diskNumberWithCd == 0))
                return ReadStream;
            return ZipSegmentedStream.ForReading(_readName ?? _name, diskNumber, _diskNumberWithCd);
        }

        internal void Reset(Boolean whileSaving)
        {
            if (_justSaved)
            {
                // read in the just-saved zip archive
                using (ZipFile x = new ZipFile())
                {
                    x._readName = x._name = whileSaving
                                                ? (_readName ?? _name)
                                                : _name;
                    x.AlternateEncoding = AlternateEncoding;
                    x.AlternateEncodingUsage = AlternateEncodingUsage;
                    ReadIntoInstance(x);
                    // copy the contents of the entries.
                    // cannot just replace the entries - the app may be holding them
                    foreach (ZipEntry e1 in x)
                    {
                        foreach (ZipEntry e2 in this)
                        {
                            if (e1.FileName == e2.FileName)
                            {
                                e2.CopyMetaData(e1);
                                break;
                            }
                        }
                    }
                }
                _justSaved = false;
            }
        }

        public void Initialize(String fileName)
        {
            try
            {
                _InitInstance(fileName, null);
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
        }

        public Boolean ContainsEntry(String name)
        {
            return _entries.ContainsKey(ZipSharedUtilities.NormalizePathForUseInZipFile(name));
        }

        public override String ToString()
        {
            return String.Format("ZipFile::{0}", Name);
        }

        public void RemoveEntry(ZipEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            _entries.Remove(ZipSharedUtilities.NormalizePathForUseInZipFile(entry.FileName));
            _zipEntriesAsList = null;
            _contentsChanged = true;
        }

        public void RemoveEntry(String fileName)
        {
            String modifiedName = ZipEntry.NameInArchive(fileName, null);
            ZipEntry e = this[modifiedName];
            if (e == null)
                throw new ArgumentException("The entry you specified was not found in the zip archive.");

            RemoveEntry(e);
        }

        #endregion Инициализация

        #region Реализация Enumerable

        public System.Collections.Generic.IEnumerator<ZipEntry> GetEnumerator()
        {
            foreach (ZipEntry e in _entries.Values)
                yield return e;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [System.Runtime.InteropServices.DispId(-4)]
        public System.Collections.IEnumerator GetNewEnum()
        {
            return GetEnumerator();
        }

        #endregion Реализация Enumerable

        #region Сохранение

        internal Boolean OnSaveBlock(ZipEntry entry, Int64 BytesXferred, Int64 totalBytesToXfer)
        {
            EventHandler<ZipSaveProgressEventArgs> sp = SaveProgress;
            if (sp != null)
            {
                var e = ZipSaveProgressEventArgs.ByteUpdate(ArchiveNameForEvent, entry,
                                                            BytesXferred, totalBytesToXfer);
                sp(this, e);
                if (e.Cancel)
                    _saveOperationCanceled = true;
            }
            return _saveOperationCanceled;
        }

        private void OnSaveEntry(int current, ZipEntry entry, Boolean before)
        {
            EventHandler<ZipSaveProgressEventArgs> sp = SaveProgress;
            if (sp != null)
            {
                var e = new ZipSaveProgressEventArgs(ArchiveNameForEvent, before, _entries.Count, current, entry);
                sp(this, e);
                if (e.Cancel)
                    _saveOperationCanceled = true;
            }
        }

        private void OnSaveEvent(ZipProgressEventType eventFlavor)
        {
            EventHandler<ZipSaveProgressEventArgs> sp = SaveProgress;
            if (sp != null)
            {
                var e = new ZipSaveProgressEventArgs(ArchiveNameForEvent, eventFlavor);
                sp(this, e);
                if (e.Cancel)
                    _saveOperationCanceled = true;
            }
        }

        private void OnSaveStarted()
        {
            EventHandler<ZipSaveProgressEventArgs> sp = SaveProgress;
            if (sp != null)
            {
                var e = ZipSaveProgressEventArgs.Started(ArchiveNameForEvent);
                sp(this, e);
                if (e.Cancel)
                    _saveOperationCanceled = true;
            }
        }

        private void OnSaveCompleted()
        {
            EventHandler<ZipSaveProgressEventArgs> sp = SaveProgress;
            if (sp != null)
            {
                var e = ZipSaveProgressEventArgs.Completed(ArchiveNameForEvent);
                sp(this, e);
            }
        }

        private void DeleteFileWithRetry(String filename)
        {
            Boolean done = false;
            int nRetries = 3;
            for (int i = 0; i < nRetries && !done; i++)
            {
                try
                {
                    File.Delete(filename);
                    done = true;
                }
                catch (System.UnauthorizedAccessException)
                {
                    Console.WriteLine("************************************************** Retry delete.");
                    System.Threading.Thread.Sleep(200 + i * 200);
                }
            }
        }

        public void Save()
        {
            try
            {
                Boolean thisSaveUsedZip64 = false;
                _saveOperationCanceled = false;
                _numberOfSegmentsForMostRecentSave = 0;
                OnSaveStarted();

                if (WriteStream == null)
                    throw new ZipBadStateException("You haven't specified where to save the zip.");

                if (_name != null && _name.EndsWith(".exe") && !_savingSfx)
                    throw new ZipBadStateException("You specified an EXE for a plain zip file.");

                // check if modified, before saving.
                if (_contentsChanged == false)
                {
                    OnSaveCompleted();
                    if (Verbose) StatusMessageTextWriter.WriteLine("No save is necessary....");
                    return;
                }

                Reset(true);

                if (Verbose) StatusMessageTextWriter.WriteLine("saving....");

                // validate the number of entries
                if (_entries.Count >= 0xFFFF && _zip64 == Zip64Option.Never)
                    throw new ZipException("The number of entries is 65535 or greater. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");

                // write an entry in the zip for each file
                int n = 0;
                // workitem 9831
                ICollection<ZipEntry> c = (SortEntriesBeforeSaving) ? EntriesSorted : Entries;
                foreach (ZipEntry e in c) // _entries.Values
                {
                    OnSaveEntry(n, e, true);
                    e.Write(WriteStream);
                    if (_saveOperationCanceled)
                        break;

                    n++;
                    OnSaveEntry(n, e, false);
                    if (_saveOperationCanceled)
                        break;

                    // Some entries can be skipped during the save.
                    if (e.IncludedInMostRecentSave)
                        thisSaveUsedZip64 |= e.OutputUsedZip64.Value;
                }

                if (_saveOperationCanceled)
                    return;

                var zss = WriteStream as ZipSegmentedStream;

                _numberOfSegmentsForMostRecentSave = (zss != null)
                                                         ? zss.CurrentSegment
                                                         : 1;

                Boolean directoryNeededZip64 =
                    ZipOutput.WriteCentralDirectoryStructure
                        (WriteStream,
                         c,
                         _numberOfSegmentsForMostRecentSave,
                         _zip64,
                         Comment,
                         new ZipContainer(this));

                OnSaveEvent(ZipProgressEventType.Saving_AfterSaveTempArchive);

                _hasBeenSaved = true;
                _contentsChanged = false;

                thisSaveUsedZip64 |= directoryNeededZip64;
                _outputUsesZip64 = thisSaveUsedZip64;

                // do the rename as necessary
                if (_name != null && (_temporaryFileName != null || zss != null))
                {
                    // _temporaryFileName may remain null if we are writing to a stream.
                    // only close the stream if there is a file behind it.
                    WriteStream.Dispose();

                    if (_saveOperationCanceled)
                        return;

                    if (_fileAlreadyExists && _readstream != null)
                    {
                        // This means we opened and read a zip file.
                        // If we are now saving to the same file, we need to close the
                        // orig file, first.
                        _readstream.Close();
                        _readstream = null;
                        // the archiveStream for each entry needs to be null
                        foreach (var e in c)
                        {
                            var zss1 = e._archiveStream as ZipSegmentedStream;
                            if (zss1 != null)
                                zss1.Dispose();
                            e._archiveStream = null;
                        }
                    }

                    String tmpName = null;
                    if (File.Exists(_name))
                    {
                        // the steps:
                        //
                        // 1. Delete tmpName
                        // 2. move existing zip to tmpName
                        // 3. rename (File.Move) working file to name of existing zip
                        // 4. delete tmpName
                        //
                        // This series of steps avoids the exception,
                        // System.IO.IOException:
                        //   "Cannot create a file when that file already exists."
                        //
                        // Cannot just call File.Replace() here because
                        // there is a possibility that the TEMP volume is different
                        // that the volume for the final file (c:\ vs d:\).
                        // So we need to do a Delete+Move pair.
                        //
                        // But, when doing the delete, Windows allows a process to
                        // delete the file, even though it is held open by, say, a
                        // virus scanner. It gets internally marked as "delete
                        // pending". The file does not actually get removed from the
                        // file system, it is still there after the File.Delete
                        // call.
                        //
                        // Therefore, we need to move the existing zip, which may be
                        // held open, to some other name. Then rename our working
                        // file to the desired name, then delete (possibly delete
                        // pending) the "other name".
                        //
                        // Ideally this would be transactional. It's possible that the
                        // delete succeeds and the move fails. Lacking transactions, if
                        // this kind of failure happens, we're hosed, and this logic will
                        // throw on the next File.Move().
                        //
                        //File.Delete(_name);
                        // workitem 10447
                        tmpName = _name + "." + Path.GetRandomFileName();
                        if (File.Exists(tmpName))
                            DeleteFileWithRetry(tmpName);
                        File.Move(_name, tmpName);
                    }

                    OnSaveEvent(ZipProgressEventType.Saving_BeforeRenameTempArchive);
                    File.Move((zss != null) ? zss.CurrentTempName : _temporaryFileName, _name);

                    OnSaveEvent(ZipProgressEventType.Saving_AfterRenameTempArchive);

                    if (tmpName != null)
                    {
                        try
                        {
                            // not critical
                            if (File.Exists(tmpName))
                                File.Delete(tmpName);
                        }
                        catch
                        {
                            // don't care about exceptions here.
                        }
                    }
                    _fileAlreadyExists = true;
                }

                NotifyEntriesSaveComplete(c);
                OnSaveCompleted();
                _justSaved = true;
            }
            finally
            {
                CleanupAfterSaveOperation();
            }

            return;
        }

        private static void NotifyEntriesSaveComplete(ICollection<ZipEntry> c)
        {
            foreach (ZipEntry e in c)
                e.NotifySaveComplete();
        }

        private void RemoveTempFile()
        {
            try
            {
                if (File.Exists(_temporaryFileName))
                    File.Delete(_temporaryFileName);
            }
            catch (IOException ex1)
            {
                if (Verbose)
                    StatusMessageTextWriter.WriteLine("ZipFile::Save: could not delete temp file: {0}.", ex1.Message);
            }
        }

        private void CleanupAfterSaveOperation()
        {
            if (_name != null)
            {
                // close the stream if there is a file behind it.
                if (_writestream != null)
                {
                    try
                    {
                        // workitem 7704
                        _writestream.Dispose();
                    }
                    catch (System.IO.IOException)
                    {
                    }
                }
                _writestream = null;

                if (_temporaryFileName != null)
                {
                    RemoveTempFile();
                    _temporaryFileName = null;
                }
            }
        }

        public void Save(String fileName)
        {
            // Check for the case where we are re-saving a zip archive
            // that was originally instantiated with a stream.  In that case,
            // the _name will be null. If so, we set _writestream to null,
            // which insures that we'll cons up a new WriteStream (with a filesystem
            // file backing it) in the Save() method.
            if (_name == null)
                _writestream = null;
            else
                _readName = _name;

            _name = fileName;
            if (Directory.Exists(_name))
                throw new ZipException("Bad Directory", new System.ArgumentException("That name specifies an existing directory. Please specify a filename.", "fileName"));
            _contentsChanged = true;
            _fileAlreadyExists = File.Exists(_name);

            Save();
        }

        public void Save(Stream outputStream)
        {
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");
            if (!outputStream.CanWrite)
                throw new ArgumentException("Must be a writable stream.", "outputStream");

            // if we had a filename to save to, we are now obliterating it.
            _name = null;

            _writestream = new ZipCountingStream(outputStream);

            _contentsChanged = true;
            _fileAlreadyExists = false;
            Save();
        }

        #endregion Сохранение

        #region Чтение

        private void OnReadStarted()
        {
            EventHandler<ZipReadProgressEventArgs> rp = ReadProgress;
            if (rp != null)
            {
                var e = ZipReadProgressEventArgs.Started(ArchiveNameForEvent);
                rp(this, e);
            }
        }

        private void OnReadCompleted()
        {
            EventHandler<ZipReadProgressEventArgs> rp = ReadProgress;
            if (rp != null)
            {
                var e = ZipReadProgressEventArgs.Completed(ArchiveNameForEvent);
                rp(this, e);
            }
        }

        internal void OnReadBytes(ZipEntry entry)
        {
            EventHandler<ZipReadProgressEventArgs> rp = ReadProgress;
            if (rp != null)
            {
                var e = ZipReadProgressEventArgs.ByteUpdate(ArchiveNameForEvent,
                                                            entry,
                                                            ReadStream.Position,
                                                            LengthOfReadStream);
                rp(this, e);
            }
        }

        internal void OnReadEntry(Boolean before, ZipEntry entry)
        {
            EventHandler<ZipReadProgressEventArgs> rp = ReadProgress;
            if (rp != null)
            {
                ZipReadProgressEventArgs e = (before)
                                                 ? ZipReadProgressEventArgs.Before(ArchiveNameForEvent, _entries.Count)
                                                 : ZipReadProgressEventArgs.After(ArchiveNameForEvent, entry, _entries.Count);
                rp(this, e);
            }
        }

        public static ZipFile Read(String fileName)
        {
            return ZipFile.Read(fileName, null, null, null);
        }

        public static ZipFile Read(String fileName, ZipReadOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            return Read(fileName,
                        options.StatusMessageWriter,
                        options.Encoding,
                        options.ReadProgress);
        }

        private static ZipFile Read(String fileName, TextWriter statusMessageWriter, System.Text.Encoding encoding, EventHandler<ZipReadProgressEventArgs> readProgress)
        {
            ZipFile zf = new ZipFile();
            zf.AlternateEncoding = encoding ?? DefaultEncoding;
            zf.AlternateEncodingUsage = ZipOption.Always;
            zf._statusMessageTextWriter = statusMessageWriter;
            zf._name = fileName;
            if (readProgress != null)
                zf.ReadProgress = readProgress;

            if (zf.Verbose) zf._statusMessageTextWriter.WriteLine("reading from {0}...", fileName);

            ReadIntoInstance(zf);
            zf._fileAlreadyExists = true;

            return zf;
        }

        public static ZipFile Read(Stream zipStream)
        {
            return Read(zipStream, null, null, null);
        }

        public static ZipFile Read(Stream zipStream, ZipReadOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            return Read(zipStream,
                        options.StatusMessageWriter,
                        options.Encoding,
                        options.ReadProgress);
        }

        private static ZipFile Read(Stream zipStream, TextWriter statusMessageWriter, System.Text.Encoding encoding, EventHandler<ZipReadProgressEventArgs> readProgress)
        {
            if (zipStream == null)
                throw new ArgumentNullException("zipStream");

            ZipFile zf = new ZipFile();
            zf._statusMessageTextWriter = statusMessageWriter;
            zf._alternateEncoding = encoding ?? ZipFile.DefaultEncoding;
            zf._alternateEncodingUsage = ZipOption.Always;
            if (readProgress != null)
                zf.ReadProgress += readProgress;
            zf._readstream = (zipStream.Position == 0L)
                                 ? zipStream
                                 : new ZipOffsetStream(zipStream);
            zf._readStreamIsOurs = false;
            if (zf.Verbose) zf._statusMessageTextWriter.WriteLine("reading from stream...");

            ReadIntoInstance(zf);
            return zf;
        }

        private static void ReadIntoInstance(ZipFile zf)
        {
            Stream s = zf.ReadStream;
            try
            {
                zf._readName = zf._name; // workitem 13915
                if (!s.CanSeek)
                {
                    ReadIntoInstance_Orig(zf);
                    return;
                }

                zf.OnReadStarted();

                // change for workitem 8098
                //zf._originPosition = s.Position;

                // Try reading the central directory, rather than scanning the file.

                uint datum = ReadFirstFourBytes(s);

                if (datum == ZipConstants.EndOfCentralDirectorySignature)
                    return;

                // start at the end of the file...
                // seek backwards a bit, then look for the EoCD signature.
                int nTries = 0;
                Boolean success = false;

                // The size of the end-of-central-directory-footer plus 2 Bytes is 18.
                // This implies an archive comment length of 0.  We'll add a margin of
                // safety and start "in front" of that, when looking for the
                // EndOfCentralDirectorySignature
                long posn = s.Length - 64;
                long maxSeekback = Math.Max(s.Length - 0x4000, 10);
                do
                {
                    if (posn < 0) posn = 0; // BOF
                    s.Seek(posn, SeekOrigin.Begin);
                    long BytesRead = ZipSharedUtilities.FindSignature(s, (int)ZipConstants.EndOfCentralDirectorySignature);
                    if (BytesRead != -1)
                        success = true;
                    else
                    {
                        if (posn == 0) break; // started at the BOF and found nothing
                        nTries++;
                        // Weird: with NETCF, negative offsets from SeekOrigin.End DO
                        // NOT WORK. So rather than seek a negative offset, we seek
                        // from SeekOrigin.Begin using a smaller number.
                        posn -= (32 * (nTries + 1) * nTries);
                    }
                } while (!success && posn > maxSeekback);

                if (success)
                {
                    // workitem 8299
                    zf._locEndOfCDS = s.Position - 4;

                    Byte[] block = new Byte[16];
                    s.Read(block, 0, block.Length);

                    zf._diskNumberWithCd = BitConverter.ToUInt16(block, 2);

                    if (zf._diskNumberWithCd == 0xFFFF)
                        throw new ZipException("Spanned archives with more than 65534 segments are not supported at this time.");

                    zf._diskNumberWithCd++; // I think the number in the file differs from reality by 1

                    int i = 12;

                    uint offset32 = BitConverter.ToUInt32(block, i);
                    if (offset32 == 0xFFFFFFFF)
                        Zip64SeekToCentralDirectory(zf);
                    else
                    {
                        zf._offsetOfCentralDirectory = offset32;
                        // change for workitem 8098
                        s.Seek(offset32, SeekOrigin.Begin);
                    }

                    ReadCentralDirectory(zf);
                }
                else
                {
                    // Could not find the central directory.
                    // Fallback to the old method.
                    // workitem 8098: ok
                    //s.Seek(zf._originPosition, SeekOrigin.Begin);
                    s.Seek(0L, SeekOrigin.Begin);
                    ReadIntoInstance_Orig(zf);
                }
            }
            catch (Exception ex1)
            {
                if (zf._readStreamIsOurs && zf._readstream != null)
                {
                    try
                    {
#if NETCF
                        zf._readstream.Close();
#else
                        zf._readstream.Dispose();
#endif
                        zf._readstream = null;
                    }
                    finally
                    {
                    }
                }

                throw new ZipException("Cannot read that as a ZipFile", ex1);
            }

            // the instance has been read in
            zf._contentsChanged = false;
        }

        private static void Zip64SeekToCentralDirectory(ZipFile zf)
        {
            Stream s = zf.ReadStream;
            Byte[] block = new Byte[16];

            // seek back to find the ZIP64 EoCD.
            // I think this might not work for .NET CF ?
            s.Seek(-40, SeekOrigin.Current);
            s.Read(block, 0, 16);

            Int64 offset64 = BitConverter.ToInt64(block, 8);
            zf._offsetOfCentralDirectory = 0xFFFFFFFF;
            zf._offsetOfCentralDirectory64 = offset64;
            // change for workitem 8098
            s.Seek(offset64, SeekOrigin.Begin);
            //zf.SeekFromOrigin(Offset64);

            uint datum = (uint)ZipSharedUtilities.ReadInt(s);
            if (datum != ZipConstants.Zip64EndOfCentralDirectoryRecordSignature)
                throw new ZipBadReadException(String.Format("  Bad signature (0x{0:X8}) looking for ZIP64 EoCD Record at position 0x{1:X8}", datum, s.Position));

            s.Read(block, 0, 8);
            Int64 Size = BitConverter.ToInt64(block, 0);

            block = new Byte[Size];
            s.Read(block, 0, block.Length);

            offset64 = BitConverter.ToInt64(block, 36);
            // change for workitem 8098
            s.Seek(offset64, SeekOrigin.Begin);
            //zf.SeekFromOrigin(Offset64);
        }

        private static uint ReadFirstFourBytes(Stream s)
        {
            uint datum = (uint)ZipSharedUtilities.ReadInt(s);
            return datum;
        }

        private static void ReadCentralDirectory(ZipFile zf)
        {
            // We must have the central directory footer record, in order to properly
            // read zip dir entries from the central directory.  This because the logic
            // knows when to open a spanned file when the volume number for the central
            // directory differs from the volume number for the zip entry.  The
            // _diskNumberWithCd was set when originally finding the offset for the
            // start of the Central Directory.

            // workitem 9214
            Boolean inputUsesZip64 = false;
            ZipEntry de;
            // in lieu of hashset, use a dictionary
            var previouslySeen = new Dictionary<String, object>();
            while ((de = ZipEntry.ReadDirEntry(zf, previouslySeen)) != null)
            {
                de.ResetDirEntry();
                zf.OnReadEntry(true, null);

                if (zf.Verbose)
                    zf.StatusMessageTextWriter.WriteLine("entry {0}", de.FileName);

                zf._entries.Add(de.FileName, de);

                // workitem 9214
                if (de._InputUsesZip64) inputUsesZip64 = true;
                previouslySeen.Add(de.FileName, null); // to prevent dupes
            }

            // workitem 9214; auto-set the zip64 flag
            if (inputUsesZip64) zf.UseZip64WhenSaving = Zip64Option.Always;

            // workitem 8299
            if (zf._locEndOfCDS > 0)
                zf.ReadStream.Seek(zf._locEndOfCDS, SeekOrigin.Begin);

            ReadCentralDirectoryFooter(zf);

            if (zf.Verbose && !String.IsNullOrEmpty(zf.Comment))
                zf.StatusMessageTextWriter.WriteLine("Zip file Comment: {0}", zf.Comment);

            // We keep the read stream open after reading.

            if (zf.Verbose)
                zf.StatusMessageTextWriter.WriteLine("read in {0} entries.", zf._entries.Count);

            zf.OnReadCompleted();
        }

        private static void ReadIntoInstance_Orig(ZipFile zf)
        {
            zf.OnReadStarted();
            //zf._entries = new System.Collections.Generic.List<ZipEntry>();
            zf._entries = new System.Collections.Generic.Dictionary<String, ZipEntry>();

            ZipEntry e;
            if (zf.Verbose)
            {
                if (zf.Name == null)
                    zf.StatusMessageTextWriter.WriteLine("Reading zip from stream...");
                else
                    zf.StatusMessageTextWriter.WriteLine("Reading zip {0}...", zf.Name);
            }

            // work item 6647:  PK00 (packed to removable disk)
            Boolean firstEntry = true;
            ZipContainer zc = new ZipContainer(zf);
            while ((e = ZipEntry.ReadEntry(zc, firstEntry)) != null)
            {
                if (zf.Verbose)
                    zf.StatusMessageTextWriter.WriteLine("  {0}", e.FileName);

                zf._entries.Add(e.FileName, e);
                firstEntry = false;
            }

            // read the zipfile's central directory structure here.
            // workitem 9912
            // But, because it may be corrupted, ignore errors.
            try
            {
                ZipEntry de;
                // in lieu of hashset, use a dictionary
                var previouslySeen = new Dictionary<String, Object>();
                while ((de = ZipEntry.ReadDirEntry(zf, previouslySeen)) != null)
                {
                    // Housekeeping: Since ZipFile exposes ZipEntry elements in the enumerator,
                    // we need to copy the comment that we grab from the ZipDirEntry
                    // into the ZipEntry, so the application can access the comment.
                    // Also since ZipEntry is used to Write zip files, we need to copy the
                    // file attributes to the ZipEntry as appropriate.
                    ZipEntry e1 = zf._entries[de.FileName];
                    if (e1 != null)
                    {
                        e1._Comment = de.Comment;
                        if (de.IsDirectory) e1.MarkAsDirectory();
                    }
                    previouslySeen.Add(de.FileName, null); // to prevent dupes
                }

                // workitem 8299
                if (zf._locEndOfCDS > 0)
                    zf.ReadStream.Seek(zf._locEndOfCDS, SeekOrigin.Begin);

                ReadCentralDirectoryFooter(zf);

                if (zf.Verbose && !String.IsNullOrEmpty(zf.Comment))
                    zf.StatusMessageTextWriter.WriteLine("Zip file Comment: {0}", zf.Comment);
            }
            catch (ZipException)
            {
            }
            catch (IOException)
            {
            }

            zf.OnReadCompleted();
        }

        private static void ReadCentralDirectoryFooter(ZipFile zf)
        {
            Stream s = zf.ReadStream;
            int signature = ZipSharedUtilities.ReadSignature(s);

            Byte[] block = null;
            int j = 0;
            if (signature == ZipConstants.Zip64EndOfCentralDirectoryRecordSignature)
            {
                // We have a ZIP64 EOCD
                // This data block is 4 Bytes sig, 8 Bytes size, 44 Bytes fixed data,
                // followed by a variable-sized extension block.  We have read the sig already.
                // 8 - datasize (64 bits)
                // 2 - version made by
                // 2 - version needed to extract
                // 4 - number of this disk
                // 4 - number of the disk with the start of the CD
                // 8 - total number of entries in the CD on this disk
                // 8 - total number of entries in the CD
                // 8 - size of the CD
                // 8 - offset of the CD
                // -----------------------
                // 52 Bytes

                block = new Byte[8 + 44];
                s.Read(block, 0, block.Length);

                Int64 DataSize = BitConverter.ToInt64(block, 0); // == 44 + the variable length

                if (DataSize < 44)
                    throw new ZipException("Bad size in the ZIP64 Central Directory.");

                BitConverter.ToUInt16(block, j);
                j += 2;
                BitConverter.ToUInt16(block, j);
                j += 2;
                zf._diskNumberWithCd = BitConverter.ToUInt32(block, j);
                j += 2;

                //zf._diskNumberWithCd++; // hack!!

                // read the extended block
                block = new Byte[DataSize - 44];
                s.Read(block, 0, block.Length);
                // discard the result

                signature = ZipSharedUtilities.ReadSignature(s);
                if (signature != ZipConstants.Zip64EndOfCentralDirectoryLocatorSignature)
                    throw new ZipException("Inconsistent metadata in the ZIP64 Central Directory.");

                block = new Byte[16];
                s.Read(block, 0, block.Length);
                // discard the result

                signature = ZipSharedUtilities.ReadSignature(s);
            }

            // Throw if this is not a signature for "end of central directory record"
            // This is a sanity check.
            if (signature != ZipConstants.EndOfCentralDirectorySignature)
            {
                s.Seek(-4, SeekOrigin.Current);
                throw new ZipBadReadException(String.Format("Bad signature ({0:X8}) at position 0x{1:X8}",
                                                            signature, s.Position));
            }

            // read the End-of-Central-Directory-Record
            block = new Byte[16];
            zf.ReadStream.Read(block, 0, block.Length);

            // off sz  data
            // -------------------------------------------------------
            //  0   4  end of central dir signature (0x06054b50)
            //  4   2  number of this disk
            //  6   2  number of the disk with start of the central directory
            //  8   2  total number of entries in the  central directory on this disk
            // 10   2  total number of entries in  the central directory
            // 12   4  size of the central directory
            // 16   4  offset of start of central directory with respect to the starting disk number
            // 20   2  ZIP file comment length
            // 22  ??  ZIP file comment

            if (zf._diskNumberWithCd == 0)
            {
                zf._diskNumberWithCd = BitConverter.ToUInt16(block, 2);
                //zf._diskNumberWithCd++; // hack!!
            }

            // read the comment here
            ReadZipFileComment(zf);
        }

        private static void ReadZipFileComment(ZipFile zf)
        {
            // read the comment here
            Byte[] block = new Byte[2];
            zf.ReadStream.Read(block, 0, block.Length);

            Int16 commentLength = (short)(block[0] + block[1] * 256);
            if (commentLength > 0)
            {
                block = new Byte[commentLength];
                zf.ReadStream.Read(block, 0, block.Length);

                // workitem 10392 - prefer ProvisionalAlternateEncoding,
                // first.  The fix for workitem 6513 tried to use UTF8
                // only as necessary, but that is impossible to test
                // for, in this direction. There's no way to know what
                // characters the already-encoded Bytes refer
                // to. Therefore, must do what the user tells us.

                String s1 = zf.AlternateEncoding.GetString(block, 0, block.Length);
                zf.Comment = s1;
            }
        }

        public static Boolean IsZipFile(String fileName)
        {
            return IsZipFile(fileName, false);
        }

        public static Boolean IsZipFile(String fileName, Boolean testExtract)
        {
            Boolean result = false;
            try
            {
                if (!File.Exists(fileName)) return false;

                using (var s = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    result = IsZipFile(s, testExtract);
            }
            catch (IOException)
            {
            }
            catch (ZipException)
            {
            }
            return result;
        }

        public static Boolean IsZipFile(Stream stream, Boolean testExtract)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Boolean result = false;
            try
            {
                if (!stream.CanRead) return false;

                var bitBucket = Stream.Null;

                using (ZipFile zip1 = ZipFile.Read(stream, null, null, null))
                {
                    if (testExtract)
                    {
                        foreach (var e in zip1)
                        {
                            if (!e.IsDirectory)
                                e.Extract(bitBucket);
                        }
                    }
                }
                result = true;
            }
            catch (IOException)
            {
            }
            catch (ZipException)
            {
            }
            return result;
        }

        #endregion Чтение

        #region Распаковка

        private void OnExtractEntry(int current, Boolean before, ZipEntry currentEntry, String path)
        {
            EventHandler<ZipExtractProgressEventArgs> ep = ExtractProgress;
            if (ep != null)
            {
                var e = new ZipExtractProgressEventArgs(ArchiveNameForEvent, before, _entries.Count, current, currentEntry, path);
                ep(this, e);
                if (e.Cancel)
                    _extractOperationCanceled = true;
            }
        }

        private void OnExtractAllCompleted(String path)
        {
            EventHandler<ZipExtractProgressEventArgs> ep = ExtractProgress;
            if (ep != null)
            {
                var e = ZipExtractProgressEventArgs.ExtractAllCompleted(ArchiveNameForEvent,
                                                                        path);
                ep(this, e);
            }
        }

        private void OnExtractAllStarted(String path)
        {
            EventHandler<ZipExtractProgressEventArgs> ep = ExtractProgress;
            if (ep != null)
            {
                var e = ZipExtractProgressEventArgs.ExtractAllStarted(ArchiveNameForEvent,
                                                                      path);
                ep(this, e);
            }
        }

        internal Boolean OnExtractBlock(ZipEntry entry, Int64 BytesWritten, Int64 totalBytesToWrite)
        {
            EventHandler<ZipExtractProgressEventArgs> ep = ExtractProgress;
            if (ep != null)
            {
                var e = ZipExtractProgressEventArgs.ByteUpdate(ArchiveNameForEvent, entry,
                                                               BytesWritten, totalBytesToWrite);
                ep(this, e);
                if (e.Cancel)
                    _extractOperationCanceled = true;
            }
            return _extractOperationCanceled;
        }

        internal Boolean OnSingleEntryExtract(ZipEntry entry, String path, Boolean before)
        {
            EventHandler<ZipExtractProgressEventArgs> ep = ExtractProgress;
            if (ep != null)
            {
                var e = (before)
                            ? ZipExtractProgressEventArgs.BeforeExtractEntry(ArchiveNameForEvent, entry, path)
                            : ZipExtractProgressEventArgs.AfterExtractEntry(ArchiveNameForEvent, entry, path);
                ep(this, e);
                if (e.Cancel)
                    _extractOperationCanceled = true;
            }
            return _extractOperationCanceled;
        }

        internal Boolean OnExtractExisting(ZipEntry entry, String path)
        {
            EventHandler<ZipExtractProgressEventArgs> ep = ExtractProgress;
            if (ep != null)
            {
                var e = ZipExtractProgressEventArgs.ExtractExisting(ArchiveNameForEvent, entry, path);
                ep(this, e);
                if (e.Cancel)
                    _extractOperationCanceled = true;
            }
            return _extractOperationCanceled;
        }

        public void ExtractAll(String path)
        {
            _InternalExtractAll(path, true);
        }

        public void ExtractAll(String path, ZipExtractExistingFileAction extractExistingFile)
        {
            ExtractExistingFile = extractExistingFile;
            _InternalExtractAll(path, true);
        }

        private void _InternalExtractAll(String path, Boolean overrideExtractExistingProperty)
        {
            Boolean header = Verbose;
            _inExtractAll = true;
            try
            {
                OnExtractAllStarted(path);

                int n = 0;
                foreach (ZipEntry e in _entries.Values)
                {
                    if (header)
                    {
                        StatusMessageTextWriter.WriteLine("\n{1,-22} {2,-8} {3,4}   {4,-8}  {0}",
                                                          "Name", "Modified", "Size", "Ratio", "Packed");
                        StatusMessageTextWriter.WriteLine(new System.String('-', 72));
                        header = false;
                    }
                    if (Verbose)
                    {
                        StatusMessageTextWriter.WriteLine("{1,-22} {2,-8} {3,4:F0}%   {4,-8} {0}",
                                                          e.FileName,
                                                          e.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
                                                          e.UncompressedSize,
                                                          e.CompressionRatio,
                                                          e.CompressedSize);
                        if (!String.IsNullOrEmpty(e.Comment))
                            StatusMessageTextWriter.WriteLine("  Comment: {0}", e.Comment);
                    }
                    e.Password = _password; // this may be null
                    OnExtractEntry(n, true, e, path);
                    if (overrideExtractExistingProperty)
                        e.ExtractExistingFile = ExtractExistingFile;
                    e.Extract(path);
                    n++;
                    OnExtractEntry(n, false, e, path);
                    if (_extractOperationCanceled)
                        break;
                }

                if (!_extractOperationCanceled)
                {
                    // workitem 8264:
                    // now, set times on directory entries, again.
                    // The problem is, extracting a file changes the times on the parent
                    // directory.  So after all files have been extracted, we have to
                    // run through the directories again.
                    foreach (ZipEntry e in _entries.Values)
                    {
                        // check if it is a directory
                        if ((e.IsDirectory) || (e.FileName.EndsWith("/")))
                        {
                            String outputFile = (e.FileName.StartsWith("/"))
                                                    ? Path.Combine(path, e.FileName.Substring(1))
                                                    : Path.Combine(path, e.FileName);

                            e._SetTimes(outputFile, false);
                        }
                    }
                    OnExtractAllCompleted(path);
                }
            }
            finally
            {
                _inExtractAll = false;
            }
        }

        #endregion Распаковка

        #region Добавление

        private void OnAddStarted()
        {
            EventHandler<ZipAddProgressEventArgs> ap = AddProgress;
            if (ap != null)
            {
                var e = ZipAddProgressEventArgs.Started(ArchiveNameForEvent);
                ap(this, e);
                if (e.Cancel) // workitem 13371
                    _addOperationCanceled = true;
            }
        }

        private void OnAddCompleted()
        {
            EventHandler<ZipAddProgressEventArgs> ap = AddProgress;
            if (ap != null)
            {
                var e = ZipAddProgressEventArgs.Completed(ArchiveNameForEvent);
                ap(this, e);
            }
        }

        internal void AfterAddEntry(ZipEntry entry)
        {
            EventHandler<ZipAddProgressEventArgs> ap = AddProgress;
            if (ap != null)
            {
                var e = ZipAddProgressEventArgs.AfterEntry(ArchiveNameForEvent, entry, _entries.Count);
                ap(this, e);
                if (e.Cancel) // workitem 13371
                    _addOperationCanceled = true;
            }
        }

        public ZipEntry AddItem(String fileOrDirectoryName)
        {
            return AddItem(fileOrDirectoryName, null);
        }

        public ZipEntry AddItem(String fileOrDirectoryName, String directoryPathInArchive)
        {
            if (File.Exists(fileOrDirectoryName))
                return AddFile(fileOrDirectoryName, directoryPathInArchive);

            if (Directory.Exists(fileOrDirectoryName))
                return AddDirectory(fileOrDirectoryName, directoryPathInArchive);

            throw new FileNotFoundException(String.Format("That file or directory ({0}) does not exist!",
                                                          fileOrDirectoryName));
        }

        public ZipEntry AddFile(String fileName)
        {
            return AddFile(fileName, "");
        }

        public ZipEntry AddFile(String fileName, String directoryPathInArchive)
        {
            String nameInArchive = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
            ZipEntry ze = ZipEntry.CreateFromFile(fileName, nameInArchive);
            if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", fileName);
            return _InternalAddEntry(ze);
        }

        public void RemoveEntries(ICollection<ZipEntry> entriesToRemove)
        {
            if (entriesToRemove == null)
                throw new ArgumentNullException("entriesToRemove");

            foreach (ZipEntry e in entriesToRemove)
                RemoveEntry(e);
        }

        public void RemoveEntries(ICollection<String> entriesToRemove)
        {
            if (entriesToRemove == null)
                throw new ArgumentNullException("entriesToRemove");

            foreach (String e in entriesToRemove)
                RemoveEntry(e);
        }

        public void AddFiles(System.Collections.Generic.IEnumerable<String> fileNames)
        {
            AddFiles(fileNames, null);
        }

        public void UpdateFiles(System.Collections.Generic.IEnumerable<String> fileNames)
        {
            UpdateFiles(fileNames, null);
        }

        public void AddFiles(System.Collections.Generic.IEnumerable<String> fileNames, String directoryPathInArchive)
        {
            AddFiles(fileNames, false, directoryPathInArchive);
        }

        public void AddFiles(System.Collections.Generic.IEnumerable<String> fileNames, Boolean preserveDirHierarchy, String directoryPathInArchive)
        {
            if (fileNames == null)
                throw new ArgumentNullException("fileNames");

            _addOperationCanceled = false;
            OnAddStarted();
            if (preserveDirHierarchy)
            {
                foreach (var f in fileNames)
                {
                    if (_addOperationCanceled) break;
                    if (directoryPathInArchive != null)
                    {
                        //String s = SharedUtilities.NormalizePath(Path.Combine(directoryPathInArchive, Path.GetDirectoryName(f)));
                        String s = Path.GetFullPath(Path.Combine(directoryPathInArchive, Path.GetDirectoryName(f)));
                        AddFile(f, s);
                    }
                    else
                        AddFile(f, null);
                }
            }
            else
            {
                foreach (var f in fileNames)
                {
                    if (_addOperationCanceled) break;
                    AddFile(f, directoryPathInArchive);
                }
            }
            if (!_addOperationCanceled)
                OnAddCompleted();
        }

        public void UpdateFiles(System.Collections.Generic.IEnumerable<String> fileNames, String directoryPathInArchive)
        {
            if (fileNames == null)
                throw new ArgumentNullException("fileNames");

            OnAddStarted();
            foreach (var f in fileNames)
                UpdateFile(f, directoryPathInArchive);
            OnAddCompleted();
        }

        public ZipEntry UpdateFile(String fileName)
        {
            return UpdateFile(fileName, null);
        }

        public ZipEntry UpdateFile(String fileName, String directoryPathInArchive)
        {
            // ideally this would all be transactional!
            var key = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
            if (this[key] != null)
                RemoveEntry(key);
            return AddFile(fileName, directoryPathInArchive);
        }

        public ZipEntry UpdateDirectory(String directoryName)
        {
            return UpdateDirectory(directoryName, null);
        }

        public ZipEntry UpdateDirectory(String directoryName, String directoryPathInArchive)
        {
            return AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, ZipAddOrUpdateAction.AddOrUpdate);
        }

        public void UpdateItem(String itemName)
        {
            UpdateItem(itemName, null);
        }

        public void UpdateItem(String itemName, String directoryPathInArchive)
        {
            if (File.Exists(itemName))
                UpdateFile(itemName, directoryPathInArchive);

            else if (Directory.Exists(itemName))
                UpdateDirectory(itemName, directoryPathInArchive);

            else
                throw new FileNotFoundException(String.Format("That file or directory ({0}) does not exist!", itemName));
        }

        public ZipEntry AddEntry(String entryName, String content)
        {
            return AddEntry(entryName, content, System.Text.Encoding.Default);
        }

        public ZipEntry AddEntry(String entryName, String content, System.Text.Encoding encoding)
        {
            // cannot employ a using clause here.  We need the stream to
            // persist after exit from this method.
            var ms = new MemoryStream();

            // cannot use a using clause here; StreamWriter takes
            // ownership of the stream and Disposes it before we are ready.
            var sw = new StreamWriter(ms, encoding);
            sw.Write(content);
            sw.Flush();

            // reset to allow reading later
            ms.Seek(0, SeekOrigin.Begin);

            return AddEntry(entryName, ms);

            // must not dispose the MemoryStream - it will be used later.
        }

        public ZipEntry AddEntry(String entryName, Stream stream)
        {
            ZipEntry ze = ZipEntry.CreateForStream(entryName, stream);
            ze.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
            if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
            return _InternalAddEntry(ze);
        }

        public ZipEntry AddEntry(String entryName, ZipWriteDelegate writer)
        {
            ZipEntry ze = ZipEntry.CreateForWriter(entryName, writer);
            if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
            return _InternalAddEntry(ze);
        }

        public ZipEntry AddEntry(String entryName, ZipOpenDelegate opener, ZipCloseDelegate closer)
        {
            ZipEntry ze = ZipEntry.CreateForJitStreamProvider(entryName, opener, closer);
            ze.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
            if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
            return _InternalAddEntry(ze);
        }

        private ZipEntry _InternalAddEntry(ZipEntry ze)
        {
            // stamp all the props onto the entry
            ze._container = new ZipContainer(this);
            ze.CompressionMethod = CompressionMethod;
            ze.CompressionLevel = CompressionLevel;
            ze.ExtractExistingFile = ExtractExistingFile;
            ze.ZipErrorAction = ZipErrorAction;
            ze.SetCompression = SetCompression;
            ze.AlternateEncoding = AlternateEncoding;
            ze.AlternateEncodingUsage = AlternateEncodingUsage;
            ze.Password = _password;
            ze.Encryption = Encryption;
            ze.EmitTimesInWindowsFormatWhenSaving = _emitNtfsTimes;
            ze.EmitTimesInUnixFormatWhenSaving = _emitUnixTimes;
            //String key = DictionaryKeyForEntry(ze);
            InternalAddEntry(ze.FileName, ze);
            AfterAddEntry(ze);

            return ze;
        }

        public ZipEntry UpdateEntry(String entryName, String content)
        {
            return UpdateEntry(entryName, content, System.Text.Encoding.Default);
        }

        public ZipEntry UpdateEntry(String entryName, String content, System.Text.Encoding encoding)
        {
            RemoveEntryForUpdate(entryName);
            return AddEntry(entryName, content, encoding);
        }

        public ZipEntry UpdateEntry(String entryName, ZipWriteDelegate writer)
        {
            RemoveEntryForUpdate(entryName);
            return AddEntry(entryName, writer);
        }

        public ZipEntry UpdateEntry(String entryName, ZipOpenDelegate opener, ZipCloseDelegate closer)
        {
            RemoveEntryForUpdate(entryName);
            return AddEntry(entryName, opener, closer);
        }

        public ZipEntry UpdateEntry(String entryName, Stream stream)
        {
            RemoveEntryForUpdate(entryName);
            return AddEntry(entryName, stream);
        }

        private void RemoveEntryForUpdate(String entryName)
        {
            if (String.IsNullOrEmpty(entryName))
                throw new ArgumentNullException("entryName");

            String directoryPathInArchive = null;
            if (entryName.IndexOf('\\') != -1)
            {
                directoryPathInArchive = Path.GetDirectoryName(entryName);
                entryName = Path.GetFileName(entryName);
            }
            var key = ZipEntry.NameInArchive(entryName, directoryPathInArchive);
            if (this[key] != null)
                RemoveEntry(key);
        }

        public ZipEntry AddEntry(String entryName, Byte[] ByteContent)
        {
            if (ByteContent == null) throw new ArgumentException("bad argument", "ByteContent");
            var ms = new MemoryStream(ByteContent);
            return AddEntry(entryName, ms);
        }

        public ZipEntry UpdateEntry(String entryName, Byte[] ByteContent)
        {
            RemoveEntryForUpdate(entryName);
            return AddEntry(entryName, ByteContent);
        }

        public ZipEntry AddDirectory(String directoryName)
        {
            return AddDirectory(directoryName, null);
        }

        public ZipEntry AddDirectory(String directoryName, String directoryPathInArchive)
        {
            return AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, ZipAddOrUpdateAction.AddOnly);
        }

        public ZipEntry AddDirectoryByName(String directoryNameInArchive)
        {
            // workitem 9073
            ZipEntry dir = ZipEntry.CreateFromNothing(directoryNameInArchive);
            dir._container = new ZipContainer(this);
            dir.MarkAsDirectory();
            dir.AlternateEncoding = AlternateEncoding; // workitem 8984
            dir.AlternateEncodingUsage = AlternateEncodingUsage;
            dir.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
            dir.EmitTimesInWindowsFormatWhenSaving = _emitNtfsTimes;
            dir.EmitTimesInUnixFormatWhenSaving = _emitUnixTimes;
            dir._source = ZipEntrySource.Stream;
            //String key = DictionaryKeyForEntry(dir);
            InternalAddEntry(dir.FileName, dir);
            AfterAddEntry(dir);
            return dir;
        }

        private ZipEntry AddOrUpdateDirectoryImpl(String directoryName, String rootDirectoryPathInArchive, ZipAddOrUpdateAction action)
        {
            if (rootDirectoryPathInArchive == null)
                rootDirectoryPathInArchive = "";

            return AddOrUpdateDirectoryImpl(directoryName, rootDirectoryPathInArchive, action, true, 0);
        }

        internal void InternalAddEntry(String name, ZipEntry entry)
        {
            _entries.Add(name, entry);
            _zipEntriesAsList = null;
            _contentsChanged = true;
        }

        private Boolean CheckFilter(String filename)
        {
            if (Filters == null || Filters.Count == 0) return false;

            filename = filename.ToLower();
            //filename = Files.GetFileNameAndExt(filename);

            foreach (String filterHigh in Filters)
            {
                if (filterHigh.IsValid())
                {
                    String filter = filterHigh.ToLower();

                    // Проверка совпадения имени
                    if (filename == filter) return true;
                }
            }

            return false;
        }

        private ZipEntry AddOrUpdateDirectoryImpl(String directoryName, String rootDirectoryPathInArchive, ZipAddOrUpdateAction action, Boolean recurse, int level)
        {
            if (Verbose)
            {
                StatusMessageTextWriter.WriteLine("{0} {1}...",
                                                  (action == ZipAddOrUpdateAction.AddOnly) ? "adding" : "Adding or updating", directoryName);
            }

            if (level == 0)
            {
                _addOperationCanceled = false;
                OnAddStarted();
            }

            // workitem 13371
            if (_addOperationCanceled)
                return null;

            String dirForEntries = rootDirectoryPathInArchive;
            ZipEntry baseDir = null;

            if (level > 0)
            {
                int f = directoryName.Length;
                for (int i = level; i > 0; i--)
                    f = directoryName.LastIndexOfAny("/\\".ToCharArray(), f - 1, f - 1);

                dirForEntries = directoryName.Substring(f + 1);
                dirForEntries = Path.Combine(rootDirectoryPathInArchive, dirForEntries);
            }

            // if not top level, or if the root is non-empty, then explicitly add the directory
            if (level > 0 || rootDirectoryPathInArchive != "")
            {
                baseDir = ZipEntry.CreateFromFile(directoryName, dirForEntries);
                baseDir._container = new ZipContainer(this);
                baseDir.AlternateEncoding = AlternateEncoding; // workitem 6410
                baseDir.AlternateEncodingUsage = AlternateEncodingUsage;
                baseDir.MarkAsDirectory();
                baseDir.EmitTimesInWindowsFormatWhenSaving = _emitNtfsTimes;
                baseDir.EmitTimesInUnixFormatWhenSaving = _emitUnixTimes;

                // add the directory only if it does not exist.
                // It's not an error if it already exists.
                if (!_entries.ContainsKey(baseDir.FileName))
                {
                    InternalAddEntry(baseDir.FileName, baseDir);
                    AfterAddEntry(baseDir);
                }
                dirForEntries = baseDir.FileName;
            }

            if (_addOperationCanceled == false)
            {
                String[] filenames = Directory.GetFiles(directoryName);

                if (recurse)
                {
                    // add the files:
                    foreach (String filename in filenames)
                    {
                        if (_addOperationCanceled) break;
                        if (CheckFilter(filename) == false)
                        {
                            if (action == ZipAddOrUpdateAction.AddOnly)
                                AddFile(filename, dirForEntries);
                            else
                                UpdateFile(filename, dirForEntries);
                        }
                    }

                    if (_addOperationCanceled == false)
                    {
                        // add the subdirectories:
                        String[] dirnames = Directory.GetDirectories(directoryName);
                        foreach (String dir in dirnames)
                        {
                            // workitem 8617: Optionally traverse reparse points
                            FileAttributes fileAttrs = System.IO.File.GetAttributes(dir);
                            if (AddDirectoryWillTraverseReparsePoints || ((fileAttrs & FileAttributes.ReparsePoint) == 0))
                            {
                                if (CheckFilter(dir) == false)
                                    AddOrUpdateDirectoryImpl(dir, rootDirectoryPathInArchive, action, recurse, level + 1);
                            }
                        }
                    }
                }
            }

            if (level == 0)
                OnAddCompleted();

            return baseDir;
        }

        #endregion Добавление

        #region Ошибки

        internal Boolean OnZipErrorSaving(ZipEntry entry, Exception exc)
        {
            if (ZipError != null)
            {
                lock (_lock)
                {
                    var e = ZipErrorEventArgs.Saving(Name, entry, exc);
                    ZipError(this, e);
                    if (e.Cancel)
                        _saveOperationCanceled = true;
                }
            }
            return _saveOperationCanceled;
        }

        #endregion Ошибки

        #endregion Методы
    }

    #endregion Класс ZipFile

    #region Класс ZipEntry

    [Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d00004")]
    [Interop.ComVisible(true)]
    [Interop.ClassInterface(Interop.ClassInterfaceType.AutoDispatch)] // AutoDual
    public class ZipEntry
    {
        #region Делегаты

        private delegate T Func<T>();

        #endregion

        #region Поля

        private static System.DateTime _unixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static System.DateTime _win32Epoch = System.DateTime.FromFileTimeUtc(0L);

        private static System.DateTime _zeroHour = new System.DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static System.Text.Encoding ibm437 = System.Text.Encoding.GetEncoding("IBM437");

        internal Int16 _BitField;

        private ZipCloseDelegate _CloseDelegate;

        internal String _Comment;

        private Byte[] _CommentBytes;

        internal Int64 _CompressedFileDataSize;

        internal Int64 _CompressedSize;

        private ZipCompressionLevel _CompressionLevel;

        internal Int16 _CompressionMethod;

        private Int16 _CompressionMethod_FromZipFile;

        internal Int32 _Crc32;

        internal ZipEncryptionAlgorithm _Encryption;

        internal ZipEncryptionAlgorithm _Encryption_FromZipFile;

        private Byte[] _EntryHeader;

        private Int32 _ExternalFileAttrs;

        internal Byte[] _Extra;

        private String _FileNameInArchive;

        internal Boolean _InputUsesZip64;

        private Int16 _InternalFileAttrs;

        private Boolean _IsDirectory;

        private Boolean _IsText;

        internal DateTime _LastModified;

        private int _LengthOfHeader;

        private int _LengthOfTrailer;

        internal String _LocalFileName;

        private ZipOpenDelegate _OpenDelegate;

        private bool? _OutputUsesZip64;

        internal String _Password;

        internal Int64 _RelativeOffsetOfLocalHeader;

        internal Int32 _TimeBlob;

        private Int64 _TotalEntrySize;

        private Boolean _TrimVolumeFromFullyQualifiedPaths = true;

        internal Int64 _UncompressedSize;

        private UInt32 _UnsupportedAlgorithmId;

        internal Int16 _VersionNeeded;

        internal Byte[] _WeakEncryptionHeader;

        private ZipWriteDelegate _WriteDelegate;

        private long __FileDataPosition = -1;

        private System.Text.Encoding _actualEncoding;

        private DateTime _actualTime;

        internal Stream _archiveStream;

        private Int16 _commentLength;

        internal ZipContainer _container;

        private Boolean _crcCalculated;

        private DateTime _createTime;

        private UInt32 _diskNumber;

        private Boolean _emitNtfsTimes = true;

        private Boolean _emitUnixTimes;

        private bool? _entryRequiresZip64;

        private Int16 _extraFieldLength;

        private Int16 _filenameLength;

        private Int64 _future_ROLH;

        private Stream _inputDecryptorStream;

        private Boolean _ioOperationCanceled;

        private DateTime _makeTime;

        private Boolean _metadataChanged;

        private Boolean _ntfsTimesAreSet;

        private Object _outputLock = new Object();

        private Boolean _presumeZip64;

        private int _readExtraDepth;

        private Boolean _restreamRequiredOnSave;

        private Boolean _skippedDuringSave;

        internal ZipEntrySource _source;

        private Boolean _sourceIsEncrypted;

        private Stream _sourceStream;

        private long? _sourceStreamOriginalPosition;

        private Boolean _sourceWasJitProvided;

        private ZipEntryTimestamp _timestamp;

        private Int16 _versionMadeBy;

        private ZipCrypto _zipCrypto_forExtract;

        private ZipCrypto _zipCrypto_forWrite;

        #endregion

        #region Свойства

        private int BufferSize
        {
            get { return _container.BufferSize; }
        }

        private int LengthOfHeader
        {
            get
            {
                if (_LengthOfHeader == 0)
                    SetFdpLoh();

                return _LengthOfHeader;
            }
        }

        internal String LocalFileName
        {
            get { return _LocalFileName; }
        }

        internal Boolean IsChanged
        {
            get { return _restreamRequiredOnSave | _metadataChanged; }
        }

        internal Stream ArchiveStream
        {
            get
            {
                if (_archiveStream == null)
                {
                    if (_container.ZipFile != null)
                    {
                        var zf = _container.ZipFile;
                        zf.Reset(false);
                        _archiveStream = zf.StreamForDiskNumber(_diskNumber);
                    }
                    else
                        _archiveStream = _container.ZipOutputStream.OutputStream;
                }
                return _archiveStream;
            }
        }

        internal long FileDataPosition
        {
            get
            {
                if (__FileDataPosition == -1)
                    SetFdpLoh();

                return __FileDataPosition;
            }
        }

        public DateTime LastModified
        {
            get { return _LastModified.ToLocalTime(); }
            set
            {
                _LastModified = (value.Kind == DateTimeKind.Unspecified)
                                    ? DateTime.SpecifyKind(value, DateTimeKind.Local)
                                    : value.ToLocalTime();
                _makeTime = ZipSharedUtilities.AdjustTime_Reverse(_LastModified).ToUniversalTime();
                _metadataChanged = true;
            }
        }

        public DateTime ModifiedTime
        {
            get { return _makeTime; }
            set { SetEntryTimes(_createTime, _actualTime, value); }
        }

        public DateTime AccessedTime
        {
            get { return _actualTime; }
            set { SetEntryTimes(_createTime, value, _makeTime); }
        }

        public DateTime CreationTime
        {
            get { return _createTime; }
            set { SetEntryTimes(value, _actualTime, _makeTime); }
        }

        public Boolean EmitTimesInWindowsFormatWhenSaving
        {
            get { return _emitNtfsTimes; }
            set
            {
                _emitNtfsTimes = value;
                _metadataChanged = true;
            }
        }

        public Boolean EmitTimesInUnixFormatWhenSaving
        {
            get { return _emitUnixTimes; }
            set
            {
                _emitUnixTimes = value;
                _metadataChanged = true;
            }
        }

        public ZipEntryTimestamp Timestamp
        {
            get { return _timestamp; }
        }

        public System.IO.FileAttributes Attributes
        {
            get { return (System.IO.FileAttributes)_ExternalFileAttrs; }
            set
            {
                _ExternalFileAttrs = (int)value;
                // Since the application is explicitly setting the attributes, overwriting
                // whatever was there, we will explicitly set the Version made by field.
                // workitem 7926 - "version made by" OS should be zero for compat with WinZip
                _versionMadeBy = (0 << 8) + 45; // v4.5 of the spec
                _metadataChanged = true;
            }
        }

        public String FileName
        {
            get { return _FileNameInArchive; }
            set
            {
                if (_container.ZipFile == null)
                    throw new ZipException("Cannot rename; this is not supported in ZipOutputStream/ZipInputStream.");

                // rename the entry!
                if (String.IsNullOrEmpty(value)) throw new ZipException("The FileName must be non empty and non-null.");

                var filename = ZipEntry.NameInArchive(value, null);
                // workitem 8180
                if (_FileNameInArchive == filename) return; // nothing to do

                // workitem 8047 - when renaming, must remove old and then add a new entry
                _container.ZipFile.RemoveEntry(this);
                _container.ZipFile.InternalAddEntry(filename, this);

                _FileNameInArchive = filename;
                _container.ZipFile.NotifyEntryChanged();
                _metadataChanged = true;
            }
        }

        public Stream InputStream
        {
            get { return _sourceStream; }
            set
            {
                if (_source != ZipEntrySource.Stream)
                    throw new ZipException("You must not set the input stream for this entry.");

                _sourceWasJitProvided = true;
                _sourceStream = value;
            }
        }

        public Boolean InputStreamWasJitProvided
        {
            get { return _sourceWasJitProvided; }
        }

        public ZipEntrySource Source
        {
            get { return _source; }
        }

        public Int16 VersionNeeded
        {
            get { return _VersionNeeded; }
        }

        public String Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                _metadataChanged = true;
            }
        }

        public bool? RequiresZip64
        {
            get { return _entryRequiresZip64; }
        }

        public bool? OutputUsedZip64
        {
            get { return _OutputUsesZip64; }
        }

        public Int16 BitField
        {
            get { return _BitField; }
        }

        public ZipCompressionMethod CompressionMethod
        {
            get { return (ZipCompressionMethod)_CompressionMethod; }
            set
            {
                if (value == (ZipCompressionMethod)_CompressionMethod) return; // nothing to do.

                if (value != ZipCompressionMethod.None && value != ZipCompressionMethod.Deflate)
                    throw new InvalidOperationException("Unsupported compression method.");

                // If the source is a zip archive and there was encryption on the
                // entry, changing the compression method is not supported.
                //                 if (this._Source == ZipEntrySource.ZipFile && _sourceIsEncrypted)
                //                     throw new InvalidOperationException("Cannot change compression method on encrypted entries read from archives.");

                _CompressionMethod = (Int16)value;

                if (_CompressionMethod == (Int16)ZipCompressionMethod.None)
                    _CompressionLevel = ZipCompressionLevel.None;
                else if (CompressionLevel == ZipCompressionLevel.None)
                    _CompressionLevel = ZipCompressionLevel.Default;

                if (_container.ZipFile != null) _container.ZipFile.NotifyEntryChanged();
                _restreamRequiredOnSave = true;
            }
        }

        public ZipCompressionLevel CompressionLevel
        {
            get { return _CompressionLevel; }
            set
            {
                if (_CompressionMethod != (short)ZipCompressionMethod.Deflate &&
                    _CompressionMethod != (short)ZipCompressionMethod.None)
                    return; // no effect

                if (value == ZipCompressionLevel.Default &&
                    _CompressionMethod == (short)ZipCompressionMethod.Deflate) return; // nothing to do
                _CompressionLevel = value;

                if (value == ZipCompressionLevel.None &&
                    _CompressionMethod == (short)ZipCompressionMethod.None)
                    return; // nothing more to do

                if (_CompressionLevel == ZipCompressionLevel.None)
                    _CompressionMethod = (short)ZipCompressionMethod.None;
                else
                    _CompressionMethod = (short)ZipCompressionMethod.Deflate;

                if (_container.ZipFile != null) _container.ZipFile.NotifyEntryChanged();
                _restreamRequiredOnSave = true;
            }
        }

        public Int64 CompressedSize
        {
            get { return _CompressedSize; }
        }

        public Int64 UncompressedSize
        {
            get { return _UncompressedSize; }
        }

        public Double CompressionRatio
        {
            get
            {
                if (UncompressedSize == 0) return 0;
                return 100 * (1.0 - (1.0 * CompressedSize) / (1.0 * UncompressedSize));
            }
        }

        public Int32 Crc
        {
            get { return _Crc32; }
        }

        public Boolean IsDirectory
        {
            get { return _IsDirectory; }
        }

        public Boolean UsesEncryption
        {
            get { return (_Encryption_FromZipFile != ZipEncryptionAlgorithm.None); }
        }

        public ZipEncryptionAlgorithm Encryption
        {
            get { return _Encryption; }
            set
            {
                if (value == _Encryption) return; // no change

                if (value == ZipEncryptionAlgorithm.Unsupported)
                    throw new InvalidOperationException("You may not set Encryption to that value.");

                // If the source is a zip archive and there was encryption
                // on the entry, this will not work. <XXX>
                //if (this._Source == ZipEntrySource.ZipFile && _sourceIsEncrypted)
                //    throw new InvalidOperationException("You cannot change the encryption method on encrypted entries read from archives.");

                _Encryption = value;
                _restreamRequiredOnSave = true;
                if (_container.ZipFile != null)
                    _container.ZipFile.NotifyEntryChanged();
            }
        }

        public String Password
        {
            set
            {
                _Password = value;
                if (_Password == null)
                    _Encryption = ZipEncryptionAlgorithm.None;
                else
                {
                    // We're setting a non-null password.

                    // For entries obtained from a zip file that are encrypted, we cannot
                    // simply restream (recompress, re-encrypt) the file data, because we
                    // need the old password in order to decrypt the data, and then we
                    // need the new password to encrypt.  So, setting the password is
                    // never going to work on an entry that is stored encrypted in a zipfile.

                    // But it is not en error to set the password, obviously: callers will
                    // set the password in order to Extract encrypted archives.

                    // If the source is a zip archive and there was previously no encryption
                    // on the entry, then we must re-stream the entry in order to encrypt it.
                    if (_source == ZipEntrySource.ZipFile && !_sourceIsEncrypted)
                        _restreamRequiredOnSave = true;

                    if (Encryption == ZipEncryptionAlgorithm.None)
                        _Encryption = ZipEncryptionAlgorithm.PkzipWeak;
                }
            }
        }

        public ZipExtractExistingFileAction ExtractExistingFile { get; set; }

        public ZipErrorAction ZipErrorAction { get; set; }

        public Boolean IncludedInMostRecentSave
        {
            get { return !_skippedDuringSave; }
        }

        public ZipSetCompressionCallback SetCompression { get; set; }

        public System.Text.Encoding AlternateEncoding { get; set; }

        public ZipOption AlternateEncodingUsage { get; set; }

        public Boolean IsText
        {
            get { return _IsText; }
            set { _IsText = value; }
        }

        public String Info
        {
            get
            {
                var builder = new System.Text.StringBuilder();
                builder
                    .Append(String.Format("          ZipEntry: {0}\n", FileName))
                    .Append(String.Format("   Version Made By: {0}\n", _versionMadeBy))
                    .Append(String.Format(" Needed to extract: {0}\n", VersionNeeded));

                if (_IsDirectory)
                    builder.Append("        Entry type: directory\n");
                else
                {
                    builder.Append(String.Format("         File type: {0}\n", _IsText ? "text" : "binary"))
                           .Append(String.Format("       Compression: {0}\n", CompressionMethod))
                           .Append(String.Format("        Compressed: 0x{0:X}\n", CompressedSize))
                           .Append(String.Format("      Uncompressed: 0x{0:X}\n", UncompressedSize))
                           .Append(String.Format("             CRC32: 0x{0:X8}\n", _Crc32));
                }
                builder.Append(String.Format("       Disk Number: {0}\n", _diskNumber));
                if (_RelativeOffsetOfLocalHeader > 0xFFFFFFFF)
                {
                    builder
                        .Append(String.Format("   Relative Offset: 0x{0:X16}\n", _RelativeOffsetOfLocalHeader));
                }
                else
                {
                    builder
                        .Append(String.Format("   Relative Offset: 0x{0:X8}\n", _RelativeOffsetOfLocalHeader));
                }

                builder
                    .Append(String.Format("         Bit Field: 0x{0:X4}\n", _BitField))
                    .Append(String.Format("        Encrypted?: {0}\n", _sourceIsEncrypted))
                    .Append(String.Format("          Timeblob: 0x{0:X8}\n", _TimeBlob))
                    .Append(String.Format("              Time: {0}\n", ZipSharedUtilities.PackedToDateTime(_TimeBlob)));

                builder.Append(String.Format("         Is Zip64?: {0}\n", _InputUsesZip64));
                if (!String.IsNullOrEmpty(_Comment))
                    builder.Append(String.Format("           Comment: {0}\n", _Comment));
                builder.Append("\n");
                return builder.ToString();
            }
        }

        private String UnsupportedAlgorithm
        {
            get
            {
                String alg = String.Empty;

                switch (_UnsupportedAlgorithmId)
                {
                    case 0:
                        alg = "--";
                        break;
                    case 0x6601:
                        alg = "DES";
                        break;
                    case 0x6602:
                        alg = "RC2";
                        break;
                    case 0x6603:
                        alg = "3DES-168";
                        break;
                    case 0x6609:
                        alg = "3DES-112";
                        break;
                    case 0x660E:
                        alg = "PKWare AES128";
                        break;
                    case 0x660F:
                        alg = "PKWare AES192";
                        break;
                    case 0x6610:
                        alg = "PKWare AES256";
                        break;
                    case 0x6702:
                        alg = "RC2";
                        break;
                    case 0x6720:
                        alg = "Blowfish";
                        break;
                    case 0x6721:
                        alg = "Twofish";
                        break;
                    case 0x6801:
                        alg = "RC4";
                        break;
                    case 0xFFFF:
                    default:
                        alg = String.Format("Unknown (0x{0:X4})", _UnsupportedAlgorithmId);
                        break;
                }
                return alg;
            }
        }

        private String UnsupportedCompressionMethod
        {
            get
            {
                String meth = String.Empty;

                switch ((int)_CompressionMethod)
                {
                    case 0:
                        meth = "Store";
                        break;
                    case 1:
                        meth = "Shrink";
                        break;
                    case 8:
                        meth = "DEFLATE";
                        break;
                    case 9:
                        meth = "Deflate64";
                        break;
                    case 12:
                        meth = "BZIP2";
                        break;
                    case 14:
                        meth = "LZMA";
                        break;
                    case 19:
                        meth = "LZ77";
                        break;
                    case 98:
                        meth = "PPMd";
                        break;
                    default:
                        meth = String.Format("Unknown (0x{0:X4})", _CompressionMethod);
                        break;
                }
                return meth;
            }
        }

        internal Boolean AttributesIndicateDirectory
        {
            get { return ((_InternalFileAttrs == 0) && ((_ExternalFileAttrs & 0x0010) == 0x0010)); }
        }

        #endregion

        #region Конструктор

        public ZipEntry()
        {
            _CompressionMethod = (Int16)ZipCompressionMethod.Deflate;
            _CompressionLevel = ZipCompressionLevel.Default;
            _Encryption = ZipEncryptionAlgorithm.None;
            _source = ZipEntrySource.None;
            AlternateEncoding = System.Text.Encoding.GetEncoding("IBM437");
            AlternateEncodingUsage = ZipOption.Never;
        }

        #endregion

        #region Методы

        public void SetEntryTimes(DateTime created, DateTime accessed, DateTime modified)
        {
            _ntfsTimesAreSet = true;
            if (created == _zeroHour && created.Kind == _zeroHour.Kind) created = _win32Epoch;
            if (accessed == _zeroHour && accessed.Kind == _zeroHour.Kind) accessed = _win32Epoch;
            if (modified == _zeroHour && modified.Kind == _zeroHour.Kind) modified = _win32Epoch;
            _createTime = created.ToUniversalTime();
            _actualTime = accessed.ToUniversalTime();
            _makeTime = modified.ToUniversalTime();
            _LastModified = _makeTime;
            if (!_emitUnixTimes && !_emitNtfsTimes)
                _emitNtfsTimes = true;
            _metadataChanged = true;
        }

        internal static String NameInArchive(String filename, String directoryPathInArchive)
        {
            String result = null;

            if (directoryPathInArchive == null)
                result = filename;
            else
            {
                if (String.IsNullOrEmpty(directoryPathInArchive))
                    result = Path.GetFileName(filename);
                else
                {
                    // explicitly specify a pathname for this file
                    result = Path.Combine(directoryPathInArchive, Path.GetFileName(filename));
                }
            }

            result = ZipSharedUtilities.NormalizePathForUseInZipFile(result);

            return result;
        }

        internal static ZipEntry CreateFromNothing(String nameInArchive)
        {
            return Create(nameInArchive, ZipEntrySource.None, null, null);
        }

        internal static ZipEntry CreateFromFile(String filename, String nameInArchive)
        {
            return Create(nameInArchive, ZipEntrySource.FileSystem, filename, null);
        }

        internal static ZipEntry CreateForStream(String entryName, Stream s)
        {
            return Create(entryName, ZipEntrySource.Stream, s, null);
        }

        internal static ZipEntry CreateForWriter(String entryName, ZipWriteDelegate d)
        {
            return Create(entryName, ZipEntrySource.WriteDelegate, d, null);
        }

        internal static ZipEntry CreateForJitStreamProvider(String nameInArchive, ZipOpenDelegate opener, ZipCloseDelegate closer)
        {
            return Create(nameInArchive, ZipEntrySource.JitStream, opener, closer);
        }

        internal static ZipEntry CreateForZipOutputStream(String nameInArchive)
        {
            return Create(nameInArchive, ZipEntrySource.ZipOutputStream, null, null);
        }

        private static ZipEntry Create(String nameInArchive, ZipEntrySource source, Object arg1, Object arg2)
        {
            if (String.IsNullOrEmpty(nameInArchive))
                throw new ZipException("The entry name must be non-null and non-empty.");

            ZipEntry entry = new ZipEntry();

            // workitem 7071
            // workitem 7926 - "version made by" OS should be zero for compat with WinZip
            entry._versionMadeBy = (0 << 8) + 45; // indicates the attributes are FAT Attributes, and v4.5 of the spec
            entry._source = source;
            entry._makeTime = entry._actualTime = entry._createTime = DateTime.UtcNow;

            if (source == ZipEntrySource.Stream)
                entry._sourceStream = (arg1 as Stream); // may  or may not be null
            else if (source == ZipEntrySource.WriteDelegate)
                entry._WriteDelegate = (arg1 as ZipWriteDelegate); // may  or may not be null
            else if (source == ZipEntrySource.JitStream)
            {
                entry._OpenDelegate = (arg1 as ZipOpenDelegate); // may  or may not be null
                entry._CloseDelegate = (arg2 as ZipCloseDelegate); // may  or may not be null
            }
            else if (source == ZipEntrySource.ZipOutputStream)
            {
            }
                // workitem 9073
            else if (source == ZipEntrySource.None)
            {
                // make this a valid value, for later.
                entry._source = ZipEntrySource.FileSystem;
            }
            else
            {
                String filename = (arg1 as String); // must not be null

                if (String.IsNullOrEmpty(filename))
                    throw new ZipException("The filename must be non-null and non-empty.");
                try
                {
                    // The named file may or may not exist at this time.  For
                    // example, when adding a directory by name.  We test existence
                    // when necessary: when saving the ZipFile, or when getting the
                    // attributes, and so on.

                    // workitem 6878??
                    entry._makeTime = File.GetLastWriteTime(filename).ToUniversalTime();
                    entry._createTime = File.GetCreationTime(filename).ToUniversalTime();
                    entry._actualTime = File.GetLastAccessTime(filename).ToUniversalTime();

                    // workitem 7071
                    // can only get attributes on files that exist.
                    if (File.Exists(filename) || Directory.Exists(filename))
                        entry._ExternalFileAttrs = (int)File.GetAttributes(filename);

                    entry._ntfsTimesAreSet = true;
                    entry._LocalFileName = Path.GetFullPath(filename); // workitem 8813
                }
                catch (System.IO.PathTooLongException ptle)
                {
                    // workitem 14035
                    var msg = String.Format("The path is too long, filename={0}",
                                            filename);
                    throw new ZipException(msg, ptle);
                }
            }

            entry._LastModified = entry._makeTime;
            entry._FileNameInArchive = ZipSharedUtilities.NormalizePathForUseInZipFile(nameInArchive);
            // We don't actually slurp in the file data until the caller invokes Write on this entry.

            return entry;
        }

        internal void MarkAsDirectory()
        {
            _IsDirectory = true;
            // workitem 6279
            if (!_FileNameInArchive.EndsWith("/"))
                _FileNameInArchive += "/";
        }

        public override String ToString()
        {
            return String.Format("ZipEntry::{0}", FileName);
        }

        private void SetFdpLoh()
        {
            // The value for FileDataPosition has not yet been set.
            // Therefore, seek to the local header, and figure the start of file data.
            // workitem 8098: ok (restore)
            long origPosition = ArchiveStream.Position;
            try
            {
                ArchiveStream.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);

                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(ArchiveStream);
            }
            catch (System.IO.IOException exc1)
            {
                String description = String.Format("Exception seeking  entry({0}) offset(0x{1:X8}) len(0x{2:X8})",
                                                   FileName, _RelativeOffsetOfLocalHeader,
                                                   ArchiveStream.Length);
                throw new ZipBadStateException(description, exc1);
            }

            Byte[] block = new Byte[30];
            ArchiveStream.Read(block, 0, block.Length);

            // At this point we could verify the contents read from the local header
            // with the contents read from the central header.  We could, but don't need to.
            // So we won't.

            Int16 filenameLength = (short)(block[26] + block[27] * 256);
            Int16 extraFieldLength = (short)(block[28] + block[29] * 256);

            // Console.WriteLine("  pos  0x{0:X8} ({0})", this.ArchiveStream.Position);
            // Console.WriteLine("  seek 0x{0:X8} ({0})", filenameLength + extraFieldLength);

            ArchiveStream.Seek(filenameLength + extraFieldLength, SeekOrigin.Current);
            // workitem 10178
            ZipSharedUtilities.Workaround_Ladybug318918(ArchiveStream);

            _LengthOfHeader = 30 + extraFieldLength + filenameLength +
                              GetLengthOfCryptoHeaderBytes(_Encryption_FromZipFile);

            // Console.WriteLine("  ROLH  0x{0:X8} ({0})", _RelativeOffsetOfLocalHeader);
            // Console.WriteLine("  LOH   0x{0:X8} ({0})", _LengthOfHeader);
            // workitem 8098: ok (arithmetic)
            __FileDataPosition = _RelativeOffsetOfLocalHeader + _LengthOfHeader;
            // Console.WriteLine("  FDP   0x{0:X8} ({0})", __FileDataPosition);

            // restore file position:
            // workitem 8098: ok (restore)
            ArchiveStream.Seek(origPosition, SeekOrigin.Begin);
            // workitem 10178
            ZipSharedUtilities.Workaround_Ladybug318918(ArchiveStream);
        }

        internal static int GetLengthOfCryptoHeaderBytes(ZipEncryptionAlgorithm a)
        {
            if (a == ZipEncryptionAlgorithm.None) return 0;
            if (a == ZipEncryptionAlgorithm.PkzipWeak)
                return 12;
            throw new ZipException("internal error");
        }

        internal void ResetDirEntry()
        {
            // Set to -1, to indicate we need to read this later.
            __FileDataPosition = -1;
            // set _LengthOfHeader to 0, to indicate we need to read later.
            _LengthOfHeader = 0;
        }

        internal static ZipEntry ReadDirEntry(ZipFile zf, Dictionary<String, Object> previouslySeen)
        {
            System.IO.Stream s = zf.ReadStream;
            System.Text.Encoding expectedEncoding = (zf.AlternateEncodingUsage == ZipOption.Always)
                                                        ? zf.AlternateEncoding
                                                        : ZipFile.DefaultEncoding;

            int signature = ZipSharedUtilities.ReadSignature(s);
            // return null if this is not a local file header signature
            if (IsNotValidZipDirEntrySig(signature))
            {
                s.Seek(-4, System.IO.SeekOrigin.Current);
                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(s);

                // Getting "not a ZipDirEntry signature" here is not always wrong or an
                // error.  This can happen when walking through a zipfile.  After the
                // last ZipDirEntry, we expect to read an
                // EndOfCentralDirectorySignature.  When we get this is how we know
                // we've reached the end of the central directory.
                if (signature != ZipConstants.EndOfCentralDirectorySignature &&
                    signature != ZipConstants.Zip64EndOfCentralDirectoryRecordSignature &&
                    signature != ZipConstants.ZipEntrySignature // workitem 8299
                    )
                    throw new ZipBadReadException(String.Format("  Bad signature (0x{0:X8}) at position 0x{1:X8}", signature, s.Position));
                return null;
            }

            Byte[] block = new Byte[42];
            int n = s.Read(block, 0, block.Length);
            if (n != block.Length) return null;

            int i = 0;
            ZipEntry zde = new ZipEntry();
            zde.AlternateEncoding = expectedEncoding;
            zde._source = ZipEntrySource.ZipFile;
            zde._container = new ZipContainer(zf);

            unchecked
            {
                zde._versionMadeBy = (short)(block[i++] + block[i++] * 256);
                zde._VersionNeeded = (short)(block[i++] + block[i++] * 256);
                zde._BitField = (short)(block[i++] + block[i++] * 256);
                zde._CompressionMethod = (Int16)(block[i++] + block[i++] * 256);
                zde._TimeBlob = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
                zde._LastModified = ZipSharedUtilities.PackedToDateTime(zde._TimeBlob);
                zde._timestamp |= ZipEntryTimestamp.DOS;

                zde._Crc32 = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
                zde._CompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                zde._UncompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
            }

            // preserve
            zde._CompressionMethod_FromZipFile = zde._CompressionMethod;

            zde._filenameLength = (short)(block[i++] + block[i++] * 256);
            zde._extraFieldLength = (short)(block[i++] + block[i++] * 256);
            zde._commentLength = (short)(block[i++] + block[i++] * 256);
            zde._diskNumber = (UInt32)(block[i++] + block[i++] * 256);

            zde._InternalFileAttrs = (short)(block[i++] + block[i++] * 256);
            zde._ExternalFileAttrs = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;

            zde._RelativeOffsetOfLocalHeader = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);

            // workitem 7801
            zde.IsText = ((zde._InternalFileAttrs & 0x01) == 0x01);

            block = new Byte[zde._filenameLength];
            n = s.Read(block, 0, block.Length);
            if ((zde._BitField & 0x0800) == 0x0800)
            {
                // UTF-8 is in use
                zde._FileNameInArchive = ZipSharedUtilities.Utf8StringFromBuffer(block);
            }
            else
                zde._FileNameInArchive = ZipSharedUtilities.StringFromBuffer(block, expectedEncoding);

            // workitem 10330
            // insure unique entry names
            while (previouslySeen.ContainsKey(zde._FileNameInArchive))
            {
                zde._FileNameInArchive = CopyHelper.AppendCopyToFileName(zde._FileNameInArchive);
                zde._metadataChanged = true;
            }

            if (zde.AttributesIndicateDirectory)
                zde.MarkAsDirectory(); // may append a slash to filename if nec.
                // workitem 6898
            else if (zde._FileNameInArchive.EndsWith("/")) zde.MarkAsDirectory();

            zde._CompressedFileDataSize = zde._CompressedSize;
            if ((zde._BitField & 0x01) == 0x01)
            {
                // this may change after processing the Extra field
                zde._Encryption_FromZipFile = zde._Encryption =
                                              ZipEncryptionAlgorithm.PkzipWeak;
                zde._sourceIsEncrypted = true;
            }

            if (zde._extraFieldLength > 0)
            {
                zde._InputUsesZip64 = (zde._CompressedSize == 0xFFFFFFFF ||
                                       zde._UncompressedSize == 0xFFFFFFFF ||
                                       zde._RelativeOffsetOfLocalHeader == 0xFFFFFFFF);

                // Console.WriteLine("  Input uses Z64?:      {0}", zde._InputUsesZip64);

                zde.ProcessExtraField(s, zde._extraFieldLength);
                zde._CompressedFileDataSize = zde._CompressedSize;
            }

            // we've processed the extra field, so we know the encryption method is set now.
            if (zde._Encryption == ZipEncryptionAlgorithm.PkzipWeak)
            {
                // the "encryption header" of 12 Bytes precedes the file data
                zde._CompressedFileDataSize -= 12;
            }
            // tally the trailing descriptor
            if ((zde._BitField & 0x0008) == 0x0008)
            {
                // sig, CRC, Comp and Uncomp sizes
                if (zde._InputUsesZip64)
                    zde._LengthOfTrailer += 24;
                else
                    zde._LengthOfTrailer += 16;
            }

            // workitem 12744
            zde.AlternateEncoding = ((zde._BitField & 0x0800) == 0x0800)
                                        ? System.Text.Encoding.UTF8
                                        : expectedEncoding;

            zde.AlternateEncodingUsage = ZipOption.Always;

            if (zde._commentLength > 0)
            {
                block = new Byte[zde._commentLength];
                n = s.Read(block, 0, block.Length);
                if ((zde._BitField & 0x0800) == 0x0800)
                {
                    // UTF-8 is in use
                    zde._Comment = ZipSharedUtilities.Utf8StringFromBuffer(block);
                }
                else
                    zde._Comment = ZipSharedUtilities.StringFromBuffer(block, expectedEncoding);
            }
            //zde._LengthOfDirEntry = BytesRead;
            return zde;
        }

        internal static Boolean IsNotValidZipDirEntrySig(int signature)
        {
            return (signature != ZipConstants.ZipDirEntrySignature);
        }

        internal void WriteCentralDirectoryEntry(Stream s)
        {
            Byte[] Bytes = new Byte[4096];
            int i = 0;
            // signature
            Bytes[i++] = (ZipConstants.ZipDirEntrySignature & 0x000000FF);
            Bytes[i++] = ((ZipConstants.ZipDirEntrySignature & 0x0000FF00) >> 8);
            Bytes[i++] = ((ZipConstants.ZipDirEntrySignature & 0x00FF0000) >> 16);
            Bytes[i++] = (Byte)((ZipConstants.ZipDirEntrySignature & 0xFF000000) >> 24);

            // Version Made By
            // workitem 7071
            // We must not overwrite the VersionMadeBy field when writing out a zip
            // archive.  The VersionMadeBy tells the zip reader the meaning of the
            // File attributes.  Overwriting the VersionMadeBy will result in
            // inconsistent metadata.  Consider the scenario where the application
            // opens and reads a zip file that had been created on Linux. Then the
            // app adds one file to the Zip archive, and saves it.  The file
            // attributes for all the entries added on Linux will be significant for
            // Linux.  Therefore the VersionMadeBy for those entries must not be
            // changed.  Only the entries that are actually created on Windows NTFS
            // should get the VersionMadeBy indicating Windows/NTFS.
            Bytes[i++] = (Byte)(_versionMadeBy & 0x00FF);
            Bytes[i++] = (Byte)((_versionMadeBy & 0xFF00) >> 8);

            // Apparently we want to duplicate the extra field here; we cannot
            // simply zero it out and assume tools and apps will use the right one.

            ////Int16 extraFieldLengthSave = (short)(_EntryHeader[28] + _EntryHeader[29] * 256);
            ////_EntryHeader[28] = 0;
            ////_EntryHeader[29] = 0;

            // Version Needed, Bitfield, compression method, lastmod,
            // crc, compressed and uncompressed sizes, filename length and extra field length.
            // These are all present in the local file header, but they may be zero values there.
            // So we cannot just copy them.

            // workitem 11969: Version Needed To Extract in central directory must be
            // the same as the local entry or MS .NET System.IO.Zip fails read.
            Int16 vNeeded = (Int16)(VersionNeeded != 0 ? VersionNeeded : 20);
            // workitem 12964
            if (_OutputUsesZip64 == null)
            {
                // a zipentry in a zipoutputstream, with zero Bytes written
                _OutputUsesZip64 = _container.Zip64 == Zip64Option.Always;
            }

            Int16 versionNeededToExtract = (Int16)(_OutputUsesZip64.Value ? 45 : vNeeded);
#if BZIP
            if (this.CompressionMethod == ZipCompressionMethod.BZip2)
                versionNeededToExtract = 46;
#endif

            Bytes[i++] = (Byte)(versionNeededToExtract & 0x00FF);
            Bytes[i++] = (Byte)((versionNeededToExtract & 0xFF00) >> 8);

            Bytes[i++] = (Byte)(_BitField & 0x00FF);
            Bytes[i++] = (Byte)((_BitField & 0xFF00) >> 8);

            Bytes[i++] = (Byte)(_CompressionMethod & 0x00FF);
            Bytes[i++] = (Byte)((_CompressionMethod & 0xFF00) >> 8);

#if AESCRYPTO
            if (Encryption == ZipEncryptionAlgorithm.WinZipAes128 ||
            Encryption == ZipEncryptionAlgorithm.WinZipAes256)
            {
                i -= 2;
                Bytes[i++] = 0x63;
                Bytes[i++] = 0;
            }
#endif

            Bytes[i++] = (Byte)(_TimeBlob & 0x000000FF);
            Bytes[i++] = (Byte)((_TimeBlob & 0x0000FF00) >> 8);
            Bytes[i++] = (Byte)((_TimeBlob & 0x00FF0000) >> 16);
            Bytes[i++] = (Byte)((_TimeBlob & 0xFF000000) >> 24);
            Bytes[i++] = (Byte)(_Crc32 & 0x000000FF);
            Bytes[i++] = (Byte)((_Crc32 & 0x0000FF00) >> 8);
            Bytes[i++] = (Byte)((_Crc32 & 0x00FF0000) >> 16);
            Bytes[i++] = (Byte)((_Crc32 & 0xFF000000) >> 24);

            int j = 0;
            if (_OutputUsesZip64.Value)
            {
                // CompressedSize (Int32) and UncompressedSize - all 0xFF
                for (j = 0; j < 8; j++)
                    Bytes[i++] = 0xFF;
            }
            else
            {
                Bytes[i++] = (Byte)(_CompressedSize & 0x000000FF);
                Bytes[i++] = (Byte)((_CompressedSize & 0x0000FF00) >> 8);
                Bytes[i++] = (Byte)((_CompressedSize & 0x00FF0000) >> 16);
                Bytes[i++] = (Byte)((_CompressedSize & 0xFF000000) >> 24);

                Bytes[i++] = (Byte)(_UncompressedSize & 0x000000FF);
                Bytes[i++] = (Byte)((_UncompressedSize & 0x0000FF00) >> 8);
                Bytes[i++] = (Byte)((_UncompressedSize & 0x00FF0000) >> 16);
                Bytes[i++] = (Byte)((_UncompressedSize & 0xFF000000) >> 24);
            }

            Byte[] fileNameBytes = GetEncodedFileNameBytes();
            Int16 filenameLength = (Int16)fileNameBytes.Length;
            Bytes[i++] = (Byte)(filenameLength & 0x00FF);
            Bytes[i++] = (Byte)((filenameLength & 0xFF00) >> 8);

            // do this again because now we have real data
            _presumeZip64 = _OutputUsesZip64.Value;

            // workitem 11131
            //
            // cannot generate the extra field again, here's why: In the case of a
            // zero-Byte entry, which uses encryption, DotNetZip will "remove" the
            // encryption from the entry.  It does this in PostProcessOutput; it
            // modifies the entry header, and rewrites it, resetting the Bitfield
            // (one bit indicates encryption), and potentially resetting the
            // compression method - for AES the Compression method is 0x63, and it
            // would get reset to zero (no compression).  It then calls SetLength()
            // to truncate the stream to remove the encryption header (12 Bytes for
            // AES256).  But, it leaves the previously-generated "Extra Field"
            // metadata (11 Bytes) for AES in the entry header. This extra field
            // data is now "orphaned" - it refers to AES encryption when in fact no
            // AES encryption is used. But no problem, the PKWARE spec says that
            // unrecognized extra fields can just be ignored. ok.  After "removal"
            // of AES encryption, the length of the Extra Field can remains the
            // same; it's just that there will be 11 Bytes in there that previously
            // pertained to AES which are now unused. Even the field code is still
            // there, but it will be unused by readers, as the encryption bit is not
            // set.
            //
            // Re-calculating the Extra field now would produce a block that is 11
            // Bytes shorter, and that mismatch - between the extra field in the
            // local header and the extra field in the Central Directory - would
            // cause problems. (where? why? what problems?)  So we can't do
            // that. It's all good though, because though the content may have
            // changed, the length definitely has not. Also, the _EntryHeader
            // contains the "updated" extra field (after PostProcessOutput) at
            // offset (30 + filenameLength).

            _Extra = ConstructExtraField(true);

            Int16 extraFieldLength = (Int16)((_Extra == null) ? 0 : _Extra.Length);
            Bytes[i++] = (Byte)(extraFieldLength & 0x00FF);
            Bytes[i++] = (Byte)((extraFieldLength & 0xFF00) >> 8);

            // File (entry) Comment Length
            // the _CommentBytes private field was set during WriteHeader()
            int commentLength = (_CommentBytes == null) ? 0 : _CommentBytes.Length;

            // the size of our buffer defines the max length of the comment we can write
            if (commentLength + i > Bytes.Length) commentLength = Bytes.Length - i;
            Bytes[i++] = (Byte)(commentLength & 0x00FF);
            Bytes[i++] = (Byte)((commentLength & 0xFF00) >> 8);

            // Disk number start
            Boolean segmented = (_container.ZipFile != null) &&
                                (_container.ZipFile.MaxOutputSegmentSize != 0);
            if (segmented) // workitem 13915
            {
                // Emit nonzero disknumber only if saving segmented archive.
                Bytes[i++] = (Byte)(_diskNumber & 0x00FF);
                Bytes[i++] = (Byte)((_diskNumber & 0xFF00) >> 8);
            }
            else
            {
                // If reading a segmneted archive and saving to a regular archive,
                // ZipEntry._diskNumber will be non-zero but it should be saved as
                // zero.
                Bytes[i++] = 0;
                Bytes[i++] = 0;
            }

            // internal file attrs
            // workitem 7801
            Bytes[i++] = (Byte)((_IsText) ? 1 : 0); // lo bit: filetype hint.  0=bin, 1=txt.
            Bytes[i++] = 0;

            // external file attrs
            // workitem 7071
            Bytes[i++] = (Byte)(_ExternalFileAttrs & 0x000000FF);
            Bytes[i++] = (Byte)((_ExternalFileAttrs & 0x0000FF00) >> 8);
            Bytes[i++] = (Byte)((_ExternalFileAttrs & 0x00FF0000) >> 16);
            Bytes[i++] = (Byte)((_ExternalFileAttrs & 0xFF000000) >> 24);

            // workitem 11131
            // relative offset of local header.
            //
            // If necessary to go to 64-bit value, then emit 0xFFFFFFFF,
            // else write out the value.
            //
            // Even if zip64 is required for other reasons - number of the entry
            // > 65534, or uncompressed size of the entry > MAX_INT32, the ROLH
            // need not be stored in a 64-bit field .
            if (_RelativeOffsetOfLocalHeader > 0xFFFFFFFFL) // _OutputUsesZip64.Value
            {
                Bytes[i++] = 0xFF;
                Bytes[i++] = 0xFF;
                Bytes[i++] = 0xFF;
                Bytes[i++] = 0xFF;
            }
            else
            {
                Bytes[i++] = (Byte)(_RelativeOffsetOfLocalHeader & 0x000000FF);
                Bytes[i++] = (Byte)((_RelativeOffsetOfLocalHeader & 0x0000FF00) >> 8);
                Bytes[i++] = (Byte)((_RelativeOffsetOfLocalHeader & 0x00FF0000) >> 16);
                Bytes[i++] = (Byte)((_RelativeOffsetOfLocalHeader & 0xFF000000) >> 24);
            }

            // actual filename
            Buffer.BlockCopy(fileNameBytes, 0, Bytes, i, filenameLength);
            i += filenameLength;

            // "Extra field"
            if (_Extra != null)
            {
                // workitem 11131
                //
                // copy from EntryHeader if available - it may have been updated.
                // if not, copy from Extra. This would be unnecessary if I just
                // updated the Extra field when updating EntryHeader, in
                // PostProcessOutput.

                //?? I don't understand why I wouldn't want to just use
                // the recalculated Extra field. ??

                // Byte[] h = _EntryHeader ?? _Extra;
                // int offx = (h == _EntryHeader) ? 30 + filenameLength : 0;
                // Buffer.BlockCopy(h, offx, Bytes, i, extraFieldLength);
                // i += extraFieldLength;

                Byte[] h = _Extra;
                int offx = 0;
                Buffer.BlockCopy(h, offx, Bytes, i, extraFieldLength);
                i += extraFieldLength;
            }

            // file (entry) comment
            if (commentLength != 0)
            {
                // now actually write the comment itself into the Byte buffer
                Buffer.BlockCopy(_CommentBytes, 0, Bytes, i, commentLength);
                // for (j = 0; (j < commentLength) && (i + j < Bytes.Length); j++)
                //     Bytes[i + j] = _CommentBytes[j];
                i += commentLength;
            }

            s.Write(Bytes, 0, i);
        }

        private Byte[] ConstructExtraField(Boolean forCentralDirectory)
        {
            var listOfBlocks = new List<Byte[]>();
            Byte[] block;

            // Conditionally emit an extra field with Zip64 information.  If the
            // Zip64 option is Always, we emit the field, before knowing that it's
            // necessary.  Later, if it turns out this entry does not need zip64,
            // we'll set the header ID to rubbish and the data will be ignored.
            // This results in additional overhead metadata in the zip file, but
            // it will be small in comparison to the entry data.
            //
            // On the other hand if the Zip64 option is AsNecessary and it's NOT
            // for the central directory, then we do the same thing.  Or, if the
            // Zip64 option is AsNecessary and it IS for the central directory,
            // and the entry requires zip64, then emit the header.
            if (_container.Zip64 == Zip64Option.Always ||
                (_container.Zip64 == Zip64Option.AsNecessary &&
                 (!forCentralDirectory || _entryRequiresZip64.Value)))
            {
                // add extra field for zip64 here
                // workitem 7924
                int sz = 4 + (forCentralDirectory ? 28 : 16);
                block = new Byte[sz];
                int i = 0;

                if (_presumeZip64 || forCentralDirectory)
                {
                    // HeaderId = always use zip64 extensions.
                    block[i++] = 0x01;
                    block[i++] = 0x00;
                }
                else
                {
                    // HeaderId = dummy data now, maybe set to 0x0001 (ZIP64) later.
                    block[i++] = 0x99;
                    block[i++] = 0x99;
                }

                // DataSize
                block[i++] = (Byte)(sz - 4); // decimal 28 or 16  (workitem 7924)
                block[i++] = 0x00;

                // The actual metadata - we may or may not have real values yet...

                // uncompressed size
                Array.Copy(BitConverter.GetBytes(_UncompressedSize), 0, block, i, 8);
                i += 8;
                // compressed size
                Array.Copy(BitConverter.GetBytes(_CompressedSize), 0, block, i, 8);
                i += 8;

                // workitem 7924 - only include this if the "extra" field is for
                // use in the central directory.  It is unnecessary and not useful
                // for local header; makes WinZip choke.
                if (forCentralDirectory)
                {
                    // relative offset
                    Array.Copy(BitConverter.GetBytes(_RelativeOffsetOfLocalHeader), 0, block, i, 8);
                    i += 8;

                    // starting disk number
                    Array.Copy(BitConverter.GetBytes(0), 0, block, i, 4);
                }
                listOfBlocks.Add(block);
            }

#if AESCRYPTO
            if (Encryption == ZipEncryptionAlgorithm.WinZipAes128 ||
                Encryption == ZipEncryptionAlgorithm.WinZipAes256)
            {
                block = new Byte[4 + 7];
                int i = 0;
                // extra field for WinZip AES
                // header id
                block[i++] = 0x01;
                block[i++] = 0x99;

                // data size
                block[i++] = 0x07;
                block[i++] = 0x00;

                // vendor number
                block[i++] = 0x01;  // AE-1 - means "Verify CRC"
                block[i++] = 0x00;

                // vendor id "AE"
                block[i++] = 0x41;
                block[i++] = 0x45;

                // key strength
                int keystrength = GetKeyStrengthInBits(Encryption);
                if (keystrength == 128)
                    block[i] = 1;
                else if (keystrength == 256)
                    block[i] = 3;
                else
                    block[i] = 0xFF;
                i++;

                // actual compression method
                block[i++] = (Byte)(_CompressionMethod & 0x00FF);
                block[i++] = (Byte)(_CompressionMethod & 0xFF00);

                listOfBlocks.Add(block);
            }
#endif

            if (_ntfsTimesAreSet && _emitNtfsTimes)
            {
                block = new Byte[32 + 4];
                // HeaderId   2 Bytes    0x000a == NTFS times
                // Datasize   2 Bytes    32
                // reserved   4 Bytes    ?? don't care
                // timetag    2 Bytes    0x0001 == NTFS time
                // size       2 Bytes    24 == 8 Bytes each for ctime, mtime, atime
                // mtime      8 Bytes    win32 ticks since win32epoch
                // atime      8 Bytes    win32 ticks since win32epoch
                // ctime      8 Bytes    win32 ticks since win32epoch
                int i = 0;
                // extra field for NTFS times
                // header id
                block[i++] = 0x0a;
                block[i++] = 0x00;

                // data size
                block[i++] = 32;
                block[i++] = 0;

                i += 4; // reserved

                // time tag
                block[i++] = 0x01;
                block[i++] = 0x00;

                // data size (again)
                block[i++] = 24;
                block[i++] = 0;

                Int64 z = _makeTime.ToFileTime();
                Array.Copy(BitConverter.GetBytes(z), 0, block, i, 8);
                i += 8;
                z = _actualTime.ToFileTime();
                Array.Copy(BitConverter.GetBytes(z), 0, block, i, 8);
                i += 8;
                z = _createTime.ToFileTime();
                Array.Copy(BitConverter.GetBytes(z), 0, block, i, 8);
                i += 8;

                listOfBlocks.Add(block);
            }

            if (_ntfsTimesAreSet && _emitUnixTimes)
            {
                int len = 5 + 4;
                if (!forCentralDirectory) len += 8;

                block = new Byte[len];
                // local form:
                // --------------
                // HeaderId   2 Bytes    0x5455 == unix timestamp
                // Datasize   2 Bytes    13
                // flags      1 Byte     7 (low three bits all set)
                // mtime      4 Bytes    seconds since unix epoch
                // atime      4 Bytes    seconds since unix epoch
                // ctime      4 Bytes    seconds since unix epoch
                //
                // central directory form:
                //---------------------------------
                // HeaderId   2 Bytes    0x5455 == unix timestamp
                // Datasize   2 Bytes    5
                // flags      1 Byte     7 (low three bits all set)
                // mtime      4 Bytes    seconds since unix epoch
                //
                int i = 0;
                // extra field for "unix" times
                // header id
                block[i++] = 0x55;
                block[i++] = 0x54;

                // data size
                block[i++] = unchecked((Byte)(len - 4));
                block[i++] = 0;

                // flags
                block[i++] = 0x07;

                Int32 z = unchecked((int)((_makeTime - _unixEpoch).TotalSeconds));
                Array.Copy(BitConverter.GetBytes(z), 0, block, i, 4);
                i += 4;
                if (!forCentralDirectory)
                {
                    z = unchecked((int)((_actualTime - _unixEpoch).TotalSeconds));
                    Array.Copy(BitConverter.GetBytes(z), 0, block, i, 4);
                    i += 4;
                    z = unchecked((int)((_createTime - _unixEpoch).TotalSeconds));
                    Array.Copy(BitConverter.GetBytes(z), 0, block, i, 4);
                    i += 4;
                }
                listOfBlocks.Add(block);
            }

            // inject other blocks here...

            // concatenate any blocks we've got:
            Byte[] aggregateBlock = null;
            if (listOfBlocks.Count > 0)
            {
                int totalLength = 0;
                int i, current = 0;
                for (i = 0; i < listOfBlocks.Count; i++)
                    totalLength += listOfBlocks[i].Length;
                aggregateBlock = new Byte[totalLength];
                for (i = 0; i < listOfBlocks.Count; i++)
                {
                    System.Array.Copy(listOfBlocks[i], 0, aggregateBlock, current, listOfBlocks[i].Length);
                    current += listOfBlocks[i].Length;
                }
            }

            return aggregateBlock;
        }

        private String NormalizeFileName()
        {
            // here, we need to flip the backslashes to forward-slashes,
            // also, we need to trim the \\server\share syntax from any UNC path.
            // and finally, we need to remove any leading .\

            String SlashFixed = FileName.Replace("\\", "/");
            String s1 = null;
            if ((_TrimVolumeFromFullyQualifiedPaths) && (FileName.Length >= 3)
                && (FileName[1] == ':') && (SlashFixed[2] == '/'))
            {
                // trim off volume letter, colon, and slash
                s1 = SlashFixed.Substring(3);
            }
            else if ((FileName.Length >= 4)
                     && ((SlashFixed[0] == '/') && (SlashFixed[1] == '/')))
            {
                int n = SlashFixed.IndexOf('/', 2);
                if (n == -1)
                    throw new ArgumentException("The path for that entry appears to be badly formatted");
                s1 = SlashFixed.Substring(n + 1);
            }
            else if ((FileName.Length >= 3)
                     && ((SlashFixed[0] == '.') && (SlashFixed[1] == '/')))
            {
                // trim off dot and slash
                s1 = SlashFixed.Substring(2);
            }
            else
                s1 = SlashFixed;
            return s1;
        }

        private Byte[] GetEncodedFileNameBytes()
        {
            // workitem 6513
            var s1 = NormalizeFileName();

            switch (AlternateEncodingUsage)
            {
                case ZipOption.Always:
                    if ((_Comment == null || _Comment.Length == 0) == false)
                        _CommentBytes = AlternateEncoding.GetBytes(_Comment);
                    _actualEncoding = AlternateEncoding;

                    return AlternateEncoding.GetBytes(s1);

                case ZipOption.Never:
                    if ((_Comment == null || _Comment.Length == 0) == false)
                        _CommentBytes = ibm437.GetBytes(_Comment);
                    _actualEncoding = ibm437;

                    return ibm437.GetBytes(s1);
            }

            // arriving here means AlternateEncodingUsage is "AsNecessary"

            // case ZipOption.AsNecessary:
            // workitem 6513: when writing, use the alternative encoding
            // only when _actualEncoding is not yet set (it can be set
            // during Read), and when ibm437 will not do.

            Byte[] result = ibm437.GetBytes(s1);
            // need to use this form of GetString() for .NET CF
            String s2 = ibm437.GetString(result, 0, result.Length);
            _CommentBytes = null;
            if (s2 != s1)
            {
                // Encoding the filename with ibm437 does not allow round-trips.
                // Therefore, use the alternate encoding.  Assume it will work,
                // no checking of round trips here.
                result = AlternateEncoding.GetBytes(s1);
                if (_Comment != null && _Comment.Length != 0)
                    _CommentBytes = AlternateEncoding.GetBytes(_Comment);
                _actualEncoding = AlternateEncoding;
                return result;
            }

            _actualEncoding = ibm437;

            // Using ibm437, FileName can be encoded without information
            // loss; now try the Comment.

            // if there is no comment, use ibm437.
            if (_Comment == null || _Comment.Length == 0)
                return result;

            // there is a comment. Get the encoded form.
            Byte[] cBytes = ibm437.GetBytes(_Comment);
            String c2 = ibm437.GetString(cBytes, 0, cBytes.Length);

            // Check for round-trip.
            if (c2 != Comment)
            {
                // Comment cannot correctly be encoded with ibm437.  Use
                // the alternate encoding.

                result = AlternateEncoding.GetBytes(s1);
                _CommentBytes = AlternateEncoding.GetBytes(_Comment);
                _actualEncoding = AlternateEncoding;
                return result;
            }

            // use IBM437
            _CommentBytes = cBytes;

            return result;
        }

        private Boolean WantReadAgain()
        {
            if (_UncompressedSize < 0x10) return false;
            if (_CompressionMethod == 0x00) return false;
            if (CompressionLevel == ZipCompressionLevel.None) return false;
            if (_CompressedSize < _UncompressedSize) return false;

            if (_source == ZipEntrySource.Stream && !_sourceStream.CanSeek) return false;
            if (_zipCrypto_forWrite != null && (CompressedSize - 12) <= UncompressedSize) return false;

            return true;
        }

        private void MaybeUnsetCompressionMethodForWriting(int cycle)
        {
            // if we've already tried with compression... turn it off this time
            if (cycle > 1)
            {
                _CompressionMethod = 0x0;
                return;
            }
            // compression for directories = 0x00 (No Compression)
            if (IsDirectory)
            {
                _CompressionMethod = 0x0;
                return;
            }

            if (_source == ZipEntrySource.ZipFile)
                return; // do nothing

            // If __FileDataPosition is zero, then that means we will get the data
            // from a file or stream.

            // It is never possible to compress a zero-length file, so we check for
            // this condition.

            if (_source == ZipEntrySource.Stream)
            {
                // workitem 7742
                if (_sourceStream != null && _sourceStream.CanSeek)
                {
                    // Length prop will throw if CanSeek is false
                    long fileLength = _sourceStream.Length;
                    if (fileLength == 0)
                    {
                        _CompressionMethod = 0x00;
                        return;
                    }
                }
            }
            else if ((_source == ZipEntrySource.FileSystem) && (ZipSharedUtilities.GetFileLength(LocalFileName) == 0L))
            {
                _CompressionMethod = 0x00;
                return;
            }

            // Ok, we're getting the data to be compressed from a
            // non-zero-length file or stream, or a file or stream of
            // unknown length, and we presume that it is non-zero.  In
            // that case we check the callback to see if the app wants
            // to tell us whether to compress or not.
            if (SetCompression != null)
                CompressionLevel = SetCompression(LocalFileName, _FileNameInArchive);

            // finally, set CompressionMethod to None if CompressionLevel is None
            if (CompressionLevel == (short)ZipCompressionLevel.None &&
                CompressionMethod == ZipCompressionMethod.Deflate)
                _CompressionMethod = 0x00;

            return;
        }

        internal void WriteHeader(Stream s, int cycle)
        {
            // Must remember the offset, within the output stream, of this particular
            // entry header.
            //
            // This is for 2 reasons:
            //
            //  1. so we can determine the RelativeOffsetOfLocalHeader (ROLH) for
            //     use in the central directory.
            //  2. so we can seek backward in case there is an error opening or reading
            //     the file, and the application decides to skip the file. In this case,
            //     we need to seek backward in the output stream to allow the next entry
            //     to be added to the zipfile output stream.
            //
            // Normally you would just store the offset before writing to the output
            // stream and be done with it.  But the possibility to use split archives
            // makes this approach ineffective.  In split archives, each file or segment
            // is bound to a max size limit, and each local file header must not span a
            // segment boundary; it must be written contiguously.  If it will fit in the
            // current segment, then the ROLH is just the current Position in the output
            // stream.  If it won't fit, then we need a new file (segment) and the ROLH
            // is zero.
            //
            // But we only can know if it is possible to write a header contiguously
            // after we know the size of the local header, a size that varies with
            // things like filename length, comments, and extra fields.  We have to
            // compute the header fully before knowing whether it will fit.
            //
            // That takes care of item #1 above.  Now, regarding #2.  If an error occurs
            // while computing the local header, we want to just seek backward. The
            // exception handling logic (in the caller of WriteHeader) uses ROLH to
            // scroll back.
            //
            // All this means we have to preserve the starting offset before computing
            // the header, and also we have to compute the offset later, to handle the
            // case of split archives.

            var counter = s as ZipCountingStream;

            // workitem 8098: ok (output)
            // This may change later, for split archives

            // Don't set _RelativeOffsetOfLocalHeader. Instead, set a temp variable.
            // This allows for re-streaming, where a zip entry might be read from a
            // zip archive (and maybe decrypted, and maybe decompressed) and then
            // written to another zip archive, with different settings for
            // compression method, compression level, or encryption algorithm.
            _future_ROLH = (counter != null) ? counter.ComputedPosition : s.Position;

            int j = 0, i = 0;

            Byte[] block = new Byte[30];

            // signature
            block[i++] = (ZipConstants.ZipEntrySignature & 0x000000FF);
            block[i++] = ((ZipConstants.ZipEntrySignature & 0x0000FF00) >> 8);
            block[i++] = ((ZipConstants.ZipEntrySignature & 0x00FF0000) >> 16);
            block[i++] = (Byte)((ZipConstants.ZipEntrySignature & 0xFF000000) >> 24);

            // Design notes for ZIP64:
            //
            // The specification says that the header must include the Compressed
            // and Uncompressed sizes, as well as the CRC32 value.  When creating
            // a zip via streamed processing, these quantities are not known until
            // after the compression is done.  Thus, a typical way to do it is to
            // insert zeroes for these quantities, then do the compression, then
            // seek back to insert the appropriate values, then seek forward to
            // the end of the file data.
            //
            // There is also the option of using bit 3 in the GP bitfield - to
            // specify that there is a data descriptor after the file data
            // containing these three quantities.
            //
            // This works when the size of the quantities is known, either 32-bits
            // or 64 bits as with the ZIP64 extensions.
            //
            // With Zip64, the 4-Byte fields are set to 0xffffffff, and there is a
            // corresponding data block in the "extra field" that contains the
            // actual Compressed, uncompressed sizes.  (As well as an additional
            // field, the "Relative Offset of Local Header")
            //
            // The problem is when the app desires to use ZIP64 extensions
            // optionally, only when necessary.  Suppose the library assumes no
            // zip64 extensions when writing the header, then after compression
            // finds that the size of the data requires zip64.  At this point, the
            // header, already written to the file, won't have the necessary data
            // block in the "extra field".  The size of the entry header is fixed,
            // so it is not possible to just "add on" the zip64 data block after
            // compressing the file.  On the other hand, always using zip64 will
            // break interoperability with many other systems and apps.
            //
            // The approach we take is to insert a 32-Byte dummy data block in the
            // extra field, whenever zip64 is to be used "as necessary". This data
            // block will get the actual zip64 HeaderId and zip64 metadata if
            // necessary.  If not necessary, the data block will get a meaningless
            // HeaderId (0x1111), and will be filled with zeroes.
            //
            // When zip64 is actually in use, we also need to set the
            // VersionNeededToExtract field to 45.
            //
            // There is one additional wrinkle: using zip64 as necessary conflicts
            // with output to non-seekable devices.  The header is emitted and
            // must indicate whether zip64 is in use, before we know if zip64 is
            // necessary.  Because there is no seeking, the header can never be
            // changed.  Therefore, on non-seekable devices,
            // Zip64Option.AsNecessary is the same as Zip64Option.Always.
            //

            // version needed- see AppNote.txt.
            //
            // need v5.1 for PKZIP strong encryption, or v2.0 for no encryption or
            // for PK encryption, 4.5 for zip64.  We may reset this later, as
            // necessary or zip64.

            _presumeZip64 = (_container.Zip64 == Zip64Option.Always ||
                             (_container.Zip64 == Zip64Option.AsNecessary && !s.CanSeek));
            Int16 VersionNeededToExtract = (Int16)(_presumeZip64 ? 45 : 20);

            // (i==4)
            block[i++] = (Byte)(VersionNeededToExtract & 0x00FF);
            block[i++] = (Byte)((VersionNeededToExtract & 0xFF00) >> 8);

            // Get Byte array. Side effect: sets ActualEncoding.
            // Must determine encoding before setting the bitfield.
            // workitem 6513
            Byte[] fileNameBytes = GetEncodedFileNameBytes();
            Int16 filenameLength = (Int16)fileNameBytes.Length;

            // general purpose bitfield
            // In the current implementation, this library uses only these bits
            // in the GP bitfield:
            //  bit 0 = if set, indicates the entry is encrypted
            //  bit 3 = if set, indicates the CRC, C and UC sizes follow the file data.
            //  bit 6 = strong encryption - for pkware's meaning of strong encryption
            //  bit 11 = UTF-8 encoding is used in the comment and filename

            // Here we set or unset the encryption bit.
            // _BitField may already be set, as with a ZipEntry added into ZipOutputStream, which
            // has bit 3 always set. We only want to set one bit
            if (_Encryption == ZipEncryptionAlgorithm.None)
                _BitField &= ~1; // encryption bit OFF
            else
                _BitField |= 1; // encryption bit ON

            // workitem 7941: WinZip does not the "strong encryption" bit  when using AES.
            // This "Strong Encryption" is a PKWare Strong encryption thing.
            //                 _BitField |= 0x0020;

            // set the UTF8 bit if necessary
#if SILVERLIGHT
            if (_actualEncoding.WebName == "utf-8")
#else
            if (_actualEncoding.CodePage == System.Text.Encoding.UTF8.CodePage)
#endif
                _BitField |= 0x0800;

            // The PKZIP spec says that if bit 3 is set (0x0008) in the General
            // Purpose BitField, then the CRC, Compressed size, and uncompressed
            // size are written directly after the file data.
            //
            // These 3 quantities are normally present in the regular zip entry
            // header. But, they are not knowable until after the compression is
            // done. So, in the normal case, we
            //
            //  - write the header, using zeros for these quantities
            //  - compress the data, and incidentally compute these quantities.
            //  - seek back and write the correct values them into the header.
            //
            // This is nice because, while it is more complicated to write the zip
            // file, it is simpler and less error prone to read the zip file, and
            // as a result more applications can read zip files produced this way,
            // with those 3 quantities in the header.
            //
            // But if seeking in the output stream is not possible, then we need
            // to set the appropriate bitfield and emit these quantities after the
            // compressed file data in the output.
            //
            // workitem 7216 - having trouble formatting a zip64 file that is
            // readable by WinZip.  not sure why!  What I found is that setting
            // bit 3 and following all the implications, the zip64 file is
            // readable by WinZip 12. and Perl's IO::Compress::Zip .  Perl takes
            // an interesting approach - it always sets bit 3 if ZIP64 in use.
            // DotNetZip now does the same; this gives better compatibility with
            // WinZip 12.

            if (IsDirectory || cycle == 99)
            {
                // (cycle == 99) indicates a zero-length entry written by ZipOutputStream

                _BitField &= ~0x0008; // unset bit 3 - no "data descriptor" - ever
                _BitField &= ~0x0001; // unset bit 1 - no encryption - ever
                Encryption = ZipEncryptionAlgorithm.None;
                Password = null;
            }
            else if (!s.CanSeek)
                _BitField |= 0x0008;

#if DONT_GO_THERE
            else if (this.Encryption == ZipEncryptionAlgorithm.PkzipWeak  &&
                     this._source != ZipEntrySource.ZipFile)
            {
                // Set bit 3 to avoid the double-read perf issue.
                //
                // When PKZIP encryption is used, Byte 11 of the encryption header is
                // used as a consistency check. It is normally set to the MSByte of the
                // CRC.  But this means the cRC must be known ebfore compression and
                // encryption, which means the entire stream has to be read twice.  To
                // avoid that, the high-Byte of the time blob (when in DOS format) can
                // be used for the consistency check (Byte 11 in the encryption header).
                // But this means the entry must have bit 3 set.
                //
                // Previously I used a more complex arrangement - using the methods like
                // FigureCrc32(), PrepOutputStream() and others, in order to manage the
                // seek-back in the source stream.  Why?  Because bit 3 is not always
                // friendly with third-party zip tools, like those on the Mac.
                //
                // This is why this code is still ifdef'd  out.
                //
                // Might consider making this yet another programmable option -
                // AlwaysUseBit3ForPkzip.  But that's for another day.
                //
                _BitField |= 0x0008;
            }
#endif

            // (i==6)
            block[i++] = (Byte)(_BitField & 0x00FF);
            block[i++] = (Byte)((_BitField & 0xFF00) >> 8);

            // Here, we want to set values for Compressed Size, Uncompressed Size,
            // and CRC.  If we have __FileDataPosition as not -1 (zero is a valid
            // FDP), then that means we are reading this zip entry from a zip
            // file, and we have good values for those quantities.
            //
            // If _FileDataPosition is -1, then we are constructing this Entry
            // from nothing.  We zero those quantities now, and we will compute
            // actual values for the three quantities later, when we do the
            // compression, and then seek back to write them into the appropriate
            // place in the header.
            if (__FileDataPosition == -1)
            {
                //_UncompressedSize = 0; // do not unset - may need this value for restream
                // _Crc32 = 0;           // ditto
                _CompressedSize = 0;
                _crcCalculated = false;
            }

            // set compression method here
            MaybeUnsetCompressionMethodForWriting(cycle);

            // (i==8) compression method
            block[i++] = (Byte)(_CompressionMethod & 0x00FF);
            block[i++] = (Byte)((_CompressionMethod & 0xFF00) >> 8);

            if (cycle == 99)
            {
                // (cycle == 99) indicates a zero-length entry written by ZipOutputStream
                SetZip64Flags();
            }

#if AESCRYPTO
            else if (Encryption == ZipEncryptionAlgorithm.WinZipAes128 || Encryption == ZipEncryptionAlgorithm.WinZipAes256)
            {
                i -= 2;
                block[i++] = 0x63;
                block[i++] = 0;
            }
#endif

            // LastMod
            _TimeBlob = ZipSharedUtilities.DateTimeToPacked(LastModified);

            // (i==10) time blob
            block[i++] = (Byte)(_TimeBlob & 0x000000FF);
            block[i++] = (Byte)((_TimeBlob & 0x0000FF00) >> 8);
            block[i++] = (Byte)((_TimeBlob & 0x00FF0000) >> 16);
            block[i++] = (Byte)((_TimeBlob & 0xFF000000) >> 24);

            // (i==14) CRC - if source==filesystem, this is zero now, actual value
            // will be calculated later.  if source==archive, this is a bonafide
            // value.
            block[i++] = (Byte)(_Crc32 & 0x000000FF);
            block[i++] = (Byte)((_Crc32 & 0x0000FF00) >> 8);
            block[i++] = (Byte)((_Crc32 & 0x00FF0000) >> 16);
            block[i++] = (Byte)((_Crc32 & 0xFF000000) >> 24);

            if (_presumeZip64)
            {
                // (i==18) CompressedSize (Int32) and UncompressedSize - all 0xFF for now
                for (j = 0; j < 8; j++)
                    block[i++] = 0xFF;
            }
            else
            {
                // (i==18) CompressedSize (Int32) - this value may or may not be
                // bonafide.  if source == filesystem, then it is zero, and we'll
                // learn it after we compress.  if source == archive, then it is
                // bonafide data.
                block[i++] = (Byte)(_CompressedSize & 0x000000FF);
                block[i++] = (Byte)((_CompressedSize & 0x0000FF00) >> 8);
                block[i++] = (Byte)((_CompressedSize & 0x00FF0000) >> 16);
                block[i++] = (Byte)((_CompressedSize & 0xFF000000) >> 24);

                // (i==22) UncompressedSize (Int32) - this value may or may not be
                // bonafide.
                block[i++] = (Byte)(_UncompressedSize & 0x000000FF);
                block[i++] = (Byte)((_UncompressedSize & 0x0000FF00) >> 8);
                block[i++] = (Byte)((_UncompressedSize & 0x00FF0000) >> 16);
                block[i++] = (Byte)((_UncompressedSize & 0xFF000000) >> 24);
            }

            // (i==26) filename length (Int16)
            block[i++] = (Byte)(filenameLength & 0x00FF);
            block[i++] = (Byte)((filenameLength & 0xFF00) >> 8);

            _Extra = ConstructExtraField(false);

            // (i==28) extra field length (short)
            Int16 extraFieldLength = (Int16)((_Extra == null) ? 0 : _Extra.Length);
            block[i++] = (Byte)(extraFieldLength & 0x00FF);
            block[i++] = (Byte)((extraFieldLength & 0xFF00) >> 8);

            // workitem 13542
            Byte[] Bytes = new Byte[i + filenameLength + extraFieldLength];

            // get the fixed portion
            Buffer.BlockCopy(block, 0, Bytes, 0, i);
            //for (j = 0; j < i; j++) Bytes[j] = block[j];

            // The filename written to the archive.
            Buffer.BlockCopy(fileNameBytes, 0, Bytes, i, fileNameBytes.Length);
            // for (j = 0; j < fileNameBytes.Length; j++)
            //     Bytes[i + j] = fileNameBytes[j];

            i += fileNameBytes.Length;

            // "Extra field"
            if (_Extra != null)
            {
                Buffer.BlockCopy(_Extra, 0, Bytes, i, _Extra.Length);
                // for (j = 0; j < _Extra.Length; j++)
                //     Bytes[i + j] = _Extra[j];
                i += _Extra.Length;
            }

            _LengthOfHeader = i;

            // handle split archives
            var zss = s as ZipSegmentedStream;
            if (zss != null)
            {
                zss.ContiguousWrite = true;
                UInt32 requiredSegment = zss.ComputeSegment(i);
                if (requiredSegment != zss.CurrentSegment)
                    _future_ROLH = 0; // rollover!
                else
                    _future_ROLH = zss.Position;

                _diskNumber = requiredSegment;
            }

            // validate the ZIP64 usage
            if (_container.Zip64 == Zip64Option.Never && (uint)_RelativeOffsetOfLocalHeader >= 0xFFFFFFFF)
                throw new ZipException("Offset within the zip archive exceeds 0xFFFFFFFF. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");

            // finally, write the header to the stream
            s.Write(Bytes, 0, i);

            // now that the header is written, we can turn off the contiguous write restriction.
            if (zss != null)
                zss.ContiguousWrite = false;

            // Preserve this header data, we'll use it again later.
            // ..when seeking backward, to write again, after we have the Crc, compressed
            //   and uncompressed sizes.
            // ..and when writing the central directory structure.
            _EntryHeader = Bytes;
        }

        private Int32 FigureCrc32()
        {
            if (_crcCalculated == false)
            {
                Stream input = null;
                // get the original stream:
                if (_source == ZipEntrySource.WriteDelegate)
                {
                    var output = new ZipCrcCalculatorStream(Stream.Null);
                    // allow the application to write the data
                    _WriteDelegate(FileName, output);
                    _Crc32 = output.Crc;
                }
                else if (_source == ZipEntrySource.ZipFile)
                {
                    // nothing to do - the CRC is already set
                }
                else
                {
                    if (_source == ZipEntrySource.Stream)
                    {
                        PrepSourceStream();
                        input = _sourceStream;
                    }
                    else if (_source == ZipEntrySource.JitStream)
                    {
                        // allow the application to open the stream
                        if (_sourceStream == null)
                            _sourceStream = _OpenDelegate(FileName);
                        PrepSourceStream();
                        input = _sourceStream;
                    }
                    else if (_source == ZipEntrySource.ZipOutputStream)
                    {
                    }
                    else
                    {
                        //input = File.OpenRead(LocalFileName);
                        input = File.Open(LocalFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    }

                    var crc32 = new ZipCRC32();
                    _Crc32 = crc32.GetCrc32(input);

                    if (_sourceStream == null)
                    {
#if NETCF
                        input.Close();
#else
                        input.Dispose();
#endif
                    }
                }
                _crcCalculated = true;
            }
            return _Crc32;
        }

        private void PrepSourceStream()
        {
            if (_sourceStream == null)
                throw new ZipException(String.Format("The input stream is null for entry '{0}'.", FileName));

            if (_sourceStreamOriginalPosition != null)
            {
                // this will happen the 2nd cycle through, if the stream is seekable
                _sourceStream.Position = _sourceStreamOriginalPosition.Value;
            }
            else if (_sourceStream.CanSeek)
            {
                // this will happen the first cycle through, if seekable
                _sourceStreamOriginalPosition = _sourceStream.Position;
            }
            else if (Encryption == ZipEncryptionAlgorithm.PkzipWeak)
            {
                // In general, using PKZIP encryption on a a zip entry whose input
                // comes from a non-seekable stream, is tricky.  Here's why:
                //
                // Byte 11 of the PKZIP encryption header is used for password
                // validation and consistency checknig.
                //
                // Normally, the highest Byte of the CRC is used as the 11th (last) Byte
                // in the PKZIP encryption header. This means the CRC must be known
                // before encryption is performed. Normally that means we read the full
                // data stream, compute the CRC, then seek back and read it again for
                // the compression+encryption phase. Obviously this is bad for
                // performance with a large input file.
                //
                // There's a twist in the ZIP spec (actually documented only in infozip
                // code, not in the spec itself) that allows the high-order Byte of the
                // last modified time for the entry, when the lastmod time is in packed
                // (DOS) format, to be used for Byte 11 in the encryption header. In
                // this case, the bit 3 "data descriptor" must be used.
                //
                // An intelligent implementation would therefore force the use of the
                // bit 3 data descriptor when PKZIP encryption is in use, regardless.
                // This avoids the double-read of the stream to be encrypted.  So far,
                // DotNetZip doesn't do that; it just punts when the input stream is
                // non-seekable, and the output does not use Bit 3.
                //
                // The other option is to use the CRC when it is already available, eg,
                // when the source for the data is a ZipEntry (when the zip file is
                // being updated). In this case we already know the CRC and can just use
                // what we know.

                if (_source != ZipEntrySource.ZipFile && ((_BitField & 0x0008) != 0x0008))
                    throw new ZipException("It is not possible to use PKZIP encryption on a non-seekable input stream");
            }
        }

        internal void CopyMetaData(ZipEntry source)
        {
            __FileDataPosition = source.__FileDataPosition;
            CompressionMethod = source.CompressionMethod;
            _CompressionMethod_FromZipFile = source._CompressionMethod_FromZipFile;
            _CompressedFileDataSize = source._CompressedFileDataSize;
            _UncompressedSize = source._UncompressedSize;
            _BitField = source._BitField;
            _source = source._source;
            _LastModified = source._LastModified;
            _makeTime = source._makeTime;
            _actualTime = source._actualTime;
            _createTime = source._createTime;
            _ntfsTimesAreSet = source._ntfsTimesAreSet;
            _emitUnixTimes = source._emitUnixTimes;
            _emitNtfsTimes = source._emitNtfsTimes;
        }

        private void OnWriteBlock(Int64 BytesXferred, Int64 totalBytesToXfer)
        {
            if (_container.ZipFile != null)
                _ioOperationCanceled = _container.ZipFile.OnSaveBlock(this, BytesXferred, totalBytesToXfer);
        }

        private void _WriteEntryData(Stream s)
        {
            // Read in the data from the input stream (often a file in the filesystem),
            // and write it to the output stream, calculating a CRC on it as we go.
            // We will also compress and encrypt as necessary.

            Stream input = null;
            long fdp = -1L;
            try
            {
                // Want to record the position in the zip file of the zip entry
                // data (as opposed to the metadata).  s.Position may fail on some
                // write-only streams, eg stdout or System.Web.HttpResponseStream.
                // We swallow that exception, because we don't care, in that case.
                // But, don't set __FileDataPosition directly.  It may be needed
                // to READ the zip entry from the zip file, if this is a
                // "re-stream" situation. In other words if the zip entry has
                // changed compression level, or compression method, or (maybe?)
                // encryption algorithm.  In that case if the original entry is
                // encrypted, we need __FileDataPosition to be the value for the
                // input zip file.  This s.Position is for the output zipfile.  So
                // we copy fdp to __FileDataPosition after this entry has been
                // (maybe) restreamed.
                fdp = s.Position;
            }
            catch (Exception)
            {
            }

            try
            {
                // Use fileLength for progress updates, and to decide whether we can
                // skip encryption and compression altogether (in case of length==zero)
                long fileLength = SetInputAndFigureFileLength(ref input);

                // Wrap a counting stream around the raw output stream:
                // This is the last thing that happens before the bits go to the
                // application-provided stream.
                //
                // Sometimes s is a CountingStream. Doesn't matter. Wrap it with a
                // counter anyway. We need to count at both levels.

                ZipCountingStream entryCounter = new ZipCountingStream(s);

                Stream encryptor;
                Stream compressor;

                if (fileLength != 0L)
                {
                    // Maybe wrap an encrypting stream around the counter: This will
                    // happen BEFORE output counting, and AFTER compression, if encryption
                    // is used.
                    encryptor = MaybeApplyEncryption(entryCounter);

                    // Maybe wrap a compressing Stream around that.
                    // This will happen BEFORE encryption (if any) as we write data out.
                    compressor = MaybeApplyCompression(encryptor, fileLength);
                }
                else
                    encryptor = compressor = entryCounter;

                // Wrap a CrcCalculatorStream around that.
                // This will happen BEFORE compression (if any) as we write data out.
                var output = new ZipCrcCalculatorStream(compressor, true);

                // output.Write() causes this flow:
                // calc-crc -> compress -> encrypt -> count -> actually write

                if (_source == ZipEntrySource.WriteDelegate)
                {
                    // allow the application to write the data
                    _WriteDelegate(FileName, output);
                }
                else
                {
                    // synchronously copy the input stream to the output stream-chain
                    Byte[] buffer = new Byte[BufferSize];
                    int n;
                    while ((n = ZipSharedUtilities.ReadWithRetry(input, buffer, 0, buffer.Length, FileName)) != 0)
                    {
                        output.Write(buffer, 0, n);
                        OnWriteBlock(output.TotalBytesSlurped, fileLength);
                        if (_ioOperationCanceled)
                            break;
                    }
                }

                FinishOutputStream(s, entryCounter, encryptor, compressor, output);
            }
            finally
            {
                if (_source == ZipEntrySource.JitStream)
                {
                    // allow the application to close the stream
                    if (_CloseDelegate != null)
                        _CloseDelegate(FileName, input);
                }
                else if ((input as FileStream) != null)
                {
#if NETCF
                    input.Close();
#else
                    input.Dispose();
#endif
                }
            }

            if (_ioOperationCanceled)
                return;

            // set FDP now, to allow for re-streaming
            __FileDataPosition = fdp;
            PostProcessOutput(s);
        }

        private long SetInputAndFigureFileLength(ref Stream input)
        {
            long fileLength = -1L;
            // get the original stream:
            if (_source == ZipEntrySource.Stream)
            {
                PrepSourceStream();
                input = _sourceStream;

                // Try to get the length, no big deal if not available.
                try
                {
                    fileLength = _sourceStream.Length;
                }
                catch (NotSupportedException)
                {
                }
            }
            else if (_source == ZipEntrySource.ZipFile)
            {
                // we are "re-streaming" the zip entry.
                String pwd = (_Encryption_FromZipFile == ZipEncryptionAlgorithm.None) ? null : (_Password ?? _container.Password);
                _sourceStream = InternalOpenReader(pwd);
                PrepSourceStream();
                input = _sourceStream;
                fileLength = _sourceStream.Length;
            }
            else if (_source == ZipEntrySource.JitStream)
            {
                // allow the application to open the stream
                if (_sourceStream == null) _sourceStream = _OpenDelegate(FileName);
                PrepSourceStream();
                input = _sourceStream;
                try
                {
                    fileLength = _sourceStream.Length;
                }
                catch (NotSupportedException)
                {
                }
            }
            else if (_source == ZipEntrySource.FileSystem)
            {
                // workitem 7145
                FileShare fs = FileShare.ReadWrite;
#if !NETCF
                // FileShare.Delete is not defined for the Compact Framework
                fs |= FileShare.Delete;
#endif
                // workitem 8423
                input = File.Open(LocalFileName, FileMode.Open, FileAccess.Read, fs);
                fileLength = input.Length;
            }

            return fileLength;
        }

        internal void FinishOutputStream(Stream s, ZipCountingStream entryCounter, Stream encryptor, Stream compressor, ZipCrcCalculatorStream output)
        {
            if (output == null) return;

            output.Close();

            // by calling Close() on the deflate stream, we write the footer Bytes, as necessary.
            if ((compressor as ZipDeflateStream) != null)
                compressor.Close();
            else if ((compressor as ZipParallelDeflateOutputStream) != null)
                compressor.Close();

            encryptor.Flush();
            encryptor.Close();

            _LengthOfTrailer = 0;
            _UncompressedSize = output.TotalBytesSlurped;
            _CompressedFileDataSize = entryCounter.BytesWritten;
            _CompressedSize = _CompressedFileDataSize; // may be adjusted
            _Crc32 = output.Crc;

            // Set _RelativeOffsetOfLocalHeader now, to allow for re-streaming
            StoreRelativeOffset();
        }

        internal void PostProcessOutput(Stream s)
        {
            var s1 = s as ZipCountingStream;

            // workitem 8931 - for WriteDelegate.
            // The WriteDelegate changes things because there can be a zero-Byte stream
            // written. In all other cases DotNetZip knows the length of the stream
            // before compressing and encrypting. In this case we have to circle back,
            // and omit all the crypto stuff - the GP bitfield, and the crypto header.
            if (_UncompressedSize == 0 && _CompressedSize == 0)
            {
                if (_source == ZipEntrySource.ZipOutputStream) return; // nothing to do...

                if (_Password != null)
                {
                    int headerBytesToRetract = 0;
                    if (Encryption == ZipEncryptionAlgorithm.PkzipWeak)
                        headerBytesToRetract = 12;
                    if (_source == ZipEntrySource.ZipOutputStream && !s.CanSeek)
                        throw new ZipException("Zero Bytes written, encryption in use, and non-seekable output.");

                    if (Encryption != ZipEncryptionAlgorithm.None)
                    {
                        // seek back in the stream to un-output the security metadata
                        s.Seek(-1 * headerBytesToRetract, SeekOrigin.Current);
                        s.SetLength(s.Position);
                        // workitem 10178
                        ZipSharedUtilities.Workaround_Ladybug318918(s);

                        // workitem 11131
                        // adjust the count on the CountingStream as necessary
                        if (s1 != null) s1.Adjust(headerBytesToRetract);

                        // subtract the size of the security header from the _LengthOfHeader
                        _LengthOfHeader -= headerBytesToRetract;
                        __FileDataPosition -= headerBytesToRetract;
                    }
                    _Password = null;

                    // turn off the encryption bit
                    _BitField &= ~(0x0001);

                    // copy the updated bitfield value into the header
                    int j = 6;
                    _EntryHeader[j++] = (Byte)(_BitField & 0x00FF);
                    _EntryHeader[j++] = (Byte)((_BitField & 0xFF00) >> 8);
                }

                CompressionMethod = 0;
                Encryption = ZipEncryptionAlgorithm.None;
            }
            else if (_zipCrypto_forWrite != null)
            {
                if (Encryption == ZipEncryptionAlgorithm.PkzipWeak)
                    _CompressedSize += 12; // 12 extra Bytes for the encryption header
            }

            int i = 8;
            _EntryHeader[i++] = (Byte)(_CompressionMethod & 0x00FF);
            _EntryHeader[i++] = (Byte)((_CompressionMethod & 0xFF00) >> 8);

            i = 14;
            // CRC - the correct value now
            _EntryHeader[i++] = (Byte)(_Crc32 & 0x000000FF);
            _EntryHeader[i++] = (Byte)((_Crc32 & 0x0000FF00) >> 8);
            _EntryHeader[i++] = (Byte)((_Crc32 & 0x00FF0000) >> 16);
            _EntryHeader[i++] = (Byte)((_Crc32 & 0xFF000000) >> 24);

            SetZip64Flags();

            // (i==26) filename length (Int16)
            Int16 filenameLength = (short)(_EntryHeader[26] + _EntryHeader[27] * 256);
            Int16 extraFieldLength = (short)(_EntryHeader[28] + _EntryHeader[29] * 256);

            if (_OutputUsesZip64.Value)
            {
                // VersionNeededToExtract - set to 45 to indicate zip64
                _EntryHeader[4] = (45 & 0x00FF);
                _EntryHeader[5] = 0x00;

                // workitem 7924 - don't need bit 3
                // // workitem 7917
                // // set bit 3 for ZIP64 compatibility with WinZip12
                // _BitField |= 0x0008;
                // _EntryHeader[6] = (Byte)(_BitField & 0x00FF);

                // CompressedSize and UncompressedSize - 0xFF
                for (int j = 0; j < 8; j++)
                    _EntryHeader[i++] = 0xff;

                // At this point we need to find the "Extra field" that follows the
                // filename.  We had already emitted it, but the data (uncomp, comp,
                // ROLH) was not available at the time we did so.  Here, we emit it
                // again, with final values.

                i = 30 + filenameLength;
                _EntryHeader[i++] = 0x01; // zip64
                _EntryHeader[i++] = 0x00;

                i += 2; // skip over data size, which is 16+4

                Array.Copy(BitConverter.GetBytes(_UncompressedSize), 0, _EntryHeader, i, 8);
                i += 8;
                Array.Copy(BitConverter.GetBytes(_CompressedSize), 0, _EntryHeader, i, 8);
            }
            else
            {
                // VersionNeededToExtract - reset to 20 since no zip64
                _EntryHeader[4] = (20 & 0x00FF);
                _EntryHeader[5] = 0x00;

                // CompressedSize - the correct value now
                i = 18;
                _EntryHeader[i++] = (Byte)(_CompressedSize & 0x000000FF);
                _EntryHeader[i++] = (Byte)((_CompressedSize & 0x0000FF00) >> 8);
                _EntryHeader[i++] = (Byte)((_CompressedSize & 0x00FF0000) >> 16);
                _EntryHeader[i++] = (Byte)((_CompressedSize & 0xFF000000) >> 24);

                // UncompressedSize - the correct value now
                _EntryHeader[i++] = (Byte)(_UncompressedSize & 0x000000FF);
                _EntryHeader[i++] = (Byte)((_UncompressedSize & 0x0000FF00) >> 8);
                _EntryHeader[i++] = (Byte)((_UncompressedSize & 0x00FF0000) >> 16);
                _EntryHeader[i++] = (Byte)((_UncompressedSize & 0xFF000000) >> 24);

                // The HeaderId in the extra field header, is already dummied out.
                if (extraFieldLength != 0)
                {
                    i = 30 + filenameLength;
                    // For zip archives written by this library, if the zip64
                    // header exists, it is the first header. Because of the logic
                    // used when first writing the _EntryHeader Bytes, the
                    // HeaderId is not guaranteed to be any particular value.  So
                    // we determine if the first header is a putative zip64 header
                    // by examining the datasize.  UInt16 HeaderId =
                    // (UInt16)(_EntryHeader[i] + _EntryHeader[i + 1] * 256);
                    Int16 DataSize = (short)(_EntryHeader[i + 2] + _EntryHeader[i + 3] * 256);
                    if (DataSize == 16)
                    {
                        // reset to Header Id to dummy value, effectively dummy-ing out the zip64 metadata
                        _EntryHeader[i++] = 0x99;
                        _EntryHeader[i++] = 0x99;
                    }
                }
            }

#if AESCRYPTO

            if (Encryption == ZipEncryptionAlgorithm.WinZipAes128 ||
                Encryption == ZipEncryptionAlgorithm.WinZipAes256)
            {
                // Must set compressionmethod to 0x0063 (decimal 99)
                //
                // and then set the compression method Bytes inside the extra
                // field to the actual compression method value.

                i = 8;
                _EntryHeader[i++] = 0x63;
                _EntryHeader[i++] = 0;

                i = 30 + filenameLength;
                do
                {
                    UInt16 HeaderId = (UInt16)(_EntryHeader[i] + _EntryHeader[i + 1] * 256);
                    Int16 DataSize = (short)(_EntryHeader[i + 2] + _EntryHeader[i + 3] * 256);
                    if (HeaderId != 0x9901)
                    {
                        // skip this header
                        i += DataSize + 4;
                    }
                    else
                    {
                        i += 9;
                        // actual compression method
                        _EntryHeader[i++] = (Byte)(_CompressionMethod & 0x00FF);
                        _EntryHeader[i++] = (Byte)(_CompressionMethod & 0xFF00);
                    }
                } while (i < (extraFieldLength - 30 - filenameLength));
            }
#endif

            // finally, write the data.

            // workitem 7216 - sometimes we don't seek even if we CAN.  ASP.NET
            // Response.OutputStream, or stdout are non-seekable.  But we may also want
            // to NOT seek in other cases, eg zip64.  For all cases, we just check bit 3
            // to see if we want to seek.  There's one exception - if using a
            // ZipOutputStream, and PKZip encryption is in use, then we set bit 3 even
            // if the out is seekable. This is so the check on the last Byte of the
            // PKZip Encryption Header can be done on the current time, as opposed to
            // the CRC, to prevent streaming the file twice.  So, test for
            // ZipOutputStream and seekable, and if so, seek back, even if bit 3 is set.

            if ((_BitField & 0x0008) != 0x0008 ||
                (_source == ZipEntrySource.ZipOutputStream && s.CanSeek))
            {
                // seek back and rewrite the entry header
                var zss = s as ZipSegmentedStream;
                if (zss != null && _diskNumber != zss.CurrentSegment)
                {
                    // In this case the entry header is in a different file,
                    // which has already been closed. Need to re-open it.
                    using (Stream hseg = ZipSegmentedStream.ForUpdate(_container.ZipFile.Name, _diskNumber))
                    {
                        hseg.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
                        hseg.Write(_EntryHeader, 0, _EntryHeader.Length);
                    }
                }
                else
                {
                    // seek in the raw output stream, to the beginning of the header for
                    // this entry.
                    // workitem 8098: ok (output)
                    s.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);

                    // write the updated header to the output stream
                    s.Write(_EntryHeader, 0, _EntryHeader.Length);

                    // adjust the count on the CountingStream as necessary
                    if (s1 != null) s1.Adjust(_EntryHeader.Length);

                    // seek in the raw output stream, to the end of the file data
                    // for this entry
                    s.Seek(_CompressedSize, SeekOrigin.Current);
                }
            }

            // emit the descriptor - only if not a directory.
            if (((_BitField & 0x0008) == 0x0008) && !IsDirectory)
            {
                Byte[] Descriptor = new Byte[16 + (_OutputUsesZip64.Value ? 8 : 0)];
                i = 0;

                // signature
                Array.Copy(BitConverter.GetBytes(ZipConstants.ZipEntryDataDescriptorSignature), 0, Descriptor, i, 4);
                i += 4;

                // CRC - the correct value now
                Array.Copy(BitConverter.GetBytes(_Crc32), 0, Descriptor, i, 4);
                i += 4;

                // workitem 7917
                if (_OutputUsesZip64.Value)
                {
                    // CompressedSize - the correct value now
                    Array.Copy(BitConverter.GetBytes(_CompressedSize), 0, Descriptor, i, 8);
                    i += 8;

                    // UncompressedSize - the correct value now
                    Array.Copy(BitConverter.GetBytes(_UncompressedSize), 0, Descriptor, i, 8);
                    i += 8;
                }
                else
                {
                    // CompressedSize - (lower 32 bits) the correct value now
                    Descriptor[i++] = (Byte)(_CompressedSize & 0x000000FF);
                    Descriptor[i++] = (Byte)((_CompressedSize & 0x0000FF00) >> 8);
                    Descriptor[i++] = (Byte)((_CompressedSize & 0x00FF0000) >> 16);
                    Descriptor[i++] = (Byte)((_CompressedSize & 0xFF000000) >> 24);

                    // UncompressedSize - (lower 32 bits) the correct value now
                    Descriptor[i++] = (Byte)(_UncompressedSize & 0x000000FF);
                    Descriptor[i++] = (Byte)((_UncompressedSize & 0x0000FF00) >> 8);
                    Descriptor[i++] = (Byte)((_UncompressedSize & 0x00FF0000) >> 16);
                    Descriptor[i++] = (Byte)((_UncompressedSize & 0xFF000000) >> 24);
                }

                // finally, write the trailing descriptor to the output stream
                s.Write(Descriptor, 0, Descriptor.Length);

                _LengthOfTrailer += Descriptor.Length;
            }
        }

        private void SetZip64Flags()
        {
            // zip64 housekeeping
            _entryRequiresZip64 = _CompressedSize >= 0xFFFFFFFF || _UncompressedSize >= 0xFFFFFFFF || _RelativeOffsetOfLocalHeader >= 0xFFFFFFFF;

            // validate the ZIP64 usage
            if (_container.Zip64 == Zip64Option.Never && _entryRequiresZip64.Value)
                throw new ZipException("Compressed or Uncompressed size, or offset exceeds the maximum value. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");

            _OutputUsesZip64 = _container.Zip64 == Zip64Option.Always || _entryRequiresZip64.Value;
        }

        internal void PrepOutputStream(Stream s, long streamLength, out ZipCountingStream outputCounter, out Stream encryptor, out Stream compressor, out ZipCrcCalculatorStream output)
        {
            TraceWriteLine("PrepOutputStream: e({0}) comp({1}) crypto({2}) zf({3})",
                           FileName,
                           CompressionLevel,
                           Encryption,
                           _container.Name);

            // Wrap a counting stream around the raw output stream:
            // This is the last thing that happens before the bits go to the
            // application-provided stream.
            outputCounter = new ZipCountingStream(s);

            // Sometimes the incoming "raw" output stream is already a CountingStream.
            // Doesn't matter. Wrap it with a counter anyway. We need to count at both
            // levels.

            if (streamLength != 0L)
            {
                // Maybe wrap an encrypting stream around that:
                // This will happen BEFORE output counting, and AFTER deflation, if encryption
                // is used.
                encryptor = MaybeApplyEncryption(outputCounter);

                // Maybe wrap a compressing Stream around that.
                // This will happen BEFORE encryption (if any) as we write data out.
                compressor = MaybeApplyCompression(encryptor, streamLength);
            }
            else
                encryptor = compressor = outputCounter;
            // Wrap a CrcCalculatorStream around that.
            // This will happen BEFORE compression (if any) as we write data out.
            output = new ZipCrcCalculatorStream(compressor, true);
        }

        private Stream MaybeApplyCompression(Stream s, long streamLength)
        {
            if (_CompressionMethod == 0x08 && CompressionLevel != ZipCompressionLevel.None)
            {
                // ParallelDeflateThreshold == 0    means ALWAYS use parallel deflate
                // ParallelDeflateThreshold == -1L  means NEVER use parallel deflate
                // Other values specify the actual threshold.
                if (_container.ParallelDeflateThreshold == 0L ||
                    (streamLength > _container.ParallelDeflateThreshold &&
                     _container.ParallelDeflateThreshold > 0L))
                {
                    // This is sort of hacky.
                    //
                    // It's expensive to create a ParallelDeflateOutputStream, because
                    // of the large memory buffers.  But the class is unlike most Stream
                    // classes in that it can be re-used, so the caller can compress
                    // multiple files with it, one file at a time.  The key is to call
                    // Reset() on it, in between uses.
                    //
                    // The ParallelDeflateOutputStream is attached to the container
                    // itself - there is just one for the entire ZipFile or
                    // ZipOutputStream. So it gets created once, per save, and then
                    // re-used many times.
                    //
                    // This approach will break when we go to a "parallel save"
                    // approach, where multiple entries within the zip file are being
                    // compressed and saved at the same time.  But for now it's ok.
                    //

                    // instantiate the ParallelDeflateOutputStream
                    if (_container.ParallelDeflater == null)
                    {
                        _container.ParallelDeflater =
                            new ZipParallelDeflateOutputStream(s,
                                                               CompressionLevel,
                                                               _container.Strategy,
                                                               true);
                        // can set the codec buffer size only before the first call to Write().
                        if (_container.CodecBufferSize > 0)
                            _container.ParallelDeflater.BufferSize = _container.CodecBufferSize;
                        if (_container.ParallelDeflateMaxBufferPairs > 0)
                        {
                            _container.ParallelDeflater.MaxBufferPairs =
                                _container.ParallelDeflateMaxBufferPairs;
                        }
                    }
                    // reset it with the new stream
                    ZipParallelDeflateOutputStream o1 = _container.ParallelDeflater;
                    o1.Reset(s);
                    return o1;
                }

                var o = new ZipDeflateStream(s, ZipCompressionMode.Compress,
                                             CompressionLevel,
                                             true);
                if (_container.CodecBufferSize > 0)
                    o.BufferSize = _container.CodecBufferSize;
                o.Strategy = _container.Strategy;
                return o;
            }

            return s;
        }

        private Stream MaybeApplyEncryption(Stream s)
        {
            if (Encryption == ZipEncryptionAlgorithm.PkzipWeak)
            {
                TraceWriteLine("MaybeApplyEncryption: e({0}) PKZIP", FileName);

                return new ZipCipherStream(s, _zipCrypto_forWrite, ZipCryptoMode.Encrypt);
            }
            TraceWriteLine("MaybeApplyEncryption: e({0}) None", FileName);

            return s;
        }

        private void OnZipErrorWhileSaving(Exception e)
        {
            if (_container.ZipFile != null)
                _ioOperationCanceled = _container.ZipFile.OnZipErrorSaving(this, e);
        }

        internal void Write(Stream s)
        {
            var cs1 = s as ZipCountingStream;
            var zss1 = s as ZipSegmentedStream;

            Boolean done = false;
            do
            {
                try
                {
                    // When the app is updating a zip file, it may be possible to
                    // just copy data for a ZipEntry from the source zipfile to the
                    // destination, as a block, without decompressing and
                    // recompressing, etc.  But, in some cases the app modifies the
                    // properties on a ZipEntry prior to calling Save(). A change to
                    // any of the metadata - the FileName, CompressioLeve and so on,
                    // means DotNetZip cannot simply copy through the existing
                    // ZipEntry data unchanged.
                    //
                    // There are two cases:
                    //
                    //  1. Changes to only metadata, which means the header and
                    //     central directory must be changed.
                    //
                    //  2. Changes to the properties that affect the compressed
                    //     stream, such as CompressionMethod, CompressionLevel, or
                    //     EncryptionAlgorithm. In this case, DotNetZip must
                    //     "re-stream" the data: the old entry data must be maybe
                    //     decrypted, maybe decompressed, then maybe re-compressed
                    //     and maybe re-encrypted.
                    //
                    // This test checks if the source for the entry data is a zip file, and
                    // if a restream is necessary.  If NOT, then it just copies through
                    // one entry, potentially changing the metadata.

                    if (_source == ZipEntrySource.ZipFile && !_restreamRequiredOnSave)
                    {
                        CopyThroughOneEntry(s);
                        return;
                    }

                    // Is the entry a directory?  If so, the write is relatively simple.
                    if (IsDirectory)
                    {
                        WriteHeader(s, 1);
                        StoreRelativeOffset();
                        _entryRequiresZip64 = _RelativeOffsetOfLocalHeader >= 0xFFFFFFFF;
                        _OutputUsesZip64 = _container.Zip64 == Zip64Option.Always || _entryRequiresZip64.Value;
                        // handle case for split archives
                        if (zss1 != null)
                            _diskNumber = zss1.CurrentSegment;

                        return;
                    }

                    // At this point, the source for this entry is not a directory, and
                    // not a previously created zip file, or the source for the entry IS
                    // a previously created zip but the settings whave changed in
                    // important ways and therefore we will need to process the
                    // Bytestream (compute crc, maybe compress, maybe encrypt) in order
                    // to write the content into the new zip.
                    //
                    // We do this in potentially 2 passes: The first time we do it as
                    // requested, maybe with compression and maybe encryption.  If that
                    // causes the Bytestream to inflate in size, and if compression was
                    // on, then we turn off compression and do it again.

                    Boolean readAgain = true;
                    int nCycles = 0;
                    do
                    {
                        nCycles++;

                        WriteHeader(s, nCycles);

                        // write the encrypted header
                        WriteSecurityMetadata(s);

                        // write the (potentially compressed, potentially encrypted) file data
                        _WriteEntryData(s);

                        // track total entry size (including the trailing descriptor and MAC)
                        _TotalEntrySize = _LengthOfHeader + _CompressedFileDataSize + _LengthOfTrailer;

                        // The file data has now been written to the stream, and
                        // the file pointer is positioned directly after file data.

                        if (nCycles > 1) readAgain = false;
                        else if (!s.CanSeek) readAgain = false;
                        else readAgain = WantReadAgain();

                        if (readAgain)
                        {
                            // Seek back in the raw output stream, to the beginning of the file
                            // data for this entry.

                            // handle case for split archives
                            if (zss1 != null)
                            {
                                // Console.WriteLine("***_diskNumber/first: {0}", _diskNumber);
                                // Console.WriteLine("***_diskNumber/current: {0}", zss.CurrentSegment);
                                zss1.TruncateBackward(_diskNumber, _RelativeOffsetOfLocalHeader);
                            }
                            else
                                // workitem 8098: ok (output).
                                s.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);

                            // If the last entry expands, we read again; but here, we must
                            // truncate the stream to prevent garbage data after the
                            // end-of-central-directory.

                            // workitem 8098: ok (output).
                            s.SetLength(s.Position);

                            // Adjust the count on the CountingStream as necessary.
                            if (cs1 != null) cs1.Adjust(_TotalEntrySize);
                        }
                    } while (readAgain);
                    _skippedDuringSave = false;
                    done = true;
                }
                catch (System.Exception exc1)
                {
                    ZipErrorAction orig = ZipErrorAction;
                    int loop = 0;
                    do
                    {
                        if (ZipErrorAction == ZipErrorAction.Throw)
                            throw;

                        if (ZipErrorAction == ZipErrorAction.Skip ||
                            ZipErrorAction == ZipErrorAction.Retry)
                        {
                            // must reset file pointer here.
                            // workitem 13903 - seek back only when necessary
                            long p1 = (cs1 != null)
                                          ? cs1.ComputedPosition
                                          : s.Position;
                            long delta = p1 - _future_ROLH;
                            if (delta > 0)
                            {
                                s.Seek(delta, SeekOrigin.Current); // may throw
                                long p2 = s.Position;
                                s.SetLength(s.Position); // to prevent garbage if this is the last entry
                                if (cs1 != null) cs1.Adjust(p1 - p2);
                            }
                            if (ZipErrorAction == ZipErrorAction.Skip)
                            {
                                WriteStatus("Skipping file {0} (exception: {1})", LocalFileName, exc1.ToString());

                                _skippedDuringSave = true;
                                done = true;
                            }
                            else
                                ZipErrorAction = orig;
                            break;
                        }

                        if (loop > 0) throw;

                        if (ZipErrorAction == ZipErrorAction.InvokeErrorEvent)
                        {
                            OnZipErrorWhileSaving(exc1);
                            if (_ioOperationCanceled)
                            {
                                done = true;
                                break;
                            }
                        }
                        loop++;
                    } while (true);
                }
            } while (!done);
        }

        internal void StoreRelativeOffset()
        {
            _RelativeOffsetOfLocalHeader = _future_ROLH;
        }

        internal void NotifySaveComplete()
        {
            // When updating a zip file, there are two contexts for properties
            // like Encryption or CompressionMethod - the values read from the
            // original zip file, and the values used in the updated zip file.
            // The _FromZipFile versions are the originals.  At the end of a save,
            // these values are the same.  So we need to update them.  This takes
            // care of the boundary case where a single zipfile instance can be
            // saved multiple times, with distinct changes to the properties on
            // the entries, in between each Save().
            _Encryption_FromZipFile = _Encryption;
            _CompressionMethod_FromZipFile = _CompressionMethod;
            _restreamRequiredOnSave = false;
            _metadataChanged = false;
            //_Source = ZipEntrySource.None;
            _source = ZipEntrySource.ZipFile; // workitem 10694
        }

        internal void WriteSecurityMetadata(Stream outstream)
        {
            if (Encryption == ZipEncryptionAlgorithm.None)
                return;

            String pwd = _Password;

            // special handling for source == ZipFile.
            // Want to support the case where we re-stream an encrypted entry. This will involve,
            // at runtime, reading, decrypting, and decompressing from the original zip file, then
            // compressing, encrypting, and writing to the output zip file.

            // If that's what we're doing, and the password hasn't been set on the entry,
            // we use the container (ZipFile/ZipOutputStream) password to decrypt.
            // This test here says to use the container password to re-encrypt, as well,
            // with that password, if the entry password is null.

            if (_source == ZipEntrySource.ZipFile && pwd == null)
                pwd = _container.Password;

            if (pwd == null)
            {
                _zipCrypto_forWrite = null;
#if AESCRYPTO
                _aesCrypto_forWrite = null;
#endif
                return;
            }

            TraceWriteLine("WriteSecurityMetadata: e({0}) crypto({1}) pw({2})",
                           FileName, Encryption.ToString(), pwd);

            if (Encryption == ZipEncryptionAlgorithm.PkzipWeak)
            {
                // If PKZip (weak) encryption is in use, then the encrypted entry data
                // is preceded by 12-Byte "encryption header" for the entry.

                _zipCrypto_forWrite = ZipCrypto.ForWrite(pwd);

                // generate the random 12-Byte header:
                var rnd = new System.Random();
                Byte[] encryptionHeader = new Byte[12];
                rnd.NextBytes(encryptionHeader);

                // workitem 8271
                if ((_BitField & 0x0008) == 0x0008)
                {
                    // In the case that bit 3 of the general purpose bit flag is set to
                    // indicate the presence of a 'data descriptor' (signature
                    // 0x08074b50), the last Byte of the decrypted header is sometimes
                    // compared with the high-order Byte of the lastmodified time,
                    // rather than the high-order Byte of the CRC, to verify the
                    // password.
                    //
                    // This is not documented in the PKWare Appnote.txt.
                    // This was discovered this by analysis of the Crypt.c source file in the
                    // InfoZip library
                    // http://www.info-zip.org/pub/infozip/

                    // Also, winzip insists on this!
                    _TimeBlob = ZipSharedUtilities.DateTimeToPacked(LastModified);
                    encryptionHeader[11] = (Byte)((_TimeBlob >> 8) & 0xff);
                }
                else
                {
                    // When bit 3 is not set, the CRC value is required before
                    // encryption of the file data begins. In this case there is no way
                    // around it: must read the stream in its entirety to compute the
                    // actual CRC before proceeding.
                    FigureCrc32();
                    encryptionHeader[11] = (Byte)((_Crc32 >> 24) & 0xff);
                }

                // Encrypt the random header, INCLUDING the final Byte which is either
                // the high-order Byte of the CRC32, or the high-order Byte of the
                // _TimeBlob.  Must do this BEFORE encrypting the file data.  This
                // step changes the state of the cipher, or in the words of the PKZIP
                // spec, it "further initializes" the cipher keys.

                Byte[] cipherText = _zipCrypto_forWrite.EncryptMessage(encryptionHeader, encryptionHeader.Length);

                // Write the ciphered bonafide encryption header.
                outstream.Write(cipherText, 0, cipherText.Length);
                _LengthOfHeader += cipherText.Length; // 12 Bytes
            }

#if AESCRYPTO
            else if (Encryption == ZipEncryptionAlgorithm.WinZipAes128 ||
                Encryption == ZipEncryptionAlgorithm.WinZipAes256)
            {
                // If WinZip AES encryption is in use, then the encrypted entry data is
                // preceded by a variable-sized Salt and a 2-Byte "password
                // verification" value for the entry.

                int keystrength = GetKeyStrengthInBits(Encryption);
                _aesCrypto_forWrite = WinZipAesCrypto.Generate(pwd, keystrength);
                outstream.Write(_aesCrypto_forWrite.Salt, 0, _aesCrypto_forWrite._Salt.Length);
                outstream.Write(_aesCrypto_forWrite.GeneratedPV, 0, _aesCrypto_forWrite.GeneratedPV.Length);
                _LengthOfHeader += _aesCrypto_forWrite._Salt.Length + _aesCrypto_forWrite.GeneratedPV.Length;

                TraceWriteLine("WriteSecurityMetadata: AES e({0}) keybits({1}) _LOH({2})",
                               FileName, keystrength, _LengthOfHeader);

            }
#endif
        }

        private void CopyThroughOneEntry(Stream outStream)
        {
            // Just read the entry from the existing input zipfile and write to the output.
            // But, if metadata has changed (like file times or attributes), or if the ZIP64
            // option has changed, we can re-stream the entry data but must recompute the
            // metadata.
            if (LengthOfHeader == 0)
                throw new ZipBadStateException("Bad header length.");

            // is it necessary to re-constitute new metadata for this entry?
            Boolean needRecompute = _metadataChanged ||
                                    (ArchiveStream is ZipSegmentedStream) ||
                                    (outStream is ZipSegmentedStream) ||
                                    (_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Never) ||
                                    (!_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Always);

            if (needRecompute)
                CopyThroughWithRecompute(outStream);
            else
                CopyThroughWithNoChange(outStream);

            // zip64 housekeeping
            _entryRequiresZip64 = _CompressedSize >= 0xFFFFFFFF || _UncompressedSize >= 0xFFFFFFFF ||
                                  _RelativeOffsetOfLocalHeader >= 0xFFFFFFFF;

            _OutputUsesZip64 = _container.Zip64 == Zip64Option.Always || _entryRequiresZip64.Value;
        }

        private void CopyThroughWithRecompute(Stream outstream)
        {
            int n;
            Byte[] Bytes = new Byte[BufferSize];
            var input = new ZipCountingStream(ArchiveStream);

            long origRelativeOffsetOfHeader = _RelativeOffsetOfLocalHeader;

            // The header length may change due to rename of file, add a comment, etc.
            // We need to retain the original.
            int origLengthOfHeader = LengthOfHeader; // including crypto Bytes!

            // WriteHeader() has the side effect of changing _RelativeOffsetOfLocalHeader
            // and setting _LengthOfHeader.  While ReadHeader() reads the crypto header if
            // present, WriteHeader() does not write the crypto header.
            WriteHeader(outstream, 0);
            StoreRelativeOffset();

            if (!FileName.EndsWith("/"))
            {
                // Not a directory; there is file data.
                // Seek to the beginning of the entry data in the input stream.

                long pos = origRelativeOffsetOfHeader + origLengthOfHeader;
                int len = GetLengthOfCryptoHeaderBytes(_Encryption_FromZipFile);
                pos -= len; // want to keep the crypto header
                _LengthOfHeader += len;

                input.Seek(pos, SeekOrigin.Begin);

                // copy through everything after the header to the output stream
                long remaining = _CompressedSize;

                while (remaining > 0)
                {
                    len = (remaining > Bytes.Length) ? Bytes.Length : (int)remaining;

                    // read
                    n = input.Read(Bytes, 0, len);
                    //_CheckRead(n);

                    // write
                    outstream.Write(Bytes, 0, n);
                    remaining -= n;
                    OnWriteBlock(input.BytesRead, _CompressedSize);
                    if (_ioOperationCanceled)
                        break;
                }

                // bit 3 descriptor
                if ((_BitField & 0x0008) == 0x0008)
                {
                    int size = 16;
                    if (_InputUsesZip64) size += 8;
                    Byte[] Descriptor = new Byte[size];
                    input.Read(Descriptor, 0, size);

                    if (_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Never)
                    {
                        // original descriptor was 24 Bytes, now we need 16.
                        // Must check for underflow here.
                        // signature + CRC.
                        outstream.Write(Descriptor, 0, 8);

                        // Compressed
                        if (_CompressedSize > 0xFFFFFFFF)
                            throw new InvalidOperationException("ZIP64 is required");
                        outstream.Write(Descriptor, 8, 4);

                        // UnCompressed
                        if (_UncompressedSize > 0xFFFFFFFF)
                            throw new InvalidOperationException("ZIP64 is required");
                        outstream.Write(Descriptor, 16, 4);
                        _LengthOfTrailer -= 8;
                    }
                    else if (!_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Always)
                    {
                        // original descriptor was 16 Bytes, now we need 24
                        // signature + CRC
                        Byte[] pad = new Byte[4];
                        outstream.Write(Descriptor, 0, 8);
                        // Compressed
                        outstream.Write(Descriptor, 8, 4);
                        outstream.Write(pad, 0, 4);
                        // UnCompressed
                        outstream.Write(Descriptor, 12, 4);
                        outstream.Write(pad, 0, 4);
                        _LengthOfTrailer += 8;
                    }
                    else
                    {
                        // same descriptor on input and output. Copy it through.
                        outstream.Write(Descriptor, 0, size);
                        //_LengthOfTrailer += size;
                    }
                }
            }

            _TotalEntrySize = _LengthOfHeader + _CompressedFileDataSize + _LengthOfTrailer;
        }

        private void CopyThroughWithNoChange(Stream outstream)
        {
            int n;
            Byte[] Bytes = new Byte[BufferSize];
            var input = new ZipCountingStream(ArchiveStream);

            // seek to the beginning of the entry data in the input stream
            input.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);

            if (_TotalEntrySize == 0)
            {
                // We've never set the length of the entry.
                // Set it here.
                _TotalEntrySize = _LengthOfHeader + _CompressedFileDataSize + _LengthOfTrailer;

                // The CompressedSize includes all the leading metadata associated
                // to encryption, if any, as well as the compressed data, or
                // compressed-then-encrypted data, and the trailer in case of AES.

                // The CompressedFileData size is the same, less the encryption
                // framing data (12 Bytes header for PKZip; 10/18 Bytes header and
                // 10 Byte trailer for AES).

                // The _LengthOfHeader includes all the zip entry header plus the
                // crypto header, if any.  The _LengthOfTrailer includes the
                // 10-Byte MAC for AES, where appropriate, and the bit-3
                // Descriptor, where applicable.
            }

            // workitem 5616
            // remember the offset, within the output stream, of this particular entry header.
            // This may have changed if any of the other entries changed (eg, if a different
            // entry was removed or added.)
            var counter = outstream as ZipCountingStream;
            _RelativeOffsetOfLocalHeader = (counter != null)
                                               ? counter.ComputedPosition
                                               : outstream.Position; // BytesWritten

            // copy through the header, filedata, trailer, everything...
            long remaining = _TotalEntrySize;
            while (remaining > 0)
            {
                int len = (remaining > Bytes.Length) ? Bytes.Length : (int)remaining;

                // read
                n = input.Read(Bytes, 0, len);
                //_CheckRead(n);

                // write
                outstream.Write(Bytes, 0, n);
                remaining -= n;
                OnWriteBlock(input.BytesRead, _TotalEntrySize);
                if (_ioOperationCanceled)
                    break;
            }
        }

        [System.Diagnostics.ConditionalAttribute("Trace")]
        private void TraceWriteLine(String format, params object[] varParams)
        {
            lock (_outputLock)
            {
                int tid = System.Threading.Thread.CurrentThread.GetHashCode();
                Console.ForegroundColor = (ConsoleColor)(tid % 8 + 8);
                Console.Write("{0:000} ZipEntry.Write ", tid);
                Console.WriteLine(format, varParams);
                Console.ResetColor();
            }
        }

        #endregion

        #region Распаковка

        public void Extract()
        {
            InternalExtract(".", null, null);
        }

        public void Extract(ZipExtractExistingFileAction extractExistingFile)
        {
            ExtractExistingFile = extractExistingFile;
            InternalExtract(".", null, null);
        }

        public void Extract(Stream stream)
        {
            InternalExtract(null, stream, null);
        }

        public void Extract(String baseDirectory)
        {
            InternalExtract(baseDirectory, null, null);
        }

        public void Extract(String baseDirectory, ZipExtractExistingFileAction extractExistingFile)
        {
            ExtractExistingFile = extractExistingFile;
            InternalExtract(baseDirectory, null, null);
        }

        public void ExtractWithPassword(String password)
        {
            InternalExtract(".", null, password);
        }

        public void ExtractWithPassword(String baseDirectory, String password)
        {
            InternalExtract(baseDirectory, null, password);
        }

        public void ExtractWithPassword(ZipExtractExistingFileAction extractExistingFile, String password)
        {
            ExtractExistingFile = extractExistingFile;
            InternalExtract(".", null, password);
        }

        public void ExtractWithPassword(String baseDirectory, ZipExtractExistingFileAction extractExistingFile, String password)
        {
            ExtractExistingFile = extractExistingFile;
            InternalExtract(baseDirectory, null, password);
        }

        public void ExtractWithPassword(Stream stream, String password)
        {
            InternalExtract(null, stream, password);
        }

        public ZipCrcCalculatorStream OpenReader()
        {
            // workitem 10923
            if (_container.ZipFile == null)
                throw new InvalidOperationException("Use OpenReader() only with ZipFile.");

            // use the entry password if it is non-null,
            // else use the zipfile password, which is possibly null
            return InternalOpenReader(_Password ?? _container.Password);
        }

        public ZipCrcCalculatorStream OpenReader(String password)
        {
            // workitem 10923
            if (_container.ZipFile == null)
                throw new InvalidOperationException("Use OpenReader() only with ZipFile.");

            return InternalOpenReader(password);
        }

        internal ZipCrcCalculatorStream InternalOpenReader(String password)
        {
            ValidateCompression();
            ValidateEncryption();
            SetupCryptoForExtract(password);

            // workitem 7958
            if (_source != ZipEntrySource.ZipFile)
                throw new ZipBadStateException("You must call ZipFile.Save before calling OpenReader");

            // LeftToRead is a count of Bytes remaining to be read (out)
            // from the stream AFTER decompression and decryption.
            // It is the uncompressed size, unless ... there is no compression in which
            // case ...?  :< I'm not sure why it's not always UncompressedSize
            Int64 LeftToRead = (_CompressionMethod_FromZipFile == (short)ZipCompressionMethod.None)
                                   ? _CompressedFileDataSize
                                   : UncompressedSize;

            Stream input = ArchiveStream;

            ArchiveStream.Seek(FileDataPosition, SeekOrigin.Begin);
            // workitem 10178
            ZipSharedUtilities.Workaround_Ladybug318918(ArchiveStream);

            _inputDecryptorStream = GetExtractDecryptor(input);
            Stream input3 = GetExtractDecompressor(_inputDecryptorStream);

            return new ZipCrcCalculatorStream(input3, LeftToRead);
        }

        private void OnExtractProgress(Int64 BytesWritten, Int64 totalBytesToWrite)
        {
            if (_container.ZipFile != null)
                _ioOperationCanceled = _container.ZipFile.OnExtractBlock(this, BytesWritten, totalBytesToWrite);
        }

        private void OnBeforeExtract(String path)
        {
            // When in the context of a ZipFile.ExtractAll, the events are generated from
            // the ZipFile method, not from within the ZipEntry instance. (why?)
            // Therefore we suppress the events originating from the ZipEntry method.
            if (_container.ZipFile != null)
            {
                if (!_container.ZipFile._inExtractAll)
                    _ioOperationCanceled = _container.ZipFile.OnSingleEntryExtract(this, path, true);
            }
        }

        private void OnAfterExtract(String path)
        {
            // When in the context of a ZipFile.ExtractAll, the events are generated from
            // the ZipFile method, not from within the ZipEntry instance. (why?)
            // Therefore we suppress the events originating from the ZipEntry method.
            if (_container.ZipFile != null)
            {
                if (!_container.ZipFile._inExtractAll)
                    _container.ZipFile.OnSingleEntryExtract(this, path, false);
            }
        }

        private void OnExtractExisting(String path)
        {
            if (_container.ZipFile != null)
                _ioOperationCanceled = _container.ZipFile.OnExtractExisting(this, path);
        }

        private static void ReallyDelete(String fileName)
        {
            if ((File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                File.SetAttributes(fileName, FileAttributes.Normal);
            File.Delete(fileName);
        }

        private void WriteStatus(String format, params Object[] args)
        {
            if (_container.ZipFile != null && _container.ZipFile.Verbose) _container.ZipFile.StatusMessageTextWriter.WriteLine(format, args);
        }

        private void InternalExtract(String baseDir, Stream outstream, String password)
        {
            // workitem 7958
            if (_container == null)
                throw new ZipBadStateException("This entry is an orphan");

            // workitem 10355
            if (_container.ZipFile == null)
                throw new InvalidOperationException("Use Extract() only with ZipFile.");

            _container.ZipFile.Reset(false);

            if (_source != ZipEntrySource.ZipFile)
                throw new ZipBadStateException("You must call ZipFile.Save before calling any Extract method");

            OnBeforeExtract(baseDir);
            _ioOperationCanceled = false;
            String targetFileName = null;
            Stream output = null;
            Boolean fileExistsBeforeExtraction = false;
            Boolean checkLaterForResetDirTimes = false;
            try
            {
                ValidateCompression();
                ValidateEncryption();

                if (ValidateOutput(baseDir, outstream, out targetFileName))
                {
                    WriteStatus("extract dir {0}...", targetFileName);
                    // if true, then the entry was a directory and has been created.
                    // We need to fire the Extract Event.
                    OnAfterExtract(baseDir);
                    return;
                }

                // workitem 10639
                // do we want to extract to a regular filesystem file?
                if (targetFileName != null)
                {
                    // Check for extracting to a previously extant file. The user
                    // can specify bejavior for that case: overwrite, don't
                    // overwrite, and throw.  Also, if the file exists prior to
                    // extraction, it affects exception handling: whether to delete
                    // the target of extraction or not.  This check needs to be done
                    // before the password check is done, because password check may
                    // throw a BadPasswordException, which triggers the catch,
                    // wherein the extant file may be deleted if not flagged as
                    // pre-existing.
                    if (File.Exists(targetFileName))
                    {
                        fileExistsBeforeExtraction = true;
                        int rc = CheckExtractExistingFile(baseDir, targetFileName);
                        if (rc == 2) goto ExitTry; // cancel
                        if (rc == 1) return; // do not overwrite
                    }
                }

                // If no password explicitly specified, use the password on the entry itself,
                // or on the zipfile itself.
                String p = password ?? _Password ?? _container.Password;
                if (_Encryption_FromZipFile != ZipEncryptionAlgorithm.None)
                {
                    if (p == null)
                        throw new ZipBadPasswordException();
                    SetupCryptoForExtract(p);
                }

                // set up the output stream
                if (targetFileName != null)
                {
                    WriteStatus("extract file {0}...", targetFileName);
                    targetFileName += ".tmp";
                    var dirName = Path.GetDirectoryName(targetFileName);
                    // ensure the target path exists
                    if (Directory.Exists(dirName) == false)
                    {
                        // we create the directory here, but we do not set the
                        // create/modified/accessed times on it because it is being
                        // created implicitly, not explcitly. There's no entry in the
                        // zip archive for the directory.
                        Directory.CreateDirectory(dirName);
                    }
                    else
                    {
                        // workitem 8264
                        if (_container.ZipFile != null)
                            checkLaterForResetDirTimes = _container.ZipFile._inExtractAll;
                    }

                    // File.Create(CreateNew) will overwrite any existing file.
                    output = new FileStream(targetFileName, FileMode.CreateNew);
                }
                else
                {
                    WriteStatus("extract entry {0} to stream...", FileName);
                    output = outstream;
                }

                if (_ioOperationCanceled)
                    goto ExitTry;

                Int32 ActualCrc32 = ExtractOne(output);

                if (_ioOperationCanceled)
                    goto ExitTry;

                VerifyCrcAfterExtract(ActualCrc32);

                if (targetFileName != null)
                {
                    output.Close();
                    output = null;

                    // workitem 10639
                    // move file to permanent home
                    String tmpName = targetFileName;
                    String zombie = null;
                    targetFileName = tmpName.Substring(0, tmpName.Length - 4);

                    if (fileExistsBeforeExtraction)
                    {
                        // An AV program may hold the target file open, which means
                        // File.Delete() will succeed, though the actual deletion
                        // remains pending. This will prevent a subsequent
                        // File.Move() from succeeding. To avoid this, when the file
                        // already exists, we need to replace it in 3 steps:
                        //
                        //     1. rename the existing file to a zombie name;
                        //     2. rename the extracted file from the temp name to
                        //        the target file name;
                        //     3. delete the zombie.
                        //
                        zombie = targetFileName + ".PendingOverwrite";
                        File.Move(targetFileName, zombie);
                    }

                    File.Move(tmpName, targetFileName);
                    _SetTimes(targetFileName, true);

                    if (zombie != null && File.Exists(zombie))
                        ReallyDelete(zombie);

                    // workitem 8264
                    if (checkLaterForResetDirTimes)
                    {
                        // This is sort of a hack.  What I do here is set the time on
                        // the parent directory, every time a file is extracted into
                        // it.  If there is a directory with 1000 files, then I set
                        // the time on the dir, 1000 times. This allows the directory
                        // to have times that reflect the actual time on the entry in
                        // the zip archive.

                        // String.Contains is not available on .NET CF 2.0
                        if (FileName.IndexOf('/') != -1)
                        {
                            String dirname = Path.GetDirectoryName(FileName);
                            if (_container.ZipFile[dirname] == null)
                                _SetTimes(Path.GetDirectoryName(targetFileName), false);
                        }
                    }

#if NETCF
    // workitem 7926 - version made by OS can be zero or 10
                    if ((_versionMadeBy & 0xFF00) == 0x0a00 || (_versionMadeBy & 0xFF00) == 0x0000)
                        NetCfFile.SetAttributes(targetFileName, (uint)_ExternalFileAttrs);

#else
                    // workitem 7071
                    //
                    // We can only apply attributes if they are relevant to the NTFS
                    // OS.  Must do this LAST because it may involve a ReadOnly bit,
                    // which would prevent us from setting the time, etc.
                    //
                    // workitem 7926 - version made by OS can be zero (FAT) or 10
                    // (NTFS)
                    if ((_versionMadeBy & 0xFF00) == 0x0a00 || (_versionMadeBy & 0xFF00) == 0x0000)
                        File.SetAttributes(targetFileName, (FileAttributes)_ExternalFileAttrs);
#endif
                }

                OnAfterExtract(baseDir);

                ExitTry:
                ;
            }
            catch (Exception)
            {
                _ioOperationCanceled = true;
                throw;
            }
            finally
            {
                if (_ioOperationCanceled)
                {
                    if (targetFileName != null)
                    {
                        try
                        {
                            if (output != null) output.Close();
                            // An exception has occurred. If the file exists, check
                            // to see if it existed before we tried extracting.  If
                            // it did not, attempt to remove the target file. There
                            // is a small possibility that the existing file has
                            // been extracted successfully, overwriting a previously
                            // existing file, and an exception was thrown after that
                            // but before final completion (setting times, etc). In
                            // that case the file will remain, even though some
                            // error occurred.  Nothing to be done about it.
                            if (File.Exists(targetFileName) && !fileExistsBeforeExtraction)
                                File.Delete(targetFileName);
                        }
                        finally
                        {
                        }
                    }
                }
            }
        }

        internal void VerifyCrcAfterExtract(Int32 actualCrc32)
        {
            if (actualCrc32 != _Crc32)
            {
                throw new ZipBadCrcException("CRC error: the file being extracted appears to be corrupted. " +
                                             String.Format("Expected 0x{0:X8}, Actual 0x{1:X8}", _Crc32, actualCrc32));
            }
        }

        private int CheckExtractExistingFile(String baseDir, String targetFileName)
        {
            int loop = 0;
            // returns: 0 == extract, 1 = don't, 2 = cancel
            do
            {
                switch (ExtractExistingFile)
                {
                    case ZipExtractExistingFileAction.OverwriteSilently:
                        WriteStatus("the file {0} exists; will overwrite it...", targetFileName);
                        return 0;

                    case ZipExtractExistingFileAction.DoNotOverwrite:
                        WriteStatus("the file {0} exists; not extracting entry...", FileName);
                        OnAfterExtract(baseDir);
                        return 1;

                    case ZipExtractExistingFileAction.InvokeExtractProgressEvent:
                        if (loop > 0)
                            throw new ZipException(String.Format("The file {0} already exists.", targetFileName));
                        OnExtractExisting(baseDir);
                        if (_ioOperationCanceled)
                            return 2;

                        // loop around
                        break;

                    case ZipExtractExistingFileAction.Throw:
                    default:
                        throw new ZipException(String.Format("The file {0} already exists.", targetFileName));
                }
                loop++;
            } while (true);
        }

        private void _CheckRead(int nBytes)
        {
            if (nBytes == 0)
            {
                throw new ZipBadReadException(String.Format("bad read of entry {0} from compressed archive.",
                                                            FileName));
            }
        }

        private Int32 ExtractOne(Stream output)
        {
            Int32 CrcResult = 0;
            Stream input = ArchiveStream;

            try
            {
                // change for workitem 8098
                input.Seek(FileDataPosition, SeekOrigin.Begin);
                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(input);

                Byte[] Bytes = new Byte[BufferSize];

                // The extraction process varies depending on how the entry was
                // stored.  It could have been encrypted, and it coould have
                // been compressed, or both, or neither. So we need to check
                // both the encryption flag and the compression flag, and take
                // the proper action in all cases.

                Int64 LeftToRead = (_CompressionMethod_FromZipFile != (short)ZipCompressionMethod.None)
                                       ? UncompressedSize
                                       : _CompressedFileDataSize;

                // Get a stream that either decrypts or not.
                _inputDecryptorStream = GetExtractDecryptor(input);

                Stream input3 = GetExtractDecompressor(_inputDecryptorStream);

                Int64 BytesWritten = 0;
                // As we read, we maybe decrypt, and then we maybe decompress. Then we write.
                using (var s1 = new ZipCrcCalculatorStream(input3))
                {
                    while (LeftToRead > 0)
                    {
                        //Console.WriteLine("ExtractOne: LeftToRead {0}", LeftToRead);

                        // Casting LeftToRead down to an int is ok here in the else clause,
                        // because that only happens when it is less than Bytes.Length,
                        // which is much less than MAX_INT.
                        int len = (LeftToRead > Bytes.Length) ? Bytes.Length : (int)LeftToRead;
                        int n = s1.Read(Bytes, 0, len);

                        // must check data read - essential for detecting corrupt zip files
                        _CheckRead(n);

                        output.Write(Bytes, 0, n);
                        LeftToRead -= n;
                        BytesWritten += n;

                        // fire the progress event, check for cancels
                        OnExtractProgress(BytesWritten, UncompressedSize);
                        if (_ioOperationCanceled)
                            break;
                    }

                    CrcResult = s1.Crc;
                }
            }
            finally
            {
                var zss = input as ZipSegmentedStream;
                if (zss != null)
                {
#if NETCF
                    zss.Close();
#else
                    // need to dispose it
                    zss.Dispose();
#endif
                    _archiveStream = null;
                }
            }

            return CrcResult;
        }

        internal Stream GetExtractDecompressor(Stream input2)
        {
            switch (_CompressionMethod_FromZipFile)
            {
                case (short)ZipCompressionMethod.None:
                    return input2;
                case (short)ZipCompressionMethod.Deflate:
                    return new ZipDeflateStream(input2, ZipCompressionMode.Decompress, true);
            }

            return null;
        }

        internal Stream GetExtractDecryptor(Stream input)
        {
            Stream input2 = null;
            if (_Encryption_FromZipFile == ZipEncryptionAlgorithm.PkzipWeak)
                input2 = new ZipCipherStream(input, _zipCrypto_forExtract, ZipCryptoMode.Decrypt);
            else
                input2 = input;

            return input2;
        }

        internal void _SetTimes(String fileOrDirectory, Boolean isFile)
        {
            // workitem 8807:
            // Because setting the time is not considered to be a fatal error,
            // and because other applications can interfere with the setting
            // of a time on a directory, we're going to swallow IO exceptions
            // in this method.

            try
            {
                if (_ntfsTimesAreSet)
                {
                    if (isFile)
                    {
                        // It's possible that the extract was cancelled, in which case,
                        // the file does not exist.
                        if (File.Exists(fileOrDirectory))
                        {
                            File.SetCreationTimeUtc(fileOrDirectory, _createTime);
                            File.SetLastAccessTimeUtc(fileOrDirectory, _actualTime);
                            File.SetLastWriteTimeUtc(fileOrDirectory, _makeTime);
                        }
                    }
                    else
                    {
                        // It's possible that the extract was cancelled, in which case,
                        // the directory does not exist.
                        if (Directory.Exists(fileOrDirectory))
                        {
                            Directory.SetCreationTimeUtc(fileOrDirectory, _createTime);
                            Directory.SetLastAccessTimeUtc(fileOrDirectory, _actualTime);
                            Directory.SetLastWriteTimeUtc(fileOrDirectory, _makeTime);
                        }
                    }
                }
                else
                {
                    // workitem 6191
                    DateTime AdjustedLastModified = ZipSharedUtilities.AdjustTime_Reverse(LastModified);

                    if (isFile)
                        File.SetLastWriteTime(fileOrDirectory, AdjustedLastModified);
                    else
                        Directory.SetLastWriteTime(fileOrDirectory, AdjustedLastModified);
                }
            }
            catch (System.IO.IOException ioexc1)
            {
                WriteStatus("failed to set time on {0}: {1}", fileOrDirectory, ioexc1.Message);
            }
        }

        #endregion Распаковка

        #region Чтение

        private void ReadExtraField()
        {
            _readExtraDepth++;
            // workitem 8098: ok (restore)
            long posn = ArchiveStream.Position;
            ArchiveStream.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
            // workitem 10178
            ZipSharedUtilities.Workaround_Ladybug318918(ArchiveStream);

            Byte[] block = new Byte[30];
            ArchiveStream.Read(block, 0, block.Length);
            int i = 26;
            Int16 filenameLength = (short)(block[i++] + block[i++] * 256);
            Int16 extraFieldLength = (short)(block[i++] + block[i++] * 256);

            // workitem 8098: ok (relative)
            ArchiveStream.Seek(filenameLength, SeekOrigin.Current);
            // workitem 10178
            ZipSharedUtilities.Workaround_Ladybug318918(ArchiveStream);

            ProcessExtraField(ArchiveStream, extraFieldLength);

            // workitem 8098: ok (restore)
            ArchiveStream.Seek(posn, SeekOrigin.Begin);
            // workitem 10178
            ZipSharedUtilities.Workaround_Ladybug318918(ArchiveStream);
            _readExtraDepth--;
        }

        private static Boolean ReadHeader(ZipEntry ze, System.Text.Encoding defaultEncoding)
        {
            int BytesRead = 0;

            // change for workitem 8098
            ze._RelativeOffsetOfLocalHeader = ze.ArchiveStream.Position;

            int signature = ZipSharedUtilities.ReadEntrySignature(ze.ArchiveStream);
            BytesRead += 4;

            // Return false if this is not a local file header signature.
            if (ZipEntry.IsNotValidSig(signature))
            {
                // Getting "not a ZipEntry signature" is not always wrong or an error.
                // This will happen after the last entry in a zipfile.  In that case, we
                // expect to read :
                //    a ZipDirEntry signature (if a non-empty zip file) or
                //    a ZipConstants.EndOfCentralDirectorySignature.
                //
                // Anything else is a surprise.

                ze.ArchiveStream.Seek(-4, SeekOrigin.Current); // unread the signature
                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(ze.ArchiveStream);
                if (ZipEntry.IsNotValidZipDirEntrySig(signature) && (signature != ZipConstants.EndOfCentralDirectorySignature))
                    throw new ZipBadReadException(String.Format("  Bad signature (0x{0:X8}) at position  0x{1:X8}", signature, ze.ArchiveStream.Position));
                return false;
            }

            Byte[] block = new Byte[26];
            int n = ze.ArchiveStream.Read(block, 0, block.Length);
            if (n != block.Length) return false;
            BytesRead += n;

            int i = 0;
            ze._VersionNeeded = (Int16)(block[i++] + block[i++] * 256);
            ze._BitField = (Int16)(block[i++] + block[i++] * 256);
            ze._CompressionMethod_FromZipFile = ze._CompressionMethod = (Int16)(block[i++] + block[i++] * 256);
            ze._TimeBlob = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
            // transform the time data into something usable (a DateTime)
            ze._LastModified = ZipSharedUtilities.PackedToDateTime(ze._TimeBlob);
            ze._timestamp |= ZipEntryTimestamp.DOS;

            if ((ze._BitField & 0x01) == 0x01)
            {
                ze._Encryption_FromZipFile = ze._Encryption = ZipEncryptionAlgorithm.PkzipWeak; // this *may* change after processing the Extra field
                ze._sourceIsEncrypted = true;
            }

            // NB: if ((ze._BitField & 0x0008) != 0x0008), then the Compressed, uncompressed and
            // CRC values are not true values; the true values will follow the entry data.
            // But, regardless of the status of bit 3 in the bitfield, the slots for
            // the three amigos may contain marker values for ZIP64.  So we must read them.
            {
                ze._Crc32 = (block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                ze._CompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                ze._UncompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);

                if ((uint)ze._CompressedSize == 0xFFFFFFFF ||
                    (uint)ze._UncompressedSize == 0xFFFFFFFF)

                    ze._InputUsesZip64 = true;
            }

            Int16 filenameLength = (short)(block[i++] + block[i++] * 256);
            Int16 extraFieldLength = (short)(block[i++] + block[i++] * 256);

            block = new Byte[filenameLength];
            n = ze.ArchiveStream.Read(block, 0, block.Length);
            BytesRead += n;

            // if the UTF8 bit is set for this entry, override the
            // encoding the application requested.

            if ((ze._BitField & 0x0800) == 0x0800)
            {
                // workitem 12744
                ze.AlternateEncoding = System.Text.Encoding.UTF8;
                ze.AlternateEncodingUsage = ZipOption.Always;
            }

            // need to use this form of GetString() for .NET CF
            ze._FileNameInArchive = ze.AlternateEncoding.GetString(block, 0, block.Length);

            // workitem 6898
            if (ze._FileNameInArchive.EndsWith("/")) ze.MarkAsDirectory();

            BytesRead += ze.ProcessExtraField(ze.ArchiveStream, extraFieldLength);

            ze._LengthOfTrailer = 0;

            // workitem 6607 - don't read for directories
            // actually get the compressed size and CRC if necessary
            if (!ze._FileNameInArchive.EndsWith("/") && (ze._BitField & 0x0008) == 0x0008)
            {
                // This descriptor exists only if bit 3 of the general
                // purpose bit flag is set (see below).  It is Byte aligned
                // and immediately follows the last Byte of compressed data,
                // as well as any encryption trailer, as with AES.
                // This descriptor is used only when it was not possible to
                // seek in the output .ZIP file, e.g., when the output .ZIP file
                // was standard output or a non-seekable device.  For ZIP64(tm) format
                // archives, the compressed and uncompressed sizes are 8 Bytes each.

                // workitem 8098: ok (restore)
                long posn = ze.ArchiveStream.Position;

                // Here, we're going to loop until we find a ZipEntryDataDescriptorSignature and
                // a consistent data record after that.   To be consistent, the data record must
                // indicate the length of the entry data.
                Boolean wantMore = true;
                long SizeOfDataRead = 0;
                int tries = 0;
                while (wantMore)
                {
                    tries++;
                    // We call the FindSignature shared routine to find the specified signature
                    // in the already-opened zip archive, starting from the current cursor
                    // position in that filestream.  If we cannot find the signature, then the
                    // routine returns -1, and the ReadHeader() method returns false,
                    // indicating we cannot read a legal entry header.  If we have found it,
                    // then the FindSignature() method returns the number of Bytes in the
                    // stream we had to seek forward, to find the sig.  We need this to
                    // determine if the zip entry is valid, later.

                    if (ze._container.ZipFile != null)
                        ze._container.ZipFile.OnReadBytes(ze);

                    long d = ZipSharedUtilities.FindSignature(ze.ArchiveStream, ZipConstants.ZipEntryDataDescriptorSignature);
                    if (d == -1) return false;

                    // total size of data read (through all loops of this).
                    SizeOfDataRead += d;

                    if (ze._InputUsesZip64)
                    {
                        // read 1x 4-Byte (CRC) and 2x 8-Bytes (Compressed Size, Uncompressed Size)
                        block = new Byte[20];
                        n = ze.ArchiveStream.Read(block, 0, block.Length);
                        if (n != 20) return false;

                        // do not increment BytesRead - it is for entry header only.
                        // the data we have just read is a footer (falls after the file data)
                        //BytesRead += n;

                        i = 0;
                        ze._Crc32 = (block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                        ze._CompressedSize = BitConverter.ToInt64(block, i);
                        i += 8;
                        ze._UncompressedSize = BitConverter.ToInt64(block, i);
                        i += 8;

                        ze._LengthOfTrailer += 24; // Bytes including sig, CRC, Comp and Uncomp sizes
                    }
                    else
                    {
                        // read 3x 4-Byte fields (CRC, Compressed Size, Uncompressed Size)
                        block = new Byte[12];
                        n = ze.ArchiveStream.Read(block, 0, block.Length);
                        if (n != 12) return false;

                        // do not increment BytesRead - it is for entry header only.
                        // the data we have just read is a footer (falls after the file data)
                        //BytesRead += n;

                        i = 0;
                        ze._Crc32 = (block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                        ze._CompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                        ze._UncompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);

                        ze._LengthOfTrailer += 16; // Bytes including sig, CRC, Comp and Uncomp sizes
                    }

                    wantMore = (SizeOfDataRead != ze._CompressedSize);

                    if (wantMore)
                    {
                        // Seek back to un-read the last 12 Bytes  - maybe THEY contain
                        // the ZipEntryDataDescriptorSignature.
                        // (12 Bytes for the CRC, Comp and Uncomp size.)
                        ze.ArchiveStream.Seek(-12, SeekOrigin.Current);
                        // workitem 10178
                        ZipSharedUtilities.Workaround_Ladybug318918(ze.ArchiveStream);

                        // Adjust the size to account for the false signature read in
                        // FindSignature().
                        SizeOfDataRead += 4;
                    }
                }

                // seek back to previous position, to prepare to read file data
                // workitem 8098: ok (restore)
                ze.ArchiveStream.Seek(posn, SeekOrigin.Begin);
                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(ze.ArchiveStream);
            }

            ze._CompressedFileDataSize = ze._CompressedSize;

            // bit 0 set indicates that some kind of encryption is in use
            if ((ze._BitField & 0x01) == 0x01)
            {
                // read in the header data for "weak" encryption
                ze._WeakEncryptionHeader = new Byte[12];
                BytesRead += ZipEntry.ReadWeakEncryptionHeader(ze._archiveStream, ze._WeakEncryptionHeader);
                // decrease the filedata size by 12 Bytes
                ze._CompressedFileDataSize -= 12;
            }

            // Remember the size of the blob for this entry.
            // We also have the starting position in the stream for this entry.
            ze._LengthOfHeader = BytesRead;
            ze._TotalEntrySize = ze._LengthOfHeader + ze._CompressedFileDataSize + ze._LengthOfTrailer;

            // We've read in the regular entry header, the extra field, and any
            // encryption header.  The pointer in the file is now at the start of the
            // filedata, which is potentially compressed and encrypted.  Just ahead in
            // the file, there are _CompressedFileDataSize Bytes of data, followed by
            // potentially a non-zero length trailer, consisting of optionally, some
            // encryption stuff (10 Byte MAC for AES), and the bit-3 trailer (16 or 24
            // Bytes).

            return true;
        }

        internal static int ReadWeakEncryptionHeader(Stream s, Byte[] buffer)
        {
            // PKZIP encrypts the compressed data stream.  Encrypted files must
            // be decrypted before they can be extracted.

            // Each PKZIP-encrypted file has an extra 12 Bytes stored at the start of the data
            // area defining the encryption header for that file.  The encryption header is
            // originally set to random values, and then itself encrypted, using three, 32-bit
            // keys.  The key values are initialized using the supplied encryption password.
            // After each Byte is encrypted, the keys are then updated using pseudo-random
            // number generation techniques in combination with the same CRC-32 algorithm used
            // in PKZIP and implemented in the CRC32.cs module in this project.

            // read the 12-Byte encryption header
            int additionalBytesRead = s.Read(buffer, 0, 12);
            if (additionalBytesRead != 12)
                throw new ZipException(String.Format("Unexpected end of data at position 0x{0:X8}", s.Position));

            return additionalBytesRead;
        }

        private static Boolean IsNotValidSig(int signature)
        {
            return (signature != ZipConstants.ZipEntrySignature);
        }

        internal static ZipEntry ReadEntry(ZipContainer zc, Boolean first)
        {
            ZipFile zf = zc.ZipFile;
            Stream s = zc.ReadStream;
            System.Text.Encoding defaultEncoding = zc.AlternateEncoding;
            ZipEntry entry = new ZipEntry();
            entry._source = ZipEntrySource.ZipFile;
            entry._container = zc;
            entry._archiveStream = s;
            if (zf != null)
                zf.OnReadEntry(true, null);

            if (first) HandlePK00Prefix(s);

            // Read entry header, including any encryption header
            if (!ReadHeader(entry, defaultEncoding)) return null;

            // Store the position in the stream for this entry
            // change for workitem 8098
            entry.__FileDataPosition = entry.ArchiveStream.Position;

            // seek past the data without reading it. We will read on Extract()
            s.Seek(entry._CompressedFileDataSize + entry._LengthOfTrailer, SeekOrigin.Current);
            // workitem 10178
            ZipSharedUtilities.Workaround_Ladybug318918(s);

            // ReadHeader moves the file pointer to the end of the entry header,
            // as well as any encryption header.

            // CompressedFileDataSize includes:
            //   the maybe compressed, maybe encrypted file data
            //   the encryption trailer, if any
            //   the bit 3 descriptor, if any

            // workitem 5306
            // http://www.codeplex.com/DotNetZip/WorkItem/View.aspx?WorkItemId=5306
            HandleUnexpectedDataDescriptor(entry);

            if (zf != null)
            {
                zf.OnReadBytes(entry);
                zf.OnReadEntry(false, entry);
            }

            return entry;
        }

        internal static void HandlePK00Prefix(Stream s)
        {
            // in some cases, the zip file begins with "PK00".  This is a throwback and is rare,
            // but we handle it anyway. We do not change behavior based on it.
            uint datum = (uint)ZipSharedUtilities.ReadInt(s);
            if (datum != ZipConstants.PackedToRemovableMedia)
            {
                s.Seek(-4, SeekOrigin.Current); // unread the block
                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(s);
            }
        }

        private static void HandleUnexpectedDataDescriptor(ZipEntry entry)
        {
            Stream s = entry.ArchiveStream;

            // In some cases, the "data descriptor" is present, without a signature, even when
            // bit 3 of the BitField is NOT SET.  This is the CRC, followed
            //    by the compressed length and the uncompressed length (4 Bytes for each
            //    of those three elements).  Need to check that here.
            //
            uint datum = (uint)ZipSharedUtilities.ReadInt(s);
            if (datum == entry._Crc32)
            {
                int sz = ZipSharedUtilities.ReadInt(s);
                if (sz == entry._CompressedSize)
                {
                    sz = ZipSharedUtilities.ReadInt(s);
                    if (sz == entry._UncompressedSize)
                    {
                        // ignore everything and discard it.
                    }
                    else
                    {
                        s.Seek(-12, SeekOrigin.Current); // unread the three blocks

                        // workitem 10178
                        ZipSharedUtilities.Workaround_Ladybug318918(s);
                    }
                }
                else
                {
                    s.Seek(-8, SeekOrigin.Current); // unread the two blocks

                    // workitem 10178
                    ZipSharedUtilities.Workaround_Ladybug318918(s);
                }
            }
            else
            {
                s.Seek(-4, SeekOrigin.Current); // unread the block

                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(s);
            }
        }

        internal static int FindExtraFieldSegment(Byte[] extra, int offx, UInt16 targetHeaderId)
        {
            int j = offx;
            while (j + 3 < extra.Length)
            {
                UInt16 headerId = (UInt16)(extra[j++] + extra[j++] * 256);
                if (headerId == targetHeaderId) return j - 2;

                // else advance to next segment
                Int16 dataSize = (short)(extra[j++] + extra[j++] * 256);
                j += dataSize;
            }

            return -1;
        }

        internal int ProcessExtraField(Stream s, Int16 extraFieldLength)
        {
            int additionalBytesRead = 0;
            if (extraFieldLength > 0)
            {
                Byte[] buffer = _Extra = new Byte[extraFieldLength];
                additionalBytesRead = s.Read(buffer, 0, buffer.Length);
                long posn = s.Position - additionalBytesRead;
                int j = 0;
                while (j + 3 < buffer.Length)
                {
                    int start = j;
                    UInt16 headerId = (UInt16)(buffer[j++] + buffer[j++] * 256);
                    Int16 dataSize = (short)(buffer[j++] + buffer[j++] * 256);

                    switch (headerId)
                    {
                        case 0x000a: // NTFS ctime, atime, mtime
                            j = ProcessExtraFieldWindowsTimes(buffer, j, dataSize, posn);
                            break;

                        case 0x5455: // Unix ctime, atime, mtime
                            j = ProcessExtraFieldUnixTimes(buffer, j, dataSize, posn);
                            break;

                        case 0x5855: // Info-zip Extra field (outdated)
                            // This is outdated, so the field is supported on
                            // read only.
                            j = ProcessExtraFieldInfoZipTimes(buffer, j, dataSize, posn);
                            break;

                        case 0x7855: // Unix uid/gid
                            // ignored. DotNetZip does not handle this field.
                            break;

                        case 0x7875: // ??
                            // ignored.  I could not find documentation on this field,
                            // though it appears in some zip files.
                            break;

                        case 0x0001: // ZIP64
                            j = ProcessExtraFieldZip64(buffer, j, dataSize, posn);
                            break;

                        case 0x0017: // workitem 7968: handle PKWare Strong encryption header
                            j = ProcessExtraFieldPkwareStrongEncryption(buffer, j);
                            break;
                    }

                    // move to the next Header in the extra field
                    j = start + dataSize + 4;
                }
            }
            return additionalBytesRead;
        }

        private int ProcessExtraFieldPkwareStrongEncryption(Byte[] Buffer, int j)
        {
            //           Value     Size     Description
            //           -----     ----     -----------
            //           0x0017    2 Bytes  Tag for this "extra" block type
            //           TSize     2 Bytes  Size of data that follows
            //           Format    2 Bytes  Format definition for this record
            //           AlgID     2 Bytes  Encryption algorithm identifier
            //           Bitlen    2 Bytes  Bit length of encryption key
            //           Flags     2 Bytes  Processing flags
            //           CertData  TSize-8  Certificate decryption extra field data
            //                              (refer to the explanation for CertData
            //                               in the section describing the
            //                               Certificate Processing Method under
            //                               the Strong Encryption Specification)

            j += 2;
            _UnsupportedAlgorithmId = (UInt16)(Buffer[j++] + Buffer[j++] * 256);
            _Encryption_FromZipFile = _Encryption = ZipEncryptionAlgorithm.Unsupported;

            // DotNetZip doesn't support this algorithm, but we don't need to throw
            // here.  we might just be reading the archive, which is fine.  We'll
            // need to throw if Extract() is called.

            return j;
        }

        private int ProcessExtraFieldZip64(Byte[] buffer, int j, Int16 dataSize, long posn)
        {
            // The PKWare spec says that any of {UncompressedSize, CompressedSize,
            // RelativeOffset} exceeding 0xFFFFFFFF can lead to the ZIP64 header,
            // and the ZIP64 header may contain one or more of those.  If the
            // values are present, they will be found in the prescribed order.
            // There may also be a 4-Byte "disk start number."
            // This means that the DataSize must be 28 Bytes or less.

            _InputUsesZip64 = true;

            // workitem 7941: check datasize before reading.
            if (dataSize > 28)
            {
                throw new ZipBadReadException(String.Format("  Inconsistent size (0x{0:X4}) for ZIP64 extra field at position 0x{1:X16}",
                                                            dataSize, posn));
            }
            int remainingData = dataSize;

            var slurp = new Func<Int64>(() =>
            {
                if (remainingData < 8)
                    throw new ZipBadReadException(String.Format("  Missing data for ZIP64 extra field, position 0x{0:X16}", posn));
                var x = BitConverter.ToInt64(buffer, j);
                j += 8;
                remainingData -= 8;
                return x;
            });

            if (_UncompressedSize == 0xFFFFFFFF)
                _UncompressedSize = slurp();

            if (_CompressedSize == 0xFFFFFFFF)
                _CompressedSize = slurp();

            if (_RelativeOffsetOfLocalHeader == 0xFFFFFFFF)
                _RelativeOffsetOfLocalHeader = slurp();

            // Ignore anything else. Potentially there are 4 more Bytes for the
            // disk start number.  DotNetZip currently doesn't handle multi-disk
            // archives.
            return j;
        }

        private int ProcessExtraFieldInfoZipTimes(Byte[] buffer, int j, Int16 dataSize, long posn)
        {
            if (dataSize != 12 && dataSize != 8)
                throw new ZipBadReadException(String.Format("  Unexpected size (0x{0:X4}) for InfoZip v1 extra field at position 0x{1:X16}", dataSize, posn));

            Int32 timet = BitConverter.ToInt32(buffer, j);
            _makeTime = _unixEpoch.AddSeconds(timet);
            j += 4;

            timet = BitConverter.ToInt32(buffer, j);
            _actualTime = _unixEpoch.AddSeconds(timet);
            j += 4;

            _createTime = DateTime.UtcNow;

            _ntfsTimesAreSet = true;
            _timestamp |= ZipEntryTimestamp.InfoZip1;
            return j;
        }

        private int ProcessExtraFieldUnixTimes(Byte[] buffer, int j, Int16 dataSize, long posn)
        {
            // The Unix filetimes are 32-bit unsigned integers,
            // storing seconds since Unix epoch.

            if (dataSize != 13 && dataSize != 9 && dataSize != 5)
                throw new ZipBadReadException(String.Format("  Unexpected size (0x{0:X4}) for Extended Timestamp extra field at position 0x{1:X16}", dataSize, posn));

            int remainingData = dataSize;

            var slurp = new Func<DateTime>(() =>
            {
                Int32 timet = BitConverter.ToInt32(buffer, j);
                j += 4;
                remainingData -= 4;
                return _unixEpoch.AddSeconds(timet);
            });

            if (dataSize == 13 || _readExtraDepth > 0)
            {
                Byte flag = buffer[j++];
                remainingData--;

                if ((flag & 0x0001) != 0 && remainingData >= 4)
                    _makeTime = slurp();

                _actualTime = ((flag & 0x0002) != 0 && remainingData >= 4)
                                  ? slurp()
                                  : DateTime.UtcNow;

                _createTime = ((flag & 0x0004) != 0 && remainingData >= 4)
                                  ? slurp()
                                  : DateTime.UtcNow;

                _timestamp |= ZipEntryTimestamp.Unix;
                _ntfsTimesAreSet = true;
                _emitUnixTimes = true;
            }
            else
                ReadExtraField(); // will recurse

            return j;
        }

        private int ProcessExtraFieldWindowsTimes(Byte[] buffer, int j, Int16 dataSize, long posn)
        {
            // The NTFS filetimes are 64-bit unsigned integers, stored in Intel
            // (least significant Byte first) Byte order. They are expressed as the
            // number of 1.0E-07 seconds (1/10th microseconds!) past WinNT "epoch",
            // which is "01-Jan-1601 00:00:00 UTC".
            //
            // HeaderId   2 Bytes    0x000a == NTFS stuff
            // Datasize   2 Bytes    ?? (usually 32)
            // reserved   4 Bytes    ??
            // timetag    2 Bytes    0x0001 == time
            // size       2 Bytes    24 == 8 Bytes each for ctime, mtime, atime
            // mtime      8 Bytes    win32 ticks since win32epoch
            // atime      8 Bytes    win32 ticks since win32epoch
            // ctime      8 Bytes    win32 ticks since win32epoch

            if (dataSize != 32)
                throw new ZipBadReadException(String.Format("  Unexpected size (0x{0:X4}) for NTFS times extra field at position 0x{1:X16}", dataSize, posn));

            j += 4; // reserved
            Int16 timetag = (Int16)(buffer[j] + buffer[j + 1] * 256);
            Int16 addlsize = (Int16)(buffer[j + 2] + buffer[j + 3] * 256);
            j += 4; // tag and size

            if (timetag == 0x0001 && addlsize == 24)
            {
                Int64 z = BitConverter.ToInt64(buffer, j);
                _makeTime = DateTime.FromFileTimeUtc(z);
                j += 8;

                // At this point the library *could* set the LastModified value
                // to coincide with the Mtime value.  In theory, they refer to
                // the same property of the file, and should be the same anyway,
                // allowing for differences in precision.  But they are
                // independent quantities in the zip archive, and this library
                // will keep them separate in the object model. There is no ill
                // effect from this, because as files are extracted, the
                // higher-precision value (Mtime) is used if it is present.
                // Apps may wish to compare the Mtime versus LastModified
                // values, but any difference when both are present is not
                // germaine to the correctness of the library. but note: when
                // explicitly setting either value, both are set. See the setter
                // for LastModified or the SetNtfsTimes() method.

                z = BitConverter.ToInt64(buffer, j);
                _actualTime = DateTime.FromFileTimeUtc(z);
                j += 8;

                z = BitConverter.ToInt64(buffer, j);
                _createTime = DateTime.FromFileTimeUtc(z);
                j += 8;

                _ntfsTimesAreSet = true;
                _timestamp |= ZipEntryTimestamp.Windows;
                _emitNtfsTimes = true;
            }
            return j;
        }

        #endregion Чтение

        #region Проверки

        internal void ValidateEncryption()
        {
            if (Encryption != ZipEncryptionAlgorithm.PkzipWeak && Encryption != ZipEncryptionAlgorithm.None)
            {
                if (_UnsupportedAlgorithmId != 0)
                {
                    throw new ZipException(String.Format("Cannot extract: Entry {0} is encrypted with an algorithm not supported: {1}",
                                                         FileName, UnsupportedAlgorithm));
                }
                else
                {
                    throw new ZipException(String.Format("Cannot extract: Entry {0} uses an unsupported encryption algorithm ({1:X2})",
                                                         FileName, (int)Encryption));
                }
            }
        }

        private void ValidateCompression()
        {
            if ((_CompressionMethod_FromZipFile != (short)ZipCompressionMethod.None) &&
                (_CompressionMethod_FromZipFile != (short)ZipCompressionMethod.Deflate))
            {
                throw new ZipException(String.Format("Entry {0} uses an unsupported compression method (0x{1:X2}, {2})",
                                                     FileName, _CompressionMethod_FromZipFile, UnsupportedCompressionMethod));
            }
        }

        private void SetupCryptoForExtract(String password)
        {
            //if (password == null) return;
            if (_Encryption_FromZipFile == ZipEncryptionAlgorithm.None) return;

            if (_Encryption_FromZipFile == ZipEncryptionAlgorithm.PkzipWeak)
            {
                if (password == null)
                    throw new ZipException("Missing password.");

                ArchiveStream.Seek(FileDataPosition - 12, SeekOrigin.Begin);
                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(ArchiveStream);
                _zipCrypto_forExtract = ZipCrypto.ForRead(password, this);
            }
        }

        private Boolean ValidateOutput(String basedir, Stream outstream, out String outFileName)
        {
            if (basedir != null)
            {
                // Sometimes the name on the entry starts with a slash.
                // Rather than unpack to the root of the volume, we're going to
                // drop the slash and unpack to the specified base directory.
                String f = FileName.Replace("\\", "/");

                // workitem 11772: remove drive letter with separator
                if (f.IndexOf(':') == 1)
                    f = f.Substring(2);

                if (f.StartsWith("/"))
                    f = f.Substring(1);

                // String.Contains is not available on .NET CF 2.0

                if (_container.ZipFile.FlattenFoldersOnExtract)
                {
                    outFileName = Path.Combine(basedir,
                                               (f.IndexOf('/') != -1) ? Path.GetFileName(f) : f);
                }
                else
                    outFileName = Path.Combine(basedir, f);

                // workitem 10639
                outFileName = outFileName.Replace("/", "\\");

                // check if it is a directory
                if ((IsDirectory) || (FileName.EndsWith("/")))
                {
                    if (!Directory.Exists(outFileName))
                    {
                        Directory.CreateDirectory(outFileName);
                        _SetTimes(outFileName, false);
                    }
                    else
                    {
                        // the dir exists, maybe we want to overwrite times.
                        if (ExtractExistingFile == ZipExtractExistingFileAction.OverwriteSilently)
                            _SetTimes(outFileName, false);
                    }
                    return true; // true == all done, caller will return
                }
                return false; // false == work to do by caller.
            }

            if (outstream != null)
            {
                outFileName = null;
                if ((IsDirectory) || (FileName.EndsWith("/")))
                {
                    // extract a directory to streamwriter?  nothing to do!
                    return true; // true == all done!  caller can return
                }
                return false;
            }

            throw new ArgumentNullException("outstream");
        }

        #endregion Проверки

        #region Класс CopyHelper

        private class CopyHelper
        {
            #region Поля

            private static int callCount;

            private static System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(" \\(copy (\\d+)\\)$");

            #endregion

            #region Методы

            internal static String AppendCopyToFileName(String f)
            {
                callCount++;
                if (callCount > 25)
                    throw new OverflowException("overflow while creating filename");

                int n = 1;
                int r = f.LastIndexOf(".");

                if (r == -1)
                {
                    // there is no extension
                    System.Text.RegularExpressions.Match m = re.Match(f);
                    if (m.Success)
                    {
                        n = Int32.Parse(m.Groups[1].Value) + 1;
                        String copy = String.Format(" (copy {0})", n);
                        f = f.Substring(0, m.Index) + copy;
                    }
                    else
                    {
                        String copy = String.Format(" (copy {0})", n);
                        f = f + copy;
                    }
                }
                else
                {
                    //System.Console.WriteLine("HasExtension");
                    System.Text.RegularExpressions.Match m = re.Match(f.Substring(0, r));
                    if (m.Success)
                    {
                        n = Int32.Parse(m.Groups[1].Value) + 1;
                        String copy = String.Format(" (copy {0})", n);
                        f = f.Substring(0, m.Index) + copy + f.Substring(r);
                    }
                    else
                    {
                        String copy = String.Format(" (copy {0})", n);
                        f = f.Substring(0, r) + copy + f.Substring(r);
                    }

                    //System.Console.WriteLine("returning f({0})", f);
                }
                return f;
            }

            #endregion
        }

        #endregion Класс CopyHelper
    }

    #endregion Класс ZipEntry

    #region Класс ZipCrypto

    internal class ZipCrypto
    {
        #region Поля

        private ZipCRC32 _crc32 = new ZipCRC32();

        private UInt32[] _keys =
            {
                0x12345678, 0x23456789, 0x34567890
            };

        #endregion

        #region Свойства

        private Byte MagicByte
        {
            get
            {
                UInt16 t = (UInt16)((UInt16)(_keys[2] & 0xFFFF) | 2);
                return (Byte)((t * (t ^ 1)) >> 8);
            }
        }

        #endregion

        #region Конструктор

        private ZipCrypto()
        {
        }

        #endregion

        #region Методы

        public static ZipCrypto ForWrite(String password)
        {
            ZipCrypto z = new ZipCrypto();
            if (password == null)
                throw new ZipBadPasswordException("This entry requires a password.");
            z.InitCipher(password);
            return z;
        }

        public static ZipCrypto ForRead(String password, ZipEntry e)
        {
            System.IO.Stream s = e._archiveStream;
            e._WeakEncryptionHeader = new Byte[12];
            Byte[] eh = e._WeakEncryptionHeader;
            ZipCrypto z = new ZipCrypto();

            if (password == null)
                throw new ZipBadPasswordException("This entry requires a password.");

            z.InitCipher(password);

            ZipEntry.ReadWeakEncryptionHeader(s, eh);

            // Decrypt the header.  This has a side effect of "further initializing the
            // encryption keys" in the traditional zip encryption.
            Byte[] DecryptedHeader = z.DecryptMessage(eh, eh.Length);

            // CRC check
            // According to the pkzip spec, the final Byte in the decrypted header
            // is the highest-order Byte in the CRC. We check it here.
            if (DecryptedHeader[11] != (Byte)((e._Crc32 >> 24) & 0xff))
            {
                // In the case that bit 3 of the general purpose bit flag is set to
                // indicate the presence of an 'Extended File Header' or a 'data
                // descriptor' (signature 0x08074b50), the last Byte of the decrypted
                // header is sometimes compared with the high-order Byte of the
                // lastmodified time, rather than the high-order Byte of the CRC, to
                // verify the password.
                //
                // This is not documented in the PKWare Appnote.txt.  It was
                // discovered this by analysis of the Crypt.c source file in the
                // InfoZip library http://www.info-zip.org/pub/infozip/
                //
                // The reason for this is that the CRC for a file cannot be known
                // until the entire contents of the file have been streamed. This
                // means a tool would have to read the file content TWICE in its
                // entirety in order to perform PKZIP encryption - once to compute
                // the CRC, and again to actually encrypt.
                //
                // This is so important for performance that using the timeblob as
                // the verification should be the standard practice for DotNetZip
                // when using PKZIP encryption. This implies that bit 3 must be
                // set. The downside is that some tools still cannot cope with ZIP
                // files that use bit 3.  Therefore, DotNetZip DOES NOT force bit 3
                // when PKZIP encryption is in use, and instead, reads the stream
                // twice.
                //

                if ((e._BitField & 0x0008) != 0x0008)
                    throw new ZipBadPasswordException("The password did not match.");
                else if (DecryptedHeader[11] != (Byte)((e._TimeBlob >> 8) & 0xff))
                    throw new ZipBadPasswordException("The password did not match.");

                // We have a good password.
            }
            else
            {
                // A-OK
            }
            return z;
        }

        public Byte[] DecryptMessage(Byte[] cipherText, int length)
        {
            if (cipherText == null)
                throw new ArgumentNullException("cipherText");

            if (length > cipherText.Length)
            {
                throw new ArgumentOutOfRangeException("length",
                                                      "Bad length during Decryption: the length parameter must be smaller than or equal to the size of the destination array.");
            }

            Byte[] plainText = new Byte[length];
            for (int i = 0; i < length; i++)
            {
                Byte C = (Byte)(cipherText[i] ^ MagicByte);
                UpdateKeys(C);
                plainText[i] = C;
            }
            return plainText;
        }

        public Byte[] EncryptMessage(Byte[] plainText, int length)
        {
            if (plainText == null)
                throw new ArgumentNullException("plaintext");

            if (length > plainText.Length)
            {
                throw new ArgumentOutOfRangeException("length",
                                                      "Bad length during Encryption: The length parameter must be smaller than or equal to the size of the destination array.");
            }

            Byte[] cipherText = new Byte[length];
            for (int i = 0; i < length; i++)
            {
                Byte C = plainText[i];
                cipherText[i] = (Byte)(plainText[i] ^ MagicByte);
                UpdateKeys(C);
            }
            return cipherText;
        }

        public void InitCipher(String passphrase)
        {
            Byte[] p = ZipSharedUtilities.StringToByteArray(passphrase);
            for (int i = 0; i < passphrase.Length; i++)
                UpdateKeys(p[i]);
        }

        private void UpdateKeys(Byte ByteValue)
        {
            _keys[0] = (UInt32)_crc32.ComputeCrc32((int)_keys[0], ByteValue);
            _keys[1] = _keys[1] + (Byte)_keys[0];
            _keys[1] = _keys[1] * 0x08088405 + 1;
            _keys[2] = (UInt32)_crc32.ComputeCrc32((int)_keys[2], (Byte)(_keys[1] >> 24));
        }

        #endregion
    }

    #endregion Класс ZipCrypto

    #region Класс ZipOutput

    internal static class ZipOutput
    {
        #region Методы

        public static Boolean WriteCentralDirectoryStructure(Stream s, ICollection<ZipEntry> entries, uint numSegments, Zip64Option zip64, String comment, ZipContainer container)
        {
            var zss = s as ZipSegmentedStream;
            if (zss != null)
                zss.ContiguousWrite = true;

            // write to a memory stream in order to keep the
            // CDR contiguous
            Int64 aLength = 0;
            using (var ms = new MemoryStream())
            {
                foreach (ZipEntry e in entries)
                {
                    if (e.IncludedInMostRecentSave)
                    {
                        // this writes a ZipDirEntry corresponding to the ZipEntry
                        e.WriteCentralDirectoryEntry(ms);
                    }
                }
                var a = ms.ToArray();
                s.Write(a, 0, a.Length);
                aLength = a.Length;
            }

            // We need to keep track of the start and
            // Finish of the Central Directory Structure.

            // Cannot always use WriteStream.Length or Position; some streams do
            // not support these. (eg, ASP.NET Response.OutputStream) In those
            // cases we have a CountingStream.

            // Also, we cannot just set Start as s.Position bfore the write, and Finish
            // as s.Position after the write.  In a split zip, the write may actually
            // flip to the next segment.  In that case, Start will be zero.  But we
            // don't know that til after we know the size of the thing to write.  So the
            // answer is to compute the directory, then ask the ZipSegmentedStream which
            // segment that directory would fall in, it it were written.  Then, include
            // that data into the directory, and finally, write the directory to the
            // output stream.

            var output = s as ZipCountingStream;
            long Finish = (output != null) ? output.ComputedPosition : s.Position; // BytesWritten
            long Start = Finish - aLength;

            // need to know which segment the EOCD record starts in
            UInt32 startSegment = (zss != null)
                                      ? zss.CurrentSegment
                                      : 0;

            Int64 SizeOfCentralDirectory = Finish - Start;

            int countOfEntries = CountEntries(entries);

            Boolean needZip64CentralDirectory =
                zip64 == Zip64Option.Always ||
                countOfEntries >= 0xFFFF ||
                SizeOfCentralDirectory > 0xFFFFFFFF ||
                Start > 0xFFFFFFFF;

            Byte[] a2 = null;

            // emit ZIP64 extensions as required
            if (needZip64CentralDirectory)
            {
                if (zip64 == Zip64Option.Never)
                {
#if NETCF
                    throw new ZipException("The archive requires a ZIP64 Central Directory. Consider enabling ZIP64 extensions.");
#else
                    System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame(1);
                    if (sf.GetMethod().DeclaringType == typeof(ZipFile))
                        throw new ZipException("The archive requires a ZIP64 Central Directory. Consider setting the ZipFile.UseZip64WhenSaving property.");
                    else
                        throw new ZipException("The archive requires a ZIP64 Central Directory. Consider setting the ZipOutputStream.EnableZip64 property.");
#endif
                }

                var a = GenZip64EndOfCentralDirectory(Start, Finish, countOfEntries, numSegments);
                a2 = GenCentralDirectoryFooter(Start, Finish, zip64, countOfEntries, comment, container);
                if (startSegment != 0)
                {
                    UInt32 thisSegment = zss.ComputeSegment(a.Length + a2.Length);
                    int i = 16;
                    // number of this disk
                    Array.Copy(BitConverter.GetBytes(thisSegment), 0, a, i, 4);
                    i += 4;
                    // number of the disk with the start of the central directory
                    //Array.Copy(BitConverter.GetBytes(startSegment), 0, a, i, 4);
                    Array.Copy(BitConverter.GetBytes(thisSegment), 0, a, i, 4);

                    i = 60;
                    // offset 60
                    // number of the disk with the start of the zip64 eocd
                    Array.Copy(BitConverter.GetBytes(thisSegment), 0, a, i, 4);
                    i += 4;
                    i += 8;

                    // offset 72
                    // total number of disks
                    Array.Copy(BitConverter.GetBytes(thisSegment), 0, a, i, 4);
                }
                s.Write(a, 0, a.Length);
            }
            else
                a2 = GenCentralDirectoryFooter(Start, Finish, zip64, countOfEntries, comment, container);

            // now, the regular footer
            if (startSegment != 0)
            {
                // The assumption is the central directory is never split across
                // segment boundaries.

                UInt16 thisSegment = (UInt16)zss.ComputeSegment(a2.Length);
                int i = 4;
                // number of this disk
                Array.Copy(BitConverter.GetBytes(thisSegment), 0, a2, i, 2);
                i += 2;
                // number of the disk with the start of the central directory
                //Array.Copy(BitConverter.GetBytes((UInt16)startSegment), 0, a2, i, 2);
                Array.Copy(BitConverter.GetBytes(thisSegment), 0, a2, i, 2);
                i += 2;
            }

            s.Write(a2, 0, a2.Length);

            // reset the contiguous write property if necessary
            if (zss != null)
                zss.ContiguousWrite = false;

            return needZip64CentralDirectory;
        }

        private static System.Text.Encoding GetEncoding(ZipContainer container, String t)
        {
            switch (container.AlternateEncodingUsage)
            {
                case ZipOption.Always:
                    return container.AlternateEncoding;
                case ZipOption.Never:
                    return container.DefaultEncoding;
            }

            // AsNecessary is in force
            var e = container.DefaultEncoding;
            if (t == null) return e;

            var Bytes = e.GetBytes(t);
            var t2 = e.GetString(Bytes, 0, Bytes.Length);
            if (t2.Equals(t)) return e;
            return container.AlternateEncoding;
        }

        private static Byte[] GenCentralDirectoryFooter(long StartOfCentralDirectory, long EndOfCentralDirectory, Zip64Option zip64, int entryCount, String comment, ZipContainer container)
        {
            System.Text.Encoding encoding = GetEncoding(container, comment);
            int j = 0;
            int bufferLength = 22;
            Byte[] block = null;
            Int16 commentLength = 0;
            if ((comment != null) && (comment.Length != 0))
            {
                block = encoding.GetBytes(comment);
                commentLength = (Int16)block.Length;
            }
            bufferLength += commentLength;
            Byte[] Bytes = new Byte[bufferLength];

            int i = 0;
            // signature
            Byte[] sig = BitConverter.GetBytes(ZipConstants.EndOfCentralDirectorySignature);
            Array.Copy(sig, 0, Bytes, i, 4);
            i += 4;

            // number of this disk
            // (this number may change later)
            Bytes[i++] = 0;
            Bytes[i++] = 0;

            // number of the disk with the start of the central directory
            // (this number may change later)
            Bytes[i++] = 0;
            Bytes[i++] = 0;

            // handle ZIP64 extensions for the end-of-central-directory
            if (entryCount >= 0xFFFF || zip64 == Zip64Option.Always)
            {
                // the ZIP64 version.
                for (j = 0; j < 4; j++)
                    Bytes[i++] = 0xFF;
            }
            else
            {
                // the standard version.
                // total number of entries in the central dir on this disk
                Bytes[i++] = (Byte)(entryCount & 0x00FF);
                Bytes[i++] = (Byte)((entryCount & 0xFF00) >> 8);

                // total number of entries in the central directory
                Bytes[i++] = (Byte)(entryCount & 0x00FF);
                Bytes[i++] = (Byte)((entryCount & 0xFF00) >> 8);
            }

            // size of the central directory
            Int64 SizeOfCentralDirectory = EndOfCentralDirectory - StartOfCentralDirectory;

            if (SizeOfCentralDirectory >= 0xFFFFFFFF || StartOfCentralDirectory >= 0xFFFFFFFF)
            {
                // The actual data is in the ZIP64 central directory structure
                for (j = 0; j < 8; j++)
                    Bytes[i++] = 0xFF;
            }
            else
            {
                // size of the central directory (we just get the low 4 Bytes)
                Bytes[i++] = (Byte)(SizeOfCentralDirectory & 0x000000FF);
                Bytes[i++] = (Byte)((SizeOfCentralDirectory & 0x0000FF00) >> 8);
                Bytes[i++] = (Byte)((SizeOfCentralDirectory & 0x00FF0000) >> 16);
                Bytes[i++] = (Byte)((SizeOfCentralDirectory & 0xFF000000) >> 24);

                // offset of the start of the central directory (we just get the low 4 Bytes)
                Bytes[i++] = (Byte)(StartOfCentralDirectory & 0x000000FF);
                Bytes[i++] = (Byte)((StartOfCentralDirectory & 0x0000FF00) >> 8);
                Bytes[i++] = (Byte)((StartOfCentralDirectory & 0x00FF0000) >> 16);
                Bytes[i++] = (Byte)((StartOfCentralDirectory & 0xFF000000) >> 24);
            }

            // zip archive comment
            if ((comment == null) || (comment.Length == 0))
            {
                // no comment!
                Bytes[i++] = 0;
                Bytes[i++] = 0;
            }
            else
            {
                // the size of our buffer defines the max length of the comment we can write
                if (commentLength + i + 2 > Bytes.Length) commentLength = (Int16)(Bytes.Length - i - 2);
                Bytes[i++] = (Byte)(commentLength & 0x00FF);
                Bytes[i++] = (Byte)((commentLength & 0xFF00) >> 8);

                if (commentLength != 0)
                {
                    // now actually write the comment itself into the Byte buffer
                    for (j = 0; (j < commentLength) && (i + j < Bytes.Length); j++)
                        Bytes[i + j] = block[j];
                    i += j;
                }
            }

            //   s.Write(Bytes, 0, i);
            return Bytes;
        }

        private static Byte[] GenZip64EndOfCentralDirectory(long StartOfCentralDirectory, long EndOfCentralDirectory, int entryCount, uint numSegments)
        {
            const int bufferLength = 12 + 44 + 20;

            Byte[] Bytes = new Byte[bufferLength];

            int i = 0;
            // signature
            Byte[] sig = BitConverter.GetBytes(ZipConstants.Zip64EndOfCentralDirectoryRecordSignature);
            Array.Copy(sig, 0, Bytes, i, 4);
            i += 4;

            // There is a possibility to include "Extensible" data in the zip64
            // end-of-central-dir record.  I cannot figure out what it might be used to
            // store, so the size of this record is always fixed.  Maybe it is used for
            // strong encryption data?  That is for another day.
            long DataSize = 44;
            Array.Copy(BitConverter.GetBytes(DataSize), 0, Bytes, i, 8);
            i += 8;

            // offset 12
            // VersionMadeBy = 45;
            Bytes[i++] = 45;
            Bytes[i++] = 0x00;

            // VersionNeededToExtract = 45;
            Bytes[i++] = 45;
            Bytes[i++] = 0x00;

            // offset 16
            // number of the disk, and the disk with the start of the central dir.
            // (this may change later)
            for (int j = 0; j < 8; j++)
                Bytes[i++] = 0x00;

            // offset 24
            long numberOfEntries = entryCount;
            Array.Copy(BitConverter.GetBytes(numberOfEntries), 0, Bytes, i, 8);
            i += 8;
            Array.Copy(BitConverter.GetBytes(numberOfEntries), 0, Bytes, i, 8);
            i += 8;

            // offset 40
            Int64 SizeofCentraldirectory = EndOfCentralDirectory - StartOfCentralDirectory;
            Array.Copy(BitConverter.GetBytes(SizeofCentraldirectory), 0, Bytes, i, 8);
            i += 8;
            Array.Copy(BitConverter.GetBytes(StartOfCentralDirectory), 0, Bytes, i, 8);
            i += 8;

            // offset 56
            // now, the locator
            // signature
            sig = BitConverter.GetBytes(ZipConstants.Zip64EndOfCentralDirectoryLocatorSignature);
            Array.Copy(sig, 0, Bytes, i, 4);
            i += 4;

            // offset 60
            // number of the disk with the start of the zip64 eocd
            // (this will change later)  (it will?)
            uint x2 = (numSegments == 0) ? 0 : (numSegments - 1);
            Array.Copy(BitConverter.GetBytes(x2), 0, Bytes, i, 4);
            i += 4;

            // offset 64
            // relative offset of the zip64 eocd
            Array.Copy(BitConverter.GetBytes(EndOfCentralDirectory), 0, Bytes, i, 8);
            i += 8;

            // offset 72
            // total number of disks
            // (this will change later)
            Array.Copy(BitConverter.GetBytes(numSegments), 0, Bytes, i, 4);
            i += 4;

            return Bytes;
        }

        private static int CountEntries(ICollection<ZipEntry> _entries)
        {
            // Cannot just emit _entries.Count, because some of the entries
            // may have been skipped.
            int count = 0;
            foreach (var entry in _entries)
                if (entry.IncludedInMostRecentSave) count++;
            return count;
        }

        #endregion
    }

    #endregion Класс ZipOutput

    #region Класс ZipSegmentedStream

    internal class ZipSegmentedStream : System.IO.Stream
    {
        #region Перечисления

        private enum RwMode
        {
            None = 0,

            ReadOnly = 1,

            Write = 2,
        }

        #endregion

        #region Поля

        private String _baseDir;

        private String _baseName;

        private uint _currentDiskNumber;

        private String _currentName;

        private String _currentTempName;

        private Boolean _exceptionPending;

        private System.IO.Stream _innerStream;

        private uint _maxDiskNumber;

        private int _maxSegmentSize;

        private RwMode _rwMode;

        #endregion

        #region Свойства

        public Boolean ContiguousWrite { get; set; }

        public UInt32 CurrentSegment
        {
            get { return _currentDiskNumber; }
            private set
            {
                _currentDiskNumber = value;
                _currentName = null;
            }
        }

        public String CurrentName
        {
            get
            {
                if (_currentName == null)
                    _currentName = _NameForSegment(CurrentSegment);

                return _currentName;
            }
        }

        public String CurrentTempName
        {
            get { return _currentTempName; }
        }

        public override Boolean CanRead
        {
            get { return (_rwMode == RwMode.ReadOnly && (_innerStream != null) && _innerStream.CanRead); }
        }

        public override Boolean CanSeek
        {
            get { return (_innerStream != null) && _innerStream.CanSeek; }
        }

        public override Boolean CanWrite
        {
            get { return (_rwMode == RwMode.Write) && (_innerStream != null) && _innerStream.CanWrite; }
        }

        public override long Length
        {
            get { return _innerStream.Length; }
        }

        public override long Position
        {
            get { return _innerStream.Position; }
            set { _innerStream.Position = value; }
        }

        #endregion

        #region Конструктор

        private ZipSegmentedStream()
        {
            _exceptionPending = false;
        }

        #endregion

        #region Методы

        public static ZipSegmentedStream ForReading(String name, uint initialDiskNumber, uint maxDiskNumber)
        {
            ZipSegmentedStream zss = new ZipSegmentedStream
                {
                    _rwMode = RwMode.ReadOnly,
                    CurrentSegment = initialDiskNumber,
                    _maxDiskNumber = maxDiskNumber,
                    _baseName = name,
                };

            // Console.WriteLine("ZSS: ForReading ({0})",
            //                    Path.GetFileName(zss.CurrentName));

            zss._SetReadStream();

            return zss;
        }

        public static ZipSegmentedStream ForWriting(String name, int maxSegmentSize)
        {
            ZipSegmentedStream zss = new ZipSegmentedStream
                {
                    _rwMode = RwMode.Write,
                    CurrentSegment = 0,
                    _baseName = name,
                    _maxSegmentSize = maxSegmentSize,
                    _baseDir = Path.GetDirectoryName(name)
                };

            // workitem 9522
            if (zss._baseDir == "") zss._baseDir = ".";

            zss._SetWriteStream(0);

            // Console.WriteLine("ZSS: ForWriting ({0})",
            //                    Path.GetFileName(zss.CurrentName));

            return zss;
        }

        public static Stream ForUpdate(String name, uint diskNumber)
        {
            if (diskNumber >= 99)
                throw new ArgumentOutOfRangeException("diskNumber");

            String fname =
                String.Format("{0}.z{1:D2}",
                              Path.Combine(Path.GetDirectoryName(name),
                                           Path.GetFileNameWithoutExtension(name)),
                              diskNumber + 1);

            // Console.WriteLine("ZSS: ForUpdate ({0})",
            //                   Path.GetFileName(fname));

            // This class assumes that the update will not expand the
            // size of the segment. Update is used only for an in-place
            // update of zip metadata. It never will try to write beyond
            // the end of a segment.

            return File.Open(fname,
                             FileMode.Open,
                             FileAccess.ReadWrite,
                             FileShare.None);
        }

        private String _NameForSegment(uint diskNumber)
        {
            if (diskNumber >= 99)
            {
                _exceptionPending = true;
                throw new OverflowException("The number of zip segments would exceed 99.");
            }

            return String.Format("{0}.z{1:D2}",
                                 Path.Combine(Path.GetDirectoryName(_baseName),
                                              Path.GetFileNameWithoutExtension(_baseName)),
                                 diskNumber + 1);
        }

        public UInt32 ComputeSegment(int length)
        {
            if (_innerStream.Position + length > _maxSegmentSize)
                // the block will go AT LEAST into the next segment
                return CurrentSegment + 1;

            // it will fit in the current segment
            return CurrentSegment;
        }

        public override String ToString()
        {
            return String.Format("{0}[{1}][{2}], pos=0x{3:X})",
                                 "ZipSegmentedStream", CurrentName,
                                 _rwMode.ToString(),
                                 Position);
        }

        private void _SetReadStream()
        {
            if (_innerStream != null)
                _innerStream.Dispose();

            if (CurrentSegment + 1 == _maxDiskNumber)
                _currentName = _baseName;

            _innerStream = File.OpenRead(CurrentName);
        }

        public override int Read(Byte[] buffer, int offset, int count)
        {
            if (_rwMode != RwMode.ReadOnly)
            {
                _exceptionPending = true;
                throw new InvalidOperationException("Stream Error: Cannot Read.");
            }

            int r = _innerStream.Read(buffer, offset, count);
            int r1 = r;

            while (r1 != count)
            {
                if (_innerStream.Position != _innerStream.Length)
                {
                    _exceptionPending = true;
                    throw new ZipException(String.Format("Read error in file {0}", CurrentName));
                }

                if (CurrentSegment + 1 == _maxDiskNumber)
                    return r; // no more to read

                CurrentSegment++;
                _SetReadStream();
                offset += r1;
                count -= r1;
                r1 = _innerStream.Read(buffer, offset, count);
                r += r1;
            }
            return r;
        }

        private void _SetWriteStream(uint increment)
        {
            if (_innerStream != null)
            {
                _innerStream.Dispose();
                if (File.Exists(CurrentName))
                    File.Delete(CurrentName);
                File.Move(_currentTempName, CurrentName);
                // Console.WriteLine("ZSS: SWS close ({0})",
                //                   Path.GetFileName(CurrentName));
            }

            if (increment > 0)
                CurrentSegment += increment;

            ZipSharedUtilities.CreateAndOpenUniqueTempFile(_baseDir,
                                                           out _innerStream,
                                                           out _currentTempName);

            // Console.WriteLine("ZSS: SWS open ({0})",
            //                   Path.GetFileName(_currentTempName));

            if (CurrentSegment == 0)
                _innerStream.Write(BitConverter.GetBytes(ZipConstants.SplitArchiveSignature), 0, 4);
        }

        public override void Write(Byte[] buffer, int offset, int count)
        {
            if (_rwMode != RwMode.Write)
            {
                _exceptionPending = true;
                throw new InvalidOperationException("Stream Error: Cannot Write.");
            }

            if (ContiguousWrite)
            {
                // enough space for a contiguous write?
                if (_innerStream.Position + count > _maxSegmentSize)
                    _SetWriteStream(1);
            }
            else
            {
                while (_innerStream.Position + count > _maxSegmentSize)
                {
                    int c = unchecked(_maxSegmentSize - (int)_innerStream.Position);
                    _innerStream.Write(buffer, offset, c);
                    _SetWriteStream(1);
                    count -= c;
                    offset += c;
                }
            }

            _innerStream.Write(buffer, offset, count);
        }

        public long TruncateBackward(uint diskNumber, long offset)
        {
            if (diskNumber >= 99)
                throw new ArgumentOutOfRangeException("diskNumber");

            if (_rwMode != RwMode.Write)
            {
                _exceptionPending = true;
                throw new ZipException("bad state.");
            }

            // Seek back in the segmented stream to a (maybe) prior segment.

            // Check if it is the same segment.  If it is, very simple.
            if (diskNumber == CurrentSegment)
            {
                var x = _innerStream.Seek(offset, SeekOrigin.Begin);
                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(_innerStream);
                return x;
            }

            // Seeking back to a prior segment.
            // The current segment and any intervening segments must be removed.
            // First, close the current segment, and then remove it.
            if (_innerStream != null)
            {
#if NETCF
                _innerStream.Close();
#else
                _innerStream.Dispose();
#endif
                if (File.Exists(_currentTempName))
                    File.Delete(_currentTempName);
            }

            // Now, remove intervening segments.
            for (uint j = CurrentSegment - 1; j > diskNumber; j--)
            {
                String s = _NameForSegment(j);
                // Console.WriteLine("***ZSS.Trunc:  removing file {0}", s);
                if (File.Exists(s))
                    File.Delete(s);
            }

            // now, open the desired segment.  It must exist.
            CurrentSegment = diskNumber;

            // get a new temp file, try 3 times:
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    _currentTempName = ZipSharedUtilities.InternalGetTempFileName();
                    // move the .z0x file back to a temp name
                    File.Move(CurrentName, _currentTempName);
                    break; // workitem 12403
                }
                catch (IOException)
                {
                    if (i == 2) throw;
                }
            }

            // open it
            _innerStream = new FileStream(_currentTempName, FileMode.Open);

            var r = _innerStream.Seek(offset, SeekOrigin.Begin);

            // workitem 10178
            ZipSharedUtilities.Workaround_Ladybug318918(_innerStream);

            return r;
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            var x = _innerStream.Seek(offset, origin);
            ZipSharedUtilities.Workaround_Ladybug318918(_innerStream);

            return x;
        }

        public override void SetLength(long value)
        {
            if (_rwMode != RwMode.Write)
            {
                _exceptionPending = true;
                throw new InvalidOperationException();
            }
            _innerStream.SetLength(value);
        }

        protected override void Dispose(Boolean disposing)
        {
            try
            {
                if (_innerStream != null)
                {
                    _innerStream.Dispose();

                    if (_rwMode == RwMode.Write)
                    {
                        if (_exceptionPending)
                        {
                            // possibly could try to clean up all the
                            // temp files created so far...
                        }
                        else
                        {
                            // // move the final temp file to the .zNN name
                            // if (File.Exists(CurrentName))
                            //     File.Delete(CurrentName);
                            // if (File.Exists(_currentTempName))
                            //     File.Move(_currentTempName, CurrentName);
                        }
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion
    }

    #endregion Класс ZipSegmentedStream

    #region Класс ZipOutputStream

    public class ZipOutputStream : Stream
    {
        #region Поля

        internal ZipParallelDeflateOutputStream ParallelDeflater;

        private Boolean _DontIgnoreCase;

        private long _ParallelDeflateThreshold;

        private System.Text.Encoding _alternateEncoding = System.Text.Encoding.GetEncoding("IBM437");

        private ZipOption _alternateEncodingUsage = ZipOption.Never;

        private Boolean _anyEntriesUsedZip64;

        private String _comment;

        private ZipEntry _currentEntry;

        private Stream _deflater;

        private Boolean _directoryNeededZip64;

        private Boolean _disposed;

        private ZipEncryptionAlgorithm _encryption;

        private Stream _encryptor;

        private Dictionary<String, ZipEntry> _entriesWritten;

        private int _entryCount;

        private ZipCrcCalculatorStream _entryOutputStream;

        private Boolean _exceptionPending;

        private Boolean _leaveUnderlyingStreamOpen;

        private int _maxBufferPairs = 16;

        private String _name;

        private Boolean _needToWriteEntryHeader;

        private ZipCountingStream _outputCounter;

        private Stream _outputStream;

        internal String _password;

        private ZipEntryTimestamp _timestamp;

        internal Zip64Option _zip64;

        #endregion

        #region Свойства

        internal Stream OutputStream
        {
            get { return _outputStream; }
        }

        internal String Name
        {
            get { return _name; }
        }

        public String Password
        {
            set
            {
                if (_disposed)
                {
                    _exceptionPending = true;
                    throw new System.InvalidOperationException("The stream has been closed.");
                }

                _password = value;
                if (_password == null)
                    _encryption = ZipEncryptionAlgorithm.None;
                else if (_encryption == ZipEncryptionAlgorithm.None)
                    _encryption = ZipEncryptionAlgorithm.PkzipWeak;
            }
        }

        public ZipEncryptionAlgorithm Encryption
        {
            get { return _encryption; }
            set
            {
                if (_disposed)
                {
                    _exceptionPending = true;
                    throw new System.InvalidOperationException("The stream has been closed.");
                }
                if (value == ZipEncryptionAlgorithm.Unsupported)
                {
                    _exceptionPending = true;
                    throw new InvalidOperationException("You may not set Encryption to that value.");
                }
                _encryption = value;
            }
        }

        public int CodecBufferSize { get; set; }

        public ZipCompressionStrategy Strategy { get; set; }

        public ZipEntryTimestamp Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (_disposed)
                {
                    _exceptionPending = true;
                    throw new System.InvalidOperationException("The stream has been closed.");
                }
                _timestamp = value;
            }
        }

        public ZipCompressionLevel CompressionLevel { get; set; }

        public ZipCompressionMethod CompressionMethod { get; set; }

        public String Comment
        {
            get { return _comment; }
            set
            {
                if (_disposed)
                {
                    _exceptionPending = true;
                    throw new System.InvalidOperationException("The stream has been closed.");
                }
                _comment = value;
            }
        }

        public Zip64Option EnableZip64
        {
            get { return _zip64; }
            set
            {
                if (_disposed)
                {
                    _exceptionPending = true;
                    throw new System.InvalidOperationException("The stream has been closed.");
                }
                _zip64 = value;
            }
        }

        public Boolean OutputUsedZip64
        {
            get { return _anyEntriesUsedZip64 || _directoryNeededZip64; }
        }

        public Boolean IgnoreCase
        {
            get { return !_DontIgnoreCase; }
            set { _DontIgnoreCase = !value; }
        }

        public System.Text.Encoding AlternateEncoding
        {
            get { return _alternateEncoding; }
            set { _alternateEncoding = value; }
        }

        public ZipOption AlternateEncodingUsage
        {
            get { return _alternateEncodingUsage; }
            set { _alternateEncodingUsage = value; }
        }

        public static System.Text.Encoding DefaultEncoding
        {
            get { return System.Text.Encoding.GetEncoding("IBM437"); }
        }

        public long ParallelDeflateThreshold
        {
            get { return _ParallelDeflateThreshold; }
            set
            {
                if ((value != 0) && (value != -1) && (value < 64 * 1024))
                    throw new ArgumentOutOfRangeException("value must be greater than 64k, or 0, or -1");
                _ParallelDeflateThreshold = value;
            }
        }

        public int ParallelDeflateMaxBufferPairs
        {
            get { return _maxBufferPairs; }
            set
            {
                if (value < 4)
                {
                    throw new ArgumentOutOfRangeException("ParallelDeflateMaxBufferPairs", "Value must be 4 or greater.");
                }
                _maxBufferPairs = value;
            }
        }

        public override Boolean CanRead
        {
            get { return false; }
        }

        public override Boolean CanSeek
        {
            get { return false; }
        }

        public override Boolean CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { return _outputStream.Position; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Конструктор

        public ZipOutputStream(Stream stream) : this(stream, false)
        {
        }

        public ZipOutputStream(String fileName)
        {
            Stream stream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            _Init(stream, false, fileName);
        }

        public ZipOutputStream(Stream stream, Boolean leaveOpen)
        {
            _Init(stream, leaveOpen, null);
        }

        #endregion

        #region Методы

        private void _Init(Stream stream, Boolean leaveOpen, String name)
        {
            _outputStream = stream.CanRead ? stream : new ZipCountingStream(stream);
            CompressionLevel = ZipCompressionLevel.Default;
            CompressionMethod = ZipCompressionMethod.Deflate;
            _encryption = ZipEncryptionAlgorithm.None;
            _entriesWritten = new Dictionary<String, ZipEntry>(StringComparer.Ordinal);
            _zip64 = Zip64Option.Never;
            _leaveUnderlyingStreamOpen = leaveOpen;
            Strategy = ZipCompressionStrategy.Default;
            _name = name ?? "(stream)";
            ParallelDeflateThreshold = -1L;
        }

        public override String ToString()
        {
            return String.Format("ZipOutputStream::{0}(leaveOpen({1})))", _name, _leaveUnderlyingStreamOpen);
        }

        private void InsureUniqueEntry(ZipEntry ze1)
        {
            if (_entriesWritten.ContainsKey(ze1.FileName))
            {
                _exceptionPending = true;
                throw new ArgumentException(String.Format("The entry '{0}' already exists in the zip archive.", ze1.FileName));
            }
        }

        public Boolean ContainsEntry(String name)
        {
            return _entriesWritten.ContainsKey(ZipSharedUtilities.NormalizePathForUseInZipFile(name));
        }

        public override void Write(Byte[] buffer, int offset, int count)
        {
            if (_disposed)
            {
                _exceptionPending = true;
                throw new System.InvalidOperationException("The stream has been closed.");
            }

            if (buffer == null)
            {
                _exceptionPending = true;
                throw new System.ArgumentNullException("buffer");
            }

            if (_currentEntry == null)
            {
                _exceptionPending = true;
                throw new System.InvalidOperationException("You must call PutNextEntry() before calling Write().");
            }

            if (_currentEntry.IsDirectory)
            {
                _exceptionPending = true;
                throw new System.InvalidOperationException("You cannot Write() data for an entry that is a directory.");
            }

            if (_needToWriteEntryHeader)
                _InitiateCurrentEntry(false);

            if (count != 0)
                _entryOutputStream.Write(buffer, offset, count);
        }

        public ZipEntry PutNextEntry(String entryName)
        {
            if (String.IsNullOrEmpty(entryName))
                throw new ArgumentNullException("entryName");

            if (_disposed)
            {
                _exceptionPending = true;
                throw new System.InvalidOperationException("The stream has been closed.");
            }

            _FinishCurrentEntry();
            _currentEntry = ZipEntry.CreateForZipOutputStream(entryName);
            _currentEntry._container = new ZipContainer(this);
            _currentEntry._BitField |= 0x0008; // workitem 8932
            _currentEntry.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
            _currentEntry.CompressionLevel = CompressionLevel;
            _currentEntry.CompressionMethod = CompressionMethod;
            _currentEntry.Password = _password; // workitem 13909
            _currentEntry.Encryption = Encryption;
            // workitem 12634
            _currentEntry.AlternateEncoding = AlternateEncoding;
            _currentEntry.AlternateEncodingUsage = AlternateEncodingUsage;

            if (entryName.EndsWith("/")) _currentEntry.MarkAsDirectory();

            _currentEntry.EmitTimesInWindowsFormatWhenSaving = ((_timestamp & ZipEntryTimestamp.Windows) != 0);
            _currentEntry.EmitTimesInUnixFormatWhenSaving = ((_timestamp & ZipEntryTimestamp.Unix) != 0);
            InsureUniqueEntry(_currentEntry);
            _needToWriteEntryHeader = true;

            return _currentEntry;
        }

        private void _InitiateCurrentEntry(Boolean finishing)
        {
            // If finishing==true, this means we're initiating the entry at the time of
            // Close() or PutNextEntry().  If this happens, it means no data was written
            // for the entry - Write() was never called.  (The usual case us to call
            // _InitiateCurrentEntry(Boolean) from within Write().)  If finishing==true,
            // the entry could be either a zero-Byte file or a directory.

            _entriesWritten.Add(_currentEntry.FileName, _currentEntry);
            _entryCount++; // could use _entriesWritten.Count, but I don't want to incur
            // the cost.

            if (_entryCount > 65534 && _zip64 == Zip64Option.Never)
            {
                _exceptionPending = true;
                throw new System.InvalidOperationException("Too many entries. Consider setting ZipOutputStream.EnableZip64.");
            }

            // Write out the header.
            //
            // If finishing, and encryption is in use, then we don't want to emit the
            // normal encryption header.  Signal that with a cycle=99 to turn off
            // encryption for zero-Byte entries or directories.
            //
            // If finishing, then we know the stream length is zero.  Else, unknown
            // stream length.  Passing stream length == 0 allows an optimization so as
            // not to setup an encryption or deflation stream, when stream length is
            // zero.

            _currentEntry.WriteHeader(_outputStream, finishing ? 99 : 0);
            _currentEntry.StoreRelativeOffset();

            if (!_currentEntry.IsDirectory)
            {
                _currentEntry.WriteSecurityMetadata(_outputStream);
                _currentEntry.PrepOutputStream(_outputStream,
                                               finishing ? 0 : -1,
                                               out _outputCounter,
                                               out _encryptor,
                                               out _deflater,
                                               out _entryOutputStream);
            }
            _needToWriteEntryHeader = false;
        }

        private void _FinishCurrentEntry()
        {
            if (_currentEntry != null)
            {
                if (_needToWriteEntryHeader)
                    _InitiateCurrentEntry(true); // an empty entry - no writes

                _currentEntry.FinishOutputStream(_outputStream, _outputCounter, _encryptor, _deflater, _entryOutputStream);
                _currentEntry.PostProcessOutput(_outputStream);
                // workitem 12964
                if (_currentEntry.OutputUsedZip64 != null)
                    _anyEntriesUsedZip64 |= _currentEntry.OutputUsedZip64.Value;

                // reset all the streams
                _outputCounter = null;
                _encryptor = _deflater = null;
                _entryOutputStream = null;
            }
        }

        protected override void Dispose(Boolean disposing)
        {
            if (_disposed) return;

            if (disposing) // not called from finalizer
            {
                // handle pending exceptions
                if (!_exceptionPending)
                {
                    _FinishCurrentEntry();
                    _directoryNeededZip64 = ZipOutput.WriteCentralDirectoryStructure(_outputStream,
                                                                                     _entriesWritten.Values,
                                                                                     1, // _numberOfSegmentsForMostRecentSave,
                                                                                     _zip64,
                                                                                     Comment,
                                                                                     new ZipContainer(this));
                    Stream wrappedStream = null;
                    ZipCountingStream cs = _outputStream as ZipCountingStream;
                    if (cs != null)
                    {
                        wrappedStream = cs.WrappedStream;
                        cs.Dispose();
                    }
                    else
                        wrappedStream = _outputStream;

                    if (!_leaveUnderlyingStreamOpen)
                        wrappedStream.Dispose();
                    _outputStream = null;
                }
            }
            _disposed = true;
        }

        public override void Flush()
        {
        }

        public override int Read(Byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Read");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seek");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс ZipOutputStream

    #region Класс ZipContainer

    internal class ZipContainer
    {
        #region Поля

        private ZipFile _zf;

        private ZipInputStream _zis;

        private ZipOutputStream _zos;

        #endregion

        #region Свойства

        public ZipFile ZipFile
        {
            get { return _zf; }
        }

        public ZipOutputStream ZipOutputStream
        {
            get { return _zos; }
        }

        public String Name
        {
            get
            {
                if (_zf != null) return _zf.Name;
                if (_zis != null) throw new NotSupportedException();
                return _zos.Name;
            }
        }

        public String Password
        {
            get
            {
                if (_zf != null) return _zf._password;
                if (_zis != null) return _zis._password;
                return _zos._password;
            }
        }

        public Zip64Option Zip64
        {
            get
            {
                if (_zf != null) return _zf._zip64;
                if (_zis != null) throw new NotSupportedException();
                return _zos._zip64;
            }
        }

        public int BufferSize
        {
            get
            {
                if (_zf != null) return _zf.BufferSize;
                if (_zis != null) throw new NotSupportedException();
                return 0;
            }
        }

        public ZipParallelDeflateOutputStream ParallelDeflater
        {
            get
            {
                if (_zf != null) return _zf.ParallelDeflater;
                if (_zis != null) return null;
                return _zos.ParallelDeflater;
            }
            set
            {
                if (_zf != null) _zf.ParallelDeflater = value;
                else if (_zos != null) _zos.ParallelDeflater = value;
            }
        }

        public long ParallelDeflateThreshold
        {
            get
            {
                if (_zf != null) return _zf.ParallelDeflateThreshold;
                return _zos.ParallelDeflateThreshold;
            }
        }

        public int ParallelDeflateMaxBufferPairs
        {
            get
            {
                if (_zf != null) return _zf.ParallelDeflateMaxBufferPairs;
                return _zos.ParallelDeflateMaxBufferPairs;
            }
        }

        public int CodecBufferSize
        {
            get
            {
                if (_zf != null) return _zf.CodecBufferSize;
                if (_zis != null) return _zis.CodecBufferSize;
                return _zos.CodecBufferSize;
            }
        }

        public ZipCompressionStrategy Strategy
        {
            get
            {
                if (_zf != null) return _zf.Strategy;
                return _zos.Strategy;
            }
        }

        public Zip64Option UseZip64WhenSaving
        {
            get
            {
                if (_zf != null) return _zf.UseZip64WhenSaving;
                return _zos.EnableZip64;
            }
        }

        public System.Text.Encoding AlternateEncoding
        {
            get
            {
                if (_zf != null) return _zf.AlternateEncoding;
                if (_zos != null) return _zos.AlternateEncoding;
                return null;
            }
        }

        public System.Text.Encoding DefaultEncoding
        {
            get
            {
                if (_zf != null) return ZipFile.DefaultEncoding;
                if (_zos != null) return ZipOutputStream.DefaultEncoding;
                return null;
            }
        }

        public ZipOption AlternateEncodingUsage
        {
            get
            {
                if (_zf != null) return _zf.AlternateEncodingUsage;
                if (_zos != null) return _zos.AlternateEncodingUsage;
                return ZipOption.Never; // n/a
            }
        }

        public Stream ReadStream
        {
            get
            {
                if (_zf != null) return _zf.ReadStream;
                return _zis.ReadStream;
            }
        }

        #endregion

        #region Конструктор

        public ZipContainer(Object o)
        {
            _zf = (o as ZipFile);
            _zos = (o as ZipOutputStream);
            _zis = (o as ZipInputStream);
        }

        #endregion
    }

    #endregion Класс ZipContainer

    #region Класс ZipParallelDeflateOutputStream

    public class ZipParallelDeflateOutputStream : System.IO.Stream
    {
        #region Перечисления

        [Flags]
        private enum TraceBits : uint
        {
            None = 0,

            NotUsed1 = 1,

            EmitLock = 2,

            EmitEnter = 4, // enter _EmitPending
            EmitBegin = 8, // begin to write out
            EmitDone = 16, // done writing out
            EmitSkip = 32, // writer skipping a workitem
            EmitAll = 58, // All Emit flags
            Flush = 64,

            Lifecycle = 128, // constructor/disposer
            Session = 256, // Close/Reset
            Synch = 512, // thread synchronization
            Instance = 1024, // instance settings
            Compress = 2048, // compress task
            Write = 4096, // filling buffers, when caller invokes Write()
            WriteEnter = 8192, // upon entry to Write()
            WriteTake = 16384, // on _toFill.Take()
            All = 0xffffffff,
        }

        #endregion

        #region Поля

        private static readonly int BufferPairsPerCore = 4;

        private static readonly int IO_BUFFER_SIZE_DEFAULT = 64 * 1024; // 128k

        private int _Crc32;

        private TraceBits _DesiredTrace =
            TraceBits.Session |
            TraceBits.Compress |
            TraceBits.WriteTake |
            TraceBits.WriteEnter |
            TraceBits.EmitEnter |
            TraceBits.EmitDone |
            TraceBits.EmitLock |
            TraceBits.EmitSkip |
            TraceBits.EmitBegin;

        private int _bufferSize = IO_BUFFER_SIZE_DEFAULT;

        private ZipCompressionLevel _compressLevel;

        private int _currentlyFilling;

        private Object _eLock = new Object();

        private Boolean _firstWriteDone;

        private Boolean _handlingException;

        private Boolean _isClosed;

        private int _lastFilled;

        private int _lastWritten;

        private int _latestCompressed;

        private Object _latestLock = new object();

        private Boolean _leaveOpen;

        private int _maxBufferPairs;

        private AutoResetEvent _newlyCompressedBlob;

        private System.IO.Stream _outStream;

        private Object _outputLock = new object();

        private volatile Exception _pendingException;

        private System.Collections.Generic.List<ZipWorkItem> _pool;

        private ZipCRC32 _runningCrc;

        private System.Collections.Generic.Queue<int> _toFill;

        private System.Collections.Generic.Queue<int> _toWrite;

        private Int64 _totalBytesProcessed;

        private Boolean emitting;

        #endregion

        #region Свойства

        public ZipCompressionStrategy Strategy { get; private set; }

        public int MaxBufferPairs
        {
            get { return _maxBufferPairs; }
            set
            {
                if (value < 4)
                {
                    throw new ArgumentException("MaxBufferPairs",
                                                "Value must be 4 or greater.");
                }
                _maxBufferPairs = value;
            }
        }

        public int BufferSize
        {
            get { return _bufferSize; }
            set
            {
                if (value < 1024)
                {
                    throw new ArgumentOutOfRangeException("BufferSize",
                                                          "BufferSize must be greater than 1024 Bytes");
                }
                _bufferSize = value;
            }
        }

        public int Crc32
        {
            get { return _Crc32; }
        }

        public Int64 BytesProcessed
        {
            get { return _totalBytesProcessed; }
        }

        public override Boolean CanSeek
        {
            get { return false; }
        }

        public override Boolean CanRead
        {
            get { return false; }
        }

        public override Boolean CanWrite
        {
            get { return _outStream.CanWrite; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { return _outStream.Position; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Конструктор

        public ZipParallelDeflateOutputStream(System.IO.Stream stream) : this(stream, ZipCompressionLevel.Default, ZipCompressionStrategy.Default, false)
        {
        }

        public ZipParallelDeflateOutputStream(System.IO.Stream stream, ZipCompressionLevel level) : this(stream, level, ZipCompressionStrategy.Default, false)
        {
        }

        public ZipParallelDeflateOutputStream(System.IO.Stream stream, Boolean leaveOpen) : this(stream, ZipCompressionLevel.Default, ZipCompressionStrategy.Default, leaveOpen)
        {
        }

        public ZipParallelDeflateOutputStream(System.IO.Stream stream, ZipCompressionLevel level, Boolean leaveOpen) : this(stream, ZipCompressionLevel.Default, ZipCompressionStrategy.Default, leaveOpen)
        {
        }

        public ZipParallelDeflateOutputStream(System.IO.Stream stream, ZipCompressionLevel level, ZipCompressionStrategy strategy, Boolean leaveOpen)
        {
            TraceOutput(TraceBits.Lifecycle | TraceBits.Session, "-------------------------------------------------------");
            TraceOutput(TraceBits.Lifecycle | TraceBits.Session, "Create {0:X8}", GetHashCode());
            _outStream = stream;
            _compressLevel = level;
            Strategy = strategy;
            _leaveOpen = leaveOpen;
            MaxBufferPairs = 16; // default
        }

        #endregion

        #region Методы

        private void _InitializePoolOfWorkItems()
        {
            _toWrite = new Queue<int>();
            _toFill = new Queue<int>();
            _pool = new System.Collections.Generic.List<ZipWorkItem>();
            int nTasks = BufferPairsPerCore * Environment.ProcessorCount;
            nTasks = Math.Min(nTasks, _maxBufferPairs);
            for (int i = 0; i < nTasks; i++)
            {
                _pool.Add(new ZipWorkItem(_bufferSize, _compressLevel, Strategy, i));
                _toFill.Enqueue(i);
            }

            _newlyCompressedBlob = new AutoResetEvent(false);
            _runningCrc = new ZipCRC32();
            _currentlyFilling = -1;
            _lastFilled = -1;
            _lastWritten = -1;
            _latestCompressed = -1;
        }

        public override void Write(Byte[] buffer, int offset, int count)
        {
            Boolean mustWait = false;

            // This method does this:
            //   0. handles any pending exceptions
            //   1. write any buffers that are ready to be written,
            //   2. fills a work buffer; when full, flip state to 'Filled',
            //   3. if more data to be written,  goto step 1

            if (_isClosed)
                throw new InvalidOperationException();

            // dispense any exceptions that occurred on the BG threads
            if (_pendingException != null)
            {
                _handlingException = true;
                var pe = _pendingException;
                _pendingException = null;
                throw pe;
            }

            if (count == 0) return;

            if (!_firstWriteDone)
            {
                // Want to do this on first Write, first session, and not in the
                // constructor.  We want to allow MaxBufferPairs to
                // change after construction, but before first Write.
                _InitializePoolOfWorkItems();
                _firstWriteDone = true;
            }

            do
            {
                // may need to make buffers available
                EmitPendingBuffers(false, mustWait);

                mustWait = false;
                // use current buffer, or get a new buffer to fill
                int ix = -1;
                if (_currentlyFilling >= 0)
                {
                    ix = _currentlyFilling;
                    TraceOutput(TraceBits.WriteTake,
                                "Write    notake   wi({0}) lf({1})",
                                ix,
                                _lastFilled);
                }
                else
                {
                    TraceOutput(TraceBits.WriteTake, "Write    take?");
                    if (_toFill.Count == 0)
                    {
                        // no available buffers, so... need to emit
                        // compressed buffers.
                        mustWait = true;
                        continue;
                    }

                    ix = _toFill.Dequeue();
                    TraceOutput(TraceBits.WriteTake,
                                "Write    take     wi({0}) lf({1})",
                                ix,
                                _lastFilled);
                    ++_lastFilled; // TODO: consider rollover?
                }

                ZipWorkItem workitem = _pool[ix];

                int limit = ((workitem._buffer.Length - workitem._inputBytesAvailable) > count)
                                ? count
                                : (workitem._buffer.Length - workitem._inputBytesAvailable);

                workitem._ordinal = _lastFilled;

                TraceOutput(TraceBits.Write,
                            "Write    lock     wi({0}) ord({1}) iba({2})",
                            workitem._index,
                            workitem._ordinal,
                            workitem._inputBytesAvailable
                    );

                // copy from the provided buffer to our workitem, starting at
                // the tail end of whatever data we might have in there currently.
                Buffer.BlockCopy(buffer,
                                 offset,
                                 workitem._buffer,
                                 workitem._inputBytesAvailable,
                                 limit);

                count -= limit;
                offset += limit;
                workitem._inputBytesAvailable += limit;
                if (workitem._inputBytesAvailable == workitem._buffer.Length)
                {
                    // No need for interlocked.increment: the Write()
                    // method is documented as not multi-thread safe, so
                    // we can assume Write() calls come in from only one
                    // thread.
                    TraceOutput(TraceBits.Write,
                                "Write    QUWI     wi({0}) ord({1}) iba({2}) nf({3})",
                                workitem._index,
                                workitem._ordinal,
                                workitem._inputBytesAvailable);

                    if (!ThreadPool.QueueUserWorkItem(_DeflateOne, workitem))
                        throw new Exception("Cannot enqueue workitem");

                    _currentlyFilling = -1; // will get a new buffer next time
                }
                else
                    _currentlyFilling = ix;

                if (count > 0)
                    TraceOutput(TraceBits.WriteEnter, "Write    more");
            } while (count > 0); // until no more to write

            TraceOutput(TraceBits.WriteEnter, "Write    exit");
            return;
        }

        private void _FlushFinish()
        {
            // After writing a series of compressed buffers, each one closed
            // with Flush.Sync, we now write the final one as Flush.Finish,
            // and then stop.
            Byte[] buffer = new Byte[128];
            var compressor = new ZlibCodec();
            int rc = compressor.InitializeDeflate(_compressLevel, false);
            compressor.InputBuffer = null;
            compressor.NextIn = 0;
            compressor.AvailableBytesIn = 0;
            compressor.OutputBuffer = buffer;
            compressor.NextOut = 0;
            compressor.AvailableBytesOut = buffer.Length;
            rc = compressor.Deflate(ZipFlushType.Finish);

            if (rc != ZipConstants.Z_STREAM_END && rc != ZipConstants.Z_OK)
                throw new Exception("deflating: " + compressor.Message);

            if (buffer.Length - compressor.AvailableBytesOut > 0)
            {
                TraceOutput(TraceBits.EmitBegin,
                            "Emit     begin    flush Bytes({0})",
                            buffer.Length - compressor.AvailableBytesOut);

                _outStream.Write(buffer, 0, buffer.Length - compressor.AvailableBytesOut);

                TraceOutput(TraceBits.EmitDone,
                            "Emit     done     flush");
            }

            compressor.EndDeflate();

            _Crc32 = _runningCrc.Crc32Result;
        }

        private void _Flush(Boolean lastInput)
        {
            if (_isClosed)
                throw new InvalidOperationException();

            if (emitting) return;

            // compress any partial buffer
            if (_currentlyFilling >= 0)
            {
                ZipWorkItem workitem = _pool[_currentlyFilling];
                _DeflateOne(workitem);
                _currentlyFilling = -1; // get a new buffer next Write()
            }

            if (lastInput)
            {
                EmitPendingBuffers(true, false);
                _FlushFinish();
            }
            else
                EmitPendingBuffers(false, false);
        }

        public override void Flush()
        {
            if (_pendingException != null)
            {
                _handlingException = true;
                var pe = _pendingException;
                _pendingException = null;
                throw pe;
            }
            if (_handlingException)
                return;

            _Flush(false);
        }

        public override void Close()
        {
            TraceOutput(TraceBits.Session, "Close {0:X8}", GetHashCode());

            if (_pendingException != null)
            {
                _handlingException = true;
                var pe = _pendingException;
                _pendingException = null;
                throw pe;
            }

            if (_handlingException)
                return;

            if (_isClosed) return;

            _Flush(true);

            if (!_leaveOpen)
                _outStream.Close();

            _isClosed = true;
        }

        public new void Dispose()
        {
            TraceOutput(TraceBits.Lifecycle, "Dispose  {0:X8}", GetHashCode());
            Close();
            _pool = null;
            Dispose(true);
        }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);
        }

        public void Reset(Stream stream)
        {
            TraceOutput(TraceBits.Session, "-------------------------------------------------------");
            TraceOutput(TraceBits.Session, "Reset {0:X8} firstDone({1})", GetHashCode(), _firstWriteDone);

            if (!_firstWriteDone) return;

            // reset all status
            _toWrite.Clear();
            _toFill.Clear();
            foreach (var workitem in _pool)
            {
                _toFill.Enqueue(workitem._index);
                workitem._ordinal = -1;
            }

            _firstWriteDone = false;
            _totalBytesProcessed = 0L;
            _runningCrc = new ZipCRC32();
            _isClosed = false;
            _currentlyFilling = -1;
            _lastFilled = -1;
            _lastWritten = -1;
            _latestCompressed = -1;
            _outStream = stream;
        }

        private void EmitPendingBuffers(Boolean doAll, Boolean mustWait)
        {
            // When combining parallel deflation with a ZipSegmentedStream, it's
            // possible for the ZSS to throw from within this method.  In that
            // case, Close/Dispose will be called on this stream, if this stream
            // is employed within a using or try/finally pair as required. But
            // this stream is unaware of the pending exception, so the Close()
            // method invokes this method AGAIN.  This can lead to a deadlock.
            // Therefore, failfast if re-entering.

            if (emitting) return;
            emitting = true;
            if (doAll || mustWait)
                _newlyCompressedBlob.WaitOne();

            do
            {
                int firstSkip = -1;
                int millisecondsToWait = doAll ? 200 : (mustWait ? -1 : 0);
                int nextToWrite = -1;

                do
                {
                    if (Monitor.TryEnter(_toWrite, millisecondsToWait))
                    {
                        nextToWrite = -1;
                        try
                        {
                            if (_toWrite.Count > 0)
                                nextToWrite = _toWrite.Dequeue();
                        }
                        finally
                        {
                            Monitor.Exit(_toWrite);
                        }

                        if (nextToWrite >= 0)
                        {
                            ZipWorkItem workitem = _pool[nextToWrite];
                            if (workitem._ordinal != _lastWritten + 1)
                            {
                                // out of order. requeue and try again.
                                TraceOutput(TraceBits.EmitSkip,
                                            "Emit     skip     wi({0}) ord({1}) lw({2}) fs({3})",
                                            workitem._index,
                                            workitem._ordinal,
                                            _lastWritten,
                                            firstSkip);

                                lock (_toWrite)
                                {
                                    _toWrite.Enqueue(nextToWrite);
                                }

                                if (firstSkip == nextToWrite)
                                {
                                    // We went around the list once.
                                    // None of the items in the list is the one we want.
                                    // Now wait for a compressor to signal again.
                                    _newlyCompressedBlob.WaitOne();
                                    firstSkip = -1;
                                }
                                else if (firstSkip == -1)
                                    firstSkip = nextToWrite;

                                continue;
                            }

                            firstSkip = -1;

                            TraceOutput(TraceBits.EmitBegin,
                                        "Emit     begin    wi({0}) ord({1})              cba({2})",
                                        workitem._index,
                                        workitem._ordinal,
                                        workitem._compressedBytesAvailable);

                            _outStream.Write(workitem._compressed, 0, workitem._compressedBytesAvailable);
                            _runningCrc.Combine(workitem._crc, workitem._inputBytesAvailable);
                            _totalBytesProcessed += workitem._inputBytesAvailable;
                            workitem._inputBytesAvailable = 0;

                            TraceOutput(TraceBits.EmitDone,
                                        "Emit     done     wi({0}) ord({1})              cba({2}) mtw({3})",
                                        workitem._index,
                                        workitem._ordinal,
                                        workitem._compressedBytesAvailable,
                                        millisecondsToWait);

                            _lastWritten = workitem._ordinal;
                            _toFill.Enqueue(workitem._index);

                            // don't wait next time through
                            if (millisecondsToWait == -1) millisecondsToWait = 0;
                        }
                    }
                    else
                        nextToWrite = -1;
                } while (nextToWrite >= 0);
            } while (doAll && (_lastWritten != _latestCompressed));

            emitting = false;
        }

        private void _DeflateOne(Object wi)
        {
            // compress one buffer
            ZipWorkItem workitem = (ZipWorkItem)wi;
            try
            {
                int myItem = workitem._index;
                ZipCRC32 crc = new ZipCRC32();

                // calc CRC on the buffer
                crc.SlurpBlock(workitem._buffer, 0, workitem._inputBytesAvailable);

                // deflate it
                DeflateOneSegment(workitem);

                // update status
                workitem._crc = crc.Crc32Result;
                TraceOutput(TraceBits.Compress,
                            "Compress          wi({0}) ord({1}) len({2})",
                            workitem._index,
                            workitem._ordinal,
                            workitem._compressedBytesAvailable
                    );

                lock (_latestLock)
                {
                    if (workitem._ordinal > _latestCompressed)
                        _latestCompressed = workitem._ordinal;
                }
                lock (_toWrite)
                {
                    _toWrite.Enqueue(workitem._index);
                }
                _newlyCompressedBlob.Set();
            }
            catch (System.Exception exc1)
            {
                lock (_eLock)
                {
                    // expose the exception to the main thread
                    if (_pendingException != null)
                        _pendingException = exc1;
                }
            }
        }

        private Boolean DeflateOneSegment(ZipWorkItem workitem)
        {
            ZlibCodec compressor = workitem._compressor;
            int rc = 0;
            compressor.ResetDeflate();
            compressor.NextIn = 0;

            compressor.AvailableBytesIn = workitem._inputBytesAvailable;

            // step 1: deflate the buffer
            compressor.NextOut = 0;
            compressor.AvailableBytesOut = workitem._compressed.Length;
            do
            {
                compressor.Deflate(ZipFlushType.None);
            } while (compressor.AvailableBytesIn > 0 || compressor.AvailableBytesOut == 0);

            // step 2: flush (sync)
            rc = compressor.Deflate(ZipFlushType.Sync);

            workitem._compressedBytesAvailable = (int)compressor.TotalBytesOut;
            return true;
        }

        [System.Diagnostics.ConditionalAttribute("Trace")]
        private void TraceOutput(TraceBits bits, String format, params object[] varParams)
        {
            if ((bits & _DesiredTrace) != 0)
            {
                lock (_outputLock)
                {
                    int tid = Thread.CurrentThread.GetHashCode();
                    Console.ForegroundColor = (ConsoleColor)(tid % 8 + 8);
                    Console.Write("{0:000} PDOS ", tid);
                    Console.WriteLine(format, varParams);
                    Console.ResetColor();
                }
            }
        }

        public override int Read(Byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс ZipParallelDeflateOutputStream

    #region Класс ZipCRC32

    [Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d0000C")]
    [Interop.ComVisible(true)]
    [Interop.ClassInterface(Interop.ClassInterfaceType.AutoDispatch)]
    public class ZipCRC32
    {
        #region Константы

        private const int BUFFER_SIZE = 8192;

        #endregion

        #region Поля

        private Int64 _TotalBytesRead;

        private UInt32 _register = 0xFFFFFFFFU;

        private UInt32[] crc32Table;

        private UInt32 dwPolynomial;

        private Boolean reverseBits;

        #endregion

        #region Свойства

        public Int64 TotalBytesRead
        {
            get { return _TotalBytesRead; }
        }

        public Int32 Crc32Result
        {
            get { return unchecked((Int32)(~_register)); }
        }

        #endregion

        #region Конструктор

        public ZipCRC32() : this(false)
        {
        }

        public ZipCRC32(Boolean reverseBits) : this(unchecked((int)0xEDB88320), reverseBits)
        {
        }

        public ZipCRC32(int polynomial, Boolean reverseBits)
        {
            this.reverseBits = reverseBits;
            dwPolynomial = (uint)polynomial;
            GenerateLookupTable();
        }

        #endregion

        #region Методы

        public Int32 GetCrc32(System.IO.Stream input)
        {
            return GetCrc32AndCopy(input, null);
        }

        public Int32 GetCrc32AndCopy(System.IO.Stream input, System.IO.Stream output)
        {
            if (input == null)
                throw new Exception("The input stream must not be null.");

            unchecked
            {
                Byte[] buffer = new Byte[BUFFER_SIZE];
                int readSize = BUFFER_SIZE;

                _TotalBytesRead = 0;
                int count = input.Read(buffer, 0, readSize);
                if (output != null) output.Write(buffer, 0, count);
                _TotalBytesRead += count;
                while (count > 0)
                {
                    SlurpBlock(buffer, 0, count);
                    count = input.Read(buffer, 0, readSize);
                    if (output != null) output.Write(buffer, 0, count);
                    _TotalBytesRead += count;
                }

                return (Int32)(~_register);
            }
        }

        public Int32 ComputeCrc32(Int32 W, Byte B)
        {
            return _InternalComputeCrc32((UInt32)W, B);
        }

        internal Int32 _InternalComputeCrc32(UInt32 W, Byte B)
        {
            return (Int32)(crc32Table[(W ^ B) & 0xFF] ^ (W >> 8));
        }

        public void SlurpBlock(Byte[] block, int offset, int count)
        {
            if (block == null)
                throw new Exception("The data buffer must not be null.");

            // bzip algorithm
            for (int i = 0; i < count; i++)
            {
                int x = offset + i;
                Byte b = block[x];
                if (reverseBits)
                {
                    UInt32 temp = (_register >> 24) ^ b;
                    _register = (_register << 8) ^ crc32Table[temp];
                }
                else
                {
                    UInt32 temp = (_register & 0x000000FF) ^ b;
                    _register = (_register >> 8) ^ crc32Table[temp];
                }
            }
            _TotalBytesRead += count;
        }

        public void UpdateCRC(Byte b)
        {
            if (reverseBits)
            {
                UInt32 temp = (_register >> 24) ^ b;
                _register = (_register << 8) ^ crc32Table[temp];
            }
            else
            {
                UInt32 temp = (_register & 0x000000FF) ^ b;
                _register = (_register >> 8) ^ crc32Table[temp];
            }
        }

        public void UpdateCRC(Byte b, int n)
        {
            while (n-- > 0)
            {
                if (reverseBits)
                {
                    uint temp = (_register >> 24) ^ b;
                    _register = (_register << 8) ^ crc32Table[(temp >= 0)
                                                                  ? temp
                                                                  : (temp + 256)];
                }
                else
                {
                    UInt32 temp = (_register & 0x000000FF) ^ b;
                    _register = (_register >> 8) ^ crc32Table[(temp >= 0)
                                                                  ? temp
                                                                  : (temp + 256)];
                }
            }
        }

        private static uint ReverseBits(uint data)
        {
            unchecked
            {
                uint ret = data;
                ret = (ret & 0x55555555) << 1 | (ret >> 1) & 0x55555555;
                ret = (ret & 0x33333333) << 2 | (ret >> 2) & 0x33333333;
                ret = (ret & 0x0F0F0F0F) << 4 | (ret >> 4) & 0x0F0F0F0F;
                ret = (ret << 24) | ((ret & 0xFF00) << 8) | ((ret >> 8) & 0xFF00) | (ret >> 24);
                return ret;
            }
        }

        private static Byte ReverseBits(Byte data)
        {
            unchecked
            {
                uint u = (uint)data * 0x00020202;
                uint m = 0x01044010;
                uint s = u & m;
                uint t = (u << 2) & (m << 1);
                return (Byte)((0x01001001 * (s + t)) >> 24);
            }
        }

        private void GenerateLookupTable()
        {
            crc32Table = new UInt32[256];
            unchecked
            {
                UInt32 dwCrc;
                Byte i = 0;
                do
                {
                    dwCrc = i;
                    for (Byte j = 8; j > 0; j--)
                    {
                        if ((dwCrc & 1) == 1)
                            dwCrc = (dwCrc >> 1) ^ dwPolynomial;
                        else
                            dwCrc >>= 1;
                    }
                    if (reverseBits)
                        crc32Table[ReverseBits(i)] = ReverseBits(dwCrc);
                    else
                        crc32Table[i] = dwCrc;
                    i++;
                } while (i != 0);
            }

#if VERBOSE
            Console.WriteLine();
            Console.WriteLine("private static readonly UInt32[] crc32Table = {");
            for (int i = 0; i < crc32Table.Length; i+=4)
            {
                Console.Write("   ");
                for (int j=0; j < 4; j++)
                {
                    Console.Write(" 0x{0:X8}U,", crc32Table[i+j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("};");
            Console.WriteLine();
#endif
        }

        private uint gf2_matrix_times(uint[] matrix, uint vec)
        {
            uint sum = 0;
            int i = 0;
            while (vec != 0)
            {
                if ((vec & 0x01) == 0x01)
                    sum ^= matrix[i];
                vec >>= 1;
                i++;
            }
            return sum;
        }

        private void gf2_matrix_square(uint[] square, uint[] mat)
        {
            for (int i = 0; i < 32; i++)
                square[i] = gf2_matrix_times(mat, mat[i]);
        }

        public void Combine(int crc, int length)
        {
            uint[] even = new uint[32]; // even-power-of-two zeros operator
            uint[] odd = new uint[32]; // odd-power-of-two zeros operator

            if (length == 0)
                return;

            uint crc1 = ~_register;
            uint crc2 = (uint)crc;

            // put operator for one zero bit in odd
            odd[0] = dwPolynomial; // the CRC-32 polynomial
            uint row = 1;
            for (int i = 1; i < 32; i++)
            {
                odd[i] = row;
                row <<= 1;
            }

            // put operator for two zero bits in even
            gf2_matrix_square(even, odd);

            // put operator for four zero bits in odd
            gf2_matrix_square(odd, even);

            uint len2 = (uint)length;

            // apply len2 zeros to crc1 (first square will put the operator for one
            // zero Byte, eight zero bits, in even)
            do
            {
                // apply zeros operator for this bit of len2
                gf2_matrix_square(even, odd);

                if ((len2 & 1) == 1)
                    crc1 = gf2_matrix_times(even, crc1);
                len2 >>= 1;

                if (len2 == 0)
                    break;

                // another iteration of the loop with odd and even swapped
                gf2_matrix_square(odd, even);
                if ((len2 & 1) == 1)
                    crc1 = gf2_matrix_times(odd, crc1);
                len2 >>= 1;
            } while (len2 != 0);

            crc1 ^= crc2;

            _register = ~crc1;

            //return (int) crc1;
            return;
        }

        public void Reset()
        {
            _register = 0xFFFFFFFFU;
        }

        #endregion
    }

    #endregion Класс ZipCRC32

    #region Класс ZipConstants

    internal static class ZipConstants
    {
        #region Константы

        public const UInt16 AesAlgId128 = 0x660E;

        public const UInt16 AesAlgId192 = 0x660F;

        public const UInt16 AesAlgId256 = 0x6610;

        public const int AesBlockSize = 128;

        public const int AesKeySize = 192;

        public const UInt32 EndOfCentralDirectorySignature = 0x06054b50;

        public const UInt32 PackedToRemovableMedia = 0x30304b50;

        public const int SplitArchiveSignature = 0x08074b50;

        public const int WindowBitsDefault = WindowBitsMax;

        public const int WindowBitsMax = 15;

        public const int WorkingBufferSizeDefault = 16384;

        public const int WorkingBufferSizeMin = 1024;

        public const int Z_BUF_ERROR = -5;

        public const int Z_DATA_ERROR = -3;

        public const int Z_NEED_DICT = 2;

        public const int Z_OK = 0;

        public const int Z_STREAM_END = 1;

        public const int Z_STREAM_ERROR = -2;

        public const UInt32 Zip64EndOfCentralDirectoryLocatorSignature = 0x07064b50;

        public const UInt32 Zip64EndOfCentralDirectoryRecordSignature = 0x06064b50;

        public const int ZipDirEntrySignature = 0x02014b50;

        public const int ZipEntryDataDescriptorSignature = 0x08074b50;

        public const int ZipEntrySignature = 0x04034b50;

        #endregion

        #region Поля

        internal static readonly int BL_CODES = 19;

        internal static readonly int D_CODES = 30;

        internal static readonly int[] InflateMask = new[]
            {
                0x00000000, 0x00000001, 0x00000003, 0x00000007,
                0x0000000f, 0x0000001f, 0x0000003f, 0x0000007f,
                0x000000ff, 0x000001ff, 0x000003ff, 0x000007ff,
                0x00000fff, 0x00001fff, 0x00003fff, 0x00007fff, 0x0000ffff
            };

        internal static readonly int LENGTH_CODES = 29;

        internal static readonly int LITERALS = 256;

        internal static readonly int L_CODES = (LITERALS + 1 + LENGTH_CODES);

        internal static readonly int MAX_BITS = 15;

        internal static readonly int MAX_BL_BITS = 7;

        internal static readonly int REPZ_11_138 = 18;

        internal static readonly int REPZ_3_10 = 17;

        internal static readonly int REP_3_6 = 16;

        #endregion
    }

    #endregion Класс ZipConstants

    #region Класс ZipProgressEventArgs

    public class ZipProgressEventArgs : EventArgs
    {
        #region Поля

        private Boolean _cancel;

        #endregion

        #region Свойства

        public int EntriesTotal { get; set; }

        public ZipEntry CurrentEntry { get; set; }

        public Boolean Cancel
        {
            get { return _cancel; }
            set { _cancel = _cancel || value; }
        }

        public ZipProgressEventType EventType { get; set; }

        public String ArchiveName { get; set; }

        public Int64 BytesTransferred { get; set; }

        public Int64 TotalBytesToTransfer { get; set; }

        #endregion

        #region Конструктор

        internal ZipProgressEventArgs()
        {
        }

        internal ZipProgressEventArgs(String archiveName, ZipProgressEventType flavor)
        {
            ArchiveName = archiveName;
            EventType = flavor;
        }

        #endregion
    }

    #endregion Класс ZipProgressEventArgs

    #region Класс ZipErrorEventArgs

    public class ZipErrorEventArgs : ZipProgressEventArgs
    {
        #region Поля

        private Exception _exc;

        #endregion

        #region Свойства

        public Exception @Exception
        {
            get { return _exc; }
        }

        public String FileName
        {
            get { return CurrentEntry.LocalFileName; }
        }

        #endregion

        #region Конструктор

        private ZipErrorEventArgs()
        {
        }

        #endregion

        #region Методы

        internal static ZipErrorEventArgs Saving(String archiveName, ZipEntry entry, Exception exception)
        {
            var x = new ZipErrorEventArgs
                {
                    EventType = ZipProgressEventType.Error_Saving,
                    ArchiveName = archiveName,
                    CurrentEntry = entry,
                    _exc = exception
                };
            return x;
        }

        #endregion
    }

    #endregion Класс ZipErrorEventArgs

    #region Класс ZipReadProgressEventArgs

    public class ZipReadProgressEventArgs : ZipProgressEventArgs
    {
        #region Конструктор

        internal ZipReadProgressEventArgs()
        {
        }

        private ZipReadProgressEventArgs(String archiveName, ZipProgressEventType flavor) : base(archiveName, flavor)
        {
        }

        #endregion

        #region Методы

        internal static ZipReadProgressEventArgs Before(String archiveName, int entriesTotal)
        {
            var x = new ZipReadProgressEventArgs(archiveName, ZipProgressEventType.Reading_BeforeReadEntry);
            x.EntriesTotal = entriesTotal;
            return x;
        }

        internal static ZipReadProgressEventArgs After(String archiveName, ZipEntry entry, int entriesTotal)
        {
            var x = new ZipReadProgressEventArgs(archiveName, ZipProgressEventType.Reading_AfterReadEntry);
            x.EntriesTotal = entriesTotal;
            x.CurrentEntry = entry;
            return x;
        }

        internal static ZipReadProgressEventArgs Started(String archiveName)
        {
            var x = new ZipReadProgressEventArgs(archiveName, ZipProgressEventType.Reading_Started);
            return x;
        }

        internal static ZipReadProgressEventArgs ByteUpdate(String archiveName, ZipEntry entry, Int64 BytesXferred, Int64 totalBytes)
        {
            var x = new ZipReadProgressEventArgs(archiveName, ZipProgressEventType.Reading_ArchiveBytesRead);
            x.CurrentEntry = entry;
            x.BytesTransferred = BytesXferred;
            x.TotalBytesToTransfer = totalBytes;
            return x;
        }

        internal static ZipReadProgressEventArgs Completed(String archiveName)
        {
            var x = new ZipReadProgressEventArgs(archiveName, ZipProgressEventType.Reading_Completed);
            return x;
        }

        #endregion
    }

    #endregion Класс ZipReadProgressEventArgs

    #region Класс ZipAddProgressEventArgs

    public class ZipAddProgressEventArgs : ZipProgressEventArgs
    {
        #region Конструктор

        internal ZipAddProgressEventArgs()
        {
        }

        private ZipAddProgressEventArgs(String archiveName, ZipProgressEventType flavor) : base(archiveName, flavor)
        {
        }

        #endregion

        #region Методы

        internal static ZipAddProgressEventArgs AfterEntry(String archiveName, ZipEntry entry, int entriesTotal)
        {
            var x = new ZipAddProgressEventArgs(archiveName, ZipProgressEventType.Adding_AfterAddEntry);
            x.EntriesTotal = entriesTotal;
            x.CurrentEntry = entry;
            return x;
        }

        internal static ZipAddProgressEventArgs Started(String archiveName)
        {
            var x = new ZipAddProgressEventArgs(archiveName, ZipProgressEventType.Adding_Started);
            return x;
        }

        internal static ZipAddProgressEventArgs Completed(String archiveName)
        {
            var x = new ZipAddProgressEventArgs(archiveName, ZipProgressEventType.Adding_Completed);
            return x;
        }

        #endregion
    }

    #endregion Класс ZipAddProgressEventArgs

    #region Класс ZipSaveProgressEventArgs

    public class ZipSaveProgressEventArgs : ZipProgressEventArgs
    {
        #region Поля

        private int _entriesSaved;

        #endregion

        #region Свойства

        public int EntriesSaved
        {
            get { return _entriesSaved; }
        }

        #endregion

        #region Конструктор

        internal ZipSaveProgressEventArgs()
        {
        }

        internal ZipSaveProgressEventArgs(String archiveName, ZipProgressEventType flavor) : base(archiveName, flavor)
        {
        }

        internal ZipSaveProgressEventArgs(String archiveName, Boolean before, int entriesTotal, int entriesSaved, ZipEntry entry)
            : base(archiveName, (before) ? ZipProgressEventType.Saving_BeforeWriteEntry : ZipProgressEventType.Saving_AfterWriteEntry)
        {
            EntriesTotal = entriesTotal;
            CurrentEntry = entry;
            _entriesSaved = entriesSaved;
        }

        #endregion

        #region Методы

        internal static ZipSaveProgressEventArgs ByteUpdate(String archiveName, ZipEntry entry, Int64 BytesXferred, Int64 totalBytes)
        {
            var x = new ZipSaveProgressEventArgs(archiveName, ZipProgressEventType.Saving_EntryBytesRead);
            x.ArchiveName = archiveName;
            x.CurrentEntry = entry;
            x.BytesTransferred = BytesXferred;
            x.TotalBytesToTransfer = totalBytes;
            return x;
        }

        internal static ZipSaveProgressEventArgs Started(String archiveName)
        {
            var x = new ZipSaveProgressEventArgs(archiveName, ZipProgressEventType.Saving_Started);
            return x;
        }

        internal static ZipSaveProgressEventArgs Completed(String archiveName)
        {
            var x = new ZipSaveProgressEventArgs(archiveName, ZipProgressEventType.Saving_Completed);
            return x;
        }

        #endregion
    }

    #endregion Класс ZipSaveProgressEventArgs

    #region Класс ZipExtractProgressEventArgs

    public class ZipExtractProgressEventArgs : ZipProgressEventArgs
    {
        #region Поля

        private int _entriesExtracted;

        private String _target;

        #endregion

        #region Свойства

        public int EntriesExtracted
        {
            get { return _entriesExtracted; }
        }

        public String ExtractLocation
        {
            get { return _target; }
        }

        #endregion

        #region Конструктор

        internal ZipExtractProgressEventArgs()
        {
        }

        internal ZipExtractProgressEventArgs(String archiveName, ZipProgressEventType flavor) : base(archiveName, flavor)
        {
        }

        internal ZipExtractProgressEventArgs(String archiveName, Boolean before, int entriesTotal, int entriesExtracted, ZipEntry entry, String extractLocation)
            : base(archiveName, (before) ? ZipProgressEventType.Extracting_BeforeExtractEntry : ZipProgressEventType.Extracting_AfterExtractEntry)
        {
            EntriesTotal = entriesTotal;
            CurrentEntry = entry;
            _entriesExtracted = entriesExtracted;
            _target = extractLocation;
        }

        #endregion

        #region Методы

        internal static ZipExtractProgressEventArgs BeforeExtractEntry(String archiveName, ZipEntry entry, String extractLocation)
        {
            var x = new ZipExtractProgressEventArgs
                {
                    ArchiveName = archiveName,
                    EventType = ZipProgressEventType.Extracting_BeforeExtractEntry,
                    CurrentEntry = entry,
                    _target = extractLocation,
                };
            return x;
        }

        internal static ZipExtractProgressEventArgs ExtractExisting(String archiveName, ZipEntry entry, String extractLocation)
        {
            var x = new ZipExtractProgressEventArgs
                {
                    ArchiveName = archiveName,
                    EventType = ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite,
                    CurrentEntry = entry,
                    _target = extractLocation,
                };
            return x;
        }

        internal static ZipExtractProgressEventArgs AfterExtractEntry(String archiveName, ZipEntry entry, String extractLocation)
        {
            var x = new ZipExtractProgressEventArgs
                {
                    ArchiveName = archiveName,
                    EventType = ZipProgressEventType.Extracting_AfterExtractEntry,
                    CurrentEntry = entry,
                    _target = extractLocation,
                };
            return x;
        }

        internal static ZipExtractProgressEventArgs ExtractAllStarted(String archiveName, String extractLocation)
        {
            var x = new ZipExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_BeforeExtractAll);
            x._target = extractLocation;
            return x;
        }

        internal static ZipExtractProgressEventArgs ExtractAllCompleted(String archiveName, String extractLocation)
        {
            var x = new ZipExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_AfterExtractAll);
            x._target = extractLocation;
            return x;
        }

        internal static ZipExtractProgressEventArgs ByteUpdate(String archiveName, ZipEntry entry, Int64 BytesWritten, Int64 totalBytes)
        {
            var x = new ZipExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_EntryBytesWritten);
            x.ArchiveName = archiveName;
            x.CurrentEntry = entry;
            x.BytesTransferred = BytesWritten;
            x.TotalBytesToTransfer = totalBytes;
            return x;
        }

        #endregion
    }

    #endregion Класс ZipExtractProgressEventArgs

    #region Класс ZipException

    [Serializable]
    [Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d00006")]
    public class ZipException : Exception
    {
        #region Конструктор

        public ZipException()
        {
        }

        public ZipException(String message) : base(message)
        {
        }

        public ZipException(String message, Exception innerException) : base(message, innerException)
        {
        }

        protected ZipException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }

    #endregion Класс ZipException

    #region Класс ZlibException

    [Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d0000E")]
    public class ZlibException : System.Exception
    {
        #region Конструктор

        public ZlibException()
        {
        }

        public ZlibException(System.String s) : base(s)
        {
        }

        #endregion
    }

    #endregion Класс ZlibException

    #region Класс ZipBadReadException

    [Serializable]
    [Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d0000A")]
    public class ZipBadReadException : ZipException
    {
        #region Конструктор

        public ZipBadReadException()
        {
        }

        public ZipBadReadException(String message) : base(message)
        {
        }

        public ZipBadReadException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ZipBadReadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }

    #endregion Класс ZipBadReadException

    #region Класс ZipBadCrcException

    [Serializable]
    [Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d00009")]
    public class ZipBadCrcException : ZipException
    {
        #region Конструктор

        public ZipBadCrcException()
        {
        }

        public ZipBadCrcException(String message) : base(message)
        {
        }

        protected ZipBadCrcException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }

    #endregion Класс ZipBadCrcException

    #region Класс ZipBadStateException

    [Serializable]
    [Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d00007")]
    public class ZipBadStateException : ZipException
    {
        #region Конструктор

        public ZipBadStateException()
        {
        }

        public ZipBadStateException(String message) : base(message)
        {
        }

        public ZipBadStateException(String message, Exception innerException) : base(message, innerException)
        {
        }

        protected ZipBadStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }

    #endregion Класс ZipBadStateException

    #region Класс ZipBadPasswordException

    [Serializable]
    [Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d0000B")]
    public class ZipBadPasswordException : ZipException
    {
        #region Конструктор

        public ZipBadPasswordException()
        {
        }

        public ZipBadPasswordException(String message) : base(message)
        {
        }

        public ZipBadPasswordException(String message, Exception innerException) : base(message, innerException)
        {
        }

        protected ZipBadPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }

    #endregion Класс ZipBadPasswordException

    #region Класс ZipCountingStream

    public class ZipCountingStream : System.IO.Stream
    {
        #region Поля

        private Int64 _bytesRead;

        private Int64 _bytesWritten;

        private Int64 _initialOffset;

        private System.IO.Stream _s;

        #endregion

        #region Свойства

        public Stream WrappedStream
        {
            get { return _s; }
        }

        public Int64 BytesWritten
        {
            get { return _bytesWritten; }
        }

        public Int64 BytesRead
        {
            get { return _bytesRead; }
        }

        public override Boolean CanRead
        {
            get { return _s.CanRead; }
        }

        public override Boolean CanSeek
        {
            get { return _s.CanSeek; }
        }

        public override Boolean CanWrite
        {
            get { return _s.CanWrite; }
        }

        public override long Length
        {
            get { return _s.Length; } // BytesWritten??
        }

        public long ComputedPosition
        {
            get { return _initialOffset + _bytesWritten; }
        }

        public override long Position
        {
            get { return _s.Position; }
            set
            {
                _s.Seek(value, System.IO.SeekOrigin.Begin);
                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(_s);
            }
        }

        #endregion

        #region Конструктор

        public ZipCountingStream(System.IO.Stream stream)
        {
            _s = stream;
            try
            {
                _initialOffset = _s.Position;
            }
            catch
            {
                _initialOffset = 0L;
            }
        }

        #endregion

        #region Методы

        public override void Flush()
        {
            _s.Flush();
        }

        public void Adjust(Int64 delta)
        {
            _bytesWritten -= delta;
            if (_bytesWritten < 0)
                throw new InvalidOperationException();
            if (_s as ZipCountingStream != null)
                ((ZipCountingStream)_s).Adjust(delta);
        }

        public override int Read(Byte[] buffer, int offset, int count)
        {
            int n = _s.Read(buffer, offset, count);
            _bytesRead += n;
            return n;
        }

        public override void Write(Byte[] buffer, int offset, int count)
        {
            if (count == 0) return;
            _s.Write(buffer, offset, count);
            _bytesWritten += count;
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            return _s.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _s.SetLength(value);
        }

        #endregion
    }

    #endregion Класс ZipCountingStream

    #region Класс ZipOffsetStream

    internal class ZipOffsetStream : System.IO.Stream, System.IDisposable
    {
        #region Поля

        private Stream _innerStream;

        private Int64 _originalPosition;

        #endregion

        #region Свойства

        public override Boolean CanRead
        {
            get { return _innerStream.CanRead; }
        }

        public override Boolean CanSeek
        {
            get { return _innerStream.CanSeek; }
        }

        public override Boolean CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _innerStream.Length; }
        }

        public override long Position
        {
            get { return _innerStream.Position - _originalPosition; }
            set { _innerStream.Position = _originalPosition + value; }
        }

        #endregion

        #region Конструктор

        public ZipOffsetStream(Stream s)
        {
            _originalPosition = s.Position;
            _innerStream = s;
        }

        #endregion

        #region Методы

        public override int Read(Byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        public override void Write(Byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            return _innerStream.Seek(_originalPosition + offset, origin) - _originalPosition;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public override void Close()
        {
            base.Close();
        }

        #endregion
    }

    #endregion Класс ZipOffsetStream

    #region Класс ZipCrcCalculatorStream

    public class ZipCrcCalculatorStream : System.IO.Stream, System.IDisposable
    {
        #region Поля

        private static readonly Int64 _unsetLengthLimit = -99;

        private ZipCRC32 _crc32;

        internal System.IO.Stream _innerStream;

        private Boolean _leaveOpen;

        private Int64 _lengthLimit = -99;

        #endregion

        #region Свойства

        public Int64 TotalBytesSlurped
        {
            get { return _crc32.TotalBytesRead; }
        }

        public Int32 Crc
        {
            get { return _crc32.Crc32Result; }
        }

        public Boolean LeaveOpen
        {
            get { return _leaveOpen; }
            set { _leaveOpen = value; }
        }

        public override Boolean CanRead
        {
            get { return _innerStream.CanRead; }
        }

        public override Boolean CanSeek
        {
            get { return false; }
        }

        public override Boolean CanWrite
        {
            get { return _innerStream.CanWrite; }
        }

        public override long Length
        {
            get
            {
                if (_lengthLimit == ZipCrcCalculatorStream._unsetLengthLimit)
                    return _innerStream.Length;
                else return _lengthLimit;
            }
        }

        public override long Position
        {
            get { return _crc32.TotalBytesRead; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Конструктор

        public ZipCrcCalculatorStream(System.IO.Stream stream) : this(true, ZipCrcCalculatorStream._unsetLengthLimit, stream, null)
        {
        }

        public ZipCrcCalculatorStream(System.IO.Stream stream, Boolean leaveOpen) : this(leaveOpen, ZipCrcCalculatorStream._unsetLengthLimit, stream, null)
        {
        }

        public ZipCrcCalculatorStream(System.IO.Stream stream, Int64 length) : this(true, length, stream, null)
        {
            if (length < 0)
                throw new ArgumentException("length");
        }

        public ZipCrcCalculatorStream(System.IO.Stream stream, Int64 length, Boolean leaveOpen) : this(leaveOpen, length, stream, null)
        {
            if (length < 0)
                throw new ArgumentException("length");
        }

        public ZipCrcCalculatorStream(System.IO.Stream stream, Int64 length, Boolean leaveOpen, ZipCRC32 crc32) : this(leaveOpen, length, stream, crc32)
        {
            if (length < 0)
                throw new ArgumentException("length");
        }

        private ZipCrcCalculatorStream(Boolean leaveOpen, Int64 length, System.IO.Stream stream, ZipCRC32 crc32)
        {
            _innerStream = stream;
            _crc32 = crc32 ?? new ZipCRC32();
            _lengthLimit = length;
            _leaveOpen = leaveOpen;
        }

        #endregion

        #region Методы

        public override int Read(Byte[] buffer, int offset, int count)
        {
            int BytesToRead = count;

            // Need to limit the # of Bytes returned, if the stream is intended to have
            // a definite length.  This is especially useful when returning a stream for
            // the uncompressed data directly to the application.  The app won't
            // necessarily read only the UncompressedSize number of Bytes.  For example
            // wrapping the stream returned from OpenReader() into a StreadReader() and
            // calling ReadToEnd() on it, We can "over-read" the zip data and get a
            // corrupt String.  The length limits that, prevents that problem.

            if (_lengthLimit != ZipCrcCalculatorStream._unsetLengthLimit)
            {
                if (_crc32.TotalBytesRead >= _lengthLimit) return 0; // EOF
                Int64 BytesRemaining = _lengthLimit - _crc32.TotalBytesRead;
                if (BytesRemaining < count) BytesToRead = (int)BytesRemaining;
            }
            int n = _innerStream.Read(buffer, offset, BytesToRead);
            if (n > 0) _crc32.SlurpBlock(buffer, offset, n);
            return n;
        }

        public override void Write(Byte[] buffer, int offset, int count)
        {
            if (count > 0) _crc32.SlurpBlock(buffer, offset, count);
            _innerStream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public override void Close()
        {
            base.Close();
            if (!_leaveOpen)
                _innerStream.Close();
        }

        #endregion
    }

    #endregion Класс ZipCrcCalculatorStream

    #region Класс ZipInputStream

    public class ZipInputStream : Stream
    {
        #region Поля

        private Int64 _LeftToRead;

        private Boolean _closed;

        private ZipContainer _container;

        private ZipCrcCalculatorStream _crcStream;

        private ZipEntry _currentEntry;

        private Int64 _endOfEntry;

        private Boolean _exceptionPending;

        private Boolean _findRequired;

        private Boolean _firstEntry;

        private Stream _inputStream;

        private Boolean _leaveUnderlyingStreamOpen;

        private String _name;

        private Boolean _needSetup;

        internal String _password;

        private System.Text.Encoding _provisionalAlternateEncoding;

        #endregion

        #region Свойства

        internal Stream ReadStream
        {
            get { return _inputStream; }
        }

        public System.Text.Encoding ProvisionalAlternateEncoding
        {
            get { return _provisionalAlternateEncoding; }
            set { _provisionalAlternateEncoding = value; }
        }

        public int CodecBufferSize { get; set; }

        public String Password
        {
            set
            {
                if (_closed)
                {
                    _exceptionPending = true;
                    throw new System.InvalidOperationException("The stream has been closed.");
                }
                _password = value;
            }
        }

        public override Boolean CanRead
        {
            get { return true; }
        }

        public override Boolean CanSeek
        {
            get { return _inputStream.CanSeek; }
        }

        public override Boolean CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _inputStream.Length; }
        }

        public override long Position
        {
            get { return _inputStream.Position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        #endregion

        #region Конструктор

        public ZipInputStream(Stream stream) : this(stream, false)
        {
        }

        public ZipInputStream(String fileName)
        {
            Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            _Init(stream, false, fileName);
        }

        public ZipInputStream(Stream stream, Boolean leaveOpen)
        {
            _Init(stream, leaveOpen, null);
        }

        #endregion

        #region Методы

        private void _Init(Stream stream, Boolean leaveOpen, String name)
        {
            _inputStream = stream;
            if (!_inputStream.CanRead)
                throw new ZipException("The stream must be readable.");
            _container = new ZipContainer(this);
            _provisionalAlternateEncoding = System.Text.Encoding.GetEncoding("IBM437");
            _leaveUnderlyingStreamOpen = leaveOpen;
            _findRequired = true;
            _name = name ?? "(stream)";
        }

        public override String ToString()
        {
            return String.Format("ZipInputStream::{0}(leaveOpen({1})))", _name, _leaveUnderlyingStreamOpen);
        }

        private void SetupStream()
        {
            // Seek to the correct posn in the file, and open a
            // stream that can be read.
            _crcStream = _currentEntry.InternalOpenReader(_password);
            _LeftToRead = _crcStream.Length;
            _needSetup = false;
        }

        public override int Read(Byte[] buffer, int offset, int count)
        {
            if (_closed)
            {
                _exceptionPending = true;
                throw new System.InvalidOperationException("The stream has been closed.");
            }

            if (_needSetup)
                SetupStream();

            if (_LeftToRead == 0) return 0;

            int len = (_LeftToRead > count) ? count : (int)_LeftToRead;
            int n = _crcStream.Read(buffer, offset, len);

            _LeftToRead -= n;

            if (_LeftToRead == 0)
            {
                int CrcResult = _crcStream.Crc;
                _currentEntry.VerifyCrcAfterExtract(CrcResult);
                _inputStream.Seek(_endOfEntry, SeekOrigin.Begin);
                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(_inputStream);
            }

            return n;
        }

        public ZipEntry GetNextEntry()
        {
            if (_findRequired)
            {
                // find the next signature
                long d = ZipSharedUtilities.FindSignature(_inputStream, ZipConstants.ZipEntrySignature);
                if (d == -1) return null;
                // back up 4 Bytes: ReadEntry assumes the file pointer is positioned before the entry signature
                _inputStream.Seek(-4, SeekOrigin.Current);
                // workitem 10178
                ZipSharedUtilities.Workaround_Ladybug318918(_inputStream);
            }
                // workitem 10923
            else if (_firstEntry)
            {
                // we've already read one entry.
                // Seek to the end of it.
                _inputStream.Seek(_endOfEntry, SeekOrigin.Begin);
                ZipSharedUtilities.Workaround_Ladybug318918(_inputStream);
            }

            _currentEntry = ZipEntry.ReadEntry(_container, !_firstEntry);
            // ReadEntry leaves the file position after all the entry
            // data and the optional bit-3 data descriptpr.  This is
            // where the next entry would normally start.
            _endOfEntry = _inputStream.Position;
            _firstEntry = true;
            _needSetup = true;
            _findRequired = false;
            return _currentEntry;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (_closed) return;

            if (disposing) // not called from finalizer
            {
                // When ZipInputStream is used within a using clause, and an
                // exception is thrown, Close() is invoked.  But we don't want to
                // try to write anything in that case.  Eventually the exception
                // will be propagated to the application.
                if (_exceptionPending) return;

                if (!_leaveUnderlyingStreamOpen)
                    _inputStream.Dispose();
            }
            _closed = true;
        }

        public override void Flush()
        {
            throw new NotSupportedException("Flush");
        }

        public override void Write(Byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Write");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            _findRequired = true;
            var x = _inputStream.Seek(offset, origin);
            // workitem 10178
            ZipSharedUtilities.Workaround_Ladybug318918(_inputStream);
            return x;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс ZipInputStream

    #region Класс ZipCipherStream

    internal class ZipCipherStream : System.IO.Stream
    {
        #region Поля

        private ZipCrypto _cipher;

        private ZipCryptoMode _mode;

        private System.IO.Stream _s;

        #endregion

        #region Свойства

        public override Boolean CanRead
        {
            get { return (_mode == ZipCryptoMode.Decrypt); }
        }

        public override Boolean CanSeek
        {
            get { return false; }
        }

        public override Boolean CanWrite
        {
            get { return (_mode == ZipCryptoMode.Encrypt); }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Конструктор

        public ZipCipherStream(System.IO.Stream s, ZipCrypto cipher, ZipCryptoMode mode)
        {
            _cipher = cipher;
            _s = s;
            _mode = mode;
        }

        #endregion

        #region Методы

        public override int Read(Byte[] buffer, int offset, int count)
        {
            if (_mode == ZipCryptoMode.Encrypt)
                throw new NotSupportedException("This stream does not encrypt via Read()");

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            Byte[] db = new Byte[count];
            int n = _s.Read(db, 0, count);
            Byte[] decrypted = _cipher.DecryptMessage(db, n);
            for (int i = 0; i < n; i++)
                buffer[offset + i] = decrypted[i];
            return n;
        }

        public override void Write(Byte[] buffer, int offset, int count)
        {
            if (_mode == ZipCryptoMode.Decrypt)
                throw new NotSupportedException("This stream does not Decrypt via Write()");

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            // workitem 7696
            if (count == 0) return;

            Byte[] plaintext = null;
            if (offset != 0)
            {
                plaintext = new Byte[count];
                for (int i = 0; i < count; i++)
                    plaintext[i] = buffer[offset + i];
            }
            else plaintext = buffer;

            Byte[] encrypted = _cipher.EncryptMessage(plaintext, count);
            _s.Write(encrypted, 0, encrypted.Length);
        }

        public override void Flush()
        {
            //throw new NotSupportedException();
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс ZipCipherStream

    #region Класс ZipDeflateStream

    public class ZipDeflateStream : System.IO.Stream
    {
        #region Поля

        internal ZipBaseStream _baseStream;

        private Boolean _disposed;

        internal System.IO.Stream _innerStream;

        #endregion

        #region Свойства

        public virtual ZipFlushType FlushMode
        {
            get { return (_baseStream._flushMode); }
            set
            {
                if (_disposed) throw new ObjectDisposedException("DeflateStream");
                _baseStream._flushMode = value;
            }
        }

        public int BufferSize
        {
            get { return _baseStream._bufferSize; }
            set
            {
                if (_disposed) throw new ObjectDisposedException("DeflateStream");
                if (_baseStream._workingBuffer != null)
                    throw new ZlibException("The working buffer is already set.");
                if (value < ZipConstants.WorkingBufferSizeMin)
                    throw new ZlibException(String.Format("Don't be silly. {0} Bytes?? Use a bigger buffer, at least {1}.", value, ZipConstants.WorkingBufferSizeMin));
                _baseStream._bufferSize = value;
            }
        }

        public ZipCompressionStrategy Strategy
        {
            get { return _baseStream.Strategy; }
            set
            {
                if (_disposed) throw new ObjectDisposedException("DeflateStream");
                _baseStream.Strategy = value;
            }
        }

        public virtual long TotalIn
        {
            get { return _baseStream._codec.TotalBytesIn; }
        }

        public virtual long TotalOut
        {
            get { return _baseStream._codec.TotalBytesOut; }
        }

        public override Boolean CanRead
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("DeflateStream");
                return _baseStream._stream.CanRead;
            }
        }

        public override Boolean CanSeek
        {
            get { return false; }
        }

        public override Boolean CanWrite
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("DeflateStream");
                return _baseStream._stream.CanWrite;
            }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                if (_baseStream._streamMode == ZipBaseStream.StreamMode.Writer)
                    return _baseStream._codec.TotalBytesOut;
                if (_baseStream._streamMode == ZipBaseStream.StreamMode.Reader)
                    return _baseStream._codec.TotalBytesIn;
                return 0;
            }
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region Конструктор

        public ZipDeflateStream(System.IO.Stream stream, ZipCompressionMode mode) : this(stream, mode, ZipCompressionLevel.Default, false)
        {
        }

        public ZipDeflateStream(System.IO.Stream stream, ZipCompressionMode mode, ZipCompressionLevel level) : this(stream, mode, level, false)
        {
        }

        public ZipDeflateStream(System.IO.Stream stream, ZipCompressionMode mode, Boolean leaveOpen) : this(stream, mode, ZipCompressionLevel.Default, leaveOpen)
        {
        }

        public ZipDeflateStream(System.IO.Stream stream, ZipCompressionMode mode, ZipCompressionLevel level, Boolean leaveOpen)
        {
            _innerStream = stream;
            _baseStream = new ZipBaseStream(stream, mode, level, ZlibStreamFlavor.DEFLATE, leaveOpen);
        }

        #endregion

        #region Методы

        protected override void Dispose(Boolean disposing)
        {
            try
            {
                if (!_disposed)
                {
                    if (disposing && (_baseStream != null))
                        _baseStream.Close();
                    _disposed = true;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            if (_disposed) throw new ObjectDisposedException("DeflateStream");
            _baseStream.Flush();
        }

        public override int Read(Byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException("DeflateStream");
            return _baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(Byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException("DeflateStream");
            _baseStream.Write(buffer, offset, count);
        }

        public static Byte[] CompressString(String s)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                System.IO.Stream compressor =
                    new ZipDeflateStream(ms, ZipCompressionMode.Compress, ZipCompressionLevel.BestCompression);
                ZipBaseStream.CompressString(s, compressor);
                return ms.ToArray();
            }
        }

        public static Byte[] CompressBuffer(Byte[] b)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                System.IO.Stream compressor =
                    new ZipDeflateStream(ms, ZipCompressionMode.Compress, ZipCompressionLevel.BestCompression);

                ZipBaseStream.CompressBuffer(b, compressor);
                return ms.ToArray();
            }
        }

        public static String UncompressString(Byte[] compressed)
        {
            using (var input = new System.IO.MemoryStream(compressed))
            {
                System.IO.Stream decompressor =
                    new ZipDeflateStream(input, ZipCompressionMode.Decompress);

                return ZipBaseStream.UncompressString(compressed, decompressor);
            }
        }

        public static Byte[] UncompressBuffer(Byte[] compressed)
        {
            using (var input = new System.IO.MemoryStream(compressed))
            {
                System.IO.Stream decompressor =
                    new ZipDeflateStream(input, ZipCompressionMode.Decompress);

                return ZipBaseStream.UncompressBuffer(compressed, decompressor);
            }
        }

        #endregion
    }

    #endregion Класс ZipDeflateStream

    #region Класс BZip2WorkItem

    internal class BZip2WorkItem
    {
        #region Поля

        public ZipBitWriter _bw;

        public int _index;

        public MemoryStream _ms;

        #endregion

        #region Свойства

        public BZip2Compressor Compressor { get; private set; }

        #endregion

        #region Конструктор

        public BZip2WorkItem(int ix, int blockSize)
        {
            _ms = new MemoryStream();
            _bw = new ZipBitWriter(_ms);
            Compressor = new BZip2Compressor(_bw, blockSize);
            _index = ix;
        }

        #endregion
    }

    #endregion Класс BZip2WorkItem

    #region Класс ZipWorkItem

    internal class ZipWorkItem
    {
        #region Поля

        public Byte[] _buffer;

        public Byte[] _compressed;

        public int _compressedBytesAvailable;

        public ZlibCodec _compressor;

        public int _crc;

        public int _index;

        public int _inputBytesAvailable;

        public int _ordinal;

        #endregion

        #region Конструктор

        public ZipWorkItem(int size, ZipCompressionLevel compressLevel, ZipCompressionStrategy strategy, int ix)
        {
            _buffer = new Byte[size];
            // alloc 5 Bytes overhead for every block (margin of safety= 2)
            int n = size + ((size / 32768) + 1) * 5 * 2;
            _compressed = new Byte[n];
            _compressor = new ZlibCodec();
            _compressor.InitializeDeflate(compressLevel, false);
            _compressor.OutputBuffer = _compressed;
            _compressor.InputBuffer = _buffer;
            _index = ix;
        }

        #endregion
    }

    #endregion Класс ZipWorkItem

    #region Класс ZipBitWriter

    internal class ZipBitWriter
    {
        #region Поля

        private uint _accumulator;

        private int _nAccumulatedBits;

        private Stream _output;

        private int _totalBytesWrittenOut;

        #endregion

        #region Свойства

        public Byte RemainingBits
        {
            get { return (Byte)(_accumulator >> (32 - _nAccumulatedBits) & 0xff); }
        }

        public int NumRemainingBits
        {
            get { return _nAccumulatedBits; }
        }

        public int TotalBytesWrittenOut
        {
            get { return _totalBytesWrittenOut; }
        }

        #endregion

        #region Конструктор

        public ZipBitWriter(Stream s)
        {
            _output = s;
        }

        #endregion

        #region Методы

        public void Reset()
        {
            _accumulator = 0;
            _nAccumulatedBits = 0;
            _totalBytesWrittenOut = 0;
            _output.Seek(0, SeekOrigin.Begin);
            _output.SetLength(0);
        }

        public void WriteBits(int nbits, uint value)
        {
            int nAccumulated = _nAccumulatedBits;
            uint u = _accumulator;

            while (nAccumulated >= 8)
            {
                _output.WriteByte((Byte)(u >> 24 & 0xff));
                _totalBytesWrittenOut++;
                u <<= 8;
                nAccumulated -= 8;
            }

            _accumulator = u | (value << (32 - nAccumulated - nbits));
            _nAccumulatedBits = nAccumulated + nbits;

            // Console.WriteLine("WriteBits({0}, 0x{1:X2}) => {2:X8} n({3})",
            //                   nbits, value, accumulator, nAccumulatedBits);
            // Console.ReadLine();

            // At this point the accumulator may contain up to 31 bits waiting for
            // output.
        }

        public void WriteByte(Byte b)
        {
            WriteBits(8, b);
        }

        public void WriteInt(uint u)
        {
            WriteBits(8, (u >> 24) & 0xff);
            WriteBits(8, (u >> 16) & 0xff);
            WriteBits(8, (u >> 8) & 0xff);
            WriteBits(8, u & 0xff);
        }

        public void Flush()
        {
            WriteBits(0, 0);
        }

        public void FinishAndPad()
        {
            Flush();

            if (NumRemainingBits > 0)
            {
                Byte b = (Byte)((_accumulator >> 24) & 0xff);
                _output.WriteByte(b);
                _totalBytesWrittenOut++;
            }
        }

        #endregion
    }

    #endregion Класс ZipBitWriter

    #region Класс BZip2Compressor

    internal class BZip2Compressor
    {
        #region Поля

        private static readonly int CLEARMASK = (~SETMASK);

        private static readonly int DEPTH_THRESH = 10;

        private static readonly Byte GREATER_ICOST = 15;

        private static readonly Byte LESSER_ICOST = 0;

        private static readonly int SETMASK = (1 << 21);

        private static readonly int SMALL_THRESH = 20;

        private static readonly int WORK_FACTOR = 30;

        private static readonly int[] increments =
            {
                1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524, 88573, 265720, 797161, 2391484
            };

        private readonly ZipCRC32 _crc = new ZipCRC32(true);

        private Boolean _blockRandomised;

        private int _blockSize100k;

        private ZipBitWriter _bw;

        private CompressionState _cstate;

        private int _currentByte = -1;

        private Boolean _firstAttempt;

        private int _last;

        private int _nInUse;

        private int _nMTF;

        private int _origPtr;

        private int _outBlockFillThreshold;

        private int _runLength;

        private int _runs;

        private int _workDone;

        private int _workLimit;

        #endregion

        #region Свойства

        public int BlockSize
        {
            get { return _blockSize100k; }
        }

        public uint Crc32 { get; private set; }

        public int AvailableBytesOut { get; private set; }

        public int UncompressedBytes
        {
            get { return _last + 1; }
        }

        #endregion

        #region Конструктор

        public BZip2Compressor(ZipBitWriter writer) : this(writer, BZip2.MaxBlockSize)
        {
        }

        public BZip2Compressor(ZipBitWriter writer, int blockSize)
        {
            _blockSize100k = blockSize;
            _bw = writer;

            // 20 provides a margin of slop (not to say "Safety"). The maximum
            // size of an encoded run in the output block is 5 Bytes, so really, 5
            // Bytes ought to do, but this is a margin of slop found in the
            // original bzip code. Not sure if important for decoding
            // (decompressing).  So we'll leave the slop.
            _outBlockFillThreshold = (blockSize * BZip2.BlockSizeMultiple) - 20;
            _cstate = new CompressionState(blockSize);
            Reset();
        }

        #endregion

        #region Методы

        private void Reset()
        {
            // initBlock();
            _crc.Reset();
            _currentByte = -1;
            _runLength = 0;
            _last = -1;
            for (int i = 256; --i >= 0;)
                _cstate.inUse[i] = false;
            //bw.Reset();  xxx? want this?  no no no
        }

        public int Fill(Byte[] buffer, int offset, int count)
        {
            if (_last >= _outBlockFillThreshold)
                return 0; // We're full, I tell you!

            int BytesWritten = 0;
            int limit = offset + count;
            int rc;

            // do run-length-encoding until block is full
            do
            {
                rc = write0(buffer[offset++]);
                if (rc > 0) BytesWritten++;
            } while (offset < limit && rc == 1);

            return BytesWritten;
        }

        private int write0(Byte b)
        {
            Boolean rc;
            // there is no current run in progress
            if (_currentByte == -1)
            {
                _currentByte = b;
                _runLength++;
                return 1;
            }

            // this Byte is the same as the current run in progress
            if (_currentByte == b)
            {
                if (++_runLength > 254)
                {
                    rc = AddRunToOutputBlock(false);
                    _currentByte = -1;
                    _runLength = 0;
                    return (rc) ? 2 : 1;
                }
                return 1; // not full
            }

            // This Byte requires a new run.
            // Put the prior run into the Run-length-encoded block,
            // and try to start a new run.
            rc = AddRunToOutputBlock(false);

            if (rc)
            {
                _currentByte = -1;
                _runLength = 0;
                // returning 0 implies the block is full, and the Byte was not written.
                return 0;
            }

            // start a new run
            _runLength = 1;
            _currentByte = b;
            return 1;
        }

        private Boolean AddRunToOutputBlock(Boolean final)
        {
            _runs++;
            /* add_pair_to_block ( EState* s ) */
            int previousLast = _last;

            // sanity check only - because of the check done at the
            // bottom of this method, and the logic in write0(), this
            // should never ever happen.
            if (previousLast >= _outBlockFillThreshold && !final)
            {
                var msg = String.Format("block overrun(final={2}): {0} >= threshold ({1})",
                                        previousLast, _outBlockFillThreshold, final);
                throw new Exception(msg);
            }

            // NB: the index used here into block is always (last+2).  This is
            // because last is -1 based - the initial value is -1, a flag value
            // used to indicate that nothing has yet been written into the
            // block. The endBlock() fn tests for -1 to detect empty blocks. Also,
            // the first Byte of block is used, during sorting, to hold block[last
            // +1], which is the final Byte value that had been written into the
            // rle block. For those two reasons, the base offset from last is
            // always +2.

            Byte b = (Byte)_currentByte;
            Byte[] block = _cstate.block;
            _cstate.inUse[b] = true;
            int rl = _runLength;
            _crc.UpdateCRC(b, rl);

            switch (rl)
            {
                case 1:
                    block[previousLast + 2] = b;
                    _last = previousLast + 1;
                    break;

                case 2:
                    block[previousLast + 2] = b;
                    block[previousLast + 3] = b;
                    _last = previousLast + 2;
                    break;

                case 3:
                    block[previousLast + 2] = b;
                    block[previousLast + 3] = b;
                    block[previousLast + 4] = b;
                    _last = previousLast + 3;
                    break;

                default:
                    rl -= 4;
                    _cstate.inUse[rl] = true;
                    block[previousLast + 2] = b;
                    block[previousLast + 3] = b;
                    block[previousLast + 4] = b;
                    block[previousLast + 5] = b;
                    block[previousLast + 6] = (Byte)rl;
                    _last = previousLast + 5;
                    break;
            }

            // is full?
            return (_last >= _outBlockFillThreshold);
        }

        public void CompressAndWrite()
        {
            if (_runLength > 0)
                AddRunToOutputBlock(true);

            _currentByte = -1;

            // Console.WriteLine("  BZip2Compressor:CompressAndWrite (r={0} bcrc={1:X8})",
            //                   runs, this.crc.Crc32Result);

            // has any data been written?
            if (_last == -1)
                return; // no data; nothing to compress

            /* sort the block and establish posn of original String */
            blockSort();

            /*
             * A 6-Byte block header, the value chosen arbitrarily as 0x314159265359
             * :-). A 32 bit value does not really give a strong enough guarantee
             * that the value will not appear by chance in the compressed
             * datastream. Worst-case probability of this event, for a 900k block,
             * is about 2.0e-3 for 32 bits, 1.0e-5 for 40 bits and 4.0e-8 for 48
             * bits. For a compressed file of size 100Gb -- about 100000 blocks --
             * only a 48-bit marker will do. NB: normal compression/ decompression
             * donot rely on these statistical properties. They are only important
             * when trying to recover blocks from damaged files.
             */
            _bw.WriteByte(0x31);
            _bw.WriteByte(0x41);
            _bw.WriteByte(0x59);
            _bw.WriteByte(0x26);
            _bw.WriteByte(0x53);
            _bw.WriteByte(0x59);

            Crc32 = (uint)_crc.Crc32Result;
            _bw.WriteInt(Crc32);

            /* Now a single bit indicating randomisation. */
            _bw.WriteBits(1, (_blockRandomised) ? 1U : 0U);

            /* Finally, block's contents proper. */
            moveToFrontCodeAndSend();

            Reset();
        }

        private void randomiseBlock()
        {
            Boolean[] inUse = _cstate.inUse;
            Byte[] block = _cstate.block;
            int lastShadow = _last;

            for (int i = 256; --i >= 0;)
                inUse[i] = false;

            int rNToGo = 0;
            int rTPos = 0;
            for (int i = 0, j = 1; i <= lastShadow; i = j, j++)
            {
                if (rNToGo == 0)
                {
                    rNToGo = (char)ZipRand.Rnums(rTPos);
                    if (++rTPos == 512)
                        rTPos = 0;
                }

                rNToGo--;
                block[j] ^= (Byte)((rNToGo == 1) ? 1 : 0);

                // handle 16 bit signed numbers
                inUse[block[j] & 0xff] = true;
            }

            _blockRandomised = true;
        }

        private void mainSort()
        {
            CompressionState dataShadow = _cstate;
            int[] runningOrder = dataShadow.mainSort_runningOrder;
            int[] copy = dataShadow.mainSort_copy;
            Boolean[] bigDone = dataShadow.mainSort_bigDone;
            int[] ftab = dataShadow.ftab;
            Byte[] block = dataShadow.block;
            int[] fmap = dataShadow.fmap;
            char[] quadrant = dataShadow.quadrant;
            int lastShadow = _last;
            int workLimitShadow = _workLimit;
            Boolean firstAttemptShadow = _firstAttempt;

            // Set up the 2-Byte frequency table
            for (int i = 65537; --i >= 0;)
                ftab[i] = 0;

            /*
             * In the various block-sized structures, live data runs from 0 to
             * last+NUM_OVERSHOOT_ByteS inclusive. First, set up the overshoot area
             * for block.
             */
            for (int i = 0; i < BZip2.NUM_OVERSHOOT_ByteS; i++)
                block[lastShadow + i + 2] = block[(i % (lastShadow + 1)) + 1];
            for (int i = lastShadow + BZip2.NUM_OVERSHOOT_ByteS + 1; --i >= 0;)
                quadrant[i] = '\0';
            block[0] = block[lastShadow + 1];

            // Complete the initial radix sort:

            int c1 = block[0] & 0xff;
            for (int i = 0; i <= lastShadow; i++)
            {
                int c2 = block[i + 1] & 0xff;
                ftab[(c1 << 8) + c2]++;
                c1 = c2;
            }

            for (int i = 1; i <= 65536; i++)
                ftab[i] += ftab[i - 1];

            c1 = block[1] & 0xff;
            for (int i = 0; i < lastShadow; i++)
            {
                int c2 = block[i + 2] & 0xff;
                fmap[--ftab[(c1 << 8) + c2]] = i;
                c1 = c2;
            }

            fmap[--ftab[((block[lastShadow + 1] & 0xff) << 8) + (block[1] & 0xff)]] = lastShadow;

            /*
             * Now ftab contains the first loc of every small bucket. Calculate the
             * running order, from smallest to largest big bucket.
             */
            for (int i = 256; --i >= 0;)
            {
                bigDone[i] = false;
                runningOrder[i] = i;
            }

            for (int h = 364; h != 1;)
            {
                h /= 3;
                for (int i = h; i <= 255; i++)
                {
                    int vv = runningOrder[i];
                    int a = ftab[(vv + 1) << 8] - ftab[vv << 8];
                    int b = h - 1;
                    int j = i;
                    for (int ro = runningOrder[j - h]; (ftab[(ro + 1) << 8] - ftab[ro << 8]) > a; ro = runningOrder[j
                                                                                                                    - h])
                    {
                        runningOrder[j] = ro;
                        j -= h;
                        if (j <= b)
                            break;
                    }
                    runningOrder[j] = vv;
                }
            }

            /*
             * The main sorting loop.
             */
            for (int i = 0; i <= 255; i++)
            {
                /*
                 * Process big buckets, starting with the least full.
                 */
                int ss = runningOrder[i];

                // Step 1:
                /*
                 * Complete the big bucket [ss] by quicksorting any unsorted small
                 * buckets [ss, j]. Hopefully previous pointer-scanning phases have
                 * already completed many of the small buckets [ss, j], so we don't
                 * have to sort them at all.
                 */
                for (int j = 0; j <= 255; j++)
                {
                    int sb = (ss << 8) + j;
                    int ftab_sb = ftab[sb];
                    if ((ftab_sb & SETMASK) != SETMASK)
                    {
                        int lo = ftab_sb & CLEARMASK;
                        int hi = (ftab[sb + 1] & CLEARMASK) - 1;
                        if (hi > lo)
                        {
                            mainQSort3(dataShadow, lo, hi, 2);
                            if (firstAttemptShadow
                                && (_workDone > workLimitShadow))
                                return;
                        }
                        ftab[sb] = ftab_sb | SETMASK;
                    }
                }

                // Step 2:
                // Now scan this big bucket so as to synthesise the
                // sorted order for small buckets [t, ss] for all t != ss.

                for (int j = 0; j <= 255; j++)
                    copy[j] = ftab[(j << 8) + ss] & CLEARMASK;

                for (int j = ftab[ss << 8] & CLEARMASK, hj = (ftab[(ss + 1) << 8] & CLEARMASK); j < hj; j++)
                {
                    int fmap_j = fmap[j];
                    c1 = block[fmap_j] & 0xff;
                    if (!bigDone[c1])
                    {
                        fmap[copy[c1]] = (fmap_j == 0) ? lastShadow : (fmap_j - 1);
                        copy[c1]++;
                    }
                }

                for (int j = 256; --j >= 0;)
                    ftab[(j << 8) + ss] |= SETMASK;

                // Step 3:
                /*
                 * The ss big bucket is now done. Record this fact, and update the
                 * quadrant descriptors. Remember to update quadrants in the
                 * overshoot area too, if necessary. The "if (i < 255)" test merely
                 * skips this updating for the last bucket processed, since updating
                 * for the last bucket is pointless.
                 */
                bigDone[ss] = true;

                if (i < 255)
                {
                    int bbStart = ftab[ss << 8] & CLEARMASK;
                    int bbSize = (ftab[(ss + 1) << 8] & CLEARMASK) - bbStart;
                    int shifts = 0;

                    while ((bbSize >> shifts) > 65534)
                        shifts++;

                    for (int j = 0; j < bbSize; j++)
                    {
                        int a2update = fmap[bbStart + j];
                        char qVal = (char)(j >> shifts);
                        quadrant[a2update] = qVal;
                        if (a2update < BZip2.NUM_OVERSHOOT_ByteS)
                            quadrant[a2update + lastShadow + 1] = qVal;
                    }
                }
            }
        }

        private void blockSort()
        {
            _workLimit = WORK_FACTOR * _last;
            _workDone = 0;
            _blockRandomised = false;
            _firstAttempt = true;
            mainSort();

            if (_firstAttempt && (_workDone > _workLimit))
            {
                randomiseBlock();
                _workLimit = _workDone = 0;
                _firstAttempt = false;
                mainSort();
            }

            int[] fmap = _cstate.fmap;
            _origPtr = -1;
            for (int i = 0, lastShadow = _last; i <= lastShadow; i++)
            {
                if (fmap[i] == 0)
                {
                    _origPtr = i;
                    break;
                }
            }

            // assert (this.origPtr != -1) : this.origPtr;
        }

        private Boolean mainSimpleSort(CompressionState dataShadow, int lo, int hi, int d)
        {
            int bigN = hi - lo + 1;
            if (bigN < 2)
                return _firstAttempt && (_workDone > _workLimit);

            int hp = 0;
            while (increments[hp] < bigN)
                hp++;

            int[] fmap = dataShadow.fmap;
            char[] quadrant = dataShadow.quadrant;
            Byte[] block = dataShadow.block;
            int lastShadow = _last;
            int lastPlus1 = lastShadow + 1;
            Boolean firstAttemptShadow = _firstAttempt;
            int workLimitShadow = _workLimit;
            int workDoneShadow = _workDone;

            // Following block contains unrolled code which could be shortened by
            // coding it in additional loops.

            // HP:
            while (--hp >= 0)
            {
                int h = increments[hp];
                int mj = lo + h - 1;

                for (int i = lo + h; i <= hi;)
                {
                    // copy
                    for (int k = 3; (i <= hi) && (--k >= 0); i++)
                    {
                        int v = fmap[i];
                        int vd = v + d;
                        int j = i;

                        // for (int a;
                        // (j > mj) && mainGtU((a = fmap[j - h]) + d, vd,
                        // block, quadrant, lastShadow);
                        // j -= h) {
                        // fmap[j] = a;
                        // }
                        //
                        // unrolled version:

                        // start inline mainGTU
                        Boolean onceRunned = false;
                        int a = 0;

                        HAMMER:
                        while (true)
                        {
                            if (onceRunned)
                            {
                                fmap[j] = a;
                                if ((j -= h) <= mj)
                                    goto END_HAMMER;
                            }
                            else
                                onceRunned = true;

                            a = fmap[j - h];
                            int i1 = a + d;
                            int i2 = vd;

                            // following could be done in a loop, but
                            // unrolled it for performance:
                            if (block[i1 + 1] == block[i2 + 1])
                            {
                                if (block[i1 + 2] == block[i2 + 2])
                                {
                                    if (block[i1 + 3] == block[i2 + 3])
                                    {
                                        if (block[i1 + 4] == block[i2 + 4])
                                        {
                                            if (block[i1 + 5] == block[i2 + 5])
                                            {
                                                if (block[(i1 += 6)] == block[(i2 += 6)])
                                                {
                                                    int x = lastShadow;
                                                    X:
                                                    while (x > 0)
                                                    {
                                                        x -= 4;

                                                        if (block[i1 + 1] == block[i2 + 1])
                                                        {
                                                            if (quadrant[i1] == quadrant[i2])
                                                            {
                                                                if (block[i1 + 2] == block[i2 + 2])
                                                                {
                                                                    if (quadrant[i1 + 1] == quadrant[i2 + 1])
                                                                    {
                                                                        if (block[i1 + 3] == block[i2 + 3])
                                                                        {
                                                                            if (quadrant[i1 + 2] == quadrant[i2 + 2])
                                                                            {
                                                                                if (block[i1 + 4] == block[i2 + 4])
                                                                                {
                                                                                    if (quadrant[i1 + 3] == quadrant[i2 + 3])
                                                                                    {
                                                                                        if ((i1 += 4) >= lastPlus1)
                                                                                            i1 -= lastPlus1;
                                                                                        if ((i2 += 4) >= lastPlus1)
                                                                                            i2 -= lastPlus1;
                                                                                        workDoneShadow++;
                                                                                        goto X;
                                                                                    }
                                                                                    else if ((quadrant[i1 + 3] > quadrant[i2 + 3]))
                                                                                        goto HAMMER;
                                                                                    else
                                                                                        goto END_HAMMER;
                                                                                }
                                                                                else if ((block[i1 + 4] & 0xff) > (block[i2 + 4] & 0xff))
                                                                                    goto HAMMER;
                                                                                else
                                                                                    goto END_HAMMER;
                                                                            }
                                                                            else if ((quadrant[i1 + 2] > quadrant[i2 + 2]))
                                                                                goto HAMMER;
                                                                            else
                                                                                goto END_HAMMER;
                                                                        }
                                                                        else if ((block[i1 + 3] & 0xff) > (block[i2 + 3] & 0xff))
                                                                            goto HAMMER;
                                                                        else
                                                                            goto END_HAMMER;
                                                                    }
                                                                    else if ((quadrant[i1 + 1] > quadrant[i2 + 1]))
                                                                        goto HAMMER;
                                                                    else
                                                                        goto END_HAMMER;
                                                                }
                                                                else if ((block[i1 + 2] & 0xff) > (block[i2 + 2] & 0xff))
                                                                    goto HAMMER;
                                                                else
                                                                    goto END_HAMMER;
                                                            }
                                                            else if ((quadrant[i1] > quadrant[i2]))
                                                                goto HAMMER;
                                                            else
                                                                goto END_HAMMER;
                                                        }
                                                        else if ((block[i1 + 1] & 0xff) > (block[i2 + 1] & 0xff))
                                                            goto HAMMER;
                                                        else
                                                            goto END_HAMMER;
                                                    }
                                                    goto END_HAMMER;
                                                } // while x > 0
                                                else
                                                {
                                                    if ((block[i1] & 0xff) > (block[i2] & 0xff))
                                                        goto HAMMER;
                                                    else
                                                        goto END_HAMMER;
                                                }
                                            }
                                            else if ((block[i1 + 5] & 0xff) > (block[i2 + 5] & 0xff))
                                                goto HAMMER;
                                            else
                                                goto END_HAMMER;
                                        }
                                        else if ((block[i1 + 4] & 0xff) > (block[i2 + 4] & 0xff))
                                            goto HAMMER;
                                        else
                                            goto END_HAMMER;
                                    }
                                    else if ((block[i1 + 3] & 0xff) > (block[i2 + 3] & 0xff))
                                        goto HAMMER;
                                    else
                                        goto END_HAMMER;
                                }
                                else if ((block[i1 + 2] & 0xff) > (block[i2 + 2] & 0xff))
                                    goto HAMMER;
                                else
                                    goto END_HAMMER;
                            }
                            else if ((block[i1 + 1] & 0xff) > (block[i2 + 1] & 0xff))
                                goto HAMMER;
                            else
                                goto END_HAMMER;
                        } // HAMMER

                        END_HAMMER:
                        // end inline mainGTU

                        fmap[j] = v;
                    }

                    if (firstAttemptShadow && (i <= hi)
                        && (workDoneShadow > workLimitShadow))
                        goto END_HP;
                }
            }
            END_HP:

            _workDone = workDoneShadow;
            return firstAttemptShadow && (workDoneShadow > workLimitShadow);
        }

        private static void vswap(int[] fmap, int p1, int p2, int n)
        {
            n += p1;
            while (p1 < n)
            {
                int t = fmap[p1];
                fmap[p1++] = fmap[p2];
                fmap[p2++] = t;
            }
        }

        private static Byte med3(Byte a, Byte b, Byte c)
        {
            return (a < b) ? (b < c ? b : a < c ? c : a) : (b > c ? b : a > c ? c : a);
        }

        private void mainQSort3(CompressionState dataShadow, int loSt, int hiSt, int dSt)
        {
            int[] stack_ll = dataShadow.stack_ll;
            int[] stack_hh = dataShadow.stack_hh;
            int[] stack_dd = dataShadow.stack_dd;
            int[] fmap = dataShadow.fmap;
            Byte[] block = dataShadow.block;

            stack_ll[0] = loSt;
            stack_hh[0] = hiSt;
            stack_dd[0] = dSt;

            for (int sp = 1; --sp >= 0;)
            {
                int lo = stack_ll[sp];
                int hi = stack_hh[sp];
                int d = stack_dd[sp];

                if ((hi - lo < SMALL_THRESH) || (d > DEPTH_THRESH))
                {
                    if (mainSimpleSort(dataShadow, lo, hi, d))
                        return;
                }
                else
                {
                    int d1 = d + 1;
                    int med = med3(block[fmap[lo] + d1],
                                   block[fmap[hi] + d1], block[fmap[(lo + hi) >> 1] + d1]) & 0xff;

                    int unLo = lo;
                    int unHi = hi;
                    int ltLo = lo;
                    int gtHi = hi;

                    while (true)
                    {
                        while (unLo <= unHi)
                        {
                            int n = (block[fmap[unLo] + d1] & 0xff)
                                    - med;
                            if (n == 0)
                            {
                                int temp = fmap[unLo];
                                fmap[unLo++] = fmap[ltLo];
                                fmap[ltLo++] = temp;
                            }
                            else if (n < 0)
                                unLo++;
                            else
                                break;
                        }

                        while (unLo <= unHi)
                        {
                            int n = (block[fmap[unHi] + d1] & 0xff)
                                    - med;
                            if (n == 0)
                            {
                                int temp = fmap[unHi];
                                fmap[unHi--] = fmap[gtHi];
                                fmap[gtHi--] = temp;
                            }
                            else if (n > 0)
                                unHi--;
                            else
                                break;
                        }

                        if (unLo <= unHi)
                        {
                            int temp = fmap[unLo];
                            fmap[unLo++] = fmap[unHi];
                            fmap[unHi--] = temp;
                        }
                        else
                            break;
                    }

                    if (gtHi < ltLo)
                    {
                        stack_ll[sp] = lo;
                        stack_hh[sp] = hi;
                        stack_dd[sp] = d1;
                        sp++;
                    }
                    else
                    {
                        int n = ((ltLo - lo) < (unLo - ltLo)) ? (ltLo - lo)
                                    : (unLo - ltLo);
                        vswap(fmap, lo, unLo - n, n);
                        int m = ((hi - gtHi) < (gtHi - unHi)) ? (hi - gtHi)
                                    : (gtHi - unHi);
                        vswap(fmap, unLo, hi - m + 1, m);

                        n = lo + unLo - ltLo - 1;
                        m = hi - (gtHi - unHi) + 1;

                        stack_ll[sp] = lo;
                        stack_hh[sp] = n;
                        stack_dd[sp] = d;
                        sp++;

                        stack_ll[sp] = n + 1;
                        stack_hh[sp] = m - 1;
                        stack_dd[sp] = d1;
                        sp++;

                        stack_ll[sp] = m;
                        stack_hh[sp] = hi;
                        stack_dd[sp] = d;
                        sp++;
                    }
                }
            }
        }

        private void generateMTFValues()
        {
            int lastShadow = _last;
            CompressionState dataShadow = _cstate;
            Boolean[] inUse = dataShadow.inUse;
            Byte[] block = dataShadow.block;
            int[] fmap = dataShadow.fmap;
            char[] sfmap = dataShadow.sfmap;
            int[] mtfFreq = dataShadow.mtfFreq;
            Byte[] unseqToSeq = dataShadow.unseqToSeq;
            Byte[] yy = dataShadow.generateMTFValues_yy;

            // make maps
            int nInUseShadow = 0;
            for (int i = 0; i < 256; i++)
            {
                if (inUse[i])
                {
                    unseqToSeq[i] = (Byte)nInUseShadow;
                    nInUseShadow++;
                }
            }
            _nInUse = nInUseShadow;

            int eob = nInUseShadow + 1;

            for (int i = eob; i >= 0; i--)
                mtfFreq[i] = 0;

            for (int i = nInUseShadow; --i >= 0;)
                yy[i] = (Byte)i;

            int wr = 0;
            int zPend = 0;

            for (int i = 0; i <= lastShadow; i++)
            {
                Byte ll_i = unseqToSeq[block[fmap[i]] & 0xff];
                Byte tmp = yy[0];
                int j = 0;

                while (ll_i != tmp)
                {
                    j++;
                    Byte tmp2 = tmp;
                    tmp = yy[j];
                    yy[j] = tmp2;
                }
                yy[0] = tmp;

                if (j == 0)
                    zPend++;
                else
                {
                    if (zPend > 0)
                    {
                        zPend--;
                        while (true)
                        {
                            if ((zPend & 1) == 0)
                            {
                                sfmap[wr] = BZip2.RUNA;
                                wr++;
                                mtfFreq[BZip2.RUNA]++;
                            }
                            else
                            {
                                sfmap[wr] = BZip2.RUNB;
                                wr++;
                                mtfFreq[BZip2.RUNB]++;
                            }

                            if (zPend >= 2)
                                zPend = (zPend - 2) >> 1;
                            else
                                break;
                        }
                        zPend = 0;
                    }
                    sfmap[wr] = (char)(j + 1);
                    wr++;
                    mtfFreq[j + 1]++;
                }
            }

            if (zPend > 0)
            {
                zPend--;
                while (true)
                {
                    if ((zPend & 1) == 0)
                    {
                        sfmap[wr] = BZip2.RUNA;
                        wr++;
                        mtfFreq[BZip2.RUNA]++;
                    }
                    else
                    {
                        sfmap[wr] = BZip2.RUNB;
                        wr++;
                        mtfFreq[BZip2.RUNB]++;
                    }

                    if (zPend >= 2)
                        zPend = (zPend - 2) >> 1;
                    else
                        break;
                }
            }

            sfmap[wr] = (char)eob;
            mtfFreq[eob]++;
            _nMTF = wr + 1;
        }

        private static void hbAssignCodes(int[] code, Byte[] length, int minLen, int maxLen, int alphaSize)
        {
            int vec = 0;
            for (int n = minLen; n <= maxLen; n++)
            {
                for (int i = 0; i < alphaSize; i++)
                {
                    if ((length[i] & 0xff) == n)
                    {
                        code[i] = vec;
                        vec++;
                    }
                }
                vec <<= 1;
            }
        }

        private void sendMTFValues()
        {
            Byte[][] len = _cstate.sendMTFValues_len;
            int alphaSize = _nInUse + 2;

            for (int t = BZip2.NGroups; --t >= 0;)
            {
                Byte[] len_t = len[t];
                for (int v = alphaSize; --v >= 0;)
                    len_t[v] = GREATER_ICOST;
            }

            /* Decide how many coding tables to use */
            // assert (this.nMTF > 0) : this.nMTF;
            int nGroups = (_nMTF < 200) ? 2 : (_nMTF < 600) ? 3
                                                  : (_nMTF < 1200) ? 4 : (_nMTF < 2400) ? 5 : 6;

            /* Generate an initial set of coding tables */
            sendMTFValues0(nGroups, alphaSize);

            /*
             * Iterate up to N_ITERS times to improve the tables.
             */
            int nSelectors = sendMTFValues1(nGroups, alphaSize);

            /* Compute MTF values for the selectors. */
            sendMTFValues2(nGroups, nSelectors);

            /* Assign actual codes for the tables. */
            sendMTFValues3(nGroups, alphaSize);

            /* Transmit the mapping table. */
            sendMTFValues4();

            /* Now the selectors. */
            sendMTFValues5(nGroups, nSelectors);

            /* Now the coding tables. */
            sendMTFValues6(nGroups, alphaSize);

            /* And finally, the block data proper */
            sendMTFValues7(nSelectors);
        }

        private void sendMTFValues0(int nGroups, int alphaSize)
        {
            Byte[][] len = _cstate.sendMTFValues_len;
            int[] mtfFreq = _cstate.mtfFreq;

            int remF = _nMTF;
            int gs = 0;

            for (int nPart = nGroups; nPart > 0; nPart--)
            {
                int tFreq = remF / nPart;
                int ge = gs - 1;
                int aFreq = 0;

                for (int a = alphaSize - 1; (aFreq < tFreq) && (ge < a);)
                    aFreq += mtfFreq[++ge];

                if ((ge > gs) && (nPart != nGroups) && (nPart != 1)
                    && (((nGroups - nPart) & 1) != 0))
                    aFreq -= mtfFreq[ge--];

                Byte[] len_np = len[nPart - 1];
                for (int v = alphaSize; --v >= 0;)
                {
                    if ((v >= gs) && (v <= ge))
                        len_np[v] = LESSER_ICOST;
                    else
                        len_np[v] = GREATER_ICOST;
                }

                gs = ge + 1;
                remF -= aFreq;
            }
        }

        private static void hbMakeCodeLengths(Byte[] len, int[] freq, CompressionState state1, int alphaSize, int maxLen)
        {
            int[] heap = state1.heap;
            int[] weight = state1.weight;
            int[] parent = state1.parent;

            for (int i = alphaSize; --i >= 0;)
                weight[i + 1] = (freq[i] == 0 ? 1 : freq[i]) << 8;

            for (Boolean tooLong = true; tooLong;)
            {
                tooLong = false;

                int nNodes = alphaSize;
                int nHeap = 0;
                heap[0] = 0;
                weight[0] = 0;
                parent[0] = -2;

                for (int i = 1; i <= alphaSize; i++)
                {
                    parent[i] = -1;
                    nHeap++;
                    heap[nHeap] = i;

                    int zz = nHeap;
                    int tmp = heap[zz];
                    while (weight[tmp] < weight[heap[zz >> 1]])
                    {
                        heap[zz] = heap[zz >> 1];
                        zz >>= 1;
                    }
                    heap[zz] = tmp;
                }

                while (nHeap > 1)
                {
                    int n1 = heap[1];
                    heap[1] = heap[nHeap];
                    nHeap--;

                    int yy = 0;
                    int zz = 1;
                    int tmp = heap[1];

                    while (true)
                    {
                        yy = zz << 1;

                        if (yy > nHeap)
                            break;

                        if ((yy < nHeap)
                            && (weight[heap[yy + 1]] < weight[heap[yy]]))
                            yy++;

                        if (weight[tmp] < weight[heap[yy]])
                            break;

                        heap[zz] = heap[yy];
                        zz = yy;
                    }

                    heap[zz] = tmp;

                    int n2 = heap[1];
                    heap[1] = heap[nHeap];
                    nHeap--;

                    yy = 0;
                    zz = 1;
                    tmp = heap[1];

                    while (true)
                    {
                        yy = zz << 1;

                        if (yy > nHeap)
                            break;

                        if ((yy < nHeap)
                            && (weight[heap[yy + 1]] < weight[heap[yy]]))
                            yy++;

                        if (weight[tmp] < weight[heap[yy]])
                            break;

                        heap[zz] = heap[yy];
                        zz = yy;
                    }

                    heap[zz] = tmp;
                    nNodes++;
                    parent[n1] = parent[n2] = nNodes;

                    int weight_n1 = weight[n1];
                    int weight_n2 = weight[n2];
                    weight[nNodes] = (int)(((uint)weight_n1 & 0xffffff00U)
                                           + ((uint)weight_n2 & 0xffffff00U))
                                     | (1 + (((weight_n1 & 0x000000ff)
                                              > (weight_n2 & 0x000000ff))
                                                 ? (weight_n1 & 0x000000ff)
                                                 : (weight_n2 & 0x000000ff)));

                    parent[nNodes] = -1;
                    nHeap++;
                    heap[nHeap] = nNodes;

                    tmp = 0;
                    zz = nHeap;
                    tmp = heap[zz];
                    int weight_tmp = weight[tmp];
                    while (weight_tmp < weight[heap[zz >> 1]])
                    {
                        heap[zz] = heap[zz >> 1];
                        zz >>= 1;
                    }
                    heap[zz] = tmp;
                }

                for (int i = 1; i <= alphaSize; i++)
                {
                    int j = 0;
                    int k = i;

                    for (int parent_k; (parent_k = parent[k]) >= 0;)
                    {
                        k = parent_k;
                        j++;
                    }

                    len[i - 1] = (Byte)j;
                    if (j > maxLen)
                        tooLong = true;
                }

                if (tooLong)
                {
                    for (int i = 1; i < alphaSize; i++)
                    {
                        int j = weight[i] >> 8;
                        j = 1 + (j >> 1);
                        weight[i] = j << 8;
                    }
                }
            }
        }

        private int sendMTFValues1(int nGroups, int alphaSize)
        {
            CompressionState dataShadow = _cstate;
            int[][] rfreq = dataShadow.sendMTFValues_rfreq;
            int[] fave = dataShadow.sendMTFValues_fave;
            short[] cost = dataShadow.sendMTFValues_cost;
            char[] sfmap = dataShadow.sfmap;
            Byte[] selector = dataShadow.selector;
            Byte[][] len = dataShadow.sendMTFValues_len;
            Byte[] len_0 = len[0];
            Byte[] len_1 = len[1];
            Byte[] len_2 = len[2];
            Byte[] len_3 = len[3];
            Byte[] len_4 = len[4];
            Byte[] len_5 = len[5];
            int nMTFShadow = _nMTF;

            int nSelectors = 0;

            for (int iter = 0; iter < BZip2.N_ITERS; iter++)
            {
                for (int t = nGroups; --t >= 0;)
                {
                    fave[t] = 0;
                    int[] rfreqt = rfreq[t];
                    for (int i = alphaSize; --i >= 0;)
                        rfreqt[i] = 0;
                }

                nSelectors = 0;

                for (int gs = 0; gs < _nMTF;)
                {
                    /* Set group start & end marks. */

                    /*
                     * Calculate the cost of this group as coded by each of the
                     * coding tables.
                     */

                    int ge = Math.Min(gs + BZip2.G_SIZE - 1, nMTFShadow - 1);

                    if (nGroups == BZip2.NGroups)
                    {
                        // unrolled version of the else-block

                        int[] c = new int[6];

                        for (int i = gs; i <= ge; i++)
                        {
                            int icv = sfmap[i];
                            c[0] += len_0[icv] & 0xff;
                            c[1] += len_1[icv] & 0xff;
                            c[2] += len_2[icv] & 0xff;
                            c[3] += len_3[icv] & 0xff;
                            c[4] += len_4[icv] & 0xff;
                            c[5] += len_5[icv] & 0xff;
                        }

                        cost[0] = (short)c[0];
                        cost[1] = (short)c[1];
                        cost[2] = (short)c[2];
                        cost[3] = (short)c[3];
                        cost[4] = (short)c[4];
                        cost[5] = (short)c[5];
                    }
                    else
                    {
                        for (int t = nGroups; --t >= 0;)
                            cost[t] = 0;

                        for (int i = gs; i <= ge; i++)
                        {
                            int icv = sfmap[i];
                            for (int t = nGroups; --t >= 0;)
                                cost[t] += (short)(len[t][icv] & 0xff);
                        }
                    }

                    /*
                     * Find the coding table which is best for this group, and
                     * record its identity in the selector table.
                     */
                    int bt = -1;
                    for (int t = nGroups, bc = 999999999; --t >= 0;)
                    {
                        int cost_t = cost[t];
                        if (cost_t < bc)
                        {
                            bc = cost_t;
                            bt = t;
                        }
                    }

                    fave[bt]++;
                    selector[nSelectors] = (Byte)bt;
                    nSelectors++;

                    /*
                     * Increment the symbol frequencies for the selected table.
                     */
                    int[] rfreq_bt = rfreq[bt];
                    for (int i = gs; i <= ge; i++)
                        rfreq_bt[sfmap[i]]++;

                    gs = ge + 1;
                }

                /*
                 * Recompute the tables based on the accumulated frequencies.
                 */
                for (int t = 0; t < nGroups; t++)
                    hbMakeCodeLengths(len[t], rfreq[t], _cstate, alphaSize, 20);
            }

            return nSelectors;
        }

        private void sendMTFValues2(int nGroups, int nSelectors)
        {
            CompressionState dataShadow = _cstate;
            Byte[] pos = dataShadow.sendMTFValues2_pos;

            for (int i = nGroups; --i >= 0;)
                pos[i] = (Byte)i;

            for (int i = 0; i < nSelectors; i++)
            {
                Byte ll_i = dataShadow.selector[i];
                Byte tmp = pos[0];
                int j = 0;

                while (ll_i != tmp)
                {
                    j++;
                    Byte tmp2 = tmp;
                    tmp = pos[j];
                    pos[j] = tmp2;
                }

                pos[0] = tmp;
                dataShadow.selectorMtf[i] = (Byte)j;
            }
        }

        private void sendMTFValues3(int nGroups, int alphaSize)
        {
            int[][] code = _cstate.sendMTFValues_code;
            Byte[][] len = _cstate.sendMTFValues_len;

            for (int t = 0; t < nGroups; t++)
            {
                int minLen = 32;
                int maxLen = 0;
                Byte[] len_t = len[t];
                for (int i = alphaSize; --i >= 0;)
                {
                    int l = len_t[i] & 0xff;
                    if (l > maxLen)
                        maxLen = l;
                    if (l < minLen)
                        minLen = l;
                }

                // assert (maxLen <= 20) : maxLen;
                // assert (minLen >= 1) : minLen;

                hbAssignCodes(code[t], len[t], minLen, maxLen, alphaSize);
            }
        }

        private void sendMTFValues4()
        {
            Boolean[] inUse = _cstate.inUse;
            Boolean[] inUse16 = _cstate.sentMTFValues4_inUse16;

            for (int i = 16; --i >= 0;)
            {
                inUse16[i] = false;
                int i16 = i * 16;
                for (int j = 16; --j >= 0;)
                {
                    if (inUse[i16 + j])
                        inUse16[i] = true;
                }
            }

            uint u = 0;
            for (int i = 0; i < 16; i++)
            {
                if (inUse16[i])
                    u |= 1U << (16 - i - 1);
            }
            _bw.WriteBits(16, u);

            for (int i = 0; i < 16; i++)
            {
                if (inUse16[i])
                {
                    int i16 = i * 16;
                    u = 0;
                    for (int j = 0; j < 16; j++)
                    {
                        if (inUse[i16 + j])
                            u |= 1U << (16 - j - 1);
                    }
                    _bw.WriteBits(16, u);
                }
            }
        }

        private void sendMTFValues5(int nGroups, int nSelectors)
        {
            _bw.WriteBits(3, (uint)nGroups);
            _bw.WriteBits(15, (uint)nSelectors);

            Byte[] selectorMtf = _cstate.selectorMtf;

            for (int i = 0; i < nSelectors; i++)
            {
                for (int j = 0, hj = selectorMtf[i] & 0xff; j < hj; j++)
                    _bw.WriteBits(1, 1);

                _bw.WriteBits(1, 0);
            }
        }

        private void sendMTFValues6(int nGroups, int alphaSize)
        {
            Byte[][] len = _cstate.sendMTFValues_len;

            for (int t = 0; t < nGroups; t++)
            {
                Byte[] len_t = len[t];
                uint curr = (uint)(len_t[0] & 0xff);
                _bw.WriteBits(5, curr);

                for (int i = 0; i < alphaSize; i++)
                {
                    int lti = len_t[i] & 0xff;
                    while (curr < lti)
                    {
                        _bw.WriteBits(2, 2U);
                        curr++; /* 10 */
                    }

                    while (curr > lti)
                    {
                        _bw.WriteBits(2, 3U);
                        curr--; /* 11 */
                    }

                    _bw.WriteBits(1, 0U);
                }
            }
        }

        private void sendMTFValues7(int nSelectors)
        {
            Byte[][] len = _cstate.sendMTFValues_len;
            int[][] code = _cstate.sendMTFValues_code;
            Byte[] selector = _cstate.selector;
            char[] sfmap = _cstate.sfmap;
            int nMTFShadow = _nMTF;

            int selCtr = 0;

            for (int gs = 0; gs < nMTFShadow;)
            {
                int ge = Math.Min(gs + BZip2.G_SIZE - 1, nMTFShadow - 1);
                int ix = selector[selCtr] & 0xff;
                int[] code_selCtr = code[ix];
                Byte[] len_selCtr = len[ix];

                while (gs <= ge)
                {
                    int sfmap_i = sfmap[gs];
                    int n = len_selCtr[sfmap_i] & 0xFF;
                    _bw.WriteBits(n, (uint)code_selCtr[sfmap_i]);
                    gs++;
                }

                gs = ge + 1;
                selCtr++;
            }
        }

        private void moveToFrontCodeAndSend()
        {
            _bw.WriteBits(24, (uint)_origPtr);
            generateMTFValues();
            sendMTFValues();
        }

        #endregion

        #region Класс CompressionState

        private class CompressionState
        {
            #region Поля

            public readonly int[] ftab = new int[65537]; // 262148 Byte

            public readonly Byte[] generateMTFValues_yy = new Byte[256];

            public readonly Boolean[] inUse = new Boolean[256];

            public readonly Boolean[] mainSort_bigDone = new Boolean[256]; // 256 Byte

            public readonly int[] mainSort_copy = new int[256]; // 1024 Byte

            public readonly int[] mainSort_runningOrder = new int[256]; // 1024 Byte

            public readonly int[] mtfFreq = new int[BZip2.MaxAlphaSize];

            public readonly Byte[] selector = new Byte[BZip2.MaxSelectors];

            public readonly Byte[] selectorMtf = new Byte[BZip2.MaxSelectors];

            public readonly Byte[] sendMTFValues2_pos = new Byte[BZip2.NGroups]; // 6 Byte

            public readonly short[] sendMTFValues_cost = new short[BZip2.NGroups]; // 12 Byte

            public readonly int[] sendMTFValues_fave = new int[BZip2.NGroups]; // 24 Byte

            public readonly Boolean[] sentMTFValues4_inUse16 = new Boolean[16]; // 16 Byte

            public readonly int[] stack_dd = new int[BZip2.QSORT_STACK_SIZE]; // 4000 Byte

            public readonly int[] stack_hh = new int[BZip2.QSORT_STACK_SIZE]; // 4000 Byte

            public readonly int[] stack_ll = new int[BZip2.QSORT_STACK_SIZE]; // 4000 Byte

            public readonly Byte[] unseqToSeq = new Byte[256];

            public Byte[] block; // 900021 Byte

            public int[] fmap; // 3600000 Byte

            public int[] heap = new int[BZip2.MaxAlphaSize + 2]; // 1040 Byte

            public int[] parent = new int[BZip2.MaxAlphaSize * 2]; // 2064 Byte

            public char[] quadrant;

            public int[][] sendMTFValues_code;

            public Byte[][] sendMTFValues_len;

            public int[][] sendMTFValues_rfreq;

            public char[] sfmap; // 3600000 Byte

            public int[] weight = new int[BZip2.MaxAlphaSize * 2]; // 2064 Byte

            #endregion

            #region Конструктор

            public CompressionState(int blockSize100k)
            {
                int n = blockSize100k * BZip2.BlockSizeMultiple;
                block = new Byte[(n + 1 + BZip2.NUM_OVERSHOOT_ByteS)];
                fmap = new int[n];
                sfmap = new char[2 * n];
                quadrant = sfmap;
                sendMTFValues_len = BZip2.InitRectangularArray<Byte>(BZip2.NGroups, BZip2.MaxAlphaSize);
                sendMTFValues_rfreq = BZip2.InitRectangularArray<int>(BZip2.NGroups, BZip2.MaxAlphaSize);
                sendMTFValues_code = BZip2.InitRectangularArray<int>(BZip2.NGroups, BZip2.MaxAlphaSize);
            }

            #endregion
        }

        #endregion Класс CompressionState
    }

    #endregion Класс BZip2Compressor

    #region Класс ZipSharedUtilities

    internal static class ZipSharedUtilities
    {
        #region Поля

        private static System.Text.RegularExpressions.Regex doubleDotRegex1 = new System.Text.RegularExpressions.Regex(@"^(.*/)?([^/\\.]+/\\.\\./)(.+)$");

        private static System.Text.Encoding ibm437 = System.Text.Encoding.GetEncoding("IBM437");

        private static System.Text.Encoding utf8 = System.Text.Encoding.GetEncoding("UTF-8");

        #endregion

        #region Методы

        public static Int64 GetFileLength(String fileName)
        {
            if (!File.Exists(fileName))
                throw new System.IO.FileNotFoundException(fileName);

            long fileLength = 0L;
            FileShare fs = FileShare.ReadWrite;
            fs |= FileShare.Delete;

            using (var s = File.Open(fileName, FileMode.Open, FileAccess.Read, fs))
                fileLength = s.Length;
            return fileLength;
        }

        [System.Diagnostics.Conditional("NETCF")]
        public static void Workaround_Ladybug318918(Stream s)
        {
            // This is a workaround for this issue:
            // https://connect.microsoft.com/VisualStudio/feedback/details/318918
            // It's required only on NETCF.
            s.Flush();
        }

        private static String SimplifyFwdSlashPath(String path)
        {
            if (path.StartsWith("./")) path = path.Substring(2);
            path = path.Replace("/./", "/");

            // Replace foo/anything/../bar with foo/bar
            path = doubleDotRegex1.Replace(path, "$1$3");

            return path;
        }

        public static String NormalizePathForUseInZipFile(String pathName)
        {
            // boundary case
            if (String.IsNullOrEmpty(pathName)) return pathName;

            // trim volume if necessary
            if ((pathName.Length >= 2) && ((pathName[1] == ':') && (pathName[2] == '\\')))
                pathName = pathName.Substring(3);

            // swap slashes
            pathName = pathName.Replace('\\', '/');

            // trim all leading slashes
            while (pathName.StartsWith("/")) pathName = pathName.Substring(1);

            return SimplifyFwdSlashPath(pathName);
        }

        internal static Byte[] StringToByteArray(String value, System.Text.Encoding encoding)
        {
            Byte[] a = encoding.GetBytes(value);
            return a;
        }

        internal static Byte[] StringToByteArray(String value)
        {
            return StringToByteArray(value, ibm437);
        }

        internal static String Utf8StringFromBuffer(Byte[] buf)
        {
            return StringFromBuffer(buf, utf8);
        }

        internal static String StringFromBuffer(Byte[] buf, System.Text.Encoding encoding)
        {
            // this form of the GetString() method is required for .NET CF compatibility
            String s = encoding.GetString(buf, 0, buf.Length);
            return s;
        }

        internal static int ReadSignature(System.IO.Stream s)
        {
            int x = 0;
            try
            {
                x = _ReadFourBytes(s, "n/a");
            }
            catch (ZipBadReadException)
            {
            }
            return x;
        }

        internal static int ReadEntrySignature(System.IO.Stream s)
        {
            // handle the case of ill-formatted zip archives - includes a data descriptor
            // when none is expected.
            int x = 0;
            try
            {
                x = _ReadFourBytes(s, "n/a");
                if (x == ZipConstants.ZipEntryDataDescriptorSignature)
                {
                    // advance past data descriptor - 12 Bytes if not zip64
                    s.Seek(12, SeekOrigin.Current);
                    // workitem 10178
                    Workaround_Ladybug318918(s);
                    x = _ReadFourBytes(s, "n/a");
                    if (x != ZipConstants.ZipEntrySignature)
                    {
                        // Maybe zip64 was in use for the prior entry.
                        // Therefore, skip another 8 Bytes.
                        s.Seek(8, SeekOrigin.Current);
                        // workitem 10178
                        Workaround_Ladybug318918(s);
                        x = _ReadFourBytes(s, "n/a");
                        if (x != ZipConstants.ZipEntrySignature)
                        {
                            // seek back to the first spot
                            s.Seek(-24, SeekOrigin.Current);
                            // workitem 10178
                            Workaround_Ladybug318918(s);
                            x = _ReadFourBytes(s, "n/a");
                        }
                    }
                }
            }
            catch (ZipBadReadException)
            {
            }
            return x;
        }

        internal static int ReadInt(System.IO.Stream s)
        {
            return _ReadFourBytes(s, "Could not read block - no data!  (position 0x{0:X8})");
        }

        private static int _ReadFourBytes(System.IO.Stream s, String message)
        {
            int n = 0;
            Byte[] block = new Byte[4];

            n = s.Read(block, 0, block.Length);
            if (n != block.Length) throw new ZipBadReadException(String.Format(message, s.Position));
            int data = unchecked((((block[3] * 256 + block[2]) * 256) + block[1]) * 256 + block[0]);
            return data;
        }

        internal static long FindSignature(System.IO.Stream stream, int SignatureToFind)
        {
            long startingPosition = stream.Position;

            int BATCH_SIZE = 65536; //  8192;
            Byte[] targetBytes = new Byte[4];
            targetBytes[0] = (Byte)(SignatureToFind >> 24);
            targetBytes[1] = (Byte)((SignatureToFind & 0x00FF0000) >> 16);
            targetBytes[2] = (Byte)((SignatureToFind & 0x0000FF00) >> 8);
            targetBytes[3] = (Byte)(SignatureToFind & 0x000000FF);
            Byte[] batch = new Byte[BATCH_SIZE];
            int n = 0;
            Boolean success = false;
            do
            {
                n = stream.Read(batch, 0, batch.Length);
                if (n != 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (batch[i] == targetBytes[3])
                        {
                            long curPosition = stream.Position;
                            stream.Seek(i - n, System.IO.SeekOrigin.Current);
                            // workitem 10178
                            Workaround_Ladybug318918(stream);

                            // workitem 7711
                            int sig = ReadSignature(stream);

                            success = (sig == SignatureToFind);
                            if (!success)
                            {
                                stream.Seek(curPosition, System.IO.SeekOrigin.Begin);
                                // workitem 10178
                                Workaround_Ladybug318918(stream);
                            }
                            else
                                break; // out of for loop
                        }
                    }
                }
                else break;
                if (success) break;
            } while (true);

            if (!success)
            {
                stream.Seek(startingPosition, System.IO.SeekOrigin.Begin);
                // workitem 10178
                Workaround_Ladybug318918(stream);
                return -1; // or throw?
            }

            // subtract 4 for the signature.
            long BytesRead = (stream.Position - startingPosition) - 4;

            return BytesRead;
        }

        internal static DateTime AdjustTime_Reverse(DateTime time)
        {
            if (time.Kind == DateTimeKind.Utc) return time;
            DateTime adjusted = time;
            if (DateTime.Now.IsDaylightSavingTime() && !time.IsDaylightSavingTime())
                adjusted = time - new System.TimeSpan(1, 0, 0);

            else if (!DateTime.Now.IsDaylightSavingTime() && time.IsDaylightSavingTime())
                adjusted = time + new System.TimeSpan(1, 0, 0);

            return adjusted;
        }

        internal static DateTime PackedToDateTime(Int32 packedDateTime)
        {
            // workitem 7074 & workitem 7170
            if (packedDateTime == 0xFFFF || packedDateTime == 0)
                return new System.DateTime(1995, 1, 1, 0, 0, 0, 0); // return a fixed date when none is supplied.

            Int16 packedTime = unchecked((Int16)(packedDateTime & 0x0000ffff));
            Int16 packedDate = unchecked((Int16)((packedDateTime & 0xffff0000) >> 16));

            int year = 1980 + ((packedDate & 0xFE00) >> 9);
            int month = (packedDate & 0x01E0) >> 5;
            int day = packedDate & 0x001F;

            int hour = (packedTime & 0xF800) >> 11;
            int minute = (packedTime & 0x07E0) >> 5;
            //int second = packedTime & 0x001F;
            int second = (packedTime & 0x001F) * 2;

            // validation and error checking.
            // this is not foolproof but will catch most errors.
            if (second >= 60)
            {
                minute++;
                second = 0;
            }
            if (minute >= 60)
            {
                hour++;
                minute = 0;
            }
            if (hour >= 24)
            {
                day++;
                hour = 0;
            }

            DateTime d = System.DateTime.Now;
            Boolean success = false;
            try
            {
                d = new System.DateTime(year, month, day, hour, minute, second, 0);
                success = true;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                if (year == 1980 && (month == 0 || day == 0))
                {
                    try
                    {
                        d = new System.DateTime(1980, 1, 1, hour, minute, second, 0);
                        success = true;
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        try
                        {
                            d = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
                            success = true;
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                        }
                    }
                }
                    // workitem 8814
                    // my god, I can't believe how many different ways applications
                    // can mess up a simple date format.
                else
                {
                    try
                    {
                        while (year < 1980) year++;
                        while (year > 2030) year--;
                        while (month < 1) month++;
                        while (month > 12) month--;
                        while (day < 1) day++;
                        while (day > 28) day--;
                        while (minute < 0) minute++;
                        while (minute > 59) minute--;
                        while (second < 0) second++;
                        while (second > 59) second--;
                        d = new System.DateTime(year, month, day, hour, minute, second, 0);
                        success = true;
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                    }
                }
            }
            if (!success)
            {
                String msg = String.Format("y({0}) m({1}) d({2}) h({3}) m({4}) s({5})", year, month, day, hour, minute, second);
                throw new ZipException(String.Format("Bad date/time format in the zip file. ({0})", msg));
            }
            // workitem 6191
            //d = AdjustTime_Reverse(d);
            d = DateTime.SpecifyKind(d, DateTimeKind.Local);
            return d;
        }

        internal static Int32 DateTimeToPacked(DateTime time)
        {
            // The time is passed in here only for purposes of writing LastModified to the
            // zip archive. It should always be LocalTime, but we convert anyway.  And,
            // since the time is being written out, it needs to be adjusted.

            time = time.ToLocalTime();
            // workitem 7966
            //time = AdjustTime_Forward(time);

            // see http://www.vsft.com/hal/dostime.htm for the format
            UInt16 packedDate = (UInt16)((time.Day & 0x0000001F) | ((time.Month << 5) & 0x000001E0) | (((time.Year - 1980) << 9) & 0x0000FE00));
            UInt16 packedTime = (UInt16)((time.Second / 2 & 0x0000001F) | ((time.Minute << 5) & 0x000007E0) | ((time.Hour << 11) & 0x0000F800));

            Int32 result = (Int32)(((UInt32)(packedDate << 16)) | packedTime);
            return result;
        }

        public static void CreateAndOpenUniqueTempFile(String dir, out Stream fs, out String filename)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    filename = Path.Combine(dir, InternalGetTempFileName());
                    fs = new FileStream(filename, FileMode.CreateNew);
                    return;
                }
                catch (IOException)
                {
                    if (i == 2) throw;
                }
            }
            throw new IOException();
        }

        public static String InternalGetTempFileName()
        {
            return "zip-" + Path.GetRandomFileName().Substring(0, 8) + ".tmp";
        }

        internal static int ReadWithRetry(System.IO.Stream s, Byte[] buffer, int offset, int count, String FileName)
        {
            int n = 0;
            Boolean done = false;
            int retries = 0;
            do
            {
                try
                {
                    n = s.Read(buffer, offset, count);
                    done = true;
                }
                catch (System.IO.IOException ioexc1)
                {
                    // Check if we can call GetHRForException,
                    // which makes unmanaged code calls.
                    var p = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                    if (p.IsUnrestricted())
                    {
                        uint hresult = _HRForException(ioexc1);
                        if (hresult != 0x80070021) // ERROR_LOCK_VIOLATION
                            throw new System.IO.IOException(String.Format("Cannot read file {0}", FileName), ioexc1);
                        retries++;
                        if (retries > 10)
                            throw new System.IO.IOException(String.Format("Cannot read file {0}, at offset 0x{1:X8} after 10 retries", FileName, offset), ioexc1);

                        // max time waited on last retry = 250 + 10*550 = 5.75s
                        // aggregate time waited after 10 retries: 250 + 55*550 = 30.5s
                        System.Threading.Thread.Sleep(250 + retries * 550);
                    }
                    else
                    {
                        // The permission.Demand() failed. Therefore, we cannot call
                        // GetHRForException, and cannot do the subtle handling of
                        // ERROR_LOCK_VIOLATION.  Just bail.
                        throw;
                    }
                }
            } while (!done);

            return n;
        }

        private static uint _HRForException(System.Exception ex1)
        {
            return unchecked((uint)Interop.Marshal.GetHRForException(ex1));
        }

        public static int URShift(int number, int bits)
        {
            return (int)((uint)number >> bits);
        }

        public static System.Int32 ReadInput(System.IO.TextReader sourceTextReader, Byte[] target, int start, int count)
        {
            // Returns 0 Bytes if not enough space in target
            if (target.Length == 0) return 0;

            char[] charArray = new char[target.Length];
            int BytesRead = sourceTextReader.Read(charArray, start, count);

            // Returns -1 if EOF
            if (BytesRead == 0) return -1;

            for (int index = start; index < start + BytesRead; index++)
                target[index] = (Byte)charArray[index];

            return BytesRead;
        }

        internal static Byte[] ToByteArray(System.String sourceString)
        {
            return System.Text.Encoding.UTF8.GetBytes(sourceString);
        }

        internal static char[] ToCharArray(Byte[] ByteArray)
        {
            return System.Text.Encoding.UTF8.GetChars(ByteArray);
        }

        #endregion
    }

    #endregion Класс ZipSharedUtilities

    #region Класс ZipReadOptions

    public class ZipReadOptions
    {
        #region Свойства

        public TextWriter StatusMessageWriter { get; set; }

        public System.Text.Encoding @Encoding { get; set; }

        public EventHandler<ZipReadProgressEventArgs> ReadProgress { get; set; }

        #endregion
    }

    #endregion Класс ZipReadOptions

    #region Класс ZipBaseStream

    internal class ZipBaseStream : System.IO.Stream
    {
        #region Перечисления

        internal enum StreamMode
        {
            Writer,

            Reader,

            Undefined,
        }

        #endregion

        #region Поля

        protected internal ZipCompressionStrategy Strategy = ZipCompressionStrategy.Default;

        protected internal String _GzipComment;

        protected internal String _GzipFileName;

        protected internal DateTime _GzipMtime;

        protected internal Byte[] _buf1 = new Byte[1];

        protected internal int _bufferSize = ZipConstants.WorkingBufferSizeDefault;

        protected internal ZlibCodec _codec = null;

        protected internal ZipCompressionMode _compressionMode;

        private ZipCRC32 _crc;

        protected internal ZlibStreamFlavor _flavor;

        protected internal ZipFlushType _flushMode;

        protected internal int _gzipHeaderByteCount;

        protected internal Boolean _leaveOpen;

        protected internal ZipCompressionLevel _level;

        private Boolean _noMoreInput;

        protected internal System.IO.Stream _stream;

        protected internal StreamMode _streamMode = StreamMode.Undefined;

        protected internal Byte[] _workingBuffer;

        #endregion

        #region Свойства

        internal int Crc32
        {
            get
            {
                if (_crc == null) return 0;

                return _crc.Crc32Result;
            }
        }

        protected internal Boolean WantCompress
        {
            get { return (_compressionMode == ZipCompressionMode.Compress); }
        }

        private ZlibCodec Codec
        {
            get
            {
                if (_codec == null)
                {
                    Boolean wantRfc1950Header = (_flavor == ZlibStreamFlavor.ZLIB);
                    _codec = new ZlibCodec();
                    if (_compressionMode == ZipCompressionMode.Decompress)
                        _codec.InitializeInflate(wantRfc1950Header);
                    else
                    {
                        _codec.Strategy = Strategy;
                        _codec.InitializeDeflate(_level, wantRfc1950Header);
                    }
                }
                return _codec;
            }
        }

        private Byte[] WorkingBuffer
        {
            get
            {
                if (_workingBuffer == null)
                    _workingBuffer = new Byte[_bufferSize];
                return _workingBuffer;
            }
        }

        public override Boolean CanRead
        {
            get { return _stream.CanRead; }
        }

        public override Boolean CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override Boolean CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override System.Int64 Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region Конструктор

        public ZipBaseStream(System.IO.Stream stream, ZipCompressionMode compressionMode, ZipCompressionLevel level, ZlibStreamFlavor flavor, Boolean leaveOpen)
        {
            _flushMode = ZipFlushType.None;
            //this._workingBuffer = new Byte[WORKING_BUFFER_SIZE_DEFAULT];
            _stream = stream;
            _leaveOpen = leaveOpen;
            _compressionMode = compressionMode;
            _flavor = flavor;
            _level = level;
            // workitem 7159
            if (flavor == ZlibStreamFlavor.GZIP)
                _crc = new ZipCRC32();
        }

        #endregion

        #region Методы

        public override void Write(System.Byte[] buffer, int offset, int count)
        {
            // workitem 7159
            // calculate the CRC on the unccompressed data  (before writing)
            if (_crc != null)
                _crc.SlurpBlock(buffer, offset, count);

            if (_streamMode == StreamMode.Undefined)
                _streamMode = StreamMode.Writer;
            else if (_streamMode != StreamMode.Writer)
                throw new ZlibException("Cannot Write after Reading.");

            if (count == 0)
                return;

            // first reference of z property will initialize the private var _z
            Codec.InputBuffer = buffer;
            _codec.NextIn = offset;
            _codec.AvailableBytesIn = count;
            Boolean done = false;
            do
            {
                _codec.OutputBuffer = WorkingBuffer;
                _codec.NextOut = 0;
                _codec.AvailableBytesOut = _workingBuffer.Length;
                int rc = (WantCompress)
                             ? _codec.Deflate(_flushMode)
                             : _codec.Inflate(_flushMode);
                if (rc != ZipConstants.Z_OK && rc != ZipConstants.Z_STREAM_END)
                    throw new ZlibException((WantCompress ? "de" : "in") + "flating: " + _codec.Message);

                //if (_workingBuffer.Length - _z.AvailableBytesOut > 0)
                _stream.Write(_workingBuffer, 0, _workingBuffer.Length - _codec.AvailableBytesOut);

                done = _codec.AvailableBytesIn == 0 && _codec.AvailableBytesOut != 0;

                // If GZIP and de-compress, we're done when 8 Bytes remain.
                if (_flavor == ZlibStreamFlavor.GZIP && !WantCompress)
                    done = (_codec.AvailableBytesIn == 8 && _codec.AvailableBytesOut != 0);
            } while (!done);
        }

        private void finish()
        {
            if (_codec == null) return;

            if (_streamMode == StreamMode.Writer)
            {
                Boolean done = false;
                do
                {
                    _codec.OutputBuffer = WorkingBuffer;
                    _codec.NextOut = 0;
                    _codec.AvailableBytesOut = _workingBuffer.Length;
                    int rc = (WantCompress)
                                 ? _codec.Deflate(ZipFlushType.Finish)
                                 : _codec.Inflate(ZipFlushType.Finish);

                    if (rc != ZipConstants.Z_STREAM_END && rc != ZipConstants.Z_OK)
                    {
                        String verb = (WantCompress ? "de" : "in") + "flating";
                        if (_codec.Message == null)
                            throw new ZlibException(String.Format("{0}: (rc = {1})", verb, rc));
                        else
                            throw new ZlibException(verb + ": " + _codec.Message);
                    }

                    if (_workingBuffer.Length - _codec.AvailableBytesOut > 0)
                        _stream.Write(_workingBuffer, 0, _workingBuffer.Length - _codec.AvailableBytesOut);

                    done = _codec.AvailableBytesIn == 0 && _codec.AvailableBytesOut != 0;
                    // If GZIP and de-compress, we're done when 8 Bytes remain.
                    if (_flavor == ZlibStreamFlavor.GZIP && !WantCompress)
                        done = (_codec.AvailableBytesIn == 8 && _codec.AvailableBytesOut != 0);
                } while (!done);

                Flush();

                // workitem 7159
                if (_flavor == ZlibStreamFlavor.GZIP)
                {
                    if (WantCompress)
                    {
                        // Emit the GZIP trailer: CRC32 and  size mod 2^32
                        int c1 = _crc.Crc32Result;
                        _stream.Write(BitConverter.GetBytes(c1), 0, 4);
                        int c2 = (Int32)(_crc.TotalBytesRead & 0x00000000FFFFFFFF);
                        _stream.Write(BitConverter.GetBytes(c2), 0, 4);
                    }
                    else
                        throw new ZlibException("Writing with decompression is not supported.");
                }
            }
                // workitem 7159
            else if (_streamMode == StreamMode.Reader)
            {
                if (_flavor == ZlibStreamFlavor.GZIP)
                {
                    if (!WantCompress)
                    {
                        // workitem 8501: handle edge case (decompress empty stream)
                        if (_codec.TotalBytesOut == 0L)
                            return;

                        // Read and potentially verify the GZIP trailer:
                        // CRC32 and size mod 2^32
                        Byte[] trailer = new Byte[8];

                        // workitems 8679 & 12554
                        if (_codec.AvailableBytesIn < 8)
                        {
                            // Make sure we have read to the end of the stream
                            Array.Copy(_codec.InputBuffer, _codec.NextIn, trailer, 0, _codec.AvailableBytesIn);
                            int BytesNeeded = 8 - _codec.AvailableBytesIn;
                            int BytesRead = _stream.Read(trailer,
                                                         _codec.AvailableBytesIn,
                                                         BytesNeeded);
                            if (BytesNeeded != BytesRead)
                            {
                                throw new ZlibException(String.Format("Missing or incomplete GZIP trailer. Expected 8 Bytes, got {0}.",
                                                                      _codec.AvailableBytesIn + BytesRead));
                            }
                        }
                        else
                            Array.Copy(_codec.InputBuffer, _codec.NextIn, trailer, 0, trailer.Length);

                        Int32 crc32_expected = BitConverter.ToInt32(trailer, 0);
                        Int32 crc32_actual = _crc.Crc32Result;
                        Int32 isize_expected = BitConverter.ToInt32(trailer, 4);
                        Int32 isize_actual = (Int32)(_codec.TotalBytesOut & 0x00000000FFFFFFFF);

                        if (crc32_actual != crc32_expected)
                            throw new ZlibException(String.Format("Bad CRC32 in GZIP trailer. (actual({0:X8})!=expected({1:X8}))", crc32_actual, crc32_expected));

                        if (isize_actual != isize_expected)
                            throw new ZlibException(String.Format("Bad size in GZIP trailer. (actual({0})!=expected({1}))", isize_actual, isize_expected));
                    }
                    else
                        throw new ZlibException("Reading with compression is not supported.");
                }
            }
        }

        private void end()
        {
            if (Codec == null)
                return;
            if (WantCompress)
                _codec.EndDeflate();
            else
                _codec.EndInflate();
            _codec = null;
        }

        public override void Close()
        {
            if (_stream == null) return;
            try
            {
                finish();
            }
            finally
            {
                end();
                if (!_leaveOpen) _stream.Close();
                _stream = null;
            }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override System.Int64 Seek(System.Int64 offset, System.IO.SeekOrigin origin)
        {
            throw new NotImplementedException();
            //_outStream.Seek(offset, origin);
        }

        public override void SetLength(System.Int64 value)
        {
            _stream.SetLength(value);
        }

        private String ReadZeroTerminatedString()
        {
            var list = new System.Collections.Generic.List<Byte>();
            Boolean done = false;
            do
            {
                // workitem 7740
                int n = _stream.Read(_buf1, 0, 1);
                if (n != 1)
                    throw new ZlibException("Unexpected EOF reading GZIP header.");
                else
                {
                    if (_buf1[0] == 0)
                        done = true;
                    else
                        list.Add(_buf1[0]);
                }
            } while (!done);
            Byte[] a = list.ToArray();
            return GZipStream.iso8859dash1.GetString(a, 0, a.Length);
        }

        private int _ReadAndValidateGzipHeader()
        {
            int totalBytesRead = 0;
            // read the header on the first read
            Byte[] header = new Byte[10];
            int n = _stream.Read(header, 0, header.Length);

            // workitem 8501: handle edge case (decompress empty stream)
            if (n == 0)
                return 0;

            if (n != 10)
                throw new ZlibException("Not a valid GZIP stream.");

            if (header[0] != 0x1F || header[1] != 0x8B || header[2] != 8)
                throw new ZlibException("Bad GZIP header.");

            Int32 timet = BitConverter.ToInt32(header, 4);
            _GzipMtime = GZipStream._unixEpoch.AddSeconds(timet);
            totalBytesRead += n;
            if ((header[3] & 0x04) == 0x04)
            {
                // read and discard extra field
                n = _stream.Read(header, 0, 2); // 2-Byte length field
                totalBytesRead += n;

                Int16 extraLength = (Int16)(header[0] + header[1] * 256);
                Byte[] extra = new Byte[extraLength];
                n = _stream.Read(extra, 0, extra.Length);
                if (n != extraLength)
                    throw new ZlibException("Unexpected end-of-file reading GZIP header.");
                totalBytesRead += n;
            }
            if ((header[3] & 0x08) == 0x08)
                _GzipFileName = ReadZeroTerminatedString();
            if ((header[3] & 0x10) == 0x010)
                _GzipComment = ReadZeroTerminatedString();
            if ((header[3] & 0x02) == 0x02)
                Read(_buf1, 0, 1); // CRC16, ignore

            return totalBytesRead;
        }

        public override System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
        {
            // According to MS documentation, any implementation of the IO.Stream.Read function must:
            // (a) throw an exception if offset & count reference an invalid part of the buffer,
            //     or if count < 0, or if buffer is null
            // (b) return 0 only upon EOF, or if count = 0
            // (c) if not EOF, then return at least 1 Byte, up to <count> Bytes

            if (_streamMode == StreamMode.Undefined)
            {
                if (!_stream.CanRead) throw new ZlibException("The stream is not readable.");
                // for the first read, set up some controls.
                _streamMode = StreamMode.Reader;
                // (The first reference to _z goes through the private accessor which
                // may initialize it.)
                Codec.AvailableBytesIn = 0;
                if (_flavor == ZlibStreamFlavor.GZIP)
                {
                    _gzipHeaderByteCount = _ReadAndValidateGzipHeader();
                    // workitem 8501: handle edge case (decompress empty stream)
                    if (_gzipHeaderByteCount == 0)
                        return 0;
                }
            }

            if (_streamMode != StreamMode.Reader)
                throw new ZlibException("Cannot Read after Writing.");

            if (count == 0) return 0;
            if (_noMoreInput && WantCompress) return 0; // workitem 8557
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            if (offset < buffer.GetLowerBound(0)) throw new ArgumentOutOfRangeException("offset");
            if ((offset + count) > buffer.GetLength(0)) throw new ArgumentOutOfRangeException("count");

            int rc = 0;

            // set up the output of the deflate/inflate codec:
            _codec.OutputBuffer = buffer;
            _codec.NextOut = offset;
            _codec.AvailableBytesOut = count;

            // This is necessary in case _workingBuffer has been resized. (new Byte[])
            // (The first reference to _workingBuffer goes through the private accessor which
            // may initialize it.)
            _codec.InputBuffer = WorkingBuffer;

            do
            {
                // need data in _workingBuffer in order to deflate/inflate.  Here, we check if we have any.
                if ((_codec.AvailableBytesIn == 0) && (!_noMoreInput))
                {
                    // No data available, so try to Read data from the captive stream.
                    _codec.NextIn = 0;
                    _codec.AvailableBytesIn = _stream.Read(_workingBuffer, 0, _workingBuffer.Length);
                    if (_codec.AvailableBytesIn == 0)
                        _noMoreInput = true;
                }
                // we have data in InputBuffer; now compress or decompress as appropriate
                rc = (WantCompress)
                         ? _codec.Deflate(_flushMode)
                         : _codec.Inflate(_flushMode);

                if (_noMoreInput && (rc == ZipConstants.Z_BUF_ERROR))
                    return 0;

                if (rc != ZipConstants.Z_OK && rc != ZipConstants.Z_STREAM_END)
                    throw new ZlibException(String.Format("{0}flating:  rc={1}  msg={2}", (WantCompress ? "de" : "in"), rc, _codec.Message));

                if ((_noMoreInput || rc == ZipConstants.Z_STREAM_END) && (_codec.AvailableBytesOut == count))
                    break; // nothing more to read
            } //while (_z.AvailableBytesOut == count && rc == ZipConstants.Z_OK);
            while (_codec.AvailableBytesOut > 0 && !_noMoreInput && rc == ZipConstants.Z_OK);

            // workitem 8557
            // is there more room in output?
            if (_codec.AvailableBytesOut > 0)
            {
                if (rc == ZipConstants.Z_OK && _codec.AvailableBytesIn == 0)
                {
                    // deferred
                }

                // are we completely done reading?
                if (_noMoreInput)
                {
                    // and in compression?
                    if (WantCompress)
                    {
                        // no more input data available; therefore we flush to
                        // try to complete the read
                        rc = _codec.Deflate(ZipFlushType.Finish);

                        if (rc != ZipConstants.Z_OK && rc != ZipConstants.Z_STREAM_END)
                            throw new ZlibException(String.Format("Deflating:  rc={0}  msg={1}", rc, _codec.Message));
                    }
                }
            }

            rc = (count - _codec.AvailableBytesOut);

            // calculate CRC after reading
            if (_crc != null)
                _crc.SlurpBlock(buffer, offset, rc);

            return rc;
        }

        public static void CompressString(String s, Stream compressor)
        {
            Byte[] uncompressed = System.Text.Encoding.UTF8.GetBytes(s);
            using (compressor)
                compressor.Write(uncompressed, 0, uncompressed.Length);
        }

        public static void CompressBuffer(Byte[] b, Stream compressor)
        {
            // workitem 8460
            using (compressor)
                compressor.Write(b, 0, b.Length);
        }

        public static String UncompressString(Byte[] compressed, Stream decompressor)
        {
            // workitem 8460
            Byte[] working = new Byte[1024];
            var encoding = System.Text.Encoding.UTF8;
            using (var output = new MemoryStream())
            {
                using (decompressor)
                {
                    int n;
                    while ((n = decompressor.Read(working, 0, working.Length)) != 0)
                        output.Write(working, 0, n);
                }

                // reset to allow read from start
                output.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(output, encoding);
                return sr.ReadToEnd();
            }
        }

        public static Byte[] UncompressBuffer(Byte[] compressed, Stream decompressor)
        {
            // workitem 8460
            Byte[] working = new Byte[1024];
            using (var output = new MemoryStream())
            {
                using (decompressor)
                {
                    int n;
                    while ((n = decompressor.Read(working, 0, working.Length)) != 0)
                        output.Write(working, 0, n);
                }
                return output.ToArray();
            }
        }

        #endregion
    }

    #endregion Класс ZipBaseStream

    #region Класс ZlibCodec

    [Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d0000D")]
    [Interop.ComVisible(true)]
    [Interop.ClassInterface(Interop.ClassInterfaceType.AutoDispatch)]
    public sealed class ZlibCodec
    {
        #region Поля

        public int AvailableBytesIn;

        public int AvailableBytesOut;

        public ZipCompressionLevel CompressLevel = ZipCompressionLevel.Default;

        public Byte[] InputBuffer;

        public System.String Message;

        public int NextIn;

        public int NextOut;

        public Byte[] OutputBuffer;

        public ZipCompressionStrategy Strategy = ZipCompressionStrategy.Default;

        public long TotalBytesIn;

        public long TotalBytesOut;

        public int WindowBits = ZipConstants.WindowBitsDefault;

        internal uint _Adler32;

        internal ZipDeflateManager dstate;

        internal ZipInflateManager istate;

        #endregion

        #region Свойства

        public int Adler32
        {
            get { return (int)_Adler32; }
        }

        #endregion

        #region Конструктор

        public ZlibCodec()
        {
        }

        public ZlibCodec(ZipCompressionMode mode)
        {
            if (mode == ZipCompressionMode.Compress)
            {
                int rc = InitializeDeflate();
                if (rc != ZipConstants.Z_OK) throw new ZlibException("Cannot initialize for deflate.");
            }
            else if (mode == ZipCompressionMode.Decompress)
            {
                int rc = InitializeInflate();
                if (rc != ZipConstants.Z_OK) throw new ZlibException("Cannot initialize for inflate.");
            }
            else throw new ZlibException("Invalid ZlibStreamFlavor.");
        }

        #endregion

        #region Методы

        public int InitializeInflate()
        {
            return InitializeInflate(WindowBits);
        }

        public int InitializeInflate(Boolean expectRfc1950Header)
        {
            return InitializeInflate(WindowBits, expectRfc1950Header);
        }

        public int InitializeInflate(int windowBits)
        {
            WindowBits = windowBits;
            return InitializeInflate(windowBits, true);
        }

        public int InitializeInflate(int windowBits, Boolean expectRfc1950Header)
        {
            WindowBits = windowBits;
            if (dstate != null) throw new ZlibException("You may not call InitializeInflate() after calling InitializeDeflate().");
            istate = new ZipInflateManager(expectRfc1950Header);
            return istate.Initialize(this, windowBits);
        }

        public int Inflate(ZipFlushType flush)
        {
            if (istate == null)
                throw new ZlibException("No Inflate State!");
            return istate.Inflate(flush);
        }

        public int EndInflate()
        {
            if (istate == null)
                throw new ZlibException("No Inflate State!");
            int ret = istate.End();
            istate = null;
            return ret;
        }

        public int SyncInflate()
        {
            if (istate == null)
                throw new ZlibException("No Inflate State!");
            return istate.Sync();
        }

        public int InitializeDeflate()
        {
            return _InternalInitializeDeflate(true);
        }

        public int InitializeDeflate(ZipCompressionLevel level)
        {
            CompressLevel = level;
            return _InternalInitializeDeflate(true);
        }

        public int InitializeDeflate(ZipCompressionLevel level, Boolean wantRfc1950Header)
        {
            CompressLevel = level;
            return _InternalInitializeDeflate(wantRfc1950Header);
        }

        public int InitializeDeflate(ZipCompressionLevel level, int bits)
        {
            CompressLevel = level;
            WindowBits = bits;
            return _InternalInitializeDeflate(true);
        }

        public int InitializeDeflate(ZipCompressionLevel level, int bits, Boolean wantRfc1950Header)
        {
            CompressLevel = level;
            WindowBits = bits;
            return _InternalInitializeDeflate(wantRfc1950Header);
        }

        private int _InternalInitializeDeflate(Boolean wantRfc1950Header)
        {
            if (istate != null) throw new ZlibException("You may not call InitializeDeflate() after calling InitializeInflate().");
            dstate = new ZipDeflateManager();
            dstate.WantRfc1950HeaderBytes = wantRfc1950Header;

            return dstate.Initialize(this, CompressLevel, WindowBits, Strategy);
        }

        public int Deflate(ZipFlushType flush)
        {
            if (dstate == null)
                throw new ZlibException("No Deflate State!");
            return dstate.Deflate(flush);
        }

        public int EndDeflate()
        {
            if (dstate == null)
                throw new ZlibException("No Deflate State!");
            // TODO: dinoch Tue, 03 Nov 2009  15:39 (test this)
            //int ret = dstate.End();
            dstate = null;
            return ZipConstants.Z_OK; //ret;
        }

        public void ResetDeflate()
        {
            if (dstate == null)
                throw new ZlibException("No Deflate State!");
            dstate.Reset();
        }

        public int SetDeflateParams(ZipCompressionLevel level, ZipCompressionStrategy strategy)
        {
            if (dstate == null)
                throw new ZlibException("No Deflate State!");
            return dstate.SetParams(level, strategy);
        }

        public int SetDictionary(Byte[] dictionary)
        {
            if (istate != null)
                return istate.SetDictionary(dictionary);

            if (dstate != null)
                return dstate.SetDictionary(dictionary);

            throw new ZlibException("No Inflate or Deflate state!");
        }

        internal void flush_pending()
        {
            int len = dstate.pendingCount;

            if (len > AvailableBytesOut)
                len = AvailableBytesOut;
            if (len == 0)
                return;

            if (dstate.pending.Length <= dstate.nextPending ||
                OutputBuffer.Length <= NextOut ||
                dstate.pending.Length < (dstate.nextPending + len) ||
                OutputBuffer.Length < (NextOut + len))
            {
                throw new ZlibException(String.Format("Invalid State. (pending.Length={0}, pendingCount={1})",
                                                      dstate.pending.Length, dstate.pendingCount));
            }

            Array.Copy(dstate.pending, dstate.nextPending, OutputBuffer, NextOut, len);

            NextOut += len;
            dstate.nextPending += len;
            TotalBytesOut += len;
            AvailableBytesOut -= len;
            dstate.pendingCount -= len;
            if (dstate.pendingCount == 0)
                dstate.nextPending = 0;
        }

        internal int read_buf(Byte[] buf, int start, int size)
        {
            int len = AvailableBytesIn;

            if (len > size)
                len = size;
            if (len == 0)
                return 0;

            AvailableBytesIn -= len;

            if (dstate.WantRfc1950HeaderBytes)
                _Adler32 = ZipAdler.Adler32(_Adler32, InputBuffer, NextIn, len);
            Array.Copy(InputBuffer, NextIn, buf, start, len);
            NextIn += len;
            TotalBytesIn += len;
            return len;
        }

        #endregion
    }

    #endregion Класс ZlibCodec

    #region Класс BZip2

    internal static class BZip2
    {
        #region Поля

        public static readonly int BlockSizeMultiple = 100000;

        public static readonly int G_SIZE = 50;

        public static readonly int MaxAlphaSize = 258;

        public static readonly int MaxBlockSize = 9;

        public static readonly int MaxCodeLength = 23;

        public static readonly int MaxSelectors = (2 + (900000 / G_SIZE));

        public static readonly int MinBlockSize = 1;

        public static readonly int NGroups = 6;

        public static readonly int NUM_OVERSHOOT_ByteS = 20;

        public static readonly int N_ITERS = 4;

        /*
         * <p> If you are ever unlucky/improbable enough to get a stack
         * overflow whilst sorting, increase the following constant and
         * try again. In practice I have never seen the stack go above 27
         * elems, so the following limit seems very generous.  </p>
         */

        internal static readonly int QSORT_STACK_SIZE = 1000;

        public static readonly char RUNA = (char)0;

        public static readonly char RUNB = (char)1;

        #endregion

        #region Методы

        internal static T[][] InitRectangularArray<T>(int d1, int d2)
        {
            var x = new T[d1][];
            for (int i = 0; i < d1; i++)
                x[i] = new T[d2];
            return x;
        }

        #endregion
    }

    #endregion Класс BZip2

    #region Класс ZipDeflateManager

    internal sealed class ZipDeflateManager
    {
        #region Делегаты

        internal delegate ZipBlockState CompressFunc(ZipFlushType flush);

        #endregion

        #region Поля

        private static readonly int BUSY_STATE = 113;

        private static readonly int Buf_size = 8 * 2;

        private static readonly int DYN_TREES = 2;

        private static readonly int END_BLOCK = 256;

        private static readonly int FINISH_STATE = 666;

        private static readonly int HEAP_SIZE = (2 * ZipConstants.L_CODES + 1);

        private static readonly int INIT_STATE = 42;

        private static readonly int MAX_MATCH = 258;

        private static readonly int MEM_LEVEL_DEFAULT = 8;

        private static readonly int MEM_LEVEL_MAX = 9;

        private static readonly int MIN_LOOKAHEAD = (MAX_MATCH + MIN_MATCH + 1);

        private static readonly int MIN_MATCH = 3;

        private static readonly int PRESET_DICT = 0x20;

        private static readonly int STATIC_TREES = 1;

        private static readonly int STORED_BLOCK = 0;

        private static readonly int Z_ASCII = 1;

        private static readonly int Z_BINARY = 0;

        private static readonly int Z_DEFLATED = 8;

        private static readonly int Z_UNKNOWN = 2;

        private static readonly System.String[] _errorMessage = new[]
            {
                "need dictionary",
                "stream end",
                "",
                "file error",
                "stream error",
                "data error",
                "insufficient memory",
                "buffer error",
                "incompatible version",
                ""
            };

        private Boolean Rfc1950BytesEmitted;

        private Boolean _WantRfc1950HeaderBytes = true;

        internal ZlibCodec _codec; // the zlib encoder/decoder

        private CompressFunc _deflateFunction;

        internal int _distanceOffset; // index into pending; points to distance data??

        internal int _lengthOffset; // index for literals or lengths

        internal short bi_buf;

        internal int bi_valid;

        internal short[] bl_count = new short[ZipConstants.MAX_BITS + 1];

        internal short[] bl_tree; // Huffman tree for bit lengths

        internal int block_start;

        internal ZipCompressionLevel compressionLevel; // compression level (1..9)

        internal ZipCompressionStrategy compressionStrategy; // favor or force Huffman coding

        private ZipConfig config;

        internal SByte data_type; // UNKNOWN, BINARY or ASCII

        internal SByte[] depth = new SByte[2 * ZipConstants.L_CODES + 1];

        internal short[] dyn_dtree; // distance tree

        internal short[] dyn_ltree; // literal and length tree

        internal int hash_bits; // log2(hash_size)

        internal int hash_mask; // hash_size-1

        internal int hash_shift;

        internal int hash_size; // number of elements in hash table

        internal short[] head; // Heads of the hash chains or NIL.

        internal int[] heap = new int[2 * ZipConstants.L_CODES + 1];

        internal int heap_len; // number of elements in the heap

        internal int heap_max; // element of largest frequency

        internal int ins_h; // hash index of String to be inserted

        internal int last_eob_len; // bit length of EOB code for last block

        internal int last_flush; // value of flush param for previous deflate call

        internal int last_lit; // running index in l_buf

        internal int lit_bufsize;

        internal int lookahead; // number of valid Bytes ahead in window

        internal int match_available; // set if previous match exists

        internal int match_length; // length of best match

        internal int match_start; // start of matching String

        internal int matches; // number of String matches in current block

        internal int nextPending; // index of next pending Byte to output to the stream

        internal int opt_len; // bit length of current block with optimal trees

        internal Byte[] pending; // output still pending - waiting to be compressed

        internal int pendingCount; // number of Bytes in the pending buffer

        internal short[] prev;

        internal int prev_length;

        internal int prev_match; // previous match

        internal int static_len; // bit length of current block with static trees

        internal int status; // as the name implies

        internal int strstart; // start of String to insert into.....????

        internal ZipTree treeBitLengths = new ZipTree(); // desc for bit length tree

        internal ZipTree treeDistances = new ZipTree(); // desc for distance tree

        internal ZipTree treeLiterals = new ZipTree(); // desc for literal tree

        internal int w_bits; // log2(w_size)  (8..16)

        internal int w_mask; // w_size - 1

        internal int w_size; // LZ77 window size (32K by default)

        internal Byte[] window;

        internal int window_size;

        #endregion

        #region Свойства

        internal Boolean WantRfc1950HeaderBytes
        {
            get { return _WantRfc1950HeaderBytes; }
            set { _WantRfc1950HeaderBytes = value; }
        }

        #endregion

        #region Конструктор

        internal ZipDeflateManager()
        {
            dyn_ltree = new short[HEAP_SIZE * 2];
            dyn_dtree = new short[(2 * ZipConstants.D_CODES + 1) * 2]; // distance tree
            bl_tree = new short[(2 * ZipConstants.BL_CODES + 1) * 2]; // Huffman tree for bit lengths
        }

        #endregion

        #region Методы

        private void _InitializeLazyMatch()
        {
            window_size = 2 * w_size;

            // clear the hash - workitem 9063
            Array.Clear(head, 0, hash_size);
            //for (int i = 0; i < hash_size; i++) head[i] = 0;

            config = ZipConfig.Lookup(compressionLevel);
            SetDeflater();

            strstart = 0;
            block_start = 0;
            lookahead = 0;
            match_length = prev_length = MIN_MATCH - 1;
            match_available = 0;
            ins_h = 0;
        }

        private void _InitializeTreeData()
        {
            treeLiterals.dyn_tree = dyn_ltree;
            treeLiterals.staticTree = ZipStaticTree.Literals;

            treeDistances.dyn_tree = dyn_dtree;
            treeDistances.staticTree = ZipStaticTree.Distances;

            treeBitLengths.dyn_tree = bl_tree;
            treeBitLengths.staticTree = ZipStaticTree.BitLengths;

            bi_buf = 0;
            bi_valid = 0;
            last_eob_len = 8; // enough lookahead for inflate

            // Initialize the first block of the first file:
            _InitializeBlocks();
        }

        internal void _InitializeBlocks()
        {
            // Initialize the trees.
            for (int i = 0; i < ZipConstants.L_CODES; i++)
                dyn_ltree[i * 2] = 0;
            for (int i = 0; i < ZipConstants.D_CODES; i++)
                dyn_dtree[i * 2] = 0;
            for (int i = 0; i < ZipConstants.BL_CODES; i++)
                bl_tree[i * 2] = 0;

            dyn_ltree[END_BLOCK * 2] = 1;
            opt_len = static_len = 0;
            last_lit = matches = 0;
        }

        internal void pqdownheap(short[] tree, int k)
        {
            int v = heap[k];
            int j = k << 1; // left son of k
            while (j <= heap_len)
            {
                // Set j to the smallest of the two sons:
                if (j < heap_len && _IsSmaller(tree, heap[j + 1], heap[j], depth))
                    j++;
                // Exit if v is smaller than both sons
                if (_IsSmaller(tree, v, heap[j], depth))
                    break;

                // Exchange v with the smallest son
                heap[k] = heap[j];
                k = j;
                // And continue down the tree, setting j to the left son of k
                j <<= 1;
            }
            heap[k] = v;
        }

        internal static Boolean _IsSmaller(short[] tree, int n, int m, SByte[] depth)
        {
            short tn2 = tree[n * 2];
            short tm2 = tree[m * 2];
            return (tn2 < tm2 || (tn2 == tm2 && depth[n] <= depth[m]));
        }

        internal void scan_tree(short[] tree, int max_code)
        {
            int n; // iterates over all tree elements
            int prevlen = -1; // last emitted length
            int curlen; // length of current code
            int nextlen = tree[0 * 2 + 1]; // length of next code
            int count = 0; // repeat count of the current code
            int max_count = 7; // max repeat count
            int min_count = 4; // min repeat count

            if (nextlen == 0)
            {
                max_count = 138;
                min_count = 3;
            }
            tree[(max_code + 1) * 2 + 1] = 0x7fff; // guard //??

            for (n = 0; n <= max_code; n++)
            {
                curlen = nextlen;
                nextlen = tree[(n + 1) * 2 + 1];
                if (++count < max_count && curlen == nextlen)
                    continue;
                else if (count < min_count)
                    bl_tree[curlen * 2] = (short)(bl_tree[curlen * 2] + count);
                else if (curlen != 0)
                {
                    if (curlen != prevlen)
                        bl_tree[curlen * 2]++;
                    bl_tree[ZipConstants.REP_3_6 * 2]++;
                }
                else if (count <= 10)
                    bl_tree[ZipConstants.REPZ_3_10 * 2]++;
                else
                    bl_tree[ZipConstants.REPZ_11_138 * 2]++;
                count = 0;
                prevlen = curlen;
                if (nextlen == 0)
                {
                    max_count = 138;
                    min_count = 3;
                }
                else if (curlen == nextlen)
                {
                    max_count = 6;
                    min_count = 3;
                }
                else
                {
                    max_count = 7;
                    min_count = 4;
                }
            }
        }

        internal int build_bl_tree()
        {
            int max_blindex; // index of last bit length code of non zero freq

            // Determine the bit length frequencies for literal and distance trees
            scan_tree(dyn_ltree, treeLiterals.max_code);
            scan_tree(dyn_dtree, treeDistances.max_code);

            // Build the bit length tree:
            treeBitLengths.build_tree(this);
            // opt_len now includes the length of the tree representations, except
            // the lengths of the bit lengths codes and the 5+5+4 bits for the counts.

            // Determine the number of bit length codes to send. The pkzip format
            // requires that at least 4 bit length codes be sent. (appnote.txt says
            // 3 but the actual value used is 4.)
            for (max_blindex = ZipConstants.BL_CODES - 1; max_blindex >= 3; max_blindex--)
            {
                if (bl_tree[ZipTree.bl_order[max_blindex] * 2 + 1] != 0)
                    break;
            }
            // Update opt_len to include the bit length tree and counts
            opt_len += 3 * (max_blindex + 1) + 5 + 5 + 4;

            return max_blindex;
        }

        internal void send_all_trees(int lcodes, int dcodes, int blcodes)
        {
            int rank; // index in bl_order

            send_bits(lcodes - 257, 5); // not +255 as stated in appnote.txt
            send_bits(dcodes - 1, 5);
            send_bits(blcodes - 4, 4); // not -3 as stated in appnote.txt
            for (rank = 0; rank < blcodes; rank++)
                send_bits(bl_tree[ZipTree.bl_order[rank] * 2 + 1], 3);
            send_tree(dyn_ltree, lcodes - 1); // literal tree
            send_tree(dyn_dtree, dcodes - 1); // distance tree
        }

        internal void send_tree(short[] tree, int max_code)
        {
            int n; // iterates over all tree elements
            int prevlen = -1; // last emitted length
            int curlen; // length of current code
            int nextlen = tree[0 * 2 + 1]; // length of next code
            int count = 0; // repeat count of the current code
            int max_count = 7; // max repeat count
            int min_count = 4; // min repeat count

            if (nextlen == 0)
            {
                max_count = 138;
                min_count = 3;
            }

            for (n = 0; n <= max_code; n++)
            {
                curlen = nextlen;
                nextlen = tree[(n + 1) * 2 + 1];
                if (++count < max_count && curlen == nextlen)
                    continue;
                else if (count < min_count)
                {
                    do
                    {
                        send_code(curlen, bl_tree);
                    } while (--count != 0);
                }
                else if (curlen != 0)
                {
                    if (curlen != prevlen)
                    {
                        send_code(curlen, bl_tree);
                        count--;
                    }
                    send_code(ZipConstants.REP_3_6, bl_tree);
                    send_bits(count - 3, 2);
                }
                else if (count <= 10)
                {
                    send_code(ZipConstants.REPZ_3_10, bl_tree);
                    send_bits(count - 3, 3);
                }
                else
                {
                    send_code(ZipConstants.REPZ_11_138, bl_tree);
                    send_bits(count - 11, 7);
                }
                count = 0;
                prevlen = curlen;
                if (nextlen == 0)
                {
                    max_count = 138;
                    min_count = 3;
                }
                else if (curlen == nextlen)
                {
                    max_count = 6;
                    min_count = 3;
                }
                else
                {
                    max_count = 7;
                    min_count = 4;
                }
            }
        }

        private void put_Bytes(Byte[] p, int start, int len)
        {
            Array.Copy(p, start, pending, pendingCount, len);
            pendingCount += len;
        }

        internal void send_code(int c, short[] tree)
        {
            int c2 = c * 2;
            send_bits((tree[c2] & 0xffff), (tree[c2 + 1] & 0xffff));
        }

        internal void send_bits(int value, int length)
        {
            int len = length;
            unchecked
            {
                if (bi_valid > Buf_size - len)
                {
                    //int val = value;
                    //      bi_buf |= (val << bi_valid);

                    bi_buf |= (short)((value << bi_valid) & 0xffff);
                    //put_short(bi_buf);
                    pending[pendingCount++] = (Byte)bi_buf;
                    pending[pendingCount++] = (Byte)(bi_buf >> 8);

                    bi_buf = (short)((uint)value >> (Buf_size - bi_valid));
                    bi_valid += len - Buf_size;
                }
                else
                {
                    //      bi_buf |= (value) << bi_valid;
                    bi_buf |= (short)((value << bi_valid) & 0xffff);
                    bi_valid += len;
                }
            }
        }

        internal void _tr_align()
        {
            send_bits(STATIC_TREES << 1, 3);
            send_code(END_BLOCK, ZipStaticTree.lengthAndLiteralsTreeCodes);

            bi_flush();

            // Of the 10 bits for the empty block, we have already sent
            // (10 - bi_valid) bits. The lookahead for the last real code (before
            // the EOB of the previous block) was thus at least one plus the length
            // of the EOB plus what we have just sent of the empty static block.
            if (1 + last_eob_len + 10 - bi_valid < 9)
            {
                send_bits(STATIC_TREES << 1, 3);
                send_code(END_BLOCK, ZipStaticTree.lengthAndLiteralsTreeCodes);
                bi_flush();
            }
            last_eob_len = 7;
        }

        internal Boolean _tr_tally(int dist, int lc)
        {
            pending[_distanceOffset + last_lit * 2] = unchecked((Byte)((uint)dist >> 8));
            pending[_distanceOffset + last_lit * 2 + 1] = unchecked((Byte)dist);
            pending[_lengthOffset + last_lit] = unchecked((Byte)lc);
            last_lit++;

            if (dist == 0)
            {
                // lc is the unmatched char
                dyn_ltree[lc * 2]++;
            }
            else
            {
                matches++;
                // Here, lc is the match length - MIN_MATCH
                dist--; // dist = match distance - 1
                dyn_ltree[(ZipTree.LengthCode[lc] + ZipConstants.LITERALS + 1) * 2]++;
                dyn_dtree[ZipTree.DistanceCode(dist) * 2]++;
            }

            if ((last_lit & 0x1fff) == 0 && (int)compressionLevel > 2)
            {
                // Compute an upper bound for the compressed length
                int out_length = last_lit << 3;
                int in_length = strstart - block_start;
                int dcode;
                for (dcode = 0; dcode < ZipConstants.D_CODES; dcode++)
                    out_length = (int)(out_length + dyn_dtree[dcode * 2] * (5L + ZipTree.ExtraDistanceBits[dcode]));
                out_length >>= 3;
                if ((matches < (last_lit / 2)) && out_length < in_length / 2)
                    return true;
            }

            return (last_lit == lit_bufsize - 1) || (last_lit == lit_bufsize);
            // dinoch - wraparound?
            // We avoid equality with lit_bufsize because of wraparound at 64K
            // on 16 bit machines and because stored blocks are restricted to
            // 64K-1 Bytes.
        }

        internal void send_compressed_block(short[] ltree, short[] dtree)
        {
            int distance; // distance of matched String
            int lc; // match length or unmatched char (if dist == 0)
            int lx = 0; // running index in l_buf
            int code; // the code to send
            int extra; // number of extra bits to send

            if (last_lit != 0)
            {
                do
                {
                    int ix = _distanceOffset + lx * 2;
                    distance = ((pending[ix] << 8) & 0xff00) |
                               (pending[ix + 1] & 0xff);
                    lc = (pending[_lengthOffset + lx]) & 0xff;
                    lx++;

                    if (distance == 0)
                        send_code(lc, ltree); // send a literal Byte
                    else
                    {
                        // literal or match pair
                        // Here, lc is the match length - MIN_MATCH
                        code = ZipTree.LengthCode[lc];

                        // send the length code
                        send_code(code + ZipConstants.LITERALS + 1, ltree);
                        extra = ZipTree.ExtraLengthBits[code];
                        if (extra != 0)
                        {
                            // send the extra length bits
                            lc -= ZipTree.LengthBase[code];
                            send_bits(lc, extra);
                        }
                        distance--; // dist is now the match distance - 1
                        code = ZipTree.DistanceCode(distance);

                        // send the distance code
                        send_code(code, dtree);

                        extra = ZipTree.ExtraDistanceBits[code];
                        if (extra != 0)
                        {
                            // send the extra distance bits
                            distance -= ZipTree.DistanceBase[code];
                            send_bits(distance, extra);
                        }
                    }

                    // Check that the overlay between pending and d_buf+l_buf is ok:
                } while (lx < last_lit);
            }

            send_code(END_BLOCK, ltree);
            last_eob_len = ltree[END_BLOCK * 2 + 1];
        }

        internal void set_data_type()
        {
            int n = 0;
            int ascii_freq = 0;
            int bin_freq = 0;
            while (n < 7)
            {
                bin_freq += dyn_ltree[n * 2];
                n++;
            }
            while (n < 128)
            {
                ascii_freq += dyn_ltree[n * 2];
                n++;
            }
            while (n < ZipConstants.LITERALS)
            {
                bin_freq += dyn_ltree[n * 2];
                n++;
            }
            data_type = (SByte)(bin_freq > (ascii_freq >> 2) ? Z_BINARY : Z_ASCII);
        }

        internal void bi_flush()
        {
            if (bi_valid == 16)
            {
                pending[pendingCount++] = (Byte)bi_buf;
                pending[pendingCount++] = (Byte)(bi_buf >> 8);
                bi_buf = 0;
                bi_valid = 0;
            }
            else if (bi_valid >= 8)
            {
                //put_Byte((Byte)bi_buf);
                pending[pendingCount++] = (Byte)bi_buf;
                bi_buf >>= 8;
                bi_valid -= 8;
            }
        }

        internal void bi_windup()
        {
            if (bi_valid > 8)
            {
                pending[pendingCount++] = (Byte)bi_buf;
                pending[pendingCount++] = (Byte)(bi_buf >> 8);
            }
            else if (bi_valid > 0)
            {
                //put_Byte((Byte)bi_buf);
                pending[pendingCount++] = (Byte)bi_buf;
            }
            bi_buf = 0;
            bi_valid = 0;
        }

        internal void copy_block(int buf, int len, Boolean header)
        {
            bi_windup(); // align on Byte boundary
            last_eob_len = 8; // enough lookahead for inflate

            if (header)
            {
                unchecked
                {
                    //put_short((short)len);
                    pending[pendingCount++] = (Byte)len;
                    pending[pendingCount++] = (Byte)(len >> 8);
                    //put_short((short)~len);
                    pending[pendingCount++] = (Byte)~len;
                    pending[pendingCount++] = (Byte)(~len >> 8);
                }
            }

            put_Bytes(window, buf, len);
        }

        internal void flush_block_only(Boolean eof)
        {
            _tr_flush_block(block_start >= 0 ? block_start : -1, strstart - block_start, eof);
            block_start = strstart;
            _codec.flush_pending();
        }

        internal ZipBlockState DeflateNone(ZipFlushType flush)
        {
            // Stored blocks are limited to 0xffff Bytes, pending is limited
            // to pending_buf_size, and each stored block has a 5 Byte header:

            int max_block_size = 0xffff;
            int max_start;

            if (max_block_size > pending.Length - 5)
                max_block_size = pending.Length - 5;

            // Copy as much as possible from input to output:
            while (true)
            {
                // Fill the window as much as possible:
                if (lookahead <= 1)
                {
                    _fillWindow();
                    if (lookahead == 0 && flush == ZipFlushType.None)
                        return ZipBlockState.NeedMore;
                    if (lookahead == 0)
                        break; // flush the current block
                }

                strstart += lookahead;
                lookahead = 0;

                // Emit a stored block if pending will be full:
                max_start = block_start + max_block_size;
                if (strstart == 0 || strstart >= max_start)
                {
                    // strstart == 0 is possible when wraparound on 16-bit machine
                    lookahead = (strstart - max_start);
                    strstart = max_start;

                    flush_block_only(false);
                    if (_codec.AvailableBytesOut == 0)
                        return ZipBlockState.NeedMore;
                }

                // Flush if we may have to slide, otherwise block_start may become
                // negative and the data will be gone:
                if (strstart - block_start >= w_size - MIN_LOOKAHEAD)
                {
                    flush_block_only(false);
                    if (_codec.AvailableBytesOut == 0)
                        return ZipBlockState.NeedMore;
                }
            }

            flush_block_only(flush == ZipFlushType.Finish);
            if (_codec.AvailableBytesOut == 0)
                return (flush == ZipFlushType.Finish) ? ZipBlockState.FinishStarted : ZipBlockState.NeedMore;

            return flush == ZipFlushType.Finish ? ZipBlockState.FinishDone : ZipBlockState.BlockDone;
        }

        internal void _tr_stored_block(int buf, int stored_len, Boolean eof)
        {
            send_bits((STORED_BLOCK << 1) + (eof ? 1 : 0), 3); // send block type
            copy_block(buf, stored_len, true); // with header
        }

        internal void _tr_flush_block(int buf, int stored_len, Boolean eof)
        {
            int opt_lenb, static_lenb; // opt_len and static_len in Bytes
            int max_blindex = 0; // index of last bit length code of non zero freq

            // Build the Huffman trees unless a stored block is forced
            if (compressionLevel > 0)
            {
                // Check if the file is ascii or binary
                if (data_type == Z_UNKNOWN)
                    set_data_type();

                // Construct the literal and distance trees
                treeLiterals.build_tree(this);

                treeDistances.build_tree(this);

                // At this point, opt_len and static_len are the total bit lengths of
                // the compressed block data, excluding the tree representations.

                // Build the bit length tree for the above two trees, and get the index
                // in bl_order of the last bit length code to send.
                max_blindex = build_bl_tree();

                // Determine the best encoding. Compute first the block length in Bytes
                opt_lenb = (opt_len + 3 + 7) >> 3;
                static_lenb = (static_len + 3 + 7) >> 3;

                if (static_lenb <= opt_lenb)
                    opt_lenb = static_lenb;
            }
            else
                opt_lenb = static_lenb = stored_len + 5; // force a stored block

            if (stored_len + 4 <= opt_lenb && buf != -1)
            {
                // 4: two words for the lengths
                // The test buf != NULL is only necessary if LIT_BUFSIZE > WSIZE.
                // Otherwise we can't have processed more than WSIZE input Bytes since
                // the last block flush, because compression would have been
                // successful. If LIT_BUFSIZE <= WSIZE, it is never too late to
                // transform a block into a stored block.
                _tr_stored_block(buf, stored_len, eof);
            }
            else if (static_lenb == opt_lenb)
            {
                send_bits((STATIC_TREES << 1) + (eof ? 1 : 0), 3);
                send_compressed_block(ZipStaticTree.lengthAndLiteralsTreeCodes, ZipStaticTree.distTreeCodes);
            }
            else
            {
                send_bits((DYN_TREES << 1) + (eof ? 1 : 0), 3);
                send_all_trees(treeLiterals.max_code + 1, treeDistances.max_code + 1, max_blindex + 1);
                send_compressed_block(dyn_ltree, dyn_dtree);
            }

            // The above check is made mod 2^32, for files larger than 512 MB
            // and uLong implemented on 32 bits.

            _InitializeBlocks();

            if (eof)
                bi_windup();
        }

        private void _fillWindow()
        {
            int n, m;
            int p;
            int more; // Amount of free space at the end of the window.

            do
            {
                more = (window_size - lookahead - strstart);

                // Deal with !@#$% 64K limit:
                if (more == 0 && strstart == 0 && lookahead == 0)
                    more = w_size;
                else if (more == -1)
                {
                    // Very unlikely, but possible on 16 bit machine if strstart == 0
                    // and lookahead == 1 (input done one Byte at time)
                    more--;

                    // If the window is almost full and there is insufficient lookahead,
                    // move the upper half to the lower one to make room in the upper half.
                }
                else if (strstart >= w_size + w_size - MIN_LOOKAHEAD)
                {
                    Array.Copy(window, w_size, window, 0, w_size);
                    match_start -= w_size;
                    strstart -= w_size; // we now have strstart >= MAX_DIST
                    block_start -= w_size;

                    // Slide the hash table (could be avoided with 32 bit values
                    // at the expense of memory usage). We slide even when level == 0
                    // to keep the hash table consistent if we switch back to level > 0
                    // later. (Using level 0 permanently is not an optimal usage of
                    // zlib, so we don't care about this pathological case.)

                    n = hash_size;
                    p = n;
                    do
                    {
                        m = (head[--p] & 0xffff);
                        head[p] = (short)((m >= w_size) ? (m - w_size) : 0);
                    } while (--n != 0);

                    n = w_size;
                    p = n;
                    do
                    {
                        m = (prev[--p] & 0xffff);
                        prev[p] = (short)((m >= w_size) ? (m - w_size) : 0);
                        // If n is not on any hash chain, prev[n] is garbage but
                        // its value will never be used.
                    } while (--n != 0);
                    more += w_size;
                }

                if (_codec.AvailableBytesIn == 0)
                    return;

                // If there was no sliding:
                //    strstart <= WSIZE+MAX_DIST-1 && lookahead <= MIN_LOOKAHEAD - 1 &&
                //    more == window_size - lookahead - strstart
                // => more >= window_size - (MIN_LOOKAHEAD-1 + WSIZE + MAX_DIST-1)
                // => more >= window_size - 2*WSIZE + 2
                // In the BIG_MEM or MMAP case (not yet supported),
                //   window_size == input_size + MIN_LOOKAHEAD  &&
                //   strstart + s->lookahead <= input_size => more >= MIN_LOOKAHEAD.
                // Otherwise, window_size == 2*WSIZE so more >= 2.
                // If there was sliding, more >= WSIZE. So in all cases, more >= 2.

                n = _codec.read_buf(window, strstart + lookahead, more);
                lookahead += n;

                // Initialize the hash value now that we have some input:
                if (lookahead >= MIN_MATCH)
                {
                    ins_h = window[strstart] & 0xff;
                    ins_h = (((ins_h) << hash_shift) ^ (window[strstart + 1] & 0xff)) & hash_mask;
                }
                // If the whole input has less than MIN_MATCH Bytes, ins_h is garbage,
                // but this is not important since only literal Bytes will be emitted.
            } while (lookahead < MIN_LOOKAHEAD && _codec.AvailableBytesIn != 0);
        }

        internal ZipBlockState DeflateFast(ZipFlushType flush)
        {
            //    short hash_head = 0; // head of the hash chain
            int hash_head = 0; // head of the hash chain
            Boolean bflush; // set if current block must be flushed

            while (true)
            {
                // Make sure that we always have enough lookahead, except
                // at the end of the input file. We need MAX_MATCH Bytes
                // for the next match, plus MIN_MATCH Bytes to insert the
                // String following the next match.
                if (lookahead < MIN_LOOKAHEAD)
                {
                    _fillWindow();
                    if (lookahead < MIN_LOOKAHEAD && flush == ZipFlushType.None)
                        return ZipBlockState.NeedMore;
                    if (lookahead == 0)
                        break; // flush the current block
                }

                // Insert the String window[strstart .. strstart+2] in the
                // dictionary, and set hash_head to the head of the hash chain:
                if (lookahead >= MIN_MATCH)
                {
                    ins_h = (((ins_h) << hash_shift) ^ (window[(strstart) + (MIN_MATCH - 1)] & 0xff)) & hash_mask;

                    //  prev[strstart&w_mask]=hash_head=head[ins_h];
                    hash_head = (head[ins_h] & 0xffff);
                    prev[strstart & w_mask] = head[ins_h];
                    head[ins_h] = unchecked((short)strstart);
                }

                // Find the longest match, discarding those <= prev_length.
                // At this point we have always match_length < MIN_MATCH

                if (hash_head != 0L && ((strstart - hash_head) & 0xffff) <= w_size - MIN_LOOKAHEAD)
                {
                    // To simplify the code, we prevent matches with the String
                    // of window index 0 (in particular we have to avoid a match
                    // of the String with itself at the start of the input file).
                    if (compressionStrategy != ZipCompressionStrategy.HuffmanOnly)
                        match_length = longest_match(hash_head);
                    // longest_match() sets match_start
                }
                if (match_length >= MIN_MATCH)
                {
                    //        check_match(strstart, match_start, match_length);

                    bflush = _tr_tally(strstart - match_start, match_length - MIN_MATCH);

                    lookahead -= match_length;

                    // Insert new Strings in the hash table only if the match length
                    // is not too large. This saves time but degrades compression.
                    if (match_length <= config.MaxLazy && lookahead >= MIN_MATCH)
                    {
                        match_length--; // String at strstart already in hash table
                        do
                        {
                            strstart++;

                            ins_h = ((ins_h << hash_shift) ^ (window[(strstart) + (MIN_MATCH - 1)] & 0xff)) & hash_mask;
                            //      prev[strstart&w_mask]=hash_head=head[ins_h];
                            hash_head = (head[ins_h] & 0xffff);
                            prev[strstart & w_mask] = head[ins_h];
                            head[ins_h] = unchecked((short)strstart);

                            // strstart never exceeds WSIZE-MAX_MATCH, so there are
                            // always MIN_MATCH Bytes ahead.
                        } while (--match_length != 0);
                        strstart++;
                    }
                    else
                    {
                        strstart += match_length;
                        match_length = 0;
                        ins_h = window[strstart] & 0xff;

                        ins_h = (((ins_h) << hash_shift) ^ (window[strstart + 1] & 0xff)) & hash_mask;
                        // If lookahead < MIN_MATCH, ins_h is garbage, but it does not
                        // matter since it will be recomputed at next deflate call.
                    }
                }
                else
                {
                    // No match, output a literal Byte

                    bflush = _tr_tally(0, window[strstart] & 0xff);
                    lookahead--;
                    strstart++;
                }
                if (bflush)
                {
                    flush_block_only(false);
                    if (_codec.AvailableBytesOut == 0)
                        return ZipBlockState.NeedMore;
                }
            }

            flush_block_only(flush == ZipFlushType.Finish);
            if (_codec.AvailableBytesOut == 0)
            {
                if (flush == ZipFlushType.Finish)
                    return ZipBlockState.FinishStarted;
                else
                    return ZipBlockState.NeedMore;
            }
            return flush == ZipFlushType.Finish ? ZipBlockState.FinishDone : ZipBlockState.BlockDone;
        }

        internal ZipBlockState DeflateSlow(ZipFlushType flush)
        {
            //    short hash_head = 0;    // head of hash chain
            int hash_head = 0; // head of hash chain
            Boolean bflush; // set if current block must be flushed

            // Process the input block.
            while (true)
            {
                // Make sure that we always have enough lookahead, except
                // at the end of the input file. We need MAX_MATCH Bytes
                // for the next match, plus MIN_MATCH Bytes to insert the
                // String following the next match.

                if (lookahead < MIN_LOOKAHEAD)
                {
                    _fillWindow();
                    if (lookahead < MIN_LOOKAHEAD && flush == ZipFlushType.None)
                        return ZipBlockState.NeedMore;

                    if (lookahead == 0)
                        break; // flush the current block
                }

                // Insert the String window[strstart .. strstart+2] in the
                // dictionary, and set hash_head to the head of the hash chain:

                if (lookahead >= MIN_MATCH)
                {
                    ins_h = (((ins_h) << hash_shift) ^ (window[(strstart) + (MIN_MATCH - 1)] & 0xff)) & hash_mask;
                    //  prev[strstart&w_mask]=hash_head=head[ins_h];
                    hash_head = (head[ins_h] & 0xffff);
                    prev[strstart & w_mask] = head[ins_h];
                    head[ins_h] = unchecked((short)strstart);
                }

                // Find the longest match, discarding those <= prev_length.
                prev_length = match_length;
                prev_match = match_start;
                match_length = MIN_MATCH - 1;

                if (hash_head != 0 && prev_length < config.MaxLazy &&
                    ((strstart - hash_head) & 0xffff) <= w_size - MIN_LOOKAHEAD)
                {
                    // To simplify the code, we prevent matches with the String
                    // of window index 0 (in particular we have to avoid a match
                    // of the String with itself at the start of the input file).

                    if (compressionStrategy != ZipCompressionStrategy.HuffmanOnly)
                        match_length = longest_match(hash_head);
                    // longest_match() sets match_start

                    if (match_length <= 5 && (compressionStrategy == ZipCompressionStrategy.Filtered ||
                                              (match_length == MIN_MATCH && strstart - match_start > 4096)))
                    {
                        // If prev_match is also MIN_MATCH, match_start is garbage
                        // but we will ignore the current match anyway.
                        match_length = MIN_MATCH - 1;
                    }
                }

                // If there was a match at the previous step and the current
                // match is not better, output the previous match:
                if (prev_length >= MIN_MATCH && match_length <= prev_length)
                {
                    int max_insert = strstart + lookahead - MIN_MATCH;
                    // Do not insert Strings in hash table beyond this.

                    //          check_match(strstart-1, prev_match, prev_length);

                    bflush = _tr_tally(strstart - 1 - prev_match, prev_length - MIN_MATCH);

                    // Insert in hash table all Strings up to the end of the match.
                    // strstart-1 and strstart are already inserted. If there is not
                    // enough lookahead, the last two Strings are not inserted in
                    // the hash table.
                    lookahead -= (prev_length - 1);
                    prev_length -= 2;
                    do
                    {
                        if (++strstart <= max_insert)
                        {
                            ins_h = (((ins_h) << hash_shift) ^ (window[(strstart) + (MIN_MATCH - 1)] & 0xff)) & hash_mask;
                            //prev[strstart&w_mask]=hash_head=head[ins_h];
                            hash_head = (head[ins_h] & 0xffff);
                            prev[strstart & w_mask] = head[ins_h];
                            head[ins_h] = unchecked((short)strstart);
                        }
                    } while (--prev_length != 0);
                    match_available = 0;
                    match_length = MIN_MATCH - 1;
                    strstart++;

                    if (bflush)
                    {
                        flush_block_only(false);
                        if (_codec.AvailableBytesOut == 0)
                            return ZipBlockState.NeedMore;
                    }
                }
                else if (match_available != 0)
                {
                    // If there was no match at the previous position, output a
                    // single literal. If there was a match but the current match
                    // is longer, truncate the previous match to a single literal.

                    bflush = _tr_tally(0, window[strstart - 1] & 0xff);

                    if (bflush)
                        flush_block_only(false);
                    strstart++;
                    lookahead--;
                    if (_codec.AvailableBytesOut == 0)
                        return ZipBlockState.NeedMore;
                }
                else
                {
                    // There is no previous match to compare with, wait for
                    // the next step to decide.

                    match_available = 1;
                    strstart++;
                    lookahead--;
                }
            }

            if (match_available != 0)
            {
                bflush = _tr_tally(0, window[strstart - 1] & 0xff);
                match_available = 0;
            }
            flush_block_only(flush == ZipFlushType.Finish);

            if (_codec.AvailableBytesOut == 0)
            {
                if (flush == ZipFlushType.Finish)
                    return ZipBlockState.FinishStarted;
                else
                    return ZipBlockState.NeedMore;
            }

            return flush == ZipFlushType.Finish ? ZipBlockState.FinishDone : ZipBlockState.BlockDone;
        }

        internal int longest_match(int cur_match)
        {
            int chain_length = config.MaxChainLength; // max hash chain length
            int scan = strstart; // current String
            int match; // matched String
            int len; // length of current match
            int best_len = prev_length; // best match length so far
            int limit = strstart > (w_size - MIN_LOOKAHEAD) ? strstart - (w_size - MIN_LOOKAHEAD) : 0;

            int niceLength = config.NiceLength;

            // Stop when cur_match becomes <= limit. To simplify the code,
            // we prevent matches with the String of window index 0.

            int wmask = w_mask;

            int strend = strstart + MAX_MATCH;
            Byte scan_end1 = window[scan + best_len - 1];
            Byte scan_end = window[scan + best_len];

            // The code is optimized for HASH_BITS >= 8 and MAX_MATCH-2 multiple of 16.
            // It is easy to get rid of this optimization if necessary.

            // Do not waste too much time if we already have a good match:
            if (prev_length >= config.GoodLength)
                chain_length >>= 2;

            // Do not look for matches beyond the end of the input. This is necessary
            // to make deflate deterministic.
            if (niceLength > lookahead)
                niceLength = lookahead;

            do
            {
                match = cur_match;

                // Skip to next match if the match length cannot increase
                // or if the match length is less than 2:
                if (window[match + best_len] != scan_end ||
                    window[match + best_len - 1] != scan_end1 ||
                    window[match] != window[scan] ||
                    window[++match] != window[scan + 1])
                    continue;

                // The check at best_len-1 can be removed because it will be made
                // again later. (This heuristic is not always a win.)
                // It is not necessary to compare scan[2] and match[2] since they
                // are always equal when the other Bytes match, given that
                // the hash keys are equal and that HASH_BITS >= 8.
                scan += 2;
                match++;

                // We check for insufficient lookahead only every 8th comparison;
                // the 256th check will be made at strstart+258.
                do
                {
                } while (window[++scan] == window[++match] &&
                         window[++scan] == window[++match] &&
                         window[++scan] == window[++match] &&
                         window[++scan] == window[++match] &&
                         window[++scan] == window[++match] &&
                         window[++scan] == window[++match] &&
                         window[++scan] == window[++match] &&
                         window[++scan] == window[++match] && scan < strend);

                len = MAX_MATCH - (strend - scan);
                scan = strend - MAX_MATCH;

                if (len > best_len)
                {
                    match_start = cur_match;
                    best_len = len;
                    if (len >= niceLength)
                        break;
                    scan_end1 = window[scan + best_len - 1];
                    scan_end = window[scan + best_len];
                }
            } while ((cur_match = (prev[cur_match & wmask] & 0xffff)) > limit && --chain_length != 0);

            if (best_len <= lookahead)
                return best_len;
            return lookahead;
        }

        internal int Initialize(ZlibCodec codec, ZipCompressionLevel level)
        {
            return Initialize(codec, level, ZipConstants.WindowBitsMax);
        }

        internal int Initialize(ZlibCodec codec, ZipCompressionLevel level, int bits)
        {
            return Initialize(codec, level, bits, MEM_LEVEL_DEFAULT, ZipCompressionStrategy.Default);
        }

        internal int Initialize(ZlibCodec codec, ZipCompressionLevel level, int bits, ZipCompressionStrategy compressionStrategy)
        {
            return Initialize(codec, level, bits, MEM_LEVEL_DEFAULT, compressionStrategy);
        }

        internal int Initialize(ZlibCodec codec, ZipCompressionLevel level, int windowBits, int memLevel, ZipCompressionStrategy strategy)
        {
            _codec = codec;
            _codec.Message = null;

            // validation
            if (windowBits < 9 || windowBits > 15)
                throw new ZlibException("windowBits must be in the range 9..15.");

            if (memLevel < 1 || memLevel > MEM_LEVEL_MAX)
                throw new ZlibException(String.Format("memLevel must be in the range 1.. {0}", MEM_LEVEL_MAX));

            _codec.dstate = this;

            w_bits = windowBits;
            w_size = 1 << w_bits;
            w_mask = w_size - 1;

            hash_bits = memLevel + 7;
            hash_size = 1 << hash_bits;
            hash_mask = hash_size - 1;
            hash_shift = ((hash_bits + MIN_MATCH - 1) / MIN_MATCH);

            window = new Byte[w_size * 2];
            prev = new short[w_size];
            head = new short[hash_size];

            // for memLevel==8, this will be 16384, 16k
            lit_bufsize = 1 << (memLevel + 6);

            // Use a single array as the buffer for data pending compression,
            // the output distance codes, and the output length codes (aka tree).
            // orig comment: This works just fine since the average
            // output size for (length,distance) codes is <= 24 bits.
            pending = new Byte[lit_bufsize * 4];
            _distanceOffset = lit_bufsize;
            _lengthOffset = (1 + 2) * lit_bufsize;

            // So, for memLevel 8, the length of the pending buffer is 65536. 64k.
            // The first 16k are pending Bytes.
            // The middle slice, of 32k, is used for distance codes.
            // The final 16k are length codes.

            compressionLevel = level;
            compressionStrategy = strategy;

            Reset();
            return ZipConstants.Z_OK;
        }

        internal void Reset()
        {
            _codec.TotalBytesIn = _codec.TotalBytesOut = 0;
            _codec.Message = null;
            //strm.data_type = Z_UNKNOWN;

            pendingCount = 0;
            nextPending = 0;

            Rfc1950BytesEmitted = false;

            status = (WantRfc1950HeaderBytes) ? INIT_STATE : BUSY_STATE;
            _codec._Adler32 = ZipAdler.Adler32(0, null, 0, 0);

            last_flush = (int)ZipFlushType.None;

            _InitializeTreeData();
            _InitializeLazyMatch();
        }

        internal int End()
        {
            if (status != INIT_STATE && status != BUSY_STATE && status != FINISH_STATE)
                return ZipConstants.Z_STREAM_ERROR;
            // Deallocate in reverse order of allocations:
            pending = null;
            head = null;
            prev = null;
            window = null;
            // free
            // dstate=null;
            return status == BUSY_STATE ? ZipConstants.Z_DATA_ERROR : ZipConstants.Z_OK;
        }

        private void SetDeflater()
        {
            switch (config.Flavor)
            {
                case ZipDeflateFlavor.Store:
                    _deflateFunction = DeflateNone;
                    break;
                case ZipDeflateFlavor.Fast:
                    _deflateFunction = DeflateFast;
                    break;
                case ZipDeflateFlavor.Slow:
                    _deflateFunction = DeflateSlow;
                    break;
            }
        }

        internal int SetParams(ZipCompressionLevel level, ZipCompressionStrategy strategy)
        {
            int result = ZipConstants.Z_OK;

            if (compressionLevel != level)
            {
                ZipConfig newConfig = ZipConfig.Lookup(level);

                // change in the deflate flavor (Fast vs slow vs none)?
                if (newConfig.Flavor != config.Flavor && _codec.TotalBytesIn != 0)
                {
                    // Flush the last buffer:
                    result = _codec.Deflate(ZipFlushType.Partial);
                }

                compressionLevel = level;
                config = newConfig;
                SetDeflater();
            }

            // no need to flush with change in strategy?  Really?
            compressionStrategy = strategy;

            return result;
        }

        internal int SetDictionary(Byte[] dictionary)
        {
            int length = dictionary.Length;
            int index = 0;

            if (dictionary == null || status != INIT_STATE)
                throw new ZlibException("Stream error.");

            _codec._Adler32 = ZipAdler.Adler32(_codec._Adler32, dictionary, 0, dictionary.Length);

            if (length < MIN_MATCH)
                return ZipConstants.Z_OK;
            if (length > w_size - MIN_LOOKAHEAD)
            {
                length = w_size - MIN_LOOKAHEAD;
                index = dictionary.Length - length; // use the tail of the dictionary
            }
            Array.Copy(dictionary, index, window, 0, length);
            strstart = length;
            block_start = length;

            // Insert all Strings in the hash table (except for the last two Bytes).
            // s->lookahead stays null, so s->ins_h will be recomputed at the next
            // call of fill_window.

            ins_h = window[0] & 0xff;
            ins_h = (((ins_h) << hash_shift) ^ (window[1] & 0xff)) & hash_mask;

            for (int n = 0; n <= length - MIN_MATCH; n++)
            {
                ins_h = (((ins_h) << hash_shift) ^ (window[(n) + (MIN_MATCH - 1)] & 0xff)) & hash_mask;
                prev[n & w_mask] = head[ins_h];
                head[ins_h] = (short)n;
            }
            return ZipConstants.Z_OK;
        }

        internal int Deflate(ZipFlushType flush)
        {
            int old_flush;

            if (_codec.OutputBuffer == null ||
                (_codec.InputBuffer == null && _codec.AvailableBytesIn != 0) ||
                (status == FINISH_STATE && flush != ZipFlushType.Finish))
            {
                _codec.Message = _errorMessage[ZipConstants.Z_NEED_DICT - (ZipConstants.Z_STREAM_ERROR)];
                throw new ZlibException(String.Format("Something is fishy. [{0}]", _codec.Message));
            }
            if (_codec.AvailableBytesOut == 0)
            {
                _codec.Message = _errorMessage[ZipConstants.Z_NEED_DICT - (ZipConstants.Z_BUF_ERROR)];
                throw new ZlibException("OutputBuffer is full (AvailableBytesOut == 0)");
            }

            old_flush = last_flush;
            last_flush = (int)flush;

            // Write the zlib (rfc1950) header Bytes
            if (status == INIT_STATE)
            {
                int header = (Z_DEFLATED + ((w_bits - 8) << 4)) << 8;
                int level_flags = (((int)compressionLevel - 1) & 0xff) >> 1;

                if (level_flags > 3)
                    level_flags = 3;
                header |= (level_flags << 6);
                if (strstart != 0)
                    header |= PRESET_DICT;
                header += 31 - (header % 31);

                status = BUSY_STATE;
                //putShortMSB(header);
                unchecked
                {
                    pending[pendingCount++] = (Byte)(header >> 8);
                    pending[pendingCount++] = (Byte)header;
                }
                // Save the adler32 of the preset dictionary:
                if (strstart != 0)
                {
                    pending[pendingCount++] = (Byte)((_codec._Adler32 & 0xFF000000) >> 24);
                    pending[pendingCount++] = (Byte)((_codec._Adler32 & 0x00FF0000) >> 16);
                    pending[pendingCount++] = (Byte)((_codec._Adler32 & 0x0000FF00) >> 8);
                    pending[pendingCount++] = (Byte)(_codec._Adler32 & 0x000000FF);
                }
                _codec._Adler32 = ZipAdler.Adler32(0, null, 0, 0);
            }

            // Flush as much pending output as possible
            if (pendingCount != 0)
            {
                _codec.flush_pending();
                if (_codec.AvailableBytesOut == 0)
                {
                    //System.out.println("  avail_out==0");
                    // Since avail_out is 0, deflate will be called again with
                    // more output space, but possibly with both pending and
                    // avail_in equal to zero. There won't be anything to do,
                    // but this is not an error situation so make sure we
                    // return OK instead of BUF_ERROR at next call of deflate:
                    last_flush = -1;
                    return ZipConstants.Z_OK;
                }

                // Make sure there is something to do and avoid duplicate consecutive
                // flushes. For repeated and useless calls with Z_FINISH, we keep
                // returning Z_STREAM_END instead of Z_BUFF_ERROR.
            }
            else if (_codec.AvailableBytesIn == 0 &&
                     (int)flush <= old_flush &&
                     flush != ZipFlushType.Finish)
            {
                // workitem 8557
                //
                // Not sure why this needs to be an error.  pendingCount == 0, which
                // means there's nothing to deflate.  And the caller has not asked
                // for a FlushType.Finish, but...  that seems very non-fatal.  We
                // can just say "OK" and do nothing.

                // _codec.Message = z_errmsg[ZipConstants.Z_NEED_DICT - (ZipConstants.Z_BUF_ERROR)];
                // throw new ZlibException("AvailableBytesIn == 0 && flush<=old_flush && flush != FlushType.Finish");

                return ZipConstants.Z_OK;
            }

            // User must not provide more input after the first FINISH:
            if (status == FINISH_STATE && _codec.AvailableBytesIn != 0)
            {
                _codec.Message = _errorMessage[ZipConstants.Z_NEED_DICT - (ZipConstants.Z_BUF_ERROR)];
                throw new ZlibException("status == FINISH_STATE && _codec.AvailableBytesIn != 0");
            }

            // Start a new block or continue the current one.
            if (_codec.AvailableBytesIn != 0 || lookahead != 0 || (flush != ZipFlushType.None && status != FINISH_STATE))
            {
                ZipBlockState bstate = _deflateFunction(flush);

                if (bstate == ZipBlockState.FinishStarted || bstate == ZipBlockState.FinishDone)
                    status = FINISH_STATE;
                if (bstate == ZipBlockState.NeedMore || bstate == ZipBlockState.FinishStarted)
                {
                    if (_codec.AvailableBytesOut == 0)
                        last_flush = -1; // avoid BUF_ERROR next call, see above
                    return ZipConstants.Z_OK;
                    // If flush != Z_NO_FLUSH && avail_out == 0, the next call
                    // of deflate should use the same flush parameter to make sure
                    // that the flush is complete. So we don't have to output an
                    // empty block here, this will be done at next call. This also
                    // ensures that for a very small output buffer, we emit at most
                    // one empty block.
                }

                if (bstate == ZipBlockState.BlockDone)
                {
                    if (flush == ZipFlushType.Partial)
                        _tr_align();
                    else
                    {
                        // FlushType.Full or FlushType.Sync
                        _tr_stored_block(0, 0, false);
                        // For a full flush, this empty block will be recognized
                        // as a special marker by inflate_sync().
                        if (flush == ZipFlushType.Full)
                        {
                            // clear hash (forget the history)
                            for (int i = 0; i < hash_size; i++)
                                head[i] = 0;
                        }
                    }
                    _codec.flush_pending();
                    if (_codec.AvailableBytesOut == 0)
                    {
                        last_flush = -1; // avoid BUF_ERROR at next call, see above
                        return ZipConstants.Z_OK;
                    }
                }
            }

            if (flush != ZipFlushType.Finish)
                return ZipConstants.Z_OK;

            if (!WantRfc1950HeaderBytes || Rfc1950BytesEmitted)
                return ZipConstants.Z_STREAM_END;

            // Write the zlib trailer (adler32)
            pending[pendingCount++] = (Byte)((_codec._Adler32 & 0xFF000000) >> 24);
            pending[pendingCount++] = (Byte)((_codec._Adler32 & 0x00FF0000) >> 16);
            pending[pendingCount++] = (Byte)((_codec._Adler32 & 0x0000FF00) >> 8);
            pending[pendingCount++] = (Byte)(_codec._Adler32 & 0x000000FF);
            //putShortMSB((int)(SharedUtils.URShift(_codec._Adler32, 16)));
            //putShortMSB((int)(_codec._Adler32 & 0xffff));

            _codec.flush_pending();

            // If avail_out is zero, the application will call deflate again
            // to flush the rest.

            Rfc1950BytesEmitted = true; // write the trailer only once!

            return pendingCount != 0 ? ZipConstants.Z_OK : ZipConstants.Z_STREAM_END;
        }

        #endregion

        #region Класс ZipConfig

        internal class ZipConfig
        {
            #region Поля

            private static readonly ZipConfig[] Table;

            internal ZipDeflateFlavor Flavor;

            internal int GoodLength;

            internal int MaxChainLength;

            internal int MaxLazy;

            internal int NiceLength;

            #endregion

            #region Конструктор

            static ZipConfig()
            {
                Table = new[]
                    {
                        new ZipConfig(0, 0, 0, 0, ZipDeflateFlavor.Store),
                        new ZipConfig(4, 4, 8, 4, ZipDeflateFlavor.Fast),
                        new ZipConfig(4, 5, 16, 8, ZipDeflateFlavor.Fast),
                        new ZipConfig(4, 6, 32, 32, ZipDeflateFlavor.Fast),
                        new ZipConfig(4, 4, 16, 16, ZipDeflateFlavor.Slow),
                        new ZipConfig(8, 16, 32, 32, ZipDeflateFlavor.Slow),
                        new ZipConfig(8, 16, 128, 128, ZipDeflateFlavor.Slow),
                        new ZipConfig(8, 32, 128, 256, ZipDeflateFlavor.Slow),
                        new ZipConfig(32, 128, 258, 1024, ZipDeflateFlavor.Slow),
                        new ZipConfig(32, 258, 258, 4096, ZipDeflateFlavor.Slow),
                    };
            }

            private ZipConfig(int goodLength, int maxLazy, int niceLength, int maxChainLength, ZipDeflateFlavor flavor)
            {
                GoodLength = goodLength;
                MaxLazy = maxLazy;
                NiceLength = niceLength;
                MaxChainLength = maxChainLength;
                Flavor = flavor;
            }

            #endregion

            #region Методы

            public static ZipConfig Lookup(ZipCompressionLevel level)
            {
                return Table[(int)level];
            }

            #endregion
        }

        #endregion Класс ZipConfig
    }

    #endregion Класс ZipDeflateManager

    #region Класс ZipInflateManager

    internal sealed class ZipInflateManager
    {
        #region Перечисления

        private enum InflateManagerMode
        {
            METHOD = 0, // waiting for method Byte
            FLAG = 1, // waiting for flag Byte
            DICT4 = 2, // four dictionary check Bytes to go
            DICT3 = 3, // three dictionary check Bytes to go
            DICT2 = 4, // two dictionary check Bytes to go
            DICT1 = 5, // one dictionary check Byte to go
            DICT0 = 6, // waiting for inflateSetDictionary
            BLOCKS = 7, // decompressing blocks
            CHECK4 = 8, // four check Bytes to go
            CHECK3 = 9, // three check Bytes to go
            CHECK2 = 10, // two check Bytes to go
            CHECK1 = 11, // one check Byte to go
            DONE = 12, // finished check, done
            BAD = 13, // got an error--stay here
        }

        #endregion

        #region Константы

        private const int PRESET_DICT = 0x20;

        private const int Z_DEFLATED = 8;

        #endregion

        #region Поля

        private static readonly Byte[] mark = new Byte[]
            {
                0, 0, 0xff, 0xff
            };

        internal ZlibCodec _codec; // pointer back to this zlib stream

        private Boolean _handleRfc1950HeaderBytes = true;

        internal ZipInflateBlocks blocks; // current inflate_blocks state

        internal uint computedCheck; // computed check value

        internal uint expectedCheck; // stream check value

        internal int marker;

        internal int method; // if FLAGS, method Byte

        private InflateManagerMode mode; // current inflate mode

        internal int wbits; // log2(window size)  (8..15, defaults to 15)

        #endregion

        #region Свойства

        internal Boolean HandleRfc1950HeaderBytes
        {
            get { return _handleRfc1950HeaderBytes; }
            set { _handleRfc1950HeaderBytes = value; }
        }

        #endregion

        #region Конструктор

        public ZipInflateManager()
        {
        }

        public ZipInflateManager(Boolean expectRfc1950HeaderBytes)
        {
            _handleRfc1950HeaderBytes = expectRfc1950HeaderBytes;
        }

        #endregion

        #region Методы

        internal int Reset()
        {
            _codec.TotalBytesIn = _codec.TotalBytesOut = 0;
            _codec.Message = null;
            mode = HandleRfc1950HeaderBytes ? InflateManagerMode.METHOD : InflateManagerMode.BLOCKS;
            blocks.Reset();
            return ZipConstants.Z_OK;
        }

        internal int End()
        {
            if (blocks != null)
                blocks.Free();
            blocks = null;
            return ZipConstants.Z_OK;
        }

        internal int Initialize(ZlibCodec codec, int w)
        {
            _codec = codec;
            _codec.Message = null;
            blocks = null;

            // handle undocumented nowrap option (no zlib header or check)
            //nowrap = 0;
            //if (w < 0)
            //{
            //    w = - w;
            //    nowrap = 1;
            //}

            // set window size
            if (w < 8 || w > 15)
            {
                End();
                throw new ZlibException("Bad window size.");

                //return ZipConstants.Z_STREAM_ERROR;
            }
            wbits = w;

            blocks = new ZipInflateBlocks(codec,
                                          HandleRfc1950HeaderBytes ? this : null,
                                          1 << w);

            // reset state
            Reset();
            return ZipConstants.Z_OK;
        }

        internal int Inflate(ZipFlushType flush)
        {
            int b;

            if (_codec.InputBuffer == null)
                throw new ZlibException("InputBuffer is null. ");

            //             int f = (flush == FlushType.Finish)
            //                 ? ZipConstants.Z_BUF_ERROR
            //                 : ZipConstants.Z_OK;

            // workitem 8870
            int f = ZipConstants.Z_OK;
            int r = ZipConstants.Z_BUF_ERROR;

            while (true)
            {
                switch (mode)
                {
                    case InflateManagerMode.METHOD:
                        if (_codec.AvailableBytesIn == 0) return r;
                        r = f;
                        _codec.AvailableBytesIn--;
                        _codec.TotalBytesIn++;
                        if (((method = _codec.InputBuffer[_codec.NextIn++]) & 0xf) != Z_DEFLATED)
                        {
                            mode = InflateManagerMode.BAD;
                            _codec.Message = String.Format("unknown compression method (0x{0:X2})", method);
                            marker = 5; // can't try inflateSync
                            break;
                        }
                        if ((method >> 4) + 8 > wbits)
                        {
                            mode = InflateManagerMode.BAD;
                            _codec.Message = String.Format("invalid window size ({0})", (method >> 4) + 8);
                            marker = 5; // can't try inflateSync
                            break;
                        }
                        mode = InflateManagerMode.FLAG;
                        break;

                    case InflateManagerMode.FLAG:
                        if (_codec.AvailableBytesIn == 0) return r;
                        r = f;
                        _codec.AvailableBytesIn--;
                        _codec.TotalBytesIn++;
                        b = (_codec.InputBuffer[_codec.NextIn++]) & 0xff;

                        if ((((method << 8) + b) % 31) != 0)
                        {
                            mode = InflateManagerMode.BAD;
                            _codec.Message = "incorrect header check";
                            marker = 5; // can't try inflateSync
                            break;
                        }

                        mode = ((b & PRESET_DICT) == 0)
                                   ? InflateManagerMode.BLOCKS
                                   : InflateManagerMode.DICT4;
                        break;

                    case InflateManagerMode.DICT4:
                        if (_codec.AvailableBytesIn == 0) return r;
                        r = f;
                        _codec.AvailableBytesIn--;
                        _codec.TotalBytesIn++;
                        expectedCheck = (uint)((_codec.InputBuffer[_codec.NextIn++] << 24) & 0xff000000);
                        mode = InflateManagerMode.DICT3;
                        break;

                    case InflateManagerMode.DICT3:
                        if (_codec.AvailableBytesIn == 0) return r;
                        r = f;
                        _codec.AvailableBytesIn--;
                        _codec.TotalBytesIn++;
                        expectedCheck += (uint)((_codec.InputBuffer[_codec.NextIn++] << 16) & 0x00ff0000);
                        mode = InflateManagerMode.DICT2;
                        break;

                    case InflateManagerMode.DICT2:

                        if (_codec.AvailableBytesIn == 0) return r;
                        r = f;
                        _codec.AvailableBytesIn--;
                        _codec.TotalBytesIn++;
                        expectedCheck += (uint)((_codec.InputBuffer[_codec.NextIn++] << 8) & 0x0000ff00);
                        mode = InflateManagerMode.DICT1;
                        break;

                    case InflateManagerMode.DICT1:
                        if (_codec.AvailableBytesIn == 0) return r;
                        r = f;
                        _codec.AvailableBytesIn--;
                        _codec.TotalBytesIn++;
                        expectedCheck += (uint)(_codec.InputBuffer[_codec.NextIn++] & 0x000000ff);
                        _codec._Adler32 = expectedCheck;
                        mode = InflateManagerMode.DICT0;
                        return ZipConstants.Z_NEED_DICT;

                    case InflateManagerMode.DICT0:
                        mode = InflateManagerMode.BAD;
                        _codec.Message = "need dictionary";
                        marker = 0; // can try inflateSync
                        return ZipConstants.Z_STREAM_ERROR;

                    case InflateManagerMode.BLOCKS:
                        r = blocks.Process(r);
                        if (r == ZipConstants.Z_DATA_ERROR)
                        {
                            mode = InflateManagerMode.BAD;
                            marker = 0; // can try inflateSync
                            break;
                        }

                        if (r == ZipConstants.Z_OK) r = f;

                        if (r != ZipConstants.Z_STREAM_END)
                            return r;

                        r = f;
                        computedCheck = blocks.Reset();
                        if (!HandleRfc1950HeaderBytes)
                        {
                            mode = InflateManagerMode.DONE;
                            return ZipConstants.Z_STREAM_END;
                        }
                        mode = InflateManagerMode.CHECK4;
                        break;

                    case InflateManagerMode.CHECK4:
                        if (_codec.AvailableBytesIn == 0) return r;
                        r = f;
                        _codec.AvailableBytesIn--;
                        _codec.TotalBytesIn++;
                        expectedCheck = (uint)((_codec.InputBuffer[_codec.NextIn++] << 24) & 0xff000000);
                        mode = InflateManagerMode.CHECK3;
                        break;

                    case InflateManagerMode.CHECK3:
                        if (_codec.AvailableBytesIn == 0) return r;
                        r = f;
                        _codec.AvailableBytesIn--;
                        _codec.TotalBytesIn++;
                        expectedCheck += (uint)((_codec.InputBuffer[_codec.NextIn++] << 16) & 0x00ff0000);
                        mode = InflateManagerMode.CHECK2;
                        break;

                    case InflateManagerMode.CHECK2:
                        if (_codec.AvailableBytesIn == 0) return r;
                        r = f;
                        _codec.AvailableBytesIn--;
                        _codec.TotalBytesIn++;
                        expectedCheck += (uint)((_codec.InputBuffer[_codec.NextIn++] << 8) & 0x0000ff00);
                        mode = InflateManagerMode.CHECK1;
                        break;

                    case InflateManagerMode.CHECK1:
                        if (_codec.AvailableBytesIn == 0) return r;
                        r = f;
                        _codec.AvailableBytesIn--;
                        _codec.TotalBytesIn++;
                        expectedCheck += (uint)(_codec.InputBuffer[_codec.NextIn++] & 0x000000ff);
                        if (computedCheck != expectedCheck)
                        {
                            mode = InflateManagerMode.BAD;
                            _codec.Message = "incorrect data check";
                            marker = 5; // can't try inflateSync
                            break;
                        }
                        mode = InflateManagerMode.DONE;
                        return ZipConstants.Z_STREAM_END;

                    case InflateManagerMode.DONE:
                        return ZipConstants.Z_STREAM_END;

                    case InflateManagerMode.BAD:
                        throw new ZlibException(String.Format("Bad state ({0})", _codec.Message));

                    default:
                        throw new ZlibException("Stream error.");
                }
            }
        }

        internal int SetDictionary(Byte[] dictionary)
        {
            int index = 0;
            int length = dictionary.Length;
            if (mode != InflateManagerMode.DICT0)
                throw new ZlibException("Stream error.");

            if (ZipAdler.Adler32(1, dictionary, 0, dictionary.Length) != _codec._Adler32)
                return ZipConstants.Z_DATA_ERROR;

            _codec._Adler32 = ZipAdler.Adler32(0, null, 0, 0);

            if (length >= (1 << wbits))
            {
                length = (1 << wbits) - 1;
                index = dictionary.Length - length;
            }
            blocks.SetDictionary(dictionary, index, length);
            mode = InflateManagerMode.BLOCKS;
            return ZipConstants.Z_OK;
        }

        internal int Sync()
        {
            int n; // number of Bytes to look at
            int p; // pointer to Bytes
            int m; // number of marker Bytes found in a row
            long r, w; // temporaries to save total_in and total_out

            // set up
            if (mode != InflateManagerMode.BAD)
            {
                mode = InflateManagerMode.BAD;
                marker = 0;
            }
            if ((n = _codec.AvailableBytesIn) == 0)
                return ZipConstants.Z_BUF_ERROR;
            p = _codec.NextIn;
            m = marker;

            // search
            while (n != 0 && m < 4)
            {
                if (_codec.InputBuffer[p] == mark[m])
                    m++;
                else if (_codec.InputBuffer[p] != 0)
                    m = 0;
                else
                    m = 4 - m;
                p++;
                n--;
            }

            // restore
            _codec.TotalBytesIn += p - _codec.NextIn;
            _codec.NextIn = p;
            _codec.AvailableBytesIn = n;
            marker = m;

            // return no joy or set up to restart on a new block
            if (m != 4)
                return ZipConstants.Z_DATA_ERROR;
            r = _codec.TotalBytesIn;
            w = _codec.TotalBytesOut;
            Reset();
            _codec.TotalBytesIn = r;
            _codec.TotalBytesOut = w;
            mode = InflateManagerMode.BLOCKS;
            return ZipConstants.Z_OK;
        }

        internal int SyncPoint(ZlibCodec z)
        {
            return blocks.SyncPoint();
        }

        #endregion
    }

    #endregion Класс ZipInflateManager

    #region Класс ZipTree

    internal sealed class ZipTree
    {
        #region Константы

        internal const int Buf_size = 8 * 2;

        #endregion

        #region Поля

        internal static readonly int[] DistanceBase = new[]
            {
                0, 1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192,
                256, 384, 512, 768, 1024, 1536, 2048, 3072, 4096, 6144, 8192, 12288, 16384, 24576
            };

        internal static readonly int[] ExtraDistanceBits = new[]
            {
                0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6,
                7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13
            };

        internal static readonly int[] ExtraLengthBits = new[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2,
                3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
            };

        private static readonly int HEAP_SIZE = (2 * ZipConstants.L_CODES + 1);

        internal static readonly int[] LengthBase = new[]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 20, 24, 28,
                32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 0
            };

        internal static readonly SByte[] LengthCode = new SByte[]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 8, 9, 9, 10, 10, 11, 11,
                12, 12, 12, 12, 13, 13, 13, 13, 14, 14, 14, 14, 15, 15, 15, 15,
                16, 16, 16, 16, 16, 16, 16, 16, 17, 17, 17, 17, 17, 17, 17, 17,
                18, 18, 18, 18, 18, 18, 18, 18, 19, 19, 19, 19, 19, 19, 19, 19,
                20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20,
                21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21,
                22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22,
                23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
                24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
                25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
                25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
                26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
                26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
                27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
                27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28
            };

        private static readonly SByte[] _dist_code = new SByte[]
            {
                0, 1, 2, 3, 4, 4, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7,
                8, 8, 8, 8, 8, 8, 8, 8, 9, 9, 9, 9, 9, 9, 9, 9,
                10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
                11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11,
                12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
                12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
                13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
                13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
                14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
                14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
                14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
                14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
                15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                0, 0, 16, 17, 18, 18, 19, 19, 20, 20, 20, 20, 21, 21, 21, 21,
                22, 22, 22, 22, 22, 22, 22, 22, 23, 23, 23, 23, 23, 23, 23, 23,
                24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
                25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
                26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
                26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
                27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
                27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
                28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
                28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
                28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
                28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
                29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
                29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
                29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
                29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29
            };

        internal static readonly SByte[] bl_order = new SByte[]
            {
                16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
            };

        internal static readonly int[] extra_blbits = new[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 7
            };

        internal short[] dyn_tree; // the dynamic tree

        internal int max_code; // largest code with non zero frequency

        internal ZipStaticTree staticTree; // the corresponding static tree

        #endregion

        #region Методы

        internal static int DistanceCode(int dist)
        {
            return (dist < 256)
                       ? _dist_code[dist]
                       : _dist_code[256 + ZipSharedUtilities.URShift(dist, 7)];
        }

        internal void gen_bitlen(ZipDeflateManager s)
        {
            short[] tree = dyn_tree;
            short[] stree = staticTree.treeCodes;
            int[] extra = staticTree.extraBits;
            int base_Renamed = staticTree.extraBase;
            int max_length = staticTree.maxLength;
            int h; // heap index
            int n, m; // iterate over the tree elements
            int bits; // bit length
            int xbits; // extra bits
            short f; // frequency
            int overflow = 0; // number of elements with bit length too large

            for (bits = 0; bits <= ZipConstants.MAX_BITS; bits++)
                s.bl_count[bits] = 0;

            // In a first pass, compute the optimal bit lengths (which may
            // overflow in the case of the bit length tree).
            tree[s.heap[s.heap_max] * 2 + 1] = 0; // root of the heap

            for (h = s.heap_max + 1; h < HEAP_SIZE; h++)
            {
                n = s.heap[h];
                bits = tree[tree[n * 2 + 1] * 2 + 1] + 1;
                if (bits > max_length)
                {
                    bits = max_length;
                    overflow++;
                }
                tree[n * 2 + 1] = (short)bits;
                // We overwrite tree[n*2+1] which is no longer needed

                if (n > max_code)
                    continue; // not a leaf node

                s.bl_count[bits]++;
                xbits = 0;
                if (n >= base_Renamed)
                    xbits = extra[n - base_Renamed];
                f = tree[n * 2];
                s.opt_len += f * (bits + xbits);
                if (stree != null)
                    s.static_len += f * (stree[n * 2 + 1] + xbits);
            }
            if (overflow == 0)
                return;

            // This happens for example on obj2 and pic of the Calgary corpus
            // Find the first bit length which could increase:
            do
            {
                bits = max_length - 1;
                while (s.bl_count[bits] == 0)
                    bits--;
                s.bl_count[bits]--; // move one leaf down the tree
                s.bl_count[bits + 1] = (short)(s.bl_count[bits + 1] + 2); // move one overflow item as its brother
                s.bl_count[max_length]--;
                // The brother of the overflow item also moves one step up,
                // but this does not affect bl_count[max_length]
                overflow -= 2;
            } while (overflow > 0);

            for (bits = max_length; bits != 0; bits--)
            {
                n = s.bl_count[bits];
                while (n != 0)
                {
                    m = s.heap[--h];
                    if (m > max_code)
                        continue;
                    if (tree[m * 2 + 1] != bits)
                    {
                        s.opt_len = (int)(s.opt_len + (bits - (long)tree[m * 2 + 1]) * tree[m * 2]);
                        tree[m * 2 + 1] = (short)bits;
                    }
                    n--;
                }
            }
        }

        internal void build_tree(ZipDeflateManager s)
        {
            short[] tree = dyn_tree;
            short[] stree = staticTree.treeCodes;
            int elems = staticTree.elems;
            int n, m; // iterate over heap elements
            int max_code = -1; // largest code with non zero frequency
            int node; // new node being created

            // Construct the initial heap, with least frequent element in
            // heap[1]. The sons of heap[n] are heap[2*n] and heap[2*n+1].
            // heap[0] is not used.
            s.heap_len = 0;
            s.heap_max = HEAP_SIZE;

            for (n = 0; n < elems; n++)
            {
                if (tree[n * 2] != 0)
                {
                    s.heap[++s.heap_len] = max_code = n;
                    s.depth[n] = 0;
                }
                else
                    tree[n * 2 + 1] = 0;
            }

            // The pkzip format requires that at least one distance code exists,
            // and that at least one bit should be sent even if there is only one
            // possible code. So to avoid special checks later on we force at least
            // two codes of non zero frequency.
            while (s.heap_len < 2)
            {
                node = s.heap[++s.heap_len] = (max_code < 2 ? ++max_code : 0);
                tree[node * 2] = 1;
                s.depth[node] = 0;
                s.opt_len--;
                if (stree != null)
                    s.static_len -= stree[node * 2 + 1];
                // node is 0 or 1 so it does not have extra bits
            }
            this.max_code = max_code;

            // The elements heap[heap_len/2+1 .. heap_len] are leaves of the tree,
            // establish sub-heaps of increasing lengths:

            for (n = s.heap_len / 2; n >= 1; n--)
                s.pqdownheap(tree, n);

            // Construct the Huffman tree by repeatedly combining the least two
            // frequent nodes.

            node = elems; // next internal node of the tree
            do
            {
                // n = node of least frequency
                n = s.heap[1];
                s.heap[1] = s.heap[s.heap_len--];
                s.pqdownheap(tree, 1);
                m = s.heap[1]; // m = node of next least frequency

                s.heap[--s.heap_max] = n; // keep the nodes sorted by frequency
                s.heap[--s.heap_max] = m;

                // Create a new node father of n and m
                tree[node * 2] = unchecked((short)(tree[n * 2] + tree[m * 2]));
                s.depth[node] = (SByte)(System.Math.Max((Byte)s.depth[n], (Byte)s.depth[m]) + 1);
                tree[n * 2 + 1] = tree[m * 2 + 1] = (short)node;

                // and insert the new node in the heap
                s.heap[1] = node++;
                s.pqdownheap(tree, 1);
            } while (s.heap_len >= 2);

            s.heap[--s.heap_max] = s.heap[1];

            // At this point, the fields freq and dad are set. We can now
            // generate the bit lengths.

            gen_bitlen(s);

            // The field len is now set, we can generate the bit codes
            gen_codes(tree, max_code, s.bl_count);
        }

        internal static void gen_codes(short[] tree, int max_code, short[] bl_count)
        {
            short[] next_code = new short[ZipConstants.MAX_BITS + 1]; // next code value for each bit length
            short code = 0; // running code value
            int bits; // bit index
            int n; // code index

            // The distribution counts are first used to generate the code values
            // without bit reversal.
            for (bits = 1; bits <= ZipConstants.MAX_BITS; bits++)
            {
                unchecked
                {
                    next_code[bits] = code = (short)((code + bl_count[bits - 1]) << 1);
                }
            }

            // Check that the bit counts in bl_count are consistent. The last code
            // must be all ones.
            //Assert (code + bl_count[MAX_BITS]-1 == (1<<MAX_BITS)-1,
            //        "inconsistent bit counts");
            //Tracev((stderr,"\ngen_codes: max_code %d ", max_code));

            for (n = 0; n <= max_code; n++)
            {
                int len = tree[n * 2 + 1];
                if (len == 0)
                    continue;
                // Now reverse the bits
                tree[n * 2] = unchecked((short)(bi_reverse(next_code[len]++, len)));
            }
        }

        internal static int bi_reverse(int code, int len)
        {
            int res = 0;
            do
            {
                res |= code & 1;
                code >>= 1; //SharedUtils.URShift(code, 1);
                res <<= 1;
            } while (--len > 0);
            return res >> 1;
        }

        #endregion
    }

    #endregion Класс ZipTree

    #region Класс ZipStaticTree

    internal sealed class ZipStaticTree
    {
        #region Поля

        internal static readonly ZipStaticTree BitLengths;

        internal static readonly ZipStaticTree Distances;

        internal static readonly ZipStaticTree Literals;

        internal static readonly short[] distTreeCodes = new short[]
            {
                0, 5, 16, 5, 8, 5, 24, 5, 4, 5, 20, 5, 12, 5, 28, 5,
                2, 5, 18, 5, 10, 5, 26, 5, 6, 5, 22, 5, 14, 5, 30, 5,
                1, 5, 17, 5, 9, 5, 25, 5, 5, 5, 21, 5, 13, 5, 29, 5,
                3, 5, 19, 5, 11, 5, 27, 5, 7, 5, 23, 5
            };

        internal static readonly short[] lengthAndLiteralsTreeCodes = new short[]
            {
                12, 8, 140, 8, 76, 8, 204, 8, 44, 8, 172, 8, 108, 8, 236, 8,
                28, 8, 156, 8, 92, 8, 220, 8, 60, 8, 188, 8, 124, 8, 252, 8,
                2, 8, 130, 8, 66, 8, 194, 8, 34, 8, 162, 8, 98, 8, 226, 8,
                18, 8, 146, 8, 82, 8, 210, 8, 50, 8, 178, 8, 114, 8, 242, 8,
                10, 8, 138, 8, 74, 8, 202, 8, 42, 8, 170, 8, 106, 8, 234, 8,
                26, 8, 154, 8, 90, 8, 218, 8, 58, 8, 186, 8, 122, 8, 250, 8,
                6, 8, 134, 8, 70, 8, 198, 8, 38, 8, 166, 8, 102, 8, 230, 8,
                22, 8, 150, 8, 86, 8, 214, 8, 54, 8, 182, 8, 118, 8, 246, 8,
                14, 8, 142, 8, 78, 8, 206, 8, 46, 8, 174, 8, 110, 8, 238, 8,
                30, 8, 158, 8, 94, 8, 222, 8, 62, 8, 190, 8, 126, 8, 254, 8,
                1, 8, 129, 8, 65, 8, 193, 8, 33, 8, 161, 8, 97, 8, 225, 8,
                17, 8, 145, 8, 81, 8, 209, 8, 49, 8, 177, 8, 113, 8, 241, 8,
                9, 8, 137, 8, 73, 8, 201, 8, 41, 8, 169, 8, 105, 8, 233, 8,
                25, 8, 153, 8, 89, 8, 217, 8, 57, 8, 185, 8, 121, 8, 249, 8,
                5, 8, 133, 8, 69, 8, 197, 8, 37, 8, 165, 8, 101, 8, 229, 8,
                21, 8, 149, 8, 85, 8, 213, 8, 53, 8, 181, 8, 117, 8, 245, 8,
                13, 8, 141, 8, 77, 8, 205, 8, 45, 8, 173, 8, 109, 8, 237, 8,
                29, 8, 157, 8, 93, 8, 221, 8, 61, 8, 189, 8, 125, 8, 253, 8,
                19, 9, 275, 9, 147, 9, 403, 9, 83, 9, 339, 9, 211, 9, 467, 9,
                51, 9, 307, 9, 179, 9, 435, 9, 115, 9, 371, 9, 243, 9, 499, 9,
                11, 9, 267, 9, 139, 9, 395, 9, 75, 9, 331, 9, 203, 9, 459, 9,
                43, 9, 299, 9, 171, 9, 427, 9, 107, 9, 363, 9, 235, 9, 491, 9,
                27, 9, 283, 9, 155, 9, 411, 9, 91, 9, 347, 9, 219, 9, 475, 9,
                59, 9, 315, 9, 187, 9, 443, 9, 123, 9, 379, 9, 251, 9, 507, 9,
                7, 9, 263, 9, 135, 9, 391, 9, 71, 9, 327, 9, 199, 9, 455, 9,
                39, 9, 295, 9, 167, 9, 423, 9, 103, 9, 359, 9, 231, 9, 487, 9,
                23, 9, 279, 9, 151, 9, 407, 9, 87, 9, 343, 9, 215, 9, 471, 9,
                55, 9, 311, 9, 183, 9, 439, 9, 119, 9, 375, 9, 247, 9, 503, 9,
                15, 9, 271, 9, 143, 9, 399, 9, 79, 9, 335, 9, 207, 9, 463, 9,
                47, 9, 303, 9, 175, 9, 431, 9, 111, 9, 367, 9, 239, 9, 495, 9,
                31, 9, 287, 9, 159, 9, 415, 9, 95, 9, 351, 9, 223, 9, 479, 9,
                63, 9, 319, 9, 191, 9, 447, 9, 127, 9, 383, 9, 255, 9, 511, 9,
                0, 7, 64, 7, 32, 7, 96, 7, 16, 7, 80, 7, 48, 7, 112, 7,
                8, 7, 72, 7, 40, 7, 104, 7, 24, 7, 88, 7, 56, 7, 120, 7,
                4, 7, 68, 7, 36, 7, 100, 7, 20, 7, 84, 7, 52, 7, 116, 7,
                3, 8, 131, 8, 67, 8, 195, 8, 35, 8, 163, 8, 99, 8, 227, 8
            };

        internal int elems; // max number of elements in the tree

        internal int extraBase; // base index for extra_bits

        internal int[] extraBits; // extra bits for each code or null

        internal int maxLength; // max bit length for the codes

        internal short[] treeCodes; // static tree or null

        #endregion

        #region Конструктор

        static ZipStaticTree()
        {
            Literals = new ZipStaticTree(lengthAndLiteralsTreeCodes, ZipTree.ExtraLengthBits, ZipConstants.LITERALS + 1, ZipConstants.L_CODES, ZipConstants.MAX_BITS);
            Distances = new ZipStaticTree(distTreeCodes, ZipTree.ExtraDistanceBits, 0, ZipConstants.D_CODES, ZipConstants.MAX_BITS);
            BitLengths = new ZipStaticTree(null, ZipTree.extra_blbits, 0, ZipConstants.BL_CODES, ZipConstants.MAX_BL_BITS);
        }

        private ZipStaticTree(short[] treeCodes, int[] extraBits, int extraBase, int elems, int maxLength)
        {
            this.treeCodes = treeCodes;
            this.extraBits = extraBits;
            this.extraBase = extraBase;
            this.elems = elems;
            this.maxLength = maxLength;
        }

        #endregion
    }

    #endregion Класс ZipStaticTree

    #region Класс ZipInfTree

    internal sealed class ZipInfTree
    {
        #region Константы

        internal const int BMAX = 15;

        private const int MANY = 1440;

        private const int Z_BUF_ERROR = -5;

        private const int Z_DATA_ERROR = -3;

        private const int Z_ERRNO = -1;

        private const int Z_MEM_ERROR = -4;

        private const int Z_NEED_DICT = 2;

        private const int Z_OK = 0;

        private const int Z_STREAM_END = 1;

        private const int Z_STREAM_ERROR = -2;

        private const int Z_VERSION_ERROR = -6;

        internal const int fixed_bd = 5;

        internal const int fixed_bl = 9;

        #endregion

        #region Поля

        internal static readonly int[] cpdext = new[]
            {
                0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13
            };

        internal static readonly int[] cpdist = new[]
            {
                1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385, 513, 769, 1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577
            };

        internal static readonly int[] cplens = new[]
            {
                3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258, 0, 0
            };

        internal static readonly int[] cplext = new[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0, 112, 112
            };

        internal static readonly int[] fixed_td = new[]
            {
                80, 5, 1, 87, 5, 257, 83, 5, 17, 91, 5, 4097, 81, 5, 5, 89, 5, 1025, 85, 5, 65, 93, 5, 16385, 80, 5, 3, 88, 5, 513, 84, 5, 33, 92, 5, 8193, 82, 5, 9, 90, 5, 2049, 86, 5, 129, 192, 5, 24577, 80, 5, 2, 87, 5, 385, 83, 5, 25, 91, 5, 6145, 81, 5, 7, 89, 5, 1537, 85, 5, 97, 93, 5, 24577, 80, 5, 4, 88, 5, 769, 84, 5, 49, 92, 5, 12289, 82, 5, 13, 90, 5, 3073, 86, 5, 193, 192, 5, 24577
            };

        internal static readonly int[] fixed_tl = new[]
            {
                96, 7, 256, 0, 8, 80, 0, 8, 16, 84, 8, 115, 82, 7, 31, 0, 8, 112, 0, 8, 48, 0, 9, 192, 80, 7, 10, 0, 8, 96, 0, 8,
                32, 0, 9, 160, 0, 8, 0, 0, 8, 128, 0, 8, 64, 0, 9, 224, 80, 7, 6, 0, 8, 88, 0, 8, 24, 0, 9, 144, 83, 7, 59, 0,
                8, 120, 0, 8, 56, 0, 9, 208, 81, 7, 17, 0, 8, 104, 0, 8, 40, 0, 9, 176, 0, 8, 8, 0, 8, 136, 0, 8, 72, 0, 9, 240, 80, 7, 4, 0, 8, 84, 0, 8, 20, 85, 8, 227, 83, 7, 43, 0, 8, 116, 0, 8, 52, 0, 9, 200, 81, 7, 13, 0, 8, 100,
                0, 8, 36, 0, 9, 168, 0, 8, 4, 0, 8, 132, 0, 8, 68, 0, 9, 232, 80, 7, 8, 0, 8, 92, 0, 8, 28, 0, 9, 152, 84, 7, 83, 0, 8, 124, 0, 8, 60, 0, 9, 216, 82, 7, 23, 0, 8, 108, 0, 8, 44, 0, 9, 184, 0, 8, 12, 0, 8, 140, 0, 8, 76, 0,
                9, 248, 80, 7, 3, 0, 8, 82, 0, 8, 18, 85, 8, 163, 83, 7, 35, 0, 8, 114, 0, 8, 50, 0, 9, 196, 81, 7, 11, 0, 8, 98, 0, 8, 34, 0, 9, 164, 0, 8, 2, 0, 8, 130, 0, 8, 66, 0, 9, 228, 80, 7, 7, 0, 8, 90, 0, 8, 26, 0, 9, 148, 84, 7,
                67, 0, 8, 122, 0, 8, 58, 0, 9, 212, 82, 7, 19, 0, 8, 106, 0, 8, 42, 0, 9, 180, 0, 8, 10, 0, 8, 138, 0, 8, 74, 0, 9, 244, 80, 7, 5, 0, 8, 86, 0, 8, 22, 192, 8, 0, 83, 7, 51, 0, 8, 118, 0, 8, 54, 0, 9, 204, 81, 7, 15, 0, 8, 102,
                0, 8, 38, 0, 9, 172, 0, 8, 6, 0, 8, 134, 0, 8, 70, 0, 9, 236, 80, 7, 9, 0, 8, 94, 0, 8, 30, 0, 9, 156, 84, 7, 99, 0, 8, 126, 0, 8, 62, 0, 9, 220, 82, 7, 27, 0, 8, 110, 0, 8, 46, 0, 9, 188, 0, 8, 14, 0, 8, 142, 0, 8, 78, 0,
                9, 252, 96, 7, 256, 0, 8, 81, 0, 8, 17, 85, 8, 131, 82, 7, 31, 0, 8, 113, 0, 8, 49, 0, 9, 194, 80, 7, 10, 0, 8, 97, 0, 8, 33, 0, 9, 162, 0, 8, 1, 0, 8, 129, 0, 8, 65, 0, 9, 226, 80, 7, 6, 0, 8, 89, 0, 8, 25, 0, 9, 146, 83,
                7, 59, 0, 8, 121, 0, 8, 57, 0, 9, 210, 81, 7, 17, 0, 8, 105, 0, 8, 41, 0, 9, 178, 0, 8, 9, 0, 8, 137, 0, 8, 73, 0, 9, 242, 80, 7, 4, 0, 8, 85, 0, 8, 21, 80, 8, 258, 83, 7, 43, 0, 8, 117, 0, 8, 53, 0, 9, 202, 81, 7, 13, 0,
                8, 101, 0, 8, 37, 0, 9, 170, 0, 8, 5, 0, 8, 133, 0, 8, 69, 0, 9, 234, 80, 7, 8, 0, 8, 93, 0, 8, 29, 0, 9, 154, 84, 7, 83, 0, 8, 125, 0, 8, 61, 0, 9, 218, 82, 7, 23, 0, 8, 109, 0, 8, 45, 0, 9, 186,
                0, 8, 13, 0, 8, 141, 0, 8, 77, 0, 9, 250, 80, 7, 3, 0, 8, 83, 0, 8, 19, 85, 8, 195, 83, 7, 35, 0, 8, 115, 0, 8, 51, 0, 9, 198, 81, 7, 11, 0, 8, 99, 0, 8, 35, 0, 9, 166, 0, 8, 3, 0, 8, 131, 0, 8, 67, 0, 9, 230, 80, 7, 7, 0,
                8, 91, 0, 8, 27, 0, 9, 150, 84, 7, 67, 0, 8, 123, 0, 8, 59, 0, 9, 214, 82, 7, 19, 0, 8, 107, 0, 8, 43, 0, 9, 182, 0, 8, 11, 0, 8, 139, 0, 8, 75, 0, 9, 246, 80, 7, 5, 0, 8, 87, 0, 8, 23, 192, 8, 0, 83, 7, 51, 0, 8, 119, 0,
                8, 55, 0, 9, 206, 81, 7, 15, 0, 8, 103, 0, 8, 39, 0, 9, 174, 0, 8, 7, 0, 8, 135, 0, 8, 71, 0, 9, 238, 80, 7, 9, 0, 8, 95, 0, 8, 31, 0, 9, 158, 84, 7, 99, 0, 8, 127, 0, 8, 63, 0, 9, 222, 82, 7, 27, 0, 8, 111, 0, 8, 47, 0, 9,
                190, 0, 8, 15, 0, 8, 143, 0, 8, 79, 0, 9, 254, 96, 7, 256, 0, 8, 80, 0, 8, 16, 84, 8, 115, 82, 7, 31, 0, 8, 112, 0, 8, 48, 0, 9, 193, 80, 7, 10, 0, 8, 96, 0, 8, 32, 0, 9, 161, 0, 8, 0, 0, 8, 128, 0, 8, 64, 0, 9, 225, 80, 7,
                6, 0, 8, 88, 0, 8, 24, 0, 9, 145, 83, 7, 59, 0, 8, 120, 0, 8, 56, 0, 9, 209, 81, 7, 17, 0, 8, 104, 0, 8, 40, 0, 9, 177, 0, 8, 8, 0, 8, 136, 0, 8, 72, 0, 9, 241, 80, 7, 4, 0, 8, 84, 0, 8, 20, 85, 8, 227, 83, 7, 43, 0, 8, 116,
                0, 8, 52, 0, 9, 201, 81, 7, 13, 0, 8, 100, 0, 8, 36, 0, 9, 169, 0, 8, 4, 0, 8, 132, 0, 8, 68, 0, 9, 233, 80, 7, 8, 0, 8, 92, 0, 8, 28, 0, 9, 153, 84, 7, 83, 0, 8, 124, 0, 8, 60, 0, 9, 217, 82, 7, 23, 0, 8, 108, 0, 8, 44, 0,
                9, 185, 0, 8, 12, 0, 8, 140, 0, 8, 76, 0, 9, 249, 80, 7, 3, 0, 8, 82, 0, 8, 18, 85, 8, 163, 83, 7, 35, 0, 8, 114, 0, 8, 50, 0, 9, 197, 81, 7, 11, 0, 8, 98, 0, 8, 34, 0, 9, 165, 0, 8, 2, 0, 8, 130, 0, 8, 66, 0, 9, 229, 80, 7,
                7, 0, 8, 90, 0, 8, 26, 0, 9, 149, 84, 7, 67, 0, 8, 122, 0, 8, 58, 0, 9, 213, 82, 7, 19, 0, 8, 106, 0, 8, 42, 0, 9, 181, 0, 8, 10, 0, 8, 138, 0, 8, 74, 0, 9, 245, 80, 7, 5, 0, 8, 86, 0, 8, 22, 192,
                8, 0, 83, 7, 51, 0, 8, 118, 0, 8, 54, 0, 9, 205, 81, 7, 15, 0, 8, 102, 0, 8, 38, 0, 9, 173, 0, 8, 6, 0, 8, 134, 0, 8, 70, 0, 9, 237, 80, 7, 9, 0, 8, 94, 0, 8, 30, 0, 9, 157, 84, 7, 99, 0, 8, 126, 0, 8, 62, 0, 9, 221, 82,
                7, 27, 0, 8, 110, 0, 8, 46, 0, 9, 189, 0, 8, 14, 0, 8, 142, 0, 8, 78, 0, 9, 253, 96, 7, 256, 0, 8, 81, 0, 8, 17, 85, 8, 131, 82, 7, 31, 0, 8, 113, 0, 8, 49, 0, 9, 195, 80, 7, 10, 0, 8, 97, 0, 8, 33, 0, 9, 163, 0, 8, 1, 0,
                8, 129, 0, 8, 65, 0, 9, 227, 80, 7, 6, 0, 8, 89, 0, 8, 25, 0, 9, 147, 83, 7, 59, 0, 8, 121, 0, 8, 57, 0, 9, 211, 81, 7, 17, 0, 8, 105, 0, 8, 41, 0, 9, 179, 0, 8, 9, 0, 8, 137, 0, 8, 73, 0, 9, 243, 80, 7, 4, 0, 8, 85, 0, 8,
                21, 80, 8, 258, 83, 7, 43, 0, 8, 117, 0, 8, 53, 0, 9, 203, 81, 7, 13, 0, 8, 101, 0, 8, 37, 0, 9, 171, 0, 8, 5, 0, 8, 133, 0, 8, 69, 0, 9, 235, 80, 7, 8, 0, 8, 93, 0, 8, 29, 0, 9, 155, 84, 7, 83, 0, 8, 125, 0, 8, 61, 0, 9,
                219, 82, 7, 23, 0, 8, 109, 0, 8, 45, 0, 9, 187, 0, 8, 13, 0, 8, 141, 0, 8, 77, 0, 9, 251, 80, 7, 3, 0, 8, 83, 0, 8, 19, 85, 8, 195, 83, 7, 35, 0, 8, 115, 0, 8, 51, 0, 9, 199, 81, 7, 11, 0, 8, 99, 0, 8, 35, 0, 9, 167, 0, 8,
                3, 0, 8, 131, 0, 8, 67, 0, 9, 231, 80, 7, 7, 0, 8, 91, 0, 8, 27, 0, 9, 151, 84, 7, 67, 0, 8, 123, 0, 8, 59, 0, 9, 215, 82, 7, 19, 0, 8, 107, 0, 8, 43, 0, 9, 183, 0, 8, 11, 0, 8, 139, 0, 8, 75, 0, 9, 247, 80, 7, 5, 0, 8, 87,
                0, 8, 23, 192, 8, 0, 83, 7, 51, 0, 8, 119, 0, 8, 55, 0, 9, 207, 81, 7, 15, 0, 8, 103, 0, 8, 39, 0, 9, 175, 0, 8, 7, 0, 8, 135, 0, 8, 71, 0, 9, 239, 80, 7, 9, 0, 8, 95, 0, 8, 31, 0, 9, 159, 84, 7, 99, 0, 8, 127, 0, 8, 63, 0,
                9, 223, 82, 7, 27, 0, 8, 111, 0, 8, 47, 0, 9, 191, 0, 8, 15, 0, 8, 143, 0, 8, 79, 0, 9, 255
            };

        internal int[] c = null; // bit length count table

        internal int[] hn = null; // hufts used in space

        internal int[] r = null; // table entry for structure assignment

        internal int[] u = null; // table stack

        internal int[] v = null; // work area for huft_build 

        internal int[] x = null; // bit offsets, then code stack

        #endregion

        #region Методы

        private int huft_build(int[] b, int bindex, int n, int s, int[] d, int[] e, int[] t, int[] m, int[] hp, int[] hn, int[] v)
        {
            // Given a list of code lengths and a maximum table size, make a set of
            // tables to decode that set of codes.  Return Z_OK on success, Z_BUF_ERROR
            // if the given code set is incomplete (the tables are still built in this
            // case), Z_DATA_ERROR if the input is invalid (an over-subscribed set of
            // lengths), or Z_MEM_ERROR if not enough memory.

            int a; // counter for codes of length k
            int f; // i repeats in table every f entries
            int g; // maximum code length
            int h; // table level
            int i; // counter, current code
            int j; // counter
            int k; // number of bits in current code
            int l; // bits per table (returned in m)
            int mask; // (1 << w) - 1, to avoid cc -O bug on HP
            int p; // pointer into c[], b[], or v[]
            int q; // points to current table
            int w; // bits before this table == (l * h)
            int xp; // pointer into x
            int y; // number of dummy codes added
            int z; // number of entries in current table

            // Generate counts for each bit length

            p = 0;
            i = n;
            do
            {
                c[b[bindex + p]]++;
                p++;
                i--; // assume all entries <= BMAX
            } while (i != 0);

            if (c[0] == n)
            {
                // null input--all zero length codes
                t[0] = -1;
                m[0] = 0;
                return Z_OK;
            }

            // Find minimum and maximum length, bound *m by those
            l = m[0];
            for (j = 1; j <= BMAX; j++)
            {
                if (c[j] != 0)
                    break;
            }
            k = j; // minimum code length
            if (l < j)
                l = j;
            for (i = BMAX; i != 0; i--)
            {
                if (c[i] != 0)
                    break;
            }
            g = i; // maximum code length
            if (l > i)
                l = i;
            m[0] = l;

            // Adjust last length count to fill out codes, if needed
            for (y = 1 << j; j < i; j++, y <<= 1)
            {
                if ((y -= c[j]) < 0)
                    return Z_DATA_ERROR;
            }
            if ((y -= c[i]) < 0)
                return Z_DATA_ERROR;
            c[i] += y;

            // Generate starting offsets into the value table for each length
            x[1] = j = 0;
            p = 1;
            xp = 2;
            while (--i != 0)
            {
                // note that i == g from above
                x[xp] = (j += c[p]);
                xp++;
                p++;
            }

            // Make a table of values in order of bit lengths
            i = 0;
            p = 0;
            do
            {
                if ((j = b[bindex + p]) != 0)
                    v[x[j]++] = i;
                p++;
            } while (++i < n);
            n = x[g]; // set n to length of v

            // Generate the Huffman codes and for each, make the table entries
            x[0] = i = 0; // first Huffman code is zero
            p = 0; // grab values in bit order
            h = -1; // no tables yet--level -1
            w = -l; // bits decoded == (l * h)
            u[0] = 0; // just to keep compilers happy
            q = 0; // ditto
            z = 0; // ditto

            // go through the bit lengths (k already is bits in shortest code)
            for (; k <= g; k++)
            {
                a = c[k];
                while (a-- != 0)
                {
                    // here i is the Huffman code of length k bits for value *p
                    // make tables up to required level
                    while (k > w + l)
                    {
                        h++;
                        w += l; // previous table always l bits
                        // compute minimum size table less than or equal to l bits
                        z = g - w;
                        z = (z > l) ? l : z; // table size upper limit
                        if ((f = 1 << (j = k - w)) > a + 1)
                        {
                            // try a k-w bit table
                            // too few codes for k-w bit table
                            f -= (a + 1); // deduct codes from patterns left
                            xp = k;
                            if (j < z)
                            {
                                while (++j < z)
                                {
                                    // try smaller tables up to z bits
                                    if ((f <<= 1) <= c[++xp])
                                        break; // enough codes to use up j bits
                                    f -= c[xp]; // else deduct codes from patterns
                                }
                            }
                        }
                        z = 1 << j; // table entries for j-bit table

                        // allocate new table
                        if (hn[0] + z > MANY)
                        {
                            // (note: doesn't matter for fixed)
                            return Z_DATA_ERROR; // overflow of MANY
                        }
                        u[h] = q = hn[0]; // DEBUG
                        hn[0] += z;

                        // connect to last table, if there is one
                        if (h != 0)
                        {
                            x[h] = i; // save pattern for backing up
                            r[0] = (SByte)j; // bits in this table
                            r[1] = (SByte)l; // bits to dump before this table
                            j = ZipSharedUtilities.URShift(i, (w - l));
                            r[2] = (q - u[h - 1] - j); // offset to this table
                            Array.Copy(r, 0, hp, (u[h - 1] + j) * 3, 3); // connect to last table
                        }
                        else
                            t[0] = q; // first table is returned result
                    }

                    // set up table entry in r
                    r[1] = (SByte)(k - w);
                    if (p >= n)
                        r[0] = 128 + 64; // out of values--invalid code
                    else if (v[p] < s)
                    {
                        r[0] = (SByte)(v[p] < 256 ? 0 : 32 + 64); // 256 is end-of-block
                        r[2] = v[p++]; // simple code is just the value
                    }
                    else
                    {
                        r[0] = (SByte)(e[v[p] - s] + 16 + 64); // non-simple--look up in lists
                        r[2] = d[v[p++] - s];
                    }

                    // fill code-like entries with r
                    f = 1 << (k - w);
                    for (j = ZipSharedUtilities.URShift(i, w); j < z; j += f)
                        Array.Copy(r, 0, hp, (q + j) * 3, 3);

                    // backwards increment the k-bit code i
                    for (j = 1 << (k - 1); (i & j) != 0; j = ZipSharedUtilities.URShift(j, 1))
                        i ^= j;
                    i ^= j;

                    // backup over finished tables
                    mask = (1 << w) - 1; // needed on HP, cc -O bug
                    while ((i & mask) != x[h])
                    {
                        h--; // don't need to update q
                        w -= l;
                        mask = (1 << w) - 1;
                    }
                }
            }
            // Return Z_BUF_ERROR if we were given an incomplete table
            return y != 0 && g != 1 ? Z_BUF_ERROR : Z_OK;
        }

        internal int inflate_trees_bits(int[] c, int[] bb, int[] tb, int[] hp, ZlibCodec z)
        {
            int result;
            initWorkArea(19);
            hn[0] = 0;
            result = huft_build(c, 0, 19, 19, null, null, tb, bb, hp, hn, v);

            if (result == Z_DATA_ERROR)
                z.Message = "oversubscribed dynamic bit lengths tree";
            else if (result == Z_BUF_ERROR || bb[0] == 0)
            {
                z.Message = "incomplete dynamic bit lengths tree";
                result = Z_DATA_ERROR;
            }
            return result;
        }

        internal int inflate_trees_dynamic(int nl, int nd, int[] c, int[] bl, int[] bd, int[] tl, int[] td, int[] hp, ZlibCodec z)
        {
            int result;

            // build literal/length tree
            initWorkArea(288);
            hn[0] = 0;
            result = huft_build(c, 0, nl, 257, cplens, cplext, tl, bl, hp, hn, v);
            if (result != Z_OK || bl[0] == 0)
            {
                if (result == Z_DATA_ERROR)
                    z.Message = "oversubscribed literal/length tree";
                else if (result != Z_MEM_ERROR)
                {
                    z.Message = "incomplete literal/length tree";
                    result = Z_DATA_ERROR;
                }
                return result;
            }

            // build distance tree
            initWorkArea(288);
            result = huft_build(c, nl, nd, 0, cpdist, cpdext, td, bd, hp, hn, v);

            if (result != Z_OK || (bd[0] == 0 && nl > 257))
            {
                if (result == Z_DATA_ERROR)
                    z.Message = "oversubscribed distance tree";
                else if (result == Z_BUF_ERROR)
                {
                    z.Message = "incomplete distance tree";
                    result = Z_DATA_ERROR;
                }
                else if (result != Z_MEM_ERROR)
                {
                    z.Message = "empty distance tree with lengths";
                    result = Z_DATA_ERROR;
                }
                return result;
            }

            return Z_OK;
        }

        internal static int inflate_trees_fixed(int[] bl, int[] bd, int[][] tl, int[][] td, ZlibCodec z)
        {
            bl[0] = fixed_bl;
            bd[0] = fixed_bd;
            tl[0] = fixed_tl;
            td[0] = fixed_td;
            return Z_OK;
        }

        private void initWorkArea(int vsize)
        {
            if (hn == null)
            {
                hn = new int[1];
                v = new int[vsize];
                c = new int[BMAX + 1];
                r = new int[3];
                u = new int[BMAX];
                x = new int[BMAX + 1];
            }
            else
            {
                if (v.Length < vsize)
                    v = new int[vsize];
                Array.Clear(v, 0, vsize);
                Array.Clear(c, 0, BMAX + 1);
                r[0] = 0;
                r[1] = 0;
                r[2] = 0;
                //  for(int i=0; i<BMAX; i++){u[i]=0;}
                //Array.Copy(c, 0, u, 0, BMAX);
                Array.Clear(u, 0, BMAX);
                //  for(int i=0; i<BMAX+1; i++){x[i]=0;}
                //Array.Copy(c, 0, x, 0, BMAX + 1);
                Array.Clear(x, 0, BMAX + 1);
            }
        }

        #endregion
    }

    #endregion Класс ZipInfTree

    #region Класс ZipInflateBlocks

    internal sealed class ZipInflateBlocks
    {
        #region Перечисления

        private enum InflateBlockMode
        {
            TYPE = 0, // get type bits (3, including end bit)
            LENS = 1, // get lengths for stored
            STORED = 2, // processing stored block
            TABLE = 3, // get table lengths
            BTREE = 4, // get bit lengths tree for a dynamic block
            DTREE = 5, // get length, distance trees for a dynamic block
            CODES = 6, // processing fixed or dynamic block
            DRY = 7, // output remaining window Bytes
            DONE = 8, // finished last block, done
            BAD = 9, // ot a data error--stuck here
        }

        #endregion

        #region Константы

        private const int MANY = 1440;

        #endregion

        #region Поля

        internal static readonly int[] border = new[]
            {
                16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
            };

        internal ZlibCodec _codec; // pointer back to this zlib stream

        internal int[] bb = new int[1]; // bit length tree depth

        internal int bitb; // bit buffer

        internal int bitk; // bits in bit buffer

        internal int[] blens; // bit lengths of codes

        internal uint check; // check on output

        internal System.Object checkfn; // check function

        internal ZipInflateCodes codes = new ZipInflateCodes(); // if CODES, current state

        internal int end; // one Byte after sliding window

        internal int[] hufts; // single malloc for tree space

        internal int index; // index into blens (or border)

        internal ZipInfTree inftree = new ZipInfTree();

        internal int last; // true if this block is the last block

        internal int left; // if STORED, Bytes left to copy

        private InflateBlockMode mode; // current inflate_block mode

        internal int readAt; // window read pointer

        internal int table; // table lengths (14 bits)

        internal int[] tb = new int[1]; // bit length decoding tree

        internal Byte[] window; // sliding window

        internal int writeAt; // window write pointer

        #endregion

        #region Конструктор

        internal ZipInflateBlocks(ZlibCodec codec, System.Object checkfn, int w)
        {
            _codec = codec;
            hufts = new int[MANY * 3];
            window = new Byte[w];
            end = w;
            this.checkfn = checkfn;
            mode = InflateBlockMode.TYPE;
            Reset();
        }

        #endregion

        #region Методы

        internal uint Reset()
        {
            uint oldCheck = check;
            mode = InflateBlockMode.TYPE;
            bitk = 0;
            bitb = 0;
            readAt = writeAt = 0;

            if (checkfn != null)
                _codec._Adler32 = check = ZipAdler.Adler32(0, null, 0, 0);
            return oldCheck;
        }

        internal int Process(int r)
        {
            int t; // temporary storage
            int b; // bit buffer
            int k; // bits in bit buffer
            int p; // input data pointer
            int n; // Bytes available there
            int q; // output window write pointer
            int m; // Bytes to end of window or read pointer

            // copy input/output information to locals (UPDATE macro restores)

            p = _codec.NextIn;
            n = _codec.AvailableBytesIn;
            b = bitb;
            k = bitk;

            q = writeAt;
            m = (q < readAt ? readAt - q - 1 : end - q);

            // process input based on current state
            while (true)
            {
                switch (mode)
                {
                    case InflateBlockMode.TYPE:

                        while (k < (3))
                        {
                            if (n != 0)
                                r = ZipConstants.Z_OK;
                            else
                            {
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                            }

                            n--;
                            b |= (_codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }
                        t = (b & 7);
                        last = t & 1;

                        switch ((uint)t >> 1)
                        {
                            case 0: // stored
                                b >>= 3;
                                k -= (3);
                                t = k & 7; // go to Byte boundary
                                b >>= t;
                                k -= t;
                                mode = InflateBlockMode.LENS; // get length of stored block
                                break;

                            case 1: // fixed
                                int[] bl = new int[1];
                                int[] bd = new int[1];
                                int[][] tl = new int[1][];
                                int[][] td = new int[1][];
                                ZipInfTree.inflate_trees_fixed(bl, bd, tl, td, _codec);
                                codes.Init(bl[0], bd[0], tl[0], 0, td[0], 0);
                                b >>= 3;
                                k -= 3;
                                mode = InflateBlockMode.CODES;
                                break;

                            case 2: // dynamic
                                b >>= 3;
                                k -= 3;
                                mode = InflateBlockMode.TABLE;
                                break;

                            case 3: // illegal
                                b >>= 3;
                                k -= 3;
                                mode = InflateBlockMode.BAD;
                                _codec.Message = "invalid block type";
                                r = ZipConstants.Z_DATA_ERROR;
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                        }
                        break;

                    case InflateBlockMode.LENS:

                        while (k < (32))
                        {
                            if (n != 0)
                                r = ZipConstants.Z_OK;
                            else
                            {
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                            }
                            n--;
                            b |= (_codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        if ((((~b) >> 16) & 0xffff) != (b & 0xffff))
                        {
                            mode = InflateBlockMode.BAD;
                            _codec.Message = "invalid stored block lengths";
                            r = ZipConstants.Z_DATA_ERROR;

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }
                        left = (b & 0xffff);
                        b = k = 0; // dump bits
                        mode = left != 0 ? InflateBlockMode.STORED : (last != 0 ? InflateBlockMode.DRY : InflateBlockMode.TYPE);
                        break;

                    case InflateBlockMode.STORED:
                        if (n == 0)
                        {
                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        if (m == 0)
                        {
                            if (q == end && readAt != 0)
                            {
                                q = 0;
                                m = (q < readAt ? readAt - q - 1 : end - q);
                            }
                            if (m == 0)
                            {
                                writeAt = q;
                                r = Flush(r);
                                q = writeAt;
                                m = (q < readAt ? readAt - q - 1 : end - q);
                                if (q == end && readAt != 0)
                                {
                                    q = 0;
                                    m = (q < readAt ? readAt - q - 1 : end - q);
                                }
                                if (m == 0)
                                {
                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }
                            }
                        }
                        r = ZipConstants.Z_OK;

                        t = left;
                        if (t > n)
                            t = n;
                        if (t > m)
                            t = m;
                        Array.Copy(_codec.InputBuffer, p, window, q, t);
                        p += t;
                        n -= t;
                        q += t;
                        m -= t;
                        if ((left -= t) != 0)
                            break;
                        mode = last != 0 ? InflateBlockMode.DRY : InflateBlockMode.TYPE;
                        break;

                    case InflateBlockMode.TABLE:

                        while (k < (14))
                        {
                            if (n != 0)
                                r = ZipConstants.Z_OK;
                            else
                            {
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                            }

                            n--;
                            b |= (_codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        table = t = (b & 0x3fff);
                        if ((t & 0x1f) > 29 || ((t >> 5) & 0x1f) > 29)
                        {
                            mode = InflateBlockMode.BAD;
                            _codec.Message = "too many length or distance symbols";
                            r = ZipConstants.Z_DATA_ERROR;

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }
                        t = 258 + (t & 0x1f) + ((t >> 5) & 0x1f);
                        if (blens == null || blens.Length < t)
                            blens = new int[t];
                        else
                        {
                            Array.Clear(blens, 0, t);
                            // for (int i = 0; i < t; i++)
                            // {
                            //     blens[i] = 0;
                            // }
                        }

                        b >>= 14;
                        k -= 14;

                        index = 0;
                        mode = InflateBlockMode.BTREE;
                        goto case InflateBlockMode.BTREE;

                    case InflateBlockMode.BTREE:
                        while (index < 4 + (table >> 10))
                        {
                            while (k < (3))
                            {
                                if (n != 0)
                                    r = ZipConstants.Z_OK;
                                else
                                {
                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }

                                n--;
                                b |= (_codec.InputBuffer[p++] & 0xff) << k;
                                k += 8;
                            }

                            blens[border[index++]] = b & 7;

                            b >>= 3;
                            k -= 3;
                        }

                        while (index < 19)
                            blens[border[index++]] = 0;

                        bb[0] = 7;
                        t = inftree.inflate_trees_bits(blens, bb, tb, hufts, _codec);
                        if (t != ZipConstants.Z_OK)
                        {
                            r = t;
                            if (r == ZipConstants.Z_DATA_ERROR)
                            {
                                blens = null;
                                mode = InflateBlockMode.BAD;
                            }

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        index = 0;
                        mode = InflateBlockMode.DTREE;
                        goto case InflateBlockMode.DTREE;

                    case InflateBlockMode.DTREE:
                        while (true)
                        {
                            t = table;
                            if (!(index < 258 + (t & 0x1f) + ((t >> 5) & 0x1f)))
                                break;

                            int i, j, c;

                            t = bb[0];

                            while (k < t)
                            {
                                if (n != 0)
                                    r = ZipConstants.Z_OK;
                                else
                                {
                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }

                                n--;
                                b |= (_codec.InputBuffer[p++] & 0xff) << k;
                                k += 8;
                            }

                            t = hufts[(tb[0] + (b & ZipConstants.InflateMask[t])) * 3 + 1];
                            c = hufts[(tb[0] + (b & ZipConstants.InflateMask[t])) * 3 + 2];

                            if (c < 16)
                            {
                                b >>= t;
                                k -= t;
                                blens[index++] = c;
                            }
                            else
                            {
                                // c == 16..18
                                i = c == 18 ? 7 : c - 14;
                                j = c == 18 ? 11 : 3;

                                while (k < (t + i))
                                {
                                    if (n != 0)
                                        r = ZipConstants.Z_OK;
                                    else
                                    {
                                        bitb = b;
                                        bitk = k;
                                        _codec.AvailableBytesIn = n;
                                        _codec.TotalBytesIn += p - _codec.NextIn;
                                        _codec.NextIn = p;
                                        writeAt = q;
                                        return Flush(r);
                                    }

                                    n--;
                                    b |= (_codec.InputBuffer[p++] & 0xff) << k;
                                    k += 8;
                                }

                                b >>= t;
                                k -= t;

                                j += (b & ZipConstants.InflateMask[i]);

                                b >>= i;
                                k -= i;

                                i = index;
                                t = table;
                                if (i + j > 258 + (t & 0x1f) + ((t >> 5) & 0x1f) || (c == 16 && i < 1))
                                {
                                    blens = null;
                                    mode = InflateBlockMode.BAD;
                                    _codec.Message = "invalid bit length repeat";
                                    r = ZipConstants.Z_DATA_ERROR;

                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }

                                c = (c == 16) ? blens[i - 1] : 0;
                                do
                                {
                                    blens[i++] = c;
                                } while (--j != 0);
                                index = i;
                            }
                        }

                        tb[0] = -1;
                        {
                            int[] bl = new[]
                                {
                                    9
                                }; // must be <= 9 for lookahead assumptions
                            int[] bd = new[]
                                {
                                    6
                                }; // must be <= 9 for lookahead assumptions
                            int[] tl = new int[1];
                            int[] td = new int[1];

                            t = table;
                            t = inftree.inflate_trees_dynamic(257 + (t & 0x1f), 1 + ((t >> 5) & 0x1f), blens, bl, bd, tl, td, hufts, _codec);

                            if (t != ZipConstants.Z_OK)
                            {
                                if (t == ZipConstants.Z_DATA_ERROR)
                                {
                                    blens = null;
                                    mode = InflateBlockMode.BAD;
                                }
                                r = t;

                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                            }
                            codes.Init(bl[0], bd[0], hufts, tl[0], hufts, td[0]);
                        }
                        mode = InflateBlockMode.CODES;
                        goto case InflateBlockMode.CODES;

                    case InflateBlockMode.CODES:
                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;

                        r = codes.Process(this, r);
                        if (r != ZipConstants.Z_STREAM_END)
                            return Flush(r);

                        r = ZipConstants.Z_OK;
                        p = _codec.NextIn;
                        n = _codec.AvailableBytesIn;
                        b = bitb;
                        k = bitk;
                        q = writeAt;
                        m = (q < readAt ? readAt - q - 1 : end - q);

                        if (last == 0)
                        {
                            mode = InflateBlockMode.TYPE;
                            break;
                        }
                        mode = InflateBlockMode.DRY;
                        goto case InflateBlockMode.DRY;

                    case InflateBlockMode.DRY:
                        writeAt = q;
                        r = Flush(r);
                        q = writeAt;
                        m = (q < readAt ? readAt - q - 1 : end - q);
                        if (readAt != writeAt)
                        {
                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }
                        mode = InflateBlockMode.DONE;
                        goto case InflateBlockMode.DONE;

                    case InflateBlockMode.DONE:
                        r = ZipConstants.Z_STREAM_END;
                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;
                        return Flush(r);

                    case InflateBlockMode.BAD:
                        r = ZipConstants.Z_DATA_ERROR;

                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;
                        return Flush(r);

                    default:
                        r = ZipConstants.Z_STREAM_ERROR;

                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;
                        return Flush(r);
                }
            }
        }

        internal void Free()
        {
            Reset();
            window = null;
            hufts = null;
        }

        internal void SetDictionary(Byte[] d, int start, int n)
        {
            Array.Copy(d, start, window, 0, n);
            readAt = writeAt = n;
        }

        internal int SyncPoint()
        {
            return mode == InflateBlockMode.LENS ? 1 : 0;
        }

        internal int Flush(int r)
        {
            int nBytes;

            for (int pass = 0; pass < 2; pass++)
            {
                if (pass == 0)
                {
                    // compute number of Bytes to copy as far as end of window
                    nBytes = ((readAt <= writeAt ? writeAt : end) - readAt);
                }
                else
                {
                    // compute Bytes to copy
                    nBytes = writeAt - readAt;
                }

                // workitem 8870
                if (nBytes == 0)
                {
                    if (r == ZipConstants.Z_BUF_ERROR)
                        r = ZipConstants.Z_OK;
                    return r;
                }

                if (nBytes > _codec.AvailableBytesOut)
                    nBytes = _codec.AvailableBytesOut;

                if (nBytes != 0 && r == ZipConstants.Z_BUF_ERROR)
                    r = ZipConstants.Z_OK;

                // update counters
                _codec.AvailableBytesOut -= nBytes;
                _codec.TotalBytesOut += nBytes;

                // update check information
                if (checkfn != null)
                    _codec._Adler32 = check = ZipAdler.Adler32(check, window, readAt, nBytes);

                // copy as far as end of window
                Array.Copy(window, readAt, _codec.OutputBuffer, _codec.NextOut, nBytes);
                _codec.NextOut += nBytes;
                readAt += nBytes;

                // see if more to copy at beginning of window
                if (readAt == end && pass == 0)
                {
                    // wrap pointers
                    readAt = 0;
                    if (writeAt == end)
                        writeAt = 0;
                }
                else pass++;
            }

            // done
            return r;
        }

        #endregion
    }

    #endregion Класс ZipInflateBlocks

    #region Класс ZipInflateCodes

    internal sealed class ZipInflateCodes
    {
        #region Константы

        private const int BADCODE = 9; // x: got error

        private const int COPY = 5; // o: copying Bytes in window, waiting for space

        private const int DIST = 3; // i: get distance next

        private const int DISTEXT = 4; // i: getting distance extra

        private const int END = 8; // x: got eob and all data flushed

        private const int LEN = 1; // i: get length/literal/eob next

        private const int LENEXT = 2; // i: getting length extra (have base)

        private const int LIT = 6; // o: got literal, waiting for output space

        private const int START = 0; // x: set up for LEN

        private const int WASH = 7; // o: got eob, possibly still output waiting

        #endregion

        #region Поля

        internal int bitsToGet; // bits to get for extra

        internal Byte dbits; // dtree bits decoder per branch

        internal int dist; // distance back to copy from

        internal int[] dtree; // distance tree

        internal int dtree_index; // distance tree

        internal Byte lbits; // ltree bits decoded per branch

        internal int len;

        internal int lit;

        internal int[] ltree; // literal/length/eob tree

        internal int ltree_index; // literal/length/eob tree

        internal int mode; // current inflate_codes mode

        internal int need; // bits needed

        internal int[] tree; // pointer into tree

        internal int tree_index = 0;

        #endregion

        #region Методы

        internal void Init(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index)
        {
            mode = START;
            lbits = (Byte)bl;
            dbits = (Byte)bd;
            ltree = tl;
            ltree_index = tl_index;
            dtree = td;
            dtree_index = td_index;
            tree = null;
        }

        internal int Process(ZipInflateBlocks blocks, int r)
        {
            int j; // temporary storage
            int tindex; // temporary pointer
            int e; // extra bits or operation
            int b = 0; // bit buffer
            int k = 0; // bits in bit buffer
            int p = 0; // input data pointer
            int n; // Bytes available there
            int q; // output window write pointer
            int m; // Bytes to end of window or read pointer
            int f; // pointer to copy Strings from

            ZlibCodec z = blocks._codec;

            // copy input/output information to locals (UPDATE macro restores)
            p = z.NextIn;
            n = z.AvailableBytesIn;
            b = blocks.bitb;
            k = blocks.bitk;
            q = blocks.writeAt;
            m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;

            // process input and output based on current state
            while (true)
            {
                switch (mode)
                {
                        // waiting for "i:"=input, "o:"=output, "x:"=nothing
                    case START: // x: set up for LEN
                        if (m >= 258 && n >= 10)
                        {
                            blocks.bitb = b;
                            blocks.bitk = k;
                            z.AvailableBytesIn = n;
                            z.TotalBytesIn += p - z.NextIn;
                            z.NextIn = p;
                            blocks.writeAt = q;
                            r = InflateFast(lbits, dbits, ltree, ltree_index, dtree, dtree_index, blocks, z);

                            p = z.NextIn;
                            n = z.AvailableBytesIn;
                            b = blocks.bitb;
                            k = blocks.bitk;
                            q = blocks.writeAt;
                            m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;

                            if (r != ZipConstants.Z_OK)
                            {
                                mode = (r == ZipConstants.Z_STREAM_END) ? WASH : BADCODE;
                                break;
                            }
                        }
                        need = lbits;
                        tree = ltree;
                        tree_index = ltree_index;

                        mode = LEN;
                        goto case LEN;

                    case LEN: // i: get length/literal/eob next
                        j = need;

                        while (k < j)
                        {
                            if (n != 0)
                                r = ZipConstants.Z_OK;
                            else
                            {
                                blocks.bitb = b;
                                blocks.bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.writeAt = q;
                                return blocks.Flush(r);
                            }
                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        tindex = (tree_index + (b & ZipConstants.InflateMask[j])) * 3;

                        b >>= (tree[tindex + 1]);
                        k -= (tree[tindex + 1]);

                        e = tree[tindex];

                        if (e == 0)
                        {
                            // literal
                            lit = tree[tindex + 2];
                            mode = LIT;
                            break;
                        }
                        if ((e & 16) != 0)
                        {
                            // length
                            bitsToGet = e & 15;
                            len = tree[tindex + 2];
                            mode = LENEXT;
                            break;
                        }
                        if ((e & 64) == 0)
                        {
                            // next table
                            need = e;
                            tree_index = tindex / 3 + tree[tindex + 2];
                            break;
                        }
                        if ((e & 32) != 0)
                        {
                            // end of block
                            mode = WASH;
                            break;
                        }
                        mode = BADCODE; // invalid code
                        z.Message = "invalid literal/length code";
                        r = ZipConstants.Z_DATA_ERROR;

                        blocks.bitb = b;
                        blocks.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.writeAt = q;
                        return blocks.Flush(r);

                    case LENEXT: // i: getting length extra (have base)
                        j = bitsToGet;

                        while (k < j)
                        {
                            if (n != 0)
                                r = ZipConstants.Z_OK;
                            else
                            {
                                blocks.bitb = b;
                                blocks.bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.writeAt = q;
                                return blocks.Flush(r);
                            }
                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        len += (b & ZipConstants.InflateMask[j]);

                        b >>= j;
                        k -= j;

                        need = dbits;
                        tree = dtree;
                        tree_index = dtree_index;
                        mode = DIST;
                        goto case DIST;

                    case DIST: // i: get distance next
                        j = need;

                        while (k < j)
                        {
                            if (n != 0)
                                r = ZipConstants.Z_OK;
                            else
                            {
                                blocks.bitb = b;
                                blocks.bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.writeAt = q;
                                return blocks.Flush(r);
                            }
                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        tindex = (tree_index + (b & ZipConstants.InflateMask[j])) * 3;

                        b >>= tree[tindex + 1];
                        k -= tree[tindex + 1];

                        e = (tree[tindex]);
                        if ((e & 0x10) != 0)
                        {
                            // distance
                            bitsToGet = e & 15;
                            dist = tree[tindex + 2];
                            mode = DISTEXT;
                            break;
                        }
                        if ((e & 64) == 0)
                        {
                            // next table
                            need = e;
                            tree_index = tindex / 3 + tree[tindex + 2];
                            break;
                        }
                        mode = BADCODE; // invalid code
                        z.Message = "invalid distance code";
                        r = ZipConstants.Z_DATA_ERROR;

                        blocks.bitb = b;
                        blocks.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.writeAt = q;
                        return blocks.Flush(r);

                    case DISTEXT: // i: getting distance extra
                        j = bitsToGet;

                        while (k < j)
                        {
                            if (n != 0)
                                r = ZipConstants.Z_OK;
                            else
                            {
                                blocks.bitb = b;
                                blocks.bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.writeAt = q;
                                return blocks.Flush(r);
                            }
                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        dist += (b & ZipConstants.InflateMask[j]);

                        b >>= j;
                        k -= j;

                        mode = COPY;
                        goto case COPY;

                    case COPY: // o: copying Bytes in window, waiting for space
                        f = q - dist;
                        while (f < 0)
                        {
                            // modulo window size-"while" instead
                            f += blocks.end; // of "if" handles invalid distances
                        }
                        while (len != 0)
                        {
                            if (m == 0)
                            {
                                if (q == blocks.end && blocks.readAt != 0)
                                {
                                    q = 0;
                                    m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;
                                }
                                if (m == 0)
                                {
                                    blocks.writeAt = q;
                                    r = blocks.Flush(r);
                                    q = blocks.writeAt;
                                    m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;

                                    if (q == blocks.end && blocks.readAt != 0)
                                    {
                                        q = 0;
                                        m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;
                                    }

                                    if (m == 0)
                                    {
                                        blocks.bitb = b;
                                        blocks.bitk = k;
                                        z.AvailableBytesIn = n;
                                        z.TotalBytesIn += p - z.NextIn;
                                        z.NextIn = p;
                                        blocks.writeAt = q;
                                        return blocks.Flush(r);
                                    }
                                }
                            }

                            blocks.window[q++] = blocks.window[f++];
                            m--;

                            if (f == blocks.end)
                                f = 0;
                            len--;
                        }
                        mode = START;
                        break;

                    case LIT: // o: got literal, waiting for output space
                        if (m == 0)
                        {
                            if (q == blocks.end && blocks.readAt != 0)
                            {
                                q = 0;
                                m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;
                            }
                            if (m == 0)
                            {
                                blocks.writeAt = q;
                                r = blocks.Flush(r);
                                q = blocks.writeAt;
                                m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;

                                if (q == blocks.end && blocks.readAt != 0)
                                {
                                    q = 0;
                                    m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;
                                }
                                if (m == 0)
                                {
                                    blocks.bitb = b;
                                    blocks.bitk = k;
                                    z.AvailableBytesIn = n;
                                    z.TotalBytesIn += p - z.NextIn;
                                    z.NextIn = p;
                                    blocks.writeAt = q;
                                    return blocks.Flush(r);
                                }
                            }
                        }
                        r = ZipConstants.Z_OK;

                        blocks.window[q++] = (Byte)lit;
                        m--;

                        mode = START;
                        break;

                    case WASH: // o: got eob, possibly more output
                        if (k > 7)
                        {
                            // return unused Byte, if any
                            k -= 8;
                            n++;
                            p--; // can always return one
                        }

                        blocks.writeAt = q;
                        r = blocks.Flush(r);
                        q = blocks.writeAt;
                        m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;

                        if (blocks.readAt != blocks.writeAt)
                        {
                            blocks.bitb = b;
                            blocks.bitk = k;
                            z.AvailableBytesIn = n;
                            z.TotalBytesIn += p - z.NextIn;
                            z.NextIn = p;
                            blocks.writeAt = q;
                            return blocks.Flush(r);
                        }
                        mode = END;
                        goto case END;

                    case END:
                        r = ZipConstants.Z_STREAM_END;
                        blocks.bitb = b;
                        blocks.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.writeAt = q;
                        return blocks.Flush(r);

                    case BADCODE: // x: got error

                        r = ZipConstants.Z_DATA_ERROR;

                        blocks.bitb = b;
                        blocks.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.writeAt = q;
                        return blocks.Flush(r);

                    default:
                        r = ZipConstants.Z_STREAM_ERROR;

                        blocks.bitb = b;
                        blocks.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.writeAt = q;
                        return blocks.Flush(r);
                }
            }
        }

        internal int InflateFast(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, ZipInflateBlocks s, ZlibCodec z)
        {
            int t; // temporary pointer
            int[] tp; // temporary pointer
            int tp_index; // temporary pointer
            int e; // extra bits or operation
            int b; // bit buffer
            int k; // bits in bit buffer
            int p; // input data pointer
            int n; // Bytes available there
            int q; // output window write pointer
            int m; // Bytes to end of window or read pointer
            int ml; // mask for literal/length tree
            int md; // mask for distance tree
            int c; // Bytes to copy
            int d; // distance back to copy from
            int r; // copy source pointer

            int tp_index_t_3; // (tp_index+t)*3

            // load input, output, bit values
            p = z.NextIn;
            n = z.AvailableBytesIn;
            b = s.bitb;
            k = s.bitk;
            q = s.writeAt;
            m = q < s.readAt ? s.readAt - q - 1 : s.end - q;

            // initialize masks
            ml = ZipConstants.InflateMask[bl];
            md = ZipConstants.InflateMask[bd];

            // do until not enough input or output space for fast loop
            do
            {
                // assume called with m >= 258 && n >= 10
                // get literal/length code
                while (k < (20))
                {
                    // max bits for literal/length code
                    n--;
                    b |= (z.InputBuffer[p++] & 0xff) << k;
                    k += 8;
                }

                t = b & ml;
                tp = tl;
                tp_index = tl_index;
                tp_index_t_3 = (tp_index + t) * 3;
                if ((e = tp[tp_index_t_3]) == 0)
                {
                    b >>= (tp[tp_index_t_3 + 1]);
                    k -= (tp[tp_index_t_3 + 1]);

                    s.window[q++] = (Byte)tp[tp_index_t_3 + 2];
                    m--;
                    continue;
                }
                do
                {
                    b >>= (tp[tp_index_t_3 + 1]);
                    k -= (tp[tp_index_t_3 + 1]);

                    if ((e & 16) != 0)
                    {
                        e &= 15;
                        c = tp[tp_index_t_3 + 2] + (b & ZipConstants.InflateMask[e]);

                        b >>= e;
                        k -= e;

                        // decode distance base of block to copy
                        while (k < 15)
                        {
                            // max bits for distance code
                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        t = b & md;
                        tp = td;
                        tp_index = td_index;
                        tp_index_t_3 = (tp_index + t) * 3;
                        e = tp[tp_index_t_3];

                        do
                        {
                            b >>= (tp[tp_index_t_3 + 1]);
                            k -= (tp[tp_index_t_3 + 1]);

                            if ((e & 16) != 0)
                            {
                                // get extra bits to add to distance base
                                e &= 15;
                                while (k < e)
                                {
                                    // get extra bits (up to 13)
                                    n--;
                                    b |= (z.InputBuffer[p++] & 0xff) << k;
                                    k += 8;
                                }

                                d = tp[tp_index_t_3 + 2] + (b & ZipConstants.InflateMask[e]);

                                b >>= e;
                                k -= e;

                                // do the copy
                                m -= c;
                                if (q >= d)
                                {
                                    // offset before dest
                                    //  just copy
                                    r = q - d;
                                    if (q - r > 0 && 2 > (q - r))
                                    {
                                        s.window[q++] = s.window[r++]; // minimum count is three,
                                        s.window[q++] = s.window[r++]; // so unroll loop a little
                                        c -= 2;
                                    }
                                    else
                                    {
                                        Array.Copy(s.window, r, s.window, q, 2);
                                        q += 2;
                                        r += 2;
                                        c -= 2;
                                    }
                                }
                                else
                                {
                                    // else offset after destination
                                    r = q - d;
                                    do
                                    {
                                        r += s.end; // force pointer in window
                                    } while (r < 0); // covers invalid distances
                                    e = s.end - r;
                                    if (c > e)
                                    {
                                        // if source crosses,
                                        c -= e; // wrapped copy
                                        if (q - r > 0 && e > (q - r))
                                        {
                                            do
                                            {
                                                s.window[q++] = s.window[r++];
                                            } while (--e != 0);
                                        }
                                        else
                                        {
                                            Array.Copy(s.window, r, s.window, q, e);
                                            q += e;
                                            r += e;
                                            e = 0;
                                        }
                                        r = 0; // copy rest from start of window
                                    }
                                }

                                // copy all or what's left
                                if (q - r > 0 && c > (q - r))
                                {
                                    do
                                    {
                                        s.window[q++] = s.window[r++];
                                    } while (--c != 0);
                                }
                                else
                                {
                                    Array.Copy(s.window, r, s.window, q, c);
                                    q += c;
                                    r += c;
                                    c = 0;
                                }
                                break;
                            }
                            else if ((e & 64) == 0)
                            {
                                t += tp[tp_index_t_3 + 2];
                                t += (b & ZipConstants.InflateMask[e]);
                                tp_index_t_3 = (tp_index + t) * 3;
                                e = tp[tp_index_t_3];
                            }
                            else
                            {
                                z.Message = "invalid distance code";

                                c = z.AvailableBytesIn - n;
                                c = (k >> 3) < c ? k >> 3 : c;
                                n += c;
                                p -= c;
                                k -= (c << 3);

                                s.bitb = b;
                                s.bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                s.writeAt = q;

                                return ZipConstants.Z_DATA_ERROR;
                            }
                        } while (true);
                        break;
                    }

                    if ((e & 64) == 0)
                    {
                        t += tp[tp_index_t_3 + 2];
                        t += (b & ZipConstants.InflateMask[e]);
                        tp_index_t_3 = (tp_index + t) * 3;
                        if ((e = tp[tp_index_t_3]) == 0)
                        {
                            b >>= (tp[tp_index_t_3 + 1]);
                            k -= (tp[tp_index_t_3 + 1]);
                            s.window[q++] = (Byte)tp[tp_index_t_3 + 2];
                            m--;
                            break;
                        }
                    }
                    else if ((e & 32) != 0)
                    {
                        c = z.AvailableBytesIn - n;
                        c = (k >> 3) < c ? k >> 3 : c;
                        n += c;
                        p -= c;
                        k -= (c << 3);

                        s.bitb = b;
                        s.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        s.writeAt = q;

                        return ZipConstants.Z_STREAM_END;
                    }
                    else
                    {
                        z.Message = "invalid literal/length code";

                        c = z.AvailableBytesIn - n;
                        c = (k >> 3) < c ? k >> 3 : c;
                        n += c;
                        p -= c;
                        k -= (c << 3);

                        s.bitb = b;
                        s.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        s.writeAt = q;

                        return ZipConstants.Z_DATA_ERROR;
                    }
                } while (true);
            } while (m >= 258 && n >= 10);

            // not enough input or output--restore pointers and return
            c = z.AvailableBytesIn - n;
            c = (k >> 3) < c ? k >> 3 : c;
            n += c;
            p -= c;
            k -= (c << 3);

            s.bitb = b;
            s.bitk = k;
            z.AvailableBytesIn = n;
            z.TotalBytesIn += p - z.NextIn;
            z.NextIn = p;
            s.writeAt = q;

            return ZipConstants.Z_OK;
        }

        #endregion
    }

    #endregion Класс ZipInflateCodes

    #region Класс GZipStream

    public class GZipStream : System.IO.Stream
    {
        #region Поля

        internal static readonly System.DateTime _unixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal static readonly System.Text.Encoding iso8859dash1 = System.Text.Encoding.GetEncoding("iso-8859-1");

        public DateTime? LastModified;

        private String _Comment;

        private int _Crc32;

        private String _FileName;

        internal ZipBaseStream _baseStream;

        private Boolean _disposed;

        private Boolean _firstReadDone;

        private int _headerByteCount;

        #endregion

        #region Свойства

        public String Comment
        {
            get { return _Comment; }
            set
            {
                if (_disposed) throw new ObjectDisposedException("GZipStream");
                _Comment = value;
            }
        }

        public String FileName
        {
            get { return _FileName; }
            set
            {
                if (_disposed) throw new ObjectDisposedException("GZipStream");
                _FileName = value;
                if (_FileName == null) return;
                if (_FileName.IndexOf("/") != -1)
                    _FileName = _FileName.Replace("/", "\\");
                if (_FileName.EndsWith("\\"))
                    throw new Exception("Illegal filename");
                if (_FileName.IndexOf("\\") != -1)
                {
                    // trim any leading path
                    _FileName = Path.GetFileName(_FileName);
                }
            }
        }

        public int Crc32
        {
            get { return _Crc32; }
        }

        public virtual ZipFlushType FlushMode
        {
            get { return (_baseStream._flushMode); }
            set
            {
                if (_disposed) throw new ObjectDisposedException("GZipStream");
                _baseStream._flushMode = value;
            }
        }

        public int BufferSize
        {
            get { return _baseStream._bufferSize; }
            set
            {
                if (_disposed) throw new ObjectDisposedException("GZipStream");
                if (_baseStream._workingBuffer != null)
                    throw new ZlibException("The working buffer is already set.");
                if (value < ZipConstants.WorkingBufferSizeMin)
                    throw new ZlibException(String.Format("Don't be silly. {0} Bytes?? Use a bigger buffer, at least {1}.", value, ZipConstants.WorkingBufferSizeMin));
                _baseStream._bufferSize = value;
            }
        }

        public virtual long TotalIn
        {
            get { return _baseStream._codec.TotalBytesIn; }
        }

        public virtual long TotalOut
        {
            get { return _baseStream._codec.TotalBytesOut; }
        }

        public override Boolean CanRead
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("GZipStream");
                return _baseStream._stream.CanRead;
            }
        }

        public override Boolean CanSeek
        {
            get { return false; }
        }

        public override Boolean CanWrite
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("GZipStream");
                return _baseStream._stream.CanWrite;
            }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                if (_baseStream._streamMode == ZipBaseStream.StreamMode.Writer)
                    return _baseStream._codec.TotalBytesOut + _headerByteCount;
                if (_baseStream._streamMode == ZipBaseStream.StreamMode.Reader)
                    return _baseStream._codec.TotalBytesIn + _baseStream._gzipHeaderByteCount;
                return 0;
            }

            set { throw new NotImplementedException(); }
        }

        #endregion

        #region Конструктор

        public GZipStream(Stream stream, ZipCompressionMode mode) : this(stream, mode, ZipCompressionLevel.Default, false)
        {
        }

        public GZipStream(Stream stream, ZipCompressionMode mode, ZipCompressionLevel level) : this(stream, mode, level, false)
        {
        }

        public GZipStream(Stream stream, ZipCompressionMode mode, Boolean leaveOpen) : this(stream, mode, ZipCompressionLevel.Default, leaveOpen)
        {
        }

        public GZipStream(Stream stream, ZipCompressionMode mode, ZipCompressionLevel level, Boolean leaveOpen)
        {
            _baseStream = new ZipBaseStream(stream, mode, level, ZlibStreamFlavor.GZIP, leaveOpen);
        }

        #endregion

        #region Методы

        protected override void Dispose(Boolean disposing)
        {
            try
            {
                if (!_disposed)
                {
                    if (disposing && (_baseStream != null))
                    {
                        _baseStream.Close();
                        _Crc32 = _baseStream.Crc32;
                    }
                    _disposed = true;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            if (_disposed) throw new ObjectDisposedException("GZipStream");
            _baseStream.Flush();
        }

        public override int Read(Byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException("GZipStream");
            int n = _baseStream.Read(buffer, offset, count);

            // Console.WriteLine("GZipStream::Read(buffer, off({0}), c({1}) = {2}", offset, count, n);
            // Console.WriteLine( Util.FormatByteArray(buffer, offset, n) );

            if (!_firstReadDone)
            {
                _firstReadDone = true;
                FileName = _baseStream._GzipFileName;
                Comment = _baseStream._GzipComment;
            }
            return n;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(Byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException("GZipStream");
            if (_baseStream._streamMode == ZipBaseStream.StreamMode.Undefined)
            {
                //Console.WriteLine("GZipStream: First write");
                if (_baseStream.WantCompress)
                {
                    // first write in compression, therefore, emit the GZIP header
                    _headerByteCount = EmitHeader();
                }
                else
                    throw new InvalidOperationException();
            }

            _baseStream.Write(buffer, offset, count);
        }

        private int EmitHeader()
        {
            Byte[] commentBytes = (Comment == null) ? null : iso8859dash1.GetBytes(Comment);
            Byte[] filenameBytes = (FileName == null) ? null : iso8859dash1.GetBytes(FileName);

            int cbLength = (Comment == null) ? 0 : commentBytes.Length + 1;
            int fnLength = (FileName == null) ? 0 : filenameBytes.Length + 1;

            int bufferLength = 10 + cbLength + fnLength;
            Byte[] header = new Byte[bufferLength];
            int i = 0;
            // ID
            header[i++] = 0x1F;
            header[i++] = 0x8B;

            // compression method
            header[i++] = 8;
            Byte flag = 0;
            if (Comment != null)
                flag ^= 0x10;
            if (FileName != null)
                flag ^= 0x8;

            // flag
            header[i++] = flag;

            // mtime
            if (!LastModified.HasValue) LastModified = DateTime.Now;
            System.TimeSpan delta = LastModified.Value - _unixEpoch;
            Int32 timet = (Int32)delta.TotalSeconds;
            Array.Copy(BitConverter.GetBytes(timet), 0, header, i, 4);
            i += 4;

            // xflg
            header[i++] = 0; // this field is totally useless
            // OS
            header[i++] = 0xFF; // 0xFF == unspecified

            // extra field length - only if FEXTRA is set, which it is not.
            //header[i++]= 0;
            //header[i++]= 0;

            // filename
            if (fnLength != 0)
            {
                Array.Copy(filenameBytes, 0, header, i, fnLength - 1);
                i += fnLength - 1;
                header[i++] = 0; // terminate
            }

            // comment
            if (cbLength != 0)
            {
                Array.Copy(commentBytes, 0, header, i, cbLength - 1);
                i += cbLength - 1;
                header[i++] = 0; // terminate
            }

            _baseStream._stream.Write(header, 0, header.Length);

            return header.Length; // Bytes written
        }

        public static Byte[] CompressString(String s)
        {
            using (var ms = new MemoryStream())
            {
                System.IO.Stream compressor =
                    new GZipStream(ms, ZipCompressionMode.Compress, ZipCompressionLevel.BestCompression);
                ZipBaseStream.CompressString(s, compressor);
                return ms.ToArray();
            }
        }

        public static Byte[] CompressBuffer(Byte[] b)
        {
            using (var ms = new MemoryStream())
            {
                System.IO.Stream compressor =
                    new GZipStream(ms, ZipCompressionMode.Compress, ZipCompressionLevel.BestCompression);

                ZipBaseStream.CompressBuffer(b, compressor);
                return ms.ToArray();
            }
        }

        public static String UncompressString(Byte[] compressed)
        {
            using (var input = new MemoryStream(compressed))
            {
                Stream decompressor = new GZipStream(input, ZipCompressionMode.Decompress);
                return ZipBaseStream.UncompressString(compressed, decompressor);
            }
        }

        public static Byte[] UncompressBuffer(Byte[] compressed)
        {
            using (var input = new System.IO.MemoryStream(compressed))
            {
                System.IO.Stream decompressor =
                    new GZipStream(input, ZipCompressionMode.Decompress);

                return ZipBaseStream.UncompressBuffer(compressed, decompressor);
            }
        }

        #endregion
    }

    #endregion Класс GZipStream

    #region Класс ZipRand

    internal static class ZipRand
    {
        #region Поля

        private static int[] RNUMS =
            {
                619, 720, 127, 481, 931, 816, 813, 233, 566, 247,
                985, 724, 205, 454, 863, 491, 741, 242, 949, 214,
                733, 859, 335, 708, 621, 574, 73, 654, 730, 472,
                419, 436, 278, 496, 867, 210, 399, 680, 480, 51,
                878, 465, 811, 169, 869, 675, 611, 697, 867, 561,
                862, 687, 507, 283, 482, 129, 807, 591, 733, 623,
                150, 238, 59, 379, 684, 877, 625, 169, 643, 105,
                170, 607, 520, 932, 727, 476, 693, 425, 174, 647,
                73, 122, 335, 530, 442, 853, 695, 249, 445, 515,
                909, 545, 703, 919, 874, 474, 882, 500, 594, 612,
                641, 801, 220, 162, 819, 984, 589, 513, 495, 799,
                161, 604, 958, 533, 221, 400, 386, 867, 600, 782,
                382, 596, 414, 171, 516, 375, 682, 485, 911, 276,
                98, 553, 163, 354, 666, 933, 424, 341, 533, 870,
                227, 730, 475, 186, 263, 647, 537, 686, 600, 224,
                469, 68, 770, 919, 190, 373, 294, 822, 808, 206,
                184, 943, 795, 384, 383, 461, 404, 758, 839, 887,
                715, 67, 618, 276, 204, 918, 873, 777, 604, 560,
                951, 160, 578, 722, 79, 804, 96, 409, 713, 940,
                652, 934, 970, 447, 318, 353, 859, 672, 112, 785,
                645, 863, 803, 350, 139, 93, 354, 99, 820, 908,
                609, 772, 154, 274, 580, 184, 79, 626, 630, 742,
                653, 282, 762, 623, 680, 81, 927, 626, 789, 125,
                411, 521, 938, 300, 821, 78, 343, 175, 128, 250,
                170, 774, 972, 275, 999, 639, 495, 78, 352, 126,
                857, 956, 358, 619, 580, 124, 737, 594, 701, 612,
                669, 112, 134, 694, 363, 992, 809, 743, 168, 974,
                944, 375, 748, 52, 600, 747, 642, 182, 862, 81,
                344, 805, 988, 739, 511, 655, 814, 334, 249, 515,
                897, 955, 664, 981, 649, 113, 974, 459, 893, 228,
                433, 837, 553, 268, 926, 240, 102, 654, 459, 51,
                686, 754, 806, 760, 493, 403, 415, 394, 687, 700,
                946, 670, 656, 610, 738, 392, 760, 799, 887, 653,
                978, 321, 576, 617, 626, 502, 894, 679, 243, 440,
                680, 879, 194, 572, 640, 724, 926, 56, 204, 700,
                707, 151, 457, 449, 797, 195, 791, 558, 945, 679,
                297, 59, 87, 824, 713, 663, 412, 693, 342, 606,
                134, 108, 571, 364, 631, 212, 174, 643, 304, 329,
                343, 97, 430, 751, 497, 314, 983, 374, 822, 928,
                140, 206, 73, 263, 980, 736, 876, 478, 430, 305,
                170, 514, 364, 692, 829, 82, 855, 953, 676, 246,
                369, 970, 294, 750, 807, 827, 150, 790, 288, 923,
                804, 378, 215, 828, 592, 281, 565, 555, 710, 82,
                896, 831, 547, 261, 524, 462, 293, 465, 502, 56,
                661, 821, 976, 991, 658, 869, 905, 758, 745, 193,
                768, 550, 608, 933, 378, 286, 215, 979, 792, 961,
                61, 688, 793, 644, 986, 403, 106, 366, 905, 644,
                372, 567, 466, 434, 645, 210, 389, 550, 919, 135,
                780, 773, 635, 389, 707, 100, 626, 958, 165, 504,
                920, 176, 193, 713, 857, 265, 203, 50, 668, 108,
                645, 990, 626, 197, 510, 357, 358, 850, 858, 364,
                936, 638
            };

        #endregion

        #region Методы

        internal static int Rnums(int i)
        {
            return RNUMS[i];
        }

        #endregion
    }

    #endregion Класс ZipRand

    #region Класс ZipAdler

    public sealed class ZipAdler
    {
        #region Константы

        private static readonly uint BASE = 65521;

        private static readonly int NMAX = 5552;

        #endregion Константы

        #region Методы

#pragma warning disable 3001
#pragma warning disable 3002
        public static uint Adler32(uint adler, Byte[] buf, int index, int len)
        {
            if (buf == null)
                return 1;

            uint s1 = (adler & 0xffff);
            uint s2 = ((adler >> 16) & 0xffff);

            while (len > 0)
            {
                int k = len < NMAX ? len : NMAX;
                len -= k;
                while (k >= 16)
                {
                    //s1 += (buf[index++] & 0xff); s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    k -= 16;
                }
                if (k != 0)
                {
                    do
                    {
                        s1 += buf[index++];
                        s2 += s1;
                    } while (--k != 0);
                }
                s1 %= BASE;
                s2 %= BASE;
            }
            return ((s2 << 16) | s1);
        }
#pragma warning restore 3001
#pragma warning restore 3002

        #endregion Методы
    }

    #endregion Класс ZipAdler

    #endregion Классы
}