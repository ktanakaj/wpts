// �|��x�������̖{�̃N���X�AWikipedia�p
#ifndef TranslateWikipediaH
#define TranslateWikipediaH

#include "stdafx.h"
#include "Translate.h"
#include "WikipediaArticle.h"

namespace wpts {

	using namespace System;
	using namespace System::Windows::Forms;

	// Wikipedia�p�̖|��x�����������N���X
	ref class TranslateWikipedia : TranslateNetworkObject {
	public:
		// �R���X�g���N�^
		TranslateWikipedia(WikipediaInformation ^i_Source, WikipediaInformation ^i_Target)
			: TranslateNetworkObject(i_Source, i_Target){}
		// �f�X�g���N�^
		virtual ~TranslateWikipedia(){}

	protected:
		// �|��x���������s���̖{��
		virtual bool runBody(String^) override;

		// �|��x���Ώۂ̋L�����m�F�E�擾
		WikipediaArticle^ chkTargetArticle(String^);
		// �w�肳�ꂽ�L�����擾�A����ԃ����N���m�F�A�Ԃ�
		virtual String^ getInterWiki(String^, bool);
		String^ getInterWiki(String^);
		// �n���ꂽ�e�L�X�g����͂��A����ԃ����N�E���o�����̕ϊ����s��
		String^ replaceText(String^, String^, bool);
		String^ replaceText(String^, String^);
		// �����N�̉�́E�u�����s��
		int replaceLink(String^ %, String^, int, String^);
		// ���������N�̕������ϊ�����
		String^ replaceInnerLink(WikipediaFormat::Link, String^);
		// �e���v���[�g�̕������ϊ�����
		String^ replaceTemplate(WikipediaFormat::Link, String^);
		// �w�肳�ꂽ�C���f�b�N�X�̈ʒu�ɑ��݂��錩�o������͂��A�\�ł���Εϊ����ĕԂ�
		virtual int chkTitleLine(String^ %, String^, int);

		// �|�󌳃R�[�h�ł̒�^��ɑ�������A�|��挾��ł̒�^����擾
		virtual String^ getKeyWord(String^);
	};
}
#endif