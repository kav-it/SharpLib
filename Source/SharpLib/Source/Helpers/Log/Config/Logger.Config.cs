using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpLib.Log
{
    [XmlRoot("Config")]
    public class LoggerConfig
    {
        #region Свойства

        public LogLevel Level { get; set; }

        [XmlArray("Modules")]
        [XmlArrayItem("Module")]
        public List<LoggerConfigModule> Modules { get; set; }

        [XmlArray("Targets")]
        [XmlArrayItem("Target")]
        public List<LoggerConfigTarget> Targets { get; set; }

        #endregion

        #region Конструктор

        public LoggerConfig()
        {
            Modules = new List<LoggerConfigModule>();
            Targets = new List<LoggerConfigTarget>();
        }

        #endregion
    }
}