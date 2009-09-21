// Wikipedia�̋L�����Ǘ����邽�߂̃N���X
#ifndef WikipediaArticleH
#define WikipediaArticleH

#include "stdafx.h"

namespace wpts {

	using namespace System;
	using namespace System::IO;
	using namespace System::Net;
	using namespace System::Xml;
	using namespace System::Windows::Forms;

	// Wikipedia�̋L���̏������������߂̃N���X
	ref class WikipediaFormat {
	public:
		// Wikipedia�̃����N�̗v�f���i�[���邽�߂̍\����
		value struct Link {
			String ^Text;				// �����N�̃e�L�X�g�i[[�`]]�j
			String ^Article;			// �����N�̋L����
			String ^Section;			// �����N�̃Z�N�V�������i#�j
			array<String^> ^PipeTexts;	// �����N�̃p�C�v��̕�����i|�j
			String ^Code;				// ����Ԃ܂��͑��v���W�F�N�g�ւ̃����N�̏ꍇ�A�R�[�h
			bool TemplateFlag;			// �e���v���[�g�i{{�`}}�j���������t���O
			bool SubPageFlag;			// �L�����̐擪�� / �Ŏn�܂邩�������t���O
			bool StartColonFlag;		// �����N�̐擪�� : �Ŏn�܂邩�������t���O
			bool MsgnwFlag;				// �e���v���[�g�̏ꍇ�Amsgnw: ���t������Ă��邩�������t���O
			bool EnterFlag;				// �e���v���[�g�̏ꍇ�A�L�����̌�ŉ��s����邩�������t���O

			// ������
			void Initialize(void);
			// ���݂�Text�ȊO�̒l����AText�𐶐�
			String^ MakeText(void);
		};

		// �R���X�g���N�^�i�T�[�o�[���w��j
		WikipediaFormat(WikipediaInformation^);
		// �f�X�g���N�^
		virtual ~WikipediaFormat(){}

		// �n���ꂽ�L�������J�e�S���[�����`�F�b�N
		virtual bool IsCategory(String^);
		// �n���ꂽ�L�������摜�����`�F�b�N
		virtual bool IsImage(String^);
		// �n���ꂽ�L�������W�����O��ԈȊO�����`�F�b�N
		virtual bool IsNotMainNamespace(String^);

		// �n���ꂽWikipedia�̓��������N�E�e���v���[�g�����
		virtual Link ParseInnerLink(String^);
		virtual Link ParseTemplate(String^);
		// �n���ꂽ�e�L�X�g�̎w�肳�ꂽ�ʒu�ɑ��݂���Wikipedia�̓��������N�E�e���v���[�g���`�F�b�N
		int ChkLinkText(Link %, String^, int);

		// �n���ꂽ�e�L�X�g�̎w�肳�ꂽ�ʒu�ɑ��݂���ϐ����`�F�b�N
		virtual int ChkVariable(String^ %, String^ %, String^, int);
		// �n���ꂽ�e�L�X�g�̎w�肳�ꂽ�ʒu����nowiki��Ԃ��̃`�F�b�N
		static int ChkNowiki(String^ %, String^, int);
		// �n���ꂽ�e�L�X�g�̎w�肳�ꂽ�ʒu����R�����g��Ԃ��̃`�F�b�N
		static int ChkComment(String^ %, String^, int);

		// Wikipedia�̌Œ�l�̏���
		static const String ^COMMENTSTART = "<!--";
		static const String ^COMMENTEND = "-->";
		static const String ^NOWIKISTART = "<nowiki>";
		static const String ^NOWIKIEND = "</nowiki>";
		static const String ^MSGNW = "msgnw:";

		// �L������������T�[�o�[���
		property WikipediaInformation ^Server {
			WikipediaInformation^ get()
			{
				return _Server;
			}
		}

	protected:
		// �L������������T�[�o�[���iproperty�j
		WikipediaInformation ^_Server;
	};

	// Wikipedia�̋L�����Ǘ����邽�߂̃N���X
	ref class WikipediaArticle : WikipediaFormat {
	public:
		// �R���X�g���N�^�i�T�[�o�[�ƋL�������w��j
		WikipediaArticle(WikipediaInformation ^i_Server, String ^i_Name)
			: WikipediaFormat(i_Server){
			// �����ݒ�
			Initialize(i_Name);
		}
		// �f�X�g���N�^
		virtual ~WikipediaArticle(){}

		// �����ݒ�
		void Initialize(String^);

		// �L�����T�[�o�[���擾
		// ��GetInterWiki(), GetNamespaces(), IsRedirect()�A����ɑ�����property�́A
		//   ���O��GetArticle()�����s���Ă����K�v������
		virtual bool GetArticle(String^, String^, TimeSpan);
		bool GetArticle(String^, String^);
		bool GetArticle(void);
		// �w�肳�ꂽ����R�[�h�ւ̌���ԃ����N��Ԃ�
		virtual String^ GetInterWiki(String^);
		// �L����XML���疼�O��ԏ����擾
		virtual array<WikipediaInformation::Namespace>^ GetNamespaces(void);

		// �L�������_�C���N�g�����`�F�b�N
		virtual bool IsRedirect(void);
		// �L�����J�e�S���[�����`�F�b�N
		virtual bool IsCategory(void);
		// �L�����摜�����`�F�b�N
		virtual bool IsImage(void);
		// �L�����W�����O��ԈȊO�����`�F�b�N
		bool IsNotMainNamespace(void);

		// �L����
		property String ^Title {
			String^ get()
			{
				return _Title;
			}
		}
		// �L����XML�f�[�^��URL�iproperty�j
		property Uri ^Url {
			Uri^ get()
			{
				return _Url;
			}
		}
		// �L����XML�f�[�^
		property XmlDocument ^Xml {
			XmlDocument^ get()
			{
				return _Xml;
			}
		}
		// �L���̍ŏI�X�V�����iUTC�j
		property DateTime Timestamp {
			DateTime get()
			{
				return _Timestamp;
			}
		}
		// �L���{��
		property String ^Text {
			String^ get()
			{
				return _Text;
			}
		}
		// ���_�C���N�g��L����
		property String ^Redirect {
			String^ get()
			{
				return _Redirect;
			}
		}
		// GetArticle���s����HttpStatus
		property HttpStatusCode GetArticleStatus {
			HttpStatusCode get()
			{
				return _GetArticleStatus;
			}
		}
		// GetArticle��O�������̗�O���
		// ��GetArticle()��false�ŁAGetArticleStatus��NotFound�ȊO�̂Ƃ��A�ݒ肳���
		property Exception ^GetArticleException {
			Exception^ get()
			{
				return _GetArticleException;
			}
		}

		// Wikipedia��XML�̌Œ�l�̏���
		const static String ^XMLNS = "http://www.mediawiki.org/xml/export-0.3/";

	protected:
		// �L����XML���T�[�o�[���擾
		bool getServerArticle(String^, String^);
		// �L����XML���L���b�V�����擾
		bool getCacheArticle(TimeSpan);
		// �L���{���̎w�肳�ꂽ�ʒu�ɑ��݂���Wikipedia��Link�����
		virtual int chkLinkText(Link %, int);
		// �L���{���̎w�肳�ꂽ�ʒu���炪�R�����g��Ԃ��̃`�F�b�N
		virtual int chkComment(String^ %, int);

		// �L�����iproperty�j
		String ^_Title;
		// �L����XML�f�[�^��URL�iproperty�j
		Uri ^_Url;
		// �L����XML�f�[�^�iproperty�j
		XmlDocument ^_Xml;
		// �L���̍ŏI�X�V�����iUTC�j�iproperty�j
		DateTime _Timestamp;
		// �L���{���iproperty�j
		String ^_Text;
		// ���_�C���N�g��L�����iproperty�j
		String ^_Redirect;
		// GetArticle���s����HttpStatus�iproperty�j
		HttpStatusCode _GetArticleStatus;
		// GetArticle��O�������̗�O���iproperty�j
		Exception ^_GetArticleException;
	};
}
#endif
