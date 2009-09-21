// Wikipedia�̊e����E�ݒ�Ȃǂ��Ǘ����邽�߂̃N���X
#ifndef WikipediaInformationH
#define WikipediaInformationH

#include "Language.h"

namespace wpts {

	using namespace System;
	using namespace System::Xml::Serialization;

	// ������E�T�[�o�[���ɉ����AWikipedia�̃T�[�o�[���Ƃ̐ݒ���i�[����N���X
	public ref class WikipediaInformation : public LanguageWithServerInformation {
	public:
		// �e�T�[�o�[�ł̖��O��Ԃ̐ݒ���i�[���邽�߂̍\����
		value struct Namespace : IComparable {
			int Key;				// ���O��Ԃ̔ԍ�
			String ^Name;			// ���O��Ԗ�

			// �z��̃\�[�g�p���\�b�h
			virtual int CompareTo(Object^);
		};

		// �R���X�g���N�^�i�V���A���C�Y�p�j
		WikipediaInformation(void)
			: LanguageWithServerInformation(){
//			System::Diagnostics::Debug::WriteLine("WikipediaInformation::WikipediaInformation > ��������Ȃ��R���X�g���N�^���g�p���Ă��܂�");
			// �K���Ȓl�Œʏ�̃R���X�g���N�^�����s
			this->WikipediaInformation::WikipediaInformation("unknown");
		}
		// �R���X�g���N�^�i�ʏ�j
		WikipediaInformation(String ^i_Code)
			: LanguageWithServerInformation(i_Code){
			// �����l�ݒ�
			setDefault();
		}
		// �f�X�g���N�^
		virtual ~WikipediaInformation(){}

		// �w�肵������ł̖��̂� �L����|���� �̌`���Ŏ擾
		String ^GetFullName(String^);
		// �w�肳�ꂽ�ԍ��̖��O��Ԃ��擾
		String^ GetNamespace(int);
		// �w�肳�ꂽ������Wikipedia�̃V�X�e���ϐ��ɑ������𔻒�
		bool ChkSystemVariable(String^);

		// �L����XML�f�[�^�����݂���p�X
		property String ^ArticleXmlPath {
			String^ get()
			{
				return _ArticleXmlPath;
			}
			void set(String ^i_Path)
			{
				_ArticleXmlPath = ((i_Path != nullptr) ? i_Path->Trim() : "");
			}
		}

		// Wikipedia�����̃V�X�e����`�ϐ�
		[XmlArrayItem("Variable")]
		array<String^> ^SystemVariables;

		// ���ʂ̃t�H�[�}�b�g
		String ^Bracket;
		// ���_�C���N�g�̕�����
		String ^Redirect;

		// ���O��Ԃ̐ݒ�
		[XmlIgnoreAttribute()]
		array<Namespace> ^Namespaces;
		// �e���v���[�g�E�J�e�S���E�摜�̖��O��Ԃ������ԍ�
		const static int TEMPLATENAMESPACENUMBER = 10;
		const static int CATEGORYNAMESPACENUMBER = 14;
		const static int IMAGENAMESPACENUMBER = 6;

		// ���o���̒�^��
		[XmlArrayItem("Title")]
		array<String ^> ^TitleKeys;

	private:
		// �����o�ϐ��̏����l�ݒ�
		void setDefault(void);

		// �L����XML�f�[�^�����݂���p�X�iproperty�j
		String ^_ArticleXmlPath;
	};

}
#endif
