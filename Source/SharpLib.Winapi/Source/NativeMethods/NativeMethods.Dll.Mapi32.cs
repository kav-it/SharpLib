// ****************************************************************************
//
// Имя файла    : 'NativeMethods.cs'
// Заголовок    : Реализация работы с WinApi-функциями
// Автор        : Крыцкий А.В./Тихомиров В.С.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.Winapi
{
    [SuppressUnmanagedCodeSecurity]
    public sealed partial class NativeMethods
    {
        #region Методы

        [DllImport(DLLNAME_MAPI32, SetLastError = true)]
        public static extern int MapiSendMail(IntPtr sess, IntPtr hwnd, MapiMessage message, int flg, int rsv);

        #endregion
    }
}