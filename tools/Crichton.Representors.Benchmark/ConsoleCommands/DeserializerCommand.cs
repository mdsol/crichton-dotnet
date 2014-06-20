using System;
using System.Diagnostics;
using System.IO;
using Crichton.Representors.Serializers;
using ManyConsole;

namespace Crichton.Representors.Benchmark.ConsoleCommands
{
    namespace Crichton.Representors.Benchmark.ConsoleCommands
    {
        public class DeserializerCommand : ConsoleCommand
        {
            private const int deserializations = 10000;
            private string filePath;
            private int iterations = 5;

            public DeserializerCommand()
            {
                IsCommand("deserializer", "Tests deserializer.");

                HasRequiredOption(
                    "f|filepath=",
                    "The {FILEPATH} of json to serialize",
                    v => filePath = v);
                HasOption(
                    "i|iterations=",
                    "The number of {TIMES} to repeat the benchmark. Default value is 5",
                    v => iterations = int.Parse(v));
            }

            public override int Run(string[] remainingArguments)
            {
                var fileContent = File.ReadAllText(filePath);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    for (int j = 0; j < deserializations; j++)
                    {
                        var serializer = new HalSerializer();
                        var builder = serializer.DeserializeToNewBuilder(fileContent, () => new RepresentorBuilder());
                        builder.ToRepresentor();
                    }
                }
                stopwatch.Stop();

                var totalSeconds = stopwatch.Elapsed.TotalSeconds;
                var averageTotalTimes = totalSeconds / iterations;
                var averageOperationMs = averageTotalTimes * 1000 / deserializations;

                Console.WriteLine("Deserializing {0} complex documents and {1} iterations took {2} seconds.", deserializations, iterations, totalSeconds.ToString("N4"));
                Console.WriteLine("Deserializing {0} complex documents took on average {1} seconds.", deserializations, averageTotalTimes.ToString("N4"));
                Console.WriteLine("It took {0} milliseconds to deserialize each document.", averageOperationMs.ToString("N4"));

                return 0;
            }
        }
    }
}
