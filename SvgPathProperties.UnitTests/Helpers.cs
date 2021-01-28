namespace SvgPathProperties.UnitTests
{
    public static class Helpers
    {
        public static bool InDelta(double actual, double expected, double delta)
        {
            return actual >= expected - delta && actual <= expected + delta;
        }
    }
}
