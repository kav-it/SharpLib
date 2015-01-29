using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Interop;

using SharpLib;

namespace Ookii.Dialogs.Wpf
{
    [DefaultProperty("MainInstruction"), DefaultEvent("UserNameChanged"), Description("Allows access to credential UI for generic credentials.")]
    public partial class CredentialDialog : Component
    {
        #region Поля

        private static readonly Dictionary<string, NetworkCredential> _applicationInstanceCredentialCache = new Dictionary<string, NetworkCredential>();

        private readonly NetworkCredential _credentials = new NetworkCredential();

        private string _caption;

        private string _confirmTarget;

        private bool _isSaveChecked;

        private string _target;

        private string _text;

        private string _windowTitle;

        #endregion

        #region Свойства

        [Category("Behavior"), Description("Indicates whether to use the application instance credential cache."), DefaultValue(false)]
        public bool UseApplicationInstanceCredentialCache { get; set; }

        [Category("Appearance"), Description("Indicates whether the \"Save password\" checkbox is checked."), DefaultValue(false)]
        public bool IsSaveChecked
        {
            get { return _isSaveChecked; }
            set
            {
                _confirmTarget = null;
                _isSaveChecked = value;
            }
        }

        [Browsable(false)]
        public string Password
        {
            get { return _credentials.Password; }
            private set
            {
                _confirmTarget = null;
                _credentials.Password = value;
                OnPasswordChanged(EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public NetworkCredential Credentials
        {
            get { return _credentials; }
        }

        [Browsable(false)]
        public string UserName
        {
            get { return _credentials.UserName ?? string.Empty; }
            private set
            {
                _confirmTarget = null;
                _credentials.UserName = value;
                OnUserNameChanged(EventArgs.Empty);
            }
        }

        [Category("Behavior"), Description("The target for the credentials, typically the server name prefixed by an application-specific identifier."), DefaultValue("")]
        public string Target
        {
            get { return _target ?? string.Empty; }
            set
            {
                _target = value;
                _confirmTarget = null;
            }
        }

        [Localizable(true), Category("Appearance"), Description("The title of the credentials dialog."), DefaultValue("")]
        public string WindowTitle
        {
            get { return _windowTitle ?? string.Empty; }
            set { _windowTitle = value; }
        }

        [Localizable(true), Category("Appearance"), Description("A brief message that will be displayed in the dialog box."), DefaultValue("")]
        public string MainInstruction
        {
            get { return _caption ?? string.Empty; }
            set { _caption = value; }
        }

        [Localizable(true), Category("Appearance"), Description("Additional text to display in the dialog."), DefaultValue(""),
         Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Content
        {
            get { return _text ?? string.Empty; }
            set { _text = value; }
        }

        [Localizable(true), Category("Appearance"), Description("Indicates how the text of the MainInstruction and Content properties is displayed on Windows XP."),
         DefaultValue(DownlevelTextMode.MainInstructionAndContent)]
        public DownlevelTextMode DownlevelTextMode { get; set; }

        [Category("Appearance"), Description("Indicates whether a check box is shown on the dialog that allows the user to choose whether to save the credentials or not."), DefaultValue(false)]
        public bool ShowSaveCheckBox { get; set; }

        [Category("Behavior"), Description("Indicates whether the dialog should be displayed even when saved credentials exist for the specified target."), DefaultValue(false)]
        public bool ShowUIForSavedCredentials { get; set; }

        public bool IsStoredCredential { get; private set; }

        #endregion

        #region События

        [Category("Property Changed"), Description("Event raised when the value of the Password property changes.")]
        public event EventHandler PasswordChanged;

        [Category("Property Changed"), Description("Event raised when the value of the UserName property changes.")]
        public event EventHandler UserNameChanged;

        #endregion

        #region Конструктор

        public CredentialDialog()
        {
            InitializeComponent();
        }

        public CredentialDialog(IContainer container)
        {
            if (container != null)
            {
                container.Add(this);
            }

            InitializeComponent();
        }

        #endregion

        #region Методы

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool ShowDialog()
        {
            return ShowDialog(null);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool ShowDialog(Window owner)
        {
            if (string.IsNullOrEmpty(_target))
            {
                throw new InvalidOperationException("CredentialEmptyTargetError");
            }

            UserName = "";
            Password = "";
            IsStoredCredential = false;

            if (RetrieveCredentialsFromApplicationInstanceCache())
            {
                IsStoredCredential = true;
                _confirmTarget = Target;
                return true;
            }

            bool storedCredentials = false;
            if (ShowSaveCheckBox && RetrieveCredentials())
            {
                IsSaveChecked = true;
                if (!ShowUIForSavedCredentials)
                {
                    IsStoredCredential = true;
                    _confirmTarget = Target;
                    return true;
                }
                storedCredentials = true;
            }

            IntPtr ownerHandle = owner == null ? NativeMethods.GetActiveWindow() : new WindowInteropHelper(owner).Handle;

            bool result = Env.IsWindowsVistaOrLater 
                ? PromptForCredentialsCredUIWin(ownerHandle, storedCredentials) 
                : PromptForCredentialsCredUI(ownerHandle, storedCredentials);

            return result;
        }

        public void ConfirmCredentials(bool confirm)
        {
            if (_confirmTarget == null || _confirmTarget != Target)
            {
                throw new InvalidOperationException("CredentialPromptNotCalled");
            }

            _confirmTarget = null;

            if (IsSaveChecked && confirm)
            {
                if (UseApplicationInstanceCredentialCache)
                {
                    lock (_applicationInstanceCredentialCache)
                    {
                        _applicationInstanceCredentialCache[Target] = new NetworkCredential(UserName, Password);
                    }
                }

                StoreCredential(Target, Credentials);
            }
        }

        public static void StoreCredential(string target, NetworkCredential credential)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (target.Length == 0)
            {
                throw new ArgumentException("The credential target may not be an empty string", "target");
            }
            if (credential == null)
            {
                throw new ArgumentNullException("credential");
            }

            NativeMethods.CREDENTIAL c = new NativeMethods.CREDENTIAL();
            c.UserName = credential.UserName;
            c.TargetName = target;
            c.Persist = NativeMethods.CredPersist.Enterprise;
            byte[] encryptedPassword = EncryptPassword(credential.Password);
            c.CredentialBlob = Marshal.AllocHGlobal(encryptedPassword.Length);
            try
            {
                Marshal.Copy(encryptedPassword, 0, c.CredentialBlob, encryptedPassword.Length);
                c.CredentialBlobSize = (uint)encryptedPassword.Length;
                c.Type = NativeMethods.CredTypes.CRED_TYPE_GENERIC;
                if (!NativeMethods.CredWrite(ref c, 0))
                {
                    throw new CredentialException(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(c.CredentialBlob);
            }
        }

        public static NetworkCredential RetrieveCredential(string target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (target.Length == 0)
            {
                throw new ArgumentException("CredentialEmptyTargetError", "target");
            }

            NetworkCredential cred = RetrieveCredentialFromApplicationInstanceCache(target);
            if (cred != null)
            {
                return cred;
            }

            IntPtr credential;
            bool result = NativeMethods.CredRead(target, NativeMethods.CredTypes.CRED_TYPE_GENERIC, 0, out credential);
            int error = Marshal.GetLastWin32Error();
            if (result)
            {
                try
                {
                    NativeMethods.CREDENTIAL c = (NativeMethods.CREDENTIAL)Marshal.PtrToStructure(credential, typeof(NativeMethods.CREDENTIAL));
                    byte[] encryptedPassword = new byte[c.CredentialBlobSize];
                    Marshal.Copy(c.CredentialBlob, encryptedPassword, 0, encryptedPassword.Length);
                    cred = new NetworkCredential(c.UserName, DecryptPassword(encryptedPassword));
                }
                finally
                {
                    NativeMethods.CredFree(credential);
                }
                return cred;
            }
            if (error == (int)NativeMethods.CredUIReturnCodes.ERROR_NOT_FOUND)
            {
                return null;
            }
            throw new CredentialException(error);
        }

        public static NetworkCredential RetrieveCredentialFromApplicationInstanceCache(string target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (target.Length == 0)
            {
                throw new ArgumentException("CredentialEmptyTargetError", "target");
            }

            lock (_applicationInstanceCredentialCache)
            {
                NetworkCredential cred;
                if (_applicationInstanceCredentialCache.TryGetValue(target, out cred))
                {
                    return cred;
                }
            }
            return null;
        }

        public static bool DeleteCredential(string target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (target.Length == 0)
            {
                throw new ArgumentException("CredentialEmptyTargetError", "target");
            }

            bool found;
            lock (_applicationInstanceCredentialCache)
            {
                found = _applicationInstanceCredentialCache.Remove(target);
            }

            if (NativeMethods.CredDelete(target, NativeMethods.CredTypes.CRED_TYPE_GENERIC, 0))
            {
                found = true;
            }
            else
            {
                int error = Marshal.GetLastWin32Error();
                if (error != (int)NativeMethods.CredUIReturnCodes.ERROR_NOT_FOUND)
                {
                    throw new CredentialException(error);
                }
            }
            return found;
        }

        protected virtual void OnUserNameChanged(EventArgs e)
        {
            if (UserNameChanged != null)
            {
                UserNameChanged(this, e);
            }
        }

        protected virtual void OnPasswordChanged(EventArgs e)
        {
            if (PasswordChanged != null)
            {
                PasswordChanged(this, e);
            }
        }

        private bool PromptForCredentialsCredUI(IntPtr owner, bool storedCredentials)
        {
            NativeMethods.CREDUI_INFO info = CreateCredUIInfo(owner, true);
            NativeMethods.CREDUI_FLAGS flags = NativeMethods.CREDUI_FLAGS.GENERIC_CREDENTIALS | NativeMethods.CREDUI_FLAGS.DO_NOT_PERSIST | NativeMethods.CREDUI_FLAGS.ALWAYS_SHOW_UI;
            if (ShowSaveCheckBox)
            {
                flags |= NativeMethods.CREDUI_FLAGS.SHOW_SAVE_CHECK_BOX;
            }

            StringBuilder user = new StringBuilder(NativeMethods.CREDUI_MAX_USERNAME_LENGTH);
            user.Append(UserName);
            StringBuilder pw = new StringBuilder(NativeMethods.CREDUI_MAX_PASSWORD_LENGTH);
            pw.Append(Password);

            NativeMethods.CredUIReturnCodes result = NativeMethods.CredUIPromptForCredentials(ref info, Target, IntPtr.Zero, 0, user, NativeMethods.CREDUI_MAX_USERNAME_LENGTH, pw,
                NativeMethods.CREDUI_MAX_PASSWORD_LENGTH, ref _isSaveChecked, flags);
            switch (result)
            {
                case NativeMethods.CredUIReturnCodes.NO_ERROR:
                    UserName = user.ToString();
                    Password = pw.ToString();
                    if (ShowSaveCheckBox)
                    {
                        _confirmTarget = Target;

                        if (storedCredentials && !IsSaveChecked)
                        {
                            DeleteCredential(Target);
                        }
                    }
                    return true;
                case NativeMethods.CredUIReturnCodes.ERROR_CANCELLED:
                    return false;
                default:
                    throw new CredentialException((int)result);
            }
        }

        private bool PromptForCredentialsCredUIWin(IntPtr owner, bool storedCredentials)
        {
            NativeMethods.CREDUI_INFO info = CreateCredUIInfo(owner, false);
            NativeMethods.CredUIWinFlags flags = NativeMethods.CredUIWinFlags.Generic;
            if (ShowSaveCheckBox)
            {
                flags |= NativeMethods.CredUIWinFlags.Checkbox;
            }

            IntPtr inBuffer = IntPtr.Zero;
            IntPtr outBuffer = IntPtr.Zero;
            try
            {
                uint inBufferSize = 0;
                if (UserName.Length > 0)
                {
                    NativeMethods.CredPackAuthenticationBuffer(0, UserName, Password, IntPtr.Zero, ref inBufferSize);
                    if (inBufferSize > 0)
                    {
                        inBuffer = Marshal.AllocCoTaskMem((int)inBufferSize);
                        if (!NativeMethods.CredPackAuthenticationBuffer(0, UserName, Password, inBuffer, ref inBufferSize))
                        {
                            throw new CredentialException(Marshal.GetLastWin32Error());
                        }
                    }
                }

                uint outBufferSize;
                uint package = 0;
                NativeMethods.CredUIReturnCodes result = NativeMethods.CredUIPromptForWindowsCredentials(ref info, 0, ref package, inBuffer, inBufferSize, out outBuffer, out outBufferSize,
                    ref _isSaveChecked, flags);
                switch (result)
                {
                    case NativeMethods.CredUIReturnCodes.NO_ERROR:
                        StringBuilder userName = new StringBuilder(NativeMethods.CREDUI_MAX_USERNAME_LENGTH);
                        StringBuilder password = new StringBuilder(NativeMethods.CREDUI_MAX_PASSWORD_LENGTH);
                        uint userNameSize = (uint)userName.Capacity;
                        uint passwordSize = (uint)password.Capacity;
                        uint domainSize = 0;
                        if (!NativeMethods.CredUnPackAuthenticationBuffer(0, outBuffer, outBufferSize, userName, ref userNameSize, null, ref domainSize, password, ref passwordSize))
                        {
                            throw new CredentialException(Marshal.GetLastWin32Error());
                        }
                        UserName = userName.ToString();
                        Password = password.ToString();
                        if (ShowSaveCheckBox)
                        {
                            _confirmTarget = Target;

                            if (storedCredentials && !IsSaveChecked)
                            {
                                DeleteCredential(Target);
                            }
                        }
                        return true;
                    case NativeMethods.CredUIReturnCodes.ERROR_CANCELLED:
                        return false;
                    default:
                        throw new CredentialException((int)result);
                }
            }
            finally
            {
                if (inBuffer != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(inBuffer);
                }
                if (outBuffer != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(outBuffer);
                }
            }
        }

        private NativeMethods.CREDUI_INFO CreateCredUIInfo(IntPtr owner, bool downlevelText)
        {
            NativeMethods.CREDUI_INFO info = new NativeMethods.CREDUI_INFO();
            info.cbSize = Marshal.SizeOf(info);
            info.hwndParent = owner;
            if (downlevelText)
            {
                info.pszCaptionText = WindowTitle;
                switch (DownlevelTextMode)
                {
                    case DownlevelTextMode.MainInstructionAndContent:
                        if (MainInstruction.Length == 0)
                        {
                            info.pszMessageText = Content;
                        }
                        else if (Content.Length == 0)
                        {
                            info.pszMessageText = MainInstruction;
                        }
                        else
                        {
                            info.pszMessageText = MainInstruction + Environment.NewLine + Environment.NewLine + Content;
                        }
                        break;
                    case DownlevelTextMode.MainInstructionOnly:
                        info.pszMessageText = MainInstruction;
                        break;
                    case DownlevelTextMode.ContentOnly:
                        info.pszMessageText = Content;
                        break;
                }
            }
            else
            {
                info.pszMessageText = Content;
                info.pszCaptionText = MainInstruction;
            }
            return info;
        }

        private bool RetrieveCredentials()
        {
            NetworkCredential credential = RetrieveCredential(Target);
            if (credential != null)
            {
                UserName = credential.UserName;
                Password = credential.Password;
                return true;
            }
            return false;
        }

        private static byte[] EncryptPassword(string password)
        {
            byte[] protectedData = System.Security.Cryptography.ProtectedData.Protect(Encoding.UTF8.GetBytes(password), null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return protectedData;
        }

        private static string DecryptPassword(byte[] encrypted)
        {
            try
            {
                return Encoding.UTF8.GetString(System.Security.Cryptography.ProtectedData.Unprotect(encrypted, null, System.Security.Cryptography.DataProtectionScope.CurrentUser));
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                return string.Empty;
            }
        }

        private bool RetrieveCredentialsFromApplicationInstanceCache()
        {
            if (UseApplicationInstanceCredentialCache)
            {
                NetworkCredential credential = RetrieveCredentialFromApplicationInstanceCache(Target);
                if (credential != null)
                {
                    UserName = credential.UserName;
                    Password = credential.Password;
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}