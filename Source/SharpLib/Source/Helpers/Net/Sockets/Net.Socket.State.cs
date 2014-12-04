namespace SharpLib.Net
{
    /// <summary>
    /// Состояния сокетов
    /// </summary>
    public enum NetSocketState
    {
        /// <summary>
        /// Неопределено (внутренее использование)
        /// </summary>
        Unknow = 0,

        /// <summary>
        /// Открыт
        /// </summary>
        Opened = 1,

        /// <summary>
        /// Закрыт
        /// </summary>
        Closed = 2,

        /// <summary>
        /// Прием входящих соединений (только для режима сервера)
        /// </summary>
        Listen = 3,

        /// <summary>
        /// Открывается (в процессе)
        /// </summary>
        Opening = 4,

        /// <summary>
        /// Закрывается (в процессе)
        /// </summary>
        Closing = 5
    }
}