// ****************************************************************************
//
// Имя файла    : 'Reger.cs'
// Заголовок    : Модуль работы с реестром
// Автор        : Крыцкий А.В. (на базе http://regexplore.codeplex.com)
// Контакты     : kav.it@mail.ru
// Дата         : 05/01/2013
//
// ****************************************************************************

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpLib
{
    #region Класс Reger
    public static class Reger
    {
        #region Методы
        public static List<RegKey> GetChilds(RegistryKey key)
        {
            int subKeyCount = key.SubKeyCount;
            if (subKeyCount == 0)
                return new List<RegKey>();

            List<RegKey> subKeys = new List<RegKey>(subKeyCount);

            String[] subKeyNames = key.GetSubKeyNames();
            for (int i = 0; i < subKeyNames.Length; i++)
            {
                try
                {
                    String keyName = subKeyNames[i];
                    RegKey item = new RegKey(keyName, key.OpenSubKey(keyName));
                    subKeys.Add(item);
                }
                catch { }
            }

            return subKeys;
        }
        public static List<RegValue> GetValues(RegistryKey key)
        {
            int valueCount = key.ValueCount;
            if (valueCount == 0)
                return new List<RegValue>();

            List<RegValue> values = new List<RegValue>(valueCount);
            String[] valueNames = key.GetValueNames();
            for (int i = 0; i < valueNames.Length; i++)
                values.Add(new RegValue(key, valueNames[i]));

            return values;
        }
        public static String GetRegValueName(String value)
        {
            return value == String.Empty ? "(Default)" : value;
        }
        public static RegistryKey ParseRootKey(String path)
        {
            RegistryKey key;
            switch (path)
            {
                case "HKEY_CLASSES_ROOT": key = Microsoft.Win32.Registry.ClassesRoot; break;
                case "HKEY_CURRENT_USER": key = Microsoft.Win32.Registry.CurrentUser; break;
                case "HKEY_LOCAL_MACHINE": key = Microsoft.Win32.Registry.LocalMachine; break;
                case "HKEY_USERS": key = Microsoft.Win32.Registry.Users; break;
                default: key = Microsoft.Win32.Registry.CurrentConfig; break;
            }
            return key;
        }
        public static void SplitKey(String key, out String hive, out String branch)
        {
            int index = key.IndexOf('\\');
            hive = String.Empty;
            branch = String.Empty;
            if (index == -1)
                hive = key;
            else
            {
                hive = key.Substring(0, index);
                branch = key.Substring(index + 1);
            }
        }
        public static Boolean DeleteKey(String key)
        {
            try
            {
                RegKey child = RegKey.Parse(key);
                RegKey parent = RegKey.Parse(child.Parent, true);
                parent.Key.DeleteSubKeyTree(child.Name);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static Boolean DeleteValue(String key, String value)
        {
            try
            {
                RegKey regKey = RegKey.Parse(key, true);
                regKey.Key.DeleteValue(value, false);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static String ToStringEx(this RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.Binary: return "REG_BINARY";
                case RegistryValueKind.DWord: return "REG_DWORD";
                case RegistryValueKind.ExpandString: return "REG_EXPAND_SZ";
                case RegistryValueKind.MultiString: return "REG_MULTI_SZ";
                case RegistryValueKind.QWord: return "REG_QWORD";
                case RegistryValueKind.String: return "REG_SZ";
                case RegistryValueKind.Unknown: return "REG_UNKNOWN";
                default: return String.Empty;
            }
        }
        #endregion Методы
    }
    #endregion Класс Reger

    #region Класс RegValue
    public class RegValue
    {
        #region Поля
        private String _name;
        #endregion Поля

        #region Свойства
        public String Name
        {
            get
            {
                if (IsDefault)
                    return "(Default)";
                else
                    return _name;
            }
            set { _name = value; }
        }
        public RegistryValueKind Kind { get; set; }
        public Object Data { get; set; }
        public Boolean IsDefault
        {
            get { return _name == String.Empty; }
        }
        public RegistryKey ParentKey { get; private set; }
        #endregion Свойства

        #region Конструктор
        public RegValue(String name, RegistryValueKind kind, Object data)
        {
            this._name = name;
            Kind = kind;
            Data = data;
        }
        public RegValue(RegistryKey parentKey, String valueName): this(valueName, parentKey.GetValueKind(valueName), parentKey.GetValue(valueName))
        {
            ParentKey = parentKey;
        }
        #endregion Конструктор

        #region Методы
        public override String ToString()
        {
            String name  = _name;
            String value = "";

            switch (Kind)
            {
                case RegistryValueKind.Binary:
                    {
                        Byte[] buffer = Data as Byte[];
                        value = buffer.ToAsciiEx();
                    }
                    break;
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString: value = Data.ToString(); break; 
                case RegistryValueKind.MultiString: value = String.Join(" ", (String[])Data); break;
                case RegistryValueKind.DWord:
                    {
                        UInt32 dword = Data.ToUInt32Ex();
                        value = dword.ToStringEx(16);
                    }
                    break;
                case RegistryValueKind.QWord:
                    {
                        UInt64 qword = Data.ToUInt64Ex();
                        value = qword.ToStringEx(16);
                    }
                    break;
                case RegistryValueKind.Unknown:
                default: value = String.Empty; break;
            }

            String text = String.Format("{0} ({1})", name, value);

            return text;
        }
        #endregion Методы
    }
    #endregion Класс RegValue

    #region Класс RegKey
    public class RegKey
    {
        #region Свойства
        public String Name { get; private set; }
        public RegistryKey Key { get; private set; }
        public String Parent { get; private set; }
        #endregion Свойства

        #region Конструктор
        public RegKey(String name, RegistryKey key)
        {
            Name = name;
            Key = key;
            int index = key.Name.Length - name.Length - 1;
            if (index > 0)
                Parent = key.Name.Substring(0, index);
        }
        public RegKey(RegistryKey key): this(key.Name.Contains(@"\") ? key.Name.Substring(key.Name.LastIndexOf(@"\")): key.Name, key) 
        { 
        }
        #endregion Конструктор

        #region Методы
        public static RegKey Parse(String keyPath)
        {
            return Parse(keyPath, false);
        }
        public static RegKey Parse(String keyPath, Boolean writable)
        {
            String[] tokens = keyPath.Split(new Char[] { '\\' }, 2);
            RegistryKey rootKey = Reger.ParseRootKey(tokens[0]);
            if (tokens.Length == 1)
                return new RegKey(rootKey);
            String path = tokens[1];
            String name = keyPath.Substring(keyPath.LastIndexOf('\\') + 1);
            try
            {
                var key = rootKey.OpenSubKey(path, writable);
                if (key == null)
                    return null;
                return new RegKey(name, key);
            }
            catch
            {
                return null;
            }
        }
        public override String ToString()
        {
            return Key.ToString();
        }
        public List<RegValue> GetValues()
        {
            return Reger.GetValues(Key);
        }
        public List<RegKey> GetChilds()
        {
            return Reger.GetChilds(Key);
        }
        #endregion Методы
    }
    #endregion Класс
}
