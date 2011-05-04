// ================================================================================================
// <summary>
//      MediaWikiのテンプレート要素を解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiTemplateParser.cs" company="honeplusのメモ帳">
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

    /// <summary>
    /// MediaWikiのテンプレート要素を解析するパーサークラスです。
    /// </summary>
    public class MediaWikiTemplateParser : AbstractParser
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
        public MediaWikiTemplateParser(MediaWikiParser parser)
        {
            this.parser = parser;
        }

        #endregion
        
        #region インタフェース実装メソッド
        
        /// <summary>
        /// 渡されたテキストをMediaWikiのテンプレートとして解析する。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析したリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public override bool TryParse(string s, out IElement result)
        {
            // 出力値初期化
            result = null;

            // 入力値確認
            if (!s.StartsWith(MediaWikiTemplate.DelimiterStart))
            {
                return false;
            }

            // 構文を解析して、{{}}内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            IList<StringBuilder> pipeTexts = new List<StringBuilder>();
            int lastIndex = -1;
            int pipeCounter = 0;
            for (int i = 2; i < s.Length; i++)
            {
                char c = s[i];

                // }}が見つかったら、処理正常終了
                if (StringUtils.StartsWith(s, MediaWikiTemplate.DelimiterEnd, i))
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は引数などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    pipeTexts.Add(new StringBuilder());
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                IElement variable;
                if (this.TryParseElement(s, i, out variable, this.parser.VariableParser))
                {
                    i += variable.ToString().Length - 1;
                    if (pipeCounter > 0)
                    {
                        pipeTexts[pipeCounter - 1].Append(variable.ToString());
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
                    // 変数以外で < > [ ] { } が含まれている場合、リンクは無効
                    if ((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}'))
                    {
                        break;
                    }

                    article += c;
                }
                else
                {
                    // | の後のとき
                    IElement element;
                    if (this.TryParseElement(s, i, out element, this.parser.CommentParser))
                    {
                        // ここにコメント（<!--）が含まれている場合、リンクは無効
                        break;
                    }
                    else if (this.TryParseElement(s, i, out element, this.parser.NowikiParser, this.parser.LinkParser, this))
                    {
                        // nowikiまたはリンク [[ {{ （{{test|[[例]]}}とか）の再帰チェック
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
            // 前後のスペース・改行は削除（見出しは後ろのみ）
            MediaWikiTemplate template = new MediaWikiTemplate(article.Trim());

            // 変数ブロックの文字列をリンクのテキストに設定
            template.ParsedString = s.Substring(0, lastIndex + 1);

            // | 以降は再帰的に解析して設定
            template.PipeTexts = new List<IElement>();
            foreach (StringBuilder b in pipeTexts)
            {
                template.PipeTexts.Add(this.parser.Parse(b.ToString()));
            }

            // 記事名から情報を抽出
            // サブページ
            if (template.Title.StartsWith("/") == true)
            {
                template.IsSubpage = true;
            }
            else if (template.Title.StartsWith(":"))
            {
                // 先頭が :
                template.IsColon = true;
                template.Title = template.Title.TrimStart(':').TrimStart();
            }

            // 先頭が msgnw:
            template.IsMsgnw = template.Title.ToLower().StartsWith(MediaWikiTemplate.Msgnw.ToLower());
            if (template.IsMsgnw)
            {
                template.Title = template.Title.Substring(MediaWikiTemplate.Msgnw.Length);
            }

            // 記事名直後の改行の有無
            if (article.TrimEnd(' ').EndsWith("\n"))
            {
                template.NewLine = true;
            }

            result = template;
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
            return MediaWikiTemplate.DelimiterStart[0] == c;
        }

        #endregion
    }
}
