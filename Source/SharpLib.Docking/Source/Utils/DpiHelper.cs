using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;

namespace Standard
{
    internal static class DpiHelper
    {
        #region Поля

        private static Matrix _transformToDevice;

        private static Matrix _transformToDip;

        #endregion

        #region Конструктор

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DpiHelper()
        {
            using (var desktop = SafeDC.GetDesktop())
            {
                int pixelsPerInchX = NativeMethods.GetDeviceCaps(desktop, DeviceCap.LOGPIXELSX);
                int pixelsPerInchY = NativeMethods.GetDeviceCaps(desktop, DeviceCap.LOGPIXELSY);

                _transformToDip = Matrix.Identity;
                _transformToDip.Scale(96d / pixelsPerInchX, 96d / pixelsPerInchY);
                _transformToDevice = Matrix.Identity;
                _transformToDevice.Scale(pixelsPerInchX / 96d, pixelsPerInchY / 96d);
            }
        }

        #endregion

        #region Методы

        public static Point LogicalPixelsToDevice(Point logicalPoint)
        {
            return _transformToDevice.Transform(logicalPoint);
        }

        public static Point DevicePixelsToLogical(Point devicePoint)
        {
            return _transformToDip.Transform(devicePoint);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static Rect LogicalRectToDevice(Rect logicalRectangle)
        {
            var topLeft = LogicalPixelsToDevice(new Point(logicalRectangle.Left, logicalRectangle.Top));
            var bottomRight = LogicalPixelsToDevice(new Point(logicalRectangle.Right, logicalRectangle.Bottom));

            return new Rect(topLeft, bottomRight);
        }

        public static Rect DeviceRectToLogical(Rect deviceRectangle)
        {
            var topLeft = DevicePixelsToLogical(new Point(deviceRectangle.Left, deviceRectangle.Top));
            var bottomRight = DevicePixelsToLogical(new Point(deviceRectangle.Right, deviceRectangle.Bottom));

            return new Rect(topLeft, bottomRight);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static Size LogicalSizeToDevice(Size logicalSize)
        {
            var pt = LogicalPixelsToDevice(new Point(logicalSize.Width, logicalSize.Height));

            return new Size
            {
                Width = pt.X,
                Height = pt.Y
            };
        }

        public static Size DeviceSizeToLogical(Size deviceSize)
        {
            var pt = DevicePixelsToLogical(new Point(deviceSize.Width, deviceSize.Height));

            return new Size(pt.X, pt.Y);
        }

        #endregion
    }
}