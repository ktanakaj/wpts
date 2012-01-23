// ================================================================================================
// <summary>
//      Wikipedia用の翻訳支援処理実装クラスソース</summary>
//
// <copyright file="MediaWikiTranslator.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
                        String.Format(Resources.QuestionMessageArticleExisted, interWiki),
                        Resources.QuestionTitle,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)
                   == System.Windows.Forms.DialogResult.No)
                {
                    this.LogLine(ENTER + String.Format(Resources.QuestionMessageArticleExisted, interWiki));
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
                    originalName = MediaWikiLink.DelimiterStart + langTitle + MediaWikiLink.DelimiterEnd + ": ";
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
            catch (ApplicationException e)
            {
                // ユーザーからの中止要求をチェック
                if (CancellationPending)
                {
                    return false;
                }
                else
                {
                    throw e;
                }
            }

            // 新しい言語間リンクと、コメントを追記
            this.Text += "\n\n" + MediaWikiLink.DelimiterStart + this.From.Language.Code + ":" + name + MediaWikiLink.DelimiterEnd + "\n";
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
                this.LogLine(Resources.RightArrow + " " + Resources.LogMessageRedirect + " " + MediaWikiLink.DelimiterStart + page.Redirect.Title + MediaWikiLink.DelimiterEnd);
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
                this.Log += Resources.LogMessageRedirect + " " + MediaWikiLink.DelimiterStart + page.Redirect.Title + MediaWikiLink.DelimiterEnd + " " + Resources.RightArrow + " ";
                page = this.GetPage(page.Redirect.Title, Resources.LogMessage_LinkArticleNothing);
            }

            // 記事があればその言語間リンクを取得
            string interWiki = null;
            if (page != null)
            {
                interWiki = page.GetInterWiki(this.To.Language.Code);
                if (!String.IsNullOrEmpty(interWiki))
                {
                    Log += MediaWikiLink.DelimiterStart + interWiki + MediaWikiLink.DelimiterEnd;
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
                        this.Log += Resources.LogMessageRedirect + " " + MediaWikiLink.DelimiterStart + item.Alias + MediaWikiLink.DelimiterEnd + " " + Resources.RightArrow + " ";
                    }

                    if (!String.IsNullOrEmpty(item.Word))
                    {
                        interWiki = item.Word;
                        Log += MediaWikiLink.DelimiterStart + interWiki + MediaWikiLink.DelimiterEnd;
                    }
                    else
                    {
                        interWiki = String.Empty;
                        Log += Resources.LogMessage_InterWikiNothing;
                    }

                    Log += Resources.LogMessageNoteTranslation;
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
                    this.Log += Resources.LogMessageRedirect + " " + MediaWikiLink.DelimiterStart + page.Redirect.Title + MediaWikiLink.DelimiterEnd + " " + Resources.RightArrow + " ";
                    page = this.GetPage(page.Redirect.Title, Resources.LogMessage_LinkArticleNothing);
                }

                // 記事があればその言語間リンクを取得
                if (page != null)
                {
                    interWiki = page.GetInterWiki(this.To.Language.Code);
                    if (!String.IsNullOrEmpty(interWiki))
                    {
                        Log += MediaWikiLink.DelimiterStart + interWiki + MediaWikiLink.DelimiterEnd;
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
                Log += MediaWikiLink.DelimiterStart + title + MediaWikiLink.DelimiterEnd + " " + Resources.RightArrow + " ";
            }
            else
            {
                Log += MediaWikiTemplate.DelimiterStart + title + MediaWikiTemplate.DelimiterEnd + " " + Resources.RightArrow + " ";
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
        /// <param name="parent">サブページ用の親記事タイトル。</param>
        /// <returns>変換後のページ要素。</returns>
        protected virtual IElement ReplaceElement(IElement element, string parent)
        {
            // ユーザーからの中止要求をチェック
            if (this.CancellationPending)
            {
                throw new ApplicationException("CancellationPending is true");
            }

            // 要素の型に応じて、必要な置き換えを行う
            if (element is MediaWikiTemplate)
            {
                // テンプレート
                return this.ReplaceTemplate((MediaWikiTemplate)element, parent);
            }
            else if (element is MediaWikiLink)
            {
                // 内部リンク
                return this.ReplaceLink((MediaWikiLink)element, parent);
            }
            else if (element is MediaWikiHeading)
            {
                // 見出し
                return this.ReplaceHeading((MediaWikiHeading)element);
            }
            else if (element is MediaWikiVariable)
            {
                // 変数
                return this.ReplaceVariable((MediaWikiVariable)element, parent);
            }
            else if (element is ListElement)
            {
                // 値を格納する要素
                return this.ReplaceListElement((ListElement)element, parent);
            }

            // それ以外は、特に何もせず元の値を返す
            return element;
        }

        /// <summary>
        /// 内部リンクを解析し、変換先言語の記事へのリンクに変換する。
        /// </summary>
        /// <param name="link">変換元リンク。</param>
        /// <param name="parent">サブページ用の親記事タイトル。</param>
        /// <returns>変換済みリンク。</returns>
        protected virtual IElement ReplaceLink(MediaWikiLink link, string parent)
        {
            // 記事名が存在しないor自記事内の別セクションへのリンクの場合、記事名絡みの処理を飛ばす
            if (!this.IsSectionLink(link, parent))
            {
                // 記事名の種類に応じて処理を実施
                MediaWikiPage article = new MediaWikiPage(this.From, link.Title);

                if (link.IsSubpage)
                {
                    // サブページの場合、記事名を補完
                    link.Title = parent + link.Title;
                }
                else if (!String.IsNullOrEmpty(link.Code))
                {
                    // 言語間リンク・姉妹プロジェクトへのリンクの場合、変換対象外とする
                    // ただし、先頭が : でない、翻訳先言語への言語間リンクだけは削除
                    return this.ReplaceLinkInterwiki(link);
                }
                else if (article.IsFile())
                {
                    // 画像の場合、名前空間を翻訳先言語の書式に変換、パラメータ部を再帰的に処理
                    return this.ReplaceLinkFile(link, parent);
                }
                else if (article.IsCategory() && !link.IsColon)
                {
                    // カテゴリで記事へのリンクでない（[[:Category:xxx]]みたいなリンクでない）場合、
                    // カテゴリ用の変換を実施
                    return this.ReplaceLinkCategory(link);
                }

                // 専用処理の無い内部リンクの場合、言語間リンクによる置き換えを行う
                string interWiki = this.GetInterWiki(link.Title);
                if (interWiki == null)
                {
                    // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
                }
                else if (interWiki == String.Empty)
                {
                    // 言語間リンクが存在しない場合、[[:en:xxx]]みたいな形式に置換
                    link.Title = this.From.Language.Code + ':' + link.Title;
                    link.IsColon = true;
                }
                else if (link.IsSubpage)
                {
                    // 言語間リンクが存在してサブページの場合、親ページ部分を消す
                    link.Title = StringUtils.Substring(interWiki, interWiki.IndexOf('/'));
                }
                else
                {
                    // 普通に言語間リンクが存在する場合、記事名を置き換え
                    link.Title = interWiki;
                }

                if (link.PipeTexts.Count == 0 && interWiki != null)
                {
                    // 表示名が存在しない場合、元の名前を表示名に設定
                    link.PipeTexts.Add(new TextElement(article.Title));
                }
            }

            // セクション部分（[[#関連項目]]とか）を変換
            if (!String.IsNullOrEmpty(link.Section))
            {
                link.Section = this.ReplaceLinkSection(link.Section);
            }

            link.ParsedString = null;
            return link;
        }

        /// <summary>
        /// テンプレートを解析し、変換先言語の記事へのテンプレートに変換する。
        /// </summary>
        /// <param name="template">変換元テンプレート。</param>
        /// <param name="parent">サブページ用の親記事タイトル。</param>
        /// <returns>変換済みテンプレート。</returns>
        protected virtual IElement ReplaceTemplate(MediaWikiTemplate template, string parent)
        {
            // システム変数（{{PAGENAME}}とか）の場合は対象外
            if (this.From.IsMagicWord(template.Title))
            {
                return template;
            }

            // テンプレートは通常名前空間が省略されているので補完する
            string filledTitle = this.FillTemplateName(template, parent);

            // リンクを辿り、対象記事の言語間リンクを取得
            string interWiki = this.GetInterWiki(filledTitle, true);
            if (interWiki == null)
            {
                // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
                return template;
            }
            else if (interWiki == String.Empty)
            {
                // 言語間リンクが存在しない場合、[[:en:Template:xxx]]みたいな普通のリンクに置換
                // おまけで、元のテンプレートの状態をコメントでつける
                ListElement list = new ListElement();
                MediaWikiLink link = new MediaWikiLink();
                link.IsColon = true;
                link.Title = this.From.Language.Code + ':' + filledTitle;
                list.Add(link);
                XmlCommentElement comment = new XmlCommentElement();
                comment.Raw = ' ' + template.ToString() + ' ';
                list.Add(comment);
                return list;
            }
            else
            {
                // 言語間リンクが存在する場合、そちらを指すように置換
                // : より前の部分を削除して出力（: が無いときは-1+1で0から）
                template.Title = interWiki.Substring(interWiki.IndexOf(':') + 1);

                // | の後に内部リンクやテンプレートが書かれている場合があるので、再帰的に処理する
                template.PipeTexts = this.ReplaceElements(template.PipeTexts, parent);
                template.ParsedString = null;
                return template;
            }
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
        /// 変数要素を再帰的に解析し、変換先言語の記事への要素に変換する。
        /// </summary>
        /// <param name="variable">変換元変数要素。</param>
        /// <param name="parent">サブページ用の親記事タイトル。</param>
        /// <returns>変換済み変数要素。</returns>
        protected virtual IElement ReplaceVariable(MediaWikiVariable variable, string parent)
        {
            // 変数、これ自体は処理しないが、再帰的に探索
            string old = variable.Value.ToString();
            variable.Value = this.ReplaceElement(variable.Value, parent);
            if (variable.Value.ToString() != old)
            {
                // 内部要素が変化した（置き換えが行われた）場合、変換前のテキストを破棄
                variable.ParsedString = null;
            }

            return variable;
        }

        /// <summary>
        /// 要素を再帰的に解析し、変換先言語の記事への要素に変換する。
        /// </summary>
        /// <param name="listElement">変換元要素。</param>
        /// <param name="parent">サブページ用の親記事タイトル。</param>
        /// <returns>変換済み要素。</returns>
        protected virtual IElement ReplaceListElement(ListElement listElement, string parent)
        {
            // 値を格納する要素、これ自体は処理しないが、再帰的に探索
            for (int i = 0; i < listElement.Count; i++)
            {
                string old = listElement[i].ToString();
                listElement[i] = this.ReplaceElement(listElement[i], parent);
                if (listElement[i].ToString() != old)
                {
                    // 内部要素が変化した（置き換えが行われた）場合、変換前のテキストを破棄
                    listElement.ParsedString = null;
                }
            }

            return listElement;
        }

        /// <summary>
        /// 同記事内の別のセクションを指すリンク（[[#関連項目]]とか[[自記事#関連項目]]とか）か？
        /// </summary>
        /// <param name="link">判定する内部リンク。</param>
        /// <param name="parent">内部リンクがあった記事。</param>
        /// <returns>セクション部分のみ変換済みリンク。</returns>
        private bool IsSectionLink(MediaWikiLink link, string parent)
        {
            // 記事名が指定されていない、または記事名が自分の記事名で
            // 言語コード等も特に無く、かつセクションが指定されている場合
            // （記事名もセクションも指定されていない・・・というケースもありえるが、
            //   その場合他に指定できるものも思いつかないので通す）
            return String.IsNullOrEmpty(link.Title)
                || (link.Title == parent && String.IsNullOrEmpty(link.Code) && !String.IsNullOrEmpty(link.Section));
        }

        /// <summary>
        /// 内部リンクのセクション部分（[[#関連項目]]とか）の定型句変換を行う。
        /// </summary>
        /// <param name="section">セクション文字列。</param>
        /// <returns>セクション部分のみ変換済みリンク。</returns>
        private string ReplaceLinkSection(string section)
        {
            // セクションが指定されている場合、定型句変換を通す
            string heading = this.GetHeading(section);
            return heading != null ? heading : section;
        }

        /// <summary>
        /// 言語間リンク指定の内部リンクを解析し、不要であれば削除する。
        /// </summary>
        /// <param name="link">変換元言語間リンク。</param>
        /// <returns>変換済み言語間リンク。</returns>
        private IElement ReplaceLinkInterwiki(MediaWikiLink link)
        {
            // 言語間リンク・姉妹プロジェクトへのリンクの場合、変換対象外とする
            // ただし、先頭が : でない、翻訳先言語への言語間リンクだけは削除
            if (!link.IsColon && link.Code == this.To.Language.Code)
            {
                return new TextElement();
            }

            return link;
        }

        /// <summary>
        /// カテゴリ指定の内部リンクを解析し、変換先言語のカテゴリへのリンクに変換する。
        /// </summary>
        /// <param name="link">変換元カテゴリ。</param>
        /// <returns>変換済みカテゴリ。</returns>
        private IElement ReplaceLinkCategory(MediaWikiLink link)
        {
            // リンクを辿り、対象記事の言語間リンクを取得
            string interWiki = this.GetInterWiki(link.Title);
            if (interWiki == null)
            {
                // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
                return link;
            }
            else if (interWiki == String.Empty)
            {
                // 言語間リンクが存在しない場合、コメントで元の文字列を保存した後
                // [[:en:xxx]]みたいな形式に置換。また | 以降は削除する
                XmlCommentElement comment = new XmlCommentElement();
                comment.Raw = ' ' + link.ToString() + ' ';

                link.Title = this.From.Language.Code + ':' + link.Title;
                link.IsColon = true;
                link.PipeTexts.Clear();
                link.ParsedString = null;

                ListElement list = new ListElement();
                list.Add(link);
                list.Add(comment);
                return list;
            }
            else
            {
                // 普通に言語間リンクが存在する場合、記事名を置き換え
                link.Title = interWiki;
                link.ParsedString = null;
                return link;
            }
        }

        /// <summary>
        /// ファイル指定の内部リンクを解析し、変換先言語で参照可能なファイルへのリンクに変換する。
        /// </summary>
        /// <param name="link">変換元リンク。</param>
        /// <param name="parent">サブページ用の親記事タイトル。</param>
        /// <returns>変換済みリンク。</returns>
        private IElement ReplaceLinkFile(MediaWikiLink link, string parent)
        {
            // 名前空間を翻訳先言語の書式に変換、またパラメータ部を再帰的に処理
            link.Title = this.ReplaceLinkNamespace(link.Title, this.To.FileNamespace);
            link.PipeTexts = this.ReplaceElements(link.PipeTexts, parent);
            link.ParsedString = null;
            return link;
        }

        /// <summary>
        /// 記事名のうち名前空間部分の変換先言語への変換を行う。
        /// </summary>
        /// <param name="title">変換元記事名。</param>
        /// <param name="id">名前空間のID。</param>
        /// <returns>変換済み記事名。</returns>
        private string ReplaceLinkNamespace(string title, int id)
        {
            // 名前空間だけ翻訳先言語の書式に変換
            IList<string> names;
            if (!this.To.Namespaces.TryGetValue(id, out names))
            {
                // 翻訳先言語に相当する名前空間が無い場合、何もしない
                return title;
            }

            // 記事名の名前空間部分を置き換えて返す
            return names[0] + title.Substring(title.IndexOf(':'));
        }

        /// <summary>
        /// 渡された要素リストに対して<see cref="ReplaceElement"/>による変換を行う。
        /// </summary>
        /// <param name="elements">変換元要素リスト。</param>
        /// <param name="parent">サブページ用の親記事タイトル。</param>
        /// <returns>変換済み要素リスト。</returns>
        private IList<IElement> ReplaceElements(IList<IElement> elements, string parent)
        {
            if (elements == null)
            {
                return null;
            }

            IList<IElement> result = new List<IElement>();
            foreach (IElement e in elements)
            {
                result.Add(this.ReplaceElement(e, parent));
            }

            return result;
        }

        /// <summary>
        /// テンプレート名に必要に応じて名前空間を補完する。
        /// </summary>
        /// <param name="template">テンプレート。</param>
        /// <param name="parent">サブページ用の親記事タイトル。</param>
        /// <returns>補完済みのテンプレート名。</returns>
        private string FillTemplateName(MediaWikiTemplate template, string parent)
        {
            if (template.IsColon || !new MediaWikiPage(this.From, template.Title).IsMain())
            {
                // 標準名前空間が指定されている（先頭にコロンが無い）
                // または何かしらの名前空間が指定されている場合、補完不要
                return template.Title;
            }
            else if (template.IsSubpage)
            {
                // サブページの場合、親記事名での補完のみ
                return parent + template.Title;
            }

            // 補完する必要がある場合、名前空間のプレフィックス（Template等）を取得
            string prefix = this.GetTemplatePrefix();
            if (String.IsNullOrEmpty(prefix))
            {
                // 名前空間の設定が存在しない場合、何も出来ないため終了
                return template.Title;
            }

            // 頭にプレフィックスを付けた記事名でアクセスし、その名前で存在するかをチェック
            string filledTitle = prefix + ":" + template.Title;
            MediaWikiPage page = null;
            try
            {
                page = this.From.GetPage(filledTitle) as MediaWikiPage;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError
                    && (e.Response as HttpWebResponse).StatusCode != HttpStatusCode.NotFound)
                {
                    // 記事が取得できない場合も、404でない場合は存在するものとして処理
                    this.LogLine(String.Format(Resources.LogMessageTemplateNameUnidentified, template.Title, prefix, e.Message));
                    return filledTitle;
                }
            }
            catch (Exception e)
            {
                // それ以外のエラー（Webではなくfileでのエラーとか）は存在しないものと扱う
                System.Diagnostics.Debug.WriteLine("MediaWikiTranslator.FillTemplateName > " + e.Message);
            }

            if (page != null)
            {
                // 記事が存在する場合、プレフィックスをつけた名前を使用
                return filledTitle;
            }

            return template.Title;
        }

        /// <summary>
        /// テンプレート名前空間のプレフィックスを取得。
        /// </summary>
        /// <returns>プレフィックス。取得できない場合<c>null</c></returns>
        private string GetTemplatePrefix()
        {
            IList<string> prefixes = this.From.Namespaces[this.From.TemplateNamespace];
            if (prefixes != null)
            {
                return prefixes.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// 指定されたコードでの見出しに相当する、別の言語での見出しを取得。
        /// </summary>
        /// <param name="heading">翻訳元言語での見出し。</param>
        /// <returns>翻訳先言語での見出し。値が存在しない場合は<c>null</c>。</returns>
        private string GetHeading(string heading)
        {
            return this.HeadingTable.GetWord(heading);
        }

        /// <summary>
        /// 指定した言語での言語名称を ページ名|略称 の形式で取得。
        /// </summary>
        /// <param name="site">サイト。</param>
        /// <param name="code">言語のコード。</param>
        /// <returns>ページ名|略称形式の言語名称。</returns>
        private string GetFullName(Website site, string code)
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

        #endregion
    }
}
