// ****************************************************************************
//
// Имя файла    : 'Channel.Serial.cs'
// Заголовок    : Класс каналов Serial (RS-232)
// Автор        : Крыцкий А.В./Тихомиров В.С.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.IO;
using System.IO.Ports;

namespace SharpLib
{
    #region Класс ChannelSerial
    public class ChannelSerial : LinkBase
    {
        #region Переменные
        private SerialPort _serialPort;
        #endregion Переменные

        #region Свойства
        public String PortName
        {
            get { return _serialPort.PortName;  }
            set 
            {
                if (_serialPort.IsOpen == false)
                {
                    _serialPort.PortName = value;
                }
            }
        }
        public int Baudrate
        {
            get { return _serialPort.BaudRate;  }
            set { _serialPort.BaudRate = value; }
        }
        public int Databits
        {
            get { return _serialPort.DataBits;  }
            set { _serialPort.DataBits = value; }
        }
        public Parity Parity
        {
            get { return _serialPort.Parity;    }
            set { _serialPort.Parity = value;   }
        }
        public StopBits Stopbits
        {
            get { return _serialPort.StopBits; }
            set { _serialPort.StopBits = value; }
        }
        public SerialPort SerialPort
        {
            get { return _serialPort; }
            set { _serialPort = value; }
        }
        #endregion Свойства
            
        #region Конструктор
        public ChannelSerial() : base(ModuleTyp.ChannelSerial, "Serial")
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = "COM1";
            _serialPort.BaudRate = 115200;
            _serialPort.DataBits = 8;
            _serialPort.Parity   = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DataReceived += OnReceive;
        }
        #endregion Конструктор

        #region Методы
        private void OnReceive(object sender, SerialDataReceivedEventArgs e)
        {
            int count = _serialPort.BytesToRead;
            Byte[] buffer = new Byte[count];

            _serialPort.Read(buffer, 0, count);

            // Трассировка входного буфера
            NotifyReceived(buffer);
            // Передача события
            RaiseDataReceived(buffer);
        }
        protected override String GetLinkName()
        {
            String result = String.Format("{0}", PortName);

            return result;
        }
        protected override Boolean OnOpen()
        {
            try
            {
                _serialPort.Open();
                NotifyOpen(String.Format("{0} открыт, скорость {1}", PortName, Baudrate));

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                LastError = new ModuleError(ModuleErrorCode.AccessDenied);
            }
            catch (ArgumentOutOfRangeException)
            {
                LastError = new ModuleError(ModuleErrorCode.WrongParam);
            }
            catch (ArgumentException)
            {
                LastError = new ModuleError(ModuleErrorCode.WrongPortName);
            }
            catch (InvalidOperationException)
            {
                LastError = new ModuleError(ModuleErrorCode.AlreadyOpen);
            }
            catch (IOException)
            {
                LastError = new ModuleError(ModuleErrorCode.PortNotPresent);
            }
            catch (Exception ex)
            {
                LastError = new ModuleError(ModuleErrorCode.Exception, ex.Message);
            }

            NotifyError(String.Format("Ошибка открытия {0}: {1}", PortName, LastError.Message));

            return false;
        }
        protected override Boolean OnClose()
        {
            try
            {
                _serialPort.Close();
                NotifyClose(String.Format("{0} закрыт", PortName));

                return true;
            }
            catch (IOException ex)
            {
                LastError = new ModuleError(ModuleErrorCode.Exception, ex.Message);
            }
            catch (Exception ex)
            {
                LastError = new ModuleError(ModuleErrorCode.Exception, ex.Message);
            }

            NotifyError(String.Format("Ошибка закрытия {0}: {1}", PortName, LastError.Message));

            return false;
        }
        protected override Boolean OnSend(Byte[] buffer)
        {
            try
            {
                _serialPort.Write(buffer, 0, buffer.Length);

                return true;
            }
            catch (ArgumentNullException)
            {
                LastError = new ModuleError(ModuleErrorCode.BufferIsNull);
            }
            catch (InvalidOperationException)
            {
                LastError = new ModuleError(ModuleErrorCode.NotOpened);
            }
            catch (ArgumentOutOfRangeException)
            {
                LastError = new ModuleError(ModuleErrorCode.WrongParam, "count=" + buffer.Length.ToString());
            }
            catch (ArgumentException)
            {
                LastError = new ModuleError(ModuleErrorCode.WrongParam, "count=" + buffer.Length.ToString());
            }
            catch (TimeoutException ex)
            {
                LastError = new ModuleError(ModuleErrorCode.Exception, ex.Message);
            }
            catch (Exception ex)
            {
                LastError = new ModuleError(ModuleErrorCode.Exception, ex.Message);
            }

            NotifyError(String.Format("Ошибка отправки {0}: {1}", PortName, LastError.Message));

            return false;
        }
        #endregion Методы
    }
    #endregion Класс ChannelSerial
}
