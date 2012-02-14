// ================================================================================================
// <summary>
//      MediaWikiの内部リンク要素を解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiLinkParser.cs" company="honeplusのメモ帳">
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

            // 開始条件 [[ のチェック
            if (!s.StartsWith(MediaWikiLink.DelimiterStart))
            {
                return false;
            }

            // 構文を解析して、[[]]内部の文字列を取得
            // ※ 構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            StringBuilder article = new StringBuilder();
            StringBuilder section = null;
            IList<IElement> pipeTexts = new List<IElement>();
            int lastIndex = -1;
            for (int i = MediaWikiLink.DelimiterStart.Length; i < s.Length; i++)
            {
                char c = s[i];

                // 終了条件 ]] のチェック
                if (StringUtils.StartsWith(s, MediaWikiLink.DelimiterEnd, i))
                {
                    lastIndex = ++i;
                    break;
                }

                // [[記事名#セクション|表示名]], [[File:ファイル名|パラメータ1|パラメータ2]]
                // といったフォーマットのため、| の前後で処理を変更
                if (c == '|')
                {
                    // | の後（表示名やパラメータ）には何でもありえるので親のパーサーで再帰的に解析
                    IElement element;
                    if (!this.parser.TryParseToDelimiter(StringUtils.Substring(s, i + 1), out element, MediaWikiLink.DelimiterEnd, "|"))
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
                        // 変数・コメントなら、解析したブロック単位で記事名orセクションに追加
                        i += element.ToString().Length - 1;
                        if (section != null)
                        {
                            section.Append(element.ToString());
                        }
                        else
                        {
                            article.Append(element.ToString());
                        }

                        continue;
                    }

                    // 変数・コメント以外で { } または < > [ ] \n が含まれている場合、リンクは無効
                    // TODO: <noinclude>も含まれていてOKだが、2012年1月現在未対応
                    if ((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}') || (c == '\n'))
                    {
                        break;
                    }

                    if (section == null)
                    {
                        // [[記事名#セクション]] のため、記事名解析中に # が登場したら格納先変更
                        if (c == '#')
                        {
                            section = new StringBuilder();
                        }
                        else
                        {
                            // それ以外の普通の文字なら1文字ずつ記事名に追加
                            article.Append(c);
                        }
                    }
                    else
                    {
                        // セクション解析中の場合、普通の文字なら1文字ずつセクションに追加
                        section.Append(c);
                    }
                }
            }

            // 終了条件でループを抜けていない場合、解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            result = this.MakeElement(
                article.ToString(),
                section != null ? section.ToString() : null,
                pipeTexts,
                s.Substring(0, lastIndex + 1));
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
        
        #region 内部処理用メソッド

        /// <summary>
        /// 内部リンクタグを解析した結果から、MediaWikiリンク要素を生成する。
        /// </summary>
        /// <param name="article">内部リンクタグ上の記事名部分の文字列。</param>
        /// <param name="section">内部リンクタグ上のセクション部分の文字列。</param>
        /// <param name="pipeTexts">内部リンクタグ上のパイプ後の文字列。</param>
        /// <param name="parsedString">解析した内部リンクタグの文字列。</param>
        /// <returns>生成したリンク要素。</returns>
        private MediaWikiLink MakeElement(string article, string section, IList<IElement> pipeTexts, string parsedString)
        {
            MediaWikiLink link = new MediaWikiLink();

            // 解析した内部リンクの素のテキストを保存
            link.ParsedString = parsedString;

            // 前後のスペースは削除（見出しは後ろのみ）
            link.Title = article.Trim();
            link.Section = section != null ? section.TrimEnd() : null;

            // | 以降は再帰的に解析した値を設定
            link.PipeTexts = pipeTexts;

            // 記事名から情報を抽出
            if (link.Title.StartsWith("/"))
            {
                // サブページ（[[/サブページ]]みたいなの）
                link.IsSubpage = true;
            }
            else if (link.Title.StartsWith(":"))
            {
                // 先頭が :（[[:en:他言語版記事名]]みたいなの）
                link.IsColon = true;
                link.Title = link.Title.TrimStart(':').TrimStart();
            }

            // 記事名の前にウィキ間リンク（en:やcommons:）が付加されているか？
            if (this.parser.Website.IsInterwiki(link.Title))
            {
                // ウィキ間リンク部分を記事名から分離
                // ※ 厳密には入れ子もあるが、2012年2月現在未対応（ウィキ間リンクの時点でこのツールの処理対象外だが）
                link.Interwiki = link.Title.Substring(0, link.Title.IndexOf(':')).TrimEnd();
                link.Title = link.Title.Substring(link.Title.IndexOf(':') + 1).TrimStart();
            }

            return link;
        }

        #endregion
    }
}
