// ================================================================================================
// <summary>
//      IParserの結果をキャッシュするラッパークラスソース</summary>
//
// <copyright file="CacheParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using Honememo.Models;
    using Honememo.Utilities;

    /// <summary>
    /// <see cref="IParser"/>の結果をキャッシュするラッパークラスです。
    /// </summary>
    /// <remarks>キャッシュは同じインスタンス内のみで有効。</remarks>
    public class CacheParser : IParser
    {
        #region 定数

        /// <summary>
        /// デフォルトのキャッシュ最大件数。
        /// </summary>
        private static readonly int DefaultCacheCapacity = 30;

        #endregion

        #region private変数

        /// <summary>
        /// ラップするパーサー。
        /// </summary>
        private IParser parser;

        /// <summary>
        /// キャッシュ。
        /// </summary>
        private MemoryCache<string, IElement> caches;

        #endregion

        #region コンストラクタ
        
        /// <summary>
        /// 指定されたキャッシュ最大件数で、渡されたパーサーをラップするインスタンスを作成。
        /// </summary>
        /// <param name="parser">ラップするインスタンス。</param>
        /// <param name="capacity">キャッシュ最大件数。</param>
        /// <exception cref="ArgumentNullException"><paramref name="parser"/>が<c>null</c>。</exception>
        /// <exception cref="ArgumentException"><paramref name="capacity"/>が0以下の値。</exception>
        public CacheParser(IParser parser, int capacity)
        {
            this.parser = Validate.NotNull(parser);
            this.caches = new MemoryCache<string, IElement>(capacity);
        }

        /// <summary>
        /// デフォルトのキャッシュ最大件数（30件）で、渡されたパーサーをラップするインスタンスを作成。
        /// </summary>
        /// <param name="parser">ラップするインスタンス。</param>
        /// <exception cref="ArgumentNullException"><paramref name="parser"/>が<c>null</c>。</exception>
        public CacheParser(IParser parser)
            : this(parser, DefaultCacheCapacity)
        {
        }

        #endregion

        #region IParserインタフェース実装メソッド

        /// <summary>
        /// ラップしているパーサーを用いて、渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <returns>解析結果。</returns>
        /// <exception cref="FormatException">
        /// 文字列が解析できないフォーマットの場合。
        /// <see cref="TryParse"/>にて解析失敗がキャッシュされている場合もこの例外を返す。
        /// </exception>
        /// <remarks>
        /// 指定された文字列に対するキャッシュが存在する場合、ラップしているパーサーを呼び出さずに結果を返す。
        /// ただし解析失敗の場合、その結果はキャッシュされない。
        /// </remarks>
        public IElement Parse(string s)
        {
            IElement result = this.caches.GetAndAddIfEmpty(
                s,
                (string str) => this.parser.Parse(str));
            if (result == null)
            {
                throw new FormatException("cache is null");
            }

            return result;
        }

        /// <summary>
        /// ラップしているパーサーを用いて、渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析結果。解析失敗の場合<c>null</c>。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>
        /// 指定された文字列に対するキャッシュが存在する場合、ラップしているパーサーを呼び出さずに結果を返す。
        /// もしラップしているパーサーが解析成功で<paramref name="result"/>が<c>null</c>
        /// や、失敗で<c>null</c>以外の値を返す場合、このメソッドの結果は元のパーサーと一致しない。
        /// </remarks>
        public bool TryParse(string s, out IElement result)
        {
            // TryParseが呼ばれた場合その処理結果を返す。
            // キャッシュの場合キャッシュに値があれば成功と返す。
            bool called = false;
            bool success = false;
            result = this.caches.GetAndAddIfEmpty(
                s,
                (string str)
                    =>
                {
                    called = true;
                    IElement element;
                    success = this.parser.TryParse(str, out element);
                    return success ? element : null;
                });
            return (called && success) || (!called && result != null);
        }

        /// <summary>
        /// ラップしているパーサーを用いて、渡された文字が<see cref="Parse"/>,
        /// <see cref="TryParse"/>の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。このクラスでは常に<c>true</c>を返す。</returns>
        /// <remarks>性能対策などで<see cref="TryParse"/>を呼ぶ前に目処を付けたい場合用。</remarks>
        public bool IsPossibleParse(char c)
        {
            return this.parser.IsPossibleParse(c);
        }

        #endregion
    }
}
