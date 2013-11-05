//*****************************************************************************
//
// Имя файла    : 'File.cs'
// Заголовок    : Компонент просмотра Xml-файлов (на базе http://www.codeproject.com/Articles/71069/A-Simple-WPF-XML-Document-Viewer-Control)
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 18/12/2012
//
//*****************************************************************************
			
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace SharpLib
{
    #region Класс XmlViewerControl
    public partial class XmlViewerControl : UserControl
    {
        #region Поля
        private XmlDocument _xmldocument;
        #endregion Поля

        #region Свойства
        public XmlDocument XmlDoc
        {
            get { return _xmldocument; }
            set
            {
                _xmldocument = value;
                BindXmlDocument();
            }
        }
        #endregion Свойства

        #region Конструктор
        public XmlViewerControl()
        {
            InitializeComponent();
        }
        #endregion Конструктор

        #region Методы
        private void BindXmlDocument()
        {
            if (_xmldocument == null)
            {
                PART_treeViewXml.ItemsSource = null;
                return;
            }

            XmlDataProvider provider = new XmlDataProvider();
            provider.Document = _xmldocument;
            Binding binding = new Binding();
            binding.Source = provider;
            binding.XPath = "child::node()";

            PART_treeViewXml.SetBinding(TreeView.ItemsSourceProperty, binding);
        }
        #endregion Методы
    }
    #endregion Класс XmlViewerControl
}
