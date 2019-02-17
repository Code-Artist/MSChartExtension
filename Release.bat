REM Release Script for Code Project
@echo off
SET SRC=D:\CKMAI_Documents\Programming\ClassLibraryNET\MSChartExtension
SET DEST=D:\CKMAI_Documents\SWRelease\MSChartExtension\
SET TARGET=%DEST%MSChartExtension
SET BINTARGET=%DEST%MSChartExtensionDemoBin
SET ZIP="C:\Program Files\7-Zip\7z.exe"
SET ZIPFile=%DEST%MSChartExtensionSource.zip
SET ZIPBin=%DEST%MSChartExtensionDemo.zip

echo Copying Source Code
del %TARGET% /s /q
rd %TARGET% /s /q
robocopy %SRC% %TARGET% /MIR /XD .hg .git Doc obj bin .vs /XF .hg* .git* *.bat *.userprefs *.pdn
%ZIP% a -tzip %ZIPFILE% %TARGET%

echo Copying Binary Files
robocopy %SRC%\MSChartExtensionDemo\bin\Debug %BINTARGET% /MIR /XF *.pdb *.mdb *.vshost*
%ZIP% a -tzip %ZIPBin% %BINTARGET%