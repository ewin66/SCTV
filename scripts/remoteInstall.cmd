echo
set remoteDestination=%1
set installerPath=%2
set exePath=%3
set logPath=%4
set userName=%5
set password=%6

set remoteDestination=igor
set installerPath="\\fileserver\published projects\SCTV2\SCTVInstaller_3.0.msi"
set exePath="C:\dev\RESEARCH\deployment\AppDeployment\AppDeployment"
set logPath="\\fileserver\published projects\SCTV2\"
set userName=lickeyAdmin
set password=soccer

SET remoteDestination=###%remoteDestination%###
SET remoteDestination=%remoteDestination:"###=%
SET remoteDestination=%remoteDestination:###"=%
SET remoteDestination=%remoteDestination:###=%

SET installerPath=###%installerPath%###
SET installerPath=%installerPath:"###=%
SET installerPath=%installerPath:###"=%
SET installerPath=%installerPath:###=%

SET exePath=###%exePath%###
SET exePath=%exePath:"###=%
SET exePath=%exePath:###"=%
SET exePath=%exePath:###=%

SET logPath=###%logPath%###
SET logPath=%logPath:"###=%
SET logPath=%logPath:###"=%
SET logPath=%logPath:###=%

echo remoteDestination: %remoteDestination%
echo installerPath: %installerPath%
echo exePath: %exePath%

"%exePath%\Scripts\psexec" "\\%remoteDestination%" -u %userName% -p %password% msiexec -i "%installerPath%" /quiet /l* "%logPath%\%remoteDestination%_OUT.txt" SilentInstall=true

REM pause