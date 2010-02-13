// ================================================================================================
// <summary>
//      Apache Commons Lang の StringUtilsを参考にしたユーティリティクラスソース。</summary>
//
// <copyright file="StringUtils.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;

    /// <summary>
    /// Apache Commons Lang の StringUtilsを参考にしたユーティリティクラスです。
    /// </summary>
    public static class StringUtils
    {
        #region 初期化用メソッド

        /// <summary>
        /// 渡された文字列をチェックし、null だった場合には空の文字列を返します。
        /// それ以外の場合には渡された文字列を返します。
        /// </summary>
        /// <param name="str">チェックを行う対象となる文字列。</param>
        /// <returns>渡された文字列、null の場合には空の文字列。</returns>
        public static string DefaultString(string str)
        {
            return DefaultString(str, String.Empty);
        }

        /// <summary>
        /// 渡された文字列をチェックし、null だった場合には指定されたデフォルトの文字列を返します。
        /// それ以外の場合には渡された文字列を返します。
        /// </summary>
        /// <param name="str">チェックを行う対象となる文字列。</param>
        /// <param name="defaultString">渡された文字列が null の場合に返されるデフォルトの文字列。</param>
        /// <returns>渡された文字列、null の場合にはデフォルトの文字列。</returns>
        public static string DefaultString(string str, string defaultString)
        {
            if (str == null)
            {
                return defaultString;
            }

            return str;
        }

        #endregion
    }
}
