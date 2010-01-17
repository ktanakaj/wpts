// ================================================================================================
// <summary>
//      Wikipedia�|��x���c�[���R�[�h���̓_�C�A���O�N���X�\�[�X</summary>
//
// <copyright file="InputLanguageCodeDialog.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Wikipedia�|��x���c�[���R�[�h���̓_�C�A���O�̃N���X�ł��B
    /// </summary>
    public partial class InputLanguageCodeDialog : Form
    {
        /// <summary>
        /// ����R�[�h�i�f�[�^�����p�j�B
        /// </summary>
        public string LanguageCode;

        /// <summary>
        /// �R���X�g���N�^�B���������\�b�h�Ăяo���̂݁B
        /// </summary>
        public InputLanguageCodeDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// �t�H�[�����[�h���̏����B�������B
        /// </summary>
        /// <param name="sender">�C�x���g�����I�u�W�F�N�g</param>
        /// <param name="e">���������C�x���g</param>
        private void InputLanguageCodeDialog_Load(object sender, EventArgs e)
        {
            // �e�L�X�g�{�b�N�X�Ɍ���R�[�h��ݒ�
            if (this.LanguageCode != null)
            {
                textBoxCode.Text = this.LanguageCode;
            }
        }

        /// <summary>
        /// �t�H�[���N���[�Y���̏����B�f�[�^�ۑ��B
        /// </summary>
        /// <param name="sender">�C�x���g�����I�u�W�F�N�g</param>
        /// <param name="e">���������C�x���g</param>
        private void InputLanguageCodeDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            // �e�L�X�g�{�b�N�X�̌���R�[�h��ۑ�
            this.LanguageCode = textBoxCode.Text.Trim();
        }
    }
}