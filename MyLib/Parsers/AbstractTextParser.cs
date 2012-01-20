// ================================================================================================
// <summary>
//      ITextParserを実装するための実装支援用抽象クラスソース</summary>
//
// <copyright file="AbstractTextParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// ITextParserを実装するための実装支援用抽象クラスです。
    /// </summary>
    public abstract class AbstractTextParser : AbstractParser, ITextParser
    {        
        #region インタフェース実装メソッド

        /// <summary>
        /// 渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析結果。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>
        /// このクラスの実装は、XMLを丸ごと解析するような大きな処理を想定。
        /// 実装として <see cref="TryParseElementAt"/> を呼び出し。
        /// </remarks>
        public override bool TryParse(string s, out IElement result)
        {
            // 正規表現のメソッドを終了条件なしで呼び出し
            int dummy;
            return this.TryParseToRegex(s, null, out result, out dummy);
        }

        /// <summary>
        /// 渡された文字列に対して、指定された文字列に遭遇するまで解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析結果。</param>
        /// <param name="endIndex">終了文字列最終インデックス。指定された文字列で終了しなかった場合は-1。</param>
        /// <param name="delimiters">解析を終了する文字列（複数指定可）。指定が無い場合最後まで解析する。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>指定された正規表現が出現しない場合、最終位置まで解析を行う。</remarks>
        public virtual bool TryParseToDelimiter(string s, out IElement result, out int endIndex, params string[] delimiters)
        {
            // 正規表現に変換し、そちらの処理にまとめる
            // ※ 処理の都合上、正規表現では重いようなら要検討
            if (delimiters == null || delimiters.Length == 0)
            {
                return this.TryParseToRegex(s, null, out result, out endIndex);
            }

            string[] escaped = new string[delimiters.Length];
            for (int i = 0; i < delimiters.Length; i++)
            {
                escaped[i] = Regex.Escape(delimiters[i]);
            }

            return this.TryParseToRegex(s, new Regex("^(" + String.Join("|", escaped) + ")"), out result, out endIndex);
        }

        /// <summary>
        /// 渡された文字列に対して、指定された正規表現にマッチする位置まで解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="regex">解析を終了する正規表現。指定が無い場合最後まで解析する。</param>
        /// <param name="result">解析結果。</param>
        /// <param name="endIndex">終了正規表現最終インデックス。指定された正規表現で終了しなかった場合は-1。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>指定された正規表現が出現しない場合、最終位置まで解析を行う。</remarks>
        public virtual bool TryParseToRegex(string s, Regex regex, out IElement result, out int endIndex)
        {
            // 文字列を1文字ずつチェックし、その内容に応じた要素のリストを作成する
            endIndex = -1;
            ListElement list = new ListElement();
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                // 終了条件のチェック、未指定時は条件なし
                if (regex != null)
                {
                    Match match = regex.Match(s.Substring(i));
                    if (match.Success)
                    {
                        endIndex = i + match.Length;
                        break;
                    }
                }

                // 各要素のTryParse処理を呼び出し
                IElement innerElement;
                if (this.TryParseElementAt(s, i, out innerElement))
                {
                    // それまでに解析済みのテキストを吐き出し、
                    // その後に解析した要素を追加
                    this.FlashText(ref list, ref b);
                    list.Add(innerElement);
                    i += innerElement.ToString().Length - 1;
                    continue;
                }

                // 通常の文字列はテキスト要素として積み上げる
                b.Append(s[i]);
            }

            // 残っていれば最後に解析済みのテキストを吐き出し
            this.FlashText(ref list, ref b);

            result = list;
            if (list.Count == 1)
            {
                // リストが1件であれば、その要素を直に返す
                result = list[0];
            }
            else if (list.Count == 0)
            {
                // 何もなければ、空文字列だったものとして空のテキスト要素を返す
                result = new TextElement();
            }

            return true;
        }

        #endregion

        #region 実装支援用メソッド

        /// <summary>
        /// 渡されたテキストの指定されたインデックス位置を各種解析処理で解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="index">処理インデックス。</param>
        /// <param name="result">解析した結果要素。</param>
        /// <returns>解析できた場合<c>true</c>。</returns>
        /// <exception cref="ArgumentOutOfRangeException">インデックスが文字列の範囲外の場合。</exception>
        /// <exception cref="NotImplementedException">このクラスでは未実装。</exception>
        /// <remarks>
        /// このクラスの<see cref="TryParse"/>実装を用いる場合、
        /// ここで<c>TryParseAt</c>等を用いてそのParserで必要な解析処理呼び出しを列挙する。
        /// </remarks>
        protected virtual bool TryParseElementAt(string s, int index, out IElement result)
        {
            throw new NotImplementedException(this.GetType() + " is not implemented");
        }

        /// <summary>
        /// 文字列が空でない場合、リストにTextエレメントを追加して、文字列をリセットする。
        /// </summary>
        /// <param name="list">追加されるリスト。</param>
        /// <param name="b">追加する文字列。</param>
        protected virtual void FlashText(ref ListElement list, ref StringBuilder b)
        {
            if (b.Length > 0)
            {
                list.Add(new TextElement(b.ToString()));
                b.Clear();
            }
        }

        #endregion
    }
}
