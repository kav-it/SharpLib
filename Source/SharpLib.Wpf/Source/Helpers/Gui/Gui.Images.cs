using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SharpLib.Wpf
{
    /// <summary>
    /// Встроенные иконки
    /// </summary>
    public static class GuiImages
    {
        #region Константы

        private const string IMAGE_PATH = "Source/Assets/png/";

        #endregion

        #region Свойства

        public static BitmapSource IconAbout { get; private set; }

        public static BitmapSource IconError { get; private set; }

        public static BitmapSource IconWarning { get; private set; }

        public static BitmapSource IconInfo { get; private set; }

        public static BitmapSource IconQuestion { get; private set; }

        public static BitmapSource IconDelete { get; private set; }

        #endregion

        #region Конструктор

        static GuiImages()
        {
            IconError = IconToBitmapSource(System.Drawing.SystemIcons.Error);
            IconWarning = IconToBitmapSource(System.Drawing.SystemIcons.Warning);
            IconInfo = IconToBitmapSource(System.Drawing.SystemIcons.Information);
            IconQuestion = IconToBitmapSource(System.Drawing.SystemIcons.Question);

            var asm = Assembly.GetExecutingAssembly();

            IconAbout = asm.LoadImageEx(IMAGE_PATH + "icon.about.png");
            IconDelete = asm.LoadImageEx(IMAGE_PATH + "icon.delete.png");
        }

        #endregion

        #region Методы

        private static BitmapSource IconToBitmapSource(System.Drawing.Icon icon)
        {
            var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return source;
        }

        #endregion
    }
}