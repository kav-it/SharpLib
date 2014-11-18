using System.Reflection;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("Расширение для Wpf")]

[assembly: XmlnsPrefix("http://codeofrussia.ru/sharplib/wpf", "sharplib")]
[assembly: XmlnsDefinition("http://codeofrussia.ru/sharplib/wpf", "SharpLibWpf")]

[assembly: XmlnsPrefix("http://codeofrussia.ru/sharplib/wpf/controls", "controls")]
[assembly: XmlnsDefinition("http://codeofrussia.ru/sharplib/wpf/controls", "SharpLibWpf.Controls")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly )]
