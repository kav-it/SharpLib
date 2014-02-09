// ****************************************************************************
//
// Имя файла    : 'Locale.cs'
// Заголовок    : Локализация приложения
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 13/12/2012
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SharpLib
{

    #region Перечисление LocaleLanguage

    public enum LocaleLanguage
    {
        Unknow,

        Russian,

        English,

        French,

        German,

        Kazakh,

        Ukrain
    }

    #endregion Перечисление LocaleLanguage

    #region Класс Localizer

    public static class Localizer
    {
        #region Поля

        private static readonly Dictionary<LocaleLanguage, String> _languages = new Dictionary<LocaleLanguage, String>
            {
                {
                    LocaleLanguage.Unknow, ""
                },
                {
                    LocaleLanguage.Russian, "ru-RU"
                },
                {
                    LocaleLanguage.English, "en-US"
                },
                {
                    LocaleLanguage.French, "fr-FR"
                },
                {
                    LocaleLanguage.German, "de-DE"
                },
                {
                    LocaleLanguage.Kazakh, "kk-KZ"
                },
                {
                    LocaleLanguage.Ukrain, "uk-UA"
                },
            };

        private static CultureInfo _culture;

        private static LocaleLanguage _language;

        private static List<XmlDataProvider> _list;

        #endregion

        #region Свойства

        public static LocaleLanguage Language
        {
            get { return _language; }
            set
            {
                if (value != LocaleLanguage.Unknow && value != _language)
                {
                    CultureInfo culture = SearchByLanguage(value);

                    if (culture != null && culture.Name != _culture.Name)
                    {
                        _language = value;
                        _culture = culture;
                        UpdateList();
                    }
                }
            }
        }

        public static CultureInfo Culture
        {
            get { return _culture; }
            private set
            {
                if (value != null && (_culture == null || _culture.Name != value.Name))
                {
                    LocaleLanguage language = SearchByCulture(value);

                    if (language != LocaleLanguage.Unknow && language != _language)
                    {
                        _language = language;
                        _culture = value;
                        UpdateList();
                    }
                }
            }
        }

        #endregion

        #region Конструктор

        static Localizer()
        {
            _list = new List<XmlDataProvider>();
            Culture = CultureInfo.CurrentCulture;
        }

        #endregion

        #region Методы

        private static CultureInfo SearchByLanguage(LocaleLanguage language)
        {
            CultureInfo info = null;
            String text = _languages[language];

            if (text.IsValid())
                info = new CultureInfo(text);

            return info;
        }

        private static LocaleLanguage SearchByCulture(CultureInfo culture)
        {
            String info = culture.Name;

            foreach (var record in _languages)
            {
                if (record.Value.Equals(info))
                    return record.Key;
            }

            return LocaleLanguage.Unknow;
        }

        private static String GetXPath()
        {
            String xpath = String.Format(@"Languages/{0}", _language.ToString());

            return xpath;
        }

        private static void UpdateList()
        {
            String xpath = GetXPath();

            foreach (XmlDataProvider data in _list)
                data.XPath = xpath;
        }

        public static void Register(XmlDataProvider data, String absolutPath = "")
        {
            if (data != null && _list.IndexOf(data) == -1)
            {
                if (absolutPath.IsValid())
                    data.Source = new Uri(@"pack://application:,,,/" + absolutPath, UriKind.Absolute);
                data.XPath = GetXPath();

                _list.Add(data);
            }
        }

        #endregion
    }

    #endregion Класс Localizer

    #region Класс MenuItemLocalizer

    public class MenuItemLocalizer : MenuItemBase
    {
        #region Свойства

        public String Text
        {
            set { Program.BindingLang(this, HeaderedItemsControl.HeaderProperty, value); }
        }

        #endregion

        #region Конструктор

        public MenuItemLocalizer(String resourcePath, RoutedEventHandler click, Object tag) : base("MenuItemLocale", click, tag)
        {
            Text = resourcePath;
        }

        public MenuItemLocalizer(String resourcePath, RoutedEventHandler click) : this(resourcePath, click, null)
        {
        }

        public MenuItemLocalizer(String resourcePath) : this(resourcePath, null, null)
        {
        }

        #endregion
    }

    #endregion Класс MenuItemLocalizer
}