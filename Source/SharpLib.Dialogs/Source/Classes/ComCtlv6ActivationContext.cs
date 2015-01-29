using System;
using System.IO;
using System.Runtime.InteropServices;

using SharpLib;

namespace Ookii.Dialogs.Wpf
{
    internal sealed class ComCtlv6ActivationContext : IDisposable
    {
        #region Поля

        private static readonly object _contextCreationLock = new object();

        private static ActivationContextSafeHandle _activationContext;

        private static bool _contextCreationSucceeded;

        private static NativeMethods.ACTCTX _enableThemingActivationContext;

        private IntPtr _cookie;

        #endregion

        #region Конструктор

        public ComCtlv6ActivationContext(bool enable)
        {
            if (enable && Env.IsWindowsXpOrLater)
            {
                if (EnsureActivateContextCreated())
                {
                    if (!NativeMethods.ActivateActCtx(_activationContext, out _cookie))
                    {
                        _cookie = IntPtr.Zero;
                    }
                }
            }
        }

        ~ComCtlv6ActivationContext()
        {
            Dispose(false);
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_cookie != IntPtr.Zero)
            {
                if (NativeMethods.DeactivateActCtx(0, _cookie))
                {
                    _cookie = IntPtr.Zero;
                }
            }
        }

        private static bool EnsureActivateContextCreated()
        {
            lock (_contextCreationLock)
            {
                if (!_contextCreationSucceeded)
                {
                    string assemblyLoc = null;

                    assemblyLoc = typeof(Object).Assembly.Location;

                    string manifestLoc = null;
                    string installDir = null;
                    if (assemblyLoc != null)
                    {
                        installDir = Path.GetDirectoryName(assemblyLoc);
                        const string manifestName = "XPThemes.manifest";
                        manifestLoc = Path.Combine(installDir, manifestName);
                    }

                    if (manifestLoc != null && installDir != null)
                    {
                        _enableThemingActivationContext = new NativeMethods.ACTCTX();
                        _enableThemingActivationContext.cbSize = Marshal.SizeOf(typeof(NativeMethods.ACTCTX));
                        _enableThemingActivationContext.lpSource = manifestLoc;

                        _enableThemingActivationContext.lpAssemblyDirectory = installDir;
                        _enableThemingActivationContext.dwFlags = NativeMethods.ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID;

                        _activationContext = NativeMethods.CreateActCtx(ref _enableThemingActivationContext);
                        _contextCreationSucceeded = !_activationContext.IsInvalid;
                    }
                }

                return _contextCreationSucceeded;
            }
        }

        #endregion
    }
}