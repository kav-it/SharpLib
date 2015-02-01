using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace SharpLib.Wpf
{
    public static class ExtensionAssembly
    {
        #region Методы

        public static BitmapImage LoadImageEx(this Assembly self, string absolutPath)
        {
            absolutPath = absolutPath.TrimStart('/');
            var name = self.GetName().Name;
            var temp = string.Format("pack://application:,,,/{0};component/{1}", name, absolutPath);
            var uri = new Uri(temp);

            var img = new BitmapImage(uri);

            return img;
        }

        #endregion
    }
}