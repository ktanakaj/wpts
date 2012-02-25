// ================================================================================================
// <summary>
//      Apache Commons LangのValidateを参考にしたユーティリティクラスソース。</summary>
//
// <copyright file="Validate.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;

    /// <summary>
    /// Apache Commons LangのValidateを参考にしたユーティリティクラスです。
    /// </summary>
    /// <remarks>
    /// Apache Commons Lang - Validate
    /// http://commons.apache.org/lang/api/org/apache/commons/lang/Validate.html
    /// </remarks>
    public static class Validate
    {
        #region NotNullメソッド

        /// <summary>
        /// 渡されたオブジェクトをチェックし、<c>null</c>の場合に例外をスローする。
        /// </summary>
        /// <param name="obj"><c>null</c>かどうかをチェックするオブジェクト。</param>
        /// <returns>渡されたオブジェクト。</returns>
        /// <remarks>オブジェクトが<c>null</c>の場合に例外に渡されるパラメータ名は "value"。</remarks>
        /// <exception cref="ArgumentNullException">オブジェクトが<c>null</c>。</exception>
        /// <typeparam name="T">オブジェクトの型。</typeparam>
        public static T NotNull<T>(T obj)
        {
            return Validate.NotNull(obj, "value");
        }

        /// <summary>
        /// 渡されたオブジェクトをチェックし、<c>null</c>の場合に例外をスローする。
        /// </summary>
        /// <param name="obj"><c>null</c>かどうかをチェックするオブジェクト。</param>
        /// <param name="paramName">オブジェクトが<c>null</c>の場合に例外に渡されるパラメータ名。</param>
        /// <returns>渡されたオブジェクト。</returns>
        /// <exception cref="ArgumentNullException">オブジェクトが<c>null</c>。</exception>
        /// <typeparam name="T">オブジェクトの型。</typeparam>
        public static T NotNull<T>(T obj, string paramName)
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
        /// <returns>渡された文字列。</returns>
        /// <remarks>文字列が空の場合に例外に渡されるパラメータ名は "value"。</remarks>
        /// <exception cref="ArgumentNullException">文字列が<c>null</c>。</exception>
        /// <exception cref="ArgumentException">文字列が長さ0。</exception>
        public static string NotEmpty(string str)
        {
            return Validate.NotEmpty(str, "value");
        }

        /// <summary>
        /// 渡された文字列をチェックし、空（<c>null</c>または長さ0）の場合に例外をスローする。
        /// </summary>
        /// <param name="str">空かどうかをチェックする文字列。</param>
        /// <param name="paramName">文字列が空の場合に例外に渡されるパラメータ名。</param>
        /// <returns>渡された文字列。</returns>
        /// <exception cref="ArgumentNullException">文字列が<c>null</c>。</exception>
        /// <exception cref="ArgumentException">文字列が長さ0。</exception>
        public static string NotEmpty(string str, string paramName)
        {
            if (str == null)
            {
                throw new ArgumentNullException(paramName);
            }
            else if (str == String.Empty)
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
        /// <returns>渡された文字列。</returns>
        /// <remarks>文字列が空の場合に例外に渡されるパラメータ名は "value"。</remarks>
        /// <exception cref="ArgumentNullException">文字列が<c>null</c>。</exception>
        /// <exception cref="ArgumentException">文字列が空か空白のみ。</exception>
        public static string NotBlank(string str)
        {
            return Validate.NotBlank(str, "value");
        }

        /// <summary>
        /// 渡された文字列をチェックし、空（<c>null</c>または空か空白のみ）の場合に例外をスローする。
        /// </summary>
        /// <param name="str">空かどうかをチェックする文字列。</param>
        /// <param name="paramName">文字列が空の場合に例外に渡されるパラメータ名。</param>
        /// <returns>渡された文字列。</returns>
        /// <exception cref="ArgumentNullException">文字列が<c>null</c>。</exception>
        /// <exception cref="ArgumentException">文字列が空か空白のみ。</exception>
        public static string NotBlank(string str, string paramName)
        {
            if (str == null)
            {
                throw new ArgumentNullException(paramName);
            }
            else if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("The validated string is blank", paramName);
            }

            return str;
        }

        #endregion
    }
}
