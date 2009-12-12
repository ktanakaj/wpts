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
    // 画面・機能によらない、共通的な関数クラス（※ 整理予定）
    public class Cmn
    {
		/* コンストラクタ（exeと同名のリソースマネージャーを起動フォルダから読み込み） */
		public Cmn()
        {
	        // コンストラクタであるため、例外は投げない。異常時はNULLで初期化
	        try{
		        // ファイルから設定を読み込み
		        Resource = ResourceManager.CreateFileBasedResourceManager(
			        Path.GetFileNameWithoutExtension(Application.ExecutablePath),
			        Application.StartupPath,
			        null);
	        }
	        catch(Exception e){
		        System.Diagnostics.Debug.WriteLine("Cmn.Cmn > 例外発生：" + e.ToString());
		        Resource = null;
	        }
        }

		/* コンストラクタ（指定されたリソースマネージャーを指定されたフォルダから設定） */
        public Cmn(String i_Resource, String i_Dir)
        {
        	System.Diagnostics.Debug.WriteLine("Cmn.Cmn > " + i_Resource + ", " + i_Dir);
        	// コンストラクタであるため、例外は投げない。異常時はNULLで初期化
        	try{
        		// ファイルから設定を読み込み
        		Resource = ResourceManager.CreateFileBasedResourceManager(i_Resource, i_Dir, null);
        	}
        	catch(Exception e){
		        System.Diagnostics.Debug.WriteLine("Cmn.Cmn > 例外発生：" + e.ToString());
        		Resource = null;
        	}
        }

		/* コンストラクタ（渡されたリソースマネージャーを使用） */
        public Cmn(ResourceManager i_Resource)
        {
        	// 渡されたリソースマネージャーをそのまま使用
	        Resource = i_Resource;
        }

        // リソースマネージャー
		public ResourceManager Resource;

		// ↓要リソースマネージャー

		// 共通ダイアログ（通知／警告／エラー）
		// ※共通ダイアログは、入力された文字列をそのまま表示するものと、入力された文字列
		//   でリソースを取得、フォーマットして表示するものの各2種類

        /* 共通通知ダイアログ（入力された文字列を表示） */
        public virtual void InformationDialog(String i_Msg)
        {
            // 渡された文字列で通知ダイアログを表示
        	// ※Resource等がNULLの場合、NullReferenceException等をそのまま返す
        	MessageBox.Show(
        		i_Msg,
        		Resources.InformationTitle,
        		MessageBoxButtons.OK,
        		MessageBoxIcon.Information);
        }

        /* 共通通知ダイアログ（入力された文字列でリソースを取得、フォーマットして表示） */
        public virtual void InformationDialogResource(String i_Key, params Object[] i_args)
        {
	        // ※ファイルからリソースが読み取れない場合はArgumentExceptionを。
	        //   それ以外はNullReferenceException等をそのまま返す

	        // キー値が簡略化されている場合、先頭に追加して処理
	        String key = (String) i_Key.Clone();
	        if(key.StartsWith("InformationMessage_") == false){
		        key = "InformationMessage_" + i_Key;
	        }
	        // .resourcesから指定されたメッセージを読み込み
	        String text = Resource.GetString(key);
	        if(text == null){
		        throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
	        }
	        // 表示用関数をコール
	        InformationDialog(String.Format(text, i_args));
        }

        /* 共通警告ダイアログ（入力された文字列を表示） */
        public virtual void WarningDialog(String i_Msg)
        {
	        // 渡された文字列で警告ダイアログを表示
	        // ※Resource等がNULLの場合、NullReferenceException等をそのまま返す
	        MessageBox.Show(
		        i_Msg,
		        Resources.WarningTitle,
		        MessageBoxButtons.OK,
		        MessageBoxIcon.Warning);
        }

        /* 共通警告ダイアログ（入力された文字列でリソースを取得、フォーマットして表示） */
        public virtual void WarningDialogResource(String i_Key, params Object[] i_args)
        {
	        // ※ファイルからリソースが読み取れない場合はArgumentExceptionを。
	        //   それ以外はNullReferenceException等をそのまま返す

	        // キー値が簡略化されている場合、先頭に追加して処理
	        String key = (String) i_Key.Clone();
	        if(key.StartsWith("WarningMessage_") == false){
		        key = "WarningMessage_" + i_Key;
	        }
	        // .resourcesから指定されたメッセージを読み込み
	        String text = Resource.GetString(key);
	        if(text == null){
		        throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
	        }
	        // 表示用関数をコール
	        WarningDialog(String.Format(text, i_args));
        }

        /* 共通エラーダイアログ（入力された文字列を表示） */
        public virtual void ErrorDialog(String i_Msg)
        {
	        // 渡された文字列でエラーダイアログを表示
	        // ※Resource等がNULLの場合、NullReferenceException等をそのまま返す
	        MessageBox.Show(
		        i_Msg,
		        Resources.ErrorTitle,
		        MessageBoxButtons.OK,
		        MessageBoxIcon.Error);
        }

        /* 共通エラーダイアログ（入力された文字列でリソースを取得、フォーマットして表示） */
        public virtual void ErrorDialogResource(String i_Key, params Object[] i_args)
        {
	        // ※ファイルからリソースが読み取れない場合はArgumentExceptionを。
	        //   それ以外はNullReferenceException等をそのまま返す

	        // キー値が簡略化されている場合、先頭に追加して処理
	        String key = (String) i_Key.Clone();
	        if(key.StartsWith("ErrorMessage_") == false){
		        key = "ErrorMessage_" + i_Key;
	        }
	        // .resourcesから指定されたメッセージを読み込み
	        String text = Resource.GetString(key);
	        if(text == null){
		        throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
	        }
	        // 表示用関数をコール
	        ErrorDialog(String.Format(text, i_args));
        }


		// ↓リソースマネージャーがnullでも動作、その場合はダイアログが出ない

        /* フォルダオープン */
        public virtual bool OpenFolder(String i_Path, bool i_ShowEnabled)
        {
	        // ※例外は投げない。失敗した場合は全てfalse

	        System.Diagnostics.Debug.WriteLine("Cmn.OpenFolder > " + i_Path);
	        // 空の場合は除外
	        if(String.IsNullOrEmpty(i_Path)){
		        return false;
	        }
	        // 対象データのフォルダを開く
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

        /* フォルダオープン（画面表示フラグがデフォルト値） */
        public bool OpenFolder(String i_Path)
        {
	        return OpenFolder(i_Path, true);
        }

        /* ファイルオープン */
        public virtual bool OpenFile(String i_Path, bool i_ShowEnabled)
        {
	        // ※例外は投げない。失敗した場合は全てfalse

	        System.Diagnostics.Debug.WriteLine("Cmn.OpenFile > " + i_Path);
	        // 空のセルは除外
	        if(String.IsNullOrEmpty(i_Path)){
		        return false;
	        }
	        // 対象データを開く
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

        /* ファイルオープン（画面表示フラグがデフォルト値） */
        public bool OpenFile(String i_Path)
        {
	        return OpenFile(i_Path, true);
        }

        /* サーバー接続チェック */
        public virtual bool Ping(String i_Server, bool i_ShowEnabled)
        {
	        // ※例外は投げない。失敗した場合は全てfalse

	        // サーバー接続チェック
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

        /* サーバー接続チェック（画面表示フラグがデフォルト値） */
        public bool Ping(String i_Server)
        {
	        return Ping(i_Server, true);
        }

        /* DataGridViewのCSVファイルへの出力 */
        public virtual bool SaveDataGridViewCsv(DataGridView i_View, String i_Path, bool i_ShowEnabled)
        {
	        // ※例外は投げない。失敗した場合は全てfalse
	        try{
		        // DataGridViewの表示内容をCSVファイルに出力
		        StreamWriter sw = new StreamWriter(i_Path, false, System.Text.Encoding.GetEncoding("Shift-JIS"));
		        try{
			        // ヘッダー出力
			        String header = "";
			        foreach(DataGridViewColumn column in i_View.Columns){
				        if(header != ""){
					        header += ",";
				        }
				        header += ("\"" + column.HeaderText + "\"");
			        }
			        sw.WriteLine(header);
			        // 本体出力
			        for(int y = 0; y < i_View.RowCount; y++){
				        // 表示されていない行は除外
				        if(i_View.Rows[y].Visible == false){
					        continue;
				        }
				        // 1行ごとに出力
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

        /* DataGridViewのCSVファイルへの出力（画面表示フラグがデフォルト値） */
        public bool SaveDataGridViewCsv(DataGridView i_View, String i_Path)
        {
	        return SaveDataGridViewCsv(i_View, i_Path, true);
        }


		// ↓以下は静的メンバ関数

        /* StringのNULL値チェック＆Trim */
        public static String NullCheckAndTrim(String i_Str)
        {
	        // ※例外なし
	        if(i_Str == null){
		        return "";
	        }
	        return i_Str.Trim();
        }

        /* DataGridViewCellのNULL値チェック＆Trim */
        public static String NullCheckAndTrim(DataGridViewCell i_Cell)
        {
	        // ※例外なし
	        if(i_Cell == null){
		        return "";
	        }
	        else if(i_Cell.Value == null){
		        return "";
	        }
	        return i_Cell.Value.ToString().Trim();
        }

        /* 配列への要素（NULL）追加 */
        public static int AddArray<T>(ref T[] io_Array)
        {
	        // ※Resize等で例外が発生した場合は、そのまま投げる
	        if(io_Array == null){
		        io_Array = new T[1]; 
		        return 0;
	        }
	        Array.Resize<T>(ref io_Array, io_Array.Length + 1);
	        return io_Array.Length - 1;
        }

        /* 配列への要素（入力値）追加 */
        public static int AddArray<T>(ref T[] io_Array, T i_Obj)
        {
	        // ※Resize時等に例外が発生した場合は、そのまま投げる
	        int index = AddArray(ref io_Array);
	        io_Array[index] = i_Obj;
	        return index;
        }

        /* ソフト名+バージョン情報の文字列取得（アセンブリから取得） */
        public static String GetProductName()
        {
	        // ※例外なし。もし発生する場合はそのまま返す

	        // アセンブリから製品名を取得し、バージョン情報(x.xx形式)を付けて返す
	        Assembly assembly = Assembly.GetExecutingAssembly();
	        AssemblyProductAttribute product = 
		        (AssemblyProductAttribute) Attribute.GetCustomAttribute(
			        assembly,
                    typeof(AssemblyProductAttribute));
	        Version ver = assembly.GetName().Version;
	        // 戻り値を返す、ビルド番号・リビジョンは無視
	        return (product.Product + " Ver" + ver.Major + "." + String.Format("{0:D2}",ver.Minor));
        }

        /* 文字列中のファイル名に使用できない文字を置換 */
        public static String ReplaceInvalidFileNameChars(String i_Str)
        {
	        // 渡された文字列にファイル名に使えない文字が含まれている場合、_ に置き換える
	        String result = i_Str;
	        char[] unuseChars = Path.GetInvalidFileNameChars();
            foreach (char c in unuseChars)
            {
		        result = result.Replace(c, '_');
	        }
	        return result;
        }

        /* オブジェクトのXMLへのシリアライズ */
        public static bool XmlSerialize(Object i_Obj, String i_FileName)
        {
	        // ※例外は投げない。失敗した場合は全てfalse

	        // 設定をシリアライズ化
	        System.Diagnostics.Debug.WriteLine("Cmn.Serialize > " + i_FileName + "にシリアライズ");

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
		        System.Diagnostics.Debug.WriteLine("Cmn.Serialize > 例外発生：" + e.ToString());
		        return false;
	        }
	        return true;
        }

        /* オブジェクトのXMLからのデシリアライズ */
        public static bool XmlDeserialize(ref Object o_Obj, Type i_Type, String i_FileName)
        {
	        // ※例外は投げない。失敗した場合は全てfalse

	        // 設定をデシリアライズ化
	        System.Diagnostics.Debug.WriteLine("Cmn.Deserialize > " + i_FileName + "からデシリアライズ");

	        // 出力値初期化
	        o_Obj = null;
	        // ↓このgcnewでログ上に例外が出るが、MSDNによれば無視していいらしい・・・
	        XmlSerializer serializer = new XmlSerializer(i_Type);
	        try{
		        Stream reader = new FileStream(i_FileName, FileMode.Open, FileAccess.Read);
		        try{
			        o_Obj = serializer.Deserialize(reader);
			        // ※ほんとはObjectの型にキャストして代入したいのだが、
			        //   方法不明なため、現状は呼び出し元で行う
		        }
		        finally{
			        reader.Close();
		        }
	        }
	        catch(Exception e){
		        System.Diagnostics.Debug.WriteLine("Cmn.Deserialize > 例外発生：" + e.ToString());
		        return false;
	        }
	        return true;
        }

        /* 対象の文字列が、渡された文字列のインデックス番目に存在するかをチェック */
        public static bool ChkTextInnerWith(String i_Text, int i_Index, String i_ChkStr)
        {
	        // ※i_Text, i_ChkStrがNULLの場合はそのままNullReferenceExceptionを。
	        //   i_Indexがi_Textの範囲外のときはArgumentExceptionを返す

	        // 入力値チェック
	        if((i_Index < 0) || (i_Index >= i_Text.Length)){
		        throw new ArgumentException("IndexOutOfRange! : " + i_Index, "i_Index");
	        }
	        if(i_ChkStr == ""){
		        return true;
	        }
	        if(i_Text == ""){
		        return false;
	        }
	        // 文字列の一致をチェック
	        // ※1つ目のifは速度的にこっちの方が速いかなと思ってやっている。要らないかも・・・
	        if(i_Text[i_Index] == i_ChkStr[0]){
		        if((i_Index + i_ChkStr.Length) <= i_Text.Length){
			        if(i_Text.Substring(i_Index, i_ChkStr.Length) == i_ChkStr){
				        return true;
			        }
		        }
	        }
	        return false;
        }

        /* コンボボックスを確認し、現在の値が一覧に無ければ登録 */
        public static bool AddComboBoxNewItem(ref ComboBox io_Box, String i_FirstStr)
        {
	        // ※io_BoxがNULLの場合などは、NullReferenceException等をそのまま返す

            //	System.Diagnostics.Debug.WriteLine("Cmn.AddComboBoxNewItem > " + io_Box.Text + ", " + i_FirstStr);
	        if(io_Box.Text != ""){
		        // 現在の値がi_FirstStrから始まっていない場合は、i_FirstStrを付けて処理
		        String text = io_Box.Text;
		        if(i_FirstStr != null && i_FirstStr != ""){
			        if(text.StartsWith(i_FirstStr) == false){
				        text = i_FirstStr + io_Box.Text;
			        }
		        }
		        // 現在の値に一致するものがあるかを確認
		        if(io_Box.Items.Contains(text) == false){
			        // 存在しない場合、現在の値を一覧に追加
			        io_Box.Items.Add(text);
			        return true;
		        }
	        }
	        return false;
        }

        /* コンボボックスを確認し、現在の値が一覧に無ければ登録 */
        public static bool AddComboBoxNewItem(ref ComboBox io_Box)
        {
	        // 追加文字列無しで、もう一つの関数をコール
	        return AddComboBoxNewItem(ref io_Box, "");
        }

        /* コンボボックスを確認し、選択されている値を削除 */
        public static bool RemoveComboBoxItem(ref ComboBox io_Box)
        {
	        // ※io_BoxがNULLの場合などは、NullReferenceException等をそのまま返す

            //	System.Diagnostics.Debug.WriteLine("Cmn.RemoveComboBoxItem > " + io_Box.SelectedIndex.ToString());
	        // 選択されているアイテムを削除
	        if(io_Box.SelectedIndex != -1){
		        io_Box.Items.Remove(io_Box.SelectedItem);
		        io_Box.SelectedText = "";
	        }
	        else{
		        // 何も選択されていない場合も、入力された文字列だけは消しておく
		        io_Box.Text = "";
		        return false;
	        }
	        return true;
        }
    }
}
