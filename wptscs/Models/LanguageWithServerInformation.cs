using System;

namespace Honememo.Wptscs.Models
{
    // ������ƁA�֘A����T�[�o�[�����i�[����N���X
    public class LanguageWithServerInformation : LanguageInformation
    {
        // �R���X�g���N�^�i�V���A���C�Y�p�j
        public LanguageWithServerInformation() : this("unknown")
        {
//			System.Diagnostics.Debug.WriteLine("LanguageWithServerInformation.LanguageWithServerInformation > ��������Ȃ��R���X�g���N�^���g�p���Ă��܂�");
			// �K���Ȓl�Œʏ�̃R���X�g���N�^�����s
		}
		// �R���X�g���N�^�i�ʏ�j
		public LanguageWithServerInformation(String i_Code) : base(i_Code){
			// �����l�ݒ�
			// �����̃N���X�͒�`�̂݁B���ۂ̐ݒ�́A�p�������N���X�ōs��
			Server = "unknown";
		}

		// �T�[�o�[�̖���
		public String Server {
			get {
				return _Server;
			}
			set {
				// ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
				if(((value != null) ? value.Trim() : "") == ""){
					throw new ArgumentNullException("i_Name");
				}
				_Server = value.Trim();
			}
		}

		// �T�[�o�[�̖��́iproperty�j
		private String _Server;
    }
}
