using SvgPathProperties.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SvgPathProperties
{
    public class SvgPath : ICommand
    {
        private readonly List<double> _partialLengths = new List<double>();
        private readonly List<ICommand> _commands = new List<ICommand>();
        private Point? _initialPoint;
        private double[] _cur = new double[2];
        private (double X, double Y) _prevPoint = (0, 0);
        private BezierCommand _lastCurve = null;
        private (double X, double Y) _ringStart = (0, 0);

        public SvgPath()
        {

        }

        public SvgPath(string path, bool unarc = false)
        {
            var parsed = Parser.Parse(path);

            for (var i = 0; i < parsed.Count; i++)
            {
                if (parsed[i].Item1 == 'M')
                {
                    _cur = new[] { parsed[i].Item2[0], parsed[i].Item2[1] };
                    _ringStart = (_cur[0], _cur[1]);
                    _commands.Add(new MoveCommand(_cur[0], _cur[1]));
                    if (i == 0)
                    {
                        _initialPoint = new Point(x: parsed[i].Item2[0], y: parsed[i].Item2[1]);
                    }
                }
                else if (parsed[i].Item1 == 'm')
                {
                    _cur = new double[] { parsed[i].Item2[0] + _cur[0], parsed[i].Item2[1] + _cur[1] };
                    _ringStart = (_cur[0], _cur[1]);
                    _commands.Add(new MoveCommand(_cur[0], _cur[1]));
                }
                // lineTo
                else if (parsed[i].Item1 == 'L')
                {
                    var l = new LineCommand(_cur[0], parsed[i].Item2[0], _cur[1], parsed[i].Item2[1]);
                    Length += l.Length;
                    _commands.Add(l);
                    _cur = new double[] { parsed[i].Item2[0], parsed[i].Item2[1] };
                }
                else if (parsed[i].Item1 == 'l')
                {
                    var l = new LineCommand(_cur[0], parsed[i].Item2[0] + _cur[0], _cur[1], parsed[i].Item2[1] + _cur[1]);
                    Length += l.Length;
                    _commands.Add(l);
                    _cur = new double[] { parsed[i].Item2[0] + _cur[0], parsed[i].Item2[1] + _cur[1] };
                }
                else if (parsed[i].Item1 == 'H')
                {
                    Length += Math.Abs(_cur[0] - parsed[i].Item2[0]);
                    _commands.Add(new LineCommand(_cur[0], parsed[i].Item2[0], _cur[1], _cur[1]));
                    _cur[0] = parsed[i].Item2[0];
                }
                else if (parsed[i].Item1 == 'h')
                {
                    Length += Math.Abs(parsed[i].Item2[0]);
                    _commands.Add(new LineCommand(_cur[0], _cur[0] + parsed[i].Item2[0], _cur[1], _cur[1]));
                    _cur[0] = parsed[i].Item2[0] + _cur[0];
                }
                else if (parsed[i].Item1 == 'V')
                {
                    Length += Math.Abs(_cur[1] - parsed[i].Item2[0]);
                    _commands.Add(new LineCommand(_cur[0], _cur[0], _cur[1], parsed[i].Item2[0]));
                    _cur[1] = parsed[i].Item2[0];
                }
                else if (parsed[i].Item1 == 'v')
                {
                    Length += Math.Abs(parsed[i].Item2[0]);
                    _commands.Add(new LineCommand(_cur[0], _cur[0], _cur[1], _cur[1] + parsed[i].Item2[0]));
                    _cur[1] = parsed[i].Item2[0] + _cur[1];
                }
                //Close path
                else if (parsed[i].Item1 == 'z' || parsed[i].Item1 == 'Z')
                {
                    var l = new LineCommand(_cur[0], _ringStart.Item1, _cur[1], _ringStart.Item2, closePath: true);
                    Length += l.Length;
                    _commands.Add(l);
                    _cur = new double[] { _ringStart.Item1, _ringStart.Item2 };
                }
                // Cubic Bezier curves
                else if (parsed[i].Item1 == 'C')
                {
                    _lastCurve = new BezierCommand(
                        _cur[0],
                        _cur[1],
                        parsed[i].Item2[0],
                        parsed[i].Item2[1],
                        parsed[i].Item2[2],
                        parsed[i].Item2[3],
                        parsed[i].Item2[4],
                        parsed[i].Item2[5]
                    );
                    Length += _lastCurve.Length;
                    _cur = new double[] { parsed[i].Item2[4], parsed[i].Item2[5] };
                    _commands.Add(_lastCurve);
                }
                else if (parsed[i].Item1 == 'c')
                {
                    _lastCurve = new BezierCommand(
                        _cur[0],
                        _cur[1],
                        _cur[0] + parsed[i].Item2[0],
                        _cur[1] + parsed[i].Item2[1],
                        _cur[0] + parsed[i].Item2[2],
                        _cur[1] + parsed[i].Item2[3],
                        _cur[0] + parsed[i].Item2[4],
                        _cur[1] + parsed[i].Item2[5]
                    );
                    if (_lastCurve.Length > 0)
                    {
                        Length += _lastCurve.Length;
                        _commands.Add(_lastCurve);
                        _cur = new double[] { parsed[i].Item2[4] + _cur[0], parsed[i].Item2[5] + _cur[1] };
                    }
                    else
                    {
                        _commands.Add(new LineCommand(_cur[0], _cur[0], _cur[1], _cur[1]));
                    }
                }
                else if (parsed[i].Item1 == 'S')
                {
                    if (i > 0 && new char[] { 'C', 'c', 'S', 's' }.Contains(parsed[i - 1].Item1))
                    {
                        if (_lastCurve != null)
                        {
                            var c = _lastCurve.Cp2OrEnd;
                            _lastCurve = new BezierCommand(
                                _cur[0],
                                _cur[1],
                                2 * _cur[0] - c.X,
                                2 * _cur[1] - c.Y,
                                parsed[i].Item2[0],
                                parsed[i].Item2[1],
                                parsed[i].Item2[2],
                                parsed[i].Item2[3]
                            );
                        }
                    }
                    else
                    {
                        _lastCurve = new BezierCommand(
                            _cur[0],
                            _cur[1],
                            _cur[0],
                            _cur[1],
                            parsed[i].Item2[0],
                            parsed[i].Item2[1],
                            parsed[i].Item2[2],
                            parsed[i].Item2[3]
                        );
                    }

                    if (_lastCurve != null)
                    {
                        Length += _lastCurve.Length;
                        _cur = new double[] { parsed[i].Item2[2], parsed[i].Item2[3] };
                        _commands.Add(_lastCurve);
                    }
                }
                else if (parsed[i].Item1 == 's')
                {
                    if (i > 0 && new char[] { 'C', 'c', 'S', 's' }.Contains(parsed[i - 1].Item1))
                    {
                        if (_lastCurve != null)
                        {
                            var c = _lastCurve.Cp2OrEnd;
                            var d = _lastCurve.End;
                            _lastCurve = new BezierCommand(
                                _cur[0],
                                _cur[1],
                                _cur[0] + d.X - c.X,
                                _cur[1] + d.Y - c.Y,
                                _cur[0] + parsed[i].Item2[0],
                                _cur[1] + parsed[i].Item2[1],
                                _cur[0] + parsed[i].Item2[2],
                                _cur[1] + parsed[i].Item2[3]
                            );
                        }
                    }
                    else
                    {
                        _lastCurve = new BezierCommand(
                            _cur[0],
                            _cur[1],
                            _cur[0],
                            _cur[1],
                            _cur[0] + parsed[i].Item2[0],
                            _cur[1] + parsed[i].Item2[1],
                            _cur[0] + parsed[i].Item2[2],
                            _cur[1] + parsed[i].Item2[3]
                        );
                    }

                    if (_lastCurve != null)
                    {
                        Length += _lastCurve.Length;
                        _cur = new double[] { parsed[i].Item2[2] + _cur[0], parsed[i].Item2[3] + _cur[1] };
                        _commands.Add(_lastCurve);
                    }
                }
                // Quadratic Bezier curves
                else if (parsed[i].Item1 == 'Q')
                {
                    if (_cur[0] == parsed[i].Item2[0] && _cur[1] == parsed[i].Item2[1])
                    {
                        var linearCurve = new LineCommand(
                            parsed[i].Item2[0],
                            parsed[i].Item2[2],
                            parsed[i].Item2[1],
                            parsed[i].Item2[3]
                        );
                        Length += linearCurve.Length;
                        _commands.Add(linearCurve);
                    }
                    else
                    {
                        _lastCurve = new BezierCommand(
                            _cur[0],
                            _cur[1],
                            parsed[i].Item2[0],
                            parsed[i].Item2[1],
                            parsed[i].Item2[2],
                            parsed[i].Item2[3],
                            null,
                            null
                        );
                        Length += _lastCurve.Length;
                        _commands.Add(_lastCurve);
                    }

                    _cur = new double[] { parsed[i].Item2[2], parsed[i].Item2[3] };
                    _prevPoint = (parsed[i].Item2[0], parsed[i].Item2[1]);
                }
                else if (parsed[i].Item1 == 'q')
                {
                    if (parsed[i].Item2[0] == 0 && parsed[i].Item2[1] == 0)
                    {
                        var linearCurve = new LineCommand(
                            _cur[0] + parsed[i].Item2[0],
                            _cur[0] + parsed[i].Item2[2],
                            _cur[1] + parsed[i].Item2[1],
                            _cur[1] + parsed[i].Item2[3]
                        );
                        Length += linearCurve.Length;
                        _commands.Add(linearCurve);
                    }
                    else
                    {
                        _lastCurve = new BezierCommand(
                            _cur[0],
                            _cur[1],
                            _cur[0] + parsed[i].Item2[0],
                            _cur[1] + parsed[i].Item2[1],
                            _cur[0] + parsed[i].Item2[2],
                            _cur[1] + parsed[i].Item2[3],
                            null,
                            null
                        );
                        Length += _lastCurve.Length;
                        _commands.Add(_lastCurve);
                    }

                    _prevPoint = (_cur[0] + parsed[i].Item2[0], _cur[1] + parsed[i].Item2[1]);
                    _cur = new double[] { parsed[i].Item2[2] + _cur[0], parsed[i].Item2[3] + _cur[1] };
                }
                else if (parsed[i].Item1 == 'T')
                {
                    if (i > 0 && new char[] { 'Q', 'q', 'T', 't' }.Contains(parsed[i - 1].Item1))
                    {
                        _lastCurve = new BezierCommand(
                            _cur[0],
                            _cur[1],
                            2 * _cur[0] - _prevPoint.Item1,
                            2 * _cur[1] - _prevPoint.Item2,
                            parsed[i].Item2[0],
                            parsed[i].Item2[1],
                            null,
                            null
                        );
                        _commands.Add(_lastCurve);
                        Length += _lastCurve.Length;
                    }
                    else
                    {
                        var linearCurve = new LineCommand(_cur[0], parsed[i].Item2[0], _cur[1], parsed[i].Item2[1]);
                        _commands.Add(linearCurve);
                        Length += linearCurve.Length;
                    }

                    _prevPoint = (2 * _cur[0] - _prevPoint.Item1, 2 * _cur[1] - _prevPoint.Item2);
                    _cur = new double[] { parsed[i].Item2[0], parsed[i].Item2[1] };
                }
                else if (parsed[i].Item1 == 't')
                {
                    if (i > 0 && new char[] { 'Q', 'q', 'T', 't' }.Contains(parsed[i - 1].Item1))
                    {
                        _lastCurve = new BezierCommand(
                            _cur[0],
                            _cur[1],
                            2 * _cur[0] - _prevPoint.Item1,
                            2 * _cur[1] - _prevPoint.Item2,
                            _cur[0] + parsed[i].Item2[0],
                            _cur[1] + parsed[i].Item2[1],
                            null,
                            null
                        );
                        Length += _lastCurve.Length;
                        _commands.Add(_lastCurve);
                    }
                    else
                    {
                        var linearCurve = new LineCommand(
                            _cur[0],
                            _cur[0] + parsed[i].Item2[0],
                            _cur[1],
                            _cur[1] + parsed[i].Item2[1]
                        );
                        Length += linearCurve.Length;
                        _commands.Add(linearCurve);
                    }

                    _prevPoint = (2 * _cur[0] - _prevPoint.Item1, 2 * _cur[1] - _prevPoint.Item2);
                    _cur = new double[] { parsed[i].Item2[0] + _cur[0], parsed[i].Item2[1] + _cur[1] };
                }
                // Arcs
                else if (parsed[i].Item1 == 'A')
                {
                    if (unarc)
                    {
                        var newSegments = ArcUtils.ToCurve(_cur[0], _cur[1], parsed[i].Item2[5], parsed[i].Item2[6],
                            parsed[i].Item2[3], parsed[i].Item2[4], parsed[i].Item2[0], parsed[i].Item2[1],
                            parsed[i].Item2[2]);
                        if (newSegments.Count == 0)
                        {
                            var l = new LineCommand(_cur[0], _cur[1], parsed[i].Item2[5], parsed[i].Item2[6]);
                            Length += l.Length;
                            _commands.Add(l);
                        }
                        else
                        {
                            foreach (var s in newSegments)
                            {
                                var c = new BezierCommand(s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7]);
                                Length += c.Length;
                                _commands.Add(c);
                            }
                        }

                        _cur = new[] { parsed[i].Item2[5], parsed[i].Item2[6] };
                    }
                    else
                    {
                        var arcCurve = new ArcCommand(
                            _cur[0],
                            _cur[1],
                            parsed[i].Item2[0],
                            parsed[i].Item2[1],
                            parsed[i].Item2[2],
                            parsed[i].Item2[3] == 1,
                            parsed[i].Item2[4] == 1,
                            parsed[i].Item2[5],
                            parsed[i].Item2[6]
                        );

                        Length += arcCurve.Length;
                        _cur = new[] { parsed[i].Item2[5], parsed[i].Item2[6] };
                        _commands.Add(arcCurve);
                    }
                }
                else if (parsed[i].Item1 == 'a')
                {
                    if (unarc)
                    {
                        var newSegments = ArcUtils.ToCurve(_cur[0], _cur[1], _cur[0] + parsed[i].Item2[5],
                            _cur[1] + parsed[i].Item2[6], parsed[i].Item2[3], parsed[i].Item2[4], parsed[i].Item2[0],
                            parsed[i].Item2[1], parsed[i].Item2[2]);

                        if (newSegments.Count == 0)
                        {
                            var l = new LineCommand(_cur[0], _cur[1], _cur[0] + parsed[i].Item2[5], _cur[1] + parsed[i].Item2[6]);
                            Length += l.Length;
                            _commands.Add(l);
                        }
                        else
                        {
                            foreach (var s in newSegments)
                            {
                                var c = new BezierCommand(s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7]);
                                Length += c.Length;
                                _commands.Add(c);
                            }
                        }

                        _cur = new[] { _cur[0] + parsed[i].Item2[5], _cur[1] + parsed[i].Item2[6] };
                    }
                    else
                    {
                        var arcCurve = new ArcCommand(
                            _cur[0],
                            _cur[1],
                            parsed[i].Item2[0],
                            parsed[i].Item2[1],
                            parsed[i].Item2[2],
                            parsed[i].Item2[3] == 1,
                            parsed[i].Item2[4] == 1,
                            _cur[0] + parsed[i].Item2[5],
                            _cur[1] + parsed[i].Item2[6]
                        );

                        Length += arcCurve.Length;
                        _cur = new[] { _cur[0] + parsed[i].Item2[5], _cur[1] + parsed[i].Item2[6] };
                        _commands.Add(arcCurve);
                    }
                }

                _partialLengths.Add(Length);
            }
        }

        public double Length { get; private set; }
        public IReadOnlyList<ICommand> Segments => _commands;

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
            var functionAtPart = fractionPart.i >= _commands.Count ? null : _commands[fractionPart.i];

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
            var functionAtPart = fractionPart.i >= _commands.Count ? null : _commands[fractionPart.i];
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

            foreach (var part in _commands)
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
            var functionAtPart = fractionPart.i >= _commands.Count ? null : _commands[fractionPart.i];
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
            for (var i = 0; i < _commands.Count; i++)
            {
                if (!(_commands[i] is MoveCommand))
                {
                    _commands[i] = _commands[i];
                    PartProperties properties = new PartProperties(_commands[i].GetPointAtLength(0),
                        _commands[i].GetPointAtLength(_partialLengths[i] - _partialLengths[i - 1]),
                        _partialLengths[i] - _partialLengths[i - 1], _commands[i]);
                    parts.Add(properties);
                }
            }

            return parts;
        }

        public SvgPath AddMoveTo(double x, double y, bool abs = true)
        {
            if (abs)
            {
                _cur = new[] { x, y };
            }
            else
            {
                _cur = new double[] { x + _cur[0], y + _cur[1] };
            }

            _ringStart = (_cur[0], _cur[1]);
            _commands.Add(new MoveCommand(_cur[0], _cur[1]));

            if (_commands.Count == 1)
            {
                _initialPoint = new Point(_cur[0], _cur[1]);
            }

            _partialLengths.Add(Length);
            return this;
        }

        public SvgPath AddLineTo(double x, double y, bool abs = true)
        {
            EnsureMoveFirst();

            if (!abs)
            {
                x += _cur[0];
                y += _cur[1];
            }

            var l = new LineCommand(_cur[0], x, _cur[1], y);
            _commands.Add(l);

            _cur[0] = x;
            _cur[1] = y;
            Length += l.Length;
            _partialLengths.Add(Length);
            return this;
        }

        public SvgPath AddHorizontalLineTo(double x, bool abs = true)
        {
            EnsureMoveFirst();

            if (!abs)
            {
                x += _cur[0];
            }

            Length += Math.Abs(_cur[0] - x);
            _commands.Add(new LineCommand(_cur[0], x, _cur[1], _cur[1]));
            _cur[0] = x;
            _partialLengths.Add(Length);
            return this;
        }

        public SvgPath AddVerticalLineTo(double y, bool abs = true)
        {
            EnsureMoveFirst();

            if (!abs)
            {
                y += _cur[1];
            }

            Length += Math.Abs(_cur[1] - y);
            _commands.Add(new LineCommand(_cur[0], _cur[0], _cur[1], y));
            _cur[1] = y;
            _partialLengths.Add(Length);
            return this;
        }

        public SvgPath AddClosePath()
        {
            EnsureMoveFirst();

            var l = new LineCommand(_cur[0], _ringStart.X, _cur[1], _ringStart.Y, closePath: true);
            _commands.Add(l);
            _cur[0] = _ringStart.X;
            _cur[1] = _ringStart.Y;
            Length += l.Length;
            _partialLengths.Add(Length);
            return this;
        }

        public SvgPath AddCubicBezierCurve(double x1, double y1, double x2, double y2, double x, double y, bool abs = true)
        {
            EnsureMoveFirst();

            if (!abs)
            {
                x1 += _cur[0];
                y1 += _cur[1];
                x2 += _cur[0];
                y2 += _cur[1];
                x += _cur[0];
                y += _cur[1];
            }

            _lastCurve = new BezierCommand(_cur[0], _cur[1], x1, y1, x2, y2, x, y);

            if (_lastCurve.Length > 0)
            {
                Length += _lastCurve.Length;
                _commands.Add(_lastCurve);
                _cur[0] = x;
                _cur[1] = y;
            }
            else
            {
                _commands.Add(new LineCommand(_cur[0], _cur[0], _cur[1], _cur[1]));
            }

            _partialLengths.Add(Length);
            return this;
        }

        public SvgPath AddSmoothCubicBezierCurve(double x2, double y2, double x, double y, bool abs = true)
        {
            EnsureMoveFirst();

            if (abs)
            {
                if (_lastCurve != null && _lastCurve.IsQuadratic == false)
                {
                    var c = _lastCurve.Cp2OrEnd;
                    _lastCurve = new BezierCommand(
                        _cur[0],
                        _cur[1],
                        2 * _cur[0] - c.X,
                        2 * _cur[1] - c.Y,
                        x2,
                        y2,
                        x,
                        y
                    );
                }
                else
                {
                    _lastCurve = new BezierCommand(
                        _cur[0],
                        _cur[1],
                        _cur[0],
                        _cur[1],
                        x2,
                        y2,
                        x,
                        y
                    );
                }

                if (_lastCurve != null)
                {
                    Length += _lastCurve.Length;
                    _cur[0] = x;
                    _cur[1] = y;
                    _commands.Add(_lastCurve);
                }
            }
            else
            {
                if (_lastCurve != null && _lastCurve.IsQuadratic == false)
                {
                    var c = _lastCurve.Cp2OrEnd;
                    var d = _lastCurve.End;
                    _lastCurve = new BezierCommand(
                        _cur[0],
                        _cur[1],
                        _cur[0] + d.X - c.X,
                        _cur[1] + d.Y - c.Y,
                        _cur[0] + x2,
                        _cur[1] + y2,
                        _cur[0] + x,
                        _cur[1] + y
                    );
                }
                else
                {
                    _lastCurve = new BezierCommand(
                        _cur[0],
                        _cur[1],
                        _cur[0],
                        _cur[1],
                        _cur[0] + x2,
                        _cur[1] + y2,
                        _cur[0] + x,
                        _cur[1] + y
                    );
                }

                if (_lastCurve != null)
                {
                    Length += _lastCurve.Length;
                    _cur[0] = x + _cur[0];
                    _cur[1] = y + _cur[1];
                    _commands.Add(_lastCurve);
                }
            }

            _partialLengths.Add(Length);
            return this;
        }

        public SvgPath AddQuadraticBezierCurve(double x1, double y1, double x, double y, bool abs = true)
        {
            EnsureMoveFirst();

            if (abs)
            {
                if (_cur[0] == x1 && _cur[1] == y1)
                {
                    var linearCurve = new LineCommand(
                        x1,
                        x,
                        y1,
                        y
                    );
                    Length += linearCurve.Length;
                    _commands.Add(linearCurve);
                }
                else
                {
                    _lastCurve = new BezierCommand(
                        _cur[0],
                        _cur[1],
                        x1,
                        y1,
                        x,
                        y,
                        null,
                        null
                    );
                    Length += _lastCurve.Length;
                    _commands.Add(_lastCurve);
                }

                _cur[0] = x;
                _cur[1] = y;
                _prevPoint = (x1, y1);
            }
            else
            {
                if (x1 == 0 && y1 == 0)
                {
                    var linearCurve = new LineCommand(
                        _cur[0] + x1,
                        _cur[0] + x,
                        _cur[1] + y1,
                        _cur[1] + y
                    );
                    Length += linearCurve.Length;
                    _commands.Add(linearCurve);
                }
                else
                {
                    _lastCurve = new BezierCommand(
                        _cur[0],
                        _cur[1],
                        _cur[0] + x1,
                        _cur[1] + y1,
                        _cur[0] + x,
                        _cur[1] + y,
                        null,
                        null
                    );
                    Length += _lastCurve.Length;
                    _commands.Add(_lastCurve);
                }

                _prevPoint = (_cur[0] + x1, _cur[1] + y1);
                _cur[0] += x;
                _cur[1] += y;
            }

            _partialLengths.Add(Length);
            return this;
        }

        public SvgPath AddSmoothQuadraticBezierCurve(double x, double y, bool abs = true)
        {
            EnsureMoveFirst();

            if (abs)
            {
                if (_commands.Count > 0 && _commands[_commands.Count - 1] is BezierCommand bc && bc.IsQuadratic)
                {
                    _lastCurve = new BezierCommand(
                        _cur[0],
                        _cur[1],
                        2 * _cur[0] - _prevPoint.X,
                        2 * _cur[1] - _prevPoint.Y,
                        x,
                        y,
                        null,
                        null
                    );
                    _commands.Add(_lastCurve);
                    Length += _lastCurve.Length;
                }
                else
                {
                    var linearCurve = new LineCommand(_cur[0], x, _cur[1], y);
                    _commands.Add(linearCurve);
                    Length += linearCurve.Length;
                }

                _prevPoint = (2 * _cur[0] - _prevPoint.X, 2 * _cur[1] - _prevPoint.Y);
                _cur[0] = x;
                _cur[1] = y;
            }
            else
            {
                if (_commands.Count > 0 && _commands[_commands.Count - 1] is BezierCommand bc && bc.IsQuadratic)
                {
                    _lastCurve = new BezierCommand(
                        _cur[0],
                        _cur[1],
                        2 * _cur[0] - _prevPoint.X,
                        2 * _cur[1] - _prevPoint.Y,
                        _cur[0] + x,
                        _cur[1] + y,
                        null,
                        null
                    );
                    Length += _lastCurve.Length;
                    _commands.Add(_lastCurve);
                }
                else
                {
                    var linearCurve = new LineCommand(
                        _cur[0],
                        _cur[0] + x,
                        _cur[1],
                        _cur[1] + y
                    );
                    Length += linearCurve.Length;
                    _commands.Add(linearCurve);
                }

                _prevPoint = (2 * _cur[0] - _prevPoint.X, 2 * _cur[1] - _prevPoint.Y);
                _cur[0] += x;
                _cur[1] += y;
            }

            _partialLengths.Add(Length);
            return this;
        }

        public SvgPath AddArc(double rx, double ry, double angle, bool largeArcFlag, bool sweepFlag, double x, double y, bool unarc = false, bool abs = true)
        {
            EnsureMoveFirst();

            if (!abs)
            {
                x += _cur[0];
                y += _cur[1];
            }

            if (unarc)
            {
                var newSegments = ArcUtils.ToCurve(_cur[0], _cur[1], x, y, Convert.ToDouble(largeArcFlag), Convert.ToDouble(sweepFlag), rx, ry, angle);
                if (newSegments.Count == 0)
                {
                    var l = new LineCommand(_cur[0], _cur[1], x, y);
                    Length += l.Length;
                    _commands.Add(l);
                }
                else
                {
                    foreach (var s in newSegments)
                    {
                        var c = new BezierCommand(s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7]);
                        Length += c.Length;
                        _commands.Add(c);
                    }
                }

                _cur = new[] { x, y };
            }
            else
            {
                var arcCurve = new ArcCommand(
                    _cur[0],
                    _cur[1],
                    rx,
                    ry,
                    angle,
                    largeArcFlag,
                    sweepFlag,
                    x,
                    y
                );

                Length += arcCurve.Length;
                _cur[0] = x;
                _cur[1] = y;
                _commands.Add(arcCurve);
            }

            _partialLengths.Add(Length);
            return this;
        }

        private void EnsureMoveFirst()
        {
            if (_commands.Count == 0)
                throw new InvalidOperationException("The first command must be a MoveTo");
        }

        public override string ToString()
        {
            return string.Join(" ", _commands.Select(c => c.ToString()));
        }
    }
}