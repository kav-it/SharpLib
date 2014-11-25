using System;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
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

            builder.AppendFormat("[{0}] [{1}] {2}{3} ",
                FormatTime(TimeStamp),
                FormatLevel(Level),
                FormatModuleName(ModuleName),
                FormatIndex(ModuleIndex));

            builder.AppendFormat(Message, Parameters);

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

        public Byte[] ToBuffer(bool addSign = true)
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
}