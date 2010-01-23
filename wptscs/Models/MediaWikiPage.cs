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
    }
}
