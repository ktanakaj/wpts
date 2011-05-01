// ================================================================================================
// <summary>
//      文字列解析処理用パーサーのインタフェースソース</summary>
//
// <copyright file="IParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;

    /// <summary>
    /// 文字列解析処理用パーサーのインタフェースです。
    /// </summary>
    public interface IParser
    {
        #region メソッド

        /// <summary>
        /// 渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <returns>解析結果。</returns>
        /// <exception cref="FormatException">文字列が解析できないフォーマットの場合。</exception>
        /// <remarks>解析に失敗した場合は、各種例外を投げる。</remarks>
        IElement Parse(string s);

        /// <summary>
        /// 渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析結果。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        bool TryParse(string s, out IElement result);

        #endregion
    }
}
