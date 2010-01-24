// ================================================================================================
// <summary>
//      MediaWiki�̃y�[�W������킷���f���N���X�\�[�X</summary>
//
// <copyright file="MediaWikiPage.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// MediaWiki�̃y�[�W������킷���f���N���X�ł��B
    /// </summary>
    public class MediaWikiPage : Page
    {
        #region �萔�錾

        /// <summary>
        /// �R�����g�̊J�n�B
        /// </summary>
        public static readonly string CommentStart = "<!--";

        /// <summary>
        /// �R�����g�̏I���B
        /// </summary>
        public static readonly string CommentEnd = "-->";

        /// <summary>
        /// nowiki�̊J�n�B
        /// </summary>
        public static readonly string NowikiStart = "<nowiki>";

        /// <summary>
        /// nowiki�̏I���B
        /// </summary>
        public static readonly string NowikiEnd = "</nowiki>";

        /// <summary>
        /// msgnw�̏����B
        /// </summary>
        public static readonly string Msgnw = "msgnw:";

        #endregion

        #region private�ϐ�

        /// <summary>
        /// �y�[�W��XML�f�[�^�B
        /// </summary>
        private XmlDocument xml;

        /// <summary>
        /// ���_�C���N�g��̃y�[�W���B
        /// </summary>
        private string redirect;

        #endregion

        #region �R���X�g���N�^

        /// <summary>
        /// �R���X�g���N�^�i�y�[�W���j�B
        /// </summary>
        /// <param name="website">�y�[�W����������E�F�u�T�C�g�B</param>
        /// <param name="title">�y�[�W�^�C�g���B</param>
        /// <param name="text">�y�[�W�̖{���B</param>
        /// <param name="timestamp">�y�[�W�̃^�C���X�^���v�B</param>
        public MediaWikiPage(MediaWiki website, string title, string text, DateTime timestamp)
            : base(website, title, text, timestamp)
        {
        }

        /// <summary>
        /// �R���X�g���N�^�i�y�[�W���j�B
        /// �y�[�W�̃^�C���X�^���v�ɂ͌��ݓ��� (UTC) ��ݒ�B
        /// </summary>
        /// <param name="website">�y�[�W����������E�F�u�T�C�g�B</param>
        /// <param name="title">�y�[�W�^�C�g���B</param>
        /// <param name="text">�y�[�W�̖{���B</param>
        public MediaWikiPage(MediaWiki website, string title, string text)
            : base(website, title, text)
        {
        }

        /// <summary>
        /// �R���X�g���N�^�iXML�j�B
        /// </summary>
        /// <param name="website">�y�[�W����������E�F�u�T�C�g�B</param>
        /// <param name="xml">MediaWiki�ŃG�N�X�|�[�g�����y�[�WXML�B</param>
        public MediaWikiPage(MediaWiki website, XmlDocument xml)
            : base(website, null, null)
        {
            // ���̃R���X�g���N�^�ł�XML�͕K�{
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            // �����ݒ�
            this.Xml = xml;

            // XML����͂��A�����o�ɐݒ�
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(this.Xml.NameTable);
            nsmgr.AddNamespace("ns", MediaWiki.Xmlns);
            XmlElement pageElement = this.Xml.SelectSingleNode("/ns:mediawiki/ns:page", nsmgr) as XmlElement;
            if (pageElement != null)
            {
                // �L�����̏㏑��
                XmlElement titleElement = pageElement.SelectSingleNode("ns:title", nsmgr) as XmlElement;
                this.Title = titleElement.InnerText;

                // �ŏI�X�V����
                XmlElement timeElement = pageElement.SelectSingleNode("ns:revision/ns:timestamp", nsmgr) as XmlElement;
                this.Timestamp = DateTime.Parse(timeElement.InnerText);

                // �L���{��
                XmlElement textElement = pageElement.SelectSingleNode("ns:revision/ns:text", nsmgr) as XmlElement;
                this.Text = textElement.InnerText;

                // ���_�C���N�g�̃`�F�b�N���s���A�����l���X�V����
                this.IsRedirect();
            }
        }

        #endregion

        #region �v���p�e�B

        /// <summary>
        /// �y�[�W��XML�f�[�^�B
        /// </summary>
        public XmlDocument Xml
        {
            get
            {
                return this.xml;
            }

            protected set
            {
                this.xml = value;
            }
        }

        /// <summary>
        /// ���_�C���N�g��̃y�[�W���B
        /// </summary>
        public string Redirect
        {
            get
            {
                return this.redirect;
            }

            protected set
            {
                this.redirect = value;
            }
        }

        #endregion

        #region �ÓI���\�b�h

        /// <summary>
        /// nowiki��Ԃ̃`�F�b�N�B
        /// </summary>
        /// <param name="o_Text"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <returns></returns>
        public static int ChkNowiki(ref string o_Text, string i_Text, int i_Index)
        {
            // �o�͒l������
            int lastIndex = -1;
            o_Text = String.Empty;

            // ���͒l�m�F
            if (Honememo.Cmn.ChkTextInnerWith(i_Text.ToLower(), i_Index, NowikiStart.ToLower()) == false)
            {
                return lastIndex;
            }

            // �u���b�N�I���܂Ŏ擾
            for (int i = i_Index + NowikiStart.Length; i < i_Text.Length; i++)
            {
                // �I�������̃`�F�b�N
                if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, NowikiEnd))
                {
                    lastIndex = i + NowikiEnd.Length - 1;
                    break;
                }

                // �R�����g�i<!--�j�̃`�F�b�N
                string dummy = String.Empty;
                int index = ChkComment(ref dummy, i_Text, i);
                if (index != -1)
                {
                    i = index;
                    continue;
                }
            }

            // �I��肪������Ȃ��ꍇ�́A�S��nowiki�u���b�N�Ɣ��f
            if (lastIndex == -1)
            {
                lastIndex = i_Text.Length - 1;
            }

            o_Text = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
            return lastIndex;
        }

        /// <summary>
        /// �R�����g��Ԃ̃`�F�b�N�B
        /// </summary>
        /// <param name="o_Text"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <returns></returns>
        public static int ChkComment(ref string o_Text, string i_Text, int i_Index)
        {
            // �o�͒l������
            int lastIndex = -1;
            o_Text = String.Empty;

            // ���͒l�m�F
            if (Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, CommentStart) == false)
            {
                return lastIndex;
            }

            // �R�����g�I���܂Ŏ擾
            for (int i = i_Index + CommentStart.Length; i < i_Text.Length; i++)
            {
                if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, CommentEnd))
                {
                    lastIndex = i + CommentEnd.Length - 1;
                    break;
                }
            }

            // �I��肪������Ȃ��ꍇ�́A�S�ăR�����g�Ɣ��f
            if (lastIndex == -1)
            {
                lastIndex = i_Text.Length - 1;
            }

            o_Text = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
            return lastIndex;
        }

        #endregion

        #region �C���X�^���X���\�b�h

        /// <summary>
        /// �w�肳�ꂽ����R�[�h�ւ̌���ԃ����N��Ԃ��B
        /// </summary>
        /// <param name="code">����R�[�h�B</param>
        /// <returns>����ԃ����N��̋L�����B������Ȃ��ꍇ�͋�B</returns>
        public string GetInterWiki(string code)
        {
            // �������ƒl�`�F�b�N
            string interWiki = String.Empty;
            if (String.IsNullOrEmpty(Text))
            {
                // �y�[�W�{�����ݒ肳��Ă��Ȃ��ꍇ���s�s��
                throw new InvalidOperationException();
            }

            // �L���ɑ��݂���w�茾��ւ̌���ԃ����N���擾
            for (int i = 0; i < Text.Length; i++)
            {
                // �R�����g�i<!--�j�̃`�F�b�N
                string comment = String.Empty;
                int index = this.ChkComment(ref comment, i);
                if (index != -1)
                {
                    i = index;
                }
                else if (Honememo.Cmn.ChkTextInnerWith(Text, i, "[[" + code + ":") == true)
                {
                    // �w�茾��ւ̌���ԃ����N�̏ꍇ�A���e���擾���A�����I��
                    Link link = this.ParseInnerLink(Text.Substring(i));
                    if (!String.IsNullOrEmpty(link.Text))
                    {
                        interWiki = link.Article;
                        break;
                    }
                }
            }

            return interWiki;
        }

        /// <summary>
        /// �y�[�W�����_�C���N�g�����`�F�b�N�B
        /// </summary>
        /// <returns><c>true</c> ���_�C���N�g�B</returns>
        public bool IsRedirect()
        {
            // �l�`�F�b�N
            if (Text == String.Empty)
            {
                // �y�[�W�{�����ݒ肳��Ă��Ȃ��ꍇ���s�s��
                throw new InvalidOperationException();
            }

            // �w�肳�ꂽ�y�[�W�����_�C���N�g�y�[�W�i#REDIRECT���j�����`�F�b�N
            // �����{��ł݂����ɁA#REDIRECT�ƌ���ŗL��#�]���݂����Ȃ̂�����Ǝv����̂ŁA
            //   �|�󌳌���Ɖp��ł̐ݒ�Ń`�F�b�N
            for (int i = 0; i < 2; i++)
            {
                string redirect = (Website as MediaWiki).Redirect.Clone() as string;
                if (i == 1)
                {
                    if (Website.Lang.Code == "en")
                    {
                        continue;
                    }

                    MediaWiki en = new MediaWiki(new Language("en"));
                    redirect = en.Redirect;
                }

                if (redirect != String.Empty)
                {
                    if (Text.ToLower().StartsWith(redirect.ToLower()))
                    {
                        Link link = this.ParseInnerLink(Text.Substring(redirect.Length).TrimStart());
                        if (!String.IsNullOrEmpty(link.Text))
                        {
                            this.Redirect = link.Article;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// �y�[�W���J�e�S���[�����`�F�b�N�B
        /// </summary>
        /// <returns><c>true</c> �J�e�S���[�B</returns>
        public bool IsCategory()
        {
            return this.IsCategory(Title);
        }

        /// <summary>
        /// �y�[�W���摜�����`�F�b�N�B
        /// </summary>
        /// <returns><c>true</c> �摜�B</returns>
        public bool IsImage()
        {
            return this.IsImage(Title);
        }

        /// <summary>
        /// �y�[�W���W�����O��ԈȊO�����`�F�b�N�B
        /// </summary>
        /// <returns><c>true</c> �W�����O��ԈȊO�B</returns>
        public bool IsNotMainNamespace()
        {
            return this.IsNotMainNamespace(Title);
        }

        /// <summary>
        /// �n���ꂽ�y�[�W�����J�e�S���[�����`�F�b�N�B
        /// </summary>
        /// <param name="title">�y�[�W���B</param>
        /// <returns><c>true</c> �J�e�S���[�B</returns>
        public bool IsCategory(string title)
        {
            // �w�肳�ꂽ�L�������J�e�S���[�iCategory:���Ŏn�܂�j�����`�F�b�N
            string category = (Website as MediaWiki).Namespaces[MediaWiki.CategoryNamespaceNumber];
            if (category != String.Empty)
            {
                if (title.ToLower().StartsWith(category.ToLower() + ":"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// �n���ꂽ�y�[�W�����摜�����`�F�b�N�B
        /// </summary>
        /// <param name="title">�y�[�W���B</param>
        /// <returns><c>true</c> �摜�B</returns>
        public bool IsImage(string title)
        {
            // �w�肳�ꂽ�y�[�W�����摜�iImage:���Ŏn�܂�j�����`�F�b�N
            // �����{��ł݂����ɁAimage: �ƌ���ŗL�� �摜: �݂����Ȃ̂�����Ǝv����̂ŁA
            //   �|�󌳌���Ɖp��ł̐ݒ�Ń`�F�b�N
            for (int i = 0; i < 2; i++)
            {
                string image = (Website as MediaWiki).Namespaces[MediaWiki.ImageNamespaceNumber];
                if (i == 1)
                {
                    if (Website.Lang.Code == "en")
                    {
                        continue;
                    }

                    MediaWiki en = new MediaWiki(new Language("en"));
                    image = en.Namespaces[MediaWiki.ImageNamespaceNumber];
                }

                if (image != String.Empty)
                {
                    if (title.ToLower().StartsWith(image.ToLower() + ":") == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// �n���ꂽ�y�[�W�����W�����O��ԈȊO�����`�F�b�N�B
        /// </summary>
        /// <param name="title">�y�[�W���B</param>
        /// <returns><c>true</c> �W�����O��ԈȊO�B</returns>
        public bool IsNotMainNamespace(string title)
        {
            // �w�肳�ꂽ�y�[�W�����W�����O��ԈȊO�̖��O��ԁiWikipedia:���Ŏn�܂�j�����`�F�b�N
            foreach (string ns in (Website as MediaWiki).Namespaces.Values)
            {
                if (title.ToLower().StartsWith(ns.ToLower() + ":") == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// �n���ꂽWikipedia�̓��������N����́B
        /// </summary>
        /// <param name="i_Text">[[�Ŏn�܂镶����</param>
        /// <returns>�����N�B</returns>
        public Link ParseInnerLink(string i_Text)
        {
            // �o�͒l������
            Link result = new Link();
            result.Initialize();

            // ���͒l�m�F
            if (i_Text.StartsWith("[[") == false)
            {
                return result;
            }

            // �\������͂��āA[[]]�����̕�������擾
            // ���\����Wikipedia�̃v���r���[�ŐF�X�����Ċm�F�A����Ȃ�������Ԉ���Ă��肷�邩���E�E�E
            string article = String.Empty;
            string section = String.Empty;
            string[] pipeTexts = new string[0];
            int lastIndex = -1;
            int pipeCounter = 0;
            bool sharpFlag = false;
            for (int i = 2; i < i_Text.Length; i++)
            {
                char c = i_Text[i];

                // ]]������������A��������I��
                if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, "]]") == true)
                {
                    lastIndex = ++i;
                    break;
                }

                // | ���܂܂�Ă���ꍇ�A�ȍ~�̕�����͕\�����ȂǂƂ��Ĉ���
                if (c == '|')
                {
                    ++pipeCounter;
                    Honememo.Cmn.AddArray(ref pipeTexts, String.Empty);
                    continue;
                }

                // �ϐ��i[[{{{1}}}]]�Ƃ��j�̍ċA�`�F�b�N
                string dummy = String.Empty;
                string variable = String.Empty;
                int index = this.ChkVariable(ref variable, ref dummy, i_Text, i);
                if (index != -1)
                {
                    i = index;
                    if (pipeCounter > 0)
                    {
                        pipeTexts[pipeCounter - 1] += variable;
                    }
                    else if (sharpFlag)
                    {
                        section += variable;
                    }
                    else
                    {
                        article += variable;
                    }

                    continue;
                }

                // | �̑O�̂Ƃ�
                if (pipeCounter <= 0)
                {
                    // �ϐ��ȊO�� { } �܂��� < > [ ] \n ���܂܂�Ă���ꍇ�A�����N�͖���
                    if ((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}') || (c == '\n'))
                    {
                        break;
                    }

                    // # �̑O�̂Ƃ�
                    if (!sharpFlag)
                    {
                        // #���܂܂�Ă���ꍇ�A�ȍ~�̕�����͌��o���ւ̃����N�Ƃ��Ĉ����i1�߂�#�̂ݗL���j
                        if (c == '#')
                        {
                            sharpFlag = true;
                        }
                        else
                        {
                            article += c;
                        }
                    }
                    else
                    {
                        // # �̌�̂Ƃ�
                        section += c;
                    }
                }
                else
                {
                    // | �̌�̂Ƃ�
                    // �R�����g�i<!--�j���܂܂�Ă���ꍇ�A�����N�͖���
                    if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, CommentStart))
                    {
                        break;
                    }

                    // nowiki�̃`�F�b�N
                    string nowiki = String.Empty;
                    index = ChkNowiki(ref nowiki, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += nowiki;
                        continue;
                    }

                    // �����N [[ {{ �i[[image:xx|[[test]]�̉摜]]�Ƃ��j�̍ċA�`�F�b�N
                    Link link = new Link();
                    index = this.ChkLinkText(ref link, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += link.Text;
                        continue;
                    }

                    pipeTexts[pipeCounter - 1] += c;
                }
            }

            // ��͂ɐ��������ꍇ�A���ʂ�߂�l�ɐݒ�
            if (lastIndex != -1)
            {
                // �ϐ��u���b�N�̕�����������N�̃e�L�X�g�ɐݒ�
                result.Text = i_Text.Substring(0, lastIndex + 1);

                // �O��̃X�y�[�X�͍폜�i���o���͌��̂݁j
                result.Article = article.Trim();
                result.Section = section.TrimEnd();

                // | �ȍ~�͂��̂܂ܐݒ�
                result.PipeTexts = pipeTexts;

                // �L����������𒊏o
                // �T�u�y�[�W
                if (result.Article.StartsWith("/"))
                {
                    result.SubPageFlag = true;
                }
                else if (result.Article.StartsWith(":"))
                {
                    // �擪�� :
                    result.StartColonFlag = true;
                    result.Article = result.Article.TrimStart(':').TrimStart();
                }

                // �W�����O��ԈȊO��[[xxx:yyy]]�̂悤�ɂȂ��Ă���ꍇ�A����R�[�h
                if (result.Article.Contains(":") && !this.IsNotMainNamespace(result.Article))
                {
                    // ���{���́A����R�[�h���̈ꗗ�����A�����ƈ�v������̂��E�E�E�Ƃ��ׂ����낤���A
                    //   �����e������Ȃ��̂� : ���܂ޖ��O��ԈȊO��S�Č���R�[�h���Ɣ���
                    result.Code = result.Article.Substring(0, result.Article.IndexOf(':')).TrimEnd();
                    result.Article = result.Article.Substring(result.Article.IndexOf(':') + 1).TrimStart();
                }
            }

            return result;
        }

        /// <summary>
        /// �n���ꂽWikipedia�̃e���v���[�g����́B
        /// </summary>
        /// <param name="i_Text">{{�Ŏn�܂镶����</param>
        /// <returns>�e���v���[�g�̃����N�B</returns>
        public Link ParseTemplate(string i_Text)
        {
            // �o�͒l������
            Link result = new Link();
            result.Initialize();
            result.TemplateFlag = true;

            // ���͒l�m�F
            if (i_Text.StartsWith("{{") == false)
            {
                return result;
            }

            // �\������͂��āA{{}}�����̕�������擾
            // ���\����Wikipedia�̃v���r���[�ŐF�X�����Ċm�F�A����Ȃ�������Ԉ���Ă��肷�邩���E�E�E
            string article = String.Empty;
            string[] pipeTexts = new string[0];
            int lastIndex = -1;
            int pipeCounter = 0;
            for (int i = 2; i < i_Text.Length; i++)
            {
                char c = i_Text[i];

                // }}������������A��������I��
                if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, "}}") == true)
                {
                    lastIndex = ++i;
                    break;
                }

                // | ���܂܂�Ă���ꍇ�A�ȍ~�̕�����͈����ȂǂƂ��Ĉ���
                if (c == '|')
                {
                    ++pipeCounter;
                    Honememo.Cmn.AddArray(ref pipeTexts, String.Empty);
                    continue;
                }

                // �ϐ��i[[{{{1}}}]]�Ƃ��j�̍ċA�`�F�b�N
                string dummy = String.Empty;
                string variable = String.Empty;
                int index = this.ChkVariable(ref variable, ref dummy, i_Text, i);
                if (index != -1)
                {
                    i = index;
                    if (pipeCounter > 0)
                    {
                        pipeTexts[pipeCounter - 1] += variable;
                    }
                    else
                    {
                        article += variable;
                    }

                    continue;
                }

                // | �̑O�̂Ƃ�
                if (pipeCounter <= 0)
                {
                    // �ϐ��ȊO�� < > [ ] { } ���܂܂�Ă���ꍇ�A�����N�͖���
                    if ((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}'))
                    {
                        break;
                    }

                    article += c;
                }
                else
                {
                    // | �̌�̂Ƃ�
                    // �R�����g�i<!--�j���܂܂�Ă���ꍇ�A�����N�͖���
                    if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, CommentStart))
                    {
                        break;
                    }

                    // nowiki�̃`�F�b�N
                    string nowiki = String.Empty;
                    index = ChkNowiki(ref nowiki, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += nowiki;
                        continue;
                    }

                    // �����N [[ {{ �i{{test|[[��]]}}�Ƃ��j�̍ċA�`�F�b�N
                    Link link = new Link();
                    index = this.ChkLinkText(ref link, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += link.Text;
                        continue;
                    }

                    pipeTexts[pipeCounter - 1] += c;
                }
            }

            // ��͂ɐ��������ꍇ�A���ʂ�߂�l�ɐݒ�
            if (lastIndex != -1)
            {
                // �ϐ��u���b�N�̕�����������N�̃e�L�X�g�ɐݒ�
                result.Text = i_Text.Substring(0, lastIndex + 1);

                // �O��̃X�y�[�X�E���s�͍폜�i���o���͌��̂݁j
                result.Article = article.Trim();

                // | �ȍ~�͂��̂܂ܐݒ�
                result.PipeTexts = pipeTexts;

                // �L����������𒊏o
                // �T�u�y�[�W
                if (result.Article.StartsWith("/") == true)
                {
                    result.SubPageFlag = true;
                }
                else if (result.Article.StartsWith(":"))
                {
                    // �擪�� :
                    result.StartColonFlag = true;
                    result.Article = result.Article.TrimStart(':').TrimStart();
                }

                // �擪�� msgnw:
                result.MsgnwFlag = result.Article.ToLower().StartsWith(Msgnw.ToLower());
                if (result.MsgnwFlag)
                {
                    result.Article = result.Article.Substring(Msgnw.Length);
                }

                // �L��������̉��s�̗L��
                if (article.TrimEnd(' ').EndsWith("\n"))
                {
                    result.EnterFlag = true;
                }
            }

            return result;
        }

        /// <summary>
        /// �n���ꂽ�e�L�X�g�̎w�肳�ꂽ�ʒu�ɑ��݂���Wikipedia�̓��������N�E�e���v���[�g���`�F�b�N�B
        /// </summary>
        /// <param name="o_Link"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <returns>���펞�̖߂�l�ɂ́A]]�̌���]�̈ʒu�̃C���f�b�N�X��Ԃ��B�ُ펞��-1</returns>
        public int ChkLinkText(ref Link o_Link, string i_Text, int i_Index)
        {
            // �o�͒l������
            int lastIndex = -1;
            o_Link.Initialize();

            // ���͒l�ɉ����āA������U�蕪��
            if (Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, "[[") == true)
            {
                // ���������N
                o_Link = this.ParseInnerLink(i_Text.Substring(i_Index));
            }
            else if (Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, "{{") == true)
            {
                // �e���v���[�g
                o_Link = this.ParseTemplate(i_Text.Substring(i_Index));
            }

            // �������ʊm�F
            if (!String.IsNullOrEmpty(o_Link.Text))
            {
                lastIndex = i_Index + o_Link.Text.Length - 1;
            }

            return lastIndex;
        }

        /// <summary>
        /// �n���ꂽ�e�L�X�g�̎w�肳�ꂽ�ʒu�ɑ��݂���ϐ�����́B
        /// </summary>
        /// <param name="o_Variable"></param>
        /// <param name="o_Value"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <returns></returns>
        public int ChkVariable(ref string o_Variable, ref string o_Value, string i_Text, int i_Index)
        {
            // �o�͒l������
            int lastIndex = -1;
            o_Variable = String.Empty;
            o_Value = String.Empty;

            // ���͒l�m�F
            if (Honememo.Cmn.ChkTextInnerWith(i_Text.ToLower(), i_Index, "{{{") == false)
            {
                return lastIndex;
            }

            // �u���b�N�I���܂Ŏ擾
            bool pipeFlag = false;
            for (int i = i_Index + 3; i < i_Text.Length; i++)
            {
                // �I�������̃`�F�b�N
                if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, "}}}") == true)
                {
                    lastIndex = i + 2;
                    break;
                }

                // �R�����g�i<!--�j�̃`�F�b�N
                string dummy = String.Empty;
                int index = ChkComment(ref dummy, i_Text, i);
                if (index != -1)
                {
                    i = index;
                    continue;
                }

                // | ���܂܂�Ă���ꍇ�A�ȍ~�̕�����͑�����ꂽ�l�Ƃ��Ĉ���
                if (i_Text[i] == '|')
                {
                    pipeFlag = true;
                }
                else if (!pipeFlag)
                {
                    // | �̑O�̂Ƃ�
                    // ��Wikipedia�̎d�l��́A{{{1{|�\��}}} �̂悤�ɕϐ����̗��� { ��
                    //   �܂߂邱�Ƃ��ł���悤�����A���ʂ�����Ȃ��̂ŁA�G���[�Ƃ���
                    //   �i�ǂ����Ӑ}���Ă���Ȃ��Ƃ���l�͋��Ȃ����낤���E�E�E�j
                    if (i_Text[i] == '{')
                    {
                        break;
                    }
                }
                else
                {
                    // | �̌�̂Ƃ�
                    // nowiki�̃`�F�b�N
                    string nowiki = String.Empty;
                    index = ChkNowiki(ref nowiki, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        o_Value += nowiki;
                        continue;
                    }

                    // �ϐ��i{{{1|{{{2}}}}}}�Ƃ��j�̍ċA�`�F�b�N
                    string variable = String.Empty;
                    index = this.ChkVariable(ref variable, ref dummy, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        o_Value += variable;
                        continue;
                    }

                    // �����N [[ {{ �i{{{1|[[test]]}}}�Ƃ��j�̍ċA�`�F�b�N
                    Link link = new Link();
                    index = this.ChkLinkText(ref link, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        o_Value += link.Text;
                        continue;
                    }

                    o_Value += i_Text[i];
                }
            }

            // �ϐ��u���b�N�̕�������o�͒l�ɐݒ�
            if (lastIndex != -1)
            {
                o_Variable = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
            }
            else
            {
                // ����ȍ\���ł͂Ȃ������ꍇ�A�o�͒l���N���A
                o_Variable = String.Empty;
                o_Value = String.Empty;
            }

            return lastIndex;
        }

        /// <summary>
        /// �n���ꂽ���������N�E�e���v���[�g����́B
        /// </summary>
        /// <param name="link">�����N�B</param>
        /// <param name="index">�{���̉�͊J�n�ʒu�̃C���f�b�N�X�B</param>
        /// <returns></returns>
        protected int ChkLinkText(ref Link link, int index)
        {
            return this.ChkLinkText(ref link, Text, index);
        }

        /// <summary>
        /// �R�����g��Ԃ̃`�F�b�N�B
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index">�{���̉�͊J�n�ʒu�̃C���f�b�N�X�B</param>
        /// <returns></returns>
        protected int ChkComment(ref string text, int index)
        {
            return MediaWikiPage.ChkComment(ref text, Text, index);
        }

        #endregion

        #region �\����

        /// <summary>
        /// Wikipedia�̃����N�̗v�f���i�[���邽�߂̍\���́B
        /// </summary>
        public struct Link
        {
            /// <summary>
            /// �����N�̃e�L�X�g�i[[�`]]�j�B
            /// </summary>
            public string Text;

            /// <summary>
            /// �����N�̋L�����B
            /// </summary>
            public string Article;

            /// <summary>
            /// �����N�̃Z�N�V�������i#�j�B
            /// </summary>
            public string Section;

            /// <summary>
            /// �����N�̃p�C�v��̕�����i|�j�B
            /// </summary>
            public string[] PipeTexts;

            /// <summary>
            /// ����Ԃ܂��͑��v���W�F�N�g�ւ̃����N�̏ꍇ�A�R�[�h�B
            /// </summary>
            public string Code;

            /// <summary>
            /// �e���v���[�g�i{{�`}}�j���������t���O�B
            /// </summary>
            public bool TemplateFlag;

            /// <summary>
            /// �L�����̐擪�� / �Ŏn�܂邩�������t���O�B
            /// </summary>
            public bool SubPageFlag;

            /// <summary>
            /// �����N�̐擪�� : �Ŏn�܂邩�������t���O�B
            /// </summary>
            public bool StartColonFlag;

            /// <summary>
            /// �e���v���[�g�̏ꍇ�Amsgnw: ���t������Ă��邩�������t���O�B
            /// </summary>
            public bool MsgnwFlag;

            /// <summary>
            /// �e���v���[�g�̏ꍇ�A�L�����̌�ŉ��s����邩�������t���O�B
            /// </summary>
            public bool EnterFlag;

            /// <summary>
            /// �������B
            /// </summary>
            public void Initialize()
            {
                // �R���X�g���N�^�̑���ɁA�K�v�Ȃ炱��ŏ�����
                this.Text = null;
                this.Article = null;
                this.Section = null;
                this.PipeTexts = new string[0];
                this.Code = null;
                this.TemplateFlag = false;
                this.SubPageFlag = false;
                this.StartColonFlag = false;
                this.MsgnwFlag = false;
                this.EnterFlag = false;
            }

            /// <summary>
            /// ���݂�Text�ȊO�̑����l����AText�����l�𐶐��B
            /// </summary>
            /// <returns>��������Text�̕�����B</returns>
            public string MakeText()
            {
                // �߂�l������
                string result = String.Empty;

                // �g�̐ݒ�
                string startSign = "[[";
                string endSign = "]]";
                if (this.TemplateFlag)
                {
                    startSign = "{{";
                    endSign = "}}";
                }

                // �擪�̘g�̕t��
                result += startSign;

                // �擪�� : �̕t��
                if (this.StartColonFlag)
                {
                    result += ":";
                }

                // msgnw: �i�e���v���[�g��<nowiki>�^�O�ŋ��ށj�̕t��
                if (this.TemplateFlag && this.MsgnwFlag)
                {
                    result += Msgnw;
                }

                // ����R�[�h�E���v���W�F�N�g�R�[�h�̕t��
                if (!String.IsNullOrEmpty(this.Code))
                {
                    result += this.Code;
                }

                // �����N�̕t��
                if (!String.IsNullOrEmpty(this.Article))
                {
                    result += this.Article;
                }

                // �Z�N�V�������̕t��
                if (!String.IsNullOrEmpty(this.Section))
                {
                    result += "#" + this.Section;
                }

                // ���s�̕t��
                if (this.EnterFlag)
                {
                    result += '\n';
                }

                // �p�C�v��̕�����̕t��
                if (this.PipeTexts != null)
                {
                    foreach (string text in this.PipeTexts)
                    {
                        result += "|";
                        if (!String.IsNullOrEmpty(text))
                        {
                            result += text;
                        }
                    }
                }

                // �I���̘g�̕t��
                result += endSign;
                return result;
            }
        }

        #endregion
    }
}
