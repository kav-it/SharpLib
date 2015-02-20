using System;
using System.Windows;
using System.Windows.Controls;

using SharpLib.Docking.Layout;

namespace DemoWpf
{
    public partial class TabDocking
    {
        #region Конструктор

        /// <summary>
        /// Левая панель
        /// </summary>
        internal LayoutAnchorablePane LeftPanel { get; private set; }

        /// <summary>
        /// Правая панель
        /// </summary>
        private LayoutAnchorablePane _rightPanel;

        /// <summary>
        /// Центральная панель
        /// </summary>
        internal LayoutDocumentPane CenterPanel { get; private set; }

        /// <summary>
        /// Нижняя панель
        /// </summary>
        private LayoutAnchorGroup _bottomPanel;

        public TabDocking()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;

        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            CreateLayout();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Создание Layout
        /// </summary>
        private void CreateLayout()
        {
            // Горизонтальная панель <Левый контент> + <Центр> + <Правый контент>
            var horizontalPanel = new LayoutPanel { Orientation = Orientation.Horizontal };

            LeftPanel = new LayoutAnchorablePane { DockWidth = new GridLength(300) };
            CenterPanel = new LayoutDocumentPane();
            _rightPanel = new LayoutAnchorablePane { DockWidth = new GridLength(300) };
            _bottomPanel = new LayoutAnchorGroup();

            horizontalPanel.Children.Add(LeftPanel);
            horizontalPanel.Children.Add(CenterPanel);
            horizontalPanel.Children.Add(_rightPanel);

            PART_rootLayout.RootPanel = horizontalPanel;
            PART_rootLayout.BottomSide.Children.Add(_bottomPanel);
        }

        private void AddCenterClick(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        private void AddLeftClick(object sender, RoutedEventArgs e)
        {
            var container = new LayoutAnchorable
            {
                Content = new Button {Content = "Left 1"},
                Title = "Title",
                ContentId = Guid.NewGuid().ToString()
            };

            LeftPanel.Children.Add(container);
        }
    }
}