// ��ʁE�@�\�ɂ��Ȃ��A���ʓI�Ȋ֐��N���X�i2006�N9��16���Łj
#ifndef MYCmnH
#define MYCmnH

namespace MYAPP {

	using namespace System;
	using namespace System::Resources;
	using namespace System::Reflection;
	using namespace System::Windows::Forms;
	using namespace System::IO;
	using namespace System::Net;
	using namespace System::Xml::Serialization;

	public ref class Cmn {
	public:
		// �R���X�g���N�^
		// ��exe�Ɠ����̃��\�[�X�}�l�[�W���[���N���t�H���_����ǂݍ���
		//   �w�肳�ꂽ���\�[�X�}�l�[�W���[���w�肳�ꂽ�t�H���_����ݒ�
		//   �n���ꂽ���\�[�X�}�l�[�W���[���g�p�A��3���
		Cmn(void);
		Cmn(String^, String^);
		Cmn(ResourceManager^);
		// �f�X�g���N�^
		virtual ~Cmn();

		// ���v���\�[�X�}�l�[�W���[

		// ���ʃ_�C�A���O�i�ʒm�^�x���^�G���[�j
		// �����ʃ_�C�A���O�́A���͂��ꂽ����������̂܂ܕ\��������̂ƁA���͂��ꂽ������
		//   �Ń��\�[�X���擾�A�t�H�[�}�b�g���ĕ\��������̂̊e2���
		virtual void InformationDialog(String^);
		virtual void InformationDialogResource(String^, ... array<Object^>^);
		virtual void WarningDialog(String^);
		virtual void WarningDialogResource(String^, ... array<Object^>^);
		virtual void ErrorDialog(String^);
		virtual void ErrorDialogResource(String^, ... array<Object^>^);

		// �����\�[�X�}�l�[�W���[��nullptr�ł�����A���̏ꍇ�̓_�C�A���O���o�Ȃ�

		// �t�H���_�^�t�@�C���I�[�v��
		virtual bool OpenFolder(String^, bool);
		bool OpenFolder(String^);
		virtual bool OpenFile(String^, bool);
		bool OpenFile(String^);

		// �T�[�o�[�ڑ��`�F�b�N
		virtual bool Ping(String^, bool);
		bool Ping(String^);

		// DataGridView��CSV�t�@�C���ւ̏o��
		virtual bool SaveDataGridViewCsv(DataGridView^, String^, bool);
		bool SaveDataGridViewCsv(DataGridView^, String^);

		// ���ȉ��͐ÓI�����o�֐�

		// NULL�l�`�F�b�N��Trim
		static String^ NullCheckAndTrim(String^);
		static String^ NullCheckAndTrim(DataGridViewCell^);

		// �z��ւ̗v�f�ǉ�
		generic<typename T> static int AddArray(array<T>^ %);
		generic<typename T> static int AddArray(array<T>^ %, T);

		// �\�t�g��+�o�[�W�������̕�����擾�i�A�Z���u������擾�j
		static String^ GetProductName(void);

		// �����񒆂̃t�@�C�����Ɏg�p�ł��Ȃ�������u��
		static String^ ReplaceInvalidFileNameChars(String^);

		// �I�u�W�F�N�g��XML�ւ̃V���A���C�Y�A�f�V���A���C�Y
		// ���f�V���A���C�Y�́A�I�u�W�F�N�g�^�Ō��ʂ��󂯎��A���̌�ɌĂяo�����ŃL���X�g���邱��
		static bool XmlSerialize(Object^, String^);
		static bool XmlDeserialize(Object^ %, Type^, String^);

		// �Ώۂ̕����񂪁A�n���ꂽ������̃C���f�b�N�X�Ԗڂɑ��݂��邩���`�F�b�N
		static bool ChkTextInnerWith(String^, int, String^);

		// �R���{�{�b�N�X���m�F���A���݂̒l���ꗗ�ɖ�����Γo�^
		static bool AddComboBoxNewItem(ComboBox^ %, String^);
		static bool AddComboBoxNewItem(ComboBox^ %);
		// �R���{�{�b�N�X���m�F���A�I������Ă���l���폜
		static bool RemoveComboBoxItem(ComboBox^ %);

		// ���\�[�X�}�l�[�W���[
		ResourceManager ^Resource;
	};

}

#endif
