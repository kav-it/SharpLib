// ****************************************************************************
//
// Имя файла    : 'WinVars.cs'
// Заголовок    : Переменные окружения
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 12/03/2014
//
// ****************************************************************************
			
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib
{

    #region Класс WinVarsEntry

    public class WinVarsEntry
    {
        #region Свойства

        public String Name { get; private set; }

        public IList<String> Values { get; private set; }

        #endregion

        #region Конструктор

        public WinVarsEntry(String name, IList<String> values)
        {
            Name = name;
            Values = values;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }

    #endregion Класс WinVarsEntry

    #region Класс WinVars

    public class WinVars
    {
        #region Свойства

        public static IList<WinVarsEntry> System
        {
            get
            {
                var dictionary = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);

                var result = Fill(dictionary);

                return result;
            }
        }

        public static IList<WinVarsEntry> User
        {
            get
            {
                var dictionary = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User);

                var result = Fill(dictionary);

                return result;
            }
        }

        public static IList<WinVarsEntry> Process
        {
            get
            {
                var dictionary = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);

                var result = Fill(dictionary);

                // Исключение полей System, User
                var system = System.Select(x => x.Name);
                var user = User.Select(x => x.Name);

                result = result.Where(x => system.Contains(x.Name) == false).ToList();
                result = result.Where(x => user.Contains(x.Name) == false).ToList();

                return result;
            }
        }

        #endregion

        #region Методы

        private static IList<WinVarsEntry> Fill(IDictionary dictionary)
        {
            var result = new List<WinVarsEntry>();

            foreach (DictionaryEntry entry in dictionary)
            {
                var key = entry.Key.ToString();
                var value = entry.Value.ToString();

                var values = Unpack(value);

                var winvarEntry = new WinVarsEntry(key, values);

                result.Add(winvarEntry);
            }

            return result;
        }

        private static List<String> Unpack(String value)
        {
            return value.Split(';').ToList();
        }

        private static String Pack(List<String> values)
        {
            return values.JoinEx(';');
        }

        private static EnvironmentVariableTarget TypToWinTyp(WinVarsTyp typ)
        {
            switch (typ)
            {
                case WinVarsTyp.System: return EnvironmentVariableTarget.Machine;
                case WinVarsTyp.User: return EnvironmentVariableTarget.User;
                case WinVarsTyp.Process: return EnvironmentVariableTarget.Process;
            }

            return EnvironmentVariableTarget.Machine;
        }

        private static List<String> Get(String key, WinVarsTyp typ)
        {
            String text = Environment.GetEnvironmentVariable(key, TypToWinTyp(typ));

            if (text.IsValid())
                return Unpack(text);

            return null;
        }

        private static void Set(String key, WinVarsTyp typ, List<String> values)
        {
            string text = values.Count == 0 ? null : Pack(values);

            Environment.SetEnvironmentVariable(key, text, TypToWinTyp(typ));
        }

        public static Boolean Add(String key, String value, WinVarsTyp typ = WinVarsTyp.System)
        {
            var values = Get(key, typ);
            if (values != null)
            {
                if (value.Contains(value) == false)
                    return true;
            }
            else
                values = new List<String>();

            // Добавление элемента
            values.Add(value);

            // Сохранение нового значения
            Set(key, typ, values);

            return true;
        }

        public static Boolean Delete(String key, String value, WinVarsTyp typ = WinVarsTyp.System)
        {
            var values = Get(key, typ);
            if (values == null) return false;

            if (value.Contains(value) == false)
                return false;

            // Удаление элемента
            values.Remove(value);

            // Сохранение нового значения
            Set(key, typ, values);

            return true;
        }

        public static Boolean Rename(String key, String value, String newValue, WinVarsTyp typ = WinVarsTyp.System)
        {
            if (value == newValue) return true;

            var values = Get(key, typ);
            if (values == null) return false;

            int index = values.IndexOf(value);
            if (index == -1)
                return false;

            // Удаление элемента
            values[index] = newValue;

            // Сохранение нового значения
            Set(key, typ, values);

            return true;
        }

        #endregion
    }

    #endregion Класс WinVars

    #region Перечисление WinVarsTyp

    public enum WinVarsTyp
    {
        Unknow = 0,

        System = 1,

        User = 2,

        Process = 3,
    }

    #endregion Перечисление  WinVarsTyp
}