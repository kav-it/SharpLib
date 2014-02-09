// ****************************************************************************
//
// Имя файла    : 'Mapi.cs'
// Заголовок    : Класс работы через MAPI (Messaging Application Programming Interface)
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 20/09/2012
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SharpLib
{

    #region Структура MapiMessage

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MapiMessage
    {
        public int Reserved;

        public String Subject;

        public String NoteText;

        public String MessageTyp;

        public String DateReceived;

        public String ConversationID;

        public int Flags;

        internal IntPtr Originator;

        public int RecipCount;

        internal IntPtr Recips;

        public int FileCount;

        internal IntPtr Files;
    }

    #endregion Структура MapiMessage

    #region Структура MapiFileDesc

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MapiFileDesc
    {
        public int Reserved;

        public int Flags;

        public int Position;

        public String Path;

        public String Name;

        internal IntPtr Typ;
    }

    #endregion Структура MapiFileDesc

    #region Структура MapiRecipDesc

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MapiRecipDesc
    {
        #region Поля

        public int Reserved;

        public int RecipClass;

        public String Name;

        public String Address;

        public int IdSize;

        internal IntPtr EntryID;

        #endregion Поля
    }

    #endregion Структура MapiRecipDesc

    #region Класс Mailer

    public class Mailer
    {
        #region Перечисления

        private enum HowTo
        {
            /// <summary>
            /// Отправитель
            /// </summary>
            MAPI_ORIG = 0,

            /// <summary>
            /// Получатель
            /// </summary>
            MAPI_TO,

            /// <summary>
            /// Получатель-копия
            /// </summary>
            MAPI_COPY,

            /// <summary>
            /// Получатель-копия (скрытая)
            /// </summary>
            MAPI_BLIND
        }

        #endregion

        #region Константы

        private const int MAPI_DIALOG = 0x00000008;

        private const int MAPI_LOGON_UI = 0x00000001;

        private const int MAX_ATTACHMENTS = 20;

        #endregion

        #region Поля

        private readonly String[] _errors;

        private List<String> _attachments;

        private int _lastError;

        private List<MapiRecipDesc> _recipients;

        #endregion

        #region Конструктор

        public Mailer()
        {
            _recipients = new List<MapiRecipDesc>();
            _attachments = new List<String>();
            _lastError = 0;
            _errors = new[]
                {
                    "OK [0]",
                    "User abort [1]",
                    "General MAPI failure [2]",
                    "MAPI login failure [3]",
                    "Disk full [4]",
                    "Insufficient memory [5]",
                    "Access denied [6]",
                    "-unknown- [7]",
                    "Too many sessions [8]",
                    "Too many files were specified [9]",
                    "Too many recipients were specified [10]",
                    "A specified attachment was not found [11]",
                    "Attachment open failure [12]",
                    "Attachment write failure [13]",
                    "Unknown recipient [14]",
                    "Bad recipient type [15]",
                    "No messages [16]",
                    "Invalid message [17]",
                    "Text too large [18]",
                    "Invalid session [19]",
                    "Type not supported [20]",
                    "A recipient was specified ambiguously [21]",
                    "Message in use [22]", "Network failure [23]",
                    "Invalid edit fields [24]",
                    "Invalid recipients [25]",
                    "Not supported [26]"
                };
        }

        #endregion

        #region Методы

        private IntPtr GetRecipients(out int recipCount)
        {
            recipCount = 0;
            if (_recipients.Count == 0)
                return IntPtr.Zero;

            int size = Marshal.SizeOf(typeof(MapiRecipDesc));
            IntPtr intPtr = Marshal.AllocHGlobal(_recipients.Count * size);

            int ptr = (int)intPtr;
            foreach (MapiRecipDesc mapiDesc in _recipients)
            {
                Marshal.StructureToPtr(mapiDesc, (IntPtr)ptr, false);
                ptr += size;
            }

            recipCount = _recipients.Count;

            return intPtr;
        }

        private IntPtr GetAttachments(out int fileCount)
        {
            fileCount = 0;
            if (_attachments == null)
                return IntPtr.Zero;

            if ((_attachments.Count <= 0) || (_attachments.Count > MAX_ATTACHMENTS))
                return IntPtr.Zero;

            int size = Marshal.SizeOf(typeof(MapiFileDesc));
            IntPtr intPtr = Marshal.AllocHGlobal(_attachments.Count * size);

            MapiFileDesc mapiFileDesc = new MapiFileDesc();
            mapiFileDesc.Position = -1;
            int ptr = (int)intPtr;

            foreach (String strAttachment in _attachments)
            {
                mapiFileDesc.Name = Path.GetFileName(strAttachment);
                mapiFileDesc.Path = strAttachment;
                Marshal.StructureToPtr(mapiFileDesc, (IntPtr)ptr, false);
                ptr += size;
            }

            fileCount = _attachments.Count;
            return intPtr;
        }

        private bool AddRecipient(String email, HowTo howTo)
        {
            MapiRecipDesc recipient = new MapiRecipDesc();

            recipient.RecipClass = (int)howTo;
            recipient.Name = email;
            _recipients.Add(recipient);

            return true;
        }

        private void Cleanup(ref MapiMessage msg)
        {
            int size = Marshal.SizeOf(typeof(MapiRecipDesc));
            int ptr;

            if (msg.Recips != IntPtr.Zero)
            {
                ptr = (int)msg.Recips;
                for (int i = 0; i < msg.RecipCount; i++)
                {
                    Marshal.DestroyStructure((IntPtr)ptr, typeof(MapiRecipDesc));
                    ptr += size;
                }
                Marshal.FreeHGlobal(msg.Recips);
            }

            if (msg.Files != IntPtr.Zero)
            {
                size = Marshal.SizeOf(typeof(MapiFileDesc));

                ptr = (int)msg.Files;
                for (int i = 0; i < msg.FileCount; i++)
                {
                    Marshal.DestroyStructure((IntPtr)ptr, typeof(MapiFileDesc));
                    ptr += size;
                }
                Marshal.FreeHGlobal(msg.Files);
            }

            _recipients.Clear();
            _attachments.Clear();
        }

        private int SendMail(String strSubject, String strBody, int how)
        {
            MapiMessage msg = new MapiMessage();

            msg.Subject = strSubject;
            msg.NoteText = strBody;

            msg.Recips = GetRecipients(out msg.RecipCount);
            msg.Files = GetAttachments(out msg.FileCount);

            _lastError = NativeMethods.MapiSendMail(new IntPtr(0), new IntPtr(0), msg, how, 0);

            Cleanup(ref msg);

            return _lastError;
        }

        public bool AddRecipientTo(String email)
        {
            return AddRecipient(email, HowTo.MAPI_TO);
        }

        public bool AddRecipientCopy(String email)
        {
            return AddRecipient(email, HowTo.MAPI_COPY);
        }

        public bool AddRecipientBlind(String email)
        {
            return AddRecipient(email, HowTo.MAPI_BLIND);
        }

        public void AddAttachment(String fileName)
        {
            _attachments.Add(fileName);
        }

        public void AddAttachmentAsFile(String fileBody, String fileName)
        {
            String tempFileName = Path.GetTempPath() + fileName;
            File.WriteAllText(tempFileName, fileBody);

            _attachments.Add(tempFileName);
        }

        public Boolean SendMail(String subject, String body)
        {
            int error = SendMail(subject, body, MAPI_LOGON_UI | MAPI_DIALOG);

            return (error == 0);
        }

        public String GetLastError()
        {
            if (_lastError <= 26)
                return _errors[_lastError];

            return "MAPI error [" + _lastError.ToString() + "]";
        }

        #endregion
    }

    #endregion Класс Mailer
}