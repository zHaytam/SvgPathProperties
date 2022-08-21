using SvgPathProperties.Base;
using System;

namespace SvgPathProperties
{
    public struct PointOnEllipticalArc
    {
        public PointOnEllipticalArc(double x, double y, double ellipticalArcAngle)
        {
            X = x;
            Y = y;
            EllipticalArcAngle = ellipticalArcAngle;
        }

        public double X { get; }
        public double Y { get; }
        public double EllipticalArcAngle { get; }
    }

    public class ArcProperties : IProperties
    {
        private readonly double _x0;
        private readonly double _y0;
        private readonly double _rx;
        private readonly double _ry;
        private readonly double _xAxisRotate;
        private readonly bool _largeArcFlag;
        private readonly bool _sweepFlag;
        private readonly double _x1;
        private readonly double _y1;
        private readonly double _length;

        public ArcProperties(double x0, double y0, double rx, double ry, double xAxisRotate,
            bool largeArcFlag, bool sweepFlag, double x1, double y1)
        {
            _x0 = x0;
            _y0 = y0;
            _rx = rx;
            _ry = ry;
            _xAxisRotate = xAxisRotate;
            _largeArcFlag = largeArcFlag;
            _sweepFlag = sweepFlag;
            _x1 = x1;
            _y1 = y1;

            _length = ApproximateArcLengthOfCurve(300, t =>
            {
                return PointOnEllipticalArc(new Point(x0, y0), rx, ry, xAxisRotate, largeArcFlag, sweepFlag,
                    new Point(x1, y1), t);
            });
        }

        public Point GetPointAtLength(double fractionLength)
        {
            if (fractionLength < 0)
            {
                fractionLength = 0;
            }
            else if (fractionLength > _length)
            {
                fractionLength = _length;
            }

            var position = PointOnEllipticalArc(new Point(x: _x0, y: _y0), _rx, _ry, _xAxisRotate,
                _largeArcFlag, _sweepFlag, new Point(x: _x1, y: _y1), fractionLength / _length);

            return new Point(x: position.X, y: position.Y);
        }

        public PointProperties GetPropertiesAtLength(double fractionLength)
        {
            var tangent = GetTangentAtLength(fractionLength);
            var point = GetPointAtLength(fractionLength);
            return new PointProperties(x: point.X, y: point.Y, tangentX: tangent.X, tangentY: tangent.Y);
        }

        public (Point, Point) GetBBox()
        {
            throw new NotImplementedException();
        }

        public Point GetTangentAtLength(double fractionLength)
        {
            if (fractionLength < 0)
            {
                fractionLength = 0;
            }
            else if (fractionLength > _length)
            {
                fractionLength = _length;
            }

            var point_dist = 0.05;
            var p1 = GetPointAtLength(fractionLength);
            Point p2;

            if (fractionLength < 0)
            {
                fractionLength = 0;
            }
            else if (fractionLength > _length)
            {
                fractionLength = _length;
            }

            if (fractionLength < _length - point_dist)
            {
                p2 = GetPointAtLength(fractionLength + point_dist);
            }
            else
            {
                p2 = GetPointAtLength(fractionLength - point_dist);
            }

            var xDist = p2.X - p1.X;
            var yDist = p2.Y - p1.Y;
            var dist = Math.Sqrt(xDist * xDist + yDist * yDist);

            if (fractionLength < _length - point_dist)
            {
                return new Point(x: -xDist / dist, y: -yDist / dist);
            }
            else
            {
                return new Point(x: xDist / dist, y: yDist / dist);
            }
        }

        public double GetTotalLength() => _length;

        private static double Mod(double x, double m) => ((x % m) + m) % m;

        private static double ToRadians(double angle) => angle * (Math.PI / 180);

        private static double Distance(Point p0, Point p1)
            => Math.Sqrt(Math.Pow(p1.X - p0.X, 2) + Math.Pow(p1.Y - p0.Y, 2));

        private static double Clamp(double val, double min, double max) => Math.Min(Math.Max(val, min), max);

        private static double AngleBetween(Point v0, Point v1)
        {
            var p = v0.X * v1.X + v0.Y * v1.Y;
            var n = Math.Sqrt((Math.Pow(v0.X, 2) + Math.Pow(v0.Y, 2)) * (Math.Pow(v1.X, 2) + Math.Pow(v1.Y, 2)));
            var sign = v0.X * v1.Y - v0.Y * v1.X < 0 ? -1 : 1;
            return sign * Math.Acos(p / n);
        }

        private static Point PointOnEllipticalArc(Point p0, double rx, double ry, double xAxisRotation,
            bool largeArcFlag, bool sweepFlag, Point p1, double t)
        {
            // In accordance to: http://www.w3.org/TR/SVG/implnote.html#ArcOutOfRangeParameters
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);
            xAxisRotation = Mod(xAxisRotation, 360);
            var xAxisRotationRadians = ToRadians(xAxisRotation);
            // If the endpoints are identical, then this is equivalent to omitting the elliptical arc segment entirely.
            if (p0.X == p1.X && p0.Y == p1.Y)
                return new Point(x: p0.X, y: p0.Y/*, ellipticalArcAngle: 0*/); // Check if angle is correct

            // If rx = 0 or ry = 0 then this arc is treated as a straight line segment joining the endpoints.
            if (rx == 0 || ry == 0)
            {
                //return this.pointOnLine(p0, p1, t);
                return new Point(x: 0, y: 0/*, ellipticalArcAngle: 0*/); // Check if angle is correct
            }

            // Following "Conversion from endpoint to center parameterization"
            // http://www.w3.org/TR/SVG/implnote.html#ArcConversionEndpointToCenter

            // Step #1: Compute transformedPoint
            var dx = (p0.X - p1.X) / 2;
            var dy = (p0.Y - p1.Y) / 2;
            var transformedPoint = new Point(Math.Cos(xAxisRotationRadians) * dx + Math.Sin(xAxisRotationRadians) * dy,
                -Math.Sin(xAxisRotationRadians) * dx + Math.Cos(xAxisRotationRadians) * dy);
            // Ensure radii are large enough
            var radiiCheck = Math.Pow(transformedPoint.X, 2) / Math.Pow(rx, 2) + Math.Pow(transformedPoint.Y, 2) / Math.Pow(ry, 2);
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
            }

            // Step #2: Compute transformedCenter
            var cSquareNumerator =
              Math.Pow(rx, 2) * Math.Pow(ry, 2) -
              Math.Pow(rx, 2) * Math.Pow(transformedPoint.Y, 2) -
              Math.Pow(ry, 2) * Math.Pow(transformedPoint.X, 2);
            var cSquareRootDenom =
              Math.Pow(rx, 2) * Math.Pow(transformedPoint.Y, 2) +
              Math.Pow(ry, 2) * Math.Pow(transformedPoint.X, 2);
            var cRadicand = cSquareNumerator / cSquareRootDenom;
            // Make sure this never drops below zero because of precision
            cRadicand = cRadicand < 0 ? 0 : cRadicand;
            var cCoef = (largeArcFlag != sweepFlag ? 1 : -1) * Math.Sqrt(cRadicand);
            var transformedCenter = new Point(x: cCoef * ((rx * transformedPoint.Y) / ry),
                y: cCoef * (-(ry * transformedPoint.X) / rx));

            // Step #3: Compute center
            var center = new Point(
                x:
                    Math.Cos(xAxisRotationRadians) * transformedCenter.X -
                    Math.Sin(xAxisRotationRadians) * transformedCenter.Y +
                    (p0.X + p1.X) / 2,
                y:
                    Math.Sin(xAxisRotationRadians) * transformedCenter.X +
                    Math.Cos(xAxisRotationRadians) * transformedCenter.Y +
                    (p0.Y + p1.Y) / 2
            );

            // Step #4: Compute start/sweep angles
            // Start angle of the elliptical arc prior to the stretch and rotate operations.
            // Difference between the start and end angles
            var startVector = new Point((transformedPoint.X - transformedCenter.X) / rx,
                (transformedPoint.Y - transformedCenter.Y) / ry);
            var startAngle = AngleBetween(new Point(1, 0), startVector);

            var endVector = new Point((-transformedPoint.X - transformedCenter.X) / rx,
                (-transformedPoint.Y - transformedCenter.Y) / ry);
            var sweepAngle = AngleBetween(startVector, endVector);

            if (!sweepFlag && sweepAngle > 0)
            {
                sweepAngle -= 2 * Math.PI;
            }
            else if (sweepFlag && sweepAngle < 0)
            {
                sweepAngle += 2 * Math.PI;
            }
            // We use % instead of `mod(..)` because we want it to be -360deg to 360deg(but actually in radians)
            sweepAngle %= 2 * Math.PI;

            // From http://www.w3.org/TR/SVG/implnote.html#ArcParameterizationAlternatives
            var angle = startAngle + sweepAngle * t;
            var ellipseComponentX = rx * Math.Cos(angle);
            var ellipseComponentY = ry * Math.Sin(angle);

            return new Point(
                x:
                    Math.Cos(xAxisRotationRadians) * ellipseComponentX -
                    Math.Sin(xAxisRotationRadians) * ellipseComponentY +
                    center.X,
                y:
                    Math.Sin(xAxisRotationRadians) * ellipseComponentX +
                    Math.Cos(xAxisRotationRadians) * ellipseComponentY +
                    center.Y/*,
                ellipticalArcAngle: angle*/
            );
        }

        public static double ApproximateArcLengthOfCurve(double? resolution, Func<double, Point> pointOnCurveFunc)
        {
            // Resolution is the number of segments we use
            resolution = resolution ?? 500;
            double resultantArcLength = 0;
            // var arcLengthMap = [];
            // var approximationLines = [];

            var prevPoint = pointOnCurveFunc(0);
            Point nextPoint;
            for (var i = 0; i < resolution; i++)
            {
                var t = Clamp(i * (1 / resolution.Value), 0, 1);
                nextPoint = pointOnCurveFunc(t);
                resultantArcLength += Distance(prevPoint, nextPoint);
                // approximationLines.push([prevPoint, nextPoint]);

                //arcLengthMap.push({
                //    t: t,
                //    arcLength: resultantArcLength
                //});

                prevPoint = nextPoint;
            }

            // Last stretch to the endpoint
            nextPoint = pointOnCurveFunc(1);
            // approximationLines.push([prevPoint, nextPoint]);
            resultantArcLength += Distance(prevPoint, nextPoint);
            //arcLengthMap.push({
            //    t: 1,
            //    arcLength: resultantArcLength
            //});

            //return {
            //    arcLength: resultantArcLength,
            //    arcLengthMap: arcLengthMap,
            //    approximationLines: approximationLines
            //};
            return resultantArcLength;
        }
    }
}
