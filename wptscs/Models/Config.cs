// ================================================================================================
// <summary>
//      XML�ւ̐ݒ�ۑ��p�N���X�\�[�X</summary>
//
// <copyright file="Config.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// XML�ւ̐ݒ�ۑ��p�N���X�ł��B
    /// </summary>
    public class Config
    {
        /// <summary>
        /// �N���C�A���g�Ƃ��Ă̋@�\�֌W�̐ݒ��ۑ��B
        /// </summary>
        public ClientConfig Client;

        /// <summary>
        /// ���ꂲ�Ƃ̏��i�T�[�o�[�̐ݒ�Ȃǂ��j��ۑ��B
        /// </summary>
        [XmlArrayItem(typeof(LanguageInformation)),
        XmlArrayItem(typeof(LanguageWithServerInformation)),
        XmlArrayItem(typeof(WikipediaInformation))]
        public LanguageInformation[] Languages;

        /// <summary>
        /// �C���X�^���X�̃t�@�C�����B
        /// </summary>
        private string path;

        /// <summary>
        /// �R���X�g���N�^�i�ʏ�j�B
        /// </summary>
        public Config()
        {
            // �����o�ϐ��̗̈�m�ہE�����ݒ�
            this.Client = new ClientConfig();
            this.Languages = new LanguageInformation[0];
        }

        /// <summary>
        /// �R���X�g���N�^�i�t�@�C���ǂݍ��݂���j�B
        /// </summary>
        /// <param name="path">�ݒ�t�@�C���p�X�B</param>
        public Config(string path)
        {
            // �t�@�C������ݒ��ǂݍ���
            this.path = Honememo.Cmn.NullCheckAndTrim(path);
            if (this.Load() == false)
            {
                // ���s�����ꍇ�A�ʏ�̃R���X�g���N�^�Ɠ��������ŏ�����
                this.Client = new ClientConfig();
                this.Languages = new LanguageInformation[0];
            }
        }

        /// <summary>
        /// �v���O�����̏������[�h�������񋓒l�ł��B
        /// </summary>
        public enum RunType
        {
            /// <summary>
            /// Wikipedia�E�܂��͎o���T�C�g
            /// </summary>
            [XmlEnum(Name = "Wikipedia")]
            Wikipedia
        }

        /// <summary>
        /// �ݒ���t�@�C���ɏ����o���B
        /// </summary>
        /// <returns><c>true</c> �����o������</returns>
        public bool Save()
        {
            // �ݒ���V���A���C�Y��
            if (this.path == String.Empty)
            {
                return false;
            }

            return Honememo.Cmn.XmlSerialize(this, this.path);
        }

        /// <summary>
        /// �ݒ���t�@�C������ǂݍ��݁B
        /// </summary>
        /// <returns><c>true</c> �ǂݍ��ݐ���</returns>
        public bool Load()
        {
            // �ݒ���f�V���A���C�Y��
            if (this.path == String.Empty)
            {
                return false;
            }

            object obj = null;
            if (Honememo.Cmn.XmlDeserialize(ref obj, this.GetType(), this.path) == true)
            {
                Config config = obj as Config;
                if (config != null)
                {
                    this.Client = config.Client;
                    this.Languages = config.Languages;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// �w�肳�ꂽ�R�[�h�̌�����i�T�[�o�[���j���擾�B
        /// �� ���݂��Ȃ��ꍇ�A<c>null</c>
        /// </summary>
        /// <param name="code">����R�[�h�B</param>
        /// <param name="mode">�������[�h�B</param>
        /// <returns>������i�T�[�o�[���j</returns>
        public LanguageInformation GetLanguage(string code, RunType mode)
        {
            Type type;
            if (mode == RunType.Wikipedia)
            {
                type = typeof(WikipediaInformation);
            }
            else
            {
                type = typeof(LanguageInformation);
            }

            foreach (LanguageInformation lang in this.Languages)
            {
                if (lang.GetType() == type)
                {
                    if (lang.Code == code)
                    {
                        return lang;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// �w�肳�ꂽ�R�[�h�̌�����i�T�[�o�[���j���擾�iRunType�̌^�j�B
        /// </summary>
        /// <param name="code">����R�[�h�B</param>
        /// <returns>������i�T�[�o�[���j</returns>
        public LanguageInformation GetLanguage(string code)
        {
            return this.GetLanguage(code, this.Client.RunMode);
        }

        /// <summary>
        /// �N���C�A���g�Ƃ��Ă̋@�\�֌W�̐ݒ���i�[����N���X�ł��B
        /// </summary>
        public class ClientConfig
        {
            /// <summary>
            /// �v���O�����̏����ΏہB
            /// </summary>
            public RunType RunMode;

            /// <summary>
            /// ���s���ʂ�ۑ�����t�H���_�B
            /// </summary>
            public string SaveDirectory;

            /// <summary>
            /// �Ō�Ɏw�肵�Ă����|�󌳌���B
            /// </summary>
            public string LastSelectedSource;

            /// <summary>
            /// �Ō�Ɏw�肵�Ă����|��挾��B
            /// </summary>
            public string LastSelectedTarget;

            /// <summary>
            /// �ʐM���Ɏg�p����UserAgent�B
            /// </summary>
            public string UserAgent;

            /// <summary>
            /// �ʐM���Ɏg�p����Referer�B
            /// </summary>
            public string Referer;

            /// <summary>
            /// �R���X�g���N�^�B
            /// </summary>
            public ClientConfig()
            {
                // �����o�ϐ��̗̈�m�ہE�����ݒ�
                this.RunMode = RunType.Wikipedia;
                this.SaveDirectory = String.Empty;
                this.LastSelectedSource = "en";
                this.LastSelectedTarget = "ja";
                this.UserAgent = String.Empty;
                this.Referer = String.Empty;
            }
        }
    }
}
