// ================================================================================================
// <summary>
//      MediaWikiのテンプレート要素を解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiTemplateParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
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

            // 開始条件 {{ のチェック
            if (!s.StartsWith(MediaWikiTemplate.DelimiterStart))
            {
                return false;
            }

            // 構文を解析して、{{}}内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            StringBuilder article = new StringBuilder();
            IList<IElement> pipeTexts = new List<IElement>();
            int lastIndex = -1;
            for (int i = MediaWikiTemplate.DelimiterStart.Length; i < s.Length; i++)
            {
                char c = s[i];

                // 終了条件 }} のチェック
                if (StringUtils.StartsWith(s, MediaWikiTemplate.DelimiterEnd, i))
                {
                    lastIndex = ++i;
                    break;
                }

                // {{テンプレート名|パラメータ1|パラメータ2}} といったフォーマットのため、| の前後で処理を変更
                if (c == '|')
                {
                    // | の後（パラメータなど）には何でもありえるので親のパーサーで再帰的に解析
                    IElement element;
                    if (!this.parser.TryParseToDelimiter(StringUtils.Substring(s, i + 1), out element, MediaWikiTemplate.DelimiterEnd, "|"))
                    {
                        // 平文でも解析するメソッドのため、基本的に失敗することは無い
                        // 万が一の場合は解析失敗とする
                        break;
                    }

                    i += element.ToString().Length;
                    pipeTexts.Add(element);
                    continue;
                }
                else
                {
                    // | の前（記事名などの部分）のとき、変数・コメントの再帰チェック
                    IElement element;
                    if (this.TryParseAt(s, i, out element, this.parser.CommentParser, this.parser.VariableParser))
                    {
                        // 変数・コメントなら、解析したブロック単位でテンプレート名に追加
                        i += element.ToString().Length - 1;
                        article.Append(element.ToString());
                        continue;
                    }

                    // 変数・コメント以外で < > [ ] { } が含まれている場合、リンクは無効
                    // TODO: <noinclude>も含まれていてOKだが、2012年1月現在未対応
                    if ((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}'))
                    {
                        break;
                    }

                    // それ以外の普通の文字なら1文字ずつテンプレート名に追加
                    article.Append(c);
                }
            }

            // 終了条件でループを抜けていない場合、解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            result = this.MakeElement(article.ToString(), pipeTexts, s.Substring(0, lastIndex + 1));
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

        #region 内部処理用メソッド

        /// <summary>
        /// テンプレートタグを解析した結果から、MediaWikiテンプレート要素を生成する。
        /// </summary>
        /// <param name="article">テンプレートタグ上のテンプレート名部分の文字列。</param>
        /// <param name="pipeTexts">テンプレートタグ上のパイプ後の文字列。</param>
        /// <param name="parsedString">解析したテンプレートタグの文字列。</param>
        /// <returns>生成したテンプレート要素。</returns>
        private MediaWikiTemplate MakeElement(string article, IList<IElement> pipeTexts, string parsedString)
        {
            // 解析結果を各種属性に格納
            // テンプレート名には、前後のスペース・改行を削除した値を設定
            MediaWikiTemplate template = new MediaWikiTemplate(article.Trim());
            template.ParsedString = parsedString;
            template.PipeTexts = pipeTexts;

            // 記事名から情報を抽出
            if (template.Title.StartsWith("/") == true)
            {
                // サブページ（{{/サブページ}}みたいなの）
                template.IsSubpage = true;
            }
            else if (template.Title.StartsWith(":"))
            {
                // 先頭が :（テンプレート名前空間ではなく標準名前空間となる）
                template.IsColon = true;
                template.Title = template.Title.TrimStart(':').TrimStart();
            }

            // 先頭が msgnw:
            template.IsMsgnw = template.Title.ToLower().StartsWith(MediaWikiTemplate.Msgnw.ToLower());
            if (template.IsMsgnw)
            {
                template.Title = template.Title.Substring(MediaWikiTemplate.Msgnw.Length);
            }

            // 記事名直後の改行の有無を記録
            if (article.TrimEnd(' ').EndsWith("\n"))
            {
                template.NewLine = true;
            }

            return template;
        }

        #endregion
    }
}
