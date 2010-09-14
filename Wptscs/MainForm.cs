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
        /// 現在読み込んでいるアプリケーションの設定。
        /// </summary>
        private Config config;

        /// <summary>
        /// 検索支援処理クラスのオブジェクト。
        /// </summary>
        private Translator translate;

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
            this.InitializeComponent();
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
            // 設定ファイルの読み込み
            try
            {
                this.config = Config.GetInstance(Settings.Default.ConfigurationFile);
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

            this.translate = null;
            Control.CheckForIllegalCrossThreadCalls = false;

            // コンボボックス設定
            this.Initialize();

            // 前回の処理状態を復元
            this.textBoxSaveDirectory.Text = Settings.Default.SaveDirectory;
            this.comboBoxSource.SelectedText = Settings.Default.LastSelectedSource;
            this.comboBoxTarget.SelectedText = Settings.Default.LastSelectedTarget;

            // コンボボックス変更時の処理をコール
            this.ComboBoxSource_SelectedIndexChanged(sender, e);
            this.ComboBoxTarget_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// フォームクローズ時の処理。処理状態を保存。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 現在の作業フォルダ、絞込み文字列を保存
            Settings.Default.SaveDirectory = this.textBoxSaveDirectory.Text;
            Settings.Default.LastSelectedSource = this.comboBoxSource.Text;
            Settings.Default.LastSelectedTarget = this.comboBoxTarget.Text;
            Settings.Default.Save();
        }

        /// <summary>
        /// 翻訳元コンボボックス変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ComboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ラベルに言語名を表示する
            this.labelSource.Text = String.Empty;
            this.linkLabelSourceURL.Text = "http://";
            if (!String.IsNullOrEmpty(this.comboBoxSource.Text))
            {
                this.comboBoxSource.Text = this.comboBoxSource.Text.Trim().ToLower();

                // その言語の、ユーザーが使用している言語での表示名を表示
                // （日本語環境だったら日本語を、英語だったら英語を）
                Language.LanguageName name;
                this.labelSource.Text = String.Empty;
                if (this.config.GetWebsite(this.comboBoxSource.Text) != null &&
                    this.config.GetWebsite(this.comboBoxSource.Text).Language.Names.TryGetValue(
                    System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                    out name))
                {
                    this.labelSource.Text = name.Name;
                }

                // サーバーURLの表示
                this.linkLabelSourceURL.Text = this.config.GetWebsite(
                    this.comboBoxSource.Text).Location;
            }
        }

        /// <summary>
        /// 翻訳元コンボボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ComboBoxSource_Leave(object sender, EventArgs e)
        {
            // 直接入力された場合の対策、変更字の処理をコール
            this.ComboBoxSource_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// リンクラベルのリンククリック時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void LinkLabelSourceURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // リンクを開く
            System.Diagnostics.Process.Start(this.linkLabelSourceURL.Text);
        }

        /// <summary>
        /// 翻訳先コンボボックス変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ComboBoxTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ラベルに言語名を表示する
            this.labelTarget.Text = String.Empty;
            if (!String.IsNullOrEmpty(this.comboBoxTarget.Text))
            {
                this.comboBoxTarget.Text = this.comboBoxTarget.Text.Trim().ToLower();

                // その言語の、ユーザーが使用している言語での表示名を表示
                // （日本語環境だったら日本語を、英語だったら英語を）
                if (this.config.GetWebsite(
                    this.comboBoxTarget.Text) != null)
                {
                    this.labelTarget.Text = this.config.GetWebsite(
                        this.comboBoxTarget.Text).Language.Names[System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName].Name;
                }
            }
        }

        /// <summary>
        /// 翻訳先コンボボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ComboBoxTarget_Leave(object sender, EventArgs e)
        {
            // 直接入力された場合の対策、変更字の処理をコール
            this.ComboBoxTarget_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// 設定ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonConfig_Click(object sender, EventArgs e)
        {
            // 設定画面を開く
            ConfigForm form = new ConfigForm();
            form.ShowDialog();

            // コンボボックス設定
            string backupSourceSelected = this.comboBoxSource.SelectedText;
            string backupSourceTarget = this.comboBoxTarget.SelectedText;
            this.Initialize();
            this.comboBoxSource.SelectedText = backupSourceSelected;
            this.comboBoxTarget.SelectedText = backupSourceTarget;

            // コンボボックス変更時の処理をコール
            this.ComboBoxSource_SelectedIndexChanged(sender, e);
            this.ComboBoxTarget_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// 参照ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonSaveDirectory_Click(object sender, EventArgs e)
        {
            // フォルダ名が入力されている場合、それを初期位置に設定
            if (!String.IsNullOrEmpty(this.textBoxSaveDirectory.Text))
            {
                this.folderBrowserDialogSaveDirectory.SelectedPath = this.textBoxSaveDirectory.Text;
            }

            // フォルダ選択画面をオープン
            if (this.folderBrowserDialogSaveDirectory.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // フォルダが選択された場合、フォルダ名に選択されたフォルダを設定
                this.textBoxSaveDirectory.Text = this.folderBrowserDialogSaveDirectory.SelectedPath;
            }
        }

        /// <summary>
        /// 出力先テキストボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxSaveDirectory_Leave(object sender, EventArgs e)
        {
            // 空白を削除
            this.textBoxSaveDirectory.Text = this.textBoxSaveDirectory.Text.Trim();
        }

        /// <summary>
        /// 実行ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonRun_Click(object sender, EventArgs e)
        {
            // 入力値チェック
            if (this.textBoxArticle.Text.Trim() == String.Empty)
            {
                // 値が設定されていないときは処理無し
                return;
            }

            // 必要な情報が設定されていない場合は処理不可
            if (Directory.Exists(this.textBoxSaveDirectory.Text) == false)
            {
                FormUtils.WarningDialog(Resources.WarningMessage_UnuseSaveDirectory);
                this.buttonSaveDirectory.Focus();
                return;
            }
            else if (this.comboBoxSource.Text == String.Empty)
            {
                FormUtils.WarningDialog(Resources.WarningMessage_NotSelectedSource);
                this.comboBoxSource.Focus();
                return;
            }
            else if (this.comboBoxTarget.Text == String.Empty)
            {
                FormUtils.WarningDialog(Resources.WarningMessage_NotSelectedTarget);
                this.comboBoxTarget.Focus();
                return;
            }
            else if (this.comboBoxSource.Text == this.comboBoxTarget.Text)
            {
                FormUtils.WarningDialog(Resources.WarningMessage_SourceEqualTarget);
                this.comboBoxTarget.Focus();
                return;
            }

            // 画面をロック
            this.LockOperation();

            // バックグラウンド処理を実行
            this.backgroundWorkerRun.RunWorkerAsync();
        }

        /// <summary>
        /// 中止ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonStop_Click(object sender, EventArgs e)
        {
            // 処理を中断
            this.buttonStop.Enabled = false;
            if (this.backgroundWorkerRun.IsBusy == true)
            {
                System.Diagnostics.Debug.WriteLine("MainForm.-Stop_Click > 処理中断");
                this.backgroundWorkerRun.CancelAsync();
                if (this.translate != null)
                {
                    this.translate.CancellationPending = true;
                }
            }
        }

        /// <summary>
        /// 実行ボタン バックグラウンド処理（スレッド）。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void BackgroundWorkerRun_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // 翻訳支援処理の前処理
                this.textBoxLog.Clear();
                this.logLastLength = 0;
                this.textBoxLog.AppendText(
                    String.Format(
                        Resources.LogMessage_Start,
                        FormUtils.ApplicationName(),
                        DateTime.Now.ToString("F")));

                // 処理結果とログのための出力ファイル名を作成
                string fileName = String.Empty;
                string logName = String.Empty;
                this.MakeFileName(ref fileName, ref logName, this.textBoxArticle.Text.Trim(), this.textBoxSaveDirectory.Text);

                // 翻訳支援処理を実行し、結果とログをファイルに出力
                try
                {
                    this.translate = Translator.Create(this.config, this.comboBoxSource.Text, this.comboBoxTarget.Text);
                }
                catch (NotImplementedException)
                {
                    // 将来の拡張用
                    this.textBoxLog.AppendText(String.Format(Resources.InformationMessage_DevelopingMethod, "Wikipedia以外の処理"));
                    FormUtils.InformationDialog(Resources.InformationMessage_DevelopingMethod, "Wikipedia以外の処理");
                    return;
                }

                this.translate.LogUpdate += new EventHandler(this.GetLogUpdate);

                // 実行前に、ユーザーから中止要求がされているかをチェック
                if (this.backgroundWorkerRun.CancellationPending)
                {
                    this.textBoxLog.AppendText(String.Format(Resources.LogMessage_Stop, logName));
                }
                else
                {
                    // 翻訳支援処理を実行
                    bool successFlag = this.translate.Run(this.textBoxArticle.Text.Trim());

                    // 処理に時間がかかるため、出力ファイル名を再確認
                    this.MakeFileName(ref fileName, ref logName, this.textBoxArticle.Text.Trim(), this.textBoxSaveDirectory.Text);
                    if (successFlag)
                    {
                        // 処理結果を出力
                        try
                        {
                            StreamWriter sw = new StreamWriter(Path.Combine(this.textBoxSaveDirectory.Text, fileName));
                            try
                            {
                                sw.Write(this.translate.Text);
                                this.textBoxLog.AppendText(String.Format(Resources.LogMessage_End, fileName, logName));
                            }
                            finally
                            {
                                sw.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            this.textBoxLog.AppendText(String.Format(Resources.LogMessage_ErrorFileSave, Path.Combine(this.textBoxSaveDirectory.Text, fileName), ex.Message));
                            this.textBoxLog.AppendText(String.Format(Resources.LogMessage_Stop, logName));
                        }
                    }
                    else
                    {
                        this.textBoxLog.AppendText(String.Format(Resources.LogMessage_Stop, logName));
                    }
                }

                // ログを出力
                try
                {
                    StreamWriter sw = new StreamWriter(Path.Combine(this.textBoxSaveDirectory.Text, logName));
                    try
                    {
                        sw.Write(this.textBoxLog.Text);
                    }
                    finally
                    {
                        sw.Close();
                    }
                }
                catch (Exception ex)
                {
                    this.textBoxLog.AppendText(String.Format(Resources.LogMessage_ErrorFileSave, Path.Combine(this.textBoxSaveDirectory.Text, logName), ex.Message));
                }
            }
            catch (Exception ex)
            {
                this.textBoxLog.AppendText("\r\n" + String.Format(Resources.ErrorMessageDevelopmentError, ex.Message, ex.StackTrace) + "\r\n");
                System.Diagnostics.Debug.WriteLine("MainForm.backgroundWorkerRun_DoWork > 想定外のエラー : " + ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// 実行ボタン バックグラウンド処理（終了時）。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void BackgroundWorkerRun_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 設定ファイルのキャッシュ情報を更新
            try
            {
                this.config.Save(Settings.Default.ConfigurationFile);
            }
            catch (Exception ex)
            {
                FormUtils.WarningDialog(
                    Resources.WarningMessageCacheSaveFailed,
                    ex.Message);
            }

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
            // コンボボックス設定
            this.comboBoxSource.Items.Clear();
            this.comboBoxTarget.Items.Clear();

            // 設定ファイルに存在する全言語を選択肢として登録する
            foreach (Website site in this.config.Websites)
            {
                this.comboBoxSource.Items.Add(site.Language.Code);
                this.comboBoxTarget.Items.Add(site.Language.Code);
            }
        }

        /// <summary>
        /// 画面をロック中に移行。
        /// </summary>
        private void LockOperation()
        {
            // 各種ボタンなどを入力不可に変更
            this.groupBoxTransfer.Enabled = false;
            this.groupBoxSaveDirectory.Enabled = false;
            this.textBoxArticle.Enabled = false;
            this.buttonRun.Enabled = false;

            // 中止ボタンを有効に変更
            this.buttonStop.Enabled = true;
        }

        /// <summary>
        /// 画面をロック中から解放。
        /// </summary>
        private void Release()
        {
            // 中止ボタンを入力不可に変更
            this.buttonStop.Enabled = false;

            // 各種ボタンなどを有効に変更
            this.groupBoxTransfer.Enabled = true;
            this.groupBoxSaveDirectory.Enabled = true;
            this.textBoxArticle.Enabled = true;
            this.buttonRun.Enabled = true;
        }

        /// <summary>
        /// 渡された文字列から.txtと.logの重複していないファイル名を作成。
        /// </summary>
        /// <param name="fileName">出力結果ファイル名。</param>
        /// <param name="logName">出力ログファイル名。</param>
        /// <param name="text">出力する結果テキスト。</param>
        /// <param name="dir">出力先ディレクトリ。</param>
        /// <returns><c>true</c> 出力成功</returns>
        private bool MakeFileName(ref string fileName, ref string logName, string text, string dir)
        {
            // 出力先フォルダに存在しないファイル名（の拡張子より前）を作成
            // ※渡されたWikipedia等の記事名にファイル名に使えない文字が含まれている場合、_ に置き換える
            //   また、ファイル名が重複している場合、xx[0].txtのように連番を付ける
            string fileNameBase = FormUtils.ReplaceInvalidFileNameChars(text);
            fileName = fileNameBase + ".txt";
            logName = fileNameBase + ".log";
            bool success = false;
            for (int i = 0; i < 100000; i++)
            {
                // ※100000まで試して空きが見つからないことは無いはず、もし見つからなかったら最後のを上書き
                if (!File.Exists(Path.Combine(dir, fileName))
                    && !File.Exists(Path.Combine(dir, logName)))
                {
                    success = true;
                    break;
                }

                fileName = fileNameBase + "[" + i + "]" + ".txt";
                logName = fileNameBase + "[" + i + "]" + ".log";
            }

            // 結果設定
            return success;
        }

        /// <summary>
        /// 翻訳支援処理クラスのイベント用。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void GetLogUpdate(object sender, System.EventArgs e)
        {
            // 前回以降に追加されたログをテキストボックスに出力
            int length = this.translate.Log.Length;
            if (length > this.logLastLength)
            {
                this.textBoxLog.AppendText(this.translate.Log.Substring(this.logLastLength, length - this.logLastLength));
            }

            this.logLastLength = length;
        }

        #endregion
    }
}