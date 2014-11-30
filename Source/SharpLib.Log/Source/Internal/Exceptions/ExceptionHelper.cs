
using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpLib.Log
{
    internal static class ExceptionHelper
    {
        #region Методы

        public static bool MustBeRethrown(this Exception exception)
        {
            if (exception is StackOverflowException)
            {
                return true;
            }

            if (exception is ThreadAbortException)
            {
                return true;
            }

            if (exception is OutOfMemoryException)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
