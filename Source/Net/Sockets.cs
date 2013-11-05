// ***************************************************************************
//
// Имя файла    : 'Sockets.cs'
// Заголовок    : Сетевые сокеты
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 01/01/2013
//
// ***************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SharpLib
{
    #region Делегаты
    public delegate void SocketBaseHandler (Object sender, Socket winsock);
    public delegate void SocketReceiveHandler(Socket sock, Byte[] buffer);
    public delegate void SocketErrorHandler(Socket winsock);
    #endregion Делегаты

    #region Класс SocketCustom
    public class SocketCustom
    {
        #region Константы
        private const int SOCKET_BUF_IN_SIZE = 8 * 1024;
        #endregion Константы

        #region Поля
        static private int _clientCount;

        private String _localIp;
        private int _localPort;
        private String _remoteIp;
        private int _remotePort;
        private String  _lastError;
        private Boolean _isRunning;
        private Socket _winSocket;
        #endregion Поля

        #region Свойства
        public String LocalIp
        {
            get { return _localIp; }
            protected set { _localIp = value; }
        }
        public int LocalPort
        {
            get { return _localPort; }
            protected set { _localPort = value; }
        }
        public String RemoteIp
        {
            get { return _remoteIp; }
            set { _remoteIp = value; }
        }
        public int RemotePort
        {
            get { return _remotePort; }
            set { _remotePort = value; }
        }
        public String LastError
        {
            get { return _lastError; }
            set { _lastError = value; }
        }
        public Socket WinSocket
        {
            get { return _winSocket; }
            set { _winSocket = value; }
        }
        public Boolean IsRunning
        {
            get { return _isRunning; }
            protected set { _isRunning = value; }
        }
        #endregion Свойства

        #region События
        public event SocketBaseHandler OnConnect;
        public event SocketBaseHandler OnDisconnect;
        public event SocketBaseHandler OnBreak;
        public event SocketReceiveHandler OnRead;
        #endregion События

        #region Конструктор
        public SocketCustom(Boolean server)
        {
            _isRunning = false;
            _clientCount   = 0;
        }
        #endregion Конструктор

        #region Методы

        #region Генерация событий
        protected void RaiseOnConnect(Socket winsock)
        {
            if (OnConnect != null)
            {
                OnConnect(this, winsock);
            }
        }
        protected void RaiseOnReceive(Socket winsock, Byte[] buffer, int size)
        {
            if (OnRead != null)
            {
                Byte[] realBuffer = Mem.Clone(buffer, 0, size);

                OnRead(winsock, realBuffer);
            }
        }
        protected void RaiseOnDisconnect(Socket winsock)
        {
            if (OnDisconnect != null)
            {
                OnDisconnect(this, winsock);
            }
        }
        protected void RaiseOnBreak(Socket winsock)
        {
            if (OnBreak != null)
            {
                OnBreak(this, winsock);
            }
        }
        #endregion Генерация событий

        #region Основной поток приема
        protected void ExecuteReceive (Socket sock)
        {
            // Для каждого нового подключения, будет создан свой поток для приема пакетов
            Thread th = new Thread(new ParameterizedThreadStart(DoReceive));
            // Именование потока для удобства отладки
            th.Name = String.Format("SockReceive.{0}", ++_clientCount);
            th.IsBackground = true;
            // Запуск потока
            th.Start(sock);
        }
        protected void DoReceive (object param)
        {
            Socket sock   = (Socket)param;
            Byte[] buffer = new Byte[SOCKET_BUF_IN_SIZE];

            while (_isRunning)
            {
                try
                {
                    // Прием данных (с ожиданием)
                    int size = sock.Receive(buffer);

                    if (size != 0)
                    {
                        RaiseOnReceive(sock, buffer, size);
                    }
                    else
                    {
                        // В записимости от IsRunning
                        // true:  Соединение закрыто по инициативе сервера
                        // false: Соединение разорвано (вызван метод Close, в котором установлен IsRunning = false)
                        if (_isRunning)
                            RaiseOnBreak(sock);
                        else
                            RaiseOnDisconnect(sock);

                        // Прекращение выполнение потока
                        _isRunning = false;
                    }
                }
                catch (Exception)
                {
                    // Соединение закрыто по инициативе сервера
                    RaiseOnBreak(sock);
                    // Прекращение выполнение потока
                    _isRunning = false;
                }
            }
        }
        #endregion Основной поток приема

        #region Передача
        private void Send (Socket sock, Byte[] bytes)
        {
            try
            {
                sock.Send(bytes);
            }
            catch (Exception)
            {
            }
        }
        public void Send(Byte[] bytes)
        {
            Send(_winSocket, bytes);
        }
        public void Send (String text)
        {
            Byte[] buffer = text.ToBufferEx();

            Send(buffer);
        }
        #endregion Передача

        #endregion Методы
    }
    #endregion Класс SocketCustom

    #region Класс SocketClient
    public class SocketClient : SocketCustom
    {
        #region Конструктор
        public SocketClient() : base(false)
        {
        }
        #endregion Конструктор

        #region Методы
        public Boolean Open()
        {
            if (IsRunning == false)
            {
                try
                {
                    WinSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    WinSocket.Connect(RemoteIp.ToIpEx(), RemotePort);

                    LocalIp   = (WinSocket.LocalEndPoint as IPEndPoint).Address.ToString();
                    LocalPort = (WinSocket.LocalEndPoint as IPEndPoint).Port;

                    IsRunning = true;

                    ExecuteReceive(WinSocket);
                }
                catch (SocketException ex) 
                {
                    LastError = ex.Message;

                    return false;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;

                    return false;
                }
            }

            RaiseOnConnect(WinSocket);

            return true;
        }
        public void Close()
        {
            if (IsRunning)
            {
                IsRunning = false;
                WinSocket.Disconnect(false);
            }
        }
        public Boolean Connect(String ip, int port)
        {
            RemoteIp = ip;
            RemotePort = port;

            return Open();
        }
        #endregion Методы
    }
    #endregion Класс SocketClient

    #region Класс SocketServer
    public class SocketServer
    {
        #region Конструктор
        public SocketServer()
        {
        }
        #endregion Конструктор

        #region Методы
        public void Listen(int port)
        {
        }
        public void Close()
        {
        }
        #endregion Методы
    }
    #endregion Класс SocketServer
}
