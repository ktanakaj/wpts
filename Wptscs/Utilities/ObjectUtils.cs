// ================================================================================================
// <summary>
//      Apache Commons Lang の ObjectUtilsを参考にしたユーティリティクラスソース。</summary>
//
// <copyright file="ObjectUtils.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;

    /// <summary>
    /// Apache Commons Lang の ObjectUtilsを参考にしたユーティリティクラスです。
    /// </summary>
    /// <remarks>http://commons.apache.org/lang/api/org/apache/commons/lang/ObjectUtils.html</remarks>
    public static class ObjectUtils
    {
        #region 初期化メソッド

        /// <summary>
        /// オブジェクトが<c>null</c>の場合に指定されたオブジェクトを返す。
        /// </summary>
        /// <param name="obj">テストするオブジェクト。<c>null</c>も可。</param>
        /// <param name="defaultValue">渡されたオブジェクトが<c>null</c>の場合に返されるデフォルトのオブジェクト。</param>
        /// <returns>渡されたオブジェクト、<c>null</c>の場合にはデフォルトのオブジェクト。</returns>
        public static T DefaultIfNull<T>(T obj, T defaultValue)
        {
            if (obj == null)
            {
                return defaultValue;
            }

            return obj;
        }

        #endregion

        #region null値許容メソッド

        /// <summary>
        /// オブジェクトが<c>null</c>の場合に空の文字列を返す<c>ToString</c>。
        /// </summary>
        /// <param name="obj"><c>ToString</c>するオブジェクト。<c>null</c>も可。</param>
        /// <returns>渡されたオブジェクトを<c>ToString</c>した結果。<c>null</c>の場合には空の文字列。</returns>
        public static string ToString(object obj)
        {
            return ToString(obj, String.Empty);
        }

        /// <summary>
        /// オブジェクトが<c>null</c>の場合に指定された文字列を返す<c>ToString</c>。
        /// </summary>
        /// <param name="obj"><c>ToString</c>するオブジェクト。<c>null</c>も可。</param>
        /// <param name="nullStr">渡されたオブジェクトが<c>null</c>の場合に返される文字列。<c>null</c>も可。</param>
        /// <returns>渡されたオブジェクトを<c>ToString</c>した結果。<c>null</c>の場合には指定された文字列。</returns>
        public static string ToString(object obj, string nullStr)
        {
            if (obj == null)
            {
                return nullStr;
            }

            return obj.ToString();
        }

        #endregion
    }
}
