// ****************************************************************************
//
// Имя файла    : 'WcfInterface.cs'
// Заголовок    : Это файл предназначен для ...
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 06/02/2014
//
// ****************************************************************************

using System;
using System.ServiceModel;

namespace SharpLib.Wcf
{

    #region Интерфейс IWcfHost

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IWcfHostCallback))]
    public interface IWcfHost
    {
        #region Методы

        /// <summary>
        /// Регистрация клиента сервиса
        /// </summary>
        [OperationContract]
        void Register();

        /// <summary>
        /// Разрегистрация клиента сервиса
        /// </summary>
        [OperationContract]
        void Unregister();

        /// <summary>
        /// Чтение версии сервиса
        /// </summary>
        [OperationContract]
        ModuleVersion GetVersion();

        #endregion
    }

    #endregion Интерфейс IWcfHost

    #region Интерфес IWcfHostCallback

    public interface IWcfHostCallback
    {
        #region Методы

        [OperationContract(IsOneWay = true)]
        void OnCallback(int code, Object data);

        #endregion
    }

    #endregion Интерфес IWcfHostCallback
}