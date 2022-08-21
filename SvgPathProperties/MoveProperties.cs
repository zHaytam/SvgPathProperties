using SvgPathProperties.Base;

namespace SvgPathProperties
{
    public class MoveProperties : IProperties
    {
        public MoveProperties(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public double GetTotalLength() => 0.0;

        public Point GetPointAtLength(double pos)
        {
            throw new System.NotImplementedException();
        }

        public Point GetTangentAtLength(double pos)
        {
            throw new System.NotImplementedException();
        }

        public PointProperties GetPropertiesAtLength(double pos)
        {
            throw new System.NotImplementedException();
        }

        public Rect GetBBox() => new Rect(X, Y, X, Y);
    }
}