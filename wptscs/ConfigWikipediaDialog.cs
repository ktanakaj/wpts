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
            InitializeComponent();
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
            this.config = new Config(Path.Combine(Application.StartupPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".xml"));

            // データ設定
            comboBoxCodeSelectedText = "";
            comboBoxCode.Items.Clear();
            dataGridViewName.Rows.Clear();
            dataGridViewTitleKey.Columns.Clear();
            // 使用言語取得
            String showCode = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            int x = 0;
            foreach (LanguageInformation lang in config.Languages)
            {
                WikipediaInformation svr = lang as WikipediaInformation;
                if (svr != null)
                {
                    // 表タイトル設定
                    String name = svr.GetName(showCode);
                    if (name != "")
                    {
                        name += (" (" + svr.Code + ")");
                    }
                    else
                    {
                        name = svr.Code;
                    }
                    dataGridViewTitleKey.Columns.Add(svr.Code, name);
                    // 表データ設定
                    for (int y = 0; y < svr.TitleKeys.Length; y++)
                    {
                        if (dataGridViewTitleKey.RowCount - 1 <= y)
                        {
                            dataGridViewTitleKey.Rows.Add();
                        }
                        dataGridViewTitleKey[x, y].Value = svr.TitleKeys[y];
                    }
                    // コンボボックス設定
                    comboBoxCode.Items.Add(svr.Code);
                    // 次の列へ
                    ++x;
                }
            }
            dataGridViewTitleKey.CurrentCell = null;
        }

        /// <summary>
        /// 言語コードコンボボックス変更時の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void comboBoxCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ConfigLanguageDialog._SelectedIndexChanged > "
                + comboBoxCodeSelectedText + " . "
                + ((comboBoxCode.SelectedItem != null) ? (comboBoxCode.SelectedItem.ToString()) : ("")));

            // 変更前の設定を保存
            // ※変更前にしろ変更後にしろ、事前に追加しているのでGetLanguageで見つからないことは無い・・・はず
            if (comboBoxCodeSelectedText != "")
            {
                WikipediaInformation svr = config.GetLanguage(comboBoxCodeSelectedText) as WikipediaInformation;
                if (svr != null)
                {
                    svr.ArticleXmlPath = textBoxXml.Text.Trim();
                    svr.Redirect = textBoxRedirect.Text.Trim();
                    // 表から呼称の情報も保存
                    dataGridViewName.Sort(dataGridViewName.Columns["Code"], ListSortDirection.Ascending);
                    svr.Names = new LanguageInformation.LanguageName[0];
                    for (int y = 0; y < dataGridViewName.RowCount - 1; y++)
                    {
                        // 値が入ってないとかはガードしているはずだが、一応チェック
                        String code = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["Code", y]);
                        if (code != "")
                        {
                            LanguageInformation.LanguageName name = new LanguageInformation.LanguageName();
                            name.Code = code;
                            name.Name = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["ArticleName", y]);
                            name.ShortName = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["ShortName", y]);
                            Honememo.Cmn.AddArray(ref svr.Names, name);
                        }
                    }
                }
            }
            // 変更後の値に応じて、画面表示を更新
            if (comboBoxCode.SelectedItem != null)
            {
                // 値を設定
                WikipediaInformation svr = config.GetLanguage(comboBoxCode.SelectedItem.ToString()) as WikipediaInformation;
                if (svr != null)
                {
                    textBoxXml.Text = svr.ArticleXmlPath;
                    textBoxRedirect.Text = svr.Redirect;
                    // 呼称の情報を表に設定
                    dataGridViewName.Rows.Clear();
                    foreach (LanguageInformation.LanguageName name in svr.Names)
                    {
                        int index = dataGridViewName.Rows.Add();
                        dataGridViewName["Code", index].Value = name.Code;
                        dataGridViewName["ArticleName", index].Value = name.Name;
                        dataGridViewName["ShortName", index].Value = name.ShortName;
                    }
                }
                // 言語のプロパティを有効に
                groupBoxStyle.Enabled = true;
                groupBoxName.Enabled = true;
                // 現在の選択値を更新
                comboBoxCodeSelectedText = comboBoxCode.SelectedItem.ToString();
            }
            else
            {
                // 言語のプロパティを無効に
                groupBoxStyle.Enabled = false;
                groupBoxName.Enabled = false;
                // 現在の選択値を更新
                comboBoxCodeSelectedText = "";
            }
        }

        /* 言語コードコンボボックスでのキー入力 */
        private void comboBoxCode_KeyDown(object sender, KeyEventArgs e)
        {
            // エンターキーが押された場合、現在の値が一覧に無ければ登録する（フォーカスを失ったときの処理）
            if (e.KeyCode == Keys.Enter)
            {
                System.Diagnostics.Debug.WriteLine("ConfigLanguageDialog._KeyDown > " + comboBoxCode.Text);
                comboBoxCode_Leave(sender, e);
            }
        }

        /* 言語コードコンボボックスからフォーカスを離したとき */
        private void comboBoxCode_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ConfigLanguageDialog._Leave > " + comboBoxCode.Text);
            // 現在の値が一覧に無ければ登録する
            comboBoxCode.Text = comboBoxCode.Text.Trim().ToLower();
            if (comboBoxCode.Text != "")
            {
                if (Honememo.Cmn.AddComboBoxNewItem(ref comboBoxCode) == true)
                {
                    // 登録した場合メンバ変数にも登録
                    WikipediaInformation svr = config.GetLanguage(comboBoxCode.Text) as WikipediaInformation;
                    // 存在しないはずだが一応は確認して追加
                    if (svr == null)
                    {
                        svr = new WikipediaInformation(comboBoxCode.Text);
                        Honememo.Cmn.AddArray(ref config.Languages, (LanguageInformation)svr);
                        // 定型句の設定表に列を追加
                        dataGridViewTitleKey.Columns.Add(comboBoxCode.Text, comboBoxCode.Text);
                    }
                    // 登録した値を選択状態に変更
                    comboBoxCode.SelectedItem = comboBoxCode.Text;
                }
            }
            else
            {
                // 空にしたとき、変更でイベントが起こらないようなので、強制的に呼ぶ
                comboBoxCode_SelectedIndexChanged(sender, e);
            }
        }

        /* 言語コードコンボボックスのコンテキストメニュー：言語コードを変更 */
        private void toolStripMenuItemModify_Click(object sender, EventArgs e)
        {
            // 選択されている言語コードに関連する情報を更新
            if (comboBoxCode.SelectedIndex != -1)
            {
                String oldCode = comboBoxCode.SelectedItem.ToString();
                // 入力画面にて変更後の言語コードを取得
                InputLanguageCodeDialog dialog = new InputLanguageCodeDialog();
                dialog.LanguageCode = oldCode;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    String newCode = dialog.LanguageCode;
                    // メンバ変数を更新
                    LanguageInformation lang = config.GetLanguage(oldCode);
                    if (lang != null)
                    {
                        lang.Code = newCode;
                    }
                    foreach (LanguageInformation langIndex in config.Languages)
                    {
                        if (langIndex.GetType() != typeof(WikipediaInformation))
                        {
                            continue;
                        }
                        for (int i = 0; i < langIndex.Names.Length; i++)
                        {
                            if (langIndex.Names[i].Code == oldCode)
                            {
                                langIndex.Names[i].Code = newCode;
                            }
                        }
                    }
                    // コンボボックスを更新
                    int index = comboBoxCode.Items.IndexOf(comboBoxCode.SelectedItem);
                    comboBoxCode.Items[index] = newCode;
                    // 定型句の設定表を更新
                    String header = lang.GetName(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                    if (header != "")
                    {
                        header += (" (" + newCode + ")");
                    }
                    else
                    {
                        header = newCode;
                    }
                    dataGridViewTitleKey.Columns[oldCode].HeaderText = header;
                    dataGridViewTitleKey.Columns[oldCode].Name = newCode;
                    // 画面の状態を更新
                    comboBoxCode_SelectedIndexChanged(sender, e);
                }
            }
        }

        /* 言語コードコンボボックスのコンテキストメニュー：言語を削除 */
        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            // 選択されている言語コードに関連する情報を削除
            if (comboBoxCode.SelectedIndex != -1)
            {
                dataGridViewTitleKey.Columns.Remove(comboBoxCode.SelectedItem.ToString());
                // メンバ変数からも削除
                LanguageInformation[] newLanguages = new LanguageInformation[0];
                foreach (LanguageInformation lang in config.Languages)
                {
                    if (lang.Code == comboBoxCode.SelectedItem.ToString() &&
                       lang.GetType() == typeof(WikipediaInformation))
                    {
                        continue;
                    }
                    Honememo.Cmn.AddArray(ref newLanguages, lang);
                }
                config.Languages = newLanguages;
            }
            Honememo.Cmn.RemoveComboBoxItem(ref comboBoxCode);
            // 画面の状態を更新
            comboBoxCode_SelectedIndexChanged(sender, e);
        }

        /* 各言語での呼称表からフォーカスを離したとき */
        private void dataGridViewName_Leave(object sender, EventArgs e)
        {
            // 値チェック
            String codeUnsetRows = "";
            String nameUnsetRows = "";
            String redundantCodeRows = "";
            for (int y = 0; y < dataGridViewName.RowCount - 1; y++)
            {
                // 言語コード列は、小文字のデータに変換
                dataGridViewName["Code", y].Value = Honememo.Cmn.NullCheckAndTrim(dataGridViewName["Code", y]).ToLower();
                // 言語コードが設定されていない行があるか？
                if (dataGridViewName["Code", y].Value.ToString() == "")
                {
                    if (codeUnsetRows != "")
                    {
                        codeUnsetRows += ",";
                    }
                    codeUnsetRows += (y + 1);
                }
                else
                {
                    // 言語コードが重複していないか？
                    for (int i = 0; i < y; i++)
                    {
                        if (dataGridViewName["Code", i].Value.ToString() == dataGridViewName["Code", y].Value.ToString())
                        {
                            if (redundantCodeRows != "")
                            {
                                redundantCodeRows += ",";
                            }
                            redundantCodeRows += (y + 1);
                            break;
                        }
                    }
                    // 呼称が設定されていないのに略称が設定されていないか？
                    if (Honememo.Cmn.NullCheckAndTrim(dataGridViewName["ShortName", y]) != "" &&
                       Honememo.Cmn.NullCheckAndTrim(dataGridViewName["ArticleName", y]) == "")
                    {
                        if (nameUnsetRows != "")
                        {
                            nameUnsetRows += ",";
                        }
                        nameUnsetRows += (y + 1);
                    }
                }
            }
            // 結果の表示
            String errorMessage = "";
            if (codeUnsetRows != "")
            {
                errorMessage += (String.Format(Resources.WarningMessage_UnsetCodeColumn, codeUnsetRows));
            }
            if (redundantCodeRows != "")
            {
                if (errorMessage != "")
                {
                    errorMessage += "\n";
                }
                errorMessage += (String.Format(Resources.WarningMessage_RedundantCodeColumn, redundantCodeRows));
            }
            if (nameUnsetRows != "")
            {
                if (errorMessage != "")
                {
                    errorMessage += "\n";
                }
                errorMessage += (String.Format(Resources.WarningMessage_UnsetArticleNameColumn, nameUnsetRows));
            }
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage, Resources.WarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dataGridViewName.Focus();
            }
        }

        /* OKボタン押下 */
        private void buttonOK_Click(object sender, EventArgs e)
        {
            // 設定を保存し、画面を閉じる
            // 表示列の現在処理中データを確定
            comboBoxCode_SelectedIndexChanged(sender, e);
            // 表の状態をメンバ変数に保存
            // 領域の初期化
            foreach (LanguageInformation lang in config.Languages)
            {
                WikipediaInformation svr = lang as WikipediaInformation;
                if (svr != null)
                {
                    Array.Resize(ref svr.TitleKeys, dataGridViewTitleKey.RowCount - 1);
                }
            }
            // データの保存
            for (int x = 0; x < dataGridViewTitleKey.ColumnCount; x++)
            {
                WikipediaInformation svr = config.GetLanguage(dataGridViewTitleKey.Columns[x].Name) as WikipediaInformation;
                if (svr != null)
                {
                    for (int y = 0; y < dataGridViewTitleKey.RowCount - 1; y++)
                    {
                        if (dataGridViewTitleKey[x, y].Value != null)
                        {
                            svr.TitleKeys[y] = dataGridViewTitleKey[x, y].Value.ToString().Trim();
                        }
                        else
                        {
                            svr.TitleKeys[y] = "";
                        }
                    }
                }
            }
            // ソート
            Array.Sort(config.Languages);

            // 設定をファイルに保存
            if (config.Save() == true)
            {
                // 画面を閉じて、設定終了
                this.Close();
            }
            else
            {
                // エラーメッセージを表示、画面は開いたまま
                cmnAP.ErrorDialogResource("ErrorMessage_MissConfigSave");
            }
        }

        #endregion
    }
}