BFG Resource File Manager
=========================

This is a file manager for DOOM 3 BFG Edition/DOOM 3 (2019) .resources files.
It allows you to see, extract, delete, preview and edit the files inside .resources files.
Also it allows you to create and edit .resource files.

Build it yourself (BIY)
=======================

Windows:
- Install Microsoft Visual Studio (2017 or later) Community (hich is free to download and Install)
- Open the ResourceFileEditor.sln
- Select the desired configuration (Debug or Release)
- right click the ResourceFileEditor Project and select build
- go to the bin folder and you will find the binary inside the folder coresponding to the selected profile

Linux:
- Install mono-project
- Install nuget
- with the terminal go to the location of the project (where the .sln is).
- type `nuget install`
- type msbuild /t:build /p:Configuration=<desired configuration (Debug or Release)>

nuget Depedencies Licenses
==========================

StbImageSharp - Public Domain