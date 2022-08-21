using Xunit;

namespace SvgPathProperties.UnitTests
{
    public class BezierTests
    {
        [Fact]
        public void TestingLengthQuadratic()
        {
            var curve = new BezierProperties(200, 300, 400, 50, 600, 300, null, null);
            Assert.True(Helpers.InDelta(curve.Length, 487.77, 0.1));
        }

        [Fact]
        public void TestLengthCubic()
        {
            var curve = new BezierProperties(200, 200, 275, 100, 575, 100, 500, 200);
            Assert.True(Helpers.InDelta(curve.Length, 383.44, 0.1));
        }

        [Fact]
        public void TestingGetPointAtLengthQuadratic()
        {
            var curve = new BezierProperties(200, 300, 400, 50, 600, 300, null, null);
            var point = curve.GetPointAtLength(487.77 / 6);
            Assert.True(Helpers.InDelta(point.X, 255.24, 1));
            Assert.True(Helpers.InDelta(point.Y, 240.47, 1));
        }

        [Fact]
        public void TestingGetPointAtLengthCubic()
        {
            var curve = new BezierProperties(200, 200, 275, 100, 575, 100, 500, 200);
            var point = curve.GetPointAtLength(383.44 / 6);
            Assert.True(Helpers.InDelta(point.X, 249.48, 1));
            Assert.True(Helpers.InDelta(point.Y, 160.37, 1));
        }

        [Fact]
        public void TestingGetTangentAtLength()
        {
            var curve = new BezierProperties(200, 200, 275, 100, 575, 100, 500, 200);
        }

        [Fact]
        public void TestingPr16Solution()
        {
            var curve = new BezierProperties(640.48, 1285.21, 642.39, 644.73, 642.39, 644.73, null, null);
            var tangent = curve.GetTangentAtLength(curve.Length / 2);
            Assert.True(Helpers.InDelta(tangent.Y, 0, 1));
            Assert.True(Helpers.InDelta(tangent.X, 0, 1));
        }
    }
}
