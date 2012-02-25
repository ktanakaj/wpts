// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツールコード入力ダイアログクラスソース</summary>
//
// <copyright file="AddLanguageDialog.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs
{
    using System;
    using System.Windows.Forms;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;
    using Honememo.Wptscs.Utilities;

    /// <summary>
    /// Wikipedia翻訳支援ツールコード入力ダイアログのクラスです。
    /// </summary>
    public partial class AddLanguageDialog : Form
    {
        #region private変数

        /// <summary>
        /// 現在設定中のアプリケーションの設定。
        /// </summary>
        private Config config;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。初期化メソッド呼び出しのみ。
        /// </summary>
        /// <param name="config">設定対象のConfig。</param>
        /// <exception cref="ArgumentNullException"><paramref name="config"/>が<c>null</c>。</exception>
        public AddLanguageDialog(Config config)
        {
            // Windows フォーム デザイナで生成されたコード
            this.InitializeComponent();

            // 設定対象のConfigを受け取る
            this.config = Honememo.Utilities.Validate.NotNull(config, "config");
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 言語コード（データやり取り用）。
        /// </summary>
        public string LanguageCode
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
            this.LanguageCode = this.textBoxCode.Text.Trim();
            if (String.IsNullOrEmpty(this.LanguageCode))
            {
                FormUtils.WarningDialog(Resources.WarningMessageEmptyLanguageCode);
                this.textBoxCode.Focus();
                return;
            }
            else if (this.config.GetWebsite(this.LanguageCode) != null)
            {
                FormUtils.WarningDialog(Resources.WarningMessageDuplicateLanguageCode);
                this.textBoxCode.Focus();
                return;
            }

            // テキストボックスの言語コードを保存して画面を閉じる
            this.DialogResult = DialogResult.OK;
        }

        #endregion
    }
}