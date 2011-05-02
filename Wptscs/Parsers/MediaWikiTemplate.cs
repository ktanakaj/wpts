// ================================================================================================
// <summary>
//      MediaWikiページのテンプレート要素をあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiTemplate.cs" company="honeplusのメモ帳">
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
    /// MediaWikiページのテンプレート要素をあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiTemplate : MediaWikiLink
    {
        #region 定数

        /// <summary>
        /// テンプレートの開始タグ。
        /// </summary>
        private static readonly string delimiterStart = "{{";

        /// <summary>
        /// テンプレートの閉じタグ。
        /// </summary>
        private static readonly string delimiterEnd = "}}";

        /// <summary>
        /// msgnwの書式。
        /// </summary>
        private static readonly string msgnw = "msgnw:";

        #endregion

        #region private変数

        /// <summary>
        /// テンプレートの記事名。
        /// </summary>
        private string title;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたタイトルのテンプレート要素をあらわすインスタンスを生成する。
        /// </summary>
        /// <param name="title">テンプレート名。</param>
        public MediaWikiTemplate(string title)
        {
            this.Title = title;
            this.PipeTexts = new List<IElement>();
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// テンプレートの記事名。
        /// </summary>
        /// <exception cref="ArgumentNullException">記事名がnullの場合。</exception>
        /// <exception cref="ArgumentException">記事名が空の場合。</exception>
        /// <remarks>テンプレートに記載されていた記事名であり、名前空間の情報などは含まない可能性があるため注意。</remarks>
        public override string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = Validate.NotBlank(value);
            }
        }

        /// <summary>
        /// テンプレートのソースをそのまま出力することを示す msgnw: が付加されているか？
        /// </summary>
        public virtual bool IsMsgnw
        {
            get;
            set;
        }

        /// <summary>
        /// 記事名の後で改行が入るか？
        /// </summary>
        public virtual bool NewLine
        {
            get;
            set;
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 渡されたテキストをMediaWikiのテンプレートとして解析する。
        /// </summary>
        /// <param name="s">{{で始まる文字列。</param>
        /// <param name="parser">解析に使用するパーサー。</param>
        /// <param name="result">解析したテンプレート。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public static bool TryParse(string s, MediaWikiParser parser, out MediaWikiTemplate result)
        {
            // 出力値初期化
            result = null;

            // 入力値確認
            if (!s.StartsWith(MediaWikiTemplate.delimiterStart))
            {
                return false;
            }

            // 構文を解析して、{{}}内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            IList<IElement> pipeTexts = new List<IElement>();
            int lastIndex = -1;
            int pipeCounter = 0;
            for (int i = 2; i < s.Length; i++)
            {
                char c = s[i];

                // }}が見つかったら、処理正常終了
                if (StringUtils.StartsWith(s, MediaWikiTemplate.delimiterEnd, i))
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は引数などとして扱う
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
                    else
                    {
                        article += variable;
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

                    // リンク [[ {{ （{{test|[[例]]}}とか）の再帰チェック
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
            // 前後のスペース・改行は削除（見出しは後ろのみ）
            result = new MediaWikiTemplate(article.Trim());
            
            // | 以降はそのまま設定
            result.PipeTexts = pipeTexts;

            // 記事名から情報を抽出
            // サブページ
            if (result.Title.StartsWith("/") == true)
            {
                result.IsSubpage = true;
            }
            else if (result.Title.StartsWith(":"))
            {
                // 先頭が :
                result.IsColon = true;
                result.Title = result.Title.TrimStart(':').TrimStart();
            }

            // 先頭が msgnw:
            result.IsMsgnw = result.Title.ToLower().StartsWith(MediaWikiTemplate.msgnw.ToLower());
            if (result.IsMsgnw)
            {
                result.Title = result.Title.Substring(MediaWikiTemplate.msgnw.Length);
            }

            // 記事名直後の改行の有無
            if (article.TrimEnd(' ').EndsWith("\n"))
            {
                result.NewLine = true;
            }

            return true;
        }

        /// <summary>
        /// 渡されたテキストをMediaWikiのテンプレートとして解析する。
        /// </summary>
        /// <param name="s">{{で始まる文字列。</param>
        /// <param name="result">解析したテンプレート。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public static bool TryParse(string s, out MediaWikiTemplate result)
        {
            // パーサーにMediaWikiParserの標準設定を指定して解析
            return MediaWikiTemplate.TryParse(s, new MediaWikiParser(), out result);
        }

        /// <summary>
        /// 渡された文字が<c>TryParse</c>等の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。</returns>
        /// <remarks>性能対策などで処理自体を呼ばせたく無い場合用。</remarks>
        public static new bool IsElementPossible(char c)
        {
            return MediaWikiTemplate.delimiterStart[0] == c;
        }

        #endregion

        #region 内部実装メソッド

        /// <summary>
        /// この要素を書式化したテンプレートテキストを返す。
        /// </summary>
        /// <returns>テンプレートテキスト。</returns>
        protected override string ToStringImpl()
        {
            // 戻り値初期化
            StringBuilder b = new StringBuilder();
            
            // 開始タグの付加
            b.Append(MediaWikiTemplate.delimiterStart);

            // 先頭の : の付加
            if (this.IsColon)
            {
                b.Append(':');
            }

            // msgnw: （テンプレートを<nowiki>タグで挟む）の付加
            if (this.IsMsgnw)
            {
                b.Append(MediaWikiTemplate.msgnw);
            }

            // 言語コード・他プロジェクトコードの付加
            if (!String.IsNullOrEmpty(this.Code))
            {
                b.Append(this.Code);
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

            // 改行の付加
            if (this.NewLine)
            {
                b.Append('\n');
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
            b.Append(MediaWikiTemplate.delimiterEnd);
            return b.ToString();
        }

        #endregion
    }
}
