%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "Rebuild.xml" /t:DeleteFiles

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "Rebuild.xml" /t:IncVersion /p:Version1=145 /p:Version2=42 /p:Version3=33 /p:Version4=1124

pause

exit