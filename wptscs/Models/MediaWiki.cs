// ================================================================================================
// <summary>
//      MediaWiki�̃E�F�u�T�C�g�i�V�X�e���j������킷���f���N���X�\�[�X</summary>
//
// <copyright file="MediaWiki.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// MediaWiki�̃E�F�u�T�C�g�i�V�X�e���j������킷���f���N���X�ł��B
    /// </summary>
    public class MediaWiki : Website
    {
        #region �萔��`

        /// <summary>
        /// Wikipedia��XML�̌Œ�l�̏����B
        /// </summary>
        public static readonly string Xmlns = "http://www.mediawiki.org/xml/export-0.4/";

        /// <summary>
        /// �e���v���[�g�̖��O��Ԃ������ԍ��B
        /// </summary>
        public static readonly int TemplateNamespaceNumber = 10;

        /// <summary>
        /// �J�e�S���̖��O��Ԃ������ԍ��B
        /// </summary>
        public static readonly int CategoryNamespaceNumber = 14;

        /// <summary>
        /// �摜�̖��O��Ԃ������ԍ��B
        /// </summary>
        public static readonly int ImageNamespaceNumber = 6;

        #endregion

        #region private�ϐ�

        // ���e�ϐ��̏����l��2006�N9�����_��Wikipedia�p��ł��

        /// <summary>
        /// Wikipedia�����̃V�X�e����`�ϐ��B
        /// </summary>
        [XmlArrayItem("Variable")]
        public string[] SystemVariables = new string[]
        {
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
                "#if:"
        };

        /// <summary>
        /// ���ʂ̃t�H�[�}�b�g�B
        /// </summary>
        public string Bracket = " ({0}) ";

        /// <summary>
        /// ���_�C���N�g�̕�����B
        /// </summary>
        public string Redirect = "#REDIRECT";

        /// <summary>
        /// ���O��Ԃ̐ݒ�B
        /// </summary>
        [XmlIgnoreAttribute()]
        public IDictionary<int, string> Namespaces = new Dictionary<int, string>();

        /// <summary>
        /// ���o���̒�^��B
        /// </summary>
        [XmlArrayItem("Title")]
        public string[] TitleKeys = new string[0];

        /// <summary>
        /// �L����XML�f�[�^�����݂���p�X�B
        /// </summary>
        private string exportPath = "wiki/Special:Export/";

        #endregion

        #region �R���X�g���N�^

        /// <summary>
        /// �R���X�g���N�^�i�V���A���C�Y�p�j�B
        /// </summary>
        public MediaWiki()
            : this(new Language("unknown"))
        {
            // �K���Ȓl�Œʏ�̃R���X�g���N�^�����s
            System.Diagnostics.Debug.WriteLine("MediaWiki.MediaWiki > ��������Ȃ��R���X�g���N�^���g�p���Ă��܂�");
        }

        /// <summary>
        /// �R���X�g���N�^�i�ʏ�j�B
        /// </summary>
        /// <param name="lang">�E�F�u�T�C�g�̌���B</param>
        public MediaWiki(Language lang)
            : base(lang)
        {
            // �����o�ϐ��̏����ݒ�
            this.Server = String.Format("{0}.wikipedia.org", lang.Code);
        }

        #endregion

        #region �v���p�e�B

        /// <summary>
        /// �y�[�W���擾�B
        /// </summary>
        /// <param name="title">�y�[�W�^�C�g���B</param>
        /// <returns>�擾�����y�[�W�B�y�[�W�����݂��Ȃ��ꍇ�� <c>null</c> ��Ԃ��B</returns>
        /// <remarks>�擾�ł��Ȃ��ꍇ�i�ʐM�G���[�Ȃǁj�͗�O�𓊂���B</remarks>
        public override Page this[string title]
        {
            get
            {
                // �������ƒl�`�F�b�N
                _Xml = null;
                _GetArticleStatus = HttpStatusCode.PaymentRequired;
                _GetArticleException = null;
                // �L����XML�f�[�^��Wikipedia�T�[�o�[����擾
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                    // UserAgent�ݒ�
                    // ��Wikipedia��UserAgent����̏ꍇ�G���[�ƂȂ�̂ŁA�K���ݒ肷��
                    if (!String.IsNullOrEmpty(i_UserAgent))
                    {
                        req.UserAgent = i_UserAgent;
                    }
                    else
                    {
                        Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                        req.UserAgent = "WikipediaTranslationSupportTool/" + ver.Major + "." + String.Format("{0:D2}", ver.Minor);
                    }
                    // Referer�ݒ�
                    if (!String.IsNullOrEmpty(i_Referer))
                    {
                        req.Referer = i_Referer;
                    }
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    _GetArticleStatus = res.StatusCode;

                    // �����f�[�^����M���邽�߂�Stream���擾���A�f�[�^���擾
                    // ���擾����XML�����킩�́A�����ł͊m�F���Ȃ�
                    _Xml = new XmlDocument();
                    _Xml.Load(res.GetResponseStream());
                    res.Close();

                    // �擾����XML���ꎞ�t�H���_�ɕۑ�
                    try
                    {
                        // �ꎞ�t�H���_���m�F
                        String tmpDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Application.ExecutablePath));
                        if (Directory.Exists(tmpDir) == false)
                        {
                            // �ꎞ�t�H���_���쐬
                            Directory.CreateDirectory(tmpDir);
                        }
                        // �t�@�C���̕ۑ�
                        Xml.Save(Path.Combine(tmpDir, Honememo.Cmn.ReplaceInvalidFileNameChars(Title) + ".xml"));
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("WikipediaArticle.getServerArticle > �ꎞ�t�@�C���̕ۑ��Ɏ��s���܂��� : " + e.Message);
                    }
                }
                catch (WebException e)
                {
                    // ProtocolError�G���[�̏ꍇ�A�X�e�[�^�X�R�[�h��ێ�
                    _Xml = null;
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        _GetArticleStatus = ((HttpWebResponse)e.Response).StatusCode;
                    }
                    _GetArticleException = e;
                    return false;
                }
                catch (Exception e)
                {
                    _Xml = null;
                    _GetArticleException = e;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// �L����XML�f�[�^�����݂���p�X�B
        /// </summary>
        public string ExportPath
        {
            get
            {
                return this.exportPath;
            }

            set
            {
                this.exportPath = value;
            }
        }

        #endregion

        #region ���\�b�h

        /// <summary>
        /// �w�肵������ł̌��ꖼ�̂� �y�[�W��|���� �̌`���Ŏ擾�B
        /// </summary>
        /// <param name="code">����̃R�[�h�B</param>
        /// <returns>�y�[�W��|���̌`���̌��ꖼ�́B</returns>
        public string GetFullName(string code)
        {
            if (Lang.Names.ContainsKey(code))
            {
                Language.LanguageName name = Lang.Names[code];
                if (!String.IsNullOrEmpty(name.ShortName))
                {
                    return name.Name + "|" + name.ShortName;
                }
                else
                {
                    return name.Name;
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// �w�肳�ꂽ������Wikipedia�̃V�X�e���ϐ��ɑ������𔻒�B
        /// </summary>
        /// <param name="text">�`�F�b�N���镶����B</param>
        /// <returns><c>true</c> �V�X�e���ϐ��ɑ����B</returns>
        public bool ChkSystemVariable(string text)
        {
            string s = text != null ? text : String.Empty;

            // ��{�͑S����v�����A�萔�� : �ŏI����Ă���ꍇ�Atext��:���O�݂̂��r
            // �� {{ns:1}}�݂����ȏꍇ�ɔ�����
            foreach (string variable in this.SystemVariables)
            {
                if (variable.EndsWith(":") == true)
                {
                    if (s.StartsWith(variable) == true)
                    {
                        return true;
                    }
                }
                else if (s == variable)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
