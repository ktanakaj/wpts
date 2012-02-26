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
        #region private変数

        /// <summary>
        /// ラップするパーサー。
        /// </summary>
        private IParser parser;

        /// <summary>
        /// キャッシュ。
        /// </summary>
        private MemoryCache<int, IElement> caches;

        /// <summary>
        /// <see cref="GetAndAddIfEmpty"/>用ロックオブジェクト。
        /// </summary>
        private LockObject lockObject = new LockObject();

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
            this.caches = new MemoryCache<int, IElement>(capacity);
        }

        /// <summary>
        /// デフォルトのキャッシュ最大件数（100件）で、渡されたパーサーをラップするインスタンスを作成。
        /// </summary>
        /// <param name="parser">ラップするインスタンス。</param>
        /// <exception cref="ArgumentNullException"><paramref name="parser"/>が<c>null</c>。</exception>
        public CacheParser(IParser parser)
        {
            this.parser = Validate.NotNull(parser);
            this.caches = new MemoryCache<int, IElement>();
        }

        #endregion

        #region IParserインタフェース実装メソッド

        /// <summary>
        /// ラップしているパーサーを用いて、渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <returns>解析結果。</returns>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
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
            IElement result = this.GetAndAddIfEmpty(
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
            if (s == null)
            {
                // nullだけ先にチェック
                result = null;
                return false;
            }

            // TryParseが呼ばれた場合その処理結果を返す。
            // キャッシュの場合キャッシュに値があれば成功と返す。
            bool called = false;
            bool success = false;
            result = this.GetAndAddIfEmpty(
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

        #region 内部メソッド

        /// <summary>
        /// 指定された文字列に対応する解析結果を取得する。
        /// まずキャッシュから取得を試み、存在しない場合<paramref name="function"/>
        /// に指定された処理で解析を行い、その結果をキャッシュに登録して返す。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="function">キャッシュが存在しない場合に用いる解析処理。</param>
        /// <returns>指定された文字列に対応する解析結果。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/>が<c>null</c>の場合。</exception>
        private IElement GetAndAddIfEmpty(string s, MemoryCache<string, IElement>.ReturnCacheValue function)
        {
            // まずキャッシュを確認
            int hashCode = Validate.NotNull(s).GetHashCode();
            IElement element;
            if (this.TryGetValue(hashCode, s, out element))
            {
                return element;
            }

            // 存在しない場合、function呼び出し用のロックを行いパラメータ取得
            // ※ ロックの単位は解析対象の文字列ごと
            lock (this.lockObject.GetObject(s))
            {
                // 一応もう一度キャッシュを確認
                if (this.TryGetValue(hashCode, s, out element))
                {
                    return element;
                }

                // それでも無ければ、渡されたfunctionで値を解析
                element = function(s);
                this.caches[hashCode] = element;
            }

            return element;
        }

        /// <summary>
        /// ハッシュコードと文字列比較の二段階でキャッシュを取得する。
        /// </summary>
        /// <param name="hashCode">解析対象の文字列のハッシュコード。</param>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="element">キャッシュから取得した解析結果。存在しない場合<c>null</c>。</param>
        /// <returns>キャッシュ取得に成功した場合<c>true</c>。</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IParser"/>の実装は、その処理の都合上<paramref name="s"/>
        /// に解析対象のページの後半部分が丸々入っていることが多く、
        /// それをそのままキャッシュのキーにしてしまうと大量のメモリを浪費してしまうため、
        /// ハッシュをキーとする
        /// （<see cref="IElement"/>だけであれば、解析成功時にはいずれにせよ
        /// オブジェクトが必要なので、メモリ使用量的には影響は小さい）。
        /// </para>
        /// <para>
        /// もし<paramref name="element"/>の<see cref="IElement.ToString()"/>
        /// が元の文字列を保持しない実装の場合、文字列比較が成功しないため、
        /// この処理は常に失敗する。
        /// </para>
        /// </remarks>
        private bool TryGetValue(int hashCode, string s, out IElement element)
        {
            // キャッシュを取得、取得したキャッシュがsと一致するかを検証
            if (this.caches.TryGetValue(hashCode, out element)
                && element != null
                && s.StartsWith(element.ToString()))
            {
                return true;
            }

            element = null;
            return false;
        }

        #endregion
    }
}
