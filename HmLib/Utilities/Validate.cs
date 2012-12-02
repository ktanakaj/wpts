// ================================================================================================
// <summary>
//      バリデート処理に関するユーティリティクラスソース。</summary>
//
// <copyright file="Validate.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// バリデート処理に関するユーティリティクラスです。
    /// </summary>
    /// <remarks>一部メソッドは、Apache Commons LangのValidateを参考にしています。</remarks>
    public static class Validate
    {
        #region NotNullメソッド

        /// <summary>
        /// 渡されたオブジェクトをチェックし、<c>null</c>の場合に例外をスローする。
        /// </summary>
        /// <typeparam name="T">オブジェクトの型。</typeparam>
        /// <param name="obj"><c>null</c>かどうかをチェックするオブジェクト。</param>
        /// <param name="paramName">オブジェクトが<c>null</c>の場合に例外に渡されるパラメータ名。デフォルトは<c>value</c>。</param>
        /// <returns>渡されたオブジェクト。</returns>
        /// <exception cref="ArgumentNullException">オブジェクトが<c>null</c>。</exception>
        public static T NotNull<T>(T obj, string paramName = "value")
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return obj;
        }

        #endregion

        #region NotEmptyメソッド

        /// <summary>
        /// 渡された文字列をチェックし、空（<c>null</c>または長さ0）の場合に例外をスローする。
        /// </summary>
        /// <param name="str">空かどうかをチェックする文字列。</param>
        /// <param name="paramName">文字列が空の場合に例外に渡されるパラメータ名。デフォルトは<c>value</c>。</param>
        /// <returns>渡された文字列。</returns>
        /// <exception cref="ArgumentNullException">文字列が<c>null</c>。</exception>
        /// <exception cref="ArgumentException">文字列が長さ0。</exception>
        public static string NotEmpty(string str, string paramName = "value")
        {
            if (NotNull(str, paramName) == string.Empty)
            {
                throw new ArgumentException("The validated string is empty", paramName);
            }

            return str;
        }

        #endregion

        #region NotBlankメソッド

        /// <summary>
        /// 渡された文字列をチェックし、空（<c>null</c>または空か空白のみ）の場合に例外をスローする。
        /// </summary>
        /// <param name="str">空かどうかをチェックする文字列。</param>
        /// <param name="paramName">文字列が空の場合に例外に渡されるパラメータ名。デフォルトは<c>value</c>。</param>
        /// <returns>渡された文字列。</returns>
        /// <exception cref="ArgumentNullException">文字列が<c>null</c>。</exception>
        /// <exception cref="ArgumentException">文字列が空か空白のみ。</exception>
        public static string NotBlank(string str, string paramName = "value")
        {
            if (string.IsNullOrWhiteSpace(NotNull(str, paramName)))
            {
                throw new ArgumentException("The validated string is blank", paramName);
            }

            return str;
        }

        #endregion

        #region InRangeメソッド

        /// <summary>
        /// 渡された文字列をチェックし、文字列が<c>null</c>またはインデックスが範囲外の場合に例外をスローする。
        /// </summary>
        /// <param name="str">文字列長をチェックする文字列。</param>
        /// <param name="index">文字列内に含まれることが期待されるインデックス。</param>
        /// <param name="paramNameStr">文字列が<c>null</c>の場合に例外に渡されるパラメータ名。デフォルトは<c>value</c>。</param>
        /// <param name="paramNameIndex">インデックスが範囲外の場合に例外に渡されるパラメータ名。</param>
        /// <exception cref="ArgumentNullException"><paramref name="str"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentOutOfRangeException">インデックスが範囲外の場合。</exception>
        public static void InRange(string str, int index, string paramNameStr = "value", string paramNameIndex = "index")
        {
            if (NotNull(str, paramNameStr).Length <= index || index < 0)
            {
                throw new ArgumentOutOfRangeException(paramNameIndex);
            }
        }

        /// <summary>
        /// 渡されたリストをチェックし、リストが<c>null</c>またはインデックスが範囲外の場合に例外をスローする。
        /// </summary>
        /// <typeparam name="T">リスト内のオブジェクトの型。</typeparam>
        /// <param name="list">長さをチェックするリスト。</param>
        /// <param name="index">リスト内に含まれることが期待されるインデックス。</param>
        /// <param name="paramNameList">リストが<c>null</c>の場合に例外に渡されるパラメータ名。デフォルトは<c>value</c>。</param>
        /// <param name="paramNameIndex">インデックスが範囲外の場合に例外に渡されるパラメータ名。デフォルトは<c>index</c>。</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentOutOfRangeException">インデックスが範囲外の場合。</exception>
        public static void InRange<T>(IList<T> list, int index, string paramNameList = "value", string paramNameIndex = "index")
        {
            if (NotNull(list, paramNameList).Count <= index || index < 0)
            {
                throw new ArgumentOutOfRangeException(paramNameIndex);
            }
        }

        #endregion
    }
}
