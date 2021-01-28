using SvgPathProperties.Base;
using System;

namespace SvgPathProperties
{
    public class BezierProperties : IProperties
    {
        private readonly Point _a;
        private readonly Point _b;
        private readonly Point _c;
        private readonly Point _d;
        private readonly Func<double[], double[], double, double> _getArcLength;
        private readonly Func<double[], double[], double, Point> _getPoint;
        private readonly Func<double[], double[], double, Point> _getDerivative;
        private double _length;

        public BezierProperties(double ax, double ay, double bx, double by, double cx, double cy,
            double? dx, double? dy)
        {
            _a = new Point(ax, ay);
            _b = new Point(bx, by);
            _c = new Point(cx, cy);

            if (dx != null && dy != null)
            {
                _getArcLength = BezierFunctions.GetCubicArcLength;
                _getPoint = BezierFunctions.CubicPoint;
                _getDerivative = BezierFunctions.CubicDerivative;
                _d = new Point(dx.Value, dy.Value);
            }
            else
            {
                _getArcLength = BezierFunctions.GetQuadraticArcLength;
                _getPoint = BezierFunctions.QuadraticPoint;
                _getDerivative = BezierFunctions.QuadraticDerivative;
                _d = new Point(0, 0);
            }

            _length = _getArcLength(new[] { _a.X, _b.X, _c.X, _d.X }, new[] { _a.Y, _b.Y, _c.Y, _d.Y }, 1);
        }

        public Point GetPointAtLength(double length)
        {
            var xs = new[] { _a.X, _b.X, _c.X, _d.X };
            var xy = new[] { _a.Y, _b.Y, _c.Y, _d.Y };
            var t = BezierFunctions.T2length(length, _length, i => _getArcLength(xs, xy, i));
            return _getPoint(xs, xy, t);
        }

        public PointProperties GetPropertiesAtLength(double length)
        {
            var xs = new[] { _a.X, _b.X, _c.X, _d.X };
            var xy = new[] { _a.Y, _b.Y, _c.Y, _d.Y };
            var t = BezierFunctions.T2length(length, _length, i => _getArcLength(xs, xy, i));

            var derivative = _getDerivative(xs, xy, t);
            var mdl = Math.Sqrt(derivative.X * derivative.X + derivative.Y * derivative.Y);
            Point tangent;
            if (mdl > 0)
            {
                tangent = new Point(x: derivative.X / mdl, y: derivative.Y / mdl);
            }
            else
            {
                tangent = new Point(0, 0);
            }
            var point = _getPoint(xs, xy, t);
            return new PointProperties(x: point.X, y: point.Y, tangentX: tangent.X, tangentY: tangent.Y);
        }

        public Point GetTangentAtLength(double length)
        {
            var xs = new[] { _a.X, _b.X, _c.X, _d.X };
            var xy = new[] { _a.Y, _b.Y, _c.Y, _d.Y };
            var t = BezierFunctions.T2length(length, _length, i => _getArcLength(xs, xy, i));

            var derivative = _getDerivative(xs, xy, t);
            var mdl = Math.Sqrt(derivative.X * derivative.X + derivative.Y * derivative.Y);
            if (mdl > 0)
            {
                return new Point(x: derivative.X / mdl, y: derivative.Y / mdl);
            }
            else
            {
                return new Point(0, 0);
            }
        }

        public double GetTotalLength() => _length;

        public Point GetC() => _c;

        public Point GetD() => _d;
    }
}
