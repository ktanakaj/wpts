// ================================================================================================
// <summary>
//      文字列処理に関するユーティリティクラスソース。</summary>
//
// <copyright file="StringUtils.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 文字列処理に関するユーティリティクラスです。
    /// </summary>
    /// <remarks>一部メソッドは、Apache Commons Lang の StringUtils やJava標準の String を参考にしています。</remarks>
    public static class StringUtils
    {
        #region 定数

        /// <summary>
        /// <see cref="FormatDollarVariable"/>で使用する正規表現。
        /// </summary>
        private static readonly Regex DollarVariableRegex = new Regex("\\$([0-9]+)");

        #endregion

        #region 初期化メソッド

        /// <summary>
        /// 渡された文字列をチェックし、<c>null</c>だった場合には空の文字列を返します。
        /// それ以外の場合には渡された文字列を返します。
        /// </summary>
        /// <param name="str">チェックを行う対象となる文字列。</param>
        /// <returns>渡された文字列、<c>null</c>の場合には空の文字列。</returns>
        public static string DefaultString(string str)
        {
            return StringUtils.DefaultString(str, String.Empty);
        }

        /// <summary>
        /// 渡された文字列をチェックし、<c>null</c>だった場合には指定されたデフォルトの文字列を返します。
        /// それ以外の場合には渡された文字列を返します。
        /// </summary>
        /// <param name="str">チェックを行う対象となる文字列。</param>
        /// <param name="defaultString">渡された文字列が<c>null</c>の場合に返されるデフォルトの文字列。</param>
        /// <returns>渡された文字列、<c>null</c>の場合にはデフォルトの文字列。</returns>
        public static string DefaultString(string str, string defaultString)
        {
            if (str == null)
            {
                return defaultString;
            }

            return str;
        }

        #endregion

        #region 切り出しメソッド

        /// <summary>
        /// 指定された文字列の部分文字列を例外を発生させることなく取得します。
        /// </summary>
        /// <param name="str">部分文字列の取得対象となる文字列。</param>
        /// <param name="startIndex">部分文字列の開始位置。</param>
        /// <returns>開始位置からの部分文字列。</returns>
        public static string Substring(string str, int startIndex)
        {
            return StringUtils.Substring(str, startIndex, Int32.MaxValue);
        }

        /// <summary>
        /// 指定された文字列の部分文字列を例外を発生させることなく取得します。
        /// </summary>
        /// <param name="str">部分文字列の取得対象となる文字列。</param>
        /// <param name="startIndex">部分文字列の開始位置。</param>
        /// <param name="length">部分文字列の文字数。</param>
        /// <returns>開始位置から指定された文字数の部分文字列。文字数が足りない場合、最後まで。</returns>
        public static string Substring(string str, int startIndex, int length)
        {
            if (str == null)
            {
                return null;
            }

            int i = startIndex > 0 ? startIndex : 0;
            if (i > str.Length)
            {
                return String.Empty;
            }

            int l = length > 0 ? length : 0;
            if (l > str.Length - i)
            {
                l = str.Length - i;
            }

            return str.Substring(i, l);
        }

        #endregion

        #region 文字列チェック

        /// <summary>
        /// この文字列の指定されたインデックス以降の部分文字列が、指定された接頭辞で始まるかどうかを判定します。
        /// </summary>
        /// <param name="str">チェックを行う対象となる文字列。</param>
        /// <param name="prefix">接頭辞。</param>
        /// <param name="toffset">この文字列の比較を開始する位置。</param>
        /// <returns>始まる場合<c>true</c>。<c>toffset</c>が負の値の場合、<c>str</c>の長さより大きい場合<c>false</c>。それ以外で<c>prefix</c>が空の場合は<c>true</c>。</returns>
        /// <remarks>引数の<c>null</c>は許容、<c>str</c>のみまたは<c>prefix</c>のみ<c>null</c>は<c>false</c>、<c>prefix</c>も<c>null</c>は<c>true</c>を返す。</remarks>
        public static bool StartsWith(string str, string prefix, int toffset)
        {
            // nullチェック
            if (str == null)
            {
                return prefix == null;
            }
            else if (prefix == null)
            {
                return false;
            }

            // 範囲チェック
            if (toffset < 0 || toffset >= str.Length)
            {
                return false;
            }

            // 長さチェック
            if (prefix.Length == 0)
            {
                return true;
            }

            // substringしてしまうと遅いので、先頭1文字だけは自前でチェック
            if (str[toffset] != prefix[0])
            {
                return false;
            }

            // 後は普通のStartWithで処理
            return str.Substring(toffset).StartsWith(prefix);
        }

        #endregion

        #region 書式化メソッド

        /// <summary>
        /// 指定した文字列の書式項目を、指定した配列内の対応するオブジェクトの文字列形式に置換します。
        /// </summary>
        /// <param name="format">$1～$数値の形式でパラメータを指定する書式指定文字列。</param>
        /// <param name="args">書式設定対象オブジェクト。</param>
        /// <returns>書式項目が <para>args</para> の対応するオブジェクトの文字列形式に置換された <para>format</para> のコピー。</returns>
        /// <exception cref="ArgumentNullException"><para>format</para>または<para>args</para>が<c>null</c>の場合。</exception>
        /// <remarks>.netではなくPerl等で見かける$～形式のフォーマットを行う。</remarks>
        public static string FormatDollarVariable(string format, params object[] args)
        {
            // nullチェック
            Validate.NotNull(format);
            Validate.NotNull(args);

            // 正規表現で$1～$数値のパラメータ部分を抜き出し、対応するパラメータに置き換える
            // 対応するパラメータが存在しない場合、空文字列となる
            return DollarVariableRegex.Replace(
                format,
                (Match match)
                =>
                {
                    int index = Int32.Parse(match.Groups[1].Value) - 1;
                    return args.Length > index ? ObjectUtils.ToString(args[index]) : String.Empty;
                });
        }

        #endregion
    }
}
