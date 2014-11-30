using System;
using System.Collections.Generic;

namespace SharpLib.Log
{
    public interface IAppDomain
    {
        string BaseDirectory { get; }

        string ConfigurationFile { get; }

        IEnumerable<string> PrivateBinPath { get; }

        string FriendlyName { get; }

        event EventHandler<EventArgs> ProcessExit;

        event EventHandler<EventArgs> DomainUnload;
    }
}