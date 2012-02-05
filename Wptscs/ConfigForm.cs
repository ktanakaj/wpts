// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツール設定画面クラスソース</summary>
//
// <copyright file="ConfigForm.cs" company="honeplusのメモ帳">
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
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;
    using Honememo.Wptscs.Utilities;
    using Honememo.Wptscs.Websites;

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
            this.config = Honememo.Utilities.Validate.NotNull(config, "config");
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
                    this.config.Save();

                    // 全部成功なら画面を閉じる
                    // ※ エラーの場合、どうしても駄目ならキャンセルボタンで閉じてもらう
                    this.Close();
                }
                catch (Exception ex)
                {
                    // 異常時はエラーメッセージを表示
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
        /// 記事の置き換え対訳表のセル編集時のバリデート成功時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void DataGridViewItems_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            // 取得日時列の場合、バリデートNGメッセージを消す
            // ※ 他の列で消さないのは、エラーを出しているのがRowValidatingの場合もあるから
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

            // 列幅をデータ長に応じて自動調整
            // ※ 常に行ってしまうと、読み込みに非常に時間がかかるため
            view.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
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
        /// <returns>空の場合<c>true</c>。</returns>
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

            // 列幅をデータ長に応じて自動調整
            // ※ 常に行ってしまうと、読み込みに時間がかかるため
            view.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
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
                if (!String.IsNullOrEmpty(this.comboBoxLanguage.Text))
                {
                    // 設定が存在しなければ基本的に自動生成されるのでそのまま使用
                    this.LoadCurrentValue(this.GetMediaWikiNeedCreate(this.config.Websites, this.comboBoxLanguage.Text));

                    // 各入力欄を有効に
                    this.buttonLanguageRemove.Enabled = true;
                    this.groupBoxServer.Enabled = true;
                    this.groupBoxLanguage.Enabled = true;

                    // 現在の選択値を更新
                    this.comboBoxLanguageSelectedText = this.comboBoxLanguage.Text;
                }
                else
                {
                    // 各入力欄を無効に
                    this.buttonLanguageRemove.Enabled = false;
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
        /// 言語の追加ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonLunguageAdd_Click(object sender, EventArgs e)
        {
            // 言語追加用ダイアログを表示
            InputLanguageCodeDialog form = new InputLanguageCodeDialog(this.config);
            form.ShowDialog();

            // 値が登録された場合
            if (!String.IsNullOrWhiteSpace(form.LanguageCode))
            {
                // 値を一覧・見出しの対訳表に追加、登録した値を選択状態に変更
                this.comboBoxLanguage.Items.Add(form.LanguageCode);
                this.dataGridViewHeading.Columns.Add(form.LanguageCode, form.LanguageCode);
                this.comboBoxLanguage.SelectedItem = form.LanguageCode;
            }
        }

        /// <summary>
        /// 言語の削除ボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonLanguageRemove_Click(object sender, EventArgs e)
        {
            // 表示されている言語を設定から削除する
            for (int i = this.config.Websites.Count - 1; i >= 0; i--)
            {
                if (this.config.Websites[i].Language.Code == this.comboBoxLanguage.Text)
                {
                    // 万が一複数あれば全て削除
                    this.config.Websites.RemoveAt(i);
                }
            }

            // コンボボックスからも削除し、表示を更新する
            this.comboBoxLanguageSelectedText = null;
            this.comboBoxLanguage.Items.Remove(this.comboBoxLanguage.Text);
            this.ComboBoxLanguuage_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// 各名前空間のIDボックスバリデート処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxNamespace_Validating(object sender, CancelEventArgs e)
        {
            // 空か数値のみ許可
            TextBox box = (TextBox)sender;
            box.Text = StringUtils.DefaultString(box.Text).Trim();
            int value;
            if (!String.IsNullOrEmpty(box.Text) && !int.TryParse(box.Text, out value))
            {
                this.errorProvider.SetError(box, Resources.WarningMessageIgnoreNumericNamespace);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 言語の設定表の行編集時のバリデート処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void DataGridViewLanguageName_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow row = this.dataGridViewLanguageName.Rows[e.RowIndex];

            // 空行（新規行など）の場合無視
            if (FormUtils.IsEmptyRow(row))
            {
                return;
            }

            // 言語コードは必須、またトリムして小文字に変換
            string code = FormUtils.ToString(row.Cells["ColumnCode"]).Trim().ToLower();
            row.Cells["ColumnCode"].Value = code;
            if (String.IsNullOrEmpty(code))
            {
                row.ErrorText = Resources.WarningMessageEmptyCodeColumn;
                e.Cancel = true;
                return;
            }

            // 略称を設定する場合、呼称を必須とする
            if (!String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnShortName"]))
                && String.IsNullOrWhiteSpace(FormUtils.ToString(row.Cells["ColumnName"])))
            {
                row.ErrorText = Resources.WarningMessageShortNameColumnOnly;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 言語の設定表バリデート処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void DataGridViewLanguageName_Validating(object sender, CancelEventArgs e)
        {
            // 言語コードの重複チェック
            IDictionary<string, int> codeMap = new Dictionary<string, int>();
            for (int i = 0; i < this.dataGridViewLanguageName.RowCount - 1; i++)
            {
                string code = FormUtils.ToString(this.dataGridViewLanguageName["ColumnCode", i]);
                int y;
                if (codeMap.TryGetValue(code, out y))
                {
                    // 重複の場合、両方の行にエラーを設定
                    this.dataGridViewLanguageName.Rows[i].ErrorText = Resources.WarningMessageDuplicateCodeColumn;
                    this.dataGridViewLanguageName.Rows[y].ErrorText = Resources.WarningMessageDuplicateCodeColumn;
                    e.Cancel = true;
                }
                else
                {
                    // それ以外はマップに出現行とともに追加
                    codeMap[code] = i;
                }
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

            // Template:Documentionは改行区切りのマルチテキストとして扱う
            StringBuilder b = new StringBuilder();
            foreach (string s in site.DocumentationTemplates)
            {
                b.Append(s).Append(Environment.NewLine);
            }

            this.textBoxDocumentationTemplate.Text = b.ToString();
            this.textBoxDocumentationTemplateDefaultPage.Text = StringUtils.DefaultString(site.DocumentationTemplateDefaultPage);
            this.textBoxLinkInterwikiFormat.Text = StringUtils.DefaultString(site.LinkInterwikiFormat);
            this.textBoxLangFormat.Text = StringUtils.DefaultString(site.LangFormat);
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
            // ※ もうちょっと綺麗に書きたかったが、うまい手が思いつかなかったので力技
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

            // Template:Documentionの設定は行ごとに格納
            // ※ この値は初期値を持たないパラメータ
            site.DocumentationTemplates.Clear();
            foreach (string s in StringUtils.DefaultString(this.textBoxDocumentationTemplate.Text).Split('\n'))
            {
                if (!String.IsNullOrWhiteSpace(s))
                {
                    site.DocumentationTemplates.Add(s.Trim());
                }
            }

            str = StringUtils.DefaultString(this.textBoxDocumentationTemplateDefaultPage.Text).Trim();
            if (str != site.DocumentationTemplateDefaultPage)
            {
                site.DocumentationTemplateDefaultPage = str;
            }

            str = StringUtils.DefaultString(this.textBoxLinkInterwikiFormat.Text).Trim();
            if (str != site.LinkInterwikiFormat)
            {
                site.LinkInterwikiFormat = str;
            }

            str = StringUtils.DefaultString(this.textBoxLangFormat.Text).Trim();
            if (str != site.LangFormat)
            {
                site.LangFormat = str;
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
        /// キャッシュ有効期限ボックスバリデート処理。。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void TextBoxCacheExpire_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            box.Text = StringUtils.DefaultString(box.Text).Trim();
            int expire;
            if (!int.TryParse(box.Text, out expire) || expire < 0)
            {
                this.errorProvider.SetError(box, Resources.WarningMessageIgnoreCacheExpire);
                e.Cancel = true;
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
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }

        #endregion

        #region 共通のイベントメソッド
        
        /// <summary>
        /// 汎用のエラープロバイダ初期化処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ResetErrorProvider_Validated(object sender, EventArgs e)
        {
            this.errorProvider.SetError((Control)sender, null);
        }

        /// <summary>
        /// 汎用の行編集時のエラーテキスト初期化処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ResetErrorText_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            ((DataGridView)sender).Rows[e.RowIndex].ErrorText = String.Empty;
        }

        /// <summary>
        /// 汎用のテーブルエラーテキスト初期化処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ResetErrorText_Validated(object sender, EventArgs e)
        {
            // 全行のエラーメッセージを解除
            foreach (DataGridViewRow row in ((DataGridView)sender).Rows)
            {
                row.ErrorText = String.Empty;
            }
        }

        #endregion

        #region 内部クラス

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
