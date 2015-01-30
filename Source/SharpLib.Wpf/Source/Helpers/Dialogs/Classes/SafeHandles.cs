// Copyright © Sven Groot (Ookii.org) 2009
// BSD license; see license.txt for details.

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using Microsoft.Win32.SafeHandles;

namespace SharpLib.Wpf.Dialogs
{
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class SafeModuleHandle : SafeHandle
    {
        #region Свойства

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        #endregion

        #region Конструктор

        public SafeModuleHandle()
            : base(IntPtr.Zero, true)
        {
        }

        #endregion

        #region Методы

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return DialogNativeMethods.FreeLibrary(handle);
        }

        #endregion
    }

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class ActivationContextSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region Конструктор

        public ActivationContextSafeHandle()
            : base(true)
        {
        }

        #endregion

        #region Методы

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            DialogNativeMethods.ReleaseActCtx(handle);
            return true;
        }

        #endregion
    }
}