
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib\SharpLib.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.Native\SharpLib.Native.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.WinForms\SharpLib.WinForms.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.Wpf\SharpLib.Wpf.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.OpenGL\SharpLib.OpenGL.csproj" /t:rebuild /p:Configuration=Debug

xcopy /Y "..\Source\SharpLib\bin\Debug\SharpLib.dll" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib\bin\Debug\SharpLib.pdb" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.Dialogs\bin\Debug\SharpLib.Dialogs.dll" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.Dialogs\bin\Debug\SharpLib.Dialogs.pdb" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.Native\bin\Debug\SharpLib.Native.dll" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.Native\bin\Debug\SharpLib.Native.pdb" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.WinForms\bin\Debug\SharpLib.WinForms.dll" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.WinForms\bin\Debug\SharpLib.WinForms.pdb" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.Wpf\bin\Debug\SharpLib.Wpf.dll" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.Wpf\bin\Debug\SharpLib.Wpf.pdb" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.OpenGL\bin\Debug\SharpLib.OpenGL.dll" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.OpenGL\bin\Debug\SharpLib.OpenGL.pdb" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.Log\bin\Debug\SharpLib.Log.dll" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.Log\bin\Debug\SharpLib.Log.pdb" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.Json\bin\Debug\SharpLib.Json.dll" "..\BuildResult\"
xcopy /Y "..\Source\SharpLib.Json\bin\Debug\SharpLib.Json.pdb" "..\BuildResult\"

pause

exit