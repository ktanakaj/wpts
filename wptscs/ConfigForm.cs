// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツール設定画面クラスソース</summary>
//
// <copyright file="ConfigForm.cs" company="honeplusのメモ帳">
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
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// Wikipedia翻訳支援ツール設定画面のクラスです。
    /// </summary>
    public partial class ConfigForm : Form
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public ConfigForm()
        {
            InitializeComponent();
        }

        #endregion

        #region 各イベントのメソッド

        /// <summary>
        /// フォームロード時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ConfigForm_Load(object sender, EventArgs e)
        {
            // 各タブの内容を初期化する
            Config config = Config.GetInstance();

            // 記事の置き換えタブの初期化
            this.ImportTranslationTableView(dataGridViewItems, config.GetModeConfig(Config.RunMode.Wikipedia).ItemTables);

            // その他タブの初期化
            this.textBoxCacheExpire.Text = Settings.Default.CacheExpire.Days.ToString();
            this.textBoxUserAgent.Text = Settings.Default.UserAgent;
            this.textBoxReferer.Text = Settings.Default.Referer;
            this.labelApplicationName.Text = FormUtils.ApplicationName();
            AssemblyCopyrightAttribute copyright = Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(),
                typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
            if (copyright != null)
            {
                this.labelCopyright.Text = copyright.Copyright;
            }
        }

        /// <summary>
        /// 記事の置き換え対訳表への行追加時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void DataGridViewItems_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex - 1; i < this.dataGridViewItems.Rows.Count; i++)
            {
                // プログラムから追加された場合は現在のインデックス、画面から追加した場合は+1したインデックスが来る
                if (i >= 0)
                {
                    this.dataGridViewItems.Rows[i].Cells["ColumnArrow"].Value = Resources.RightArrow;
                }
            }
        }

        /// <summary>
        /// OKボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonOk_Click(object sender, EventArgs e)
        {
            // 各タブの内容を設定ファイルに保存する
            Config config = Config.GetInstance();

            // Wikipediaに関する設定
            // ※ 無かったら新規作成
            Config.ModeConfig wikipedia = config.GetModeConfig(Config.RunMode.Wikipedia);

            // 記事の置き換えタブの保存
            wikipedia.ItemTables = this.ExportTranslationTableView(this.dataGridViewItems);

            // その他タブの保存
            Settings.Default.CacheExpire = new TimeSpan(int.Parse(this.textBoxCacheExpire.Text), 0, 0, 0);
            Settings.Default.UserAgent = this.textBoxUserAgent.Text;
            Settings.Default.Referer = this.textBoxReferer.Text;

            // 設定をファイルに保存
            Settings.Default.Save();
            try
            {
                config.Save();
            }
            catch (Exception ex)
            {
                // 異常時はエラーメッセージを表示
                // ※ この場合でもConfigオブジェクトは更新済みのため設定は一時的に有効
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                FormUtils.ErrorDialog(Resources.ErrorMessageConfigSaveFailed, ex.Message);
            }

            // 画面を閉じる
            this.Close();
        }

        /// <summary>
        /// キャッシュ有効期限ボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxCacheExpire_Leave(object sender, EventArgs e)
        {
            int expire;
            if (!int.TryParse(this.textBoxCacheExpire.Text, out expire) || expire < 0)
            {
                FormUtils.WarningDialog(Resources.WarningMessageCacheExpireValue);
                this.textBoxCacheExpire.Focus();
            }
        }

        /// <summary>
        /// ウェブサイトURLクリック時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void LinkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // リンクを開く
            System.Diagnostics.Process.Start(this.linkLabelWebsite.Text);
        }

        #endregion

        #region 対訳表処理メソッド

        /// <summary>
        /// 対訳表を使用する<see cref="DataGridView"/>の値設定を行う。
        /// </summary>
        /// <param name="view">対訳表を表示するビュー</param>
        /// <param name="tables">対訳表データ</param>
        /// <remarks><c>DataGridView</c>の1～2, 3～5列にはFrom, To, TablesのKey, Goal.Word, Timestampに対応する値を持つこと。</remarks>
        private void ImportTranslationTableView(DataGridView view, IList<Translation> tables)
        {
            // 初期設定以外の場合も想定して最初にクリア
            view.Rows.Clear();
            foreach (Translation table in tables)
            {
                foreach (KeyValuePair<string, Translation.Goal> item in table)
                {
                    // 1行分の初期値を設定。右矢印は別途イベントで追加すること
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(view);
                    row.Cells[0].Value = table.From;
                    row.Cells[1].Value = item.Key;
                    row.Cells[3].Value = table.To;
                    row.Cells[4].Value = item.Value.Word;
                    row.Cells[5].Value = item.Value.Timestamp;
                    view.Rows.Add(row);
                }
            }
        }

        /// <summary>
        /// 対訳表を使用する<see cref="DataGridView"/>からデータを抽出する。
        /// </summary>
        /// <param name="view">対訳表を表示するビュー</param>
        /// <returns>対訳表データ</returns>
        /// <remarks>
        /// <c>DataGridView</c>の1～2, 3～5列にはFrom, To, TablesのKey, Goal.Word, Timestampに対応する値を持つこと。
        /// Goal.Word, Timestamp以外の値は必須。
        /// またTimestampは<see cref="DateTime.Parse(string)"/>できること。
        /// </remarks>
        private IList<Translation> ExportTranslationTableView(DataGridView view)
        {
            IList<Translation> tables = new List<Translation>();
            foreach (DataGridViewRow row in view.Rows)
            {
                string from = ObjectUtils.ToString(row.Cells[0].Value);
                string to = ObjectUtils.ToString(row.Cells[3].Value);

                // 画面での追加用の最終行が空で渡されてくるので無視
                if (String.IsNullOrEmpty(from))
                {
                    continue;
                }

                // その行で対象とする言語を探索
                Translation table = null;
                foreach (Translation t in tables)
                {
                    if (t.From == from && t.To == to)
                    {
                        table = t;
                        break;
                    }
                }

                if (table == null)
                {
                    // 無かったら新規作成
                    table = new Translation(from, to);
                    tables.Add(table);
                }

                // 値を格納
                Translation.Goal goal = new Translation.Goal();
                goal.Word = ObjectUtils.ToString(row.Cells[4].Value);
                string timestamp = ObjectUtils.ToString(row.Cells[5].Value);
                if (!String.IsNullOrEmpty(timestamp))
                {
                    goal.Timestamp = DateTime.Parse(timestamp);
                }

                table[ObjectUtils.ToString(row.Cells[1].Value)] = goal;
            }

            return tables;
        }
        
        #endregion
    }
}
