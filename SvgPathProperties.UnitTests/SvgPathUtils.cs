using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SvgPathProperties.UnitTests
{
    public static class SvgPathUtils
    {
        private static readonly Dictionary<char, MethodInfo> _methods = new Dictionary<char, MethodInfo>
        {
            { 'M', typeof(SvgPath).GetMethod(nameof(SvgPath.AddMoveTo), BindingFlags.Instance | BindingFlags.Public) },
            { 'L', typeof(SvgPath).GetMethod(nameof(SvgPath.AddLineTo), BindingFlags.Instance | BindingFlags.Public) },
            { 'H', typeof(SvgPath).GetMethod(nameof(SvgPath.AddHorizontalLineTo), BindingFlags.Instance | BindingFlags.Public) },
            { 'V', typeof(SvgPath).GetMethod(nameof(SvgPath.AddVerticalLineTo), BindingFlags.Instance | BindingFlags.Public) },
            { 'Z', typeof(SvgPath).GetMethod(nameof(SvgPath.AddClosePath), BindingFlags.Instance | BindingFlags.Public) },
            { 'C', typeof(SvgPath).GetMethod(nameof(SvgPath.AddCubicBezierCurve), BindingFlags.Instance | BindingFlags.Public) },
            { 'S', typeof(SvgPath).GetMethod(nameof(SvgPath.AddSmoothCubicBezierCurve), BindingFlags.Instance | BindingFlags.Public) },
            { 'Q', typeof(SvgPath).GetMethod(nameof(SvgPath.AddQuadraticBezierCurve), BindingFlags.Instance | BindingFlags.Public) },
            { 'T', typeof(SvgPath).GetMethod(nameof(SvgPath.AddSmoothQuadraticBezierCurve), BindingFlags.Instance | BindingFlags.Public) },
            { 'A', typeof(SvgPath).GetMethod(nameof(SvgPath.AddArc), BindingFlags.Instance | BindingFlags.Public) },
        };

        public static SvgPath FluentFromStr(string path, bool unarc = false)
        {
            var parsed = Parser.Parse(path);
            var svgPath = new SvgPath();

            foreach (var kvp in parsed)
            {
                var type = kvp.Item1;
                var ut = char.ToUpper(type);
                var method = _methods[ut];
                List<object> @params = new List<object>();

                if (ut == 'Z')
                {

                }
                else
                {
                    @params.AddRange(kvp.Item2.Select(x => (object)x));
                    if (ut == 'A')
                    {
                        @params[3] = Convert.ToDouble(@params[3]) == 1;
                        @params[4] = Convert.ToDouble(@params[4]) == 1;
                        @params.Add(unarc);
                    }

                    @params.Add(type == ut);
                }

                method.Invoke(svgPath, @params.ToArray());
            }

            return svgPath;
        }
    }
}
