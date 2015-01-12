using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Defines a build-in ContextMenuStrip manager for HexBox control to show Copy, Cut, Paste menu in contextmenu of the
    /// control.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    internal sealed class HexBoxContextMenu : Component
    {
        #region Поля

        /// <summary>
        /// Ссылка на элемент HexBox
        /// </summary>
        private readonly HexBox _hexBox;

        /// <summary>
        /// Элемент меню "Отображение адреса в Hex-формате"
        /// </summary>
        private ToolStripMenuItem _addrAsHexToolStripMenuItem;

        /// <summary>
        /// Контекстное меню
        /// </summary>
        private ContextMenuStrip _contextMenuStrip;

        /// <summary>
        /// Элемент меню "Копировать"
        /// </summary>
        private ToolStripMenuItem _copyToolStripMenuItem;

        /// <summary>
        /// Элемент меню "Вырезать"
        /// </summary>
        private ToolStripMenuItem _cutToolStripMenuItem;

        /// <summary>
        /// Элемент меню "Перейти"
        /// </summary>
        private ToolStripMenuItem _gotoToolStripMenuItem;

        /// <summary>
        /// Элемент меню "Поиск"
        /// </summary>
        private ToolStripMenuItem _findToolStripMenuItem;

        /// <summary>
        /// Элемент меню "Вставить"
        /// </summary>
        private ToolStripMenuItem _pasteToolStripMenuItem;

        /// <summary>
        /// Элемент меню "Выделить все"
        /// </summary>
        private ToolStripMenuItem _selectAllToolStripMenuItem;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        internal HexBoxContextMenu(HexBox hexBox)
        {
            _hexBox = hexBox;
            _hexBox.ByteProviderChanged += HexBox_ByteProviderChanged;
        }

        #endregion

        #region Методы

        /// <summary>
        /// If DataSource
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        private void HexBox_ByteProviderChanged(object sender, EventArgs e)
        {
            CheckBuiltInContextMenu();
        }

        /// <summary>
        /// Построение контекстного меню
        /// </summary>
        private void CheckBuiltInContextMenu()
        {
            if (DesignHelper.IsDesigntime)
            {
                return;
            }

            if (_contextMenuStrip == null)
            {
                var menu = new ContextMenuStrip();
                _cutToolStripMenuItem = new ToolStripMenuItem("Вырезать", null, CutMenuItemClick);
                _copyToolStripMenuItem = new ToolStripMenuItem("Копировать", null, CopyMenuItemClick);
                _pasteToolStripMenuItem = new ToolStripMenuItem("Вставить", null, PasteMenuItemClick);
                _selectAllToolStripMenuItem = new ToolStripMenuItem("Выделить все", null, SelectAllMenuItemClick);
                _gotoToolStripMenuItem = new ToolStripMenuItem("Перейти ...", null, GotoMenuItemClick);
                _findToolStripMenuItem = new ToolStripMenuItem("Поиск ...", null, FindMenuItemClick);
                _addrAsHexToolStripMenuItem = new ToolStripMenuItem("Адрес в Hex-формате", null, ShowAddrAsHex);
                _addrAsHexToolStripMenuItem.CheckOnClick = true;
                _addrAsHexToolStripMenuItem.Checked = _hexBox.ShowAddrAsHex;

                menu.Items.Add(_gotoToolStripMenuItem);
                menu.Items.Add(_findToolStripMenuItem);
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(_copyToolStripMenuItem);
                menu.Items.Add(_cutToolStripMenuItem);
                menu.Items.Add(_pasteToolStripMenuItem);
                menu.Items.Add(_selectAllToolStripMenuItem);
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(_addrAsHexToolStripMenuItem);

                menu.Opening += BuildInContextMenuStrip_Opening;

                _contextMenuStrip = menu;
            }

            if (_hexBox.DataSource == null && _hexBox.ContextMenuStrip == _contextMenuStrip)
            {
                _hexBox.ContextMenuStrip = null;
            }
            else if (_hexBox.DataSource != null && _hexBox.ContextMenuStrip == null)
            {
                _hexBox.ContextMenuStrip = _contextMenuStrip;
            }
        }

        /// <summary>
        /// Перед открытием меню установка Enable/Disable пунктов меню
        /// </summary>
        private void BuildInContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            _cutToolStripMenuItem.Enabled = _hexBox.CanCut();
            _copyToolStripMenuItem.Enabled = _hexBox.CanCopy();
            _pasteToolStripMenuItem.Enabled = _hexBox.CanPaste();
            _selectAllToolStripMenuItem.Enabled = _hexBox.CanSelectAll();
        }

        /// <summary>
        /// Обработчик события "Вырезать"
        /// </summary>
        private void CutMenuItemClick(object sender, EventArgs e)
        {
            _hexBox.Cut();
        }

        /// <summary>
        /// Обработчик события "Копировать"
        /// </summary>
        private void CopyMenuItemClick(object sender, EventArgs e)
        {
            _hexBox.Copy();
        }

        /// <summary>
        /// Обработчик события "Вставить"
        /// </summary>
        private void PasteMenuItemClick(object sender, EventArgs e)
        {
            _hexBox.Paste();
        }

        /// <summary>
        /// Обработчик события "Выделить все"
        /// </summary>
        private void SelectAllMenuItemClick(object sender, EventArgs e)
        {
            _hexBox.SelectAll();
        }

        /// <summary>
        /// Обработчик события "Режим отображения адреса"
        /// </summary>
        private void ShowAddrAsHex(object sender, EventArgs eventArgs)
        {
            _hexBox.ShowAddrAsHex = _addrAsHexToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Обработка события "Перейти"
        /// </summary>
        private void GotoMenuItemClick(object sender, EventArgs eventArgs)
        {
            _hexBox.Goto();
        }

        /// <summary>
        /// Обработка события "Поиск"
        /// </summary>
        private void FindMenuItemClick(object sender, EventArgs eventArgs)
        {
            _hexBox.Find();
        }

        #endregion
    }
}