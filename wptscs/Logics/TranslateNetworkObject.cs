// ================================================================================================
// <summary>
//      �l�b�g���[�N���g�p����|��x���������������邽�߂̋��ʃN���X�\�[�X</summary>
//
// <copyright file="TranslateNetworkObject.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Net.NetworkInformation;

    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// �l�b�g���[�N���g�p����|��x���������������邽�߂̋��ʃN���X�ł��B
    /// </summary>
    public abstract class TranslateNetworkObject : Translate
    {
        #region private�ϐ�

        /// <summary>
        /// �ʐM���Ɏg�p����UserAgent�B
        /// </summary>
        public string UserAgent;

        /// <summary>
        /// �ʐM���Ɏg�p����Referer�B
        /// </summary>
        public string Referer;
        
        #endregion

        #region �R���X�g���N�^

        /// <summary>
        /// �R���X�g���N�^�B
        /// </summary>
        /// <param name="source">�|�󌳌���B</param>
        /// <param name="target">�|��挾��B</param>
        public TranslateNetworkObject(
            LanguageWithServerInformation source, LanguageWithServerInformation target)
            : base(source, target)
        {
        }
        
        #endregion
        
        #region ���\�b�h

        /// <summary>
        /// �|��x���������s�B
        /// </summary>
        /// <param name="name">�L�����B</param>
        /// <returns><c>true</c> ���������B</returns>
        public override bool Run(string name)
        {
            // �ϐ���������
            RunInitialize();

            // �T�[�o�[�ڑ��`�F�b�N
            if (this.Ping(((LanguageWithServerInformation) source).Server) == false)
            {
                return false;
            }

            // �|��x���������s���̖{�̂����s
            // ���ȍ~�̏����́A�p���N���X�ɂĒ�`
            return RunBody(name);
        }

        /// <summary>
        /// �T�[�o�[�ڑ��`�F�b�N�B
        /// </summary>
        /// <param name="server">�T�[�o�[���B</param>
        /// <returns><c>true</c> �ڑ������B</returns>
        private bool Ping(string server)
        {
            // �T�[�o�[�ڑ��`�F�b�N
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(server);
                if (reply.Status != IPStatus.Success)
                {
                    LogLine(String.Format(Resources.ErrorMessage_MissNetworkAccess, reply.Status.ToString()));
                    return false;
                }
            }
            catch (Exception e)
            {
                LogLine(String.Format(Resources.ErrorMessage_MissNetworkAccess, e.InnerException.Message));
                return false;
            }

            return true;
        }
        
        #endregion
    }
}
