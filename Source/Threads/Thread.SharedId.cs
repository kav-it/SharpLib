// ****************************************************************************
//
// Имя файла    : 'Thread.SharedId.cs'
// Заголовок    : Общий идентификатор
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 02/02/2014
//
// ****************************************************************************
			
namespace SharpLib
{
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
                    Id = 0;

                return Id;
            }
        }

        #endregion
    }
}