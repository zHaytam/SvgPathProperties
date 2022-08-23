﻿using FluentAssertions;
using Xunit;

namespace SvgPathProperties.UnitTests
{
    public class TangentTests
    {
        [Fact]
        public void GetTangentAtLengthTesting()
        {
            var paths = new dynamic[] {
                new {
                    path = "M0,50L500,50",
                    xValues = new double[] { 1, 1, 1, 1, 1, 1, 1 },
                    yValues = new double[] { 0, 0, 0, 0, 0, 0, 0 }
                },
                new {
                    path = "M0,50L300,300",
                    xValues = new double[] {
                        0.7682212795973759,
                        0.7682212795973759,
                        0.7682212795973759,
                        0.7682212795973759,
                        0.7682212795973759,
                        0.7682212795973759,
                        0.7682212795973759
                    },
                    yValues = new double[] {
                        0.6401843996644798,
                        0.6401843996644798,
                        0.6401843996644798,
                        0.6401843996644798,
                        0.6401843996644798,
                        0.6401843996644798,
                        0.6401843996644798
                    }
                },
                new {
                    path = "M0,50H300",
                    xValues = new double[] { 1, 1, 1, 1, 1, 1, 1 },
                    yValues = new double[] { 0, 0, 0, 0, 0, 0, 0 }
                },
                new {
                    path = "M50,50h300",
                    xValues = new double[] { 1, 1, 1, 1, 1, 1, 1 },
                    yValues = new double[] {0, 0, 0, 0, 0, 0, 0 }
                },
                new {
                    path = "M50,0V200",
                    xValues = new double[] { 0, 0, 0, 0, 0, 0, 0 },
                    yValues = new double[] {1, 1, 1, 1, 1, 1, 1 }
                },
                new {
                    path = "M50,10v200",
                     xValues = new double[] { 0, 0, 0, 0, 0, 0, 0 },
                     yValues = new double[] {1, 1, 1, 1, 1, 1, 1 }
                },
                new {
                    path = "M50,50H300V200H50Z",
                     xValues = new double[] { 1, 1, 0, 0, -1, 0, 0 },
                  yValues = new double[] {0, 0, 1, 1, 0, -1, -1 }
                 },
                  new {
                    path = "M200,300 Q400,50 600,300",
                    xValues = new double[] {
                        0.6246950475544243,
                        0.7418002077808687,
                        0.8984910301093634,
                        0.9999999999999247,
                        0.8984912864152661,
                        0.7418004257503856,
                        0.6246951852934535
                    },
                    yValues = new double[] {
                        -0.7808688094430304,
                        -0.6706209448982786,
                        -0.43899187784401555,
                        -3.8801383766437346e-7,
                        0.4389913532586264,
                        0.6706207037935429,
                        0.7808686992517869
                    }
                },
                new {
                    path = "M0,100 Q50,-50 100,100 T200,100",
                    xValues = new double[] {
                        0.3162277660168379,
                        0.564254701134953,
                        0.5642557779681502,
                        0.3162278830087659,
                        0.5642536243071706,
                        0.5642568548067621,
                        0.3162280000014775
                    },
                    yValues = new double[] {
                        -0.9486832980505138,
                        -0.8256007704981294,
                        0.8256000345382487,
                        0.9486832590531965,
                        0.8256015064522486,
                        -0.8255992985726069,
                        -0.948683220055602
                    }
                },
                new {
                    path = "M0,100 q50,-150 100,0 t100,0",
                    xValues = new double[] {
                        0.3162277660168379,
                        0.564254701134953,
                        0.5642557779681502,
                        0.3162278830087659,
                        0.5642536243071706,
                        0.5642568548067621,
                        0.3162280000014775
                    },
                    yValues = new double[] {
                        -0.9486832980505138,
                        -0.8256007704981294,
                        0.8256000345382487,
                        0.9486832590531965,
                        0.8256015064522486,
                        -0.8255992985726069,
                        -0.948683220055602
                    }
                },
                new {
                    path = "M0,100 T200,100",
                    xValues = new double[] { 1, 1, 1, 1, 1, 1, 1 },
                    yValues = new double[] {0, 0, 0, 0, 0, 0, 0 }
                },
                new {
                    path = "M0,100 Q50,-50 100,100 T200,100 T300,100",
                    xValues = new double[] {
                        0.3162277660168379,
                        0.9999999999982623,
                        0.3162278434645575,
                        0.9999999999843601,
                        0.3162279209126205,
                        0.999999999956556,
                        0.31622799836102694
                    },
                    yValues = new double[] {
                        -0.9486832980505138,
                        -0.000001864275757835856,
                        0.9486832722346038,
                        0.000005592827273145598,
                        -0.9486832464185723,
                        -0.000009321378787369434,
                        0.948683220602419
                    }
                },
                new {
                    path = "M200,200 C275,100 575,100 500,200",
                    xValues = new double[] {
                        0.6,
                        0.8855412510377398,
                        0.9662834030858953,
                        0.9955306269200779,
                        0.9950635163752912,
                        0.8102625798004855,
                        -0.5999966647640088
                    },
                    yValues = new double[] {
                        -0.8,
                        -0.4645607524431165,
                        -0.25748084379374897,
                        -0.09443924430085592,
                        0.09924010468979105,
                        0.5860670198663817,
                        0.8000025014161303
                    }
                },
                new {
                    path = "M100,200 C100,100 250,100 250,200 S400,300 400,200",
                    xValues = new double[] {
                        0,
                        0.8804617844502954,
                        0.8804628147711906,
                        0.0000014914995832878143,
                        0.8804607541243551,
                        0.8804638450870454,
                        0.0000029830040755489834
                    },
                    yValues = new double[] {
                        -1,
                        -0.47411712278993,
                        0.47411520942192104,
                        0.9999999999988878,
                        0.47411903615734735,
                        -0.47411329605331204,
                        -0.9999999999955509
                    }
                },
                new  {
                    path = "M100,200 S400,300 400,200",
                      xValues = new double[] {
                        0,
                        0.9647425496827284,
                        0.9760648482761272,
                        0.9886843239589528,
                        0.9995932816004552,
                        0.953018740113177,
                        2.2070215559197298e-7
                    },
                    yValues = new double[] {
                        0,
                        0.26319538907752243,
                        0.21747968171693818,
                        0.15001102478760836,
                        0.028517913304325987,
                        -0.30291134180332835,
                        -0.9999999999999756
                    }
                },
                new {
                    path = "M240,100C290,100,240,225,290,200S290,75,340,50S515,100,390,150S215,200,90,150S90,25,140,50S140,175,190,200S190,100,240,100",
                    xValues = new double[] {
                        1,
                        0.0011574896425055297,
                        -0.3871202612806902,
                        -0.999999999999897,
                        -0.3871144077881202,
                        0.001157352564866387,
                        1
                    },
                    yValues = new double[] {
                        0,
                        -0.9999993301086394,
                        0.9220292312643728,
                        -4.5391975842908476e-7,
                        -0.9220316888712953,
                        0.999999330267296,
                    -2.2204460492503162e-15
                    }
                },
                new {
                    path = "m240,100c50,0,0,125,50,100s0,-125,50,-150s175,50,50,100s-175,50,-300,0s0,-125,50,-100s0,125,50,150s0,-100,50,-100",
                    xValues = new double[] {
                        1,
                        0.0011574896425055297,
                        -0.3871202612806902,
                        -0.999999999999897,
                        -0.3871144077881202,
                        0.001157352564866387,
                        1
                    },
                    yValues = new double[] {
                        0,
                        -0.9999993301086394,
                        0.9220292312643728,
                        -4.5391975842908476e-7,
                        -0.9220316888712953,
                        0.999999330267296,
                        -2.2204460492503162e-15
                    }
                }
            };

            for (var i = 0; i < paths.Length; i++)
            {
                var properties = new SvgPath(paths[i].path);
                for (var j = 0; j < paths[i].xValues.Length; j++)
                {
                    var tangent = properties.GetTangentAtLength(
                      (j * properties.Length) / (paths[i].xValues.Length - 1)
                    );
                    Assert.True(Helpers.InDelta(tangent.X, paths[i].xValues[j], 0.1));
                    Assert.True(Helpers.InDelta(tangent.Y, paths[i].yValues[j], 0.1));
                }

                properties.GetTangentAtLength(10000000).Should().BeEquivalentTo(properties.GetTangentAtLength(properties.Length));
                properties.GetTangentAtLength(-1).Should().BeEquivalentTo(properties.GetTangentAtLength(0));
            }
        }

        [Fact]
        public void GetTangentAtLengthTesting_Fluent()
        {
            var paths = new dynamic[] {
                new {
                    path = "M0,50L500,50",
                    xValues = new double[] { 1, 1, 1, 1, 1, 1, 1 },
                    yValues = new double[] { 0, 0, 0, 0, 0, 0, 0 }
                },
                new {
                    path = "M0,50L300,300",
                    xValues = new double[] {
                        0.7682212795973759,
                        0.7682212795973759,
                        0.7682212795973759,
                        0.7682212795973759,
                        0.7682212795973759,
                        0.7682212795973759,
                        0.7682212795973759
                    },
                    yValues = new double[] {
                        0.6401843996644798,
                        0.6401843996644798,
                        0.6401843996644798,
                        0.6401843996644798,
                        0.6401843996644798,
                        0.6401843996644798,
                        0.6401843996644798
                    }
                },
                new {
                    path = "M0,50H300",
                    xValues = new double[] { 1, 1, 1, 1, 1, 1, 1 },
                    yValues = new double[] { 0, 0, 0, 0, 0, 0, 0 }
                },
                new {
                    path = "M50,50h300",
                    xValues = new double[] { 1, 1, 1, 1, 1, 1, 1 },
                    yValues = new double[] {0, 0, 0, 0, 0, 0, 0 }
                },
                new {
                    path = "M50,0V200",
                    xValues = new double[] { 0, 0, 0, 0, 0, 0, 0 },
                    yValues = new double[] {1, 1, 1, 1, 1, 1, 1 }
                },
                new {
                    path = "M50,10v200",
                     xValues = new double[] { 0, 0, 0, 0, 0, 0, 0 },
                     yValues = new double[] {1, 1, 1, 1, 1, 1, 1 }
                },
                new {
                    path = "M50,50H300V200H50Z",
                     xValues = new double[] { 1, 1, 0, 0, -1, 0, 0 },
                  yValues = new double[] {0, 0, 1, 1, 0, -1, -1 }
                 },
                  new {
                    path = "M200,300 Q400,50 600,300",
                    xValues = new double[] {
                        0.6246950475544243,
                        0.7418002077808687,
                        0.8984910301093634,
                        0.9999999999999247,
                        0.8984912864152661,
                        0.7418004257503856,
                        0.6246951852934535
                    },
                    yValues = new double[] {
                        -0.7808688094430304,
                        -0.6706209448982786,
                        -0.43899187784401555,
                        -3.8801383766437346e-7,
                        0.4389913532586264,
                        0.6706207037935429,
                        0.7808686992517869
                    }
                },
                new {
                    path = "M0,100 Q50,-50 100,100 T200,100",
                    xValues = new double[] {
                        0.3162277660168379,
                        0.564254701134953,
                        0.5642557779681502,
                        0.3162278830087659,
                        0.5642536243071706,
                        0.5642568548067621,
                        0.3162280000014775
                    },
                    yValues = new double[] {
                        -0.9486832980505138,
                        -0.8256007704981294,
                        0.8256000345382487,
                        0.9486832590531965,
                        0.8256015064522486,
                        -0.8255992985726069,
                        -0.948683220055602
                    }
                },
                new {
                    path = "M0,100 q50,-150 100,0 t100,0",
                    xValues = new double[] {
                        0.3162277660168379,
                        0.564254701134953,
                        0.5642557779681502,
                        0.3162278830087659,
                        0.5642536243071706,
                        0.5642568548067621,
                        0.3162280000014775
                    },
                    yValues = new double[] {
                        -0.9486832980505138,
                        -0.8256007704981294,
                        0.8256000345382487,
                        0.9486832590531965,
                        0.8256015064522486,
                        -0.8255992985726069,
                        -0.948683220055602
                    }
                },
                new {
                    path = "M0,100 T200,100",
                    xValues = new double[] { 1, 1, 1, 1, 1, 1, 1 },
                    yValues = new double[] {0, 0, 0, 0, 0, 0, 0 }
                },
                new {
                    path = "M0,100 Q50,-50 100,100 T200,100 T300,100",
                    xValues = new double[] {
                        0.3162277660168379,
                        0.9999999999982623,
                        0.3162278434645575,
                        0.9999999999843601,
                        0.3162279209126205,
                        0.999999999956556,
                        0.31622799836102694
                    },
                    yValues = new double[] {
                        -0.9486832980505138,
                        -0.000001864275757835856,
                        0.9486832722346038,
                        0.000005592827273145598,
                        -0.9486832464185723,
                        -0.000009321378787369434,
                        0.948683220602419
                    }
                },
                new {
                    path = "M200,200 C275,100 575,100 500,200",
                    xValues = new double[] {
                        0.6,
                        0.8855412510377398,
                        0.9662834030858953,
                        0.9955306269200779,
                        0.9950635163752912,
                        0.8102625798004855,
                        -0.5999966647640088
                    },
                    yValues = new double[] {
                        -0.8,
                        -0.4645607524431165,
                        -0.25748084379374897,
                        -0.09443924430085592,
                        0.09924010468979105,
                        0.5860670198663817,
                        0.8000025014161303
                    }
                },
                new {
                    path = "M100,200 C100,100 250,100 250,200 S400,300 400,200",
                    xValues = new double[] {
                        0,
                        0.8804617844502954,
                        0.8804628147711906,
                        0.0000014914995832878143,
                        0.8804607541243551,
                        0.8804638450870454,
                        0.0000029830040755489834
                    },
                    yValues = new double[] {
                        -1,
                        -0.47411712278993,
                        0.47411520942192104,
                        0.9999999999988878,
                        0.47411903615734735,
                        -0.47411329605331204,
                        -0.9999999999955509
                    }
                },
                new  {
                    path = "M100,200 S400,300 400,200",
                      xValues = new double[] {
                        0,
                        0.9647425496827284,
                        0.9760648482761272,
                        0.9886843239589528,
                        0.9995932816004552,
                        0.953018740113177,
                        2.2070215559197298e-7
                    },
                    yValues = new double[] {
                        0,
                        0.26319538907752243,
                        0.21747968171693818,
                        0.15001102478760836,
                        0.028517913304325987,
                        -0.30291134180332835,
                        -0.9999999999999756
                    }
                },
                new {
                    path = "M240,100C290,100,240,225,290,200S290,75,340,50S515,100,390,150S215,200,90,150S90,25,140,50S140,175,190,200S190,100,240,100",
                    xValues = new double[] {
                        1,
                        0.0011574896425055297,
                        -0.3871202612806902,
                        -0.999999999999897,
                        -0.3871144077881202,
                        0.001157352564866387,
                        1
                    },
                    yValues = new double[] {
                        0,
                        -0.9999993301086394,
                        0.9220292312643728,
                        -4.5391975842908476e-7,
                        -0.9220316888712953,
                        0.999999330267296,
                    -2.2204460492503162e-15
                    }
                },
                new {
                    path = "m240,100c50,0,0,125,50,100s0,-125,50,-150s175,50,50,100s-175,50,-300,0s0,-125,50,-100s0,125,50,150s0,-100,50,-100",
                    xValues = new double[] {
                        1,
                        0.0011574896425055297,
                        -0.3871202612806902,
                        -0.999999999999897,
                        -0.3871144077881202,
                        0.001157352564866387,
                        1
                    },
                    yValues = new double[] {
                        0,
                        -0.9999993301086394,
                        0.9220292312643728,
                        -4.5391975842908476e-7,
                        -0.9220316888712953,
                        0.999999330267296,
                        -2.2204460492503162e-15
                    }
                }
            };

            for (var i = 0; i < paths.Length; i++)
            {
                SvgPath properties = SvgPathUtils.FluentFromStr(paths[i].path);
                for (var j = 0; j < paths[i].xValues.Length; j++)
                {
                    var tangent = properties.GetTangentAtLength(
                      (j * properties.Length) / (paths[i].xValues.Length - 1)
                    );
                    Assert.True(Helpers.InDelta(tangent.X, paths[i].xValues[j], 0.1));
                    Assert.True(Helpers.InDelta(tangent.Y, paths[i].yValues[j], 0.1));
                }

                properties.GetTangentAtLength(10000000).Should().BeEquivalentTo(properties.GetTangentAtLength(properties.Length));
                properties.GetTangentAtLength(-1).Should().BeEquivalentTo(properties.GetTangentAtLength(0));
            }
        }
    }
}
