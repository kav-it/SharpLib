using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    [Target("File")]
    public class FileTarget : TargetWithLayoutHeaderAndFooter, ICreateFileParameters
    {
        #region Поля

        private readonly DynamicArchiveFileHandlerClass _dynamicArchiveFileHandler;

        private readonly Dictionary<string, DateTime> _initializedFiles;

        private IFileAppenderFactory _appenderFactory;

        private Timer _autoClosingTimer;

        private int _initializedFilesCounter;

        private LineEndingMode _lineEndingMode;

        private int _maxArchiveFilesField;

        private BaseFileAppender[] _recentAppenders;

        #endregion

        #region Свойства

        [RequiredParameter]
        public Layout FileName { get; set; }

        [DefaultValue(true)]
        [Advanced]
        public bool CreateDirs { get; set; }

        [DefaultValue(false)]
        public bool DeleteOldFileOnStartup { get; set; }

        [DefaultValue(false)]
        public bool ArchiveOldFileOnStartup { get; set; }

        [DefaultValue(false)]
        [Advanced]
        public bool ReplaceFileContentsOnEachWrite { get; set; }

        [DefaultValue(false)]
        public bool KeepFileOpen { get; set; }

        [DefaultValue(true)]
        public bool EnableFileDelete { get; set; }

        [DefaultValue("")]
        public string ArchiveDateFormat { get; set; }

        [Advanced]
        public Win32FileAttributes FileAttributes { get; set; }

        [Advanced]
        public LineEndingMode LineEnding
        {
            get { return _lineEndingMode; }

            set
            {
                _lineEndingMode = value;
                switch (value)
                {
                    case LineEndingMode.CR:
                        NewLineChars = "\r";
                        break;

                    case LineEndingMode.LF:
                        NewLineChars = "\n";
                        break;

                    case LineEndingMode.CRLF:
                        NewLineChars = "\r\n";
                        break;

                    case LineEndingMode.Default:
                        NewLineChars = EnvironmentHelper.NewLine;
                        break;

                    case LineEndingMode.None:
                        NewLineChars = string.Empty;
                        break;
                }
            }
        }

        [DefaultValue(true)]
        public bool AutoFlush { get; set; }

        [DefaultValue(5)]
        [Advanced]
        public int OpenFileCacheSize { get; set; }

        [DefaultValue(-1)]
        [Advanced]
        public int OpenFileCacheTimeout { get; set; }

        [DefaultValue(32768)]
        public int BufferSize { get; set; }

        public Encoding Encoding { get; set; }

        [DefaultValue(true)]
        public bool ConcurrentWrites { get; set; }

        [DefaultValue(false)]
        public bool NetworkWrites { get; set; }

        [DefaultValue(10)]
        [Advanced]
        public int ConcurrentWriteAttempts { get; set; }

        [DefaultValue(1)]
        [Advanced]
        public int ConcurrentWriteAttemptDelay { get; set; }

        public long ArchiveAboveSize { get; set; }

        public FileArchivePeriod ArchiveEvery { get; set; }

        public Layout ArchiveFileName { get; set; }

        [DefaultValue(9)]
        public int MaxArchiveFiles
        {
            get { return _maxArchiveFilesField; }
            set
            {
                _maxArchiveFilesField = value;

                _dynamicArchiveFileHandler.MaxArchiveFileToKeep = value;
            }
        }

        [DefaultValue(false)]
        public bool ForceManaged { get; set; }

        public ArchiveNumberingMode ArchiveNumbering { get; set; }

        protected internal string NewLineChars { get; private set; }

        #endregion

        #region Конструктор

        public FileTarget()
        {
            _lineEndingMode = LineEndingMode.Default;
            _initializedFiles = new Dictionary<string, DateTime>();
            ArchiveNumbering = ArchiveNumberingMode.Sequence;
            _maxArchiveFilesField = 9;
            ConcurrentWriteAttemptDelay = 1;
            ArchiveEvery = FileArchivePeriod.None;
            ArchiveAboveSize = -1;
            ConcurrentWriteAttempts = 10;
            ConcurrentWrites = true;
            Encoding = Encoding.Default;
            BufferSize = 32768;
            AutoFlush = true;
            FileAttributes = Win32FileAttributes.Normal;
            NewLineChars = EnvironmentHelper.NewLine;
            EnableFileDelete = true;
            OpenFileCacheTimeout = -1;
            OpenFileCacheSize = 5;
            CreateDirs = true;
            _dynamicArchiveFileHandler = new DynamicArchiveFileHandlerClass(MaxArchiveFiles);
            ForceManaged = false;
            ArchiveDateFormat = string.Empty;
        }

        #endregion

        #region Методы

        public void CleanupInitializedFiles()
        {
            CleanupInitializedFiles(DateTime.Now.AddDays(-2));
        }

        public void CleanupInitializedFiles(DateTime cleanupThreshold)
        {
            var filesToUninitialize = new List<string>();

            foreach (var de in _initializedFiles)
            {
                string fileName = de.Key;
                DateTime lastWriteTime = de.Value;
                if (lastWriteTime < cleanupThreshold)
                {
                    filesToUninitialize.Add(fileName);
                }
            }

            foreach (string fileName in filesToUninitialize)
            {
                WriteFooterAndUninitialize(fileName);
            }
        }

        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            try
            {
                foreach (BaseFileAppender t in _recentAppenders)
                {
                    if (t == null)
                    {
                        break;
                    }

                    t.Flush();
                }

                asyncContinuation(null);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                asyncContinuation(exception);
            }
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (!KeepFileOpen)
            {
                _appenderFactory = RetryingMultiProcessFileAppender.TheFactory;
            }
            else
            {
                if (ArchiveAboveSize != -1 || ArchiveEvery != FileArchivePeriod.None)
                {
                    if (NetworkWrites)
                    {
                        _appenderFactory = RetryingMultiProcessFileAppender.TheFactory;
                    }
                    else if (ConcurrentWrites)
                    {
                        _appenderFactory = MutexMultiProcessFileAppender.TheFactory;
                    }
                    else
                    {
                        _appenderFactory = CountingSingleProcessFileAppender.TheFactory;
                    }
                }
                else
                {
                    if (NetworkWrites)
                    {
                        _appenderFactory = RetryingMultiProcessFileAppender.TheFactory;
                    }
                    else if (ConcurrentWrites)
                    {
                        _appenderFactory = MutexMultiProcessFileAppender.TheFactory;
                    }
                    else
                    {
                        _appenderFactory = SingleProcessFileAppender.TheFactory;
                    }
                }
            }

            _recentAppenders = new BaseFileAppender[OpenFileCacheSize];

            if ((OpenFileCacheSize > 0 || EnableFileDelete) && OpenFileCacheTimeout > 0)
            {
                _autoClosingTimer = new Timer(
                    AutoClosingTimerCallback,
                    null,
                    OpenFileCacheTimeout * 1000,
                    OpenFileCacheTimeout * 1000);
            }
        }

        protected override void CloseTarget()
        {
            base.CloseTarget();

            foreach (string fileName in new List<string>(_initializedFiles.Keys))
            {
                WriteFooterAndUninitialize(fileName);
            }

            if (_autoClosingTimer != null)
            {
                _autoClosingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _autoClosingTimer.Dispose();
                _autoClosingTimer = null;
            }

            if (_recentAppenders != null)
            {
                for (int i = 0; i < _recentAppenders.Length; ++i)
                {
                    if (_recentAppenders[i] == null)
                    {
                        break;
                    }

                    _recentAppenders[i].Close();
                    _recentAppenders[i] = null;
                }
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            string fileName = CleanupFileName(FileName.Render(logEvent));
            byte[] bytes = GetBytesToWrite(logEvent);

            if (ShouldAutoArchive(fileName, logEvent, bytes.Length))
            {
                InvalidateCacheItem(fileName);
                DoAutoArchive(fileName, logEvent);
            }

            WriteToFile(fileName, bytes, false);
        }

        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            var buckets = logEvents.BucketSort(c => FileName.Render(c.LogEvent));
            using (var ms = new MemoryStream())
            {
                foreach (var bucket in buckets)
                {
                    string fileName = CleanupFileName(bucket.Key);

                    ms.SetLength(0);
                    ms.Position = 0;

                    LogEventInfo firstLogEvent = null;

                    foreach (AsyncLogEventInfo ev in bucket.Value)
                    {
                        if (firstLogEvent == null)
                        {
                            firstLogEvent = ev.LogEvent;
                        }

                        byte[] bytes = GetBytesToWrite(ev.LogEvent);
                        ms.Write(bytes, 0, bytes.Length);
                    }

                    FlushCurrentFileWrites(fileName, firstLogEvent, ms);
                }
            }
        }

        protected virtual string GetFormattedMessage(LogEventInfo logEvent)
        {
            return Layout.Render(logEvent);
        }

        protected virtual byte[] GetBytesToWrite(LogEventInfo logEvent)
        {
            string renderedText = GetFormattedMessage(logEvent) + NewLineChars;
            return TransformBytes(Encoding.GetBytes(renderedText));
        }

        protected virtual byte[] TransformBytes(byte[] value)
        {
            return value;
        }

        private static Boolean IsContainValidNumberPatternForReplacement(string pattern)
        {
            int startingIndex = pattern.IndexOf("{#", StringComparison.Ordinal);
            int endingIndex = pattern.IndexOf("#}", StringComparison.Ordinal);

            return (startingIndex != -1 && endingIndex != -1 && startingIndex < endingIndex);
        }

        private static string ReplaceNumber(string pattern, int value)
        {
            int firstPart = pattern.IndexOf("{#", StringComparison.Ordinal);
            int lastPart = pattern.IndexOf("#}", StringComparison.Ordinal) + 2;
            int numDigits = lastPart - firstPart - 2;

            return pattern.Substring(0, firstPart) + Convert.ToString(value, 10).PadLeft(numDigits, '0') + pattern.Substring(lastPart);
        }

        private void FlushCurrentFileWrites(string currentFileName, LogEventInfo firstLogEvent, MemoryStream ms)
        {
            try
            {
                if (currentFileName != null)
                {
                    if (ShouldAutoArchive(currentFileName, firstLogEvent, (int)ms.Length))
                    {
                        WriteFooterAndUninitialize(currentFileName);
                        InvalidateCacheItem(currentFileName);
                        DoAutoArchive(currentFileName, firstLogEvent);
                    }

                    WriteToFile(currentFileName, ms.ToArray(), false);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }
            }
        }

        private void RecursiveRollingRename(string fileName, string pattern, int archiveNumber)
        {
            if (archiveNumber >= MaxArchiveFiles)
            {
                File.Delete(fileName);
                return;
            }

            if (!File.Exists(fileName))
            {
                return;
            }

            string newFileName = ReplaceNumber(pattern, archiveNumber);
            if (File.Exists(fileName))
            {
                RecursiveRollingRename(newFileName, pattern, archiveNumber + 1);
            }

            try
            {
                MoveFileToArchive(fileName, newFileName);
            }
            catch (IOException)
            {
                string dir = Path.GetDirectoryName(newFileName);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                MoveFileToArchive(fileName, newFileName);
            }
        }

        private void SequentialArchive(string fileName, string pattern)
        {
            string baseNamePattern = Path.GetFileName(pattern);

            if (baseNamePattern != null)
            {
                int firstPart = baseNamePattern.IndexOf("{#", StringComparison.Ordinal);
                int lastPart = baseNamePattern.IndexOf("#}", StringComparison.Ordinal) + 2;
                int trailerLength = baseNamePattern.Length - lastPart;

                string fileNameMask = baseNamePattern.Substring(0, firstPart) + "*" + baseNamePattern.Substring(lastPart);

                string dirName = Path.GetDirectoryName(Path.GetFullPath(pattern));
                int nextNumber = -1;
                int minNumber = -1;

                var numberToName = new Dictionary<int, string>();

                try
                {
                    foreach (string s in Directory.GetFiles(dirName, fileNameMask))
                    {
                        string baseName = Path.GetFileName(s);
                        string number = baseName.Substring(firstPart, baseName.Length - trailerLength - firstPart);
                        int num;

                        try
                        {
                            num = Convert.ToInt32(number, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException)
                        {
                            continue;
                        }

                        nextNumber = Math.Max(nextNumber, num);
                        minNumber = minNumber != -1 ? Math.Min(minNumber, num) : num;

                        numberToName[num] = s;
                    }

                    nextNumber++;
                }
                catch (DirectoryNotFoundException)
                {
                    Directory.CreateDirectory(dirName);
                    nextNumber = 0;
                }

                if (minNumber != -1)
                {
                    int minNumberToKeep = nextNumber - MaxArchiveFiles + 1;
                    for (int i = minNumber; i < minNumberToKeep; ++i)
                    {
                        string s;

                        if (numberToName.TryGetValue(i, out s))
                        {
                            File.Delete(s);
                        }
                    }
                }

                string newFileName = ReplaceNumber(pattern, nextNumber);
                MoveFileToArchive(fileName, newFileName);
            }
        }

        private void MoveFileToArchive(string existingFileName, string archiveFileName)
        {
            File.Move(existingFileName, archiveFileName);
            var fileName = Path.GetFileName(existingFileName);
            if (fileName == null)
            {
                return;
            }

            if (_initializedFiles.ContainsKey(fileName))
            {
                _initializedFiles.Remove(fileName);
            }
            else if (_initializedFiles.ContainsKey(existingFileName))
            {
                _initializedFiles.Remove(existingFileName);
            }
        }

        private void DateArchive(string fileName, string pattern)
        {
            string baseNamePattern = Path.GetFileName(pattern);

            int firstPart = baseNamePattern.IndexOf("{#", StringComparison.Ordinal);
            int lastPart = baseNamePattern.IndexOf("#}", StringComparison.Ordinal) + 2;
            string fileNameMask = baseNamePattern.Substring(0, firstPart) + "*" + baseNamePattern.Substring(lastPart);
            string dirName = Path.GetDirectoryName(Path.GetFullPath(pattern));
            string dateFormat = GetDateFormatString(ArchiveDateFormat);

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dirName);
                List<string> files = directoryInfo.GetFiles(fileNameMask).OrderBy(n => n.CreationTime).Select(n => n.FullName).ToList();
                List<string> filesByDate = new List<string>();

                for (int index = 0; index < files.Count; index++)
                {
                    string archiveFileName = Path.GetFileName(files[index]);
                    string datePart = archiveFileName.Substring(fileNameMask.LastIndexOf('*'), dateFormat.Length);
                    DateTime fileDate = DateTime.MinValue;
                    if (DateTime.TryParseExact(datePart, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out fileDate))
                    {
                        filesByDate.Add(files[index]);
                    }
                }

                for (int fileIndex = 0; fileIndex < filesByDate.Count; fileIndex++)
                {
                    if (fileIndex > files.Count - MaxArchiveFiles)
                    {
                        break;
                    }

                    File.Delete(filesByDate[fileIndex]);
                }
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(dirName);
            }

            DateTime newFileDate = GetArchiveDate();
            string newFileName = Path.Combine(dirName, fileNameMask.Replace("*", newFileDate.ToString(dateFormat)));
            MoveFileToArchive(fileName, newFileName);
        }

        private string GetDateFormatString(string defaultFormat)
        {
            string formatString = defaultFormat;
            if (string.IsNullOrEmpty(formatString))
            {
                switch (ArchiveEvery)
                {
                    case FileArchivePeriod.Year:
                        formatString = "yyyy";
                        break;

                    case FileArchivePeriod.Month:
                        formatString = "yyyyMM";
                        break;

                    default:
                        formatString = "yyyyMMdd";
                        break;

                    case FileArchivePeriod.Hour:
                        formatString = "yyyyMMddHH";
                        break;

                    case FileArchivePeriod.Minute:
                        formatString = "yyyyMMddHHmm";
                        break;
                }
            }
            return formatString;
        }

        private DateTime GetArchiveDate()
        {
            DateTime archiveDate = DateTime.Now;

            switch (ArchiveEvery)
            {
                case FileArchivePeriod.Day:
                    archiveDate = archiveDate.AddDays(-1);
                    break;

                case FileArchivePeriod.Hour:
                    archiveDate = archiveDate.AddHours(-1);
                    break;

                case FileArchivePeriod.Minute:
                    archiveDate = archiveDate.AddMinutes(-1);
                    break;

                case FileArchivePeriod.Month:
                    archiveDate = archiveDate.AddMonths(-1);
                    break;

                case FileArchivePeriod.Year:
                    archiveDate = archiveDate.AddYears(-1);
                    break;
            }

            return archiveDate;
        }

        private void DoAutoArchive(string fileName, LogEventInfo ev)
        {
            var fi = new FileInfo(fileName);
            if (!fi.Exists)
            {
                return;
            }

            string fileNamePattern;

            if (ArchiveFileName == null)
            {
                string ext = Path.GetExtension(fileName);
                fileNamePattern = Path.ChangeExtension(fi.FullName, ".{#}" + ext);
            }
            else
            {
                fileNamePattern = ArchiveFileName.Render(ev);
            }

            if (!IsContainValidNumberPatternForReplacement(fileNamePattern))
            {
                if (_dynamicArchiveFileHandler.AddToArchive(fileNamePattern, fi.FullName, CreateDirs))
                {
                    if (_initializedFiles.ContainsKey(fi.FullName))
                    {
                        _initializedFiles.Remove(fi.FullName);
                    }
                }
            }
            else
            {
                switch (ArchiveNumbering)
                {
                    case ArchiveNumberingMode.Rolling:
                        RecursiveRollingRename(fi.FullName, fileNamePattern, 0);
                        break;

                    case ArchiveNumberingMode.Sequence:
                        SequentialArchive(fi.FullName, fileNamePattern);
                        break;

                    case ArchiveNumberingMode.Date:
                        DateArchive(fi.FullName, fileNamePattern);
                        break;
                }
            }
        }

        private bool ShouldAutoArchive(string fileName, LogEventInfo ev, int upcomingWriteSize)
        {
            if (ArchiveAboveSize == -1 && ArchiveEvery == FileArchivePeriod.None)
            {
                return false;
            }

            DateTime lastWriteTime;
            long fileLength;

            if (!GetFileInfo(fileName, out lastWriteTime, out fileLength))
            {
                return false;
            }

            if (ArchiveAboveSize != -1)
            {
                if (fileLength + upcomingWriteSize > ArchiveAboveSize)
                {
                    return true;
                }
            }

            if (ArchiveEvery != FileArchivePeriod.None)
            {
                string formatString = GetDateFormatString(string.Empty);
                string ts = lastWriteTime.ToString(formatString, CultureInfo.InvariantCulture);
                string ts2 = ev.TimeStamp.ToLocalTime().ToString(formatString, CultureInfo.InvariantCulture);

                if (ts != ts2)
                {
                    return true;
                }
            }

            return false;
        }

        private void AutoClosingTimerCallback(object state)
        {
            lock (SyncRoot)
            {
                if (!IsInitialized)
                {
                    return;
                }

                try
                {
                    DateTime timeToKill = DateTime.Now.AddSeconds(-OpenFileCacheTimeout);
                    for (int i = 0; i < _recentAppenders.Length; ++i)
                    {
                        if (_recentAppenders[i] == null)
                        {
                            break;
                        }

                        if (_recentAppenders[i].OpenTime < timeToKill)
                        {
                            for (int j = i; j < _recentAppenders.Length; ++j)
                            {
                                if (_recentAppenders[j] == null)
                                {
                                    break;
                                }

                                _recentAppenders[j].Close();
                                _recentAppenders[j] = null;
                            }

                            break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }
                }
            }
        }

        private void WriteToFile(string fileName, byte[] bytes, bool justData)
        {
            if (ReplaceFileContentsOnEachWrite)
            {
                using (FileStream fs = File.Create(fileName))
                {
                    byte[] headerBytes = GetHeaderBytes();
                    byte[] footerBytes = GetFooterBytes();

                    if (headerBytes != null)
                    {
                        fs.Write(headerBytes, 0, headerBytes.Length);
                    }

                    fs.Write(bytes, 0, bytes.Length);
                    if (footerBytes != null)
                    {
                        fs.Write(footerBytes, 0, footerBytes.Length);
                    }
                }

                return;
            }

            bool writeHeader = false;

            if (!justData)
            {
                if (!_initializedFiles.ContainsKey(fileName))
                {
                    if (ArchiveOldFileOnStartup)
                    {
                        try
                        {
                            DoAutoArchive(fileName, null);
                        }
                        catch (Exception exception)
                        {
                            if (exception.MustBeRethrown())
                            {
                                throw;
                            }
                        }
                    }
                    if (DeleteOldFileOnStartup)
                    {
                        try
                        {
                            File.Delete(fileName);
                        }
                        catch (Exception exception)
                        {
                            if (exception.MustBeRethrown())
                            {
                                throw;
                            }
                        }
                    }

                    _initializedFiles[fileName] = DateTime.Now;
                    _initializedFilesCounter++;
                    writeHeader = true;

                    if (_initializedFilesCounter >= 100)
                    {
                        _initializedFilesCounter = 0;
                        CleanupInitializedFiles();
                    }
                }

                _initializedFiles[fileName] = DateTime.Now;
            }

            BaseFileAppender appenderToWrite = null;
            int freeSpot = _recentAppenders.Length - 1;

            for (int i = 0; i < _recentAppenders.Length; ++i)
            {
                if (_recentAppenders[i] == null)
                {
                    freeSpot = i;
                    break;
                }

                if (_recentAppenders[i].FileName == fileName)
                {
                    BaseFileAppender app = _recentAppenders[i];
                    for (int j = i; j > 0; --j)
                    {
                        _recentAppenders[j] = _recentAppenders[j - 1];
                    }

                    _recentAppenders[0] = app;
                    appenderToWrite = app;
                    break;
                }
            }

            if (appenderToWrite == null)
            {
                BaseFileAppender newAppender = _appenderFactory.Open(fileName, this);

                if (_recentAppenders[freeSpot] != null)
                {
                    _recentAppenders[freeSpot].Close();
                    _recentAppenders[freeSpot] = null;
                }

                for (int j = freeSpot; j > 0; --j)
                {
                    _recentAppenders[j] = _recentAppenders[j - 1];
                }

                _recentAppenders[0] = newAppender;
                appenderToWrite = newAppender;
            }

            if (writeHeader)
            {
                long fileLength;
                DateTime lastWriteTime;

                if (!appenderToWrite.GetFileInfo(out lastWriteTime, out fileLength) || fileLength == 0)
                {
                    byte[] headerBytes = GetHeaderBytes();
                    if (headerBytes != null)
                    {
                        appenderToWrite.Write(headerBytes);
                    }
                }
            }

            appenderToWrite.Write(bytes);

            if (AutoFlush)
            {
                appenderToWrite.Flush();
            }
        }

        private byte[] GetHeaderBytes()
        {
            if (Header == null)
            {
                return null;
            }

            string renderedText = Header.Render(LogEventInfo.CreateNullEvent()) + NewLineChars;
            return TransformBytes(Encoding.GetBytes(renderedText));
        }

        private byte[] GetFooterBytes()
        {
            if (Footer == null)
            {
                return null;
            }

            string renderedText = Footer.Render(LogEventInfo.CreateNullEvent()) + NewLineChars;
            return TransformBytes(Encoding.GetBytes(renderedText));
        }

        private void WriteFooterAndUninitialize(string fileName)
        {
            byte[] footerBytes = GetFooterBytes();
            if (footerBytes != null)
            {
                if (File.Exists(fileName))
                {
                    WriteToFile(fileName, footerBytes, true);
                }
            }

            _initializedFiles.Remove(fileName);
        }

        private bool GetFileInfo(string fileName, out DateTime lastWriteTime, out long fileLength)
        {
            foreach (BaseFileAppender t in _recentAppenders)
            {
                if (t == null)
                {
                    break;
                }

                if (t.FileName == fileName)
                {
                    t.GetFileInfo(out lastWriteTime, out fileLength);
                    return true;
                }
            }

            var fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                fileLength = fi.Length;
                lastWriteTime = fi.LastWriteTime;
                return true;
            }

            fileLength = -1;
            lastWriteTime = DateTime.MinValue;
            return false;
        }

        private void InvalidateCacheItem(string fileName)
        {
            for (int i = 0; i < _recentAppenders.Length; ++i)
            {
                if (_recentAppenders[i] == null)
                {
                    break;
                }

                if (_recentAppenders[i].FileName == fileName)
                {
                    _recentAppenders[i].Close();
                    for (int j = i; j < _recentAppenders.Length - 1; ++j)
                    {
                        _recentAppenders[j] = _recentAppenders[j + 1];
                    }

                    _recentAppenders[_recentAppenders.Length - 1] = null;
                    break;
                }
            }
        }

        private static string CleanupFileName(string fileName)
        {
            var lastDirSeparator =
                fileName.LastIndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });

            var fileName1 = fileName.Substring(lastDirSeparator + 1);
            var dirName = lastDirSeparator > 0 ? fileName.Substring(0, lastDirSeparator) : string.Empty;
            fileName1 = Path.GetInvalidFileNameChars().Aggregate(fileName1, (current, c) => current.Replace(c, '_'));
            return Path.Combine(dirName, fileName1);
        }

        #endregion

        #region Вложенный класс: DynamicArchiveFileHandlerClass

        private class DynamicArchiveFileHandlerClass
        {
            #region Поля

            private readonly Queue<string> _archiveFileEntryQueue;

            #endregion

            #region Свойства

            public int MaxArchiveFileToKeep { get; set; }

            #endregion

            #region Конструктор

            public DynamicArchiveFileHandlerClass(int maxArchivedFiles)
                : this()
            {
                MaxArchiveFileToKeep = maxArchivedFiles;
            }

            private DynamicArchiveFileHandlerClass()
            {
                MaxArchiveFileToKeep = -1;

                _archiveFileEntryQueue = new Queue<string>();
            }

            #endregion

            #region Методы

            public bool AddToArchive(string archiveFileName, string fileName, bool createDirectoryIfNotExists)
            {
                if (MaxArchiveFileToKeep < 1)
                {
                    return false;
                }

                if (!File.Exists(fileName))
                {
                    return false;
                }

                while (_archiveFileEntryQueue.Count >= MaxArchiveFileToKeep)
                {
                    string oldestArchivedFileName = _archiveFileEntryQueue.Dequeue();

                    try
                    {
                        File.Delete(oldestArchivedFileName);
                    }
                    catch
                    {
                    }
                }

                String archiveFileNamePattern = archiveFileName;

                if (_archiveFileEntryQueue.Contains(archiveFileName))
                {
                    int numberToStartWith = 1;

                    archiveFileNamePattern = Path.GetFileNameWithoutExtension(archiveFileName) + ".{#}" + Path.GetExtension(archiveFileName);

                    while (File.Exists(ReplaceNumber(archiveFileNamePattern, numberToStartWith)))
                    {
                        numberToStartWith++;
                    }
                }

                try
                {
                    File.Move(fileName, archiveFileNamePattern);
                }
                catch (DirectoryNotFoundException)
                {
                    if (createDirectoryIfNotExists)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(archiveFileName));

                        File.Move(fileName, archiveFileNamePattern);
                    }
                    else
                    {
                        throw;
                    }
                }

                _archiveFileEntryQueue.Enqueue(archiveFileName);
                return true;
            }

            #endregion
        }

        #endregion
    }
}