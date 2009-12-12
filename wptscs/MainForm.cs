using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using wptscs.Properties;
using wptscs.logic;
using wptscs.model;

namespace wptscs
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

		// 共通関数クラスのオブジェクト
		private MYAPP.Cmn cmnAP;
		// 各種設定クラスのオブジェクト
		private Config config;
		// 検索支援処理クラスのオブジェクト
		private Translate transAP;
		// 表示済みログ文字列長
		private int logLastLength;

        /* 初期化 */
        private void MainForm_Load(object sender, EventArgs e)
        {
	        // 初期化処理
            try
            {
                cmnAP = new MYAPP.Cmn();
                config = new Config(Path.Combine(Application.StartupPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".xml"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MainForm._Load > 初期化中に例外 : " + ex.Message);
            }
	        transAP = null;
            Control.CheckForIllegalCrossThreadCalls = false;

	        // コンボボックス設定
	        initialize();

	        // 前回の処理状態を復元
	        textBoxSaveDirectory.Text = config.Client.SaveDirectory;
	        comboBoxSource.SelectedText = config.Client.LastSelectedSource;
	        comboBoxTarget.SelectedText = config.Client.LastSelectedTarget;
	        // コンボボックス変更時の処理をコール
	        comboBoxSource_SelectedIndexChanged(sender, e);
	        comboBoxTarget_SelectedIndexChanged(sender, e);
        }

        /* 終了処理、処理状態を保存 */
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
	        // 複数立ち上げの場合、言語設定が更新されている可能性があるので、設定を再読み込み
	        try{
                config.Load();
                // 現在の作業フォルダ、絞込み文字列を保存
                config.Client.SaveDirectory = textBoxSaveDirectory.Text;
                config.Client.LastSelectedSource = comboBoxSource.Text;
                config.Client.LastSelectedTarget = comboBoxTarget.Text;
                config.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MainForm._FormClosed > 設定保存中に例外 : " + ex.Message);
            }
	        // キャッシュフォルダの古いファイルのクリア
	        try{
		        DirectoryInfo dir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Application.ExecutablePath)));
		        if(dir.Exists == true){
			        FileInfo[] files = dir.GetFiles("*.xml");
			        foreach(FileInfo file in files){
				        // 1週間以上前のキャッシュは削除
				        if((DateTime.UtcNow - file.LastWriteTimeUtc) > new TimeSpan(7, 0, 0, 0)){
					        // 万が一消せなかったら、無視して次のファイルへ
					        try{
						        file.Delete();
					        }
					        catch(Exception ex){
						        System.Diagnostics.Debug.WriteLine("MainForm._FormClosed > キャッシュ削除時に例外 : " + ex.Message);
					        }
				        }
			        }
		        }
	        }
	        catch(Exception ex){
		        System.Diagnostics.Debug.WriteLine("MainForm._FormClosed > キャッシュ削除中に例外 : " + ex.Message);
	        }
        }

        /* 翻訳元コンボボックスの変更 */
        private void comboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
	        // ラベルに言語名を表示する
	        if(comboBoxSource.Text != ""){
		        comboBoxSource.Text = comboBoxSource.Text.Trim().ToLower();
		        LanguageInformation lang = config.GetLanguage(comboBoxSource.Text);
		        if(lang != null){
			        labelSource.Text = lang.GetName(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
		        }
		        else{
			        labelSource.Text = "";
		        }
		        // サーバーURLの表示
		        if(config.Client.RunMode == Config.RunType.Wikipedia){
			        WikipediaInformation svr = new WikipediaInformation();
			        if(lang != null){
                        svr = lang as WikipediaInformation;
			        }
			        if(svr == null){
				        svr = new WikipediaInformation(comboBoxSource.Text);
			        }
			        linkLabelSourceURL.Text = "http://" + svr.Server;
		        }
		        else{
			        // 将来の拡張（？）用
			        linkLabelSourceURL.Text = "http://";
		        }
	        }
	        else{
		        labelSource.Text = "";
		        linkLabelSourceURL.Text = "http://";
	        }
        }

        /* 翻訳元コンボボックスのフォーカス喪失 */
        private void comboBoxSource_Leave(object sender, EventArgs e)
        {
            // 直接入力された場合の対策、変更字の処理をコール
            comboBoxSource_SelectedIndexChanged(sender, e);
        }

        /* リンクラベルのリンククリック */
        private void linkLabelSourceURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
	        // リンクを開く
	        System.Diagnostics.Process.Start(linkLabelSourceURL.Text);
        }

        /* 翻訳先コンボボックスの変更 */
        private void comboBoxTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
	        // ラベルに言語名を表示する
	        if(comboBoxTarget.Text != ""){
		        comboBoxTarget.Text = comboBoxTarget.Text.Trim().ToLower();
		        LanguageInformation lang = config.GetLanguage(comboBoxTarget.Text);
		        if(lang != null){
			        labelTarget.Text = lang.GetName(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
		        }
		        else{
			        labelTarget.Text = "";
		        }
	        }
	        else{
		        labelTarget.Text = "";
	        }
        }

        /* 翻訳先コンボボックスのフォーカス喪失 */
        private void comboBoxTarget_Leave(object sender, EventArgs e)
        {
            // 直接入力された場合の対策、変更字の処理をコール
            comboBoxTarget_SelectedIndexChanged(sender, e);
        }

        /* 設定ボタン押下 */
        private void buttonConfig_Click(object sender, EventArgs e)
        {
	        if(config.Client.RunMode == Config.RunType.Wikipedia){
		        // 言語の設定画面を開く
		        ConfigWikipediaDialog dialog = new ConfigWikipediaDialog();
		        dialog.ShowDialog();
		        // 結果に関係なく、設定を再読み込み
		        config.Load();
		        // コンボボックス設定
		        String backupSourceSelected = comboBoxSource.SelectedText;
		        String backupSourceTarget = comboBoxTarget.SelectedText;
		        initialize();
		        comboBoxSource.SelectedText = backupSourceSelected;
		        comboBoxTarget.SelectedText = backupSourceTarget;
		        // コンボボックス変更時の処理をコール
		        comboBoxSource_SelectedIndexChanged(sender, e);
		        comboBoxTarget_SelectedIndexChanged(sender, e);
	        }
	        else{
		        // 将来の拡張（？）用
		        cmnAP.InformationDialogResource("InformationMessage_DevelopingMethod", "Wikipedia以外の処理");
	        }
        }

        /* 参照ボタン押下 */
        private void buttonSaveDirectory_Click(object sender, EventArgs e)
        {
	        // フォルダ名が入力されている場合、それを初期位置に設定
	        if(textBoxSaveDirectory.Text != ""){
		        folderBrowserDialogSaveDirectory.SelectedPath = textBoxSaveDirectory.Text;
	        }
	        // フォルダ選択画面をオープン
	        if(folderBrowserDialogSaveDirectory.ShowDialog() == System.Windows.Forms.DialogResult.OK){
		        // フォルダが選択された場合、フォルダ名に選択されたフォルダを設定
		        textBoxSaveDirectory.Text = folderBrowserDialogSaveDirectory.SelectedPath;
	        }
                }

                /* 出力先テキストボックスのフォーカス喪失 */
                private void textBoxSaveDirectory_Leave(object sender, EventArgs e)
                {
                    // 空白を削除
                    textBoxSaveDirectory.Text = textBoxSaveDirectory.Text.Trim();
                }

                /* 実行ボタン押下 */
                private void buttonRun_Click(object sender, EventArgs e)
                {
	        // 入力値チェック
	        if(textBoxArticle.Text.Trim() == ""){
		        // 値が設定されていないときは処理無し
		        return;
	        }
	        // 必要な情報が設定されていない場合は処理不可
	        if(Directory.Exists(textBoxSaveDirectory.Text) == false){
		        cmnAP.WarningDialogResource("WarningMessage_UnuseSaveDirectory");
		        buttonSaveDirectory.Focus();
		        return;
	        }
	        else if(comboBoxSource.Text == ""){
		        cmnAP.WarningDialogResource("WarningMessage_NotSelectedSource");
		        comboBoxSource.Focus();
		        return;
	        }
	        else if(comboBoxTarget.Text == ""){
		        cmnAP.WarningDialogResource("WarningMessage_NotSelectedTarget");
		        comboBoxTarget.Focus();
		        return;
	        }
	        else if(comboBoxSource.Text == comboBoxTarget.Text){
		        cmnAP.WarningDialogResource("WarningMessage_SourceEqualTarget");
		        comboBoxTarget.Focus();
		        return;
	        }
	        // 画面をロック
	        lockOperation();
	        // バックグラウンド処理を実行
	        backgroundWorkerRun.RunWorkerAsync();
        }

        /* 中止ボタン押下 */
        private void buttonStop_Click(object sender, EventArgs e)
        {
	        // 処理を中断
	        buttonStop.Enabled = false;
	        if(backgroundWorkerRun.IsBusy == true){
		        System.Diagnostics.Debug.WriteLine("MainForm.-Stop_Click > 処理中断");
		        backgroundWorkerRun.CancelAsync();
		        if(transAP != null){
			        transAP.CancellationPending = true;
		        }
	        }
        }

        /* 実行ボタン バックグラウンド処理（スレッド） */
        private void backgroundWorkerRun_DoWork(object sender, DoWorkEventArgs e)
        {
	        try{
		        // 翻訳支援処理の前処理
		        textBoxLog.Clear();
		        logLastLength = 0;
		        textBoxLog.AppendText(String.Format(Resources.LogMessage_Start, MYAPP.Cmn.GetProductName(),
			        DateTime.Now.ToString("F")));
		        // 処理結果とログのための出力ファイル名を作成
		        String fileName = "";
		        String logName = "";
		        makeFileName(ref fileName, ref logName, textBoxArticle.Text.Trim(), textBoxSaveDirectory.Text);

		        // 翻訳支援処理を実行し、結果とログをファイルに出力
		        // ※処理対象に応じてTranslateを継承したオブジェクトを生成
		        if(config.Client.RunMode == Config.RunType.Wikipedia){
                    WikipediaInformation source = config.GetLanguage(comboBoxSource.Text) as WikipediaInformation;
			        if(source == null){
				        source = new WikipediaInformation(comboBoxSource.Text);
			        }
                    WikipediaInformation target = config.GetLanguage(comboBoxTarget.Text) as WikipediaInformation;
			        if(target == null){
				        target = new WikipediaInformation(comboBoxTarget.Text);
			        }
			        transAP = new TranslateWikipedia(source, target);
			        ((TranslateWikipedia) transAP).UserAgent = config.Client.UserAgent;
			        ((TranslateWikipedia) transAP).Referer = config.Client.Referer;
		        }
		        else{
			        // 将来の拡張（？）用
			        textBoxLog.AppendText(String.Format(Resources.InformationMessage_DevelopingMethod, "Wikipedia以外の処理"));
			        cmnAP.InformationDialogResource("InformationMessage_DevelopingMethod", "Wikipedia以外の処理");
			        return;
		        }
		        transAP.LogUpdate += new EventHandler(this.getLogUpdate);
		        // 実行前に、ユーザーから中止要求がされているかをチェック
		        if(backgroundWorkerRun.CancellationPending == true){
			        textBoxLog.AppendText(String.Format(Resources.LogMessage_Stop, logName));
		        }
		        // 翻訳支援処理を実行
		        else{
			        bool successFlag = transAP.Run(textBoxArticle.Text.Trim());
			        // 処理に時間がかかるため、出力ファイル名を再確認
			        makeFileName(ref fileName, ref logName, textBoxArticle.Text.Trim(), textBoxSaveDirectory.Text);
			        if(successFlag){
				        // 処理結果を出力
				        try{
					        StreamWriter sw = new StreamWriter(Path.Combine(textBoxSaveDirectory.Text, fileName));
					        try{
						        sw.Write(transAP.Text);
						        textBoxLog.AppendText(String.Format(Resources.LogMessage_End, fileName, logName));
					        }
					        finally{
						        sw.Close();
					        }
				        }
				        catch(Exception ex){
					        textBoxLog.AppendText(String.Format(Resources.LogMessage_ErrorFileSave, Path.Combine(textBoxSaveDirectory.Text, fileName), ex.Message));
					        textBoxLog.AppendText(String.Format(Resources.LogMessage_Stop, logName));
				        }
			        }
			        else{
				        textBoxLog.AppendText(String.Format(Resources.LogMessage_Stop, logName));
			        }
		        }
		        // ログを出力
		        try{
			        StreamWriter sw = new StreamWriter(Path.Combine(textBoxSaveDirectory.Text, logName));
			        try{
				        sw.Write(textBoxLog.Text);
			        }
			        finally{
				        sw.Close();
			        }
		        }
		        catch(Exception ex){
			        textBoxLog.AppendText(String.Format(Resources.LogMessage_ErrorFileSave, Path.Combine(textBoxSaveDirectory.Text, logName), ex.Message));
		        }
        	}
        	catch(Exception ex){
        		textBoxLog.AppendText("\r\n" + String.Format(Resources.ErrorMessage_DevelopmentMiss, ex.Message) + "\r\n");
        	}
        }

        /* 実行ボタン バックグラウンド処理（終了時） */
        private void backgroundWorkerRun_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
	        // 画面をロック中から解放
	        release();
        }

    	// 画面初期化処理
		private void initialize()
        {
	        // コンボボックス設定
	        comboBoxSource.Items.Clear();
	        comboBoxTarget.Items.Clear();
	        foreach(LanguageInformation lang in config.Languages){
		        comboBoxSource.Items.Add(lang.Code);
		        comboBoxTarget.Items.Add(lang.Code);
	        }
	        return;
        }

        // 画面をロック中に移行
        private void lockOperation()
        {
	        // 各種ボタンなどを入力不可に変更
	        groupBoxTransfer.Enabled = false;
	        groupBoxSaveDirectory.Enabled = false;
	        textBoxArticle.Enabled = false;
	        buttonRun.Enabled = false;
	        // 中止ボタンを有効に変更
	        buttonStop.Enabled = true;
        }

        // 画面をロック中から解放
        private void release()
        {
	        // 中止ボタンを入力不可に変更
	        buttonStop.Enabled = false;
	        // 各種ボタンなどを有効に変更
	        groupBoxTransfer.Enabled = true;
	        groupBoxSaveDirectory.Enabled = true;
	        textBoxArticle.Enabled = true;
	        buttonRun.Enabled = true;
        }

        // 渡された文字列から.txtと.logの重複していないファイル名を作成
        private bool makeFileName(ref String o_FileName, ref String o_LogName, String i_Text, String i_Dir)
        {
	        // 出力値初期化
	        o_FileName = "";
	        o_LogName = "";
	        // 出力先フォルダに存在しないファイル名（の拡張子より前）を作成
	        // ※渡されたWikipedia等の記事名にファイル名に使えない文字が含まれている場合、_ に置き換える
	        //   また、ファイル名が重複している場合、xx[0].txtのように連番を付ける
	        String fileNameBase = MYAPP.Cmn.ReplaceInvalidFileNameChars(i_Text);
	        String fileName = fileNameBase + ".txt";
	        String logName = fileNameBase + ".log";
	        bool successFlag = false;
	        for(int i = 0 ; i < 100000 ; i++){
		        // ※100000まで試して空きが見つからないことは無いはず、もし見つからなかったら最後のを上書き
		        if(File.Exists(Path.Combine(i_Dir, fileName)) == false &&
		           File.Exists(Path.Combine(i_Dir, logName)) == false){
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

		// 翻訳支援処理クラスのイベント用
		private void getLogUpdate(System.Object sender, System.EventArgs  e)
        {
        	// 前回以降に追加されたログをテキストボックスに出力
        	int length = transAP.Log.Length;
        	if(length > logLastLength){
        		textBoxLog.AppendText(transAP.Log.Substring(logLastLength, length - logLastLength));
	        }
	        logLastLength = length;
        }
    }
}