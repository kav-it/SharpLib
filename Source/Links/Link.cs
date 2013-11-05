// ****************************************************************************
//
// Имя файла    : 'Link.cs'
// Заголовок    : Базовый класс потоков данных
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 09/07/2012
//
// ****************************************************************************

using System;
using System.Text;

namespace SharpLib
{
    #region Перечисление LinkState
    public enum LinkState
    {
        Closed  = 0,              // Закрыт
        Opened  = 1,              // Открыт         
        Listen  = 2,              // Ожидание входящего соединения
        Opening = 3,              // Идет процесс открытия
        Closing = 4               // Идет процесс закрытия
    }
    #endregion Перечисление LinkState

    #region Перечисление ProtoError
    /// <summary>
    /// Коды ошибок выполнения 
    /// </summary>
    public enum ProtoError
    {
        [EnumText("")]
        Ok = 0,
        [EnumText("Неверная длина пакета")]
        LenWrong = 1,
        [EnumText("Ошибка контрольной суммы при декодировании заголовка")]
        CrcHeader = 2,
        [EnumText("Ошибка контрольной суммы при декодировании всего пакета")]
        CrcBody = 3,
        [EnumText("Истекло время ожидания пакета")]
        Timeout = 4,
        [EnumText("Неверные параметры")]
        WrongParam = 5,
        [EnumText("Ошибка выполнения команды")]
        CmdError = 6,
        [EnumText("Операция отменена")]
        Interrupted = 7
    }
    #endregion Перечисление ProtoError

    #region Класс LinkBase
    public abstract class LinkBase : ModuleBase
    {
        #region Поля
        /// <summary>
        /// Режим работы канала (клиент/сервер)
        /// </summary>
        private Boolean _server;
        /// <summary>
        /// Состояние (открыт/закрыт и т.д.)
        /// </summary>
        private LinkState _state;
        #endregion Поля

        #region Свойства
        /// <summary>
        /// Имя канала (COM1, //USBXXX, TCP (localhost:7000) и т.д.)
        /// </summary>
        public String LinkName
        {
            get { return GetLinkName(); }
        }
        /// <summary>
        /// Режим работы канала (клиент/сервер)
        /// </summary>
        public Boolean Server
        {
            get { return _server; }
            set { _server = value; }
        }
        /// <summary>
        /// Состояние канала (открыт/закрыт и т.д.)
        /// </summary>
        public LinkState State
        {
            get { return _state; }
            set { _state = value; }
        }
        /// <summary>
        /// Признак открытого канала
        /// </summary>
        public Boolean IsOpened
        {
            get { return _state == LinkState.Opened; }
        }
        /// <summary>
        /// Признак закрытого канала
        /// </summary>
        public Boolean IsClosed
        {
            get { return _state == LinkState.Closed; }
        }
        #endregion Свойства

        #region События
        /// <summary>
        /// Событие "Канал открыт"
        /// </summary>
        public event ModuleEventHandler Opened;
        /// <summary>
        /// Событие "Канал закрыт"
        /// </summary>
        public event ModuleEventHandler Closed;
        /// <summary>
        /// Событие "Канал разорван по инициативе ответной стороны"
        /// </summary>
        public event ModuleEventHandler Break;
        /// <summary>
        /// Событие "Ошибка работы канала"
        /// </summary>
        public event ModuleEventHandler Error;
        /// <summary>
        /// Событие "Приняты данные"
        /// </summary>
        public event ModuleDataReceivedEventHandler DataReceived;
        #endregion События

        #region Конструктор
        /// <summary>
        /// Конструктор объекта
        /// </summary>
        /// <param name="typ">Тип канала</param>
        /// <param name="name">Имя канала</param>
        protected LinkBase(ModuleTyp typ, String name) : base(typ, name)
        {
            _server = false;
            _state  = LinkState.Closed;

            Opened  = null;
            Closed  = null;
            Error   = null;
            DataReceived = null;
        }
        #endregion Конструктор

        #region Методы

        #region Абстрактные классы (реализация в наследниках) 
        protected abstract Boolean OnOpen();
        protected abstract Boolean OnClose();
        protected abstract Boolean OnSend(Byte[] buffer);
        protected abstract String GetLinkName();
        #endregion Абстрактные классы (реализация в наследниках)

        #region Генерация событий
        protected virtual void RaiseClosed()
        {
            if (Closed != null)
                Closed(this, null);
        }
        protected virtual void RaiseBreak()
        {
            if (Break != null)
                Break(this, null);
        }
        protected virtual void RaiseDataReceived(Byte[] buffer)
        {
            if (DataReceived != null)
                DataReceived(this, buffer);
        }
        #endregion Генерация событий

        #region Перегруженные классы (использование в наследниках)
        protected void NotifyOpen(String text)
        {
            TraceInfo(text);

            if (Opened != null)
            {
                Opened(this, new ModuleEventArgs(ModuleEvent.ChannelOpened, text));
            }
        }
        protected void NotifyClose(String text)
        {
            TraceInfo(text);

            if (Closed != null)
            {
                Closed(this, new ModuleEventArgs(ModuleEvent.ChannelClosed, text));
            }
        }
        protected void NotifyError(String text)
        {
            TraceError(text);

            if (Error != null)
            {
                Error(this, new ModuleEventArgs(ModuleEvent.ChannelError, text));
            }
        }
        protected void NotifyReceived(Byte[] buffer)
        {
            TraceDump(DebugLevel.In, GetLinkName(), buffer);
        }
        #endregion Перегруженные классы (использование в наследниках)

        #region Внешние функции (видимость для потребителей)
        public Boolean Open()
        {
            LastError = new ModuleError(ModuleErrorCode.None);

            TraceInfo(String.Format("Открытие {0}", LinkName)); 

            if (State == LinkState.Opened)
            {
                TraceWarn(String.Format("Уже открыт {0}", LinkName)); 

                return true;
            }

            State = LinkState.Opening;

            if (OnOpen())
            {
                State = LinkState.Opened;

                return true;
            }

            State = LinkState.Closed;

            return false;
        }
        public Boolean Close()
        {
            TraceInfo(String.Format("Закрытие {0}", LinkName));

            if (State == LinkState.Closed)
            {
                TraceWarn(String.Format("Уже закрыт {0}", LinkName));

                return true;
            }

            State = LinkState.Closing;

            OnClose();
            
            State = LinkState.Closed;

            return true;
        }
        public Boolean SendBytes(Byte[] buffer)
        {
            if (State == LinkState.Opened)
            {
                Boolean result = OnSend(buffer);
                if (result)
                {
                    TraceDump(DebugLevel.Out, LinkName, buffer);
                }
                return result;
            }
            else
            {
                // Передача данных в закрытый порт
                LastError = new ModuleError(ModuleErrorCode.SendToClosed);
            }
            return false;
        }
        public Boolean SendString(String text)
        {
            Byte[] buffer = Encoding.Default.GetBytes(text);
            return SendBytes(buffer);
        }
        #endregion Внешние функции (видимость для потребителей)

        #endregion Методы
    }
    #endregion Класс LinkBase
}
