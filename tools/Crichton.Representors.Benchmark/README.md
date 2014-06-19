### Crichton.Representors.Benchmark

Crichton.Representors.Benchmark is a console application for benchmarking HalSerializer.DeserializeToNewBuilder().

Expected usage:

    Crichton.Representors.Benchmark.exe deserializer <options>
    <options> available:
    -f, --filepath=FILEPATH    The FILEPATH of json to serialize.
    -i, --iterations=TIMES     The number of TIMES to repeat the benchmark. Default value is 5

Sample command:
```
    Crichton.Representors.Benchmark.exe deserializer --filepath "tests\Crichton.Representors.Tests\Integration\TestData\Hal\AllLinkObjectProperties.json"
```

You can find a sample batch file(run.bat) in the project root.