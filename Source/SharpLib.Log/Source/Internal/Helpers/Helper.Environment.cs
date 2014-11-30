using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;

namespace SharpLib.Log
{
    internal static class EnvironmentHelper
    {
        #region Свойства

        internal static string NewLine
        {
            get
            {
                string newline = Environment.NewLine;

                return newline;
            }
        }

        #endregion

        #region Методы

        internal static string GetSafeEnvironmentVariable(string name)
        {
            try
            {
                string s = Environment.GetEnvironmentVariable(name);

                if (string.IsNullOrEmpty(s))
                {
                    return null;
                }

                return s;
            }
            catch (SecurityException)
            {
                return string.Empty;
            }
        }

        #endregion
    }
}