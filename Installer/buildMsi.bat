@echo off
rem 
rem MSI�C���X�g�[���r���h�o�b�`
rem 
rem Wikipedia �|��x���c�[����MSI�`���̃C���X�g�[�����쐬����B
rem ���s���ɂ�WiX���C���X�g�[������Ă���K�v������B
rem �܂��A�o�b�`���s�O�ɕʓrVisual Studio�ɂ�郊���[�X�r���h���s���Ă����K�v������A
rem ���̑��\�[�X�ꎮ���܂Ƃ߂� src.zip ���p�ӂ��Ă��̃t�H���_�ɓ���Ȃ���΂Ȃ�Ȃ��B
rem 

setlocal

rem WiX�C���X�g�[����t�H���_�iPATH���ݒ肳��Ă��Ȃ��ꍇ�j
set PATH=C:\Program Files (x86)\Windows Installer XML v3.5\bin;C:\Program Files\Microsoft SDKs\Windows\v7.0\Bin;C:\Program Files\Microsoft SDKs\Windows\v7.0\Samples\sysmgmt\msi\scripts;%PATH%

rem WiX�r���h�t�@�C��
set WXS_FILE=Wptscs.wxs
set WXL_JAJP_FILE=Wptscs.ja-jp.wxl
set WXL_ENUS_FILE=Wptscs.en-us.wxl

rem MSI�C���X�g�[��
set MSI_INSTALLER=setup.msi

rem ���ԃt�@�C���i��L�t�@�C�������琶���j
for /f "delims=" %%F in ("%WXS_FILE%") do set WIXOBJ_FILE=%%~nF.wixobj
for /f "delims=" %%F in ("%MSI_INSTALLER%") do set MSI_INSTALLER_JAJP=%%~nF-ja.msi
for /f "delims=" %%F in ("%MSI_INSTALLER%") do set MST_JAJP=%%~nF-ja.mst

rem �o�b�`�̃t�H���_�Ɉړ�
cd /d %~dp0

rem �R���p�C��
candle.exe -out "%WIXOBJ_FILE%" "%WXS_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem �����N�i�p��ŃC���X�g�[���j
light.exe -ext WixUIExtension -ext WixNetFxExtension ^
	-cultures:en-us -loc "%WXL_ENUS_FILE%" ^
	-out "%MSI_INSTALLER%" "%WIXOBJ_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem �����N�i���{��ŃC���X�g�[���j
light.exe -ext WixUIExtension -ext WixNetFxExtension ^
	-cultures:ja-jp -loc "%WXL_JAJP_FILE%" ^
	-out "%MSI_INSTALLER_JAJP%" "%WIXOBJ_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem ����g�����X�t�@�[���쐬
msitran -g "%MSI_INSTALLER%" "%MSI_INSTALLER_JAJP%" "%MST_JAJP%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem �}���`�����Q�[�W�C���X�g�[�����쐬
rem msidb -d "%MSI_INSTALLER%" -r "%MST_JAJP%"
rem set RET=%ERRORLEVEL%

WiSubStg.vbs "%MSI_INSTALLER%" "%MST_JAJP%" 1041
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%
WiLangId.vbs "%MSI_INSTALLER%" Package 1033,1041
set RET=%ERRORLEVEL%

endlocal & exit /b %RET%
