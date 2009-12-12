using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace wptscs
{
    public partial class InputLanguageCodeDialog : Form
    {
        public InputLanguageCodeDialog()
        {
            InitializeComponent();
        }

		// ����R�[�h�i�f�[�^�����p�j
		public String LanguageCode;

        private void InputLanguageCodeDialog_Load(object sender, EventArgs e)
        {
            // �e�L�X�g�{�b�N�X�Ɍ���R�[�h��ݒ�
            if (LanguageCode != null)
            {
                textBoxCode.Text = LanguageCode;
            }
        }

        private void InputLanguageCodeDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            // �e�L�X�g�{�b�N�X�̌���R�[�h��ۑ�
            LanguageCode = textBoxCode.Text.Trim();
        }
    }
}