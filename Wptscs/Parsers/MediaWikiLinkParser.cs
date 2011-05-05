// ================================================================================================
// <summary>
//      MediaWikiの内部リンク要素を解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiLinkParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Honememo.Parsers;
    using Honememo.Utilities;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// MediaWikiの内部リンク要素を解析するパーサークラスです。
    /// </summary>
    public class MediaWikiLinkParser : AbstractParser
    {
        #region private変数

        /// <summary>
        /// このパーサーが参照する<see cref="MediaWikiParser"/>。
        /// </summary>
        private MediaWikiParser parser;

        #endregion
        
        #region コンストラクタ

        /// <summary>
        /// 指定された<see cref="MediaWikiParser"/>を元に見出しを解析するためのパーサーを作成する。
        /// </summary>
        /// <param name="parser">このパーサーが参照する<see cref="MediaWikiParser"/>。</param>
        public MediaWikiLinkParser(MediaWikiParser parser)
        {
            this.parser = parser;
        }

        #endregion
        
        #region インタフェース実装メソッド
        
        /// <summary>
        /// 渡されたテキストをMediaWikiの内部リンクとして解析する。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析したリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public override bool TryParse(string s, out IElement result)
        {
            // 出力値初期化
            result = null;

            // 入力値確認
            if (!s.StartsWith(MediaWikiLink.DelimiterStart))
            {
                return false;
            }

            // 構文を解析して、[[]]内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            string section = null;
            IList<StringBuilder> pipeTexts = new List<StringBuilder>();
            int lastIndex = -1;
            int pipeCounter = 0;
            bool sharpFlag = false;
            for (int i = 2; i < s.Length; i++)
            {
                char c = s[i];

                // ]]が見つかったら、処理正常終了
                if (StringUtils.StartsWith(s, MediaWikiLink.DelimiterEnd, i))
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は表示名などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    pipeTexts.Add(new StringBuilder());
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                // TODO: これをこのまま返してよいかよう検討
                IElement variable;
                if (this.TryParseAt(s, i, out variable, this.parser.VariableParser))
                {
                    i += variable.ToString().Length - 1;
                    if (pipeCounter > 0)
                    {
                        pipeTexts[pipeCounter - 1].Append(variable.ToString());
                    }
                    else if (sharpFlag)
                    {
                        section += variable.ToString();
                    }
                    else
                    {
                        article += variable.ToString();
                    }

                    continue;
                }

                // | の前のとき
                if (pipeCounter <= 0)
                {
                    // 変数以外で { } または < > [ ] \n が含まれている場合、リンクは無効
                    if ((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}') || (c == '\n'))
                    {
                        break;
                    }

                    // # の前のとき
                    if (!sharpFlag)
                    {
                        // #が含まれている場合、以降の文字列は見出しへのリンクとして扱う（1つめの#のみ有効）
                        if (c == '#')
                        {
                            sharpFlag = true;
                            section = String.Empty;
                        }
                        else
                        {
                            article += c;
                        }
                    }
                    else
                    {
                        // # の後のとき
                        section += c;
                    }
                }
                else
                {
                    // | の後のとき
                    IElement element;
                    if (this.TryParseAt(s, i, out element, this.parser.CommentParser))
                    {
                        // ここにコメント（<!--）が含まれている場合、リンクは無効
                        break;
                    }
                    else if (this.TryParseAt(s, i, out element, this.parser.NowikiParser, this, this.parser.TemplateParser))
                    {
                        // nowikiまたはリンク [[ {{ （[[image:xx|[[test]]の画像]]とか）の再帰
                        i += element.ToString().Length - 1;
                        pipeTexts[pipeCounter - 1].Append(element.ToString());
                        continue;
                    }

                    pipeTexts[pipeCounter - 1].Append(c);
                }
            }

            // 解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            MediaWikiLink link = new MediaWikiLink();

            // 変数ブロックの文字列をリンクのテキストに設定
            link.ParsedString = s.Substring(0, lastIndex + 1);

            // 前後のスペースは削除（見出しは後ろのみ）
            link.Title = article.Trim();
            link.Section = section != null ? section.TrimEnd() : section;

            // | 以降は再帰的に解析して設定
            link.PipeTexts = new List<IElement>();
            foreach (StringBuilder b in pipeTexts)
            {
                link.PipeTexts.Add(this.parser.Parse(b.ToString()));
            }

            // 記事名から情報を抽出
            // サブページ
            if (link.Title.StartsWith("/"))
            {
                link.IsSubpage = true;
            }
            else if (link.Title.StartsWith(":"))
            {
                // 先頭が :
                link.IsColon = true;
                link.Title = link.Title.TrimStart(':').TrimStart();
            }

            // 標準名前空間以外で[[xxx:yyy]]のようになっている場合、言語コード
            if (link.Title.Contains(":") && new MediaWikiPage(this.parser.Website, link.Title).IsMain())
            {
                // ※本当は、言語コード等の一覧を作り、其処と一致するものを・・・とすべきだろうが、
                //   メンテしきれないので : を含む名前空間以外を全て言語コード等と判定
                link.Code = link.Title.Substring(0, link.Title.IndexOf(':')).TrimEnd();
                link.Title = link.Title.Substring(link.Title.IndexOf(':') + 1).TrimStart();
            }

            result = link;
            return true;
        }

        /// <summary>
        /// 渡された文字が<see cref="TryParse"/>等の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。このクラスでは常に<c>true</c>を返す。</returns>
        /// <remarks>性能対策などで<see cref="TryParse"/>を呼ぶ前に目処を付けたい場合用。</remarks>
        public override bool IsPossibleParse(char c)
        {
            return MediaWikiLink.DelimiterStart[0] == c;
        }
        
        #endregion
    }
}
