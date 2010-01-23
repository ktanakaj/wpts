// ================================================================================================
// <summary>
//      ��ʁE�@�\�ɂ��Ȃ��A���ʓI�Ȋ֐��N���X�\�[�X</summary>
//
// <copyright file="Cmn.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Resources;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// ��ʁE�@�\�ɂ��Ȃ��A���ʓI�Ȋ֐��̃N���X�ł��B
    /// �� �����\��
    /// </summary>
    public class Cmn
    {
        /// <summary>
        /// ���\�[�X�}�l�[�W���[
        /// </summary>
        public ResourceManager Resource;

        /// <summary>
        /// �R���X�g���N�^�iexe�Ɠ����̃��\�[�X�}�l�[�W���[���N���t�H���_����ǂݍ��݁j�B
        /// </summary>
        public Cmn()
        {
            // �R���X�g���N�^�ł��邽�߁A��O�͓����Ȃ��B�ُ펞��NULL�ŏ�����
            try
            {
                // �t�@�C������ݒ��ǂݍ���
                this.Resource = ResourceManager.CreateFileBasedResourceManager(
                    Path.GetFileNameWithoutExtension(Application.ExecutablePath),
                    Application.StartupPath,
                    null);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Cmn.Cmn > ��O�����F" + e.ToString());
                this.Resource = null;
            }
        }

        /// <summary>
        /// �R���X�g���N�^�i�w�肳�ꂽ���\�[�X�}�l�[�W���[���w�肳�ꂽ�t�H���_����ݒ�j�B
        /// </summary>
        /// <param name="i_Resource">���\�[�X�}�l�[�W���[�B</param>
        /// <param name="i_Dir">���\�[�X�̂���t�H���_�B</param>
        public Cmn(string i_Resource, string i_Dir)
        {
            System.Diagnostics.Debug.WriteLine("Cmn.Cmn > " + i_Resource + ", " + i_Dir);

            // �R���X�g���N�^�ł��邽�߁A��O�͓����Ȃ��B�ُ펞��NULL�ŏ�����
            try
            {
                // �t�@�C������ݒ��ǂݍ���
                Resource = ResourceManager.CreateFileBasedResourceManager(i_Resource, i_Dir, null);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Cmn.Cmn > ��O�����F" + e.ToString());
                this.Resource = null;
            }
        }

        /// <summary>
        /// �R���X�g���N�^�i�n���ꂽ���\�[�X�}�l�[�W���[���g�p�j�B
        /// </summary>
        /// <param name="resource">���\�[�X�}�l�[�W���[�B</param>
        public Cmn(ResourceManager resource)
        {
            // �n���ꂽ���\�[�X�}�l�[�W���[�����̂܂܎g�p
            this.Resource = resource;
        }

        // ���ȉ��͐ÓI�����o

        /// <summary>
        /// String��NULL�l�`�F�b�N��Trim�B
        /// </summary>
        /// <param name="s">�Ώە�����B</param>
        /// <returns>�`�F�b�N��̕�����</returns>
        public static string NullCheckAndTrim(string s)
        {
            // ����O�Ȃ�
            if (s == null)
            {
                return String.Empty;
            }

            return s.Trim();
        }

        /// <summary>
        /// DataGridViewCell��NULL�l�`�F�b�N��Trim�B
        /// </summary>
        /// <param name="c">�ΏۃZ���B</param>
        /// <returns>�`�F�b�N��̕�����</returns>
        public static string NullCheckAndTrim(DataGridViewCell c)
        {
            // ����O�Ȃ�
            if (c == null)
            {
                return String.Empty;
            }
            else if (c.Value == null)
            {
                return String.Empty;
            }

            return c.Value.ToString().Trim();
        }

        /// <summary>
        /// �z��ւ̗v�f�iNULL�j�ǉ��B
        /// </summary>
        /// <typeparam name="T">�v�f�̌^</typeparam>
        /// <param name="io_Array">�z��B</param>
        /// <returns>�ǉ������C���f�b�N�X</returns>
        public static int AddArray<T>(ref T[] io_Array)
        {
            // ��Resize���ŗ�O�����������ꍇ�́A���̂܂ܓ�����
            if (io_Array == null)
            {
                io_Array = new T[1];
                return 0;
            }

            Array.Resize<T>(ref io_Array, io_Array.Length + 1);
            return io_Array.Length - 1;
        }

        /// <summary>
        /// �z��ւ̗v�f�i���͒l�j�ǉ��B
        /// </summary>
        /// <typeparam name="T">�v�f�̌^</typeparam>
        /// <param name="io_Array">�z��B</param>
        /// <param name="i_Obj">�ǉ�����I�u�W�F�N�g�B</param>
        /// <returns>�ǉ������C���f�b�N�X</returns>
        public static int AddArray<T>(ref T[] io_Array, T i_Obj)
        {
            // ��Resize�����ɗ�O�����������ꍇ�́A���̂܂ܓ�����
            int index = AddArray(ref io_Array);
            io_Array[index] = i_Obj;
            return index;
        }

        /// <summary>
        /// �\�t�g��+�o�[�W�������̕�����擾�i�A�Z���u������擾�j�B
        /// </summary>
        /// <returns>�\�t�g���</returns>
        public static string GetProductName()
        {
            // ����O�Ȃ��B������������ꍇ�͂��̂܂ܕԂ�

            // �A�Z���u�����琻�i�����擾���A�o�[�W�������(x.xx�`��)��t���ĕԂ�
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyProductAttribute product =
                (AssemblyProductAttribute)Attribute.GetCustomAttribute(
                    assembly,
                    typeof(AssemblyProductAttribute));
            Version ver = assembly.GetName().Version;

            // �߂�l��Ԃ��A�r���h�ԍ��E���r�W�����͖���
            return product.Product + " Ver" + ver.Major + "." + String.Format("{0:D2}", ver.Minor);
        }

        /// <summary>
        /// �����񒆂̃t�@�C�����Ɏg�p�ł��Ȃ�������u���B
        /// </summary>
        /// <param name="s">�t�@�C�����B</param>
        /// <returns>�u����̕�����</returns>
        public static string ReplaceInvalidFileNameChars(string s)
        {
            // �n���ꂽ������Ƀt�@�C�����Ɏg���Ȃ��������܂܂�Ă���ꍇ�A_ �ɒu��������
            string result = s;
            char[] unuseChars = Path.GetInvalidFileNameChars();
            foreach (char c in unuseChars)
            {
                result = result.Replace(c, '_');
            }

            return result;
        }

        /// <summary>
        /// �I�u�W�F�N�g��XML�ւ̃V���A���C�Y�B
        /// </summary>
        /// <param name="i_Obj">�I�u�W�F�N�g�B</param>
        /// <param name="i_FileName">XML�t�@�C���p�X�B</param>
        /// <returns><c>true</c> �V���A���C�Y����</returns>
        public static bool XmlSerialize(object i_Obj, string i_FileName)
        {
            // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false

            // �ݒ���V���A���C�Y��
            System.Diagnostics.Debug.WriteLine("Cmn.Serialize > " + i_FileName + "�ɃV���A���C�Y");

            XmlSerializer serializer = new XmlSerializer(i_Obj.GetType());
            try
            {
                Stream writer = new FileStream(i_FileName, FileMode.Create);
                try
                {
                    serializer.Serialize(writer, i_Obj);
                }
                finally
                {
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Cmn.Serialize > ��O�����F" + e.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// �I�u�W�F�N�g��XML����̃f�V���A���C�Y�B
        /// </summary>
        /// <param name="o_Obj">�I�u�W�F�N�g�B</param>
        /// <param name="i_Type">�I�u�W�F�N�g�̌^�B</param>
        /// <param name="i_FileName">XML�t�@�C���p�X�B</param>
        /// <returns><c>true</c> �f�V���A���C�Y����</returns>
        public static bool XmlDeserialize(ref object o_Obj, Type i_Type, string i_FileName)
        {
            // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false

            // �ݒ���f�V���A���C�Y��
            System.Diagnostics.Debug.WriteLine("Cmn.Deserialize > " + i_FileName + "����f�V���A���C�Y");

            // �o�͒l������
            o_Obj = null;

            // ������gcnew�Ń��O��ɗ�O���o�邪�AMSDN�ɂ��Ζ������Ă����炵���E�E�E
            XmlSerializer serializer = new XmlSerializer(i_Type);
            try
            {
                Stream reader = new FileStream(i_FileName, FileMode.Open, FileAccess.Read);
                try
                {
                    // �� �ق�Ƃ�Object�̌^�ɃL���X�g���đ���������̂����A
                    //    ���@�s���Ȃ��߁A����͌Ăяo�����ōs��
                    o_Obj = serializer.Deserialize(reader);
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Cmn.Deserialize > ��O�����F" + e.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// �Ώۂ̕����񂪁A�n���ꂽ������̃C���f�b�N�X�Ԗڂɑ��݂��邩���`�F�b�N�B
        /// </summary>
        /// <param name="i_Text">������S�́B</param>
        /// <param name="i_Index">�`�F�b�N�ʒu�B</param>
        /// <param name="i_ChkStr">�T�����镶����B</param>
        /// <returns><c>true</c> ���݂���</returns>
        public static bool ChkTextInnerWith(string i_Text, int i_Index, string i_ChkStr)
        {
            // ��i_Text, i_ChkStr��NULL�̏ꍇ�͂��̂܂�NullReferenceException���B
            //   i_Index��i_Text�͈̔͊O�̂Ƃ���ArgumentException��Ԃ�

            // ���͒l�`�F�b�N
            if ((i_Index < 0) || (i_Index >= i_Text.Length))
            {
                throw new ArgumentException("IndexOutOfRange! : " + i_Index, "i_Index");
            }

            if (i_ChkStr == String.Empty)
            {
                return true;
            }

            if (i_Text == String.Empty)
            {
                return false;
            }

            // ������̈�v���`�F�b�N
            // ��1�ڂ�if�͑��x�I�ɂ������̕����������ȂƎv���Ă���Ă���B�v��Ȃ������E�E�E
            if (i_Text[i_Index] == i_ChkStr[0])
            {
                if ((i_Index + i_ChkStr.Length) <= i_Text.Length)
                {
                    if (i_Text.Substring(i_Index, i_ChkStr.Length) == i_ChkStr)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// �R���{�{�b�N�X���m�F���A���݂̒l���ꗗ�ɖ�����Γo�^�B
        /// </summary>
        /// <param name="io_Box">�R���{�{�b�N�X�B</param>
        /// <param name="i_FirstStr">�v���t�B�b�N�X�B</param>
        /// <returns><c>true</c> �o�^����</returns>
        public static bool AddComboBoxNewItem(ref ComboBox io_Box, string i_FirstStr)
        {
            // ��io_Box��NULL�̏ꍇ�Ȃǂ́ANullReferenceException�������̂܂ܕԂ�

            // System.Diagnostics.Debug.WriteLine("Cmn.AddComboBoxNewItem > " + io_Box.Text + ", " + i_FirstStr);
            if (io_Box.Text != String.Empty)
            {
                // ���݂̒l��i_FirstStr����n�܂��Ă��Ȃ��ꍇ�́Ai_FirstStr��t���ď���
                string text = io_Box.Text;
                if (i_FirstStr != null && i_FirstStr != String.Empty)
                {
                    if (text.StartsWith(i_FirstStr) == false)
                    {
                        text = i_FirstStr + io_Box.Text;
                    }
                }

                // ���݂̒l�Ɉ�v������̂����邩���m�F
                if (io_Box.Items.Contains(text) == false)
                {
                    // ���݂��Ȃ��ꍇ�A���݂̒l���ꗗ�ɒǉ�
                    io_Box.Items.Add(text);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// �R���{�{�b�N�X���m�F���A���݂̒l���ꗗ�ɖ�����Γo�^�B
        /// </summary>
        /// <param name="io_Box">�R���{�{�b�N�X�B</param>
        /// <returns><c>true</c> �o�^����</returns>
        public static bool AddComboBoxNewItem(ref ComboBox io_Box)
        {
            // �ǉ������񖳂��ŁA������̊֐����R�[��
            return AddComboBoxNewItem(ref io_Box, String.Empty);
        }

        /// <summary>
        /// �R���{�{�b�N�X���m�F���A�I������Ă���l���폜�B
        /// </summary>
        /// <param name="io_Box">�R���{�{�b�N�X�B</param>
        /// <returns><c>true</c> �폜����</returns>
        public static bool RemoveComboBoxItem(ref ComboBox io_Box)
        {
            // ��io_Box��NULL�̏ꍇ�Ȃǂ́ANullReferenceException�������̂܂ܕԂ�

            // System.Diagnostics.Debug.WriteLine("Cmn.RemoveComboBoxItem > " + io_Box.SelectedIndex.ToString());
            // �I������Ă���A�C�e�����폜
            if (io_Box.SelectedIndex != -1)
            {
                io_Box.Items.Remove(io_Box.SelectedItem);
                io_Box.SelectedText = String.Empty;
            }
            else
            {
                // �����I������Ă��Ȃ��ꍇ���A���͂��ꂽ�����񂾂��͏����Ă���
                io_Box.Text = String.Empty;
                return false;
            }

            return true;
        }

        // ���v���\�[�X�}�l�[�W���[�̃����o

        // ���ʃ_�C�A���O�i�ʒm�^�x���^�G���[�j
        // �����ʃ_�C�A���O�́A���͂��ꂽ����������̂܂ܕ\��������̂ƁA���͂��ꂽ������
        //   �Ń��\�[�X���擾�A�t�H�[�}�b�g���ĕ\��������̂̊e2���

        /// <summary>
        /// ���ʒʒm�_�C�A���O�i���͂��ꂽ�������\���j�B
        /// </summary>
        /// <param name="msg">���b�Z�[�W�B</param>
        public virtual void InformationDialog(string msg)
        {
            // �n���ꂽ������Œʒm�_�C�A���O��\��
            // ��Resource����NULL�̏ꍇ�ANullReferenceException�������̂܂ܕԂ�
            MessageBox.Show(
                msg,
                Resources.InformationTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// ���ʒʒm�_�C�A���O�i���͂��ꂽ������Ń��\�[�X���擾�A�t�H�[�}�b�g���ĕ\���j�B
        /// </summary>
        /// <param name="i_Key">���\�[�X�̃L�[�B</param>
        /// <param name="i_args">�t�H�[�}�b�g�̈����B</param>
        public virtual void InformationDialogResource(string i_Key, params object[] i_args)
        {
            // ���t�@�C�����烊�\�[�X���ǂݎ��Ȃ��ꍇ��ArgumentException���B
            //   ����ȊO��NullReferenceException�������̂܂ܕԂ�

            // �L�[�l���ȗ�������Ă���ꍇ�A�擪�ɒǉ����ď���
            string key = (string) i_Key.Clone();
            if (key.StartsWith("InformationMessage_") == false)
            {
                key = "InformationMessage_" + i_Key;
            }

            // .resources����w�肳�ꂽ���b�Z�[�W��ǂݍ���
            string text = this.Resource.GetString(key);
            if (text == null)
            {
                throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
            }

            // �\���p�֐����R�[��
            this.InformationDialog(String.Format(text, i_args));
        }

        /// <summary>
        /// ���ʌx���_�C�A���O�i���͂��ꂽ�������\���j�B
        /// </summary>
        /// <param name="msg">���b�Z�[�W�B</param>
        public virtual void WarningDialog(string msg)
        {
            // �n���ꂽ������Ōx���_�C�A���O��\��
            // ��Resource����NULL�̏ꍇ�ANullReferenceException�������̂܂ܕԂ�
            MessageBox.Show(
                msg,
                Resources.WarningTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// ���ʌx���_�C�A���O�i���͂��ꂽ������Ń��\�[�X���擾�A�t�H�[�}�b�g���ĕ\���j�B
        /// </summary>
        /// <param name="i_Key">���\�[�X�̃L�[�B</param>
        /// <param name="i_args">�t�H�[�}�b�g�̈����B</param>
        public virtual void WarningDialogResource(string i_Key, params object[] i_args)
        {
            // ���t�@�C�����烊�\�[�X���ǂݎ��Ȃ��ꍇ��ArgumentException���B
            //   ����ȊO��NullReferenceException�������̂܂ܕԂ�

            // �L�[�l���ȗ�������Ă���ꍇ�A�擪�ɒǉ����ď���
            string key = (string) i_Key.Clone();
            if (key.StartsWith("WarningMessage_") == false)
            {
                key = "WarningMessage_" + i_Key;
            }

            // .resources����w�肳�ꂽ���b�Z�[�W��ǂݍ���
            string text = this.Resource.GetString(key);
            if (text == null)
            {
                throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
            }

            // �\���p�֐����R�[��
            this.WarningDialog(String.Format(text, i_args));
        }

        /// <summary>
        /// ���ʃG���[�_�C�A���O�i���͂��ꂽ�������\���j�B
        /// </summary>
        /// <param name="msg">���b�Z�[�W�B</param>
        public virtual void ErrorDialog(string msg)
        {
            // �n���ꂽ������ŃG���[�_�C�A���O��\��
            // ��Resource����NULL�̏ꍇ�ANullReferenceException�������̂܂ܕԂ�
            MessageBox.Show(
                msg,
                Resources.ErrorTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// ���ʃG���[�_�C�A���O�i���͂��ꂽ������Ń��\�[�X���擾�A�t�H�[�}�b�g���ĕ\���j�B
        /// </summary>
        /// <param name="i_Key">���\�[�X�̃L�[�B</param>
        /// <param name="i_args">�t�H�[�}�b�g�̈����B</param>
        public virtual void ErrorDialogResource(string i_Key, params object[] i_args)
        {
            // ���t�@�C�����烊�\�[�X���ǂݎ��Ȃ��ꍇ��ArgumentException���B
            //   ����ȊO��NullReferenceException�������̂܂ܕԂ�

            // �L�[�l���ȗ�������Ă���ꍇ�A�擪�ɒǉ����ď���
            string key = (string) i_Key.Clone();
            if (key.StartsWith("ErrorMessage_") == false)
            {
                key = "ErrorMessage_" + i_Key;
            }

            // .resources����w�肳�ꂽ���b�Z�[�W��ǂݍ���
            string text = this.Resource.GetString(key);
            if (text == null)
            {
                throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
            }

            // �\���p�֐����R�[��
            this.ErrorDialog(String.Format(text, i_args));
        }

        // �����\�[�X�}�l�[�W���[��null�ł�����A���̏ꍇ�̓_�C�A���O���o�Ȃ�

        /// <summary>
        /// �t�H���_�I�[�v���B
        /// </summary>
        /// <param name="i_Path">�t�H���_�̃p�X�B</param>
        /// <param name="i_ShowEnabled"><c>true</c> �G���[���Ƀ_�C�A���O��\���B</param>
        /// <returns><c>true</c> �I�[�v������</returns>
        public virtual bool OpenFolder(string i_Path, bool i_ShowEnabled)
        {
            // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false
            System.Diagnostics.Debug.WriteLine("Cmn.OpenFolder > " + i_Path);

            // ��̏ꍇ�͏��O
            if (String.IsNullOrEmpty(i_Path))
            {
                return false;
            }

            // �Ώۃf�[�^�̃t�H���_���J��
            if (Directory.Exists(i_Path))
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", "/n," + i_Path);
                }
                catch (Exception e)
                {
                    if (this.Resource != null && i_ShowEnabled)
                    {
                        this.ErrorDialogResource("ErrorMessage_NotDataOpen", e.Message);
                    }

                    return false;
                }
            }
            else
            {
                if (this.Resource != null && i_ShowEnabled)
                {
                    this.ErrorDialogResource("ErrorMessage_NotDataExist");
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// �t�H���_�I�[�v���i�G���[���Ƀ_�C�A���O��\���j�B
        /// </summary>
        /// <param name="path">�t�H���_�̃p�X�B</param>
        /// <returns><c>true</c> �I�[�v������</returns>
        public bool OpenFolder(string path)
        {
            return this.OpenFolder(path, true);
        }

        /// <summary>
        /// �t�@�C���I�[�v���B
        /// </summary>
        /// <param name="i_Path">�t�@�C���̃p�X�B</param>
        /// <param name="i_ShowEnabled"><c>true</c> �G���[���Ƀ_�C�A���O��\���B</param>
        /// <returns><c>true</c> �I�[�v������</returns>
        public virtual bool OpenFile(string i_Path, bool i_ShowEnabled)
        {
            // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false
            System.Diagnostics.Debug.WriteLine("Cmn.OpenFile > " + i_Path);

            // ��̃Z���͏��O
            if (String.IsNullOrEmpty(i_Path))
            {
                return false;
            }

            // �Ώۃf�[�^���J��
            if (File.Exists(i_Path))
            {
                try
                {
                    System.Diagnostics.Process.Start(i_Path);
                }
                catch (Exception e)
                {
                    if (this.Resource != null && i_ShowEnabled)
                    {
                        this.ErrorDialogResource("ErrorMessage_NotDataOpen", e.Message);
                    }

                    return false;
                }
            }
            else
            {
                if (this.Resource != null && i_ShowEnabled)
                {
                    this.ErrorDialogResource("ErrorMessage_NotDataExist");
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// �t�@�C���I�[�v���i�G���[���Ƀ_�C�A���O��\���j�B
        /// </summary>
        /// <param name="path">�t�@�C���̃p�X�B</param>
        /// <returns><c>true</c> �I�[�v������</returns>
        public bool OpenFile(string path)
        {
            return this.OpenFile(path, true);
        }

        /// <summary>
        /// �T�[�o�[�ڑ��`�F�b�N�B
        /// </summary>
        /// <param name="i_Server">�T�[�o�[���B</param>
        /// <param name="i_ShowEnabled"><c>true</c> �G���[���Ƀ_�C�A���O��\���B</param>
        /// <returns><c>true</c> �ڑ�����</returns>
        public virtual bool Ping(string i_Server, bool i_ShowEnabled)
        {
            // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false

            // �T�[�o�[�ڑ��`�F�b�N
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(i_Server);
                if (reply.Status != IPStatus.Success)
                {
                    if (this.Resource != null && i_ShowEnabled)
                    {
                        this.ErrorDialogResource("ErrorMessage_MissNetworkAccess", reply.Status.ToString());
                    }

                    return false;
                }
            }
            catch (Exception e)
            {
                if (this.Resource != null && i_ShowEnabled)
                {
                    this.ErrorDialogResource("ErrorMessage_MissNetworkAccess", e.InnerException.Message);
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// �T�[�o�[�ڑ��`�F�b�N�i�G���[���Ƀ_�C�A���O��\���j�B
        /// </summary>
        /// <param name="server">�T�[�o�[���B</param>
        /// <returns><c>true</c> �ڑ�����</returns>
        public bool Ping(string server)
        {
            return this.Ping(server, true);
        }

        /// <summary>
        /// DataGridView��CSV�t�@�C���ւ̏o�́B
        /// </summary>
        /// <param name="i_View">CSV�t�@�C�����o�͂���DataGridView�B</param>
        /// <param name="i_Path">CSV�t�@�C���p�X�B</param>
        /// <param name="i_ShowEnabled"><c>true</c> �G���[���Ƀ_�C�A���O��\���B</param>
        /// <returns><c>true</c> �o�͐���</returns>
        public virtual bool SaveDataGridViewCsv(DataGridView i_View, string i_Path, bool i_ShowEnabled)
        {
            // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false
            try
            {
                // DataGridView�̕\�����e��CSV�t�@�C���ɏo��
                StreamWriter sw = new StreamWriter(i_Path, false, System.Text.Encoding.GetEncoding("Shift-JIS"));
                try
                {
                    // �w�b�_�[�o��
                    string header = String.Empty;
                    foreach (DataGridViewColumn column in i_View.Columns)
                    {
                        if (header != String.Empty)
                        {
                            header += ",";
                        }

                        header += "\"" + column.HeaderText + "\"";
                    }

                    sw.WriteLine(header);

                    // �{�̏o��
                    for (int y = 0; y < i_View.RowCount; y++)
                    {
                        // �\������Ă��Ȃ��s�͏��O
                        if (i_View.Rows[y].Visible == false)
                        {
                            continue;
                        }

                        // 1�s���Ƃɏo��
                        string line = String.Empty;
                        for (int x = 0; x < i_View.ColumnCount; x++)
                        {
                            if (x != 0)
                            {
                                line += ",";
                            }

                            line += "\"" + NullCheckAndTrim(i_View[x, y]) + "\"";
                        }

                        sw.WriteLine(line);
                    }
                }
                finally
                {
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                if (this.Resource != null && i_ShowEnabled)
                {
                    this.ErrorDialogResource("ErrorMessage_MissFileSaveView", e.Message);
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// DataGridView��CSV�t�@�C���ւ̏o�́i�G���[���Ƀ_�C�A���O��\���j�B
        /// </summary>
        /// <param name="i_View">CSV�t�@�C�����o�͂���DataGridView�B</param>
        /// <param name="i_Path">CSV�t�@�C���p�X�B</param>
        /// <returns><c>true</c> �o�͐���</returns>
        public bool SaveDataGridViewCsv(DataGridView i_View, string i_Path)
        {
            return this.SaveDataGridViewCsv(i_View, i_Path, true);
        }
    }
}
