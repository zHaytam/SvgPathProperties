using BenchmarkDotNet.Running;
using SvgPathProperties.Benchmarks;

var summary = BenchmarkRunner.Run<General>();