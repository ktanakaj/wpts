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

		// 言語コード（データやり取り用）
		public String LanguageCode;

        private void InputLanguageCodeDialog_Load(object sender, EventArgs e)
        {
            // テキストボックスに言語コードを設定
            if (LanguageCode != null)
            {
                textBoxCode.Text = LanguageCode;
            }
        }

        private void InputLanguageCodeDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            // テキストボックスの言語コードを保存
            LanguageCode = textBoxCode.Text.Trim();
        }
    }
}