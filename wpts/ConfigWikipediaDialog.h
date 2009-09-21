#include "stdafx.h"
#include "Config.h"

#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace wpts {

	/// <summary>
	/// ConfigLanguageDialog の概要
	///
	/// 警告: このクラスの名前を変更する場合、このクラスが依存するすべての .resx ファイルに関連付けられた
	///          マネージ リソース コンパイラ ツールに対して 'Resource File Name' プロパティを
	///          変更する必要があります。この変更を行わないと、
	///          デザイナと、このフォームに関連付けられたローカライズ済みリソースとが、
	///          正しく相互に利用できなくなります。
	/// </summary>
	public ref class ConfigWikipediaDialog : public System::Windows::Forms::Form
	{
	public:
		ConfigWikipediaDialog(void)
		{
			InitializeComponent();
			//
			//TODO: ここにコンストラクタ コードを追加します
			//
		}

	protected:
		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		~ConfigWikipediaDialog()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::Button^  buttonOK;
	protected: 
	private: System::Windows::Forms::Button^  buttonCancel;
	private: System::Windows::Forms::DataGridView^  dataGridViewTitleKey;
	private: System::Windows::Forms::GroupBox^  groupBoxTitleKey;
	private: System::Windows::Forms::ToolTip^  toolTip;
	private: System::Windows::Forms::Label^  labelCode;
	private: System::Windows::Forms::GroupBox^  groupBoxStyle;
	private: System::Windows::Forms::TextBox^  textBoxRedirect;
	private: System::Windows::Forms::Label^  labelRedirect;
	private: System::Windows::Forms::GroupBox^  groupBoxName;
	private: System::Windows::Forms::DataGridView^  dataGridViewName;
	private: System::Windows::Forms::ComboBox^  comboBoxCode;
	private: System::Windows::Forms::GroupBox^  groupBoxLanguage;
	private: System::Windows::Forms::ContextMenuStrip^  contextMenuStripCode;
	private: System::Windows::Forms::ToolStripMenuItem^  toolStripMenuItemModify;
	private: System::Windows::Forms::ToolStripMenuItem^  toolStripMenuItemDelete;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  Code;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  ArticleName;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  ShortName;
	private: System::Windows::Forms::TextBox^  textBoxXml;
	private: System::Windows::Forms::Label^  labelXml;




	private: System::ComponentModel::IContainer^  components;


	private:
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>


#pragma region Windows Form Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::Windows::Forms::DataGridViewCellStyle^  dataGridViewCellStyle1 = (gcnew System::Windows::Forms::DataGridViewCellStyle());
			System::Windows::Forms::DataGridViewCellStyle^  dataGridViewCellStyle2 = (gcnew System::Windows::Forms::DataGridViewCellStyle());
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(ConfigWikipediaDialog::typeid));
			this->buttonOK = (gcnew System::Windows::Forms::Button());
			this->buttonCancel = (gcnew System::Windows::Forms::Button());
			this->dataGridViewTitleKey = (gcnew System::Windows::Forms::DataGridView());
			this->groupBoxTitleKey = (gcnew System::Windows::Forms::GroupBox());
			this->toolTip = (gcnew System::Windows::Forms::ToolTip(this->components));
			this->labelCode = (gcnew System::Windows::Forms::Label());
			this->groupBoxStyle = (gcnew System::Windows::Forms::GroupBox());
			this->textBoxXml = (gcnew System::Windows::Forms::TextBox());
			this->labelXml = (gcnew System::Windows::Forms::Label());
			this->textBoxRedirect = (gcnew System::Windows::Forms::TextBox());
			this->labelRedirect = (gcnew System::Windows::Forms::Label());
			this->groupBoxName = (gcnew System::Windows::Forms::GroupBox());
			this->dataGridViewName = (gcnew System::Windows::Forms::DataGridView());
			this->Code = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->ArticleName = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->ShortName = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->comboBoxCode = (gcnew System::Windows::Forms::ComboBox());
			this->contextMenuStripCode = (gcnew System::Windows::Forms::ContextMenuStrip(this->components));
			this->toolStripMenuItemModify = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripMenuItemDelete = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->groupBoxLanguage = (gcnew System::Windows::Forms::GroupBox());
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dataGridViewTitleKey))->BeginInit();
			this->groupBoxTitleKey->SuspendLayout();
			this->groupBoxStyle->SuspendLayout();
			this->groupBoxName->SuspendLayout();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dataGridViewName))->BeginInit();
			this->contextMenuStripCode->SuspendLayout();
			this->groupBoxLanguage->SuspendLayout();
			this->SuspendLayout();
			// 
			// buttonOK
			// 
			this->buttonOK->Anchor = System::Windows::Forms::AnchorStyles::Bottom;
			this->buttonOK->Location = System::Drawing::Point(228, 334);
			this->buttonOK->Name = L"buttonOK";
			this->buttonOK->Size = System::Drawing::Size(75, 23);
			this->buttonOK->TabIndex = 1;
			this->buttonOK->Text = L"OK";
			this->buttonOK->UseVisualStyleBackColor = true;
			this->buttonOK->Click += gcnew System::EventHandler(this, &ConfigWikipediaDialog::buttonOK_Click);
			// 
			// buttonCancel
			// 
			this->buttonCancel->Anchor = System::Windows::Forms::AnchorStyles::Bottom;
			this->buttonCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->buttonCancel->Location = System::Drawing::Point(330, 334);
			this->buttonCancel->Name = L"buttonCancel";
			this->buttonCancel->Size = System::Drawing::Size(75, 23);
			this->buttonCancel->TabIndex = 0;
			this->buttonCancel->Text = L"キャンセル";
			this->buttonCancel->UseVisualStyleBackColor = true;
			// 
			// dataGridViewTitleKey
			// 
			this->dataGridViewTitleKey->AllowUserToResizeRows = false;
			this->dataGridViewTitleKey->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->dataGridViewTitleKey->AutoSizeColumnsMode = System::Windows::Forms::DataGridViewAutoSizeColumnsMode::AllCells;
			this->dataGridViewTitleKey->AutoSizeRowsMode = System::Windows::Forms::DataGridViewAutoSizeRowsMode::AllCells;
			dataGridViewCellStyle1->Alignment = System::Windows::Forms::DataGridViewContentAlignment::MiddleLeft;
			dataGridViewCellStyle1->BackColor = System::Drawing::SystemColors::Control;
			dataGridViewCellStyle1->Font = (gcnew System::Drawing::Font(L"MS UI Gothic", 9, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(128)));
			dataGridViewCellStyle1->ForeColor = System::Drawing::SystemColors::WindowText;
			dataGridViewCellStyle1->SelectionBackColor = System::Drawing::SystemColors::Highlight;
			dataGridViewCellStyle1->SelectionForeColor = System::Drawing::SystemColors::HighlightText;
			dataGridViewCellStyle1->WrapMode = System::Windows::Forms::DataGridViewTriState::True;
			this->dataGridViewTitleKey->ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this->dataGridViewTitleKey->ColumnHeadersHeightSizeMode = System::Windows::Forms::DataGridViewColumnHeadersHeightSizeMode::AutoSize;
			dataGridViewCellStyle2->Alignment = System::Windows::Forms::DataGridViewContentAlignment::MiddleLeft;
			dataGridViewCellStyle2->BackColor = System::Drawing::SystemColors::Window;
			dataGridViewCellStyle2->Font = (gcnew System::Drawing::Font(L"MS UI Gothic", 9, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(128)));
			dataGridViewCellStyle2->ForeColor = System::Drawing::SystemColors::ControlText;
			dataGridViewCellStyle2->SelectionBackColor = System::Drawing::SystemColors::Highlight;
			dataGridViewCellStyle2->SelectionForeColor = System::Drawing::SystemColors::HighlightText;
			dataGridViewCellStyle2->WrapMode = System::Windows::Forms::DataGridViewTriState::False;
			this->dataGridViewTitleKey->DefaultCellStyle = dataGridViewCellStyle2;
			this->dataGridViewTitleKey->Location = System::Drawing::Point(11, 20);
			this->dataGridViewTitleKey->Name = L"dataGridViewTitleKey";
			this->dataGridViewTitleKey->RowHeadersWidth = 20;
			this->dataGridViewTitleKey->RowTemplate->Height = 21;
			this->dataGridViewTitleKey->Size = System::Drawing::Size(291, 279);
			this->dataGridViewTitleKey->TabIndex = 0;
			// 
			// groupBoxTitleKey
			// 
			this->groupBoxTitleKey->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->groupBoxTitleKey->Controls->Add(this->dataGridViewTitleKey);
			this->groupBoxTitleKey->Location = System::Drawing::Point(12, 12);
			this->groupBoxTitleKey->Name = L"groupBoxTitleKey";
			this->groupBoxTitleKey->Size = System::Drawing::Size(314, 313);
			this->groupBoxTitleKey->TabIndex = 3;
			this->groupBoxTitleKey->TabStop = false;
			this->groupBoxTitleKey->Text = L"定型句の設定";
			// 
			// labelCode
			// 
			this->labelCode->AutoSize = true;
			this->labelCode->Location = System::Drawing::Point(6, 21);
			this->labelCode->Name = L"labelCode";
			this->labelCode->Size = System::Drawing::Size(62, 12);
			this->labelCode->TabIndex = 0;
			this->labelCode->Text = L"言語コード：";
			// 
			// groupBoxStyle
			// 
			this->groupBoxStyle->Controls->Add(this->textBoxXml);
			this->groupBoxStyle->Controls->Add(this->labelXml);
			this->groupBoxStyle->Controls->Add(this->textBoxRedirect);
			this->groupBoxStyle->Controls->Add(this->labelRedirect);
			this->groupBoxStyle->Enabled = false;
			this->groupBoxStyle->Location = System::Drawing::Point(8, 44);
			this->groupBoxStyle->Name = L"groupBoxStyle";
			this->groupBoxStyle->Size = System::Drawing::Size(274, 70);
			this->groupBoxStyle->TabIndex = 2;
			this->groupBoxStyle->TabStop = false;
			this->groupBoxStyle->Text = L"各言語での書式";
			// 
			// textBoxXml
			// 
			this->textBoxXml->Location = System::Drawing::Point(102, 18);
			this->textBoxXml->MaxLength = 100;
			this->textBoxXml->Name = L"textBoxXml";
			this->textBoxXml->Size = System::Drawing::Size(129, 19);
			this->textBoxXml->TabIndex = 1;
			// 
			// labelXml
			// 
			this->labelXml->AutoSize = true;
			this->labelXml->Location = System::Drawing::Point(6, 21);
			this->labelXml->Name = L"labelXml";
			this->labelXml->Size = System::Drawing::Size(90, 12);
			this->labelXml->TabIndex = 0;
			this->labelXml->Text = L"XMLデータのパス：";
			// 
			// textBoxRedirect
			// 
			this->textBoxRedirect->Location = System::Drawing::Point(102, 43);
			this->textBoxRedirect->MaxLength = 100;
			this->textBoxRedirect->Name = L"textBoxRedirect";
			this->textBoxRedirect->Size = System::Drawing::Size(129, 19);
			this->textBoxRedirect->TabIndex = 3;
			// 
			// labelRedirect
			// 
			this->labelRedirect->AutoSize = true;
			this->labelRedirect->Location = System::Drawing::Point(6, 46);
			this->labelRedirect->Name = L"labelRedirect";
			this->labelRedirect->Size = System::Drawing::Size(61, 12);
			this->labelRedirect->TabIndex = 2;
			this->labelRedirect->Text = L"リダイレクト：";
			// 
			// groupBoxName
			// 
			this->groupBoxName->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->groupBoxName->Controls->Add(this->dataGridViewName);
			this->groupBoxName->Enabled = false;
			this->groupBoxName->Location = System::Drawing::Point(8, 120);
			this->groupBoxName->Name = L"groupBoxName";
			this->groupBoxName->Size = System::Drawing::Size(274, 187);
			this->groupBoxName->TabIndex = 3;
			this->groupBoxName->TabStop = false;
			this->groupBoxName->Text = L"各言語での呼称";
			// 
			// dataGridViewName
			// 
			this->dataGridViewName->AllowUserToResizeRows = false;
			this->dataGridViewName->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->dataGridViewName->AutoSizeColumnsMode = System::Windows::Forms::DataGridViewAutoSizeColumnsMode::AllCells;
			this->dataGridViewName->AutoSizeRowsMode = System::Windows::Forms::DataGridViewAutoSizeRowsMode::AllCells;
			this->dataGridViewName->ColumnHeadersHeightSizeMode = System::Windows::Forms::DataGridViewColumnHeadersHeightSizeMode::AutoSize;
			this->dataGridViewName->Columns->AddRange(gcnew cli::array< System::Windows::Forms::DataGridViewColumn^  >(3) {this->Code, 
				this->ArticleName, this->ShortName});
			this->dataGridViewName->Location = System::Drawing::Point(6, 18);
			this->dataGridViewName->Name = L"dataGridViewName";
			this->dataGridViewName->RowHeadersWidth = 20;
			this->dataGridViewName->RowTemplate->Height = 21;
			this->dataGridViewName->Size = System::Drawing::Size(262, 161);
			this->dataGridViewName->TabIndex = 0;
			this->dataGridViewName->Leave += gcnew System::EventHandler(this, &ConfigWikipediaDialog::dataGridViewName_Leave);
			// 
			// Code
			// 
			this->Code->HeaderText = L"コード";
			this->Code->MaxInputLength = 10;
			this->Code->Name = L"Code";
			this->Code->Width = 57;
			// 
			// ArticleName
			// 
			this->ArticleName->HeaderText = L"記事名";
			this->ArticleName->MaxInputLength = 100;
			this->ArticleName->Name = L"ArticleName";
			this->ArticleName->Width = 66;
			// 
			// ShortName
			// 
			this->ShortName->HeaderText = L"略称";
			this->ShortName->MaxInputLength = 100;
			this->ShortName->Name = L"ShortName";
			this->ShortName->Width = 54;
			// 
			// comboBoxCode
			// 
			this->comboBoxCode->ContextMenuStrip = this->contextMenuStripCode;
			this->comboBoxCode->FormattingEnabled = true;
			this->comboBoxCode->ImeMode = System::Windows::Forms::ImeMode::Disable;
			this->comboBoxCode->Location = System::Drawing::Point(74, 18);
			this->comboBoxCode->MaxLength = 10;
			this->comboBoxCode->Name = L"comboBoxCode";
			this->comboBoxCode->Size = System::Drawing::Size(85, 20);
			this->comboBoxCode->TabIndex = 1;
			this->comboBoxCode->Leave += gcnew System::EventHandler(this, &ConfigWikipediaDialog::comboBoxCode_Leave);
			this->comboBoxCode->SelectedIndexChanged += gcnew System::EventHandler(this, &ConfigWikipediaDialog::comboBoxCode_SelectedIndexChanged);
			this->comboBoxCode->KeyDown += gcnew System::Windows::Forms::KeyEventHandler(this, &ConfigWikipediaDialog::comboBoxCode_KeyDown);
			// 
			// contextMenuStripCode
			// 
			this->contextMenuStripCode->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {this->toolStripMenuItemModify, 
				this->toolStripMenuItemDelete});
			this->contextMenuStripCode->Name = L"contextMenuStripCode";
			this->contextMenuStripCode->Size = System::Drawing::Size(173, 48);
			// 
			// toolStripMenuItemModify
			// 
			this->toolStripMenuItemModify->Name = L"toolStripMenuItemModify";
			this->toolStripMenuItemModify->Size = System::Drawing::Size(172, 22);
			this->toolStripMenuItemModify->Text = L"言語コードの変更(&M)";
			this->toolStripMenuItemModify->Click += gcnew System::EventHandler(this, &ConfigWikipediaDialog::toolStripMenuItemModify_Click);
			// 
			// toolStripMenuItemDelete
			// 
			this->toolStripMenuItemDelete->Name = L"toolStripMenuItemDelete";
			this->toolStripMenuItemDelete->Size = System::Drawing::Size(172, 22);
			this->toolStripMenuItemDelete->Text = L"言語の削除(&D)";
			this->toolStripMenuItemDelete->Click += gcnew System::EventHandler(this, &ConfigWikipediaDialog::toolStripMenuItemDelete_Click);
			// 
			// groupBoxLanguage
			// 
			this->groupBoxLanguage->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->groupBoxLanguage->Controls->Add(this->labelCode);
			this->groupBoxLanguage->Controls->Add(this->groupBoxName);
			this->groupBoxLanguage->Controls->Add(this->groupBoxStyle);
			this->groupBoxLanguage->Controls->Add(this->comboBoxCode);
			this->groupBoxLanguage->Location = System::Drawing::Point(332, 12);
			this->groupBoxLanguage->Name = L"groupBoxLanguage";
			this->groupBoxLanguage->Size = System::Drawing::Size(288, 313);
			this->groupBoxLanguage->TabIndex = 2;
			this->groupBoxLanguage->TabStop = false;
			this->groupBoxLanguage->Text = L"言語のプロパティ";
			// 
			// ConfigWikipediaDialog
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->buttonCancel;
			this->ClientSize = System::Drawing::Size(632, 366);
			this->Controls->Add(this->groupBoxTitleKey);
			this->Controls->Add(this->groupBoxLanguage);
			this->Controls->Add(this->buttonCancel);
			this->Controls->Add(this->buttonOK);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->MaximizeBox = false;
			this->MinimizeBox = false;
			this->MinimumSize = System::Drawing::Size(640, 400);
			this->Name = L"ConfigWikipediaDialog";
			this->ShowIcon = false;
			this->ShowInTaskbar = false;
			this->Text = L"言語の設定";
			this->Load += gcnew System::EventHandler(this, &ConfigWikipediaDialog::ConfigWikipediaDialog_Load);
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dataGridViewTitleKey))->EndInit();
			this->groupBoxTitleKey->ResumeLayout(false);
			this->groupBoxStyle->ResumeLayout(false);
			this->groupBoxStyle->PerformLayout();
			this->groupBoxName->ResumeLayout(false);
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dataGridViewName))->EndInit();
			this->contextMenuStripCode->ResumeLayout(false);
			this->groupBoxLanguage->ResumeLayout(false);
			this->groupBoxLanguage->PerformLayout();
			this->ResumeLayout(false);

		}
#pragma endregion
	private:
		// 共通関数クラスのオブジェクト
		MYAPP::Cmn ^cmnAP;
		// 各種設定
		Config ^config;
		// comboBoxColumnで選択していたアイテムのバックアップ
		String ^comboBoxCodeSelectedText;

	private: System::Void ConfigWikipediaDialog_Load(System::Object^  sender, System::EventArgs^  e);
	private: System::Void buttonOK_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void comboBoxCode_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e);
	private: System::Void comboBoxCode_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e);
	private: System::Void comboBoxCode_Leave(System::Object^  sender, System::EventArgs^  e);
	private: System::Void dataGridViewName_Leave(System::Object^  sender, System::EventArgs^  e);
	private: System::Void toolStripMenuItemModify_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void toolStripMenuItemDelete_Click(System::Object^  sender, System::EventArgs^  e);
};
}
