//*****************************************************************************
//
// Имя файла    : 'FileCsv.cs'
// Заголовок    : Формат "Csv"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru 
// Дата         : 03/04/2014
//
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Cvs
{

    #region Класс FileCsv

    public class FileCsv
    {
        #region Constructors

        public FileCsv()
        {
            Config = new CsvConfig();
            Header = null;
            Records = new List<CvsRecord>();
        }

        #endregion

        #region Properties

        public CsvConfig Config { get; set; }

        public List<string> Header { get; set; }

        public List<CvsRecord> Records { get; set; }

        #endregion

        #region Methods

        public void ParseText(string text)
        {
            Records = new List<CvsRecord>();
            Header = null;

            // Разделение на строки
            var lines = text.SplitEx(Config.NewLine);

            foreach (var line in lines)
            {
                // Разделение строк на элементы
                var values = line.SplitEx(Config.Delimiter).ToList();

                // Заполнение записи данными
                if (Config.IsHasHeader && Header == null)
                {
                    Header = values;
                }
                else
                {
                    var record = new CvsRecord(Header)
                    {
                        Values = values
                    };

                    Records.Add(record);
                }
            }
        }

        public void ParseFile(string filename)
        {
            string text = Files.ReadText(filename);

            ParseText(text);
        }

        #endregion
    }

    #endregion Класс FileCsv

    #region Класс FileCsv<T>

    public class FileCsv<T> where T : class
    {
        #region Constructors

        public FileCsv()
        {
            Config = new CsvConfig();
        }

        #endregion

        #region Properties

        public CsvConfig Config { get; set; }

        #endregion

        #region Methods

        private List<T> PerformResult(FileCsv csv)
        {
            List<T> result = new List<T>();

            // Составление списка имен
            var typeClass = typeof (T);
            var properties = typeClass.GetProperties();
            // var names = properties.Select(prop => prop.Name).ToList();

            foreach (var record in csv.Records)
            {
                var objectClass = Reflector.CreateObject(typeClass);

                foreach (var prop in properties)
                {
                    // Чтение значения переменной Csv
                    var value = record[prop.Name];

                    // Установка значения свойства
                    if (value != null)
                    {
                        prop.SetValueEx(objectClass, value);
                    }
                }

                // Добавление объекта в список
                result.Add(objectClass as T);
            }

            return result;
        }

        public List<T> ParseTextInstance(string text, string delimetr = null)
        {
            if (delimetr != null)
                Config.Delimiter = delimetr;

            // Создание класса
            FileCsv csv = new FileCsv { Config = Config };

            // Разбор текста
            csv.ParseText(text);

            // Формирование результата
            return PerformResult(csv);
        }

        public List<T> ParseFileInstance(string filename, string delimetr = null)
        {
            string text = Files.ReadText(filename);

            return ParseTextInstance(text, delimetr);
        }

        public static List<T> ParseFile(string filename, string delimetr = null)
        {
            var csv = new FileCsv<T>();

            return csv.ParseFileInstance(filename, delimetr);
        }

        public static List<T> ParseText(string text, string delimetr = null)
        {
            var csv = new FileCsv<T>();

            return csv.ParseTextInstance(text, delimetr);
        }

        #endregion
    }

    #endregion Класс FileCsv<T>

    #region Класс CsvConfig

    public class CsvConfig
    {
        #region Constructors

        public CsvConfig()
        {
            NewLine = Environment.NewLine;
            Delimiter = ",";
            Quote = "\"";
            Comment = "#";
            IsHasHeader = true;
        }

        #endregion

        #region Properties

        public bool IsHasHeader { get; set; }

        public string Delimiter { get; set; }

        public string Quote { get; set; }

        public string Comment { get; set; }

        public string NewLine { get; set; }

        #endregion
    }

    #endregion Класс CsvConfig

    #region Класс CvsRecord

    public class CvsRecord
    {
        #region Constructors

        public CvsRecord(List<string> header)
        {
            Header = header;
            Values = new List<string>();
        }

        #endregion

        #region Properties

        public List<string> Header { get; private set; }

        public List<string> Values { get; set; }

        public string this[string indexer]
        {
            get
            {
                int index = Header.IndexOf(indexer);

                if (index != -1 && index < Values.Count)
                {
                    return Values[index];
                }

                return null;
            }
        }

        #endregion
    }

    #endregion Класс CvsRecord
}