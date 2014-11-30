using System;
using System.Collections.Generic;
using System.IO;

namespace SharpLib.Log
{
    internal class MultiFileWatcher : IDisposable
    {
        #region Поля

        private readonly List<FileSystemWatcher> _watchers;

        #endregion

        #region События

        public event EventHandler OnChange;

        #endregion

        #region Конструктор

        public MultiFileWatcher()
        {
            _watchers = new List<FileSystemWatcher>();
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            StopWatching();
            GC.SuppressFinalize(this);
        }

        public void StopWatching()
        {
            lock (this)
            {
                foreach (FileSystemWatcher watcher in _watchers)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }

                _watchers.Clear();
            }
        }

        public void Watch(IEnumerable<string> fileNames)
        {
            if (fileNames == null)
            {
                return;
            }

            foreach (string s in fileNames)
            {
                Watch(s);
            }
        }

        internal void Watch(string fileName)
        {
            var watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(fileName),
                Filter = Path.GetFileName(fileName),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.Security | NotifyFilters.Attributes
            };

            watcher.Created += OnWatcherChanged;
            watcher.Changed += OnWatcherChanged;
            watcher.Deleted += OnWatcherChanged;
            watcher.EnableRaisingEvents = true;

            lock (this)
            {
                _watchers.Add(watcher);
            }
        }

        private void OnWatcherChanged(object source, FileSystemEventArgs e)
        {
            lock (this)
            {
                if (OnChange != null)
                {
                    OnChange(source, e);
                }
            }
        }

        #endregion
    }
}