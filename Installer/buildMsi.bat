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
set PATH=C:\Program Files (x86)\Windows Installer XML v3.5\bin;%PATH%

rem WiX�r���h�t�@�C��
set WXS_FILE=Wptscs.wxs
set WXL_JAJP_FILE=Wptscs.ja-jp.wxl
set WXL_ENUS_FILE=Wptscs.en-us.wxl

rem MSI�C���X�g�[��
set MSI_INSTALLER=wptscs120.msi

rem ���ԃt�@�C���i�r���h�t�@�C�������琶���j
for /f "delims=" %%F in ("%WXS_FILE%") do set WIXOBJ_FILE=%%~nF.wixobj

rem �o�b�`�̃t�H���_�Ɉړ�
cd /d %~dp0

rem �R���p�C��
candle.exe -out "%WIXOBJ_FILE%" "%WXS_FILE%"
set RET=%ERRORLEVEL%
if not %RET% == 0 endlocal & exit /b %RET%

rem �����N
light.exe -ext WixUIExtension -ext WixNetFxExtension ^
	-cultures:ja-jp,en-us ^
	-loc "%WXL_JAJP_FILE%" -loc "%WXL_ENUS_FILE%" ^
	-out "%MSI_INSTALLER%" "%WIXOBJ_FILE%"
set RET=%ERRORLEVEL%

endlocal & exit /b %RET%
