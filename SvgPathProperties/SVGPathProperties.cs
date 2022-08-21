using SvgPathProperties.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SvgPathProperties
{
    public class SVGPathProperties : IProperties
    {
        private readonly List<double> _partialLengths = new List<double>();
        private readonly List<IProperties> _functions = new List<IProperties>();
        private readonly Point? _initialPoint;

        public SVGPathProperties(string path, bool unarc = false)
        {
            var parsed = Parser.Parse(path);
            var cur = new double[2];
            (double, double) prevPoint = (0, 0);
            BezierProperties curve = null;
            (double, double) ringStart = (0, 0);

            for (var i = 0; i < parsed.Count; i++)
            {
                if (parsed[i].Item1 == 'M')
                {
                    cur = new[] { parsed[i].Item2[0], parsed[i].Item2[1] };
                    ringStart = (cur[0], cur[1]);
                    _functions.Add(new MoveProperties(cur[0], cur[1]));
                    if (i == 0)
                    {
                        _initialPoint = new Point(x: parsed[i].Item2[0], y: parsed[i].Item2[1]);
                    }
                }
                else if (parsed[i].Item1 == 'm')
                {
                    cur = new double[] { parsed[i].Item2[0] + cur[0], parsed[i].Item2[1] + cur[1] };
                    ringStart = (cur[0], cur[1]);
                    _functions.Add(new MoveProperties(cur[0], cur[1]));
                }
                // lineTo
                else if (parsed[i].Item1 == 'L')
                {
                    Length += Math.Sqrt(Math.Pow(cur[0] - parsed[i].Item2[0], 2) +
                                         Math.Pow(cur[1] - parsed[i].Item2[1], 2));
                    _functions.Add(new LinearProperties(cur[0], parsed[i].Item2[0], cur[1], parsed[i].Item2[1]));
                    cur = new double[] { parsed[i].Item2[0], parsed[i].Item2[1] };
                }
                else if (parsed[i].Item1 == 'l')
                {
                    Length += Math.Sqrt(Math.Pow(parsed[i].Item2[0], 2) + Math.Pow(parsed[i].Item2[1], 2));
                    _functions.Add(new LinearProperties(cur[0], parsed[i].Item2[0] + cur[0], cur[1],
                        parsed[i].Item2[1] + cur[1]));
                    cur = new double[] { parsed[i].Item2[0] + cur[0], parsed[i].Item2[1] + cur[1] };
                }
                else if (parsed[i].Item1 == 'H')
                {
                    Length += Math.Abs(cur[0] - parsed[i].Item2[0]);
                    _functions.Add(new LinearProperties(cur[0], parsed[i].Item2[0], cur[1], cur[1]));
                    cur[0] = parsed[i].Item2[0];
                }
                else if (parsed[i].Item1 == 'h')
                {
                    Length += Math.Abs(parsed[i].Item2[0]);
                    _functions.Add(new LinearProperties(cur[0], cur[0] + parsed[i].Item2[0], cur[1], cur[1]));
                    cur[0] = parsed[i].Item2[0] + cur[0];
                }
                else if (parsed[i].Item1 == 'V')
                {
                    Length += Math.Abs(cur[1] - parsed[i].Item2[0]);
                    _functions.Add(new LinearProperties(cur[0], cur[0], cur[1], parsed[i].Item2[0]));
                    cur[1] = parsed[i].Item2[0];
                }
                else if (parsed[i].Item1 == 'v')
                {
                    Length += Math.Abs(parsed[i].Item2[0]);
                    _functions.Add(new LinearProperties(cur[0], cur[0], cur[1], cur[1] + parsed[i].Item2[0]));
                    cur[1] = parsed[i].Item2[0] + cur[1];
                    //Close path
                }
                else if (parsed[i].Item1 == 'z' || parsed[i].Item1 == 'Z')
                {
                    Length += Math.Sqrt(Math.Pow(ringStart.Item1 - cur[0], 2) + Math.Pow(ringStart.Item2 - cur[1], 2));
                    _functions.Add(new LinearProperties(cur[0], ringStart.Item1, cur[1], ringStart.Item2));
                    cur = new double[] { ringStart.Item1, ringStart.Item2 };
                }
                // Cubic Bezier curves
                else if (parsed[i].Item1 == 'C')
                {
                    curve = new BezierProperties(
                        cur[0],
                        cur[1],
                        parsed[i].Item2[0],
                        parsed[i].Item2[1],
                        parsed[i].Item2[2],
                        parsed[i].Item2[3],
                        parsed[i].Item2[4],
                        parsed[i].Item2[5]
                    );
                    Length += curve.Length;
                    cur = new double[] { parsed[i].Item2[4], parsed[i].Item2[5] };
                    _functions.Add(curve);
                }
                else if (parsed[i].Item1 == 'c')
                {
                    curve = new BezierProperties(
                        cur[0],
                        cur[1],
                        cur[0] + parsed[i].Item2[0],
                        cur[1] + parsed[i].Item2[1],
                        cur[0] + parsed[i].Item2[2],
                        cur[1] + parsed[i].Item2[3],
                        cur[0] + parsed[i].Item2[4],
                        cur[1] + parsed[i].Item2[5]
                    );
                    if (curve.Length > 0)
                    {
                        Length += curve.Length;
                        _functions.Add(curve);
                        cur = new double[] { parsed[i].Item2[4] + cur[0], parsed[i].Item2[5] + cur[1] };
                    }
                    else
                    {
                        _functions.Add(new LinearProperties(cur[0], cur[0], cur[1], cur[1]));
                    }
                }
                else if (parsed[i].Item1 == 'S')
                {
                    if (i > 0 && new char[] { 'C', 'c', 'S', 's' }.Contains(parsed[i - 1].Item1))
                    {
                        if (curve != null)
                        {
                            var c = curve.GetC();
                            curve = new BezierProperties(
                                cur[0],
                                cur[1],
                                2 * cur[0] - c.X,
                                2 * cur[1] - c.Y,
                                parsed[i].Item2[0],
                                parsed[i].Item2[1],
                                parsed[i].Item2[2],
                                parsed[i].Item2[3]
                            );
                        }
                    }
                    else
                    {
                        curve = new BezierProperties(
                            cur[0],
                            cur[1],
                            cur[0],
                            cur[1],
                            parsed[i].Item2[0],
                            parsed[i].Item2[1],
                            parsed[i].Item2[2],
                            parsed[i].Item2[3]
                        );
                    }

                    if (curve != null)
                    {
                        Length += curve.Length;
                        cur = new double[] { parsed[i].Item2[2], parsed[i].Item2[3] };
                        _functions.Add(curve);
                    }
                }
                else if (parsed[i].Item1 == 's')
                {
                    //240 225
                    if (i > 0 && new char[] { 'C', 'c', 'S', 's' }.Contains(parsed[i - 1].Item1))
                    {
                        if (curve != null)
                        {
                            var c = curve.GetC();
                            var d = curve.GetD();
                            curve = new BezierProperties(
                                cur[0],
                                cur[1],
                                cur[0] + d.X - c.X,
                                cur[1] + d.Y - c.Y,
                                cur[0] + parsed[i].Item2[0],
                                cur[1] + parsed[i].Item2[1],
                                cur[0] + parsed[i].Item2[2],
                                cur[1] + parsed[i].Item2[3]
                            );
                        }
                    }
                    else
                    {
                        curve = new BezierProperties(
                            cur[0],
                            cur[1],
                            cur[0],
                            cur[1],
                            cur[0] + parsed[i].Item2[0],
                            cur[1] + parsed[i].Item2[1],
                            cur[0] + parsed[i].Item2[2],
                            cur[1] + parsed[i].Item2[3]
                        );
                    }

                    if (curve != null)
                    {
                        Length += curve.Length;
                        cur = new double[] { parsed[i].Item2[2] + cur[0], parsed[i].Item2[3] + cur[1] };
                        _functions.Add(curve);
                    }
                }
                // Quadratic Bezier curves
                else if (parsed[i].Item1 == 'Q')
                {
                    if (cur[0] == parsed[i].Item2[0] && cur[1] == parsed[i].Item2[1])
                    {
                        var linearCurve = new LinearProperties(
                            parsed[i].Item2[0],
                            parsed[i].Item2[2],
                            parsed[i].Item2[1],
                            parsed[i].Item2[3]
                        );
                        Length += linearCurve.Length;
                        _functions.Add(linearCurve);
                    }
                    else
                    {
                        curve = new BezierProperties(
                            cur[0],
                            cur[1],
                            parsed[i].Item2[0],
                            parsed[i].Item2[1],
                            parsed[i].Item2[2],
                            parsed[i].Item2[3],
                            null,
                            null
                        );
                        Length += curve.Length;
                        _functions.Add(curve);
                    }

                    cur = new double[] { parsed[i].Item2[2], parsed[i].Item2[3] };
                    prevPoint = (parsed[i].Item2[0], parsed[i].Item2[1]);
                }
                else if (parsed[i].Item1 == 'q')
                {
                    if (!(parsed[i].Item2[0] == 0 && parsed[i].Item2[1] == 0))
                    {
                        curve = new BezierProperties(
                            cur[0],
                            cur[1],
                            cur[0] + parsed[i].Item2[0],
                            cur[1] + parsed[i].Item2[1],
                            cur[0] + parsed[i].Item2[2],
                            cur[1] + parsed[i].Item2[3],
                            null,
                            null
                        );
                        Length += curve.Length;
                        _functions.Add(curve);
                    }
                    else
                    {
                        var linearCurve = new LinearProperties(
                            cur[0] + parsed[i].Item2[0],
                            cur[0] + parsed[i].Item2[2],
                            cur[1] + parsed[i].Item2[1],
                            cur[1] + parsed[i].Item2[3]
                        );
                        Length += linearCurve.Length;
                        _functions.Add(linearCurve);
                    }

                    prevPoint = (cur[0] + parsed[i].Item2[0], cur[1] + parsed[i].Item2[1]);
                    cur = new double[] { parsed[i].Item2[2] + cur[0], parsed[i].Item2[3] + cur[1] };
                }
                else if (parsed[i].Item1 == 'T')
                {
                    if (i > 0 && new char[] { 'Q', 'q', 'T', 't' }.Contains(parsed[i - 1].Item1))
                    {
                        curve = new BezierProperties(
                            cur[0],
                            cur[1],
                            2 * cur[0] - prevPoint.Item1,
                            2 * cur[1] - prevPoint.Item2,
                            parsed[i].Item2[0],
                            parsed[i].Item2[1],
                            null,
                            null
                        );
                        _functions.Add(curve);
                        Length += curve.Length;
                    }
                    else
                    {
                        var linearCurve = new LinearProperties(cur[0], parsed[i].Item2[0], cur[1], parsed[i].Item2[1]);
                        _functions.Add(linearCurve);
                        Length += linearCurve.Length;
                    }

                    prevPoint = (2 * cur[0] - prevPoint.Item1, 2 * cur[1] - prevPoint.Item2);
                    cur = new double[] { parsed[i].Item2[0], parsed[i].Item2[1] };
                }
                else if (parsed[i].Item1 == 't')
                {
                    if (i > 0 && new char[] { 'Q', 'q', 'T', 't' }.Contains(parsed[i - 1].Item1))
                    {
                        curve = new BezierProperties(
                            cur[0],
                            cur[1],
                            2 * cur[0] - prevPoint.Item1,
                            2 * cur[1] - prevPoint.Item2,
                            cur[0] + parsed[i].Item2[0],
                            cur[1] + parsed[i].Item2[1],
                            null,
                            null
                        );
                        Length += curve.Length;
                        _functions.Add(curve);
                    }
                    else
                    {
                        var linearCurve = new LinearProperties(
                            cur[0],
                            cur[0] + parsed[i].Item2[0],
                            cur[1],
                            cur[1] + parsed[i].Item2[1]
                        );
                        Length += linearCurve.Length;
                        _functions.Add(linearCurve);
                    }

                    prevPoint = (2 * cur[0] - prevPoint.Item1, 2 * cur[1] - prevPoint.Item2);
                    cur = new double[] { parsed[i].Item2[0] + cur[0], parsed[i].Item2[1] + cur[1] };
                }
                // Arcs
                else if (parsed[i].Item1 == 'A')
                {
                    if (unarc)
                    {
                        var newSegments = ArcUtils.ToCurve(cur[0], cur[1], parsed[i].Item2[5], parsed[i].Item2[6],
                            parsed[i].Item2[3], parsed[i].Item2[4], parsed[i].Item2[0], parsed[i].Item2[1],
                            parsed[i].Item2[2]);
                        if (newSegments.Count == 0)
                        {
                            var l = new LinearProperties(cur[0], cur[1], parsed[i].Item2[5], parsed[i].Item2[6]);
                            Length += l.Length;
                            _functions.Add(l);
                        }
                        else
                        {
                            foreach (var s in newSegments)
                            {
                                var c = new BezierProperties(s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7]);
                                Length += c.Length;
                                _functions.Add(c);
                            }
                        }

                        cur = new[] { parsed[i].Item2[5], parsed[i].Item2[6] };
                    }
                    else
                    {
                        var arcCurve = new ArcProperties(
                            cur[0],
                            cur[1],
                            parsed[i].Item2[0],
                            parsed[i].Item2[1],
                            parsed[i].Item2[2],
                            parsed[i].Item2[3] == 1,
                            parsed[i].Item2[4] == 1,
                            parsed[i].Item2[5],
                            parsed[i].Item2[6]
                        );

                        Length += arcCurve.Length;
                        cur = new [] { parsed[i].Item2[5], parsed[i].Item2[6] };
                        _functions.Add(arcCurve);
                    }
                }
                else if (parsed[i].Item1 == 'a')
                {
                    if (unarc)
                    {
                        var newSegments = ArcUtils.ToCurve(cur[0], cur[1], cur[0] + parsed[i].Item2[5],
                            cur[1] + parsed[i].Item2[6], parsed[i].Item2[3], parsed[i].Item2[4], parsed[i].Item2[0],
                            parsed[i].Item2[1], parsed[i].Item2[2]);
                        
                        if (newSegments.Count == 0)
                        {
                            var l = new LinearProperties(cur[0], cur[1], parsed[i].Item2[5], parsed[i].Item2[6]);
                            Length += l.Length;
                            _functions.Add(l);
                        }
                        else
                        {
                            foreach (var s in newSegments)
                            {
                                var c = new BezierProperties(s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7]);
                                Length += c.Length;
                                _functions.Add(c);
                            }
                        }

                        cur = new[] { cur[0] + parsed[i].Item2[5], cur[1] + parsed[i].Item2[6] };
                    }
                    else
                    {
                        var arcCurve = new ArcProperties(
                            cur[0],
                            cur[1],
                            parsed[i].Item2[0],
                            parsed[i].Item2[1],
                            parsed[i].Item2[2],
                            parsed[i].Item2[3] == 1,
                            parsed[i].Item2[4] == 1,
                            cur[0] + parsed[i].Item2[5],
                            cur[1] + parsed[i].Item2[6]
                        );

                        Length += arcCurve.Length;
                        cur = new[] { cur[0] + parsed[i].Item2[5], cur[1] + parsed[i].Item2[6] };
                        _functions.Add(arcCurve);
                    }
                }

                _partialLengths.Add(Length);
            }
        }
        
        public double Length { get; }
        public IReadOnlyList<IProperties> Segments => _functions;

        public (int i, double fraction) GetPartAtLength(double fractionLength)
        {
            if (fractionLength < 0)
            {
                fractionLength = 0;
            }
            else if (fractionLength > Length)
            {
                fractionLength = Length;
            }

            var i = _partialLengths.Count - 1;
            while (_partialLengths[i] >= fractionLength && i > 0)
            {
                i--;
            }

            i++;

            return (i, fractionLength - _partialLengths[i - 1]);
        }

        public Point GetPointAtLength(double fractionLength)
        {
            var fractionPart = GetPartAtLength(fractionLength);
            var functionAtPart = fractionPart.i >= _functions.Count ? null : _functions[fractionPart.i];

            if (functionAtPart != null)
            {
                return functionAtPart.GetPointAtLength(fractionPart.fraction);
            }
            else if (_initialPoint != null)
            {
                return _initialPoint.Value;
            }

            throw new Exception("Wrong function at this part.");
        }

        public PointProperties GetPropertiesAtLength(double fractionLength)
        {
            var fractionPart = GetPartAtLength(fractionLength);
            var functionAtPart = fractionPart.i >= _functions.Count ? null : _functions[fractionPart.i];
            if (functionAtPart != null)
            {
                return functionAtPart.GetPropertiesAtLength(fractionPart.fraction);
            }
            else if (_initialPoint != null)
            {
                return new PointProperties(x: _initialPoint.Value.X, y: _initialPoint.Value.Y, tangentX: 0,
                    tangentY: 0);
            }

            throw new Exception("Wrong function at this part.");
        }

        public Rect GetBBox()
        {
            var minX = double.PositiveInfinity;
            var minY = double.PositiveInfinity;
            var maxX = double.NegativeInfinity;
            var maxY = double.NegativeInfinity;

            foreach (var part in _functions)
            {
                var bbox = part.GetBBox();
                minX = Math.Min(minX, bbox.Left);
                minY = Math.Min(minY, bbox.Top);
                maxX = Math.Max(maxX, bbox.Right);
                maxY = Math.Max(maxY, bbox.Bottom);
            }

            return new Rect(minX, minY, maxX, maxY);
        }

        public Point GetTangentAtLength(double fractionLength)
        {
            var fractionPart = GetPartAtLength(fractionLength);
            var functionAtPart = fractionPart.i >= _functions.Count ? null : _functions[fractionPart.i];
            if (functionAtPart != null)
            {
                return functionAtPart.GetTangentAtLength(fractionPart.fraction);
            }
            else if (_initialPoint != null)
            {
                return new Point(0, 0);
            }

            throw new Exception("Wrong function at this part.");
        }

        public List<PartProperties> GetParts()
        {
            var parts = new List<PartProperties>();
            for (var i = 0; i < _functions.Count; i++)
            {
                if (!(_functions[i] is MoveProperties))
                {
                    _functions[i] = _functions[i];
                    PartProperties properties = new PartProperties(_functions[i].GetPointAtLength(0),
                        _functions[i].GetPointAtLength(_partialLengths[i] - _partialLengths[i - 1]),
                        _partialLengths[i] - _partialLengths[i - 1], _functions[i]);
                    parts.Add(properties);
                }
            }

            return parts;
        }
    }
}