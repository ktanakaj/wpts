using System;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Xml.Serialization;
using wptscs.Properties;

namespace MYAPP
{
    // ��ʁE�@�\�ɂ��Ȃ��A���ʓI�Ȋ֐��N���X�i�� �����\��j
    public class Cmn
    {
		/* �R���X�g���N�^�iexe�Ɠ����̃��\�[�X�}�l�[�W���[���N���t�H���_����ǂݍ��݁j */
		public Cmn()
        {
	        // �R���X�g���N�^�ł��邽�߁A��O�͓����Ȃ��B�ُ펞��NULL�ŏ�����
	        try{
		        // �t�@�C������ݒ��ǂݍ���
		        Resource = ResourceManager.CreateFileBasedResourceManager(
			        Path.GetFileNameWithoutExtension(Application.ExecutablePath),
			        Application.StartupPath,
			        null);
	        }
	        catch(Exception e){
		        System.Diagnostics.Debug.WriteLine("Cmn.Cmn > ��O�����F" + e.ToString());
		        Resource = null;
	        }
        }

		/* �R���X�g���N�^�i�w�肳�ꂽ���\�[�X�}�l�[�W���[���w�肳�ꂽ�t�H���_����ݒ�j */
        public Cmn(String i_Resource, String i_Dir)
        {
        	System.Diagnostics.Debug.WriteLine("Cmn.Cmn > " + i_Resource + ", " + i_Dir);
        	// �R���X�g���N�^�ł��邽�߁A��O�͓����Ȃ��B�ُ펞��NULL�ŏ�����
        	try{
        		// �t�@�C������ݒ��ǂݍ���
        		Resource = ResourceManager.CreateFileBasedResourceManager(i_Resource, i_Dir, null);
        	}
        	catch(Exception e){
		        System.Diagnostics.Debug.WriteLine("Cmn.Cmn > ��O�����F" + e.ToString());
        		Resource = null;
        	}
        }

		/* �R���X�g���N�^�i�n���ꂽ���\�[�X�}�l�[�W���[���g�p�j */
        public Cmn(ResourceManager i_Resource)
        {
        	// �n���ꂽ���\�[�X�}�l�[�W���[�����̂܂܎g�p
	        Resource = i_Resource;
        }

        // ���\�[�X�}�l�[�W���[
		public ResourceManager Resource;

		// ���v���\�[�X�}�l�[�W���[

		// ���ʃ_�C�A���O�i�ʒm�^�x���^�G���[�j
		// �����ʃ_�C�A���O�́A���͂��ꂽ����������̂܂ܕ\��������̂ƁA���͂��ꂽ������
		//   �Ń��\�[�X���擾�A�t�H�[�}�b�g���ĕ\��������̂̊e2���

        /* ���ʒʒm�_�C�A���O�i���͂��ꂽ�������\���j */
        public virtual void InformationDialog(String i_Msg)
        {
            // �n���ꂽ������Œʒm�_�C�A���O��\��
        	// ��Resource����NULL�̏ꍇ�ANullReferenceException�������̂܂ܕԂ�
        	MessageBox.Show(
        		i_Msg,
        		Resources.InformationTitle,
        		MessageBoxButtons.OK,
        		MessageBoxIcon.Information);
        }

        /* ���ʒʒm�_�C�A���O�i���͂��ꂽ������Ń��\�[�X���擾�A�t�H�[�}�b�g���ĕ\���j */
        public virtual void InformationDialogResource(String i_Key, params Object[] i_args)
        {
	        // ���t�@�C�����烊�\�[�X���ǂݎ��Ȃ��ꍇ��ArgumentException���B
	        //   ����ȊO��NullReferenceException�������̂܂ܕԂ�

	        // �L�[�l���ȗ�������Ă���ꍇ�A�擪�ɒǉ����ď���
	        String key = (String) i_Key.Clone();
	        if(key.StartsWith("InformationMessage_") == false){
		        key = "InformationMessage_" + i_Key;
	        }
	        // .resources����w�肳�ꂽ���b�Z�[�W��ǂݍ���
	        String text = Resource.GetString(key);
	        if(text == null){
		        throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
	        }
	        // �\���p�֐����R�[��
	        InformationDialog(String.Format(text, i_args));
        }

        /* ���ʌx���_�C�A���O�i���͂��ꂽ�������\���j */
        public virtual void WarningDialog(String i_Msg)
        {
	        // �n���ꂽ������Ōx���_�C�A���O��\��
	        // ��Resource����NULL�̏ꍇ�ANullReferenceException�������̂܂ܕԂ�
	        MessageBox.Show(
		        i_Msg,
		        Resources.WarningTitle,
		        MessageBoxButtons.OK,
		        MessageBoxIcon.Warning);
        }

        /* ���ʌx���_�C�A���O�i���͂��ꂽ������Ń��\�[�X���擾�A�t�H�[�}�b�g���ĕ\���j */
        public virtual void WarningDialogResource(String i_Key, params Object[] i_args)
        {
	        // ���t�@�C�����烊�\�[�X���ǂݎ��Ȃ��ꍇ��ArgumentException���B
	        //   ����ȊO��NullReferenceException�������̂܂ܕԂ�

	        // �L�[�l���ȗ�������Ă���ꍇ�A�擪�ɒǉ����ď���
	        String key = (String) i_Key.Clone();
	        if(key.StartsWith("WarningMessage_") == false){
		        key = "WarningMessage_" + i_Key;
	        }
	        // .resources����w�肳�ꂽ���b�Z�[�W��ǂݍ���
	        String text = Resource.GetString(key);
	        if(text == null){
		        throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
	        }
	        // �\���p�֐����R�[��
	        WarningDialog(String.Format(text, i_args));
        }

        /* ���ʃG���[�_�C�A���O�i���͂��ꂽ�������\���j */
        public virtual void ErrorDialog(String i_Msg)
        {
	        // �n���ꂽ������ŃG���[�_�C�A���O��\��
	        // ��Resource����NULL�̏ꍇ�ANullReferenceException�������̂܂ܕԂ�
	        MessageBox.Show(
		        i_Msg,
		        Resources.ErrorTitle,
		        MessageBoxButtons.OK,
		        MessageBoxIcon.Error);
        }

        /* ���ʃG���[�_�C�A���O�i���͂��ꂽ������Ń��\�[�X���擾�A�t�H�[�}�b�g���ĕ\���j */
        public virtual void ErrorDialogResource(String i_Key, params Object[] i_args)
        {
	        // ���t�@�C�����烊�\�[�X���ǂݎ��Ȃ��ꍇ��ArgumentException���B
	        //   ����ȊO��NullReferenceException�������̂܂ܕԂ�

	        // �L�[�l���ȗ�������Ă���ꍇ�A�擪�ɒǉ����ď���
	        String key = (String) i_Key.Clone();
	        if(key.StartsWith("ErrorMessage_") == false){
		        key = "ErrorMessage_" + i_Key;
	        }
	        // .resources����w�肳�ꂽ���b�Z�[�W��ǂݍ���
	        String text = Resource.GetString(key);
	        if(text == null){
		        throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
	        }
	        // �\���p�֐����R�[��
	        ErrorDialog(String.Format(text, i_args));
        }


		// �����\�[�X�}�l�[�W���[��null�ł�����A���̏ꍇ�̓_�C�A���O���o�Ȃ�

        /* �t�H���_�I�[�v�� */
        public virtual bool OpenFolder(String i_Path, bool i_ShowEnabled)
        {
	        // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false

	        System.Diagnostics.Debug.WriteLine("Cmn.OpenFolder > " + i_Path);
	        // ��̏ꍇ�͏��O
	        if(String.IsNullOrEmpty(i_Path)){
		        return false;
	        }
	        // �Ώۃf�[�^�̃t�H���_���J��
	        if(Directory.Exists(i_Path)){
		        try{
                    System.Diagnostics.Process.Start("explorer.exe", "/n," + i_Path);
		        }
		        catch(Exception e){
			        if(Resource != null && i_ShowEnabled){
				        ErrorDialogResource("ErrorMessage_NotDataOpen", e.Message);
			        }
			        return false;
		        }
	        }
	        else{
		        if(Resource != null && i_ShowEnabled){
			        ErrorDialogResource("ErrorMessage_NotDataExist");
		        }
		        return false;
	        }
	        return true;
        }

        /* �t�H���_�I�[�v���i��ʕ\���t���O���f�t�H���g�l�j */
        public bool OpenFolder(String i_Path)
        {
	        return OpenFolder(i_Path, true);
        }

        /* �t�@�C���I�[�v�� */
        public virtual bool OpenFile(String i_Path, bool i_ShowEnabled)
        {
	        // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false

	        System.Diagnostics.Debug.WriteLine("Cmn.OpenFile > " + i_Path);
	        // ��̃Z���͏��O
	        if(String.IsNullOrEmpty(i_Path)){
		        return false;
	        }
	        // �Ώۃf�[�^���J��
	        if(File.Exists(i_Path)){
		        try{
                    System.Diagnostics.Process.Start(i_Path);
		        }
		        catch(Exception e){
			        if(Resource != null && i_ShowEnabled){
				        ErrorDialogResource("ErrorMessage_NotDataOpen", e.Message);
			        }
			        return false;
		        }
	        }
	        else{
		        if(Resource != null && i_ShowEnabled){
			        ErrorDialogResource("ErrorMessage_NotDataExist");
		        }
		        return false;
	        }
	        return true;
        }

        /* �t�@�C���I�[�v���i��ʕ\���t���O���f�t�H���g�l�j */
        public bool OpenFile(String i_Path)
        {
	        return OpenFile(i_Path, true);
        }

        /* �T�[�o�[�ڑ��`�F�b�N */
        public virtual bool Ping(String i_Server, bool i_ShowEnabled)
        {
	        // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false

	        // �T�[�o�[�ڑ��`�F�b�N
	        Ping ping = new Ping();
	        try{
		        PingReply reply = ping.Send(i_Server);
		        if(reply.Status != IPStatus.Success){
			        if(Resource != null && i_ShowEnabled){
				        ErrorDialogResource("ErrorMessage_MissNetworkAccess", reply.Status.ToString());
			        }
			        return false;
		        }
	        }
	        catch(Exception e){
		        if(Resource != null && i_ShowEnabled){
			        ErrorDialogResource("ErrorMessage_MissNetworkAccess", e.InnerException.Message);
		        }
		        return false;
	        }
	        return true;
        }

        /* �T�[�o�[�ڑ��`�F�b�N�i��ʕ\���t���O���f�t�H���g�l�j */
        public bool Ping(String i_Server)
        {
	        return Ping(i_Server, true);
        }

        /* DataGridView��CSV�t�@�C���ւ̏o�� */
        public virtual bool SaveDataGridViewCsv(DataGridView i_View, String i_Path, bool i_ShowEnabled)
        {
	        // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false
	        try{
		        // DataGridView�̕\�����e��CSV�t�@�C���ɏo��
		        StreamWriter sw = new StreamWriter(i_Path, false, System.Text.Encoding.GetEncoding("Shift-JIS"));
		        try{
			        // �w�b�_�[�o��
			        String header = "";
			        foreach(DataGridViewColumn column in i_View.Columns){
				        if(header != ""){
					        header += ",";
				        }
				        header += ("\"" + column.HeaderText + "\"");
			        }
			        sw.WriteLine(header);
			        // �{�̏o��
			        for(int y = 0; y < i_View.RowCount; y++){
				        // �\������Ă��Ȃ��s�͏��O
				        if(i_View.Rows[y].Visible == false){
					        continue;
				        }
				        // 1�s���Ƃɏo��
				        String line = "";
				        for(int x = 0; x < i_View.ColumnCount; x++){
					        if(x != 0){
						        line += ",";
					        }
					        line += ("\"" + NullCheckAndTrim(i_View[x,y]) + "\"");
				        }
				        sw.WriteLine(line);
			        }
		        }
		        finally{
			        sw.Close();
		        }
	        }
	        catch(Exception e){
		        if(Resource != null && i_ShowEnabled){
			        ErrorDialogResource("ErrorMessage_MissFileSaveView", e.Message);
		        }
		        return false;
	        }
	        return true;
        }

        /* DataGridView��CSV�t�@�C���ւ̏o�́i��ʕ\���t���O���f�t�H���g�l�j */
        public bool SaveDataGridViewCsv(DataGridView i_View, String i_Path)
        {
	        return SaveDataGridViewCsv(i_View, i_Path, true);
        }


		// ���ȉ��͐ÓI�����o�֐�

        /* String��NULL�l�`�F�b�N��Trim */
        public static String NullCheckAndTrim(String i_Str)
        {
	        // ����O�Ȃ�
	        if(i_Str == null){
		        return "";
	        }
	        return i_Str.Trim();
        }

        /* DataGridViewCell��NULL�l�`�F�b�N��Trim */
        public static String NullCheckAndTrim(DataGridViewCell i_Cell)
        {
	        // ����O�Ȃ�
	        if(i_Cell == null){
		        return "";
	        }
	        else if(i_Cell.Value == null){
		        return "";
	        }
	        return i_Cell.Value.ToString().Trim();
        }

        /* �z��ւ̗v�f�iNULL�j�ǉ� */
        public static int AddArray<T>(ref T[] io_Array)
        {
	        // ��Resize���ŗ�O�����������ꍇ�́A���̂܂ܓ�����
	        if(io_Array == null){
		        io_Array = new T[1]; 
		        return 0;
	        }
	        Array.Resize<T>(ref io_Array, io_Array.Length + 1);
	        return io_Array.Length - 1;
        }

        /* �z��ւ̗v�f�i���͒l�j�ǉ� */
        public static int AddArray<T>(ref T[] io_Array, T i_Obj)
        {
	        // ��Resize�����ɗ�O�����������ꍇ�́A���̂܂ܓ�����
	        int index = AddArray(ref io_Array);
	        io_Array[index] = i_Obj;
	        return index;
        }

        /* �\�t�g��+�o�[�W�������̕�����擾�i�A�Z���u������擾�j */
        public static String GetProductName()
        {
	        // ����O�Ȃ��B������������ꍇ�͂��̂܂ܕԂ�

	        // �A�Z���u�����琻�i�����擾���A�o�[�W�������(x.xx�`��)��t���ĕԂ�
	        Assembly assembly = Assembly.GetExecutingAssembly();
	        AssemblyProductAttribute product = 
		        (AssemblyProductAttribute) Attribute.GetCustomAttribute(
			        assembly,
                    typeof(AssemblyProductAttribute));
	        Version ver = assembly.GetName().Version;
	        // �߂�l��Ԃ��A�r���h�ԍ��E���r�W�����͖���
	        return (product.Product + " Ver" + ver.Major + "." + String.Format("{0:D2}",ver.Minor));
        }

        /* �����񒆂̃t�@�C�����Ɏg�p�ł��Ȃ�������u�� */
        public static String ReplaceInvalidFileNameChars(String i_Str)
        {
	        // �n���ꂽ������Ƀt�@�C�����Ɏg���Ȃ��������܂܂�Ă���ꍇ�A_ �ɒu��������
	        String result = i_Str;
	        char[] unuseChars = Path.GetInvalidFileNameChars();
            foreach (char c in unuseChars)
            {
		        result = result.Replace(c, '_');
	        }
	        return result;
        }

        /* �I�u�W�F�N�g��XML�ւ̃V���A���C�Y */
        public static bool XmlSerialize(Object i_Obj, String i_FileName)
        {
	        // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false

	        // �ݒ���V���A���C�Y��
	        System.Diagnostics.Debug.WriteLine("Cmn.Serialize > " + i_FileName + "�ɃV���A���C�Y");

	        XmlSerializer serializer = new XmlSerializer(i_Obj.GetType());
	        try{
		        Stream writer = new FileStream(i_FileName, FileMode.Create);
		        try{
			        serializer.Serialize(writer, i_Obj);
		        }
		        finally{
			        writer.Close();
		        }
	        }
	        catch(Exception e){
		        System.Diagnostics.Debug.WriteLine("Cmn.Serialize > ��O�����F" + e.ToString());
		        return false;
	        }
	        return true;
        }

        /* �I�u�W�F�N�g��XML����̃f�V���A���C�Y */
        public static bool XmlDeserialize(ref Object o_Obj, Type i_Type, String i_FileName)
        {
	        // ����O�͓����Ȃ��B���s�����ꍇ�͑S��false

	        // �ݒ���f�V���A���C�Y��
	        System.Diagnostics.Debug.WriteLine("Cmn.Deserialize > " + i_FileName + "����f�V���A���C�Y");

	        // �o�͒l������
	        o_Obj = null;
	        // ������gcnew�Ń��O��ɗ�O���o�邪�AMSDN�ɂ��Ζ������Ă����炵���E�E�E
	        XmlSerializer serializer = new XmlSerializer(i_Type);
	        try{
		        Stream reader = new FileStream(i_FileName, FileMode.Open, FileAccess.Read);
		        try{
			        o_Obj = serializer.Deserialize(reader);
			        // ���ق�Ƃ�Object�̌^�ɃL���X�g���đ���������̂����A
			        //   ���@�s���Ȃ��߁A����͌Ăяo�����ōs��
		        }
		        finally{
			        reader.Close();
		        }
	        }
	        catch(Exception e){
		        System.Diagnostics.Debug.WriteLine("Cmn.Deserialize > ��O�����F" + e.ToString());
		        return false;
	        }
	        return true;
        }

        /* �Ώۂ̕����񂪁A�n���ꂽ������̃C���f�b�N�X�Ԗڂɑ��݂��邩���`�F�b�N */
        public static bool ChkTextInnerWith(String i_Text, int i_Index, String i_ChkStr)
        {
	        // ��i_Text, i_ChkStr��NULL�̏ꍇ�͂��̂܂�NullReferenceException���B
	        //   i_Index��i_Text�͈̔͊O�̂Ƃ���ArgumentException��Ԃ�

	        // ���͒l�`�F�b�N
	        if((i_Index < 0) || (i_Index >= i_Text.Length)){
		        throw new ArgumentException("IndexOutOfRange! : " + i_Index, "i_Index");
	        }
	        if(i_ChkStr == ""){
		        return true;
	        }
	        if(i_Text == ""){
		        return false;
	        }
	        // ������̈�v���`�F�b�N
	        // ��1�ڂ�if�͑��x�I�ɂ������̕����������ȂƎv���Ă���Ă���B�v��Ȃ������E�E�E
	        if(i_Text[i_Index] == i_ChkStr[0]){
		        if((i_Index + i_ChkStr.Length) <= i_Text.Length){
			        if(i_Text.Substring(i_Index, i_ChkStr.Length) == i_ChkStr){
				        return true;
			        }
		        }
	        }
	        return false;
        }

        /* �R���{�{�b�N�X���m�F���A���݂̒l���ꗗ�ɖ�����Γo�^ */
        public static bool AddComboBoxNewItem(ref ComboBox io_Box, String i_FirstStr)
        {
	        // ��io_Box��NULL�̏ꍇ�Ȃǂ́ANullReferenceException�������̂܂ܕԂ�

            //	System.Diagnostics.Debug.WriteLine("Cmn.AddComboBoxNewItem > " + io_Box.Text + ", " + i_FirstStr);
	        if(io_Box.Text != ""){
		        // ���݂̒l��i_FirstStr����n�܂��Ă��Ȃ��ꍇ�́Ai_FirstStr��t���ď���
		        String text = io_Box.Text;
		        if(i_FirstStr != null && i_FirstStr != ""){
			        if(text.StartsWith(i_FirstStr) == false){
				        text = i_FirstStr + io_Box.Text;
			        }
		        }
		        // ���݂̒l�Ɉ�v������̂����邩���m�F
		        if(io_Box.Items.Contains(text) == false){
			        // ���݂��Ȃ��ꍇ�A���݂̒l���ꗗ�ɒǉ�
			        io_Box.Items.Add(text);
			        return true;
		        }
	        }
	        return false;
        }

        /* �R���{�{�b�N�X���m�F���A���݂̒l���ꗗ�ɖ�����Γo�^ */
        public static bool AddComboBoxNewItem(ref ComboBox io_Box)
        {
	        // �ǉ������񖳂��ŁA������̊֐����R�[��
	        return AddComboBoxNewItem(ref io_Box, "");
        }

        /* �R���{�{�b�N�X���m�F���A�I������Ă���l���폜 */
        public static bool RemoveComboBoxItem(ref ComboBox io_Box)
        {
	        // ��io_Box��NULL�̏ꍇ�Ȃǂ́ANullReferenceException�������̂܂ܕԂ�

            //	System.Diagnostics.Debug.WriteLine("Cmn.RemoveComboBoxItem > " + io_Box.SelectedIndex.ToString());
	        // �I������Ă���A�C�e�����폜
	        if(io_Box.SelectedIndex != -1){
		        io_Box.Items.Remove(io_Box.SelectedItem);
		        io_Box.SelectedText = "";
	        }
	        else{
		        // �����I������Ă��Ȃ��ꍇ���A���͂��ꂽ�����񂾂��͏����Ă���
		        io_Box.Text = "";
		        return false;
	        }
	        return true;
        }
    }
}
