// ****************************************************************************
//
// Имя файла    : 'Resources.cs'
// Заголовок    : Реализация менеджера работы с ресурсами
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 22/08/2012
//
// ****************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Resources;
using System.Xml;

namespace SharpLib
{

    #region Ресурсы WPF

    #region Класс ResourceElement

    public class ResourceWpfElement
    {
        #region Свойства

        public String Name { get; set; }

        public String Container { get; set; }

        #endregion

        #region Конструктор

        public ResourceWpfElement(String container, String name)
        {
            Container = container;
            Name = name;
        }

        #endregion
    }

    #endregion Класс ResourceElement

    #region Класс ResourcesWpf

    public static class ResourcesWpf
    {
        #region Методы

        public static List<ResourceWpfElement> Enum(Assembly assembly)
        {
            List<ResourceWpfElement> list = new List<ResourceWpfElement>();

            String[] names = assembly.GetManifestResourceNames();
            if (names.Length > 0)
            {
                for (int namesIndex = 0; namesIndex < names.Length; namesIndex++)
                {
                    Stream s = assembly.GetManifestResourceStream(names[namesIndex]);

                    if (s != null)
                    {
                        IResourceReader reader = new ResourceReader(s);
                        IDictionaryEnumerator en = reader.GetEnumerator();

                        while (en.MoveNext())
                        {
                            ResourceWpfElement element = new ResourceWpfElement(names[namesIndex], (String)en.Key);

                            list.Add(element);
                        }
                        reader.Close();
                    }
                }
            }

            return list;
        }

        public static void Print(Assembly assembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetEntryAssembly();

            Console.WriteLine("Enum resources");
            List<ResourceWpfElement> list = Enum(assembly);
            Console.WriteLine("Print resources");
            foreach (ResourceWpfElement element in list)
            {
                String text = String.Format("{0}: {1}", element.Container, element.Name);
                Console.WriteLine(text);
            }
        }

        private static Uri GetFullUri(String absolutPath, Assembly assembly)
        {
            // Регистрация схемы "pack" для определения путей к ресурсам (без создания Application (WPF))
            if (System.IO.Packaging.PackUriHelper.UriSchemePack == null)
                throw new NotImplementedException();

            if (assembly == null)
                assembly = Assembly.GetEntryAssembly();

            absolutPath = absolutPath.TrimStart('/');
            String uriPath = String.Format(@"pack://application:,,,/{0};component/{1}", assembly.GetName().Name, absolutPath);
            Uri uri = new Uri(uriPath, UriKind.Absolute);

            return uri;
        }

        public static StreamResourceInfo LoadStreamResource(String absolutPath, Assembly assembly)
        {
            var uri = GetFullUri(absolutPath, assembly);

            StreamResourceInfo streamResource = Application.GetResourceStream(uri);

            return streamResource;
        }

        public static Stream LoadStream(String absolutPath, Assembly assembly)
        {
            StreamResourceInfo streamResource = LoadStreamResource(absolutPath, assembly);

            if (streamResource == null)
                return null;

            return streamResource.Stream;
        }

        /// <summary>
        /// Загрузка текста из ресурсов 
        /// </summary>
        public static String LoadText(String absolutPath, Assembly asm = null)
        {
            Stream stream = LoadStream(absolutPath, asm);
            String text = stream.ToStringEx();

            return text;
        }

        /// <summary>
        /// Загрузка буфера из ресурсов 
        /// </summary>
        public static Byte[] LoadBuffer(String absolutPath, Assembly asm = null)
        {
            Stream stream = LoadStream(absolutPath, asm);
            Byte[] data = stream.ToByfferEx();

            return data;
        }

        /// <summary>
        /// Копирование файла из ресурсов
        /// </summary>
        public static Boolean CopyContent(String absolutPath, Boolean rewrite = false, String destPath = "")
        {
            // Получение потока нужного ресурса
            Uri uri = GetFullUri(absolutPath, null);

            StreamResourceInfo streamResource;

            try
            {
                streamResource = Application.GetResourceStream(uri);
            }
            catch (Exception)
            {
                return false;
            }

            // Определение имени файла
            String filename = Files.GetFileNameAndExt(absolutPath);

            if (destPath.IsValid())
            {
                destPath = Files.GetPath(destPath);
                // Создание директории (если необходимо)
                if (Directory.Exists(destPath) == false)
                    Files.CreateDir(destPath);

                // Добавление к директории имени файла
                if (Directory.Exists(destPath))
                    filename = destPath + "\\" + filename;
            }

            // Сохранение файла
            if (rewrite || Files.IsExists(filename) == false)
            {
                if (streamResource != null)
                    streamResource.Stream.Save(filename);
            }

            return true;
        }

        /// <summary>
        /// Загрузка изображения из ресурсов в формате BitmapSource
        /// </summary>
        public static ImageSource LoadImageSource(String absolutPath, Assembly asm = null)
        {
            return LoadImage(absolutPath, asm).Source;
        }

        /// <summary>
        /// Загрузка изображения из ресурсов в формате Image
        /// </summary>
        public static Image LoadImage(String absolutPath, Assembly asm = null)
        {
            var stream = LoadStreamResource(absolutPath, asm);

            var bitmap = new System.Windows.Media.Imaging.BitmapImage();
            bitmap.StreamSource = stream.Stream;

            Image image = new Image();
            image.Source = bitmap;

            return image;
        }

        /// <summary>
        /// Загрузка из ресурсов XML-документа
        /// </summary>
        /// <param name="absolutPath"></param>
        /// <returns></returns>
        public static XmlDocument LoadXml(String absolutPath)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            Stream stream = LoadStream(absolutPath, assembly);
            if (stream == null) return null;

            XmlDocument doc = new XmlDocument();
            doc.Load(stream);

            return doc;
        }

        #endregion
    }

    #endregion Класс ResourcesWpf

    #endregion Ресурсы WPF

    #region Ресурсы Windows

    #region Класс ResourcesWin

    public static class ResourcesWin
    {
        #region Свойства

        public static UInt16 NeutralLangId
        {
            get { return MakeLangId(NativeMethods.LANG_NEUTRAL, NativeMethods.SUBLANG_NEUTRAL); }
        }

        public static UInt16 UseEnglishLangId
        {
            get { return MakeLangId(NativeMethods.LANG_ENGLISH, NativeMethods.SUBLANG_ENGLISH_US); }
        }

        #endregion

        #region Методы

        public static UInt16 MakeLangId(int primary, int sub)
        {
            return (UInt16)((((UInt16)sub) << 10) | ((UInt16)primary));
        }

        public static UInt16 PrimaryLangId(UInt16 lcid)
        {
            return (UInt16)((lcid) & 0x3ff);
        }

        public static UInt16 SubLangId(UInt16 lcid)
        {
            return (UInt16)((lcid) >> 10);
        }

        internal static IntPtr Align(Int32 p)
        {
            return new IntPtr((p + 3) & ~3);
        }

        internal static IntPtr Align(IntPtr p)
        {
            return Align(p.ToInt32());
        }

        internal static long PadToWord(BinaryWriter w)
        {
            long pos = w.BaseStream.Position;

            if (pos % 2 != 0)
            {
                long count = 2 - (pos % 2);
                Pad(w, (UInt16)count);
                pos += count;
            }

            return pos;
        }

        internal static long PadToDword(BinaryWriter w)
        {
            long pos = w.BaseStream.Position;

            if (pos % 4 != 0)
            {
                long count = 4 - (pos % 4);
                Pad(w, (UInt16)count);
                pos += count;
            }

            return pos;
        }

        internal static UInt16 HiWord(UInt32 value)
        {
            return (UInt16)((value & 0xFFFF0000) >> 16);
        }

        internal static UInt16 LoWord(UInt32 value)
        {
            return (UInt16)(value & 0x0000FFFF);
        }

        internal static void WriteAt(BinaryWriter w, long value, long address)
        {
            long cur = w.BaseStream.Position;
            w.Seek((int)address, SeekOrigin.Begin);
            w.Write((UInt16)value);
            w.Seek((int)cur, SeekOrigin.Begin);
        }

        internal static long Pad(BinaryWriter w, UInt16 len)
        {
            while (len-- > 0)
                w.Write((byte)0);
            return w.BaseStream.Position;
        }

        internal static Byte[] GetBytes<T>(T anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);
            Byte[] rawdatas = new byte[rawsize];
            Marshal.Copy(buffer, rawdatas, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawdatas;
        }

        internal static List<String> FlagsToList<T>(UInt32 flagValue)
        {
            List<String> flags = new List<String>();

            foreach (T f in Enum.GetValues(typeof(T)))
            {
                UInt32 f_ui = Convert.ToUInt32(f);
                if ((flagValue & f_ui) > 0 || flagValue == f_ui)
                    flags.Add(f.ToString());
            }

            return flags;
        }

        internal static String FlagsToString<T>(UInt32 flagValue)
        {
            List<String> flags = new List<String>();
            flags.AddRange(FlagsToList<T>(flagValue));
            return String.Join(" | ", flags.ToArray());
        }

        /// <summary>
        /// Чтение версии приложения из ресурсов
        /// </summary>
        public static ModuleVersion LoadVersion(String filepath)
        {
            String ext = Files.GetFileExt(filepath);

            if (ext == "exe")
            {
                // Чтение ресурсов из файла
                ResourceWinVersion versionResource = new ResourceWinVersion();
                versionResource.LoadFrom(filepath);

                // Чтение массива таблиц строк (каждый элемент для одной кодировки)
                ResourceWinStringFileInfo stringTableArray = (ResourceWinStringFileInfo)versionResource.Resources["StringFileInfo"];
                // Чтение таблицы строк
                ResourceWinStringTable stringTable = stringTableArray.Default;
                // Чтение версии
                String versionText = stringTable["FileVersion"];
                String titleText = stringTable["ProductName"];

                versionText = versionText.TrimEnd('\0');
                titleText = titleText.TrimEnd('\0');

                // Декодирование строки
                ModuleVersion version = new ModuleVersion();
                // Обновление полей версии
                version.UpdateFromExternal(versionText, titleText);

                return version;
            }

            return null;
        }

        /// <summary>
        /// Установка версии приложения
        /// </summary>
        public static void SaveVersion(String filepath, ModuleVersion version)
        {
            // Чтение ресурсов из файла
            ResourceWinVersion versionResource = new ResourceWinVersion();
            versionResource.LoadFrom(filepath);
            String textVersion = version.ToStringAssembly();

            // Модификация секции ресурсов "VERSION INFO"
            versionResource.FileVersion = textVersion;
            versionResource.ProductVersion = textVersion;

            // ===========================================
            // Модификация строковых значений версии 
            // ===========================================
            // 
            // Чтение массива таблиц строк (каждый элемент для одной кодировки)
            ResourceWinStringFileInfo stringTableArray = (ResourceWinStringFileInfo)versionResource.Resources["StringFileInfo"];

            // Чтение таблицы строк
            ResourceWinStringTable stringTable = stringTableArray.Default;
            stringTable["ProductVersion"] = textVersion;
            stringTable["FileVersion"] = textVersion;
            stringTable["Assembly Version"] = textVersion;

            // Может в будущем пригодится
            // stringTable["ProductName"]     = version;
            // stringTable["FileDescription"] = version;
            // stringTable["LegalCopyright"]  = version;
            // stringTable["Comments"]        = version;
            // ===========================================

            // Сохранение новых значений в ресурсах
            versionResource.SaveTo(filepath);
        }

        #endregion
    }

    #endregion Класс ResourcesWin

    #region Класс ResourceId

    public class ResourceWinId
    {
        #region Поля

        private IntPtr _name = IntPtr.Zero;

        #endregion

        #region Свойства

        public IntPtr Id
        {
            get { return _name; }
            set { _name = IsIntResource(value) ? value : Marshal.StringToHGlobalUni(Marshal.PtrToStringUni(value)); }
        }

        public NativeMethods.ResourceTypes ResourceType
        {
            get
            {
                if (IsIntResource())
                    return (NativeMethods.ResourceTypes)_name;

                throw new InvalidCastException(String.Format(
                                                             "Resource {0} is not of built-in type.", Name));
            }
            set { _name = (IntPtr)value; }
        }

        public String Name
        {
            get { return IsIntResource() ? _name.ToString() : Marshal.PtrToStringUni(_name); }
            set { _name = Marshal.StringToHGlobalUni(value); }
        }

        #endregion

        #region Конструктор

        public ResourceWinId(IntPtr value)
        {
            Id = value;
        }

        public ResourceWinId(uint value)
        {
            Id = new IntPtr(value);
        }

        public ResourceWinId(NativeMethods.ResourceTypes value)
        {
            Id = (IntPtr)value;
        }

        public ResourceWinId(String value)
        {
            Name = value;
        }

        #endregion

        #region Методы

        public Boolean IsIntResource()
        {
            return IsIntResource(_name);
        }

        internal static Boolean IsIntResource(IntPtr value)
        {
            return (uint)value <= UInt16.MaxValue;
        }

        public override String ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return IsIntResource() ? Id.ToInt32() : Name.GetHashCode();
        }

        public override Boolean Equals(object obj)
        {
            if (obj is ResourceWinId && obj == this)
                return true;

            if (obj is ResourceWinId && (obj as ResourceWinId).GetHashCode() == GetHashCode())
                return true;

            return false;
        }

        #endregion
    }

    #endregion Класс ResourceId

    #region Класс ResourceWin

    public abstract class ResourceWin
    {
        #region Поля

        protected UInt16 _language;

        protected ResourceWinId _name;

        protected int _size = 0;

        protected ResourceWinId _type;

        #endregion

        #region Свойства

        public int Size
        {
            get { return _size; }
        }

        public UInt16 Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public ResourceWinId Type
        {
            get { return _type; }
        }

        public String TypeName
        {
            get { return (_type.IsIntResource() ? _type.ResourceType.ToString() : _type.Name); }
        }

        public ResourceWinId Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region Конструктор

        internal ResourceWin()
        {
        }

        internal ResourceWin(IntPtr hModule, IntPtr hResource, ResourceWinId type, ResourceWinId name, UInt16 language, int size)
        {
            _type = type;
            _name = name;
            _language = language;
            _size = size;

            LockAndReadResource(hModule, hResource);
        }

        #endregion

        #region Методы

        internal static void Delete(String filename, ResourceWinId type, ResourceWinId name, UInt16 lang)
        {
            SaveTo(filename, type, name, lang, null);
        }

        internal static void SaveTo(String filename, ResourceWinId type, ResourceWinId name, UInt16 lang, Byte[] data)
        {
            IntPtr h = NativeMethods.BeginUpdateResource(filename, false);

            if (h == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (NativeMethods.UpdateResource(h, type.Id, name.Id, lang, data, (data == null ? 0 : (uint)data.Length)) == false)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (NativeMethods.EndUpdateResource(h, false) == false)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        internal void LockAndReadResource(IntPtr hModule, IntPtr hResource)
        {
            if (hResource == IntPtr.Zero)
                return;

            IntPtr lpRes = NativeMethods.LockResource(hResource);

            if (lpRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Read(hModule, lpRes);
        }

        public virtual void LoadFrom(String filename)
        {
            LoadFrom(filename, _type, _name, _language);
        }

        internal void LoadFrom(String filename, ResourceWinId type, ResourceWinId name, UInt16 lang)
        {
            IntPtr hModule = IntPtr.Zero;

            try
            {
                hModule = NativeMethods.LoadLibraryEx(filename, IntPtr.Zero,
                                                      NativeMethods.DONT_RESOLVE_DLL_REFERENCES | NativeMethods.LOAD_LIBRARY_AS_DATAFILE);

                LoadFrom(hModule, type, name, lang);
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                    NativeMethods.FreeLibrary(hModule);
            }
        }

        internal void LoadFrom(IntPtr hModule, ResourceWinId type, ResourceWinId name, UInt16 lang)
        {
            if (IntPtr.Zero == hModule)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr hRes = NativeMethods.FindResourceEx(hModule, type.Id, name.Id, lang);

            if (IntPtr.Zero == hRes)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr hGlobal = NativeMethods.LoadResource(hModule, hRes);
            if (IntPtr.Zero == hGlobal)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr lpRes = NativeMethods.LockResource(hGlobal);

            if (lpRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            _size = NativeMethods.SizeofResource(hModule, hRes);
            if (_size <= 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            _type = type;
            _name = name;
            _language = lang;

            Read(hModule, lpRes);
        }

        internal abstract IntPtr Read(IntPtr hModule, IntPtr lpRes);

        internal abstract void Write(BinaryWriter w);

        public Byte[] WriteAndGetBytes()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms, Encoding.Default);
            Write(w);
            w.Close();
            return ms.ToArray();
        }

        public virtual void SaveTo(String filename)
        {
            SaveTo(filename, _type, _name, _language);
        }

        internal void SaveTo(String filename, ResourceWinId type, ResourceWinId name, UInt16 langid)
        {
            Byte[] data = WriteAndGetBytes();
            SaveTo(filename, type, name, langid, data);
        }

        public virtual void DeleteFrom(String filename)
        {
            Delete(filename, _type, _name, _language);
        }

        #endregion
    }

    #endregion Класс ResourceWin

    #region ResourceWinTableHeader

    public class ResourceWinTableHeader
    {
        #region Поля

        protected NativeMethods.ResourceHeader _header;

        protected String _key;

        #endregion

        #region Свойства

        public String Key
        {
            get { return _key; }
        }

        public NativeMethods.ResourceHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }

        #endregion

        #region Конструктор

        public ResourceWinTableHeader()
        {
        }

        public ResourceWinTableHeader(String key)
        {
            _key = key;
        }

        internal ResourceWinTableHeader(IntPtr lpRes)
        {
            Read(lpRes);
        }

        #endregion

        #region Методы

        internal virtual IntPtr Read(IntPtr lpRes)
        {
            _header = (NativeMethods.ResourceHeader)Marshal.PtrToStructure(
                                                                           lpRes, typeof(NativeMethods.ResourceHeader));

            IntPtr pBlockKey = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));
            _key = Marshal.PtrToStringUni(pBlockKey);

            return ResourcesWin.Align(pBlockKey.ToInt32() + ((_key.Length + 1) * Marshal.SystemDefaultCharSize));
        }

        internal virtual void Write(BinaryWriter w)
        {
            // wLength
            w.Write(_header.Length);
            // wValueLength
            w.Write(_header.ValueLength);
            // wType
            w.Write(_header.Typ);
            // write key
            w.Write(Encoding.Unicode.GetBytes(_key));
            // null-terminator
            w.Write((UInt16)0);
            // pad fixed info
            ResourcesWin.PadToDword(w);
        }

        public override String ToString()
        {
            return ToString(0);
        }

        public virtual String ToString(int indent)
        {
            return base.ToString();
        }

        #endregion
    }

    #endregion ResourceWinTableHeader

    #region Класс ResourceWinVersion

    public class ResourceWinVersion : ResourceWin
    {
        #region Поля

        private ResourceWinFixedFileInfo _fixedfileinfo = new ResourceWinFixedFileInfo();

        private ResourceWinTableHeader _header = new ResourceWinTableHeader("VS_VERSION_INFO");

        private Dictionary<String, ResourceWinTableHeader> _resources = new Dictionary<String, ResourceWinTableHeader>();

        #endregion

        #region Свойства

        public ResourceWinTableHeader Header
        {
            get { return _header; }
        }

        public Dictionary<String, ResourceWinTableHeader> Resources
        {
            get { return _resources; }
        }

        public String FileVersion
        {
            get { return _fixedfileinfo.FileVersion; }
            set { _fixedfileinfo.FileVersion = value; }
        }

        public String ProductVersion
        {
            get { return _fixedfileinfo.ProductVersion; }
            set { _fixedfileinfo.ProductVersion = value; }
        }

        public ResourceWinTableHeader this[String key]
        {
            get { return Resources[key]; }
            set { Resources[key] = value; }
        }

        #endregion

        #region Конструктор

        public ResourceWinVersion(IntPtr hModule, IntPtr hResource, ResourceWinId type, ResourceWinId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {
        }

        public ResourceWinVersion()
            : base(IntPtr.Zero,
                   IntPtr.Zero,
                   new ResourceWinId(NativeMethods.ResourceTypes.RT_VERSION),
                   new ResourceWinId(1),
                   ResourcesWin.NeutralLangId,
                   // ResourceUtil.USENGLISHLANGID, 
                   0)
        {
            _header.Header = new NativeMethods.ResourceHeader(_fixedfileinfo.Size);
        }

        #endregion

        #region Методы

        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            _resources.Clear();

            IntPtr pFixedFileInfo = _header.Read(lpRes);

            if (_header.Header.ValueLength != 0)
            {
                _fixedfileinfo = new ResourceWinFixedFileInfo();
                _fixedfileinfo.Read(pFixedFileInfo);
            }

            IntPtr pChild = ResourcesWin.Align(pFixedFileInfo.ToInt32() + _header.Header.ValueLength);

            while (pChild.ToInt32() < (lpRes.ToInt32() + _header.Header.Length))
            {
                ResourceWinTableHeader rc = new ResourceWinTableHeader(pChild);
                switch (rc.Key)
                {
                    case "StringFileInfo":
                        ResourceWinStringFileInfo sr = new ResourceWinStringFileInfo(pChild);
                        rc = sr;
                        break;
                    default:
                        rc = new ResourceWinVarFileInfo(pChild);
                        break;
                }

                _resources.Add(rc.Key, rc);
                pChild = ResourcesWin.Align(pChild.ToInt32() + rc.Header.Length);
            }

            return new IntPtr(lpRes.ToInt32() + _header.Header.Length);
        }

        internal override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            _header.Write(w);

            if (_fixedfileinfo != null)
                _fixedfileinfo.Write(w);

            Dictionary<String, ResourceWinTableHeader>.Enumerator resourceEnum = _resources.GetEnumerator();
            while (resourceEnum.MoveNext())
                resourceEnum.Current.Value.Write(w);

            ResourcesWin.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_fixedfileinfo != null)
                sb.Append(_fixedfileinfo);
            sb.AppendLine("BEGIN");
            Dictionary<String, ResourceWinTableHeader>.Enumerator resourceEnum = _resources.GetEnumerator();
            while (resourceEnum.MoveNext())
                sb.Append(resourceEnum.Current.Value.ToString(1));
            sb.AppendLine("END");
            return sb.ToString();
        }

        #endregion
    }

    #endregion Класс VersionResource

    #region Класс ResourceWinVarFileInfo

    public class ResourceWinVarFileInfo : ResourceWinTableHeader
    {
        #region Поля

        private Dictionary<String, ResourceWinVarTable> _vars = new Dictionary<String, ResourceWinVarTable>();

        #endregion

        #region Свойства

        public Dictionary<String, ResourceWinVarTable> Vars
        {
            get { return _vars; }
        }

        public ResourceWinVarTable Default
        {
            get
            {
                Dictionary<String, ResourceWinVarTable>.Enumerator varsEnum = _vars.GetEnumerator();
                if (varsEnum.MoveNext()) return varsEnum.Current.Value;
                return null;
            }
        }

        public UInt16 this[UInt16 language]
        {
            get { return Default[language]; }
            set { Default[language] = value; }
        }

        #endregion

        #region Конструктор

        public ResourceWinVarFileInfo() : base("VarFileInfo")
        {
            _header.Typ = (UInt16)NativeMethods.ResourceHeaderTyp.StringData;
        }

        internal ResourceWinVarFileInfo(IntPtr lpRes)
        {
            Read(lpRes);
        }

        #endregion

        #region Методы

        internal override IntPtr Read(IntPtr lpRes)
        {
            _vars.Clear();
            IntPtr pChild = base.Read(lpRes);

            while (pChild.ToInt32() < (lpRes.ToInt32() + _header.Length))
            {
                ResourceWinVarTable res = new ResourceWinVarTable(pChild);
                _vars.Add(res.Key, res);
                pChild = ResourcesWin.Align(pChild.ToInt32() + res.Header.Length);
            }

            return new IntPtr(lpRes.ToInt32() + _header.Length);
        }

        internal override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            Dictionary<String, ResourceWinVarTable>.Enumerator varsEnum = _vars.GetEnumerator();
            while (varsEnum.MoveNext())
                varsEnum.Current.Value.Write(w);

            ResourcesWin.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }

        public override String ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("{0}BEGIN", new String(' ', indent)));
            foreach (ResourceWinVarTable var in _vars.Values)
                sb.Append(var.ToString(indent + 1));
            sb.AppendLine(String.Format("{0}END", new String(' ', indent)));
            return sb.ToString();
        }

        #endregion
    }

    #endregion Класс ResourceWinVarFileInfo

    #region Класс ResourceWinVarTable

    public class ResourceWinVarTable : ResourceWinTableHeader
    {
        #region Поля

        private Dictionary<UInt16, UInt16> _languages = new Dictionary<UInt16, UInt16>();

        #endregion

        #region Свойства

        public Dictionary<UInt16, UInt16> Languages
        {
            get { return _languages; }
        }

        public UInt16 this[UInt16 key]
        {
            get { return _languages[key]; }
            set { _languages[key] = value; }
        }

        #endregion

        #region Конструктор

        public ResourceWinVarTable()
        {
        }

        public ResourceWinVarTable(String key) : base(key)
        {
        }

        internal ResourceWinVarTable(IntPtr lpRes)
        {
            Read(lpRes);
        }

        #endregion

        #region Методы

        internal override IntPtr Read(IntPtr lpRes)
        {
            _languages.Clear();
            IntPtr pVar = base.Read(lpRes);

            while (pVar.ToInt32() < (lpRes.ToInt32() + _header.Length))
            {
                NativeMethods.VarHeader var = (NativeMethods.VarHeader)Marshal.PtrToStructure(
                                                                                              pVar, typeof(NativeMethods.VarHeader));
                _languages.Add(var.LanguageIdms, var.CodePageIbm);
                pVar = new IntPtr(pVar.ToInt32() + Marshal.SizeOf(var));
            }

            return new IntPtr(lpRes.ToInt32() + _header.Length);
        }

        internal override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            Dictionary<UInt16, UInt16>.Enumerator languagesEnum = _languages.GetEnumerator();
            long valuePos = w.BaseStream.Position;
            while (languagesEnum.MoveNext())
            {
                // id
                w.Write(languagesEnum.Current.Key);
                // code page
                w.Write(languagesEnum.Current.Value);
            }

            ResourcesWin.WriteAt(w, w.BaseStream.Position - valuePos, headerPos + 2);
            ResourcesWin.PadToDword(w);
            ResourcesWin.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }

        public override String ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("{0}BEGIN", new String(' ', indent)));
            Dictionary<UInt16, UInt16>.Enumerator languagesEnumerator = _languages.GetEnumerator();
            while (languagesEnumerator.MoveNext())
            {
                sb.AppendLine(String.Format("{0}VALUE \"Translation\", 0x{1:x}, 0x{2:x}",
                                            new String(' ', indent + 1), languagesEnumerator.Current.Key, languagesEnumerator.Current.Value));
            }
            sb.AppendLine(String.Format("{0}END", new String(' ', indent)));
            return sb.ToString();
        }

        #endregion
    }

    #endregion Класс ResourceWinVarTable

    #region Класс ResourceWinFixedFileInfo

    public class ResourceWinFixedFileInfo
    {
        #region Поля

        private NativeMethods.VsFixedFileInfo _fixedfileinfo = NativeMethods.VsFixedFileInfo.GetWindowsDefault();

        #endregion

        #region Свойства

        public NativeMethods.VsFixedFileInfo Value
        {
            get { return _fixedfileinfo; }
        }

        public String FileVersion
        {
            get
            {
                return String.Format("{0}.{1}.{2}.{3}",
                                     ResourcesWin.HiWord(_fixedfileinfo.FileVersionMs),
                                     ResourcesWin.LoWord(_fixedfileinfo.FileVersionMs),
                                     ResourcesWin.HiWord(_fixedfileinfo.FileVersionLs),
                                     ResourcesWin.LoWord(_fixedfileinfo.FileVersionLs));
            }
            set
            {
                UInt32 major = 0, minor = 0, build = 0, release = 0;
                String[] version_s = value.Split(".".ToCharArray(), 4);
                if (version_s.Length >= 1) major = UInt32.Parse(version_s[0]);
                if (version_s.Length >= 2) minor = UInt32.Parse(version_s[1]);
                if (version_s.Length >= 3) build = UInt32.Parse(version_s[2]);
                if (version_s.Length >= 4) release = UInt32.Parse(version_s[3]);
                _fixedfileinfo.FileVersionMs = (major << 16) + minor;
                _fixedfileinfo.FileVersionLs = (build << 16) + release;
            }
        }

        public String ProductVersion
        {
            get
            {
                return String.Format("{0}.{1}.{2}.{3}",
                                     ResourcesWin.HiWord(_fixedfileinfo.ProductVersionMs),
                                     ResourcesWin.LoWord(_fixedfileinfo.ProductVersionMs),
                                     ResourcesWin.HiWord(_fixedfileinfo.ProductVersionLs),
                                     ResourcesWin.LoWord(_fixedfileinfo.ProductVersionLs));
            }
            set
            {
                UInt32 major = 0, minor = 0, build = 0, release = 0;
                String[] version_s = value.Split(".".ToCharArray(), 4);
                if (version_s.Length >= 1) major = UInt32.Parse(version_s[0]);
                if (version_s.Length >= 2) minor = UInt32.Parse(version_s[1]);
                if (version_s.Length >= 3) build = UInt32.Parse(version_s[2]);
                if (version_s.Length >= 4) release = UInt32.Parse(version_s[3]);
                _fixedfileinfo.ProductVersionMs = (major << 16) + minor;
                _fixedfileinfo.ProductVersionLs = (build << 16) + release;
            }
        }

        public UInt16 Size
        {
            get { return (UInt16)Marshal.SizeOf(_fixedfileinfo); }
        }

        #endregion

        #region Методы

        internal void Read(IntPtr lpRes)
        {
            _fixedfileinfo = (NativeMethods.VsFixedFileInfo)Marshal.PtrToStructure(lpRes, typeof(NativeMethods.VsFixedFileInfo));
        }

        public void Write(BinaryWriter w)
        {
            w.Write(ResourcesWin.GetBytes(_fixedfileinfo));
            ResourcesWin.PadToDword(w);
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("FILEVERSION {0},{1},{2},{3}",
                                        ResourcesWin.HiWord(_fixedfileinfo.FileVersionMs),
                                        ResourcesWin.LoWord(_fixedfileinfo.FileVersionMs),
                                        ResourcesWin.HiWord(_fixedfileinfo.FileVersionLs),
                                        ResourcesWin.LoWord(_fixedfileinfo.FileVersionLs)));
            sb.AppendLine(String.Format("PRODUCTVERSION {0},{1},{2},{3}",
                                        ResourcesWin.HiWord(_fixedfileinfo.ProductVersionMs),
                                        ResourcesWin.LoWord(_fixedfileinfo.ProductVersionMs),
                                        ResourcesWin.HiWord(_fixedfileinfo.ProductVersionLs),
                                        ResourcesWin.LoWord(_fixedfileinfo.ProductVersionLs)));
            if (_fixedfileinfo.FileFlagsMask == NativeMethods.ResourceWinVer.VS_FFI_FILEFLAGSMASK)
                sb.AppendLine("FILEFLAGSMASK VS_FFI_FILEFLAGSMASK");
            else
            {
                sb.AppendLine(String.Format("FILEFLAGSMASK 0x{0:x}",
                                            _fixedfileinfo.FileFlagsMask.ToString()));
            }
            sb.AppendLine(String.Format("FILEFLAGS {0}",
                                        _fixedfileinfo.FileFlags == 0 ? "0" : ResourcesWin.FlagsToString<NativeMethods.ResourceWinVer.FileFlags>(_fixedfileinfo.FileFlags)));
            sb.AppendLine(String.Format("FILEOS {0}",
                                        ResourcesWin.FlagsToString<NativeMethods.ResourceWinVer.FileOs>(_fixedfileinfo.FileFlags)));
            sb.AppendLine(String.Format("FILETYPE {0}",
                                        ResourcesWin.FlagsToString<NativeMethods.ResourceWinVer.FileType>(_fixedfileinfo.FileTyp)));
            sb.AppendLine(String.Format("FILESUBTYPE {0}",
                                        ResourcesWin.FlagsToString<NativeMethods.ResourceWinVer.FileSubType>(_fixedfileinfo.FileSubtyp)));
            return sb.ToString();
        }

        #endregion
    }

    #endregion Класс ResourceWinFixedFileInfo

    #region Класс ResourceWinStringFileInfo

    public class ResourceWinStringFileInfo : ResourceWinTableHeader
    {
        #region Поля

        private Dictionary<String, ResourceWinStringTable> _strings = new Dictionary<String, ResourceWinStringTable>();

        #endregion

        #region Свойства

        public Dictionary<String, ResourceWinStringTable> Strings
        {
            get { return _strings; }
        }

        public ResourceWinStringTable Default
        {
            get
            {
                Dictionary<String, ResourceWinStringTable>.Enumerator iter = _strings.GetEnumerator();
                if (iter.MoveNext()) return iter.Current.Value;
                return null;
            }
        }

        public String this[String key]
        {
            get { return Default[key]; }
            set { Default[key] = value; }
        }

        #endregion

        #region Конструктор

        public ResourceWinStringFileInfo() : base("StringFileInfo")
        {
            _header.Typ = (UInt16)NativeMethods.ResourceHeaderTyp.StringData;
        }

        internal ResourceWinStringFileInfo(IntPtr lpRes)
        {
            Read(lpRes);
        }

        #endregion

        #region Методы

        internal override IntPtr Read(IntPtr lpRes)
        {
            _strings.Clear();
            IntPtr pChild = base.Read(lpRes);

            while (pChild.ToInt32() < (lpRes.ToInt32() + _header.Length))
            {
                ResourceWinStringTable res = new ResourceWinStringTable(pChild);
                _strings.Add(res.Key, res);
                pChild = ResourcesWin.Align(pChild.ToInt32() + res.Header.Length);
            }

            return new IntPtr(lpRes.ToInt32() + _header.Length);
        }

        internal override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            Dictionary<String, ResourceWinStringTable>.Enumerator stringsEnum = _strings.GetEnumerator();
            while (stringsEnum.MoveNext())
                stringsEnum.Current.Value.Write(w);

            ResourcesWin.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
            ResourcesWin.PadToDword(w);
        }

        public override String ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("{0}BEGIN", new String(' ', indent)));
            sb.AppendLine(String.Format("{0}BLOCK \"{1}\"", new String(' ', indent + 1), _key));
            foreach (ResourceWinStringTable stringTable in _strings.Values)
                sb.Append(stringTable.ToString(indent + 1));
            sb.AppendLine(String.Format("{0}END", new String(' ', indent)));
            return sb.ToString();
        }

        #endregion
    }

    #endregion Класс ResourceWinStringFileInfo

    #region Класс ResourceWinStringTable

    public class ResourceWinStringTable : ResourceWinTableHeader
    {
        #region Поля

        private Dictionary<String, ResourceWinStringTableEntry> _strings = new Dictionary<String, ResourceWinStringTableEntry>();

        #endregion

        #region Свойства

        public Dictionary<String, ResourceWinStringTableEntry> Strings
        {
            get { return _strings; }
        }

        public UInt16 LanguageID
        {
            get
            {
                if (String.IsNullOrEmpty(_key))
                    return 0;

                return Convert.ToUInt16(_key.Substring(0, 4), 16);
            }
            set { _key = String.Format("{0:x4}{1:x4}", value, CodePage); }
        }

        public UInt16 CodePage
        {
            get
            {
                if (String.IsNullOrEmpty(_key))
                    return 0;

                return Convert.ToUInt16(_key.Substring(4, 4), 16);
            }
            set { _key = String.Format("{0:x4}{1:x4}", LanguageID, value); }
        }

        public String this[String key]
        {
            get { return _strings[key].Value; }
            set
            {
                ResourceWinStringTableEntry sr = null;
                if (_strings.TryGetValue(key, out sr) == false)
                {
                    sr = new ResourceWinStringTableEntry(key);
                    _strings.Add(key, sr);
                }

                sr.Value = value;
            }
        }

        #endregion

        #region Конструктор

        public ResourceWinStringTable()
        {
        }

        public ResourceWinStringTable(String key) : base(key)
        {
            _header.Typ = (UInt16)NativeMethods.ResourceHeaderTyp.StringData;
        }

        internal ResourceWinStringTable(IntPtr lpRes)
        {
            Read(lpRes);
        }

        #endregion

        #region Методы

        internal override IntPtr Read(IntPtr lpRes)
        {
            _strings.Clear();
            IntPtr pChild = base.Read(lpRes);

            while (pChild.ToInt32() < (lpRes.ToInt32() + _header.Length))
            {
                ResourceWinStringTableEntry res = new ResourceWinStringTableEntry(pChild);
                _strings.Add(res.Key, res);
                pChild = ResourcesWin.Align(pChild.ToInt32() + res.Header.Length);
            }

            return new IntPtr(lpRes.ToInt32() + _header.Length);
        }

        internal override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            int total = _strings.Count;
            Dictionary<String, ResourceWinStringTableEntry>.Enumerator stringsEnum = _strings.GetEnumerator();
            while (stringsEnum.MoveNext())
            {
                stringsEnum.Current.Value.Write(w);
                ResourcesWin.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
                // total parent structure size must not include padding
                if (--total != 0) ResourcesWin.PadToDword(w);
            }
        }

        public override String ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("{0}BEGIN", new String(' ', indent)));
            sb.AppendLine(String.Format("{0}BLOCK \"{1}\"", new String(' ', indent + 1), _key));
            sb.AppendLine(String.Format("{0}BEGIN", new String(' ', indent + 1)));
            foreach (ResourceWinStringTableEntry resourceText in _strings.Values)
            {
                sb.AppendLine(String.Format("{0}VALUE \"{1}\", \"{2}\"",
                                            new String(' ', indent + 2),
                                            resourceText.Key, resourceText.StringValue));
            }
            sb.AppendLine(String.Format("{0}END", new String(' ', indent + 1)));
            sb.AppendLine(String.Format("{0}END", new String(' ', indent)));
            return sb.ToString();
        }

        #endregion
    }

    #endregion Класс ResourceWinStringTable

    #region Класс ResourceWinStringTable

    public class ResourceWinStringTableEntry
    {
        #region Поля

        private NativeMethods.ResourceHeader _header;

        private String _key;

        private String _value;

        #endregion

        #region Свойства

        public NativeMethods.ResourceHeader Header
        {
            get { return _header; }
        }

        public String Key
        {
            get { return _key; }
        }

        public String StringValue
        {
            get
            {
                if (_value == null)
                    return _value;

                return _value.Substring(0, _value.Length - 1);
            }
        }

        public String Value
        {
            get { return _value; }
            set
            {
                if (value == null)
                {
                    _value = null;
                    _header.ValueLength = 0;
                }
                else
                {
                    if (value.Length == 0 || value[value.Length - 1] != '\0')
                        _value = value + '\0';
                    else
                        _value = value;

                    _header.ValueLength = (UInt16)_value.Length;
                }
            }
        }

        #endregion

        #region Конструктор

        public ResourceWinStringTableEntry(String key)
        {
            _key = key;
            _header.Typ = 1;
            _header.Length = 0;
            _header.ValueLength = 0;
        }

        internal ResourceWinStringTableEntry(IntPtr lpRes)
        {
            Read(lpRes);
        }

        #endregion

        #region Методы

        internal void Read(IntPtr lpRes)
        {
            _header = (NativeMethods.ResourceHeader)Marshal.PtrToStructure(
                                                                           lpRes, typeof(NativeMethods.ResourceHeader));

            IntPtr pKey = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));
            _key = Marshal.PtrToStringUni(pKey);

            IntPtr pValue = ResourcesWin.Align(
                                               pKey.ToInt32() + ((_key.Length + 1) * Marshal.SystemDefaultCharSize));
            _value = ((_header.ValueLength > 0)
                          ? Marshal.PtrToStringUni(pValue, _header.ValueLength)
                          : null);
        }

        internal void Write(BinaryWriter w)
        {
            // write the block info
            long headerPos = w.BaseStream.Position;
            // wLength
            w.Write(_header.Length);
            // wValueLength
            w.Write(_header.ValueLength);
            // wType
            w.Write(_header.Typ);
            // szKey
            w.Write(Encoding.Unicode.GetBytes(_key));
            // null terminator
            w.Write((UInt16)0);
            // pad fixed info
            ResourcesWin.PadToDword(w);
            long valuePos = w.BaseStream.Position;
            if (_value != null)
            {
                // value (always double-null-terminated)
                w.Write(Encoding.Unicode.GetBytes(_value));
            }
            // wValueLength
            ResourcesWin.WriteAt(w, (w.BaseStream.Position - valuePos) / Marshal.SystemDefaultCharSize, headerPos + 2);
            // wLength
            ResourcesWin.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }

        #endregion
    }

    #endregion Класс ResourceWinStringTable

    #endregion Windows
}