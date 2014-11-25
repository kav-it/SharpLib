namespace SharpLib.Log
{
    public abstract class Target
    {
        #region Свойства

        public TargetTyp Typ { get; private set; }

        #endregion

        #region Конструктор

        protected Target(TargetTyp typ)
        {
            Typ = typ;
        }

        #endregion

        #region Методы

        public abstract void Write(LogMessage message);

        #endregion
    }
}