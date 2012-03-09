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
set PATH=C:\Program Files (x86)\Windows Installer XML v3.5\bin;%PATH%

rem WiXビルドファイル
set WXS_FILE=Wptscs.wxs
set WXL_JAJP_FILE=Wptscs.ja-jp.wxl
set WXL_ENUS_FILE=Wptscs.en-us.wxl

rem MSIインストーラ
set MSI_INSTALLER=wptscs120.msi

rem 中間ファイル（ビルドファイル名から生成）
for /f "delims=" %%F in ("%WXS_FILE%") do set WIXOBJ_FILE=%%~nF.wixobj

rem バッチのフォルダに移動
cd /d %~dp0

rem コンパイル
candle.exe -out "%WIXOBJ_FILE%" "%WXS_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem リンク
light.exe -ext WixUIExtension -ext WixNetFxExtension ^
	-cultures:ja-jp,en-us ^
	-loc "%WXL_JAJP_FILE%" -loc "%WXL_ENUS_FILE%" ^
	-out "%MSI_INSTALLER%" "%WIXOBJ_FILE%"
set RET=%ERRORLEVEL%

endlocal & exit /b %RET%
