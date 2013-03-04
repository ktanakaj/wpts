// ================================================================================================
// <summary>
//      MediaWikiのページをあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiPage.cs" company="honeplusのメモ帳">
//      Copyright (C) 2013 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Honememo.Models;
    using Honememo.Utilities;
    using Honememo.Wptscs.Parsers;

    /// <summary>
    /// MediaWikiのページをあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiPage : Page
    {
        #region コンストラクタ

        /// <summary>
        /// 指定されたMediaWikiの渡されたタイトル, 本文, タイムスタンプのページを作成。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <param name="text">ページの本文。</param>
        /// <param name="timestamp">ページのタイムスタンプ。</param>
        /// <param name="uri">ページのURI。</param>
        /// <exception cref="ArgumentNullException"><paramref name="website"/>または<paramref name="title"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentException"><paramref name="title"/>が空の文字列の場合。</exception>
        public MediaWikiPage(MediaWiki website, string title, string text, DateTime? timestamp, Uri uri)
            : base(website, title, text, timestamp, uri)
        {
            this.Interlanguages = new Dictionary<string, string>();
        }

        /// <summary>
        /// 指定されたMediaWikiの渡されたタイトル, 本文のページを作成。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <param name="text">ページの本文。</param>
        /// <remarks>ページのタイムスタンプ, URIには<c>null</c>を設定。</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="website"/>または<paramref name="title"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentException"><paramref name="title"/>が空の文字列の場合。</exception>
        public MediaWikiPage(MediaWiki website, string title, string text)
            : base(website, title, text)
        {
            this.Interlanguages = new Dictionary<string, string>();
        }

        /// <summary>
        /// 指定されたMediaWikiの渡されたタイトルのページを作成。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <remarks>ページの本文, タイムスタンプ, URIには<c>null</c>を設定。</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="website"/>または<paramref name="title"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentException"><paramref name="title"/>が空の文字列の場合。</exception>
        public MediaWikiPage(MediaWiki website, string title)
            : base(website, title)
        {
            this.Interlanguages = new Dictionary<string, string>();
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
        /// <remarks>
        /// get時に値が設定されていない場合、サーバーから本文を取得する。
        /// ページの取得に失敗した場合（通信エラーなど）は、その状況に応じた例外を投げる。
        /// </remarks>
        public override string Text
        {
            get
            {
                if (base.Text == null)
                {
                    this.SetPageBodyAndTimestamp();
                }

                return base.Text;
            }

            protected set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// ページのタイムスタンプ。
        /// </summary>
        /// <remarks>
        /// get時に値が設定されていない場合、サーバーからタイムスタンプを取得する。
        /// ページの取得に失敗した場合（通信エラーなど）は、その状況に応じた例外を投げる。
        /// </remarks>
        public override DateTime? Timestamp
        {
            get
            {
                if (base.Timestamp == null)
                {
                    this.SetPageBodyAndTimestamp();
                }

                return base.Timestamp;
            }

            protected set
            {
                base.Timestamp = value;
            }
        }

        /// <summary>
        /// リダイレクト元の記事名。
        /// </summary>
        public string Redirect
        {
            get;
            set;
        }

        /// <summary>
        /// 言語間リンクの対応表。
        /// </summary>
        protected IDictionary<string, string> Interlanguages
        {
            get;
            private set;
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// APIから取得した言語間リンク情報から、ページを取得する。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="uri">クエリーを取得したURI。</param>
        /// <param name="query">
        /// MediaWiki APIから取得した言語間リンク情報。
        /// <c>pages/page (ns="0"), redirects/r</c> を使用する。
        /// </param>
        /// <returns>言語間リンク情報から取得したページ。</returns>
        /// <exception cref="InvalidDataException">XMLのフォーマットが想定外。</exception>
        /// <exception cref="NullReferenceException">XMLのフォーマットが想定外。</exception>
        /// <exception cref="FileNotFoundException">ページが存在しない場合。</exception>
        public static MediaWikiPage GetFromQuery(MediaWiki website, Uri uri, XElement query)
        {
            // ページエレメントを取得
            // ※ この問い合わせでは、ページが無い場合も要素自体は毎回ある模様
            //    一件しか返らないはずなので先頭データを対象とする
            XElement pe;
            try
            {
                pe = (from pages in query.Elements("pages")
                      from n in pages.Elements("page")
                      select n).First();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("parse failed : pages/page element is not found");
            }

            // ページの解析
            if (pe.Attribute("missing") != null)
            {
                // missing属性が存在する場合、ページ無し
                throw new FileNotFoundException("page not found");
            }

            // ページ名、URI、リダイレクト、言語間リンク情報を詰めたオブジェクトを返す
            // ※ ページ名以外はデータがあれば格納
            MediaWikiPage page = new MediaWikiPage(website, pe.Attribute("title").Value);
            page.Uri = uri;
            var le = from links in pe.Elements("langlinks")
                     from n in links.Elements("ll")
                     select n;
            foreach (var ll in le)
            {
                page.Interlanguages.Add(ll.Attribute("lang").Value, ll.Value);
            }

            var re = from redirects in query.Elements("redirects")
                     from n in redirects.Elements("r")
                     select n;
            foreach (var r in re)
            {
                page.Redirect = r.Attribute("from").Value;
            }

            return page;
        }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// 指定された言語コードへの言語間リンクを返す。
        /// </summary>
        /// <param name="code">言語コード。</param>
        /// <returns>言語間リンク。見つからない場合は<c>null</c>。</returns>
        public virtual string GetInterlanguage(string code)
        {
            // 対応表から返す
            string interlanguage;
            if (this.Interlanguages.TryGetValue(code, out interlanguage))
            {
                return interlanguage;
            }

            return null;
        }

        /// <summary>
        /// ページがリダイレクトかをチェック。
        /// </summary>
        /// <returns><c>true</c> リダイレクト。</returns>
        public bool IsRedirect()
        {
            return this.Redirect != null;
        }

        /// <summary>
        /// ページがテンプレートかをチェック。
        /// </summary>
        /// <returns><c>true</c> テンプレート。</returns>
        public bool IsTemplate()
        {
            // ページ名がカテゴリー（Category:等で始まる）かをチェック
            return this.IsNamespacePage(this.Website.TemplateNamespace);
        }

        /// <summary>
        /// ページがカテゴリーかをチェック。
        /// </summary>
        /// <returns><c>true</c> カテゴリー。</returns>
        public bool IsCategory()
        {
            // ページ名がカテゴリー（Category:等で始まる）かをチェック
            return this.IsNamespacePage(this.Website.CategoryNamespace);
        }

        /// <summary>
        /// ページが画像かをチェック。
        /// </summary>
        /// <returns><c>true</c> 画像。</returns>
        public bool IsFile()
        {
            // ページ名がファイル（Image:等で始まる）かをチェック
            return this.IsNamespacePage(this.Website.FileNamespace);
        }

        /// <summary>
        /// ページが標準名前空間かをチェック。
        /// </summary>
        /// <returns><c>true</c> 標準名前空間。</returns>
        public bool IsMain()
        {
            // ページ名が標準名前空間以外のなんらかの名前空間かをチェック
            return !this.Website.IsNamespace(this.Title);
        }

        /// <summary>
        /// このページ内のリンクの記事名（サブページ等）を完全な記事名にする。
        /// </summary>
        /// <param name="link">このページ内のリンク。</param>
        /// <returns>変換した記事名。</returns>
        public virtual string Normalize(MediaWikiLink link)
        {
            string title = StringUtils.DefaultString(link.Title);
            if (link.IsSubpage())
            {
                // サブページ関連の正規化
                title = this.NormalizeSubpage(title);
            }
            else if (link is MediaWikiTemplate)
            {
                // テンプレート関連の正規化（サブページの場合は不要）
                title = this.NormalizeTemplate((MediaWikiTemplate)link);
            }

            return title;
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// ページの本文・タイムスタンプをサーバーから取得。
        /// </summary>
        /// <exception cref="Honememo.Wptscs.Utilities.EndPeriodException">
        /// 末尾がピリオドのページの場合（既知の不具合への対応）。
        /// </exception>
        /// <remarks>ページの取得に失敗した場合（通信エラーなど）は、その状況に応じた例外を投げる。</remarks>
        protected void SetPageBodyAndTimestamp()
        {
            Page body = this.Website.GetPageBodyAndTimestamp(this.Title);
            this.Text = body.Text;
            this.Timestamp = body.Timestamp;
            this.Uri = body.Uri;
        }

        /// <summary>
        /// ページが指定された番号の名前空間に所属するかをチェック。
        /// </summary>
        /// <param name="id">名前空間のID。</param>
        /// <returns>所属する場合<c>true</c>。</returns>
        /// <remarks>大文字小文字は区別しない。</remarks>
        protected bool IsNamespacePage(int id)
        {
            // 指定された記事名がカテゴリー（Category:等で始まる）かをチェック
            int index = this.Title.IndexOf(':');
            if (index < 0)
            {
                return false;
            }

            string title = this.Title.Remove(index);
            IgnoreCaseSet prefixes = this.Website.Namespaces[id];
            return prefixes != null && prefixes.Contains(title);
        }

        /// <summary>
        /// このページ内のリンクのサブページ形式の記事名を完全な記事名にする。
        /// </summary>
        /// <param name="subpage">サブページ形式の記事名。</param>
        /// <returns>変換した記事名。</returns>
        private string NormalizeSubpage(string subpage)
        {
            string title = subpage;
            if (subpage.StartsWith("/"))
            {
                // サブページ（子）へのリンクの場合、親の記事名を補填
                title = this.Title + subpage;
            }
            else if (subpage.StartsWith("../"))
            {
                // サブページ（親・兄弟・おじ）へのリンクの場合、各階層の記事名を補填
                string subtitle = subpage;
                int count = 0;
                while (subtitle.StartsWith("../"))
                {
                    // 階層をカウント
                    ++count;
                    subtitle = subtitle.Substring("../".Length);
                }

                // 指定された階層の記事名を補填
                string parent = this.Title;
                for (int i = 0; i < count; i++)
                {
                    int index = parent.LastIndexOf('/');
                    if (index < 0)
                    {
                        // 階層が足りない場合、補填できないので元の記事名を返す
                        return subpage;
                    }

                    parent = parent.Remove(index);
                }

                // 親記事名と子記事名（あれば）を結合して完了
                title = parent;
                if (!string.IsNullOrEmpty(subtitle))
                {
                    title += "/" + subtitle;
                }
            }

            // 末尾に / が付いている場合、表示名に関する指定なので除去
            return title.TrimEnd('/');
        }

        /// <summary>
        /// このページ内のテンプレート形式のリンクの記事名を完全な記事名にする。
        /// </summary>
        /// <param name="template">このページ内のテンプレート形式のリンク。</param>
        /// <returns>変換した記事名。</returns>
        private string NormalizeTemplate(MediaWikiTemplate template)
        {
            if (template.IsColon || this.Website.IsNamespace(template.Title)
                || this.Website.IsMagicWord(template.Title))
            {
                // 標準名前空間が指定されている（先頭にコロン）
                // または何かしらの名前空間が指定されている、
                // またはテンプレート呼び出しではなくマジックナンバーの場合、補完不要
                return template.Title;
            }

            // 補完する必要がある場合、名前空間のプレフィックス（Template等）を取得
            string prefix = this.GetTemplatePrefix();
            if (string.IsNullOrEmpty(prefix))
            {
                // 名前空間の設定が存在しない場合、何も出来ないため終了
                return template.Title;
            }

            // 頭にプレフィックスを付けた記事名を返す
            return prefix + ":" + template.Title;
        }

        /// <summary>
        /// テンプレート名前空間のプレフィックスを取得。
        /// </summary>
        /// <returns>プレフィックス。取得できない場合<c>null</c></returns>
        private string GetTemplatePrefix()
        {
            ISet<string> prefixes = this.Website.Namespaces[this.Website.TemplateNamespace];
            if (prefixes != null)
            {
                return prefixes.FirstOrDefault();
            }

            return null;
        }

        #endregion
    }
}
