%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "Utils.xml" /t:DeleteFiles

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "Utils.xml" /t:IncVersion /p:Version=1.2.3.4

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "Utils.xml" /t:Deploy /p:DeployPath="e:\Disk D\_build\TeamCity.Result\SharpLib\1.2.3.4" /p:Version=1.3.3.3

pause

exit