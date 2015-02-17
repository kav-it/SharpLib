using System;

namespace SharpLib
{
    /// <summary>
    /// Набор кнопок MessageBoxEx
    /// </summary>
    [Flags]
    public enum MessageBoxExButtons
    {
        Unknown = 0,

        Ok = (1 << 0),

        Cancel = (1 << 1),

        Yes = (1 << 2),

        No = (1 << 3),

        YesToAll = (1 << 4),

        NoToAll = (1 << 5),

        OkCancel = Ok | Cancel,

        YesNo = Yes | No,

        YesNoCancel = Yes | No | Cancel,

        YesNoAll = Yes | No | YesToAll | NoToAll
    }
}