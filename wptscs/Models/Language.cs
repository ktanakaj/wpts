// ================================================================================================
// <summary>
//      ����Ɋւ����������킷���f���N���X�\�[�X</summary>
//
// <copyright file="Language.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// ����Ɋւ����������킷���f���N���X�ł��B
    /// </summary>
    public class Language : IComparable
    {
        #region private�ϐ�

        /// <summary>
        /// ����̃R�[�h�B
        /// </summary>
        private string code;

        /// <summary>
        /// ���̌���́A�e����ł̖��́B
        /// </summary>
        private IDictionary<string, LanguageName> names = new Dictionary<string, LanguageName>();

        #endregion

        #region �R���X�g���N�^

        /// <summary>
        /// �R���X�g���N�^�i�V���A���C�Y�p�j�B
        /// </summary>
        public Language()
            : this("unknown")
        {
            // ���K���Ȓl�Œʏ�̃R���X�g���N�^�����s
            System.Diagnostics.Debug.WriteLine("Language.Language : ��������Ȃ��R���X�g���N�^���g�p���Ă��܂�");
        }

        /// <summary>
        /// �R���X�g���N�^�i�ʏ�j�B
        /// </summary>
        /// <param name="code">����̃R�[�h�B�B</param>
        public Language(string code)
        {
            // �����o�ϐ��̏����ݒ�
            this.Code = code;
        }

        #endregion

        #region �v���p�e�B

        /// <summary>
        /// ����̃R�[�h�B
        /// </summary>
        [XmlAttributeAttribute("Code")]
        public string Code
        {
            get
            {
                return this.code;
            }

            set
            {
                // ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
                if (String.IsNullOrEmpty(value != null ? value.Trim() : value))
                {
                    throw new ArgumentNullException("value");
                }

                this.code = value.Trim().ToLower();
            }
        }

        /// <summary>
        /// ���̌���́A�e����ł̖��́B
        /// </summary>
        public IDictionary<string, LanguageName> Names
        {
            get
            {
                return this.names;
            }

            set
            {
                // ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.names = value;
            }
        }

        #endregion

        #region ���\�b�h

        /// <summary>
        /// �z��̃\�[�g�p���\�b�h�B
        /// </summary>
        /// <param name="obj">��r�Ώۂ̃I�u�W�F�N�g�B</param>
        /// <returns>��r�ΏۃI�u�W�F�N�g�̑��Ώ��������������l�B</returns>
        public int CompareTo(object obj)
        {
            // ����R�[�h�Ń\�[�g
            Language lang = obj as Language;
            return this.Code.CompareTo(lang.Code);
        }

        #endregion

        #region �\����

        /// <summary>
        /// ���錾��́A�e����ł̖��́E���̂��i�[���邽�߂̍\���̂ł��B
        /// </summary>
        public struct LanguageName
        {
            /// <summary>
            /// ����̖��́B
            /// </summary>
            public string Name;

            /// <summary>
            /// ����̗��́B
            /// </summary>
            public string ShortName;
        }

        #endregion
    }
}
