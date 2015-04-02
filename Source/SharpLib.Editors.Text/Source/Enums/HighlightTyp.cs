using System.ComponentModel;

namespace SharpLib.Notepad
{
    public enum HighlightTyp
    {
        [Description("*")]
        Unknown,

        [Description("XmlDoc")]
        XmlDoc,

        [Description("C#")]
        Sharp,

        [Description("JavaScript")]
        JavaScript,

        [Description("HTML")]
        Html,

        [Description("ASP/XHTML")]
        Asp,

        [Description("Boo")]
        Boo,

        [Description("Coco")]
        Coco,

        [Description("CSS")]
        Css,

        [Description("C++")]
        Cpp,

        [Description("Java")]
        Java,

        [Description("Patch")]
        Patch,

        [Description("PowerShell")]
        PowerShell,

        [Description("PHP")]
        Php,

        [Description("TeX")]
        Tex,

        [Description("VB")]
        VB,

        [Description("XML")]
        Xml,

        [Description("MarkDown")]
        MarkDown
    }
}