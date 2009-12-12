using System;
using System.Xml.Serialization;

namespace wptscs.model
{
    // XML�ւ̐ݒ�ۑ��p�N���X
    public class Config
    {
        // �v���O�����̏������[�h�������񋓒l
		public enum RunType{
			[XmlEnum(Name = "Wikipedia")]
			Wikipedia};

		// �N���C�A���g�Ƃ��Ă̋@�\�֌W�̐ݒ���i�[����N���X
		public class ClientConfig {
			// �R���X�g���N�^
			public ClientConfig(){
				// �����o�ϐ��̗̈�m�ہE�����ݒ�
				RunMode = RunType.Wikipedia;
				SaveDirectory = "";
				LastSelectedSource = "en";
				LastSelectedTarget = "ja";
				UserAgent = "";
				Referer = "";
			}

			// �v���O�����̏����Ώ�
			public RunType RunMode;

			// ���s���ʂ�ۑ�����t�H���_
			public String SaveDirectory;

			// �Ō�Ɏw�肵�Ă����|�󌳌���
			public String LastSelectedSource;
			// �Ō�Ɏw�肵�Ă����|��挾��
			public String LastSelectedTarget;

			// �ʐM���Ɏg�p����UserAgent
			public String UserAgent;
			// �ʐM���Ɏg�p����Referer
			public String Referer;
		};

		// �R���X�g���N�^�i�ʏ�j
		public Config(){
			// �����o�ϐ��̗̈�m�ہE�����ݒ�
			Client = new ClientConfig();
			Languages = new LanguageInformation[0];
		}
		// �R���X�g���N�^�i�t�@�C���ǂݍ��݂���j
		public Config(String i_Path){
			// �t�@�C������ݒ��ǂݍ���
			path = MYAPP.Cmn.NullCheckAndTrim(i_Path);
			if(Load() == false){
                // ���s�����ꍇ�A�ʏ�̃R���X�g���N�^�Ɠ��������ŏ�����
                Client = new ClientConfig();
                Languages = new LanguageInformation[0];
			}
		}

		// �ݒ���t�@�C���ɏ����o��
		public bool Save(){
			// �ݒ���V���A���C�Y��
			if(path == ""){
				return false;
			}
			return MYAPP.Cmn.XmlSerialize(this, path);
		}
		// �ݒ���t�@�C������ǂݍ���
		public bool Load(){
			// �ݒ���f�V���A���C�Y��
			if(path == ""){
				return false;
			}
			Object obj = null;
			if(MYAPP.Cmn.XmlDeserialize(ref obj, this.GetType(), path) == true){
                Config config = obj as Config;
				if(config != null){
					this.Client = config.Client;
					this.Languages = config.Languages;
					return true;
				}
			}
			return false;
		}

		// �w�肳�ꂽ�R�[�h�̌�����i�T�[�o�[���j���擾
		// �����݂��Ȃ��ꍇ�Anull
		public LanguageInformation GetLanguage(String i_Code, RunType i_Mode){
			Type type;
			if(i_Mode == RunType.Wikipedia){
				type = typeof(WikipediaInformation);
			}
			else{
                type = typeof(LanguageInformation);
			}
			foreach(LanguageInformation lang in Languages){
				if(lang.GetType() == type){
					if(lang.Code == i_Code){
						return lang;
					}
				}
			}
			return null;
		}
		// �w�肳�ꂽ�R�[�h�̌�����i�T�[�o�[���j���擾�iRunType�̌^�j
		public LanguageInformation GetLanguage(String i_Code){
			return GetLanguage(i_Code, Client.RunMode);
		}

		// �N���C�A���g�Ƃ��Ă̋@�\�֌W�̐ݒ��ۑ�
		public ClientConfig Client;

		// ���ꂲ�Ƃ̏��i�T�[�o�[�̐ݒ�Ȃǂ��j��ۑ�
		[XmlArrayItem(typeof(LanguageInformation)),
		XmlArrayItem(typeof(LanguageWithServerInformation)),
		XmlArrayItem(typeof(WikipediaInformation))]
		public LanguageInformation[] Languages;

		// �C���X�^���X�̃t�@�C����
		private String path;
    }
}
