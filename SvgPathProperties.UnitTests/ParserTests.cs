using System;
using System.Collections.Generic;
using Xunit;

namespace SvgPathProperties.UnitTests
{
    public class ParserTests
    {
        [Fact]
        public void OverloadMoveTo()
        {
            var result = Parser.Parse("m 12.5,52 39,0 0,-40 -39,0 z");
            Assert.Equal(new List<(char, List<double>)>
            {
                ('m', new List<double> { 12.5, 52 }),
                ('l', new List<double> { 39, 0 }),
                ('l', new List<double> { 0, -40 }),
                ('l', new List<double> { -39, 0 }),
                ('z', new List<double> { })
            }, result);
        }

        [Fact]
        public void CurveTo()
        {
            var a = Parser.Parse("c 50,0 50,100 100,100 50,0 50,-100 100,-100");
            var b = Parser.Parse("c 50,0 50,100 100,100 c 50,0 50,-100 100,-100");

            Assert.Equal(new List<(char, List<double>)>
            {
                ('c', new List<double> { 50, 0, 50, 100, 100, 100 }),
                ('c', new List<double> { 50, 0, 50, -100, 100, -100 })
            }, a);

            Assert.Equal(a, b);
        }

        [Fact]
        public void LineTo()
        {
            var ex = Assert.Throws<Exception>(() => Parser.Parse("l 10 10 0"));
            Assert.StartsWith("Malformed", ex.Message);

            Assert.Equal(new List<(char, List<double>)>
            {
                ('l', new List<double> { 10, 10 }),
            }, Parser.Parse("l 10,10"));

            Assert.Equal(new List<(char, List<double>)>
            {
                ('L', new List<double> { 10, 10 }),
            }, Parser.Parse("L 10,10"));

            Assert.Equal(new List<(char, List<double>)>
            {
                ('l', new List<double> { 10, 10 }),
                ('l', new List<double> { 10, 10 }),
            }, Parser.Parse("l10 10 10 10"));

            Assert.Equal(new List<(char, List<double>)>
            {
                ('h', new List<double> { 10.5 }),
            }, Parser.Parse("h 10.5"));

            Assert.Equal(new List<(char, List<double>)>
            {
                ('v', new List<double> { 10.5 }),
            }, Parser.Parse("v 10.5"));
        }

        [Fact]
        public void ArcToQuadraticToSmoothCurveToSmoothQuadraticCurveTo()
        {
            Assert.Equal(new List<(char, List<double>)>
            {
                ('A', new List<double> { 30, 50, 0, 0, 1, 162.55, 162.45 }),
            }, Parser.Parse("A 30 50 0 0 1 162.55 162.45"));

            Assert.Equal(new List<(char, List<double>)>
            {
                ('M', new List<double> { 10, 80 }),
                ('Q', new List<double> { 95, 10, 180, 80 }),
            }, Parser.Parse("M10 80 Q 95 10 180 80"));

            Assert.Equal(new List<(char, List<double>)>
            {
                ('S', new List<double> { 1, 2, 3, 4 }),
            }, Parser.Parse("S 1 2, 3 4"));

            Assert.Equal(new List<(char, List<double>)>
            {
                ('T', new List<double> { 1, -2e2 }),
            }, Parser.Parse("T 1 -2e2"));

            Assert.Throws<Exception>(() => Parser.Parse("t 1 2 3"));
        }

        [Fact]
        public void EmptyOrNull()
        {
            Assert.Equal(new List<(char, List<double>)>
            {
                ('M', new List<double> { 0, 0 }),
            }, Parser.Parse(""));

            Assert.Equal(new List<(char, List<double>)>
            {
                ('M', new List<double> { 0, 0 }),
            }, Parser.Parse(null));
        }
    }
}
