// ================================================================================================
// <summary>
//      Wikipedia用の翻訳支援処理実装クラスソース</summary>
//
// <copyright file="TranslateMediaWiki.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Windows.Forms;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// Wikipedia用の翻訳支援処理実装クラスです。
    /// </summary>
    public class TranslateMediaWiki : Translate
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="from">翻訳元サイト。</param>
        /// <param name="to">翻訳先サイト。</param>
        public TranslateMediaWiki(
            MediaWiki from, MediaWiki to)
            : base(from, to)
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 翻訳元言語のサイト。
        /// </summary>
        protected new MediaWiki From
        {
            get
            {
                return base.From as MediaWiki;
            }
        }

        /// <summary>
        /// 翻訳先言語のサイト。
        /// </summary>
        protected new MediaWiki To
        {
            get
            {
                return base.To as MediaWiki;
            }
        }
        
        #endregion

        #region メイン処理メソッド

        /// <summary>
        /// 翻訳支援処理実行部の本体。
        /// ※継承クラスでは、この関数に処理を実装すること
        /// </summary>
        /// <param name="i_Name">記事名。</param>
        /// <returns><c>true</c> 処理成功。</returns>
        protected override bool RunBody(string i_Name)
        {
            System.Diagnostics.Debug.WriteLine("\nTranslateMediaWiki.runBody > " + i_Name);

            // 対象記事を取得
            MediaWikiPage article = this.ChkTargetArticle(i_Name);
            if (article == null)
            {
                return false;
            }

            // 対象記事に言語間リンクが存在する場合、処理を継続するか確認
            string interWiki = article.GetInterWiki(this.To.Language);
            if (interWiki != String.Empty)
            {
                if (MessageBox.Show(
                        String.Format(Resources.QuestionMessage_ArticleExist, interWiki),
                        Resources.QuestionTitle,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)
                   == System.Windows.Forms.DialogResult.No)
                {
                    LogLine(ENTER + String.Format(Resources.QuestionMessage_ArticleExist, interWiki));
                    return false;
                }
                else
                {
                    LogLine(Resources.RightArrow + " " + String.Format(Resources.LogMessage_ArticleExistInterWiki, interWiki));
                }
            }

            // 冒頭部を作成
            this.Text += "'''xxx'''";
            string bracket = Config.GetInstance().GetLanguage(this.To.Language).Bracket;
            if (bracket.Contains("{0}"))
            {
                string originalName = String.Empty;
                string langTitle = this.GetFullName(this.From, this.To.Language);
                if (langTitle != String.Empty)
                {
                    originalName = "[[" + langTitle + "]]: ";
                }

                this.Text += String.Format(bracket, originalName + "'''" + i_Name + "'''");
            }

            this.Text += "\n\n";

            // 言語間リンク・定型句の変換
            LogLine(ENTER + Resources.RightArrow + " " + String.Format(Resources.LogMessage_CheckAndReplaceStart, interWiki));
            this.Text += this.ReplaceText(article.Text, article.Title);

            // ユーザーからの中止要求をチェック
            if (CancellationPending)
            {
                return false;
            }

            // 新しい言語間リンクと、コメントを追記
            this.Text += "\n\n[[" + this.From.Language + ":" + i_Name + "]]\n";
            this.Text += String.Format(
                Resources.ArticleFooter,
                FormUtils.ApplicationName(),
                this.From.Language,
                i_Name,
                article.Timestamp.HasValue ? article.Timestamp.Value.ToString("U") : String.Empty) + "\n";

            // ダウンロードされるテキストがLFなので、ここで全てCRLFに変換
            // ※ダウンロード時にCRLFにするような仕組みが見つかれば、そちらを使う
            //   その場合、上のように\nをべたに吐いている部分を修正する
            this.Text = this.Text.Replace("\n", ENTER);

            System.Diagnostics.Debug.WriteLine("TranslateMediaWiki.runBody > Success!");
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
            LogLine(String.Format(Resources.LogMessage_GetArticle, this.From.Location, title));
            MediaWikiPage page = this.GetPage(title, Resources.RightArrow + " " + Resources.LogMessage_ArticleNothing);

            // リダイレクトかをチェックし、リダイレクトであれば、その先の記事を取得
            if (page != null && page.IsRedirect())
            {
                LogLine(Resources.RightArrow + " " + Resources.LogMessage_Redirect + " [[" + page.Redirect + "]]");
                page = this.GetPage(title, Resources.RightArrow + " " + Resources.LogMessage_ArticleNothing);
            }

            return page;
        }

        /// <summary>
        /// ログメッセージを出力しつつ、指定された記事の指定された言語コードへの言語間リンクを返す。
        /// </summary>
        /// <param name="title">記事名。</param>
        /// <param name="code">言語コード。</param>
        /// <returns>言語間リンク先の記事名。見つからない場合は空。</returns>
        /// <remarks>対訳表が指定されている場合、その内容を使用する。また取得結果を対訳表に追加する。</remarks>
        protected string GetInterWiki(string title, string code)
        {
            MediaWikiPage page = this.GetPage(title, Resources.RightArrow + " " + Resources.LogMessage_LinkArticleNothing);

            // リダイレクトかをチェックし、リダイレクトであれば、その先の記事を取得
            if (page != null && page.IsRedirect())
            {
                this.Log += Resources.LogMessage_Redirect + " [[" + page.Redirect + "]] " + Resources.RightArrow + " ";
                page = this.GetPage(title, Resources.RightArrow + " " + Resources.LogMessage_LinkArticleNothing);
            }

            // 記事があればその言語間リンクを取得
            string interWiki = null;
            if (page != null)
            {
                interWiki = page.GetInterWiki(this.To.Language);
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
        /// <returns>言語間リンク先の記事名。見つからない場合は空。</returns>
        /// <remarks>対訳表が指定されている場合、その内容を使用する。また取得結果を対訳表に追加する。</remarks>
        protected string GetInterWikiUseTable(string title, string code)
        {
            if (this.ItemTable == null)
            {
                // 対訳表が指定されていない場合は、普通に記事を取得
                return this.GetInterWiki(title, code);
            }

            string interWiki;
            lock (this.ItemTable)
            {
                if (this.ItemTable.ContainsKey(title))
                {
                    // 対訳表に存在する場合はその値を使用
                    interWiki = this.ItemTable[title].Word;
                    if (!String.IsNullOrEmpty(interWiki))
                    {
                        Log += "[[" + interWiki + "]]";
                    }
                    else
                    {
                        Log += Resources.LogMessage_InterWikiNothing;
                    }

                    Log += Resources.LogMessageTranslation;
                    return interWiki;
                }

                // 対訳表に存在しない場合は、普通に取得し表に記録
                // ※ nullも存在しないことの記録として格納
                interWiki = this.GetInterWiki(title, code);
                Translation.Goal goal = new Translation.Goal();
                goal.Word = interWiki;
                goal.Timestamp = DateTime.UtcNow;
                this.ItemTable[title] = goal;
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
            string interWiki = this.GetInterWikiUseTable(title, this.To.Language);

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
        /// <param name="i_Text">記事テキスト。</param>
        /// <param name="i_Parent"></param>
        /// <param name="i_TitleFlag"></param>
        /// <returns>変換後の記事テキスト。</returns>
        protected string ReplaceText(string i_Text, string i_Parent, bool i_TitleFlag)
        {
            // 指定された記事の言語間リンク・見出しを探索し、翻訳先言語での名称に変換し、それに置換した文字列を返す
            string result = String.Empty;
            bool enterFlag = true;
            MediaWikiPage wikiAP = new MediaWikiPage(this.From, "dummy", null);
            for (int i = 0; i < i_Text.Length; i++)
            {
                // ユーザーからの中止要求をチェック
                if (CancellationPending == true)
                {
                    break;
                }

                char c = i_Text[i];

                // 見出しも処理対象の場合
                if (i_TitleFlag)
                {
                    // 改行の場合、次のループで見出し行チェックを行う
                    if (c == '\n')
                    {
                        enterFlag = true;
                        result += c;
                        continue;
                    }

                    // 行の始めでは、その行が見出しの行かのチェックを行う
                    if (enterFlag)
                    {
                        string newTitleLine = String.Empty;
                        int index2 = this.ChkTitleLine(ref newTitleLine, i_Text, i);
                        if (index2 != -1)
                        {
                            // 行の終わりまでインデックスを移動
                            i = index2;

                            // 置き換えられた見出し行を出力
                            result += newTitleLine;
                            continue;
                        }
                        else
                        {
                            enterFlag = false;
                        }
                    }
                }

                // コメント（<!--）のチェック
                string comment = String.Empty;
                int index = MediaWikiPage.ChkComment(ref comment, i_Text, i);
                if (index != -1)
                {
                    i = index;
                    result += comment;
                    if (comment.Contains("\n") == true)
                    {
                        enterFlag = true;
                    }

                    continue;
                }

                // nowikiのチェック
                string nowiki = String.Empty;
                index = MediaWikiPage.ChkNowiki(ref nowiki, i_Text, i);
                if (index != -1)
                {
                    i = index;
                    result += nowiki;
                    continue;
                }

                // 変数（{{{1}}}とか）のチェック
                string variable = String.Empty;
                string value = String.Empty;
                index = wikiAP.ChkVariable(ref variable, ref value, i_Text, i);
                if (index != -1)
                {
                    i = index;

                    // 変数の | 以降に値が記述されている場合、それに対して再帰的に処理を行う
                    int valueIndex = variable.IndexOf('|');
                    if (valueIndex != -1 && !String.IsNullOrEmpty(value))
                    {
                        variable = variable.Substring(0, valueIndex + 1) + this.ReplaceText(value, i_Parent) + "}}}";
                    }

                    result += variable;
                    continue;
                }

                // 内部リンク・テンプレートのチェック＆変換、言語間リンクを取得し出力する
                string text = String.Empty;
                index = this.ReplaceLink(ref text, i_Text, i, i_Parent);
                if (index != -1)
                {
                    i = index;
                    result += text;
                    continue;
                }

                // 通常はそのままコピー
                result += i_Text[i];
            }

            return result;
        }

        /// <summary>
        /// 渡されたテキストを解析し、言語間リンク・見出し等の変換を行う。
        /// </summary>
        /// <param name="text">記事テキスト。</param>
        /// <param name="parent"></param>
        /// <returns>変換後の記事テキスト。</returns>
        protected string ReplaceText(string text, string parent)
        {
            return ReplaceText(text, parent, true);
        }

        /// <summary>
        /// リンクの解析・置換を行う。
        /// </summary>
        /// <param name="o_Link"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <param name="i_Parent"></param>
        /// <returns></returns>
        protected int ReplaceLink(ref string o_Link, string i_Text, int i_Index, string i_Parent)
        {
            // 出力値初期化
            int lastIndex = -1;
            o_Link = String.Empty;
            MediaWikiPage.Link link = new MediaWikiPage.Link();

            // 内部リンク・テンプレートの確認と解析
            MediaWikiPage wikiAP = new MediaWikiPage(this.From, "dummy", null);
            lastIndex = wikiAP.ChkLinkText(ref link, i_Text, i_Index);
            if (lastIndex != -1)
            {
                // 記事名に変数が使われている場合があるので、そのチェックと展開
                int index = link.Article.IndexOf("{{{");
                if (index != -1)
                {
                    string variable = String.Empty;
                    string value = String.Empty;
                    int lastIndex2 = wikiAP.ChkVariable(ref variable, ref value, link.Article, index);
                    if (lastIndex2 != -1 && !String.IsNullOrEmpty(value))
                    {
                        // 変数の | 以降に値が記述されている場合、それに置き換える
                        string newArticle = link.Article.Substring(0, index) + value;
                        if (lastIndex2 + 1 < link.Article.Length)
                        {
                            newArticle += link.Article.Substring(lastIndex2 + 1);
                        }

                        link.Article = newArticle;
                    }
                    else
                    {
                        // 値が設定されていない場合、処理してもしょうがないので、除外
                        System.Diagnostics.Debug.WriteLine("TranslateMediaWiki.replaceLink > 対象外 : " + link.Text);
                        return -1;
                    }
                }

                string newText = null;

                // 内部リンクの場合
                if (i_Text[i_Index] == '[')
                {
                    // 内部リンクの変換後文字列を取得
                    newText = this.ReplaceInnerLink(link, i_Parent);
                }
                else if (i_Text[i_Index] == '{')
                {
                    // テンプレートの場合
                    // テンプレートの変換後文字列を取得
                    newText = this.ReplaceTemplate(link, i_Parent);
                }
                else
                {
                    // 上記以外の場合は、対象外
                    System.Diagnostics.Debug.WriteLine("TranslateMediaWiki.replaceLink > プログラムミス : " + link.Text);
                }

                // 変換後文字列がNULL以外
                if (newText != null)
                {
                    o_Link = newText;
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
        /// <param name="i_Link"></param>
        /// <param name="i_Parent"></param>
        /// <returns></returns>
        protected string ReplaceInnerLink(MediaWikiPage.Link i_Link, string i_Parent)
        {
            // 変数初期設定
            string result = "[[";
            string comment = String.Empty;
            MediaWikiPage.Link link = i_Link;

            // 記事内を指している場合（[[#関連項目]]だけとか）以外
            if (!String.IsNullOrEmpty(link.Article) &&
               !(link.Article == i_Parent && String.IsNullOrEmpty(link.Code) && !String.IsNullOrEmpty(link.Section)))
            {
                // 変換の対象外とするリンクかをチェック
                MediaWikiPage article = new MediaWikiPage(this.From, link.Article);

                // サブページの場合、記事名を補填
                if (link.SubPageFlag)
                {
                    link.Article = i_Parent + link.Article;
                }
                else if (!String.IsNullOrEmpty(link.Code) || article.IsFile())
                {
                    // 言語間リンク・姉妹プロジェクトへのリンク・画像は対象外
                    result = String.Empty;

                    // 先頭が : でない、翻訳先言語への言語間リンクの場合
                    if (!link.StartColonFlag && link.Code == this.To.Language)
                    {
                        // 削除する。正常終了で、置換後文字列なしを返す
                        System.Diagnostics.Debug.WriteLine("TranslateMediaWiki.replaceInnerLink > " + link.Text + " を削除");
                        return String.Empty;
                    }

                    // それ以外は対象外
                    System.Diagnostics.Debug.WriteLine("TranslateMediaWiki.replaceInnerLink > 対象外 : " + link.Text);
                    return null;
                }

                // リンクを辿り、対象記事の言語間リンクを取得
                string interWiki = this.GetInterWiki(link.Article);

                // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
                if (interWiki == null)
                {
                    result += link.Article;
                }
                else if (interWiki == String.Empty)
                {
                    // 言語間リンクが存在しない場合、[[:en:xxx]]みたいな形式に置換
                    result += ":" + this.From.Language + ":" + link.Article;
                }
                else
                {
                    // 言語間リンクが存在する場合、そちらを指すように置換
                    // 前の文字列を復元
                    if (link.SubPageFlag)
                    {
                        int index = interWiki.IndexOf('/');
                        if (index == -1)
                        {
                            index = 0;
                        }

                        result += interWiki.Substring(index);
                    }
                    else if (link.StartColonFlag)
                    {
                        result += ":" + interWiki;
                    }
                    else
                    {
                        result += interWiki;
                    }
                }

                // カテゴリーの場合は、コメントで元の文字列を追加する
                if (article.IsCategory() && !link.StartColonFlag)
                {
                    comment = MediaWikiPage.CommentStart + " " + link.Text + " " + MediaWikiPage.CommentEnd;

                    // カテゴリーで[[:en:xxx]]みたいな形式にした場合、| 以降は不要なので削除
                    if (interWiki == String.Empty)
                    {
                        link.PipeTexts = new List<string>();
                    }
                }
                else if (link.PipeTexts.Count == 0 && interWiki != null)
                {
                    // 表示名が存在しない場合、元の名前を表示名に設定
                    link.PipeTexts.Add(article.Title);
                }
            }

            // 見出し（[[#関連項目]]とか）を出力
            if (!String.IsNullOrEmpty(link.Section))
            {
                // 見出しは、定型句変換を通す
                string heading = this.GetHeading(link.Section);
                result += "#" + (heading != null ? heading : link.Section);
            }

            // 表示名を出力
            foreach (string text in link.PipeTexts)
            {
                result += "|";
                if (!String.IsNullOrEmpty(text))
                {
                    // 画像の場合、| の後に内部リンクやテンプレートが書かれている場合があるが、
                    // 画像は処理対象外でありその中のリンクは個別に再度処理されるため、ここでは特に何もしない
                    result += text;
                }
            }

            // リンクを閉じる
            result += "]]";

            // コメントを付加
            if (comment != String.Empty)
            {
                result += comment;
            }

            System.Diagnostics.Debug.WriteLine("TranslateMediaWiki.replaceInnerLink > " + link.Text);
            return result;
        }

        /// <summary>
        /// テンプレートの文字列を変換する。
        /// </summary>
        /// <param name="i_Link"></param>
        /// <param name="i_Parent"></param>
        /// <returns></returns>
        protected string ReplaceTemplate(MediaWikiPage.Link i_Link, string i_Parent)
        {
            // 変数初期設定
            string result = String.Empty;
            MediaWikiPage.Link link = i_Link;

            // テンプレートは記事名が必須
            if (String.IsNullOrEmpty(link.Article))
            {
                System.Diagnostics.Debug.WriteLine("TranslateMediaWiki.replaceTemplate > 対象外 : " + link.Text);
                return null;
            }

            // システム変数の場合は対象外
            if (this.From.IsMagicWord(link.Article))
            {
                System.Diagnostics.Debug.WriteLine("TranslateMediaWiki.replaceTemplate > システム変数 : " + link.Text);
                return null;
            }

            // テンプレート名前空間か、普通の記事かを判定
            if (!link.StartColonFlag && !link.SubPageFlag)
            {
                string prefix = null;
                IList<string> prefixes = this.From.Namespaces[this.From.TemplateNamespace];
                if (prefixes != null && prefixes.Count > 0)
                {
                    prefix = prefixes[0];
                }

                if (!String.IsNullOrEmpty(prefix) && !link.Article.StartsWith(prefix + ":"))
                {
                    // 頭にTemplate:を付けた記事名でアクセスし、テンプレートが存在するかをチェック
                    string title = prefix + ":" + link.Article;
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
                            this.LogLine(String.Format(Resources.LogMessage_TemplateUnknown, link.Article, prefix, e.Message));
                            link.Article = title;
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("TranslateMediaWiki.ReplaceTemplate > " + e.Message);
                    }

                    if (page != null)
                    {
                        // 記事が存在する場合、テンプレートをつけた名前を使用
                        link.Article = title;
                    }
                }
            }
            else if (link.SubPageFlag)
            {
                // サブページの場合、記事名を補填
                link.Article = i_Parent + link.Article;
            }

            // リンクを辿り、対象記事の言語間リンクを取得
            string interWiki = this.GetInterWiki(link.Article, true);

            // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
            if (interWiki == null)
            {
                result += link.Text;
            }
            else if (interWiki == String.Empty)
            {
                // 言語間リンクが存在しない場合、[[:en:Template:xxx]]みたいな普通のリンクに置換
                // おまけで、元のテンプレートの状態をコメントでつける
                result += "[[:" + this.From.Language + ":" + link.Article + "]]" + MediaWikiPage.CommentStart + " " + link.Text + " " + MediaWikiPage.CommentEnd;
            }
            else
            {
                // 言語間リンクが存在する場合、そちらを指すように置換
                result += "{{";

                // 前の文字列を復元
                if (link.StartColonFlag)
                {
                    result += ":";
                }

                if (link.MsgnwFlag)
                {
                    result += MediaWikiPage.Msgnw;
                }

                // : より前の部分を削除して出力（: が無いときは-1+1で0から）
                result += interWiki.Substring(interWiki.IndexOf(':') + 1);

                // 改行を復元
                if (link.EnterFlag)
                {
                    result += "\n";
                }

                // | の後を付加
                foreach (string text in link.PipeTexts)
                {
                    result += "|";
                    if (!String.IsNullOrEmpty(text))
                    {
                        // | の後に内部リンクやテンプレートが書かれている場合があるので、再帰的に処理する
                        result += this.ReplaceText(text, i_Parent);
                    }
                }

                // リンクを閉じる
                result += "}}";
            }

            System.Diagnostics.Debug.WriteLine("TranslateMediaWiki.replaceTemplate > " + link.Text);
            return result;
        }

        /// <summary>
        /// 指定されたインデックスの位置に存在する見出し(==関連項目==みたいなの)を解析し、可能であれば変換して返す。
        /// </summary>
        /// <param name="o_Title"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <returns></returns>
        protected virtual int ChkTitleLine(ref string o_Title, string i_Text, int i_Index)
        {
            // 初期化
            // ※見出しではない、構文がおかしいなどの場合、-1を返す
            int lastIndex = -1;
            o_Title = String.Empty;
            
            // 構文を解析して、1行の文字列と、=の個数を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            // ※Wikipediaでは <!--test-.=<!--test-.=関連項目<!--test-.==<!--test-. みたいなのでも
            //   正常に認識するので、できるだけ対応する
            // ※変換が正常に行われた場合、コメントは削除される
            bool startFlag = true;
            int startSignCounter = 0;
            string nonCommentLine = String.Empty;
            for (lastIndex = i_Index; lastIndex < i_Text.Length; lastIndex++)
            {
                char c = i_Text[lastIndex];

                // 改行まで
                if (c == '\n')
                {
                    break;
                }

                // コメントは無視する
                string comment = String.Empty;
                int index = MediaWikiPage.ChkComment(ref comment, i_Text, lastIndex);
                if (index != -1)
                {
                    o_Title += comment;
                    lastIndex = index;
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
                o_Title += c;
            }

            // 改行文字、または文章の最後+1になっているはずなので、1文字戻す
            --lastIndex;

            // = で始まる行ではない場合、処理対象外
            if (startSignCounter < 1)
            {
                o_Title = String.Empty;
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
                o_Title = String.Empty;
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

                string newTitle = sign + newText + sign;
                LogLine(ENTER + o_Title + " " + Resources.RightArrow + " " + newTitle);
                o_Title = newTitle;
            }
            else
            {
                LogLine(ENTER + o_Title);
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
            if (!String.IsNullOrEmpty(heading) && this.HeadingTable != null)
            {
                // そのまま返してしまうと大文字小文字違いを判定できないので回す
                foreach (KeyValuePair<string, Translation.Goal> p in this.HeadingTable)
                {
                    if (p.Key.ToLower() == heading.ToLower())
                    {
                        return p.Value.Word;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 指定した言語での言語名称を ページ名|略称 の形式で取得。
        /// </summary>
        /// <param name="site">サイト。</param>
        /// <param name="code">言語のコード。</param>
        /// <returns>ページ名|略称形式の言語名称。</returns>
        protected string GetFullName(Website site, string code)
        {
            if (Config.GetInstance().GetLanguage(site.Language).Names.ContainsKey(code))
            {
                Language.LanguageName name = Config.GetInstance().GetLanguage(site.Language).Names[code];
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

        #endregion
    }
}
