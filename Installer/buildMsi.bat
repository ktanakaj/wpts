@echo off
rem 
rem MSIインストーラビルドバッチ
rem 
rem Wikipedia 翻訳支援ツールのMSI形式のインストーラを作成する。
rem 実行環境にはWiXがインストールされている必要がある。
rem また、バッチ実行前に別途Visual Studioによるリリースビルドを行っておく必要があり、
rem その他ソース一式をまとめた src.zip も用意してこのフォルダに入れなければならない。
rem 

setlocal

rem WiXインストール先フォルダ（PATHが設定されていない場合）
set PATH=C:\Program Files (x86)\Windows Installer XML v3.5\bin;C:\Program Files\Microsoft SDKs\Windows\v7.0\Bin;C:\Program Files\Microsoft SDKs\Windows\v7.0\Samples\sysmgmt\msi\scripts;%PATH%

rem WiXビルドファイル
set WXS_FILE=Wptscs.wxs
set WXL_JAJP_FILE=Wptscs.ja-jp.wxl
set WXL_ENUS_FILE=Wptscs.en-us.wxl

rem MSIインストーラ
set MSI_INSTALLER=setup.msi

rem 中間ファイル（上記ファイル名から生成）
for /f "delims=" %%F in ("%WXS_FILE%") do set WIXOBJ_FILE=%%~nF.wixobj
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
	-out "%MSI_INSTALLER%" "%WIXOBJ_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem リンク（日本語版インストーラ）
light.exe -ext WixUIExtension -ext WixNetFxExtension ^
	-cultures:ja-jp -loc "%WXL_JAJP_FILE%" ^
	-out "%MSI_INSTALLER_JAJP%" "%WIXOBJ_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem 言語トランスファー情報作成
msitran -g "%MSI_INSTALLER%" "%MSI_INSTALLER_JAJP%" "%MST_JAJP%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem マルチランゲージインストーラを作成
rem msidb -d "%MSI_INSTALLER%" -r "%MST_JAJP%"
rem set RET=%ERRORLEVEL%

WiSubStg.vbs "%MSI_INSTALLER%" "%MST_JAJP%" 1041
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%
WiLangId.vbs "%MSI_INSTALLER%" Package 1033,1041
set RET=%ERRORLEVEL%

endlocal & exit /b %RET%
