// ================================================================================================
// <summary>
//      IParserを実装するための実装支援用抽象クラスソース</summary>
//
// <copyright file="AbstractParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using Honememo.Utilities;

    /// <summary>
    /// <see cref="IParser"/>を実装するための実装支援用抽象クラスです。
    /// </summary>
    public abstract class AbstractParser : IParser
    {        
        #region インタフェース実装メソッド

        /// <summary>
        /// 渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <returns>解析結果。</returns>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        /// <exception cref="FormatException">文字列が解析できないフォーマットの場合。</exception>
        /// <remarks>
        /// <see cref="TryParse"/>を呼び出す。<see cref="TryParse"/>の結果が<c>false</c>の場合、例外として返す。
        /// </remarks>
        public virtual IElement Parse(string s)
        {
            IElement result;
            if (this.TryParse(Validate.NotNull(s, "s"), out result))
            {
                return result;
            }

            throw new FormatException("Invalid String : " + s);
        }

        /// <summary>
        /// 渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析結果。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public abstract bool TryParse(string s, out IElement result);

        /// <summary>
        /// 渡された文字が<see cref="Parse"/>, <see cref="TryParse"/>の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。このクラスでは常に<c>true</c>を返す。</returns>
        /// <remarks>性能対策などで<see cref="TryParse"/>を呼ぶ前に目処を付けたい場合用。</remarks>
        public virtual bool IsPossibleParse(char c)
        {
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
        /// <param name="parsers">解析に用いるパーサー。指定された順に使用。</param>
        /// <returns>いずれかのパーサーで解析できた場合<c>true</c>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/>または<paramref name="parsers"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentOutOfRangeException">インデックスが文字列の範囲外の場合。</exception>
        protected virtual bool TryParseAt(string s, int index, out IElement result, params IParser[] parsers)
        {
            Validate.InRange(s, index, "s", "index");
            char c = s[index];
            string substr = null;
            foreach (IParser parser in Validate.NotNull(parsers, "parsers"))
            {
                if (parser.IsPossibleParse(c))
                {
                    if (substr == null)
                    {
                        // Substringする負荷も気になるので、TryParseが必要な場合だけ
                        substr = s.Substring(index);
                    }

                    if (parser.TryParse(substr, out result))
                    {
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        #endregion
    }
}
