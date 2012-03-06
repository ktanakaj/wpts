// ================================================================================================
// <summary>
//      MediaWikiのリダイレクトページを解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiRedirectParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using Honememo.Parsers;
    using Honememo.Wptscs.Properties;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// MediaWikiのリダイレクトページを解析するパーサークラスです。
    /// </summary>
    public class MediaWikiRedirectParser : MediaWikiParser
    {
        #region コンストラクタ

        /// <summary>
        /// 指定されたMediaWikiサーバーのページを解析するためのパーサーを作成する。
        /// </summary>
        /// <param name="site">このパーサーが対応するMediaWiki。</param>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        public MediaWikiRedirectParser(MediaWiki site)
            : base(site)
        {
        }

        #endregion

        #region ITextParserインタフェース実装メソッド

        /// <summary>
        /// 渡されたMediaWikiページをMediaWikiのリダイレクトページとして解析する。
        /// </summary>
        /// <param name="s">解析対象のMediaWikiページ本文。</param>
        /// <param name="condition">解析を終了するかの判定を行うデリゲート。本クラスでは無視されます。</param>
        /// <param name="result">解析したリダイレクトリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>
        /// このメソッドへはMediaWikiのページ全体を渡す必要があります。
        /// また、ページ全体を解析する必要があることから、
        /// <paramref name="condition"/>が指定されていても無視します。
        /// </remarks>
        /// <exception cref="ObjectDisposedException"><see cref="MediaWikiParser.Dispose"/>が実行済みの場合。</exception>
        public override bool TryParseToEndCondition(string s, IsEndCondition condition, out IElement result)
        {
            result = null;
            if (this.LinkParser == null)
            {
                // 子パーサーが解放済みの場合Dispose済みで処理不可
                throw new ObjectDisposedException(this.GetType().Name);
            }
            else if (String.IsNullOrEmpty(s))
            {
                // 入力値が空の場合は即終了
                return false;
            }

            // 日本語版みたいに、#REDIRECTと言語固有の#転送みたいなのがあると思われるので、
            // 翻訳元言語とデフォルトの設定でチェック
            string trim = s.TrimStart();
            string lower = trim.ToLower();
            for (int i = 0; i < 2; i++)
            {
                string format = this.Website.Redirect;
                if (i == 1)
                {
                    format = Settings.Default.MediaWikiRedirect;
                }

                if (!String.IsNullOrEmpty(format)
                    && lower.StartsWith(format.ToLower()))
                {
                    // "#REDIRECT "の部分をカットして後ろの[[～]]の部分のリンクを解析
                    if (this.LinkParser.TryParse(trim.Substring(format.Length).TrimStart(), out result))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        #endregion
    }
}
