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
    using Honememo.Parsers;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Parsers;
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
            try
            {
                // 指定された記事の言語間リンク・見出しを探索し、翻訳先言語での名称に変換し、それに置換した文字列を返す
                this.Text += this.ReplaceElement(new MediaWikiParser(this.From).Parse(article.Text), article.Title).ToString();
            }
            catch (ApplicationException)
            {
                // ユーザーからの中止要求をチェック
                if (CancellationPending)
                {
                    return false;
                }
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
        /// 渡されたページ要素の変換を行う。
        /// </summary>
        /// <param name="element">ページ要素。</param>
        /// <param name="parent">元記事タイトル。</param>
        /// <returns>変換後のページ要素。</returns>
        protected IElement ReplaceElement(IElement element, string parent)
        {
            // ユーザーからの中止要求をチェック
            if (this.CancellationPending)
            {
                // TODO: ちゃんとした例外型を用意する
                throw new ApplicationException("CancellationPending is true");
            }

            // 要素の型に応じて、必要な置き換えを行う
            // TODO: 第一段階として、とりあえずみんな既存同様テキストで返してる
            if (element is MediaWikiTemplate)
            {
                // テンプレート
                return this.ReplaceTemplate((MediaWikiTemplate)element, parent);
            }
            else if (element is MediaWikiLink)
            {
                // 内部リンク
                // TODO: ここも再帰がいるはず
                return this.ReplaceInnerLink((MediaWikiLink)element, parent);
            }
            else if (element is MediaWikiHeading)
            {
                // 見出し
                // TODO: ここも再帰がいるはず
                return this.ReplaceHeading((MediaWikiHeading)element);
            }
            else if (element is MediaWikiVariable)
            {
                // 変数、これ自体は処理しないが、再帰的に探索
                IElement innerElement = ((MediaWikiVariable)element).Value;
                string old = innerElement.ToString();
                innerElement = this.ReplaceElement(innerElement, parent);
                if (innerElement.ToString() != old)
                {
                    element.ParsedString = null;
                }

                return element;
            }
            else if (element is ListElement)
            {
                // 値を格納する要素、これ自体は処理しないが、再帰的に探索
                ListElement listElement = (ListElement)element;
                for (int i = 0; i < listElement.Count; i++)
                {
                    string old = listElement[i].ToString();
                    listElement[i] = this.ReplaceElement(listElement[i], parent);
                    if (listElement[i].ToString() != old)
                    {
                        element.ParsedString = null;
                    }
                }

                return element;
            }

            // それ以外は、特に何もせず元の値を返す
            return element;
        }

        /// <summary>
        /// 内部リンクの文字列を変換する。
        /// </summary>
        /// <param name="link">変換元リンク文字列。</param>
        /// <param name="parent">元記事タイトル。</param>
        /// <returns>変換済みリンク文字列。</returns>
        protected IElement ReplaceInnerLink(MediaWikiLink link, string parent)
        {
            // 変数初期設定
            string comment = String.Empty;

            // 記事内を指している場合（[[#関連項目]]だけとか）以外
            if (!String.IsNullOrEmpty(link.Title)
                && !(link.Title == parent && String.IsNullOrEmpty(link.Code) && !String.IsNullOrEmpty(link.Section)))
            {
                // 変換の対象外とするリンクかをチェック
                MediaWikiPage article = new MediaWikiPage(this.From, link.Title);

                // サブページの場合、記事名を補填
                if (link.IsSubpage)
                {
                    link.Title = parent + link.Title;
                }
                else if (!String.IsNullOrEmpty(link.Code))
                {
                    // 言語間リンク・姉妹プロジェクトへのリンクは対象外
                    // 先頭が : でない、翻訳先言語への言語間リンクの場合
                    if (!link.IsColon && link.Code == this.To.Language.Code)
                    {
                        // 削除する。正常終了で、置換後文字列を空で返す
                        System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceInnerLink > " + link.ToString() + " を削除");
                        return new TextElement();
                    }

                    // それ以外は対象外でそのまま
                    System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceInnerLink > 対象外 : " + link.ToString());
                    return link;
                }
                else if (article.IsFile())
                {
                    // 画像も対象外だが、名前空間だけ翻訳先言語の書式に変換
                    return this.ReplaceFileLink(link);
                }

                // リンクを辿り、対象記事の言語間リンクを取得
                string interWiki = this.GetInterWiki(link.Title);

                if (interWiki == null)
                {
                    // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
                }
                else if (interWiki == String.Empty)
                {
                    // 言語間リンクが存在しない場合、[[:en:xxx]]みたいな形式に置換
                    link.IsColon = true;
                    link.Title = this.From.Language.Code + ':' + link.Title;
                }
                else
                {
                    // 言語間リンクが存在する場合、そちらを指すように置換
                    if (link.IsSubpage)
                    {
                        // サブページの場合、親ページ部分は消す
                        int index = interWiki.IndexOf('/');
                        if (index == -1)
                        {
                            index = 0;
                        }

                        link.Title = interWiki.Substring(index);
                    }
                    else
                    {
                        link.Title = interWiki;
                    }
                }

                // カテゴリーの場合は、コメントで元の文字列を追加する
                if (article.IsCategory() && !link.IsColon)
                {
                    comment = ' ' + link.ToString() + ' ';

                    // カテゴリーで[[:en:xxx]]みたいな形式にした場合、| 以降は不要なので削除
                    if (interWiki == String.Empty)
                    {
                        link.PipeTexts.Clear();
                    }
                }
                else if (link.PipeTexts.Count == 0 && interWiki != null)
                {
                    // 表示名が存在しない場合、元の名前を表示名に設定
                    link.PipeTexts.Add(new TextElement(article.Title));
                }
            }

            // 見出し（[[#関連項目]]とか）を出力
            if (!String.IsNullOrEmpty(link.Section))
            {
                // 見出しは、定型句変換を通す
                string heading = this.GetHeading(link.Section);
                link.Section = heading != null ? heading : link.Section;
            }

            link.ParsedString = null;

            // コメントを付加
            IElement result = link;
            if (comment != String.Empty)
            {
                ListElement list = new ListElement();
                list.Add(link);
                list.Add(new XmlCommentElement(comment));
                result = list;
            }

            System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceInnerLink > " + result.ToString());
            return result;
        }

        /// <summary>
        /// テンプレートの文字列を変換する。
        /// </summary>
        /// <param name="link">変換元テンプレート文字列。</param>
        /// <param name="parent">元記事タイトル。</param>
        /// <returns>変換済みテンプレート文字列。</returns>
        protected IElement ReplaceTemplate(MediaWikiTemplate link, string parent)
        {
            // システム変数の場合は対象外
            if (this.From.IsMagicWord(link.Title))
            {
                System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceTemplate > システム変数 : " + link.ToString());
                return link;
            }

            // テンプレート名前空間か、普通の記事かを判定
            if (!link.IsColon && !link.IsSubpage)
            {
                string prefix = null;
                IList<string> prefixes = this.From.Namespaces[this.From.TemplateNamespace];
                if (prefixes != null && prefixes.Count > 0)
                {
                    prefix = prefixes[0];
                }

                if (!String.IsNullOrEmpty(prefix) && !link.Title.StartsWith(prefix + ":"))
                {
                    // 頭にTemplate:を付けた記事名でアクセスし、テンプレートが存在するかをチェック
                    string title = prefix + ":" + link.Title;
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
                            this.LogLine(String.Format(Resources.LogMessage_TemplateUnknown, link.Title, prefix, e.Message));
                            link.Title = title;
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.ReplaceTemplate > " + e.Message);
                    }

                    if (page != null)
                    {
                        // 記事が存在する場合、テンプレートをつけた名前を使用
                        link.Title = title;
                    }
                }
            }
            else if (link.IsSubpage)
            {
                // サブページの場合、記事名を補填
                link.Title = parent + link.Title;
            }

            // リンクを辿り、対象記事の言語間リンクを取得
            string interWiki = this.GetInterWiki(link.Title, true);
            IElement result = link;

            if (interWiki == null)
            {
                // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
            }
            else if (interWiki == String.Empty)
            {
                // 言語間リンクが存在しない場合、[[:en:Template:xxx]]みたいな普通のリンクに置換
                // おまけで、元のテンプレートの状態をコメントでつける
                ListElement list = new ListElement();
                MediaWikiLink l = new MediaWikiLink();
                l.IsColon = true;
                l.Title = this.From.Language.Code + ':' + link.Title;
                list.Add(l);
                list.Add(new XmlCommentElement(' ' + link.ToString() + ' '));
                result = list;
            }
            else
            {
                // 言語間リンクが存在する場合、そちらを指すように置換
                // : より前の部分を削除して出力（: が無いときは-1+1で0から）
                link.Title = interWiki.Substring(interWiki.IndexOf(':') + 1);

                // | の後に内部リンクやテンプレートが書かれている場合があるので、再帰的に処理する
                for (int i = 0; i < link.PipeTexts.Count; i++)
                {
                    link.PipeTexts[i] = this.ReplaceElement(link.PipeTexts[i], parent);
                }

                link.ParsedString = null;
            }

            System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.replaceTemplate > " + result.ToString());
            return result;
        }

        /// <summary>
        /// 指定された見出しに対して、対訳表による変換を行う。
        /// </summary>
        /// <param name="element">見出し。</param>
        /// <returns>変換後の見出し。</returns>
        protected virtual IElement ReplaceHeading(MediaWikiHeading element)
        {
            // 定型句変換
            StringBuilder oldText = new StringBuilder();
            foreach (IElement e in element)
            {
                oldText.Append(e.ToString());
            }

            string oldHeading = element.ToString();
            string newText = this.GetHeading(oldText.ToString().Trim());
            if (newText != null)
            {
                element.Clear();
                element.ParsedString = null;
                element.Add(new XmlTextElement(newText));
                this.LogLine(ENTER + oldHeading + " " + Resources.RightArrow + " " + element.ToString());
            }
            else
            {
                this.LogLine(ENTER + element.ToString());
            }

            return element;
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
        private IElement ReplaceFileLink(MediaWikiLink link)
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
            link.ParsedString = null;
            return link;
        }

        #endregion
    }
}
