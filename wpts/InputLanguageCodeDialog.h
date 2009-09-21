#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace wpts {

	/// <summary>
	/// InputLanguageCodeDialog �̊T�v
	///
	/// �x��: ���̃N���X�̖��O��ύX����ꍇ�A���̃N���X���ˑ����邷�ׂĂ� .resx �t�@�C���Ɋ֘A�t����ꂽ
	///          �}�l�[�W ���\�[�X �R���p�C�� �c�[���ɑ΂��� 'Resource File Name' �v���p�e�B��
	///          �ύX����K�v������܂��B���̕ύX���s��Ȃ��ƁA
	///          �f�U�C�i�ƁA���̃t�H�[���Ɋ֘A�t����ꂽ���[�J���C�Y�ς݃��\�[�X�Ƃ��A
	///          ���������݂ɗ��p�ł��Ȃ��Ȃ�܂��B
	/// </summary>
	public ref class InputLanguageCodeDialog : public System::Windows::Forms::Form
	{
	public:
		InputLanguageCodeDialog(void)
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
		~InputLanguageCodeDialog()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::TextBox^  textBoxCode;
	private: System::Windows::Forms::Button^  buttonOK;
	private: System::Windows::Forms::Button^  buttonCancel;
	protected: 

	private:
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		void InitializeComponent(void)
		{
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(InputLanguageCodeDialog::typeid));
			this->textBoxCode = (gcnew System::Windows::Forms::TextBox());
			this->buttonOK = (gcnew System::Windows::Forms::Button());
			this->buttonCancel = (gcnew System::Windows::Forms::Button());
			this->SuspendLayout();
			// 
			// textBoxCode
			// 
			this->textBoxCode->CharacterCasing = System::Windows::Forms::CharacterCasing::Lower;
			this->textBoxCode->ImeMode = System::Windows::Forms::ImeMode::Disable;
			this->textBoxCode->Location = System::Drawing::Point(26, 12);
			this->textBoxCode->MaxLength = 10;
			this->textBoxCode->Name = L"textBoxCode";
			this->textBoxCode->Size = System::Drawing::Size(100, 19);
			this->textBoxCode->TabIndex = 0;
			// 
			// buttonOK
			// 
			this->buttonOK->DialogResult = System::Windows::Forms::DialogResult::OK;
			this->buttonOK->Location = System::Drawing::Point(10, 41);
			this->buttonOK->Name = L"buttonOK";
			this->buttonOK->Size = System::Drawing::Size(64, 23);
			this->buttonOK->TabIndex = 2;
			this->buttonOK->Text = L"OK";
			this->buttonOK->UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this->buttonCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->buttonCancel->Location = System::Drawing::Point(80, 41);
			this->buttonCancel->Name = L"buttonCancel";
			this->buttonCancel->Size = System::Drawing::Size(64, 23);
			this->buttonCancel->TabIndex = 1;
			this->buttonCancel->Text = L"�L�����Z��";
			this->buttonCancel->UseVisualStyleBackColor = true;
			// 
			// InputLanguageCodeDialog
			// 
			this->AcceptButton = this->buttonOK;
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->buttonCancel;
			this->ClientSize = System::Drawing::Size(154, 72);
			this->Controls->Add(this->buttonCancel);
			this->Controls->Add(this->buttonOK);
			this->Controls->Add(this->textBoxCode);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->MaximizeBox = false;
			this->MinimizeBox = false;
			this->Name = L"InputLanguageCodeDialog";
			this->ShowIcon = false;
			this->ShowInTaskbar = false;
			this->Text = L"����R�[�h�����";
			this->FormClosed += gcnew System::Windows::Forms::FormClosedEventHandler(this, &InputLanguageCodeDialog::InputLanguageCodeDialog_FormClosed);
			this->Load += gcnew System::EventHandler(this, &InputLanguageCodeDialog::InputLanguageCodeDialog_Load);
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion
	public:
		// ����R�[�h�i�f�[�^�����p�j
		String ^LanguageCode;

	private: System::Void InputLanguageCodeDialog_Load(System::Object^  sender, System::EventArgs^  e) {
				 // �e�L�X�g�{�b�N�X�Ɍ���R�[�h��ݒ�
				 if(LanguageCode != nullptr){
					 textBoxCode->Text = LanguageCode;
				 }
			 }
	private: System::Void InputLanguageCodeDialog_FormClosed(System::Object^  sender, System::Windows::Forms::FormClosedEventArgs^  e) {
				 // �e�L�X�g�{�b�N�X�̌���R�[�h��ۑ�
				 LanguageCode = textBoxCode->Text->Trim();
			 }
	};
}
