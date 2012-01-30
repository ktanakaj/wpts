// ================================================================================================
// <summary>
//      テキスト全体のような文字列の解析処理用パーサーのインタフェースソース</summary>
//
// <copyright file="ITextParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// <see cref="TryParseToEndCondition"/>の終了条件を判定するためのデリゲート。
    /// </summary>
    /// <param name="s">解析対象の文字列。</param>
    /// <param name="index">処理インデックス。</param>
    /// <returns>終了条件を満たす場合<c>true</c>。</returns>
    public delegate bool IsEndCondition(string s, int index);

    /// <summary>
    /// テキスト全体のような文字列の解析処理用パーサーのインタフェースです。
    /// </summary>
    /// <remarks>特定の要素だけを解析するのではなく、XMLテキスト全体のような文章を解析するパーサー用の仕組みを定義。</remarks>
    public interface ITextParser : IParser
    {
        #region メソッド

        /// <summary>
        /// 渡された文字列に対して、指定された文字列に遭遇するまで解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析結果。</param>
        /// <param name="delimiters">解析を終了する文字列（複数指定可）。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>指定された文字列が出現しない場合、最終位置まで解析を行う。</remarks>
        bool TryParseToDelimiter(string s, out IElement result, params string[] delimiters);

        /// <summary>
        /// 渡された文字列に対して、指定された終了条件を満たすまで解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="condition">解析を終了するかの判定を行うデリゲート。</param>
        /// <param name="result">解析結果。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>指定された終了条件を満たさない場合、最終位置まで解析を行う。</remarks>
        bool TryParseToEndCondition(string s, IsEndCondition condition, out IElement result);

        #endregion
    }
}
