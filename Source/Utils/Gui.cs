// ****************************************************************************
//
// Имя файла    : 'Gui.cs'
// Заголовок    : Вспомогательный модуль визуализации
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/06/2012
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SharpLib
{
    #region Делегат GuiSynchronizeHandler
    public delegate void GuiSynchronizeHandler (Object data);
    #endregion Делегат GuiSynchronizeHandler

    #region Перечисление ImageTyp
    public enum ImageTyp
    {
        Unknow,
        Png,
        Bmp,
        Tif,
        Jpg,
        Gif
    }
    #endregion Перечисление ImageTyp

    #region Перечисление NotifyBalloonIcon
    public enum NotifyBalloonIcon
    {
        None,
        Info,
        Warning,
        Error,
    }
    #endregion Перечисление

    #region Класс Gui
    public static class Gui
    {
        #region Методы
        public static void Synchronize (GuiSynchronizeHandler handler, Object data)
        {
            if (Application.Current != null && handler != null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    (Action)(() => { handler(data); })
                );
            }
        }
        public static Brush HexToColor(String color)
        {
            BrushConverter bc = new BrushConverter();

            return (Brush)bc.ConvertFrom(color);
        }
        public static IntPtr GetIconHandle(ImageSource source)
        {
            System.Windows.Media.Imaging.BitmapFrame frame = source as System.Windows.Media.Imaging.BitmapFrame;

            if (frame != null && frame.Decoder.Frames.Count > 0)
            {
                frame = frame.Decoder.Frames[0];

                int width = frame.PixelWidth;
                int height = frame.PixelHeight;
                int stride = width * ((frame.Format.BitsPerPixel + 7) / 8);

                Byte[] bits = new Byte[height * stride];

                frame.CopyPixels(bits, stride, 0);

                GCHandle gcHandle = GCHandle.Alloc(bits, GCHandleType.Pinned);

                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(
                    width,
                    height,
                    stride,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb,
                    gcHandle.AddrOfPinnedObject());

                IntPtr hIcon = bitmap.GetHicon();

                gcHandle.Free();

                return hIcon;
            }

            return IntPtr.Zero;
        }
        public static ImageSource BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
        public static void SaveToClipboard(ImageSource imageSource)
        {
            BitmapSource bitmapSource = imageSource as BitmapSource;

            if (bitmapSource != null)
                Clipboard.SetImage(bitmapSource);
        }
        public static Point GetMousePosition()
        {
            NativeMethods.POINT winPoint;
            NativeMethods.GetCursorPos(out winPoint);

            Point point = new Point(winPoint.X, winPoint.Y);

            return point;
        }
        public static MemoryStream ImageSourceToStream(ImageSource imageSource, ImageTyp typ)
        {
            BitmapSource  bitmapSource = imageSource as BitmapSource;
            BitmapEncoder encoder = null;

            switch (typ)
            {
                // Тип неизвестен: Берется из разрешения
                case ImageTyp.Bmp: encoder = new BmpBitmapEncoder(); break;
                case ImageTyp.Tif: encoder = new TiffBitmapEncoder(); break;
                case ImageTyp.Jpg: encoder = new JpegBitmapEncoder(); break;
                case ImageTyp.Gif: encoder = new GifBitmapEncoder(); break;

                case ImageTyp.Png:
                default: encoder = new PngBitmapEncoder(); break;
            }

            // Добавление кадра изображения
            BitmapFrame frame = BitmapFrame.Create(bitmapSource);
            encoder.Frames.Add(frame);

            // Сохранение картинки в поток (память)
            MemoryStream stream = new MemoryStream();
            encoder.Save(stream);

            return stream;
        }
        public static ImageSource StreamToImageSource(MemoryStream stream)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            return bitmapImage;
        }
        #endregion Методы
    }
    #endregion Класс Gui

    #region Класс GuiImages
    public class GuiImages
    {
        #region Константы
        // Для ресурсов WPF добавленных как Link (используется имя файла без префикса пути)
        private const String IMAGE_PATH = @"";
        // Дома на VS2010 (10.0.40219.1 SP1Rel)
        // private static String IMAGE_PATH = @"Source/SharpLib/Images/";
        #endregion Константы

        #region Свойства
        public static Image IconError
        {
            get 
            {
                Image iconError = new Image();
                iconError.Source = IconToImageSource(System.Drawing.SystemIcons.Error);

                return iconError; 
            }
        }
        public static Image IconWarning
        {
            get 
            {
                Image iconWarning = new Image();
                iconWarning.Source = IconToImageSource(System.Drawing.SystemIcons.Warning);
                
                return iconWarning; 
            }
        }
        public static Image IconInformation
        {
            get 
            {
                Image iconInformation = new Image();
                iconInformation.Source = IconToImageSource(System.Drawing.SystemIcons.Information);
                
                return iconInformation; 
            }
        }
        public static Image IconQuestion
        {
            get 
            {
                Image iconQuestion = new Image();
                iconQuestion.Source = IconToImageSource(System.Drawing.SystemIcons.Question);
                
                return iconQuestion; 
            }
        }
        public static Image IconCmd
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "icon.cmd.png"); }
        }
        public static Image IconBatFile
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "icon.batfile.png"); }
        }
        public static Image IconSettings
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "icon.settings.png"); }
        }
        public static Image IconAdd
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "icon.add.png"); }
        }
        public static Image IconDelete
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "icon.delete.png"); }
        }
        public static Image IconDeleteLight
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "icon.delete.light.png"); }
        }
        public static Image IconOpen
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "icon.open.png"); }
        }
        public static Image IconAbout
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "icon.about.png"); }
        }
        public static Image ShlGuard
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.guard.png"); }
        }
        public static Image ShlFire
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.fire.png"); }
        }
        public static Image ShlFireTwoSmoke
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.fire.twosmoke.png"); }
        }
        public static Image ShlFireOneSmoke
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.fire.onesmoke.png"); }
        }
        public static Image ShlFireTwoHot
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.fire.twohot.png"); }
        }
        public static Image ShlFireOne
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.fire.one.png"); }
        }
        public static Image ShlGuardAlways
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.guard.always.png"); }
        }
        public static Image ShlGuardAuto
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.guard.auto.png"); }
        }
        public static Image ShlGuardNight
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.guard.night.png"); }
        }
        public static Image ShlGuardPause
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.guard.pause.png"); }
        }
        public static Image ShlGuardQuiet
        {
            get  { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.guard.quiet.png"); }
        }
        public static Image ShlGuardQuick
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.guard.quick.png"); }
        }
        public static Image ShlGuardSched
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "shl.guard.sched.png"); }
        }
        public static Image PcnDispatcher
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "pcn.dispatcher.png"); }
        }
        public static Image PcnConnect
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "pcn.connect.png"); }
        }
        public static Image PcnDisconnect
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "pcn.disconnect.png"); }
        }
        public static Image PcnSearch
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "pcn.search.png"); }
        }
        public static Image PcnUsers
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "pcn.users.png"); }
        }
        public static Image PcnDevices
        {
            get { return ResourcesWpf.LoadImage(IMAGE_PATH + "pcn.devices.png"); }
        }
        #endregion Свойства

        #region Методы 
        public static ImageSource IconToImageSource(System.Drawing.Icon icon)
        {
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
        #endregion Методы 
    }
    #endregion Класс GuiImages

    #region Класс ImageBase
    public class ImageBase
    {
        #region Поля
        private System.Drawing.Image _image;
        private Byte[] _data;
        #endregion Поля

        #region Свойства
        public Byte[] Data
        {
            get { return _data; }
        }
        #endregion Свойства

        #region Конструктор
        public ImageBase()
        {
            _image = null;
            _data  = null;
        }
        public ImageBase(Byte[] data): this()
        {
            LoadFromBuffer(data);
        }
        public ImageBase(String name, bool resource): this()
        {
            Byte[] data;

            if (resource)
                data = ResourcesWpf.GetStream(name).ToByfferEx();
            else
                data = Files.Read(name);

            if (data != null)
            {
                LoadFromBuffer(data);    
            }
        }
        #endregion Конструктор

        #region Методы
        public void LoadFromBuffer(Byte[] data)
        {
            MemoryStream stream = data.ToMemoryStreamEx();
            _image = System.Drawing.Image.FromStream(stream);
            _data  = data;
        }
        public void Save(String filename, ImageTyp typ)
        {
            if (_image != null)
            {
                switch (typ)
                {
                    case ImageTyp.Jpg: _image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                    case ImageTyp.Bmp: _image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp); break;
                    case ImageTyp.Gif: _image.Save(filename, System.Drawing.Imaging.ImageFormat.Gif); break;
                    case ImageTyp.Tif: _image.Save(filename, System.Drawing.Imaging.ImageFormat.Tiff); break;

                    default:
                    case ImageTyp.Png: _image.Save(filename, System.Drawing.Imaging.ImageFormat.Png); break;
                }
            }
        }
        #endregion Методы
    }
    #endregion Класс ImageBase

    /*
    #region Класс NotifyIcon
    [ContentProperty("Text"), DefaultEvent("MouseDoubleClick"),
    UIPermission(SecurityAction.InheritanceDemand, Window = UIPermissionWindow.AllWindows)]
    public sealed class NotifyIcon : FrameworkElement, IDisposable, IAddChild
    {
        #region Поля
        private static readonly int TaskbarCreatedWindowMessage;
        private static readonly UIPermission _allWindowsPermission;
        private static int _nextId;
        private readonly Object _syncObj;
        private NotifyIconHwndSource _hwndSource;
        private int _id;
        private Boolean _iconCreated;
        private Boolean _doubleClick;
        #endregion Поля

        #region Свойства
        public NotifyBalloonIcon BalloonTipIcon
        {
            get { return (NotifyBalloonIcon)GetValue(BalloonTipIconProperty); }
            set { SetValue(BalloonTipIconProperty, value); }
        }
        public String BalloonTipText
        {
            get { return (String)GetValue(BalloonTipTextProperty); }
            set { SetValue(BalloonTipTextProperty, value); }
        }
        public String BalloonTipTitle
        {
            get { return (String)GetValue(BalloonTipTitleProperty); }
            set { SetValue(BalloonTipTitleProperty, value); }
        }
        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        #endregion Свойства

        #region Свойства зависимости
        public static readonly DependencyProperty BalloonTipTitleProperty;
        public static readonly DependencyProperty BalloonTipTextProperty;
        public static readonly DependencyProperty BalloonTipIconProperty;
        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty IconProperty;
        #endregion Свойства зависимости

        #region События
        public event RoutedEventHandler BalloonTipClick
        {
            add { AddHandler(BalloonTipClickEvent, value); }
            remove { RemoveHandler(BalloonTipClickEvent, value); }
        }
        public event RoutedEventHandler BalloonTipClosed
        {
            add { AddHandler(BalloonTipClosedEvent, value); }
            remove { RemoveHandler(BalloonTipClosedEvent, value); }
        }
        public event RoutedEventHandler BalloonTipShown
        {
            add { AddHandler(BalloonTipShownEvent, value); }
            remove { RemoveHandler(BalloonTipShownEvent, value); }
        }
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        public event RoutedEventHandler DoubleClick
        {
            add { AddHandler(DoubleClickEvent, value); }
            remove { RemoveHandler(DoubleClickEvent, value); }
        }
        public event MouseButtonEventHandler MouseClick
        {
            add { AddHandler(MouseClickEvent, value); }
            remove { RemoveHandler(MouseClickEvent, value); }
        }
        public event MouseButtonEventHandler MouseDoubleClick
        {
            add { AddHandler(MouseDoubleClickEvent, value); }
            remove { RemoveHandler(MouseDoubleClickEvent, value); }
        }
        #endregion События

        #region Маршрутизируемые события
        public static readonly RoutedEvent BalloonTipClickEvent;
        public static readonly RoutedEvent BalloonTipClosedEvent;
        public static readonly RoutedEvent BalloonTipShownEvent;
        public static readonly RoutedEvent ClickEvent;
        public static readonly RoutedEvent DoubleClickEvent;
        public static readonly RoutedEvent MouseClickEvent;
        public static readonly RoutedEvent MouseDoubleClickEvent;
        #endregion Маршрутизируемые события

        #region Конструктор
        [SecurityCritical]
        static NotifyIcon()
        {
            //_allWindowsPermission = new UIPermission(UIPermissionWindow.AllWindows);

            //BalloonTipTitleProperty = DependencyProperty.Register("BalloonTipTitle", typeof(String), typeof(NotifyIcon), new FrameworkPropertyMetadata());
            //BalloonTipTextProperty = DependencyProperty.Register("BalloonTipText", typeof(String), typeof(NotifyIcon), new FrameworkPropertyMetadata());
            //BalloonTipIconProperty = DependencyProperty.Register("BalloonTipIcon", typeof(NotifyBalloonIcon), typeof(NotifyIcon), new FrameworkPropertyMetadata(NotifyBalloonIcon.None));
            //TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(NotifyIcon), new FrameworkPropertyMetadata(OnTextPropertyChanged, OnCoerceTextProperty), ValidateTextPropety);
            //IconProperty = Window.IconProperty.AddOwner(typeof(NotifyIcon), new FrameworkPropertyMetadata(OnNotifyIconChanged) { Inherits = true });

            //BalloonTipClickEvent = EventManager.RegisterRoutedEvent("BalloonTipClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));
            //BalloonTipClosedEvent = EventManager.RegisterRoutedEvent("BalloonTipClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));
            //BalloonTipShownEvent = EventManager.RegisterRoutedEvent("BalloonTipShown", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));
            //ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));
            //DoubleClickEvent = EventManager.RegisterRoutedEvent("DoubleClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));
            //MouseClickEvent = EventManager.RegisterRoutedEvent("MouseClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));
            //MouseDoubleClickEvent = EventManager.RegisterRoutedEvent("MouseDoubleClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

            //TaskbarCreatedWindowMessage = NativeMethods.RegisterWindowMessage("TaskbarCreated");

            //VisibilityProperty.OverrideMetadata(typeof(NotifyIcon), new FrameworkPropertyMetadata(OnVisibilityChanged));
        }
        [SecurityCritical]
        public NotifyIcon()
        {
            //_syncObj = new Object();
            //_id = _nextId++;

            //UpdateIconForVisibility();

            //IsVisibleChanged += OnIsVisibleChanged;
        }
        [SecurityCritical]
        ~NotifyIcon()
        {
            Dispose(false);
        }
        [SecurityCritical]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        [SecurityCritical]
        private void Dispose(Boolean isDisposing)
        {
            if (isDisposing)
            {
                if (_hwndSource != null)
                {
                    UpdateIcon(false);
                    _hwndSource.Dispose();
                }
            }
            else if (_hwndSource != null)
            {
            }
        }
        #endregion Конструктор

        #region Методы

        #region Внутренние методы
        [SecurityCritical]
        private static void OnNotifyIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NotifyIcon notifyIcon = (NotifyIcon)o;

            notifyIcon.UpdateIcon(notifyIcon.IsVisible);
        }
        private static Boolean ValidateTextPropety(Object baseValue)
        {
            String value = (String)baseValue;

            return value == null || value.Length <= 0x3f;
        }
        [SecurityCritical]
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NotifyIcon notifyIcon = (NotifyIcon)d;

            if (notifyIcon._iconCreated)
            {
                notifyIcon.UpdateIcon(true);
            }
        }
        private static Object OnCoerceTextProperty(DependencyObject d, Object baseValue)
        {
            String value = (String)baseValue;

            if (value == null)
            {
                value = String.Empty;
            }

            return value;
        }
        [SecurityCritical]
        private void ShowContextMenu()
        {
            if (ContextMenu != null)
            {
                NativeMethods.SetForegroundWindow(new HandleRef(_hwndSource, _hwndSource.Handle));

                ContextMenuService.SetPlacement(ContextMenu, PlacementMode.MousePoint);
                ContextMenu.IsOpen = true;
            }
        }
        [SecurityCritical]
        private void UpdateIconForVisibility()
        {
            UpdateIcon((IsVisible && Visibility == Visibility.Visible) || Visibility == Visibility.Hidden);
        }
        [SecurityCritical]
        private void UpdateIcon(Boolean showIconInTray)
        {
            lock (_syncObj)
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    IntPtr iconHandle = IntPtr.Zero;

                    try
                    {
                        _allWindowsPermission.Demand();

                        if (showIconInTray && _hwndSource == null)
                        {
                            _hwndSource = new NotifyIconHwndSource(this);
                        }

                        if (_hwndSource != null)
                        {
                            _hwndSource.LockReference(showIconInTray);

                            NativeMethods.NOTIFYICONDATA pnid = new NativeMethods.NOTIFYICONDATA
                            {
                                uCallbackMessage = (int)NativeMethods.WindowMessage.TrayMouseMessage,
                                uFlags = NativeMethods.NotifyIconFlags.Message | NativeMethods.NotifyIconFlags.ToolTip,
                                hWnd = _hwndSource.Handle,
                                uID = _id,
                                szTip = Text
                            };
                            if (Icon != null)
                            {
                                iconHandle = Gui.GetIconHandle(Icon);

                                pnid.uFlags |= NativeMethods.NotifyIconFlags.Icon;
                                pnid.hIcon = iconHandle;
                            }

                            if (showIconInTray && iconHandle != null)
                            {
                                if (_iconCreated == false)
                                {
                                    NativeMethods.Shell_NotifyIcon(0, pnid);
                                    _iconCreated = true;
                                }
                                else
                                {
                                    NativeMethods.Shell_NotifyIcon(1, pnid);
                                }
                            }
                            else if (_iconCreated)
                            {
                                NativeMethods.Shell_NotifyIcon(2, pnid);
                                _iconCreated = false;
                            }
                        }
                    }
                    finally
                    {
                        if (iconHandle != IntPtr.Zero)
                        {
                            NativeMethods.DestroyIcon(iconHandle);
                        }
                    }
                }
            }
        }
        [SecurityCritical]
        private static void OnVisibilityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((NotifyIcon)o).UpdateIconForVisibility();
        }
        [SecurityCritical]
        private void OnIsVisibleChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateIconForVisibility();
        }
        #endregion Внутренние методы

        #region Внешние методы
        [SecurityCritical]
        public void ShowBalloonTip(int timeout)
        {
            ShowBalloonTip(timeout, BalloonTipTitle, BalloonTipText, BalloonTipIcon);
        }
        [SecurityCritical]
        public void ShowBalloonTip(int timeout, String tipTitle, String tipText, NotifyBalloonIcon tipIcon)
        {
            if (timeout < 0) timeout = 1000;

            if (_iconCreated)
            {
                _allWindowsPermission.Demand();

                NativeMethods.NOTIFYICONDATA pnid = new NativeMethods.NOTIFYICONDATA
                {
                    hWnd = _hwndSource.Handle,
                    uID = _id,
                    uFlags = NativeMethods.NotifyIconFlags.Balloon,
                    uTimeoutOrVersion = timeout,
                    szInfoTitle = tipTitle,
                    szInfo = tipText,
                    dwInfoFlags = (int)tipIcon
                };
                NativeMethods.Shell_NotifyIcon(1, pnid);
            }
        }
        #endregion Внешние методы

        #region Обработка WndProc
        private void WmMouseDown(MouseButton button, int clicks)
        {
            MouseButtonEventArgs args = null;

            if (clicks == 2)
            {
                RaiseEvent(new RoutedEventArgs(DoubleClickEvent));

                args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button);
                args.RoutedEvent = MouseDoubleClickEvent;
                RaiseEvent(args);

                _doubleClick = true;
            }

            args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button);
            args.RoutedEvent = MouseDownEvent;
            RaiseEvent(args);
        }
        private void WmMouseMove()
        {
            MouseEventArgs args = new MouseEventArgs(InputManager.Current.PrimaryMouseDevice, 0);
            args.RoutedEvent = MouseMoveEvent;
            RaiseEvent(args);
        }
        private void WmMouseUp(MouseButton button)
        {
            MouseButtonEventArgs args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button);
            args.RoutedEvent = MouseUpEvent;
            RaiseEvent(args);

            if (_doubleClick == false)
            {
                RaiseEvent(new RoutedEventArgs(ClickEvent));

                args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button);
                args.RoutedEvent = MouseClickEvent;
                RaiseEvent(args);
            }

            _doubleClick = false;
        }
        [SecurityCritical]
        private void WmTaskbarCreated()
        {
            _iconCreated = false;
            UpdateIcon(IsVisible);
        }
        [SecurityCritical]
        private void WndProc(int message, IntPtr wParam, IntPtr lParam, out Boolean handled)
        {
            handled = true;

            if (message <= (int)NativeMethods.WindowMessage.MeasureItem)
            {
                if (message == (int)NativeMethods.WindowMessage.Destroy)
                {
                    UpdateIcon(false);
                    return;
                }
            }
            else
            {
                if (message != (int)NativeMethods.WindowMessage.TrayMouseMessage)
                {
                    if (message == TaskbarCreatedWindowMessage)
                    {
                        WmTaskbarCreated();
                    }
                    handled = false;
                    return;
                }
                switch ((NativeMethods.WindowMessage)lParam)
                {
                    case NativeMethods.WindowMessage.MouseMove:
                        WmMouseMove();
                        return;
                    case NativeMethods.WindowMessage.MouseDown:
                        WmMouseDown(MouseButton.Left, 1);
                        return;
                    case NativeMethods.WindowMessage.LButtonUp:
                        WmMouseUp(MouseButton.Left);
                        return;
                    case NativeMethods.WindowMessage.LButtonDblClk:
                        WmMouseDown(MouseButton.Left, 2);
                        return;
                    case NativeMethods.WindowMessage.RButtonDown:
                        WmMouseDown(MouseButton.Right, 1);
                        return;
                    case NativeMethods.WindowMessage.RButtonUp:
                        ShowContextMenu();
                        WmMouseUp(MouseButton.Right);
                        return;
                    case NativeMethods.WindowMessage.RButtonDblClk:
                        WmMouseDown(MouseButton.Right, 2);
                        return;
                    case NativeMethods.WindowMessage.MButtonDown:
                        WmMouseDown(MouseButton.Middle, 1);
                        return;
                    case NativeMethods.WindowMessage.MButtonUp:
                        WmMouseUp(MouseButton.Middle);
                        return;
                    case NativeMethods.WindowMessage.MButtonDblClk:
                        WmMouseDown(MouseButton.Middle, 2);
                        return;
                }
                switch ((NativeMethods.NotifyIconMessage)lParam)
                {
                    case NativeMethods.NotifyIconMessage.BalloonShow:
                        RaiseEvent(new RoutedEventArgs(BalloonTipShownEvent));
                        return;
                    case NativeMethods.NotifyIconMessage.BalloonHide:
                    case NativeMethods.NotifyIconMessage.BalloonTimeout:
                        RaiseEvent(new RoutedEventArgs(BalloonTipClosedEvent));
                        return;
                    case NativeMethods.NotifyIconMessage.BalloonUserClick:
                        RaiseEvent(new RoutedEventArgs(BalloonTipClickEvent));
                        return;
                }
                return;
            }
            if (message == TaskbarCreatedWindowMessage)
            {
                WmTaskbarCreated();
            }
            handled = false;
        }
        [SecurityCritical]
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref Boolean handled)
        {
            WndProc(msg, wParam, lParam, out handled);

            return IntPtr.Zero;
        }
        #endregion Обработка WndProc

        #region Реализация IChild
        void IAddChild.AddChild(Object value)
        {
            throw new InvalidOperationException();
        }
        void IAddChild.AddText(String text)
        {
            Text = text;
        }
        #endregion Реализация IChild

        #endregion Методы

        #region Класс NotifyIconNativeWindow
        private class NotifyIconHwndSource : HwndSource
        {
            #region Поля
            private NotifyIcon _reference;
            private GCHandle _rootRef;
            #endregion Поля

            #region Конструктор
            [SecurityCritical]
            internal NotifyIconHwndSource(NotifyIcon component): base(0, 0, 0, 0, 0, null, IntPtr.Zero)
            {
                _reference = component;

                AddHook(_reference.WndProc);
            }
            [SecurityCritical]
            ~NotifyIconHwndSource()
            {
                if (Handle != IntPtr.Zero)
                {
                }
            }
            #endregion Конструктор

            #region Методы
            public void LockReference(Boolean locked)
            {
                if (locked)
                {
                    if (_rootRef.IsAllocated == false)
                    {
                        _rootRef = GCHandle.Alloc(_reference, GCHandleType.Normal);
                    }
                }
                else if (_rootRef.IsAllocated)
                {
                    _rootRef.Free();
                }
            }
            #endregion Методы
        }
        #endregion Класс NotifyIconNativeWindow
    }
    #endregion Класс NotifyIcon
    */
}
