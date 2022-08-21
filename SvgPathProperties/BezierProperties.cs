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

        public (Point, Point) GetBBox()
        {
            var min = new[] { double.PositiveInfinity, double.PositiveInfinity };
            var max = new[] { double.NegativeInfinity, double.NegativeInfinity };
            var x = _a.X;
            var y = _a.Y;

            if (_d.X == 0 && _d.Y == 0) // Quadratic
            {
                var qxMinMax = CurveMinMax.MinMaxQ(new[] { x, _b.X, _c.X });
                if (min[0] > qxMinMax[0])
                {
                    min[0] = qxMinMax[0];
                }

                if (max[0] < qxMinMax[1])
                {
                    max[0] = qxMinMax[1];
                }

                var qyMinMax = CurveMinMax.MinMaxQ(new[] { y, _b.Y, _c.Y });
                if (min[1] > qyMinMax[0])
                {
                    min[1] = qyMinMax[0];
                }

                if (max[1] < qyMinMax[1])
                {
                    max[1] = qyMinMax[1];
                }
            }
            else
            {
                // _ bX   bY  cX   cY  dX  dY
                // 0  1   2    3    4   5   6
                // C 337 205, 76 494, 191 494 
                var cxMinMax = CurveMinMax.MinMaxC(new[] { x, _b.X, _c.X, _d.X });
                if (min[0] > cxMinMax[0])
                {
                    min[0] = cxMinMax[0];
                }

                if (max[0] < cxMinMax[1])
                {
                    max[0] = cxMinMax[1];
                }

                var cyMinMax = CurveMinMax.MinMaxC(new[] { y, _b.Y, _c.Y, _d.Y });
                if (min[1] > cyMinMax[0])
                {
                    min[1] = cyMinMax[0];
                }

                if (max[1] < cyMinMax[1])
                {
                    max[1] = cyMinMax[1];
                }
            }

            return (new Point(min[0], min[1]), new Point(max[0], max[1]));
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