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
    using Honememo.Utilities;
    using Honememo.Wptscs.Logics;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// Wikipedia翻訳支援ツール主画面のクラスです。
    /// </summary>
    public partial class MainForm : Form
    {
        #region private変数

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

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。初期化メソッド呼び出しのみ。
        /// </summary>
        public MainForm()
        {
            // Windows フォーム デザイナで生成されたコード
            InitializeComponent();
        }

        #endregion

        #region 各イベントのメソッド

        /// <summary>
        /// フォームロード時の処理。初期化。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // 初期化処理
            try
            {
                this.cmnAP = new Honememo.Cmn();
                this.config = Config.GetInstance();
            }
            catch (FileNotFoundException ex)
            {
                // 設定ファイルが見つからない場合
                System.Diagnostics.Debug.WriteLine("MainForm._Load > 初期化中に例外 : " + ex.Message);
                FormUtils.ErrorDialog(
                    Resources.ErrorMessageConfigNotFound,
                    Settings.Default.ConfigurationFile);

                // どうしようもないのでそのまま終了
                this.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                FormUtils.ErrorDialog(
                    Resources.ErrorMessageConfigLordFailed,
                    ex.Message);

                // どうしようもないのでそのまま終了
                this.Close();
            }

            this.transAP = null;
            Control.CheckForIllegalCrossThreadCalls = false;

            // コンボボックス設定
            this.Initialize();

            // 前回の処理状態を復元
            textBoxSaveDirectory.Text = Settings.Default.SaveDirectory;
            comboBoxSource.SelectedText = Settings.Default.LastSelectedSource;
            comboBoxTarget.SelectedText = Settings.Default.LastSelectedTarget;

            // コンボボックス変更時の処理をコール
            this.comboBoxSource_SelectedIndexChanged(sender, e);
            this.comboBoxTarget_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// フォームクローズ時の処理。処理状態を保存。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 現在の作業フォルダ、絞込み文字列を保存
            Settings.Default.SaveDirectory = textBoxSaveDirectory.Text;
            Settings.Default.LastSelectedSource = comboBoxSource.Text;
            Settings.Default.LastSelectedTarget = comboBoxTarget.Text;
            Settings.Default.Save();
        }

        /// <summary>
        /// 翻訳元コンボボックス変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void comboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ラベルに言語名を表示する
            this.labelSource.Text = String.Empty;
            this.linkLabelSourceURL.Text = "http://";
            if (!String.IsNullOrEmpty(comboBoxSource.Text))
            {
                this.comboBoxSource.Text = this.comboBoxSource.Text.Trim().ToLower();
                Language lang = Config.GetInstance().GetLanguage(this.comboBoxSource.Text);
                if (lang != null)
                {
                    // その言語の、ユーザーが使用している言語での表示名を表示
                    // （日本語環境だったら日本語を、英語だったら英語を）
                    this.labelSource.Text = lang.Names[System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName].Name;

                    // サーバーURLの表示
                    Website site = Config.GetInstance().GetWebsite(this.comboBoxSource.Text);
                    if (site == null && Config.GetInstance().Mode == Config.RunMode.Wikipedia)
                    {
                        site = new MediaWiki(this.comboBoxSource.Text);
                    }

                    this.linkLabelSourceURL.Text = site.Location;
                }
            }
        }

        /// <summary>
        /// 翻訳元コンボボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void comboBoxSource_Leave(object sender, EventArgs e)
        {
            // 直接入力された場合の対策、変更字の処理をコール
            this.comboBoxSource_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// リンクラベルのリンククリック時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void linkLabelSourceURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // リンクを開く
            System.Diagnostics.Process.Start(linkLabelSourceURL.Text);
        }

        /// <summary>
        /// 翻訳先コンボボックス変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void comboBoxTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ラベルに言語名を表示する
            labelTarget.Text = String.Empty;
            if (!String.IsNullOrEmpty(comboBoxTarget.Text))
            {
                comboBoxTarget.Text = comboBoxTarget.Text.Trim().ToLower();
                Language lang = Config.GetInstance().GetLanguage(this.comboBoxTarget.Text);
                if (lang != null)
                {
                    // その言語の、ユーザーが使用している言語での表示名を表示
                    // （日本語環境だったら日本語を、英語だったら英語を）
                    labelTarget.Text = lang.Names[System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName].Name;
                }
            }
        }

        /// <summary>
        /// 翻訳先コンボボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void comboBoxTarget_Leave(object sender, EventArgs e)
        {
            // 直接入力された場合の対策、変更字の処理をコール
            this.comboBoxTarget_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// 設定ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void buttonConfig_Click(object sender, EventArgs e)
        {
            // 設定画面を開く
            ConfigForm form = new ConfigForm();
            form.ShowDialog();

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

        /// <summary>
        /// 参照ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
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
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void textBoxSaveDirectory_Leave(object sender, EventArgs e)
        {
            // 空白を削除
            textBoxSaveDirectory.Text = textBoxSaveDirectory.Text.Trim();
        }

        /// <summary>
        /// 実行ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
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
                FormUtils.WarningDialog(Resources.WarningMessage_UnuseSaveDirectory);
                buttonSaveDirectory.Focus();
                return;
            }
            else if (comboBoxSource.Text == String.Empty)
            {
                FormUtils.WarningDialog(Resources.WarningMessage_NotSelectedSource);
                comboBoxSource.Focus();
                return;
            }
            else if (comboBoxTarget.Text == String.Empty)
            {
                FormUtils.WarningDialog(Resources.WarningMessage_NotSelectedTarget);
                comboBoxTarget.Focus();
                return;
            }
            else if (comboBoxSource.Text == comboBoxTarget.Text)
            {
                FormUtils.WarningDialog(Resources.WarningMessage_SourceEqualTarget);
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
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
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
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
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
                        FormUtils.ApplicationName(),
                        DateTime.Now.ToString("F")));

                // 処理結果とログのための出力ファイル名を作成
                string fileName = String.Empty;
                string logName = String.Empty;
                this.MakeFileName(ref fileName, ref logName, textBoxArticle.Text.Trim(), textBoxSaveDirectory.Text);

                // 翻訳支援処理を実行し、結果とログをファイルに出力
                // ※処理対象に応じてTranslateを継承したオブジェクトを生成
                if (this.config.Mode == Config.RunMode.Wikipedia)
                {
                    Website source = this.config.GetWebsite(comboBoxSource.Text) as Website;
                    if (source == null)
                    {
                        source = new MediaWiki(comboBoxSource.Text);
                    }

                    Website target = this.config.GetWebsite(comboBoxTarget.Text) as Website;
                    if (target == null)
                    {
                        target = new MediaWiki(comboBoxTarget.Text);
                    }

                    this.transAP = new TranslateMediaWiki(source as MediaWiki, target as MediaWiki);
                }
                else
                {
                    // 将来の拡張（？）用
                    textBoxLog.AppendText(String.Format(Resources.InformationMessage_DevelopingMethod, "Wikipedia以外の処理"));
                    FormUtils.InformationDialog(Resources.InformationMessage_DevelopingMethod, "Wikipedia以外の処理");
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
                textBoxLog.AppendText("\r\n" + String.Format(Resources.ErrorMessageDevelopmentError, ex.Message, ex.StackTrace) + "\r\n");
                System.Diagnostics.Debug.WriteLine("MainForm.backgroundWorkerRun_DoWork > 想定外のエラー : " + ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// 実行ボタン バックグラウンド処理（終了時）。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void backgroundWorkerRun_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 画面をロック中から解放
            this.Release();
        }

        #endregion

        #region それ以外のメソッド

        /// <summary>
        /// 画面初期化処理。
        /// </summary>
        private void Initialize()
        {
            // もし古いバージョンの設定があればバージョンアップ
            // ※ 互換性がなくなるときはコメントアウトする
            Properties.Settings.Default.Upgrade();

            // コンボボックス設定
            comboBoxSource.Items.Clear();
            comboBoxTarget.Items.Clear();
            if (Config.GetInstance().Configs.ContainsKey(this.config.Mode))
            {
                // 設定ファイルに存在する全言語を選択肢として登録する
                foreach (Website site in Config.GetInstance().Configs[this.config.Mode].Websites)
                {
                    comboBoxSource.Items.Add(site.Language);
                    comboBoxTarget.Items.Add(site.Language);
                }
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
        /// <param name="o_FileName">出力結果ファイル名。</param>
        /// <param name="o_LogName">出力ログファイル名。</param>
        /// <param name="i_Text">出力する結果テキスト。</param>
        /// <param name="i_Dir">出力先ディレクトリ。</param>
        /// <returns><c>true</c> 出力成功</returns>
        private bool MakeFileName(ref string o_FileName, ref string o_LogName, string i_Text, string i_Dir)
        {
            // 出力値初期化
            o_FileName = String.Empty;
            o_LogName = String.Empty;

            // 出力先フォルダに存在しないファイル名（の拡張子より前）を作成
            // ※渡されたWikipedia等の記事名にファイル名に使えない文字が含まれている場合、_ に置き換える
            //   また、ファイル名が重複している場合、xx[0].txtのように連番を付ける
            string fileNameBase = FormUtils.ReplaceInvalidFileNameChars(i_Text);
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
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
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

        #endregion
    }
}