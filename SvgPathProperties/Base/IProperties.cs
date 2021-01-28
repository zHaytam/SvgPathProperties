namespace SvgPathProperties.Base
{
    public interface IProperties
    {
        double GetTotalLength();
        Point GetPointAtLength(double pos);
        Point GetTangentAtLength(double pos);
        PointProperties GetPropertiesAtLength(double pos);
    }
}
