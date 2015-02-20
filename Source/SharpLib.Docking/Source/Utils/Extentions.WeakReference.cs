using System;

namespace SharpLib.Docking
{
    internal static class Extensions
    {
        #region Методы

        public static T GetValueOrDefault<T>(this WeakReference self)
        {
            if (self == null || !self.IsAlive)
            {
                return default(T);
            }
            return (T)self.Target;
        }

        #endregion
    }
}