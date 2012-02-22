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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using Honememo.Utilities;

    /// <summary>
    /// <see cref="IParser"/>の結果をキャッシュするラッパークラスです。
    /// </summary>
    /// <remarks>キャッシュは同じインスタンス内のみで有効。</remarks>
    public class CacheParser : IParser, IDisposable
    {
        #region private変数

        /// <summary>
        /// ラップするパーサー。
        /// </summary>
        private IParser parser;

        /// <summary>
        /// キャッシュ。
        /// </summary>
        private IDictionary<string, IElement> caches = new ConcurrentDictionary<string, IElement>();

        /// <summary>
        /// ロックオブジェクト。
        /// </summary>
        private HashLock cacheLock = new HashLock();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたパーサーをラップするインスタンスを作成。
        /// </summary>
        /// <param name="parser">ラップするインスタンス。</param>
        /// <exception cref="ArgumentNullException"><para>parser</para>が<c>null</c>。</exception>
        public CacheParser(IParser parser)
        {
            this.parser = Validate.NotNull(parser);
        }

        #endregion

        #region デストラクタ

        /// <summary>
        /// オブジェクトのリソースを破棄する。
        /// </summary>
        /// <remarks><see cref="Dispose"/>の呼び出しのみ。</remarks>
        ~CacheParser()
        {
            this.Dispose();
        }

        #endregion

        #region デリゲート

        /// <summary>
        /// <see cref="GetSetCache"/>でキャッシュが存在しない場合に解析に用いる処理。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <returns>解析結果。解析失敗時は<c>null</c>。</returns>
        private delegate IElement ReturnElement(string s);

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
            IElement result = this.GetSetCache(s, (string str) => this.parser.Parse(str));
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
        /// もしラップしているパーサーが解析成功で<para>result</para>が<c>null</c>
        /// や、失敗で<c>null</c>以外の値を返す場合、このメソッドの結果は元のパーサーと一致しない。
        /// </remarks>
        public bool TryParse(string s, out IElement result)
        {
            // TryParseが呼ばれた場合その処理結果を返す。
            // キャッシュの場合キャッシュに値があれば成功と返す。
            bool called = false;
            bool success = false;
            result = this.GetSetCache(
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

        #region IDisposableインタフェース実装メソッド

        /// <summary>
        /// ロックオブジェクトを解放する。
        /// </summary>
        public virtual void Dispose()
        {
            // ロックオブジェクトを解放
            if (this.cacheLock != null)
            {
                this.cacheLock.Dispose();
            }

            // ファイナライザ（このクラスではDisposeを呼ぶだけ）が不要であることを通知
            GC.SuppressFinalize(this);
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// キャッシュされた要素の取得を行う。
        /// もし存在しない場合指定された処理を用いて解析を行い、
        /// その結果をキャッシュに登録して返す。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="function">キャッシュが存在しない場合に解析に用いる処理。</param>
        /// <returns>キャッシュから取得した、または解析した結果の要素。</returns>
        /// <remarks>キャッシュアクセスのロックは解析対象の文字列単位で行う。</remarks>
        private IElement GetSetCache(string s, ReturnElement function)
        {
            // 読み取りの同期を取りつつ、キャッシュされた要素を取得
            IElement element;
            ReaderWriterLockSlim lockObject = this.cacheLock.GetReaderWriterLock(s);
            lockObject.EnterReadLock();
            try
            {
                if (this.caches.TryGetValue(s, out element))
                {
                    return element;
                }
            }
            finally
            {
                lockObject.ExitReadLock();
            }

            // 存在しない場合、まず更新準備の同期を取りつつキャッシュを再確認
            lockObject.EnterUpgradeableReadLock();
            try
            {
                if (this.caches.TryGetValue(s, out element))
                {
                    return element;
                }

                // それでも無ければ、更新の同期を取りつつラップメソッドを呼び出し、
                // 取得した要素をキャッシュに登録
                lockObject.EnterWriteLock();
                try
                {
                    element = function(s);
                    this.caches[s] = element;
                }
                finally
                {
                    lockObject.ExitWriteLock();
                }
            }
            finally
            {
                lockObject.ExitUpgradeableReadLock();
            }

            return element;
        }

        #endregion
    }
}
