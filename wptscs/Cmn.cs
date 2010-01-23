// ================================================================================================
// <summary>
//      画面・機能によらない、共通的な関数クラスソース</summary>
//
// <copyright file="Cmn.cs" company="honeplusのメモ帳">
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
    /// 画面・機能によらない、共通的な関数のクラスです。
    /// ※ 整理予定
    /// </summary>
    public class Cmn
    {
        /// <summary>
        /// リソースマネージャー
        /// </summary>
        public ResourceManager Resource;

        /// <summary>
        /// コンストラクタ（exeと同名のリソースマネージャーを起動フォルダから読み込み）。
        /// </summary>
        public Cmn()
        {
            // コンストラクタであるため、例外は投げない。異常時はNULLで初期化
            try
            {
                // ファイルから設定を読み込み
                this.Resource = ResourceManager.CreateFileBasedResourceManager(
                    Path.GetFileNameWithoutExtension(Application.ExecutablePath),
                    Application.StartupPath,
                    null);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Cmn.Cmn > 例外発生：" + e.ToString());
                this.Resource = null;
            }
        }

        /// <summary>
        /// コンストラクタ（指定されたリソースマネージャーを指定されたフォルダから設定）。
        /// </summary>
        /// <param name="i_Resource">リソースマネージャー。</param>
        /// <param name="i_Dir">リソースのあるフォルダ。</param>
        public Cmn(string i_Resource, string i_Dir)
        {
            System.Diagnostics.Debug.WriteLine("Cmn.Cmn > " + i_Resource + ", " + i_Dir);

            // コンストラクタであるため、例外は投げない。異常時はNULLで初期化
            try
            {
                // ファイルから設定を読み込み
                Resource = ResourceManager.CreateFileBasedResourceManager(i_Resource, i_Dir, null);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Cmn.Cmn > 例外発生：" + e.ToString());
                this.Resource = null;
            }
        }

        /// <summary>
        /// コンストラクタ（渡されたリソースマネージャーを使用）。
        /// </summary>
        /// <param name="resource">リソースマネージャー。</param>
        public Cmn(ResourceManager resource)
        {
            // 渡されたリソースマネージャーをそのまま使用
            this.Resource = resource;
        }

        // ↓以下は静的メンバ

        /// <summary>
        /// StringのNULL値チェック＆Trim。
        /// </summary>
        /// <param name="s">対象文字列。</param>
        /// <returns>チェック後の文字列</returns>
        public static string NullCheckAndTrim(string s)
        {
            // ※例外なし
            if (s == null)
            {
                return String.Empty;
            }

            return s.Trim();
        }

        /// <summary>
        /// DataGridViewCellのNULL値チェック＆Trim。
        /// </summary>
        /// <param name="c">対象セル。</param>
        /// <returns>チェック後の文字列</returns>
        public static string NullCheckAndTrim(DataGridViewCell c)
        {
            // ※例外なし
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
        /// 配列への要素（NULL）追加。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="io_Array">配列。</param>
        /// <returns>追加したインデックス</returns>
        public static int AddArray<T>(ref T[] io_Array)
        {
            // ※Resize等で例外が発生した場合は、そのまま投げる
            if (io_Array == null)
            {
                io_Array = new T[1];
                return 0;
            }

            Array.Resize<T>(ref io_Array, io_Array.Length + 1);
            return io_Array.Length - 1;
        }

        /// <summary>
        /// 配列への要素（入力値）追加。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="io_Array">配列。</param>
        /// <param name="i_Obj">追加するオブジェクト。</param>
        /// <returns>追加したインデックス</returns>
        public static int AddArray<T>(ref T[] io_Array, T i_Obj)
        {
            // ※Resize時等に例外が発生した場合は、そのまま投げる
            int index = AddArray(ref io_Array);
            io_Array[index] = i_Obj;
            return index;
        }

        /// <summary>
        /// ソフト名+バージョン情報の文字列取得（アセンブリから取得）。
        /// </summary>
        /// <returns>ソフト情報</returns>
        public static string GetProductName()
        {
            // ※例外なし。もし発生する場合はそのまま返す

            // アセンブリから製品名を取得し、バージョン情報(x.xx形式)を付けて返す
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyProductAttribute product =
                (AssemblyProductAttribute)Attribute.GetCustomAttribute(
                    assembly,
                    typeof(AssemblyProductAttribute));
            Version ver = assembly.GetName().Version;

            // 戻り値を返す、ビルド番号・リビジョンは無視
            return product.Product + " Ver" + ver.Major + "." + String.Format("{0:D2}", ver.Minor);
        }

        /// <summary>
        /// 文字列中のファイル名に使用できない文字を置換。
        /// </summary>
        /// <param name="s">ファイル名。</param>
        /// <returns>置換後の文字列</returns>
        public static string ReplaceInvalidFileNameChars(string s)
        {
            // 渡された文字列にファイル名に使えない文字が含まれている場合、_ に置き換える
            string result = s;
            char[] unuseChars = Path.GetInvalidFileNameChars();
            foreach (char c in unuseChars)
            {
                result = result.Replace(c, '_');
            }

            return result;
        }

        /// <summary>
        /// オブジェクトのXMLへのシリアライズ。
        /// </summary>
        /// <param name="i_Obj">オブジェクト。</param>
        /// <param name="i_FileName">XMLファイルパス。</param>
        /// <returns><c>true</c> シリアライズ成功</returns>
        public static bool XmlSerialize(object i_Obj, string i_FileName)
        {
            // ※例外は投げない。失敗した場合は全てfalse

            // 設定をシリアライズ化
            System.Diagnostics.Debug.WriteLine("Cmn.Serialize > " + i_FileName + "にシリアライズ");

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
                System.Diagnostics.Debug.WriteLine("Cmn.Serialize > 例外発生：" + e.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// オブジェクトのXMLからのデシリアライズ。
        /// </summary>
        /// <param name="o_Obj">オブジェクト。</param>
        /// <param name="i_Type">オブジェクトの型。</param>
        /// <param name="i_FileName">XMLファイルパス。</param>
        /// <returns><c>true</c> デシリアライズ成功</returns>
        public static bool XmlDeserialize(ref object o_Obj, Type i_Type, string i_FileName)
        {
            // ※例外は投げない。失敗した場合は全てfalse

            // 設定をデシリアライズ化
            System.Diagnostics.Debug.WriteLine("Cmn.Deserialize > " + i_FileName + "からデシリアライズ");

            // 出力値初期化
            o_Obj = null;

            // ↓このgcnewでログ上に例外が出るが、MSDNによれば無視していいらしい・・・
            XmlSerializer serializer = new XmlSerializer(i_Type);
            try
            {
                Stream reader = new FileStream(i_FileName, FileMode.Open, FileAccess.Read);
                try
                {
                    // ※ ほんとはObjectの型にキャストして代入したいのだが、
                    //    方法不明なため、現状は呼び出し元で行う
                    o_Obj = serializer.Deserialize(reader);
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Cmn.Deserialize > 例外発生：" + e.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// 対象の文字列が、渡された文字列のインデックス番目に存在するかをチェック。
        /// </summary>
        /// <param name="i_Text">文字列全体。</param>
        /// <param name="i_Index">チェック位置。</param>
        /// <param name="i_ChkStr">探索する文字列。</param>
        /// <returns><c>true</c> 存在する</returns>
        public static bool ChkTextInnerWith(string i_Text, int i_Index, string i_ChkStr)
        {
            // ※i_Text, i_ChkStrがNULLの場合はそのままNullReferenceExceptionを。
            //   i_Indexがi_Textの範囲外のときはArgumentExceptionを返す

            // 入力値チェック
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

            // 文字列の一致をチェック
            // ※1つ目のifは速度的にこっちの方が速いかなと思ってやっている。要らないかも・・・
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
        /// コンボボックスを確認し、現在の値が一覧に無ければ登録。
        /// </summary>
        /// <param name="io_Box">コンボボックス。</param>
        /// <param name="i_FirstStr">プレフィックス。</param>
        /// <returns><c>true</c> 登録成功</returns>
        public static bool AddComboBoxNewItem(ref ComboBox io_Box, string i_FirstStr)
        {
            // ※io_BoxがNULLの場合などは、NullReferenceException等をそのまま返す

            // System.Diagnostics.Debug.WriteLine("Cmn.AddComboBoxNewItem > " + io_Box.Text + ", " + i_FirstStr);
            if (io_Box.Text != String.Empty)
            {
                // 現在の値がi_FirstStrから始まっていない場合は、i_FirstStrを付けて処理
                string text = io_Box.Text;
                if (i_FirstStr != null && i_FirstStr != String.Empty)
                {
                    if (text.StartsWith(i_FirstStr) == false)
                    {
                        text = i_FirstStr + io_Box.Text;
                    }
                }

                // 現在の値に一致するものがあるかを確認
                if (io_Box.Items.Contains(text) == false)
                {
                    // 存在しない場合、現在の値を一覧に追加
                    io_Box.Items.Add(text);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// コンボボックスを確認し、現在の値が一覧に無ければ登録。
        /// </summary>
        /// <param name="io_Box">コンボボックス。</param>
        /// <returns><c>true</c> 登録成功</returns>
        public static bool AddComboBoxNewItem(ref ComboBox io_Box)
        {
            // 追加文字列無しで、もう一つの関数をコール
            return AddComboBoxNewItem(ref io_Box, String.Empty);
        }

        /// <summary>
        /// コンボボックスを確認し、選択されている値を削除。
        /// </summary>
        /// <param name="io_Box">コンボボックス。</param>
        /// <returns><c>true</c> 削除成功</returns>
        public static bool RemoveComboBoxItem(ref ComboBox io_Box)
        {
            // ※io_BoxがNULLの場合などは、NullReferenceException等をそのまま返す

            // System.Diagnostics.Debug.WriteLine("Cmn.RemoveComboBoxItem > " + io_Box.SelectedIndex.ToString());
            // 選択されているアイテムを削除
            if (io_Box.SelectedIndex != -1)
            {
                io_Box.Items.Remove(io_Box.SelectedItem);
                io_Box.SelectedText = String.Empty;
            }
            else
            {
                // 何も選択されていない場合も、入力された文字列だけは消しておく
                io_Box.Text = String.Empty;
                return false;
            }

            return true;
        }

        // ↓要リソースマネージャーのメンバ

        // 共通ダイアログ（通知／警告／エラー）
        // ※共通ダイアログは、入力された文字列をそのまま表示するものと、入力された文字列
        //   でリソースを取得、フォーマットして表示するものの各2種類

        /// <summary>
        /// 共通通知ダイアログ（入力された文字列を表示）。
        /// </summary>
        /// <param name="msg">メッセージ。</param>
        public virtual void InformationDialog(string msg)
        {
            // 渡された文字列で通知ダイアログを表示
            // ※Resource等がNULLの場合、NullReferenceException等をそのまま返す
            MessageBox.Show(
                msg,
                Resources.InformationTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// 共通通知ダイアログ（入力された文字列でリソースを取得、フォーマットして表示）。
        /// </summary>
        /// <param name="i_Key">リソースのキー。</param>
        /// <param name="i_args">フォーマットの引数。</param>
        public virtual void InformationDialogResource(string i_Key, params object[] i_args)
        {
            // ※ファイルからリソースが読み取れない場合はArgumentExceptionを。
            //   それ以外はNullReferenceException等をそのまま返す

            // キー値が簡略化されている場合、先頭に追加して処理
            string key = (string) i_Key.Clone();
            if (key.StartsWith("InformationMessage_") == false)
            {
                key = "InformationMessage_" + i_Key;
            }

            // .resourcesから指定されたメッセージを読み込み
            string text = this.Resource.GetString(key);
            if (text == null)
            {
                throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
            }

            // 表示用関数をコール
            this.InformationDialog(String.Format(text, i_args));
        }

        /// <summary>
        /// 共通警告ダイアログ（入力された文字列を表示）。
        /// </summary>
        /// <param name="msg">メッセージ。</param>
        public virtual void WarningDialog(string msg)
        {
            // 渡された文字列で警告ダイアログを表示
            // ※Resource等がNULLの場合、NullReferenceException等をそのまま返す
            MessageBox.Show(
                msg,
                Resources.WarningTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 共通警告ダイアログ（入力された文字列でリソースを取得、フォーマットして表示）。
        /// </summary>
        /// <param name="i_Key">リソースのキー。</param>
        /// <param name="i_args">フォーマットの引数。</param>
        public virtual void WarningDialogResource(string i_Key, params object[] i_args)
        {
            // ※ファイルからリソースが読み取れない場合はArgumentExceptionを。
            //   それ以外はNullReferenceException等をそのまま返す

            // キー値が簡略化されている場合、先頭に追加して処理
            string key = (string) i_Key.Clone();
            if (key.StartsWith("WarningMessage_") == false)
            {
                key = "WarningMessage_" + i_Key;
            }

            // .resourcesから指定されたメッセージを読み込み
            string text = this.Resource.GetString(key);
            if (text == null)
            {
                throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
            }

            // 表示用関数をコール
            this.WarningDialog(String.Format(text, i_args));
        }

        /// <summary>
        /// 共通エラーダイアログ（入力された文字列を表示）。
        /// </summary>
        /// <param name="msg">メッセージ。</param>
        public virtual void ErrorDialog(string msg)
        {
            // 渡された文字列でエラーダイアログを表示
            // ※Resource等がNULLの場合、NullReferenceException等をそのまま返す
            MessageBox.Show(
                msg,
                Resources.ErrorTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// 共通エラーダイアログ（入力された文字列でリソースを取得、フォーマットして表示）。
        /// </summary>
        /// <param name="i_Key">リソースのキー。</param>
        /// <param name="i_args">フォーマットの引数。</param>
        public virtual void ErrorDialogResource(string i_Key, params object[] i_args)
        {
            // ※ファイルからリソースが読み取れない場合はArgumentExceptionを。
            //   それ以外はNullReferenceException等をそのまま返す

            // キー値が簡略化されている場合、先頭に追加して処理
            string key = (string) i_Key.Clone();
            if (key.StartsWith("ErrorMessage_") == false)
            {
                key = "ErrorMessage_" + i_Key;
            }

            // .resourcesから指定されたメッセージを読み込み
            string text = this.Resource.GetString(key);
            if (text == null)
            {
                throw new ArgumentException("Resource \"" + key + "\" Not Exist!", "i_Key");
            }

            // 表示用関数をコール
            this.ErrorDialog(String.Format(text, i_args));
        }

        // ↓リソースマネージャーがnullでも動作、その場合はダイアログが出ない

        /// <summary>
        /// フォルダオープン。
        /// </summary>
        /// <param name="i_Path">フォルダのパス。</param>
        /// <param name="i_ShowEnabled"><c>true</c> エラー時にダイアログを表示。</param>
        /// <returns><c>true</c> オープン成功</returns>
        public virtual bool OpenFolder(string i_Path, bool i_ShowEnabled)
        {
            // ※例外は投げない。失敗した場合は全てfalse
            System.Diagnostics.Debug.WriteLine("Cmn.OpenFolder > " + i_Path);

            // 空の場合は除外
            if (String.IsNullOrEmpty(i_Path))
            {
                return false;
            }

            // 対象データのフォルダを開く
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
        /// フォルダオープン（エラー時にダイアログを表示）。
        /// </summary>
        /// <param name="path">フォルダのパス。</param>
        /// <returns><c>true</c> オープン成功</returns>
        public bool OpenFolder(string path)
        {
            return this.OpenFolder(path, true);
        }

        /// <summary>
        /// ファイルオープン。
        /// </summary>
        /// <param name="i_Path">ファイルのパス。</param>
        /// <param name="i_ShowEnabled"><c>true</c> エラー時にダイアログを表示。</param>
        /// <returns><c>true</c> オープン成功</returns>
        public virtual bool OpenFile(string i_Path, bool i_ShowEnabled)
        {
            // ※例外は投げない。失敗した場合は全てfalse
            System.Diagnostics.Debug.WriteLine("Cmn.OpenFile > " + i_Path);

            // 空のセルは除外
            if (String.IsNullOrEmpty(i_Path))
            {
                return false;
            }

            // 対象データを開く
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
        /// ファイルオープン（エラー時にダイアログを表示）。
        /// </summary>
        /// <param name="path">ファイルのパス。</param>
        /// <returns><c>true</c> オープン成功</returns>
        public bool OpenFile(string path)
        {
            return this.OpenFile(path, true);
        }

        /// <summary>
        /// サーバー接続チェック。
        /// </summary>
        /// <param name="i_Server">サーバー名。</param>
        /// <param name="i_ShowEnabled"><c>true</c> エラー時にダイアログを表示。</param>
        /// <returns><c>true</c> 接続成功</returns>
        public virtual bool Ping(string i_Server, bool i_ShowEnabled)
        {
            // ※例外は投げない。失敗した場合は全てfalse

            // サーバー接続チェック
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
        /// サーバー接続チェック（エラー時にダイアログを表示）。
        /// </summary>
        /// <param name="server">サーバー名。</param>
        /// <returns><c>true</c> 接続成功</returns>
        public bool Ping(string server)
        {
            return this.Ping(server, true);
        }

        /// <summary>
        /// DataGridViewのCSVファイルへの出力。
        /// </summary>
        /// <param name="i_View">CSVファイルを出力するDataGridView。</param>
        /// <param name="i_Path">CSVファイルパス。</param>
        /// <param name="i_ShowEnabled"><c>true</c> エラー時にダイアログを表示。</param>
        /// <returns><c>true</c> 出力成功</returns>
        public virtual bool SaveDataGridViewCsv(DataGridView i_View, string i_Path, bool i_ShowEnabled)
        {
            // ※例外は投げない。失敗した場合は全てfalse
            try
            {
                // DataGridViewの表示内容をCSVファイルに出力
                StreamWriter sw = new StreamWriter(i_Path, false, System.Text.Encoding.GetEncoding("Shift-JIS"));
                try
                {
                    // ヘッダー出力
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

                    // 本体出力
                    for (int y = 0; y < i_View.RowCount; y++)
                    {
                        // 表示されていない行は除外
                        if (i_View.Rows[y].Visible == false)
                        {
                            continue;
                        }

                        // 1行ごとに出力
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
        /// DataGridViewのCSVファイルへの出力（エラー時にダイアログを表示）。
        /// </summary>
        /// <param name="i_View">CSVファイルを出力するDataGridView。</param>
        /// <param name="i_Path">CSVファイルパス。</param>
        /// <returns><c>true</c> 出力成功</returns>
        public bool SaveDataGridViewCsv(DataGridView i_View, string i_Path)
        {
            return this.SaveDataGridViewCsv(i_View, i_Path, true);
        }
    }
}
