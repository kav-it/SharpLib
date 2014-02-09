// ****************************************************************************
//
// Имя файла    : 'Tracer.cs'
// Заголовок    : Модуль трассировки всех потоков данных внутри системы
// Автор        : Крыцкий А.В./Тихомиров В.С.
// Контакты     : kav.it@mail.ru
// Дата         : 11/01/2012
//
// ****************************************************************************

using System;
using System.Diagnostics;

using SharpLib.Log;

namespace SharpLib
{

    #region Класс Tracer

    public static class Tracer
    {
        #region Поля

        private static Logger _logger;

        #endregion

        #region Свойства

        public static bool EnableFile { get; set; }

        #endregion

        #region Конструктор

        static Tracer()
        {
            InitLogger();
        }

        #endregion

        #region Инициализация

        private static void InitLogger()
        {
            String loggerName = ProgramBase.Location.ExePathWithoutExt + ".log";
            _logger = LogEngine.GetLogger(loggerName);
        }

        #endregion Инициализация

        #region Отладка

        public static void DebugRaw(String text)
        {
#if __DEBUG__
            Debug.Write(text);
#endif
        }

        public static void DebugLine(String text, Boolean withTime = true)
        {
            text = String.Format("\r\n[{0}] {1}", Time.NowToStr(TimeStampFormat.TimeMSec), text);
            DebugRaw(text);
        }

        private static void TraceFile(String text)
        {
#warning Убрать
            // Запись в файл
            //if (EnableFile)
            //    _logger.Write(text);
        }

        private static void Trace(String text)
        {
            // Вывод в отладку Visual Studio
            DebugRaw(text);
            // Запись в файл
            TraceFile(text);
        }

        private static void TraceLine(String text)
        {
            Trace(text + "\r\n");
        }

        public static void TraceMod(LogLevel level, String mod, int modIndex, String text)
        {
            text = FormatMessage(level, mod, modIndex, text, true);

            TraceLine(text);
        }

        #endregion Отладка

        #region Формирование отладочных сообщений

        private static String FormatModuleName(String mod)
        {
            if (mod == null)
                mod = String.Copy("");
            else if (mod != String.Empty)
            {
                if (mod.Length > 8) mod = mod.Substring(0, 8);
                mod = String.Format("[{0,-8}]", mod);
            }
            return mod;
        }

        private static String FormatModuleIndex(int modIndex)
        {
            if (modIndex != 0)
                return String.Format("[{0,-4}]", ((UInt16)modIndex).ToStringEx(16));

            return null;
        }

        private static String LogLevelToStr(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return "D";
                case LogLevel.Info:
                    return "I";
                case LogLevel.Warn:
                    return "W";
                case LogLevel.Error:
                    return "E";
                case LogLevel.In:
                    return "<";
                case LogLevel.Out:
                    return ">";
            }
            return "";
        }

        public static String FormatMessage(LogLevel level, String mod, int modIndex, String message, Boolean addTime)
        {
            mod = FormatModuleName(mod);
            String modIndexText = FormatModuleIndex(modIndex);

            if (addTime)
                message = String.Format("[{0}][{1}]{2}{3} {4}", Time.NowToStr(), LogLevelToStr(level), mod, modIndexText, message);
            else
                message = String.Format("[{0}]{1}{2} {3}", LogLevelToStr(level), mod, modIndexText, message);

            return message;
        }

        #endregion Формирование отладочных сообщений
    }

    #endregion Класс Tracer
}