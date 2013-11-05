
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Project\SharpLib.sln" /t:clean /verbosity:normal
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Project\SharpLib.sln" /t:build /verbosity:normal

pause
