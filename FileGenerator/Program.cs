using System.Diagnostics;
using FileGenerator;

var sw = Stopwatch.StartNew();
Runner.Run(outputFilePath: args[0], sizeInBytes: int.Parse(args[1]));
Console.WriteLine(sw.Elapsed.TotalSeconds);