using System.Reflection;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("Расширение для Wpf")]

[assembly: XmlnsPrefix("http://codeofrussia.ru/sharplib/wpf/controls", "controls")]
[assembly: XmlnsDefinition("http://codeofrussia.ru/sharplib/wpf/controls", "SharpLib.Wpf.Controls")]

[assembly: XmlnsPrefix("http://codeofrussia.ru/sharplib/wpf/dragging", "dragging")]
[assembly: XmlnsDefinition("http://codeofrussia.ru/sharplib/wpf/dragging", "SharpLib.Wpf.Dragging")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly )]
