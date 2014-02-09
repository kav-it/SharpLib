// ****************************************************************************
//
// Имя файла    : 'Types.cs'
// Заголовок    : Базовые классы/интерфейсы взаимодействия фреймворка SharpLib
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 04/05/2012
// Дата         : 09/02/2014
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace SharpLib
{

    #region Перечисление ModuleTyp

    /// <summary>
    /// Типы базовых модулей
    /// </summary>
    public enum ModuleTyp
    {
        /// <summary>
        /// Тип не определен
        /// </summary>
        Unknow = 0,

        /// <summary>
        /// Канал (базовая реализация)
        /// </summary>
        Channel = 100,

        /// <summary>
        /// Канал TCP
        /// </summary>
        ChannelTcp = Channel + 1,

        /// <summary>
        /// Канал UDP
        /// </summary>
        ChannelUdp = Channel + 2,

        /// <summary>
        /// Канал USB
        /// </summary>
        ChannelUsb = Channel + 3,

        /// <summary>
        /// Канал последовательного порта (RS232)
        /// </summary>
        ChannelSerial = Channel + 4,

        /// <summary>
        /// Модем
        /// </summary>
        ChannelModem = Channel + 5,

        /// <summary>
        /// Канал DATA
        /// </summary>
        ChannelData = Channel + 6,

        /// <summary>
        /// Канал VOICE
        /// </summary>
        ChannelVoice = Channel + 7,

        /// <summary>
        /// Канал SMS
        /// </summary>
        ChannelSms = Channel + 8,

        /// <summary>
        /// Канал "Сокет"
        /// </summary>
        ChannelSocket = Channel + 9,

        /// <summary>
        /// Протокол (базовая реализация)
        /// </summary>
        Proto = 200,

        /// <summary>
        /// MSRV-протокол (реализация Стелс)
        /// </summary>
        ProtoMsrv = Proto + 1,

        /// <summary>
        /// BRMI-протокол (реализация Стелс)
        /// </summary>
        ProtoBrmi = Proto + 2,

        /// <summary>
        /// Ademco-протокол ContactID 
        /// </summary>
        ProtoCid = Proto + 3,

        /// <summary>
        /// MJGS-протокол (реализация Стелс)
        /// </summary>
        ProtoMjgs = Proto + 4,

        /// <summary>
        /// Устройство (базовая реализация)
        /// </summary>
        Device = 300,

        /// <summary>
        /// Соединение с устройством Msrv
        /// </summary>
        DeviceConnect = Device + 1,

        /// <summary>
        /// Менеджер соединений с устройством Msrv по USB
        /// </summary>
        DeviceConnectManagerUsb = Device + 2,

        /// <summary>
        /// Менеджер соединений с устройством Msrv по TCP (сервер принимающий соединения)
        /// </summary>
        DeviceConnectManagerTcp = Device + 3,

        /// <summary>
        /// Менеджер соединений с устройством Msrv по TCP через сервер ПЦН
        /// </summary>
        DeviceConnectManagerPcn = Device + 4,

        /// <summary>
        /// Сокет (базовая реализация)
        /// </summary>
        Socket = 400,

        /// <summary>
        /// Сокет Tcp
        /// </summary>
        SocketTcp = Socket + 1,

        /// <summary>
        /// Сокет Tcp
        /// </summary>
        SocketUdp = Socket + 2,

        /// <summary>
        /// Плагин (модуль с собственной бизнес-логикой)(базовая реализация)
        /// </summary>
        Plugin = 500,

        /// <summary>
        /// Плагин на сокетах
        /// </summary>
        PluginSocket = Plugin + 1,
    }

    #endregion Перечисление ModuleTyp

    #region Перечисление ModuleEvent

    /// <summary>
    /// Коды событий модулей
    /// </summary>
    public enum ModuleEvent
    {
        None = 0,

        ChannelBase = 100,

        ChannelOpened = ChannelBase + 1,

        ChannelClosed = ChannelBase + 2,

        ChannelError = ChannelBase + 3
    }

    #endregion Перечисление ModuleEvent

    #region Перечисление ModuleErrorCode

    /// <summary>
    /// Коды ошибок модуля
    /// </summary>
    public enum ModuleErrorCode
    {
        None = 0, // 
        Exception = 1, // Исключение 
        AccessDenied = 50, // "Доступ запрещен"
        WrongParam = 51, // "Неверные параметры"
        WrongPortName = 52, // "Неверное имя порта" 
        NotOpened = 53, // "Не открыт" 
        AlreadyOpen = 54, // "Уже открыт" 
        PortNotPresent = 55, // "Порт не существует"
        SendToClosed = 56, // "Передача данных в закрытый порт"
        SendError = 57, // "Передача данных (общая ошибка)"
        BufferIsNull = 58, // "Буфер равен null"
        Timeout = 59, // "Таймаут выполнения операции"
        Interrupted = 60, // "Операция прервана"
    }

    #endregion Перечисление ModuleErrorCode

    #region Перечисление ModuleThreadState

    public enum ModuleThreadState
    {
        Unknow = 0,

        Run = 1,

        Runing = 2,

        Suspending = 3,

        Suspend = 4,

        Stoping = 5,

        Stop = 6
    }

    #endregion Перечисление ModuleThreadState

    #region Перечисление SortDirection

    public enum SortDirection
    {
        None = 0,

        Inc = 1,

        Dec = -1
    }

    #endregion Перечисление SortDirection

    #region Перечисление ThreadWaitState

    public enum ThreadWaitResult
    {
        Unknow = 0,

        Timeout = 1,

        Break = 2,

        Event = 3
    }

    #endregion Перечисление ThreadWaitState

    #region Делегат ModuleEventHandler

    /// <summary>
    /// Базовое событие модуля
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ModuleEventHandler(object sender, ModuleEventArgs args);

    #endregion Делегат ModuleEventHandler

    #region Делегат ModuleDataReceivedEventHandler

    /// <summary>
    /// Базовое событие приема данных
    /// </summary>
    public delegate void ModuleDataReceivedEventHandler(object sender, Byte[] buffer);

    #endregion Делегат ModuleDataReceivedEventHandler

    #region Делегат ModuleThreadDelegate

    public delegate void ModuleThreadDelegate(Object sender, Object data);

    #endregion Делегат ModuleThreadDelegate

    #region Интерфейс IMarshalBuffer

    /// <summary>
    /// Интерфейс преобразвания класса в буфер и обратно
    /// </summary>
    public interface IMarshalBuffer
    {
        #region Методы

        Byte[] ToBuffer();

        void FromBuffer(Byte[] buffer);

        #endregion
    }

    #endregion Интерфейс IMarshalBuffer

    #region Класс ModuleEventArgs

    /// <summary>
    /// Данные события модулей
    /// </summary>
    public class ModuleEventArgs : EventArgs
    {
        #region Свойства

        public ModuleEvent Event { get; set; }

        public String Message { get; set; }

        #endregion

        #region Конструктор

        public ModuleEventArgs() : this("")
        {
        }

        public ModuleEventArgs(String message) : this(ModuleEvent.None, message)
        {
        }

        public ModuleEventArgs(ModuleEvent e, String message)
        {
            Event = e;
            Message = message;
        }

        #endregion
    }

    #endregion Класс ModuleEventArgs

    #region Класс ModuleError

    /// <summary>
    /// Коды ошибок
    /// </summary>
    public class ModuleError
    {
        #region Переменные

        private ModuleErrorCode _code;

        private String _message;

        #endregion Переменные

        #region Свойства

        /// <summary>
        /// Код ошибки
        /// </summary>
        public ModuleErrorCode Code
        {
            get { return _code; }
        }

        /// <summary>
        /// Текст ошибки (дополнительное описание)
        /// </summary>
        public String Message
        {
            get { return _message; }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        public ModuleError(ModuleErrorCode code, String message)
        {
            _code = code;
            _message = message;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ModuleError(ModuleErrorCode code)
        {
            switch (code)
            {
                case ModuleErrorCode.None:
                    _message = "";
                    break;
                case ModuleErrorCode.Exception:
                    _message = "Исключение";
                    break;
                case ModuleErrorCode.AccessDenied:
                    _message = "Доступ запрещен";
                    break;
                case ModuleErrorCode.WrongParam:
                    _message = "Неверные параметры";
                    break;
                case ModuleErrorCode.WrongPortName:
                    _message = "Неверное имя порта";
                    break;
                case ModuleErrorCode.NotOpened:
                    _message = "Не открыт";
                    break;
                case ModuleErrorCode.AlreadyOpen:
                    _message = "Уже открыт";
                    break;
                case ModuleErrorCode.PortNotPresent:
                    _message = "Порт не существует";
                    break;
                case ModuleErrorCode.SendToClosed:
                    _message = "Передача данных в закрытый порт";
                    break;
                case ModuleErrorCode.BufferIsNull:
                    _message = "Буфер равен null";
                    break;
                case ModuleErrorCode.Timeout:
                    _message = "Время выполнения операции истекло";
                    break;
                case ModuleErrorCode.Interrupted:
                    _message = "Операция отменена";
                    break;

                default:
                    throw new Exception("Не определен текст ошибки");
            }
        }

        #endregion
    }

    #endregion Класс ModuleError

    #region Класс IpAddr

    /// <summary>
    /// Класс представления Ip-адреса
    /// </summary>
    public class IpAddr
    {
        #region Поля

        private UInt32 _value;

        #endregion

        #region Свойства

        [XmlIgnore]
        public UInt32 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public String Text
        {
            get { return ToString(); }
            set { _value = ToIp(value); }
        }

        #endregion

        #region Конструктор

        public IpAddr()
        {
        }

        public IpAddr(String text)
        {
            Text = text;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return ToText(Value);
        }

        public static UInt32 ToIp(String text)
        {
            text = text.ToLower();
            text = text.Replace(',', '.');
            if (text == "localhost") text = "127.0.0.1";
            String[] arr = text.Split('.');
            if (arr.Length == 4)
            {
                int a = arr[0].ToIntEx();
                int b = arr[1].ToIntEx();
                int c = arr[2].ToIntEx();
                int d = arr[3].ToIntEx();

                return Pack(a, b, c, d);
            }
            return 0;
        }

        public static String ToText(UInt32 value)
        {
            Byte a = (Byte)(value >> 24);
            Byte b = (Byte)(value >> 16);
            Byte c = (Byte)(value >> 8);
            Byte d = (Byte)(value >> 0);

            String text = String.Format("{0}.{1}.{2}.{3}", a, b, c, d);

            return text;
        }

        public static UInt32 Pack(int a, int b, int c, int d)
        {
            UInt32 value = (((UInt32)a << 24) +
                            ((UInt32)b << 16) +
                            ((UInt32)c << 8) +
                            ((UInt32)d << 0));
            return value;
        }

        #endregion
    }

    #endregion Класс IpAddr

    #region Класс ModuleNotifyBase

    public class ModuleNotifyBase : INotifyPropertyChanged
    {
        #region События

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Методы

        protected void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    #endregion Класс ModuleNotifyBase

    #region Класс ModileThreadBase

    public class ModuleThreadBase
    {
        #region Константы

        protected const int TIMEOUT_DEFAULT = 300;

        protected const int TIMEOUT_INFINITE = Timeout.Infinite;

        protected const int TIMEOUT_SUSPEND = 500;

        #endregion

        #region Поля

        private AutoResetEvent _eventDo;

        private AutoResetEvent _eventStop;

        private AutoResetEvent[] _eventsWait;

        private Object _lockObject;

        private Thread _thread;

        private String _threadName;

        private ThreadStart _threadStart;

        private ModuleThreadState _threadState;

        #endregion

        #region Свойства

        public ModuleThreadState ThreadState
        {
            get { return _threadState; }
            set
            {
                lock (_lockObject)
                {
                    _threadState = value;
                }
            }
        }

        public Boolean IsRun
        {
            get { return (ThreadState < ModuleThreadState.Stoping); }
        }

        public Boolean IsStop
        {
            get { return ThreadState == ModuleThreadState.Stop; }
        }

        #endregion

        #region События

        public event ModuleThreadDelegate Callback;

        public event ModuleThreadDelegate Finished;

        public event ModuleThreadDelegate Started;

        #endregion

        #region Конструктор

        public ModuleThreadBase(String threadName, ThreadStart threadStart)
        {
            _eventStop = new AutoResetEvent(false);
            _eventDo = new AutoResetEvent(false);
            _eventsWait = new[]
                {
                    _eventStop, _eventDo
                };
            _threadState = ModuleThreadState.Stop;
            _lockObject = new Object();
            _threadName = threadName;
            _threadStart = threadStart;
        }

        public ModuleThreadBase(String threadName) : this(threadName, null)
        {
        }

        #endregion

        #region Генерация событий

        protected virtual void RaiseStartedAsync(Object data)
        {
            ThreadState = ModuleThreadState.Run;
            if (Started != null)
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { Started(this, data); }));
        }

        protected virtual void RaiseFinishedAsync(Object data)
        {
            if (Finished != null)
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { Finished(this, data); }));
        }

        public virtual void RaiseCallbackAsync(Object data)
        {
            if ((Callback != null) && (Application.Current != null))
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { Callback(this, data); }));
        }

        #endregion Генерация событий

        #region Управление потоком

        private void Execute()
        {
            ThreadState = ModuleThreadState.Run;

            RaiseStartedAsync(null);

            if (_threadStart != null)
                _threadStart();
            else
                OnExecute();

            ThreadState = ModuleThreadState.Stop;

            RaiseFinishedAsync(null);
        }

        public void StartThread()
        {
            if (ThreadState == ModuleThreadState.Stop)
            {
                ThreadState = ModuleThreadState.Runing;
                _thread = new Thread(Execute);
                _thread.Name = _threadName;
                _thread.Start();

                // 20101007 - Для варианта если поток после "_thread.Start" выполняется сразу в 
                //            кдиентской области и завершается, до начала проверки "ThreadState != ModuleThreadState.Run"
                // нужно предусмотреть проверки "ThreadState != ModuleThreadState.Stop"
                while (ThreadState == ModuleThreadState.Runing)
                {
                    // Ожидание запуска потока
                    Thread.Sleep(0);
                }
            }
            else if (ThreadState == ModuleThreadState.Suspend)
            {
                // Возобновление работы потока
                ResumeThread();
            }
        }

        public void StopThread()
        {
            if (ThreadState == ModuleThreadState.Stop)
                return;
            ThreadState = ModuleThreadState.Stoping;
            _eventStop.Set();
            while (ThreadState != ModuleThreadState.Stop)
            {
                // Ожидание завершения потока
                Thread.Sleep(0);
            }
        }

        public void SuspendThread()
        {
            if (ThreadState == ModuleThreadState.Run)
            {
                ThreadState = ModuleThreadState.Suspending;
                _eventDo.Set();
                while (ThreadState != ModuleThreadState.Suspend)
                {
                    // Ожидание приостановки потока
                    Thread.Sleep(0);
                }
            }
        }

        public void ResumeThread()
        {
            if (ThreadState == ModuleThreadState.Suspend)
            {
                ThreadState = ModuleThreadState.Runing;
                _eventDo.Set();
                while (ThreadState != ModuleThreadState.Run)
                {
                    // Ожидание возобновления потока
                    Thread.Sleep(0);
                }
            }
        }

        public ThreadWaitResult WaitEvent(int timeout)
        {
            int index = WaitHandle.WaitAny(_eventsWait, timeout);
            switch (index)
            {
                case WaitHandle.WaitTimeout:
                    return ThreadWaitResult.Timeout;
                case 0:
                    return ThreadWaitResult.Break;
                case 1:
                    {
                        if (ThreadState == ModuleThreadState.Suspending)
                        {
                            // приостановка потока
                            OnSuspendingThread();
                            ThreadState = ModuleThreadState.Suspend;
                            while (true)
                            {
                                index = WaitHandle.WaitAny(_eventsWait, TIMEOUT_SUSPEND);
                                switch (index)
                                {
                                    case 0:
                                        return ThreadWaitResult.Break;
                                    case 1:
                                        if (ThreadState == ModuleThreadState.Runing)
                                        {
                                            OnResumingThread();
                                            // возобновление потока
                                            ThreadState = ModuleThreadState.Run;
                                            // Program.TraceMod(DebugLevel.Notify, _thread.Name, "Поток " + _thread.ManagedThreadId + " ВОЗОБНОВЛЕН!!!!!!!!");
                                            return ThreadWaitResult.Timeout;
                                        }
                                        break;
                                    case WaitHandle.WaitTimeout:
                                        break;
                                }
                            }
                        }
                        return ThreadWaitResult.Event;
                    }
                default:
                    return ThreadWaitResult.Unknow;
            }
        }

        #endregion Управление потоком

        #region Виртуальные методы (реализация в наследниках)

        protected virtual void OnExecute()
        {
        }

        protected virtual void OnSuspendingThread()
        {
        }

        protected virtual void OnResumingThread()
        {
        }

        #endregion Виртуальные методы (реализация в наследниках)
    }

    #endregion Класс ModileThreadBase

    #region Класс SyncQueue

    public class SyncQueue<T>
    {
        #region Поля

        protected readonly Object _locker;

        protected readonly Queue<T> _queue;

        #endregion

        #region Свойства

        public int Count
        {
            get
            {
                lock (_locker)
                {
                    return _queue.Count;
                }
            }
        }

        #endregion

        #region Конструктор

        public SyncQueue()
        {
            _locker = new Object();
            _queue = new Queue<T>();
        }

        #endregion

        #region Методы

        public void Push(T item)
        {
            lock (_locker)
            {
                _queue.Enqueue(item);
            }
        }

        public T Pop()
        {
            lock (_locker)
            {
                return _queue.Dequeue();
            }
        }

        public T Peek()
        {
            lock (_locker)
            {
                return _queue.Peek();
            }
        }

        public void Clear()
        {
            lock (_locker)
            {
                _queue.Clear();
            }
        }

        #endregion
    }

    #endregion Класс SyncQueue

    #region Класс SyncEventQueue

    public class SyncEventQueue<T> : SyncQueue<T>
    {
        #region Поля

        private AutoResetEvent _event;

        #endregion

        #region Свойства

        public AutoResetEvent Event
        {
            get { return _event; }
            set { _event = value; }
        }

        #endregion

        #region Конструктор

        public SyncEventQueue()
        {
            _event = null;
        }

        #endregion

        #region Методы

        public new void Push(T item)
        {
            lock (_locker)
            {
                _queue.Enqueue(item);
                _event.Set();
            }
        }

        #endregion
    }

    #endregion Класс SyncEventQueue

    #region Класс DictionaryXml

    [XmlRoot("Dictionary")]
    public class DictionaryXml<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region Методы

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            Boolean wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        #endregion
    }

    #endregion Класс DictionaryXml

    #region Класс ModuleVersion

    public class ModuleVersion
    {
        #region Поля

        private DateTime _dateTime;

        private int _v1;

        private int _v2;

        private int _v3;

        private int _v4;

        #endregion

        #region Свойства

        public String Title { get; set; }

        public int V1
        {
            get { return _v1; }
            set { _v1 = value; }
        }

        public int V2
        {
            get { return _v2; }
            set { _v2 = value; }
        }

        public int V3
        {
            get { return _v3; }
            set { _v3 = value; }
        }

        public int V4
        {
            get { return _v4; }
            set { _v4 = value; }
        }

        public DateTime DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        public int Build
        {
            get { return V4; }
            set { V4 = value; }
        }

        public String DateTimeText
        {
            get
            {
                String text = _dateTime.ToLongDateString() + " " + _dateTime.ToLongTimeString();

                return text;
            }
        }

        public Boolean IsBeta
        {
            get { return (_v3 != 0); }
        }

        #endregion

        #region Конструктор

        public ModuleVersion(int v1, int v2, int v3, int v4)
        {
            Title = "Не определено";
            _v1 = v1;
            _v2 = v2;
            _v3 = v3;
            _v4 = v4;
            _dateTime = DateTime.Now;
        }

        public ModuleVersion() : this(0, 0, 0, 0)
        {
        }

        public ModuleVersion(String version) : this()
        {
            // Возможные форматы
            // "0"                   = 0.0.0.0
            // "0.1"                 = 0.1.0.0
            // "0.1.2"               = 0.1.2.0
            // "0.1.2.3"             = 0.1.2.3
            // "4.8.17 (build 1182)" = 4.8.17.1182

            int index = version.SearchEx(" (build ");
            if (index != -1)
            {
                // Заполнение номера build
                _v4 = version.GetIntEx(index);
                // Удаление записи build
                version = version.Substring(0, version.IndexOf(" (build "));
            }

            String[] values = version.SplitEx(".");

            for (int i = 0; i < 4; i++)
            {
                if (i < values.Length)
                {
                    int v = values[i].ToIntEx();
                    switch (i)
                    {
                        case 0:
                            _v1 = v;
                            break;
                        case 1:
                            _v2 = v;
                            break;
                        case 2:
                            _v3 = v;
                            break;
                        case 3:
                            _v4 = v;
                            break;
                    }
                }
            }
        }

        #endregion

        #region Методы

        private static void Copy(ModuleVersion dest, ModuleVersion source)
        {
            dest.V1 = source.V1;
            dest.V2 = source.V2;
            dest.V3 = source.V3;
            dest.V4 = source.V4;
            dest.Title = source.Title;
            dest.DateTime = new DateTime(source.DateTime.Ticks);
        }

        /// <summary>
        /// Инициализация значениями из указанной сборки
        /// </summary>
        public void UpdateFromAssembly(Assembly assembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            FileVersionInfo version = FileVersionInfo.GetVersionInfo(assembly.Location);

            V1 = version.FileMajorPart;
            V2 = version.FileMinorPart;
            V3 = version.FileBuildPart;
            V4 = version.FilePrivatePart;
            Title = version.ProductName;

            FileInfo fileInfo = new FileInfo(assembly.Location);
            DateTime = fileInfo.LastWriteTime;
        }

        /// <summary>
        /// Преобразование в строку (стандартное отображение)
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            String text = String.Format("{0}.{1}", _v1, _v2);

            if (_v3 != 0)
                text += String.Format(".{0} (build {1})", _v3, _v4);

            return text;
        }

        /// <summary>
        /// Преобразование в строку (компактное представление)
        /// </summary>
        public String ToStringCompact()
        {
            String text;

            if (IsBeta)
                text = String.Format("{0}.{1}.{2}", V1, V2, V3);
            else
                text = String.Format("{0}.{1}", V1, V2);

            return text;
        }

        /// <summary>
        /// Представление версии в формете сборки (1.1.0.994)
        /// </summary>
        /// <returns></returns>
        public String ToStringAssembly()
        {
            String text = String.Format("{0}.{1}.{2}.{3}", _v1, _v2, _v3, _v4);

            return text;
        }

        /// <summary>
        /// Установка параметров версии (при загрузки из вне)
        /// </summary>
        /// <param name="assemblyVersion">"1.2.1.1"</param>
        /// <param name="title">"Название приложения"</param>
        public void UpdateFromExternal(String assemblyVersion, String title)
        {
            String[] arr = assemblyVersion.Split('.');

            V1 = int.Parse(arr[0]);
            V2 = int.Parse(arr[1]);
            V3 = int.Parse(arr[2]);
            V4 = int.Parse(arr[3]);

            Title = title;
        }

        /// <summary>
        /// Перерасчет версии до производственной
        /// </summary>
        public ModuleVersion UpgradeToRelease()
        {
            ModuleVersion version = new ModuleVersion();

            Copy(version, this);

            if (version.IsBeta)
            {
                if (version.V1 == 0)
                {
                    version.V1 += 1;
                    version.V2 = 0;
                }
                else
                    version.V2 += 1;

                version.V3 = 0;
            }

            return version;
        }

        public Boolean Equals(ModuleVersion other)
        {
            Boolean result =
                (_v1 == other._v1) &&
                (_v2 == other._v2) &&
                (_v3 == other._v3) &&
                (_v4 == other._v4);

            return result;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
                return base.Equals(obj);

            if ((obj is ModuleVersion) == false)
                throw new InvalidCastException("The 'obj' argument is not a ModuleVersion object.");

            return Equals(obj as ModuleVersion);
        }

        public override int GetHashCode()
        {
            int hash1 = V1.GetHashCode();
            int hash2 = V2.GetHashCode();
            int hash3 = V3.GetHashCode();
            int hash4 = V4.GetHashCode();

            return (hash1 ^ hash2 ^ hash3 ^ hash4);
        }

        #endregion

        public static Boolean operator ==(ModuleVersion a, ModuleVersion b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            Boolean result = a.Equals(b);

            return result;
        }

        public static Boolean operator !=(ModuleVersion version1, ModuleVersion version2)
        {
            Boolean result = !(version1 == version2);

            return result;
        }

        public static Boolean operator <(ModuleVersion a, ModuleVersion b)
        {
            if (a.V1 < b.V1) return true;
            if (a.V1 > b.V1) return false;

            if (a.V2 < b.V2) return true;
            if (a.V2 > b.V2) return false;

            if (a.V3 < b.V3) return true;
            if (a.V3 > b.V3) return false;

            if (a.V4 < b.V4) return true;

            return false;
        }

        public static Boolean operator >(ModuleVersion a, ModuleVersion b)
        {
            if (a == b) return false;
            if (a < b) return false;

            return true;
        }

        public static Boolean operator <=(ModuleVersion a, ModuleVersion b)
        {
            if (a < b) return true;
            if (a == b) return true;

            return false;
        }

        public static Boolean operator >=(ModuleVersion a, ModuleVersion b)
        {
            if (a > b) return true;
            if (a == b) return true;

            return false;
        }
    }

    #endregion Класс ModuleVersion

    #region Класс GuiList

    public class GuiList<T> : ObservableCollection<T>
    {
        #region Конструктор

        public GuiList()
        {
        }

        public GuiList(List<T> list) : base(list)
        {
        }

        public GuiList(IEnumerable<T> collection) : base(collection)
        {
        }

        #endregion

        #region Методы

        private void ApplySort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();

            foreach (var item in sortedItemsList)
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, ListSortDirection direction)
        {
            switch (direction)
            {
                case ListSortDirection.Ascending:
                    {
                        ApplySort(Items.OrderBy(keySelector));
                        break;
                    }
                case ListSortDirection.Descending:
                    {
                        ApplySort(Items.OrderByDescending(keySelector));
                        break;
                    }
            }
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            ApplySort(Items.OrderBy(keySelector, comparer));
        }

        public void AddSafety(T value)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                                                           (Action)(() => { Add(value); })
                    );
            }
        }

        public List<T> ToList()
        {
            List<T> result = new List<T>(Items);

            return result;
        }

        #endregion
    }

    #endregion Класс GuiList

    #region Класс MenuItemBase

    public class MenuItemBase : MenuItem
    {
        #region Конструктор

        public MenuItemBase(String title, RoutedEventHandler click, Object tag)
        {
            Header = title;
            if (click != null)
                Click += click;
            Tag = tag;
        }

        public MenuItemBase(String title) : this(title, null, null)
        {
        }

        public MenuItemBase(String title, RoutedEventHandler click) : this(title, click, null)
        {
        }

        #endregion

        #region Методы

        public void Add(MenuItem item)
        {
            Items.Add(item);
        }

        public void AddSeparator()
        {
            Items.Add(new Separator());
        }

        public void Clear()
        {
            Items.Clear();
        }

        #endregion
    }

    #endregion Класс MenuItemBase

    #region Класс ContextMenuBase

    public class ContextMenuBase : ContextMenu
    {
        #region Методы

        public void Add(MenuItem item)
        {
            Items.Add(item);
        }

        public void AddSeparator()
        {
            Items.Add(new Separator());
        }

        #endregion
    }

    #endregion Класс ContextMenuBase

    #region Класс Tree

    public class Tree<T> where T : class
    {
        #region Свойства

        public T Value { get; set; }

        public Tree<T> Parent { get; private set; }

        public Tree<T> Root
        {
            get
            {
                Tree<T> curr = this;

                while (curr != null)
                {
                    if (curr.Parent == null)
                        return curr;
                    curr = curr.Parent;
                }

                return null;
            }
        }

        public List<Tree<T>> Childs { get; private set; }

        public Boolean HasChilds
        {
            get { return (Childs.Count > 0); }
        }

        #endregion

        #region Конструктор

        public Tree() : this(null, null)
        {
        }

        public Tree(T value) : this(value, null)
        {
        }

        public Tree(T value, Tree<T> parent)
        {
            Value = value;
            Parent = parent;
            Childs = new List<Tree<T>>();
        }

        #endregion

        #region Вспомогательные

        private Tree<T> SearchRecurce(Tree<T> parent, T value)
        {
            foreach (Tree<T> item in parent.Childs)
            {
                if (item.Value == value) return item;

                if (item.HasChilds)
                {
                    Tree<T> result = SearchRecurce(item, value);
                    if (result != null) return result;
                }
            }

            return null;
        }

        private void ToListRecurce(Tree<T> parent, List<T> list)
        {
            foreach (Tree<T> item in parent.Childs)
            {
                list.Add(item.Value);

                if (item.HasChilds)
                    ToListRecurce(item, list);
            }
        }

        #endregion Вспомогательные

        #region Внешние

        public Tree<T> AddChild(T value)
        {
            Tree<T> tree = new Tree<T>(value, this);
            Childs.Add(tree);

            return tree;
        }

        public void RemoveChild(T value)
        {
            foreach (Tree<T> child in Childs)
            {
                if (child.Value == value)
                {
                    Childs.Remove(child);
                    return;
                }
            }
        }

        public Tree<T> Search(T value)
        {
            Tree<T> root = Root;
            Tree<T> result = SearchRecurce(root, value);

            return result;
        }

        public Tree<T> SearchParent(T value)
        {
            Tree<T> result = Search(value);

            return result.Parent;
        }

        public void Clear()
        {
            Childs.Clear();
            Parent = null;
            Value = null;
        }

        public List<T> ToList()
        {
            List<T> list = new List<T>();

            ToListRecurce(Root, list);

            return list;
        }

        private void PrintRecourse(Tree<T> root, int level)
        {
            if (root != null)
            {
                String spaces = "".ExpandRightEx(level * 2);
                String result = String.Format("{0}{1}", spaces, root.Value);
                Debug.WriteLine(result);

                foreach (Tree<T> child in root.Childs)
                    PrintRecourse(child, level + 1);
            }
        }

        public void Print(String caption = "\r\n=================== Print Tree ===================")
        {
            Debug.WriteLine(caption);

            PrintRecourse(this, 0);
        }

        #endregion Внешние
    }

    #endregion Класс Tree
}