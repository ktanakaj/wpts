// XML�ւ̐ݒ�ۑ��p�N���X
#ifndef ConfigH
#define ConfigH

#include "stdafx.h"

namespace wpts {

	using namespace System;
	using namespace System::Xml::Serialization;

	public ref class Config {
	public:
		// �v���O�����̏������[�h�������񋓒l
		enum class RunType{
			[XmlEnum(Name = "Wikipedia")]
			Wikipedia};

		// �N���C�A���g�Ƃ��Ă̋@�\�֌W�̐ݒ���i�[����N���X
		ref class ClientConfig {
		public:
			// �R���X�g���N�^
			ClientConfig(void){
				// �����o�ϐ��̗̈�m�ہE�����ݒ�
				RunMode = RunType::Wikipedia;
				SaveDirectory = "";
				LastSelectedSource = "en";
				LastSelectedTarget = "ja";
				UserAgent = "";
				Referer = "";
			}
			// �f�X�g���N�^
			virtual ~ClientConfig(){}

			// �v���O�����̏����Ώ�
			RunType RunMode;

			// ���s���ʂ�ۑ�����t�H���_
			String ^SaveDirectory;

			// �Ō�Ɏw�肵�Ă����|�󌳌���
			String ^LastSelectedSource;
			// �Ō�Ɏw�肵�Ă����|��挾��
			String ^LastSelectedTarget;

			// �ʐM���Ɏg�p����UserAgent
			String ^UserAgent;
			// �ʐM���Ɏg�p����Referer
			String ^Referer;
		};

		// �R���X�g���N�^�i�ʏ�j
		Config(void){
			// �����o�ϐ��̗̈�m�ہE�����ݒ�
			Client = gcnew ClientConfig();
			Languages = gcnew array<LanguageInformation^>(0);;
		}
		// �R���X�g���N�^�i�t�@�C���ǂݍ��݂���j
		Config(String ^i_Path){
			// �t�@�C������ݒ��ǂݍ���
			path = MYAPP::Cmn::NullCheckAndTrim(i_Path);
			if(Load() == false){
				// ���s�����ꍇ�A�ʏ�̃R���X�g���N�^�ŏ�����
				this->Config::Config();
			}
		}
		// �f�X�g���N�^
		~Config(){}

		// �ݒ���t�@�C���ɏ����o��
		bool Save(void){
			// �ݒ���V���A���C�Y��
			if(path == ""){
				return false;
			}
			return MYAPP::Cmn::XmlSerialize(this, path);
		}
		// �ݒ���t�@�C������ǂݍ���
		bool Load(void){
			// �ݒ���f�V���A���C�Y��
			if(path == ""){
				return false;
			}
			Object ^obj = nullptr;
			if(MYAPP::Cmn::XmlDeserialize(obj, this->GetType(), path) == true){
				Config ^config = dynamic_cast<Config^>(obj);
				if(config != nullptr){
					this->Client = config->Client;
					this->Languages = config->Languages;
					return true;
				}
			}
			return false;
		}

		// �w�肳�ꂽ�R�[�h�̌�����i�T�[�o�[���j���擾
		// �����݂��Ȃ��ꍇ�Anullptr
		LanguageInformation^ GetLanguage(String ^i_Code, RunType i_Mode){
			Type ^type;
			if(i_Mode == RunType::Wikipedia){
				type = WikipediaInformation::typeid;
			}
			else{
				type = LanguageInformation::typeid;
			}
			for each(LanguageInformation ^lang in Languages){
				if(lang->GetType() == type){
					if(lang->Code == i_Code){
						return lang;
					}
				}
			}
			return nullptr;
		}
		// �w�肳�ꂽ�R�[�h�̌�����i�T�[�o�[���j���擾�iRunType�̌^�j
		LanguageInformation^ GetLanguage(String ^i_Code){
			return GetLanguage(i_Code, Client->RunMode);
		}

		// �N���C�A���g�Ƃ��Ă̋@�\�֌W�̐ݒ��ۑ�
		ClientConfig ^Client;

		// ���ꂲ�Ƃ̏��i�T�[�o�[�̐ݒ�Ȃǂ��j��ۑ�
		[XmlArrayItem(LanguageInformation::typeid),
		XmlArrayItem(LanguageWithServerInformation::typeid),
		XmlArrayItem(WikipediaInformation::typeid)]
		array<LanguageInformation^> ^Languages;

	private:
		// �C���X�^���X�̃t�@�C����
		String ^path;
	};
}
#endif