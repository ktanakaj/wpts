// �|��x�������̖{�̃N���X
#include "stdafx.h"
#include "Translate.h"

using namespace wpts;

// Translate

/* �R���X�g���N�^ */
Translate::Translate(LanguageInformation ^i_Source, LanguageInformation ^i_Target){
	// ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
	if(i_Source == nullptr){
		throw gcnew ArgumentNullException("i_Source");
	}
	else if(i_Target == nullptr){
		throw gcnew ArgumentNullException("i_Target");
	}
	// �����o�ϐ��̏�����
	cmnAP = gcnew MYAPP::Cmn();
	source = i_Source;
	target = i_Target;
	runInitialize();
}

/* �|��x���������s */
bool Translate::Run(String ^i_Name)
{
	// �ϐ���������
	runInitialize();
	// �|��x���������s���̖{�̂����s
	// ���ȍ~�̏����́A�p���N���X�ɂĒ�`
	return runBody(i_Name);
}

/* �|��x���������s���̏��������� */
void Translate::runInitialize(void)
{
	// �ϐ���������
	_Log = "";
	Text = "";
	CancellationPending = false;
}

/* ���O���b�Z�[�W��1�s�ǉ��o�� */
void Translate::logLine(String ^i_Log)
{
	// ���O�̃��O�����s����Ă��Ȃ��ꍇ�A���s���ďo��
	if(Log != "" && Log->EndsWith((String^)ENTER) == false){
		Log += (ENTER + i_Log + ENTER);
	}
	else{
		Log += (i_Log + ENTER);
	}
}


// TranslateNetworkObject

/* �|��x���������s */
bool TranslateNetworkObject::Run(String ^i_Name)
{
	// �ϐ���������
	runInitialize();
	// �T�[�o�[�ڑ��`�F�b�N
	if(ping(static_cast<LanguageWithServerInformation^>(source)->Server) == false){
		return false;
	}
	// �|��x���������s���̖{�̂����s
	// ���ȍ~�̏����́A�p���N���X�ɂĒ�`
	return runBody(i_Name);
}

/* �T�[�o�[�ڑ��`�F�b�N */
bool TranslateNetworkObject::ping(String ^i_Server)
{
	// �T�[�o�[�ڑ��`�F�b�N
	NetworkInformation::Ping ^ping = gcnew NetworkInformation::Ping();
	try{
		NetworkInformation::PingReply ^reply = ping->Send(i_Server);
		if(reply->Status != NetworkInformation::IPStatus::Success){
			logLine(String::Format(cmnAP->Resource->GetString("ErrorMessage_MissNetworkAccess"), reply->Status.ToString()));
			return false;
		}
	}
	catch(Exception ^e){
		logLine(String::Format(cmnAP->Resource->GetString("ErrorMessage_MissNetworkAccess"), e->InnerException->Message));
		return false;
	}
	return true;
}
