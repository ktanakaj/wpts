using System;
using System.Xml.Serialization;

namespace wptscs.model
{
    // ������E�T�[�o�[���ɉ����AWikipedia�̃T�[�o�[���Ƃ̐ݒ���i�[����N���X
    public class WikipediaInformation : LanguageWithServerInformation
    {
        // �e�T�[�o�[�ł̖��O��Ԃ̐ݒ���i�[���邽�߂̍\����
		public struct Namespace : IComparable {
			public int Key;				// ���O��Ԃ̔ԍ�
			public String Name;			// ���O��Ԗ�

			// �z��̃\�[�g�p���\�b�h
            public int CompareTo(Object obj)
            {
	            // ���O��Ԃ̔ԍ��Ń\�[�g
                Namespace ns = (Namespace) obj;
	            return this.Key.CompareTo(ns.Key);
            }
		};

		// �R���X�g���N�^�i�V���A���C�Y�p�j
        public WikipediaInformation() : this("unknown")
        {
//			System.Diagnostics.Debug.WriteLine("WikipediaInformation.WikipediaInformation > ��������Ȃ��R���X�g���N�^���g�p���Ă��܂�");
			// �K���Ȓl�Œʏ�̃R���X�g���N�^�����s
		}
		// �R���X�g���N�^�i�ʏ�j
		public WikipediaInformation(String i_Code) : base(i_Code){
			// �����l�ݒ�
			setDefault();
		}
    
        /* �����o�ϐ��̏����l�ݒ� */
        public void setDefault()
        {
	        // �����o�ϐ��̗̈�m�ہE�����ݒ�
	        // ���e�����l��2006�N9�����_��Wikipedia�p��ł��
	        Server = String.Format("{0}.wikipedia.org", Code);
	        ArticleXmlPath = "wiki/Special:Export/";
	        SystemVariables = new String[]{
		        "CURRENTMONTH",
		        "CURRENTMONTHNAME",
		        "CURRENTDAY",
		        "CURRENTDAYNAME",
		        "CURRENTYEAR",
		        "CURRENTTIME",
		        "NUMBEROFARTICLES",
		        "SITENAME",
		        "SERVER",
		        "NAMESPACE",
		        "PAGENAME",
		        "ns:",
		        "localurl:",
		        "fullurl:",
		        "#if:"};
	        Bracket = " ({0}) ";
	        Redirect = "#REDIRECT";
	        Namespaces = new Namespace[0];
	        TitleKeys = new String[0];
        }

        /* �w�肵������ł̖��̂� �L����|���� �̌`���Ŏ擾 */
        public String GetFullName(String i_Code)
        {
	        foreach(LanguageName name in Names){
		        if(name.Code == i_Code){
			        if(name.ShortName != ""){
				        return (name.Name + "|" + name.ShortName);
			        }
			        else{
				        return name.Name;
			        }
		        }
	        }
	        return "";
        }

        /* �w�肳�ꂽ�ԍ��̖��O��Ԃ��擾 */
        public String GetNamespace(int i_Key)
        {
	        foreach(Namespace ns in Namespaces){
		        if(ns.Key == i_Key){
			        return ns.Name;
		        }
	        }
	        return "";
        }

        /* �w�肳�ꂽ������Wikipedia�̃V�X�e���ϐ��ɑ������𔻒� */
        public bool ChkSystemVariable(String i_Text)
        {
	        String text = ((i_Text != null) ? i_Text : "");
	        // ��{�͑S����v�����A�萔�� : �ŏI����Ă���ꍇ�Atext��:���O�݂̂��r
	        // �� {{ns:1}}�݂����ȏꍇ�ɔ�����
	        foreach(String variable in SystemVariables){
		        if(variable.EndsWith(":") == true){
			        if(text.StartsWith(variable) == true){
				        return true;
			        }
		        }
		        else if(text == variable){
			        return true;
		        }
	        }
	        return false;
        }

		// �L����XML�f�[�^�����݂���p�X
		public String ArticleXmlPath {
			get {
				return _ArticleXmlPath;
			}
			set {
				_ArticleXmlPath = ((value != null) ? value.Trim() : "");
			}
		}

		// Wikipedia�����̃V�X�e����`�ϐ�
		[XmlArrayItem("Variable")]
		public String[] SystemVariables;

		// ���ʂ̃t�H�[�}�b�g
		public String Bracket;
		// ���_�C���N�g�̕�����
		public String Redirect;

		// ���O��Ԃ̐ݒ�
		[XmlIgnoreAttribute()]
		public Namespace[] Namespaces;
		// �e���v���[�g�E�J�e�S���E�摜�̖��O��Ԃ������ԍ�
		public static readonly int TEMPLATENAMESPACENUMBER = 10;
        public static readonly int CATEGORYNAMESPACENUMBER = 14;
        public static readonly int IMAGENAMESPACENUMBER = 6;

		// ���o���̒�^��
		[XmlArrayItem("Title")]
		public String[] TitleKeys;

		// �L����XML�f�[�^�����݂���p�X�iproperty�j
		private String _ArticleXmlPath;
    }
}
