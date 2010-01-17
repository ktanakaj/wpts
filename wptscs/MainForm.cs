// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツール主画面クラスソース</summary>
//
// <copyright file="MainForm.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using Honememo.Wptscs.Logics;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// Wikipedia翻訳支援ツール主画面のクラスです。
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// 共通関数クラスのオブジェクト。
        /// </summary>
        private Honememo.Cmn cmnAP;

        /// <summary>
        /// 各種設定クラスのオブジェクト。
        /// </summary>
        private Config config;

        /// <summary>
        /// 検索支援処理クラスのオブジェクト。
        /// </summary>
        private Translate transAP;

        /// <summary>
        /// 表示済みログ文字列長。
        /// </summary>
        private int logLastLength;

        /// <summary>
        /// コンストラクタ。初期化メソッド呼び出しのみ。
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームロード時の処理。初期化。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // 初期化処理
            try
            {
                this.cmnAP = new Honememo.Cmn();
                this.config = new Config(Path.Combine(Application.StartupPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".xml"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MainForm._Load > 初期化中に例外 : " + ex.Message);
            }

            this.transAP = null;
            Control.CheckForIllegalCrossThreadCalls = false;

            // コンボボックス設定
            this.Initialize();

            // 前回の処理状態を復元
            textBoxSaveDirectory.Text = this.config.Client.SaveDirectory;
            comboBoxSource.SelectedText = this.config.Client.LastSelectedSource;
            comboBoxTarget.SelectedText = this.config.Client.LastSelectedTarget;

            // コンボボックス変更時の処理をコール
            this.comboBoxSource_SelectedIndexChanged(sender, e);
            this.comboBoxTarget_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// フォームクローズ時の処理。処理状態を保存。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 複数立ち上げの場合、言語設定が更新されている可能性があるので、設定を再読み込み
            try
            {
                this.config.Load();

                // 現在の作業フォルダ、絞込み文字列を保存
                this.config.Client.SaveDirectory = textBoxSaveDirectory.Text;
                this.config.Client.LastSelectedSource = comboBoxSource.Text;
                this.config.Client.LastSelectedTarget = comboBoxTarget.Text;
                this.config.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MainForm._FormClosed > 設定保存中に例外 : " + ex.Message);
            }

            // キャッシュフォルダの古いファイルのクリア
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Application.ExecutablePath)));
                if (dir.Exists == true)
                {
                    FileInfo[] files = dir.GetFiles("*.xml");
                    foreach (FileInfo file in files)
                    {
                        // 1週間以上前のキャッシュは削除
                        if ((DateTime.UtcNow - file.LastWriteTimeUtc) > new TimeSpan(7, 0, 0, 0))
                        {
                            // 万が一消せなかったら、無視して次のファイルへ
                            try
                            {
                                file.Delete();
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("MainForm._FormClosed > キャッシュ削除時に例外 : " + ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MainForm._FormClosed > キャッシュ削除中に例外 : " + ex.Message);
            }
        }

        /// <summary>
        /// 翻訳元コンボボックス変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void comboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ラベルに言語名を表示する
            if (comboBoxSource.Text != String.Empty)
            {
                comboBoxSource.Text = comboBoxSource.Text.Trim().ToLower();
                LanguageInformation lang = this.config.GetLanguage(this.comboBoxSource.Text);
                if (lang != null)
                {
                    labelSource.Text = lang.GetName(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                }
                else
                {
                    labelSource.Text = String.Empty;
                }

                // サーバーURLの表示
                if (this.config.Client.RunMode == Config.RunType.Wikipedia)
                {
                    WikipediaInformation svr = new WikipediaInformation();
                    if (lang != null)
                    {
                        svr = lang as WikipediaInformation;
                    }

                    if (svr == null)
                    {
                        svr = new WikipediaInformation(comboBoxSource.Text);
                    }

                    linkLabelSourceURL.Text = "http://" + svr.Server;
                }
                else
                {
                    // 将来の拡張（？）用
                    linkLabelSourceURL.Text = "http://";
                }
            }
            else
            {
                labelSource.Text = String.Empty;
                linkLabelSourceURL.Text = "http://";
            }
        }

        /// <summary>
        /// 翻訳元コンボボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void comboBoxSource_Leave(object sender, EventArgs e)
        {
            // 直接入力された場合の対策、変更字の処理をコール
            this.comboBoxSource_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// リンクラベルのリンククリック時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void linkLabelSourceURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // リンクを開く
            System.Diagnostics.Process.Start(linkLabelSourceURL.Text);
        }

        /// <summary>
        /// 翻訳先コンボボックス変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void comboBoxTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ラベルに言語名を表示する
            if (comboBoxTarget.Text != String.Empty)
            {
                comboBoxTarget.Text = comboBoxTarget.Text.Trim().ToLower();
                LanguageInformation lang = this.config.GetLanguage(comboBoxTarget.Text);
                if (lang != null)
                {
                    labelTarget.Text = lang.GetName(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                }
                else
                {
                    labelTarget.Text = String.Empty;
                }
            }
            else
            {
                labelTarget.Text = String.Empty;
            }
        }

        /// <summary>
        /// 翻訳先コンボボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void comboBoxTarget_Leave(object sender, EventArgs e)
        {
            // 直接入力された場合の対策、変更字の処理をコール
            this.comboBoxTarget_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// 設定ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void buttonConfig_Click(object sender, EventArgs e)
        {
            if (this.config.Client.RunMode == Config.RunType.Wikipedia)
            {
                // 言語の設定画面を開く
                ConfigWikipediaDialog dialog = new ConfigWikipediaDialog();
                dialog.ShowDialog();

                // 結果に関係なく、設定を再読み込み
                this.config.Load();

                // コンボボックス設定
                string backupSourceSelected = comboBoxSource.SelectedText;
                string backupSourceTarget = comboBoxTarget.SelectedText;
                this.Initialize();
                comboBoxSource.SelectedText = backupSourceSelected;
                comboBoxTarget.SelectedText = backupSourceTarget;

                // コンボボックス変更時の処理をコール
                this.comboBoxSource_SelectedIndexChanged(sender, e);
                this.comboBoxTarget_SelectedIndexChanged(sender, e);
            }
            else
            {
                // 将来の拡張（？）用
                this.cmnAP.InformationDialogResource("InformationMessage_DevelopingMethod", "Wikipedia以外の処理");
            }
        }

        /// <summary>
        /// 参照ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void buttonSaveDirectory_Click(object sender, EventArgs e)
        {
            // フォルダ名が入力されている場合、それを初期位置に設定
            if (textBoxSaveDirectory.Text != String.Empty)
            {
                folderBrowserDialogSaveDirectory.SelectedPath = textBoxSaveDirectory.Text;
            }

            // フォルダ選択画面をオープン
            if (folderBrowserDialogSaveDirectory.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // フォルダが選択された場合、フォルダ名に選択されたフォルダを設定
                textBoxSaveDirectory.Text = folderBrowserDialogSaveDirectory.SelectedPath;
            }
        }

        /// <summary>
        /// 出力先テキストボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void textBoxSaveDirectory_Leave(object sender, EventArgs e)
        {
            // 空白を削除
            textBoxSaveDirectory.Text = textBoxSaveDirectory.Text.Trim();
        }

        /// <summary>
        /// 実行ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void buttonRun_Click(object sender, EventArgs e)
        {
            // 入力値チェック
            if (textBoxArticle.Text.Trim() == String.Empty)
            {
                // 値が設定されていないときは処理無し
                return;
            }

            // 必要な情報が設定されていない場合は処理不可
            if (Directory.Exists(textBoxSaveDirectory.Text) == false)
            {
                this.cmnAP.WarningDialogResource("WarningMessage_UnuseSaveDirectory");
                buttonSaveDirectory.Focus();
                return;
            }
            else if (comboBoxSource.Text == String.Empty)
            {
                this.cmnAP.WarningDialogResource("WarningMessage_NotSelectedSource");
                comboBoxSource.Focus();
                return;
            }
            else if (comboBoxTarget.Text == String.Empty)
            {
                this.cmnAP.WarningDialogResource("WarningMessage_NotSelectedTarget");
                comboBoxTarget.Focus();
                return;
            }
            else if (comboBoxSource.Text == comboBoxTarget.Text)
            {
                this.cmnAP.WarningDialogResource("WarningMessage_SourceEqualTarget");
                comboBoxTarget.Focus();
                return;
            }

            // 画面をロック
            this.LockOperation();

            // バックグラウンド処理を実行
            backgroundWorkerRun.RunWorkerAsync();
        }

        /// <summary>
        /// 中止ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            // 処理を中断
            buttonStop.Enabled = false;
            if (backgroundWorkerRun.IsBusy == true)
            {
                System.Diagnostics.Debug.WriteLine("MainForm.-Stop_Click > 処理中断");
                backgroundWorkerRun.CancelAsync();
                if (this.transAP != null)
                {
                    this.transAP.CancellationPending = true;
                }
            }
        }

        /// <summary>
        /// 実行ボタン バックグラウンド処理（スレッド）。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void backgroundWorkerRun_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // 翻訳支援処理の前処理
                textBoxLog.Clear();
                this.logLastLength = 0;
                textBoxLog.AppendText(
                    String.Format(
                        Resources.LogMessage_Start,
                        Honememo.Cmn.GetProductName(),
                        DateTime.Now.ToString("F")));

                // 処理結果とログのための出力ファイル名を作成
                string fileName = String.Empty;
                string logName = String.Empty;
                this.MakeFileName(ref fileName, ref logName, textBoxArticle.Text.Trim(), textBoxSaveDirectory.Text);

                // 翻訳支援処理を実行し、結果とログをファイルに出力
                // ※処理対象に応じてTranslateを継承したオブジェクトを生成
                if (this.config.Client.RunMode == Config.RunType.Wikipedia)
                {
                    WikipediaInformation source = this.config.GetLanguage(comboBoxSource.Text) as WikipediaInformation;
                    if (source == null)
                    {
                        source = new WikipediaInformation(comboBoxSource.Text);
                    }

                    WikipediaInformation target = this.config.GetLanguage(comboBoxTarget.Text) as WikipediaInformation;
                    if (target == null)
                    {
                        target = new WikipediaInformation(comboBoxTarget.Text);
                    }

                    this.transAP = new TranslateWikipedia(source, target);
                    ((TranslateWikipedia)this.transAP).UserAgent = this.config.Client.UserAgent;
                    ((TranslateWikipedia)this.transAP).Referer = this.config.Client.Referer;
                }
                else
                {
                    // 将来の拡張（？）用
                    textBoxLog.AppendText(String.Format(Resources.InformationMessage_DevelopingMethod, "Wikipedia以外の処理"));
                    this.cmnAP.InformationDialogResource("InformationMessage_DevelopingMethod", "Wikipedia以外の処理");
                    return;
                }

                this.transAP.LogUpdate += new EventHandler(this.GetLogUpdate);

                // 実行前に、ユーザーから中止要求がされているかをチェック
                if (backgroundWorkerRun.CancellationPending == true)
                {
                    textBoxLog.AppendText(String.Format(Resources.LogMessage_Stop, logName));
                }
                else
                {
                    // 翻訳支援処理を実行
                    bool successFlag = this.transAP.Run(textBoxArticle.Text.Trim());

                    // 処理に時間がかかるため、出力ファイル名を再確認
                    this.MakeFileName(ref fileName, ref logName, textBoxArticle.Text.Trim(), textBoxSaveDirectory.Text);
                    if (successFlag)
                    {
                        // 処理結果を出力
                        try
                        {
                            StreamWriter sw = new StreamWriter(Path.Combine(textBoxSaveDirectory.Text, fileName));
                            try
                            {
                                sw.Write(this.transAP.Text);
                                textBoxLog.AppendText(String.Format(Resources.LogMessage_End, fileName, logName));
                            }
                            finally
                            {
                                sw.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            textBoxLog.AppendText(String.Format(Resources.LogMessage_ErrorFileSave, Path.Combine(textBoxSaveDirectory.Text, fileName), ex.Message));
                            textBoxLog.AppendText(String.Format(Resources.LogMessage_Stop, logName));
                        }
                    }
                    else
                    {
                        textBoxLog.AppendText(String.Format(Resources.LogMessage_Stop, logName));
                    }
                }

                // ログを出力
                try
                {
                    StreamWriter sw = new StreamWriter(Path.Combine(textBoxSaveDirectory.Text, logName));
                    try
                    {
                        sw.Write(textBoxLog.Text);
                    }
                    finally
                    {
                        sw.Close();
                    }
                }
                catch (Exception ex)
                {
                    textBoxLog.AppendText(String.Format(Resources.LogMessage_ErrorFileSave, Path.Combine(textBoxSaveDirectory.Text, logName), ex.Message));
                }
            }
            catch (Exception ex)
            {
                textBoxLog.AppendText("\r\n" + String.Format(Resources.ErrorMessage_DevelopmentMiss, ex.Message) + "\r\n");
            }
        }

        /// <summary>
        /// 実行ボタン バックグラウンド処理（終了時）。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void backgroundWorkerRun_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 画面をロック中から解放
            this.Release();
        }

        /// <summary>
        /// 画面初期化処理。
        /// </summary>
        private void Initialize()
        {
            // コンボボックス設定
            comboBoxSource.Items.Clear();
            comboBoxTarget.Items.Clear();
            foreach (LanguageInformation lang in this.config.Languages)
            {
                comboBoxSource.Items.Add(lang.Code);
                comboBoxTarget.Items.Add(lang.Code);
            }
        }

        /// <summary>
        /// 画面をロック中に移行。
        /// </summary>
        private void LockOperation()
        {
            // 各種ボタンなどを入力不可に変更
            groupBoxTransfer.Enabled = false;
            groupBoxSaveDirectory.Enabled = false;
            textBoxArticle.Enabled = false;
            buttonRun.Enabled = false;

            // 中止ボタンを有効に変更
            buttonStop.Enabled = true;
        }

        /// <summary>
        /// 画面をロック中から解放。
        /// </summary>
        private void Release()
        {
            // 中止ボタンを入力不可に変更
            buttonStop.Enabled = false;

            // 各種ボタンなどを有効に変更
            groupBoxTransfer.Enabled = true;
            groupBoxSaveDirectory.Enabled = true;
            textBoxArticle.Enabled = true;
            buttonRun.Enabled = true;
        }

        /// <summary>
        /// 渡された文字列から.txtと.logの重複していないファイル名を作成。
        /// </summary>
        /// <param name="o_FileName">出力結果ファイル名</param>
        /// <param name="o_LogName">出力ログファイル名</param>
        /// <param name="i_Text">出力する結果テキスト</param>
        /// <param name="i_Dir">出力先ディレクトリ</param>
        /// <returns><c>true</c> 出力成功</returns>
        private bool MakeFileName(ref string o_FileName, ref string o_LogName, string i_Text, string i_Dir)
        {
            // 出力値初期化
            o_FileName = String.Empty;
            o_LogName = String.Empty;

            // 出力先フォルダに存在しないファイル名（の拡張子より前）を作成
            // ※渡されたWikipedia等の記事名にファイル名に使えない文字が含まれている場合、_ に置き換える
            //   また、ファイル名が重複している場合、xx[0].txtのように連番を付ける
            string fileNameBase = Honememo.Cmn.ReplaceInvalidFileNameChars(i_Text);
            string fileName = fileNameBase + ".txt";
            string logName = fileNameBase + ".log";
            bool successFlag = false;
            for (int i = 0; i < 100000; i++)
            {
                // ※100000まで試して空きが見つからないことは無いはず、もし見つからなかったら最後のを上書き
                if (File.Exists(Path.Combine(i_Dir, fileName)) == false
                    && File.Exists(Path.Combine(i_Dir, logName)) == false)
                {
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

        /// <summary>
        /// 翻訳支援処理クラスのイベント用。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void GetLogUpdate(object sender, System.EventArgs e)
        {
            // 前回以降に追加されたログをテキストボックスに出力
            int length = this.transAP.Log.Length;
            if (length > this.logLastLength)
            {
                textBoxLog.AppendText(this.transAP.Log.Substring(this.logLastLength, length - this.logLastLength));
            }

            this.logLastLength = length;
        }
    }
}