
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    public delegate void AsynchronousAction(AsyncContinuation asyncContinuation);

    public delegate void AsynchronousAction<T>(T argument, AsyncContinuation asyncContinuation);
}
