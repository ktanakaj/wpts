// ConfigLanguageDialogのボタン動作本体や、自作メソッドなど
#include "StdAfx.h"
#include "ConfigWikipediaDialog.h"
#include "InputLanguageCodeDialog.h"

using namespace wpts;

/* 初期化 */
System::Void ConfigWikipediaDialog::ConfigWikipediaDialog_Load(System::Object^  sender, System::EventArgs^  e)
{
	// 初期化処理
	cmnAP = gcnew MYAPP::Cmn();
	config = gcnew Config(IO::Path::Combine(Application::StartupPath, IO::Path::GetFileNameWithoutExtension(Application::ExecutablePath) + ".xml"));

	// ツールチップの設定
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

	// データ設定
	comboBoxCodeSelectedText = "";
	comboBoxCode->Items->Clear();
	dataGridViewName->Rows->Clear();
	dataGridViewTitleKey->Columns->Clear();
	// 使用言語取得
	String ^showCode = System::Globalization::CultureInfo::CurrentCulture->TwoLetterISOLanguageName;
	int x = 0;
	for each(LanguageInformation ^lang in config->Languages){
		WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(lang);
		if(svr != nullptr){
			// 表タイトル設定
			String ^name = svr->GetName(showCode);
			if(name != ""){
				name += (" (" + svr->Code + ")");
			}
			else{
				name = svr->Code;
			}
			dataGridViewTitleKey->Columns->Add(svr->Code, name);
			// 表データ設定
			for(int y = 0 ; y < svr->TitleKeys->Length ; y++){
				if(dataGridViewTitleKey->RowCount - 1 <= y){
					dataGridViewTitleKey->Rows->Add();
				}
				dataGridViewTitleKey[x, y]->Value = svr->TitleKeys[y];
			}
			// コンボボックス設定
			comboBoxCode->Items->Add(svr->Code);
			// 次の列へ
			++x;
		}
	}
	dataGridViewTitleKey->CurrentCell = nullptr;
}

/* OKボタン押下 */
System::Void ConfigWikipediaDialog::buttonOK_Click(System::Object^  sender, System::EventArgs^  e)
{
	// 設定を保存し、画面を閉じる
	// 表示列の現在処理中データを確定
	comboBoxCode_SelectedIndexChanged(sender, e);
	// 表の状態をメンバ変数に保存
	// 領域の初期化
	for each(LanguageInformation ^lang in config->Languages){
		WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(lang);
		if(svr != nullptr){
			Array::Resize(svr->TitleKeys, dataGridViewTitleKey->RowCount - 1);
		}
	}
	// データの保存
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
	// ソート
	Array::Sort(config->Languages);

	// 設定をファイルに保存
	if(config->Save() == true){
		// 画面を閉じて、設定終了
		this->Close();
	}
	else{
		// エラーメッセージを表示、画面は開いたまま
		cmnAP->ErrorDialogResource("ErrorMessage_MissConfigSave");
	}
	return;
}

/* 言語コードコンボボックスの変更 */
System::Void ConfigWikipediaDialog::comboBoxCode_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e)
{
	System::Diagnostics::Debug::WriteLine("ConfigLanguageDialog::_SelectedIndexChanged > "
		+ comboBoxCodeSelectedText + " -> "
		+ ((comboBoxCode->SelectedItem != nullptr) ? (comboBoxCode->SelectedItem->ToString()) : ("")));

	// 変更前の設定を保存
	// ※変更前にしろ変更後にしろ、事前に追加しているのでGetLanguageで見つからないことは無い・・・はず
	if(comboBoxCodeSelectedText != ""){
		WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(config->GetLanguage(comboBoxCodeSelectedText));
		if(svr != nullptr){
			svr->ArticleXmlPath = textBoxXml->Text->Trim();
			svr->Redirect = textBoxRedirect->Text->Trim();
			// 表から呼称の情報も保存
			dataGridViewName->Sort(dataGridViewName->Columns["Code"], ListSortDirection::Ascending);
			svr->Names = gcnew array<LanguageInformation::LanguageName>(0);
			for(int y = 0 ; y < dataGridViewName->RowCount - 1 ; y++){
				// 値が入ってないとかはガードしているはずだが、一応チェック
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
	// 変更後の値に応じて、画面表示を更新
	if(comboBoxCode->SelectedItem != nullptr){
		// 値を設定
		WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(config->GetLanguage(comboBoxCode->SelectedItem->ToString()));
		if(svr != nullptr){
			textBoxXml->Text = svr->ArticleXmlPath;
			textBoxRedirect->Text = svr->Redirect;
			// 呼称の情報を表に設定
			dataGridViewName->Rows->Clear();
			for each(LanguageInformation::LanguageName name in svr->Names){
				int index = dataGridViewName->Rows->Add();
				dataGridViewName["Code", index]->Value = name.Code;
				dataGridViewName["ArticleName", index]->Value = name.Name;
				dataGridViewName["ShortName", index]->Value = name.ShortName;
			}
		}
		// 言語のプロパティを有効に
		groupBoxStyle->Enabled = true;
		groupBoxName->Enabled = true;
		// 現在の選択値を更新
		comboBoxCodeSelectedText = comboBoxCode->SelectedItem->ToString();
	}
	else{
		// 言語のプロパティを無効に
		groupBoxStyle->Enabled = false;
		groupBoxName->Enabled = false;
		// 現在の選択値を更新
		comboBoxCodeSelectedText = "";
	}
}

/* 言語コードコンボボックスでのキー入力 */
System::Void ConfigWikipediaDialog::comboBoxCode_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e)
{
	// エンターキーが押された場合、現在の値が一覧に無ければ登録する（フォーカスを失ったときの処理）
	if(e->KeyCode == Keys::Enter){
		System::Diagnostics::Debug::WriteLine("ConfigLanguageDialog::_KeyDown > " + comboBoxCode->Text);
		comboBoxCode_Leave(sender, e);
	}
}

/* 言語コードコンボボックスからフォーカスを離したとき */
System::Void ConfigWikipediaDialog::comboBoxCode_Leave(System::Object^  sender, System::EventArgs^  e)
{
	System::Diagnostics::Debug::WriteLine("ConfigLanguageDialog::_Leave > " + comboBoxCode->Text);
	// 現在の値が一覧に無ければ登録する
	comboBoxCode->Text = comboBoxCode->Text->Trim()->ToLower();
	if(comboBoxCode->Text != ""){
		if(MYAPP::Cmn::AddComboBoxNewItem(comboBoxCode) == true){
			// 登録した場合メンバ変数にも登録
			WikipediaInformation ^svr = dynamic_cast<WikipediaInformation^>(config->GetLanguage(comboBoxCode->Text));
			// 存在しないはずだが一応は確認して追加
			if(svr == nullptr){
				svr = gcnew WikipediaInformation(comboBoxCode->Text);
				MYAPP::Cmn::AddArray(config->Languages, static_cast<LanguageInformation^>(svr));
				// 定型句の設定表に列を追加
				dataGridViewTitleKey->Columns->Add(comboBoxCode->Text, comboBoxCode->Text);
			}
			// 登録した値を選択状態に変更
			comboBoxCode->SelectedItem = comboBoxCode->Text;
		}
	}
	else{
		// 空にしたとき、変更でイベントが起こらないようなので、強制的に呼ぶ
		comboBoxCode_SelectedIndexChanged(sender, e);
	}
}

/* 各言語での呼称表からフォーカスを離したとき */
System::Void ConfigWikipediaDialog::dataGridViewName_Leave(System::Object^  sender, System::EventArgs^  e)
{
	// 値チェック
	String ^codeUnsetRows = "";
	String ^nameUnsetRows = "";
	String ^redundantCodeRows = "";
	for(int y = 0 ; y < dataGridViewName->RowCount - 1 ; y++){
		// 言語コード列は、小文字のデータに変換
		dataGridViewName["Code", y]->Value = MYAPP::Cmn::NullCheckAndTrim(dataGridViewName["Code", y])->ToLower();
		// 言語コードが設定されていない行があるか？
		if(dataGridViewName["Code", y]->Value->ToString() == ""){
			if(codeUnsetRows != ""){
				codeUnsetRows += ",";
			}
			codeUnsetRows += (y + 1);
		}
		else{
			// 言語コードが重複していないか？
			for(int i = 0 ; i < y ; i++){
				if(dataGridViewName["Code", i]->Value->ToString() == dataGridViewName["Code", y]->Value->ToString()){
					if(redundantCodeRows != ""){
						redundantCodeRows += ",";
					}
					redundantCodeRows += (y + 1);
					break;
				}
			}
			// 呼称が設定されていないのに略称が設定されていないか？
			if(MYAPP::Cmn::NullCheckAndTrim(dataGridViewName["ShortName", y]) != "" &&
			   MYAPP::Cmn::NullCheckAndTrim(dataGridViewName["ArticleName", y]) == ""){
				if(nameUnsetRows != ""){
					nameUnsetRows += ",";
				}
				nameUnsetRows += (y + 1);
			}
		}
	}
	// 結果の表示
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

/* 言語コードコンボボックスのコンテキストメニュー：言語コードを変更 */
System::Void ConfigWikipediaDialog::toolStripMenuItemModify_Click(System::Object^  sender, System::EventArgs^  e)
{
	// 選択されている言語コードに関連する情報を更新
	if(comboBoxCode->SelectedIndex != -1){
		String ^oldCode = comboBoxCode->SelectedItem->ToString();
		// 入力画面にて変更後の言語コードを取得
		InputLanguageCodeDialog ^dialog = gcnew InputLanguageCodeDialog();
		dialog->LanguageCode = oldCode;
		if(dialog->ShowDialog() == System::Windows::Forms::DialogResult::OK){
			String ^newCode = dialog->LanguageCode;
			// メンバ変数を更新
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
			// コンボボックスを更新
			int index = comboBoxCode->Items->IndexOf(comboBoxCode->SelectedItem);
			comboBoxCode->Items[index] = newCode;
			// 定型句の設定表を更新
			String ^header = lang->GetName(System::Globalization::CultureInfo::CurrentCulture->TwoLetterISOLanguageName);
			if(header != ""){
				header += (" (" + newCode + ")");
			}
			else{
				header = newCode;
			}
			dataGridViewTitleKey->Columns[oldCode]->HeaderText = header;
			dataGridViewTitleKey->Columns[oldCode]->Name = newCode;
			// 画面の状態を更新
			comboBoxCode_SelectedIndexChanged(sender, e);
		}
		delete dialog;
	}
}

/* 言語コードコンボボックスのコンテキストメニュー：言語を削除 */
System::Void ConfigWikipediaDialog::toolStripMenuItemDelete_Click(System::Object^  sender, System::EventArgs^  e)
{
	// 選択されている言語コードに関連する情報を削除
	if(comboBoxCode->SelectedIndex != -1){
		dataGridViewTitleKey->Columns->Remove(comboBoxCode->SelectedItem->ToString());
		// メンバ変数からも削除
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
	// 画面の状態を更新
	comboBoxCode_SelectedIndexChanged(sender, e);
}
