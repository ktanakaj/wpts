@echo off
rem 
rem MSI�C���X�g�[���r���h�o�b�`
rem 
rem Wikipedia �|��x���c�[����MSI�`���̃C���X�g�[�����쐬����B
rem ���s���ɂ�WiX���т�Windows SDK�ibin�ƃT���v���X�N���v�g�j���C���X�g�[������Ă���K�v������B
rem �܂��A�o�b�`���s�O�ɕʓrVisual Studio�ɂ�郊���[�X�r���h���s���Ă����K�v������B
rem 

setlocal

rem WiX�Ȃ�т�Windows SDK�C���X�g�[���t�H���_
set WIX=%ProgramFiles(x86)%WiX Toolset v3.6
set WINSDK=%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.1A
set PATH=%WIX%\bin;%WINSDK%\Bin;%WINSDK%\Samples\sysmgmt\msi\scripts;%PATH%

rem WiX�r���h�t�@�C���i�ȉ��A�t�@�C�����͂��̃o�b�`����̑��΃p�X�j
set WXS_FILE=Wptscs.wxs
set WXL_ENUS_FILE=Wptscs.en-us.wxl
set WXL_JAJP_FILE=Wptscs.ja-jp.wxl

rem MSI�C���X�g�[��
set MSI_INSTALLER=setup.msi

rem ���ԃt�@�C���i��L�t�@�C�������琶���j
for /f "delims=" %%F in ("%MSI_INSTALLER%") do set WIXOBJ_FILE=%%~nF.wixobj
for /f "delims=" %%F in ("%MSI_INSTALLER%") do set MSI_INSTALLER_ENUS=%%~nF-en.msi
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
	-out "%MSI_INSTALLER_ENUS%" "%WIXOBJ_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem �����N�i���{��ŃC���X�g�[���j
light.exe -ext WixUIExtension -ext WixNetFxExtension ^
	-cultures:ja-jp -loc "%WXL_JAJP_FILE%" ^
	-out "%MSI_INSTALLER_JAJP%" "%WIXOBJ_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem �p��ŃC���X�g�[������ɁA����g�����X�t�@�[���쐬
msitran -g "%MSI_INSTALLER_ENUS%" "%MSI_INSTALLER_JAJP%" "%MST_JAJP%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem �p��ŃC���X�g�[���ƌ���g�����X�t�@�[��񂩂�A�}���`�����Q�[�W�C���X�g�[�����쐬
copy /b /y "%MSI_INSTALLER_ENUS%" "%MSI_INSTALLER%"

WiSubStg.vbs "%MSI_INSTALLER%" "%MST_JAJP%" 1041
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

WiLangId.vbs "%MSI_INSTALLER%" Package 1033,1041
set RET=%ERRORLEVEL%

endlocal & exit /b %RET%
