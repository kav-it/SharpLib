
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib\SharpLib.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.Native\SharpLib.Native.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.WinForms\SharpLib.WinForms.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.Wpf\SharpLib.Wpf.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.OpenGL\SharpLib.OpenGL.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.Audio\SharpLib.Audio.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.Json\SharpLib.Json.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.Log\SharpLib.Log.csproj" /t:rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "..\Source\SharpLib.Docking\SharpLib.Docking.csproj" /t:rebuild /p:Configuration=Debug

copy /Y "..\Source\SharpLib\bin\Debug\SharpLib.dll" "..\BuildResult\"
copy /Y "..\Source\SharpLib\bin\Debug\SharpLib.pdb" "..\BuildResult\"
copy /Y "..\Source\SharpLib.Audio\bin\Debug\SharpLib.Audio.dll" "..\BuildResult\"
copy /Y "..\Source\SharpLib.Audio\bin\Debug\SharpLib.Audio.pdb" "..\BuildResult\"
copy /Y "..\Source\SharpLib.Native\bin\Debug\SharpLib.Native.dll" "..\BuildResult\"
copy /Y "..\Source\SharpLib.Native\bin\Debug\SharpLib.Native.pdb" "..\BuildResult\"
copy /Y "..\Source\SharpLib.WinForms\bin\Debug\SharpLib.WinForms.dll" "..\BuildResult\"
copy /Y "..\Source\SharpLib.WinForms\bin\Debug\SharpLib.WinForms.pdb" "..\BuildResult\"
copy /Y "..\Source\SharpLib.Wpf\bin\Debug\SharpLib.Wpf.dll" "..\BuildResult\"
copy /Y "..\Source\SharpLib.Wpf\bin\Debug\SharpLib.Wpf.pdb" "..\BuildResult\"
copy /Y "..\Source\SharpLib.OpenGL\bin\Debug\SharpLib.OpenGL.dll" "..\BuildResult\"
copy /Y "..\Source\SharpLib.OpenGL\bin\Debug\SharpLib.OpenGL.pdb" "..\BuildResult\"
copy /Y "..\Source\SharpLib.Log\bin\Debug\SharpLib.Log.dll" "..\BuildResult\"
copy /Y "..\Source\SharpLib.Log\bin\Debug\SharpLib.Log.pdb" "..\BuildResult\"
copy /Y "..\Source\SharpLib.Json\bin\Debug\SharpLib.Json.dll" "..\BuildResult\"
copy /Y "..\Source\SharpLib.Json\bin\Debug\SharpLib.Json.pdb" "..\BuildResult\"
copy /Y "..\Source\SharpLib\bin\Debug\SharpLib.Docking.dll" "..\BuildResult\"
copy /Y "..\Source\SharpLib\bin\Debug\SharpLib.Docking.pdb" "..\BuildResult\"

pause

exit