// �|��x�������̖{�̃N���X�i���ہj
#ifndef TranslateH
#define TranslateH

#include "stdafx.h"

namespace wpts {

	using namespace System;
	using namespace System::Net;

	// �|��x���������������邽�߂̋��ʃN���X
	ref class Translate abstract {
	public:
		// �R���X�g���N�^
		Translate(LanguageInformation^, LanguageInformation^);
		// �f�X�g���N�^
		virtual ~Translate(){}

		// �|��x���������s
		virtual bool Run(String ^);

		// ������r���ŏI�������邽�߂̃t���O
		bool CancellationPending;

		// ���O�X�V�`�B�C�x���g
		event EventHandler ^LogUpdate;
		// ���O���b�Z�[�W
		property String ^Log {
			public:
				String^ get()
				{
					return _Log;
				}
			protected:
				void set(String ^i_Log)
				{
					_Log = ((i_Log != nullptr) ? i_Log : "");
					LogUpdate(this, EventArgs::Empty);
				}
		}

		// �ϊ���e�L�X�g
		property String ^Text {
			public:
				String^ get()
				{
					return _Text;
				}
			protected:
				void set(String ^i_Text)
				{
					_Text = ((i_Text != nullptr) ? i_Text : "");
				}
		}

		// ���s�R�[�h
		static const String ^ENTER = "\r\n";

	protected:
		// �|��x���������s���̖{��
		// ���p���N���X�ł́A���̊֐��ɏ������������邱��
		virtual bool runBody(String^) = 0;
		// �|��x���������s���̏���������
		void runInitialize(void);
		// ���O���b�Z�[�W��1�s�o��
		void logLine(String^);

		// ���ʊ֐��N���X�̃I�u�W�F�N�g
		MYAPP::Cmn ^cmnAP;

		// �|�󌳁^�挾��̌���R�[�h
		LanguageInformation ^source;
		LanguageInformation ^target;

	private:
		// ���O���b�Z�[�W�iproperty�j
		String ^_Log;
		// �ϊ���e�L�X�g�iproperty�j
		String ^_Text;
	};

	// Wikipedia�p�̖|��x�����������N���X
	ref class TranslateNetworkObject abstract : public Translate {
	public:
		// �R���X�g���N�^
		// ��
		TranslateNetworkObject(LanguageWithServerInformation ^i_Source, LanguageWithServerInformation ^i_Target)
			: Translate(i_Source, i_Target){}
		// �f�X�g���N�^
		virtual ~TranslateNetworkObject(){}

		// �|��x���������s
		virtual bool Run(String ^) override;

		// �ʐM���Ɏg�p����UserAgent
		String ^UserAgent;
		// �ʐM���Ɏg�p����Referer
		String ^Referer;

	private:
		// �T�[�o�[�ڑ��`�F�b�N
		bool ping(String^);
	};

}
#endif