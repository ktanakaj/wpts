using System;
using System.Net.NetworkInformation;

using wptscs.model;
using wptscs.Properties;

namespace wptscs.logic
{
    public abstract class TranslateNetworkObject : Translate
    {
		// �ʐM���Ɏg�p����UserAgent
        public String UserAgent;
		// �ʐM���Ɏg�p����Referer
        public String Referer;

		// �R���X�g���N�^
		public TranslateNetworkObject(LanguageWithServerInformation i_Source, LanguageWithServerInformation i_Target)
			: base(i_Source, i_Target) {}
        
        /* �|��x���������s */
        public override bool Run(String i_Name)
        {
	        // �ϐ���������
	        runInitialize();
	        // �T�[�o�[�ڑ��`�F�b�N
	        if(ping(((LanguageWithServerInformation) source).Server) == false){
		        return false;
	        }
	        // �|��x���������s���̖{�̂����s
	        // ���ȍ~�̏����́A�p���N���X�ɂĒ�`
	        return runBody(i_Name);
        }

        /* �T�[�o�[�ڑ��`�F�b�N */
        private bool ping(String i_Server)
        {
	        // �T�[�o�[�ڑ��`�F�b�N
	        Ping ping = new Ping();
	        try{
		        PingReply reply = ping.Send(i_Server);
		        if(reply.Status != IPStatus.Success){
			        logLine(String.Format(Resources.ErrorMessage_MissNetworkAccess, reply.Status.ToString()));
			        return false;
		        }
	        }
	        catch(Exception e){
		        logLine(String.Format(Resources.ErrorMessage_MissNetworkAccess, e.InnerException.Message));
		        return false;
	        }
	        return true;
        }
    }
}
