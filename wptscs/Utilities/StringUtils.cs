// ================================================================================================
// <summary>
//      Apache Commons Lang �� StringUtils���Q�l�ɂ������[�e�B���e�B�N���X�\�[�X�B</summary>
//
// <copyright file="StringUtils.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;

    /// <summary>
    /// Apache Commons Lang �� StringUtils���Q�l�ɂ������[�e�B���e�B�N���X�ł��B
    /// </summary>
    public static class StringUtils
    {
        #region �ÓI���\�b�h

        /// <summary>
        /// �n���ꂽ��������`�F�b�N���Anull �������ꍇ�ɂ͋�̕������Ԃ��܂��B
        /// ����ȊO�̏ꍇ�ɂ͓n���ꂽ�������Ԃ��܂��B
        /// </summary>
        /// <param name="str">�`�F�b�N���s���ΏۂƂȂ镶����B</param>
        /// <returns>�n���ꂽ������Anull �̏ꍇ�ɂ͋�̕�����B</returns>
        public static string DefaultString(string str)
        {
            return DefaultString(str, String.Empty);
        }

        /// <summary>
        /// �n���ꂽ��������`�F�b�N���Anull �������ꍇ�ɂ͎w�肳�ꂽ�f�t�H���g�̕������Ԃ��܂��B
        /// ����ȊO�̏ꍇ�ɂ͓n���ꂽ�������Ԃ��܂��B
        /// </summary>
        /// <param name="str">�`�F�b�N���s���ΏۂƂȂ镶����B</param>
        /// <param name="defaultString">�n���ꂽ������ null �̏ꍇ�ɕԂ����f�t�H���g�̕�����B</param>
        /// <returns>�n���ꂽ������Anull �̏ꍇ�ɂ̓f�t�H���g�̕�����B</returns>
        public static string DefaultString(string str, string defaultString)
        {
            if (str == null)
            {
                return defaultString;
            }

            return str;
        }

        #endregion
    }
}
