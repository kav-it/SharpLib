using System;
using System.Diagnostics;
using System.Xml;

namespace SharpLib.Texter.Highlighting
{
    internal sealed class DefaultHighlightingManager : HighlightingManager
    {
        #region Конструктор

        public DefaultHighlightingManager()
        {
            RegisterHighlighting(HighlightTyp.XmlDoc, null, "XmlDoc.xshd");
            RegisterHighlighting(HighlightTyp.Sharp, new[] { ".cs" }, "CSharp-Mode.xshd");

            RegisterHighlighting(HighlightTyp.JavaScript, new[] { ".js" }, "JavaScript-Mode.xshd");
            RegisterHighlighting(HighlightTyp.Html, new[] { ".htm", ".html" }, "HTML-Mode.xshd");
            RegisterHighlighting(HighlightTyp.Asp, new[] { ".asp", ".aspx", ".asax", ".asmx", ".ascx", ".master" }, "ASPX.xshd");

            RegisterHighlighting(HighlightTyp.Boo, new[] { ".boo" }, "Boo.xshd");
            RegisterHighlighting(HighlightTyp.Coco, new[] { ".atg" }, "Coco-Mode.xshd");
            RegisterHighlighting(HighlightTyp.Css, new[] { ".css" }, "CSS-Mode.xshd");
            RegisterHighlighting(HighlightTyp.Cpp, new[] { ".c", ".h", ".cc", ".cpp", ".hpp" }, "CPP-Mode.xshd");
            RegisterHighlighting(HighlightTyp.Java, new[] { ".java" }, "Java-Mode.xshd");
            RegisterHighlighting(HighlightTyp.Patch, new[] { ".patch", ".diff" }, "Patch-Mode.xshd");
            RegisterHighlighting(HighlightTyp.PowerShell, new[] { ".ps1", ".psm1", ".psd1" }, "PowerShell.xshd");
            RegisterHighlighting(HighlightTyp.Php, new[] { ".php" }, "PHP-Mode.xshd");
            RegisterHighlighting(HighlightTyp.Tex, new[] { ".tex" }, "Tex-Mode.xshd");
            RegisterHighlighting(HighlightTyp.VB, new[] { ".vb" }, "VB-Mode.xshd");
            RegisterHighlighting(HighlightTyp.Xml, (".xml;.xsl;.xslt;.xsd;.manifest;.config;.addin;" +
                                         ".xshd;.wxs;.wxi;.wxl;.proj;.csproj;.vbproj;.ilproj;" +
                                         ".booproj;.build;.xfrm;.targets;.xaml;.xpt;" +
                                         ".xft;.map;.wsdl;.disco;.ps1xml;.nuspec").Split(';'),
                "XML-Mode.xshd");
            RegisterHighlighting(HighlightTyp.MarkDown, new[] { ".md" }, "MarkDown-Mode.xshd");
        }

        #endregion

        #region Методы

        internal void RegisterHighlighting(HighlightTyp typ, string[] extensions, string resourceName)
        {
            var name = typ.ToStringEx();

            try
            {
#if DEBUG
                Xshd.XshdSyntaxDefinition xshd;
                using (var s = HighlightingResources.Instance.OpenStream(HighlightingResources.PREFIX_XSHD + resourceName))
                {
                    using (var reader = new XmlTextReader(s))
                    {
                        xshd = Xshd.HighlightingLoader.LoadXshd(reader, false);
                    }
                }
                Debug.Assert(name == xshd.Name);
                if (extensions != null)
                {
                    Debug.Assert(System.Linq.Enumerable.SequenceEqual(extensions, xshd.Extensions));
                }
                else
                {
                    Debug.Assert(xshd.Extensions.Count == 0);
                }

                RegisterHighlighting(name, extensions, Xshd.HighlightingLoader.Load(xshd, this));
#else
		        RegisterHighlighting(name, extensions, LoadHighlighting(resourceName));
#endif
            }
            catch (HighlightingDefinitionInvalidException ex)
            {
                throw new InvalidOperationException("The built-in highlighting '" + name + "' is invalid.", ex);
            }
        }

        #endregion
    }
}