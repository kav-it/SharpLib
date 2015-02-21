using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Standard
{
    internal static class Utility
    {
        #region Поля

        private static readonly Version _osVersion = Environment.OSVersion.Version;

        private static readonly Version _presentationFrameworkVersion = Assembly.GetAssembly(typeof(Window)).GetName().Version;

        private static int _sBitDepth;

        #endregion

        #region Свойства

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsOsVistaOrNewer
        {
            get { return _osVersion >= new Version(6, 0); }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsOsWindows7OrNewer
        {
            get { return _osVersion >= new Version(6, 1); }
        }

        public static bool IsPresentationFrameworkVersionLessThan4
        {
            get { return _presentationFrameworkVersion < new Version(4, 0); }
        }

        #endregion

        #region Методы

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static bool _MemCmp(IntPtr left, IntPtr right, long cb)
        {
            int offset = 0;

            for (; offset < (cb - sizeof (Int64)); offset += sizeof (Int64))
            {
                Int64 left64 = Marshal.ReadInt64(left, offset);
                Int64 right64 = Marshal.ReadInt64(right, offset);

                if (left64 != right64)
                {
                    return false;
                }
            }

            for (; offset < cb; offset += sizeof (byte))
            {
                byte left8 = Marshal.ReadByte(left, offset);
                byte right8 = Marshal.ReadByte(right, offset);

                if (left8 != right8)
                {
                    return false;
                }
            }

            return true;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int Rgb(Color c)
        {
            return c.R | (c.G << 8) | (c.B << 16);
        }

        public static Color ColorFromArgbDword(uint color)
        {
            return Color.FromArgb(
                (byte)((color & 0xFF000000) >> 24),
                (byte)((color & 0x00FF0000) >> 16),
                (byte)((color & 0x0000FF00) >> 8),
                (byte)((color & 0x000000FF) >> 0));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int GET_X_LPARAM(IntPtr lParam)
        {
            return Loword(lParam.ToInt32());
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int GET_Y_LPARAM(IntPtr lParam)
        {
            return Hiword(lParam.ToInt32());
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int Hiword(int i)
        {
            return (short)(i >> 16);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int Loword(int i)
        {
            return (short)(i & 0xFFFF);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static bool AreStreamsEqual(Stream left, Stream right)
        {
            if (null == left)
            {
                return right == null;
            }
            if (null == right)
            {
                return false;
            }

            if (!left.CanRead || !right.CanRead)
            {
                throw new NotSupportedException("The streams can't be read for comparison");
            }

            if (left.Length != right.Length)
            {
                return false;
            }

            var length = (int)left.Length;

            left.Position = 0;
            right.Position = 0;

            int totalReadLeft = 0;
            int totalReadRight = 0;

            int cbReadLeft = 0;
            int cbReadRight = 0;

            var leftBuffer = new byte[512];
            var rightBuffer = new byte[512];

            var handleLeft = GCHandle.Alloc(leftBuffer, GCHandleType.Pinned);
            var ptrLeft = handleLeft.AddrOfPinnedObject();

            var handleRight = GCHandle.Alloc(rightBuffer, GCHandleType.Pinned);
            var ptrRight = handleRight.AddrOfPinnedObject();

            try
            {
                while (totalReadLeft < length)
                {
                    Assert.AreEqual(totalReadLeft, totalReadRight);

                    cbReadLeft = left.Read(leftBuffer, 0, leftBuffer.Length);
                    cbReadRight = right.Read(rightBuffer, 0, rightBuffer.Length);

                    if (cbReadLeft != cbReadRight)
                    {
                        return false;
                    }

                    if (!_MemCmp(ptrLeft, ptrRight, cbReadLeft))
                    {
                        return false;
                    }

                    totalReadLeft += cbReadLeft;
                    totalReadRight += cbReadRight;
                }

                Assert.AreEqual(cbReadLeft, cbReadRight);
                Assert.AreEqual(totalReadLeft, totalReadRight);
                Assert.AreEqual(length, totalReadLeft);

                return true;
            }
            finally
            {
                handleLeft.Free();
                handleRight.Free();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool GuidTryParse(string guidString, out Guid guid)
        {
            Verify.IsNeitherNullNorEmpty(guidString, "guidString");

            try
            {
                guid = new Guid(guidString);
                return true;
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }

            guid = default(Guid);
            return false;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsFlagSet(int value, int mask)
        {
            return 0 != (value & mask);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsFlagSet(uint value, uint mask)
        {
            return 0 != (value & mask);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsFlagSet(long value, long mask)
        {
            return 0 != (value & mask);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsFlagSet(ulong value, ulong mask)
        {
            return 0 != (value & mask);
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr GenerateHicon(ImageSource image, Size dimensions)
        {
            if (image == null)
            {
                return IntPtr.Zero;
            }

            var bf = image as BitmapFrame;
            if (bf != null)
            {
                bf = GetBestMatch(bf.Decoder.Frames, (int)dimensions.Width, (int)dimensions.Height);
            }
            else
            {
                var drawingDimensions = new Rect(0, 0, dimensions.Width, dimensions.Height);

                double renderRatio = dimensions.Width / dimensions.Height;
                double aspectRatio = image.Width / image.Height;

                if (image.Width <= dimensions.Width && image.Height <= dimensions.Height)
                {
                    drawingDimensions = new Rect((dimensions.Width - image.Width) / 2, (dimensions.Height - image.Height) / 2, image.Width, image.Height);
                }
                else if (renderRatio > aspectRatio)
                {
                    double scaledRenderWidth = (image.Width / image.Height) * dimensions.Width;
                    drawingDimensions = new Rect((dimensions.Width - scaledRenderWidth) / 2, 0, scaledRenderWidth, dimensions.Height);
                }
                else if (renderRatio < aspectRatio)
                {
                    double scaledRenderHeight = (image.Height / image.Width) * dimensions.Height;
                    drawingDimensions = new Rect(0, (dimensions.Height - scaledRenderHeight) / 2, dimensions.Width, scaledRenderHeight);
                }

                var dv = new DrawingVisual();
                var dc = dv.RenderOpen();
                dc.DrawImage(image, drawingDimensions);
                dc.Close();

                var bmp = new RenderTargetBitmap((int)dimensions.Width, (int)dimensions.Height, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(dv);
                bf = BitmapFrame.Create(bmp);
            }

            using (var memstm = new MemoryStream())
            {
                BitmapEncoder enc = new PngBitmapEncoder();
                enc.Frames.Add(bf);
                enc.Save(memstm);

                using (var istm = new ManagedIStream(memstm))
                {
                    var bitmap = IntPtr.Zero;
                    try
                    {
                        var gpStatus = NativeMethods.GdipCreateBitmapFromStream(istm, out bitmap);
                        if (Status.Ok != gpStatus)
                        {
                            return IntPtr.Zero;
                        }

                        IntPtr hicon;
                        gpStatus = NativeMethods.GdipCreateHICONFromBitmap(bitmap, out hicon);
                        if (Status.Ok != gpStatus)
                        {
                            return IntPtr.Zero;
                        }

                        return hicon;
                    }
                    finally
                    {
                        SafeDisposeImage(ref bitmap);
                    }
                }
            }
        }

        public static BitmapFrame GetBestMatch(IList<BitmapFrame> frames, int width, int height)
        {
            return _GetBestMatch(frames, _GetBitDepth(), width, height);
        }

        private static int _MatchImage(BitmapFrame frame, int bitDepth, int width, int height, int bpp)
        {
            int score = 2 * _WeightedAbs(bpp, bitDepth, false) +
                        _WeightedAbs(frame.PixelWidth, width, true) +
                        _WeightedAbs(frame.PixelHeight, height, true);

            return score;
        }

        private static int _WeightedAbs(int valueHave, int valueWant, bool fPunish)
        {
            int diff = (valueHave - valueWant);

            if (diff < 0)
            {
                diff = (fPunish ? -2 : -1) * diff;
            }

            return diff;
        }

        private static BitmapFrame _GetBestMatch(IList<BitmapFrame> frames, int bitDepth, int width, int height)
        {
            int bestScore = int.MaxValue;
            int bestBpp = 0;
            int bestIndex = 0;

            bool isBitmapIconDecoder = frames[0].Decoder is IconBitmapDecoder;

            for (int i = 0; i < frames.Count && bestScore != 0; ++i)
            {
                int currentIconBitDepth = isBitmapIconDecoder ? frames[i].Thumbnail.Format.BitsPerPixel : frames[i].Format.BitsPerPixel;

                if (currentIconBitDepth == 0)
                {
                    currentIconBitDepth = 8;
                }

                int score = _MatchImage(frames[i], bitDepth, width, height, currentIconBitDepth);
                if (score < bestScore)
                {
                    bestIndex = i;
                    bestBpp = currentIconBitDepth;
                    bestScore = score;
                }
                else if (score == bestScore)
                {
                    if (bestBpp < currentIconBitDepth)
                    {
                        bestIndex = i;
                        bestBpp = currentIconBitDepth;
                    }
                }
            }

            return frames[bestIndex];
        }

        private static int _GetBitDepth()
        {
            if (_sBitDepth == 0)
            {
                using (var dc = SafeDC.GetDesktop())
                {
                    _sBitDepth = NativeMethods.GetDeviceCaps(dc, DeviceCap.BITSPIXEL) * NativeMethods.GetDeviceCaps(dc, DeviceCap.PLANES);
                }
            }
            return _sBitDepth;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDeleteFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                File.Delete(path);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDeleteObject(ref IntPtr gdiObject)
        {
            var p = gdiObject;
            gdiObject = IntPtr.Zero;
            if (IntPtr.Zero != p)
            {
                NativeMethods.DeleteObject(p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDestroyIcon(ref IntPtr hicon)
        {
            var p = hicon;
            hicon = IntPtr.Zero;
            if (IntPtr.Zero != p)
            {
                NativeMethods.DestroyIcon(p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDestroyWindow(ref IntPtr hwnd)
        {
            var p = hwnd;
            hwnd = IntPtr.Zero;
            if (NativeMethods.IsWindow(p))
            {
                NativeMethods.DestroyWindow(p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDispose<T>(ref T disposable) where T : IDisposable
        {
            IDisposable t = disposable;
            disposable = default(T);
            if (null != t)
            {
                t.Dispose();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDisposeImage(ref IntPtr gdipImage)
        {
            var p = gdipImage;
            gdipImage = IntPtr.Zero;
            if (IntPtr.Zero != p)
            {
                NativeMethods.GdipDisposeImage(p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void SafeCoTaskMemFree(ref IntPtr ptr)
        {
            var p = ptr;
            ptr = IntPtr.Zero;
            if (IntPtr.Zero != p)
            {
                Marshal.FreeCoTaskMem(p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void SafeFreeHGlobal(ref IntPtr hglobal)
        {
            var p = hglobal;
            hglobal = IntPtr.Zero;
            if (IntPtr.Zero != p)
            {
                Marshal.FreeHGlobal(p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void SafeRelease<T>(ref T comObject) where T : class
        {
            var t = comObject;
            comObject = default(T);
            if (null != t)
            {
                Assert.IsTrue(Marshal.IsComObject(t));
                Marshal.ReleaseComObject(t);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void GeneratePropertyString(StringBuilder source, string propertyName, string value)
        {
            Assert.IsNotNull(source);
            Assert.IsFalse(string.IsNullOrEmpty(propertyName));

            if (0 != source.Length)
            {
                source.Append(' ');
            }

            source.Append(propertyName);
            source.Append(": ");
            if (string.IsNullOrEmpty(value))
            {
                source.Append("<null>");
            }
            else
            {
                source.Append('\"');
                source.Append(value);
                source.Append('\"');
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [Obsolete]
        public static string GenerateToString<T>(T @object) where T : struct
        {
            var sbRet = new StringBuilder();
            foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (0 != sbRet.Length)
                {
                    sbRet.Append(", ");
                }
                Assert.AreEqual(0, property.GetIndexParameters().Length);
                var value = property.GetValue(@object, null);
                string format = null == value ? "{0}: <null>" : "{0}: \"{1}\"";
                sbRet.AppendFormat(format, property.Name, value);
            }
            return sbRet.ToString();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void CopyStream(Stream destination, Stream source)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(destination);

            destination.Position = 0;

            if (source.CanSeek)
            {
                source.Position = 0;

                destination.SetLength(source.Length);
            }

            var buffer = new byte[4096];
            int cbRead;

            do
            {
                cbRead = source.Read(buffer, 0, buffer.Length);
                if (0 != cbRead)
                {
                    destination.Write(buffer, 0, cbRead);
                }
            } while (buffer.Length == cbRead);

            destination.Position = 0;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string HashStreamMd5(Stream stm)
        {
            stm.Position = 0;
            var hashBuilder = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                foreach (byte b in md5.ComputeHash(stm))
                {
                    hashBuilder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                }
            }

            return hashBuilder.ToString();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool MemCmp(byte[] left, byte[] right, int cb)
        {
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);

            Assert.IsTrue(cb <= Math.Min(left.Length, right.Length));

            var handleLeft = GCHandle.Alloc(left, GCHandleType.Pinned);
            var ptrLeft = handleLeft.AddrOfPinnedObject();

            var handleRight = GCHandle.Alloc(right, GCHandleType.Pinned);
            var ptrRight = handleRight.AddrOfPinnedObject();

            bool fRet = _MemCmp(ptrLeft, ptrRight, cb);

            handleLeft.Free();
            handleRight.Free();

            return fRet;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string UrlDecode(string url)
        {
            if (url == null)
            {
                return null;
            }

            var decoder = new UrlDecoder(url.Length, Encoding.UTF8);
            int length = url.Length;
            for (int i = 0; i < length; ++i)
            {
                char ch = url[i];

                if (ch == '+')
                {
                    decoder.AddByte((byte)' ');
                    continue;
                }

                if (ch == '%' && i < length - 2)
                {
                    if (url[i + 1] == 'u' && i < length - 5)
                    {
                        int a = _HexToInt(url[i + 2]);
                        int b = _HexToInt(url[i + 3]);
                        int c = _HexToInt(url[i + 4]);
                        int d = _HexToInt(url[i + 5]);
                        if (a >= 0 && b >= 0 && c >= 0 && d >= 0)
                        {
                            decoder.AddChar((char)((a << 12) | (b << 8) | (c << 4) | d));
                            i += 5;

                            continue;
                        }
                    }
                    else
                    {
                        int a = _HexToInt(url[i + 1]);
                        int b = _HexToInt(url[i + 2]);

                        if (a >= 0 && b >= 0)
                        {
                            decoder.AddByte((byte)((a << 4) | b));
                            i += 2;

                            continue;
                        }
                    }
                }

                if ((ch & 0xFF80) == 0)
                {
                    decoder.AddByte((byte)ch);
                }
                else
                {
                    decoder.AddChar(ch);
                }
            }

            return decoder.GetString();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string UrlEncode(string url)
        {
            if (url == null)
            {
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(url);

            bool needsEncoding = false;
            int unsafeCharCount = 0;
            foreach (byte b in bytes)
            {
                if (b == ' ')
                {
                    needsEncoding = true;
                }
                else if (!_UrlEncodeIsSafe(b))
                {
                    ++unsafeCharCount;
                    needsEncoding = true;
                }
            }

            if (needsEncoding)
            {
                var buffer = new byte[bytes.Length + (unsafeCharCount * 2)];
                int writeIndex = 0;
                foreach (byte b in bytes)
                {
                    if (_UrlEncodeIsSafe(b))
                    {
                        buffer[writeIndex++] = b;
                    }
                    else if (b == ' ')
                    {
                        buffer[writeIndex++] = (byte)'+';
                    }
                    else
                    {
                        buffer[writeIndex++] = (byte)'%';
                        buffer[writeIndex++] = _IntToHex((b >> 4) & 0xF);
                        buffer[writeIndex++] = _IntToHex(b & 0xF);
                    }
                }
                bytes = buffer;
                Assert.AreEqual(buffer.Length, writeIndex);
            }

            return Encoding.ASCII.GetString(bytes);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static bool _UrlEncodeIsSafe(byte b)
        {
            if (_IsAsciiAlphaNumeric(b))
            {
                return true;
            }

            switch ((char)b)
            {
                case '-':
                case '_':
                case '.':
                case '!':

                case '*':
                case '\'':
                case '(':
                case ')':
                    return true;
            }

            return false;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static bool _IsAsciiAlphaNumeric(byte b)
        {
            return (b >= 'a' && b <= 'z')
                   || (b >= 'A' && b <= 'Z')
                   || (b >= '0' && b <= '9');
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static byte _IntToHex(int n)
        {
            Assert.BoundedInteger(0, n, 16);
            if (n <= 9)
            {
                return (byte)(n + '0');
            }
            return (byte)(n - 10 + 'A');
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static int _HexToInt(char h)
        {
            if (h >= '0' && h <= '9')
            {
                return h - '0';
            }

            if (h >= 'a' && h <= 'f')
            {
                return h - 'a' + 10;
            }

            if (h >= 'A' && h <= 'F')
            {
                return h - 'A' + 10;
            }

            Assert.Fail("Invalid hex character " + h);
            return -1;
        }

        public static void AddDependencyPropertyChangeListener(object component, DependencyProperty property, EventHandler listener)
        {
            if (component == null)
            {
                return;
            }
            Assert.IsNotNull(property);
            Assert.IsNotNull(listener);

            var dpd = DependencyPropertyDescriptor.FromProperty(property, component.GetType());
            dpd.AddValueChanged(component, listener);
        }

        public static void RemoveDependencyPropertyChangeListener(object component, DependencyProperty property, EventHandler listener)
        {
            if (component == null)
            {
                return;
            }
            Assert.IsNotNull(property);
            Assert.IsNotNull(listener);

            var dpd = DependencyPropertyDescriptor.FromProperty(property, component.GetType());
            dpd.RemoveValueChanged(component, listener);
        }

        public static bool IsThicknessNonNegative(Thickness thickness)
        {
            if (!IsDoubleFiniteAndNonNegative(thickness.Top))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(thickness.Left))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(thickness.Bottom))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(thickness.Right))
            {
                return false;
            }

            return true;
        }

        public static bool IsCornerRadiusValid(CornerRadius cornerRadius)
        {
            if (!IsDoubleFiniteAndNonNegative(cornerRadius.TopLeft))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(cornerRadius.TopRight))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(cornerRadius.BottomLeft))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(cornerRadius.BottomRight))
            {
                return false;
            }

            return true;
        }

        public static bool IsDoubleFiniteAndNonNegative(double d)
        {
            if (double.IsNaN(d) || double.IsInfinity(d) || d < 0)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Вложенный класс: _UrlDecoder

        private class UrlDecoder
        {
            #region Поля

            private readonly byte[] _byteBuffer;

            private readonly char[] _charBuffer;

            private readonly Encoding _encoding;

            private int _byteCount;

            private int _charCount;

            #endregion

            #region Конструктор

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public UrlDecoder(int size, Encoding encoding)
            {
                _encoding = encoding;
                _charBuffer = new char[size];
                _byteBuffer = new byte[size];
            }

            #endregion

            #region Методы

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public void AddByte(byte b)
            {
                _byteBuffer[_byteCount++] = b;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public void AddChar(char ch)
            {
                _FlushBytes();
                _charBuffer[_charCount++] = ch;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            private void _FlushBytes()
            {
                if (_byteCount > 0)
                {
                    _charCount += _encoding.GetChars(_byteBuffer, 0, _byteCount, _charBuffer, _charCount);
                    _byteCount = 0;
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public string GetString()
            {
                _FlushBytes();
                if (_charCount > 0)
                {
                    return new string(_charBuffer, 0, _charCount);
                }
                return "";
            }

            #endregion
        }

        #endregion
    }
}