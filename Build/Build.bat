
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Project\SharpLib_35\SharpLib_35.csproj" /t:rebuild /p:Configuration=Release
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Project\SharpLib_35\SharpLib_35.csproj" /t:rebuild /p:Configuration=Debug

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Project\SharpLib_40\SharpLib_40.csproj" /t:rebuild /p:Configuration=Release
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Project\SharpLib_40\SharpLib_40.csproj" /t:rebuild /p:Configuration=Debug

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Project\SharpLib_45\SharpLib_45.csproj" /t:rebuild /p:Configuration=Release
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Project\SharpLib_45\SharpLib_45.csproj" /t:rebuild /p:Configuration=Debug

pause

exit
