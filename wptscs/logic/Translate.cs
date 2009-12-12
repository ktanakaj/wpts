using System;
using System.Net;
using MYAPP;
using wptscs.model;

namespace wptscs.logic
{
    // �|��x���������������邽�߂̋��ʃN���X
    public abstract class Translate
    {
		// ������r���ŏI�������邽�߂̃t���O
		public bool CancellationPending;

		// ���O�X�V�`�B�C�x���g
		public event EventHandler LogUpdate;

		// ���O���b�Z�[�W
		public String Log {
			get {
                return _Log;
            }
			protected set {
                _Log = ((value != null) ? value : "");
                LogUpdate(this, EventArgs.Empty);
			}
		}

		// �ϊ���e�L�X�g
		public String Text {
			get {
                return _Text;
            }
			protected set {
                _Text = ((value != null) ? value : "");
            }
		}

		// ���s�R�[�h
        public static readonly String ENTER = "\r\n";

		// ���ʊ֐��N���X�̃I�u�W�F�N�g
		protected MYAPP.Cmn cmnAP;

		// �|�󌳁^�挾��̌���R�[�h
		protected LanguageInformation source;
		protected LanguageInformation target;

		// ���O���b�Z�[�W�iproperty�j
		private String _Log;
		// �ϊ���e�L�X�g�iproperty�j
		private String _Text;

        /* �R���X�g���N�^ */
        public Translate(LanguageInformation i_Source, LanguageInformation i_Target)
        {
	        // ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
	        if(i_Source == null){
		        throw new ArgumentNullException("i_Source");
	        }
	        else if(i_Target == null){
		        throw new ArgumentNullException("i_Target");
	        }
	        // �����o�ϐ��̏�����
	        cmnAP = new MYAPP.Cmn();
	        source = i_Source;
	        target = i_Target;
	        runInitialize();
        }

		// �|��x���������s���̖{��
		// ���p���N���X�ł́A���̊֐��ɏ������������邱��
        protected abstract bool runBody(String i_Name);

        /* �|��x���������s */
        public virtual bool Run(String i_Name)
        {
	        // �ϐ���������
	        runInitialize();
	        // �|��x���������s���̖{�̂����s
	        // ���ȍ~�̏����́A�p���N���X�ɂĒ�`
	        return runBody(i_Name);
        }

        /* �|��x���������s���̏��������� */
        protected void runInitialize()
        {
	        // �ϐ���������
	        _Log = "";
	        Text = "";
	        CancellationPending = false;
        }

        /* ���O���b�Z�[�W��1�s�ǉ��o�� */
        protected void logLine(String i_Log)
        {
	        // ���O�̃��O�����s����Ă��Ȃ��ꍇ�A���s���ďo��
	        if(Log != "" && Log.EndsWith(ENTER) == false){
		        Log += (ENTER + i_Log + ENTER);
	        }
	        else{
		        Log += (i_Log + ENTER);
	        }
        }
    }
}
