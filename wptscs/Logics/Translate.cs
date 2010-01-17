// ================================================================================================
// <summary>
//      �|��x���������������邽�߂̋��ʃN���X�\�[�X</summary>
//
// <copyright file="Translate.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Net;
    using Honememo;
    using Honememo.Wptscs.Models;

    /// <summary>
    /// �|��x���������������邽�߂̋��ʃN���X�ł��B
    /// </summary>
    public abstract class Translate
    {
        /// <summary>
        /// ���s�R�[�h�B
        /// </summary>
        public static readonly string ENTER = "\r\n";

        /// <summary>
        /// ������r���ŏI�������邽�߂̃t���O�B
        /// </summary>
        public bool CancellationPending;

        /// <summary>
        /// ���ʊ֐��N���X�̃I�u�W�F�N�g�B
        /// </summary>
        protected Honememo.Cmn cmnAP;

        /// <summary>
        /// �|�󌳌���̌���R�[�h�B
        /// </summary>
        protected LanguageInformation source;

        /// <summary>
        /// �|��挾��̌���R�[�h�B
        /// </summary>
        protected LanguageInformation target;

        /// <summary>
        /// ���O���b�Z�[�W�iproperty�j�B
        /// </summary>
        private string log;

        /// <summary>
        /// �ϊ���e�L�X�g�iproperty�j�B
        /// </summary>
        private string text;

        /// <summary>
        /// �R���X�g���N�^�B
        /// </summary>
        /// <param name="source">�|�󌳌���</param>
        /// <param name="target">�|��挾��</param>
        public Translate(LanguageInformation source, LanguageInformation target)
        {
            // ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            else if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            // �����o�ϐ��̏�����
            this.cmnAP = new Honememo.Cmn();
            this.source = source;
            this.target = target;
            this.RunInitialize();
        }

        /// <summary>
        /// ���O�X�V�`�B�C�x���g�B
        /// </summary>
        public event EventHandler LogUpdate;

        /// <summary>
        /// ���O���b�Z�[�W�B
        /// </summary>
        public string Log
        {
            get
            {
                return this.log;
            }

            protected set
            {
                this.log = (value != null) ? value : String.Empty;
                this.LogUpdate(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// �ϊ���e�L�X�g�B
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            protected set
            {
                this.text = (value != null) ? value : String.Empty;
            }
        }

        /// <summary>
        /// �|��x���������s�B
        /// </summary>
        /// <param name="name">�L����</param>
        /// <returns><c>true</c> ��������</returns>
        public virtual bool Run(string name)
        {
            // �ϐ���������
            this.RunInitialize();

            // �|��x���������s���̖{�̂����s
            // ���ȍ~�̏����́A�p���N���X�ɂĒ�`
            return this.RunBody(name);
        }

        /// <summary>
        /// �|��x���������s���̖{�́B
        /// ���p���N���X�ł́A���̊֐��ɏ������������邱��
        /// </summary>
        /// <param name="name">�L����</param>
        /// <returns><c>true</c> ��������</returns>
        protected abstract bool RunBody(string name);

        /// <summary>
        /// �|��x���������s���̏����������B
        /// </summary>
        protected void RunInitialize()
        {
            // �ϐ���������
            this.log = String.Empty;
            this.Text = String.Empty;
            this.CancellationPending = false;
        }

        /// <summary>
        /// ���O���b�Z�[�W��1�s�ǉ��o�́B
        /// </summary>
        /// <param name="log">���O���b�Z�[�W</param>
        protected void LogLine(string log)
        {
            // ���O�̃��O�����s����Ă��Ȃ��ꍇ�A���s���ďo��
            if (this.Log != String.Empty && this.Log.EndsWith(ENTER) == false)
            {
                this.Log += ENTER + log + ENTER;
            }
            else
            {
                this.Log += log + ENTER;
            }
        }
    }
}
