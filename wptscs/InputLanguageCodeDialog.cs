// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツールコード入力ダイアログクラスソース</summary>
//
// <copyright file="InputLanguageCodeDialog.cs" company="honeplusのメモ帳">
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
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Wikipedia翻訳支援ツールコード入力ダイアログのクラスです。
    /// </summary>
    public partial class InputLanguageCodeDialog : Form
    {
        /// <summary>
        /// 言語コード（データやり取り用）。
        /// </summary>
        public string LanguageCode;

        /// <summary>
        /// コンストラクタ。初期化メソッド呼び出しのみ。
        /// </summary>
        public InputLanguageCodeDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームロード時の処理。初期化。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void InputLanguageCodeDialog_Load(object sender, EventArgs e)
        {
            // テキストボックスに言語コードを設定
            if (this.LanguageCode != null)
            {
                textBoxCode.Text = this.LanguageCode;
            }
        }

        /// <summary>
        /// フォームクローズ時の処理。データ保存。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private void InputLanguageCodeDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            // テキストボックスの言語コードを保存
            this.LanguageCode = textBoxCode.Text.Trim();
        }
    }
}