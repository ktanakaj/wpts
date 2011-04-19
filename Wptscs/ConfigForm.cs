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
        /// <remarks>設定画面を閉じた後は再読み込みされるので、必要に応じて随時更新してよい。</remarks>
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
        /// <param name="config">設定対象のConfig。</param>
        /// <remarks>configは設定画面の操作により随時更新される。呼び出し元では再読み込みすること。</remarks>
        public ConfigForm(Config config)
        {
            this.InitializeComponent();

            // 設定対象のConfigを受け取る
            this.config = Utilities.Validate.NotNull(config, "config");
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
            try
            {
                // 各タブの内容を初期化する

                // 記事の置き換えタブの初期化
                this.ImportTranslationDictionaryView(this.dataGridViewItems, this.config.ItemTables);

                // 見出しの置き換えタブの初期化
                this.ImportTranslationTableView(this.dataGridViewHeading, this.config.HeadingTable);

                // サーバー／言語タブの初期化
                foreach (Website site in this.config.Websites)
                {
                    this.comboBoxLanguage.Items.Add(site.Language.Code);
                }

                // その他タブの初期化
                this.textBoxCacheExpire.Text = Settings.Default.CacheExpire.Days.ToString();
                this.textBoxUserAgent.Text = Settings.Default.UserAgent;
                this.textBoxReferer.Text = Settings.Default.Referer;
                this.checkBoxIgnoreError.Checked = Settings.Default.IgnoreError;
                this.labelApplicationName.Text = FormUtils.ApplicationName();
                AssemblyCopyrightAttribute copyright = Attribute.GetCustomAttribute(
                    Assembly.GetExecutingAssembly(),
                    typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
                if (copyright != null)
                {
                    this.labelCopyright.Text = copyright.Copyright;
                }
            }
            catch (Exception ex)
            {
                // 通常この処理では例外は発生しないはず（Configに読めているので）。想定外のエラー用
                FormUtils.ErrorDialog(Resources.ErrorMessageDevelopmentError, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// OKボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonOk_Click(object sender, EventArgs e)
        {
            try
            {
                // 各タブの内容を設定ファイルに保存する

                // 記事の置き換えタブの保存
                this.config.ItemTables = this.ExportTranslationDictionaryView(this.dataGridViewItems);

                // 見出しの置き換えタブの保存
                this.config.HeadingTable = this.ExportTranslationTableView(this.dataGridViewHeading);

                // サーバー／言語タブの保存
                // ※ このタブはコンボボックス変更のタイミングで保存されるので、そのメソッドを呼ぶ
                this.ComboBoxLanguuage_SelectedIndexChanged(sender, e);

                // その他タブの保存
                Settings.Default.CacheExpire = new TimeSpan(int.Parse(this.textBoxCacheExpire.Text), 0, 0, 0);
                Settings.Default.UserAgent = this.textBoxUserAgent.Text;
                Settings.Default.Referer = this.textBoxReferer.Text;
                Settings.Default.IgnoreError = this.checkBoxIgnoreError.Checked;

                // 設定をファイルに保存
                Settings.Default.Save();
                try
                {
                    this.config.Save(Settings.Default.ConfigurationFile);

                    // 全部成功なら画面を閉じる
                    // ※ エラーの場合、どうしても駄目ならキャンセルボタンで閉じてもらう
                    this.Close();
                }
                catch (Exception ex)
                {
                    // 異常時はエラーメッセージを表示
                    // ※ この場合でもConfigオブジェクトは更新済みのため設定は一時的に有効
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    FormUtils.ErrorDialog(Resources.ErrorMessageConfigSaveFailed, ex.Message);
                }
            }
            catch (Exception ex)
            {
                // 通常ファイル保存以外では例外は発生しないはず。想定外のエラー用
                FormUtils.ErrorDialog(Resources.ErrorMessageDevelopmentError, ex.Message, ex.StackTrace);
            }
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

        /// <summary>
        /// 記事の置き換え対訳表のセル編集時のバリデート処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void DataGridViewItems_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // 取得日時列のみチェック
            if (this.dataGridViewItems.Columns[e.ColumnIndex].Name != "ColumnTimestamp")
            {
                return;
            }

            // 空または日付として認識可能な値の場合OK
            string value = e.FormattedValue.ToString();
            DateTime dummy;
            if (String.IsNullOrWhiteSpace(value) || DateTime.TryParse(value, out dummy))
            {
                return;
            }

            // 不許可値の場合、NGメッセージを表示
            this.dataGridViewItems.Rows[e.RowIndex].ErrorText = Resources.WarningMessageUnformatedTimestamp;
            e.Cancel = true;
        }

        /// <summary>
        /// 記事の置き換え対訳表のセル編集時のバリデート終了時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void DataGridViewItems_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            // 取得日時列の場合、バリデートNGメッセージを消す
            if (this.dataGridViewItems.Columns[e.ColumnIndex].Name == "ColumnTimestamp")
            {
                this.dataGridViewItems.Rows[e.RowIndex].ErrorText = String.Empty;
            }
        }

        /// <summary>
        /// 記事の置き換え対訳表のセル変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void DataGridViewItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 取得日時列が空の場合、有効期限が無期限として背景色を変更
            // ※ ただし全列が空（新規行など）の場合は無視
            if (e.RowIndex >= 0)
            {
                string value = FormUtils.ToString(this.dataGridViewItems["ColumnTimestamp", e.RowIndex]);
                if (String.IsNullOrWhiteSpace(value)
                    && !this.IsEmptyDataGridViewItemsRow(this.dataGridViewItems.Rows[e.RowIndex]))
                {
                    this.dataGridViewItems.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Bisque;
                }
                else
                {
                    this.dataGridViewItems.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.dataGridViewItems.DefaultCellStyle.BackColor;
                }
            }
        }

        /// <summary>
        /// 記事の置き換え対訳表の行編集時のバリデート処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void DataGridViewItems_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            // 翻訳元、記事名、翻訳先が未入力の場合、バリデートNGメッセージを表示
            // ※ ただし全列が空（新規行など）の場合は無視
            DataGridViewRow row = this.dataGridViewItems.Rows[e.RowIndex];
            if ((String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnFromCode"]))
                || String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnToCode"]))
                || String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnFromTitle"])))
                && !this.IsEmptyDataGridViewItemsRow(row))
            {
                row.ErrorText = Resources.WarningMessageEmptyTranslationDictionary;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 記事の置き換え対訳表の行編集時のバリデート終了時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void DataGridViewItems_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            // バリデートNGメッセージを消す
            this.dataGridViewItems.Rows[e.RowIndex].ErrorText = String.Empty;
        }

        /// <summary>
        /// 記事の置き換え対訳表を使用する<see cref="DataGridView"/>の値設定を行う。
        /// </summary>
        /// <param name="view">対訳表を表示するビュー。</param>
        /// <param name="dictionaries">対訳表データ。</param>
        private void ImportTranslationDictionaryView(DataGridView view, IList<TranslationDictionary> dictionaries)
        {
            // 初期設定以外の場合も想定して最初にクリア
            view.Rows.Clear();
            foreach (TranslationDictionary dic in dictionaries)
            {
                foreach (KeyValuePair<string, TranslationDictionary.Item> item in dic)
                {
                    // 行を追加しその行を取得
                    DataGridViewRow row = view.Rows[view.Rows.Add()];

                    // 1行分の初期値を設定。右矢印は別途イベントで追加すること
                    row.Cells["ColumnFromCode"].Value = dic.From;
                    row.Cells["ColumnFromTitle"].Value = item.Key;
                    row.Cells["ColumnAlias"].Value = item.Value.Alias;
                    row.Cells["ColumnToCode"].Value = dic.To;
                    row.Cells["ColumnToTitle"].Value = item.Value.Word;
                    if (item.Value.Timestamp.HasValue)
                    {
                        row.Cells["ColumnTimestamp"].Value = item.Value.Timestamp.Value.ToLocalTime().ToString("G");
                    }
                }
            }

            // 取得日時の降順でソート、空の列は先頭にする
            this.dataGridViewItems.Sort(new TranslationDictionaryViewComparer());
        }

        /// <summary>
        /// 記事の置き換え対訳表を使用する<see cref="DataGridView"/>からデータを抽出する。
        /// </summary>
        /// <param name="view">対訳表を表示するビュー。</param>
        /// <returns>対訳表データ。</returns>
        private IList<TranslationDictionary> ExportTranslationDictionaryView(DataGridView view)
        {
            IList<TranslationDictionary> dictionaries = new List<TranslationDictionary>();
            foreach (DataGridViewRow row in view.Rows)
            {
                // 画面での追加用の最終行が空で渡されてくるので無視
                if (this.IsEmptyDataGridViewItemsRow(row))
                {
                    continue;
                }

                // その行で対象とする言語を探索、無ければ新規作成
                string from = FormUtils.ToString(row.Cells["ColumnFromCode"]);
                string to = FormUtils.ToString(row.Cells["ColumnToCode"]);
                TranslationDictionary dic
                    = TranslationDictionary.GetDictionaryNeedCreate(dictionaries, from, to);

                // 値を格納
                TranslationDictionary.Item item = new TranslationDictionary.Item
                {
                    Word = FormUtils.ToString(row.Cells["ColumnToTitle"]),
                    Alias = FormUtils.ToString(row.Cells["ColumnAlias"])
                };

                string timestamp = FormUtils.ToString(row.Cells["ColumnTimestamp"]);
                if (!String.IsNullOrWhiteSpace(timestamp))
                {
                    item.Timestamp = DateTime.Parse(timestamp);

                    // UTCでもなくタイムゾーンでも無い場合、ローカル時刻として設定する
                    if (item.Timestamp.Value.Kind == DateTimeKind.Unspecified)
                    {
                        item.Timestamp = DateTime.SpecifyKind(item.Timestamp.Value, DateTimeKind.Local);
                    }
                }

                dic[FormUtils.ToString(row.Cells["ColumnFromTitle"])] = item;
            }

            return dictionaries;
        }
        
        /// <summary>
        /// 記事の置き換え対訳表の行が空かを判定する。
        /// </summary>
        /// <param name="row">対訳表の1行。</param>
        /// <returns>空の場合 true。</returns>
        private bool IsEmptyDataGridViewItemsRow(DataGridViewRow row)
        {
            return String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnFromCode"]))
                && String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnFromTitle"]))
                && String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnAlias"]))
                && String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnToCode"]))
                && String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnToTitle"]))
                && String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnTimestamp"]));
        }

        #endregion

        #region 見出しの置き換えタブのイベントのメソッド

        /// <summary>
        /// 見出しの置き換え対訳表を使用する<see cref="DataGridView"/>の値設定を行う。
        /// </summary>
        /// <param name="view">対訳表を表示するビュー。</param>
        /// <param name="table">対訳表データ。</param>
        private void ImportTranslationTableView(DataGridView view, TranslationTable table)
        {
            // 初期設定以外の場合も想定して最初にクリア
            view.Columns.Clear();

            // 言語コードを列、語句を行とする。登録されている全言語分の列を作成する
            foreach (Website site in this.config.Websites)
            {
                this.AddTranslationTableColumn(view.Columns, site.Language.Code, this.GetHeaderLanguage(site.Language));
            }

            // 各行にデータを取り込み
            foreach (IDictionary<string, string> record in table)
            {
                // 行を追加しその行を取得
                DataGridViewRow row = view.Rows[view.Rows.Add()];

                foreach (KeyValuePair<string, string> cell in record)
                {
                    // 上で登録した列では足りなかった場合、その都度生成する
                    if (!view.Columns.Contains(cell.Key))
                    {
                        this.AddTranslationTableColumn(view.Columns, cell.Key, cell.Key);
                    }

                    row.Cells[cell.Key].Value = cell.Value;
                }
            }
        }

        /// <summary>
        /// 見出しの置き換え対訳表を使用する<see cref="DataGridView"/>からデータを抽出する。
        /// </summary>
        /// <param name="view">対訳表を表示するビュー。</param>
        /// <returns>対訳表データ。</returns>
        private TranslationTable ExportTranslationTableView(DataGridView view)
        {
            TranslationTable table = new TranslationTable();
            foreach (DataGridViewRow row in view.Rows)
            {
                IDictionary<string, string> record = new SortedDictionary<string, string>();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    // 空のセルは格納しない、該当の組み合わせは消える
                    string value = FormUtils.ToString(cell);
                    if (!String.IsNullOrWhiteSpace(value))
                    {
                        record[cell.OwningColumn.Name] = value;
                    }
                }

                // 1件もデータが無い行は丸々カットする
                if (record.Count > 0)
                {
                    table.Add(record);
                }
            }

            return table;
        }

        /// <summary>
        /// 指定された情報を元に見出しの置き換え対訳表の列を追加する。
        /// </summary>
        /// <param name="columns">列コレクション。</param>
        /// <param name="columnName">列名。</param>
        /// <param name="headerText">列見出し。</param>
        private void AddTranslationTableColumn(DataGridViewColumnCollection columns, string columnName, string headerText)
        {
            columns.Add(columnName, headerText);
            columns[columnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        /// <summary>
        /// 指定された言語用の表示名を返す。
        /// </summary>
        /// <param name="lang">表示言語コード。</param>
        /// <returns>表示名、無ければ言語コード。</returns>
        private string GetHeaderLanguage(Language lang)
        {
            Language.LanguageName name;
            if (lang.Names.TryGetValue(
                System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName, out name))
            {
                if (!String.IsNullOrEmpty(name.Name))
                {
                    return String.Format(Resources.HeadingViewHeaderText, name.Name, lang.Code);
                }
            }

            return lang.Code;
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
            try
            {
                // 変更前の設定を保存
                if (!String.IsNullOrEmpty(this.comboBoxLanguageSelectedText))
                {
                    // 設定が存在しなければ自動生成される
                    this.SaveChangedValue(this.GetMediaWikiNeedCreate(this.config.Websites, this.comboBoxLanguageSelectedText));
                }

                // 変更後の値に応じて、画面表示を更新
                string code = ObjectUtils.ToString(this.comboBoxLanguage.SelectedItem).Trim();
                if (!String.IsNullOrEmpty(code))
                {
                    // 設定が存在しなければ基本的に自動生成されるのでそのまま使用
                    this.LoadCurrentValue(this.GetMediaWikiNeedCreate(this.config.Websites, code));

                    // 各入力欄を有効に
                    this.groupBoxServer.Enabled = true;
                    this.groupBoxLanguage.Enabled = true;

                    // 現在の選択値を更新
                    this.comboBoxLanguageSelectedText = code;
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
            catch (Exception ex)
            {
                // 通常この処理では例外は発生しないはず。想定外のエラー用
                FormUtils.ErrorDialog(Resources.ErrorMessageDevelopmentError, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 言語コンボボックスキー入力時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ComboBoxLanguage_KeyDown(object sender, KeyEventArgs e)
        {
            // エンターキーが押された場合、現在の値が一覧に無ければ登録する（フォーカスを失ったときの処理）
            if (e.KeyCode == Keys.Enter)
            {
                System.Diagnostics.Debug.WriteLine("ComboBoxLanguage::_KeyDown > " + this.comboBoxLanguage.Text);
                this.ComboBoxLanguage_Leave(sender, e);
            }
        }

        /// <summary>
        /// 言語コンボボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ComboBoxLanguage_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ComboBoxLanguage::_Leave > " + this.comboBoxLanguage.Text);

            // 現在の値が一覧に無ければ登録する
            this.comboBoxLanguage.Text = this.comboBoxLanguage.Text.Trim().ToLower();
            if (String.IsNullOrEmpty(this.comboBoxLanguage.Text))
            {
                this.comboBoxLanguage.Items.Add(this.comboBoxLanguage.Text);

                // 登録した場合、見出しの対訳表にも列を追加
                this.dataGridViewHeading.Columns.Add(this.comboBoxLanguage.Text, this.comboBoxLanguage.Text);

                // 登録した値を選択状態に変更
                this.comboBoxLanguage.SelectedItem = this.comboBoxLanguage.Text;
            }
            else
            {
                // 空にしたとき、変更でイベントが起こらないようなので、強制的に呼ぶ
                this.ComboBoxLanguuage_SelectedIndexChanged(sender, e);
            }
        }

        /// <summary>
        /// テンプレート名前空間のIDボックスフォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxTemplateNamespace_Leave(object sender, EventArgs e)
        {
            // 空か数値のみ許可
            this.textBoxTemplateNamespace.Text = StringUtils.DefaultString(this.textBoxTemplateNamespace.Text).Trim();
            int value;
            if (!String.IsNullOrEmpty(textBoxTemplateNamespace.Text) && !int.TryParse(this.textBoxTemplateNamespace.Text, out value))
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
            if (!String.IsNullOrEmpty(textBoxCategoryNamespace.Text) && !int.TryParse(this.textBoxCategoryNamespace.Text, out value))
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
            if (!String.IsNullOrEmpty(textBoxFileNamespace.Text) && !int.TryParse(this.textBoxFileNamespace.Text, out value))
            {
                FormUtils.WarningDialog(Resources.WarningMessageNamespaceNumberValue);
                this.textBoxFileNamespace.Focus();
            }
        }

        /// <summary>
        /// 言語の設定表フォーカス喪失時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void DataGridViewLanguageName_Leave(object sender, EventArgs e)
        {
            // 値チェック
            string codeUnsetRows = String.Empty;
            string nameUnsetRows = String.Empty;
            string redundantCodeRows = String.Empty;
            for (int y = 0; y < this.dataGridViewLanguageName.RowCount - 1; y++)
            {
                string code = FormUtils.ToString(this.dataGridViewLanguageName["ColumnCode", y]).Trim().ToLower();

                // 言語コードが設定されていない行があるか？
                if (String.IsNullOrEmpty(code))
                {
                    if (!String.IsNullOrEmpty(codeUnsetRows))
                    {
                        codeUnsetRows += ",";
                    }

                    codeUnsetRows += y + 1;
                }
                else
                {
                    // 言語コード列は、小文字のデータに変換
                    this.dataGridViewLanguageName["ColumnCode", y].Value = code;

                    // チェック済みの行に言語コードが重複したものがないか？
                    for (int i = 0; i < y; i++)
                    {
                        if (FormUtils.ToString(this.dataGridViewLanguageName["ColumnCode", i]) == code)
                        {
                            if (!String.IsNullOrEmpty(redundantCodeRows))
                            {
                                redundantCodeRows += ",";
                            }

                            redundantCodeRows += y + 1;
                            break;
                        }
                    }

                    // 呼称が設定されていないのに略称が設定されていないか？
                    if (String.IsNullOrWhiteSpace(FormUtils.ToString(this.dataGridViewLanguageName["ColumnShortName", y]))
                        && String.IsNullOrWhiteSpace(FormUtils.ToString(this.dataGridViewLanguageName["ColumnName", y])))
                    {
                        if (!String.IsNullOrEmpty(nameUnsetRows))
                        {
                            nameUnsetRows += ",";
                        }

                        nameUnsetRows += y + 1;
                    }
                }
            }

            // 結果の表示
            string errorMessage = String.Empty;
            if (!String.IsNullOrEmpty(codeUnsetRows))
            {
                errorMessage += String.Format(Resources.WarningMessage_UnsetCodeColumn, codeUnsetRows);
            }

            if (!String.IsNullOrEmpty(redundantCodeRows))
            {
                if (!String.IsNullOrEmpty(errorMessage))
                {
                    errorMessage += "\n";
                }

                errorMessage += String.Format(Resources.WarningMessage_RedundantCodeColumn, redundantCodeRows);
            }

            if (!String.IsNullOrEmpty(nameUnsetRows))
            {
                if (!String.IsNullOrEmpty(errorMessage))
                {
                    errorMessage += "\n";
                }

                errorMessage += String.Format(Resources.WarningMessage_UnsetArticleNameColumn, nameUnsetRows);
            }

            if (!String.IsNullOrEmpty(errorMessage))
            {
                FormUtils.WarningDialog(errorMessage);
                this.dataGridViewLanguageName.Focus();
            }
        }

        #region イベント実装支援用メソッド

        /// <summary>
        /// コレクションから指定された言語のMediaWikiを取得する。
        /// 存在しない場合は空のインスタンスを生成、コレクションに追加して返す。
        /// </summary>
        /// <param name="collection">翻訳元言語。</param>
        /// <param name="lang">言語コード。</param>
        /// <returns>翻訳パターン。存在しない場合は新たに作成した翻訳パターンを返す。</returns>
        private MediaWiki GetMediaWikiNeedCreate(ICollection<Website> collection, string lang)
        {
            // 設定が存在すれば取得した値を返す
            foreach (Website s in collection)
            {
                if (s.Language.Code == lang)
                {
                    if (s is MediaWiki)
                    {
                        return s as MediaWiki;
                    }

                    // 万が一同じ言語コードで違う型の値があったら上書き
                    collection.Remove(s);
                    break;
                }
            }

            // 存在しないか上書きの場合、作成した翻訳パターンをコレクションに追加し、返す
            MediaWiki site = new MediaWiki(new Language(lang));
            collection.Add(site);
            return site;
        }

        /// <summary>
        /// 指定されたLanguage設定を画面表示／編集用に読み込む。
        /// </summary>
        /// <param name="lang">読込元Language設定。</param>
        /// <remarks>一部パラメータには初期値が存在するが、格納時に対処するため全て読み込む。</remarks>
        private void LoadCurrentValue(Language lang)
        {
            // 言語情報を読み込み
            // ※ Bracketは初期値があるパラメータのため、必ず値が返る
            this.textBoxBracket.Text = lang.Bracket;

            // 呼称の情報を表に設定
            this.dataGridViewLanguageName.Rows.Clear();
            foreach (KeyValuePair<string, Language.LanguageName> name in lang.Names)
            {
                int index = this.dataGridViewLanguageName.Rows.Add();
                this.dataGridViewLanguageName["ColumnCode", index].Value = name.Key;
                this.dataGridViewLanguageName["ColumnName", index].Value = name.Value.Name;
                this.dataGridViewLanguageName["ColumnShortName", index].Value = name.Value.ShortName;
            }
        }

        /// <summary>
        /// 指定されたWebsite設定を画面表示／編集用に読み込む。
        /// </summary>
        /// <param name="site">読込元Website設定。</param>
        private void LoadCurrentValue(Website site)
        {
            // Languageクラス分の読み込みを行う
            this.LoadCurrentValue(site.Language);

            // サイト情報を読み込み
            this.textBoxLocation.Text = site.Location;
        }

        /// <summary>
        /// 指定されたMediaWiki設定を画面表示／編集用に読み込む。
        /// </summary>
        /// <param name="site">読込元MediaWiki設定。</param>
        /// <remarks>一部パラメータには初期値が存在するが、格納時に対処するため全て読み込む。</remarks>
        private void LoadCurrentValue(MediaWiki site)
        {
            // Websiteクラス分の読み込みを行う
            this.LoadCurrentValue((Website)site);

            // MediaWikiクラス分の読み込み
            this.textBoxExportPath.Text = StringUtils.DefaultString(site.ExportPath);
            this.textBoxNamespacePath.Text = StringUtils.DefaultString(site.NamespacePath);
            this.textBoxTemplateNamespace.Text = site.TemplateNamespace.ToString();
            this.textBoxCategoryNamespace.Text = site.CategoryNamespace.ToString();
            this.textBoxFileNamespace.Text = site.FileNamespace.ToString();
            this.textBoxRedirect.Text = StringUtils.DefaultString(site.Redirect);
            this.textBoxDocumentationTemplate.Text = StringUtils.DefaultString(site.DocumentationTemplate);
            this.textBoxDocumentationTemplateDefaultPage.Text = StringUtils.DefaultString(site.DocumentationTemplateDefaultPage);
        }

        /// <summary>
        /// 指定されたLanguage設定に画面上で変更された値の格納を行う。
        /// </summary>
        /// <param name="lang">格納先Language設定。</param>
        /// <remarks>一部パラメータには初期値が存在するため、変更がある場合のみ格納する。</remarks>
        private void SaveChangedValue(Language lang)
        {
            // Bracketは初期値を持つパラメータのため、変更された場合のみ格納する。
            // ※ この値は前後の空白に意味があるため、Trimしてはいけない
            string str = StringUtils.DefaultString(this.textBoxBracket.Text);
            if (str != lang.Bracket)
            {
                lang.Bracket = str;
            }

            // 表から呼称の情報も保存
            this.dataGridViewLanguageName.Sort(this.dataGridViewLanguageName.Columns["ColumnCode"], ListSortDirection.Ascending);
            lang.Names.Clear();
            for (int y = 0; y < this.dataGridViewLanguageName.RowCount - 1; y++)
            {
                // 値が入ってないとかはガードしているはずだが、一応チェック
                string code = FormUtils.ToString(this.dataGridViewLanguageName["ColumnCode", y]).Trim();
                if (!String.IsNullOrEmpty(code))
                {
                    Language.LanguageName name = new Language.LanguageName();
                    name.Name = FormUtils.ToString(this.dataGridViewLanguageName["ColumnName", y]).Trim();
                    name.ShortName = FormUtils.ToString(this.dataGridViewLanguageName["ColumnShortName", y]).Trim();
                    lang.Names[code] = name;
                }
            }
        }

        /// <summary>
        /// 指定されたWebsite設定に画面上で変更された値の格納を行う。
        /// </summary>
        /// <param name="site">格納先Website設定。</param>
        /// <remarks>Websiteについては特に特殊な処理は無いため全て上書きする。</remarks>
        private void SaveChangedValue(Website site)
        {
            // Languageクラス分の設定を行う
            this.SaveChangedValue(site.Language);

            // サイト情報を格納
            site.Location = StringUtils.DefaultString(this.textBoxLocation.Text).Trim();
        }

        /// <summary>
        /// 指定されたMediaWiki設定に画面上で変更された値の格納を行う。
        /// </summary>
        /// <param name="site">格納先MediaWiki設定。</param>
        /// <remarks>一部パラメータには初期値が存在するため、変更がある場合のみ格納する。</remarks>
        private void SaveChangedValue(MediaWiki site)
        {
            // Websiteクラス分の設定を行う
            this.SaveChangedValue((Website)site);

            // 初期値を持つパラメータがあるため、全て変更された場合のみ格納する。
            // ※ もうちょっと綺麗に書きたかったが、リフレクションを使わないと共通化できなさそうだったので力技
            //    MediaWikiクラス側で行わないのは、場合によっては意図的に初期値と同じ値を設定すること
            //    もありえるから（初期値が変わる可能性がある場合など）。
            string str = StringUtils.DefaultString(this.textBoxExportPath.Text).Trim();
            if (str != site.ExportPath)
            {
                site.ExportPath = str;
            }
            
            str = StringUtils.DefaultString(this.textBoxNamespacePath.Text).Trim();
            if (str != site.NamespacePath)
            {
                site.NamespacePath = str;
            }

            str = StringUtils.DefaultString(this.textBoxRedirect.Text).Trim();
            if (str != site.Redirect)
            {
                site.Redirect = str;
            }

            str = StringUtils.DefaultString(this.textBoxDocumentationTemplate.Text).Trim();
            if (str != site.DocumentationTemplate)
            {
                site.DocumentationTemplate = str;
            }

            str = StringUtils.DefaultString(this.textBoxDocumentationTemplateDefaultPage.Text).Trim();
            if (str != site.DocumentationTemplateDefaultPage)
            {
                site.DocumentationTemplateDefaultPage = str;
            }

            // 以下、数値へのparseは事前にチェックしてあるので、ここではチェックしない
            if (!String.IsNullOrWhiteSpace(this.textBoxTemplateNamespace.Text))
            {
                int num = int.Parse(this.textBoxTemplateNamespace.Text);
                if (site.TemplateNamespace != num)
                {
                    site.TemplateNamespace = num;
                }
            }

            if (!String.IsNullOrWhiteSpace(this.textBoxCategoryNamespace.Text))
            {
                int num = int.Parse(this.textBoxCategoryNamespace.Text);
                if (site.CategoryNamespace != num)
                {
                    site.CategoryNamespace = num;
                }
            }

            if (!String.IsNullOrWhiteSpace(this.textBoxFileNamespace.Text))
            {
                int num = int.Parse(this.textBoxFileNamespace.Text);
                if (site.FileNamespace != num)
                {
                    site.FileNamespace = num;
                }
            }
        }

        #endregion

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

        #region "内部クラス"
        
        /// <summary>
        /// 記事の置き換え対訳表の日付並び替え用クラスです。
        /// </summary>
        /// <remarks>取得日時の降順でソート、空の列は先頭にします。</remarks>
        public class TranslationDictionaryViewComparer : System.Collections.IComparer
        {
            /// <summary>
            /// 2行を比較し、一方が他方より小さいか、等しいか、大きいかを示す値を返します。
            /// </summary>
            /// <param name="y">比較する最初の行です。</param>
            /// <param name="x">比較する 2 番目の行。</param>
            /// <returns>1以下:xはyより小さい, 0:等しい, 1以上:xはyより大きい</returns>
            public int Compare(object y, object x)
            {
                string xstr = ObjectUtils.ToString(((DataGridViewRow)x).Cells["ColumnTimestamp"].Value);
                string ystr = ObjectUtils.ToString(((DataGridViewRow)y).Cells["ColumnTimestamp"].Value);
                if (String.IsNullOrWhiteSpace(xstr) && String.IsNullOrWhiteSpace(ystr))
                {
                    return 0;
                }
                else if (String.IsNullOrWhiteSpace(xstr))
                {
                    return 1;
                }
                else if (String.IsNullOrWhiteSpace(ystr))
                {
                    return -1;
                }

                return xstr.CompareTo(ystr);
            }
        }

        #endregion
    }
}
