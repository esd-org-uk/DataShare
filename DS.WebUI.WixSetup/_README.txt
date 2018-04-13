
Readme for installation of datashare 
==========================================================
IMPORTANT !!!! run heat to generate wxs file for component 
==========================================================
1. Select build configuration ( release preferably as for release version)
2. Run Build on DS.WebUI.WixSetup. ( if built ok - go get the msi file) 

3. if error with building the DS.WebUI.WixSetup - 
this means that the msbuild of the release version of DS.WebUI will be in the DS.WEbUi Obj folder
i.e. "C:\tfs\DataShare Live\Dev\DS.WebUI\obj\Release\Package\PackageTmp\\"
this means the DS.WebUI is built and the package is ready. 
BUT the WixSetup heat was unable to create the msi because the darice.cub is missing from the temp folder where wix uses to create the msi. 

now wix is bad as it doesn't tell you via Visual studio error output,
 so we will have to run the heat thing manually to generate the latest DS.WebUI.wxs in order to use it for the project. 
i.e. 
"C:\Program Files (x86)\WiX Toolset v3.8\bin\Heat.exe" dir "C:\tfs\DataShare Live\Dev\DS.WebUI\obj\ReleaseToLive\Package\PackageTmp\\" -cg DS.WebUI_Project -dr WEBINSTALLATIONDIR -scom -sreg -srd -var var.BasePath -gg -sfrag -out <INSERT_SOME_TEMP_PATH_FOR_THELOCATION_OFTHE_OUTPUT>DS.WebUI.wxs
this will run and then come up with an error that says something like this 
i.e. Error	2	The cube file 'C:\Users\supkenchin\AppData\Local\assembly\dl3\GK3B5NE4.T2K\38NO1MAQ.HEB\6f7938a4\002acd04_01ecce01\darice.cub' cannot be found.  This file is required for MSI validation.	light.exe	0	1	DS.WebUI.WixSetup

so what we need to do is to 
copy from the installation directory of your wix toolset "darice.cub" to whichever folder() that command window says it is not found. 
(here my wix installation directory is C:\Program Files (x86)\WiX Toolset v3.8\bin\)
and the temp folder that wix heat uses is (C:\Users\supkenchin\AppData\Local\assembly\dl3\GK3B5NE4.T2K\38NO1MAQ.HEB\6f7938a4\002acd04_01ecce01\)
copy and paste darice.cub in there and then run heat.exe 

4. now we have 2 options, 
option 1
i. Get the DS.WebUI.wxs and put it into the project
ii. edit the project file of DS.WEbUi.WixSetup and comment out the section where it generates
i.e. below here
<!--<HeatDirectory PreprocessorVariable="var.BasePath" OutputFile="DS.WebUI.wxs" Directory="%(ProjectReference.RootDir)%(ProjectReference.Directory)obj\$(Configuration)\Package\PackageTmp\" DirectoryRefId="WEBINSTALLATIONDIR" ComponentGroupName="%(ProjectReference.Filename)_Project" SuppressCom="true" SuppressFragments="true" SuppressRegistry="true" SuppressRootDirectory="true" AutoGenerateGuids="false" GenerateGuidsNow="true" ToolPath="$(WixToolPath)" Condition="'%(ProjectReference.PackageThisProject)'=='True'" />-->
iii. then run build for DS.WebUI.WixSetup again. you will get your MSBuild file. 


option 2. 
just run build on DS.WebUI.WixSetup again and you're ok to get your msi.  ( this doesn't always work that's why i use option 1 sometimes)



==========================================================
IMPORTANT !!!! run heat to generate wxs file for component 
==========================================================
*Note: a log file can be generated when installing with the msi from the commandline
msiexec /i "C:\tempken\Datashare141.msi" /L*V "C:\tempken\DsInstall_001.log"
