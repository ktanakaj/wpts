// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツール設定名入力ダイアログクラスソース</summary>
//
// <copyright file="AddConfigDialog.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using Honememo.Models;
    using Honememo.Wptscs.Logics;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;
    using Honememo.Wptscs.Utilities;

    /// <summary>
    /// Wikipedia翻訳支援ツール設定名入力ダイアログのクラスです。
    /// </summary>
    public partial class AddConfigDialog : Form
    {
        #region private変数

        /// <summary>
        /// 登録済みの設定ファイル名。
        /// </summary>
        private IgnoreCaseSet configNames;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。初期化メソッド呼び出しのみ。
        /// </summary>
        /// <param name="configNames">登録済みの設定ファイル名。</param>
        /// <exception cref="ArgumentNullException"><para>configNames</para>が<c>null</c>。</exception>
        public AddConfigDialog(IEnumerable<string> configNames)
        {
            // Windows フォーム デザイナで生成されたコード
            this.InitializeComponent();

            // 重複チェック用の既存の設定名一覧を受け取る
            this.configNames = new IgnoreCaseSet(configNames);
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 設定名（データやり取り用）。
        /// </summary>
        public string ConfigName
        {
            get;
            set;
        }

        #endregion

        #region フォームの各イベントのメソッド

        /// <summary>
        /// OKボタン押下時の処理。データ保存。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void ButtonOk_Click(object sender, EventArgs e)
        {
            // 入力値チェック
            this.ConfigName = this.textBoxName.Text.Trim();
            if (String.IsNullOrEmpty(this.ConfigName))
            {
                FormUtils.WarningDialog(Resources.WarningMessageEmptyConfigName);
                this.textBoxName.Focus();
                return;
            }
            else if (this.ConfigName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                FormUtils.WarningDialog(
                    Resources.WarningMessageInvalidConfigName,
                    String.Join(", ", Path.GetInvalidFileNameChars()));
                this.textBoxName.Focus();
                return;
            }
            else if (this.configNames.Contains(this.ConfigName))
            {
                FormUtils.WarningDialog(Resources.WarningMessageDuplicateConfigName);
                this.textBoxName.Focus();
                return;
            }

            // テキストボックスの設定名を保存、設定名からMediaWiki用のパラメータで設定を作成
            Config config = new Config();
            config.File = this.ConfigName + Settings.Default.ConfigurationExtension;
            config.Translator = typeof(MediaWikiTranslator);
            try
            {
                // 設定ファイルを一旦保存、成功なら画面を閉じる
                // ※ エラーの場合、どうしても駄目ならキャンセルボタンで閉じてもらう
                config.Save();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                // 異常時はエラーメッセージを表示
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                FormUtils.ErrorDialog(Resources.ErrorMessageConfigSaveFailed, ex.Message);
            }
        }

        #endregion
    }
}
