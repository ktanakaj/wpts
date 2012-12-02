@echo off
rem 
rem MSIインストーラビルドバッチ
rem 
rem Wikipedia 翻訳支援ツールのMSI形式のインストーラを作成する。
rem 実行環境にはWiX並びにWindows SDK（binとサンプルスクリプト）がインストールされている必要がある。
rem また、バッチ実行前に別途Visual Studioによるリリースビルドを行っておく必要がある。
rem 

setlocal

rem WiXならびにWindows SDKインストールフォルダ
set WIX=%ProgramFiles(x86)%WiX Toolset v3.6
set WINSDK=%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.1A
set PATH=%WIX%\bin;%WINSDK%\Bin;%WINSDK%\Samples\sysmgmt\msi\scripts;%PATH%

rem WiXビルドファイル（以下、ファイル名はこのバッチからの相対パス）
set WXS_FILE=Wptscs.wxs
set WXL_ENUS_FILE=Wptscs.en-us.wxl
set WXL_JAJP_FILE=Wptscs.ja-jp.wxl

rem MSIインストーラ
set MSI_INSTALLER=setup.msi

rem 中間ファイル（上記ファイル名から生成）
for /f "delims=" %%F in ("%MSI_INSTALLER%") do set WIXOBJ_FILE=%%~nF.wixobj
for /f "delims=" %%F in ("%MSI_INSTALLER%") do set MSI_INSTALLER_ENUS=%%~nF-en.msi
for /f "delims=" %%F in ("%MSI_INSTALLER%") do set MSI_INSTALLER_JAJP=%%~nF-ja.msi
for /f "delims=" %%F in ("%MSI_INSTALLER%") do set MST_JAJP=%%~nF-ja.mst

rem バッチのフォルダに移動
cd /d %~dp0

rem コンパイル
candle.exe -out "%WIXOBJ_FILE%" "%WXS_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem リンク（英語版インストーラ）
light.exe -ext WixUIExtension -ext WixNetFxExtension ^
	-cultures:en-us -loc "%WXL_ENUS_FILE%" ^
	-out "%MSI_INSTALLER_ENUS%" "%WIXOBJ_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem リンク（日本語版インストーラ）
light.exe -ext WixUIExtension -ext WixNetFxExtension ^
	-cultures:ja-jp -loc "%WXL_JAJP_FILE%" ^
	-out "%MSI_INSTALLER_JAJP%" "%WIXOBJ_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem 英語版インストーラを基準に、言語トランスファー情報作成
msitran -g "%MSI_INSTALLER_ENUS%" "%MSI_INSTALLER_JAJP%" "%MST_JAJP%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem 英語版インストーラと言語トランスファー情報から、マルチランゲージインストーラを作成
copy /b /y "%MSI_INSTALLER_ENUS%" "%MSI_INSTALLER%"

WiSubStg.vbs "%MSI_INSTALLER%" "%MST_JAJP%" 1041
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

WiLangId.vbs "%MSI_INSTALLER%" Package 1033,1041
set RET=%ERRORLEVEL%

endlocal & exit /b %RET%
