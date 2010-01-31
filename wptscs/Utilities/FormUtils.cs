// ================================================================================================
// <summary>
//      Windows処理に関するユーティリティクラスソース。</summary>
//
// <copyright file="FormUtils.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Windows.Forms;

    // ※ プロパティを含むので、そのまま他のプロジェクトに流用することはできない
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// Windows処理に関するユーティリティクラスです。
    /// </summary>
    public static class FormUtils
    {
        #region ダイアログ

        /// <summary>
        /// 単純デザインの通知ダイアログ（入力された文字列を表示）。
        /// </summary>
        /// <param name="msg">メッセージ。</param>
        public static void InformationDialog(string msg)
        {
            // 渡された文字列で通知ダイアログを表示
            MessageBox.Show(
                msg,
                Resources.InformationTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// 単純デザインの通知ダイアログ（入力された文字列を書式化して表示）。
        /// </summary>
        /// <param name="format">書式項目を含んだメッセージ。</param>
        /// <param name="args">書式設定対象オブジェクト配列。</param>
        public static void InformationDialog(string format, params object[] args)
        {
            // オーバーロードメソッドをコール
            FormUtils.InformationDialog(String.Format(format, args));
        }

        /// <summary>
        /// 単純デザインの警告ダイアログ（入力された文字列を表示）。
        /// </summary>
        /// <param name="msg">メッセージ。</param>
        public static void WarningDialog(string msg)
        {
            // 渡された文字列で警告ダイアログを表示
            MessageBox.Show(
                msg,
                Resources.WarningTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 単純デザインの警告ダイアログ（入力された文字列を書式化して表示）。
        /// </summary>
        /// <param name="format">書式項目を含んだメッセージ。</param>
        /// <param name="args">書式設定対象オブジェクト配列。</param>
        public static void WarningDialog(string format, params object[] args)
        {
            // オーバーロードメソッドをコール
            FormUtils.WarningDialog(String.Format(format, args));
        }

        /// <summary>
        /// 単純デザインのエラーダイアログ（入力された文字列を表示）。
        /// </summary>
        /// <param name="msg">メッセージ。</param>
        public static void ErrorDialog(string msg)
        {
            // 渡された文字列でエラーダイアログを表示
            MessageBox.Show(
                msg,
                Resources.ErrorTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// 単純デザインのエラーダイアログ（入力された文字列を書式化して表示）。
        /// </summary>
        /// <param name="format">書式項目を含んだメッセージ。</param>
        /// <param name="args">書式設定対象オブジェクト配列。</param>
        public static void ErrorDialog(string format, params object[] args)
        {
            // オーバーロードメソッドをコール
            FormUtils.ErrorDialog(String.Format(format, args));
        }

        #endregion
    }
}
