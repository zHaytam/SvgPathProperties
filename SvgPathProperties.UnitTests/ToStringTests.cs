using System.Globalization;

using Xunit;

namespace SvgPathProperties.UnitTests
{
    public class ToStringTests
    {
        [Theory]
        //with . separator
        [InlineData("en-US", 0, 1.5, 0, 1.5, "L 1.5 1.5")]

        [InlineData("en-US", 0, 1_000_000.5, 0, 1.5, "L 1000000.5 1.5")]
        //with , separator
        [InlineData("de-DE", 0, 1.5, 0, 1.5, "L 1.5 1.5")]
        [InlineData("it-IT", 0, 1.5, 0, 1.5, "L 1.5 1.5")]
        [InlineData("es-ES", 0, 1.5, 0, 1.5, "L 1.5 1.5")]
        [InlineData("nl-BE", 0, 1.5, 0, 1.5, "L 1.5 1.5")]

        [InlineData("de-DE", 0, 1_000_000.5, 0, 1_000_000.5, "L 1000000.5 1000000.5")]
        [InlineData("it-IT", 0, 1_000_000.5, 0, 1_000_000.5, "L 1000000.5 1000000.5")]
        [InlineData("es-ES", 0, 1_000_000.5, 0, 1_000_000.5, "L 1000000.5 1000000.5")]
        [InlineData("nl-BE", 0, 1_000_000.5, 0, 1_000_000.5, "L 1000000.5 1000000.5")]
        // with / separator
        [InlineData("fa-IR", 0, 1.5, 0, 1.5, "L 1.5 1.5")]
        [InlineData("fa-IR", 0, 1_000_000.5, 0, 1_000_000.5, "L 1000000.5 1000000.5")]
        public void CuluresWithPointSeparator(string cultureCode, double fromX, double toX, double fromY, double toY, string result)
        {
            var command = new LineCommand(fromX, toX, fromY, toY);
            CultureInfo.CurrentCulture = new CultureInfo(cultureCode); ;

            Assert.Equal(result, command.ToString());
        }
    }
}
