// ****************************************************************************
//
// Имя файла    : 'WcfClient.cs'
// Заголовок    : Это файл предназначен для ...
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 06/02/2014
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace SharpLib.Wcf
{
    #region Класс WcfHost

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public abstract class WcfHost<TInterface, TInterfaceCallback> : IWcfHost
    {
        #region Поля

        private readonly AutoResetEvent _cancelEvent;

        private readonly String _threadName;

        private Binding _binding;

        private ServiceHost _host;

        private Thread _thread;

        #endregion

        #region Свойства

        public Boolean IsOpened
        {
            get { return (_host != null); }
        }

        public IList<TInterfaceCallback> CallbackList { get; private set; }

        public String Address { get; private set; }

        #endregion

        #region Конструктор

        public WcfHost(String threadName)
        {
            CallbackList = new List<TInterfaceCallback>();
            _threadName = threadName;
            _host = null;
            _cancelEvent = new AutoResetEvent(false);
        }

        #endregion

        #region Методы

        public void StartPipe(String address)
        {
            _binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            Address = address;

            Start();
        }

        private void Start()
        {
            if (_thread == null)
            {
                _cancelEvent.Reset();

                _thread = new Thread(DoExecute);

                _thread.Name = _threadName;
                _thread.Start();
            }
        }

        public void Stop()
        {
            if (_thread != null)
            {
                _cancelEvent.Set();
                _thread.Join();
                _thread = null;
            }
        }

        private void DoExecute()
        {
            using (ServiceHost serviceHost = new ServiceHost(this))
            {
                // Конфигурирование сервиса
                serviceHost.AddServiceEndpoint(typeof(TInterface), _binding, Address);

                serviceHost.Opened += ServiceHostOpened;
                serviceHost.Closed += ServiceHostClosed;

                try
                {
                    // Открытие сервиса
                    serviceHost.Open();
                    // Ожидание завершения работы сервиса
                    _cancelEvent.WaitOne();

                    // Закрытие сервиса
                    serviceHost.Close();
                }
                catch (CommunicationObjectFaultedException)
                {
                    //log exception
                }
                catch (TimeoutException)
                {
                    //log exception
                }
            }

            _thread = null;
            _host = null;
        }

        private void ServiceHostOpened(object sender, EventArgs e)
        {
            _host = sender as ServiceHost;
        }

        private void ServiceHostClosed(object sender, EventArgs e)
        {
            _host = null;
        }

        public void Register()
        {
            var callback = OperationContext.Current.GetCallbackChannel<TInterfaceCallback>();
            if (callback != null)
            {
                var communicate = (callback as ICommunicationObject);

                if (communicate != null)
                {
                    communicate.Closed += communicate_Closed;
                    CallbackList.Add(callback);
                }
            }
        }

        private void communicate_Closed(object sender, EventArgs e)
        {
            CallbackList.Remove((TInterfaceCallback)sender);
        }

        public void Unregister()
        {
            var callback = OperationContext.Current.GetCallbackChannel<TInterfaceCallback>();
            if (callback != null)
                CallbackList.Remove(callback);
        }

        public ModuleVersion GetVersion()
        {
            return new ModuleVersion();
        }

        #endregion
    }

    #endregion Класс WcfHost
}