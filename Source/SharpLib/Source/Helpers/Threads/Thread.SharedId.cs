namespace SharpLib
{
    /// <summary>
    /// Потокобезопасный общий идентификаторов
    /// </summary>
    internal class SharedId
    {
        #region Свойства

        private object Locker { get; set; }

        private int Id { get; set; }

        #endregion

        #region Конструктор

        public SharedId()
        {
            Locker = new object();
            Id = 0;
        }

        #endregion

        #region Методы

        public int GetNext()
        {
            lock (Locker)
            {
                if (++Id < 0)
                {
                    Id = 0;
                }

                return Id;
            }
        }

        #endregion
    }
}