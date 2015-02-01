using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SharpLib.Wpf.Dragging.Icons
{
    public static class IconFactory
    {
        #region Свойства

        public static BitmapImage EffectNone
        {
            get { return GetImage("EffectNone.png", 12); }
        }

        public static BitmapImage EffectCopy
        {
            get { return GetImage("EffectCopy.png", 12); }
        }

        public static BitmapImage EffectMove
        {
            get { return GetImage("EffectMove.png", 12); }
        }

        public static BitmapImage EffectLink
        {
            get { return GetImage("EffectLink.png", 12); }
        }

        #endregion

        #region Методы

        private static BitmapImage GetImage(string iconName, int size)
        {
            var path = string.Format("Source/Helpers/DragDrop/Icons/{0}", iconName);
            var icon = Assembly.GetExecutingAssembly().LoadImageEx(path);

            icon.DecodePixelHeight = size;
            icon.DecodePixelWidth = size;

            return icon;
        }

        public static Cursor CreateCursor(double rx, double ry, SolidColorBrush brush, Pen pen)
        {
            var vis = new DrawingVisual();
            using (var dc = vis.RenderOpen())
            {
                dc.DrawRectangle(brush, new Pen(Brushes.Black, 0.1), new Rect(0, 0, rx, ry));
                dc.Close();
            }
            var rtb = new RenderTargetBitmap(64, 64, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(vis);

            using (var ms1 = new MemoryStream())
            {
                var penc = new PngBitmapEncoder();
                penc.Frames.Add(BitmapFrame.Create(rtb));
                penc.Save(ms1);

                var pngBytes = ms1.ToArray();
                var size = pngBytes.GetLength(0);

                using (var ms = new MemoryStream())
                {
                    {
                        ms.Write(BitConverter.GetBytes((Int16)0), 0, 2);
                        ms.Write(BitConverter.GetBytes((Int16)2), 0, 2);
                        ms.Write(BitConverter.GetBytes((Int16)1), 0, 2);
                    }

                    {
                        ms.WriteByte(32);
                        ms.WriteByte(32);

                        ms.WriteByte(0);
                        ms.WriteByte(0);

                        ms.Write(BitConverter.GetBytes((Int16)(rx / 2.0)), 0, 2);
                        ms.Write(BitConverter.GetBytes((Int16)(ry / 2.0)), 0, 2);

                        ms.Write(BitConverter.GetBytes(size), 0, 4);
                        ms.Write(BitConverter.GetBytes(22), 0, 4);
                    }

                    ms.Write(pngBytes, 0, size);
                    ms.Seek(0, SeekOrigin.Begin);
                    return new Cursor(ms);
                }
            }
        }

        #endregion
    }
}