
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib\SharpLib.csproj" /t:rebuild /p:Configuration=Release
copy /Y "..\Source\SharpLib\bin\Debug\SharpLib.dll" "..\BuildResult"

"..\Nuget\nuget.exe" pack "..\Nuget\SharpLib.nuspec" -OutputDirectory "..\BuildResult"

pause

exit