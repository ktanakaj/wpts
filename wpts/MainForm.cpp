// MainForm�̃{�^������{�̂�A���상�\�b�h�Ȃ�
#include "stdafx.h"
#include "MainForm.h"
#include "ConfigWikipediaDialog.h"

using namespace wpts;

/* ������ */
System::Void MainForm::MainForm_Load(System::Object^  sender, System::EventArgs^  e)
{
	// ����������
	cmnAP = gcnew MYAPP::Cmn();
	config = gcnew Config(Path::Combine(Application::StartupPath, Path::GetFileNameWithoutExtension(Application::ExecutablePath) + ".xml"));
	transAP = nullptr;
	textBoxLog->CheckForIllegalCrossThreadCalls = false;

	// �c�[���`�b�v�̐ݒ�
	toolTip->SetToolTip(groupBoxTransfer, cmnAP->Resource->GetString("ToolTipMain_GroupBoxTransfer"));
	toolTip->SetToolTip(comboBoxSource, cmnAP->Resource->GetString("ToolTipMain_ComboBoxSource"));
	toolTip->SetToolTip(comboBoxTarget, cmnAP->Resource->GetString("ToolTipMain_ComboBoxTarget"));
	toolTip->SetToolTip(labelSource, cmnAP->Resource->GetString("ToolTipMain_LabelSource"));
	toolTip->SetToolTip(labelTarget, cmnAP->Resource->GetString("ToolTipMain_LabelTarget"));
	toolTip->SetToolTip(linkLabelSourceURL, cmnAP->Resource->GetString("ToolTipMain_LinkLabelSourceURL"));
	toolTip->SetToolTip(buttonConfig, cmnAP->Resource->GetString("ToolTipMain_ButtonConfig"));
	toolTip->SetToolTip(groupBoxSaveDirectory, cmnAP->Resource->GetString("ToolTipMain_GroupBoxSaveDirectory"));
	toolTip->SetToolTip(buttonSaveDirectory, cmnAP->Resource->GetString("ToolTipMain_ButtonSaveDirectory"));
	toolTip->SetToolTip(textBoxSaveDirectory, cmnAP->Resource->GetString("ToolTipMain_TextBoxSaveDirectory"));
	toolTip->SetToolTip(groupBoxRun, cmnAP->Resource->GetString("ToolTipMain_GroupBoxRun"));
	toolTip->SetToolTip(textBoxArticle, cmnAP->Resource->GetString("ToolTipMain_TextBoxArticle"));
	toolTip->SetToolTip(labelArticle, cmnAP->Resource->GetString("ToolTipMain_TextBoxArticle"));
	toolTip->SetToolTip(buttonRun, cmnAP->Resource->GetString("ToolTipMain_ButtonRun"));
	toolTip->SetToolTip(buttonStop, cmnAP->Resource->GetString("ToolTipMain_ButtonStop"));

	// �R���{�{�b�N�X�ݒ�
	initialize();

	// �O��̏�����Ԃ𕜌�
	textBoxSaveDirectory->Text = config->Client->SaveDirectory;
	comboBoxSource->SelectedText = config->Client->LastSelectedSource;
	comboBoxTarget->SelectedText = config->Client->LastSelectedTarget;
	// �R���{�{�b�N�X�ύX���̏������R�[��
	comboBoxSource_SelectedIndexChanged(sender, e);
	comboBoxTarget_SelectedIndexChanged(sender, e);
}

/* �I�������A������Ԃ�ۑ� */
System::Void MainForm::MainForm_FormClosed(System::Object^  sender, System::Windows::Forms::FormClosedEventArgs^  e)
{
	// ���������グ�̏ꍇ�A����ݒ肪�X�V����Ă���\��������̂ŁA�ݒ���ēǂݍ���
	config->Load();
	// ���݂̍�ƃt�H���_�A�i���ݕ������ۑ�
	config->Client->SaveDirectory = textBoxSaveDirectory->Text;
	config->Client->LastSelectedSource = comboBoxSource->Text;
	config->Client->LastSelectedTarget = comboBoxTarget->Text;
	config->Save();
	// �L���b�V���t�H���_�̌Â��t�@�C���̃N���A
	try{
		DirectoryInfo ^dir = gcnew DirectoryInfo(Path::Combine(Path::GetTempPath(), Path::GetFileNameWithoutExtension(Application::ExecutablePath)));
		if(dir->Exists == true){
			array<FileInfo^> ^files = dir->GetFiles("*.xml");
			for each(FileInfo ^file in files){
				// 1�T�Ԉȏ�O�̃L���b�V���͍폜
				if((DateTime::UtcNow - file->LastWriteTimeUtc) > TimeSpan(7, 0, 0, 0)){
					// ����������Ȃ�������A�������Ď��̃t�@�C����
					try{
						file->Delete();
					}
					catch(Exception ^e){
						System::Diagnostics::Debug::WriteLine("MainForm::_FormClosed > �L���b�V���폜���ɗ�O : " + e->Message);
					}
				}
			}
		}
	}
	catch(Exception ^e){
		System::Diagnostics::Debug::WriteLine("MainForm::_FormClosed > �L���b�V���폜���ɗ�O : " + e->Message);
	}
}

/* �|�󌳃R���{�{�b�N�X�̕ύX */
System::Void MainForm::comboBoxSource_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e)
{
	// ���x���Ɍ��ꖼ��\������
	if(comboBoxSource->Text != ""){
		comboBoxSource->Text = comboBoxSource->Text->Trim()->ToLower();
		LanguageInformation ^lang = config->GetLanguage(comboBoxSource->Text);
		if(lang != nullptr){
			labelSource->Text = lang->GetName(System::Globalization::CultureInfo::CurrentCulture->TwoLetterISOLanguageName);
		}
		else{
			labelSource->Text = "";
		}
		// �T�[�o�[URL�̕\��
		if(config->Client->RunMode == Config::RunType::Wikipedia){
			WikipediaInformation ^svr;
			if(lang != nullptr){
				svr = dynamic_cast<WikipediaInformation^>(lang);
			}
			if(svr == nullptr){
				svr = gcnew WikipediaInformation(comboBoxSource->Text);
			}
			linkLabelSourceURL->Text = "http://" + svr->Server;
		}
		else{
			// �����̊g���i�H�j�p
			linkLabelSourceURL->Text = "http://";
		}
	}
	else{
		labelSource->Text = "";
		linkLabelSourceURL->Text = "http://";
	}
}

/* �|���R���{�{�b�N�X�̕ύX */
System::Void MainForm::comboBoxTarget_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e)
{
	// ���x���Ɍ��ꖼ��\������
	if(comboBoxTarget->Text != ""){
		comboBoxTarget->Text = comboBoxTarget->Text->Trim()->ToLower();
		LanguageInformation ^lang = config->GetLanguage(comboBoxTarget->Text);
		if(lang != nullptr){
			labelTarget->Text = lang->GetName(System::Globalization::CultureInfo::CurrentCulture->TwoLetterISOLanguageName);
		}
		else{
			labelTarget->Text = "";
		}
	}
	else{
		labelTarget->Text = "";
	}
}

/* �|�󌳃R���{�{�b�N�X�̃t�H�[�J�X�r�� */
System::Void MainForm::comboBoxSource_Leave(System::Object^  sender, System::EventArgs^  e)
{
	// ���ړ��͂��ꂽ�ꍇ�̑΍�A�ύX���̏������R�[��
	comboBoxSource_SelectedIndexChanged(sender, e);
}

/* �|���R���{�{�b�N�X�̃t�H�[�J�X�r�� */
System::Void MainForm::comboBoxTarget_Leave(System::Object^  sender, System::EventArgs^  e)
{
	// ���ړ��͂��ꂽ�ꍇ�̑΍�A�ύX���̏������R�[��
	comboBoxTarget_SelectedIndexChanged(sender, e);
}

/* �o�͐�e�L�X�g�{�b�N�X�̃t�H�[�J�X�r�� */
System::Void MainForm::textBoxSaveDirectory_Leave(System::Object^  sender, System::EventArgs^  e)
{
	// �󔒂��폜
	textBoxSaveDirectory->Text = textBoxSaveDirectory->Text->Trim();
}

/* �����N���x���̃����N�N���b�N */
System::Void MainForm::linkLabelSourceURL_LinkClicked(System::Object^  sender, System::Windows::Forms::LinkLabelLinkClickedEventArgs^  e)
{
	// �����N���J��
	System::Diagnostics::Process::Start(linkLabelSourceURL->Text);
}

/* �ݒ�{�^������ */
System::Void MainForm::buttonConfig_Click(System::Object^  sender, System::EventArgs^  e)
{
	if(config->Client->RunMode == Config::RunType::Wikipedia){
		// ����̐ݒ��ʂ��J��
		ConfigWikipediaDialog ^dialog = gcnew ConfigWikipediaDialog();
		dialog->ShowDialog();
		// ���ʂɊ֌W�Ȃ��A�ݒ���ēǂݍ���
		config->Load();
		// �R���{�{�b�N�X�ݒ�
		String ^backupSourceSelected = comboBoxSource->SelectedText;
		String ^backupSourceTarget = comboBoxTarget->SelectedText;
		initialize();
		comboBoxSource->SelectedText = backupSourceSelected;
		comboBoxTarget->SelectedText = backupSourceTarget;
		// �R���{�{�b�N�X�ύX���̏������R�[��
		comboBoxSource_SelectedIndexChanged(sender, e);
		comboBoxTarget_SelectedIndexChanged(sender, e);
		// �_�C�A���O������
		delete dialog;
	}
	else{
		// �����̊g���i�H�j�p
		cmnAP->InformationDialogResource("InformationMessage_DevelopingMethod", "Wikipedia�ȊO�̏���");
	}
}

/* �Q�ƃ{�^������ */
System::Void MainForm::buttonSaveDirectory_Click(System::Object^  sender, System::EventArgs^  e)
{
	// �t�H���_�������͂���Ă���ꍇ�A����������ʒu�ɐݒ�
	if(textBoxSaveDirectory->Text != ""){
		folderBrowserDialogSaveDirectory->SelectedPath = textBoxSaveDirectory->Text;
	}
	// �t�H���_�I����ʂ��I�[�v��
	if(folderBrowserDialogSaveDirectory->ShowDialog() == System::Windows::Forms::DialogResult::OK){
		// �t�H���_���I�����ꂽ�ꍇ�A�t�H���_���ɑI�����ꂽ�t�H���_��ݒ�
		textBoxSaveDirectory->Text = folderBrowserDialogSaveDirectory->SelectedPath;
	}
}

/* ���s�{�^������ */
System::Void MainForm::buttonRun_Click(System::Object^  sender, System::EventArgs^  e)
{
	// ���͒l�`�F�b�N
	if(textBoxArticle->Text->Trim() == ""){
		// �l���ݒ肳��Ă��Ȃ��Ƃ��͏�������
		return;
	}
	// �K�v�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�͏����s��
	if(Directory::Exists(textBoxSaveDirectory->Text) == false){
		cmnAP->WarningDialogResource("WarningMessage_UnuseSaveDirectory");
		buttonSaveDirectory->Focus();
		return;
	}
	else if(comboBoxSource->Text == ""){
		cmnAP->WarningDialogResource("WarningMessage_NotSelectedSource");
		comboBoxSource->Focus();
		return;
	}
	else if(comboBoxTarget->Text == ""){
		cmnAP->WarningDialogResource("WarningMessage_NotSelectedTarget");
		comboBoxTarget->Focus();
		return;
	}
	else if(comboBoxSource->Text == comboBoxTarget->Text){
		cmnAP->WarningDialogResource("WarningMessage_SourceEqualTarget");
		comboBoxTarget->Focus();
		return;
	}
	// ��ʂ����b�N
	lock();
	// �o�b�N�O���E���h���������s
	backgroundWorkerRun->RunWorkerAsync();
}

/* ���~�{�^������ */
System::Void MainForm::buttonStop_Click(System::Object^  sender, System::EventArgs^  e)
{
	// �����𒆒f
	buttonStop->Enabled = false;
	if(backgroundWorkerRun->IsBusy == true){
		System::Diagnostics::Debug::WriteLine("MainForm::-Stop_Click > �������f");
		backgroundWorkerRun->CancelAsync();
		if(transAP != nullptr){
			transAP->CancellationPending = true;
		}
	}
}

/* ���s�{�^�� �o�b�N�O���E���h�����i�X���b�h�j */
System::Void MainForm::backgroundWorkerRun_DoWork(System::Object^  sender, System::ComponentModel::DoWorkEventArgs^  e)
{
	try{
		// �|��x�������̑O����
		textBoxLog->Clear();
		logLastLength = 0;
		textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("LogMessage_Start"), MYAPP::Cmn::GetProductName(),
			DateTime::Now.ToString("F")));
		// �������ʂƃ��O�̂��߂̏o�̓t�@�C�������쐬
		String ^fileName = "";
		String ^logName = "";
		makeFileName(fileName, logName, textBoxArticle->Text->Trim(), textBoxSaveDirectory->Text);

		// �|��x�����������s���A���ʂƃ��O���t�@�C���ɏo��
		// �������Ώۂɉ�����Translate���p�������I�u�W�F�N�g�𐶐�
		if(config->Client->RunMode == Config::RunType::Wikipedia){
			WikipediaInformation ^source = dynamic_cast<WikipediaInformation^>(config->GetLanguage(comboBoxSource->Text));
			if(source == nullptr){
				source = gcnew WikipediaInformation(comboBoxSource->Text);
			}
			WikipediaInformation ^target = dynamic_cast<WikipediaInformation^>(config->GetLanguage(comboBoxTarget->Text));
			if(target == nullptr){
				target = gcnew WikipediaInformation(comboBoxTarget->Text);
			}
			transAP = gcnew TranslateWikipedia(source, target);
			static_cast<TranslateWikipedia^>(transAP)->UserAgent = config->Client->UserAgent;
			static_cast<TranslateWikipedia^>(transAP)->Referer = config->Client->Referer;
		}
		else{
			// �����̊g���i�H�j�p
			textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("InformationMessage_DevelopingMethod"), "Wikipedia�ȊO�̏���"));
			cmnAP->InformationDialogResource("InformationMessage_DevelopingMethod", "Wikipedia�ȊO�̏���");
			return;
		}
		transAP->LogUpdate += gcnew EventHandler(this, &MainForm::getLogUpdate);
		// ���s�O�ɁA���[�U�[���璆�~�v��������Ă��邩���`�F�b�N
		if(backgroundWorkerRun->CancellationPending == true){
			textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("LogMessage_Stop"), logName));
		}
		// �|��x�����������s
		else{
			bool successFlag = transAP->Run(textBoxArticle->Text->Trim());
			// �����Ɏ��Ԃ������邽�߁A�o�̓t�@�C�������Ċm�F
			makeFileName(fileName, logName, textBoxArticle->Text->Trim(), textBoxSaveDirectory->Text);
			if(successFlag){
				// �������ʂ��o��
				try{
					StreamWriter ^sw = gcnew StreamWriter(Path::Combine(textBoxSaveDirectory->Text, fileName));
					try{
						sw->Write(transAP->Text);
						textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("LogMessage_End"), fileName, logName));
					}
					finally{
						sw->Close();
					}
				}
				catch(Exception ^e){
					textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("LogMessage_ErrorFileSave"), Path::Combine(textBoxSaveDirectory->Text, fileName), e->Message));
					textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("LogMessage_Stop"), logName));
				}
			}
			else{
				textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("LogMessage_Stop"), logName));
			}
		}
		// ���O���o��
		try{
			StreamWriter ^sw = gcnew StreamWriter(Path::Combine(textBoxSaveDirectory->Text, logName));
			try{
				sw->Write(textBoxLog->Text);
			}
			finally{
				sw->Close();
			}
		}
		catch(Exception ^e){
			textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("LogMessage_ErrorFileSave"), Path::Combine(textBoxSaveDirectory->Text, logName), e->Message));
		}
		// �g�p�����I�u�W�F�N�g������i���Ԃ�v��Ȃ����A�O�̂��߁j
		delete transAP;
	}
	catch(Exception ^e){
		textBoxLog->AppendText("\r\n" + String::Format(cmnAP->Resource->GetString("ErrorMessage_DevelopmentMiss"), e->Message) + "\r\n");
	}
}

/* ���s�{�^�� �o�b�N�O���E���h�����i�I�����j */
System::Void MainForm::backgroundWorkerRun_RunWorkerCompleted(System::Object^  sender, System::ComponentModel::RunWorkerCompletedEventArgs^  e)
{
	// ��ʂ����b�N��������
	release();
}

/* ��ʏ��������� */
Void MainForm::initialize(void)
{
	// �R���{�{�b�N�X�ݒ�
	comboBoxSource->Items->Clear();
	comboBoxTarget->Items->Clear();
	for each(LanguageInformation ^lang in config->Languages){
		comboBoxSource->Items->Add(lang->Code);
		comboBoxTarget->Items->Add(lang->Code);
	}
	return;
}

/* ��ʂ����b�N���Ɉڍs */
void MainForm::lock(void)
{
	// �e��{�^���Ȃǂ���͕s�ɕύX
	groupBoxTransfer->Enabled = false;
	groupBoxSaveDirectory->Enabled = false;
	textBoxArticle->Enabled = false;
	buttonRun->Enabled = false;
	// ���~�{�^����L���ɕύX
	buttonStop->Enabled = true;
}

/* ��ʂ����b�N�������� */
void MainForm::release(void)
{
	// ���~�{�^������͕s�ɕύX
	buttonStop->Enabled = false;
	// �e��{�^���Ȃǂ�L���ɕύX
	groupBoxTransfer->Enabled = true;
	groupBoxSaveDirectory->Enabled = true;
	textBoxArticle->Enabled = true;
	buttonRun->Enabled = true;
}

//* �n���ꂽ��������t�@�C�����Ɏg���镶����ɕϊ� */
bool MainForm::makeFileName(String ^%o_FileName, String ^%o_LogName, String ^i_Text, String ^i_Dir)
{
	// �o�͒l������
	o_FileName = "";
	o_LogName = "";
	// �o�͐�t�H���_�ɑ��݂��Ȃ��t�@�C�����i�̊g���q���O�j���쐬
	// ���n���ꂽWikipedia���̋L�����Ƀt�@�C�����Ɏg���Ȃ��������܂܂�Ă���ꍇ�A_ �ɒu��������
	//   �܂��A�t�@�C�������d�����Ă���ꍇ�Axx[0].txt�̂悤�ɘA�Ԃ�t����
	String ^fileNameBase = MYAPP::Cmn::ReplaceInvalidFileNameChars(i_Text);
	String ^fileName = fileNameBase + ".txt";
	String ^logName = fileNameBase + ".log";
	bool successFlag = false;
	for(int i = 0 ; i < 100000 ; i++){
		// ��100000�܂Ŏ����ċ󂫂�������Ȃ����Ƃ͖����͂��A����������Ȃ�������Ō�̂��㏑��
		if(File::Exists(Path::Combine(i_Dir, fileName)) == false &&
		   File::Exists(Path::Combine(i_Dir, logName)) == false){
			successFlag = true;
			break;
		}
		fileName = fileNameBase + "[" + i + "]" + ".txt";
		logName = fileNameBase + "[" + i + "]" + ".log";
	}
	// ���ʐݒ�
	o_FileName = fileName;
	o_LogName = logName;
	return successFlag;
}

/* �|��x�������N���X�̃C�x���g�p */
void MainForm::getLogUpdate(System::Object^  sender, System::EventArgs^  e)
{
	// �O��ȍ~�ɒǉ����ꂽ���O���e�L�X�g�{�b�N�X�ɏo��
	int length = transAP->Log->Length;
	if(length > logLastLength){
		textBoxLog->AppendText(transAP->Log->Substring(logLastLength, length - logLastLength));
	}
	logLastLength = length;
}
