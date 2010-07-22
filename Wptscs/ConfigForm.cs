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
        #region private変数

        /// <summary>
        /// 現在設定中のアプリケーションの設定。
        /// </summary>
        private Config config;

        /// <summary>
        /// <seealso cref="comboBoxLanguage"/>で選択していたアイテムのバックアップ。
        /// </summary>
        private string comboBoxLanguageSelectedText;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public ConfigForm()
        {
            this.InitializeComponent();
        }

        #endregion
        
        #region フォームの各イベントのメソッド

        /// <summary>
        /// フォームロード時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ConfigForm_Load(object sender, EventArgs e)
        {
            // 各タブの内容を初期化する
            this.config = Config.GetInstance(Settings.Default.ConfigurationFile);

            // 記事の置き換えタブの初期化
            this.ImportTranslationTableView(this.dataGridViewItems, this.config.ItemTables);

            // サーバー／言語タブの初期化
            foreach (Website site in this.config.Websites)
            {
                this.comboBoxLanguage.Items.Add(site.Language);
            }

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
        /// OKボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonOk_Click(object sender, EventArgs e)
        {
            // 各タブの内容を設定ファイルに保存する

            // 記事の置き換えタブの保存
            this.config.ItemTables = this.ExportTranslationTableView(this.dataGridViewItems);

            // その他タブの保存
            Settings.Default.CacheExpire = new TimeSpan(int.Parse(this.textBoxCacheExpire.Text), 0, 0, 0);
            Settings.Default.UserAgent = this.textBoxUserAgent.Text;
            Settings.Default.Referer = this.textBoxReferer.Text;

            // 設定をファイルに保存
            Settings.Default.Save();
            try
            {
                this.config.Save(Settings.Default.ConfigurationFile);
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

        #endregion

        #region 記事の置き換えタブのイベントのメソッド

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

        #endregion

        #region 言語／サーバータブのイベントのメソッド

        /// <summary>
        /// 言語コンボボックス変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ComboBoxLanguuage_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ConfigForm._SelectedIndexChanged > "
                + this.comboBoxLanguageSelectedText + " -> "
                + ObjectUtils.ToString(this.comboBoxLanguage.SelectedItem));

            // 変更前の設定を保存
            if (!String.IsNullOrEmpty(this.comboBoxLanguageSelectedText))
            {
                // 設定が存在しなければ基本的に自動生成されるのでそのまま格納
                // TODO: 値の保管先
                MediaWiki wiki = new MediaWiki(this.comboBoxLanguageSelectedText, StringUtils.DefaultString(this.textBoxLocation.Text).Trim());
                wiki.ExportPath = StringUtils.DefaultString(this.textBoxExportPath.Text).Trim();
                wiki.Xmlns = StringUtils.DefaultString(this.textBoxXmlns.Text).Trim();
                wiki.NamespacePath = StringUtils.DefaultString(this.textBoxNamespacePath.Text).Trim();
                wiki.Redirect = StringUtils.DefaultString(this.textBoxRedirect.Text).Trim();

                // 以下、数値へのparseは事前にチェックしてあるので、ここではチェックしない
                wiki.TemplateNamespace = int.Parse(this.textBoxTemplateNamespace.Text);
                wiki.CategoryNamespace = int.Parse(this.textBoxCategoryNamespace.Text);
                wiki.FileNamespace = int.Parse(this.textBoxFileNamespace.Text);

                // 表から呼称の情報も保存
                this.dataGridViewLanguageName.Sort(this.dataGridViewLanguageName.Columns["ColumnLanguageNameCode"], ListSortDirection.Ascending);
                Language lang = this.config.GetLanguage(this.comboBoxLanguageSelectedText);
                lang.Bracket = StringUtils.DefaultString(this.textBoxBracket.Text).Trim();
                lang.Names.Clear();
                for (int y = 0; y < this.dataGridViewLanguageName.RowCount - 1; y++)
                {
                    // 値が入ってないとかはガードしているはずだが、一応チェック
                    string code = FormUtils.ToString(this.dataGridViewLanguageName["ColumnLanguageNameCode", y]).Trim();
                    if (!String.IsNullOrEmpty(code))
                    {
                        Language.LanguageName name = new Language.LanguageName();
                        name.Name = FormUtils.ToString(this.dataGridViewLanguageName["ColumnLanguageNameName", y]).Trim();
                        name.ShortName = FormUtils.ToString(this.dataGridViewLanguageName["ColumnLanguageNameShortName", y]).Trim();
                        lang.Names[code] = name;
                    }
                }
            }

            // 変更後の値に応じて、画面表示を更新
            if (this.comboBoxLanguage.SelectedItem != null)
            {
                // 設定が存在しなければ基本的に自動生成されるのでそのまま使用
                // TODO: nullの場合の初期化が不十分（今のところnullは無いけど）
                Website site = this.config.GetWebsite(this.comboBoxLanguage.SelectedItem.ToString());
                this.textBoxLocation.Text = site.Location;
                MediaWiki wiki = site as MediaWiki;
                if (wiki != null)
                {
                    this.textBoxExportPath.Text = wiki.ExportPath;
                    this.textBoxXmlns.Text = wiki.Xmlns;
                    this.textBoxNamespacePath.Text = wiki.NamespacePath;
                    this.textBoxTemplateNamespace.Text = wiki.TemplateNamespace.ToString();
                    this.textBoxCategoryNamespace.Text = wiki.CategoryNamespace.ToString();
                    this.textBoxFileNamespace.Text = wiki.FileNamespace.ToString();
                    this.textBoxRedirect.Text = wiki.Redirect;
                }

                // 呼称の情報を表に設定
                this.dataGridViewLanguageName.Rows.Clear();
                Language lang = this.config.GetLanguage(this.comboBoxLanguage.SelectedItem.ToString());
                this.textBoxBracket.Text = lang.Bracket;
                foreach (KeyValuePair<string, Language.LanguageName> name in lang.Names)
                {
                    int index = this.dataGridViewLanguageName.Rows.Add();
                    this.dataGridViewLanguageName["ColumnLanguageNameCode", index].Value = name.Key;
                    this.dataGridViewLanguageName["ColumnLanguageNameName", index].Value = name.Value.Name;
                    this.dataGridViewLanguageName["ColumnLanguageNameShortName", index].Value = name.Value.ShortName;
                }

                // 各入力欄を有効に
                this.groupBoxServer.Enabled = true;
                this.groupBoxLanguage.Enabled = true;

                // 現在の選択値を更新
                this.comboBoxLanguageSelectedText = this.comboBoxLanguage.SelectedItem.ToString();
            }
            else
            {
                // 各入力欄を無効に
                this.groupBoxServer.Enabled = false;
                this.groupBoxLanguage.Enabled = false;

                // 現在の選択値を更新
                this.comboBoxLanguageSelectedText = String.Empty;
            }
        }

        /// <summary>
        /// テンプレート名前空間のIDボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxTemplateNamespace_Leave(object sender, EventArgs e)
        {
            this.textBoxTemplateNamespace.Text = StringUtils.DefaultString(this.textBoxTemplateNamespace.Text).Trim();
            int value;
            if (!int.TryParse(this.textBoxTemplateNamespace.Text, out value))
            {
                FormUtils.WarningDialog(Resources.WarningMessageNamespaceNumberValue);
                this.textBoxTemplateNamespace.Focus();
            }
        }

        /// <summary>
        /// カテゴリ名前空間のIDボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxCategoryNamespace_Leave(object sender, EventArgs e)
        {
            this.textBoxCategoryNamespace.Text = StringUtils.DefaultString(this.textBoxCategoryNamespace.Text).Trim();
            int value;
            if (!int.TryParse(this.textBoxCategoryNamespace.Text, out value))
            {
                FormUtils.WarningDialog(Resources.WarningMessageNamespaceNumberValue);
                this.textBoxCategoryNamespace.Focus();
            }
        }

        /// <summary>
        /// ファイル名前空間のIDボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxFileNamespace_Leave(object sender, EventArgs e)
        {
            this.textBoxFileNamespace.Text = StringUtils.DefaultString(this.textBoxFileNamespace.Text).Trim();
            int value;
            if (!int.TryParse(this.textBoxFileNamespace.Text, out value))
            {
                FormUtils.WarningDialog(Resources.WarningMessageNamespaceNumberValue);
                this.textBoxFileNamespace.Focus();
            }
        }

        #endregion

        #region その他タブのイベントのメソッド

        /// <summary>
        /// キャッシュ有効期限ボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxCacheExpire_Leave(object sender, EventArgs e)
        {
            this.textBoxCacheExpire.Text = StringUtils.DefaultString(this.textBoxCacheExpire.Text).Trim();
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
