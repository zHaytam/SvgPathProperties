# SvgPathProperties

Pure C# SVG path functions (`getTotalLength()` and `getPointAtLength()`). This was mainly created to save some JS Calls in Blazor, but it's a .netstandard 2 library so you can use it anywhere.

**This is a port (code & unit tests) of the JavaScript version: https://github.com/rveciana/svg-path-properties**

| NuGet Package          | Version                                                                                                                      | Download                                                                                                                      |
| ---------------------- | ---------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| SvgPathProperties | [![NuGet](https://img.shields.io/nuget/v/SvgPathProperties.svg)](https://www.nuget.org/packages/SvgPathProperties) | [![Nuget](https://img.shields.io/nuget/dt/SvgPathProperties.svg)](https://www.nuget.org/packages/SvgPathProperties) |

## Usage

```c#
var properties = new SVGPathProperties("M0,100 Q50,-50 100,100 T200,100");
var length = properties.GetTotalLength(); // double
var point = properties.GetPointAtLength(200); // Point
var tangent = properties.GetTangentAtLength(200); // Point
var allProperties = properties.GetPropertiesAtLength(200); // PointProperties
var parts = properties.GetParts(); // List<PartProperties>
```
