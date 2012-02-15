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
    using Honememo.Models;
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
        #region コンストラクタ

        /// <summary>
        /// インスタンスを生成する。
        /// </summary>
        public MediaWikiTranslator()
        {
            // このクラス用のロガーと、デフォルトの確認処理としてメッセージダイアログ版を設定
            this.Logger = new MediaWikiLogger();
            this.IsContinueAtInterwikiExisted = this.IsContinueAtInterwikiExistedWithDialog;
        }

        #endregion

        #region デリゲート

        /// <summary>
        /// 対象記事に言語間リンクが存在する場合の確認処理を表すデリゲート。
        /// </summary>
        /// <param name="interwiki">言語間リンク先記事。</param>
        /// <returns>処理を続行する場合<c>true</c>。</returns>
        public delegate bool IsContinueAtInterwikiExistedDelegate(string interwiki);

        #endregion

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

        /// <summary>
        /// 対象記事に言語間リンクが存在する場合の確認処理。
        /// </summary>
        /// <remarks>確認を行わない場合<c>null</c>。</remarks>
        public IsContinueAtInterwikiExistedDelegate IsContinueAtInterwikiExisted
        {
            get;
            set;
        }

        #endregion

        #region メイン処理メソッド

        /// <summary>
        /// 翻訳支援処理実行部の本体。
        /// ※継承クラスでは、この関数に処理を実装すること
        /// </summary>
        /// <param name="name">記事名。</param>
        /// <exception cref="ApplicationException">処理が中断された場合。中断の理由は<see cref="Translator.Logger"/>に出力される。</exception>
        protected override void RunBody(string name)
        {
            // 対象記事を取得
            MediaWikiPage article = this.GetTargetPage(name);
            if (article == null)
            {
                throw new ApplicationException("article is not found");
            }

            // 対象記事に言語間リンクが存在する場合、処理を継続するか確認
            // ※ 言語間リンク取得中は、処理状態を解析中に変更
            MediaWikiLink interlanguage = null;
            this.ChangeStatusInExecuting(
                () => interlanguage = article.GetInterlanguage(this.To.Language.Code),
                Resources.StatusParsing);
            if (interlanguage != null)
            {
                // 確認処理の最中は処理時間をカウントしない（ダイアログ等を想定するため）
                this.Stopwatch.Stop();
                if (this.IsContinueAtInterwikiExisted != null && !this.IsContinueAtInterwikiExisted(interlanguage.Title))
                {
                    throw new ApplicationException("user canceled");
                }

                this.Stopwatch.Start();
                this.Logger.AddResponse(Resources.LogMessageTargetArticleHadInterWiki, interlanguage.Title);
            }

            // 冒頭部を作成
            this.Text += this.CreateOpening(article.Title);

            // 言語間リンク・定型句の変換、実行中は処理状態を解析中に設定
            this.Logger.AddSeparator();
            this.Logger.AddResponse(Resources.LogMessageStartParseAndReplace);
            this.ChangeStatusInExecuting(
                () => this.Text += this.ReplaceElement(article.Element, article).ToString(),
                Resources.StatusParsing);

            // 記事の末尾に新しい言語間リンクと、コメントを追記
            this.Text += this.CreateEnding(article);

            // ダウンロードされるテキストがLFなので、最後にクライアント環境に合わせた改行コードに変換
            // ※ダウンロード時に変換するような仕組みが見つかれば、そちらを使う
            //   その場合、上のように\nをべたに吐いている部分を修正する
            this.Text = this.Text.Replace("\n", Environment.NewLine);
        }

        #endregion

        #region 他のクラスの処理をこのクラスにあわせて拡張したメソッド

        /// <summary>
        /// ログ出力によるエラー処理を含んだページ取得処理。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <param name="page">取得したページ。ページが存在しない場合は <c>null</c> を返す。</param>
        /// <returns>処理が成功した（404も含む）場合<c>true</c>、失敗した（通信エラーなど）の場合<c>false</c>。</returns>
        /// <exception cref="ApplicationException"><see cref="Translator.CancellationPending"/>が<c>true</c>の場合。</exception>
        /// <remarks>
        /// 本メソッドは、大きく3パターンの動作を行う。
        /// <list type="number">
        /// <item><description>正常にページが取得できた → <c>true</c>でページを設定、ログ出力無し</description></item>
        /// <item><description>404など想定内の例外でページが取得できなかった → <c>true</c>でページ無し、ログ出力無し</description></item>
        /// <item><description>想定外の例外でページが取得できなかった → <c>false</c>でページ無し、ログ出力有り
        ///                    or <c>ApplicationException</c>で処理中断（アプリケーション設定のIgnoreErrorによる）。</description></item>
        /// </list>
        /// また、実行中は処理状態をサーバー接続中に更新する。
        /// 実行前後には終了要求のチェックも行う。
        /// </remarks>
        protected bool TryGetPage(string title, out MediaWikiPage page)
        {
            // &amp; &nbsp; 等の特殊文字をデコードして、親クラスのメソッドを呼び出し
            Page p;
            bool success = base.TryGetPage(WebUtility.HtmlDecode(title), out p);
            page = p as MediaWikiPage;
            return success;
        }

        #endregion

        #region 冒頭／末尾ブロックの生成メソッド

        /// <summary>
        /// 変換後記事冒頭用の「'''日本語記事名'''（[[英語|英]]: '''{{Lang|en|英語記事名}}'''）」みたいなのを作成する。
        /// </summary>
        /// <param name="title">翻訳支援対象の記事名。</param>
        /// <returns>冒頭部のテキスト。</returns>
        protected virtual string CreateOpening(string title)
        {
            string langPart = String.Empty;
            MediaWikiLink langLink = this.GetLanguageLink(this.From, this.To.Language.Code);
            if (langLink != null)
            {
                langPart = langLink.ToString() + ": ";
            }

            string langBody = this.To.FormatLang(this.From.Language.Code, title);
            if (String.IsNullOrEmpty(langBody))
            {
                langBody = title;
            }

            StringBuilder b = new StringBuilder("'''xxx'''");
            b.Append(this.To.Language.FormatBracket(langPart + "'''" + langBody + "'''"));
            b.Append("\n\n");
            return b.ToString();
        }

        /// <summary>
        /// 変換後記事末尾用の新しい言語間リンクとコメントを作成する。
        /// </summary>
        /// <param name="page">翻訳支援対象の記事。</param>
        /// <returns>末尾部のテキスト。</returns>
        protected virtual string CreateEnding(MediaWikiPage page)
        {
            MediaWikiLink link = new MediaWikiLink();
            link.Title = page.Title;
            link.Interwiki = this.From.Language.Code;
            return "\n\n" + link.ToString() + "\n" + String.Format(
                Resources.ArticleFooter,
                FormUtils.ApplicationName(),
                this.From.Language.Code,
                page.Title,
                page.Timestamp.HasValue ? page.Timestamp.Value.ToString("U") : String.Empty) + "\n";
        }

        #endregion

        #region 要素の変換メソッド

        /// <summary>
        /// 渡されたページ要素の変換を行う。
        /// </summary>
        /// <param name="element">ページ要素。</param>
        /// <param name="parent">ページ要素を取得した変換元記事。</param>
        /// <returns>変換後のページ要素。</returns>
        protected virtual IElement ReplaceElement(IElement element, MediaWikiPage parent)
        {
            // ユーザーからの中止要求をチェック
            this.ThrowExceptionIfCanceled();

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
                return this.ReplaceHeading((MediaWikiHeading)element, parent);
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
        /// <param name="parent">ページ要素を取得した変換元記事。</param>
        /// <returns>変換済みリンク。</returns>
        protected virtual IElement ReplaceLink(MediaWikiLink link, MediaWikiPage parent)
        {
            // 記事名が存在しないor自記事内の別セクションへのリンクの場合、記事名絡みの処理を飛ばす
            if (!this.IsSectionLink(link, parent.Title))
            {
                // 記事名の種類に応じて処理を実施
                MediaWikiPage article = new MediaWikiPage(this.From, link.Title);

                bool child = false;
                if (link.IsSubpage())
                {
                    // サブページ（子）の場合だけ後で記事名を復元するので記録
                    child = link.Title.StartsWith("/");
                    
                    // ページ名を完全な形に補完
                    string title = parent.Normalize(link);
                    if (parent.Title.StartsWith(title))
                    {
                        // サブページ（親）の場合、変換してもしょうがないのでセクションだけチェックして終了
                        if (!String.IsNullOrEmpty(link.Section))
                        {
                            link.Section = this.ReplaceLinkSection(link.Section);
                            link.ParsedString = null;
                        }

                        return link;
                    }

                    link.Title = title;
                }
                else if (!String.IsNullOrEmpty(link.Interwiki))
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
                string interWiki = this.GetInterlanguage(link);
                if (interWiki == null)
                {
                    // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
                }
                else if (interWiki == String.Empty)
                {
                    // 言語間リンクが存在しない場合、可能なら{{仮リンク}}に置き換え
                    if (!String.IsNullOrEmpty(this.To.LinkInterwikiFormat))
                    {
                        return this.ReplaceLinkLinkInterwiki(link);
                    }

                    // 設定が無ければ [[:en:xxx]] みたいな形式に置換
                    link.Title = this.From.Language.Code + ':' + link.Title;
                    link.IsColon = true;
                }
                else if (child)
                {
                    // 言語間リンクが存在してサブページ（子）の場合、親ページ部分を消す
                    // TODO: 兄弟や叔父のパターンも対処したい（ややこしいので現状未対応）
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
                    // 元の名前にはあればセクションも含む
                    link.PipeTexts.Add(
                        new TextElement(new MediaWikiLink { Title = article.Title, Section = link.Section }
                            .GetLinkString()));
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
        /// <param name="parent">ページ要素を取得した変換元記事。</param>
        /// <returns>変換済みテンプレート。</returns>
        protected virtual IElement ReplaceTemplate(MediaWikiTemplate template, MediaWikiPage parent)
        {
            // システム変数（{{PAGENAME}}とか）の場合は対象外
            if (this.From.IsMagicWord(template.Title))
            {
                return template;
            }

            // テンプレートは通常名前空間が省略されているので補完する
            string filledTitle = this.FillTemplateName(template, parent);

            // リンクを辿り、対象記事の言語間リンクを取得
            string interWiki = this.GetInterlanguage(new MediaWikiTemplate(filledTitle));
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
        /// <param name="heading">見出し。</param>
        /// <param name="parent">ページ要素を取得した変換元記事。</param>
        /// <returns>変換後の見出し。</returns>
        protected virtual IElement ReplaceHeading(MediaWikiHeading heading, MediaWikiPage parent)
        {
            // 変換元ログ出力
            this.Logger.AddSource(heading);

            // 定型句変換
            StringBuilder oldText = new StringBuilder();
            foreach (IElement e in heading)
            {
                oldText.Append(e.ToString());
            }

            string newText = this.GetHeading(oldText.ToString().Trim());
            if (newText != null)
            {
                // 対訳表による変換が行えた場合、変換先をログ出力し処理終了
                heading.Clear();
                heading.ParsedString = null;
                heading.Add(new XmlTextElement(newText));
                this.Logger.AddDestination(heading);
                return heading;
            }

            // 対訳表に存在しない場合、内部要素を通常の変換で再帰的に処理
            return this.ReplaceListElement(heading, parent);
        }

        /// <summary>
        /// 変数要素を再帰的に解析し、変換先言語の記事への要素に変換する。
        /// </summary>
        /// <param name="variable">変換元変数要素。</param>
        /// <param name="parent">ページ要素を取得した変換元記事。</param>
        /// <returns>変換済み変数要素。</returns>
        protected virtual IElement ReplaceVariable(MediaWikiVariable variable, MediaWikiPage parent)
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
        /// <param name="parent">ページ要素を取得した変換元記事。</param>
        /// <returns>変換済み要素。</returns>
        protected virtual IElement ReplaceListElement(ListElement listElement, MediaWikiPage parent)
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

        #endregion

        #region 対訳表アクセスメソッド

        /// <summary>
        /// 対訳表に指定された記事名の情報が登録されているか？
        /// </summary>
        /// <param name="title">記事名。</param>
        /// <returns>指定した記事の情報が登録されている場合<c>true</c>。</returns>
        /// <remarks>複数スレッドからのアクセスに対応する。また項目の対訳表が無い場合も動作する。</remarks>
        protected bool ContainsAtItemTable(string title)
        {
            if (this.ItemTable == null)
            {
                return false;
            }

            // 以下マルチスレッドで使われることも想定して対訳表へのアクセス時はロック
            lock (this.ItemTable)
            {
                // 対訳表へのキーとしてはHTMLデコードした記事名を使用する
                return this.ItemTable.ContainsKey(WebUtility.HtmlDecode(title));
            }
        }

        /// <summary>
        /// 対訳表から指定された記事名の情報を取得する。
        /// </summary>
        /// <param name="title">記事名。</param>
        /// <param name="item">翻訳先情報。</param>
        /// <returns>指定した記事の情報が登録されている場合<c>true</c>。</returns>
        /// <remarks>複数スレッドからのアクセスに対応する。</remarks>
        protected bool TryGetValueAtItemTable(string title, out TranslationDictionary.Item item)
        {
            // 以下マルチスレッドで使われることも想定して対訳表へのアクセス時はロック
            // ※ 同時アクセスを防いでいるだけで、更新処理とは同期していない。
            //    現状では同じページを同時に解析してしまう可能性があり、効率が良いソースではない。
            //    また、効率を目指すならItemTableではなく記事名ごとに一意なオブジェクトをロックすべき
            lock (this.ItemTable)
            {
                // 対訳表へのキーとしてはHTMLデコードした記事名を使用する
                return this.ItemTable.TryGetValue(WebUtility.HtmlDecode(title), out item);
            }
        }

        /// <summary>
        /// 対訳表に指定された記事名の情報を登録する。
        /// </summary>
        /// <param name="title">記事名。</param>
        /// <param name="item">翻訳先情報。</param>
        /// <remarks>複数スレッドからのアクセスに対応する。</remarks>
        protected void PutValueAtItemTable(string title, TranslationDictionary.Item item)
        {
            // 以下マルチスレッドで使われることも想定して対訳表へのアクセス時はロック
            // ※ 同時アクセスを防いでいるだけで、読込処理とは同期していない。
            //    現状では同じページを同時に解析してしまう可能性があり、効率が良いソースではない。
            //    また、効率を目指すならItemTableではなく記事名ごとに一意なオブジェクトをロックすべき
            lock (this.ItemTable)
            {
                // 対訳表へのキーとしてはHTMLデコードした記事名を使用する
                this.ItemTable[WebUtility.HtmlDecode(title)] = item;
            }
        }

        /// <summary>
        /// 指定されたコードでの見出しに相当する、別の言語での見出しを取得。
        /// </summary>
        /// <param name="heading">翻訳元言語での見出し。</param>
        /// <returns>翻訳先言語での見出し。値が存在しない場合は<c>null</c>。</returns>
        /// <remarks>見出しの対訳表が無い場合も動作する。</remarks>
        protected string GetHeading(string heading)
        {
            if (this.HeadingTable == null)
            {
                return null;
            }

            return this.HeadingTable.GetWord(heading);
        }

        #endregion

        #region 言語間リンク取得メソッド
        
        /// <summary>
        /// ロガーに取得結果を出力しつつ、指定された要素の記事の翻訳先言語への言語間リンクを返す。
        /// </summary>
        /// <param name="element">内部リンク要素。</param>
        /// <returns>言語間リンク先の記事名。見つからない場合は空。ページ自体が存在しない場合は<c>null</c>。</returns>
        /// <remarks>取得処理では対訳表を使用する。また新たな取得結果は対訳表に追加する。</remarks>
        protected string GetInterlanguage(MediaWikiLink element)
        {
            // 翻訳元をロガーに出力
            this.Logger.AddSource(element);
            string title = element.Title;
            TranslationDictionary.Item item;
            if (this.ItemTable == null)
            {
                // 対訳表が指定されていない場合は、使わずに言語間リンクを探索して終了
                return this.GetInterlanguageWithCreateCache(title, out item);
            }

            // 対訳表を使用して言語間リンクを探索
            if (this.TryGetValueAtItemTable(title, out item))
            {
                // 存在する場合はその値を使用
                if (!String.IsNullOrWhiteSpace(item.Alias))
                {
                    // リダイレクトがあれば、そのメッセージも表示
                    this.Logger.AddAlias(new MediaWikiLink(item.Alias));
                }

                if (!String.IsNullOrEmpty(item.Word))
                {
                    this.Logger.AddDestination(new MediaWikiLink(item.Word), true);
                    return item.Word;
                }
                else
                {
                    this.Logger.AddDestination(new TextElement(Resources.LogMessageInterWikiNotFound), true);
                    return String.Empty;
                }
            }

            // 対訳表に存在しない場合は、普通に取得し表に記録
            string interlanguage = this.GetInterlanguageWithCreateCache(title, out item);
            if (interlanguage != null)
            {
                // ページ自体が存在しない場合を除き、結果を対訳表に登録
                // ※ キャッシュとしては登録すべきかもしれないが、一応"対訳表"であるので
                this.PutValueAtItemTable(title, item);
            }

            return interlanguage;
        }

        /// <summary>
        /// ロガーに取得結果を出力しつつ、指定された記事の翻訳先言語への言語間リンクを返す。
        /// キャッシュ用の処理結果情報も出力する。
        /// </summary>
        /// <param name="title">記事名。</param>
        /// <param name="item">キャッシュ用の処理結果情報。</param>
        /// <returns>言語間リンク先の記事名。見つからない場合は空。ページ自体が存在しない場合は<c>null</c>。</returns>
        private string GetInterlanguageWithCreateCache(string title, out TranslationDictionary.Item item)
        {
            // 記事名から記事を探索
            item = new TranslationDictionary.Item { Timestamp = DateTime.UtcNow };
            MediaWikiPage page = this.GetDestinationPage(title);
            if (page != null && page.IsRedirect())
            {
                // リダイレクトの場合、リダイレクトである旨出力し、その先の記事を取得
                this.Logger.AddAlias(new MediaWikiLink(page.Redirect.Title));
                item.Alias = page.Redirect.Title;
                page = this.GetDestinationPage(page.Redirect.Title);
            }

            if (page == null)
            {
                // ページ自体が存在しない場合はnull
                return null;
            }

            // 記事があればその言語間リンクを取得
            MediaWikiLink interlanguage = page.GetInterlanguage(this.To.Language.Code);
            if (interlanguage != null)
            {
                item.Word = interlanguage.Title;
                this.Logger.AddDestination(interlanguage);
            }
            else
            {
                // 見つからない場合は空
                item.Word = String.Empty;
                this.Logger.AddDestination(new TextElement(Resources.LogMessageInterWikiNotFound));
            }

            return item.Word;
        }

        /// <summary>
        /// 変換先の記事を取得する。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したページ。ページが存在しない場合は <c>null</c> を返す。</returns>
        /// <remarks>記事が無い場合、通信エラーなど例外が発生した場合は、エラーログを出力する。</remarks>
        private MediaWikiPage GetDestinationPage(string title)
        {
            MediaWikiPage page;
            if (this.TryGetPage(title, out page) && page == null)
            {
                // 記事が存在しない場合だけ、変換先に「記事無し」を出力
                // ※ エラー時のログはTryGetPageが自動的に出力
                this.Logger.AddDestination(new TextElement(Resources.LogMessageLinkArticleNotFound));
            }

            return page;
        }

        #endregion

        #region 要素の変換関連その他メソッド

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
                || (link.Title == parent && String.IsNullOrEmpty(link.Interwiki) && !String.IsNullOrEmpty(link.Section));
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
            if (!link.IsColon && link.Interwiki == this.To.Language.Code)
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
            string interWiki = this.GetInterlanguage(link);
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
        /// <param name="parent">ページ要素を取得した変換元記事。</param>
        /// <returns>変換済みリンク。</returns>
        private IElement ReplaceLinkFile(MediaWikiLink link, MediaWikiPage parent)
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
            IgnoreCaseSet names;
            if (!this.To.Namespaces.TryGetValue(id, out names))
            {
                // 翻訳先言語に相当する名前空間が無い場合、何もしない
                return title;
            }

            // 記事名の名前空間部分を置き換えて返す
            return names.FirstOrDefault() + title.Substring(title.IndexOf(':'));
        }

        /// <summary>
        /// 内部リンクを他言語版への{{仮リンク}}等に変換する。。
        /// </summary>
        /// <param name="link">変換元言語間リンク。</param>
        /// <returns>変換済み言語間リンク。</returns>
        private IElement ReplaceLinkLinkInterwiki(MediaWikiLink link)
        {
            // 仮リンクにはセクションの指定が可能なので、存在する場合付加する
            // ※ 渡されたlinkをそのまま使わないのは、余計なゴミが含まれる可能性があるため
            MediaWikiLink title = new MediaWikiLink { Title = link.Title, Section = link.Section };
            string langTitle = title.GetLinkString();
            if (!String.IsNullOrEmpty(title.Section))
            {
                // 変換先言語版のセクションは、セクションの変換を通したものにする
                title.Section = this.ReplaceLinkSection(title.Section);
            }

            // 表示名は、設定されていればその値を、なければ変換元言語の記事名を使用
            string label = langTitle;
            if (link.PipeTexts.Count > 0)
            {
                label = link.PipeTexts.Last().ToString();
            }

            // 書式化した文字列を返す
            // ※ {{仮リンク}}を想定しているが、やろうと思えば何でもできるのでテキストで処理
            return new TextElement(this.To.FormatLinkInterwiki(title.GetLinkString(), this.From.Language.Code, langTitle, label));
        }

        /// <summary>
        /// 渡された要素リストに対して<see cref="ReplaceElement"/>による変換を行う。
        /// </summary>
        /// <param name="elements">変換元要素リスト。</param>
        /// <param name="parent">ページ要素を取得した変換元記事。</param>
        /// <returns>変換済み要素リスト。</returns>
        private IList<IElement> ReplaceElements(IList<IElement> elements, MediaWikiPage parent)
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
        /// <param name="parent">ページ要素を取得した変換元記事。</param>
        /// <returns>補完済みのテンプレート名。</returns>
        private string FillTemplateName(MediaWikiTemplate template, MediaWikiPage parent)
        {
            // プレフィックスが付いた記事名を作成
            string filledTitle = parent.Normalize(template);
            if (filledTitle == template.Title || template.IsSubpage())
            {
                // 補完が不要な場合、またはサブページだった場合、ここで終了
                return filledTitle;
            }

            // プレフィックスが付いた記事名が実際に存在するかを確認
            // ※ 不要かもしれないが、マジックワードの漏れ等の誤検出を減らしたいので
            if (this.ContainsAtItemTable(filledTitle))
            {
                // 対訳表に記事名が確認されている場合、既知の名前として確定
                return filledTitle;
            }

            // 実際に頭にプレフィックスを付けた記事名でアクセスし、存在するかをチェック
            // TODO: GetInterWikiの方とあわせ、テンプレートでは2度GetPageが呼ばれている。可能であれば共通化する
            MediaWikiPage page = null;
            try
            {
                // 記事が存在する場合、プレフィックスをつけた名前を使用
                page = this.From.GetPage(WebUtility.HtmlDecode(filledTitle)) as MediaWikiPage;
                return filledTitle;
            }
            catch (FileNotFoundException)
            {
                // 記事が存在しない場合、元のページ名を使用
                return template.Title;
            }
            catch (Exception e)
            {
                // 想定外の例外が発生した場合
                if (!Settings.Default.IgnoreError)
                {
                    // エラーを無視しない場合、ここで翻訳支援処理を中断する
                    throw new ApplicationException(e.Message, e);
                }

                // 続行する場合は、とりあえずプレフィックスをつけた名前で処理
                this.Logger.AddMessage(Resources.LogMessageTemplateNameUnidentified, template.Title, filledTitle, e.Message);
                return filledTitle;
            }
        }

        #endregion

        #region その他内部処理用メソッド

        /// <summary>
        /// 翻訳支援対象のページを取得。
        /// </summary>
        /// <param name="title">翻訳支援対象の記事名。</param>
        /// <returns>取得したページ。取得失敗時は<c>null</c>。</returns>
        private MediaWikiPage GetTargetPage(string title)
        {
            // 指定された記事をWikipediaから取得、リダイレクトの場合その先まで探索
            // ※ この処理ではキャッシュは使用しない。
            // ※ 万が一相互にリダイレクトしていると無限ループとなるが、特に判定はしない。
            //    ユーザーが画面上から止めることを期待。
            this.Logger.AddMessage(Resources.LogMessageGetTargetArticle, this.From.Location, title);
            MediaWikiPage page;
            for (string s = title; this.TryGetPage(s, out page); s = page.Redirect.Title)
            {
                if (page == null)
                {
                    // 記事が存在しない場合、メッセージを出力して終了
                    this.Logger.AddResponse(Resources.LogMessageTargetArticleNotFound);
                    break;
                }
                else if (!page.IsRedirect())
                {
                    // リダイレクト以外もここで終了
                    break;
                }

                // リダイレクトであれば、さらにその先の記事を取得
                this.Logger.AddResponse(Resources.LogMessageRedirect
                    + " " + new MediaWikiLink(page.Redirect.Title).ToString());
            }

            return page;
        }

        /// <summary>
        /// 指定した言語での言語名称を [[言語名称|略称]]の内部リンクで取得。
        /// </summary>
        /// <param name="site">サイト。</param>
        /// <param name="code">言語のコード。</param>
        /// <returns>[[言語名称|略称]]の内部リンク。登録されていない場合<c>null</c>。</returns>
        private MediaWikiLink GetLanguageLink(Website site, string code)
        {
            if (!site.Language.Names.ContainsKey(code))
            {
                return null;
            }

            Language.LanguageName name = site.Language.Names[code];
            MediaWikiLink link = new MediaWikiLink(name.Name);
            if (!String.IsNullOrEmpty(name.ShortName))
            {
                link.PipeTexts.Add(new TextElement(name.ShortName));
            }

            return link;
        }

        /// <summary>
        /// 対象記事に言語間リンクが存在する場合にメッセージダイアログでユーザーに確認する処理。
        /// </summary>
        /// <param name="interwiki">言語間リンク先記事。</param>
        /// <returns>処理を続行する場合<c>true</c>。</returns>
        private bool IsContinueAtInterwikiExistedWithDialog(string interwiki)
        {
            // 確認ダイアログを表示
            if (MessageBox.Show(
                        String.Format(Resources.QuestionMessageArticleExisted, interwiki),
                        Resources.QuestionTitle,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)
                   == DialogResult.No)
            {
                // 中断の場合、同じメッセージをログにも表示
                this.Logger.AddSeparator();
                this.Logger.AddMessage(Resources.QuestionMessageArticleExisted, interwiki);
                return false;
            }

            return true;
        }

        #endregion
    }
}
