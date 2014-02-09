// ****************************************************************************
//
// Имя файла    : 'Logger.cs'
// Заголовок    : Реализация записи лог-файлов
// Автор        : Тихомиров В.С./Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{

    #region Перечисление LogLevel

    [Flags]
    public enum LogLevel
    {
        Off = (0 << 0),

        Debug = (1 << 0),

        Info = (1 << 1),

        Warn = (1 << 2),

        Error = (1 << 3),

        In = (1 << 10),

        Out = (1 << 11),

        On = (Debug | Info | Warn | Error),

        All = (On | In | Out)
    }

    #endregion Перечисление LogLevel

    #region Класс Logger

    public class Logger
    {
        #region Свойства

        /// <summary>
        /// Класс ядра логгирования (внутреннее использование)
        /// </summary>
        private LogEngine Engine { get; set; }

        /// <summary>
        /// Текущий уровень отладки
        /// </summary>
        private LogLevel CurrentLevel { get; set; }

        /// <summary>
        /// Имя модуля (назначается приложением)
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Индекс модуля (назначается приложением)
        /// </summary>
        public int Index { get; set; }

        #endregion

        #region Конструктор

        protected internal Logger()
        {
        }

        #endregion

        #region Инициализация

        internal void Init(LogEngine engine, String name, int index)
        {
            Engine = engine;
            Name = name;
            Index = index;
        }

        internal void UpdateConfig()
        {
            CurrentLevel = Engine.GetLevelByModule(Name);
        }

        #endregion Инициализация

        #region Вспомогательные методы

        private void WriteLog(LogLevel level, int index, String message, params Object[] args)
        {
            LogLevel mask = CurrentLevel & level;

            if (mask != LogLevel.Off)
            {
                LogMessage logMessage = new LogMessage(level, Name, index, message, args);

                Engine.WriteLog(logMessage);
            }
        }

        #endregion Вспомогательные методы

        #region Вывод в отладку

        public void Debug(String message, params Object[] args)
        {
            WriteLog(LogLevel.Debug, Index, message, args);
        }

        public void Info(String message, params Object[] args)
        {
            WriteLog(LogLevel.Info, Index, message, args);
        }

        public void Warn(String message, params Object[] args)
        {
            WriteLog(LogLevel.Warn, Index, message, args);
        }

        public void Error(String message, params Object[] args)
        {
            WriteLog(LogLevel.Error, Index, message, args);
        }

        #endregion Вывод в отладку
    }

    #endregion Класс Logger

    #region Класс LogMessage

    public class LogMessage
    {
        #region Константы

        /// <summary>
        /// Сигнатура пакета
        /// </summary>
        public const UInt32 SIGN_NET_SEND = 0xAA4C534F; // 'L', 'O', 'S', 0xAA

        /// <summary>
        /// Минимальный размер пакета
        /// </summary>
        public const UInt32 MIN_NET_SIZE = 21;

        #endregion

        #region Поля

        private static int _globalSequenceId;

        #endregion

        #region Свойства

        /// <summary>
        /// Уникальный номер сообщения
        /// </summary>
        public int SequenceId { get; private set; }

        /// <summary>
        /// Время формирования сообщения
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Уровень сообщения
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Имя модуля сформировавшего сообщение
        /// </summary>
        public String ModuleName { get; set; }

        /// <summary>
        /// Идентификатор модуля сформировавшего сообщение
        /// </summary>
        public int ModuleIndex { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Дополнительные параметры сообщения
        /// </summary>
        public Object[] Parameters { get; set; }

        #endregion

        #region Конструктор

        public LogMessage()
        {
        }

        public LogMessage(LogLevel level, String moduleName, int index, String message) : this(level, moduleName, index, message, null)
        {
        }

        public LogMessage(LogLevel level, String moduleName, int index, String message, Object[] parameters)
        {
            TimeStamp = DateTime.Now;
            Level = level;
            ModuleName = moduleName;
            ModuleIndex = index;
            Message = message;
            Parameters = parameters;
            SequenceId = Interlocked.Increment(ref _globalSequenceId);
        }

        #endregion

        #region Преобразования

        private String FormatTime(DateTime time)
        {
            String result = time.ToString(@"yyyy-MM-dd HH\:mm\:ss.fff");

            return result;
        }

        private String FormatModuleName(String mod)
        {
            if (mod == null) mod = "";
            else if (mod != String.Empty)
            {
                if (mod.Length > 12) mod = mod.Substring(0, 12);
                mod = String.Format("[{0,-12}]", mod);
            }
            return mod;
        }

        private String FormatIndex(int modIndex)
        {
            if (modIndex != 0)
                return String.Format("[{0}]", ((UInt16)modIndex).ToStringEx(16));

            return null;
        }

        private String FormatLevel(LogLevel level)
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
            return "-";
        }

        public String ToText()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("[{0}] [{1}] {2}{3} {4}",
                                 FormatTime(TimeStamp),
                                 FormatLevel(Level),
                                 FormatModuleName(ModuleName),
                                 FormatIndex(ModuleIndex),
                                 Message
                );

            String result = builder.ToString();

            return result;
        }

        #endregion Преобразования

        #region Вспомогательные методы

        public override String ToString()
        {
            String result = String.Format("Log Event: Module='{0}' Level='{1}' Message='{2}' SequenceId='{3}'", ModuleName, Level, Message, SequenceId);

            return result;
        }

        #endregion Вспомогательные методы

        #region Методы

        public Byte[] ToBuffer(Boolean addSign = true)
        {
            ByteList list = new ByteList();

            //   [Id   ]      - Уникальный номер сообщения
            //   [Time ]      - Время
            //   [Level]      - Уровень отладки
            //   [Mod  ]      - Имя модуля
            //   [Index]      - Индекс модуля
            //   [Message]    - Сообщение

            if (addSign)
            {
                list.AddByte32(SIGN_NET_SEND);
                list.AddByte32(0);
            }

            list.AddInt(SequenceId);
            list.AddDateTime(TimeStamp);
            list.AddByte8((Byte)Level);
            list.AddString(ModuleName);
            list.AddInt(ModuleIndex);
            list.AddString(Message);

            Byte[] result = list.ToBuffer();

            if (addSign)
            {
                // Заполнение поля "Размер данных" действительными значениями
                result.SetByte32Ex(4, (UInt32)result.Length, Endianess.Little);
            }

            return result;
        }

        public void FromBuffer(Byte[] buffer)
        {
            ByteList list = new ByteList(buffer);

            //   [4][Id   ]      - Уникальный номер сообщения
            //   [8][Time ]      - Время
            //   [4][Level]      - Уровень отладки
            //   [1][Mod  ]      - Имя модуля
            //   [4][Index]      - Индекс модуля
            //   [x][Message]    - Сообщение

            SequenceId = list.GetInt();
            TimeStamp = list.GetDateTime();
            Level = (LogLevel)list.GetByte8();
            ModuleName = list.GetString();
            ModuleIndex = list.GetInt();
            Message = list.GetString();
        }

        public void ToBufferWithSign()
        {
        }

        #endregion
    }

    #endregion Класс LogMessage
}