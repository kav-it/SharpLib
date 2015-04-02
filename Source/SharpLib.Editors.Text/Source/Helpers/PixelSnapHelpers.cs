﻿using System;
using System.Windows;
using System.Windows.Media;

namespace SharpLib.Notepad.Utils
{
    public static class PixelSnapHelpers
    {
        #region Методы

        public static Size GetPixelSize(Visual visual)
        {
            if (visual == null)
            {
                throw new ArgumentNullException("visual");
            }
            var source = PresentationSource.FromVisual(visual);
            if (source != null)
            {
                var matrix = source.CompositionTarget.TransformFromDevice;
                return new Size(matrix.M11, matrix.M22);
            }
            return new Size(1, 1);
        }

        public static double PixelAlign(double value, double pixelSize)
        {
            return pixelSize * (Math.Round((value / pixelSize) + 0.5) - 0.5);
        }

        public static Rect PixelAlign(Rect rect, Size pixelSize)
        {
            rect.X = PixelAlign(rect.X, pixelSize.Width);
            rect.Y = PixelAlign(rect.Y, pixelSize.Height);
            rect.Width = Round(rect.Width, pixelSize.Width);
            rect.Height = Round(rect.Height, pixelSize.Height);
            return rect;
        }

        public static Point Round(Point point, Size pixelSize)
        {
            return new Point(Round(point.X, pixelSize.Width), Round(point.Y, pixelSize.Height));
        }

        public static Rect Round(Rect rect, Size pixelSize)
        {
            return new Rect(Round(rect.X, pixelSize.Width), Round(rect.Y, pixelSize.Height),
                Round(rect.Width, pixelSize.Width), Round(rect.Height, pixelSize.Height));
        }

        public static double Round(double value, double pixelSize)
        {
            return pixelSize * Math.Round(value / pixelSize);
        }

        public static double RoundToOdd(double value, double pixelSize)
        {
            return Round(value - pixelSize, pixelSize * 2) + pixelSize;
        }

        #endregion
    }
}