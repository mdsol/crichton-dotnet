set MSBuildDirPath=C:\Windows\Microsoft.NET\Framework\v4.0.30319\
@echo off
%MSBuildDirPath%msbuild.exe src\Crichton.Representors\Crichton.Representors.csproj /t:Clean;Rebuild /property:Configuration=Release /clp:ErrorsOnly
%MSBuildDirPath%msbuild.exe tools\Crichton.Representors.Benchmark\Crichton.Representors.Benchmark.csproj /t:Clean;Rebuild /p:Configuration=Release /clp:ErrorsOnly

tools\Crichton.Representors.Benchmark\bin\Release\Crichton.Representors.Benchmark.exe deserializer ^
    --iterations 5 --filepath tests\Crichton.Representors.Tests\Integration\TestData\Hal\AllLinkObjectProperties.json
pause
