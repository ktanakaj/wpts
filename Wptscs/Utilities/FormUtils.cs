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
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    // ※ プロパティを含むので、そのまま他のプロジェクトに流用することはできない
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// Windows処理に関するユーティリティクラスです。
    /// </summary>
    public static class FormUtils
    {
        #region リソース関連

        /// <summary>
        /// バージョン情報を含んだアプリケーション名を返す。
        /// </summary>
        /// <returns>アプリケーション名</returns>
        public static string ApplicationName()
        {
            // アセンブリからバージョン情報を取得し、書式化して返す
            // ビルド番号・リビジョンは無視
            // ※例外なし。もし万が一発生する場合はそのまま投げる
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format(Resources.ApplicationName, ver.Major, ver.Minor);
        }

        /// <summary>
        /// 指定されたファイルを UserAppDataPath
        /// → 旧バージョンのUserAppDataPath
        /// → StartupPath の順に探索し、その際のパスを返す。
        /// </summary>
        /// <param name="fileName">ファイル名。</param>
        /// <param name="compatible">探索する旧バージョンの最大。</param>
        /// <returns>ファイルが存在したパス、どこにも存在しない場合は<c>null</c>。</returns>
        /// <remarks>アセンブリ名が変わっている場合、旧バージョンは探索不可。</remarks>
        public static string SearchUserAppData(string fileName, string compatible)
        {
            // 現在の UserAppDataPath を探索
            string path = Path.Combine(Application.UserAppDataPath, fileName);
            if (File.Exists(path))
            {
                return path;
            }

            // 可能であれば、旧バージョンの UserAppDataPath を探索
            if (!String.IsNullOrEmpty(compatible))
            {
                // UserAppDataPath は
                // <ベースパス>\<CompanyName>\<ProductName>\<ProductVersion>
                // という構成のはずなので、一つ上のフォルダから自分より前のフォルダを探索
                string parent = Path.GetDirectoryName(Application.UserAppDataPath);
                if (!String.IsNullOrEmpty(parent))
                {
                    // 現在のバージョンのフォルダ名
                    string now = Path.GetFileName(Application.UserAppDataPath);

                    // 同じ階層のフォルダをすべて取得し、降順にソート
                    string[] directories = Directory.GetDirectories(parent);
                    Array.Sort(directories);
                    Array.Reverse(directories);

                    // ファイルが見つかるまで探索
                    foreach (string dir in directories)
                    {
                        string ver = Path.GetFileName(dir);
                        if (compatible.CompareTo(ver) <= 0 && ver.CompareTo(now) < 0)
                        {
                            path = Path.Combine(dir, fileName);
                            if (File.Exists(path))
                            {
                                return path;
                            }
                        }
                    }
                }
            }

            // どこにも無い場合は、exeと同じフォルダを探索
            path = Path.Combine(Application.StartupPath, fileName);
            if (File.Exists(path))
            {
                return path;
            }

            return null;
        }

        /// <summary>
        /// 指定されたファイルを UserAppDataPath
        /// → StartupPath の順に探索し、その際のパスを返す。
        /// </summary>
        /// <param name="fileName">ファイル名。</param>
        /// <returns>ファイルが存在したパス、どこにも存在しない場合は<c>null</c>。</returns>
        public static string SearchUserAppData(string fileName)
        {
            // オーバーロードメソッドをコール
            return FormUtils.SearchUserAppData(fileName, null);
        }

        /// <summary>
        /// 文字列中のファイル名に使用できない文字を「_」に置換。
        /// </summary>
        /// <param name="fileName">ファイル名。</param>
        /// <returns>置換後のファイル名。</returns>
        public static string ReplaceInvalidFileNameChars(string fileName)
        {
            // 渡された文字列にファイル名に使えない文字が含まれている場合、_ に置き換える
            string result = fileName;
            char[] unuseChars = Path.GetInvalidFileNameChars();
            foreach (char c in unuseChars)
            {
                result = result.Replace(c, '_');
            }

            return result;
        }

        #endregion

        #region null値許容メソッド

        /// <summary>
        /// <seealso cref="DataGridViewCell"/>が<c>null</c>の場合に空の文字列を返す<c>ToString</c>。
        /// </summary>
        /// <param name="obj"><c>ToString</c>するオブジェクト。<c>null</c>も可。</param>
        /// <returns>渡されたオブジェクトの<c>Value</c>を<c>ToString</c>した結果。<c>null</c>の場合には空の文字列。</returns>
        public static string ToString(DataGridViewCell obj)
        {
            return FormUtils.ToString(obj, String.Empty);
        }

        /// <summary>
        /// <seealso cref="DataGridViewCell"/>が<c>null</c>の場合に指定された文字列を返す<c>ToString</c>。
        /// </summary>
        /// <param name="obj"><c>ToString</c>するオブジェクト。<c>null</c>も可。</param>
        /// <param name="nullStr">渡されたオブジェクトが<c>null</c>の場合に返される文字列。<c>null</c>も可。</param>
        /// <returns>渡されたオブジェクトの<c>Value</c>を<c>ToString</c>した結果。<c>null</c>の場合には指定された文字列。</returns>
        public static string ToString(DataGridViewCell obj, string nullStr)
        {
            if (obj == null)
            {
                return nullStr;
            }
            else if (obj.Value == null)
            {
                return nullStr;
            }

            return obj.Value.ToString();
        }

        #endregion

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
