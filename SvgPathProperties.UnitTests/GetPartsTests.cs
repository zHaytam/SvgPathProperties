using FluentAssertions;
using SvgPathProperties.Base;
using Xunit;

namespace SvgPathProperties.UnitTests
{
    public class GetPartsTests
    {
        [Fact]
        public void TestingThGetPartsWithSimplePath()
        {
            var properties = new SvgPath("m10,0l10,0");
            var parts = properties.GetParts();

            Assert.Single(parts);

            properties = new SvgPath("m10,0l10,0l10,0");
            parts = properties.GetParts();

            parts[0].Start.Should().BeEquivalentTo(new Point(10, 0));
            parts[1].Start.Should().BeEquivalentTo(new Point(20, 0));

            parts[0].End.Should().BeEquivalentTo(new Point(20, 0));
            parts[1].End.Should().BeEquivalentTo(new Point(30, 0));

            parts[0].Length.Should().Be(10);
            parts[1].Length.Should().Be(10);

            parts[0].Properties.GetPointAtLength(5).Should().BeEquivalentTo(new Point(15, 0));
            parts[1].Properties.GetPointAtLength(5).Should().BeEquivalentTo(new Point(25, 0));

            parts[0].Properties.GetTangentAtLength(5).Should().BeEquivalentTo(new Point(1, 0));
            parts[1].Properties.GetTangentAtLength(5).Should().BeEquivalentTo(new Point(1, 0));

            parts[0].Properties.GetPropertiesAtLength(5).Should().BeEquivalentTo(new PointProperties(15, 0, 1, 0));
            parts[1].Properties.GetPropertiesAtLength(5).Should().BeEquivalentTo(new PointProperties(25, 0, 1, 0));
        }

        [Fact]
        public void TestingTheGetPartsWithSimplePath()
        {
            var properties = new SvgPath("M100,200 C100,100 250,100 250,200 S400,300 400,200");
            var parts = properties.GetParts();

            Assert.Equal(2, parts.Count);

            parts[0].Properties.GetPointAtLength(5).Should().BeEquivalentTo(properties.GetPointAtLength(5));
            parts[1].Properties.GetPointAtLength(5).Should().BeEquivalentTo(properties.GetPointAtLength(parts[0].Length + 5));
        }

        [Fact]
        public void Issue15()
        {
            var def = "M0,0 c 0.025,-0.052 0.081,-0.1387 0.2031,-0.2598 0,0 0,0 0,0";
            var properties = new SvgPath(def);
            properties.GetParts(); //The above path used to hang the programd

            def ="M0,0 c 0.025,-0.052 0.081,-0.1387 0.2031,-0.2598 0,0 0,0 0,0 c 0.1865,-0.31055 0.3632,-0.71289 0.5371,-1.22266 0.1963,-0.40625 0.3261,-0.78516 0.3857,-1.13184 0,-0.008 0,-0.0156 0,-0.0225";
            properties = new SvgPath(def);
            properties.GetParts();
        }
    }
}
