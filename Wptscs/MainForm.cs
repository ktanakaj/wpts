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
    using System.Net;
    using System.Text;
    using System.Threading;
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

        #region フォームの各イベントのメソッド

        /// <summary>
        /// フォームロード時の処理。初期化。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // フォームの初期設定
            Control.CheckForIllegalCrossThreadCalls = false;

            // 表示言語選択メニュー、設定選択メニューの初期設定
            this.InitializeDropDownButtonLanguage();
            this.InitializeDropDownButtonConfig();

            // 設定ファイルの読み込みと関連項目の初期設定
            this.InitializeByConfig();

            // 出力先フォルダの設定を復元
            this.textBoxSaveDirectory.Text = Settings.Default.SaveDirectory;
        }

        /// <summary>
        /// フォームクローズ時の処理。処理状態を保存。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 現在の出力先フォルダ、翻訳元／先言語、
            // また更新されていれば表示言語や設定ファイルの選択を保存
            this.SetSettings();
            Settings.Default.Save();
        }

        #endregion

        #region 翻訳元／先言語グループのイベントのメソッド

        /// <summary>
        /// 翻訳元コンボボックス変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ComboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ラベルに言語名を表示する
            Website site = this.config.GetWebsite(this.comboBoxSource.Text);
            this.SetLanguageNameLabel(this.labelSource, site);

            // サーバーURLの表示
            this.linkLabelSourceURL.Text = "http://";
            if (site != null)
            {
                this.linkLabelSourceURL.Text = site.Location;
            }
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
            this.SetLanguageNameLabel(this.labelTarget, this.config.GetWebsite(this.comboBoxTarget.Text));
        }

        /// <summary>
        /// 設定ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonConfig_Click(object sender, EventArgs e)
        {
            // 現在の画面の表示状態を保存
            this.SetSettings();

            // 設定画面を開く
            using (ConfigForm form = new ConfigForm(this.config))
            {
                form.ShowDialog();
            }

            // 戻ってきたら設定ファイルを再読み込みして表示を更新
            // ※ キャンセル時もインスタンスは更新されてしまうので
            this.InitializeByConfig();
        }

        #region イベント実装支援用メソッド

        /// <summary>
        /// 翻訳元／先言語コンボボックスの初期化処理。
        /// </summary>
        private void InitializeComboBox()
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

            // 選択されていた項目を選択中に復元
            this.comboBoxSource.SelectedItem = Settings.Default.LastSelectedSource;
            this.comboBoxTarget.SelectedItem = Settings.Default.LastSelectedTarget;

            // コンボボックス変更時の処理をコール
            // ※ 項目が存在する場合は↑で自動的に呼ばれるが、無い場合は呼ばれないため
            this.ComboBoxSource_SelectedIndexChanged(this.comboBoxSource, new EventArgs());
            this.ComboBoxTarget_SelectedIndexChanged(this.comboBoxTarget, new EventArgs());
        }

        /// <summary>
        /// ウェブサイトの言語の表示名ラベルの表示を設定する。
        /// </summary>
        /// <param name="label">言語の表示名用ラベル。</param>
        /// <param name="site">選択されている言語のウェブサイト。</param>
        private void SetLanguageNameLabel(Label label, Website site)
        {
            // ラベルを初期化
            label.Text = String.Empty;
            if (site == null)
            {
                return;
            }

            // ウェブサイトが空でない場合、その言語の、ユーザーが使用している言語での表示名を表示
            // （日本語環境だったら日本語を、英語環境だったら英語を）
            Language.LanguageName name;
            if (site.Language.Names.TryGetValue(
                Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName,
                out name))
            {
                label.Text = name.Name;
            }
        }

        #endregion

        #endregion

        #region フォルダの選択グループのイベントのメソッド

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

        #endregion

        #region 記事を指定して実行グループのイベントのメソッド

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
                // 初期化と開始メッセージ、別スレッドになるので表示言語も再度設定
                Program.LoadSelectedCulture();
                this.textBoxLog.Clear();
                this.logLength = 0;
                this.textBoxLog.AppendText(String.Format(Resources.LogMessageStart, FormUtils.ApplicationName(), DateTime.Now));

                // 翻訳支援処理ロジックのオブジェクトを生成
                this.translator = Translator.Create(this.config, this.comboBoxSource.Text, this.comboBoxTarget.Text);

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
            catch (WebException ex)
            {
                // 想定外の通信エラー（↓とまとめてもよいが、こちらはサーバーの状況などで発生しやすいので）
                this.textBoxLog.AppendText(Environment.NewLine + String.Format(Resources.ErrorMessageConnectionFailed, ex.Message) + Environment.NewLine);
                if (ex.Response != null)
                {
                    // 出せるならエラーとなったURLも出力
                    this.textBoxLog.AppendText(Resources.RightArrow + " " + String.Format(Resources.LogMessageErrorURL, ex.Response.ResponseUri) + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                // 想定外のエラー
                this.textBoxLog.AppendText(Environment.NewLine + String.Format(Resources.ErrorMessageDevelopmentError, ex.Message, ex.StackTrace) + Environment.NewLine);
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

        #region イベント実装支援用メソッド

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
            this.toolStripDropDownButtonLanguage.Enabled = false;
            this.toolStripDropDownButtonConfig.Enabled = false;

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
            this.toolStripDropDownButtonLanguage.Enabled = true;
            this.toolStripDropDownButtonConfig.Enabled = true;
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
                // ※ 100000まで試して空きが見つからないことは無いはず、もし見つからなかったら最後のを上書き
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

        #endregion

        #region ステータスバーのイベントのメソッド

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

        /// <summary>
        /// 表示言語選択メニュー日本語クリック時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ToolStripMenuItemJapanese_Click(object sender, EventArgs e)
        {
            // 表示言語を日本語に設定し再起動する
            this.ChangeCultureAndRestart("ja-JP");
        }

        /// <summary>
        /// 表示言語選択メニュー英語(US)クリック時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ToolStripMenuItemEnglishUS_Click(object sender, EventArgs e)
        {
            // 表示言語を英語(US)に設定し再起動する
            this.ChangeCultureAndRestart("en-US");
        }

        /// <summary>
        /// 表示言語選択メニュー英語(GB)クリック時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ToolStripMenuItemEnglishGB_Click(object sender, EventArgs e)
        {
            // 表示言語を英語(GB)に設定し再起動する
            this.ChangeCultureAndRestart("en-GB");
        }

        /// <summary>
        /// 表示言語選択メニュー自動クリック時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ToolStripMenuItemAuto_Click(object sender, EventArgs e)
        {
            // 表示言語を空欄に設定し再起動する
            this.ChangeCultureAndRestart(String.Empty);
        }

        /// <summary>
        /// 設定選択メニュークリック時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ToolStripMenuItemConfig_Click(object sender, EventArgs e)
        {
            // メニュー項目を一旦全て未選択状態に更新
            foreach (ToolStripMenuItem i in this.toolStripDropDownButtonConfig.DropDownItems)
            {
                i.Checked = false;
                i.Enabled = true;
            }

            // メニュー項目名から設定ファイル名を作成、再読み込みする
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Settings.Default.LastSelectedConfiguration = item.Text;
            this.SetSettings();
            this.InitializeByConfig();
        }

        /// <summary>
        /// 設定選択メニュー追加クリック時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ToolStripMenuItemNew_Click(object sender, EventArgs e)
        {
            // 重複チェック用の登録済みの設定一覧を用意
            IList<string> configNames = new List<string>();
            foreach (ToolStripMenuItem item in this.toolStripDropDownButtonConfig.DropDownItems)
            {
                if (item != this.toolStripMenuItemNew)
                {
                    configNames.Add(item.Text);
                }
            }

            // 設定追加用ダイアログで言語コードを入力
            using (AddConfigDialog form = new AddConfigDialog(configNames))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // 設定選択メニューに新しい設定を追加。
                    // 設定ファイルが作成されているため、それを読み込みなおす。
                    this.ToolStripMenuItemConfig_Click(
                        this.AddToolStripDropDownButtonConfigItem(form.ConfigName),
                        e);
                }
            }
        }

        #region イベント実装支援用メソッド

        /// <summary>
        /// 表示言語選択メニューの初期化処理。
        /// </summary>
        private void InitializeDropDownButtonLanguage()
        {
            // 選択中の言語のメニュー項目を抽出
            ToolStripMenuItem item;
            switch (Settings.Default.LastSelectedLanguage)
            {
                case "en-US":
                    item = this.toolStripMenuItemEnglishUS;
                    break;
                case "en-GB":
                    item = this.toolStripMenuItemEnglishGB;
                    break;
                case "ja-JP":
                    item = this.toolStripMenuItemJapanese;
                    break;
                default:
                    item = this.toolStripMenuItemAuto;
                    break;
            }

            // 選択中の項目をチェック状態＆押下不能とする
            item.Checked = true;
            item.Enabled = false;
            if (item != this.toolStripMenuItemAuto)
            {
                // 自動以外の場合、ステータスバーの表示も更新
                this.toolStripDropDownButtonLanguage.Text = item.Text;
            }
        }

        /// <summary>
        /// 設定ファイル選択メニューの初期化処理。
        /// </summary>
        private void InitializeDropDownButtonConfig()
        {
            // exeまたはユーザーフォルダにある設定ファイルをメニュー項目としてリストアップ
            foreach (string file in FormUtils.GetFilesAtUserAppData(
                "*" + Settings.Default.ConfigurationExtension,
                Settings.Default.ConfigurationCompatible))
            {
                try
                {
                    // 関係ないXMLファイルを除外するため、読み込めるフォーマットかをチェック
                    // ※ ちょっと時間がかかるが・・・
                    Config.GetInstance(file);

                    // 問題なければファイル名を見出しにメニューに追加
                    this.AddToolStripDropDownButtonConfigItem(Path.GetFileNameWithoutExtension(file));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "MainForm.InitializeDropDownButtonConfig : " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 設定選択メニューに新しい設定を追加する。
        /// </summary>
        /// <param name="name">設定名。</param>
        /// <returns>追加したメニュー。</returns>
        /// <remarks>追加メニューがあるのでその後ろに登録する。</remarks>
        private ToolStripMenuItem AddToolStripDropDownButtonConfigItem(string name)
        {
            // 設定変更のイベントを設定する
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = name;
            item.Click += new EventHandler(this.ToolStripMenuItemConfig_Click);
            this.toolStripDropDownButtonConfig.DropDownItems.Insert(
                this.toolStripDropDownButtonConfig.DropDownItems.Count - 1,
                item);
            return item;
        }

        /// <summary>
        /// アプリケーションの現在の表示言語を変更、再起動する。
        /// </summary>
        /// <param name="name">変更先カルチャ名。</param>
        /// <remarks>このメソッドを呼び出すとアプリケーションが一旦終了します。</remarks>
        private void ChangeCultureAndRestart(string name)
        {
            // 現在の画面表示と表示言語設定を保存した後、アプリケーションを再起動
            this.SetSettings();
            Settings.Default.LastSelectedLanguage = name;
            Settings.Default.Save();
            Application.Restart();
            this.Close();
        }

        #endregion

        #endregion

        #region その他のメソッド

        /// <summary>
        /// 設定ファイルによる初期化処理。
        /// </summary>
        /// <remarks>
        /// 読み込みに失敗した場合、空の設定を登録し、操作の大半をロックする。
        /// （設定変更メニューから正しい設定に変更されることを期待。）
        /// </remarks>
        private void InitializeByConfig()
        {
            // 設定ファイルの読み込み
            this.LoadConfig();
            if (this.config == null)
            {
                // 読み込みに失敗した場合、空の設定を作成（設定値には適当な値を設定）
                // 設定選択メニューの表示を更新し、画面をほぼ操作不可に変更
                this.config = new Config();
                this.groupBoxTransfer.Enabled = false;
                this.groupBoxSaveDirectory.Enabled = false;
                this.groupBoxRun.Enabled = false;
                this.toolStripDropDownButtonConfig.Text = Resources.DropDownConfigLoadConfigFailed;
            }
            else
            {
                // 設定選択メニューの表示を更新し、画面を操作可能な状態に戻す
                this.groupBoxTransfer.Enabled = true;
                this.groupBoxSaveDirectory.Enabled = true;
                this.groupBoxRun.Enabled = true;
                this.toolStripDropDownButtonConfig.Text = Path.GetFileNameWithoutExtension(this.config.File);
                foreach (ToolStripMenuItem item in this.toolStripDropDownButtonConfig.DropDownItems)
                {
                    // 読み込んだ設定を選択中（チェック状態＆押下不能）に更新
                    if (item != this.toolStripMenuItemNew
                        && item.Text == this.toolStripDropDownButtonConfig.Text)
                    {
                        item.Checked = true;
                        item.Enabled = false;
                    }
                }
            }

            // コンボボックスを読み込んだ設定で初期化
            this.InitializeComboBox();
        }

        /// <summary>
        /// 設定ファイル読み込み。
        /// </summary>
        private void LoadConfig()
        {
            // 設定ファイルの読み込み
            // ※ 微妙に時間がかかるので、ステータスバーに通知
            string file = Settings.Default.LastSelectedConfiguration + Settings.Default.ConfigurationExtension;
            try
            {
                this.toolStripStatusLabelStatus.Text = Resources.StatusConfigReading;
                try
                {
                    this.config = Config.GetInstance(file);
                }
                finally
                {
                    this.toolStripStatusLabelStatus.Text = String.Empty;
                }
            }
            catch (FileNotFoundException ex)
            {
                // 設定ファイルが見つからない場合、エラーメッセージを表示
                System.Diagnostics.Debug.WriteLine(
                    "MainForm.LoadConfig > 設定ファイル読み込み失敗 : " + ex.Message);
                FormUtils.ErrorDialog(Resources.ErrorMessageConfigNotFound, file);
            }
            catch (Exception ex)
            {
                // その他の例外（権限が無いとかファイルが壊れているとか）
                System.Diagnostics.Debug.WriteLine(
                    "MainForm.LoadConfig > 設定ファイル読み込み時エラー : " + ex.ToString());
                FormUtils.ErrorDialog(Resources.ErrorMessageConfigLordFailed, ex.Message);
            }
        }

        /// <summary>
        /// 現在の出力先フォルダ、翻訳元／先言語をアプリケーション設定に反映。
        /// </summary>
        /// <remarks>表示言語や設定ファイルの選択については必要な場合のみ別途実施。</remarks>
        private void SetSettings()
        {
            Settings.Default.SaveDirectory = this.textBoxSaveDirectory.Text;
            Settings.Default.LastSelectedSource = this.comboBoxSource.Text;
            Settings.Default.LastSelectedTarget = this.comboBoxTarget.Text;
        }

        #endregion
    }
}