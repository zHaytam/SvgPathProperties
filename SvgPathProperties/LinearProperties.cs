﻿using SvgPathProperties.Base;
using System;

namespace SvgPathProperties
{
    public class LinearProperties : IProperties
    {
        private readonly double _x0;
        private readonly double _y0;
        private readonly double _x1;
        private readonly double _y1;

        public LinearProperties(double x0, double x1, double y0, double y1)
        {
            _x0 = x0;
            _y0 = y0;
            _x1 = x1;
            _y1 = y1;
            Length = Math.Sqrt(Math.Pow(_x0 - _x1, 2) + Math.Pow(_y0 - _y1, 2));
        }

        public double Length { get; }

        public Point GetPointAtLength(double pos)
        {
            var fraction = pos / Length;
            fraction = Double.IsNaN(fraction) ? 1 : fraction;
            var newDeltaX = (_x1 - _x0) * fraction;
            var newDeltaY = (_y1 - _y0) * fraction;
            return new Point(x: _x0 + newDeltaX, y: _y0 + newDeltaY);
        }

        public PointProperties GetPropertiesAtLength(double pos)
        {
            var point = GetPointAtLength(pos);
            var tangent = GetTangentAtLength(pos);
            return new PointProperties(x: point.X, y: point.Y, tangentX: tangent.X, tangentY: tangent.Y);
        }

        public Rect GetBBox()
        {
            var minX = Math.Min(_x0, _x1);
            var minY = Math.Min(_y0, _y1);
            var maxX = Math.Max(_x0, _x1);
            var maxY = Math.Max(_y0, _y1);
            return new Rect(minX, minY, maxX, maxY);
        }

        public Point GetTangentAtLength(double pos)
        {
            var module = Math.Sqrt((_x1 - _x0) * (_x1 - _x0) + (_y1 - _y0) * (_y1 - _y0));
            return new Point(x: (_x1 - _x0) / module, y: (_y1 - _y0) / module);
        }
    }
}
