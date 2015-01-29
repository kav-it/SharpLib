using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using SharpLib;

namespace Ookii.Dialogs.Wpf
{
    [DefaultEvent("DoWork"), DefaultProperty("Text"), Description("Represents a dialog that can be used to report progress to the user.")]
    public partial class ProgressDialog : Component
    {
        #region Поля

        private bool _cancellationPending;

        private string _cancellationText;

        private SafeModuleHandle _currentAnimationModuleHandle;

        private string _description;

        private Interop.IProgressDialog _dialog;

        private string _text;

        private bool _useCompactPathsForDescription;

        private bool _useCompactPathsForText;

        private string _windowTitle;

        #endregion

        #region Свойства

        [Localizable(true), Category("Appearance"), Description("The text in the progress dialog's title bar."), DefaultValue("")]
        public string WindowTitle
        {
            get { return _windowTitle ?? string.Empty; }
            set { _windowTitle = value; }
        }

        [Localizable(true), Category("Appearance"), Description("A short description of the operation being carried out.")]
        public string Text
        {
            get { return _text ?? string.Empty; }
            set
            {
                _text = value;
                if (_dialog != null)
                {
                    _dialog.SetLine(1, Text, UseCompactPathsForText, IntPtr.Zero);
                }
            }
        }

        [Category("Behavior"), Description("Indicates whether path strings in the Text property should be compacted if they are too large to fit on one line."), DefaultValue(false)]
        public bool UseCompactPathsForText
        {
            get { return _useCompactPathsForText; }
            set
            {
                _useCompactPathsForText = value;
                if (_dialog != null)
                {
                    _dialog.SetLine(1, Text, UseCompactPathsForText, IntPtr.Zero);
                }
            }
        }

        [Localizable(true), Category("Appearance"), Description("Additional details about the operation being carried out."), DefaultValue("")]
        public string Description
        {
            get { return _description ?? string.Empty; }
            set
            {
                _description = value;
                if (_dialog != null)
                {
                    _dialog.SetLine(2, Description, UseCompactPathsForDescription, IntPtr.Zero);
                }
            }
        }

        [Category("Behavior"), Description("Indicates whether path strings in the Description property should be compacted if they are too large to fit on one line."), DefaultValue(false)]
        public bool UseCompactPathsForDescription
        {
            get { return _useCompactPathsForDescription; }
            set
            {
                _useCompactPathsForDescription = value;
                if (_dialog != null)
                {
                    _dialog.SetLine(2, Description, UseCompactPathsForDescription, IntPtr.Zero);
                }
            }
        }

        [Localizable(true), Category("Appearance"), Description("The text that will be shown after the Cancel button is pressed."), DefaultValue("")]
        public string CancellationText
        {
            get { return _cancellationText ?? string.Empty; }
            set { _cancellationText = value; }
        }

        [Category("Appearance"), Description("Indicates whether an estimate of the remaining time will be shown."), DefaultValue(false)]
        public bool ShowTimeRemaining { get; set; }

        [Category("Appearance"), Description("Indicates whether the dialog has a cancel button. Do not set to false unless absolutely necessary."), DefaultValue(true)]
        public bool ShowCancelButton { get; set; }

        [Category("Window Style"), Description("Indicates whether the progress dialog has a minimize button."), DefaultValue(true)]
        public bool MinimizeBox { get; set; }

        [Browsable(false)]
        public bool CancellationPending
        {
            get
            {
                _backgroundWorker.ReportProgress(-1);

                return _cancellationPending;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationResource Animation { get; set; }

        [Category("Appearance"), Description("Indicates the style of the progress bar."), DefaultValue(ProgressBarStyle.ProgressBar)]
        public ProgressBarStyle ProgressBarStyle { get; set; }

        [Browsable(false)]
        public bool IsBusy
        {
            get { return _backgroundWorker.IsBusy; }
        }

        #endregion

        #region События

        public event DoWorkEventHandler DoWork;

        public event ProgressChangedEventHandler ProgressChanged;

        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        #endregion

        #region Конструктор

        public ProgressDialog()
            : this(null)
        {
        }

        public ProgressDialog(IContainer container)
        {
            if (container != null)
            {
                container.Add(this);
            }

            InitializeComponent();

            ProgressBarStyle = ProgressBarStyle.ProgressBar;
            ShowCancelButton = true;
            MinimizeBox = true;

            if (!Env.IsWindowsVistaOrLater)
            {
                Animation = AnimationResource.GetShellAnimation(Ookii.Dialogs.Wpf.ShellAnimation.FlyingPapers);
            }
        }

        #endregion

        #region Методы

        public void Show()
        {
            Show(null);
        }

        public void Show(object argument)
        {
            RunProgressDialog(IntPtr.Zero, argument);
        }

        public void ShowDialog()
        {
            ShowDialog(null, null);
        }

        public void ShowDialog(Window owner)
        {
            ShowDialog(owner, null);
        }

        public void ShowDialog(Window owner, object argument)
        {
            RunProgressDialog(owner == null ? NativeMethods.GetActiveWindow() : new WindowInteropHelper(owner).Handle, argument);
        }

        public void ReportProgress(int percentProgress)
        {
            ReportProgress(percentProgress, null, null, null);
        }

        public void ReportProgress(int percentProgress, string text, string description)
        {
            ReportProgress(percentProgress, text, description, null);
        }

        public void ReportProgress(int percentProgress, string text, string description, object userState)
        {
            if (percentProgress < 0 || percentProgress > 100)
            {
                throw new ArgumentOutOfRangeException("percentProgress");
            }
            if (_dialog == null)
            {
                throw new InvalidOperationException("ProgressDialogNotRunningError");
            }
            _backgroundWorker.ReportProgress(percentProgress, new ProgressChangedData
            {
                Text = text,
                Description = description,
                UserState = userState
            });
        }

        protected virtual void OnDoWork(DoWorkEventArgs e)
        {
            DoWorkEventHandler handler = DoWork;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            RunWorkerCompletedEventHandler handler = RunWorkerCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = ProgressChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RunProgressDialog(IntPtr owner, object argument)
        {
            if (_backgroundWorker.IsBusy)
            {
                throw new InvalidOperationException("ProgressDialogRunning");
            }

            if (Animation != null)
            {
                try
                {
                    _currentAnimationModuleHandle = Animation.LoadLibrary();
                }
                catch (Win32Exception ex)
                {
                    throw new InvalidOperationException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unable to load the progress dialog animation: {0}", ex.Message), ex);
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    throw new InvalidOperationException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unable to load the progress dialog animation: {0}", ex.Message), ex);
                }
            }

            _cancellationPending = false;
            _dialog = new Interop.ProgressDialog();
            _dialog.SetTitle(WindowTitle);
            if (Animation != null)
            {
                _dialog.SetAnimation(_currentAnimationModuleHandle, (ushort)Animation.ResourceId);
            }

            if (CancellationText.Length > 0)
            {
                _dialog.SetCancelMsg(CancellationText, null);
            }
            _dialog.SetLine(1, Text, UseCompactPathsForText, IntPtr.Zero);
            _dialog.SetLine(2, Description, UseCompactPathsForDescription, IntPtr.Zero);

            Interop.ProgressDialogFlags flags = Ookii.Dialogs.Wpf.Interop.ProgressDialogFlags.Normal;
            if (owner != IntPtr.Zero)
            {
                flags |= Ookii.Dialogs.Wpf.Interop.ProgressDialogFlags.Modal;
            }
            switch (ProgressBarStyle)
            {
                case ProgressBarStyle.None:
                    flags |= Ookii.Dialogs.Wpf.Interop.ProgressDialogFlags.NoProgressBar;
                    break;
                case ProgressBarStyle.MarqueeProgressBar:
                    if (Env.IsWindowsVistaOrLater)
                    {
                        flags |= Ookii.Dialogs.Wpf.Interop.ProgressDialogFlags.MarqueeProgress;
                    }
                    else
                    {
                        flags |= Ookii.Dialogs.Wpf.Interop.ProgressDialogFlags.NoProgressBar;
                    }
                    break;
            }
            if (ShowTimeRemaining)
            {
                flags |= Ookii.Dialogs.Wpf.Interop.ProgressDialogFlags.AutoTime;
            }
            if (!ShowCancelButton)
            {
                flags |= Ookii.Dialogs.Wpf.Interop.ProgressDialogFlags.NoCancel;
            }
            if (!MinimizeBox)
            {
                flags |= Ookii.Dialogs.Wpf.Interop.ProgressDialogFlags.NoMinimize;
            }

            _dialog.StartProgressDialog(owner, null, flags, IntPtr.Zero);
            _backgroundWorker.RunWorkerAsync(argument);
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            OnDoWork(e);
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _dialog.StopProgressDialog();
            Marshal.ReleaseComObject(_dialog);
            _dialog = null;
            if (_currentAnimationModuleHandle != null)
            {
                _currentAnimationModuleHandle.Dispose();
                _currentAnimationModuleHandle = null;
            }

            OnRunWorkerCompleted(new RunWorkerCompletedEventArgs((!e.Cancelled && e.Error == null) ? e.Result : null, e.Error, e.Cancelled));
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _cancellationPending = _dialog.HasUserCancelled();

            if (e.ProgressPercentage >= 0 && e.ProgressPercentage <= 100)
            {
                _dialog.SetProgress((uint)e.ProgressPercentage, 100);
                ProgressChangedData data = e.UserState as ProgressChangedData;
                if (data != null)
                {
                    if (data.Text != null)
                    {
                        Text = data.Text;
                    }
                    if (data.Description != null)
                    {
                        Description = data.Description;
                    }
                    OnProgressChanged(new ProgressChangedEventArgs(e.ProgressPercentage, data.UserState));
                }
            }
        }

        #endregion

        #region Вложенный класс: ProgressChangedData

        private class ProgressChangedData
        {
            #region Свойства

            public string Text { get; set; }

            public string Description { get; set; }

            public object UserState { get; set; }

            #endregion
        }

        #endregion
    }
}