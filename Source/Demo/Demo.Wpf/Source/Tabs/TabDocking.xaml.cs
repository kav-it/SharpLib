using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using SharpLib;
using SharpLib.Docking;

namespace DemoWpf
{
    public partial class TabDocking
    {
        #region Поля

        /// <summary>
        /// Нижняя панель
        /// </summary>
        private LayoutAnchorGroup _bottomPanel;

        /// <summary>
        /// Центральная панель
        /// </summary>
        internal LayoutDocumentPane _centerPanel;

        /// <summary>
        /// Левая панель
        /// </summary>
        internal LayoutAnchorablePane _leftPanel;

        /// <summary>
        /// Правая панель
        /// </summary>
        private LayoutAnchorablePane _rightPanel;

        #endregion

        #region Конструктор

        public TabDocking()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            CreateLayout();
        }

        #endregion

        #region Методы

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
        }

        /// <summary>
        /// Создание Layout
        /// </summary>
        private void CreateLayout()
        {
            // Горизонтальная панель <Левый контент> + <Центр> + <Правый контент>
            var horizontalPanel = new LayoutPanel
            {
                Orientation = Orientation.Horizontal,
                InternalDesc = "Horizontal panel (root)"
            };

            _leftPanel = new LayoutAnchorablePane
            {
                DockWidth = new GridLength(300),
                InternalDesc = "Left panel"
            };
            _centerPanel = new LayoutDocumentPane()
            {
                InternalDesc = "Center panel"
            };
            _rightPanel = new LayoutAnchorablePane
            {
                DockWidth = new GridLength(300),
                InternalDesc = "Right panel"
            };
            _bottomPanel = new LayoutAnchorGroup()
            {
                InternalDesc = "Bottom panel"
            };

            horizontalPanel.Children.Add(_leftPanel);
            horizontalPanel.Children.Add(_centerPanel);
            horizontalPanel.Children.Add(_rightPanel);

            PART_rootLayout.RootPanel = horizontalPanel;
            // PART_rootLayout.BottomSide.Children.Add(_bottomPanel);
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            var serializer = new XmlLayoutSerializer(PART_dockingManager);
            using (var stream = new StreamWriter("docking.layout.xml"))
            {
                serializer.Serialize(stream);
            }
        }

        private void CollectClick(object sender, RoutedEventArgs e)
        {
            PART_dockingManager.Layout.CollectGarbage();
        }

        private void ShowHidden(object sender, RoutedEventArgs e)
        {
            var list = PART_dockingManager.Layout.Hidden.ToList();
            list.ForEach(x => x.Show());
        }

        private void AddCenterClick(object sender, RoutedEventArgs e)
        {
            int count = _centerPanel.Children.Count;
            var text = "Center " + (count + 1).ToString(CultureInfo.InvariantCulture);

            var container = new LayoutDocument
            {
                Content = new Button
                {
                    Content = text,
                    Width = 75,
                    Height = 27,
                },
                Title = text,
                ContentId = Guid.NewGuid().ToString(),
                InternalDesc = text
            };

            _centerPanel.Children.Add(container);
        }

        private void AddLeftClick(object sender, RoutedEventArgs e)
        {
            int count = _leftPanel.Children.Count;
            var text = "Left " + (count + 1).ToString(CultureInfo.InvariantCulture);

            var container = new LayoutAnchorable
            {
                Content = new Button
                {
                    Content = text,
                    Width = 75,
                    Height = 27,
                },
                Title = text,
                ContentId = Guid.NewGuid().ToString(),
                CanHide = true,
                CanClose = true,
                InternalDesc = text
            };

            _leftPanel.Children.Add(container);
        }

        private void AddRightClick(object sender, RoutedEventArgs e)
        {
            int count = _rightPanel.Children.Count;
            var text = "Right " + (count + 1).ToString(CultureInfo.InvariantCulture);

            var container = new LayoutAnchorable
            {
                Content = new Button
                {
                    Content = text,
                    Width = 75,
                    Height = 27,
                },
                Title = text,
                ContentId = Guid.NewGuid().ToString(),
                CanHide = true,
                CanClose = true,
                InternalDesc = text
            };

            _rightPanel.Children.Add(container);
        }

        #endregion
    }
}