using System;
using System.Xml.Serialization;

namespace wptscs.model
{
    // ����Ɋւ�������i�[����N���X
    public class LanguageInformation : IComparable
    {
        // ���錾��́A�e����ł̖��́E���̂��i�[���邽�߂̍\����
        public struct LanguageName : IComparable
        {
			// ����R�[�h
			[XmlAttributeAttribute("Code")]
			public String Code {
				get {
                    return _Code;
				}
				set {
					// ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
					if(((value != null) ? value.Trim() : "") == ""){
						throw new ArgumentNullException("value");
					}
					_Code = value.Trim().ToLower();
				}
			}
			public String Name;			// ���̌���̖��́iWikipedia�̏ꍇ�A�L�����j
			public String ShortName;		// ���̌���̗���

			// �z��̃\�[�g�p���\�b�h
            public int CompareTo(Object obj)
            {
				// ����R�[�h�Ń\�[�g
                LanguageName name = (LanguageName)obj;
				return this.Code.CompareTo(name.Code);
			}
			// ����R�[�h�iproperty�j
			private String _Code;
		};

		// �R���X�g���N�^�i�V���A���C�Y�p�j
        public LanguageInformation() : this("unknown")
        {
//			System.Diagnostics.Debug.WriteLine("LanguageInformation.LanguageInformation > ��������Ȃ��R���X�g���N�^���g�p���Ă��܂�");
			// �K���Ȓl�Œʏ�̃R���X�g���N�^�����s
		}
		// �R���X�g���N�^�i�ʏ�j
		public LanguageInformation(String i_Code){
			// �����o�ϐ��̗̈�m�ۂƏ����ݒ�
			Code = i_Code;
			Names = new LanguageName[1];
			Names[0].Code = i_Code;
			Names[0].Name = "";
			Names[0].ShortName = "";
		}

		// �z��̃\�[�g�p���\�b�h
        public virtual int CompareTo(Object obj)
        {
			// ����R�[�h�Ń\�[�g
            LanguageInformation lang = obj as LanguageInformation;
			return this.Code.CompareTo(lang.Code);
		}

		// �w�肵������ł̖��̂��擾
		public String GetName(String i_Code){
			foreach(LanguageName name in Names){
				if(name.Code == i_Code){
					return name.Name;
				}
			}
			return "";
		}

		// ����R�[�h
		[XmlAttributeAttribute("Code")]
		public String Code {
			get {
				return _Code;
			}
			set {
				// ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
				if(((value != null) ? value.Trim() : "") == ""){
					throw new ArgumentNullException("value");
				}
				_Code = value.Trim().ToLower();
			}
		}
		// ���̌���́A�e����ł̖���
		public LanguageName[] Names;

		// ����R�[�h�iproperty�j
		private String _Code;
    }
}
