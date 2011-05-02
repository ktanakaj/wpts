// ================================================================================================
// <summary>
//      MediaWikiのページをあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiPage.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Honememo.Parsers;
    using Honememo.Utilities;
    using Honememo.Wptscs.Parsers;

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
        private MediaWikiLink redirect;

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
        public MediaWikiLink Redirect
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
            XmlParser parser = new XmlParser();
            XmlElement element;
            if (parser.TryParseXmlElement(text, out element))
            {
                if (element.Name.ToLower() == MediaWikiPage.NowikiTag)
                {
                    nowiki = element.ToString();
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
        /// <remarks>言語間リンクが複数存在する場合は、先に発見したものを返す。</remarks>
        public string GetInterWiki(string code)
        {
            // Textが設定されている場合のみ有効
            this.ValidateIncomplete();

            // 記事に存在する指定言語への言語間リンクを取得
            for (int i = 0; i < this.Text.Length; i++)
            {
                char c = this.Text[i];
                MediaWikiLink link;
                switch (c)
                {
                    case '<':
                        // コメント（<!--）またはnowiki区間の場合飛ばす
                        string subtext = this.Text.Substring(i);
                        CommentElement comment;
                        string value;
                        if (CommentElement.TryParseLazy(subtext, out comment))
                        {
                            i += comment.ToString().Length - 1;
                        }
                        else if (MediaWikiPage.TryParseNowiki(subtext, out value))
                        {
                            i += value.Length - 1;
                        }

                        break;

                    case '{':
                        // テンプレート
                        if (MediaWikiTemplate.TryParse(this.Text.Substring(i), out link))
                        {
                            i += link.Length - 1;

                            // Documentationテンプレートがある場合は、その中を探索
                            string interWiki = this.GetDocumentationInterWiki(link, code);
                            if (!String.IsNullOrEmpty(interWiki))
                            {
                                return interWiki;
                            }
                        }

                        break;

                    case '[':
                        // リンク
                        if (MediaWikiLink.TryParse(this.Text.Substring(i), out link))
                        {
                            i += link.Length - 1;

                            // 指定言語への言語間リンクの場合、内容を取得し、処理終了
                            if (link.Code == code && !link.IsColon)
                            {
                                return link.Title;
                            }
                        }

                        break;
                }
            }

            // 未発見の場合、空文字列
            return String.Empty;
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
        /// ページがテンプレートかをチェック。
        /// </summary>
        /// <returns><c>true</c> テンプレート。</returns>
        public bool IsTemplate()
        {
            // 指定された記事名がカテゴリー（Category:等で始まる）かをチェック
            return this.IsNamespacePage(this.Website.TemplateNamespace);
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
                    MediaWikiLink link;
                    if (MediaWikiLink.TryParse(this.Text.Substring(format.Length).TrimStart(), out link))
                    {
                        this.Redirect = link;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 渡されたTemplate:Documentationの呼び出しから、指定された言語コードへの言語間リンクを返す。
        /// </summary>
        /// <param name="link">テンプレート呼び出しのリンク。</param>
        /// <param name="code">言語コード。</param>
        /// <returns>言語間リンク先の記事名。見つからない場合またはパラメータが対象外の場合は空。</returns>
        /// <remarks>言語間リンクが複数存在する場合は、先に発見したものを返す。</remarks>
        private string GetDocumentationInterWiki(MediaWikiLink link, string code)
        {
            // テンプレートタグか、この言語にTemplate:Documentationの設定がされているかを確認
            string docTitle = this.Website.DocumentationTemplate;
            if (link is MediaWikiTemplate || String.IsNullOrEmpty(docTitle))
            {
                return String.Empty;
            }

            // Documentationテンプレートのリンクかを確認
            if (link.Title.ToLower() != docTitle.ToLower())
            {
                // 名前空間で一致していない可能性があるので、名前空間を取ってもう一度判定
                int index = docTitle.IndexOf(':');
                if (new MediaWikiPage(this.Website, docTitle).IsTemplate()
                    && index >= 0 && index + 1 < docTitle.Length)
                {
                    docTitle = docTitle.Substring(docTitle.IndexOf(':') + 1);
                }

                if (link.Title.ToLower() != docTitle.ToLower())
                {
                    // どちらでも一致しない場合は別のテンプレートなりなので無視
                    return String.Empty;
                }
            }

            // 解説記事名を確認
            string subtitle = link.PipeTexts.ElementAtOrDefault(0).ToString();
            if (String.IsNullOrWhiteSpace(subtitle) || subtitle.Contains('='))
            {
                // 指定されていない場合はデフォルトのページを探索
                subtitle = this.Website.DocumentationTemplateDefaultPage;
            }

            if (String.IsNullOrEmpty(subtitle))
            {
                return String.Empty;
            }

            // サブページの場合、親ページのページ名を付加
            // TODO: サブページの仕組みについては要再検討
            if (subtitle.StartsWith("/"))
            {
                subtitle = this.Title + subtitle;
            }

            // 解説ページから言語間リンクを取得
            MediaWikiPage subpage = null;
            try
            {
                // ※ 本当はここでの取得状況も画面に見せたいが、今のつくりで
                //    そうするとややこしくなるので隠蔽する。
                subpage = this.Website.GetPage(subtitle) as MediaWikiPage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            if (subpage != null)
            {
                string interWiki = subpage.GetInterWiki(code);
                if (!String.IsNullOrEmpty(interWiki))
                {
                    return interWiki;
                }
            }

            // 未発見の場合、空文字列
            return String.Empty;
        }

        #endregion
    }
}