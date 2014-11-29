// 
// Copyright (c) 2004-2011 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections.Generic;
using System.IO;

using NLog.Common;

#if !SILVERLIGHT

namespace NLog.Internal
{
    /// <summary>
    /// Watches multiple files at the same time and raises an event whenever
    /// a single change is detected in any of those files.
    /// </summary>
    internal class MultiFileWatcher : IDisposable
    {
        #region Поля

        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        #endregion

        #region События

        /// <summary>
        /// Occurs when a change is detected in one of the monitored files.
        /// </summary>
        public event EventHandler OnChange;

        #endregion

        #region Методы

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            StopWatching();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Stops the watching.
        /// </summary>
        public void StopWatching()
        {
            lock (this)
            {
                foreach (FileSystemWatcher watcher in watchers)
                {
                    InternalLogger.Info("Stopping file watching for path '{0}' filter '{1}'", watcher.Path, watcher.Filter);
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }

                watchers.Clear();
            }
        }

        /// <summary>
        /// Watches the specified files for changes.
        /// </summary>
        /// <param name="fileNames">The file names.</param>
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Watcher is released in Dispose()")]
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
            InternalLogger.Info("Watching path '{0}' filter '{1}' for changes.", watcher.Path, watcher.Filter);

            lock (this)
            {
                watchers.Add(watcher);
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

#endif