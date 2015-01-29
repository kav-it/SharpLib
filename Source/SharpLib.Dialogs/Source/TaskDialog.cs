using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

using SharpLib;

namespace Ookii.Dialogs.Wpf
{
    [DefaultProperty("MainInstruction"), DefaultEvent("ButtonClicked"), Description("Displays a task dialog."), Designer(typeof(TaskDialogDesigner))]
    public partial class TaskDialog : Component, IWin32Window
    {
        #region Поля

        private TaskDialogItemCollection<TaskDialogButton> _buttons;

        private Dictionary<int, TaskDialogButton> _buttonsById;

        private NativeMethods.TASKDIALOGCONFIG _config;

        private System.Drawing.Icon _customFooterIcon;

        private System.Drawing.Icon _customMainIcon;

        private TaskDialogIcon _footerIcon;

        private IntPtr _handle;

        private int _inEventHandler;

        private TaskDialogIcon _mainIcon;

        private int _progressBarMarqueeAnimationSpeed = 100;

        private int _progressBarMaximum = 100;

        private int _progressBarMinimimum;

        private ProgressBarState _progressBarState = ProgressBarState.Normal;

        private int _progressBarValue;

        private TaskDialogItemCollection<TaskDialogRadioButton> _radioButtons;

        private Dictionary<int, TaskDialogRadioButton> _radioButtonsById;

        private bool _updatePending;

        private System.Drawing.Icon _windowIcon;

        #endregion

        #region Свойства

        public static bool OSSupportsTaskDialogs
        {
            get { return Env.IsWindowsVistaOrLater; }
        }

        [Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance"), Description("A list of the buttons on the Task Dialog.")]
        public TaskDialogItemCollection<TaskDialogButton> Buttons
        {
            get { return _buttons ?? (_buttons = new TaskDialogItemCollection<TaskDialogButton>(this)); }
        }

        [Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance"), Description("A list of the radio buttons on the Task Dialog.")]
        public TaskDialogItemCollection<TaskDialogRadioButton> RadioButtons
        {
            get { return _radioButtons ?? (_radioButtons = new TaskDialogItemCollection<TaskDialogRadioButton>(this)); }
        }

        [Localizable(true), Category("Appearance"), Description("The window title of the task dialog."), DefaultValue("")]
        public string WindowTitle
        {
            get { return _config.pszWindowTitle ?? string.Empty; }
            set
            {
                _config.pszWindowTitle = string.IsNullOrEmpty(value) ? null : value;
                UpdateDialog();
            }
        }

        [Localizable(true), Category("Appearance"), Description("The dialog's main instruction."), DefaultValue(""),
         Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public string MainInstruction
        {
            get { return _config.pszMainInstruction ?? string.Empty; }
            set
            {
                _config.pszMainInstruction = string.IsNullOrEmpty(value) ? null : value;
                SetElementText(NativeMethods.TaskDialogElements.MainInstruction, MainInstruction);
            }
        }

        [Localizable(true), Category("Appearance"), Description("The dialog's primary content."), DefaultValue(""),
         Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public string Content
        {
            get { return _config.pszContent ?? string.Empty; }
            set
            {
                _config.pszContent = string.IsNullOrEmpty(value) ? null : value;
                SetElementText(NativeMethods.TaskDialogElements.Content, Content);
            }
        }

        [Localizable(true), Category("Appearance"), Description("The icon to be used in the title bar of the dialog. Used only when the dialog is shown as a modeless dialog."), DefaultValue(null)]
        public System.Drawing.Icon WindowIcon
        {
            get
            {
                if (IsDialogRunning)
                {
                    IntPtr icon = NativeMethods.SendMessage(Handle, NativeMethods.WM_GETICON, new IntPtr(NativeMethods.ICON_SMALL), IntPtr.Zero);
                    return System.Drawing.Icon.FromHandle(icon);
                }
                return _windowIcon;
            }
            set { _windowIcon = value; }
        }

        [Localizable(true), Category("Appearance"), Description("The icon to display in the task dialog."), DefaultValue(TaskDialogIcon.Custom)]
        public TaskDialogIcon MainIcon
        {
            get { return _mainIcon; }
            set
            {
                if (_mainIcon != value)
                {
                    _mainIcon = value;
                    UpdateDialog();
                }
            }
        }

        [Localizable(true), Category("Appearance"), Description("A custom icon to display in the dialog."), DefaultValue(null)]
        public System.Drawing.Icon CustomMainIcon
        {
            get { return _customMainIcon; }
            set
            {
                if (_customMainIcon != value)
                {
                    _customMainIcon = value;
                    UpdateDialog();
                }
            }
        }

        [Localizable(true), Category("Appearance"), Description("The icon to display in the footer area of the task dialog."), DefaultValue(TaskDialogIcon.Custom)]
        public TaskDialogIcon FooterIcon
        {
            get { return _footerIcon; }
            set
            {
                if (_footerIcon != value)
                {
                    _footerIcon = value;
                    UpdateDialog();
                }
            }
        }

        [Localizable(true), Category("Appearance"), Description("A custom icon to display in the footer area of the task dialog."), DefaultValue(null)]
        public System.Drawing.Icon CustomFooterIcon
        {
            get { return _customFooterIcon; }
            set
            {
                if (_customFooterIcon != value)
                {
                    _customFooterIcon = value;

                    UpdateDialog();
                }
            }
        }

        [Category("Behavior"), Description("Indicates whether custom buttons should be displayed as normal buttons or command links."), DefaultValue(TaskDialogButtonStyle.Standard)]
        public TaskDialogButtonStyle ButtonStyle
        {
            get
            {
                return GetFlag(NativeMethods.TaskDialogFlags.UseCommandLinksNoIcon)
                    ? TaskDialogButtonStyle.CommandLinksNoIcon
                    : GetFlag(NativeMethods.TaskDialogFlags.UseCommandLinks)
                        ? TaskDialogButtonStyle.CommandLinks
                        : TaskDialogButtonStyle.Standard;
            }
            set
            {
                SetFlag(NativeMethods.TaskDialogFlags.UseCommandLinks, value == TaskDialogButtonStyle.CommandLinks);
                SetFlag(NativeMethods.TaskDialogFlags.UseCommandLinksNoIcon, value == TaskDialogButtonStyle.CommandLinksNoIcon);
                UpdateDialog();
            }
        }

        [Localizable(true), Category("Appearance"), Description("The label for the verification checkbox."), DefaultValue("")]
        public string VerificationText
        {
            get { return _config.pszVerificationText ?? string.Empty; }
            set
            {
                string realValue = string.IsNullOrEmpty(value) ? null : value;
                if (_config.pszVerificationText != realValue)
                {
                    _config.pszVerificationText = realValue;
                    UpdateDialog();
                }
            }
        }

        [Category("Behavior"), Description("Indicates whether the verification checkbox is checked ot not."), DefaultValue(false)]
        public bool IsVerificationChecked
        {
            get { return GetFlag(NativeMethods.TaskDialogFlags.VerificationFlagChecked); }
            set
            {
                if (value != IsVerificationChecked)
                {
                    SetFlag(NativeMethods.TaskDialogFlags.VerificationFlagChecked, value);
                    if (IsDialogRunning)
                    {
                        ClickVerification(value, false);
                    }
                }
            }
        }

        [Localizable(true), Category("Appearance"), Description("Additional information to be displayed on the dialog."), DefaultValue(""),
         Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public string ExpandedInformation
        {
            get { return _config.pszExpandedInformation ?? string.Empty; }
            set
            {
                _config.pszExpandedInformation = string.IsNullOrEmpty(value) ? null : value;
                SetElementText(NativeMethods.TaskDialogElements.ExpandedInformation, ExpandedInformation);
            }
        }

        [Localizable(true), Category("Appearance"), Description("The text to use for the control for collapsing the expandable information."), DefaultValue("")]
        public string ExpandedControlText
        {
            get { return _config.pszExpandedControlText ?? string.Empty; }
            set
            {
                string realValue = string.IsNullOrEmpty(value) ? null : value;
                if (_config.pszExpandedControlText != realValue)
                {
                    _config.pszExpandedControlText = realValue;
                    UpdateDialog();
                }
            }
        }

        [Localizable(true), Category("Appearance"), Description("The text to use for the control for expanding the expandable information."), DefaultValue("")]
        public string CollapsedControlText
        {
            get { return _config.pszCollapsedControlText ?? string.Empty; }
            set
            {
                string realValue = string.IsNullOrEmpty(value) ? null : value;
                if (_config.pszCollapsedControlText != realValue)
                {
                    _config.pszCollapsedControlText = string.IsNullOrEmpty(value) ? null : value;
                    UpdateDialog();
                }
            }
        }

        [Localizable(true), Category("Appearance"), Description("The text to be used in the footer area of the task dialog."), DefaultValue(""),
         Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public string Footer
        {
            get { return _config.pszFooterText ?? string.Empty; }
            set
            {
                _config.pszFooterText = string.IsNullOrEmpty(value) ? null : value;
                SetElementText(NativeMethods.TaskDialogElements.Footer, Footer);
            }
        }

        [Localizable(true), Category("Appearance"), Description("the width of the task dialog's client area in DLU's. If 0, task dialog will calculate the ideal width."), DefaultValue(0)]
        public int Width
        {
            get { return (int)_config.cxWidth; }
            set
            {
                if (_config.cxWidth != (uint)value)
                {
                    _config.cxWidth = (uint)value;
                    UpdateDialog();
                }
            }
        }

        [Category("Behavior"), Description("Indicates whether hyperlinks are allowed for the Content, ExpandedInformation and Footer properties."), DefaultValue(false)]
        public bool EnableHyperlinks
        {
            get { return GetFlag(NativeMethods.TaskDialogFlags.EnableHyperLinks); }
            set
            {
                if (EnableHyperlinks != value)
                {
                    SetFlag(NativeMethods.TaskDialogFlags.EnableHyperLinks, value);
                    UpdateDialog();
                }
            }
        }

        [Category("Behavior"), Description("Indicates that the dialog should be able to be closed using Alt-F4, Escape and the title bar's close button even if no cancel button is specified."),
         DefaultValue(false)]
        public bool AllowDialogCancellation
        {
            get { return GetFlag(NativeMethods.TaskDialogFlags.AllowDialogCancellation); }
            set
            {
                if (AllowDialogCancellation != value)
                {
                    SetFlag(NativeMethods.TaskDialogFlags.AllowDialogCancellation, value);
                    UpdateDialog();
                }
            }
        }

        [Category("Behavior"),
         Description(
             "Indicates that the string specified by the ExpandedInformation property should be displayed at the bottom of the dialog's footer area instead of immediately after the dialog's content."),
         DefaultValue(false)]
        public bool ExpandFooterArea
        {
            get { return GetFlag(NativeMethods.TaskDialogFlags.ExpandFooterArea); }
            set
            {
                if (ExpandFooterArea != value)
                {
                    SetFlag(NativeMethods.TaskDialogFlags.ExpandFooterArea, value);
                    UpdateDialog();
                }
            }
        }

        [Category("Behavior"), Description("Indicates that the string specified by the ExpandedInformation property should be displayed by default."), DefaultValue(false)]
        public bool ExpandedByDefault
        {
            get { return GetFlag(NativeMethods.TaskDialogFlags.ExpandedByDefault); }
            set
            {
                if (ExpandedByDefault != value)
                {
                    SetFlag(NativeMethods.TaskDialogFlags.ExpandedByDefault, value);
                    UpdateDialog();
                }
            }
        }

        [Category("Behavior"), Description("Indicates whether the Timer event is raised periodically while the dialog is visible."), DefaultValue(false)]
        public bool RaiseTimerEvent
        {
            get { return GetFlag(NativeMethods.TaskDialogFlags.CallbackTimer); }
            set
            {
                if (RaiseTimerEvent != value)
                {
                    SetFlag(NativeMethods.TaskDialogFlags.CallbackTimer, value);
                    UpdateDialog();
                }
            }
        }

        [Category("Layout"), Description("Indicates whether the dialog is centered in the parent window instead of the screen."), DefaultValue(false)]
        public bool CenterParent
        {
            get { return GetFlag(NativeMethods.TaskDialogFlags.PositionRelativeToWindow); }
            set
            {
                if (CenterParent != value)
                {
                    SetFlag(NativeMethods.TaskDialogFlags.PositionRelativeToWindow, value);
                    UpdateDialog();
                }
            }
        }

        [Localizable(true), Category("Appearance"), Description("Indicates whether text is displayed right to left."), DefaultValue(false)]
        public bool RightToLeft
        {
            get { return GetFlag(NativeMethods.TaskDialogFlags.RtlLayout); }
            set
            {
                if (RightToLeft != value)
                {
                    SetFlag(NativeMethods.TaskDialogFlags.RtlLayout, value);
                    UpdateDialog();
                }
            }
        }

        [Category("Window Style"), Description("Indicates whether the dialog has a minimize box on its caption bar."), DefaultValue(false)]
        public bool MinimizeBox
        {
            get { return GetFlag(NativeMethods.TaskDialogFlags.CanBeMinimized); }
            set
            {
                if (MinimizeBox != value)
                {
                    SetFlag(NativeMethods.TaskDialogFlags.CanBeMinimized, value);
                    UpdateDialog();
                }
            }
        }

        [Category("Behavior"), Description("The type of progress bar displayed on the dialog."), DefaultValue(ProgressBarStyle.None)]
        public ProgressBarStyle ProgressBarStyle
        {
            get
            {
                if (GetFlag(NativeMethods.TaskDialogFlags.ShowMarqueeProgressBar))
                {
                    return ProgressBarStyle.MarqueeProgressBar;
                }
                if (GetFlag(NativeMethods.TaskDialogFlags.ShowProgressBar))
                {
                    return ProgressBarStyle.ProgressBar;
                }
                return ProgressBarStyle.None;
            }
            set
            {
                SetFlag(NativeMethods.TaskDialogFlags.ShowMarqueeProgressBar, value == ProgressBarStyle.MarqueeProgressBar);
                SetFlag(NativeMethods.TaskDialogFlags.ShowProgressBar, value == ProgressBarStyle.ProgressBar);
                UpdateProgressBarStyle();
            }
        }

        [Category("Behavior"), Description("The marquee animation speed of the progress bar in milliseconds."), DefaultValue(100)]
        public int ProgressBarMarqueeAnimationSpeed
        {
            get { return _progressBarMarqueeAnimationSpeed; }
            set
            {
                _progressBarMarqueeAnimationSpeed = value;
                UpdateProgressBarMarqueeSpeed();
            }
        }

        [Category("Behavior"), Description("The lower bound of the range of the task dialog's progress bar."), DefaultValue(0)]
        public int ProgressBarMinimum
        {
            get { return _progressBarMinimimum; }
            set
            {
                if (_progressBarMaximum <= value)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _progressBarMinimimum = value;
                UpdateProgressBarRange();
            }
        }

        [Category("Behavior"), Description("The upper bound of the range of the task dialog's progress bar."), DefaultValue(100)]
        public int ProgressBarMaximum
        {
            get { return _progressBarMaximum; }
            set
            {
                if (value <= _progressBarMinimimum)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _progressBarMaximum = value;
                UpdateProgressBarRange();
            }
        }

        [Category("Behavior"), Description("The current value of the task dialog's progress bar."), DefaultValue(0)]
        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                if (value < ProgressBarMinimum || value > ProgressBarMaximum)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _progressBarValue = value;
                UpdateProgressBarValue();
            }
        }

        [Category("Behavior"), Description("The state of the task dialog's progress bar."), DefaultValue(ProgressBarState.Normal)]
        public ProgressBarState ProgressBarState
        {
            get { return _progressBarState; }
            set
            {
                _progressBarState = value;
                UpdateProgressBarState();
            }
        }

        [Category("Data"), Description("User-defined data about the component."), DefaultValue(null)]
        public object Tag { get; set; }

        private bool IsDialogRunning
        {
            get { return _handle != IntPtr.Zero; }
        }

        [Browsable(false)]
        public IntPtr Handle
        {
            get
            {
                CheckCrossThreadCall();
                return _handle;
            }
        }

        #endregion

        #region События

        [Category("Action"), Description("Event raised when the user clicks a button.")]
        public event EventHandler<TaskDialogItemClickedEventArgs> ButtonClicked;

        [Category("Behavior"), Description("Event raised when the task dialog has been created.")]
        public event EventHandler Created;

        [Category("Behavior"), Description("Event raised when the task dialog has been destroyed.")]
        public event EventHandler Destroyed;

        [Category("Action"), Description("Event raised when the user clicks the expand button on the task dialog.")]
        public event EventHandler<ExpandButtonClickedEventArgs> ExpandButtonClicked;

        [Category("Action"), Description("Event raised when the user presses F1 while the dialog has focus.")]
        public event EventHandler HelpRequested;

        [Category("Action"), Description("Event raised when the user clicks a hyperlink.")]
        public event EventHandler<HyperlinkClickedEventArgs> HyperlinkClicked;

        [Category("Action"), Description("Event raised when the user clicks a button.")]
        public event EventHandler<TaskDialogItemClickedEventArgs> RadioButtonClicked;

        [Category("Behavior"), Description("Event raised periodically while the dialog is displayed.")]
        public event EventHandler<TimerEventArgs> Timer;

        [Category("Action"), Description("Event raised when the user clicks the verification check box.")]
        public event EventHandler VerificationClicked;

        #endregion

        #region Конструктор

        public TaskDialog()
        {
            InitializeComponent();

            _config.cbSize = (uint)Marshal.SizeOf(_config);
            _config.pfCallback = TaskDialogCallback;
        }

        public TaskDialog(IContainer container)
        {
            if (container != null)
            {
                container.Add(this);
            }

            InitializeComponent();

            _config.cbSize = (uint)Marshal.SizeOf(_config);
            _config.pfCallback = TaskDialogCallback;
        }

        #endregion

        #region Методы

        public TaskDialogButton Show()
        {
            return ShowDialog(IntPtr.Zero);
        }

        public TaskDialogButton ShowDialog()
        {
            return ShowDialog(null);
        }

        public TaskDialogButton ShowDialog(Window owner)
        {
            IntPtr ownerHandle;
            if (owner == null)
            {
                ownerHandle = NativeMethods.GetActiveWindow();
            }
            else
            {
                ownerHandle = new WindowInteropHelper(owner).Handle;
            }
            return ShowDialog(ownerHandle);
        }

        public void ClickVerification(bool checkState, bool setFocus)
        {
            if (!IsDialogRunning)
            {
                throw new InvalidOperationException("TaskDialogNotRunningError");
            }

            NativeMethods.SendMessage(Handle, (int)NativeMethods.TaskDialogMessages.ClickVerification, new IntPtr(checkState ? 1 : 0), new IntPtr(setFocus ? 1 : 0));
        }

        protected virtual void OnHyperlinkClicked(HyperlinkClickedEventArgs e)
        {
            if (HyperlinkClicked != null)
            {
                HyperlinkClicked(this, e);
            }
        }

        protected virtual void OnButtonClicked(TaskDialogItemClickedEventArgs e)
        {
            if (ButtonClicked != null)
            {
                ButtonClicked(this, e);
            }
        }

        protected virtual void OnRadioButtonClicked(TaskDialogItemClickedEventArgs e)
        {
            if (RadioButtonClicked != null)
            {
                RadioButtonClicked(this, e);
            }
        }

        protected virtual void OnVerificationClicked(EventArgs e)
        {
            if (VerificationClicked != null)
            {
                VerificationClicked(this, e);
            }
        }

        protected virtual void OnCreated(EventArgs e)
        {
            if (Created != null)
            {
                Created(this, e);
            }
        }

        protected virtual void OnTimer(TimerEventArgs e)
        {
            if (Timer != null)
            {
                Timer(this, e);
            }
        }

        protected virtual void OnDestroyed(EventArgs e)
        {
            if (Destroyed != null)
            {
                Destroyed(this, e);
            }
        }

        protected virtual void OnExpandButtonClicked(ExpandButtonClickedEventArgs e)
        {
            if (ExpandButtonClicked != null)
            {
                ExpandButtonClicked(this, e);
            }
        }

        protected virtual void OnHelpRequested(EventArgs e)
        {
            if (HelpRequested != null)
            {
                HelpRequested(this, e);
            }
        }

        internal void SetItemEnabled(TaskDialogItem item)
        {
            if (IsDialogRunning)
            {
                NativeMethods.SendMessage(Handle, (int)(item is TaskDialogButton ? NativeMethods.TaskDialogMessages.EnableButton : NativeMethods.TaskDialogMessages.EnableRadioButton),
                    new IntPtr(item.Id), new IntPtr(item.Enabled ? 1 : 0));
            }
        }

        internal void SetButtonElevationRequired(TaskDialogButton button)
        {
            if (IsDialogRunning)
            {
                NativeMethods.SendMessage(Handle, (int)NativeMethods.TaskDialogMessages.SetButtonElevationRequiredState, new IntPtr(button.Id), new IntPtr(button.ElevationRequired ? 1 : 0));
            }
        }

        internal void ClickItem(TaskDialogItem item)
        {
            if (!IsDialogRunning)
            {
                throw new InvalidOperationException("TaskDialogNotRunningError");
            }

            NativeMethods.SendMessage(Handle, (int)(item is TaskDialogButton ? NativeMethods.TaskDialogMessages.ClickButton : NativeMethods.TaskDialogMessages.ClickRadioButton), new IntPtr(item.Id),
                IntPtr.Zero);
        }

        private TaskDialogButton ShowDialog(IntPtr owner)
        {
            if (!OSSupportsTaskDialogs)
            {
                throw new NotSupportedException("TaskDialogsNotSupportedError");
            }

            if (IsDialogRunning)
            {
                throw new InvalidOperationException("TaskDialogRunningError");
            }

            if (_buttons.Count == 0)
            {
                throw new InvalidOperationException("TaskDialogNoButtonsError");
            }

            _config.hwndParent = owner;
            _config.dwCommonButtons = 0;
            _config.pButtons = IntPtr.Zero;
            _config.cButtons = 0;
            List<NativeMethods.TASKDIALOG_BUTTON> buttons = SetupButtons();
            List<NativeMethods.TASKDIALOG_BUTTON> radioButtons = SetupRadioButtons();

            SetupIcon();

            try
            {
                MarshalButtons(buttons, out _config.pButtons, out _config.cButtons);
                MarshalButtons(radioButtons, out _config.pRadioButtons, out _config.cRadioButtons);
                int buttonId;
                int radioButton;
                bool verificationFlagChecked;
                using (new ComCtlv6ActivationContext(true))
                {
                    NativeMethods.TaskDialogIndirect(ref _config, out buttonId, out radioButton, out verificationFlagChecked);
                }
                IsVerificationChecked = verificationFlagChecked;

                TaskDialogRadioButton selectedRadioButton;
                if (_radioButtonsById.TryGetValue(radioButton, out selectedRadioButton))
                {
                    selectedRadioButton.Checked = true;
                }

                TaskDialogButton selectedButton;
                if (_buttonsById.TryGetValue(buttonId, out selectedButton))
                {
                    return selectedButton;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                CleanUpButtons(ref _config.pButtons, ref _config.cButtons);
                CleanUpButtons(ref _config.pRadioButtons, ref _config.cRadioButtons);
            }
        }

        internal void UpdateDialog()
        {
            if (IsDialogRunning)
            {
                if (_inEventHandler > 0)
                {
                    _updatePending = true;
                }
                else
                {
                    _updatePending = false;
                    CleanUpButtons(ref _config.pButtons, ref _config.cButtons);
                    CleanUpButtons(ref _config.pRadioButtons, ref _config.cRadioButtons);
                    _config.dwCommonButtons = 0;

                    List<NativeMethods.TASKDIALOG_BUTTON> buttons = SetupButtons();
                    List<NativeMethods.TASKDIALOG_BUTTON> radioButtons = SetupRadioButtons();

                    SetupIcon();

                    MarshalButtons(buttons, out _config.pButtons, out _config.cButtons);
                    MarshalButtons(radioButtons, out _config.pRadioButtons, out _config.cRadioButtons);

                    int size = Marshal.SizeOf(_config);
                    IntPtr memory = Marshal.AllocHGlobal(size);
                    try
                    {
                        Marshal.StructureToPtr(_config, memory, false);
                        NativeMethods.SendMessage(Handle, (int)NativeMethods.TaskDialogMessages.NavigatePage, IntPtr.Zero, memory);
                    }
                    finally
                    {
                        Marshal.DestroyStructure(memory, typeof(NativeMethods.TASKDIALOGCONFIG));
                        Marshal.FreeHGlobal(memory);
                    }
                }
            }
        }

        private void SetElementText(NativeMethods.TaskDialogElements element, string text)
        {
            if (IsDialogRunning)
            {
                IntPtr newTextPtr = Marshal.StringToHGlobalUni(text);
                try
                {
                    IntPtr result = NativeMethods.SendMessage(Handle, (int)NativeMethods.TaskDialogMessages.SetElementText, new IntPtr((int)element), newTextPtr);
                }
                finally
                {
                    if (newTextPtr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(newTextPtr);
                    }
                }
            }
        }

        private void SetupIcon()
        {
            SetupIcon(MainIcon, CustomMainIcon, NativeMethods.TaskDialogFlags.UseHIconMain);
            SetupIcon(FooterIcon, CustomFooterIcon, NativeMethods.TaskDialogFlags.UseHIconFooter);
        }

        private void SetupIcon(TaskDialogIcon icon, System.Drawing.Icon customIcon, NativeMethods.TaskDialogFlags flag)
        {
            SetFlag(flag, false);
            if (icon == TaskDialogIcon.Custom)
            {
                if (customIcon != null)
                {
                    SetFlag(flag, true);
                    if (flag == NativeMethods.TaskDialogFlags.UseHIconMain)
                    {
                        _config.hMainIcon = customIcon.Handle;
                    }
                    else
                    {
                        _config.hFooterIcon = customIcon.Handle;
                    }
                }
            }
            else
            {
                if (flag == NativeMethods.TaskDialogFlags.UseHIconMain)
                {
                    _config.hMainIcon = new IntPtr((int)icon);
                }
                else
                {
                    _config.hFooterIcon = new IntPtr((int)icon);
                }
            }
        }

        private static void CleanUpButtons(ref IntPtr buttons, ref uint count)
        {
            if (buttons != IntPtr.Zero)
            {
                int elementSize = Marshal.SizeOf(typeof(NativeMethods.TASKDIALOG_BUTTON));
                for (int x = 0; x < count; ++x)
                {
                    IntPtr offset = new IntPtr(buttons.ToInt64() + x * elementSize);
                    Marshal.DestroyStructure(offset, typeof(NativeMethods.TASKDIALOG_BUTTON));
                }
                Marshal.FreeHGlobal(buttons);
                buttons = IntPtr.Zero;
                count = 0;
            }
        }

        private static void MarshalButtons(List<NativeMethods.TASKDIALOG_BUTTON> buttons, out IntPtr buttonsPtr, out uint count)
        {
            buttonsPtr = IntPtr.Zero;
            count = 0;
            if (buttons.Count > 0)
            {
                int elementSize = Marshal.SizeOf(typeof(NativeMethods.TASKDIALOG_BUTTON));
                buttonsPtr = Marshal.AllocHGlobal(elementSize * buttons.Count);
                for (int x = 0; x < buttons.Count; ++x)
                {
                    IntPtr offset = new IntPtr(buttonsPtr.ToInt64() + x * elementSize);
                    Marshal.StructureToPtr(buttons[x], offset, false);
                }
                count = (uint)buttons.Count;
            }
        }

        private List<NativeMethods.TASKDIALOG_BUTTON> SetupButtons()
        {
            _buttonsById = new Dictionary<int, TaskDialogButton>();
            List<NativeMethods.TASKDIALOG_BUTTON> buttons = new List<NativeMethods.TASKDIALOG_BUTTON>();
            _config.nDefaultButton = 0;
            foreach (TaskDialogButton button in Buttons)
            {
                if (button.Id < 1)
                {
                    throw new InvalidOperationException("InvalidTaskDialogItemIdError");
                }
                _buttonsById.Add(button.Id, button);
                if (button.Default)
                {
                    _config.nDefaultButton = button.Id;
                }
                if (button.ButtonType == ButtonType.Custom)
                {
                    if (string.IsNullOrEmpty(button.Text))
                    {
                        throw new InvalidOperationException("TaskDialogEmptyButtonLabelError");
                    }
                    NativeMethods.TASKDIALOG_BUTTON taskDialogButton = new NativeMethods.TASKDIALOG_BUTTON();
                    taskDialogButton.nButtonID = button.Id;
                    taskDialogButton.pszButtonText = button.Text;
                    if (ButtonStyle == TaskDialogButtonStyle.CommandLinks || ButtonStyle == TaskDialogButtonStyle.CommandLinksNoIcon && !string.IsNullOrEmpty(button.CommandLinkNote))
                    {
                        taskDialogButton.pszButtonText += "\n" + button.CommandLinkNote;
                    }
                    buttons.Add(taskDialogButton);
                }
                else
                {
                    _config.dwCommonButtons |= button.ButtonFlag;
                }
            }
            return buttons;
        }

        private List<NativeMethods.TASKDIALOG_BUTTON> SetupRadioButtons()
        {
            _radioButtonsById = new Dictionary<int, TaskDialogRadioButton>();
            List<NativeMethods.TASKDIALOG_BUTTON> radioButtons = new List<NativeMethods.TASKDIALOG_BUTTON>();
            _config.nDefaultRadioButton = 0;
            foreach (TaskDialogRadioButton radioButton in RadioButtons)
            {
                if (string.IsNullOrEmpty(radioButton.Text))
                {
                    throw new InvalidOperationException("TaskDialogEmptyButtonLabelError");
                }
                if (radioButton.Id < 1)
                {
                    throw new InvalidOperationException("InvalidTaskDialogItemIdError");
                }
                _radioButtonsById.Add(radioButton.Id, radioButton);
                if (radioButton.Checked)
                {
                    _config.nDefaultRadioButton = radioButton.Id;
                }
                NativeMethods.TASKDIALOG_BUTTON taskDialogButton = new NativeMethods.TASKDIALOG_BUTTON();
                taskDialogButton.nButtonID = radioButton.Id;
                taskDialogButton.pszButtonText = radioButton.Text;
                radioButtons.Add(taskDialogButton);
            }
            SetFlag(NativeMethods.TaskDialogFlags.NoDefaultRadioButton, _config.nDefaultRadioButton == 0);
            return radioButtons;
        }

        private void SetFlag(NativeMethods.TaskDialogFlags flag, bool value)
        {
            if (value)
            {
                _config.dwFlags |= flag;
            }
            else
            {
                _config.dwFlags &= ~flag;
            }
        }

        private bool GetFlag(NativeMethods.TaskDialogFlags flag)
        {
            return (_config.dwFlags & flag) != 0;
        }

        private uint TaskDialogCallback(IntPtr hwnd, uint uNotification, IntPtr wParam, IntPtr lParam, IntPtr dwRefData)
        {
            Interlocked.Increment(ref _inEventHandler);
            try
            {
                switch ((NativeMethods.TaskDialogNotifications)uNotification)
                {
                    case NativeMethods.TaskDialogNotifications.Created:
                        _handle = hwnd;
                        DialogCreated();
                        OnCreated(EventArgs.Empty);
                        break;
                    case NativeMethods.TaskDialogNotifications.Destroyed:
                        _handle = IntPtr.Zero;
                        OnDestroyed(EventArgs.Empty);
                        break;
                    case NativeMethods.TaskDialogNotifications.Navigated:
                        DialogCreated();
                        break;
                    case NativeMethods.TaskDialogNotifications.HyperlinkClicked:
                        string url = Marshal.PtrToStringUni(lParam);
                        OnHyperlinkClicked(new HyperlinkClickedEventArgs(url));
                        break;
                    case NativeMethods.TaskDialogNotifications.ButtonClicked:
                        TaskDialogButton button;
                        if (_buttonsById.TryGetValue((int)wParam, out button))
                        {
                            TaskDialogItemClickedEventArgs e = new TaskDialogItemClickedEventArgs(button);
                            OnButtonClicked(e);
                            if (e.Cancel)
                            {
                                return 1;
                            }
                        }
                        break;
                    case NativeMethods.TaskDialogNotifications.VerificationClicked:
                        IsVerificationChecked = ((int)wParam) == 1;
                        OnVerificationClicked(EventArgs.Empty);
                        break;
                    case NativeMethods.TaskDialogNotifications.RadioButtonClicked:
                        TaskDialogRadioButton radioButton;
                        if (_radioButtonsById.TryGetValue((int)wParam, out radioButton))
                        {
                            radioButton.Checked = true;
                            TaskDialogItemClickedEventArgs e = new TaskDialogItemClickedEventArgs(radioButton);
                            OnRadioButtonClicked(e);
                        }
                        break;
                    case NativeMethods.TaskDialogNotifications.Timer:
                        TimerEventArgs timerEventArgs = new TimerEventArgs(wParam.ToInt32());
                        OnTimer(timerEventArgs);
                        return (uint)(timerEventArgs.ResetTickCount ? 1 : 0);
                    case NativeMethods.TaskDialogNotifications.ExpandoButtonClicked:
                        OnExpandButtonClicked(new ExpandButtonClickedEventArgs(wParam.ToInt32() != 0));
                        break;
                    case NativeMethods.TaskDialogNotifications.Help:
                        OnHelpRequested(EventArgs.Empty);
                        break;
                }
                return 0;
            }
            finally
            {
                Interlocked.Decrement(ref _inEventHandler);
                if (_updatePending)
                {
                    UpdateDialog();
                }
            }
        }

        private void DialogCreated()
        {
            if (_config.hwndParent == IntPtr.Zero && _windowIcon != null)
            {
                NativeMethods.SendMessage(Handle, NativeMethods.WM_SETICON, new IntPtr(NativeMethods.ICON_SMALL), _windowIcon.Handle);
            }

            foreach (TaskDialogButton button in Buttons)
            {
                if (!button.Enabled)
                {
                    SetItemEnabled(button);
                }
                if (button.ElevationRequired)
                {
                    SetButtonElevationRequired(button);
                }
            }
            UpdateProgressBarStyle();
            UpdateProgressBarMarqueeSpeed();
            UpdateProgressBarRange();
            UpdateProgressBarValue();
            UpdateProgressBarState();
        }

        private void UpdateProgressBarStyle()
        {
            if (IsDialogRunning)
            {
                NativeMethods.SendMessage(Handle, (int)NativeMethods.TaskDialogMessages.SetMarqueeProgressBar, new IntPtr(ProgressBarStyle == ProgressBarStyle.MarqueeProgressBar ? 1 : 0), IntPtr.Zero);
            }
        }

        private void UpdateProgressBarMarqueeSpeed()
        {
            if (IsDialogRunning)
            {
                NativeMethods.SendMessage(Handle, (int)NativeMethods.TaskDialogMessages.SetProgressBarMarquee, new IntPtr(ProgressBarMarqueeAnimationSpeed > 0 ? 1 : 0),
                    new IntPtr(ProgressBarMarqueeAnimationSpeed));
            }
        }

        private void UpdateProgressBarRange()
        {
            if (IsDialogRunning)
            {
                NativeMethods.SendMessage(Handle, (int)NativeMethods.TaskDialogMessages.SetProgressBarRange, IntPtr.Zero, new IntPtr(ProgressBarMaximum << 16 | ProgressBarMinimum));
            }
            if (ProgressBarValue < ProgressBarMinimum)
            {
                ProgressBarValue = ProgressBarMinimum;
            }
            if (ProgressBarValue > ProgressBarMaximum)
            {
                ProgressBarValue = ProgressBarMaximum;
            }
        }

        private void UpdateProgressBarValue()
        {
            if (IsDialogRunning)
            {
                NativeMethods.SendMessage(Handle, (int)NativeMethods.TaskDialogMessages.SetProgressBarPos, new IntPtr(ProgressBarValue), IntPtr.Zero);
            }
        }

        private void UpdateProgressBarState()
        {
            if (IsDialogRunning)
            {
                NativeMethods.SendMessage(Handle, (int)NativeMethods.TaskDialogMessages.SetProgressBarState, new IntPtr((int)ProgressBarState + 1), IntPtr.Zero);
            }
        }

        private void CheckCrossThreadCall()
        {
            IntPtr handle = _handle;
            if (handle != IntPtr.Zero)
            {
                int processId;
                int windowThreadId = NativeMethods.GetWindowThreadProcessId(handle, out processId);
                int threadId = NativeMethods.GetCurrentThreadId();
                if (windowThreadId != threadId)
                {
                    throw new InvalidOperationException("TaskDialogIllegalCrossThreadCallError");
                }
            }
        }

        #endregion
    }
}