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
        public MediaWikiPage(MediaWiki website, string title, string text, DateTime timestamp)
            : base(website, title, text, timestamp)
        {
            // 本文の指定がある場合は、リダイレクトのチェックを行い属性値を更新する
            if (String.IsNullOrEmpty(text))
            {
                this.IsRedirect();
            }
        }

        /// <summary>
        /// コンストラクタ。
        /// ページのタイムスタンプには現在日時 (UTC) を設定。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <param name="text">ページの本文。</param>
        public MediaWikiPage(MediaWiki website, string title, string text)
            : base(website, title, text)
        {
            // 本文の指定がある場合は、リダイレクトのチェックを行い属性値を更新する
            if (!String.IsNullOrEmpty(text))
            {
                this.IsRedirect();
            }
        }
        
        /// <summary>
        /// コンストラクタ。
        /// ページの本文には<c>null</c>を、タイムスタンプには現在日時 (UTC) を設定。
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
        /// リダイレクト先のページ名。
        /// </summary>
        public string Redirect
        {
            get
            {
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
        /// <param name="o_Text"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <returns></returns>
        public static int ChkNowiki(ref string o_Text, string i_Text, int i_Index)
        {
            // 出力値初期化
            int lastIndex = -1;
            o_Text = String.Empty;

            // 入力値確認
            if (Honememo.Cmn.ChkTextInnerWith(i_Text.ToLower(), i_Index, NowikiStart.ToLower()) == false)
            {
                return lastIndex;
            }

            // ブロック終了まで取得
            for (int i = i_Index + NowikiStart.Length; i < i_Text.Length; i++)
            {
                // 終了条件のチェック
                if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, NowikiEnd))
                {
                    lastIndex = i + NowikiEnd.Length - 1;
                    break;
                }

                // コメント（<!--）のチェック
                string dummy = String.Empty;
                int index = ChkComment(ref dummy, i_Text, i);
                if (index != -1)
                {
                    i = index;
                    continue;
                }
            }

            // 終わりが見つからない場合は、全てnowikiブロックと判断
            if (lastIndex == -1)
            {
                lastIndex = i_Text.Length - 1;
            }

            o_Text = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
            return lastIndex;
        }

        /// <summary>
        /// コメント区間のチェック。
        /// </summary>
        /// <param name="o_Text"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <returns></returns>
        public static int ChkComment(ref string o_Text, string i_Text, int i_Index)
        {
            // 出力値初期化
            int lastIndex = -1;
            o_Text = String.Empty;

            // 入力値確認
            if (Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, CommentStart) == false)
            {
                return lastIndex;
            }

            // コメント終了まで取得
            for (int i = i_Index + CommentStart.Length; i < i_Text.Length; i++)
            {
                if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, CommentEnd))
                {
                    lastIndex = i + CommentEnd.Length - 1;
                    break;
                }
            }

            // 終わりが見つからない場合は、全てコメントと判断
            if (lastIndex == -1)
            {
                lastIndex = i_Text.Length - 1;
            }

            o_Text = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
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
            // 初期化と値チェック
            string interWiki = String.Empty;
            if (String.IsNullOrEmpty(Text))
            {
                // ページ本文が設定されていない場合実行不可
                throw new InvalidOperationException();
            }

            // 記事に存在する指定言語への言語間リンクを取得
            for (int i = 0; i < Text.Length; i++)
            {
                // コメント（<!--）のチェック
                string comment = String.Empty;
                int index = this.ChkComment(ref comment, i);
                if (index != -1)
                {
                    i = index;
                }
                else if (Honememo.Cmn.ChkTextInnerWith(Text, i, "[[" + code + ":") == true)
                {
                    // 指定言語への言語間リンクの場合、内容を取得し、処理終了
                    Link link = this.ParseInnerLink(Text.Substring(i));
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
            // 値チェック
            if (String.IsNullOrEmpty(Text))
            {
                // ページ本文が設定されていない場合実行不可
                throw new InvalidOperationException();
            }

            // 指定されたページがリダイレクトページ（#REDIRECT等）かをチェック
            // ※日本語版みたいに、#REDIRECTと言語固有の#転送みたいなのがあると思われるので、
            //   翻訳元言語と英語版の設定でチェック
            for (int i = 0; i < 2; i++)
            {
                string redirect = (Website as MediaWiki).Redirect.Clone() as string;
                if (i == 1)
                {
                    if (Website.Lang.Code == "en")
                    {
                        continue;
                    }

                    MediaWiki en = new MediaWiki(new Language("en"));
                    redirect = en.Redirect;
                }

                if (!String.IsNullOrEmpty(redirect))
                {
                    if (Text.ToLower().StartsWith(redirect.ToLower()))
                    {
                        Link link = this.ParseInnerLink(Text.Substring(redirect.Length).TrimStart());
                        if (!String.IsNullOrEmpty(link.Text))
                        {
                            this.Redirect = link.Article;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// ページがカテゴリーかをチェック。
        /// </summary>
        /// <returns><c>true</c> カテゴリー。</returns>
        public bool IsCategory()
        {
            return this.IsCategory(Title);
        }

        /// <summary>
        /// ページが画像かをチェック。
        /// </summary>
        /// <returns><c>true</c> 画像。</returns>
        public bool IsImage()
        {
            return this.IsImage(Title);
        }

        /// <summary>
        /// ページが標準名前空間以外かをチェック。
        /// </summary>
        /// <returns><c>true</c> 標準名前空間以外。</returns>
        public bool IsNotMainNamespace()
        {
            return this.IsNotMainNamespace(Title);
        }

        /// <summary>
        /// 渡されたページ名がカテゴリーかをチェック。
        /// </summary>
        /// <param name="title">ページ名。</param>
        /// <returns><c>true</c> カテゴリー。</returns>
        public bool IsCategory(string title)
        {
            // 指定された記事名がカテゴリー（Category:等で始まる）かをチェック
            string category = (this.Website as MediaWiki).Namespaces[(this.Website as MediaWiki).CategoryNamespace];
            if (category != String.Empty)
            {
                if (title.ToLower().StartsWith(category.ToLower() + ":"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 渡されたページ名が画像かをチェック。
        /// </summary>
        /// <param name="title">ページ名。</param>
        /// <returns><c>true</c> 画像。</returns>
        public bool IsImage(string title)
        {
            // 指定されたページ名が画像（Image:等で始まる）かをチェック
            // ※日本語版みたいに、image: と言語固有の 画像: みたいなのがあると思われるので、
            //   翻訳元言語と英語版の設定でチェック
            for (int i = 0; i < 2; i++)
            {
                string image = (this.Website as MediaWiki).Namespaces[(this.Website as MediaWiki).FileNamespace];
                if (i == 1)
                {
                    if (this.Website.Lang.Code == "en")
                    {
                        continue;
                    }

                    MediaWiki en = new MediaWiki(new Language("en"));
                    image = en.Namespaces[(this.Website as MediaWiki).FileNamespace];
                }

                if (image != String.Empty)
                {
                    if (title.ToLower().StartsWith(image.ToLower() + ":") == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 渡されたページ名が標準名前空間以外かをチェック。
        /// </summary>
        /// <param name="title">ページ名。</param>
        /// <returns><c>true</c> 標準名前空間以外。</returns>
        public bool IsNotMainNamespace(string title)
        {
            // 指定されたページ名が標準名前空間以外の名前空間（Wikipedia:等で始まる）かをチェック
            foreach (string ns in (Website as MediaWiki).Namespaces.Values)
            {
                if (title.ToLower().StartsWith(ns.ToLower() + ":") == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 渡されたWikipediaの内部リンクを解析。
        /// </summary>
        /// <param name="i_Text">[[で始まる文字列</param>
        /// <returns>リンク。</returns>
        public Link ParseInnerLink(string i_Text)
        {
            // 出力値初期化
            Link result = new Link();
            result.Initialize();

            // 入力値確認
            if (i_Text.StartsWith("[[") == false)
            {
                return result;
            }

            // 構文を解析して、[[]]内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            string section = String.Empty;
            string[] pipeTexts = new string[0];
            int lastIndex = -1;
            int pipeCounter = 0;
            bool sharpFlag = false;
            for (int i = 2; i < i_Text.Length; i++)
            {
                char c = i_Text[i];

                // ]]が見つかったら、処理正常終了
                if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, "]]") == true)
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は表示名などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    Honememo.Cmn.AddArray(ref pipeTexts, String.Empty);
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                string dummy = String.Empty;
                string variable = String.Empty;
                int index = this.ChkVariable(ref variable, ref dummy, i_Text, i);
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
                    if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, CommentStart))
                    {
                        break;
                    }

                    // nowikiのチェック
                    string nowiki = String.Empty;
                    index = ChkNowiki(ref nowiki, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += nowiki;
                        continue;
                    }

                    // リンク [[ {{ （[[image:xx|[[test]]の画像]]とか）の再帰チェック
                    Link link = new Link();
                    index = this.ChkLinkText(ref link, i_Text, i);
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
                result.Text = i_Text.Substring(0, lastIndex + 1);

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
                if (result.Article.Contains(":") && !this.IsNotMainNamespace(result.Article))
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
        /// <param name="i_Text">{{で始まる文字列</param>
        /// <returns>テンプレートのリンク。</returns>
        public Link ParseTemplate(string i_Text)
        {
            // 出力値初期化
            Link result = new Link();
            result.Initialize();
            result.TemplateFlag = true;

            // 入力値確認
            if (i_Text.StartsWith("{{") == false)
            {
                return result;
            }

            // 構文を解析して、{{}}内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            string[] pipeTexts = new string[0];
            int lastIndex = -1;
            int pipeCounter = 0;
            for (int i = 2; i < i_Text.Length; i++)
            {
                char c = i_Text[i];

                // }}が見つかったら、処理正常終了
                if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, "}}") == true)
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は引数などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    Honememo.Cmn.AddArray(ref pipeTexts, String.Empty);
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                string dummy = String.Empty;
                string variable = String.Empty;
                int index = this.ChkVariable(ref variable, ref dummy, i_Text, i);
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
                    if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, CommentStart))
                    {
                        break;
                    }

                    // nowikiのチェック
                    string nowiki = String.Empty;
                    index = ChkNowiki(ref nowiki, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1] += nowiki;
                        continue;
                    }

                    // リンク [[ {{ （{{test|[[例]]}}とか）の再帰チェック
                    Link link = new Link();
                    index = this.ChkLinkText(ref link, i_Text, i);
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
                result.Text = i_Text.Substring(0, lastIndex + 1);

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
        /// <param name="o_Link"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <returns>正常時の戻り値には、]]の後ろの]の位置のインデックスを返す。異常時は-1</returns>
        public int ChkLinkText(ref Link o_Link, string i_Text, int i_Index)
        {
            // 出力値初期化
            int lastIndex = -1;
            o_Link.Initialize();

            // 入力値に応じて、処理を振り分け
            if (Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, "[[") == true)
            {
                // 内部リンク
                o_Link = this.ParseInnerLink(i_Text.Substring(i_Index));
            }
            else if (Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, "{{") == true)
            {
                // テンプレート
                o_Link = this.ParseTemplate(i_Text.Substring(i_Index));
            }

            // 処理結果確認
            if (!String.IsNullOrEmpty(o_Link.Text))
            {
                lastIndex = i_Index + o_Link.Text.Length - 1;
            }

            return lastIndex;
        }

        /// <summary>
        /// 渡されたテキストの指定された位置に存在する変数を解析。
        /// </summary>
        /// <param name="o_Variable"></param>
        /// <param name="o_Value"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <returns></returns>
        public int ChkVariable(ref string o_Variable, ref string o_Value, string i_Text, int i_Index)
        {
            // 出力値初期化
            int lastIndex = -1;
            o_Variable = String.Empty;
            o_Value = String.Empty;

            // 入力値確認
            if (Honememo.Cmn.ChkTextInnerWith(i_Text.ToLower(), i_Index, "{{{") == false)
            {
                return lastIndex;
            }

            // ブロック終了まで取得
            bool pipeFlag = false;
            for (int i = i_Index + 3; i < i_Text.Length; i++)
            {
                // 終了条件のチェック
                if (Honememo.Cmn.ChkTextInnerWith(i_Text, i, "}}}") == true)
                {
                    lastIndex = i + 2;
                    break;
                }

                // コメント（<!--）のチェック
                string dummy = String.Empty;
                int index = ChkComment(ref dummy, i_Text, i);
                if (index != -1)
                {
                    i = index;
                    continue;
                }

                // | が含まれている場合、以降の文字列は代入された値として扱う
                if (i_Text[i] == '|')
                {
                    pipeFlag = true;
                }
                else if (!pipeFlag)
                {
                    // | の前のとき
                    // ※Wikipediaの仕様上は、{{{1{|表示}}} のように変数名の欄に { を
                    //   含めることができるようだが、判別しきれないので、エラーとする
                    //   （どうせ意図してそんなことする人は居ないだろうし・・・）
                    if (i_Text[i] == '{')
                    {
                        break;
                    }
                }
                else
                {
                    // | の後のとき
                    // nowikiのチェック
                    string nowiki = String.Empty;
                    index = ChkNowiki(ref nowiki, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        o_Value += nowiki;
                        continue;
                    }

                    // 変数（{{{1|{{{2}}}}}}とか）の再帰チェック
                    string variable = String.Empty;
                    index = this.ChkVariable(ref variable, ref dummy, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        o_Value += variable;
                        continue;
                    }

                    // リンク [[ {{ （{{{1|[[test]]}}}とか）の再帰チェック
                    Link link = new Link();
                    index = this.ChkLinkText(ref link, i_Text, i);
                    if (index != -1)
                    {
                        i = index;
                        o_Value += link.Text;
                        continue;
                    }

                    o_Value += i_Text[i];
                }
            }

            // 変数ブロックの文字列を出力値に設定
            if (lastIndex != -1)
            {
                o_Variable = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
            }
            else
            {
                // 正常な構文ではなかった場合、出力値をクリア
                o_Variable = String.Empty;
                o_Value = String.Empty;
            }

            return lastIndex;
        }

        /// <summary>
        /// 渡された内部リンク・テンプレートを解析。
        /// </summary>
        /// <param name="link">リンク。</param>
        /// <param name="index">本文の解析開始位置のインデックス。</param>
        /// <returns></returns>
        protected int ChkLinkText(ref Link link, int index)
        {
            return this.ChkLinkText(ref link, Text, index);
        }

        /// <summary>
        /// コメント区間のチェック。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index">本文の解析開始位置のインデックス。</param>
        /// <returns></returns>
        protected int ChkComment(ref string text, int index)
        {
            return MediaWikiPage.ChkComment(ref text, Text, index);
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
            public string[] PipeTexts;

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
                this.PipeTexts = new string[0];
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
