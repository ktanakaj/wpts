// ================================================================================================
// <summary>
//      Wikipedia�|��x���c�[���ݒ��ʃN���X�\�[�X</summary>
//
// <copyright file="ConfigWikipediaDialog.cs" company="honeplus�̃�����">
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
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// Wikipedia�|��x���c�[���ݒ��ʂ̃N���X�ł��B
    /// </summary>
    public partial class ConfigWikipediaDialog : Form
    {
        /// <summary>
        /// ���ʊ֐��N���X�̃I�u�W�F�N�g�B
        /// </summary>
        private Honememo.Cmn cmnAP;

        /// <summary>
        /// �e��ݒ�B
        /// </summary>
        private Config config;

        /// <summary>
        /// comboBoxColumn�őI�����Ă����A�C�e���̃o�b�N�A�b�v�B
        /// </summary>
        private string comboBoxCodeSelectedText;

        /// <summary>
        /// �R���X�g���N�^�B���������\�b�h�Ăяo���̂݁B
        /// </summary>
        public ConfigWikipediaDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// �t�H�[�����[�h���̏����B�������B
        /// </summary>
        /// <param name="sender">�C�x���g�����I�u�W�F�N�g</param>
        /// <param name="e">���������C�x���g</param>
        private void ConfigWikipediaDialog_Load(object sender, EventArgs e)
        {
            // ����������
            this.cmnAP = new Honememo.Cmn();
            this.config = new Config(Path.Combine(Application.StartupPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".xml"));

            // �f�[�^�ݒ�
            comboBoxCodeSelectedText = "";
            comboBoxCode.Items.Clear();
            dataGridViewName.Rows.Clear();
            dataGridViewTitleKey.Columns.Clear();
            // �g�p����擾
            String showCode = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            int x = 0;
            foreach(LanguageInformation lang in config.Languages){
                WikipediaInformation svr = lang as WikipediaInformation;
                if(svr != null){
                    // �\�^�C�g���ݒ�
                    String name = svr.GetName(showCode);
                    if(name != ""){
                        name += (" (" + svr.Code + ")");
                    }
                    else{
                        name = svr.Code;
                    }
                    dataGridViewTitleKey.Columns.Add(svr.Code, name);
                    // �\�f�[�^�ݒ�
                    for(int y = 0 ; y < svr.TitleKeys.Length ; y++){
                        if(dataGridViewTitleKey.RowCount - 1 <= y){
                            dataGridViewTitleKey.Rows.Add();
                        }
                        dataGridViewTitleKey[x, y].Value = svr.TitleKeys[y];
                    }
                    // �R���{�{�b�N�X�ݒ�
                    comboBoxCode.Items.Add(svr.Code);
                    // ���̗��
                    ++x;
                }
            }
            dataGridViewTitleKey.CurrentCell = null;
        }

        /// <summary>
        /// ����R�[�h�R���{�{�b�N�X�ύX���̏����B
        /// </summary>
        /// <param name="sender">�C�x���g�����I�u�W�F�N�g</param>
        /// <param name="e">���������C�x���g</param>
        private void comboBoxCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ConfigLanguageDialog._SelectedIndexChanged > "
                + comboBoxCodeSelectedText + " . "
                + ((comboBoxCode.SelectedItem != null) ? (comboBoxCode.SelectedItem.ToString()) : ("")));

            // �ύX�O�̐ݒ��ۑ�
            // ���ύX�O�ɂ���ύX��ɂ���A���O�ɒǉ����Ă���̂�GetLanguage�Ō�����Ȃ����Ƃ͖����E�E�E�͂�
            if(comboBoxCodeSelectedText != ""){
                WikipediaInformation svr = config.GetLanguage(comboBoxCodeSelectedText) as WikipediaInformation;
                if(svr != null){
                    svr.ArticleXmlPath = textBoxXml.Text.Trim();
                    svr.Redirect = textBoxRedirect.Text.Trim();
                    // �\����ď̂̏����ۑ�
                    dataGridViewName.Sort(dataGridViewName.Columns["Code"], ListSortDirection.Ascending);
                    svr.Names = new LanguageInformation.LanguageName[0];
                    for(int y = 0 ; y < dataGridViewName.RowCount - 1 ; y++){
                        // �l�������ĂȂ��Ƃ��̓K�[�h���Ă���͂������A�ꉞ�`�F�b�N
                        String code = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["Code", y]);
                        if(code != ""){
                            LanguageInformation.LanguageName name = new LanguageInformation.LanguageName();
                            name.Code = code;
                            name.Name = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["ArticleName", y]);
                            name.ShortName = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["ShortName", y]);
                            Honememo.Cmn.AddArray(ref svr.Names, name);
                        }
                    }
                }
            }
            // �ύX��̒l�ɉ����āA��ʕ\�����X�V
            if(comboBoxCode.SelectedItem != null){
                // �l��ݒ�
                WikipediaInformation svr = config.GetLanguage(comboBoxCode.SelectedItem.ToString()) as WikipediaInformation;
                if(svr != null){
                    textBoxXml.Text = svr.ArticleXmlPath;
                    textBoxRedirect.Text = svr.Redirect;
                    // �ď̂̏���\�ɐݒ�
                    dataGridViewName.Rows.Clear();
                    foreach(LanguageInformation.LanguageName name in svr.Names){
                        int index = dataGridViewName.Rows.Add();
                        dataGridViewName["Code", index].Value = name.Code;
                        dataGridViewName["ArticleName", index].Value = name.Name;
                        dataGridViewName["ShortName", index].Value = name.ShortName;
                    }
                }
                // ����̃v���p�e�B��L����
                groupBoxStyle.Enabled = true;
                groupBoxName.Enabled = true;
                // ���݂̑I��l���X�V
                comboBoxCodeSelectedText = comboBoxCode.SelectedItem.ToString();
            }
            else{
                // ����̃v���p�e�B�𖳌���
                groupBoxStyle.Enabled = false;
                groupBoxName.Enabled = false;
                // ���݂̑I��l���X�V
                comboBoxCodeSelectedText = "";
            }
        }

        /* ����R�[�h�R���{�{�b�N�X�ł̃L�[���� */
        private void comboBoxCode_KeyDown(object sender, KeyEventArgs e)
        {
            // �G���^�[�L�[�������ꂽ�ꍇ�A���݂̒l���ꗗ�ɖ�����Γo�^����i�t�H�[�J�X���������Ƃ��̏����j
            if(e.KeyCode == Keys.Enter){
                System.Diagnostics.Debug.WriteLine("ConfigLanguageDialog._KeyDown > " + comboBoxCode.Text);
                comboBoxCode_Leave(sender, e);
            }
        }

        /* ����R�[�h�R���{�{�b�N�X����t�H�[�J�X�𗣂����Ƃ� */
        private void comboBoxCode_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ConfigLanguageDialog._Leave > " + comboBoxCode.Text);
            // ���݂̒l���ꗗ�ɖ�����Γo�^����
            comboBoxCode.Text = comboBoxCode.Text.Trim().ToLower();
            if(comboBoxCode.Text != ""){
                if(Honememo.Cmn.AddComboBoxNewItem(ref comboBoxCode) == true){
                    // �o�^�����ꍇ�����o�ϐ��ɂ��o�^
                    WikipediaInformation svr = config.GetLanguage(comboBoxCode.Text) as WikipediaInformation;
                    // ���݂��Ȃ��͂������ꉞ�͊m�F���Ēǉ�
                    if(svr == null){
                        svr = new WikipediaInformation(comboBoxCode.Text);
                        Honememo.Cmn.AddArray(ref config.Languages, (LanguageInformation) svr);
                        // ��^��̐ݒ�\�ɗ��ǉ�
                        dataGridViewTitleKey.Columns.Add(comboBoxCode.Text, comboBoxCode.Text);
                    }
                    // �o�^�����l��I����ԂɕύX
                    comboBoxCode.SelectedItem = comboBoxCode.Text;
                }
            }
            else{
                // ��ɂ����Ƃ��A�ύX�ŃC�x���g���N����Ȃ��悤�Ȃ̂ŁA�����I�ɌĂ�
                comboBoxCode_SelectedIndexChanged(sender, e);
            }
        }

        /* ����R�[�h�R���{�{�b�N�X�̃R���e�L�X�g���j���[�F����R�[�h��ύX */
        private void toolStripMenuItemModify_Click(object sender, EventArgs e)
        {
            // �I������Ă��錾��R�[�h�Ɋ֘A��������X�V
            if(comboBoxCode.SelectedIndex != -1){
                String oldCode = comboBoxCode.SelectedItem.ToString();
                // ���͉�ʂɂĕύX��̌���R�[�h���擾
                InputLanguageCodeDialog dialog = new InputLanguageCodeDialog();
                dialog.LanguageCode = oldCode;
                if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK){
                    String newCode = dialog.LanguageCode;
                    // �����o�ϐ����X�V
                    LanguageInformation lang = config.GetLanguage(oldCode);
                    if(lang != null){
                        lang.Code = newCode;
                    }
                    foreach(LanguageInformation langIndex in config.Languages){
                        if(langIndex.GetType() != typeof(WikipediaInformation)){
                            continue;
                        }
                        for (int i = 0; i < langIndex.Names.Length; i++)
                        {
                            if (langIndex.Names[i].Code == oldCode)
                            {
                                langIndex.Names[i].Code = newCode;
                            }
                        }
                    }
                    // �R���{�{�b�N�X���X�V
                    int index = comboBoxCode.Items.IndexOf(comboBoxCode.SelectedItem);
                    comboBoxCode.Items[index] = newCode;
                    // ��^��̐ݒ�\���X�V
                    String header = lang.GetName(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                    if(header != ""){
                        header += (" (" + newCode + ")");
                    }
                    else{
                        header = newCode;
                    }
                    dataGridViewTitleKey.Columns[oldCode].HeaderText = header;
                    dataGridViewTitleKey.Columns[oldCode].Name = newCode;
                    // ��ʂ̏�Ԃ��X�V
                    comboBoxCode_SelectedIndexChanged(sender, e);
                }
            }
        }

        /* ����R�[�h�R���{�{�b�N�X�̃R���e�L�X�g���j���[�F������폜 */
        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            // �I������Ă��錾��R�[�h�Ɋ֘A��������폜
            if(comboBoxCode.SelectedIndex != -1){
                dataGridViewTitleKey.Columns.Remove(comboBoxCode.SelectedItem.ToString());
                // �����o�ϐ�������폜
                LanguageInformation[] newLanguages = new LanguageInformation[0];
                foreach(LanguageInformation lang in config.Languages){
                    if(lang.Code == comboBoxCode.SelectedItem.ToString() &&
                       lang.GetType() == typeof(WikipediaInformation)) {
                        continue;
                    }
                    Honememo.Cmn.AddArray(ref newLanguages, lang);
                }
                config.Languages = newLanguages;
            }
            Honememo.Cmn.RemoveComboBoxItem(ref comboBoxCode);
            // ��ʂ̏�Ԃ��X�V
            comboBoxCode_SelectedIndexChanged(sender, e);
        }

        /* �e����ł̌ď̕\����t�H�[�J�X�𗣂����Ƃ� */
        private void dataGridViewName_Leave(object sender, EventArgs e)
        {
            // �l�`�F�b�N
            String codeUnsetRows = "";
            String nameUnsetRows = "";
            String redundantCodeRows = "";
            for(int y = 0 ; y < dataGridViewName.RowCount - 1 ; y++){
                // ����R�[�h��́A�������̃f�[�^�ɕϊ�
                dataGridViewName["Code", y].Value = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["Code", y]).ToLower();
                // ����R�[�h���ݒ肳��Ă��Ȃ��s�����邩�H
                if(dataGridViewName["Code", y].Value.ToString() == ""){
                    if(codeUnsetRows != ""){
                        codeUnsetRows += ",";
                    }
                    codeUnsetRows += (y + 1);
                }
                else{
                    // ����R�[�h���d�����Ă��Ȃ����H
                    for(int i = 0 ; i < y ; i++){
                        if(dataGridViewName["Code", i].Value.ToString() == dataGridViewName["Code", y].Value.ToString()){
                            if(redundantCodeRows != ""){
                                redundantCodeRows += ",";
                            }
                            redundantCodeRows += (y + 1);
                            break;
                        }
                    }
                    // �ď̂��ݒ肳��Ă��Ȃ��̂ɗ��̂��ݒ肳��Ă��Ȃ����H
                    if(Honememo.Cmn.NullCheckAndTrim(dataGridViewName["ShortName", y]) != "" &&
                       Honememo.Cmn.NullCheckAndTrim(dataGridViewName["ArticleName", y]) == ""){
                        if(nameUnsetRows != ""){
                            nameUnsetRows += ",";
                        }
                        nameUnsetRows += (y + 1);
                    }
                }
            }
            // ���ʂ̕\��
            String errorMessage = "";
            if(codeUnsetRows != ""){
                errorMessage += (String.Format(Resources.WarningMessage_UnsetCodeColumn, codeUnsetRows));
            }
            if(redundantCodeRows != ""){
                if(errorMessage != ""){
                    errorMessage += "\n";
                }
                errorMessage += (String.Format(Resources.WarningMessage_RedundantCodeColumn, redundantCodeRows));
            }
            if(nameUnsetRows != ""){
                if(errorMessage != ""){
                    errorMessage += "\n";
                }
                errorMessage += (String.Format(Resources.WarningMessage_UnsetArticleNameColumn, nameUnsetRows));
            }
            if(errorMessage != ""){
                MessageBox.Show(errorMessage, Resources.WarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dataGridViewName.Focus();
            }
        }

        /* OK�{�^������ */
        private void buttonOK_Click(object sender, EventArgs e)
        {
            // �ݒ��ۑ����A��ʂ����
            // �\����̌��ݏ������f�[�^���m��
            comboBoxCode_SelectedIndexChanged(sender, e);
            // �\�̏�Ԃ������o�ϐ��ɕۑ�
            // �̈�̏�����
            foreach(LanguageInformation lang in config.Languages){
                WikipediaInformation svr = lang as WikipediaInformation;
                if(svr != null){
                    Array.Resize(ref svr.TitleKeys, dataGridViewTitleKey.RowCount - 1);
                }
            }
            // �f�[�^�̕ۑ�
            for(int x = 0 ; x < dataGridViewTitleKey.ColumnCount ; x++){
                WikipediaInformation svr = config.GetLanguage(dataGridViewTitleKey.Columns[x].Name) as WikipediaInformation;
                if(svr != null){
                    for(int y = 0; y < dataGridViewTitleKey.RowCount - 1; y++){
                        if(dataGridViewTitleKey[x, y].Value != null){
                            svr.TitleKeys[y] = dataGridViewTitleKey[x, y].Value.ToString().Trim();
                        }
                        else{
                            svr.TitleKeys[y] = "";
                        }
                    }
                }
            }
            // �\�[�g
            Array.Sort(config.Languages);

            // �ݒ���t�@�C���ɕۑ�
            if(config.Save() == true){
                // ��ʂ���āA�ݒ�I��
                this.Close();
            }
            else{
                // �G���[���b�Z�[�W��\���A��ʂ͊J�����܂�
                cmnAP.ErrorDialogResource("ErrorMessage_MissConfigSave");
            }
        }
    }
}