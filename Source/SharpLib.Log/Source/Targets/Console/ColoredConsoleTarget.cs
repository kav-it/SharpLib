using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace SharpLib.Log
{
    [Target("ColoredConsole")]
    public sealed class ColoredConsoleTarget : TargetWithLayoutHeaderAndFooter
    {
        #region Поля

        private static readonly IList<ConsoleRowHighlightingRule> _defaultConsoleRowHighlightingRules = new List<ConsoleRowHighlightingRule>
        {
            new ConsoleRowHighlightingRule("level == LogLevel.Fatal", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange),
            new ConsoleRowHighlightingRule("level == LogLevel.Error", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange),
            new ConsoleRowHighlightingRule("level == LogLevel.Warn", ConsoleOutputColor.Magenta, ConsoleOutputColor.NoChange),
            new ConsoleRowHighlightingRule("level == LogLevel.Info", ConsoleOutputColor.White, ConsoleOutputColor.NoChange),
            new ConsoleRowHighlightingRule("level == LogLevel.Debug", ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange),
            new ConsoleRowHighlightingRule("level == LogLevel.Trace", ConsoleOutputColor.DarkGray, ConsoleOutputColor.NoChange),
        };

        #endregion

        #region Свойства

        [DefaultValue(false)]
        public bool ErrorStream { get; set; }

        [DefaultValue(true)]
        public bool UseDefaultRowHighlightingRules { get; set; }

        [ArrayParameter(typeof(ConsoleRowHighlightingRule), "highlight-row")]
        public IList<ConsoleRowHighlightingRule> RowHighlightingRules { get; private set; }

        [ArrayParameter(typeof(ConsoleWordHighlightingRule), "highlight-word")]
        public IList<ConsoleWordHighlightingRule> WordHighlightingRules { get; private set; }

        #endregion

        #region Конструктор

        public ColoredConsoleTarget()
        {
            WordHighlightingRules = new List<ConsoleWordHighlightingRule>();
            RowHighlightingRules = new List<ConsoleRowHighlightingRule>();
            UseDefaultRowHighlightingRules = true;
        }

        #endregion

        #region Методы

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            if (Header != null)
            {
                LogEventInfo lei = LogEventInfo.CreateNullEvent();
                Output(lei, Header.Render(lei));
            }
        }

        protected override void CloseTarget()
        {
            if (Footer != null)
            {
                LogEventInfo lei = LogEventInfo.CreateNullEvent();
                Output(lei, Footer.Render(lei));
            }

            base.CloseTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            Output(logEvent, Layout.Render(logEvent));
        }

        private static void ColorizeEscapeSequences(
            TextWriter output,
            string message,
            ColorPair startingColor,
            ColorPair defaultColor)
        {
            var colorStack = new Stack<ColorPair>();

            colorStack.Push(startingColor);

            int p0 = 0;

            while (p0 < message.Length)
            {
                int p1 = p0;
                while (p1 < message.Length && message[p1] >= 32)
                {
                    p1++;
                }

                if (p1 != p0)
                {
                    output.Write(message.Substring(p0, p1 - p0));
                }

                if (p1 >= message.Length)
                {
                    p0 = p1;
                    break;
                }

                char c1 = message[p1];
                char c2 = (char)0;

                if (p1 + 1 < message.Length)
                {
                    c2 = message[p1 + 1];
                }

                if (c1 == '\a' && c2 == '\a')
                {
                    output.Write('\a');
                    p0 = p1 + 2;
                    continue;
                }

                if (c1 == '\r' || c1 == '\n')
                {
                    Console.ForegroundColor = defaultColor.ForegroundColor;
                    Console.BackgroundColor = defaultColor.BackgroundColor;
                    output.Write(c1);
                    Console.ForegroundColor = colorStack.Peek().ForegroundColor;
                    Console.BackgroundColor = colorStack.Peek().BackgroundColor;
                    p0 = p1 + 1;
                    continue;
                }

                if (c1 == '\a')
                {
                    if (c2 == 'X')
                    {
                        colorStack.Pop();
                        Console.ForegroundColor = colorStack.Peek().ForegroundColor;
                        Console.BackgroundColor = colorStack.Peek().BackgroundColor;
                        p0 = p1 + 2;
                        continue;
                    }

                    var foreground = (ConsoleOutputColor)(c2 - 'A');
                    var background = (ConsoleOutputColor)(message[p1 + 2] - 'A');

                    if (foreground != ConsoleOutputColor.NoChange)
                    {
                        Console.ForegroundColor = (ConsoleColor)foreground;
                    }

                    if (background != ConsoleOutputColor.NoChange)
                    {
                        Console.BackgroundColor = (ConsoleColor)background;
                    }

                    colorStack.Push(new ColorPair(Console.ForegroundColor, Console.BackgroundColor));
                    p0 = p1 + 3;
                    continue;
                }

                output.Write(c1);
                p0 = p1 + 1;
            }

            if (p0 < message.Length)
            {
                output.Write(message.Substring(p0));
            }
        }

        private void Output(LogEventInfo logEvent, string message)
        {
            ConsoleColor oldForegroundColor = Console.ForegroundColor;
            ConsoleColor oldBackgroundColor = Console.BackgroundColor;

            try
            {
                ConsoleRowHighlightingRule matchingRule = null;

                foreach (ConsoleRowHighlightingRule cr in RowHighlightingRules)
                {
                    if (cr.CheckCondition(logEvent))
                    {
                        matchingRule = cr;
                        break;
                    }
                }

                if (UseDefaultRowHighlightingRules && matchingRule == null)
                {
                    foreach (ConsoleRowHighlightingRule cr in _defaultConsoleRowHighlightingRules)
                    {
                        if (cr.CheckCondition(logEvent))
                        {
                            matchingRule = cr;
                            break;
                        }
                    }
                }

                if (matchingRule == null)
                {
                    matchingRule = ConsoleRowHighlightingRule.Default;
                }

                if (matchingRule.ForegroundColor != ConsoleOutputColor.NoChange)
                {
                    Console.ForegroundColor = (ConsoleColor)matchingRule.ForegroundColor;
                }

                if (matchingRule.BackgroundColor != ConsoleOutputColor.NoChange)
                {
                    Console.BackgroundColor = (ConsoleColor)matchingRule.BackgroundColor;
                }

                message = message.Replace("\a", "\a\a");

                foreach (ConsoleWordHighlightingRule hl in WordHighlightingRules)
                {
                    message = hl.ReplaceWithEscapeSequences(message);
                }

                ColorizeEscapeSequences(ErrorStream ? Console.Error : Console.Out, message, new ColorPair(Console.ForegroundColor, Console.BackgroundColor),
                    new ColorPair(oldForegroundColor, oldBackgroundColor));
            }
            finally
            {
                Console.ForegroundColor = oldForegroundColor;
                Console.BackgroundColor = oldBackgroundColor;
            }

            if (ErrorStream)
            {
                Console.Error.WriteLine();
            }
            else
            {
                Console.WriteLine();
            }
        }

        #endregion

        #region Вложенный класс: ColorPair

        internal struct ColorPair
        {
            #region Поля

            private readonly ConsoleColor _backgroundColor;

            private readonly ConsoleColor _foregroundColor;

            #endregion

            #region Свойства

            internal ConsoleColor BackgroundColor
            {
                get { return _backgroundColor; }
            }

            internal ConsoleColor ForegroundColor
            {
                get { return _foregroundColor; }
            }

            #endregion

            #region Конструктор

            internal ColorPair(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
            {
                _foregroundColor = foregroundColor;
                _backgroundColor = backgroundColor;
            }

            #endregion
        }

        #endregion
    }
}