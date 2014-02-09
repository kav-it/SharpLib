//*****************************************************************************
//
// Имя файла    : 'Core.Net.cs'
// Заголовок    : Подсистема сокетов на базе SharpLib
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/04/2013
//
//*****************************************************************************

using System;
using System.Net;
using System.Net.Sockets;

namespace SharpLib
{

    #region Класс NetSocket

    public class NetSocket
    {
        #region Константы

        public const int BUFFER_SIZE = 8 * 1024;

        public const int LISTEN_CONN_MAX = 1024;

        #endregion

        #region Свойства

        private static SharedId SharedId { get; set; }

        private SocketAsyncEventArgs AcceptAsyncArgs { get; set; }

        private Socket Sock { get; set; }

        public Boolean IsRunning
        {
            get { return (State == NetSocketState.Opened || State == NetSocketState.Listen); }
        }

        public NetSocketState State { get; protected set; }

        public int Id { get; private set; }

        public NetProto Proto { get; private set; }

        public NetAddr LocalPoint { get; private set; }

        public NetAddr RemotePoint { get; private set; }

        #endregion

        #region События

        public event NetSocketNotifyEvent OnAccept;

        public event NetSocketNotifyEvent OnBreak;

        public event NetSocketNotifyEvent OnClose;

        public event NetSocketNotifyEvent OnConnect;

        public event NetSocketNotifyEvent OnDisconnect;

        public event NetSocketErrorEvent OnError;

        public event NetSocketNotifyEvent OnListen;

        public event NetSocketReadEvent OnReceive;

        public event NetSocketNotifyEvent OnSend;

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

        public NetSocket(): this(NetProto.Tcp4)
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

        #endregion

        #region Методы

        public override string ToString()
        {
            String text = String.Format("[{0}] {1} -> {2}", Id, LocalPoint, RemotePoint);

            return text;
        }

        #endregion

        #region Вспомогательные методы

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

        #endregion Вспомогательные методы

        #region Генерация событий

        protected void RaiseEventConnect(Socket sock)
        {
            if (OnConnect != null)
                OnConnect(this, sock);
        }

        protected void RaiseEventDisconnect(Socket sock)
        {
            if (OnDisconnect != null)
                OnDisconnect(this, sock);
        }

        protected void RaiseEventBreak(Socket sock)
        {
            if (OnBreak != null)
                OnBreak(this, sock);
        }

        protected void RaiseEventError(Socket sock, SocketError error)
        {
            if (OnError != null)
                OnError(this, sock, error);
        }

        protected void RaiseEventReceive(Socket sock, Byte[] buf)
        {
            if (OnReceive != null)
                OnReceive(this, sock, buf);
        }

        protected void RaiseEventSend(Socket sock)
        {
            if (OnSend != null)
                OnSend(this, sock);
        }

        protected void RaiseEventListen(Socket sock)
        {
            if (OnListen != null)
                OnListen(this, sock);
        }

        protected void RaiseEventClose(Socket sock)
        {
            if (OnClose != null)
                OnClose(this, sock);
        }

        protected void RaiseEventAccept(Socket sock)
        {
            if (OnAccept != null)
                OnAccept(this, sock);
        }

        #endregion Генерация событий

        #region Асинхронные операции

        private void CheckSyncCompleted(Boolean result, Object sender, SocketAsyncEventArgs args)
        {
            if (result == false)
            {
                // Асинхронный вызов завершился синхронно: Вызов обработчика напрямую
                OnSocketAsyncEventCompleted(sender, args);
            }
        }

        private void OnSocketAsyncEventCompleted(Object sender, SocketAsyncEventArgs args)
        {
            Socket sock = args.UserToken as Socket;
            SocketError error = args.SocketError;
            SocketAsyncOperation oper = args.LastOperation;

            switch (oper)
            {
                case SocketAsyncOperation.None:
                    break;

                    // Соединение установлено
                case SocketAsyncOperation.Connect:
                    {
                        if (error == SocketError.Success)
                        {
                            // Установка буфера приема
                            Byte[] buffer = new Byte[BUFFER_SIZE];
                            // Генерация нового асинхронного ожидания
                            SocketAsyncEventArgs async = new SocketAsyncEventArgs();
                            async.UserToken = sock;
                            async.SetBuffer(buffer, 0, buffer.Length);
                            async.Completed += OnSocketAsyncEventCompleted;

                            // Перевод в режим приема данных
                            Boolean result = sock.ReceiveAsync(async);

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
                        int size = args.BytesTransferred;

                        // Принят буфер
                        if (size > 0)
                        {
                            Byte[] temp = args.Buffer;
                            int offset = args.Offset;
                            Byte[] buf = Mem.Clone(temp, offset, size);

                            // Перевод в режим приема данных
                            Boolean result = sock.ReceiveAsync(args);

                            // Передача данных приложению
                            RaiseEventReceive(sock, buf);

                            // Проверка синхронного выполнения
                            CheckSyncCompleted(result, sender, args);

                            // Обязательный выход, чтобы избежать освобождения ресурсов в конце фукнции
                            return;
                        }
                        else
                        {
                            if (IsRunning)
                            {
                                // Данных нет: Соединение разорвано по инициативе сервера
                                sock.Shutdown(SocketShutdown.Both);
                                sock.Close();

                                State = NetSocketState.Closed;

                                // Оповещение приложения
                                RaiseEventBreak(sock);
                            }
                            else
                            {
                                // Соединение уже прервано по инициативе клиента
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
                        else
                        {
                            // Соединение прервано (по инициативе клиента)
                            State = NetSocketState.Closed;

                            // Закрыто соединение
                            RaiseEventClose(sock);
                        }
                    }
                    break;
            } // end switch (анализ результата асинхронной операции)

            // ===============================================
            // 20130513 - Исправление ошибки утечки памяти
            // ===============================================
            args.Dispose();
        }

        private void AcceptAsync(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            Boolean isFinished = Sock.AcceptAsync(args);
            if (isFinished == false)
                OnSocketAsyncEventCompleted(Sock, args);
        }

        #endregion Асинхронные операции

        #region Управление

        public void Connect(NetAddr addr)
        {
            Connect(addr.Ip.Value, (UInt16)addr.Port, 0);
        }

        public void Connect(String destIp, UInt16 destPort)
        {
            UInt32 destIpx32 = NetIpAddr.ToIp(destIp);

            Connect(destIpx32, destPort, 0);
        }

        public void Connect(UInt32 destIp, UInt16 destPort)
        {
            Connect(destIp, destPort, 0);
        }

        public void Connect(UInt16 destPort)
        {
            Connect(NetIpAddr.Localhost.Value, destPort, 0);
        }

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
                Boolean result = Sock.ConnectAsync(async);

                // Проверка синхронного выполнения
                CheckSyncCompleted(result, Sock, async);
            }
        }

        public void Disconnect()
        {
            if (IsRunning)
            {
                State = NetSocketState.Closing;

                SocketAsyncEventArgs async = new SocketAsyncEventArgs();
                async.Completed += OnSocketAsyncEventCompleted;

                Boolean result = Sock.DisconnectAsync(async);
                // Проверка синхронного выполнения
                CheckSyncCompleted(result, Sock, async);
            }
        }

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

        public void Close()
        {
            if (IsRunning)
            {
                Sock.Close();

                State = NetSocketState.Closing;
            }
        }

        #endregion Управление

        #region Передача данных

        public void SendText(String value)
        {
            Byte[] buffer = value.ToBufferEx();

            SendBuffer(buffer);
        }

        public void SendBuffer(Byte[] value)
        {
            if (IsRunning)
            {
                Boolean result;

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
            Byte[] buffer = new Byte[BUFFER_SIZE];
            // Генерация нового асинхронного ожидания
            SocketAsyncEventArgs async = new SocketAsyncEventArgs();
            async.UserToken = Sock;
            async.SetBuffer(buffer, 0, buffer.Length);
            async.Completed += OnSocketAsyncEventCompleted;

            // Перевод в режим приема данных
            Boolean result = Sock.ReceiveAsync(async);

            // Проверка результата выполнения
            CheckSyncCompleted(result, this, async);
        }

        #endregion Передача данных
    }

    #endregion  NetSocket
}