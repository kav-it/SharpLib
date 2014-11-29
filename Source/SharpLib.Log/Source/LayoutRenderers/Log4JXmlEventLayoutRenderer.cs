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
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

using NLog.Config;
using NLog.Internal;
using NLog.Internal.Fakeables;
using NLog.Targets;

namespace NLog.LayoutRenderers
{
    /// <summary>
    /// XML event description compatible with log4j, Chainsaw and NLogViewer.
    /// </summary>
    [LayoutRenderer("log4jxmlevent")]
    public class Log4JXmlEventLayoutRenderer : LayoutRenderer, IUsesStackTrace
    {
        #region Поля

        private static readonly string dummyNLogNamespace = "http://nlog-project.org/dummynamespace/" + Guid.NewGuid();

        private static readonly string dummyNamespace = "http://nlog-project.org/dummynamespace/" + Guid.NewGuid();

        private static readonly DateTime log4jDateBase = new DateTime(1970, 1, 1);

        #endregion

        #region Свойства

        /// <summary>
        /// Gets or sets a value indicating whether to include NLog-specific extensions to log4j schema.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        [DefaultValue(true)]
        public bool IncludeNLogData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the XML should use spaces for indentation.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IndentXml { get; set; }

        /// <summary>
        /// Gets or sets the AppInfo field. By default it's the friendly name of the current AppDomain.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public string AppInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include call site (class and method name) in the information sent over the
        /// network.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeCallSite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include source info (file name and line number) in the information sent over
        /// the network.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeSourceInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include contents of the <see cref="MappedDiagnosticsContext" /> dictionary.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeMdc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include contents of the <see cref="NestedDiagnosticsContext" /> stack.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeNdc { get; set; }

        /// <summary>
        /// Gets or sets the NDC item separator.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        [DefaultValue(" ")]
        public string NdcItemSeparator { get; set; }

        /// <summary>
        /// Gets the level of stack trace information required by the implementing class.
        /// </summary>
        StackTraceUsage IUsesStackTrace.StackTraceUsage
        {
            get
            {
                if (IncludeSourceInfo)
                {
                    return StackTraceUsage.Max;
                }

                if (IncludeCallSite)
                {
                    return StackTraceUsage.WithoutSource;
                }

                return StackTraceUsage.None;
            }
        }

        internal IList<NLogViewerParameterInfo> Parameters { get; set; }

        #endregion

        #region Конструктор

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4JXmlEventLayoutRenderer" /> class.
        /// </summary>
        public Log4JXmlEventLayoutRenderer() : this(AppDomainWrapper.CurrentDomain)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4JXmlEventLayoutRenderer" /> class.
        /// </summary>
        public Log4JXmlEventLayoutRenderer(IAppDomain appDomain)
        {
            IncludeNLogData = true;
            NdcItemSeparator = " ";

#if SILVERLIGHT
            this.AppInfo = "Silverlight Application";
#else
            AppInfo = string.Format(
                CultureInfo.InvariantCulture,
                "{0}({1})",
                appDomain.FriendlyName,
                ThreadIDHelper.Instance.CurrentProcessID);
#endif

            Parameters = new List<NLogViewerParameterInfo>();
        }

        #endregion

        #region Методы

        internal void AppendToStringBuilder(StringBuilder sb, LogEventInfo logEvent)
        {
            Append(sb, logEvent);
        }

        /// <summary>
        /// Renders the XML logging event and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var settings = new XmlWriterSettings
            {
                Indent = IndentXml,
                ConformanceLevel = ConformanceLevel.Fragment,
                IndentChars = "  ",
            };

            var sb = new StringBuilder();
            using (XmlWriter xtw = XmlWriter.Create(sb, settings))
            {
                xtw.WriteStartElement("log4j", "event", dummyNamespace);
                xtw.WriteAttributeSafeString("xmlns", "nlog", null, dummyNLogNamespace);
                xtw.WriteAttributeSafeString("logger", logEvent.LoggerName);
                xtw.WriteAttributeSafeString("level", logEvent.Level.Name.ToUpper(CultureInfo.InvariantCulture));
                xtw.WriteAttributeSafeString("timestamp", Convert.ToString((long)(logEvent.TimeStamp.ToUniversalTime() - log4jDateBase).TotalMilliseconds, CultureInfo.InvariantCulture));
                xtw.WriteAttributeSafeString("thread", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));

                xtw.WriteElementSafeString("log4j", "message", dummyNamespace, logEvent.FormattedMessage);
                if (logEvent.Exception != null)
                {
                    xtw.WriteElementSafeString("log4j", "throwable", dummyNamespace, logEvent.Exception.ToString());
                }

                if (IncludeNdc)
                {
                    xtw.WriteElementSafeString("log4j", "NDC", dummyNamespace, string.Join(NdcItemSeparator, NestedDiagnosticsContext.GetAllMessages()));
                }

                if (logEvent.Exception != null)
                {
                    xtw.WriteStartElement("log4j", "throwable", dummyNamespace);
                    xtw.WriteSafeCData(logEvent.Exception.ToString());
                    xtw.WriteEndElement();
                }

                if (IncludeCallSite || IncludeSourceInfo)
                {
                    System.Diagnostics.StackFrame frame = logEvent.UserStackFrame;
                    if (frame != null)
                    {
                        MethodBase methodBase = frame.GetMethod();
                        Type type = methodBase.DeclaringType;

                        xtw.WriteStartElement("log4j", "locationInfo", dummyNamespace);
                        if (type != null)
                        {
                            xtw.WriteAttributeSafeString("class", type.FullName);
                        }

                        xtw.WriteAttributeSafeString("method", methodBase.ToString());
#if !SILVERLIGHT
                        if (IncludeSourceInfo)
                        {
                            xtw.WriteAttributeSafeString("file", frame.GetFileName());
                            xtw.WriteAttributeSafeString("line", frame.GetFileLineNumber().ToString(CultureInfo.InvariantCulture));
                        }
#endif
                        xtw.WriteEndElement();

                        if (IncludeNLogData)
                        {
                            xtw.WriteElementSafeString("nlog", "eventSequenceNumber", dummyNLogNamespace, logEvent.SequenceID.ToString(CultureInfo.InvariantCulture));
                            xtw.WriteStartElement("nlog", "locationInfo", dummyNLogNamespace);
                            if (type != null)
                            {
                                xtw.WriteAttributeSafeString("assembly", type.Assembly.FullName);
                            }

                            xtw.WriteEndElement();

                            xtw.WriteStartElement("nlog", "properties", dummyNLogNamespace);
                            foreach (var contextProperty in logEvent.Properties)
                            {
                                xtw.WriteStartElement("nlog", "data", dummyNLogNamespace);
                                xtw.WriteAttributeSafeString("name", Convert.ToString(contextProperty.Key, CultureInfo.InvariantCulture));
                                xtw.WriteAttributeSafeString("value", Convert.ToString(contextProperty.Value, CultureInfo.InvariantCulture));
                                xtw.WriteEndElement();
                            }
                            xtw.WriteEndElement();
                        }
                    }
                }

                xtw.WriteStartElement("log4j", "properties", dummyNamespace);
                if (IncludeMdc)
                {
                    foreach (KeyValuePair<string, string> entry in MappedDiagnosticsContext.ThreadDictionary)
                    {
                        xtw.WriteStartElement("log4j", "data", dummyNamespace);
                        xtw.WriteAttributeSafeString("name", entry.Key);
                        xtw.WriteAttributeSafeString("value", entry.Value);
                        xtw.WriteEndElement();
                    }
                }

                foreach (NLogViewerParameterInfo parameter in Parameters)
                {
                    xtw.WriteStartElement("log4j", "data", dummyNamespace);
                    xtw.WriteAttributeSafeString("name", parameter.Name);
                    xtw.WriteAttributeSafeString("value", parameter.Layout.Render(logEvent));
                    xtw.WriteEndElement();
                }

                xtw.WriteStartElement("log4j", "data", dummyNamespace);
                xtw.WriteAttributeSafeString("name", "log4japp");
                xtw.WriteAttributeSafeString("value", AppInfo);
                xtw.WriteEndElement();

                xtw.WriteStartElement("log4j", "data", dummyNamespace);
                xtw.WriteAttributeSafeString("name", "log4jmachinename");

#if SILVERLIGHT
            xtw.WriteAttributeSafeString("value", "silverlight");
#else
                xtw.WriteAttributeSafeString("value", Environment.MachineName);
#endif
                xtw.WriteEndElement();
                xtw.WriteEndElement();

                xtw.WriteEndElement();
                xtw.Flush();

                // get rid of 'nlog' and 'log4j' namespace declarations
                sb.Replace(" xmlns:log4j=\"" + dummyNamespace + "\"", string.Empty);
                sb.Replace(" xmlns:nlog=\"" + dummyNLogNamespace + "\"", string.Empty);

                builder.Append(sb);
            }
        }

        #endregion
    }
}