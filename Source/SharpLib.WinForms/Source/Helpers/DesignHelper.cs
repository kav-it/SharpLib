using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace SharpLib.WinForms
{
    /// <summary>
    /// Класс-помошник взаимодействия разработываемого кода с VisualStudio
    /// </summary>
    public static class DesignHelper
    {
        #region Поля

        /// <summary>
        /// Признак выполнения кода в режиме дизайнера
        /// </summary>
        private static readonly bool _isDesigntime;

        #endregion

        #region Свойства

        /// <summary>
        /// Признак выполнения кода в режиме дизайнера
        /// </summary>
        public static bool IsDesigntime
        {
            get { return _isDesigntime; }
        }

        /// <summary>
        /// Признак выполнения кода не в режиме дизайнера
        /// </summary>
        public static bool IsRuntime
        {
            get { return !IsDesigntime; }
        }

        #endregion

        #region Конструктор

        static DesignHelper()
        {
            // Подробности: http://dotnetfacts.blogspot.ru/2009/01/identifying-run-time-and-design-mode.html
            _isDesigntime = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

            if (_isDesigntime)
            {
                // Определен режим "Design". Дальнейший анализ не требуется
                return;
            }

            var designerHosts = new List<string>
            {
                "devenv",
                "vcsexpress",
                "vbexpress",
                "vcexpress",
                "sharpdevelop"
            };

            using (var process = Process.GetCurrentProcess())
            {
                var processName = process.ProcessName.ToLower();
                _isDesigntime = designerHosts.Contains(processName);
            }

        }

        #endregion
    }
}