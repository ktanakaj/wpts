// ����֌W�̊e��ݒ�Ȃǂ��Ǘ����邽�߂̃N���X
#ifndef LanguageH
#define LanguageH

namespace wpts {

	using namespace System;
	using namespace System::Xml::Serialization;

	// ����Ɋւ�������i�[����N���X
	public ref class LanguageInformation : IComparable {
	public:
		// ���錾��́A�e����ł̖��́E���̂��i�[���邽�߂̍\����
		value struct LanguageName : IComparable {
		public:
			// ����R�[�h
			[XmlAttributeAttribute("Code")]
			property String ^Code {
				String^ get()
				{
					return _Code;
				}
				void set(String ^i_Code)
				{
					// ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
					if(((i_Code != nullptr) ? i_Code->Trim() : "") == ""){
						throw gcnew ArgumentNullException("i_Code");
					}
					_Code = i_Code->Trim()->ToLower();
				}
			}
			String ^Name;			// ���̌���̖��́iWikipedia�̏ꍇ�A�L�����j
			String ^ShortName;		// ���̌���̗���

			// �z��̃\�[�g�p���\�b�h
			virtual int CompareTo(Object ^obj){
				// ����R�[�h�Ń\�[�g
				LanguageName ^name = dynamic_cast<LanguageName^>(obj);
				return this->Code->CompareTo(name->Code);
			}
		private:
			// ����R�[�h�iproperty�j
			String ^_Code;
		};

		// �R���X�g���N�^�i�V���A���C�Y�p�j
		LanguageInformation(void){
//			System::Diagnostics::Debug::WriteLine("LanguageInformation::LanguageInformation > ��������Ȃ��R���X�g���N�^���g�p���Ă��܂�");
			// �K���Ȓl�Œʏ�̃R���X�g���N�^�����s
			this->LanguageInformation::LanguageInformation("unknown");
		}
		// �R���X�g���N�^�i�ʏ�j
		LanguageInformation(String ^i_Code){
			// �����o�ϐ��̗̈�m�ۂƏ����ݒ�
			Code = i_Code;
			Names = gcnew array<LanguageName>(1);
			Names[0].Code = i_Code;
			Names[0].Name = "";
			Names[0].ShortName = "";
		}
		// �f�X�g���N�^
		virtual ~LanguageInformation(){}

		// �z��̃\�[�g�p���\�b�h
		virtual int CompareTo(Object ^obj){
			// ����R�[�h�Ń\�[�g
			LanguageInformation ^lang = dynamic_cast<LanguageInformation^>(obj);
			return this->Code->CompareTo(lang->Code);
		}

		// �w�肵������ł̖��̂��擾
		String ^GetName(String ^i_Code){
			for each(LanguageName ^name in Names){
				if(name->Code == i_Code){
					return name->Name;
				}
			}
			return "";
		}

		// ����R�[�h
		[XmlAttributeAttribute("Code")]
		property String ^Code {
			String^ get()
			{
				return _Code;
			}
			void set(String ^i_Code)
			{
				// ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
				if(((i_Code != nullptr) ? i_Code->Trim() : "") == ""){
					throw gcnew ArgumentNullException("i_Code");
				}
				_Code = i_Code->Trim()->ToLower();
			}
		}
		// ���̌���́A�e����ł̖���
		array<LanguageName> ^Names;

	private:
		// ����R�[�h�iproperty�j
		String ^_Code;
	};

	// ������ƁA�֘A����T�[�o�[�����i�[����N���X
	public ref class LanguageWithServerInformation : public LanguageInformation {
	public:
		// �R���X�g���N�^�i�V���A���C�Y�p�j
		LanguageWithServerInformation(void)
			: LanguageInformation(){
//			System::Diagnostics::Debug::WriteLine("LanguageWithServerInformation::LanguageWithServerInformation > ��������Ȃ��R���X�g���N�^���g�p���Ă��܂�");
			// �K���Ȓl�Œʏ�̃R���X�g���N�^�����s
			this->LanguageWithServerInformation::LanguageWithServerInformation("unknown");
		}
		// �R���X�g���N�^�i�ʏ�j
		LanguageWithServerInformation(String ^i_Code)
			: LanguageInformation(i_Code){
			// �����l�ݒ�
			// �����̃N���X�͒�`�̂݁B���ۂ̐ݒ�́A�p�������N���X�ōs��
			Server = "unknown";
		}
		// �f�X�g���N�^
		virtual ~LanguageWithServerInformation(){}

		// �T�[�o�[�̖���
		property String ^Server {
			String^ get()
			{
				return _Server;
			}
			void set(String ^i_Name)
			{
				// ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
				if(((i_Name != nullptr) ? i_Name->Trim() : "") == ""){
					throw gcnew ArgumentNullException("i_Name");
				}
				_Server = i_Name->Trim();
			}
		}

	private:
		// �T�[�o�[�̖��́iproperty�j
		String ^_Server;
	};

}
#endif
