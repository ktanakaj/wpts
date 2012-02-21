// ================================================================================================
// <summary>
//      コレクション／配列処理に関するユーティリティクラスソース。</summary>
//
// <copyright file="CollectionUtils.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// コレクション／配列処理に関するユーティリティクラスです。
    /// </summary>
    public static class CollectionUtils
    {
        #region 比較メソッド

        /// <summary>
        /// 指定された文字列が渡されたコレクション内に存在するかを大文字小文字を無視して判定する。
        /// </summary>
        /// <param name="collection">探索するコレクション。</param>
        /// <param name="item">含まれるか判定する文字列。</param>
        /// <returns>指定された文字列が含まれる場合<c>true</c>。</returns>
        /// <exception cref="ArgumentNullException"><para>collection</para>が<c>null</c>の場合。</exception>
        public static bool ContainsIgnoreCase(IEnumerable<string> collection, string item)
        {
            foreach (string s in Validate.NotNull(collection))
            {
                if (s == item || (s != null && item != null && s.ToLower() == item.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 加工メソッド

        /// <summary>
        /// 渡された文字列配列の中の要素を全て<see cref="String.Trim()"/>した配列を返す。
        /// </summary>
        /// <param name="array"><c>Trim</c>する文字列配列。</param>
        /// <returns><c>Trim</c>された文字列配列。</returns>
        /// <exception cref="ArgumentNullException"><para>array</para>が<c>null</c>の場合。</exception>
        /// <remarks><para>array</para>中に<c>null</c>要素が存在するのは可。</remarks>
        public static string[] Trim(string[] array)
        {
            string[] result = new string[Validate.NotNull(array).Length];
            for (int i = 0; i < array.Length; i++)
            {
                string s = array[i];
                if (s != null)
                {
                    result[i] = s.Trim();
                }
            }

            return result;
        }

        #endregion
    }
}
