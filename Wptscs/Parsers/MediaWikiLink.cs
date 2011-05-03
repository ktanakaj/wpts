// ================================================================================================
// <summary>
//      MediaWikiページの内部リンク要素をあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiLink.cs" company="honeplusのメモ帳">
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
    /// MediaWikiページの内部リンク要素をあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiLink : AbstractElement
    {
        #region 定数

        /// <summary>
        /// 内部リンクの開始タグ。
        /// </summary>
        private static readonly string delimiterStart = "[[";

        /// <summary>
        /// 内部リンクの閉じタグ。
        /// </summary>
        private static readonly string delimiterEnd = "]]";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 内部リンク要素をあらわす空のインスタンスを生成する。
        /// </summary>
        public MediaWikiLink()
        {
            this.PipeTexts = new List<IElement>();
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// リンクの記事名。
        /// </summary>
        /// <remarks>リンクに記載されていた記事名であり、名前空間の情報などは含まない可能性があるため注意。</remarks>
        public virtual string Title
        {
            get;
            set;
        }

        /// <summary>
        /// リンクのセクション名（#）。
        /// </summary>
        public virtual string Section
        {
            get;
            set;
        }

        /// <summary>
        /// リンクのパイプ後の文字列（|）。
        /// </summary>
        public virtual IList<IElement> PipeTexts
        {
            get;
            set;
        }

        /// <summary>
        /// 言語間または他プロジェクトへのリンクの場合、コード。
        /// </summary>
        public virtual string Code
        {
            get;
            set;
        }

        /// <summary>
        /// リンクの先頭が : で始まるかを示すフラグ。
        /// </summary>
        public virtual bool IsColon
        {
            get;
            set;
        }

        /// <summary>
        /// 記事名の先頭がサブページを示す / で始まるか？
        /// </summary>
        /// <remarks>※ 2011年5月現在、この処理には不足あり。</remarks>
        public virtual bool IsSubpage
        {
            // TODO: サブページには相対パスで[[../～]]や[[../../～]]というような書き方もある模様。
            //       この辺りの処理は[[Help:サブページ]]を元に全面的に見直す必要あり
            get;
            set;
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 渡されたテキストをMediaWikiの内部リンクとして解析する。
        /// </summary>
        /// <param name="s">[[で始まる文字列。</param>
        /// <param name="parser">解析に使用するパーサー。</param>
        /// <param name="result">解析したリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public static bool TryParse(string s, MediaWikiParser parser, out MediaWikiLink result)
        {
            // 出力値初期化
            result = null;

            // 入力値確認
            if (!s.StartsWith(MediaWikiLink.delimiterStart))
            {
                return false;
            }

            // 構文を解析して、[[]]内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            string section = null;
            IList<IElement> pipeTexts = new List<IElement>();
            int lastIndex = -1;
            int pipeCounter = 0;
            bool sharpFlag = false;
            for (int i = 2; i < s.Length; i++)
            {
                char c = s[i];

                // ]]が見つかったら、処理正常終了
                if (StringUtils.StartsWith(s, MediaWikiLink.delimiterEnd, i))
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は表示名などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    pipeTexts.Add(new TextElement(String.Empty));
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                string dummy;
                string variable;
                int index = parser.ChkVariable(out variable, out dummy, s, i);
                if (index != -1)
                {
                    i = index;
                    if (pipeCounter > 0)
                    {
                        ((TextElement)pipeTexts[pipeCounter - 1]).Text += variable;
                    }
                    else if (sharpFlag)
                    {
                        section += variable;
                    }
                    else
                    {
                        article += variable;
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
                    if (c == '<')
                    {
                        string subtext = s.Substring(i);
                        CommentElement comment;
                        string value;
                        if (CommentElement.TryParseLazy(subtext, out comment))
                        {
                            // コメント（<!--）が含まれている場合、リンクは無効
                            break;
                        }
                        else if (MediaWikiParser.TryParseNowiki(subtext, out value))
                        {
                            // nowikiブロック
                            i += value.Length - 1;
                            ((TextElement)pipeTexts[pipeCounter - 1]).Text += value;
                            continue;
                        }
                    }

                    // リンク [[ {{ （[[image:xx|[[test]]の画像]]とか）の再帰チェック
                    IElement l;
                    index = parser.ChkLinkText(out l, s, i);
                    if (index != -1)
                    {
                        i = index;
                        ((TextElement)pipeTexts[pipeCounter - 1]).Text += l.ToString();
                        continue;
                    }

                    ((TextElement)pipeTexts[pipeCounter - 1]).Text += c;
                }
            }

            // 解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            result = new MediaWikiLink();

            // 変数ブロックの文字列をリンクのテキストに設定
            result.ParsedString = s.Substring(0, lastIndex + 1);

            // 前後のスペースは削除（見出しは後ろのみ）
            result.Title = article.Trim();
            result.Section = section != null ? section.TrimEnd() : section;

            // | 以降はそのまま設定
            result.PipeTexts = pipeTexts;

            // 記事名から情報を抽出
            // サブページ
            if (result.Title.StartsWith("/"))
            {
                result.IsSubpage = true;
            }
            else if (result.Title.StartsWith(":"))
            {
                // 先頭が :
                result.IsColon = true;
                result.Title = result.Title.TrimStart(':').TrimStart();
            }

            // 標準名前空間以外で[[xxx:yyy]]のようになっている場合、言語コード
            if (result.Title.Contains(":") && new MediaWikiPage(parser.Website, result.Title).IsMain())
            {
                // ※本当は、言語コード等の一覧を作り、其処と一致するものを・・・とすべきだろうが、
                //   メンテしきれないので : を含む名前空間以外を全て言語コード等と判定
                result.Code = result.Title.Substring(0, result.Title.IndexOf(':')).TrimEnd();
                result.Title = result.Title.Substring(result.Title.IndexOf(':') + 1).TrimStart();
            }

            return true;
        }

        /// <summary>
        /// 渡された文字が<c>TryParse</c>等の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。</returns>
        /// <remarks>性能対策などで処理自体を呼ばせたく無い場合用。</remarks>
        public static bool IsElementPossible(char c)
        {
            return MediaWikiLink.delimiterStart[0] == c;
        }

        #endregion

        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// この要素を書式化した内部リンクテキストを返す。
        /// </summary>
        /// <returns>内部リンクテキスト。</returns>
        protected override string ToStringImpl()
        {
            // 戻り値初期化
            StringBuilder b = new StringBuilder();
            
            // 開始タグの付加
            b.Append(MediaWikiLink.delimiterStart);

            // 先頭の : の付加
            if (this.IsColon)
            {
                b.Append(':');
            }

            // 言語コード・他プロジェクトコードの付加
            if (!String.IsNullOrEmpty(this.Code))
            {
                b.Append(this.Code);
                b.Append(':');
            }

            // リンクの付加
            if (!String.IsNullOrEmpty(this.Title))
            {
                b.Append(this.Title);
            }

            // セクション名の付加
            if (!String.IsNullOrEmpty(this.Section))
            {
                b.Append('#');
                b.Append(this.Section);
            }

            // パイプ後の文字列の付加
            if (this.PipeTexts != null)
            {
                foreach (IElement p in this.PipeTexts)
                {
                    b.Append('|');
                    b.Append(p.ToString());
                }
            }

            // 閉じタグの付加
            b.Append(MediaWikiLink.delimiterEnd);
            return b.ToString();
        }

        #endregion
    }
}
