// ****************************************************************************
//
// Имя файла    : 'Logger.cs'
// Заголовок    : Реализация записи лог-файлов
// Автор        : Тихомиров В.С./Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

namespace SharpLib.Log
{

    #region Перечисление TargetTyp

    public enum TargetTyp
    {
        Unknow = 0,

        File = 1,

        Database = 2,

        Net = 3,

        Service = 4
    }

    #endregion Перечисление TargetTyp

    #region Класс Target

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

    #endregion Класс Target
}