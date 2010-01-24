// ================================================================================================
// <summary>
//      �y�[�W�iWikipedia�̋L���Ȃǁj������킷���f���N���X�\�[�X</summary>
//
// <copyright file="Page.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;

    /// <summary>
    /// �y�[�W�iWikipedia�̋L���Ȃǁj������킷���f���N���X�ł��B
    /// </summary>
    public class Page
    {
        #region private�ϐ�

        /// <summary>
        /// �y�[�W����������E�F�u�T�C�g�B
        /// </summary>
        private Website website;

        /// <summary>
        /// �y�[�W�^�C�g���B
        /// </summary>
        private string title;

        /// <summary>
        /// �y�[�W�̖{���B
        /// </summary>
        private string text;

        /// <summary>
        /// �y�[�W�̃^�C���X�^���v�B
        /// </summary>
        private DateTime timestamp;

        #endregion

        #region �R���X�g���N�^

        /// <summary>
        /// �R���X�g���N�^�B
        /// </summary>
        /// <param name="website">�y�[�W����������E�F�u�T�C�g�B</param>
        /// <param name="title">�y�[�W�^�C�g���B</param>
        /// <param name="text">�y�[�W�̖{���B</param>
        /// <param name="timestamp">�y�[�W�̃^�C���X�^���v�B</param>
        public Page(Website website, string title, string text, DateTime timestamp)
        {
            // �����l�ݒ�A��{�I�ɈȌ�O����ύX����邱�Ƃ�z�肵�Ȃ�
            this.Website = website;
            this.Title = title;
            this.Text = text;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// �R���X�g���N�^�B
        /// �y�[�W�̃^�C���X�^���v�ɂ͌��ݓ��� (UTC) ��ݒ�B
        /// </summary>
        /// <param name="website">�y�[�W����������E�F�u�T�C�g�B</param>
        /// <param name="title">�y�[�W�^�C�g���B</param>
        /// <param name="text">�y�[�W�̖{���B</param>
        public Page(Website website, string title, string text)
            : this(website, title, text, System.DateTime.UtcNow)
        {
        }

        #endregion

        #region �v���p�e�B

        /// <summary>
        /// �y�[�W����������E�F�u�T�C�g�B
        /// </summary>
        public Website Website
        {
            get
            {
                return this.website;
            }

            protected set
            {
                // �E�F�u�T�C�g�͕K�{
                if (value == null)
                {
                    throw new ArgumentNullException("website");
                }

                this.website = value;
            }
        }

        /// <summary>
        /// �y�[�W�^�C�g���B
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            protected set
            {
                // �y�[�W�^�C�g���͕K�{
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("title");
                }

                this.title = value;
            }
        }
        
        /// <summary>
        /// �y�[�W�̖{���B
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            protected set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// �y�[�W�̃^�C���X�^���v�B
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return this.timestamp;
            }

            protected set
            {
                this.timestamp = value;
            }
        }
        
        #endregion
    }
}
