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
        /// <param name="site">このパーサーが対応するMediaWiki</param>
        public MediaWikiRedirectParser(MediaWiki site)
            : base(site)
        {
        }

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// 渡されたテキストをMediaWikiのリダイレクトページとして解析する。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析したリダイレクトリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>MediaWikiのページ全体を渡す必要がある。</remarks>
        public override bool TryParse(string s, out IElement result)
        {
            // 日本語版みたいに、#REDIRECTと言語固有の#転送みたいなのがあると思われるので、
            // 翻訳元言語とデフォルトの設定でチェック
            result = null;
            for (int i = 0; i < 2; i++)
            {
                string format = this.Website.Redirect;
                if (i == 1)
                {
                    format = Settings.Default.MediaWikiRedirect;
                }

                if (!String.IsNullOrEmpty(format)
                    && s.ToLower().StartsWith(format.ToLower()))
                {
                    if (this.LinkParser.TryParse(s.Substring(format.Length).TrimStart(), out result))
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
