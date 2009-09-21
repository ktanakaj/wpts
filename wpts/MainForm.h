#include "stdafx.h"
#include "Config.h"
#include "TranslateWikipedia.h"

#pragma once


namespace wpts {

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace System::IO;

	/// <summary>
	/// MainForm �̊T�v
	///
	/// �x��: ���̃N���X�̖��O��ύX����ꍇ�A���̃N���X���ˑ����邷�ׂĂ� .resx �t�@�C���Ɋ֘A�t����ꂽ
	///          �}�l�[�W ���\�[�X �R���p�C�� �c�[���ɑ΂��� 'Resource File Name' �v���p�e�B��
	///          �ύX����K�v������܂��B���̕ύX���s��Ȃ��ƁA
	///          �f�U�C�i�ƁA���̃t�H�[���Ɋ֘A�t����ꂽ���[�J���C�Y�ς݃��\�[�X�Ƃ��A
	///          ���������݂ɗ��p�ł��Ȃ��Ȃ�܂��B
	/// </summary>
	public ref class MainForm : public System::Windows::Forms::Form
	{
	public:
		MainForm(void)
		{
			InitializeComponent();
			//
			//TODO: �����ɃR���X�g���N�^ �R�[�h��ǉ����܂�
			//
		}

	protected:
		/// <summary>
		/// �g�p���̃��\�[�X�����ׂăN���[���A�b�v���܂��B
		/// </summary>
		~MainForm()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::ComboBox^  comboBoxSource;
	private: System::Windows::Forms::ComboBox^  comboBoxTarget;
	private: System::Windows::Forms::Label^  labelSource;
	private: System::Windows::Forms::Label^  labelTarget;
	private: System::Windows::Forms::Label^  labelArrow;
	private: System::Windows::Forms::GroupBox^  groupBoxTransfer;
	private: System::Windows::Forms::TextBox^  textBoxSaveDirectory;
	private: System::Windows::Forms::Button^  buttonSaveDirectory;
	private: System::Windows::Forms::GroupBox^  groupBoxSaveDirectory;
	private: System::Windows::Forms::LinkLabel^  linkLabelSourceURL;
	private: System::Windows::Forms::FolderBrowserDialog^  folderBrowserDialogSaveDirectory;
	private: System::Windows::Forms::TextBox^  textBoxLog;
	private: System::Windows::Forms::Button^  buttonConfig;
	private: System::Windows::Forms::Label^  labelArticle;
	private: System::Windows::Forms::TextBox^  textBoxArticle;
	private: System::Windows::Forms::Button^  buttonRun;
	private: System::Windows::Forms::GroupBox^  groupBoxRun;
	private: System::Windows::Forms::Button^  buttonStop;
	private: System::ComponentModel::BackgroundWorker^  backgroundWorkerRun;
	private: System::Windows::Forms::ToolTip^  toolTip;
	private: System::ComponentModel::IContainer^  components;





	protected: 

	protected: 

	private:
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>


#pragma region Windows Form Designer generated code
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(MainForm::typeid));
			this->comboBoxSource = (gcnew System::Windows::Forms::ComboBox());
			this->comboBoxTarget = (gcnew System::Windows::Forms::ComboBox());
			this->labelSource = (gcnew System::Windows::Forms::Label());
			this->labelTarget = (gcnew System::Windows::Forms::Label());
			this->labelArrow = (gcnew System::Windows::Forms::Label());
			this->groupBoxTransfer = (gcnew System::Windows::Forms::GroupBox());
			this->buttonConfig = (gcnew System::Windows::Forms::Button());
			this->linkLabelSourceURL = (gcnew System::Windows::Forms::LinkLabel());
			this->textBoxSaveDirectory = (gcnew System::Windows::Forms::TextBox());
			this->buttonSaveDirectory = (gcnew System::Windows::Forms::Button());
			this->groupBoxSaveDirectory = (gcnew System::Windows::Forms::GroupBox());
			this->folderBrowserDialogSaveDirectory = (gcnew System::Windows::Forms::FolderBrowserDialog());
			this->textBoxLog = (gcnew System::Windows::Forms::TextBox());
			this->labelArticle = (gcnew System::Windows::Forms::Label());
			this->textBoxArticle = (gcnew System::Windows::Forms::TextBox());
			this->buttonRun = (gcnew System::Windows::Forms::Button());
			this->groupBoxRun = (gcnew System::Windows::Forms::GroupBox());
			this->buttonStop = (gcnew System::Windows::Forms::Button());
			this->backgroundWorkerRun = (gcnew System::ComponentModel::BackgroundWorker());
			this->toolTip = (gcnew System::Windows::Forms::ToolTip(this->components));
			this->groupBoxTransfer->SuspendLayout();
			this->groupBoxSaveDirectory->SuspendLayout();
			this->groupBoxRun->SuspendLayout();
			this->SuspendLayout();
			// 
			// comboBoxSource
			// 
			this->comboBoxSource->FormattingEnabled = true;
			this->comboBoxSource->ImeMode = System::Windows::Forms::ImeMode::Disable;
			this->comboBoxSource->Location = System::Drawing::Point(11, 21);
			this->comboBoxSource->MaxLength = 10;
			this->comboBoxSource->Name = L"comboBoxSource";
			this->comboBoxSource->Size = System::Drawing::Size(57, 20);
			this->comboBoxSource->Sorted = true;
			this->comboBoxSource->TabIndex = 0;
			this->comboBoxSource->Leave += gcnew System::EventHandler(this, &MainForm::comboBoxSource_Leave);
			this->comboBoxSource->SelectedIndexChanged += gcnew System::EventHandler(this, &MainForm::comboBoxSource_SelectedIndexChanged);
			// 
			// comboBoxTarget
			// 
			this->comboBoxTarget->FormattingEnabled = true;
			this->comboBoxTarget->ImeMode = System::Windows::Forms::ImeMode::Disable;
			this->comboBoxTarget->Location = System::Drawing::Point(11, 64);
			this->comboBoxTarget->MaxLength = 10;
			this->comboBoxTarget->Name = L"comboBoxTarget";
			this->comboBoxTarget->Size = System::Drawing::Size(57, 20);
			this->comboBoxTarget->Sorted = true;
			this->comboBoxTarget->TabIndex = 2;
			this->comboBoxTarget->Leave += gcnew System::EventHandler(this, &MainForm::comboBoxTarget_Leave);
			this->comboBoxTarget->SelectedIndexChanged += gcnew System::EventHandler(this, &MainForm::comboBoxTarget_SelectedIndexChanged);
			// 
			// labelSource
			// 
			this->labelSource->BackColor = System::Drawing::SystemColors::Control;
			this->labelSource->BorderStyle = System::Windows::Forms::BorderStyle::Fixed3D;
			this->labelSource->Location = System::Drawing::Point(74, 21);
			this->labelSource->Name = L"labelSource";
			this->labelSource->Size = System::Drawing::Size(101, 20);
			this->labelSource->TabIndex = 1;
			this->labelSource->TextAlign = System::Drawing::ContentAlignment::MiddleLeft;
			// 
			// labelTarget
			// 
			this->labelTarget->BackColor = System::Drawing::SystemColors::Control;
			this->labelTarget->BorderStyle = System::Windows::Forms::BorderStyle::Fixed3D;
			this->labelTarget->Location = System::Drawing::Point(74, 64);
			this->labelTarget->Name = L"labelTarget";
			this->labelTarget->Size = System::Drawing::Size(101, 20);
			this->labelTarget->TabIndex = 3;
			this->labelTarget->TextAlign = System::Drawing::ContentAlignment::MiddleLeft;
			// 
			// labelArrow
			// 
			this->labelArrow->AutoSize = true;
			this->labelArrow->Font = (gcnew System::Drawing::Font(L"MS UI Gothic", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(128)));
			this->labelArrow->Location = System::Drawing::Point(75, 44);
			this->labelArrow->Name = L"labelArrow";
			this->labelArrow->Size = System::Drawing::Size(25, 16);
			this->labelArrow->TabIndex = 6;
			this->labelArrow->Text = L"��";
			// 
			// groupBoxTransfer
			// 
			this->groupBoxTransfer->Controls->Add(this->buttonConfig);
			this->groupBoxTransfer->Controls->Add(this->linkLabelSourceURL);
			this->groupBoxTransfer->Controls->Add(this->labelTarget);
			this->groupBoxTransfer->Controls->Add(this->labelArrow);
			this->groupBoxTransfer->Controls->Add(this->comboBoxSource);
			this->groupBoxTransfer->Controls->Add(this->comboBoxTarget);
			this->groupBoxTransfer->Controls->Add(this->labelSource);
			this->groupBoxTransfer->Location = System::Drawing::Point(12, 12);
			this->groupBoxTransfer->Name = L"groupBoxTransfer";
			this->groupBoxTransfer->Size = System::Drawing::Size(395, 96);
			this->groupBoxTransfer->TabIndex = 0;
			this->groupBoxTransfer->TabStop = false;
			this->groupBoxTransfer->Text = L"�|�󌳁���̌����ݒ�";
			// 
			// buttonConfig
			// 
			this->buttonConfig->Location = System::Drawing::Point(191, 61);
			this->buttonConfig->Name = L"buttonConfig";
			this->buttonConfig->Size = System::Drawing::Size(43, 23);
			this->buttonConfig->TabIndex = 5;
			this->buttonConfig->Text = L"�ݒ�";
			this->buttonConfig->UseVisualStyleBackColor = true;
			this->buttonConfig->Click += gcnew System::EventHandler(this, &MainForm::buttonConfig_Click);
			// 
			// linkLabelSourceURL
			// 
			this->linkLabelSourceURL->BackColor = System::Drawing::SystemColors::Control;
			this->linkLabelSourceURL->BorderStyle = System::Windows::Forms::BorderStyle::Fixed3D;
			this->linkLabelSourceURL->Location = System::Drawing::Point(191, 21);
			this->linkLabelSourceURL->Name = L"linkLabelSourceURL";
			this->linkLabelSourceURL->Size = System::Drawing::Size(191, 20);
			this->linkLabelSourceURL->TabIndex = 4;
			this->linkLabelSourceURL->TabStop = true;
			this->linkLabelSourceURL->Text = L"http://";
			this->linkLabelSourceURL->TextAlign = System::Drawing::ContentAlignment::MiddleLeft;
			this->linkLabelSourceURL->LinkClicked += gcnew System::Windows::Forms::LinkLabelLinkClickedEventHandler(this, &MainForm::linkLabelSourceURL_LinkClicked);
			// 
			// textBoxSaveDirectory
			// 
			this->textBoxSaveDirectory->Location = System::Drawing::Point(60, 18);
			this->textBoxSaveDirectory->Name = L"textBoxSaveDirectory";
			this->textBoxSaveDirectory->Size = System::Drawing::Size(256, 19);
			this->textBoxSaveDirectory->TabIndex = 1;
			this->textBoxSaveDirectory->Leave += gcnew System::EventHandler(this, &MainForm::textBoxSaveDirectory_Leave);
			// 
			// buttonSaveDirectory
			// 
			this->buttonSaveDirectory->Location = System::Drawing::Point(11, 16);
			this->buttonSaveDirectory->Name = L"buttonSaveDirectory";
			this->buttonSaveDirectory->Size = System::Drawing::Size(43, 23);
			this->buttonSaveDirectory->TabIndex = 0;
			this->buttonSaveDirectory->Text = L"�Q��";
			this->buttonSaveDirectory->UseVisualStyleBackColor = true;
			this->buttonSaveDirectory->Click += gcnew System::EventHandler(this, &MainForm::buttonSaveDirectory_Click);
			// 
			// groupBoxSaveDirectory
			// 
			this->groupBoxSaveDirectory->Controls->Add(this->textBoxSaveDirectory);
			this->groupBoxSaveDirectory->Controls->Add(this->buttonSaveDirectory);
			this->groupBoxSaveDirectory->Location = System::Drawing::Point(12, 114);
			this->groupBoxSaveDirectory->Name = L"groupBoxSaveDirectory";
			this->groupBoxSaveDirectory->Size = System::Drawing::Size(329, 49);
			this->groupBoxSaveDirectory->TabIndex = 1;
			this->groupBoxSaveDirectory->TabStop = false;
			this->groupBoxSaveDirectory->Text = L"�������ʂ��o�͂���t�H���_�̑I��";
			// 
			// folderBrowserDialogSaveDirectory
			// 
			this->folderBrowserDialogSaveDirectory->Description = L"�������ʂ��o�͂���t�H���_��I�����Ă��������B";
			// 
			// textBoxLog
			// 
			this->textBoxLog->AcceptsReturn = true;
			this->textBoxLog->AcceptsTab = true;
			this->textBoxLog->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->textBoxLog->BackColor = System::Drawing::SystemColors::Window;
			this->textBoxLog->Location = System::Drawing::Point(12, 56);
			this->textBoxLog->MaxLength = 0;
			this->textBoxLog->Multiline = true;
			this->textBoxLog->Name = L"textBoxLog";
			this->textBoxLog->ReadOnly = true;
			this->textBoxLog->ScrollBars = System::Windows::Forms::ScrollBars::Vertical;
			this->textBoxLog->Size = System::Drawing::Size(424, 197);
			this->textBoxLog->TabIndex = 4;
			// 
			// labelArticle
			// 
			this->labelArticle->AutoSize = true;
			this->labelArticle->Location = System::Drawing::Point(15, 23);
			this->labelArticle->Name = L"labelArticle";
			this->labelArticle->Size = System::Drawing::Size(41, 12);
			this->labelArticle->TabIndex = 0;
			this->labelArticle->Text = L"�L����";
			// 
			// textBoxArticle
			// 
			this->textBoxArticle->Location = System::Drawing::Point(62, 20);
			this->textBoxArticle->MaxLength = 1000;
			this->textBoxArticle->Name = L"textBoxArticle";
			this->textBoxArticle->Size = System::Drawing::Size(108, 19);
			this->textBoxArticle->TabIndex = 1;
			// 
			// buttonRun
			// 
			this->buttonRun->Location = System::Drawing::Point(183, 18);
			this->buttonRun->Name = L"buttonRun";
			this->buttonRun->Size = System::Drawing::Size(43, 23);
			this->buttonRun->TabIndex = 2;
			this->buttonRun->Text = L"���s";
			this->buttonRun->UseVisualStyleBackColor = true;
			this->buttonRun->Click += gcnew System::EventHandler(this, &MainForm::buttonRun_Click);
			// 
			// groupBoxRun
			// 
			this->groupBoxRun->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->groupBoxRun->Controls->Add(this->buttonStop);
			this->groupBoxRun->Controls->Add(this->buttonRun);
			this->groupBoxRun->Controls->Add(this->textBoxLog);
			this->groupBoxRun->Controls->Add(this->labelArticle);
			this->groupBoxRun->Controls->Add(this->textBoxArticle);
			this->groupBoxRun->Location = System::Drawing::Point(12, 169);
			this->groupBoxRun->Name = L"groupBoxRun";
			this->groupBoxRun->Size = System::Drawing::Size(448, 265);
			this->groupBoxRun->TabIndex = 2;
			this->groupBoxRun->TabStop = false;
			this->groupBoxRun->Text = L"�|�󂷂�L�����w�肵�āA���s";
			// 
			// buttonStop
			// 
			this->buttonStop->Enabled = false;
			this->buttonStop->Location = System::Drawing::Point(241, 18);
			this->buttonStop->Name = L"buttonStop";
			this->buttonStop->Size = System::Drawing::Size(43, 23);
			this->buttonStop->TabIndex = 3;
			this->buttonStop->Text = L"���~";
			this->buttonStop->UseVisualStyleBackColor = true;
			this->buttonStop->Click += gcnew System::EventHandler(this, &MainForm::buttonStop_Click);
			// 
			// backgroundWorkerRun
			// 
			this->backgroundWorkerRun->WorkerSupportsCancellation = true;
			this->backgroundWorkerRun->DoWork += gcnew System::ComponentModel::DoWorkEventHandler(this, &MainForm::backgroundWorkerRun_DoWork);
			this->backgroundWorkerRun->RunWorkerCompleted += gcnew System::ComponentModel::RunWorkerCompletedEventHandler(this, &MainForm::backgroundWorkerRun_RunWorkerCompleted);
			// 
			// toolTip
			// 
			this->toolTip->AutoPopDelay = 10000;
			this->toolTip->InitialDelay = 500;
			this->toolTip->ReshowDelay = 100;
			// 
			// MainForm
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(472, 446);
			this->Controls->Add(this->groupBoxRun);
			this->Controls->Add(this->groupBoxSaveDirectory);
			this->Controls->Add(this->groupBoxTransfer);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->MinimumSize = System::Drawing::Size(480, 480);
			this->Name = L"MainForm";
			this->Text = L"Wikipedia �|��x���c�[��";
			this->FormClosed += gcnew System::Windows::Forms::FormClosedEventHandler(this, &MainForm::MainForm_FormClosed);
			this->Load += gcnew System::EventHandler(this, &MainForm::MainForm_Load);
			this->groupBoxTransfer->ResumeLayout(false);
			this->groupBoxTransfer->PerformLayout();
			this->groupBoxSaveDirectory->ResumeLayout(false);
			this->groupBoxSaveDirectory->PerformLayout();
			this->groupBoxRun->ResumeLayout(false);
			this->groupBoxRun->PerformLayout();
			this->ResumeLayout(false);

		}
#pragma endregion
	private:
		// ��ʏ���������
		void initialize(void);
		// ��ʂ����b�N���Ɉڍs
		void lock(void);
		// ��ʂ����b�N��������
		void release(void);
		// �n���ꂽ�����񂩂�.txt��.log�̏d�����Ă��Ȃ��t�@�C�������쐬
		bool makeFileName(String^ %, String^ %, String^, String^);
		// �|��x�������N���X�̃C�x���g�p
		void getLogUpdate(System::Object^, System::EventArgs^);

		// ���ʊ֐��N���X�̃I�u�W�F�N�g
		MYAPP::Cmn ^cmnAP;
		// �e��ݒ�N���X�̃I�u�W�F�N�g
		Config ^config;
		// �����x�������N���X�̃I�u�W�F�N�g
		Translate ^transAP;
		// �\���ς݃��O������
		int logLastLength;

	private: System::Void MainForm_Load(System::Object^  sender, System::EventArgs^  e);
	private: System::Void MainForm_FormClosed(System::Object^  sender, System::Windows::Forms::FormClosedEventArgs^  e);
	private: System::Void comboBoxSource_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e);
	private: System::Void comboBoxTarget_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e);
	private: System::Void comboBoxSource_Leave(System::Object^  sender, System::EventArgs^  e);
	private: System::Void comboBoxTarget_Leave(System::Object^  sender, System::EventArgs^  e);
	private: System::Void textBoxSaveDirectory_Leave(System::Object^  sender, System::EventArgs^  e);
	private: System::Void linkLabelSourceURL_LinkClicked(System::Object^  sender, System::Windows::Forms::LinkLabelLinkClickedEventArgs^  e);
	private: System::Void buttonConfig_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void buttonSaveDirectory_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void buttonRun_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void buttonStop_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void backgroundWorkerRun_DoWork(System::Object^  sender, System::ComponentModel::DoWorkEventArgs^  e);
	private: System::Void backgroundWorkerRun_RunWorkerCompleted(System::Object^  sender, System::ComponentModel::RunWorkerCompletedEventArgs^  e);
};
}

