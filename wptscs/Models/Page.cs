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
        /// �y�[�W�^�C�g���B
        /// </summary>
        private string title;

        /// <summary>
        /// �y�[�W�^�C�g���B
        /// </summary>
        private string text;

        /// <summary>
        /// �y�[�W�̃^�C���X�^���v�B
        /// </summary>
        private DateTime timestamp;

        /// <summary>
        /// �y�[�W��URL�B
        /// </summary>
        private Uri url;

        #endregion

        #region �v���p�e�B

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

        /// <summary>
        /// �y�[�W��URL�B
        /// </summary>
        public Uri Url
        {
            get
            {
                return this.url;
            }

            protected set
            {
                this.url = value;
            }
        }
        
        #endregion
    }
}
