// ================================================================================================
// <summary>
//      MediaWikiのページを解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiParser.cs" company="honeplusのメモ帳">
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
    /// MediaWikiのページを解析するパーサークラスです。
    /// </summary>
    public class MediaWikiParser : XmlParser
    {
        #region private変数

        /// <summary>
        /// このパーサーが対応するMediaWiki。
        /// </summary>
        private MediaWiki website;

        #endregion
        
        #region コンストラクタ

        /// <summary>
        /// 指定されたMediaWikiサーバーのページを解析するためのパーサーを作成する。
        /// </summary>
        /// <param name="site">このパーサーが対応するMediaWiki</param>
        public MediaWikiParser(MediaWiki site)
        {
            this.Website = site;
            this.CommentParser = new XmlCommentElementParser();
            this.NowikiParser = new MediaWikiNowikiParser(this);
            this.LinkParser = new MediaWikiLinkParser(this);
            this.TemplateParser = new MediaWikiTemplateParser(this);
            this.VariableParser = new MediaWikiVariableParser(this);
            this.HeadingParser = new MediaWikiHeadingParser(this);
        }

        #endregion

        #region 公開プロパティ

        /// <summary>
        /// このパーサーが対応するMediaWiki。
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        public MediaWiki Website
        {
            get
            {
                return this.website;
            }

            set
            {
                this.website = Validate.NotNull(value);
            }
        }

        #endregion

        #region 関連クラス公開プロパティ

        // ※ 各要素のパーサーについては相互参照しているものが多々あり、
        //    個別のクラスでnewされると危険なことから、ここで生成して公開する。

        /// <summary>
        /// パーサー内で使用するXMLコメント要素のパーサー。
        /// </summary>
        internal IParser CommentParser
        {
            get;
            private set;
        }

        /// <summary>
        /// パーサー内で使用するnowikiブロックのパーサー。
        /// </summary>
        internal IParser NowikiParser
        {
            get;
            private set;
        }

        /// <summary>
        /// パーサー内で使用するMediaWiki内部リンクのパーサー。
        /// </summary>
        internal IParser LinkParser
        {
            get;
            private set;
        }

        /// <summary>
        /// パーサー内で使用するMediaWikiテンプレートのパーサー。
        /// </summary>
        internal IParser TemplateParser
        {
            get;
            private set;
        }

        /// <summary>
        /// パーサー内で使用するMediaWiki変数のパーサー。
        /// </summary>
        internal IParser VariableParser
        {
            get;
            private set;
        }

        /// <summary>
        /// パーサー内で使用するMediaWiki変数のパーサー。
        /// </summary>
        internal IParser HeadingParser
        {
            get;
            private set;
        }

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// 渡されたMediaWikiページの解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析結果。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public override bool TryParse(string s, out IElement result)
        {
            // 文字列を1文字ずつチェックし、その内容に応じた要素のリストを作成する
            ListElement list = new ListElement();
            StringBuilder b = new StringBuilder();
            bool newLine = false;
            for (int i = 0; i < s.Length; i++)
            {
                IElement innerElement;

                if (s[i] == '\n')
                {
                    // 改行の場合、次回に見出しの解析が必要なため記録
                    b.Append(s[i]);
                    newLine = true;
                    continue;
                }
                else if (newLine)
                {
                    // 見出しの解析
                    newLine = false;
                    if (this.TryParseElement(s, i, out innerElement, this.HeadingParser))
                    {
                        // それまでに解析済みのテキストを吐き出し、
                        // その後に解析した要素を追加
                        this.FlashText(ref list, ref b);
                        list.Add(innerElement);
                        i += innerElement.ToString().Length - 1;
                        continue;
                    }
                }

                // コメントの解析
                if (this.TryParseElement(s, i, out innerElement, this.CommentParser))
                {
                    // それまでに解析済みのテキストを吐き出し、
                    // その後に解析した要素を追加
                    this.FlashText(ref list, ref b);
                    list.Add(innerElement);
                    i += innerElement.ToString().Length - 1;

                    // コメント中に改行が含まれた場合も、見出しの処理を有効化する
                    if (innerElement.ToString().Contains("\n"))
                    {
                        newLine = true;
                    }

                    continue;
                }

                // それ以外のnowiki, 変数, 内部リンク, テンプレートの各要素のTryParse処理を呼び出し
                if (this.TryParseElement(
                    s,
                    i,
                    out innerElement,
                    this.NowikiParser,
                    this.VariableParser,
                    this.LinkParser,
                    this.TemplateParser))
                {
                    // それまでに解析済みのテキストを吐き出し、
                    // その後に解析した要素を追加
                    this.FlashText(ref list, ref b);
                    list.Add(innerElement);
                    i += innerElement.ToString().Length - 1;
                    continue;
                }

                // 通常の文字列はテキスト要素として積み上げる
                b.Append(s[i]);
            }

            // 残っていれば最後に解析済みのテキストを吐き出し
            this.FlashText(ref list, ref b);

            result = list;
            if (list.Count == 1)
            {
                // リストが1件であれば、その要素を直に返す
                result = list[0];
            }
            else if (list.Count == 0)
            {
                // 何もなければ、空文字列だったものとして空のテキスト要素を返す
                result = new TextElement();
            }

            return true;
        }

        #endregion
    }
}
