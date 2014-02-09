// ****************************************************************************
//
// Имя файла    : 'Screenshot.cs'
// Заголовок    : Библиотека снимков экрана
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 19/11/2012
//
// ****************************************************************************

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SharpLib
{

    #region Класс Screenshot

    public class Screenshot
    {
        #region Методы

        public static ImageSource CaptureUserArea()
        {
            ScreenshotWindow window = new ScreenshotWindow();
            if (window.ShowDialog() == true)
            {
                Rect area = window.Area;

                return CaptureRegion((int)area.X, (int)area.Y, (int)area.Width, (int)area.Height);
            }

            return null;
        }

        public static ImageSource CaptureScreen()
        {
            int width = Hardware.MonitorInfo.Width;
            int height = Hardware.MonitorInfo.Height;

            return CaptureRegion(0, 0, width, height);
        }

        public static ImageSource CaptureWindow(Window window)
        {
            int width = (int)window.ActualWidth;
            int height = (int)window.ActualHeight;

            return CaptureRegion(0, 0, width, height);
        }

        public static ImageSource CaptureRegion(int x, int y, int width, int height)
        {
            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height))
            {
                using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
                {
                    System.Drawing.Point sourcePoint = new System.Drawing.Point(x, y);
                    System.Drawing.Point destPoint = new System.Drawing.Point(0, 0);
                    System.Drawing.Size selectSize = new System.Drawing.Size(width, height);

                    graphics.CopyFromScreen(sourcePoint, destPoint, selectSize);
                }

                ImageSource imageSource = Gui.BitmapToImageSource(bitmap);

                return imageSource;
            }
        }

        #endregion
    }

    #endregion Класс Screenshot

    #region Класс ScreenshotWindow

    public class ScreenshotWindow : Window
    {
        #region Константы

        private const Double BORDER_WIDTH = 3;

        private const Double CORNER_RADIUS = 3;

        #endregion

        #region Поля

        private readonly Color BORDER_COLOR = Colors.Lime;

        private Rect _area;

        private Canvas _canvas;

        private Boolean _isMouseDown;

        private Rectangle _rect;

        private Double _scale;

        private Point _startPoint;

        #endregion

        #region Свойства

        public Rect Area
        {
            get { return _area; }
            set { _area = value; }
        }

        #endregion

        #region Конструктор

        public ScreenshotWindow()
        {
            Owner = Program.CurrentWindow;
            ShowInTaskbar = false;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;

            Topmost = true;
            AllowsTransparency = true;
            Background = Brushes.Black;
            Opacity = 0.3;
            ResizeMode = ResizeMode.NoResize;

            Loaded += ScreenshotWindow_Loaded;
            KeyDown += ScreenshotWindow_KeyDown;
            PreviewMouseLeftButtonDown += ScreenshotWindow_PreviewMouseLeftButtonDown;
            PreviewMouseMove += ScreenshotWindow_PreviewMouseMove;

            _startPoint = new Point(0, 0);
            _scale = Hardware.MonitorInfo.Scale;
        }

        #endregion

        #region Методы

        private void ScreenshotWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _isMouseDown = false;

            _rect = new Rectangle();
            _rect.Stroke = new SolidColorBrush(BORDER_COLOR);
            _rect.Fill = new SolidColorBrush(Colors.White);
            _rect.StrokeThickness = BORDER_WIDTH;
            _rect.RadiusX = CORNER_RADIUS;
            _rect.RadiusY = CORNER_RADIUS;
            _rect.Width = 0;
            _rect.Height = 0;

            _canvas = new Canvas();
            _canvas.Children.Add(_rect);
            Content = _canvas;
        }

        private void ScreenshotWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            _startPoint = Gui.GetMousePosition();
        }

        private void ScreenshotWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                Point currPoint = Gui.GetMousePosition();

                // Определение ширины/высоты прямоугольника
                _rect.Width = Math.Abs(currPoint.X - _startPoint.X) / _scale;
                _rect.Height = Math.Abs(currPoint.Y - _startPoint.Y) / _scale;

                // Определение самого левой верхней точки 
                // для учета смещения влево относительно начальной точки)
                Double curr_x = Math.Min(currPoint.X, _startPoint.X);
                Double curr_y = Math.Min(currPoint.Y, _startPoint.Y);

                // Установка прямоугольника
                curr_x = curr_x / _scale;
                curr_y = curr_y / _scale;
                Canvas.SetLeft(_rect, curr_x);
                Canvas.SetTop(_rect, curr_y);

                if (e.LeftButton == MouseButtonState.Released)
                {
                    _isMouseDown = false;

                    _area = new Rect(curr_x * _scale, curr_y * _scale, _rect.Width * _scale, _rect.Height * _scale);

                    DialogResult = true;
                    Close();
                }
            }
        }

        private void ScreenshotWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        #endregion
    }

    #endregion Класс ScreenshotWindow
}