BFG Resource File Manager
=========================

This is a file manager for DOOM 3 BFG Edition/DOOM 3 (2019) .resources files.
It allows you to see, extract, delete, preview and edit the files inside .resources files.
Also it allows you to create and edit .resource files.

THE PROJECT HAS PRE_BUILD BINARIES (.EXE FILES) HERE: https://github.com/MadDeCoDeR/BFG-Resource-File-Manager/releases
YOU DONT HAVE TO FOLLOW THE INSTRUCTIONS BELOW. THOSE INSTRUCTIONS ARE FOR DEVELOPERS WHO WANT TO TEST OR CONTRIBUTE TO THE PROJECT.
IF YOU ARE A LINUX USER MAKE SURE TO DOWNLOAD AND INSTALL THE MONO PROJECT: https://www.mono-project.com/download/stable/#download-lin

Build it yourself (BIY)
=======================

Windows:
- Install Microsoft Visual Studio (2017 or later) Community (is free to download and Install)
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

- StbImageSharp - Public Domain
- StbImageWriteSharp - Public Domain
- BCnEncoder.Net45 - MIT or Unlicensed
- System.Numerics.Vector - MIT
