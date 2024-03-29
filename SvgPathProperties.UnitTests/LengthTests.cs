﻿using Xunit;

namespace SvgPathProperties.UnitTests
{
    public class LengthTests
    {
        [Fact]
        public void TestingLinearPositionDirectly()
        {
            var properties = new LineCommand(0, 0, 10, 0);
            Assert.Equal(10, properties.Length);
        }

        [Fact]
        public void TestingTheLineTo()
        {
            var properties = new SvgPath("m0,0l10,0");
            Assert.Equal(10, properties.Length);

            properties = new SvgPath("M0,0L10,0");
            Assert.Equal(10, properties.Length);

            properties = new SvgPath("M0,0L10,0M0,0L10,0");
            Assert.Equal(20, properties.Length);

            properties = new SvgPath("M0,0L10,0m0,0L10,0");
            Assert.Equal(10, properties.Length);

            properties = new SvgPath("M0,0L10,0l10,0");
            Assert.Equal(20, properties.Length);
        }

        [Fact]
        public void TestingHAndV()
        {
            var properties = new SvgPath("m0,0h10");
            Assert.Equal(10, properties.Length);

            properties = new SvgPath("M50, 0H40");
            Assert.Equal(10, properties.Length);

            properties = new SvgPath("m0,0v10");
            Assert.Equal(10, properties.Length);

            properties = new SvgPath("M0,50V40");
            Assert.Equal(10, properties.Length);
        }

        [Fact]
        public void TestingZ()
        {
            var properties = new SvgPath("m0,0h10z");
            Assert.Equal(20, properties.Length);

            properties = new SvgPath("m0,0h10Z");
            Assert.Equal(20, properties.Length);
        }

        [Fact]
        public void TestingCubicBezier()
        {
            var properties = new SvgPath("M100, 25C10, 90, 110, 100, 150, 195");
            Assert.True(Helpers.InDelta(properties.Length, 213.8, 0.1));

            properties = new SvgPath("m100,25c-90,65,10,75,50,170");
            Assert.True(Helpers.InDelta(properties.Length, 213.8, 0.1));

            properties = new SvgPath("M100,200 C100,100 250,100 250,200 S400,300 400,200");
            Assert.True(Helpers.InDelta(properties.Length, 475.746, 0.1));

            properties = new SvgPath("M100,200 c0,-100 150,-100 150,0 s150,100 150,0");
            Assert.True(Helpers.InDelta(properties.Length, 475.746, 0.1));

            properties = new SvgPath("M100,200 S400,300 400,200");
            Assert.True(Helpers.InDelta(properties.Length, 327.9618, 0.1));

            properties = new SvgPath("M100,200 s300,100 300,0");
            Assert.True(Helpers.InDelta(properties.Length, 327.9618, 0.1));
        }

        [Fact]
        public void TestingQuadraticBezier()
        {
            var properties = new SvgPath("M200,300 Q400,50 600,300");
            Assert.True(Helpers.InDelta(properties.Length, 487.77, 0.1));

            properties = new SvgPath("M200,300 q200,-250 400,0");
            Assert.True(Helpers.InDelta(properties.Length, 487.77, 0.1));

            properties = new SvgPath("M0,100 Q50,-50 100,100 T200,100");
            Assert.True(Helpers.InDelta(properties.Length, 376.84, 0.1));

            properties = new SvgPath("M0,100 q50,-150 100,0 t100,0");
            Assert.True(Helpers.InDelta(properties.Length, 376.84, 0.1));

            properties = new SvgPath("M0,100 Q50,-50 100,100 T200,100 T300,100");
            Assert.True(Helpers.InDelta(properties.Length, 565.26, 0.1));

            properties = new SvgPath("M0, 100 T200, 100");
            Assert.True(Helpers.InDelta(properties.Length, 200, 0.1));

            properties = new SvgPath("M0,100 t200,100");
            Assert.True(Helpers.InDelta(properties.Length, 223.606, 0.1));
        }

        [Fact]
        public void TestingArcs()
        {
            var properties = new SvgPath("M50,20A50,50,0,0,0,150,20");
            Assert.True(Helpers.InDelta(properties.Length, 157.1, 0.1));

            properties = new SvgPath("M50,20A50,50,0,0,0,150,20Z");
            Assert.True(Helpers.InDelta(properties.Length, 257.1, 0.1));

            properties = new SvgPath("M50,20a50,50,0,0,0,100,0");
            Assert.True(Helpers.InDelta(properties.Length, 157.1, 0.1));
        }

        [Fact]
        public void SomeComplexExamples()
        {
            var properties = new SvgPath("M137.69692698614858,194.75002119995685L140.5811864522362,200.02784443179866L145.21300688556522,205.5730786360974L151.96589957664872,210.57916233863872L157.11811791245674,216.958427402148L160.38007797705498,217.5517159659712L170.86150068075614,226.50677931755828L184.78753673995035,229.40372164152683L188.48682846625186,231.74464203758626L194.96220985606624,232.24831761753774L199.0151340580992,235.98908347947008L200.33619274822317,239.1501414459547L208.1352797340722,240.97174662891314L214.55451361971706,243.72269753526453L217.92992784370034,242.79750552259512L222.422382828094,245.95312239185364L226.33834281296274,246.6562900586742L232.1785094475572,250.37579609444018L247.67126011118384,253.41216989328635L249.86860925383274,259.67235659237457L258.0102758151366,263.53584756964034L265.7094539012957,271.9301187141604L275.3442092382522,280.797134878233L292.5367640425162,281.439215857073L300.3900165167456,283.19277126134665L317.1541418598862,288.08140107614616L325.68746219694265,282.98731281377525L334.20900545032936,279.42687578910136L341.89090086141164,279.65662234387565L344.6975683081848,280.71420717321774L352.73368224017975,278.81635544720564L357.8378453664788,280.8621873013037L360.27780217558785,280.351713437805L366.10835670115375,282.6140677325477L369.09298803246423,282.32880268111796L376.79699044083907,278.5755589629451L382.0884404158815,278.74374570898004L386.6969703376813,280.7868194847831L391.5118882394122,287.6851129793625L401.6043570144851,289.4523241399227L418.32264375071753,303.60974325767233L416.56748832810626,308.8321991418072L421.85304030224415,309.8073672357337L426.9233662531078,306.30064325383734L428.39794675453993,303.9729502861741L433.7178516894217,301.12745610964237L435.55518815288303,303.2790040699963L429.98849506106274,310.0981677440247L430.3920258191735,315.904266873991L431.8697365975619,320.41310652120495L431.51963155330213,325.7229788905284L437.6672507546333,329.58621381302714L437.3918696288182,334.8637567665635L439.98603260092784,334.44629338092415L446.1764597142119,341.8547790472293L453.6668527230894,346.9381545890387L457.5294853076264,347.9669234517022L462.48118856871827,352.94569484976665L466.87142760911547,353.62325409732335L470.1647323309724,356.65500849656917L478.52329558789495,361.73028232300277L486.88560554821527,370.7823973990582L489.73056770534674,376.3046557640006L489.2413765676388,379.0217789927731L492.6796339000674,384.9123226146289L500.3373626256565,376.6596349946864L507.84942333888387,380.4063594074064L511.8061547036337,380.01502900094323");
            Assert.True(Helpers.InDelta(properties.Length, 498.031, 0.1));

            properties = new SvgPath("M240,100C290,100,240,225,290,200S290,75,340,50S515,100,390,150S215,200,90,150S90,25,140,50S140,175,190,200S190,100,240,100");
            Assert.True(Helpers.InDelta(properties.Length, 1329.45, 0.1));

            properties = new SvgPath("m240,100c50,0,0,125,50,100s0,-125,50,-150s175,50,50,100s-175,50,-300,0s0,-125,50,-100s0,125,50,150s0,-100,50,-100");
            Assert.True(Helpers.InDelta(properties.Length, 1329.45, 0.1));

            properties = new SvgPath("M100,100h100v100h-100Z m200,0h1v1h-1z");
            Assert.True(Helpers.InDelta(properties.Length, 404, 0.1));

            properties = new SvgPath("M470,623Q468,627,467,629");
            Assert.True(Helpers.InDelta(properties.Length, 6.71, 0.1));

            properties = new SvgPath("M 408 666 Q 408 569 276 440 Q 270 435 270 429 Q 269 427 270 426 Q 271 426 274 426 Q 347 445 443 578 Q 456 598 466 609 Q 472 616 470 623 Q 468 627 467 629");
            Assert.True(Helpers.InDelta(properties.Length, 580, 0.1));

            properties = new SvgPath("M0,0L31.081620209059235,726.1062992125984Q41.44216027874565,726.1062992125984,41.44216027874565,726.1062992125984");
            Assert.True(Helpers.InDelta(properties.Length, 737.13, 0.1));

            properties = new SvgPath("M270 830 Q270 830 270 830");
            Assert.True(Helpers.InDelta(properties.Length, 0, 0.1));
        }

        [Fact]
        public void TestingTheLineTo_Fluent()
        {
            var properties = SvgPathUtils.FluentFromStr("m0,0l10,0");
            Assert.Equal(10, properties.Length);

            properties = SvgPathUtils.FluentFromStr("M0,0L10,0");
            Assert.Equal(10, properties.Length);

            properties = SvgPathUtils.FluentFromStr("M0,0L10,0M0,0L10,0");
            Assert.Equal(20, properties.Length);

            properties = SvgPathUtils.FluentFromStr("M0,0L10,0m0,0L10,0");
            Assert.Equal(10, properties.Length);

            properties = SvgPathUtils.FluentFromStr("M0,0L10,0l10,0");
            Assert.Equal(20, properties.Length);
        }

        [Fact]
        public void TestingHAndV_Fluent()
        {
            var properties = SvgPathUtils.FluentFromStr("m0,0h10");
            Assert.Equal(10, properties.Length);

            properties = SvgPathUtils.FluentFromStr("M50, 0H40");
            Assert.Equal(10, properties.Length);

            properties = SvgPathUtils.FluentFromStr("m0,0v10");
            Assert.Equal(10, properties.Length);

            properties = SvgPathUtils.FluentFromStr("M0,50V40");
            Assert.Equal(10, properties.Length);
        }

        [Fact]
        public void TestingZ_Fluent()
        {
            var properties = SvgPathUtils.FluentFromStr("m0,0h10z");
            Assert.Equal(20, properties.Length);

            properties = SvgPathUtils.FluentFromStr("m0,0h10Z");
            Assert.Equal(20, properties.Length);
        }

        [Fact]
        public void TestingCubicBezier_Fluent()
        {
            var properties = SvgPathUtils.FluentFromStr("M100, 25C10, 90, 110, 100, 150, 195");
            Assert.True(Helpers.InDelta(properties.Length, 213.8, 0.1));

            properties = SvgPathUtils.FluentFromStr("m100,25c-90,65,10,75,50,170");
            Assert.True(Helpers.InDelta(properties.Length, 213.8, 0.1));

            properties = SvgPathUtils.FluentFromStr("M100,200 C100,100 250,100 250,200 S400,300 400,200");
            Assert.True(Helpers.InDelta(properties.Length, 475.746, 0.1));

            properties = SvgPathUtils.FluentFromStr("M100,200 c0,-100 150,-100 150,0 s150,100 150,0");
            Assert.True(Helpers.InDelta(properties.Length, 475.746, 0.1));

            properties = SvgPathUtils.FluentFromStr("M100,200 S400,300 400,200");
            Assert.True(Helpers.InDelta(properties.Length, 327.9618, 0.1));

            properties = SvgPathUtils.FluentFromStr("M100,200 s300,100 300,0");
            Assert.True(Helpers.InDelta(properties.Length, 327.9618, 0.1));
        }

        [Fact]
        public void TestingQuadraticBezier_Fluent()
        {
            var properties = SvgPathUtils.FluentFromStr("M200,300 Q400,50 600,300");
            Assert.True(Helpers.InDelta(properties.Length, 487.77, 0.1));

            properties = SvgPathUtils.FluentFromStr("M200,300 q200,-250 400,0");
            Assert.True(Helpers.InDelta(properties.Length, 487.77, 0.1));

            properties = SvgPathUtils.FluentFromStr("M0,100 Q50,-50 100,100 T200,100");
            Assert.True(Helpers.InDelta(properties.Length, 376.84, 0.1));

            properties = SvgPathUtils.FluentFromStr("M0,100 q50,-150 100,0 t100,0");
            Assert.True(Helpers.InDelta(properties.Length, 376.84, 0.1));

            properties = SvgPathUtils.FluentFromStr("M0,100 Q50,-50 100,100 T200,100 T300,100");
            Assert.True(Helpers.InDelta(properties.Length, 565.26, 0.1));

            properties = SvgPathUtils.FluentFromStr("M0, 100 T200, 100");
            Assert.True(Helpers.InDelta(properties.Length, 200, 0.1));

            properties = SvgPathUtils.FluentFromStr("M0,100 t200,100");
            Assert.True(Helpers.InDelta(properties.Length, 223.606, 0.1));
        }

        [Fact]
        public void TestingArcs_Fluent()
        {
            var properties = SvgPathUtils.FluentFromStr("M50,20A50,50,0,0,0,150,20");
            Assert.True(Helpers.InDelta(properties.Length, 157.1, 0.1));

            properties = SvgPathUtils.FluentFromStr("M50,20A50,50,0,0,0,150,20Z");
            Assert.True(Helpers.InDelta(properties.Length, 257.1, 0.1));

            properties = SvgPathUtils.FluentFromStr("M50,20a50,50,0,0,0,100,0");
            Assert.True(Helpers.InDelta(properties.Length, 157.1, 0.1));
        }

        [Fact]
        public void SomeComplexExamples_Fluent()
        {
            var properties = SvgPathUtils.FluentFromStr("M137.69692698614858,194.75002119995685L140.5811864522362,200.02784443179866L145.21300688556522,205.5730786360974L151.96589957664872,210.57916233863872L157.11811791245674,216.958427402148L160.38007797705498,217.5517159659712L170.86150068075614,226.50677931755828L184.78753673995035,229.40372164152683L188.48682846625186,231.74464203758626L194.96220985606624,232.24831761753774L199.0151340580992,235.98908347947008L200.33619274822317,239.1501414459547L208.1352797340722,240.97174662891314L214.55451361971706,243.72269753526453L217.92992784370034,242.79750552259512L222.422382828094,245.95312239185364L226.33834281296274,246.6562900586742L232.1785094475572,250.37579609444018L247.67126011118384,253.41216989328635L249.86860925383274,259.67235659237457L258.0102758151366,263.53584756964034L265.7094539012957,271.9301187141604L275.3442092382522,280.797134878233L292.5367640425162,281.439215857073L300.3900165167456,283.19277126134665L317.1541418598862,288.08140107614616L325.68746219694265,282.98731281377525L334.20900545032936,279.42687578910136L341.89090086141164,279.65662234387565L344.6975683081848,280.71420717321774L352.73368224017975,278.81635544720564L357.8378453664788,280.8621873013037L360.27780217558785,280.351713437805L366.10835670115375,282.6140677325477L369.09298803246423,282.32880268111796L376.79699044083907,278.5755589629451L382.0884404158815,278.74374570898004L386.6969703376813,280.7868194847831L391.5118882394122,287.6851129793625L401.6043570144851,289.4523241399227L418.32264375071753,303.60974325767233L416.56748832810626,308.8321991418072L421.85304030224415,309.8073672357337L426.9233662531078,306.30064325383734L428.39794675453993,303.9729502861741L433.7178516894217,301.12745610964237L435.55518815288303,303.2790040699963L429.98849506106274,310.0981677440247L430.3920258191735,315.904266873991L431.8697365975619,320.41310652120495L431.51963155330213,325.7229788905284L437.6672507546333,329.58621381302714L437.3918696288182,334.8637567665635L439.98603260092784,334.44629338092415L446.1764597142119,341.8547790472293L453.6668527230894,346.9381545890387L457.5294853076264,347.9669234517022L462.48118856871827,352.94569484976665L466.87142760911547,353.62325409732335L470.1647323309724,356.65500849656917L478.52329558789495,361.73028232300277L486.88560554821527,370.7823973990582L489.73056770534674,376.3046557640006L489.2413765676388,379.0217789927731L492.6796339000674,384.9123226146289L500.3373626256565,376.6596349946864L507.84942333888387,380.4063594074064L511.8061547036337,380.01502900094323");
            Assert.True(Helpers.InDelta(properties.Length, 498.031, 0.1));

            properties = SvgPathUtils.FluentFromStr("M240,100C290,100,240,225,290,200S290,75,340,50S515,100,390,150S215,200,90,150S90,25,140,50S140,175,190,200S190,100,240,100");
            Assert.True(Helpers.InDelta(properties.Length, 1329.45, 0.1));

            properties = SvgPathUtils.FluentFromStr("m240,100c50,0,0,125,50,100s0,-125,50,-150s175,50,50,100s-175,50,-300,0s0,-125,50,-100s0,125,50,150s0,-100,50,-100");
            Assert.True(Helpers.InDelta(properties.Length, 1329.45, 0.1));

            properties = SvgPathUtils.FluentFromStr("M100,100h100v100h-100Z m200,0h1v1h-1z");
            Assert.True(Helpers.InDelta(properties.Length, 404, 0.1));

            properties = SvgPathUtils.FluentFromStr("M470,623Q468,627,467,629");
            Assert.True(Helpers.InDelta(properties.Length, 6.71, 0.1));

            properties = SvgPathUtils.FluentFromStr("M 408 666 Q 408 569 276 440 Q 270 435 270 429 Q 269 427 270 426 Q 271 426 274 426 Q 347 445 443 578 Q 456 598 466 609 Q 472 616 470 623 Q 468 627 467 629");
            Assert.True(Helpers.InDelta(properties.Length, 580, 0.1));

            properties = SvgPathUtils.FluentFromStr("M0,0L31.081620209059235,726.1062992125984Q41.44216027874565,726.1062992125984,41.44216027874565,726.1062992125984");
            Assert.True(Helpers.InDelta(properties.Length, 737.13, 0.1));

            properties = SvgPathUtils.FluentFromStr("M270 830 Q270 830 270 830");
            Assert.True(Helpers.InDelta(properties.Length, 0, 0.1));
        }
    }
}
