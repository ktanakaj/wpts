// ================================================================================================
// <summary>
//      Wikipedia用の翻訳支援処理実装クラスソース</summary>
//
// <copyright file="MediaWikiTranslator.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Windows.Forms;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;
    using Honememo.Wptscs.Utilities;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// Wikipedia用の翻訳支援処理実装クラスです。
    /// </summary>
    public class MediaWikiTranslator : Translator
    {
        #region プロパティ

        /// <summary>
        /// 翻訳元言語のサイト。
        /// </summary>
        public new MediaWiki From
        {
            get
            {
                return base.From as MediaWiki;
            }

            set
            {
                base.From = value;
            }
        }

        /// <summary>
        /// 翻訳先言語のサイト。
        /// </summary>
        public new MediaWiki To
        {
            get
            {
                return base.To as MediaWiki;
            }

            set
            {
                base.To = value;
            }
        }
        
        #endregion

        #region 整理予定の静的メソッド

        /// <summary>
        /// コメント区間のチェック。
        /// </summary>
        /// <param name="comment">解析したコメント。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>コメント区間の場合、終了位置のインデックスを返す。それ以外は-1。</returns>
        public static int ChkComment(out string comment, string text, int index)
        {
            // 入力値確認
            if (String.IsNullOrEmpty(text))
            {
                comment = String.Empty;
                return -1;
            }

            // 改良版メソッドをコール
            if (!LazyXmlParser.TryParseComment(text.Substring(index), out comment))
            {
                comment = String.Empty;
                return -1;
            }

            return index + comment.Length - 1;
        }

        /// <summary>
        /// nowiki区間のチェック。
        /// </summary>
        /// <param name="nowiki">解析したnowikiブロック。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>nowiki区間の場合、終了位置のインデックスを返す。それ以外は-1。</returns>
        public static int ChkNowiki(out string nowiki, string text, int index)
        {
            // 入力値確認
            if (String.IsNullOrEmpty(text))
            {
                nowiki = String.Empty;
                return -1;
            }

            // 改良版メソッドをコール
            if (!MediaWikiPage.TryParseNowiki(text.Substring(index), out nowiki))
            {
                nowiki = String.Empty;
                return -1;
            }

            return index + nowiki.Length - 1;
        }

        #endregion

        #region メイン処理メソッド

        /// <summary>
        /// 翻訳支援処理実行部の本体。
        /// ※継承クラスでは、この関数に処理を実装すること
        /// </summary>
        /// <param name="name">記事名。</param>
        /// <returns><c>true</c> 処理成功。</returns>
        protected override bool RunBody(string name)
        {
            System.Diagnostics.Debug.WriteLine("\nMediaWikiTranslator.runBody > " + name);

            // 対象記事を取得
            MediaWikiPage article = this.ChkTargetArticle(name);
            if (article == null)
            {
                return false;
            }

            // 対象記事に言語間リンクが存在する場合、処理を継続するか確認
            string interWiki = article.GetInterWiki(this.To.Language.Code);
            if (interWiki != String.Empty)
            {
                if (MessageBox.Show(
                        String.Format(Resources.QuestionMessage_ArticleExist, interWiki),
                        Resources.QuestionTitle,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)
                   == System.Windows.Forms.DialogResult.No)
                {
                    this.LogLine(ENTER + String.Format(Resources.QuestionMessage_ArticleExist, interWiki));
                    return false;
                }
                else
                {
                    this.LogLine(Resources.RightArrow + " " + String.Format(Resources.LogMessage_ArticleExistInterWiki, interWiki));
                }
            }

            // 冒頭部を作成
            this.Text += "'''xxx'''";
            string bracket = this.To.Language.Bracket;
            if (bracket.Contains("{0}"))
            {
                string originalName = String.Empty;
                string langTitle = this.GetFullName(this.From, this.To.Language.Code);
                if (langTitle != String.Empty)
                {
                    originalName = "[[" + langTitle + "]]: ";
                }

                this.Text += String.Format(bracket, originalName + "'''" + name + "'''");
            }

            this.Text += "\n\n";

            // 言語間リンク・定型句の変換
            this.LogLine(ENTER + Resources.RightArrow + " " + String.Format(Resources.LogMessage_CheckAndReplaceStart, interWiki));
            this.Text += this.ReplaceText(article.Text, article.Title);

            // ユーザーからの中止要求をチェック
            if (CancellationPending)
            {
                return false;
            }

            // 新しい言語間リンクと、コメントを追記
            this.Text += "\n\n[[" + this.From.Language.Code + ":" + name + "]]\n";
            this.Text += String.Format(
                Resources.ArticleFooter,
                FormUtils.ApplicationName(),
                this.From.Language.Code,
                name,
                article.Timestamp.HasValue ? article.Timestamp.Value.ToString("U") : String.Empty) + "\n";

            // ダウンロードされるテキストがLFなので、ここで全てCRLFに変換
            // ※ダウンロード時にCRLFにするような仕組みが見つかれば、そちらを使う
            //   その場合、上のように\nをべたに吐いている部分を修正する
            this.Text = this.Text.Replace("\n", ENTER);

            System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.runBody > Success!");
            return true;
        }

        #endregion

        #region 他のクラスの処理をこのクラスにあわせて拡張したメソッド

        /// <summary>
        /// ログメッセージを出力しつつページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <param name="notFoundMsg">取得できない場合に出力するメッセージ。</param>
        /// <returns>取得したページ。ページが存在しない場合は <c>null</c> を返す。</returns>
        /// <remarks>通信エラーなど例外が発生した場合は、別途エラーログを出力する。</remarks>
        protected new MediaWikiPage GetPage(string title, string notFoundMsg)
        {
            // 親クラスのメソッドを戻り値の型だけ変更
            return base.GetPage(title, notFoundMsg) as MediaWikiPage;
        }

        #endregion

        #region 各処理のメソッド
        
        /// <summary>
        /// 翻訳支援対象のページを取得。
        /// </summary>
        /// <param name="title">ページ名。</param>
        /// <returns>取得したページ。取得失敗時は<c>null</c>。</returns>
        protected MediaWikiPage ChkTargetArticle(string title)
        {
            // 指定された記事の生データをWikipediaから取得
            this.LogLine(String.Format(Resources.LogMessage_GetArticle, this.From.Location, title));
            MediaWikiPage page = this.GetPage(title, Resources.RightArrow + " " + Resources.LogMessage_ArticleNothing);

            // リダイレクトかをチェックし、リダイレクトであれば、その先の記事を取得
            if (page != null && page.IsRedirect())
            {
                this.LogLine(Resources.RightArrow + " " + Resources.LogMessage_Redirect + " [[" + page.Redirect.Title + "]]");
                page = this.GetPage(page.Redirect.Title, Resources.RightArrow + " " + Resources.LogMessage_ArticleNothing);
            }

            return page;
        }

        /// <summary>
        /// ログメッセージを出力しつつ、指定された記事の指定された言語コードへの言語間リンクを返す。
        /// </summary>
        /// <param name="title">記事名。</param>
        /// <param name="code">言語コード。</param>
        /// <returns>言語間リンク先の記事名。見つからない場合は空。ページ自体が存在しない場合は<c>null</c>。</returns>
        protected string GetInterWiki(string title, string code)
        {
            MediaWikiPage page = this.GetPage(title, Resources.LogMessage_LinkArticleNothing);

            // リダイレクトかをチェックし、リダイレクトであれば、その先の記事を取得
            if (page != null && page.IsRedirect())
            {
                this.Log += Resources.LogMessage_Redirect + " [[" + page.Redirect.Title + "]] " + Resources.RightArrow + " ";
                page = this.GetPage(page.Redirect.Title, Resources.LogMessage_LinkArticleNothing);
            }

            // 記事があればその言語間リンクを取得
            string interWiki = null;
            if (page != null)
            {
                interWiki = page.GetInterWiki(this.To.Language.Code);
                if (!String.IsNullOrEmpty(interWiki))
                {
                    Log += "[[" + interWiki + "]]";
                }
                else
                {
                    Log += Resources.LogMessage_InterWikiNothing;
                }
            }

            return interWiki;
        }

        /// <summary>
        /// ログメッセージを出力しつつ、指定された記事の指定された言語コードへの言語間リンクを返す。
        /// </summary>
        /// <param name="title">記事名。</param>
        /// <param name="code">言語コード。</param>
        /// <returns>言語間リンク先の記事名。見つからない場合は空。ページ自体が存在しない場合は<c>null</c>。</returns>
        /// <remarks>対訳表が指定されている場合、その内容を使用する。また取得結果を対訳表に追加する。</remarks>
        protected string GetInterWikiUseTable(string title, string code)
        {
            if (this.ItemTable == null)
            {
                // 対訳表が指定されていない場合は、普通に記事を取得
                return this.GetInterWiki(title, code);
            }

            string interWiki = null;
            lock (this.ItemTable)
            {
                TranslationDictionary.Item item;
                if (this.ItemTable.TryGetValue(title, out item))
                {
                    // 対訳表に存在する場合はその値を使用
                    // リダイレクトがあれば、そのメッセージも表示
                    if (!String.IsNullOrWhiteSpace(item.Alias))
                    {
                        this.Log += Resources.LogMessage_Redirect + " [[" + item.Alias + "]] " + Resources.RightArrow + " ";
                    }

                    if (!String.IsNullOrEmpty(item.Word))
                    {
                        interWiki = item.Word;
                        Log += "[[" + interWiki + "]]";
                    }
                    else
                    {
                        interWiki = String.Empty;
                        Log += Resources.LogMessage_InterWikiNothing;
                    }

                    Log += Resources.LogMessageTranslation;
                    return interWiki;
                }

                // 対訳表に存在しない場合は、普通に取得し表に記録
                // ※ nullも存在しないことの記録として格納
                item = new TranslationDictionary.Item { Timestamp = DateTime.UtcNow };
                MediaWikiPage page = this.GetPage(title, Resources.LogMessage_LinkArticleNothing);

                // リダイレクトかをチェックし、リダイレクトであれば、その先の記事を取得
                if (page != null && page.IsRedirect())
                {
                    item.Alias = page.Redirect.Title;
                    this.Log += Resources.LogMessage_Redirect + " [[" + page.Redirect.Title + "]] " + Resources.RightArrow + " ";
                    page = this.GetPage(page.Redirect.Title, Resources.LogMessage_LinkArticleNothing);
                }

                // 記事があればその言語間リンクを取得
                if (page != null)
                {
                    interWiki = page.GetInterWiki(this.To.Language.Code);
                    if (!String.IsNullOrEmpty(interWiki))
                    {
                        Log += "[[" + interWiki + "]]";
                    }
                    else
                    {
                        Log += Resources.LogMessage_InterWikiNothing;
                    }

                    item.Word = interWiki;
                    this.ItemTable[title] = item;
                }
            }

            return interWiki;
        }
        
        /// <summary>
        /// 指定された記事を取得し、言語間リンクを確認、返す。
        /// </summary>
        /// <param name="title">記事名。</param>
        /// <param name="template"><c>true</c> テンプレート。</param>
        /// <returns>言語間リンク先の記事、存在しない場合 <c>null</c>。</returns>
        protected string GetInterWiki(string title, bool template)
        {
            // 指定された記事の生データをWikipediaから取得
            // ※記事自体が存在しない場合、NULLを返す
            if (!template)
            {
                Log += "[[" + title + "]] " + Resources.RightArrow + " ";
            }
            else
            {
                Log += "{{" + title + "}} " + Resources.RightArrow + " ";
            }

            // リダイレクトかをチェックし、リダイレクトであれば、その先の記事を取得
            string interWiki = this.GetInterWikiUseTable(title, this.To.Language.Code);

            // 改行が出力されていない場合（正常時）、改行
            if (!Log.EndsWith(ENTER))
            {
                Log += ENTER;
            }

            return interWiki;
        }

        /// <summary>
        /// 指定された記事を取得し、言語間リンクを確認、返す（テンプレート以外）。
        /// </summary>
        /// <param name="name">記事名。</param>
        /// <returns>言語間リンク先の記事、存在しない場合 <c>null</c>。</returns>
        protected string GetInterWiki(string name)
        {
            return this.GetInterWiki(name, false);
        }

        /// <summary>
        /// 渡されたテキストを解析し、言語間リンク・見出し等の変換を行う。
        /// </summary>
        /// <param name="text">記事テキスト。</param>
        /// <param name="parent">元記事タイトル。</param>
        /// <param name="headingEnable">見出しのチェックを行うか？</param>
        /// <returns>変換後の記事テキスト。</returns>
        protected string ReplaceText(string text, string parent, bool headingEnable)
        {
            // 指定された記事の言語間リンク・見出しを探索し、翻訳先言語での名称に変換し、それに置換した文字列を返す
            StringBuilder b = new StringBuilder();
            bool enterFlag = true;
            MediaWikiPage wikiAP = new MediaWikiPage(this.From, "dummy", null);
            for (int i = 0; i < text.Length; i++)
            {
                // ユーザーからの中止要求をチェック
                if (CancellationPending == true)
                {
                    break;
                }

                char c = text[i];

                // 見出しも処理対象の場合
                if (headingEnable)
                {
                    // 改行の場合、次のループで見出し行チェックを行う
                    if (c == '\n')
                    {
                        enterFlag = true;
                        b.Append(c);
                        continue;
                    }

                    // 行の始めでは、その行が見出しの行かのチェックを行う
                    if (enterFlag)
                    {
                        string newTitleLine;
                        int index2 = this.ChkTitleLine(out newTitleLine, text, i);
                        if (index2 != -1)
                        {
                            // 行の終わりまでインデックスを移動
                            i = index2;

                            // 置き換えられた見出し行を出力
                            b.Append(newTitleLine);
                            continue;
                        }
                        else
                        {
                            enterFlag = false;
                        }
                    }
                }

                // コメント（<!--）のチェック
                string comment;
                int index = MediaWikiTranslator.ChkComment(out comment, text, i);
                if (index != -1)
                {
                    i = index;
                    b.Append(comment);
                    if (comment.Contains("\n") == true)
                    {
                        enterFlag = true;
                    }

                    continue;
                }

                // nowikiのチェック
                string nowiki;
                index = MediaWikiTranslator.ChkNowiki(out nowiki, text, i);
                if (index != -1)
                {
                    i = index;
                    b.Append(nowiki);
                    continue;
                }

                // 変数（{{{1}}}とか）のチェック
                string variable;
                string value;
                index = wikiAP.ChkVariable(out variable, out value, text, i);
                if (index != -1)
                {
                    i = index;

                    // 変数の | 以降に値が記述されている場合、それに対して再帰的に処理を行う
                    int valueIndex = variable.IndexOf('|');
                    if (valueIndex != -1 && !String.IsNullOrEmpty(value))
                    {
                        variable = variable.Substring(0, valueIndex + 1) + this.ReplaceText(value, parent) + "}}}";
                    }

                    b.Append(variable);
                    continue;
                }

                // 内部リンク・テンプレートのチェック＆変換、言語間リンクを取得し出力する
                string subtext;
                index = this.ReplaceLink(out subtext, text, i, parent);
                if (index != -1)
                {
                    i = index;
                    b.Append(subtext);
                    continue;
                }

                // 通常はそのままコピー
                b.Append(text[i]);
            }

            return b.ToString();
        }

        /// <summary>
        /// 渡されたテキストを解析し、言語間リンク・見出し等の変換を行う。
        /// </summary>
        /// <param name="text">記事テキスト。</param>
        /// <param name="parent">元記事タイトル。</param>
        /// <returns>変換後の記事テキスト。</returns>
        protected string ReplaceText(string text, string parent)
        {
            return this.ReplaceText(text, parent, true);
        }

        /// <summary>
        /// リンクの解析・置換を行う。
        /// </summary>
        /// <param name="link">解析したリンク。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <param name="parent">元記事タイトル。</param>
        /// <returns>リンクの場合、終了位置のインデックスを返す。それ以外は-1。</returns>
        protected int ReplaceLink(out string link, string text, int index, string parent)
        {
            // 出力値初期化
            int lastIndex = -1;
            link = String.Empty;
            MediaWikiPage.Link l;

            // 内部リンク・テンプレートの確認と解析
            MediaWikiPage wikiAP = new MediaWikiPage(this.From, "dummy", null);
            lastIndex = wikiAP.ChkLinkText(out l, text, index);
            if (lastIndex != -1)
            {
                // 記事名に変数が使われている場合があるので、そのチェックと展開
                int subindex = l.Title.IndexOf("{{{");
                if (subindex != -1)
                {
                    string variable;
                    string value;
                    int lastIndex2 = wikiAP.ChkVariable(out variable, out value, l.Title, subindex);
                    if (lastIndex2 != -1 && !String.IsNullOrEmpty(value))
                    {
                        // 変数の | 以降に値が記述されている場合、それに置き換える
                        string newArticle = l.Title.Substring(0, subindex) + value;
                        if (lastIndex2 + 1 < l.Title.Length)
                        {
                            newArticle += l.Title.Substring(lastIndex2 + 1);
                        }

                        l.Title = newArticle;
                    }
                    else
                    {
                        // 値が設定されていない場合、処理してもしょうがないので、除外
                        System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceLink > 対象外 : " + l.OriginalText);
                        return -1;
                    }
                }

                string newText = null;

                // 内部リンクの場合
                if (text[index] == '[')
                {
                    // 内部リンクの変換後文字列を取得
                    newText = this.ReplaceInnerLink(l, parent);
                }
                else if (text[index] == '{')
                {
                    // テンプレートの場合
                    // テンプレートの変換後文字列を取得
                    newText = this.ReplaceTemplate(l, parent);
                }
                else
                {
                    // 上記以外の場合は、対象外
                    System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceLink > プログラムミス : " + l.OriginalText);
                }

                // 変換後文字列がNULL以外
                if (newText != null)
                {
                    link = newText;
                }
                else
                {
                    lastIndex = -1;
                }
            }

            return lastIndex;
        }

        /// <summary>
        /// 内部リンクの文字列を変換する。
        /// </summary>
        /// <param name="link">変換元リンク文字列。</param>
        /// <param name="parent">元記事タイトル。</param>
        /// <returns>変換済みリンク文字列。</returns>
        protected string ReplaceInnerLink(MediaWikiPage.Link link, string parent)
        {
            // 変数初期設定
            StringBuilder b = new StringBuilder("[[");
            string comment = String.Empty;
            MediaWikiPage.Link l = link;

            // 記事内を指している場合（[[#関連項目]]だけとか）以外
            if (!String.IsNullOrEmpty(l.Title) &&
               !(l.Title == parent && String.IsNullOrEmpty(l.Code) && !String.IsNullOrEmpty(l.Section)))
            {
                // 変換の対象外とするリンクかをチェック
                MediaWikiPage article = new MediaWikiPage(this.From, l.Title);

                // サブページの場合、記事名を補填
                if (l.IsSubpage)
                {
                    l.Title = parent + l.Title;
                }
                else if (!String.IsNullOrEmpty(l.Code))
                {
                    // 言語間リンク・姉妹プロジェクトへのリンクは対象外
                    // 先頭が : でない、翻訳先言語への言語間リンクの場合
                    if (!l.IsColon && l.Code == this.To.Language.Code)
                    {
                        // 削除する。正常終了で、置換後文字列なしを返す
                        System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceInnerLink > " + l.OriginalText + " を削除");
                        return String.Empty;
                    }

                    // それ以外は対象外
                    System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceInnerLink > 対象外 : " + l.OriginalText);
                    return null;
                }
                else if (article.IsFile())
                {
                    // 画像も対象外だが、名前空間だけ翻訳先言語の書式に変換
                    return this.ReplaceFileLink(l);
                }

                // リンクを辿り、対象記事の言語間リンクを取得
                string interWiki = this.GetInterWiki(l.Title);

                // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
                if (interWiki == null)
                {
                    b.Append(l.Title);
                }
                else if (interWiki == String.Empty)
                {
                    // 言語間リンクが存在しない場合、[[:en:xxx]]みたいな形式に置換
                    b.Append(":");
                    b.Append(this.From.Language.Code);
                    b.Append(":");
                    b.Append(l.Title);
                }
                else
                {
                    // 言語間リンクが存在する場合、そちらを指すように置換
                    // 前の文字列を復元
                    if (l.IsSubpage)
                    {
                        int index = interWiki.IndexOf('/');
                        if (index == -1)
                        {
                            index = 0;
                        }

                        b.Append(interWiki.Substring(index));
                    }
                    else if (l.IsColon)
                    {
                        b.Append(":");
                        b.Append(interWiki);
                    }
                    else
                    {
                        b.Append(interWiki);
                    }
                }

                // カテゴリーの場合は、コメントで元の文字列を追加する
                if (article.IsCategory() && !l.IsColon)
                {
                    comment = "<!-- " + l.OriginalText + " -->";

                    // カテゴリーで[[:en:xxx]]みたいな形式にした場合、| 以降は不要なので削除
                    if (interWiki == String.Empty)
                    {
                        l.PipeTexts = new List<string>();
                    }
                }
                else if (l.PipeTexts.Count == 0 && interWiki != null)
                {
                    // 表示名が存在しない場合、元の名前を表示名に設定
                    l.PipeTexts.Add(article.Title);
                }
            }

            // 見出し（[[#関連項目]]とか）を出力
            if (!String.IsNullOrEmpty(l.Section))
            {
                // 見出しは、定型句変換を通す
                string heading = this.GetHeading(l.Section);
                b.Append("#");
                b.Append(heading != null ? heading : l.Section);
            }

            // 表示名を出力
            foreach (string text in l.PipeTexts)
            {
                b.Append("|");
                if (!String.IsNullOrEmpty(text))
                {
                    // 画像の場合、| の後に内部リンクやテンプレートが書かれている場合があるが、
                    // 画像は処理対象外でありその中のリンクは個別に再度処理されるため、ここでは特に何もしない
                    b.Append(text);
                }
            }

            // リンクを閉じる
            b.Append("]]");

            // コメントを付加
            if (comment != String.Empty)
            {
                b.Append(comment);
            }

            System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceInnerLink > " + l.OriginalText);
            return b.ToString();
        }

        /// <summary>
        /// テンプレートの文字列を変換する。
        /// </summary>
        /// <param name="link">変換元テンプレート文字列。</param>
        /// <param name="parent">元記事タイトル。</param>
        /// <returns>変換済みテンプレート文字列。</returns>
        protected string ReplaceTemplate(MediaWikiPage.Link link, string parent)
        {
            // 変数初期設定
            MediaWikiPage.Link l = link;

            // テンプレートは記事名が必須
            if (String.IsNullOrEmpty(l.Title))
            {
                System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceTemplate > 対象外 : " + l.OriginalText);
                return null;
            }

            // システム変数の場合は対象外
            if (this.From.IsMagicWord(l.Title))
            {
                System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceTemplate > システム変数 : " + l.OriginalText);
                return null;
            }

            // テンプレート名前空間か、普通の記事かを判定
            if (!l.IsColon && !l.IsSubpage)
            {
                string prefix = null;
                IList<string> prefixes = this.From.Namespaces[this.From.TemplateNamespace];
                if (prefixes != null && prefixes.Count > 0)
                {
                    prefix = prefixes[0];
                }

                if (!String.IsNullOrEmpty(prefix) && !l.Title.StartsWith(prefix + ":"))
                {
                    // 頭にTemplate:を付けた記事名でアクセスし、テンプレートが存在するかをチェック
                    string title = prefix + ":" + l.Title;
                    MediaWikiPage page = null;
                    try
                    {
                        page = this.From.GetPage(title) as MediaWikiPage;
                    }
                    catch (WebException e)
                    {
                        if (e.Status == WebExceptionStatus.ProtocolError
                            && (e.Response as HttpWebResponse).StatusCode != HttpStatusCode.NotFound)
                        {
                            // 記事が取得できない場合も、404でない場合は存在するとして処理
                            this.LogLine(String.Format(Resources.LogMessage_TemplateUnknown, l.Title, prefix, e.Message));
                            l.Title = title;
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.ReplaceTemplate > " + e.Message);
                    }

                    if (page != null)
                    {
                        // 記事が存在する場合、テンプレートをつけた名前を使用
                        l.Title = title;
                    }
                }
            }
            else if (l.IsSubpage)
            {
                // サブページの場合、記事名を補填
                l.Title = parent + l.Title;
            }

            // リンクを辿り、対象記事の言語間リンクを取得
            string interWiki = this.GetInterWiki(l.Title, true);

            // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
            StringBuilder b = new StringBuilder();
            if (interWiki == null)
            {
                b.Append(l.OriginalText);
            }
            else if (interWiki == String.Empty)
            {
                // 言語間リンクが存在しない場合、[[:en:Template:xxx]]みたいな普通のリンクに置換
                // おまけで、元のテンプレートの状態をコメントでつける
                b.Append("[[:");
                b.Append(this.From.Language.Code);
                b.Append(":");
                b.Append(l.Title);
                b.Append("]]<!-- ");
                b.Append(l.OriginalText);
                b.Append(" -->");
            }
            else
            {
                // 言語間リンクが存在する場合、そちらを指すように置換
                b.Append("{{");

                // 前の文字列を復元
                if (l.IsColon)
                {
                    b.Append(":");
                }

                if (l.IsMsgnw)
                {
                    b.Append(MediaWikiPage.Msgnw);
                }

                // : より前の部分を削除して出力（: が無いときは-1+1で0から）
                b.Append(interWiki.Substring(interWiki.IndexOf(':') + 1));

                // 改行を復元
                if (l.Enter)
                {
                    b.Append("\n");
                }

                // | の後を付加
                foreach (string text in l.PipeTexts)
                {
                    b.Append("|");
                    if (!String.IsNullOrEmpty(text))
                    {
                        // | の後に内部リンクやテンプレートが書かれている場合があるので、再帰的に処理する
                        b.Append(this.ReplaceText(text, parent));
                    }
                }

                // リンクを閉じる
                b.Append("}}");
            }

            System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceTemplate > " + l.OriginalText);
            return b.ToString();
        }

        /// <summary>
        /// 指定されたインデックスの位置に存在する見出し(==関連項目==みたいなの)を解析し、可能であれば変換して返す。
        /// </summary>
        /// <param name="heading">変換後の見出し。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>見出しの場合、見出し終了位置のインデックスを返す。それ以外は-1。</returns>
        protected virtual int ChkTitleLine(out string heading, string text, int index)
        {
            // 初期化
            // ※見出しではない、構文がおかしいなどの場合、-1を返す
            int lastIndex = -1;
            
            // 構文を解析して、1行の文字列と、=の個数を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            // ※Wikipediaでは <!--test-.=<!--test-.=関連項目<!--test-.==<!--test-. みたいなのでも
            //   正常に認識するので、できるだけ対応する
            // ※変換が正常に行われた場合、コメントは削除される
            bool startFlag = true;
            int startSignCounter = 0;
            string nonCommentLine = String.Empty;
            StringBuilder b = new StringBuilder();
            for (lastIndex = index; lastIndex < text.Length; lastIndex++)
            {
                char c = text[lastIndex];

                // 改行まで
                if (c == '\n')
                {
                    break;
                }

                // コメントは無視する
                string comment;
                int subindex = MediaWikiTranslator.ChkComment(out comment, text, lastIndex);
                if (subindex != -1)
                {
                    b.Append(comment);
                    lastIndex = subindex;
                    continue;
                }
                else if (startFlag)
                {
                    // 先頭部の場合、=の数を数える
                    if (c == '=')
                    {
                        ++startSignCounter;
                    }
                    else
                    {
                        startFlag = false;
                    }
                }

                nonCommentLine += c;
                b.Append(c);
            }

            heading = b.ToString();

            // 改行文字、または文章の最後+1になっているはずなので、1文字戻す
            --lastIndex;

            // = で始まる行ではない場合、処理対象外
            if (startSignCounter < 1)
            {
                heading = String.Empty;
                return -1;
            }

            // 終わりの = の数を確認
            // ※↓の処理だと中身の無い行（====とか）は弾かれてしまうが、どうせ処理できないので許容する
            int endSignCounter = 0;
            for (int i = nonCommentLine.Length - 1; i >= startSignCounter; i--)
            {
                if (nonCommentLine[i] == '=')
                {
                    ++endSignCounter;
                }
                else
                {
                    break;
                }
            }

            // = で終わる行ではない場合、処理対象外
            if (endSignCounter < 1)
            {
                heading = String.Empty;
                return -1;
            }

            // 始まりと終わり、=の少ないほうにあわせる（==test===とか用の処理）
            int signCounter = startSignCounter;
            if (startSignCounter > endSignCounter)
            {
                signCounter = endSignCounter;
            }

            // 定型句変換
            string oldText = nonCommentLine.Substring(signCounter, nonCommentLine.Length - (signCounter * 2)).Trim();
            string newText = this.GetHeading(oldText);
            if (newText != null)
            {
                string sign = "=";
                for (int i = 1; i < signCounter; i++)
                {
                    sign += "=";
                }

                string newHeading = sign + newText + sign;
                this.LogLine(ENTER + heading + " " + Resources.RightArrow + " " + newHeading);
                heading = newHeading;
            }
            else
            {
                this.LogLine(ENTER + heading);
            }

            return lastIndex;
        }

        /// <summary>
        /// 指定されたコードでの見出しに相当する、別の言語での見出しを取得。
        /// </summary>
        /// <param name="heading">翻訳元言語での見出し。</param>
        /// <returns>翻訳先言語での見出し。値が存在しない場合は<c>null</c>。</returns>
        protected string GetHeading(string heading)
        {
            return this.HeadingTable.GetWord(heading);
        }

        /// <summary>
        /// 指定した言語での言語名称を ページ名|略称 の形式で取得。
        /// </summary>
        /// <param name="site">サイト。</param>
        /// <param name="code">言語のコード。</param>
        /// <returns>ページ名|略称形式の言語名称。</returns>
        protected string GetFullName(Website site, string code)
        {
            if (site.Language.Names.ContainsKey(code))
            {
                Language.LanguageName name = site.Language.Names[code];
                if (!String.IsNullOrEmpty(name.ShortName))
                {
                    return name.Name + "|" + name.ShortName;
                }
                else
                {
                    return name.Name;
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// 画像などのファイルへの内部リンクの置き換えを行う。
        /// </summary>
        /// <param name="link">内部リンク。</param>
        /// <returns>置き換え後のリンク文字列、置き換えを行わない場合<c>null</c>。</returns>
        private string ReplaceFileLink(MediaWikiPage.Link link)
        {
            // 名前空間だけ翻訳先言語の書式に変換
            IList<string> names;
            if (!this.To.Namespaces.TryGetValue(this.To.FileNamespace, out names))
            {
                // 翻訳先言語に相当する名前空間が無い場合、何もしない
                return null;
            }

            // 記事名の名前空間部分を置き換えて返す
            link.Title = names[0] + link.Title.Substring(link.Title.IndexOf(':'));
            return link.Text;
        }

        #endregion
    }
}
