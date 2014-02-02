REM Build the project exe
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" "E:\Programming\!Csharp\MinifyLib\minifylib.sln" /property:Configuration=Release

REM Run the unit tests and generate the coverage file
"E:\Programming Enviroments\OpenCover\OpenCover.Console.exe" -register:user "-target:E:\Programming Enviroments\NUnit 2.6.2\bin\nunit-console.exe" -targetargs:"/fixture:MinifyLibTests \"E:\Programming\!Csharp\MinifyLib\Tests\bin\Debug\MinifyLibTests.dll\" /noshadow" "-output:E:\Programming\!Csharp\MinifyLib\Docs\TestResults.xml" -filter:"+[*]*"

REM Generate the coverage report
"E:\Programming Enviroments\ReportGenerator_1.8.0.0\ReportGenerator.exe" "E:\Programming\!Csharp\MinifyLib\Docs\TestResults.xml" "E:\Programming\!Csharp\MinifyLib\Docs\CoverageResult"

REM Generate code metrics
"E:\Programming Enviroments\Microsoft Visual Studio 10.0\Team Tools\Static Analysis Tools\FxCop\metrics.exe" /file:"E:\Programming\!Csharp\MinifyLib\bin\Release\MinifyLib.dll" /out:"E:\Programming\!Csharp\MinifyLib\Docs\CodeMetrics\MetricsResults.xml" /directory:"E:\Programming\!Csharp\MinifyLib\bin\Release" /igc

REM Generate CRAP Analysis
REM "E:\Programming Enviroments\Crap4n\0.3.0.46\crap4n-console.exe" /cc=E:\Programming\!Csharp\MinifyLib\Docs\TestResults.xml /cm=SourceMonitorResult.xml

REM Generate SandCastle Documentation
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" "E:\Programming\!Csharp\MinifyLib\Docs\minidocs.shfbproj"

REM Clean up
COPY "E:\Programming\!Csharp\report.css" "E:\Programming\!Csharp\MinifyLib\Docs\CoverageResult"
COPY "E:\Programming\!Csharp\Presentation.css" "E:\Programming\!Csharp\MinifyLib\Docs\Documentation\styles"
COPY "E:\Programming\!Csharp\TOC.css" "E:\Programming\!Csharp\MinifyLib\Docs\Documentation"
DEL "E:\Programming\!Csharp\MinifyLib\Docs\TestResults.xml"
DEL "E:\Programming\!Csharp\MinifyLib\TestResults.xml"

REM Open the generate docs
"E:\Programming\!Csharp\MinifyLib\Docs\CoverageResult\index.htm"
"E:\Programming\!Csharp\MinifyLib\Docs\Documentation\Index.html"
exit
