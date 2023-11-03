// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using MMQ.Benchmark.Cmd;

Console.WriteLine("Hello, World!");
var summary = BenchmarkRunner.Run<SendAndReadDataBenchmark>(new DebugBuildConfig());
