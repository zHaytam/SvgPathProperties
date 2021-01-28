namespace SvgPathProperties.Base
{
    public class PartProperties
    {
        public Point Start { get; }
        public Point End { get; }
        public double Length { get; }
        public IProperties Properties { get; }

        public PartProperties(Point start, Point end, double length, IProperties properties)
        {
            Start = start;
            End = end;
            Length = length;
            Properties = properties;
        }
    }
}
