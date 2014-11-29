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
using System.Text;

using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers
{
    /// <summary>
    /// Render environmental information related to logging events.
    /// </summary>
    [NLogConfigurationItem]
    public abstract class LayoutRenderer : ISupportsInitialize, IRenderable, IDisposable
    {
        #region ���������

        private const int MaxInitialRenderBufferLength = 16384;

        #endregion

        #region ����

        private bool isInitialized;

        private int maxRenderedLength;

        #endregion

        #region ��������

        /// <summary>
        /// Gets the logging configuration this target is part of.
        /// </summary>
        protected LoggingConfiguration LoggingConfiguration { get; private set; }

        #endregion

        #region ������

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var lra = (LayoutRendererAttribute)Attribute.GetCustomAttribute(GetType(), typeof(LayoutRendererAttribute));
            if (lra != null)
            {
                return "Layout Renderer: ${" + lra.Name + "}";
            }

            return GetType().Name;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Renders the the value of layout renderer in the context of the specified log event.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>String representation of a layout renderer.</returns>
        public string Render(LogEventInfo logEvent)
        {
            int initialLength = maxRenderedLength;
            if (initialLength > MaxInitialRenderBufferLength)
            {
                initialLength = MaxInitialRenderBufferLength;
            }

            var builder = new StringBuilder(initialLength);

            Render(builder, logEvent);
            if (builder.Length > maxRenderedLength)
            {
                maxRenderedLength = builder.Length;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        void ISupportsInitialize.Initialize(LoggingConfiguration configuration)
        {
            Initialize(configuration);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void ISupportsInitialize.Close()
        {
            Close();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        internal void Initialize(LoggingConfiguration configuration)
        {
            if (!isInitialized)
            {
                LoggingConfiguration = configuration;
                isInitialized = true;
                InitializeLayoutRenderer();
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            if (isInitialized)
            {
                LoggingConfiguration = null;
                isInitialized = false;
                CloseLayoutRenderer();
            }
        }

        internal void Render(StringBuilder builder, LogEventInfo logEvent)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                InitializeLayoutRenderer();
            }

            try
            {
                Append(builder, logEvent);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Warn("Exception in layout renderer: {0}", exception);
            }
        }

        /// <summary>
        /// Renders the specified environmental information and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected abstract void Append(StringBuilder builder, LogEventInfo logEvent);

        /// <summary>
        /// Initializes the layout renderer.
        /// </summary>
        protected virtual void InitializeLayoutRenderer()
        {
        }

        /// <summary>
        /// Closes the layout renderer.
        /// </summary>
        protected virtual void CloseLayoutRenderer()
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        #endregion
    }
}