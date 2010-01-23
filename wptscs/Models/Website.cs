// ================================================================================================
// <summary>
//      �E�F�u�T�C�g������킷���f���N���X�\�[�X</summary>
//
// <copyright file="Website.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// �E�F�u�T�C�g������킷���f���N���X�ł��B
    /// </summary>
    /// <remarks>���ꂪ�قȂ�ꍇ�́A�ʂ̃E�F�u�T�C�g�Ƃ��Ĉ����܂��B</remarks>
    public class Website
    {
        #region private�ϐ�

        /// <summary>
        /// �T�[�o�[���i�h���C�� or IP�A�h���X�j�B
        /// </summary>
        private string server;

        /// <summary>
        /// �E�F�u�T�C�g�̌���B
        /// </summary>
        private Language lang;

        #endregion

        #region �R���X�g���N�^
        
        /// <summary>
        /// �R���X�g���N�^�i�V���A���C�Y�p�j�B
        /// </summary>
        public Website()
            : this(new Language("unknown"))
        {
            // ���K���Ȓl�Œʏ�̃R���X�g���N�^�����s
            System.Diagnostics.Debug.WriteLine("Website.Website : ��������Ȃ��R���X�g���N�^���g�p���Ă��܂�");
        }

        /// <summary>
        /// �R���X�g���N�^�i�ʏ�j�B
        /// </summary>
        /// <param name="lang">�E�F�u�T�C�g�̌���B</param>
        public Website(Language lang)
        {
            // �����o�ϐ��̏����ݒ�
            this.lang = lang;
        }

        #endregion

        #region �v���p�e�B

        /// <summary>
        /// �T�[�o�[���i�h���C�� or IP�A�h���X�j�B
        /// </summary>
        public string Server
        {
            get
            {
                return this.server;
            }

            protected set
            {
                // ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
                if (String.IsNullOrEmpty(value != null ? value.Trim() : value))
                {
                    throw new ArgumentNullException("value");
                }

                this.server = value.Trim();
            }
        }

        /// <summary>
        /// �E�F�u�T�C�g�̌���B
        /// </summary>
        public Language Lang
        {
            get
            {
                return this.lang;
            }

            protected set
            {
                // ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.lang = value;
            }
        }

        #endregion
    }
}
