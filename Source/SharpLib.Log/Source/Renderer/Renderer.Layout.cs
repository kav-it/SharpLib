using System;
using System.Text;

namespace SharpLib.Log
{
    [LogConfigurationItem]
    public abstract class LayoutRenderer : ISupportsInitialize, IRenderable, IDisposable
    {
        #region Константы

        private const int MaxInitialRenderBufferLength = 16384;

        #endregion

        #region Поля

        private bool isInitialized;

        private int maxRenderedLength;

        #endregion

        #region Свойства

        protected LoggingConfiguration LoggingConfiguration { get; private set; }

        #endregion

        #region Методы

        public override string ToString()
        {
            var lra = (LayoutRendererAttribute)Attribute.GetCustomAttribute(GetType(), typeof(LayoutRendererAttribute));
            if (lra != null)
            {
                return "Layout Renderer: ${" + lra.Name + "}";
            }

            return GetType().Name;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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

        void ISupportsInitialize.Initialize(LoggingConfiguration configuration)
        {
            Initialize(configuration);
        }

        void ISupportsInitialize.Close()
        {
            Close();
        }

        internal void Initialize(LoggingConfiguration configuration)
        {
            if (!isInitialized)
            {
                LoggingConfiguration = configuration;
                isInitialized = true;
                InitializeLayoutRenderer();
            }
        }

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
            }
        }

        protected abstract void Append(StringBuilder builder, LogEventInfo logEvent);

        protected virtual void InitializeLayoutRenderer()
        {
        }

        protected virtual void CloseLayoutRenderer()
        {
        }

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