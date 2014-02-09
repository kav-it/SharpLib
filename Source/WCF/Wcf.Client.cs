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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace SharpLib.Wcf
{
    public class WcfClient<TInterface> : IWcfHostCallback
    {
        #region Поля

        private ChannelFactory<TInterface> _pipeFactory;

        private TInterface _pipeProxy;

        private Binding _binding;

        #endregion

        #region Свойства

        public String Address { get; private set; }

        public ModuleVersion HostVersion { get; private set; }

        public Boolean IsOpened
        {
            get { return (((IClientChannel)_pipeProxy).State == CommunicationState.Opened); }
        }

        #endregion

        #region Методы

        public void ConnectPipe(String address)
        {
            _binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            Address = address;

            Connect();
        }

        private void Connect()
        {
            var contract = ContractDescription.GetContract(typeof(TInterface));
            var address = new EndpointAddress(Address);
            var endpoint = new ServiceEndpoint(contract, _binding, address);

            _pipeFactory = new DuplexChannelFactory<TInterface>(this, endpoint);
            _pipeProxy = _pipeFactory.CreateChannel();

            //((IClientChannel)pipeProxy).Faulted += PipeProxyFaulted;
            //((IClientChannel)pipeProxy).Opened += PipeProxyOpened;

            try
            {
                // Открытие канала
                ((IClientChannel)_pipeProxy).Open();

                // Регистрация модуля в сервере
                ((IWcfHost)_pipeProxy).Register();

                // Вызов функции
                HostVersion = ((IWcfHost)_pipeProxy).GetVersion();
            }
            catch (Exception)
            {
                //for example show status text or log;
            }
        }

        public void Disconnect()
        {
            // Регистрация модуля в сервере
            ((IWcfHost)_pipeProxy).Register();

            // Закрытие канала
            ((IClientChannel)_pipeProxy).Close();
        }

        public void OnCallback(int code, object data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}