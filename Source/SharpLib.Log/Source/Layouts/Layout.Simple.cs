
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace SharpLib.Log
{
    [Layout("SimpleLayout")]
    [ThreadAgnostic]
    [AppDomainFixedOutput]
    public class SimpleLayout : Layout
    {
        #region Константы

        private const int MaxInitialRenderBufferLength = 16384;

        #endregion

        #region Поля

        private readonly ConfigurationItemFactory configurationItemFactory;

        private string fixedText;

        private string layoutText;

        private int maxRenderedLength;

        #endregion

        #region Свойства

        public string Text
        {
            get { return layoutText; }

            set
            {
                LayoutRenderer[] renderers;
                string txt;

                renderers = LayoutParser.CompileLayout(
                    configurationItemFactory,
                    new SimpleStringReader(value),
                    false,
                    out txt);

                SetRenderers(renderers, txt);
            }
        }

        public ReadOnlyCollection<LayoutRenderer> Renderers { get; private set; }

        #endregion

        #region Конструктор

        public SimpleLayout()
            : this(string.Empty)
        {
        }

        public SimpleLayout(string txt)
            : this(txt, ConfigurationItemFactory.Default)
        {
        }

        public SimpleLayout(string txt, ConfigurationItemFactory configurationItemFactory)
        {
            this.configurationItemFactory = configurationItemFactory;
            Text = txt;
        }

        internal SimpleLayout(LayoutRenderer[] renderers, string text, ConfigurationItemFactory configurationItemFactory)
        {
            this.configurationItemFactory = configurationItemFactory;
            SetRenderers(renderers, text);
        }

        #endregion

        #region Методы

        public static string Escape(string text)
        {
            return text.Replace("${", "${literal:text=${}");
        }

        public static string Evaluate(string text, LogEventInfo logEvent)
        {
            var l = new SimpleLayout(text);
            return l.Render(logEvent);
        }

        public static string Evaluate(string text)
        {
            return Evaluate(text, LogEventInfo.CreateNullEvent());
        }

        public override string ToString()
        {
            return "'" + Text + "'";
        }

        internal void SetRenderers(LayoutRenderer[] renderers, string text)
        {
            Renderers = new ReadOnlyCollection<LayoutRenderer>(renderers);
            if (Renderers.Count == 1 && Renderers[0] is LiteralLayoutRenderer)
            {
                fixedText = ((LiteralLayoutRenderer)Renderers[0]).Text;
            }
            else
            {
                fixedText = null;
            }

            layoutText = text;
        }

        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            if (fixedText != null)
            {
                return fixedText;
            }

            string cachedValue;

            if (logEvent.TryGetCachedLayoutValue(this, out cachedValue))
            {
                return cachedValue;
            }

            int initialSize = maxRenderedLength;
            if (initialSize > MaxInitialRenderBufferLength)
            {
                initialSize = MaxInitialRenderBufferLength;
            }

            var builder = new StringBuilder(initialSize);

            foreach (LayoutRenderer renderer in Renderers)
            {
                try
                {
                    renderer.Render(builder, logEvent);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                }
            }

            if (builder.Length > maxRenderedLength)
            {
                maxRenderedLength = builder.Length;
            }

            string value = builder.ToString();
            logEvent.AddCachedLayoutValue(this, value);
            return value;
        }

        #endregion

        public static implicit operator SimpleLayout(string text)
        {
            return new SimpleLayout(text);
        }
    }
}
