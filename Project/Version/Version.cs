//*****************************************************************************
//
// Имя файла    : 'Version.cs'
// Заголовок    : Файл версионности проекта
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 06/11/2013
//
//*****************************************************************************

using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyProduct("Библиотека SharpLib")]
[assembly: AssemblyFileVersion("5.0.0.1")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyCopyright("2014")]

[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly
)]

[assembly: XmlnsPrefix("http://codeofrussia.ru/sharplib", "sharplib")]
[assembly: XmlnsPrefix("http://codeofrussia.ru/sharplib/controls", "sharplibControls")]

[assembly: XmlnsDefinition("http://codeofrussia.ru/sharplib", "SharpLib")]
[assembly: XmlnsDefinition("http://codeofrussia.ru/sharplib/controls", "SharpLib.Controls")]

