Option 1 (using Web Platform Installer)
1) Prepare 3 files needed for installing Webdeploy
	- WebDeploy zip package
	- WPIFeed.xml
	- Params.app
   Preferably those files should reside in same folder.
2) Create empty website
3) Create empty database
4) Add host file entry
5) Update Params.app file with valid data and credentials
6) Update installerURL in WPIFeed.xml. It should point to webdeploy package you are going to install
8) Run: WebPICMD.exe /Install /AcceptEula /UseRemoteDatabase /Application:Hotcakes@Params.app /Feeds:d:\Work\ArrowTFS12\Hotcakes\HCC\Main\_Install\WPIFeed.xml /Log:WPILog.txt
	Command have to be run from folder that contains Params.app file. Path to WPIFeed.xml should lead to your prepared WPIFeed file
9) Navigate to http://hostedtrialwebdeploy/Install/Install.aspx?mode=install
10) Navigate to http://hostedtrialwebdeploy/


Option 2 (using MSDeploy) ( Shouldn't be used. This way doesn't allow specify parameters on a fly )
1) Create website hostedtrialwebdeploy
2) Add host file entry
3) msdeploy -verb:sync -source:package="d:\Work\ArrowTFS12\Hotcakes\HCC\Main\_Install\HotcakesCommerce_01.06.00_Deploy.zip" -dest:auto -setParam:kind=ProviderPath,scope=iisApp,value=hostedtrialwebdeploy
4) Navigate to http://hostedtrialwebdeploy/Install/Install.aspx?mode=install
5) Navigate to http://hostedtrialwebdeploy/


Files that have to be updated if base DNN needs to be updated:
1) Build.proj
2) DNN_Platform_<version>_Install.zip
3) DotNetNuke.Data.SqlDataProvider
4) admin.template ( needed only for website template files with version != 5.0 . This seems to be used for templates that were created with DNN even less than 5.6)

