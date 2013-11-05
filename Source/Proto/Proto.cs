//*****************************************************************************
//
// Имя файла    : 'ProtoBase.cs'
// Заголовок    : Базовый класс протоколов
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 19/04/2013
//
//*****************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SharpLib
{
    #region Делегат ProtoNotifyEvent
    public delegate void ProtoNotifyEvent (ProtoBase sender);
    #endregion Делегат ProtoNotifyEvent

    #region Делегат ProtoWriteEvent
    public delegate void ProtoErrorEvent(ProtoBase sender, EProtoError error);
    #endregion Делегат ProtoWriteEvent

    #region Делегат ProtoWriteEvent
    public delegate void ProtoWriteEvent(ProtoBase sender, Byte[] data);
    #endregion Делегат ProtoWriteEvent

    #region Перечисление EProtoError
    public enum EProtoError
    {
        None                = 0,            // Нет ошибки
        Timeout             = 1,            // Тайм-аут передачи
        Send                = 2,            // Ошибка передачи данных
    }
    #endregion Перечисление EProtoError

    #region Перечисление EProtoState
    public enum EProtoState
    {
        Closed              = 0,            // Закрыт
        Opened              = 1,            // Открыт
        Listen              = 2             // Ожидание входящих соединений
    }
    #endregion Перечисление EProtoState
    
    #region Перечисление EProtoTask
    public enum EProtoTask
    {
        None                = 0,            // Нет задач
        Opening             = 1,            // Нет задач
        Closing             = 2,            // Нет задач
        Sending             = 3             // Нет задач
    }
    #endregion Перечисление EProtoTask
    
    #region Перечисление EProtoResult
    public enum EProtoResult
    {
        Unknow              = 0,            // Неопределено
        Ok                  = 1,            // Задача выполнена успешно        
        Timeout             = 2,            // Тайм-аут выполнения задачи
        InternalError       = 3             // Ошибка внутренних состояний
    }
    #endregion Перечисление EProtoResult

    #region Класс ProtoBase
    public abstract class ProtoBase : ModuleBase, ICoreExecuteModule
    {
        #region Поля
        private static int _sharedId = 0;               // Автономер для экземпляров объектов
        private EProtoState         _state;             // Состояние протокола
        private EProtoTask          _task;              // Текущая задача протокола
        private EProtoResult        _result;            // Результат выполнения задачи
        private Boolean             _isTimeout;         // Флаг тайм-аута передачи
        private CoreExecuteState    _executeState;      // Состояние выполнения Poll
        #endregion Поля

        #region Свойства
	    public EProtoState State
	    {
		    get { return _state; }
	    }
	    public EProtoTask Task
	    {
		    get { return _task; }
	    }
	    public EProtoResult Result
	    {
		    get { return _result; }
	    }
        public Boolean IsOpened
        {
            get { return (_state == EProtoState.Opened); }
        }
        public Boolean IsClosed
        {
            get { return (_state == EProtoState.Closed); }
        }
        public Boolean IsTimeout
        {
            get { return _isTimeout; }
            set { _isTimeout = value; }
        }
        public CoreExecuteState ExecuteState
        {
            get { return _executeState; }
            set { _executeState = value; }
        }
        #endregion Свойства

        #region События
        public event ProtoNotifyEvent OnOpen;
        public event ProtoNotifyEvent OnClose;
        public event ProtoNotifyEvent OnBreak;
        public event ProtoErrorEvent  OnError;
        public event ProtoNotifyEvent OnListen;
        public event ProtoNotifyEvent OnSend;
        public event ProtoWriteEvent  OnWrite;
        #endregion События

        #region Конструктор
        public ProtoBase(ModuleTyp typ, String title, int index): base(typ, title, index)
        {
            _state        = EProtoState.Closed;
            _task         = EProtoTask.None;
            _result       = EProtoResult.Unknow;
            _executeState = CoreExecuteState.Active;

            OnOpen   = null;
            OnClose  = null;
            OnBreak  = null;
            OnError  = null;
            OnListen = null;

            ModIndex = ++_sharedId;
        }
        #endregion Конструктор

        #region Методы

        #region Генерация событий
        protected void RaiseEventOpen()
        {
            if (OnOpen != null)
                OnOpen(this);
        }
        protected void RaiseEventClose()
        {
            if (OnClose != null)
                OnClose(this);
        }
        protected void RaiseEventBreak()
        {
            if (OnBreak != null)
                OnBreak(this);
        }
        protected void RaiseEventError(EProtoError error)
        {
            if (OnError != null)
                OnError(this, error);
        }
        protected void RaiseEventListen()
        {
            if (OnListen != null)
                OnListen(this);
        }
        protected void RaiseEventSend()
        {
            if (OnSend != null)
                OnSend(this);
        }
        protected void RaiseEventWrite(Byte[] data)
        {
            if (OnWrite != null)
                OnWrite(this, data);
        }
        #endregion Генерация событий

        #region Основной цикл
        public void Poll()
        {
            DoPoll();
        }
        #endregion Основной цикл

        #region Виртуальные методы
        protected abstract Boolean DoOpen();
        protected abstract Boolean DoClose();
        protected abstract void DoPoll();
        public abstract void PutBuf(Byte[] buf);
        #endregion Виртуальные методы

        #region Управление состоянием
        protected void SetState(EProtoState state, EProtoResult result = EProtoResult.Ok)
        {
            _state = state;
    
            switch (state)
            {
                case EProtoState.Closed:  
                    {
                        SetTask(EProtoTask.None, result);
                        // TRACE_PROTO(level_INFO, "Состояние: Closed");    
                        RaiseEventClose();
                    }
                    break;
            
                case EProtoState.Opened: 
                    {
                        _isTimeout = false;
                        SetTask(EProtoTask.None, result);
                        // TRACE_PROTO(level_INFO, "Состояние: Opened");      
                        RaiseEventOpen();
                    }
                    break;

                case EProtoState.Listen: 
                    {
                        SetTask(EProtoTask.None, result);
                        // TRACE_PROTO(level_INFO, "Состояние: Listen");      
                        RaiseEventListen();
                    }
                    break;
            } // end switch (анализ состояния)
        }
        protected void SetTask (EProtoTask task, EProtoResult result = EProtoResult.Ok)
        {
            _task = task;
    
            if (task == EProtoTask.None)
            {
                if (result == EProtoResult.Timeout)
                    _isTimeout = true;    
                _result = result;
            }
            else
            {
                _result = EProtoResult.Unknow;
            }

            switch (task)
            {
                case EProtoTask.Opening:   
                    // TRACE_PROTO(level_INFO, "Задача: Открытие");
                    // CALLBACK(IoctlCode.IOCTL_PROTO_OPENING);   
                    break;
            
                case EProtoTask.Closing:   
                    // TRACE_PROTO(level_INFO, "Задача: Закрытие");            
                    // CALLBACK(IoctlCode.IOCTL_PROTO_CLOSING);   
                    break;    
            
                case EProtoTask.Sending:   
                    // TRACE_PROTO(level_INFO, "Задача: Передача");
                    // CALLBACK(IoctlCode.IOCTL_PROTO_SENDING);   
                    _isTimeout = false;
                    break;
            }
        }
        public Boolean Open()
        {
            if (_state == EProtoState.Closed)
            {
                if ( DoOpen() )
                    SetState(EProtoState.Opened);
                else
                    SetTask(EProtoTask.Opening);
    
                return true;
            }
            else
            {
                _result = EProtoResult.Ok;
                // TRACE_PROTO(level_WARNING, "Ошибка открытия: Уже открыт");    
            }
      
            return false;
        }
        public Boolean Close()
        {
            if (_state != EProtoState.Closed || _task == EProtoTask.Opening)  
            {
                if ( DoClose() )
                    SetState(EProtoState.Closed);
                else
                    SetTask(EProtoTask.Closing);
  
                return true;
            }
            else
            {
                _result = EProtoResult.Ok;
                // TRACE_PROTO(level_WARNING, "Ошибка закрытия: Уже закрыт");
            }
  
            return false;    
        }
        public void Reset()
        {
            SetState(EProtoState.Closed, EProtoResult.Unknow);    
        }
        #endregion Управление состоянием

        #endregion Методы
    }
    #endregion Класс ProtoBase
}