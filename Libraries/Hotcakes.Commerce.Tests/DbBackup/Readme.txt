HotcakesDevTest.bak is file with database backup. It should be kept up to date with database structure of DB of corresponding branch on which tests have to be run.
There are few possible ways to keept it up to date:

1) Rollover db changes on this backup
To do this follow next steps:
- Restore HotcakesDevTest.bak on Sql Server
- Rollover latest scripts in *.SqlDataProvider from \Main\Website\DesktopModules\Hotcakes\Providers\DataProviders\SqlDataProvider\
- Backup DB on Sql Server and update file under TFS with new backup

2) Create completely new backup
- Install Hotcakes package build from corresponding branch on clean DNN install
- Go through Hotcakes Setup Wizard
- Create sample products and categories
- Rename logical DB file name to "HotcakesDevTest" and "HotcakesDevTest_log" if thery had other names
- Backup DB on Sql Server and update file under TFS with new backup
