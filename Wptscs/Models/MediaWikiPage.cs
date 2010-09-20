// ================================================================================================
// <summary>
//      MediaWikiのページをあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiPage.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using Honememo.Utilities;

    /// <summary>
    /// MediaWikiのページをあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiPage : Page
    {
        #region 定数宣言

        /// <summary>
        /// コメントの開始。
        /// </summary>
        public static readonly string CommentStart = "<!--";

        /// <summary>
        /// コメントの終了。
        /// </summary>
        public static readonly string CommentEnd = "-->";

        /// <summary>
        /// nowikiの開始。
        /// </summary>
        public static readonly string NowikiStart = "<nowiki>";

        /// <summary>
        /// nowikiの終了。
        /// </summary>
        public static readonly string NowikiEnd = "</nowiki>";

        /// <summary>
        /// msgnwの書式。
        /// </summary>
        public static readonly string Msgnw = "msgnw:";

        #endregion

        #region private変数

        /// <summary>
        /// リダイレクト先のページ名。
        /// </summary>
        private string redirect;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <param name="text">ページの本文。</param>
        /// <param name="timestamp">ページのタイムスタンプ。</param>
        public MediaWikiPage(MediaWiki website, string title, string text, DateTime? timestamp)
            : base(website, title, text, timestamp)
        {
        }

        /// <summary>
        /// コンストラクタ。
        /// ページのタイムスタンプには<c>null</c>を設定。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <param name="text">ページの本文。</param>
        public MediaWikiPage(MediaWiki website, string title, string text)
            : base(website, title, text)
        {
        }
        
        /// <summary>
        /// コンストラクタ。
        /// ページの本文, タイムスタンプには<c>null</c>を設定。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        public MediaWikiPage(MediaWiki website, string title)
            : base(website, title)
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// ページが所属するウェブサイト。
        /// </summary>
        public new MediaWiki Website
        {
            get
            {
                return base.Website as MediaWiki;
            }

            protected set
            {
                base.Website = value;
            }
        }

        /// <summary>
        /// ページの本文。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }

            protected set
            {
                // 本文は普通に格納
                base.Text = value;

                // 本文格納のタイミングでリダイレクトページ（#REDIRECT等）かを判定
                if (!String.IsNullOrEmpty(base.Text))
                {
                    this.ParseRedirect();
                }
            }
        }

        /// <summary>
        /// リダイレクト先のページ名。
        /// </summary>
        public string Redirect
        {
            get
            {
                // Textが設定されている場合のみ有効
                this.ValidateIncomplete();
                return this.redirect;
            }

            protected set
            {
                this.redirect = value;
            }
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// nowiki区間のチェック。
        /// </summary>
        /// <param name="nowiki">解析したnowikiブロック。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>nowiki区間の場合、終了位置のインデックスを返す。それ以外は-1。</returns>
        public static int ChkNowiki(ref string nowiki, string text, int index)
        {
            // 出力値初期化
            int lastIndex = -1;
            nowiki = String.Empty;

            // 入力値確認
            if (!StringUtils.StartsWith(text.ToLower(), NowikiStart.ToLower(), index))
            {
                return lastIndex;
            }

            // ブロック終了まで取得
            for (int i = index + NowikiStart.Length; i < text.Length; i++)
            {
                // 終了条件のチェック
                if (StringUtils.StartsWith(text, NowikiEnd, i))
                {
                    lastIndex = i + NowikiEnd.Length - 1;
                    break;
                }

                // コメント（<!--）のチェック
                string dummy = String.Empty;
                int subindex = ChkComment(ref dummy, text, i);
                if (subindex != -1)
                {
                    i = subindex;
                    continue;
                }
            }

            // 終わりが見つからない場合は、全てnowikiブロックと判断
            if (lastIndex == -1)
            {
                lastIndex = text.Length - 1;
            }

            nowiki = text.Substring(index, lastIndex - index + 1);
            return lastIndex;
        }

        /// <summary>
        /// コメント区間のチェック。
        /// </summary>
        /// <param name="comment">解析したコメント。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>コメント区間の場合、終了位置のインデックスを返す。それ以外は-1。</returns>
        public static int ChkComment(ref string comment, string text, int index)
        {
            // 出力値初期化
            int lastIndex = -1;
            comment = String.Empty;

            // 入力値確認
            if (!StringUtils.StartsWith(text, CommentStart, index))
            {
                return lastIndex;
            }

            // コメント終了まで取得
            for (int i = index + CommentStart.Length; i < text.Length; i++)
            {
                if (StringUtils.StartsWith(text, CommentEnd, i))
                {
                    lastIndex = i + CommentEnd.Length - 1;
                    break;
                }
            }

            // 終わりが見つからない場合は、全てコメントと判断
            if (lastIndex == -1)
            {
                lastIndex = text.Length - 1;
            }

            comment = text.Substring(index, lastIndex - index + 1);
            return lastIndex;
        }

        #endregion

        #region インスタンスメソッド

        /// <summary>
        /// 指定された言語コードへの言語間リンクを返す。
        /// </summary>
        /// <param name="code">言語コード。</param>
        /// <returns>言語間リンク先の記事名。見つからない場合は空。</returns>
        public string GetInterWiki(string code)
        {
            // Textが設定されている場合のみ有効
            this.ValidateIncomplete();

            // 初期化と値チェック
            string interWiki = String.Empty;

            // 記事に存在する指定言語への言語間リンクを取得
            for (int i = 0; i < this.Text.Length; i++)
            {
                // コメント（<!--）のチェック
                string comment = String.Empty;
                int index = this.ChkComment(ref comment, i);
                if (index != -1)
                {
                    i = index;
                }
                else if (StringUtils.StartsWith(this.Text, "[[" + code + ":", i))
                {
                    // 指定言語への言語間リンクの場合、内容を取得し、処理終了
                    Link link = this.ParseInnerLink(this.Text.Substring(i));
                    if (!String.IsNullOrEmpty(link.Text))
                    {
                        interWiki = link.Article;
                        break;
                    }
                }
            }

            return interWiki;
        }

        /// <summary>
        /// ページがリダイレクトかをチェック。
        /// </summary>
        /// <returns><c>true</c> リダイレクト。</returns>
        public bool IsRedirect()
        {
            // Textが設定されている場合のみ有効
            return this.Redirect != null;
        }

        /// <summary>
        /// ページがカテゴリーかをチェック。
        /// </summary>
        /// <returns><c>true</c> カテゴリー。</returns>
        public bool IsCategory()
        {
            // 指定された記事名がカテゴリー（Category:等で始まる）かをチェック
            return this.IsNamespacePage(this.Website.CategoryNamespace);
        }

        /// <summary>
        /// ページが画像かをチェック。
        /// </summary>
        /// <returns><c>true</c> 画像。</returns>
        public bool IsFile()
        {
            // 指定されたページ名がファイル（Image:等で始まる）かをチェック
            return this.IsNamespacePage(this.Website.FileNamespace);
        }

        /// <summary>
        /// ページが標準名前空間かをチェック。
        /// </summary>
        /// <returns><c>true</c> 標準名前空間。</returns>
        public bool IsMain()
        {
            // 指定されたページ名が標準名前空間以外の名前空間（Wikipedia:等で始まる）かをチェック
            string title = this.Title.ToLower();
            foreach (IList<string> prefixes in this.Website.Namespaces.Values)
            {
                foreach (string prefix in prefixes)
                {
                    if (title.StartsWith(prefix.ToLower() + ":"))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // TODO: 以下の各メソッドのうち、リンクに関するものはLinkクラスに移したい。
        //       また、余計な依存関係を持っているものを整理したい。

        /// <summary>
        /// 渡されたWikipediaの内部リンクを解析。
        /// </summary>
        /// <param name="text">[[で始まる文字列。</param>
        /// <returns>リンク。</returns>
        public Link ParseInnerLink(string text)
        {
            // 出力値初期化
            Link result = new Link();
            result.Initialize();

            // 入力値確認
            if (!text.StartsWith("[["))
            {
                return result;
            }

            // 構文を解析して、[[]]内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            string section = String.Empty;
            IList<string> pipeTexts = new List<string>();
            int lastIndex = -1;
            int pipeCounter = 0;
            bool sharpFlag = false;
            for (int i = 2; i < text.Length; i++)
            {
                char c = text[i];

                // ]]が見つかったら、処理正常終了
                if (StringUtils.StartsWith(text, "]]", i))
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は表示名などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    pipeTexts.Add(String.Empty);
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                string dummy = String.Empty;
                string variable = String.Empty;
                int index = this.ChkVariable(ref variable, ref dummy, text, i);
                if (index != -1)
                {
                    i = index;
                    if (pipeCounter > 0)
                    {
                        pipeTexts[pipeCounter - 1] += variable;
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
                    // コメント（<!--）が含まれている場合、リンクは無効
                    if (StringUtils.StartsWith(text, CommentStart, i))
                    {
                        break;
                    }

                    // nowikiのチェック
                    string nowiki = String.Empty;
                    index = ChkNowiki(ref nowiki, text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += nowiki;
                        continue;
                    }

                    // リンク [[ {{ （[[image:xx|[[test]]の画像]]とか）の再帰チェック
                    Link link = new Link();
                    index = this.ChkLinkText(ref link, text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += link.Text;
                        continue;
                    }

                    pipeTexts[pipeCounter - 1] += c;
                }
            }

            // 解析に成功した場合、結果を戻り値に設定
            if (lastIndex != -1)
            {
                // 変数ブロックの文字列をリンクのテキストに設定
                result.Text = text.Substring(0, lastIndex + 1);

                // 前後のスペースは削除（見出しは後ろのみ）
                result.Article = article.Trim();
                result.Section = section.TrimEnd();

                // | 以降はそのまま設定
                result.PipeTexts = pipeTexts;

                // 記事名から情報を抽出
                // サブページ
                if (result.Article.StartsWith("/"))
                {
                    result.SubPageFlag = true;
                }
                else if (result.Article.StartsWith(":"))
                {
                    // 先頭が :
                    result.StartColonFlag = true;
                    result.Article = result.Article.TrimStart(':').TrimStart();
                }

                // 標準名前空間以外で[[xxx:yyy]]のようになっている場合、言語コード
                if (result.Article.Contains(":") && new MediaWikiPage(this.Website, result.Article).IsMain())
                {
                    // ※本当は、言語コード等の一覧を作り、其処と一致するものを・・・とすべきだろうが、
                    //   メンテしきれないので : を含む名前空間以外を全て言語コード等と判定
                    result.Code = result.Article.Substring(0, result.Article.IndexOf(':')).TrimEnd();
                    result.Article = result.Article.Substring(result.Article.IndexOf(':') + 1).TrimStart();
                }
            }

            return result;
        }

        /// <summary>
        /// 渡されたWikipediaのテンプレートを解析。
        /// </summary>
        /// <param name="text">{{で始まる文字列。</param>
        /// <returns>テンプレートのリンク。</returns>
        public Link ParseTemplate(string text)
        {
            // 出力値初期化
            Link result = new Link();
            result.Initialize();
            result.TemplateFlag = true;

            // 入力値確認
            if (!text.StartsWith("{{"))
            {
                return result;
            }

            // 構文を解析して、{{}}内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            IList<string> pipeTexts = new List<string>();
            int lastIndex = -1;
            int pipeCounter = 0;
            for (int i = 2; i < text.Length; i++)
            {
                char c = text[i];

                // }}が見つかったら、処理正常終了
                if (StringUtils.StartsWith(text, "}}", i))
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は引数などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    pipeTexts.Add(String.Empty);
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                string dummy = String.Empty;
                string variable = String.Empty;
                int index = this.ChkVariable(ref variable, ref dummy, text, i);
                if (index != -1)
                {
                    i = index;
                    if (pipeCounter > 0)
                    {
                        pipeTexts[pipeCounter - 1] += variable;
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
                    // コメント（<!--）が含まれている場合、リンクは無効
                    if (StringUtils.StartsWith(text, CommentStart, i))
                    {
                        break;
                    }

                    // nowikiのチェック
                    string nowiki = String.Empty;
                    index = ChkNowiki(ref nowiki, text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += nowiki;
                        continue;
                    }

                    // リンク [[ {{ （{{test|[[例]]}}とか）の再帰チェック
                    Link link = new Link();
                    index = this.ChkLinkText(ref link, text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += link.Text;
                        continue;
                    }

                    pipeTexts[pipeCounter - 1] += c;
                }
            }

            // 解析に成功した場合、結果を戻り値に設定
            if (lastIndex != -1)
            {
                // 変数ブロックの文字列をリンクのテキストに設定
                result.Text = text.Substring(0, lastIndex + 1);

                // 前後のスペース・改行は削除（見出しは後ろのみ）
                result.Article = article.Trim();

                // | 以降はそのまま設定
                result.PipeTexts = pipeTexts;

                // 記事名から情報を抽出
                // サブページ
                if (result.Article.StartsWith("/") == true)
                {
                    result.SubPageFlag = true;
                }
                else if (result.Article.StartsWith(":"))
                {
                    // 先頭が :
                    result.StartColonFlag = true;
                    result.Article = result.Article.TrimStart(':').TrimStart();
                }

                // 先頭が msgnw:
                result.MsgnwFlag = result.Article.ToLower().StartsWith(Msgnw.ToLower());
                if (result.MsgnwFlag)
                {
                    result.Article = result.Article.Substring(Msgnw.Length);
                }

                // 記事名直後の改行の有無
                if (article.TrimEnd(' ').EndsWith("\n"))
                {
                    result.EnterFlag = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 渡されたテキストの指定された位置に存在するWikipediaの内部リンク・テンプレートをチェック。
        /// </summary>
        /// <param name="link">解析したリンク。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>正常時の戻り値には、]]の後ろの]の位置のインデックスを返す。異常時は-1。</returns>
        public int ChkLinkText(ref Link link, string text, int index)
        {
            // 出力値初期化
            int lastIndex = -1;
            link.Initialize();

            // 入力値に応じて、処理を振り分け
            if (StringUtils.StartsWith(text, "[[", index))
            {
                // 内部リンク
                link = this.ParseInnerLink(text.Substring(index));
            }
            else if (StringUtils.StartsWith(text, "{{", index))
            {
                // テンプレート
                link = this.ParseTemplate(text.Substring(index));
            }

            // 処理結果確認
            if (!String.IsNullOrEmpty(link.Text))
            {
                lastIndex = index + link.Text.Length - 1;
            }

            return lastIndex;
        }

        /// <summary>
        /// 渡されたテキストの指定された位置に存在する変数を解析。
        /// </summary>
        /// <param name="variable">解析した変数。</param>
        /// <param name="value">変数のパラメータ値。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>正常時の戻り値には、変数の終了位置のインデックスを返す。異常時は-1。</returns>
        public int ChkVariable(ref string variable, ref string value, string text, int index)
        {
            // 出力値初期化
            int lastIndex = -1;
            variable = String.Empty;
            value = String.Empty;

            // 入力値確認
            if (!StringUtils.StartsWith(text.ToLower(), "{{{", index))
            {
                return lastIndex;
            }

            // ブロック終了まで取得
            bool pipeFlag = false;
            for (int i = index + 3; i < text.Length; i++)
            {
                // 終了条件のチェック
                if (StringUtils.StartsWith(text, "}}}", i))
                {
                    lastIndex = i + 2;
                    break;
                }

                // コメント（<!--）のチェック
                string dummy = String.Empty;
                int subindex = ChkComment(ref dummy, text, i);
                if (subindex != -1)
                {
                    i = subindex;
                    continue;
                }

                // | が含まれている場合、以降の文字列は代入された値として扱う
                if (text[i] == '|')
                {
                    pipeFlag = true;
                }
                else if (!pipeFlag)
                {
                    // | の前のとき
                    // ※Wikipediaの仕様上は、{{{1{|表示}}} のように変数名の欄に { を
                    //   含めることができるようだが、判別しきれないので、エラーとする
                    //   （どうせ意図してそんなことする人は居ないだろうし・・・）
                    if (text[i] == '{')
                    {
                        break;
                    }
                }
                else
                {
                    // | の後のとき
                    // nowikiのチェック
                    string nowiki = String.Empty;
                    subindex = ChkNowiki(ref nowiki, text, i);
                    if (subindex != -1)
                    {
                        i = subindex;
                        value += nowiki;
                        continue;
                    }

                    // 変数（{{{1|{{{2}}}}}}とか）の再帰チェック
                    string var = String.Empty;
                    subindex = this.ChkVariable(ref var, ref dummy, text, i);
                    if (subindex != -1)
                    {
                        i = subindex;
                        value += var;
                        continue;
                    }

                    // リンク [[ {{ （{{{1|[[test]]}}}とか）の再帰チェック
                    Link link = new Link();
                    subindex = this.ChkLinkText(ref link, text, i);
                    if (subindex != -1)
                    {
                        i = subindex;
                        value += link.Text;
                        continue;
                    }

                    value += text[i];
                }
            }

            // 変数ブロックの文字列を出力値に設定
            if (lastIndex != -1)
            {
                variable = text.Substring(index, lastIndex - index + 1);
            }
            else
            {
                // 正常な構文ではなかった場合、出力値をクリア
                variable = String.Empty;
                value = String.Empty;
            }

            return lastIndex;
        }

        /// <summary>
        /// ページが指定された番号の名前空間に所属するかをチェック。
        /// </summary>
        /// <param name="id">名前空間のID。</param>
        /// <returns><c>true</c> 所属する。</returns>
        protected bool IsNamespacePage(int id)
        {
            // 指定された記事名がカテゴリー（Category:等で始まる）かをチェック
            IList<string> prefixes = this.Website.Namespaces[id];
            if (prefixes != null)
            {
                string title = this.Title.ToLower();
                foreach (string prefix in prefixes)
                {
                    if (title.StartsWith(prefix.ToLower() + ":"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 渡された内部リンク・テンプレートを解析。
        /// </summary>
        /// <param name="link">リンク。</param>
        /// <param name="index">本文の解析開始位置のインデックス。</param>
        /// <returns>正常時の戻り値には、]]の後ろの]の位置のインデックスを返す。異常時は-1。</returns>
        protected int ChkLinkText(ref Link link, int index)
        {
            return this.ChkLinkText(ref link, this.Text, index);
        }

        /// <summary>
        /// コメント区間のチェック。
        /// </summary>
        /// <param name="comment">解析したコメント。</param>
        /// <param name="index">本文の解析開始位置のインデックス。</param>
        /// <returns>コメント区間の場合、終了位置のインデックスを返す。それ以外は-1。</returns>
        protected int ChkComment(ref string comment, int index)
        {
            return MediaWikiPage.ChkComment(ref comment, this.Text, index);
        }

        /// <summary>
        /// オブジェクトがメソッドの実行に不完全な状態でないか検証する。
        /// 不完全な場合、例外をスローする。
        /// </summary>
        /// <exception cref="InvalidOperationException">オブジェクトは不完全。</exception>
        protected void ValidateIncomplete()
        {
            if (String.IsNullOrEmpty(this.Text))
            {
                // ページ本文が設定されていない場合不完全と判定
                throw new InvalidOperationException("Text is unset");
            }
        }

        /// <summary>
        /// 渡されたページがリダイレクトかを解析する。
        /// </summary>
        /// <remarks>リダイレクトの場合、転送先ページ名をプロパティに格納。</remarks>
        private void ParseRedirect()
        {
            // 日本語版みたいに、#REDIRECTと言語固有の#転送みたいなのがあると思われるので、
            // 翻訳元言語とデフォルトの設定でチェック
            this.Redirect = null;
            for (int i = 0; i < 2; i++)
            {
                string format = this.Website.Redirect;
                if (i == 1)
                {
                    format = Properties.Settings.Default.MediaWikiRedirect;
                }

                if (!String.IsNullOrEmpty(format)
                    && this.Text.ToLower().StartsWith(format.ToLower()))
                {
                    Link link = this.ParseInnerLink(this.Text.Substring(format.Length).TrimStart());
                    if (!String.IsNullOrEmpty(link.Text))
                    {
                        this.Redirect = link.Article;
                        break;
                    }
                }
            }
        }

        #endregion

        #region 構造体

        /// <summary>
        /// Wikipediaのリンクの要素を格納するための構造体。
        /// </summary>
        public struct Link
        {
            /// <summary>
            /// リンクのテキスト（[[～]]）。
            /// </summary>
            public string Text;

            /// <summary>
            /// リンクの記事名。
            /// </summary>
            public string Article;

            /// <summary>
            /// リンクのセクション名（#）。
            /// </summary>
            public string Section;

            /// <summary>
            /// リンクのパイプ後の文字列（|）。
            /// </summary>
            public IList<string> PipeTexts;

            /// <summary>
            /// 言語間または他プロジェクトへのリンクの場合、コード。
            /// </summary>
            public string Code;

            /// <summary>
            /// テンプレート（{{～}}）かを示すフラグ。
            /// </summary>
            public bool TemplateFlag;

            /// <summary>
            /// 記事名の先頭が / で始まるかを示すフラグ。
            /// </summary>
            public bool SubPageFlag;

            /// <summary>
            /// リンクの先頭が : で始まるかを示すフラグ。
            /// </summary>
            public bool StartColonFlag;

            /// <summary>
            /// テンプレートの場合、msgnw: が付加されているかを示すフラグ。
            /// </summary>
            public bool MsgnwFlag;

            /// <summary>
            /// テンプレートの場合、記事名の後で改行されるかを示すフラグ。
            /// </summary>
            public bool EnterFlag;

            /// <summary>
            /// 初期化。
            /// </summary>
            public void Initialize()
            {
                // コンストラクタの代わりに、必要ならこれで初期化
                this.Text = null;
                this.Article = null;
                this.Section = null;
                this.PipeTexts = new List<string>();
                this.Code = null;
                this.TemplateFlag = false;
                this.SubPageFlag = false;
                this.StartColonFlag = false;
                this.MsgnwFlag = false;
                this.EnterFlag = false;
            }

            /// <summary>
            /// 現在のText以外の属性値から、Text属性値を生成。
            /// </summary>
            /// <returns>生成したTextの文字列。</returns>
            public string MakeText()
            {
                // 戻り値初期化
                string result = String.Empty;

                // 枠の設定
                string startSign = "[[";
                string endSign = "]]";
                if (this.TemplateFlag)
                {
                    startSign = "{{";
                    endSign = "}}";
                }

                // 先頭の枠の付加
                result += startSign;

                // 先頭の : の付加
                if (this.StartColonFlag)
                {
                    result += ":";
                }

                // msgnw: （テンプレートを<nowiki>タグで挟む）の付加
                if (this.TemplateFlag && this.MsgnwFlag)
                {
                    result += Msgnw;
                }

                // 言語コード・他プロジェクトコードの付加
                if (!String.IsNullOrEmpty(this.Code))
                {
                    result += this.Code;
                }

                // リンクの付加
                if (!String.IsNullOrEmpty(this.Article))
                {
                    result += this.Article;
                }

                // セクション名の付加
                if (!String.IsNullOrEmpty(this.Section))
                {
                    result += "#" + this.Section;
                }

                // 改行の付加
                if (this.EnterFlag)
                {
                    result += '\n';
                }

                // パイプ後の文字列の付加
                if (this.PipeTexts != null)
                {
                    foreach (string text in this.PipeTexts)
                    {
                        result += "|";
                        if (!String.IsNullOrEmpty(text))
                        {
                            result += text;
                        }
                    }
                }

                // 終わりの枠の付加
                result += endSign;
                return result;
            }
        }

        #endregion
    }
}
