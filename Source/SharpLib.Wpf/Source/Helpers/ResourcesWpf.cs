using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Resources;

namespace SharpLib.Wpf
{
    /// <summary>
    /// Класс помощник работы с ресурсами WPF
    /// </summary>
    public static class ResourcesWpf
    {
        /// <summary>
        /// Формирование полного пути Uri
        /// </summary>
        private static Uri GetFullUri(String absolutPath, Assembly assembly)
        {
            // Регистрация схемы "pack" для определения путей к ресурсам (без создания Application (WPF))
            if (System.IO.Packaging.PackUriHelper.UriSchemePack == null)
                throw new NotImplementedException();

            if (assembly == null)
            { 
                assembly = Assembly.GetEntryAssembly();
            }

            absolutPath = absolutPath.TrimStart('/');
            string uriPath = String.Format(@"pack://application:,,,/{0};component/{1}", assembly.GetName().Name, absolutPath);
            Uri uri = new Uri(uriPath, UriKind.Absolute);

            return uri;
        }

        /// <summary>
        /// Загрузка информации о запрошенном ресурсе
        /// </summary>
        public static StreamResourceInfo LoadStreamResource(String absolutPath, Assembly assembly)
        {
            var uri = GetFullUri(absolutPath, assembly);

            StreamResourceInfo streamResource = Application.GetResourceStream(uri);

            return streamResource;
        }

        /// <summary>
        /// Загрузка потока по указанному адреса
        /// </summary>
        public static Stream LoadStream(String absolutPath, Assembly assembly)
        {
            var streamResource = LoadStreamResource(absolutPath, assembly);

            return streamResource == null ? null : streamResource.Stream;
        }

        /// <summary>
        /// Загрузка текста из ресурсов 
        /// </summary>
        public static String LoadText(String absolutPath, Assembly asm = null)
        {
            var stream = LoadStream(absolutPath, asm);
            var text = stream.ToStringEx();

            return text;
        }

        /// <summary>
        /// Загрузка буфера из ресурсов 
        /// </summary>
        public static byte[] LoadBuffer(string absolutPath, Assembly asm = null)
        {
            var stream = LoadStream(absolutPath, asm);
            var data = stream.ToByfferEx();

            return data;
        }

        /// <summary>
        /// Загрузка изображения из ресурсов
        /// </summary>
        public static Image LoadImage(string absolutPath, Assembly asm = null)
        {
            var stream = LoadStreamResource(absolutPath, asm);

            var bitmap = new System.Windows.Media.Imaging.BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream.Stream;
            bitmap.EndInit();

            Image image = new Image();

            image.Source = bitmap;
            image.Width = bitmap.PixelWidth;
            image.Height = bitmap.PixelHeight;

            return image;
        }

        /// <summary>
        /// Загрузка изображения из ресурсов в формате BitmapSource
        /// </summary>
        public static ImageSource LoadImageSource(String absolutPath, Assembly asm = null)
        {
            return LoadImage(absolutPath, asm).Source;
        }

    }
}
