// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツール設定画面クラスソース</summary>
//
// <copyright file="ConfigWikipediaDialog.cs" company="honeplusのメモ帳">
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
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// Wikipedia翻訳支援ツール設定画面のクラスです。
    /// </summary>
    public partial class ConfigWikipediaDialog : Form
    {
        #region private変数

        /// <summary>
        /// 共通関数クラスのオブジェクト。
        /// </summary>
        private Honememo.Cmn cmnAP;

        /// <summary>
        /// 各種設定。
        /// </summary>
        private Config config;

        /// <summary>
        /// comboBoxColumnで選択していたアイテムのバックアップ。
        /// </summary>
        private string comboBoxCodeSelectedText;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。初期化メソッド呼び出しのみ。
        /// </summary>
        public ConfigWikipediaDialog()
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
        private void ConfigWikipediaDialog_Load(object sender, EventArgs e)
        {
            // 初期化処理
            this.cmnAP = new Honememo.Cmn();
            this.config = Config.GetInstance();

            // データ設定
            this.comboBoxCodeSelectedText = String.Empty;
            this.comboBoxCode.Items.Clear();
            this.dataGridViewName.Rows.Clear();
            this.dataGridViewTitleKey.Columns.Clear();

            // 使用言語取得
            string showCode = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            int x = 0;
            if (this.config.Configs.ContainsKey(this.config.Mode))
            {
                // 設定ファイルに存在する全言語を選択肢として登録する
                foreach (Website site in this.config.Configs[this.config.Mode].Websites)
                {
                    // 表タイトル設定
                    Language lang = Config.GetInstance().GetLanguage(site.Language);
                    string name = site.Language;
                    if (lang != null && !String.IsNullOrEmpty(lang.Names[showCode].Name))
                    {
                        name = lang.Names[showCode].Name + " (" + site.Language + ")";
                    }

                    this.dataGridViewTitleKey.Columns.Add(site.Language, name);

                    // 表データ設定
                    if (site as MediaWiki != null)
                    {
                        foreach (KeyValuePair<int, string> title in (site as MediaWiki).Headings)
                        {
                            while (this.dataGridViewTitleKey.RowCount - 1 <= title.Key)
                            {
                                this.dataGridViewTitleKey.Rows.Add();
                            }

                            this.dataGridViewTitleKey[x, title.Key].Value = title.Value;
                        }
                    }

                    // コンボボックス設定
                    this.comboBoxCode.Items.Add(site.Language);

                    // 次の列へ
                    ++x;
                }
            }

            this.dataGridViewTitleKey.CurrentCell = null;
        }

        /// <summary>
        /// 言語コードコンボボックス変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void comboBoxCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ConfigLanguageDialog._SelectedIndexChanged > "
                + this.comboBoxCodeSelectedText + " . "
                + (this.comboBoxCode.SelectedItem != null ? this.comboBoxCode.SelectedItem.ToString() : String.Empty));

            // 変更前の設定を保存
            // ※変更前にしろ変更後にしろ、事前に追加しているのでGetLanguageで見つからないことは無い・・・はず
            if (!String.IsNullOrEmpty(this.comboBoxCodeSelectedText))
            {
                MediaWiki svr = this.config.GetWebsite(this.comboBoxCodeSelectedText) as MediaWiki;
                if (svr != null)
                {
                    svr.ExportPath = textBoxXml.Text.Trim();
                    svr.Redirect = textBoxRedirect.Text.Trim();

                    // 表から呼称の情報も保存
                    this.dataGridViewName.Sort(this.dataGridViewName.Columns["Code"], ListSortDirection.Ascending);
                    Language lang = Config.GetInstance().GetLanguage(svr.Language);
                    if (lang == null)
                    {
                        lang = new Language(svr.Language);
                    }
                    lang.Names.Clear();
                    for (int y = 0; y < this.dataGridViewName.RowCount - 1; y++)
                    {
                        // 値が入ってないとかはガードしているはずだが、一応チェック
                        string code = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["Code", y]);
                        if (!String.IsNullOrEmpty(code))
                        {
                            Language.LanguageName name = new Language.LanguageName();
                            name.Name = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["ArticleName", y]);
                            name.ShortName = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["ShortName", y]);
                            lang.Names.Add(code, name);
                        }
                    }
                }
            }

            // 変更後の値に応じて、画面表示を更新
            if (comboBoxCode.SelectedItem != null)
            {
                // 値を設定
                MediaWiki svr = this.config.GetWebsite(comboBoxCode.SelectedItem.ToString()) as MediaWiki;
                if (svr != null)
                {
                    this.textBoxXml.Text = svr.ExportPath;
                    this.textBoxRedirect.Text = svr.Redirect;

                    // 呼称の情報を表に設定
                    this.dataGridViewName.Rows.Clear();
                    Language lang = Config.GetInstance().GetLanguage(svr.Language);
                    if (lang == null)
                    {
                        lang = new Language(svr.Language);
                    }

                    foreach (KeyValuePair<string, Language.LanguageName> name in lang.Names)
                    {
                        int index = this.dataGridViewName.Rows.Add();
                        this.dataGridViewName["Code", index].Value = name.Key;
                        this.dataGridViewName["ArticleName", index].Value = name.Value.Name;
                        this.dataGridViewName["ShortName", index].Value = name.Value.ShortName;
                    }
                }

                // 言語のプロパティを有効に
                this.groupBoxStyle.Enabled = true;
                this.groupBoxName.Enabled = true;

                // 現在の選択値を更新
                this.comboBoxCodeSelectedText = this.comboBoxCode.SelectedItem.ToString();
            }
            else
            {
                // 言語のプロパティを無効に
                this.groupBoxStyle.Enabled = false;
                this.groupBoxName.Enabled = false;

                // 現在の選択値を更新
                this.comboBoxCodeSelectedText = String.Empty;
            }
        }

        /// <summary>
        /// 言語コードコンボボックスでのキー入力時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void comboBoxCode_KeyDown(object sender, KeyEventArgs e)
        {
            // エンターキーが押された場合、現在の値が一覧に無ければ登録する（フォーカスを失ったときの処理）
            if (e.KeyCode == Keys.Enter)
            {
                System.Diagnostics.Debug.WriteLine("ConfigLanguageDialog._KeyDown > " + this.comboBoxCode.Text);
                this.comboBoxCode_Leave(sender, e);
            }
        }

        /// <summary>
        /// 言語コードコンボボックスからフォーカスを離した時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void comboBoxCode_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ConfigLanguageDialog._Leave > " + comboBoxCode.Text);

            // 現在の値が一覧に無ければ登録する
            this.comboBoxCode.Text = this.comboBoxCode.Text.Trim().ToLower();
            if (!String.IsNullOrEmpty(this.comboBoxCode.Text))
            {
                if (Honememo.Cmn.AddComboBoxNewItem(ref this.comboBoxCode) == true)
                {
                    // 登録した場合メンバ変数にも登録
                    MediaWiki svr = this.config.GetWebsite(this.comboBoxCode.Text) as MediaWiki;

                    // 存在しないはずだが一応は確認して追加
                    if (svr == null)
                    {
                        svr = new MediaWiki(this.comboBoxCode.Text);
                        if (!this.config.Configs.ContainsKey(Config.RunMode.Wikipedia))
                        {
                            this.config.Configs[Config.RunMode.Wikipedia] = new Config.ModeConfig();
                        }

                        this.config.Configs[Config.RunMode.Wikipedia].Websites.Add(svr);

                        // 定型句の設定表に列を追加
                        this.dataGridViewTitleKey.Columns.Add(this.comboBoxCode.Text, this.comboBoxCode.Text);
                    }

                    // 登録した値を選択状態に変更
                    this.comboBoxCode.SelectedItem = this.comboBoxCode.Text;
                }
            }
            else
            {
                // 空にしたとき、変更でイベントが起こらないようなので、強制的に呼ぶ
                this.comboBoxCode_SelectedIndexChanged(sender, e);
            }
        }

        /// <summary>
        /// 言語コードコンボボックスのコンテキストメニュー：言語コードを変更。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void toolStripMenuItemModify_Click(object sender, EventArgs e)
        {
            // 選択されている言語コードに関連する情報を更新
            if (this.comboBoxCode.SelectedIndex != -1)
            {
                string oldCode = this.comboBoxCode.SelectedItem.ToString();

                // 入力画面にて変更後の言語コードを取得
                InputLanguageCodeDialog dialog = new InputLanguageCodeDialog();
                dialog.LanguageCode = oldCode;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string newCode = dialog.LanguageCode;

                    // その言語のコードを更新
                    Website site = this.config.GetWebsite(oldCode);
                    if (site != null)
                    {
//                        site.Lang.Code = newCode;
                    }

                    // そのコードを参照している言語コードを更新
                    if (this.config.Configs.ContainsKey(this.config.Mode))
                    {
                        foreach (Website s in this.config.Configs[this.config.Mode].Websites)
                        {
                            // もし新しいコードが既に存在する場合は上書き
//                            s.Lang.Names[newCode] = site.Lang.Names[oldCode];
//                            s.Lang.Names.Remove(oldCode);
                        }
                    }

                    // コンボボックスを更新
                    int index = this.comboBoxCode.Items.IndexOf(comboBoxCode.SelectedItem);
                    this.comboBoxCode.Items[index] = newCode;

                    // 定型句の設定表を更新
                    //string header = site.Lang.Names[System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName].Name;
                    //if (!String.IsNullOrEmpty(header))
                    //{
                    //    header += " (" + newCode + ")";
                    //}
                    //else
                    //{
                    //    header = newCode;
                    //}

                    //this.dataGridViewTitleKey.Columns[oldCode].HeaderText = header;
                    this.dataGridViewTitleKey.Columns[oldCode].Name = newCode;

                    // 画面の状態を更新
                    this.comboBoxCode_SelectedIndexChanged(sender, e);
                }
            }
        }

        /// <summary>
        /// 言語コードコンボボックスのコンテキストメニュー：言語を削除。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            // 選択されている言語コードに関連する情報を削除
            if (this.comboBoxCode.SelectedIndex != -1)
            {
                this.dataGridViewTitleKey.Columns.Remove(this.comboBoxCode.SelectedItem.ToString());

                // メンバ変数からも削除
                Website site = this.config.GetWebsite(comboBoxCode.SelectedItem.ToString());
                if (site != null)
                {
                    this.config.Configs[this.config.Mode].Websites.Remove(site);
                }
            }

            Honememo.Cmn.RemoveComboBoxItem(ref comboBoxCode);

            // 画面の状態を更新
            this.comboBoxCode_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// 各言語での呼称表からフォーカスを離したとき時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void dataGridViewName_Leave(object sender, EventArgs e)
        {
            // 値チェック
            string codeUnsetRows = String.Empty;
            string nameUnsetRows = String.Empty;
            string redundantCodeRows = String.Empty;
            for (int y = 0; y < this.dataGridViewName.RowCount - 1; y++)
            {
                // 言語コード列は、小文字のデータに変換
                this.dataGridViewName["Code", y].Value = Honememo.Cmn.NullCheckAndTrim(this.dataGridViewName["Code", y]).ToLower();

                // 言語コードが設定されていない行があるか？
                if (String.IsNullOrEmpty(this.dataGridViewName["Code", y].Value.ToString()))
                {
                    if (!String.IsNullOrEmpty(codeUnsetRows))
                    {
                        codeUnsetRows += ",";
                    }

                    codeUnsetRows += y + 1;
                }
                else
                {
                    // 言語コードが重複していないか？
                    for (int i = 0; i < y; i++)
                    {
                        if (this.dataGridViewName["Code", i].Value.ToString() == this.dataGridViewName["Code", y].Value.ToString())
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
                    if (!String.IsNullOrEmpty(Honememo.Cmn.NullCheckAndTrim(this.dataGridViewName["ShortName", y]))
                        && String.IsNullOrEmpty(Honememo.Cmn.NullCheckAndTrim(this.dataGridViewName["ArticleName", y])))
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
                if (errorMessage != String.Empty)
                {
                    errorMessage += "\n";
                }

                errorMessage += String.Format(Resources.WarningMessage_RedundantCodeColumn, redundantCodeRows);
            }

            if (!String.IsNullOrEmpty(nameUnsetRows))
            {
                if (errorMessage != String.Empty)
                {
                    errorMessage += "\n";
                }

                errorMessage += String.Format(Resources.WarningMessage_UnsetArticleNameColumn, nameUnsetRows);
            }

            if (!String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, Resources.WarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.dataGridViewName.Focus();
            }
        }

        /// <summary>
        /// OKボタン押下時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            // 設定を保存し、画面を閉じる
            // 表示列の現在処理中データを確定
            this.comboBoxCode_SelectedIndexChanged(sender, e);

            // 表の状態をメンバ変数に保存
            // 領域の初期化
            if (!this.config.Configs.ContainsKey(Config.RunMode.Wikipedia))
            {
                this.config.Configs[Config.RunMode.Wikipedia] = new Config.ModeConfig();
            }

            // データの保存
            for (int x = 0; x < this.dataGridViewTitleKey.ColumnCount; x++)
            {
                MediaWiki svr = this.config.GetWebsite(this.dataGridViewTitleKey.Columns[x].Name) as MediaWiki;
                if (svr != null)
                {
                    svr.Headings.Clear();
                    for (int y = 0; y < this.dataGridViewTitleKey.RowCount - 1; y++)
                    {
                        if (this.dataGridViewTitleKey[x, y].Value != null)
                        {
                            svr.Headings[y] = this.dataGridViewTitleKey[x, y].Value.ToString().Trim();
                        }
                    }
                }
            }

            // ソート
            // TODO: MediaWikiを言語順にソートする。

            // 設定をファイルに保存
            try
            {
                this.config.Save();
            }
            catch (Exception ex)
            {
                // エラーメッセージを表示、画面は開いたまま
                System.Diagnostics.Debug.WriteLine("ConfigWikipediaDialog.buttonOK_Click > 設定保存中に例外 : " + ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                //FormUtils.ErrorDialog(Resources.ErrorMessageConfigSaveFaild);
            }

            // 画面を閉じて、設定終了
            this.Close();
        }

        #endregion
    }
}