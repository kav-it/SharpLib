using System;

namespace SharpLib
{
    public class EnviromentsFolders
    {
        #region Свойства

        public string Desktop { get; private set; }

        #endregion

        #region Конструктор

        public EnviromentsFolders()
        {
            Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        #endregion
    }
}