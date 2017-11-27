set DIR_PATH=%~dp0
start msbuild /t:pack %DIR_PATH%src\Maqduni.AspNetCore.Identity.RavenDb\Maqduni.AspNetCore.Identity.RavenDb.csproj /p:IncludeSymbols=true /p:IncludeSource=true /p:PackageOutputPath="C:\Temp\NuGet Local"
start msbuild /t:pack %DIR_PATH%src\Maqduni.RavenDb.Extensions\Maqduni.RavenDb.Extensions.csproj /p:IncludeSymbols=true /p:IncludeSource=true /p:PackageOutputPath="C:\Temp\NuGet Local"
pause