// ================================================================================================
// <summary>
//      Windows�����Ɋւ��郆�[�e�B���e�B�N���X�\�[�X�B</summary>
//
// <copyright file="FormUtils.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Windows.Forms;

    // �� �v���p�e�B���܂ނ̂ŁA���̂܂ܑ��̃v���W�F�N�g�ɗ��p���邱�Ƃ͂ł��Ȃ�
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// Windows�����Ɋւ��郆�[�e�B���e�B�N���X�ł��B
    /// </summary>
    public static class FormUtils
    {
        #region �_�C�A���O

        /// <summary>
        /// �P���f�U�C���̒ʒm�_�C�A���O�i���͂��ꂽ�������\���j�B
        /// </summary>
        /// <param name="msg">���b�Z�[�W�B</param>
        public static void InformationDialog(string msg)
        {
            // �n���ꂽ������Œʒm�_�C�A���O��\��
            MessageBox.Show(
                msg,
                Resources.InformationTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// �P���f�U�C���̒ʒm�_�C�A���O�i���͂��ꂽ����������������ĕ\���j�B
        /// </summary>
        /// <param name="format">�������ڂ��܂񂾃��b�Z�[�W�B</param>
        /// <param name="args">�����ݒ�ΏۃI�u�W�F�N�g�z��B</param>
        public static void InformationDialog(string format, params object[] args)
        {
            // �I�[�o�[���[�h���\�b�h���R�[��
            FormUtils.InformationDialog(String.Format(format, args));
        }

        /// <summary>
        /// �P���f�U�C���̌x���_�C�A���O�i���͂��ꂽ�������\���j�B
        /// </summary>
        /// <param name="msg">���b�Z�[�W�B</param>
        public static void WarningDialog(string msg)
        {
            // �n���ꂽ������Ōx���_�C�A���O��\��
            MessageBox.Show(
                msg,
                Resources.WarningTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// �P���f�U�C���̌x���_�C�A���O�i���͂��ꂽ����������������ĕ\���j�B
        /// </summary>
        /// <param name="format">�������ڂ��܂񂾃��b�Z�[�W�B</param>
        /// <param name="args">�����ݒ�ΏۃI�u�W�F�N�g�z��B</param>
        public static void WarningDialog(string format, params object[] args)
        {
            // �I�[�o�[���[�h���\�b�h���R�[��
            FormUtils.WarningDialog(String.Format(format, args));
        }

        /// <summary>
        /// �P���f�U�C���̃G���[�_�C�A���O�i���͂��ꂽ�������\���j�B
        /// </summary>
        /// <param name="msg">���b�Z�[�W�B</param>
        public static void ErrorDialog(string msg)
        {
            // �n���ꂽ������ŃG���[�_�C�A���O��\��
            MessageBox.Show(
                msg,
                Resources.ErrorTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// �P���f�U�C���̃G���[�_�C�A���O�i���͂��ꂽ����������������ĕ\���j�B
        /// </summary>
        /// <param name="format">�������ڂ��܂񂾃��b�Z�[�W�B</param>
        /// <param name="args">�����ݒ�ΏۃI�u�W�F�N�g�z��B</param>
        public static void ErrorDialog(string format, params object[] args)
        {
            // �I�[�o�[���[�h���\�b�h���R�[��
            FormUtils.ErrorDialog(String.Format(format, args));
        }

        #endregion
    }
}
