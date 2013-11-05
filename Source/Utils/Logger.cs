// ****************************************************************************
//
// Имя файла    : 'Logger.cs'
// Заголовок    : Реализация записи лог-файлов
// Автор        : Тихомиров В.С./Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.IO;
using System.Text;

namespace SharpLib
{
    #region Класс Logger
    public class Logger
    {
        #region Поля
        private String _fileName;
        private Object _locker;
        #endregion Поля

        #region Свойства
        public String FileName 
        {
            get { return _fileName; }
        }
        #endregion Свойства

        #region Конструктор
        public Logger()
        {
            _fileName = "";
            _locker   = new object();
        }
        public Logger (String fileName): this()
        {
            _fileName = fileName;
        }
        #endregion Конструктор

        #region Методы
        public void Write (String text)
        {
            if (_fileName != "")
            {
                lock (_locker)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(_fileName, true, Encoding.GetEncoding(1251)))
                        {
                            sw.Write(text);
                        }
                    }
                    catch
                    {
                        // Неудачная запись в файл
                    }
                }
            }
        }
        public void WriteLine(String text)
        {
            Write(text + "\r\n");
        }
        #endregion Методы
    }
    #endregion Класс Logger
}
