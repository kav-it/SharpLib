using System.Xml.Serialization;

namespace SharpLib
{
    /// <summary>
    /// Базовый класс xml-конфигурации
    /// </summary>
    public class XmlerConfig
    {
        /// <summary>
        /// Путь к файлу конфигурации
        /// </summary>
        [XmlIgnore]
        public string Filename { get; private set; }

        public XmlerConfig()
        {
            
        }

        public XmlerConfig(string filename)
        {
            Filename = filename;
        }

        public void Fill(object source)
        {
            Reflector.DeepCopy(this, source);
        }

        public void Save()
        {
            Xmler.SaveSerialize(Filename, this);
        }

        public void Load()
        {
            var localConfig = (XmlerConfig)Xmler.LoadSerialize(Filename, GetType());

            if (localConfig == null)
            {
                // Создание объекта конфигурации
                localConfig = (XmlerConfig)Reflector.CreateObject(GetType());
                // Копирование полей
                Reflector.DeepCopy(this, localConfig);
                // Сохранение новой конфигурации
                Save();
            }
            else
            {
                // Копирование полей
                Reflector.DeepCopy(this, localConfig);
            }
        }
    }
}