using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SharpLib
{
    public class ModuleVersion
    {
        #region Поля

        #endregion

        #region Свойства

        public String Title { get; set; }

        public int V1 { get; set; }

        public int V2 { get; set; }

        public int V3 { get; set; }

        public int V4 { get; set; }

        public DateTime DateTime { get; set; }

        public int Build
        {
            get { return V4; }
            set { V4 = value; }
        }

        public string DateTimeText
        {
            get
            {
                string text = DateTime.ToLongDateString() + " " + DateTime.ToLongTimeString();

                return text;
            }
        }

        public bool IsBeta
        {
            get { return (V3 != 0); }
        }

        public string Location { get; set; }

        #endregion

        #region Конструктор

        public ModuleVersion(int v1, int v2, int v3, int v4)
        {
            Title = "Не определено";
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
            DateTime = DateTime.Now;
        }

        public ModuleVersion()
            : this(0, 0, 0, 0)
        {
        }

        public ModuleVersion(String version)
            : this()
        {
            // Возможные форматы
            // "0"                   = 0.0.0.0
            // "0.1"                 = 0.1.0.0
            // "0.1.2"               = 0.1.2.0
            // "0.1.2.3"             = 0.1.2.3
            // "4.8.17 (build 1182)" = 4.8.17.1182

            int index = version.SearchEx(" (build ");
            if (index != -1)
            {
                // Заполнение номера build
                V4 = version.GetIntEx(index);
                // Удаление записи build
                version = version.Substring(0, version.IndexOf(" (build ", StringComparison.Ordinal));
            }

            string[] values = version.SplitEx(".");

            for (int i = 0; i < 4; i++)
            {
                if (i < values.Length)
                {
                    int v = values[i].ToIntEx();
                    switch (i)
                    {
                        case 0:
                            V1 = v;
                            break;
                        case 1:
                            V2 = v;
                            break;
                        case 2:
                            V3 = v;
                            break;
                        case 3:
                            V4 = v;
                            break;
                    }
                }
            }
        }

        public ModuleVersion(Assembly assembly)
            : this()
        {
            UpdateFromAssembly(assembly);
        }

        #endregion

        #region Методы

        private static void Copy(ModuleVersion dest, ModuleVersion source)
        {
            dest.V1 = source.V1;
            dest.V2 = source.V2;
            dest.V3 = source.V3;
            dest.V4 = source.V4;
            dest.Title = source.Title;
            dest.DateTime = new DateTime(source.DateTime.Ticks);
        }

        public static ModuleVersion UpdateFromAssemblyInstance(Assembly assembly = null)
        {
            var version = new ModuleVersion();
            version.UpdateFromAssembly(assembly);

            return version;
        }

        /// <summary>
        /// Инициализация значениями из указанной сборки
        /// </summary>
        public void UpdateFromAssembly(Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetExecutingAssembly();
            }

            FileVersionInfo version = FileVersionInfo.GetVersionInfo(assembly.Location);

            V1 = version.FileMajorPart;
            V2 = version.FileMinorPart;
            V3 = version.FileBuildPart;
            V4 = version.FilePrivatePart;
            Title = version.ProductName;

            FileInfo fileInfo = new FileInfo(assembly.Location);
            DateTime = fileInfo.LastWriteTime;

            Location = assembly.Location;
        }

        /// <summary>
        /// Преобразование в строку (стандартное отображение)
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            String text = String.Format("{0}.{1}", V1, V2);

            if (V3 != 0)
            {
                text += String.Format(".{0} (build {1})", V3, V4);
            }

            return text;
        }

        /// <summary>
        /// Преобразование в строку (компактное представление)
        /// </summary>
        public String ToStringCompact()
        {
            string text = IsBeta ? string.Format("{0}.{1}.{2}", V1, V2, V3) : string.Format("{0}.{1}", V1, V2);

            return text;
        }

        /// <summary>
        /// Представление версии в формете сборки (1.1.0.994)
        /// </summary>
        /// <returns></returns>
        public String ToStringAssembly()
        {
            String text = String.Format("{0}.{1}.{2}.{3}", V1, V2, V3, V4);

            return text;
        }

        /// <summary>
        /// Установка параметров версии (при загрузки из вне)
        /// </summary>
        /// <param name="assemblyVersion">"1.2.1.1"</param>
        /// <param name="title">"Название приложения"</param>
        public void UpdateFromExternal(String assemblyVersion, String title)
        {
            String[] arr = assemblyVersion.Split('.');

            V1 = int.Parse(arr[0]);
            V2 = int.Parse(arr[1]);
            V3 = int.Parse(arr[2]);
            V4 = int.Parse(arr[3]);

            Title = title;
        }

        /// <summary>
        /// Перерасчет версии до производственной
        /// </summary>
        public ModuleVersion UpgradeToRelease()
        {
            ModuleVersion version = new ModuleVersion();

            Copy(version, this);

            if (version.IsBeta)
            {
                if (version.V1 == 0)
                {
                    version.V1 += 1;
                    version.V2 = 0;
                }
                else
                {
                    version.V2 += 1;
                }

                version.V3 = 0;
            }

            return version;
        }

        public Boolean Equals(ModuleVersion other)
        {
            bool result =
                (V1 == other.V1) &&
                (V2 == other.V2) &&
                (V3 == other.V3) &&
                (V4 == other.V4);

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if ((obj is ModuleVersion) == false)
            {
                throw new InvalidCastException("The 'obj' argument is not a ModuleVersion object.");
            }

            return Equals(obj as ModuleVersion);
        }

        public override int GetHashCode()
        {
            int hash1 = V1.GetHashCode();
            int hash2 = V2.GetHashCode();
            int hash3 = V3.GetHashCode();
            int hash4 = V4.GetHashCode();

            return (hash1 ^ hash2 ^ hash3 ^ hash4);
        }

        #endregion

        public static bool operator ==(ModuleVersion a, ModuleVersion b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            bool result = a.Equals(b);

            return result;
        }

        public static bool operator !=(ModuleVersion version1, ModuleVersion version2)
        {
            bool result = !(version1 == version2);

            return result;
        }

        public static bool operator <(ModuleVersion a, ModuleVersion b)
        {
            if (a.V1 < b.V1)
            {
                return true;
            }
            if (a.V1 > b.V1)
            {
                return false;
            }

            if (a.V2 < b.V2)
            {
                return true;
            }
            if (a.V2 > b.V2)
            {
                return false;
            }

            if (a.V3 < b.V3)
            {
                return true;
            }
            if (a.V3 > b.V3)
            {
                return false;
            }

            if (a.V4 < b.V4)
            {
                return true;
            }

            return false;
        }

        public static bool operator >(ModuleVersion a, ModuleVersion b)
        {
            if (a == b)
            {
                return false;
            }
            if (a < b)
            {
                return false;
            }

            return true;
        }

        public static bool operator <=(ModuleVersion a, ModuleVersion b)
        {
            if (a < b)
            {
                return true;
            }
            if (a == b)
            {
                return true;
            }

            return false;
        }

        public static bool operator >=(ModuleVersion a, ModuleVersion b)
        {
            if (a > b)
            {
                return true;
            }
            if (a == b)
            {
                return true;
            }

            return false;
        }
    }
}