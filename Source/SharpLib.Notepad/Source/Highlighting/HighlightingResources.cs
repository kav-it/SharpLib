using System.IO;
using System.Reflection;

namespace SharpLib.Notepad.Highlighting
{
    internal static class HighlightingResources
    {
        #region Поля

        private const string PREFIX = "Source/Assets/xshd/";

        #endregion

        #region Методы

        public static Stream OpenStream(string name)
        {
            var stream = Assembly.GetExecutingAssembly().GetEmbeddedResourceAsStreamEx(PREFIX + name);
            if (stream == null)
            {
                throw new FileNotFoundException("The resource file '" + name + "' was not found.");
            }

            return stream;
        }

        internal static void RegisterBuiltInHighlightings(HighlightingManager.DefaultHighlightingManager hlm)
        {
            hlm.RegisterHighlighting("XmlDoc", null, "XmlDoc.xshd");
            hlm.RegisterHighlighting("C#", new[] { ".cs" }, "CSharp-Mode.xshd");

            hlm.RegisterHighlighting("JavaScript", new[] { ".js" }, "JavaScript-Mode.xshd");
            hlm.RegisterHighlighting("HTML", new[] { ".htm", ".html" }, "HTML-Mode.xshd");
            hlm.RegisterHighlighting("ASP/XHTML", new[] { ".asp", ".aspx", ".asax", ".asmx", ".ascx", ".master" }, "ASPX.xshd");

            hlm.RegisterHighlighting("Boo", new[] { ".boo" }, "Boo.xshd");
            hlm.RegisterHighlighting("Coco", new[] { ".atg" }, "Coco-Mode.xshd");
            hlm.RegisterHighlighting("CSS", new[] { ".css" }, "CSS-Mode.xshd");
            hlm.RegisterHighlighting("C++", new[] { ".c", ".h", ".cc", ".cpp", ".hpp" }, "CPP-Mode.xshd");
            hlm.RegisterHighlighting("Java", new[] { ".java" }, "Java-Mode.xshd");
            hlm.RegisterHighlighting("Patch", new[] { ".patch", ".diff" }, "Patch-Mode.xshd");
            hlm.RegisterHighlighting("PowerShell", new[] { ".ps1", ".psm1", ".psd1" }, "PowerShell.xshd");
            hlm.RegisterHighlighting("PHP", new[] { ".php" }, "PHP-Mode.xshd");
            hlm.RegisterHighlighting("TeX", new[] { ".tex" }, "Tex-Mode.xshd");
            hlm.RegisterHighlighting("VB", new[] { ".vb" }, "VB-Mode.xshd");
            hlm.RegisterHighlighting("XML", (".xml;.xsl;.xslt;.xsd;.manifest;.config;.addin;" +
                                             ".xshd;.wxs;.wxi;.wxl;.proj;.csproj;.vbproj;.ilproj;" +
                                             ".booproj;.build;.xfrm;.targets;.xaml;.xpt;" +
                                             ".xft;.map;.wsdl;.disco;.ps1xml;.nuspec").Split(';'),
                "XML-Mode.xshd");
            hlm.RegisterHighlighting("MarkDown", new[] { ".md" }, "MarkDown-Mode.xshd");
        }

        #endregion
    }
}