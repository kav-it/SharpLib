using System.ComponentModel;

namespace SharpLib.Net
{
    /// <summary>
    /// Коды ошибок при открытии/закрытии сокета
    /// </summary>
    public enum NetSocketError
    {
        [Description("Ошибка")]
        Unknow = 0,

        [Description("")]
        Ok = 1,

        [Description("Порт занят")]
        Busy = 2
    }
}