using System;
using System.Net.Sockets;

namespace SharpLib.Net
{
    /// <summary>
    /// Базовый класс асинхронного сокета
    /// </summary>
    public class NetSocket: IDisposable
    {
        #region Константы

        /// <summary>
        /// Размер буфера (прием)
        /// </summary>
        public const int BUFFER_SIZE = 8 * 1024;

        /// <summary>
        /// Максимальное количество входящих соединенений
        /// </summary>
        public const int LISTEN_CONN_MAX = 1024;

        #endregion

        #region Свойства

        /// <summary>
        /// Общий идентификатор (для отладки)
        /// </summary>
        private static SharedId SharedId { get; set; }

        /// <summary>
        /// Асинхронная операция "Подключился клиент". Вынесена в переменную, т.к. это необходимо в режиме сервера
        /// </summary>
        private SocketAsyncEventArgs AcceptAsyncArgs { get; set; }

        /// <summary>
        /// .NET сокет
        /// </summary>
        public Socket Sock { get; private set; }

        /// <summary>
        /// Признак открытого/запущенного сокета
        /// </summary>
        public bool IsRunning
        {
            get { return (State == NetSocketState.Opened || State == NetSocketState.Listen); }
        }

        /// <summary>
        /// Состояние сокета
        /// </summary>
        public NetSocketState State { get; protected set; }

        /// <summary>
        /// Идентификатор сокета
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Текущий протокол сокета
        /// </summary>
        public NetProto Proto { get; private set; }

        /// <summary>
        /// Локальный адрес
        /// </summary>
        public NetAddr LocalPoint { get; private set; }

        /// <summary>
        /// Удаленный адрес
        /// </summary>
        public NetAddr RemotePoint { get; private set; }

        #endregion

        #region События

        /// <summary>
        /// Соединение установлено (событие сокета в режиме сервера)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnAccept;

        /// <summary>
        /// Сокет закрыт (по удаленной инициативе)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnBreak;

        /// <summary>
        /// Сокет закрыт (локальная инициатива)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnClose;

        /// <summary>
        /// Соединение установлено (событие сокета в режиме клиента)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnConnect;

        /// <summary>
        /// Соединение разорвано
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnDisconnect;

        /// <summary>
        /// Ошибка в работе сокета
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnError;

        /// <summary>
        /// Сокет открыт (событие сокета в режиме сервера)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnListen;

        /// <summary>
        /// Получены данные
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnReceive;

        /// <summary>
        /// Данные отправлены
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnSend;

        #endregion

        #region Конструктор

        static NetSocket()
        {
            SharedId = new SharedId();
        }

        public NetSocket(NetProto proto)
        {
            OnConnect = null;
            OnDisconnect = null;
            OnBreak = null;
            OnListen = null;
            OnClose = null;
            OnAccept = null;
            OnReceive = null;
            OnSend = null;
            OnError = null;

            State = NetSocketState.Closed;
            Id = SharedId.GetNext();
            Sock = null;
            Proto = proto;
            LocalPoint = new NetAddr();
            RemotePoint = new NetAddr();
            AcceptAsyncArgs = null;
        }

        public NetSocket() : this(NetProto.Tcp4)
        {
        }

        public NetSocket(Socket sock) : this()
        {
            Sock = sock;
            Proto = ConvertToProtoTyp(sock.ProtocolType);
            State = NetSocketState.Opened;
            LocalPoint = sock.LocalEndPoint.ToNetAddrEx();
            RemotePoint = sock.RemoteEndPoint.ToNetAddrEx();
        }

        public void Dispose()
        {
            if (Sock != null)
            {
                Sock.Dispose();
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Текстовое представление объекта
        /// </summary>
        public override string ToString()
        {
            string text = String.Format("[{0}] {1} -> {2}", Id, LocalPoint, RemotePoint);

            return text;
        }

        /// <summary>
        /// Преобразование типа протокола во внутренний формат
        /// </summary>
        private NetProto ConvertToProtoTyp(ProtocolType typ)
        {
            switch (typ)
            {
                case ProtocolType.IP:
                    return NetProto.Ipv4;
                case ProtocolType.IPv4:
                    return NetProto.Ipv4;
                case ProtocolType.IPv6:
                    return NetProto.Ipv6;
                case ProtocolType.Udp:
                    return NetProto.Udp;
                case ProtocolType.Tcp:
                    return NetProto.Tcp4;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Преобразование типа протокола во внешний формат
        /// </summary>
        private ProtocolType ConvertFromoProtoTyp(NetProto typ)
        {
            switch (typ)
            {
                case NetProto.Ipv4:
                    return ProtocolType.IP;
                case NetProto.Ipv6:
                    return ProtocolType.IPv6;
                case NetProto.Udp:
                    return ProtocolType.Udp;
                case NetProto.Tcp4:
                    return ProtocolType.Tcp;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Генерация события "Соединение установлено"
        /// </summary>
        protected void RaiseEventConnect(Socket sock)
        {
            if (OnConnect != null)
            {
                OnConnect(this, new NetSocketEventArgs(sock));
            }
        }

        /// <summary>
        /// Генерация события "Соединение разорвано (инициатива удаленной точки)"
        /// </summary>
        protected void RaiseEventDisconnect(Socket sock)
        {
            if (OnDisconnect != null)
            {
                OnDisconnect(this, new NetSocketEventArgs(sock));
            }
        }

        /// <summary>
        /// Генерация события "Соединение разорвано (инициатива локальная)"
        /// </summary>
        protected void RaiseEventBreak(Socket sock)
        {
            if (OnBreak != null)
            {
                OnBreak(this, new NetSocketEventArgs(sock));
            }
        }

        /// <summary>
        /// Генерация события "Ошибка в процессе работы"
        /// </summary>
        protected void RaiseEventError(Socket sock, SocketError error)
        {
            if (OnError != null)
            {
                OnError(this, new NetSocketEventArgs(sock, error, null));
            }
        }

        /// <summary>
        /// Генерация события "Получены данные"
        /// </summary>
        protected void RaiseEventReceive(Socket sock, byte[] buf)
        {
            if (OnReceive != null)
            {
                OnReceive(this, new NetSocketEventArgs(sock, SocketError.Success, buf));
            }
        }

        /// <summary>
        /// Генерация события "Переданы данные"
        /// </summary>
        protected void RaiseEventSend(Socket sock)
        {
            if (OnSend != null)
            {
                OnSend(this, new NetSocketEventArgs(sock));
            }
        }

        /// <summary>
        /// Генерация события "Сокет открыт для приема входящих соединений"
        /// </summary>
        protected void RaiseEventListen(Socket sock)
        {
            if (OnListen != null)
            {
                OnListen(this, new NetSocketEventArgs(sock));
            }
        }

        /// <summary>
        /// Генерация события "Сокет закрыт"
        /// </summary>
        protected void RaiseEventClose(Socket sock)
        {
            if (OnClose != null)
            {
                OnClose(this, new NetSocketEventArgs(sock));
            }
        }

        /// <summary>
        /// Генерация события "Установлено входящее соединение"
        /// </summary>
        protected void RaiseEventAccept(Socket sock)
        {
            if (OnAccept != null)
            {
                OnAccept(this, new NetSocketEventArgs(sock));
            }
        }

        /// <summary>
        /// Проверка завершена ли операция асинхронная операция синхронно
        /// </summary>
        private void CheckSyncCompleted(bool result, object sender, SocketAsyncEventArgs args)
        {
            if (result == false)
            {
                // Асинхронный вызов завершился синхронно: Вызов обработчика напрямую
                OnSocketAsyncEventCompleted(sender, args);
            }
        }

        /// <summary>
        /// Обработка асинхронных событий
        /// </summary>
        private void OnSocketAsyncEventCompleted(object sender, SocketAsyncEventArgs args)
        {
            var sock = args.UserToken as Socket;
            var error = args.SocketError;
            var oper = args.LastOperation;

            switch (oper)
            {
                case SocketAsyncOperation.None:
                    break;

                    // Соединение установлено
                case SocketAsyncOperation.Connect:
                    {
                        if (error == SocketError.Success && sock != null)
                        {
                            // Установка буфера приема
                            var buffer = new Byte[BUFFER_SIZE];
                            // Генерация нового асинхронного ожидания
                            SocketAsyncEventArgs async = new SocketAsyncEventArgs();
                            async.UserToken = sock;
                            async.SetBuffer(buffer, 0, buffer.Length);
                            async.Completed += OnSocketAsyncEventCompleted;

                            // Перевод в режим приема данных
                            bool result = sock.ReceiveAsync(async);

                            // Установка состояния
                            State = NetSocketState.Opened;
                            // Генерация события
                            RaiseEventConnect(sock);

                            // Проверка синхронного выполнения
                            CheckSyncCompleted(result, sender, args);
                        }
                        else
                        {
                            // Ошибка установки соединения: Генерация события
                            RaiseEventError(sock, error);
                            // Смена состояния
                            State = NetSocketState.Closed;
                        }
                    }
                    break;

                    // Соединение закрыто
                case SocketAsyncOperation.Disconnect:
                    {
                        State = NetSocketState.Closed;

                        // Генерация события
                        RaiseEventDisconnect(sock);
                    }
                    break;

                    // Данные приняты
                case SocketAsyncOperation.Receive:
                    {
                        if (sock != null)
                        {
                            int size = args.BytesTransferred;

                            // Принят буфер
                            if (size > 0)
                            {
                                byte[] temp = args.Buffer;
                                int offset = args.Offset;
                                byte[] buf = Mem.Clone(temp, offset, size);

                                // Перевод в режим приема данных
                                bool result = sock.ReceiveAsync(args);

                                // Передача данных приложению
                                RaiseEventReceive(sock, buf);

                                // Проверка синхронного выполнения
                                CheckSyncCompleted(result, sender, args);

                                // Обязательный выход, чтобы избежать освобождения ресурсов в конце фукнции
                                return;
                            }

                            if (State == NetSocketState.Opened)
                            {
                                // Соединение разорвано по удаленной инициативе
                                State = NetSocketState.Closed;

                                // Оповещение приложения
                                RaiseEventBreak(sock);

                                // Удаление сокета                                
                                sock.Shutdown(SocketShutdown.Both);
                                sock.Close();

                            }
                            else if (State == NetSocketState.Closing)
                            {
                                // Соединение разорвано по локальной инициативе
                                State = NetSocketState.Closed;
                                // Оповещение приложения
                                RaiseEventDisconnect(sock);
                            }
                        }
                    }
                    break;

                    // Данные переданы
                case SocketAsyncOperation.Send:
                case SocketAsyncOperation.SendTo:
                    {
                        if (error == SocketError.Success)
                        {
                            // Данные переданы успешно
                            RaiseEventSend(sock);
                        }
                        else
                        {
                            // Ошибка передачи
                            RaiseEventError(sock, error);
                        }
                    }
                    break;

                    // Установлено входящее соединение
                case SocketAsyncOperation.Accept:
                    {
                        if (error == SocketError.Success)
                        {
                            // Получение сокета установленного соединения
                            sock = args.AcceptSocket;

                            // Ожидание следующего подключения
                            AcceptAsync(args);

                            // Генерация события
                            RaiseEventAccept(sock);

                            // Выход без Dispose асинхронного объекта
                            return;
                        }

                        // Соединение прервано (по инициативе клиента)
                        State = NetSocketState.Closed;

                        // Закрыто соединение
                        RaiseEventClose(sock);
                    }
                    break;
            } // end switch (анализ результата асинхронной операции)

            // ===============================================
            // 20130513 - Исправление ошибки утечки памяти
            // ===============================================
            args.Dispose();
        }

        /// <summary>
        /// Перевод сокета в асинхронное ожидание входящего соединения
        /// </summary>
        private void AcceptAsync(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool isFinished = Sock.AcceptAsync(args);
            if (isFinished == false)
            {
                OnSocketAsyncEventCompleted(Sock, args);
            }
        }

        /// <summary>
        /// Установка соединения
        /// </summary>
        public void Connect(NetAddr addr)
        {
            Connect(addr.Ip.Value, (UInt16)addr.Port, 0);
        }

        /// <summary>
        /// Установка соединения
        /// </summary>
        public void Connect(string destIp, UInt16 destPort)
        {
            UInt32 destIpx32 = NetIpAddr.ToIp(destIp);

            Connect(destIpx32, destPort, 0);
        }

        /// <summary>
        /// Установка соединения
        /// </summary>
        public void Connect(UInt32 destIp, UInt16 destPort)
        {
            Connect(destIp, destPort, 0);
        }

        /// <summary>
        /// Установка соединения
        /// </summary>
        public void Connect(UInt16 destPort)
        {
            Connect(NetIpAddr.Localhost.Value, destPort, 0);
        }

        /// <summary>
        /// Установка соединения
        /// </summary>
        public void Connect(UInt32 destIp, UInt16 destPort, UInt16 localPort)
        {
            if (IsRunning == false)
            {
                NetAddr removeAddr = new NetAddr(destIp, destPort);

                if (Proto == NetProto.Udp)
                {
                    Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    RemotePoint = removeAddr;
                    State = NetSocketState.Opened;

                    return;
                }

                State = NetSocketState.Opening;
                Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ConvertFromoProtoTyp(Proto));

                SocketAsyncEventArgs async = new SocketAsyncEventArgs();
                async.UserToken = Sock;
                async.RemoteEndPoint = removeAddr.ToEndPointEx();
                async.Completed += OnSocketAsyncEventCompleted;

                // Асинхронное соединение с сервером и обработка отвека в OnSocketAsyncEventCompleted
                bool result = Sock.ConnectAsync(async);

                // Проверка синхронного выполнения
                CheckSyncCompleted(result, Sock, async);
            }
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        public void Disconnect()
        {
            if (IsRunning)
            {
                State = NetSocketState.Closing;

                SocketAsyncEventArgs async = new SocketAsyncEventArgs();
                async.Completed += OnSocketAsyncEventCompleted;

                bool result = Sock.DisconnectAsync(async);
                // Проверка синхронного выполнения
                CheckSyncCompleted(result, Sock, async);
            }
        }

        /// <summary>
        /// Перевод сокета в режим приема входящих соединений 
        /// </summary>
        public NetSocketError Listen(int port)
        {
            if (IsRunning == false)
            {
                LocalPoint = new NetAddr(port);

                try
                {
                    if (Proto == NetProto.Udp)
                    {
                        Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        Sock.Bind(LocalPoint.ToEndPointEx());

                        State = NetSocketState.Listen;

                        Receive();

                        return NetSocketError.Ok;
                    }

                    Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    Sock.Bind(LocalPoint.ToEndPointEx());
                    Sock.Listen(LISTEN_CONN_MAX);
                }
                catch (SocketException)
                {
                    return NetSocketError.Busy;
                }
                catch (Exception)
                {
                    return NetSocketError.Unknow;
                }

                State = NetSocketState.Listen;

                AcceptAsyncArgs = new SocketAsyncEventArgs();
                AcceptAsyncArgs.Completed += OnSocketAsyncEventCompleted;
                AcceptAsyncArgs.UserToken = Sock;

                // Перевод сокета в режим ожидания подключений
                AcceptAsync(AcceptAsyncArgs);

                // Генерация события
                RaiseEventListen(Sock);
            }

            return NetSocketError.Ok;
        }

        /// <summary>
        /// Закрытие сокета
        /// </summary>
        public void Close()
        {
            if (IsRunning)
            {
                State = NetSocketState.Closing;

                Sock.Close();
            }
        }

        public void SendText(string value)
        {
            byte[] buffer = value.ToBufferEx();

            SendBuffer(buffer);
        }

        public void SendBuffer(byte[] value)
        {
            if (IsRunning)
            {
                bool result;

                SocketAsyncEventArgs async = new SocketAsyncEventArgs();
                async.SetBuffer(value, 0, value.Length);
                async.UserToken = Sock;
                async.Completed += OnSocketAsyncEventCompleted;

                if (Proto == NetProto.Udp)
                {
                    async.RemoteEndPoint = RemotePoint.ToEndPointEx();
                    result = Sock.SendToAsync(async);
                }
                else
                {
                    result = Sock.SendAsync(async);
                }

                // Проверка синхронного выполнения
                CheckSyncCompleted(result, Sock, async);
            }
        }

        public void Receive()
        {
            // Установка буфера приема
            byte[] buffer = new Byte[BUFFER_SIZE];
            // Генерация нового асинхронного ожидания
            SocketAsyncEventArgs async = new SocketAsyncEventArgs();
            async.UserToken = Sock;
            async.SetBuffer(buffer, 0, buffer.Length);
            async.Completed += OnSocketAsyncEventCompleted;

            // Перевод в режим приема данных
            bool result = Sock.ReceiveAsync(async);

            // Проверка результата выполнения
            CheckSyncCompleted(result, this, async);
        }

        #endregion
    }
}