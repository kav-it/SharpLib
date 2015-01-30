using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace SharpLib.Wpf.Dialogs
{
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class SafeModuleHandle : SafeHandle
    {
        #region ��������

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        #endregion

        #region �����������

        public SafeModuleHandle()
            : base(IntPtr.Zero, true)
        {
        }

        #endregion

        #region ������

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return DialogNativeMethods.FreeLibrary(handle);
        }

        #endregion
    }
}