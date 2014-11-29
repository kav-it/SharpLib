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
using System.IO;
using System.Security;

namespace NLog.Internal.FileAppenders
{
    /// <summary>
    /// Implementation of <see cref="BaseFileAppender" /> which caches
    /// file information.
    /// </summary>
    [SecuritySafeCritical]
    internal class CountingSingleProcessFileAppender : BaseFileAppender
    {
        #region ����

        public static readonly IFileAppenderFactory TheFactory = new Factory();

        private long currentFileLength;

        private FileStream file;

        #endregion

        #region �����������

        /// <summary>
        /// Initializes a new instance of the <see cref="CountingSingleProcessFileAppender" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="parameters">The parameters.</param>
        public CountingSingleProcessFileAppender(string fileName, ICreateFileParameters parameters)
            : base(fileName, parameters)
        {
            var fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                FileTouched(fi.LastWriteTime);
                currentFileLength = fi.Length;
            }
            else
            {
                FileTouched();
                currentFileLength = 0;
            }

            file = CreateFileStream(false);
        }

        #endregion

        #region ������

        /// <summary>
        /// Closes this instance of the appender.
        /// </summary>
        public override void Close()
        {
            if (file != null)
            {
                file.Close();
                file = null;
            }
        }

        /// <summary>
        /// Flushes this current appender.
        /// </summary>
        public override void Flush()
        {
            if (file == null)
            {
                return;
            }

            file.Flush();
            FileTouched();
        }

        /// <summary>
        /// Gets the file info.
        /// </summary>
        /// <param name="lastWriteTime">The last write time.</param>
        /// <param name="fileLength">Length of the file.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        public override bool GetFileInfo(out DateTime lastWriteTime, out long fileLength)
        {
            lastWriteTime = LastWriteTime;
            fileLength = currentFileLength;
            return true;
        }

        /// <summary>
        /// Writes the specified bytes to a file.
        /// </summary>
        /// <param name="bytes">The bytes to be written.</param>
        public override void Write(byte[] bytes)
        {
            if (file == null)
            {
                return;
            }

            currentFileLength += bytes.Length;
            file.Write(bytes, 0, bytes.Length);
            FileTouched();
        }

        #endregion

        #region ��������� �����: Factory

        /// <summary>
        /// Factory class which creates <see cref="CountingSingleProcessFileAppender" /> objects.
        /// </summary>
        private class Factory : IFileAppenderFactory
        {
            #region ������

            /// <summary>
            /// Opens the appender for given file name and parameters.
            /// </summary>
            /// <param name="fileName">Name of the file.</param>
            /// <param name="parameters">Creation parameters.</param>
            /// <returns>
            /// Instance of <see cref="BaseFileAppender" /> which can be used to write to the file.
            /// </returns>
            BaseFileAppender IFileAppenderFactory.Open(string fileName, ICreateFileParameters parameters)
            {
                return new CountingSingleProcessFileAppender(fileName, parameters);
            }

            #endregion
        }

        #endregion
    }
}