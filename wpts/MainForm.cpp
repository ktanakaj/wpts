// MainFormのボタン動作本体や、自作メソッドなど
#include "stdafx.h"
#include "MainForm.h"
#include "ConfigWikipediaDialog.h"

using namespace wpts;

/* 初期化 */
System::Void MainForm::MainForm_Load(System::Object^  sender, System::EventArgs^  e)
{
	// 初期化処理
	cmnAP = gcnew MYAPP::Cmn();
	config = gcnew Config(Path::Combine(Application::StartupPath, Path::GetFileNameWithoutExtension(Application::ExecutablePath) + ".xml"));
	transAP = nullptr;
	textBoxLog->CheckForIllegalCrossThreadCalls = false;

	// ツールチップの設定
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

	// コンボボックス設定
	initialize();

	// 前回の処理状態を復元
	textBoxSaveDirectory->Text = config->Client->SaveDirectory;
	comboBoxSource->SelectedText = config->Client->LastSelectedSource;
	comboBoxTarget->SelectedText = config->Client->LastSelectedTarget;
	// コンボボックス変更時の処理をコール
	comboBoxSource_SelectedIndexChanged(sender, e);
	comboBoxTarget_SelectedIndexChanged(sender, e);
}

/* 終了処理、処理状態を保存 */
System::Void MainForm::MainForm_FormClosed(System::Object^  sender, System::Windows::Forms::FormClosedEventArgs^  e)
{
	// 複数立ち上げの場合、言語設定が更新されている可能性があるので、設定を再読み込み
	config->Load();
	// 現在の作業フォルダ、絞込み文字列を保存
	config->Client->SaveDirectory = textBoxSaveDirectory->Text;
	config->Client->LastSelectedSource = comboBoxSource->Text;
	config->Client->LastSelectedTarget = comboBoxTarget->Text;
	config->Save();
	// キャッシュフォルダの古いファイルのクリア
	try{
		DirectoryInfo ^dir = gcnew DirectoryInfo(Path::Combine(Path::GetTempPath(), Path::GetFileNameWithoutExtension(Application::ExecutablePath)));
		if(dir->Exists == true){
			array<FileInfo^> ^files = dir->GetFiles("*.xml");
			for each(FileInfo ^file in files){
				// 1週間以上前のキャッシュは削除
				if((DateTime::UtcNow - file->LastWriteTimeUtc) > TimeSpan(7, 0, 0, 0)){
					// 万が一消せなかったら、無視して次のファイルへ
					try{
						file->Delete();
					}
					catch(Exception ^e){
						System::Diagnostics::Debug::WriteLine("MainForm::_FormClosed > キャッシュ削除時に例外 : " + e->Message);
					}
				}
			}
		}
	}
	catch(Exception ^e){
		System::Diagnostics::Debug::WriteLine("MainForm::_FormClosed > キャッシュ削除中に例外 : " + e->Message);
	}
}

/* 翻訳元コンボボックスの変更 */
System::Void MainForm::comboBoxSource_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e)
{
	// ラベルに言語名を表示する
	if(comboBoxSource->Text != ""){
		comboBoxSource->Text = comboBoxSource->Text->Trim()->ToLower();
		LanguageInformation ^lang = config->GetLanguage(comboBoxSource->Text);
		if(lang != nullptr){
			labelSource->Text = lang->GetName(System::Globalization::CultureInfo::CurrentCulture->TwoLetterISOLanguageName);
		}
		else{
			labelSource->Text = "";
		}
		// サーバーURLの表示
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
			// 将来の拡張（？）用
			linkLabelSourceURL->Text = "http://";
		}
	}
	else{
		labelSource->Text = "";
		linkLabelSourceURL->Text = "http://";
	}
}

/* 翻訳先コンボボックスの変更 */
System::Void MainForm::comboBoxTarget_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e)
{
	// ラベルに言語名を表示する
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

/* 翻訳元コンボボックスのフォーカス喪失 */
System::Void MainForm::comboBoxSource_Leave(System::Object^  sender, System::EventArgs^  e)
{
	// 直接入力された場合の対策、変更字の処理をコール
	comboBoxSource_SelectedIndexChanged(sender, e);
}

/* 翻訳先コンボボックスのフォーカス喪失 */
System::Void MainForm::comboBoxTarget_Leave(System::Object^  sender, System::EventArgs^  e)
{
	// 直接入力された場合の対策、変更字の処理をコール
	comboBoxTarget_SelectedIndexChanged(sender, e);
}

/* 出力先テキストボックスのフォーカス喪失 */
System::Void MainForm::textBoxSaveDirectory_Leave(System::Object^  sender, System::EventArgs^  e)
{
	// 空白を削除
	textBoxSaveDirectory->Text = textBoxSaveDirectory->Text->Trim();
}

/* リンクラベルのリンククリック */
System::Void MainForm::linkLabelSourceURL_LinkClicked(System::Object^  sender, System::Windows::Forms::LinkLabelLinkClickedEventArgs^  e)
{
	// リンクを開く
	System::Diagnostics::Process::Start(linkLabelSourceURL->Text);
}

/* 設定ボタン押下 */
System::Void MainForm::buttonConfig_Click(System::Object^  sender, System::EventArgs^  e)
{
	if(config->Client->RunMode == Config::RunType::Wikipedia){
		// 言語の設定画面を開く
		ConfigWikipediaDialog ^dialog = gcnew ConfigWikipediaDialog();
		dialog->ShowDialog();
		// 結果に関係なく、設定を再読み込み
		config->Load();
		// コンボボックス設定
		String ^backupSourceSelected = comboBoxSource->SelectedText;
		String ^backupSourceTarget = comboBoxTarget->SelectedText;
		initialize();
		comboBoxSource->SelectedText = backupSourceSelected;
		comboBoxTarget->SelectedText = backupSourceTarget;
		// コンボボックス変更時の処理をコール
		comboBoxSource_SelectedIndexChanged(sender, e);
		comboBoxTarget_SelectedIndexChanged(sender, e);
		// ダイアログを消去
		delete dialog;
	}
	else{
		// 将来の拡張（？）用
		cmnAP->InformationDialogResource("InformationMessage_DevelopingMethod", "Wikipedia以外の処理");
	}
}

/* 参照ボタン押下 */
System::Void MainForm::buttonSaveDirectory_Click(System::Object^  sender, System::EventArgs^  e)
{
	// フォルダ名が入力されている場合、それを初期位置に設定
	if(textBoxSaveDirectory->Text != ""){
		folderBrowserDialogSaveDirectory->SelectedPath = textBoxSaveDirectory->Text;
	}
	// フォルダ選択画面をオープン
	if(folderBrowserDialogSaveDirectory->ShowDialog() == System::Windows::Forms::DialogResult::OK){
		// フォルダが選択された場合、フォルダ名に選択されたフォルダを設定
		textBoxSaveDirectory->Text = folderBrowserDialogSaveDirectory->SelectedPath;
	}
}

/* 実行ボタン押下 */
System::Void MainForm::buttonRun_Click(System::Object^  sender, System::EventArgs^  e)
{
	// 入力値チェック
	if(textBoxArticle->Text->Trim() == ""){
		// 値が設定されていないときは処理無し
		return;
	}
	// 必要な情報が設定されていない場合は処理不可
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
	// 画面をロック
	lock();
	// バックグラウンド処理を実行
	backgroundWorkerRun->RunWorkerAsync();
}

/* 中止ボタン押下 */
System::Void MainForm::buttonStop_Click(System::Object^  sender, System::EventArgs^  e)
{
	// 処理を中断
	buttonStop->Enabled = false;
	if(backgroundWorkerRun->IsBusy == true){
		System::Diagnostics::Debug::WriteLine("MainForm::-Stop_Click > 処理中断");
		backgroundWorkerRun->CancelAsync();
		if(transAP != nullptr){
			transAP->CancellationPending = true;
		}
	}
}

/* 実行ボタン バックグラウンド処理（スレッド） */
System::Void MainForm::backgroundWorkerRun_DoWork(System::Object^  sender, System::ComponentModel::DoWorkEventArgs^  e)
{
	try{
		// 翻訳支援処理の前処理
		textBoxLog->Clear();
		logLastLength = 0;
		textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("LogMessage_Start"), MYAPP::Cmn::GetProductName(),
			DateTime::Now.ToString("F")));
		// 処理結果とログのための出力ファイル名を作成
		String ^fileName = "";
		String ^logName = "";
		makeFileName(fileName, logName, textBoxArticle->Text->Trim(), textBoxSaveDirectory->Text);

		// 翻訳支援処理を実行し、結果とログをファイルに出力
		// ※処理対象に応じてTranslateを継承したオブジェクトを生成
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
			// 将来の拡張（？）用
			textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("InformationMessage_DevelopingMethod"), "Wikipedia以外の処理"));
			cmnAP->InformationDialogResource("InformationMessage_DevelopingMethod", "Wikipedia以外の処理");
			return;
		}
		transAP->LogUpdate += gcnew EventHandler(this, &MainForm::getLogUpdate);
		// 実行前に、ユーザーから中止要求がされているかをチェック
		if(backgroundWorkerRun->CancellationPending == true){
			textBoxLog->AppendText(String::Format(cmnAP->Resource->GetString("LogMessage_Stop"), logName));
		}
		// 翻訳支援処理を実行
		else{
			bool successFlag = transAP->Run(textBoxArticle->Text->Trim());
			// 処理に時間がかかるため、出力ファイル名を再確認
			makeFileName(fileName, logName, textBoxArticle->Text->Trim(), textBoxSaveDirectory->Text);
			if(successFlag){
				// 処理結果を出力
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
		// ログを出力
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
		// 使用したオブジェクトを解放（たぶん要らないが、念のため）
		delete transAP;
	}
	catch(Exception ^e){
		textBoxLog->AppendText("\r\n" + String::Format(cmnAP->Resource->GetString("ErrorMessage_DevelopmentMiss"), e->Message) + "\r\n");
	}
}

/* 実行ボタン バックグラウンド処理（終了時） */
System::Void MainForm::backgroundWorkerRun_RunWorkerCompleted(System::Object^  sender, System::ComponentModel::RunWorkerCompletedEventArgs^  e)
{
	// 画面をロック中から解放
	release();
}

/* 画面初期化処理 */
Void MainForm::initialize(void)
{
	// コンボボックス設定
	comboBoxSource->Items->Clear();
	comboBoxTarget->Items->Clear();
	for each(LanguageInformation ^lang in config->Languages){
		comboBoxSource->Items->Add(lang->Code);
		comboBoxTarget->Items->Add(lang->Code);
	}
	return;
}

/* 画面をロック中に移行 */
void MainForm::lock(void)
{
	// 各種ボタンなどを入力不可に変更
	groupBoxTransfer->Enabled = false;
	groupBoxSaveDirectory->Enabled = false;
	textBoxArticle->Enabled = false;
	buttonRun->Enabled = false;
	// 中止ボタンを有効に変更
	buttonStop->Enabled = true;
}

/* 画面をロック中から解放 */
void MainForm::release(void)
{
	// 中止ボタンを入力不可に変更
	buttonStop->Enabled = false;
	// 各種ボタンなどを有効に変更
	groupBoxTransfer->Enabled = true;
	groupBoxSaveDirectory->Enabled = true;
	textBoxArticle->Enabled = true;
	buttonRun->Enabled = true;
}

//* 渡された文字列をファイル名に使える文字列に変換 */
bool MainForm::makeFileName(String ^%o_FileName, String ^%o_LogName, String ^i_Text, String ^i_Dir)
{
	// 出力値初期化
	o_FileName = "";
	o_LogName = "";
	// 出力先フォルダに存在しないファイル名（の拡張子より前）を作成
	// ※渡されたWikipedia等の記事名にファイル名に使えない文字が含まれている場合、_ に置き換える
	//   また、ファイル名が重複している場合、xx[0].txtのように連番を付ける
	String ^fileNameBase = MYAPP::Cmn::ReplaceInvalidFileNameChars(i_Text);
	String ^fileName = fileNameBase + ".txt";
	String ^logName = fileNameBase + ".log";
	bool successFlag = false;
	for(int i = 0 ; i < 100000 ; i++){
		// ※100000まで試して空きが見つからないことは無いはず、もし見つからなかったら最後のを上書き
		if(File::Exists(Path::Combine(i_Dir, fileName)) == false &&
		   File::Exists(Path::Combine(i_Dir, logName)) == false){
			successFlag = true;
			break;
		}
		fileName = fileNameBase + "[" + i + "]" + ".txt";
		logName = fileNameBase + "[" + i + "]" + ".log";
	}
	// 結果設定
	o_FileName = fileName;
	o_LogName = logName;
	return successFlag;
}

/* 翻訳支援処理クラスのイベント用 */
void MainForm::getLogUpdate(System::Object^  sender, System::EventArgs^  e)
{
	// 前回以降に追加されたログをテキストボックスに出力
	int length = transAP->Log->Length;
	if(length > logLastLength){
		textBoxLog->AppendText(transAP->Log->Substring(logLastLength, length - logLastLength));
	}
	logLastLength = length;
}
