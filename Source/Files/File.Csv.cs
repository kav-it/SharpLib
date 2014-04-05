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
using System.Text;

namespace SharpLib.Cvs
{

    #region Класс FileCsv

    public class FileCsv
    {
        private const string InternalDelimetr = "\u0001";

        private const string InternalNewLine = "\u0002";

        #region Constructors

        public FileCsv()
        {
            Config = new CsvConfig();
            Header = null;
            Records = new List<CsvRecord>();
        }

        #endregion

        #region Properties

        public CsvConfig Config { get; set; }

        public List<string> Header { get; set; }

        public List<CsvRecord> Records { get; set; }

        #endregion

        #region Methods

        private string ReplaceMasked(string text, out string delimetrLine, out string delimetrBlock)
        {
            // По спецификации rfc4180 блоки, содержащие 
            //  + символы разделителей колонок (например ; или ,)
            //  + символы разделителей строк (пример <CR><LF> или <LF>)
            // должны обрамляться символами <"> (двойные кавычки)
            // 
            // "aaa","bbb","ccc ; , "" "
            // "ddd","eee","fff"

            string quote = Config.Quote;
            string doubleQuote = Config.Quote + Config.Quote;

            delimetrBlock = Config.Delimiter;
            delimetrLine = Config.NewLine;

            if (text.Contains(quote) == false)
                return text;

            var builder = new StringBuilder(text.Length * 2);

            int indexStart = 0;

            while (true)
            {
                int indexQuote = text.SearchEx(Config.Quote, indexStart);
                string block;
                string blockQuotes = null;

                if (indexQuote == -1)
                {
                    // Не найдено 
                    block = text.SubstringEx(indexStart, text.Length);
                }
                else
                {
                    block = text.SubstringEx(indexStart, indexQuote - 1);

                    // Найден разделитель, обрамляющий запись, поиск закрывающей кавычки
                    // Пример: ...,".......""......","..............."<CR><LF>
                    //             |<-- элемент -->| |<-- элемент -->|

                    // Поиск завершающей последовательности
                    bool maybeEnd = false;
                    int indexEnd = -1;

                    for (int index = indexQuote; index < text.Length; index++)
                    {
                        char ch = text[index];

                        if (ch == Config.Quote[0])
                        {
                            maybeEnd = !maybeEnd;
                        }
                        else
                        {
                            // Найден символ после символа <">
                            if (maybeEnd)
                            {
                                indexEnd = index;
                                break;
                            }
                        }
                    }

                    if (indexEnd == -1)
                    {
                        // Не найдено корректного окончания. Берется вся строка до конца
                        indexEnd = text.Length;
                    }

                    // После поиска индексы указывают на позицию после искомой, поэтому нужно сдвинуть на -1
                    indexStart = indexEnd;

                    blockQuotes = text.SubstringEx(indexQuote - 1, indexEnd);
                    blockQuotes = blockQuotes.Replace(doubleQuote, quote).TrimEx(quote);
                }

                if (block.IsValid())
                {
                    block = block
                        .Replace(Config.NewLine, InternalNewLine)
                        .Replace(Config.Delimiter, InternalDelimetr);

                    builder.Append(block);

                    delimetrBlock = InternalDelimetr;
                    delimetrLine = InternalNewLine;
                }

                if (blockQuotes.IsValid())
                {
                    builder.Append(blockQuotes);
                }

                // Выход из цикла
                if (blockQuotes.IsNotValid())
                    break;
            }

            return builder.ToString();
        }

        private string GenerateBlock(string block)
        {
                // Если текст содержит встроенные символы, обрамление строки в Qutes
            if (block.Contains(Config.Delimiter) ||
                block.Contains(Config.NewLine))
            {
                block = block.Replace(Config.Delimiter, Config.Delimiter + Config.Delimiter);

                block = string.Format("{0}{1}{2}", Config.Delimiter, block, Config.Delimiter);
            }

            return block;
        }

        private void GenerateLine(StringBuilder builder, List<string> blocks)
        {
            int count = blocks.Count;

            for (int i = 0; i < count; i++)
            {
                string block = GenerateBlock(blocks[i]);

                builder.Append(block);

                if ((i + 1) != count)
                    builder.Append(Config.Delimiter);
            }
        }

        public void ParseText(string text)
        {
            Records = new List<CsvRecord>();
            Header = null;

            // Замена в тексте экранированных блоков ""
            string delimetrBlock;
            string delimetrLine;
            text = ReplaceMasked(text, out delimetrLine, out delimetrBlock);

            // Разделение на строки (используется встроенный разделитель)
            var lines = text.SplitEx(delimetrLine);

            foreach (var line in lines)
            {
                // Разделение строк на элементы
                var values = line.SplitEx(delimetrBlock).ToList();

                // Заполнение записи данными
                if (Config.IsHasHeader && Header == null)
                {
                    Header = values;
                }
                else
                {
                    var record = new CsvRecord(Header)
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

        public string SaveText()
        {
            StringBuilder builder = new StringBuilder();

            if (Header != null)
            {
                GenerateLine(builder, Header);
                builder.Append(Config.NewLine);
            }

            foreach (var record in Records)
            {
                GenerateLine(builder, record.Values);
                builder.Append(Config.NewLine);
            }

            return builder.ToString();
        }

        public void SaveFile(string filename)
        {
            string text = SaveText();

            Files.WriteText(filename, text);
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
                        try
                        {
                            prop.SetValueEx(objectClass, value);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                // Добавление объекта в список
                result.Add(objectClass as T);
            }

            return result;
        }

        private List<CsvRecord> PerformRecords(IEnumerable<T> records)
        {
            List<CsvRecord> result = new List<CsvRecord>();

            // Составление списка имен
            var typeClass = typeof(T);
            var properties = typeClass.GetProperties();

            List<string> header = properties.Select(x => x.Name).ToList();

            foreach (var record in records)
            {
                var csvRecord = new CsvRecord(header);

                foreach (var prop in properties)
                {
                    var value = prop.GetValueEx(record);

                    csvRecord[prop.Name] = value.ToString();
                }

                // Добавление объекта в список
                result.Add(csvRecord);
            }

            return result;
        }

        public List<T> ParseTextInstance(string text, string delimetr = null)
        {
            if (delimetr != null)
            {
                Config.Delimiter = delimetr;
            }

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

        public string SaveTextInstance(List<T> values, string delimetr = null)
        {
            if (delimetr != null)
            {
                Config.Delimiter = delimetr;
            }

            // Создание класса
            FileCsv csv = new FileCsv
            {
                Config = Config
            };

            csv.Records = PerformRecords(values);
            csv.Header = csv.Records.Count > 0 ? csv.Records[0].Header : null;

            return csv.SaveText();
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

        public static string SaveText(List<T> values, string delimetr = null)
        {
            var csv = new FileCsv<T>();

            return csv.SaveTextInstance(values, delimetr);
        }

        public static void SaveFile(string filename, List<T> values, string delimetr = null)
        {
            string result = SaveText(values, delimetr);

            Files.WriteText(filename, result);
        }

        #endregion
    }

    #endregion Класс FileCsv<T>

    #region Класс CsvConfig

    public class CsvConfig
    {
        #region Constants

        private const string CommentDefault = "#";

        private const string DelimetrDefault = ",";

        private const string QuoteDefault = "\"";

        #endregion

        #region Constructors

        public CsvConfig()
        {
            NewLine = Environment.NewLine;
            Delimiter = DelimetrDefault;
            Quote = QuoteDefault;
            Comment = CommentDefault;
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

    #region Класс CsvRecord

    public class CsvRecord
    {
        #region Constructors

        public CsvRecord(List<string> header)
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
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                int index = Header.IndexOf(indexer);

                if (index == -1)
                    return;
                
                if (Values.Count <= index)
                {
                    for (int i = Values.Count; i <= index; i++)
                    {
                        Values.Add(string.Empty);
                    }
                }

                Values[index] = value;
            }
        }

        #endregion
    }

    #endregion Класс CsvRecord
}