using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    internal sealed class LayoutParser
    {
        #region Методы

        internal static LayoutRenderer[] CompileLayout(ConfigurationItemFactory configurationItemFactory, SimpleStringReader sr, bool isNested, out string text)
        {
            var result = new List<LayoutRenderer>();
            var literalBuf = new StringBuilder();

            int ch;

            int p0 = sr.Position;

            while ((ch = sr.Peek()) != -1)
            {
                if (isNested && (ch == '}' || ch == ':'))
                {
                    break;
                }

                sr.Read();

                if (ch == '$' && sr.Peek() == '{')
                {
                    if (literalBuf.Length > 0)
                    {
                        result.Add(new LiteralLayoutRenderer(literalBuf.ToString()));
                        literalBuf.Length = 0;
                    }

                    LayoutRenderer newLayoutRenderer = ParseLayoutRenderer(configurationItemFactory, sr);
                    if (CanBeConvertedToLiteral(newLayoutRenderer))
                    {
                        newLayoutRenderer = ConvertToLiteral(newLayoutRenderer);
                    }

                    result.Add(newLayoutRenderer);
                }
                else
                {
                    literalBuf.Append((char)ch);
                }
            }

            if (literalBuf.Length > 0)
            {
                result.Add(new LiteralLayoutRenderer(literalBuf.ToString()));
                literalBuf.Length = 0;
            }

            int p1 = sr.Position;

            MergeLiterals(result);
            text = sr.Substring(p0, p1);

            return result.ToArray();
        }

        private static string ParseLayoutRendererName(SimpleStringReader sr)
        {
            int ch;

            var nameBuf = new StringBuilder();
            while ((ch = sr.Peek()) != -1)
            {
                if (ch == ':' || ch == '}')
                {
                    break;
                }

                nameBuf.Append((char)ch);
                sr.Read();
            }

            return nameBuf.ToString();
        }

        private static string ParseParameterName(SimpleStringReader sr)
        {
            int ch;
            int nestLevel = 0;

            var nameBuf = new StringBuilder();
            while ((ch = sr.Peek()) != -1)
            {
                if ((ch == '=' || ch == '}' || ch == ':') && nestLevel == 0)
                {
                    break;
                }

                if (ch == '$')
                {
                    sr.Read();
                    nameBuf.Append('$');
                    if (sr.Peek() == '{')
                    {
                        nameBuf.Append('{');
                        nestLevel++;
                        sr.Read();
                    }

                    continue;
                }

                if (ch == '}')
                {
                    nestLevel--;
                }

                if (ch == '\\')
                {
                    sr.Read();

                    nameBuf.Append((char)sr.Read());
                    continue;
                }

                nameBuf.Append((char)ch);
                sr.Read();
            }

            return nameBuf.ToString();
        }

        private static string ParseParameterValue(SimpleStringReader sr)
        {
            int ch;

            var nameBuf = new StringBuilder();
            while ((ch = sr.Peek()) != -1)
            {
                if (ch == ':' || ch == '}')
                {
                    break;
                }

                if (ch == '\\')
                {
                    sr.Read();

                    var nextChar = (char)sr.Peek();

                    switch (nextChar)
                    {
                        case ':':
                            sr.Read();
                            nameBuf.Append(':');
                            break;
                        case '{':
                            sr.Read();
                            nameBuf.Append('{');
                            break;
                        case '}':
                            sr.Read();
                            nameBuf.Append('}');
                            break;

                        case '\'':
                            sr.Read();
                            nameBuf.Append('\'');
                            break;
                        case '"':
                            sr.Read();
                            nameBuf.Append('"');
                            break;
                        case '\\':
                            sr.Read();
                            nameBuf.Append('\\');
                            break;
                        case '0':
                            sr.Read();
                            nameBuf.Append('\0');
                            break;
                        case 'a':
                            sr.Read();
                            nameBuf.Append('\a');
                            break;
                        case 'b':
                            sr.Read();
                            nameBuf.Append('\b');
                            break;
                        case 'f':
                            sr.Read();
                            nameBuf.Append('\f');
                            break;
                        case 'n':
                            sr.Read();
                            nameBuf.Append('\n');
                            break;
                        case 'r':
                            sr.Read();
                            nameBuf.Append('\r');
                            break;
                        case 't':
                            sr.Read();
                            nameBuf.Append('\t');
                            break;
                        case 'u':
                            sr.Read();
                            var uChar = GetUnicode(sr, 4);
                            nameBuf.Append(uChar);
                            break;
                        case 'U':
                            sr.Read();
                            var UChar = GetUnicode(sr, 8);
                            nameBuf.Append(UChar);
                            break;
                        case 'x':
                            sr.Read();
                            var xChar = GetUnicode(sr, 4);
                            nameBuf.Append(xChar);
                            break;
                        case 'v':
                            sr.Read();
                            nameBuf.Append('\v');
                            break;
                    }

                    continue;
                }

                nameBuf.Append((char)ch);
                sr.Read();
            }

            return nameBuf.ToString();
        }

        private static char GetUnicode(SimpleStringReader sr, int maxDigits)
        {
            int code = 0;

            for (int cnt = 0; cnt < maxDigits; cnt++)
            {
                var digitCode = sr.Peek();
                if (digitCode >= '0' && digitCode <= '9')
                {
                    digitCode = digitCode - '0';
                }
                else if (digitCode >= 'a' && digitCode <= 'f')
                {
                    digitCode = digitCode - 'a' + 10;
                }
                else if (digitCode >= 'A' && digitCode <= 'F')
                {
                    digitCode = digitCode - 'A' + 10;
                }
                else
                {
                    break;
                }

                sr.Read();
                code = code * 16 + digitCode;
            }

            return (char)code;
        }

        private static LayoutRenderer ParseLayoutRenderer(ConfigurationItemFactory configurationItemFactory, SimpleStringReader sr)
        {
            int ch = sr.Read();
            Debug.Assert(ch == '{', "'{' expected in layout specification");

            string name = ParseLayoutRendererName(sr);
            LayoutRenderer lr = configurationItemFactory.LayoutRenderers.CreateInstance(name);

            var wrappers = new Dictionary<Type, LayoutRenderer>();
            var orderedWrappers = new List<LayoutRenderer>();

            ch = sr.Read();
            while (ch != -1 && ch != '}')
            {
                string parameterName = ParseParameterName(sr).Trim();
                if (sr.Peek() == '=')
                {
                    sr.Read();
                    PropertyInfo pi;
                    LayoutRenderer parameterTarget = lr;

                    if (!PropertyHelper.TryGetPropertyInfo(lr, parameterName, out pi))
                    {
                        Type wrapperType;

                        if (configurationItemFactory.AmbientProperties.TryGetDefinition(parameterName, out wrapperType))
                        {
                            LayoutRenderer wrapperRenderer;

                            if (!wrappers.TryGetValue(wrapperType, out wrapperRenderer))
                            {
                                wrapperRenderer = configurationItemFactory.AmbientProperties.CreateInstance(parameterName);
                                wrappers[wrapperType] = wrapperRenderer;
                                orderedWrappers.Add(wrapperRenderer);
                            }

                            if (!PropertyHelper.TryGetPropertyInfo(wrapperRenderer, parameterName, out pi))
                            {
                                pi = null;
                            }
                            else
                            {
                                parameterTarget = wrapperRenderer;
                            }
                        }
                    }

                    if (pi == null)
                    {
                        ParseParameterValue(sr);
                    }
                    else
                    {
                        if (typeof(Layout).IsAssignableFrom(pi.PropertyType))
                        {
                            var nestedLayout = new SimpleLayout();
                            string txt;
                            LayoutRenderer[] renderers = CompileLayout(configurationItemFactory, sr, true, out txt);

                            nestedLayout.SetRenderers(renderers, txt);
                            pi.SetValue(parameterTarget, nestedLayout, null);
                        }
                        else if (typeof(ConditionExpression).IsAssignableFrom(pi.PropertyType))
                        {
                            var conditionExpression = ConditionParser.ParseExpression(sr, configurationItemFactory);
                            pi.SetValue(parameterTarget, conditionExpression, null);
                        }
                        else
                        {
                            string value = ParseParameterValue(sr);
                            PropertyHelper.SetPropertyFromString(parameterTarget, parameterName, value, configurationItemFactory);
                        }
                    }
                }
                else
                {
                    PropertyInfo pi;

                    if (PropertyHelper.TryGetPropertyInfo(lr, string.Empty, out pi))
                    {
                        if (typeof(SimpleLayout) == pi.PropertyType)
                        {
                            pi.SetValue(lr, new SimpleLayout(parameterName), null);
                        }
                        else
                        {
                            string value = parameterName;
                            PropertyHelper.SetPropertyFromString(lr, pi.Name, value, configurationItemFactory);
                        }
                    }
                }

                ch = sr.Read();
            }

            lr = ApplyWrappers(configurationItemFactory, lr, orderedWrappers);

            return lr;
        }

        private static LayoutRenderer ApplyWrappers(ConfigurationItemFactory configurationItemFactory, LayoutRenderer lr, List<LayoutRenderer> orderedWrappers)
        {
            for (int i = orderedWrappers.Count - 1; i >= 0; --i)
            {
                var newRenderer = (WrapperLayoutRendererBase)orderedWrappers[i];
                if (CanBeConvertedToLiteral(lr))
                {
                    lr = ConvertToLiteral(lr);
                }

                newRenderer.Inner = new SimpleLayout(new[] { lr }, string.Empty, configurationItemFactory);
                lr = newRenderer;
            }

            return lr;
        }

        private static bool CanBeConvertedToLiteral(LayoutRenderer lr)
        {
            foreach (IRenderable renderable in ObjectGraphScanner.FindReachableObjects<IRenderable>(lr))
            {
                if (renderable.GetType() == typeof(SimpleLayout))
                {
                    continue;
                }

                if (!renderable.GetType().IsDefined(typeof(AppDomainFixedOutputAttribute), false))
                {
                    return false;
                }
            }

            return true;
        }

        private static void MergeLiterals(List<LayoutRenderer> list)
        {
            for (int i = 0; i + 1 < list.Count; )
            {
                var lr1 = list[i] as LiteralLayoutRenderer;
                var lr2 = list[i + 1] as LiteralLayoutRenderer;
                if (lr1 != null && lr2 != null)
                {
                    lr1.Text += lr2.Text;
                    list.RemoveAt(i + 1);
                }
                else
                {
                    i++;
                }
            }
        }

        private static LayoutRenderer ConvertToLiteral(LayoutRenderer renderer)
        {
            return new LiteralLayoutRenderer(renderer.Render(LogEventInfo.CreateNullEvent()));
        }

        #endregion
    }
}
