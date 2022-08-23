# SvgPathProperties

Pure C# SVG Path parser and calculations.

| NuGet Package          | Version                                                                                                                      | Download                                                                                                                      |
| ---------------------- | ---------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| SvgPathProperties | [![NuGet](https://img.shields.io/nuget/v/SvgPathProperties.svg)](https://www.nuget.org/packages/SvgPathProperties) | [![Nuget](https://img.shields.io/nuget/dt/SvgPathProperties.svg)](https://www.nuget.org/packages/SvgPathProperties) |

## Features

- Parse SVG into class commands (`Segments`)
- Calculate the total length of a path (equivalent to `getTotalLength`)
- Get point at length (equivalent to `getPointAtLength`)
- Calculate bbox of a path (equivalent  to `getBBox`)
- Fluent API to build a path
- Generate string path from the added commands

## Usage

```c#
var properties = new SvgPath("M0,100 Q50,-50 100,100 T200,100");
var length = properties.GetTotalLength(); // double
var point = properties.GetPointAtLength(200); // Point
var tangent = properties.GetTangentAtLength(200); // Point
var allProperties = properties.GetPropertiesAtLength(200); // PointProperties
var parts = properties.GetParts(); // List<PartProperties>
var bbox = properties.GetBBox(); // Rect

new SvgPath("M 15 10 L 35 10")
    .AddVerticalLineTo(15)
    .AddHorizontalLineTo(40)
    .AddCubicBezierCurve(50, 5, 60, 15, 50, 25)
    .AddSmoothCubicBezierCurve(40, 20, 35, 30)
    .AddQuadraticBezierCurve(25, 40, 15, 30)
    .AddSmoothQuadraticBezierCurve(5, 15)
    .AddArc(1, 1, 0, false, false, 5, 5, unarc: true)
    .AddClosePath()
    .ToString();

// M 15 10 L 35 10
// L 35 15
// L 40 15
// C 50 5 60 15 50 25
// C 40 35 40 20 35 30
// Q 25 40 15 30
// Q 5 20 5 15
// C 7.761423749153967 15 10 12.761423749153966 10 10
// C 10 7.238576250846034 7.761423749153967 5 5 5
// Z
```

## Credits

- https://github.com/rveciana/svg-path-properties
- https://github.com/fontello/svgpath
- https://github.com/mondeja/svg-path-bbox
