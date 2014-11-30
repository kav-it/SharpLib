
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    public enum AsyncTargetWrapperOverflowAction
    {
        Grow,

        Discard,

        Block,
    }
}
