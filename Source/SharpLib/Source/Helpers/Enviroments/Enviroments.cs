namespace SharpLib
{
    public static class Enviroments
    {
        #region Свойства

        public static EnviromentsFolders Folders { get; private set; }

        #endregion

        #region Конструктор

        static Enviroments()
        {
            Folders = new EnviromentsFolders();
        }

        #endregion
    }
}