namespace SharpLib.Log
{
    internal abstract class ThreadIdHelper
    {
        #region Свойства

        public static ThreadIdHelper Instance { get; private set; }

        public abstract int CurrentThreadId { get; }

        public abstract int CurrentProcessId { get; }

        public abstract string CurrentProcessName { get; }

        public abstract string CurrentProcessBaseName { get; }

        #endregion

        #region Конструктор

        static ThreadIdHelper()
        {
            if (PlatformDetector.IsWin32)
            {
                Instance = new Win32ThreadIdHelper();
            }
            else
            {
                Instance = new PortableThreadIdHelper();
            }
        }

        #endregion
    }
}