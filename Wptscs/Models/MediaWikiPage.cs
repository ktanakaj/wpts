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
    using System.Linq;
    using System.Text;
    using Honememo.Utilities;

    /// <summary>
    /// MediaWikiのページをあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiPage : Page
    {
        #region 定数宣言

        /// <summary>
        /// nowikiタグ。
        /// </summary>
        public static readonly string NowikiTag = "nowiki";

        /// <summary>
        /// msgnwの書式。
        /// </summary>
        public static readonly string Msgnw = "msgnw:";

        #endregion

        #region private変数

        /// <summary>
        /// リダイレクト先のページ名。
        /// </summary>
        private Link redirect;

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
                    this.TryParseRedirect();
                }
            }
        }

        /// <summary>
        /// リダイレクト先へのリンク。
        /// </summary>
        public Link Redirect
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

        #region 公開静的メソッド
        
        /// <summary>
        /// 渡されたテキストがnowikiブロックかを解析する。
        /// </summary>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="nowiki">解析したnowikiブロック。</param>
        /// <returns>nowikiブロックの場合<c>true</c>。</returns>
        /// <remarks>
        /// nowikiブロックと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// また、入れ子は考慮しない。
        /// </remarks>
        public static bool TryParseNowiki(string text, out string nowiki)
        {
            nowiki = null;
            LazyXmlParser parser = new LazyXmlParser();
            LazyXmlParser.SimpleElement element;
            if (parser.TryParse(text, out element))
            {
                if (element.Name.ToLower() == MediaWikiPage.NowikiTag)
                {
                    nowiki = element.OuterXml;
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 公開インスタンスメソッド

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
            string start = "[[" + code + ":";
            for (int i = 0; i < this.Text.Length; i++)
            {
                char c = this.Text[i];
                if (c != '<' && c != '[')
                {
                    // チェックしても無駄なため探索しない
                    // ※ 性能改善のため。そもそもアルゴリズムが良くないのだけど・・・
                    continue;
                }

                string subtext = this.Text.Substring(i);
                string value;
                if (LazyXmlParser.TryParseComment(subtext, out value))
                {
                    // コメント（<!--）
                    i += value.Length - 1;
                }
                else if (MediaWikiPage.TryParseNowiki(subtext, out value))
                {
                    // nowiki区間
                    i += value.Length - 1;
                }
                else if (subtext.StartsWith(start))
                {
                    // 指定言語への言語間リンクの場合、内容を取得し、処理終了
                    Link link;
                    if (this.TryParseLink(subtext, out link))
                    {
                        interWiki = link.Title;
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

        #region Linkクラスに移動したいメソッド

        // TODO: 以下の各メソッドのうち、リンクに関するものはLinkクラスに移したい。
        //       また、余計な依存関係を持っているものを整理したい。

        /// <summary>
        /// 渡されたWikipediaの内部リンクを解析。
        /// </summary>
        /// <param name="text">[[で始まる文字列。</param>
        /// <param name="link">解析したリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public bool TryParseLink(string text, out Link link)
        {
            // 出力値初期化
            link = null;

            // 入力値確認
            if (!text.StartsWith("[["))
            {
                return false;
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
                string dummy;
                string variable;
                int index = this.ChkVariable(out variable, out dummy, text, i);
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
                    string subtext = text.Substring(i);
                    string value;
                    if (LazyXmlParser.TryParseComment(subtext, out value))
                    {
                        // コメント（<!--）が含まれている場合、リンクは無効
                        break;
                    }
                    else if (MediaWikiPage.TryParseNowiki(subtext, out value))
                    {
                        // nowikiブロック
                        i += value.Length - 1;
                        pipeTexts[pipeCounter - 1] += value;
                        continue;
                    }

                    // リンク [[ {{ （[[image:xx|[[test]]の画像]]とか）の再帰チェック
                    Link l;
                    index = this.ChkLinkText(out l, text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += l.OriginalText;
                        continue;
                    }

                    pipeTexts[pipeCounter - 1] += c;
                }
            }

            // 解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            link = new Link();

            // 変数ブロックの文字列をリンクのテキストに設定
            link.OriginalText = text.Substring(0, lastIndex + 1);

            // 前後のスペースは削除（見出しは後ろのみ）
            link.Title = article.Trim();
            link.Section = section.TrimEnd();

            // | 以降はそのまま設定
            link.PipeTexts = pipeTexts;

            // 記事名から情報を抽出
            // サブページ
            if (link.Title.StartsWith("/"))
            {
                link.SubPage = true;
            }
            else if (link.Title.StartsWith(":"))
            {
                // 先頭が :
                link.StartColon = true;
                link.Title = link.Title.TrimStart(':').TrimStart();
            }

            // 標準名前空間以外で[[xxx:yyy]]のようになっている場合、言語コード
            if (link.Title.Contains(":") && new MediaWikiPage(this.Website, link.Title).IsMain())
            {
                // ※本当は、言語コード等の一覧を作り、其処と一致するものを・・・とすべきだろうが、
                //   メンテしきれないので : を含む名前空間以外を全て言語コード等と判定
                link.Code = link.Title.Substring(0, link.Title.IndexOf(':')).TrimEnd();
                link.Title = link.Title.Substring(link.Title.IndexOf(':') + 1).TrimStart();
            }

            return true;
        }

        /// <summary>
        /// 渡されたWikipediaのテンプレートを解析。
        /// </summary>
        /// <param name="text">{{で始まる文字列。</param>
        /// <param name="link">解析したテンプレートのリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public bool TryParseTemplate(string text, out Link link)
        {
            // 出力値初期化
            link = null;

            // 入力値確認
            if (!text.StartsWith("{{"))
            {
                return false;
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
                string dummy;
                string variable;
                int index = this.ChkVariable(out variable, out dummy, text, i);
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
                    string subtext = text.Substring(i);
                    string value;
                    if (LazyXmlParser.TryParseComment(subtext, out value))
                    {
                        // コメント（<!--）が含まれている場合、リンクは無効
                        break;
                    }
                    else if (MediaWikiPage.TryParseNowiki(subtext, out value))
                    {
                        // nowikiブロック
                        i += value.Length - 1;
                        pipeTexts[pipeCounter - 1] += value;
                        continue;
                    }

                    // リンク [[ {{ （{{test|[[例]]}}とか）の再帰チェック
                    Link l;
                    index = this.ChkLinkText(out l, text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += l.OriginalText;
                        continue;
                    }

                    pipeTexts[pipeCounter - 1] += c;
                }
            }

            // 解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            link = new Link();
            link.Template = true;

            // 変数ブロックの文字列をリンクのテキストに設定
            link.OriginalText = text.Substring(0, lastIndex + 1);

            // 前後のスペース・改行は削除（見出しは後ろのみ）
            link.Title = article.Trim();

            // | 以降はそのまま設定
            link.PipeTexts = pipeTexts;

            // 記事名から情報を抽出
            // サブページ
            if (link.Title.StartsWith("/") == true)
            {
                link.SubPage = true;
            }
            else if (link.Title.StartsWith(":"))
            {
                // 先頭が :
                link.StartColon = true;
                link.Title = link.Title.TrimStart(':').TrimStart();
            }

            // 先頭が msgnw:
            link.Msgnw = link.Title.ToLower().StartsWith(Msgnw.ToLower());
            if (link.Msgnw)
            {
                link.Title = link.Title.Substring(Msgnw.Length);
            }

            // 記事名直後の改行の有無
            if (article.TrimEnd(' ').EndsWith("\n"))
            {
                link.Enter = true;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// 渡されたテキストの指定された位置に存在するWikipediaの内部リンク・テンプレートをチェック。
        /// </summary>
        /// <param name="link">解析したリンク。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>正常時の戻り値には、]]の後ろの]の位置のインデックスを返す。異常時は-1。</returns>
        public int ChkLinkText(out Link link, string text, int index)
        {
            // 入力値に応じて、処理を振り分け
            if (StringUtils.StartsWith(text, "[[", index))
            {
                // 内部リンク
                if (this.TryParseLink(text.Substring(index), out link))
                {
                    return index + link.OriginalText.Length - 1;
                }
            }
            else if (StringUtils.StartsWith(text, "{{", index))
            {
                // テンプレート
                if (this.TryParseTemplate(text.Substring(index), out link))
                {
                    return index + link.OriginalText.Length - 1;
                }
            }

            // 出力値初期化。リンク以外の場合、空のオブジェクトを返す
            // （昔構造体を返していた名残。）
            link = new Link();
            return -1;
        }

        /// <summary>
        /// 渡されたテキストの指定された位置に存在する変数を解析。
        /// </summary>
        /// <param name="variable">解析した変数。</param>
        /// <param name="value">変数のパラメータ値。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>正常時の戻り値には、変数の終了位置のインデックスを返す。異常時は-1。</returns>
        public int ChkVariable(out string variable, out string value, string text, int index)
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

                string subtext = text.Substring(i);
                string comment;
                if (LazyXmlParser.TryParseComment(subtext, out comment))
                {
                    // コメント（<!--）ブロック
                    i += comment.Length - 1;
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
                    string nowiki;
                    if (MediaWikiPage.TryParseNowiki(subtext, out nowiki))
                    {
                        // nowikiブロック
                        i += nowiki.Length - 1;
                        value += nowiki;
                        continue;
                    }

                    // 変数（{{{1|{{{2}}}}}}とか）の再帰チェック
                    string var;
                    string dummy;
                    int subindex = this.ChkVariable(out var, out dummy, text, i);
                    if (subindex != -1)
                    {
                        i = subindex;
                        value += var;
                        continue;
                    }

                    // リンク [[ {{ （{{{1|[[test]]}}}とか）の再帰チェック
                    Link link;
                    subindex = this.ChkLinkText(out link, text, i);
                    if (subindex != -1)
                    {
                        i = subindex;
                        value += link.OriginalText;
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

        #endregion

        #region 内部処理用インスタンスメソッド

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
        protected int ChkLinkText(out Link link, int index)
        {
            return this.ChkLinkText(out link, this.Text, index);
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
        /// 現在のページをリダイレクトとして解析する。
        /// </summary>
        /// <returns>リダイレクトの場合<c>true</c>。</returns>
        /// <remarks>リダイレクトの場合、転送先ページ名をプロパティに格納。</remarks>
        private bool TryParseRedirect()
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
                    Link link;
                    if (this.TryParseLink(this.Text.Substring(format.Length).TrimStart(), out link))
                    {
                        this.Redirect = link;
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region 内部クラス

        /// <summary>
        /// Wikipediaのリンクの要素を格納するための構造体。
        /// </summary>
        public class Link
        {
            #region プロパティ

            /// <summary>
            /// リンクのオブジェクト作成時の元テキスト（[[～]]）。
            /// </summary>
            public string OriginalText
            {
                get;
                //// TODO: このクラスにParseを移動完了したら、protectedにする
                set;
            }

            /// <summary>
            /// リンクの記事名。
            /// </summary>
            public string Title
            {
                get;
                set;
            }

            /// <summary>
            /// リンクのセクション名（#）。
            /// </summary>
            public string Section
            {
                get;
                set;
            }

            /// <summary>
            /// リンクのパイプ後の文字列（|）。
            /// </summary>
            public IList<string> PipeTexts
            {
                get;
                set;
            }

            /// <summary>
            /// 言語間または他プロジェクトへのリンクの場合、コード。
            /// </summary>
            public string Code
            {
                get;
                set;
            }

            /// <summary>
            /// テンプレート呼び出し（{{～}}）かを示すフラグ。
            /// </summary>
            public bool Template
            {
                get;
                set;
            }

            /// <summary>
            /// 記事名の先頭が / で始まるかを示すフラグ。
            /// </summary>
            public bool SubPage
            {
                get;
                set;
            }

            /// <summary>
            /// リンクの先頭が : で始まるかを示すフラグ。
            /// </summary>
            public bool StartColon
            {
                get;
                set;
            }

            /// <summary>
            /// テンプレートの場合、msgnw: が付加されているかを示すフラグ。
            /// </summary>
            public bool Msgnw
            {
                get;
                set;
            }

            /// <summary>
            /// テンプレートの場合、記事名の後で改行されるかを示すフラグ。
            /// </summary>
            public bool Enter
            {
                get;
                set;
            }

            /// <summary>
            /// リンクが表すテキスト（[[～]]）。
            /// </summary>
            public string Text
            {
                get
                {
                    // 戻り値初期化
                    StringBuilder b = new StringBuilder();

                    // 枠の設定
                    string startSign = "[[";
                    string endSign = "]]";
                    if (this.Template)
                    {
                        startSign = "{{";
                        endSign = "}}";
                    }

                    // 先頭の枠の付加
                    b.Append(startSign);

                    // 先頭の : の付加
                    if (this.StartColon)
                    {
                        b.Append(':');
                    }

                    // msgnw: （テンプレートを<nowiki>タグで挟む）の付加
                    if (this.Template && this.Msgnw)
                    {
                        b.Append(MediaWikiPage.Msgnw);
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
                    if (this.Enter)
                    {
                        b.Append('\n');
                    }

                    // パイプ後の文字列の付加
                    if (this.PipeTexts != null)
                    {
                        foreach (string s in this.PipeTexts)
                        {
                            b.Append('|');
                            if (!String.IsNullOrEmpty(s))
                            {
                                b.Append(s);
                            }
                        }
                    }

                    // 終わりの枠の付加
                    b.Append(endSign);
                    return b.ToString();
                }
            }

            #endregion

            #region 公開メソッド

            /// <summary>
            /// このオブジェクトを表すリンク文字列を返す。
            /// </summary>
            /// <returns>オブジェクトを表すリンク文字列。</returns>
            public override string ToString()
            {
                // リンクを表すテキスト、ならびに元テキストを返す
                return this.Text + "<!-- " + this.OriginalText + " -->";
            }

            #endregion
        }

        #endregion
    }
}
