using System;
using System.Xml;
using System.Xml.Schema;

namespace SharpLib.Texter.Highlighting.Xshd
{
    public static class HighlightingLoader
    {
        #region Методы

        public static XshdSyntaxDefinition LoadXshd(XmlReader reader)
        {
            return LoadXshd(reader, false);
        }

        internal static XshdSyntaxDefinition LoadXshd(XmlReader reader, bool skipValidation)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            try
            {
                reader.MoveToContent();
                if (reader.NamespaceURI == V2Loader.Namespace)
                {
                    return V2Loader.LoadDefinition(reader, skipValidation);
                }
                return V1Loader.LoadDefinition(reader, skipValidation);
            }
            catch (XmlSchemaException ex)
            {
                throw WrapException(ex, ex.LineNumber, ex.LinePosition);
            }
            catch (XmlException ex)
            {
                throw WrapException(ex, ex.LineNumber, ex.LinePosition);
            }
        }

        private static Exception WrapException(Exception ex, int lineNumber, int linePosition)
        {
            return new HighlightingDefinitionInvalidException(FormatExceptionMessage(ex.Message, lineNumber, linePosition), ex);
        }

        internal static string FormatExceptionMessage(string message, int lineNumber, int linePosition)
        {
            if (lineNumber <= 0)
            {
                return message;
            }
            return "Error at position (line " + lineNumber + ", column " + linePosition + "):\n" + message;
        }

        internal static XmlReader GetValidatingReader(XmlReader input, bool ignoreWhitespace, XmlSchemaSet schemaSet)
        {
            var settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = ignoreWhitespace;
            if (schemaSet != null)
            {
                settings.Schemas = schemaSet;
                settings.ValidationType = ValidationType.Schema;
            }
            return XmlReader.Create(input, settings);
        }

        internal static XmlSchemaSet LoadSchemaSet(XmlReader schemaInput)
        {
            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, schemaInput);
            schemaSet.ValidationEventHandler += delegate(object sender, ValidationEventArgs args) { throw new HighlightingDefinitionInvalidException(args.Message); };
            return schemaSet;
        }

        public static IHighlightingDefinition Load(XshdSyntaxDefinition syntaxDefinition, IHighlightingDefinitionReferenceResolver resolver)
        {
            if (syntaxDefinition == null)
            {
                throw new ArgumentNullException("syntaxDefinition");
            }
            return new XmlHighlightingDefinition(syntaxDefinition, resolver);
        }

        public static IHighlightingDefinition Load(XmlReader reader, IHighlightingDefinitionReferenceResolver resolver)
        {
            return Load(LoadXshd(reader), resolver);
        }

        #endregion
    }
}