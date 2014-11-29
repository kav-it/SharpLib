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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Internal.Fakeables;
using NLog.Layouts;

#if !SILVERLIGHT

namespace NLog.Targets
{
    /// <summary>
    /// Writes log message to the Event Log.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/EventLog_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    /// To set up the target in the <a href="config.html">configuration file</a>,
    /// use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/EventLog/NLog.config" />
    ///     <p>
    /// This assumes just one target and a single rule. More configuration
    /// options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    /// To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/EventLog/Simple/Example.cs" />
    /// </example>
    [Target("EventLog")]
    public class EventLogTarget : TargetWithLayout, IInstallable
    {
        #region ����

        private EventLog eventLogInstance;

        #endregion

        #region ��������

        /// <summary>
        /// Gets or sets the name of the machine on which Event Log service is running.
        /// </summary>
        /// <docgen category='Event Log Options' order='10' />
        [DefaultValue(".")]
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the layout that renders event ID.
        /// </summary>
        /// <docgen category='Event Log Options' order='10' />
        public Layout EventId { get; set; }

        /// <summary>
        /// Gets or sets the layout that renders event Category.
        /// </summary>
        /// <docgen category='Event Log Options' order='10' />
        public Layout Category { get; set; }

        /// <summary>
        /// Gets or sets the value to be used as the event Source.
        /// </summary>
        /// <remarks>
        /// By default this is the friendly name of the current AppDomain.
        /// </remarks>
        /// <docgen category='Event Log Options' order='10' />
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the name of the Event Log to write to. This can be System, Application or
        /// any user-defined name.
        /// </summary>
        /// <docgen category='Event Log Options' order='10' />
        [DefaultValue("Application")]
        public string Log { get; set; }

        #endregion

        #region �����������

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogTarget" /> class.
        /// </summary>
        public EventLogTarget() : this(AppDomainWrapper.CurrentDomain)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogTarget" /> class.
        /// </summary>
        public EventLogTarget(IAppDomain appDomain)
        {
            Source = appDomain.FriendlyName;
            Log = "Application";
            MachineName = ".";
        }

        #endregion

        #region ������

        /// <summary>
        /// Performs installation which requires administrative permissions.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        public void Install(InstallationContext installationContext)
        {
            if (EventLog.SourceExists(Source, MachineName))
            {
                string currentLogName = EventLog.LogNameFromSourceName(Source, MachineName);
                if (!currentLogName.Equals(Log, StringComparison.CurrentCultureIgnoreCase))
                {
                    // re-create the association between Log and Source
                    EventLog.DeleteEventSource(Source, MachineName);

                    var escd = new EventSourceCreationData(Source, Log)
                    {
                        MachineName = MachineName
                    };

                    EventLog.CreateEventSource(escd);
                }
            }
            else
            {
                var escd = new EventSourceCreationData(Source, Log)
                {
                    MachineName = MachineName
                };

                EventLog.CreateEventSource(escd);
            }
        }

        /// <summary>
        /// Performs uninstallation which requires administrative permissions.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        public void Uninstall(InstallationContext installationContext)
        {
            EventLog.DeleteEventSource(Source, MachineName);
        }

        /// <summary>
        /// Determines whether the item is installed.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        /// <returns>
        /// Value indicating whether the item is installed or null if it is not possible to determine.
        /// </returns>
        public bool? IsInstalled(InstallationContext installationContext)
        {
            return EventLog.SourceExists(Source, MachineName);
        }

        /// <summary>
        /// Initializes the target.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            var s = EventLog.LogNameFromSourceName(Source, MachineName);
            if (!s.Equals(Log, StringComparison.CurrentCultureIgnoreCase))
            {
                CreateEventSourceIfNeeded();
            }
        }

        /// <summary>
        /// Writes the specified logging event to the event log.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            string message = Layout.Render(logEvent);
            if (message.Length > 16384)
            {
                // limitation of EventLog API
                message = message.Substring(0, 16384);
            }

            EventLogEntryType entryType;

            if (logEvent.Level >= LogLevel.Error)
            {
                entryType = EventLogEntryType.Error;
            }
            else if (logEvent.Level >= LogLevel.Warn)
            {
                entryType = EventLogEntryType.Warning;
            }
            else
            {
                entryType = EventLogEntryType.Information;
            }

            int eventId = 0;

            if (EventId != null)
            {
                eventId = Convert.ToInt32(EventId.Render(logEvent), CultureInfo.InvariantCulture);
            }

            short category = 0;

            if (Category != null)
            {
                category = Convert.ToInt16(Category.Render(logEvent), CultureInfo.InvariantCulture);
            }

            var eventLog = GetEventLog();
            eventLog.WriteEntry(message, entryType, eventId, category);
        }

        private EventLog GetEventLog()
        {
            return eventLogInstance ?? (eventLogInstance = new EventLog(Log, MachineName, Source));
        }

        private void CreateEventSourceIfNeeded()
        {
            // if we throw anywhere, we remain non-operational
            try
            {
                if (EventLog.SourceExists(Source, MachineName))
                {
                    string currentLogName = EventLog.LogNameFromSourceName(Source, MachineName);
                    if (!currentLogName.Equals(Log, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // re-create the association between Log and Source
                        EventLog.DeleteEventSource(Source, MachineName);
                        var escd = new EventSourceCreationData(Source, Log)
                        {
                            MachineName = MachineName
                        };

                        EventLog.CreateEventSource(escd);
                    }
                }
                else
                {
                    var escd = new EventSourceCreationData(Source, Log)
                    {
                        MachineName = MachineName
                    };

                    EventLog.CreateEventSource(escd);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("Error when connecting to EventLog: {0}", exception);
                throw;
            }
        }

        #endregion
    }
}

#endif