// ConfigLanguageDialog�̃{�^������{�̂�A���상�\�b�h�Ȃ�
#include "StdAfx.h"
#include "ConfigWikipediaDialog.h"
#include "InputLanguageCodeDialog.h"

using namespace wpts;

/* ������ */
System::Void ConfigWikipediaDialog::ConfigWikipediaDialog_Load(System::Object^  sender, System::EventArgs^  e)
{
	// ����������
	cmnAP = gcnew MYAPP::Cmn();
	config = gcnew Config(IO::Path::Combine(Application::StartupPath, IO::Path::GetFileNameWithoutExtension(Application::ExecutablePath) + ".xml"));

	// �c�[���`�b�v�̐ݒ�
	toolTip->SetToolTip(groupBoxLanguage, cmnAP->Resource->GetString("ToolTipConfigWikipedia_GroupBoxLanguage"));
	toolTip->SetToolTip(comboBoxCode, cmnAP->Resource->GetString("ToolTipConfigWikipedia_ComboBoxCode"));
	toolTip->SetToolTip(labelCode, cmnAP->Resource->GetString("ToolTipConfigWikipedia_ComboBoxCode"));
	toolTip->SetToolTip(groupBoxStyle, cmnAP->Resource->GetString("ToolTipConfigWikipedia_GroupBoxStyle"));
	toolTip->SetToolTip(textBoxXml, cmnAP->Resource->GetString("ToolTipConfigWikipedia_TextBoxXml"));
	toolTip->SetToolTip(labelXml, cmnAP->Resource->GetString("ToolTipConfigWikipedia_TextBoxXml"));
	toolTip->SetToolTip(textBoxRedirect, cmnAP->Resource->GetString("ToolTipConfigWikipedia_TextBoxRedirect"));
	toolTip->SetToolTip(labelRedirect, cmnAP->Resource->GetString("ToolTipConfigWikipedia_TextBoxRedirect"));
	toolTip->SetToolTip(groupBoxName, cmnAP->Resource->GetString("ToolTipConfigWikipedia_GroupBoxName"));
	toolTip->SetToolTip(groupBoxTitleKey, cmnAP->Resource->GetString("ToolTipConfigWikipedia_GroupBoxTitleKey"));
	toolStripMenuItemModify->ToolTipText = cmnAP->Resource->GetString("ToolTipConfigWikipedia_ToolStripMenuItemModify");
	toolStripMenuItemDelete->ToolTipText = cmnAP->Resource->GetString("ToolTipConfigWikipedia_ToolStripMenuItemDelete");
	dataGridViewName->Columns["Code"]->ToolTipText = cmnAP->Resource->GetString("ToolTipConfigWikipediaColumn_Code");
	dataGridViewName->Columns["ArticleName"]->ToolTipText = cmnAP->Resource->GetString("ToolTipConfigWikipediaColumn_ArticleName");
	dataGridViewName->Columns["ShortName"]->ToolTipText = cmnAP->Resource->GetString("ToolTipConfigWikipediaColumn_ShortName");

	// �f�[�^�ݒ�
	comboBoxCodeSelectedText = "";
	comboBoxCode->Items->Clear();
	dataGridViewName->Rows->Clear();
	dataGridViewTitleKey->Columns->Clear();
	// �g�p����擾
	String ^showCode = System::Globalization::CultureInfo::CurrentCulture->TwoLetterISOLanguageName;
	int x = 0;
	for each(LanguageInformation ^lang in config->Languages){
		WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(lang);
		if(svr != nullptr){
			// �\�^�C�g���ݒ�
			String ^name = svr->GetName(showCode);
			if(name != ""){
				name += (" (" + svr->Code + ")");
			}
			else{
				name = svr->Code;
			}
			dataGridViewTitleKey->Columns->Add(svr->Code, name);
			// �\�f�[�^�ݒ�
			for(int y = 0 ; y < svr->TitleKeys->Length ; y++){
				if(dataGridViewTitleKey->RowCount - 1 <= y){
					dataGridViewTitleKey->Rows->Add();
				}
				dataGridViewTitleKey[x, y]->Value = svr->TitleKeys[y];
			}
			// �R���{�{�b�N�X�ݒ�
			comboBoxCode->Items->Add(svr->Code);
			// ���̗��
			++x;
		}
	}
	dataGridViewTitleKey->CurrentCell = nullptr;
}

/* OK�{�^������ */
System::Void ConfigWikipediaDialog::buttonOK_Click(System::Object^  sender, System::EventArgs^  e)
{
	// �ݒ��ۑ����A��ʂ����
	// �\����̌��ݏ������f�[�^���m��
	comboBoxCode_SelectedIndexChanged(sender, e);
	// �\�̏�Ԃ������o�ϐ��ɕۑ�
	// �̈�̏�����
	for each(LanguageInformation ^lang in config->Languages){
		WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(lang);
		if(svr != nullptr){
			Array::Resize(svr->TitleKeys, dataGridViewTitleKey->RowCount - 1);
		}
	}
	// �f�[�^�̕ۑ�
	for(int x = 0 ; x < dataGridViewTitleKey->ColumnCount ; x++){
		WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(config->GetLanguage(dataGridViewTitleKey->Columns[x]->Name));
		if(svr != nullptr){
			for(int y = 0; y < dataGridViewTitleKey->RowCount - 1; y++){
				if(dataGridViewTitleKey[x, y]->Value != nullptr){
					svr->TitleKeys[y] = dataGridViewTitleKey[x, y]->Value->ToString()->Trim();
				}
				else{
					svr->TitleKeys[y] = "";
				}
			}
		}
	}
	// �\�[�g
	Array::Sort(config->Languages);

	// �ݒ���t�@�C���ɕۑ�
	if(config->Save() == true){
		// ��ʂ���āA�ݒ�I��
		this->Close();
	}
	else{
		// �G���[���b�Z�[�W��\���A��ʂ͊J�����܂�
		cmnAP->ErrorDialogResource("ErrorMessage_MissConfigSave");
	}
	return;
}

/* ����R�[�h�R���{�{�b�N�X�̕ύX */
System::Void ConfigWikipediaDialog::comboBoxCode_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e)
{
	System::Diagnostics::Debug::WriteLine("ConfigLanguageDialog::_SelectedIndexChanged > "
		+ comboBoxCodeSelectedText + " -> "
		+ ((comboBoxCode->SelectedItem != nullptr) ? (comboBoxCode->SelectedItem->ToString()) : ("")));

	// �ύX�O�̐ݒ��ۑ�
	// ���ύX�O�ɂ���ύX��ɂ���A���O�ɒǉ����Ă���̂�GetLanguage�Ō�����Ȃ����Ƃ͖����E�E�E�͂�
	if(comboBoxCodeSelectedText != ""){
		WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(config->GetLanguage(comboBoxCodeSelectedText));
		if(svr != nullptr){
			svr->ArticleXmlPath = textBoxXml->Text->Trim();
			svr->Redirect = textBoxRedirect->Text->Trim();
			// �\����ď̂̏����ۑ�
			dataGridViewName->Sort(dataGridViewName->Columns["Code"], ListSortDirection::Ascending);
			svr->Names = gcnew array<LanguageInformation::LanguageName>(0);
			for(int y = 0 ; y < dataGridViewName->RowCount - 1 ; y++){
				// �l�������ĂȂ��Ƃ��̓K�[�h���Ă���͂������A�ꉞ�`�F�b�N
				String ^code = MYAPP::Cmn::NullCheckAndTrim(dataGridViewName["Code", y]);
				if(code != ""){
					LanguageInformation::LanguageName name;
					name.Code = code;
					name.Name = MYAPP::Cmn::NullCheckAndTrim(dataGridViewName["ArticleName", y]);
					name.ShortName = MYAPP::Cmn::NullCheckAndTrim(dataGridViewName["ShortName", y]);
					MYAPP::Cmn::AddArray(svr->Names, name);
				}
			}
		}
	}
	// �ύX��̒l�ɉ����āA��ʕ\�����X�V
	if(comboBoxCode->SelectedItem != nullptr){
		// �l��ݒ�
		WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(config->GetLanguage(comboBoxCode->SelectedItem->ToString()));
		if(svr != nullptr){
			textBoxXml->Text = svr->ArticleXmlPath;
			textBoxRedirect->Text = svr->Redirect;
			// �ď̂̏���\�ɐݒ�
			dataGridViewName->Rows->Clear();
			for each(LanguageInformation::LanguageName name in svr->Names){
				int index = dataGridViewName->Rows->Add();
				dataGridViewName["Code", index]->Value = name.Code;
				dataGridViewName["ArticleName", index]->Value = name.Name;
				dataGridViewName["ShortName", index]->Value = name.ShortName;
			}
		}
		// ����̃v���p�e�B��L����
		groupBoxStyle->Enabled = true;
		groupBoxName->Enabled = true;
		// ���݂̑I��l���X�V
		comboBoxCodeSelectedText = comboBoxCode->SelectedItem->ToString();
	}
	else{
		// ����̃v���p�e�B�𖳌���
		groupBoxStyle->Enabled = false;
		groupBoxName->Enabled = false;
		// ���݂̑I��l���X�V
		comboBoxCodeSelectedText = "";
	}
}

/* ����R�[�h�R���{�{�b�N�X�ł̃L�[���� */
System::Void ConfigWikipediaDialog::comboBoxCode_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e)
{
	// �G���^�[�L�[�������ꂽ�ꍇ�A���݂̒l���ꗗ�ɖ�����Γo�^����i�t�H�[�J�X���������Ƃ��̏����j
	if(e->KeyCode == Keys::Enter){
		System::Diagnostics::Debug::WriteLine("ConfigLanguageDialog::_KeyDown > " + comboBoxCode->Text);
		comboBoxCode_Leave(sender, e);
	}
}

/* ����R�[�h�R���{�{�b�N�X����t�H�[�J�X�𗣂����Ƃ� */
System::Void ConfigWikipediaDialog::comboBoxCode_Leave(System::Object^  sender, System::EventArgs^  e)
{
	System::Diagnostics::Debug::WriteLine("ConfigLanguageDialog::_Leave > " + comboBoxCode->Text);
	// ���݂̒l���ꗗ�ɖ�����Γo�^����
	comboBoxCode->Text = comboBoxCode->Text->Trim()->ToLower();
	if(comboBoxCode->Text != ""){
		if(MYAPP::Cmn::AddComboBoxNewItem(comboBoxCode) == true){
			// �o�^�����ꍇ�����o�ϐ��ɂ��o�^
			WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(config->GetLanguage(comboBoxCode->Text));
			// ���݂��Ȃ��͂������ꉞ�͊m�F���Ēǉ�
			if(svr == nullptr){
				svr = gcnew WikipediaInformation(comboBoxCode->Text);
				MYAPP::Cmn::AddArray(config->Languages, static_cast<LanguageInformation^>(svr));
				// ��^��̐ݒ�\�ɗ��ǉ�
				dataGridViewTitleKey->Columns->Add(comboBoxCode->Text, comboBoxCode->Text);
			}
			// �o�^�����l��I����ԂɕύX
			comboBoxCode->SelectedItem = comboBoxCode->Text;
		}
	}
	else{
		// ��ɂ����Ƃ��A�ύX�ŃC�x���g���N����Ȃ��悤�Ȃ̂ŁA�����I�ɌĂ�
		comboBoxCode_SelectedIndexChanged(sender, e);
	}
}

/* �e����ł̌ď̕\����t�H�[�J�X�𗣂����Ƃ� */
System::Void ConfigWikipediaDialog::dataGridViewName_Leave(System::Object^  sender, System::EventArgs^  e)
{
	// �l�`�F�b�N
	String ^codeUnsetRows = "";
	String ^nameUnsetRows = "";
	String ^redundantCodeRows = "";
	for(int y = 0 ; y < dataGridViewName->RowCount - 1 ; y++){
		// ����R�[�h��́A�������̃f�[�^�ɕϊ�
		dataGridViewName["Code", y]->Value = MYAPP::Cmn::NullCheckAndTrim(dataGridViewName["Code", y])->ToLower();
		// ����R�[�h���ݒ肳��Ă��Ȃ��s�����邩�H
		if(dataGridViewName["Code", y]->Value->ToString() == ""){
			if(codeUnsetRows != ""){
				codeUnsetRows += ",";
			}
			codeUnsetRows += (y + 1);
		}
		else{
			// ����R�[�h���d�����Ă��Ȃ����H
			for(int i = 0 ; i < y ; i++){
				if(dataGridViewName["Code", i]->Value->ToString() == dataGridViewName["Code", y]->Value->ToString()){
					if(redundantCodeRows != ""){
						redundantCodeRows += ",";
					}
					redundantCodeRows += (y + 1);
					break;
				}
			}
			// �ď̂��ݒ肳��Ă��Ȃ��̂ɗ��̂��ݒ肳��Ă��Ȃ����H
			if(MYAPP::Cmn::NullCheckAndTrim(dataGridViewName["ShortName", y]) != "" &&
			   MYAPP::Cmn::NullCheckAndTrim(dataGridViewName["ArticleName", y]) == ""){
				if(nameUnsetRows != ""){
					nameUnsetRows += ",";
				}
				nameUnsetRows += (y + 1);
			}
		}
	}
	// ���ʂ̕\��
	String ^errorMessage = "";
	if(codeUnsetRows != ""){
		errorMessage += (String::Format(cmnAP->Resource->GetString("WarningMessage_UnsetCodeColumn"), codeUnsetRows));
	}
	if(redundantCodeRows != ""){
		if(errorMessage != ""){
			errorMessage += "\n";
		}
		errorMessage += (String::Format(cmnAP->Resource->GetString("WarningMessage_RedundantCodeColumn"), redundantCodeRows));
	}
	if(nameUnsetRows != ""){
		if(errorMessage != ""){
			errorMessage += "\n";
		}
		errorMessage += (String::Format(cmnAP->Resource->GetString("WarningMessage_UnsetArticleNameColumn"), nameUnsetRows));
	}
	if(errorMessage != ""){
		MessageBox::Show(errorMessage, cmnAP->Resource->GetString("WarningTitle"), MessageBoxButtons::OK, MessageBoxIcon::Warning);
		dataGridViewName->Focus();
	}
}

/* ����R�[�h�R���{�{�b�N�X�̃R���e�L�X�g���j���[�F����R�[�h��ύX */
System::Void ConfigWikipediaDialog::toolStripMenuItemModify_Click(System::Object^  sender, System::EventArgs^  e)
{
	// �I������Ă��錾��R�[�h�Ɋ֘A��������X�V
	if(comboBoxCode->SelectedIndex != -1){
		String ^oldCode = comboBoxCode->SelectedItem->ToString();
		// ���͉�ʂɂĕύX��̌���R�[�h���擾
		InputLanguageCodeDialog ^dialog = gcnew InputLanguageCodeDialog();
		dialog->LanguageCode = oldCode;
		if(dialog->ShowDialog() == System::Windows::Forms::DialogResult::OK){
			String ^newCode = dialog->LanguageCode;
			// �����o�ϐ����X�V
			LanguageInformation ^lang = config->GetLanguage(oldCode);
			if(lang != nullptr){
				lang->Code = newCode;
			}
			for each(LanguageInformation ^langIndex in config->Languages){
				if(langIndex->GetType() != WikipediaInformation::typeid){
					continue;
				}
				for each(LanguageInformation::LanguageName name in langIndex->Names){
					if(name.Code == oldCode){
						name.Code = newCode;
					}
				}
			}
			// �R���{�{�b�N�X���X�V
			int index = comboBoxCode->Items->IndexOf(comboBoxCode->SelectedItem);
			comboBoxCode->Items[index] = newCode;
			// ��^��̐ݒ�\���X�V
			String ^header = lang->GetName(System::Globalization::CultureInfo::CurrentCulture->TwoLetterISOLanguageName);
			if(header != ""){
				header += (" (" + newCode + ")");
			}
			else{
				header = newCode;
			}
			dataGridViewTitleKey->Columns[oldCode]->HeaderText = header;
			dataGridViewTitleKey->Columns[oldCode]->Name = newCode;
			// ��ʂ̏�Ԃ��X�V
			comboBoxCode_SelectedIndexChanged(sender, e);
		}
		delete dialog;
	}
}

/* ����R�[�h�R���{�{�b�N�X�̃R���e�L�X�g���j���[�F������폜 */
System::Void ConfigWikipediaDialog::toolStripMenuItemDelete_Click(System::Object^  sender, System::EventArgs^  e)
{
	// �I������Ă��錾��R�[�h�Ɋ֘A��������폜
	if(comboBoxCode->SelectedIndex != -1){
		dataGridViewTitleKey->Columns->Remove(comboBoxCode->SelectedItem->ToString());
		// �����o�ϐ�������폜
		array<LanguageInformation^> ^newLanguages = gcnew array<LanguageInformation^>(0);
		for each(LanguageInformation ^lang in config->Languages){
			if(lang->Code == comboBoxCode->SelectedItem->ToString() &&
			   lang->GetType() == WikipediaInformation::typeid){
				continue;
			}
			MYAPP::Cmn::AddArray(newLanguages, lang);
		}
		config->Languages = newLanguages;
	}
	MYAPP::Cmn::RemoveComboBoxItem(comboBoxCode);
	// ��ʂ̏�Ԃ��X�V
	comboBoxCode_SelectedIndexChanged(sender, e);
}
