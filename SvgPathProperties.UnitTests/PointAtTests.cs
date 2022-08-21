using FluentAssertions;
using SvgPathProperties.Base;
using Xunit;

namespace SvgPathProperties.UnitTests
{
    public class PointAtTests
    {
        [Theory]
        [InlineData("M0,50L500,50",
            new double[] { 0, 100, 200, 300, 400, 500 },
            new double[] { 50, 50, 50, 50, 50, 50 })]
        [InlineData("M0,50L300,300",
            new double[] { 0, 59.999996185302734, 119.99999237060547, 180, 239.99998474121094, 300 },
            new double[] { 50, 100, 150, 200, 249.99998474121094, 300 })]
        [InlineData("M0,50H300",
            new double[] { 0, 50, 100, 150, 200, 250, 300 },
            new double[] { 50, 50, 50, 50, 50, 50, 50 })]
        [InlineData("M50,50h300",
            new double[] { 50, 100, 150, 200, 250, 300, 350 },
            new double[] { 50, 50, 50, 50, 50, 50, 50 })]
        [InlineData("M50,0V200",
            new double[] { 50, 50, 50, 50, 50, 50, 50 },
            new double[] { 0, 33.33333206176758, 66.66666412353516, 100, 133.3333282470703, 166.6666717529297, 200 })]
        [InlineData("M50,10v200",
            new double[] { 50, 50, 50, 50, 50, 50, 50 },
            new double[] { 10, 43.33333206176758, 76.66666412353516, 110, 143.3333282470703, 176.6666717529297, 210 })]
        [InlineData("M50,50H300V200H50Z",
            new double[] { 50, 183.3333282470703, 300, 300, 166.66668701171875, 50, 50 },
            new double[] { 50, 50, 66.66665649414062, 200, 200, 183.33331298828125, 50 })]
        public void GetPointAtLengthTestingLineTo(string path, double[] xValues, double[] yValues)
        {
            var properties = new SVGPathProperties(path);
            for (var j = 0; j < xValues.Length; j++)
            {
                var position = properties.GetPointAtLength((j * properties.Length) / (xValues.Length - 1));
                Assert.True(Helpers.InDelta(position.X, xValues[j], 0.1));
                Assert.True(Helpers.InDelta(position.Y, yValues[j], 0.1));
            }

            properties.GetPointAtLength(10000000).Should().BeEquivalentTo(properties.GetPointAtLength(properties.Length));
            properties.GetPointAtLength(-1).Should().BeEquivalentTo(properties.GetPointAtLength(0));
        }

        [Theory]
        [InlineData("M200,200 C275,100 575,100 500,200",
            new double[] { 200, 249.48426818847656, 309.1169738769531, 371.97515869140625, 435.7851257324219, 496.41815185546875, 500.0001220703125 },
            new double[] { 200, 160.3770294189453, 137.765380859375, 126.64154052734375, 126.40363311767578, 144.5059051513672, 199.99981689453125 })]
        [InlineData("M100,200 C100,100 250,100 250,200 S400,300 400,200",
            new double[] { 100, 136.8885955810547, 213.11134338378906, 250, 286.88836669921875, 363.11114501953125, 400 },
            new double[] { 200, 134.37181091308594, 134.3717498779297, 199.99984741210938, 265.6280517578125, 265.62835693359375, 200 })]
        [InlineData("M100,200 S400,300 400,200",
            new double[] { 100, 152.38723754882812, 205.42906188964844, 259.1198425292969, 313.48455810546875, 367.6199951171875, 400 },
            new double[] { 200, 215.58023071289062, 228.76190185546875, 238.95660400390625, 244.3085174560547, 238.78338623046875, 200 })]
        [InlineData("M240,100C290,100,240,225,290,200S290,75,340,50S515,100,390,150S215,200,90,150S90,25,140,50S140,175,190,200S190,100,240,100",
            new double[] { 240, 315.0015563964844, 441.4165954589844, 240.0000762939453, 38.58317947387695, 164.99853515625, 240 },
            new double[] { 100, 121.3836898803711, 111.11810302734375, 187.49990844726562, 111.11775207519531, 121.38365936279297, 100 })]
        [InlineData("m240,100c50,0,0,125,50,100s0,-125,50,-150s175,50,50,100s-175,50,-300,0s0,-125,50,-100s0,125,50,150s0,-100,50,-100",
            new double[] { 240, 315.0015563964844, 441.4165954589844, 240.0000762939453, 38.58317947387695, 164.99853515625, 240 },
            new double[] { 100, 121.3836898803711, 111.11810302734375, 187.49990844726562, 111.11775207519531, 121.38365936279297, 100 })]
        public void GetPointAtLengthTestingQuadraticBézier(string path, double[] xValues, double[] yValues)
        {
            var properties = new SVGPathProperties(path);
            for (var j = 0; j < xValues.Length; j++)
            {
                var position = properties.GetPointAtLength((j * properties.Length) / (xValues.Length - 1));
                Assert.True(Helpers.InDelta(position.X, xValues[j], 1));
                Assert.True(Helpers.InDelta(position.Y, yValues[j], 1));
            }

            properties.GetPointAtLength(10000000).Should().BeEquivalentTo(properties.GetPointAtLength(properties.Length));
            properties.GetPointAtLength(-1).Should().BeEquivalentTo(properties.GetPointAtLength(0));
        }

        [Fact]
        public void GetPointAtLengthBugTesting()
        {
            var properties = new SVGPathProperties("M 211.6687111164928,312.6478542077994 C 211.6687111164928,312.6478542077994 211.6687111164928,312.6478542077994 219,293");
            var pos1 = properties.GetPointAtLength(12);
            var pos2 = properties.GetPointAtLength(11.95);
            var pos3 = properties.GetPointAtLength(12.05);
            Assert.True(Helpers.InDelta(pos1.X, pos2.X, 0.1));
            Assert.True(Helpers.InDelta(pos1.X, pos3.X, 0.1));
            Assert.True(Helpers.InDelta(pos1.Y, pos2.Y, 0.1));
            Assert.True(Helpers.InDelta(pos1.Y, pos3.Y, 0.1));
        }

        [Fact]
        public void TestingGetPointAtLengthWithStraighLineBezierCurve()
        {
            var pathData = new SVGPathProperties("M500,300Q425,325 350,350");
            var pathLen = pathData.Length;
            Assert.True(Helpers.InDelta(pathLen, 158.11, 0.1)); //Gave undefined

            var pos = pathData.GetPointAtLength(0);
            Assert.True(Helpers.InDelta(pos.X, 500, 0.00001));
            Assert.True(Helpers.InDelta(pos.Y, 300, 0.00001));
            pos = pathData.GetPointAtLength(pathLen);
            Assert.True(Helpers.InDelta(pos.X, 350, 0.00001));
            Assert.True(Helpers.InDelta(pos.Y, 350, 0.00001));
        }

        [Fact]
        public void TestingWithMultipleRings()
        {
            var properties = new SVGPathProperties("M100,100h100v100h-100Z m200,0h1v1h-1z");
            properties.GetPointAtLength(0).Should().BeEquivalentTo(new Point(100, 100));
            properties.GetPointAtLength(401).Should().BeEquivalentTo(new Point(301, 100));

            properties = new SVGPathProperties("M100,100L200,100 M300,100L400,100");
            properties.GetPointAtLength(0).Should().BeEquivalentTo(new Point(100, 100));
            properties.GetPointAtLength(100).Should().BeEquivalentTo(new Point(200, 100));
            properties.GetPointAtLength(200).Should().BeEquivalentTo(new Point(400, 100));
            properties.GetPointAtLength(200).Should().BeEquivalentTo(properties.GetPointAtLength(500));

            properties = new SVGPathProperties("M100,100 L101,100 M200,0 M500,600 M0,0L1,0L1,1L0,1Z");
            properties.GetPointAtLength(0).Should().BeEquivalentTo(new Point(100, 100));
            properties.GetPointAtLength(1).Should().BeEquivalentTo(new Point(101, 100));
            properties.GetPointAtLength(2).Should().BeEquivalentTo(new Point(1, 0));
        }

        [Fact]
        public void TestingIssue9()
        {
            var properties = new SVGPathProperties("M60,20Q60,20 150,20");
            properties.GetPointAtLength(2).Should().BeEquivalentTo(new Point(62, 20));
            properties.Length.Should().Be(90);

            properties = new SVGPathProperties("M60,20q0,0 90,0");
            properties.GetPointAtLength(2).Should().BeEquivalentTo(new Point(62, 20));
            properties.Length.Should().Be(90);
        }

        [Fact]
        public void CheckNullPathIssue35()
        {
            var properties = new SVGPathProperties("M0, 0");
            properties.GetPointAtLength(0).Should().BeEquivalentTo(new Point(0, 0));
            properties.GetTangentAtLength(0).Should().BeEquivalentTo(new Point(0, 0));
            properties.GetPropertiesAtLength(0).Should().BeEquivalentTo(new PointProperties(0, 0, 0, 0));
            properties = new SVGPathProperties(null);
            properties.GetPointAtLength(0).Should().BeEquivalentTo(new Point(0, 0));
            properties.GetTangentAtLength(0).Should().BeEquivalentTo(new Point(0, 0));
            properties.GetPropertiesAtLength(0).Should().BeEquivalentTo(new PointProperties(0, 0, 0, 0));
            properties = new SVGPathProperties("");
            properties.GetPointAtLength(0).Should().BeEquivalentTo(new Point(0, 0));
            properties.GetTangentAtLength(0).Should().BeEquivalentTo(new Point(0, 0));
            properties.GetPropertiesAtLength(0).Should().BeEquivalentTo(new PointProperties(0, 0, 0, 0));
            properties = new SVGPathProperties("M10, 10");
            properties.GetPointAtLength(0).Should().BeEquivalentTo(new Point(10, 10));
            properties.GetTangentAtLength(0).Should().BeEquivalentTo(new Point(0, 0));
            properties.GetPropertiesAtLength(0).Should().BeEquivalentTo(new PointProperties(10, 10, 0, 0));
        }

        [Fact]
        public void TestingDenegeratedQuadraticCurvesIssue43()
        {
            var properties = new SVGPathProperties(
                "M224,32C153,195,69,366,76,544C77,567,97,585,105,606C133,683,137,768,175,840C193,875,225,902,250,932"
            );
            properties.GetPointAtLength(300).Should().BeEquivalentTo(new Point(111.7377391058728, 310.0179550672576));
            properties.GetTangentAtLength(300).Should().BeEquivalentTo(new Point(-0.29770950875462776, 0.9546565080682572));
            properties.GetPropertiesAtLength(300).Should().BeEquivalentTo(new PointProperties(111.7377391058728,
                310.0179550672576, -0.29770950875462776, 0.9546565080682572));
        }

        [Fact]
        public void TestingFirstPointOfZeroLengthFraction()
        {
            var properties = new SVGPathProperties("M 0,0 l 0 0 l 10 10");
            properties.GetPointAtLength(0).Should().BeEquivalentTo(new Point(0, 0));

            properties = new SVGPathProperties("M 1,1 l 1 1");
            properties.GetPointAtLength(0).Should().BeEquivalentTo(new Point(1, 1));
        }
    }
}
