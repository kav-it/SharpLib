// ****************************************************************************
//
// Имя файла    : 'Logger.cs'
// Заголовок    : Реализация записи лог-файлов
// Автор        : Тихомиров В.С./Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.Windows;
using System.Windows.Input;

namespace SharpLib
{
    #region Делегат DragDroperCallback
    public delegate Object DragDroperCallback(DragDroperOperation oper, Object data);
    #endregion Делегат DragDroperCallback

    #region Делегат RoutedCommandCallback
    public delegate Boolean RoutedCommandCallback(RoutedCommandBase command, ExecutedRoutedEventArgs args);
    public delegate Boolean RoutedCommandCanCallback(RoutedCommandBase command, CanExecuteRoutedEventArgs args);
    #endregion Делегат RoutedCommandCallback

    #region Перечисление DragDroperOperation
    public enum DragDroperOperation
    {
        Unknow  = 0,
        Start   = 1,
        Move    = 2,
        Stop    = 3,
        EndMove = 4,
        EndCopy = 5
    }
    #endregion Перечисление DragDroperOperation

    #region Класс Keyboarder
    public static class Keyboarder
    {
        #region Константы
        private const UInt16 VK_INSERT   = 0x2D;
        private const UInt16 VK_CAPSLOCK = 0x14;
        #endregion Константы

        #region Поля
        private static Byte[] _keybordKeys;
        #endregion Поля

        #region Свойства
        public static Boolean IsControl
        {
            get 
            {
                Boolean result = Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl);

                return result;
            }
        }
        public static Boolean IsShift
        {
            get
            {
                Boolean result = Keyboard.IsKeyDown(Key.LeftShift) | Keyboard.IsKeyDown(Key.RightShift);

                return result;
            }
        }
        public static Boolean IsNumLock
        {
            get { return Console.NumberLock; }
        }
        public static Boolean IsCapsLock
        {
            get { return Console.CapsLock; }
        }
        public static Boolean IsInsert
        {
            get { return GetKeyState(VK_INSERT); }
        }        
        #endregion Свойства

        #region Конструктор
        static Keyboarder()
        {
            _keybordKeys = new Byte[255];
        }
        #endregion Конструктор

        #region Методы
        private static Boolean GetKeyState(UInt16 keyCode)
        {
            NativeMethods.GetKeyboardState(_keybordKeys);

            Boolean result = (_keybordKeys[keyCode] & 1) == 1 ? true : false;

            return result;
        }
        #endregion Методы
    }
    #endregion Класс Keyboarder

    #region Класс DragDroper
    public class DragDroper
    {
        #region Константы
        private const String DRAG_DROP_FORMAT = "DragDroperFormat";
        #endregion Константы

        #region Конструктор
        
        #endregion Конструктор

        #region Поля
        private UIElement          _element;
        private DragDroperCallback _callback;
        private Point              _startPoint;
        private Boolean            _isDraging;
        #endregion Поля

        #region Конструктор
        public DragDroper()
        {
        }
        #endregion Конструктор

        #region Методы
        public void Init(UIElement element, DragDroperCallback callback)
        {
            _element  = element;
            _callback = callback;

            _element.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Element_PreviewMouseLeftButtonDown);
            _element.PreviewMouseMove += new MouseEventHandler(Element_PreviewMouseMove);
            _element.DragOver += new DragEventHandler(Element_DragOver);
            _element.AllowDrop = true;
            _isDraging = false;
        }
        private Object RaiseCallback(DragDroperOperation oper, Object data)
        {
            if (_callback != null)
            {
                Object result = _callback(oper, data);

                return result;
            }

            return null;
        }
        private void Element_DragOver(Object sender, DragEventArgs e)
        {
            Object data = e.Data.GetData(DRAG_DROP_FORMAT);

            RaiseCallback(DragDroperOperation.Move, data);
        }
        private void Element_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }
        private void Element_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraging == false && e.LeftButton == MouseButtonState.Pressed)
            {
                Point  position = e.GetPosition(null);
                Double deltaX   = Math.Abs(position.X - _startPoint.X);
                Double deltaY   = Math.Abs(position.Y - _startPoint.Y);

                if (deltaX > SystemParameters.MinimumHorizontalDragDistance || deltaY > SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag(e);
                }
            }
        }
        private void StartDrag(MouseEventArgs e)
        {
            _isDraging = true;

            if (_callback != null)
            {
                Object temp = RaiseCallback(DragDroperOperation.Start, null);

                if (temp != null)
                {
                    DataObject data = new DataObject(DRAG_DROP_FORMAT, temp);

                    if (data != null)
                    {
                        DragDropEffects effects = DragDropEffects.Move;
                        if (e.RightButton == MouseButtonState.Pressed)
                            effects  = DragDropEffects.All;

                        DragDropEffects de = DragDrop.DoDragDrop(_element, data, effects);

                        // Передача информации об окончании операции
                        RaiseCallback(DragDroperOperation.Stop, temp);
                    }
                }
            }

            _isDraging = false;
        }
        #endregion Методы
    }
    #endregion Класс DragDroper

    #region Класс Mouser
    public static class Mouser
    {
        #region Методы
        public static Point GetCursorPos()
        {
            NativeMethods.POINT winPoint;
            NativeMethods.GetCursorPos(out winPoint);

            Point point = new Point(winPoint.X, winPoint.Y);

            return point;
        }
        public static Point GetCursorPos(UIElement element)
        {
            Point point = GetCursorPos();
            point = element.PointFromScreen(point);

            return point;
        }
        #endregion Методы
    }
    #endregion Класс Mouser

    #region Класс RoutedCommandBase
    public class RoutedCommandBase : RoutedUICommand
    {
        #region Константы
        private const String TEXT_CTRL    = "CTRL";
        private const String TEXT_CONTROL = "CONTROL";
        private const String TEXT_ALT     = "ATL";
        private const String TEXT_SHIFT   = "SHIFT";
        private const String TEXT_WIN     = "WIN";
        #endregion Константы

        #region Поля
        private Object                _tag;
        private RoutedCommandCallback _callbackExecute;
        private RoutedCommandCanCallback _callbackCanExecute;
        #endregion Поля

        #region Свойства
        public Object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        #endregion Свойства

        #region Конструктор
        public RoutedCommandBase(String textKeys, RoutedCommandCallback execute, Object tag): this(textKeys, execute, tag, null)
        {
        }
        public RoutedCommandBase(String textKeys, RoutedCommandCallback execute, Object tag, RoutedCommandCanCallback canExecute)
        {
            _callbackExecute    = execute;
            _callbackCanExecute = canExecute;
            _tag                = tag;

            Key          key;
            ModifierKeys modifier;

            if (ParseString(textKeys, out key, out modifier) == true)
            {
                AddGesture(key, modifier);
            }
        }
        #endregion Конструктор

        #region Методы
        /// <summary>
        /// Инициализация Binding на MainWindow 
        /// Подразумевается, что используется для WPF приложений в которых есть MainWindow
        /// Обязательно вызывается ПОСЛЕ создания MainWindow
        /// </summary>
        public void InitBinding()
        {
            CommandBinding bind = new CommandBinding(this, CommandExecuted, CanCommandExecuted);
            Application.Current.MainWindow.CommandBindings.Add(bind);
        }
        private void CommandExecuted(Object sender, ExecutedRoutedEventArgs e)
        {
            if (_callbackExecute != null)
                _callbackExecute(this, e);
        }
        private void CanCommandExecuted(Object sender, CanExecuteRoutedEventArgs e)
        {
            Boolean result = true;

            if (_callbackCanExecute != null)
                result = _callbackCanExecute(this, e);
            
            e.CanExecute = result;
        }
        private void AddGesture(Key key, ModifierKeys modifier)
        {
            KeyGesture keyGesture = new KeyGesture(key, modifier);

            this.InputGestures.Add(keyGesture);
        }
        private Boolean ParseString(String text, out Key key, out ModifierKeys modifier)
        {
            key      = Key.None;
            modifier = ModifierKeys.None;

            text = text.ToUpper();
            text = text.Replace(" ", "");

            String[] arr = text.SplitEx("+");

            if (arr.Length == 0) return false;

            foreach (String chunk in arr)
            {
                if (chunk.Contains(TEXT_ALT))          modifier |= ModifierKeys.Alt;
                else if (chunk.Contains(TEXT_WIN))     modifier |= ModifierKeys.Windows;
                else if (chunk.Contains(TEXT_CTRL))    modifier |= ModifierKeys.Control;
                else if (chunk.Contains(TEXT_CONTROL)) modifier |= ModifierKeys.Control;
                else if (chunk.Contains(TEXT_SHIFT))   modifier |= ModifierKeys.Shift;
                else
                {
                    key = (Key)Enum.Parse(typeof(Key), chunk, true);
                }
            }

            return true;
        }
        public override string ToString()
        {
            String text = "";

            if (this.InputGestures.Count > 0)
            {
                KeyGesture   gesture  = (KeyGesture)this.InputGestures[0];
                Key          key      = gesture.Key;
                ModifierKeys modifier = gesture.Modifiers;

                if (modifier != ModifierKeys.None)
                    text += modifier.ToString();

                if (key != Key.None)
                {
                    if (text != "") text += "+";
                    text += key.ToString();
                }
            }

            return text;
        }
        #endregion Методы
    }
    #endregion Класс RoutedCommandBase

}
