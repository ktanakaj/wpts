// ================================================================================================
// <summary>
//      フォーム処理に関するユーティリティクラスソース。</summary>
//
// <copyright file="FormUtils.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using Honememo.Models;

    // ※ プロパティを含むので、そのまま他のプロジェクトに流用することはできない
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// フォーム処理に関するユーティリティクラスです。
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
            return string.Format(Resources.ApplicationName, ver.Major, ver.Minor);
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
            // ※ 以下GetFilesAtUserAppDataと共通化してもよいが、性能に影響あるかと考え自前で処理
            // 現在の UserAppDataPath を探索
            string path = Path.Combine(Application.UserAppDataPath, fileName);
            if (File.Exists(path))
            {
                return path;
            }

            // 可能であれば、旧バージョンの UserAppDataPath を探索
            if (!string.IsNullOrEmpty(compatible))
            {
                // ファイルが見つかるまで探索
                foreach (string dir in FormUtils.GetCompatibleUserAppDataPaths(compatible))
                {
                    path = Path.Combine(dir, fileName);
                    if (File.Exists(path))
                    {
                        return path;
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
        /// UserAppDataPath
        /// → 旧バージョンのUserAppDataPath
        /// → StartupPath から、指定した検索パターンに一致するファイル名を返す。
        /// </summary>
        /// <param name="searchPattern">ファイル名と対応させる検索文字列。</param>
        /// <param name="compatible">探索する旧バージョンの最大。</param>
        /// <returns>
        /// 指定した検索パターンに一致するファイル名を格納するString配列。ファイル名には完全パスを含む。
        /// 同名のファイルが複数のパスに存在する場合、最初に発見したもののみを返す。
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="searchPattern"/>に有効なパターンが含まれていない場合。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="searchPattern"/>が<c>null</c>の場合。</exception>
        /// <exception cref="UnauthorizedAccessException">呼び出し元に、必要なアクセス許可がない場合。</exception>
        /// <remarks>アセンブリ名が変わっている場合、旧バージョンは探索不可。</remarks>
        public static string[] GetFilesAtUserAppData(string searchPattern, string compatible)
        {
            // 現在の UserAppDataPath を探索
            List<string> files = new List<string>();
            IgnoreCaseSet names = new IgnoreCaseSet();
            if (Directory.Exists(Application.UserAppDataPath))
            {
                FormUtils.MergeFiles(files, names, Directory.GetFiles(Application.UserAppDataPath, searchPattern));
            }

            // 可能であれば、旧バージョンの UserAppDataPath を探索
            if (!string.IsNullOrEmpty(compatible))
            {
                // 各ディレクトリのファイル名を取得
                foreach (string dir in FormUtils.GetCompatibleUserAppDataPaths(compatible))
                {
                    FormUtils.MergeFiles(files, names, Directory.GetFiles(dir, searchPattern));
                }
            }

            // 最後に、exeと同じフォルダを探索
            FormUtils.MergeFiles(files, names, Directory.GetFiles(Application.StartupPath, searchPattern));
            return files.ToArray();
        }

        /// <summary>
        /// UserAppDataPath
        /// → StartupPath から、指定した検索パターンに一致するファイル名を返す。
        /// </summary>
        /// <param name="searchPattern">ファイル名と対応させる検索文字列。</param>
        /// <returns>
        /// 指定した検索パターンに一致するファイル名を格納するString配列。ファイル名には完全パスを含む。
        /// 同名のファイルが複数のパスに存在する場合、最初に発見したもののみを返す。
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="searchPattern"/>に有効なパターンが含まれていない場合。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="searchPattern"/>が<c>null</c>の場合。</exception>
        /// <exception cref="UnauthorizedAccessException">呼び出し元に、必要なアクセス許可がない場合。</exception>
        public static string[] GetFilesAtUserAppData(string searchPattern)
        {
            // オーバーロードメソッドをコール
            return FormUtils.GetFilesAtUserAppData(searchPattern, null);
        }

        /// <summary>
        /// UserAppDataPath → StartupPath から、全ファイル名を返す。
        /// </summary>
        /// <returns>
        /// フォルダ内の全ファイル名を格納するString配列。ファイル名には完全パスを含む。
        /// 同名のファイルが複数のパスに存在する場合、最初に発見したもののみを返す。
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">呼び出し元に、必要なアクセス許可がない場合。</exception>
        public static string[] GetFilesAtUserAppData()
        {
            // オーバーロードメソッドをコール
            return FormUtils.GetFilesAtUserAppData("*", null);
        }

        /// <summary>
        /// 文字列中のファイル名に使用できない文字を「_」に置換。
        /// また、&amp;nbsp;由来の半角スペース (u00a0) も普通の半角スペース (u0020) に置換する。
        /// </summary>
        /// <param name="fileName">ファイル名。</param>
        /// <returns>置換後のファイル名。</returns>
        public static string ReplaceInvalidFileNameChars(string fileName)
        {
            // 渡された文字列にファイル名に使えない文字が含まれている場合、_ に置き換える
            string result = fileName;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                result = result.Replace(c, '_');
            }

            // &nbsp;由来の半角スペース (u00a0) も普通の半角スペース (u0020) に置き換える
            result = result.Replace(' ', ' ');
            return result;
        }

        #endregion

        #region null値許容メソッド

        /// <summary>
        /// <see cref="DataGridViewCell"/>が<c>null</c>の場合に空の文字列を返す<see cref="Object.ToString"/>。
        /// </summary>
        /// <param name="obj">値を<see cref="Object.ToString"/>するセル。<c>null</c>も可。</param>
        /// <returns>渡されたセルの<see cref="DataGridViewCell.Value"/>を<see cref="Object.ToString"/>した結果。<c>null</c>の場合には空の文字列。</returns>
        public static string ToString(DataGridViewCell obj)
        {
            return FormUtils.ToString(obj, string.Empty);
        }

        /// <summary>
        /// <see cref="DataGridViewCell"/>が<c>null</c>の場合に指定された文字列を返す<see cref="Object.ToString"/>。
        /// </summary>
        /// <param name="obj">値を<see cref="Object.ToString"/>するセル。<c>null</c>も可。</param>
        /// <param name="nullStr">渡されたセルが<c>null</c>の場合に返される文字列。<c>null</c>も可。</param>
        /// <returns>渡されたセルの<see cref="DataGridViewCell.Value"/>を<see cref="Object.ToString"/>した結果。<c>null</c>の場合には指定された文字列。</returns>
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
            FormUtils.InformationDialog(string.Format(format, args));
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
            FormUtils.WarningDialog(string.Format(format, args));
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
            FormUtils.ErrorDialog(string.Format(format, args));
        }

        #endregion

        #region テーブル処理

        /// <summary>
        /// <see cref="DataGridViewRow"/>が空行かを判定する。
        /// </summary>
        /// <param name="row">1行。</param>
        /// <returns>空行の場合<c>true</c>。</returns>
        public static bool IsEmptyRow(DataGridViewRow row)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (!string.IsNullOrEmpty(FormUtils.ToString(cell)))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// 指定されたバージョン以上の旧バージョンのUserAppDataPathを取得する。
        /// </summary>
        /// <param name="compatible">探索する旧バージョンの最大。</param>
        /// <returns>旧バージョンと自バージョンの間のフォルダ名を格納するString配列。フォルダ名には完全パスを含む。</returns>
        /// <remarks>
        /// フォルダが異なる同じファイル名のパスが存在する場合、登録しない。
        /// アセンブリ名が変わっている場合、旧バージョンは探索不可。
        /// </remarks>
        private static string[] GetCompatibleUserAppDataPaths(string compatible)
        {
            // UserAppDataPath は
            // <ベースパス>\<CompanyName>\<ProductName>\<ProductVersion>
            // という構成のはずなので、一つ上のフォルダから自分より前のフォルダを探索
            List<string> paths = new List<string>();
            string parent = Path.GetDirectoryName(Application.UserAppDataPath);
            if (!string.IsNullOrEmpty(parent))
            {
                // 現在のバージョンのフォルダ名
                string now = Path.GetFileName(Application.UserAppDataPath);

                // 同じ階層のフォルダをすべて取得し、降順にソート
                string[] directories = Directory.GetDirectories(parent);
                Array.Sort(directories);
                Array.Reverse(directories);

                // 指定された互換バージョンと自バージョンの間のパスのみを取得
                foreach (string dir in directories)
                {
                    string ver = Path.GetFileName(dir);
                    if (compatible.CompareTo(ver) <= 0 && ver.CompareTo(now) < 0)
                    {
                        paths.Add(dir);
                    }
                }
            }

            return paths.ToArray();
        }

        /// <summary>
        /// <see cref="GetFilesAtUserAppData(string, string)"/>用のファイル名リストのマージを行う。
        /// </summary>
        /// <param name="mergeto">マージ先ファイル名リスト。</param>
        /// <param name="names">比較高速化用のパスを含まないファイル名セット。</param>
        /// <param name="mergefrom">マージ元ファイル名リスト。</param>
        /// <remarks>フォルダが異なる同じファイル名のパスが存在する場合、登録しない。</remarks>
        private static void MergeFiles(IList<string> mergeto, IgnoreCaseSet names, IList<string> mergefrom)
        {
            foreach (string file in mergefrom)
            {
                string name = Path.GetFileName(file);
                if (!names.Contains(name))
                {
                    mergeto.Add(file);
                    names.Add(name);
                }
            }
        }

        #endregion
    }
}
