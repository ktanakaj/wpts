// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツール主画面クラスソース</summary>
//
// <copyright file="MainForm.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
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
    using Honememo.Wptscs.Utilities;
    using Honememo.Wptscs.Websites;

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
        private Translator translator;

        /// <summary>
        /// 表示済みログ文字列長。
        /// </summary>
        private int logLength;

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
            if (!this.LoadConfig())
            {
                // 読み込み失敗時はどうしようもないのでそのまま終了
                this.Close();
            }

            this.translator = null;
            Control.CheckForIllegalCrossThreadCalls = false;

            // コンボボックス設定
            this.Initialize();

            // 前回の処理状態を復元
            this.textBoxSaveDirectory.Text = Settings.Default.SaveDirectory;
            this.comboBoxSource.SelectedItem = Settings.Default.LastSelectedSource;
            this.comboBoxTarget.SelectedItem = Settings.Default.LastSelectedTarget;

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
            if (!String.IsNullOrWhiteSpace(this.comboBoxSource.Text))
            {
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
            // 直接入力された場合の対策、変更時の処理をコール
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
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
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
            if (!String.IsNullOrWhiteSpace(this.comboBoxTarget.Text))
            {
                this.comboBoxTarget.Text = this.comboBoxTarget.Text.Trim().ToLower();

                // その言語の、ユーザーが使用している言語での表示名を表示
                // （日本語環境だったら日本語を、英語だったら英語を）
                if (this.config.GetWebsite(this.comboBoxTarget.Text) != null)
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
            // 直接入力された場合の対策、変更時の処理をコール
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
            ConfigForm form = new ConfigForm(this.config);
            form.ShowDialog();

            // 戻ってきたら設定ファイルを再読み込み
            // ※ キャンセル時もインスタンスは更新されてしまうので
            this.LoadConfig();

            // コンボボックス設定
            string backupSourceSelected = this.comboBoxSource.Text;
            string backupSourceTarget = this.comboBoxTarget.Text;
            this.Initialize();
            this.comboBoxSource.SelectedItem = backupSourceSelected;
            this.comboBoxTarget.SelectedItem = backupSourceTarget;

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
            // フォーム入力値をチェック
            if (String.IsNullOrWhiteSpace(this.comboBoxSource.Text))
            {
                FormUtils.WarningDialog(Resources.WarningMessageNotSelectedSource);
                this.comboBoxSource.Focus();
                return;
            }
            else if (String.IsNullOrWhiteSpace(this.comboBoxTarget.Text))
            {
                FormUtils.WarningDialog(Resources.WarningMessageNotSelectedTarget);
                this.comboBoxTarget.Focus();
                return;
            }
            else if (!String.IsNullOrWhiteSpace(this.comboBoxSource.Text)
                && this.comboBoxSource.Text == this.comboBoxTarget.Text)
            {
                FormUtils.WarningDialog(Resources.WarningMessageEqualsSourceAndTarget);
                this.comboBoxTarget.Focus();
                return;
            }
            else if (String.IsNullOrWhiteSpace(this.textBoxSaveDirectory.Text))
            {
                FormUtils.WarningDialog(Resources.WarningMessageEmptySaveDirectory);
                this.textBoxSaveDirectory.Focus();
                return;
            }
            else if (!Directory.Exists(this.textBoxSaveDirectory.Text))
            {
                FormUtils.WarningDialog(Resources.WarningMessageIgnoreSaveDirectory);
                this.textBoxSaveDirectory.Focus();
                return;
            }
            else if (String.IsNullOrWhiteSpace(this.textBoxArticle.Text))
            {
                FormUtils.WarningDialog(Resources.WarningMessageEmptyArticle);
                this.textBoxArticle.Focus();
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
                if (this.translator != null)
                {
                    this.translator.CancellationPending = true;
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
                // 初期化と開始メッセージ
                this.textBoxLog.Clear();
                this.logLength = 0;
                this.textBoxLog.AppendText(String.Format(Resources.LogMessageStart, FormUtils.ApplicationName(), DateTime.Now.ToString("F")));

                // 翻訳支援処理ロジックのオブジェクトを生成
                try
                {
                    this.translator = Translator.Create(this.config, this.comboBoxSource.Text, this.comboBoxTarget.Text);
                }
                catch (NotImplementedException)
                {
                    // 設定ファイルに対応していないパターンが書かれている場合の例外、将来の拡張用
                    this.textBoxLog.AppendText(String.Format(Resources.InformationMessageDevelopingMethod, "Wikipedia以外の処理"));
                    FormUtils.InformationDialog(Resources.InformationMessageDevelopingMethod, "Wikipedia以外の処理");
                    return;
                }

                // ログ・処理状態更新通知を受け取るためのイベント登録
                // 処理時間更新用にタイマーを起動
                this.translator.LogUpdate += new EventHandler(this.GetLogUpdate);
                this.translator.StatusUpdate += new EventHandler(this.GetStatusUpdate);
                this.Invoke((MethodInvoker)delegate { this.timerStatusStopwatch.Start(); });

                // 翻訳支援処理を実行
                bool success = true;
                try
                {
                    this.translator.Run(this.textBoxArticle.Text.Trim());
                }
                catch (ApplicationException)
                {
                    // 中止要求で停止した場合、その旨イベントに格納する
                    e.Cancel = this.backgroundWorkerRun.CancellationPending;
                    success = false;
                }
                finally
                {
                    // 処理時間更新用のタイマーを終了
                    this.Invoke((MethodInvoker)delegate { this.timerStatusStopwatch.Stop(); });
                }

                // 実行結果から、ログと変換後テキストをファイル出力
                this.WriteResult(success);
            }
            catch (Exception ex)
            {
                this.textBoxLog.AppendText("\r\n" + String.Format(Resources.ErrorMessageDevelopmentError, ex.Message, ex.StackTrace) + "\r\n");
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
            // ※ 微妙に時間がかかるので、ステータスバーに通知
            try
            {
                this.toolStripStatusLabelStatus.Text = Resources.StatusCacheUpdating;
                try
                {
                    this.config.Save();
                }
                finally
                {
                    this.toolStripStatusLabelStatus.Text = String.Empty;
                }
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

        /// <summary>
        /// ステータスバー処理時間更新タイマー処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TimerStatusStopwatch_Tick(object sender, EventArgs e)
        {
            // 処理時間をステータスバーに反映
            this.toolStripStatusLabelStopwatch.Text = String.Format(Resources.ElapsedTime, this.translator.Stopwatch.Elapsed);
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
        /// 設定ファイル読み込み。
        /// </summary>
        /// <returns>読み込み成功時は<c>true</c>。</returns>
        private bool LoadConfig()
        {
            // 設定ファイルの読み込み
            // ※ 微妙に時間がかかるので、ステータスバーに通知
            try
            {
                this.toolStripStatusLabelStatus.Text = Resources.StatusConfigReading;
                try
                {
                    this.config = Config.GetInstance(Settings.Default.ConfigurationFile);
                }
                finally
                {
                    this.toolStripStatusLabelStatus.Text = String.Empty;
                }
            }
            catch (FileNotFoundException ex)
            {
                // 設定ファイルが見つからない場合
                System.Diagnostics.Debug.WriteLine(
                    "MainForm.LoadConfig > 設定ファイル読み込み失敗 : " + ex.Message);
                FormUtils.ErrorDialog(
                    Resources.ErrorMessageConfigNotFound,
                    Settings.Default.ConfigurationFile);

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "MainForm.LoadConfig > 設定ファイル読み込み時エラー : " + ex.StackTrace);
                FormUtils.ErrorDialog(
                    Resources.ErrorMessageConfigLordFailed,
                    ex.Message);

                return false;
            }

            return true;
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
        /// 翻訳支援処理のログ・変換後テキストをファイル出力。
        /// </summary>
        /// <param name="success">翻訳支援処理が成功した場合<c>true</c>。</param>
        private void WriteResult(bool success)
        {
            // 若干時間がかかるのでステータスバーに通知
            this.toolStripStatusLabelStatus.Text = Resources.StatusFileWriting;
            try
            {
                // 使用可能な出力ファイル名を生成
                string fileName;
                string logName;
                this.MakeFileName(out fileName, out logName, this.textBoxArticle.Text.Trim(), this.textBoxSaveDirectory.Text);

                if (success)
                {
                    // 翻訳支援処理成功時は変換後テキストを出力
                    try
                    {
                        File.WriteAllText(Path.Combine(this.textBoxSaveDirectory.Text, fileName), this.translator.Text);
                        this.textBoxLog.AppendText(String.Format(Resources.LogMessageEnd, fileName, logName));
                    }
                    catch (Exception ex)
                    {
                        this.textBoxLog.AppendText(String.Format(Resources.LogMessageFileSaveFailed, Path.Combine(this.textBoxSaveDirectory.Text, fileName), ex.Message));
                        this.textBoxLog.AppendText(String.Format(Resources.LogMessageStop, logName));
                    }
                }
                else
                {
                    this.textBoxLog.AppendText(String.Format(Resources.LogMessageStop, logName));
                }

                // ログを出力
                try
                {
                    File.WriteAllText(Path.Combine(this.textBoxSaveDirectory.Text, logName), this.textBoxLog.Text);
                }
                catch (Exception ex)
                {
                    this.textBoxLog.AppendText(String.Format(Resources.LogMessageFileSaveFailed, Path.Combine(this.textBoxSaveDirectory.Text, logName), ex.Message));
                }
            }
            finally
            {
                // ステータスバーをクリア
                this.toolStripStatusLabelStatus.Text = String.Empty;
            }
        }

        /// <summary>
        /// 渡された文字列から.txtと.logの重複していないファイル名を作成。
        /// </summary>
        /// <param name="fileName">出力結果ファイル名。</param>
        /// <param name="logName">出力ログファイル名。</param>
        /// <param name="text">出力する結果テキスト。</param>
        /// <param name="dir">出力先ディレクトリ。</param>
        /// <returns><c>true</c> 出力成功</returns>
        private bool MakeFileName(out string fileName, out string logName, string text, string dir)
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
        /// 翻訳支援処理クラスのログ更新イベント用。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void GetLogUpdate(object sender, EventArgs e)
        {
            // 前回以降に追加されたログをテキストボックスに出力
            int length = this.translator.Log.Length;
            if (length > this.logLength)
            {
                this.textBoxLog.AppendText(this.translator.Log.Substring(this.logLength, length - this.logLength));
            }

            this.logLength = length;
        }

        /// <summary>
        /// 翻訳支援処理クラスの処理状態更新イベント用。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void GetStatusUpdate(object sender, EventArgs e)
        {
            // 処理状態をステータスバーに通知
            this.toolStripStatusLabelStatus.Text = this.translator.Status;
        }

        #endregion
    }
}