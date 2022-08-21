using System;
using System.Collections.Generic;
using System.Linq;

namespace SvgPathProperties
{
    // Credit: https://github.com/fontello/svgpath/blob/master/lib/a2c.js
    public static class A2C
    {
        public const double TAU = Math.PI * 2;


        /* eslint-disable space-infix-ops */

        // Calculate an angle between two unit vectors
        //
        // Since we measure angle between radii of circular arcs,
        // we can use simplified math (without length normalization)
        //
        private static double unit_vector_angle(double ux, double uy, double vx, double vy)
        {
            var sign = (ux * vy - uy * vx < 0) ? -1 : 1;
            var dot = ux * vx + uy * vy;

            // Add this to work with arbitrary vectors:
            // dot /= Math.sqrt(ux * ux + uy * uy) * Math.sqrt(vx * vx + vy * vy);

            // rounding errors, e.g. -1.0000000000000002 can screw up this
            if (dot > 1.0)
            {
                dot = 1.0;
            }

            if (dot < -1.0)
            {
                dot = -1.0;
            }

            return sign * Math.Acos(dot);
        }


        // Convert from endpoint to center parameterization,
        // see http://www.w3.org/TR/SVG11/implnote.html#ArcImplementationNotes
        //
        // Return [cx, cy, theta1, delta_theta]
        //
        private static double[] get_arc_center(double x1, double y1, double x2, double y2, double fa, double fs,
            double rx, double ry, double sin_phi, double cos_phi)
        {
            // Step 1.
            //
            // Moving an ellipse so origin will be the middlepoint between our two
            // points. After that, rotate it to line up ellipse axes with coordinate
            // axes.
            //
            var x1p = cos_phi * (x1 - x2) / 2 + sin_phi * (y1 - y2) / 2;
            var y1p = -sin_phi * (x1 - x2) / 2 + cos_phi * (y1 - y2) / 2;

            var rx_sq = rx * rx;
            var ry_sq = ry * ry;
            var x1p_sq = x1p * x1p;
            var y1p_sq = y1p * y1p;

            // Step 2.
            //
            // Compute coordinates of the centre of this ellipse (cx', cy')
            // in the new coordinate system.
            //
            var radicant = (rx_sq * ry_sq) - (rx_sq * y1p_sq) - (ry_sq * x1p_sq);

            if (radicant < 0)
            {
                // due to rounding errors it might be e.g. -1.3877787807814457e-17
                radicant = 0;
            }

            radicant /= (rx_sq * y1p_sq) + (ry_sq * x1p_sq);
            radicant = Math.Sqrt(radicant) * (fa == fs ? -1 : 1);

            var cxp = radicant * rx / ry * y1p;
            var cyp = radicant * -ry / rx * x1p;

            // Step 3.
            //
            // Transform back to get centre coordinates (cx, cy) in the original
            // coordinate system.
            //
            var cx = cos_phi * cxp - sin_phi * cyp + (x1 + x2) / 2;
            var cy = sin_phi * cxp + cos_phi * cyp + (y1 + y2) / 2;

            // Step 4.
            //
            // Compute angles (theta1, delta_theta).
            //
            var v1x = (x1p - cxp) / rx;
            var v1y = (y1p - cyp) / ry;
            var v2x = (-x1p - cxp) / rx;
            var v2y = (-y1p - cyp) / ry;

            var theta1 = unit_vector_angle(1, 0, v1x, v1y);
            var delta_theta = unit_vector_angle(v1x, v1y, v2x, v2y);

            if (fs == 0 && delta_theta > 0)
            {
                delta_theta -= TAU;
            }

            if (fs == 1 && delta_theta < 0)
            {
                delta_theta += TAU;
            }

            return new[] { cx, cy, theta1, delta_theta };
        }

        //
        // Approximate one unit arc segment with bézier curves,
        // see http://math.stackexchange.com/questions/873224
        //
        private static double[] approximate_unit_arc(double theta1, double delta_theta)
        {
            var alpha = 4.0 / 3.0 * Math.Tan(delta_theta / 4.0);

            var x1 = Math.Cos(theta1);
            var y1 = Math.Sin(theta1);
            var x2 = Math.Cos(theta1 + delta_theta);
            var y2 = Math.Sin(theta1 + delta_theta);

            return new[] { x1, y1, x1 - y1 * alpha, y1 + x1 * alpha, x2 + y2 * alpha, y2 - x2 * alpha, x2, y2 };
        }

        public static List<double[]> a2c(double x1, double y1, double x2, double y2, double fa, double fs, double rx,
            double ry, double phi)
        {
            var result = new List<double[]>();
            var sin_phi = Math.Sin(phi * TAU / 360);
            var cos_phi = Math.Cos(phi * TAU / 360);

            // Make sure radii are valid
            //
            var x1p = cos_phi * (x1 - x2) / 2 + sin_phi * (y1 - y2) / 2;
            var y1p = -sin_phi * (x1 - x2) / 2 + cos_phi * (y1 - y2) / 2;

            if (x1p == 0 && y1p == 0)
            {
                // we're asked to draw line to itself
                return result;
            }

            if (rx == 0 || ry == 0)
            {
                // one of the radii is zero
                return result;
            }


            // Compensate out-of-range radii
            //
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);

            var lambda = (x1p * x1p) / (rx * rx) + (y1p * y1p) / (ry * ry);
            if (lambda > 1)
            {
                rx *= Math.Sqrt(lambda);
                ry *= Math.Sqrt(lambda);
            }


            // Get center parameters (cx, cy, theta1, delta_theta)
            //
            var cc = get_arc_center(x1, y1, x2, y2, fa, fs, rx, ry, sin_phi, cos_phi);

            var theta1 = cc[2];
            var delta_theta = cc[3];

            // Split an arc to multiple segments, so each segment
            // will be less than τ/4 (= 90°)
            //
            var segments = Math.Max(Math.Ceiling(Math.Abs(delta_theta) / (TAU / 4)), 1);
            delta_theta /= segments;

            for (var i = 0; i < segments; i++)
            {
                result.Add(approximate_unit_arc(theta1, delta_theta));
                theta1 += delta_theta;
            }

            // We have a bezier approximation of a unit circle,
            // now need to transform back to the original ellipse
            //
            foreach (var curve in result)
            {
                for (var i = 0; i < curve.Length; i += 2)
                {
                    var x = curve[i + 0];
                    var y = curve[i + 1];

                    // scale
                    x *= rx;
                    y *= ry;

                    // rotate
                    var xp = cos_phi * x - sin_phi * y;
                    var yp = sin_phi * x + cos_phi * y;

                    // translate
                    curve[i + 0] = xp + cc[0];
                    curve[i + 1] = yp + cc[1];
                }
            }
            
            return result;
        }
    }
}