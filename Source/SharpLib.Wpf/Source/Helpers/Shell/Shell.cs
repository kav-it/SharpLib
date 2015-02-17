using System.IO;
using System.Linq;

using SharpLib.Native.Windows;

namespace SharpLib.Wpf
{
    public static class Shell
    {
        /// <summary>
        /// Определение иконки для файла/директории/диска
        /// </summary>
        internal static System.Windows.Media.Imaging.BitmapSource GetIconByLocation(string filename, bool largeSize)
        {
            System.Windows.Media.Imaging.BitmapSource bitmap = null;

            bool validDrive = DriveInfo.GetDrives().Any(x => x.Name.EqualsOrdinalEx(filename));

            if ((File.Exists(filename)) || (Directory.Exists(filename)) || (validDrive))
            {
                using (System.Drawing.Icon sysIcon = largeSize ? NativeMethods.GetLargeIcon(filename) : NativeMethods.GetSmallIcon(filename))
                {
                    try
                    {
                        bitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            sysIcon.Handle,
                            System.Windows.Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(sysIcon.Width, sysIcon.Height));
                    }
                    catch
                    {
                        bitmap = null;
                    }
                }
            }

            return bitmap;
        }
    }
}
