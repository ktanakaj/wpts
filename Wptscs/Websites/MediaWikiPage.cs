// ================================================================================================
// <summary>
//      MediaWikiのページをあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiPage.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Honememo.Models;
    using Honememo.Parsers;
    using Honememo.Utilities;
    using Honememo.Wptscs.Parsers;

    /// <summary>
    /// MediaWikiのページをあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiPage : Page
    {
        #region private変数

        /// <summary>
        /// リダイレクト先のページ名。
        /// </summary>
        private MediaWikiLink redirect;

        #endregion

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
                this.redirect = null;

                // 本文格納のタイミングでリダイレクトページ（#REDIRECT等）かを判定
                if (!String.IsNullOrEmpty(base.Text))
                {
                    IElement element;
                    using (MediaWikiRedirectParser parser = new MediaWikiRedirectParser(this.Website))
                    {
                        if (parser.TryParse(base.Text, out element))
                        {
                            this.redirect = element as MediaWikiLink;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// リダイレクト先へのリンク。
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Text"/>が<c>null</c>の場合。</exception>
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
        
        #region 公開メソッド

        /// <summary>
        /// 指定された言語コードへの言語間リンクを返す。
        /// </summary>
        /// <param name="code">言語コード。</param>
        /// <returns>言語間リンク。見つからない場合は<c>null</c>。</returns>
        /// <exception cref="InvalidOperationException"><see cref="Text"/>が<c>null</c>の場合。</exception>
        /// <remarks>言語間リンクが複数存在する場合は、先に発見したものを返す。</remarks>
        public virtual MediaWikiLink GetInterlanguage(string code)
        {
            // Textが設定されている場合のみ有効
            this.ValidateIncomplete();

            // ページ本文から言語間リンクを探索
            // ※ 自ページの解析なのでnoincludeとして前処理を行う
            return this.GetInterlanguage(code, MediaWikiPreparser.PreprocessByNoinclude(this.Text));
        }

        /// <summary>
        /// ページがリダイレクトかをチェック。
        /// </summary>
        /// <returns><c>true</c> リダイレクト。</returns>
        /// <exception cref="InvalidOperationException"><see cref="Text"/>が<c>null</c>の場合。</exception>
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
        /// オブジェクトがメソッドの実行に不完全な状態でないか検証する。
        /// 不完全な場合、例外をスローする。
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Text"/>が<c>null</c>の場合。</exception>
        protected virtual void ValidateIncomplete()
        {
            if (this.Text == null)
            {
                // ページ本文が設定されていない場合不完全と判定
                throw new InvalidOperationException("Text is unset");
            }
        }

        /// <summary>
        /// 指定されたページテキストから言語間リンクを取得。
        /// </summary>
        /// <param name="code">言語コード。</param>
        /// <param name="text">ページテキスト。</param>
        /// <returns>言語間リンク。見つからない場合は<c>null</c>。</returns>
        /// <remarks>
        /// <para>
        /// 言語間リンクが複数存在する場合は、先に発見したものを返す。
        /// </para>
        /// <para>
        /// 稀に障害なのか<see cref="MediaWiki.MetaApi"/>から<c>interwikimap</c>
        /// が0件で返ってくることがある
        /// （API構文ミスなどでなく、それまで動いていたはずのものが）。
        /// その場合、そのままでは言語すら判別できないので、念のためこの処理では
        /// 実行前に強制的に翻訳元／先のコードを
        /// <see cref="MediaWiki.InterwikiPrefixs"/>に追加している
        /// （一時的な追加。例外にした方がよいのかもしれないが、それだとその間全く
        /// 処理が行えないため。かつトランスレータ側なら余計なログが出るぐらいで
        /// あまり影響がなくても、こちらは解析自体が失敗してしまうため）。
        /// </para>
        /// </remarks>
        private MediaWikiLink GetInterlanguage(string code, string text)
        {
            // interwikimapに強制的に翻訳元／先のコードを一時的に追加
            // ※ 2012年2月現在、キャッシュのセットが返ってくるので単純にそこに追加
            this.Website.InterwikiPrefixs.Add(this.Website.Language.Code);
            this.Website.InterwikiPrefixs.Add(code);

            // 渡されたテキストを要素単位に解析し、その結果から言語間リンクを探索する
            IElement element;
            using (MediaWikiParser parser = new MediaWikiParser(this.Website))
            {
                element = parser.Parse(text);
            }

            return this.GetInterlanguage(code, element);
        }

        /// <summary>
        /// 指定されたページ解析結果要素から言語間リンクを取得。
        /// </summary>
        /// <param name="code">言語コード。</param>
        /// <param name="element">要素。</param>
        /// <returns>言語間リンク。見つからない場合は<c>null</c>。</returns>
        /// <remarks>言語間リンクが複数存在する場合は、先に発見したものを返す。</remarks>
        private MediaWikiLink GetInterlanguage(string code, IElement element)
        {
            if (element is MediaWikiTemplate)
            {
                // Documentationテンプレートがある場合は、その中を探索
                MediaWikiLink interlanguage = this.GetDocumentationInterlanguage((MediaWikiTemplate)element, code);
                if (interlanguage != null)
                {
                    return interlanguage;
                }
            }
            else if (element is MediaWikiLink)
            {
                // 指定言語への言語間リンクの場合、内容を取得し、処理終了
                MediaWikiLink link = (MediaWikiLink)element;
                if (link.Interwiki == code && !link.IsColon)
                {
                    return link;
                }
            }
            else if (element is IEnumerable<IElement>)
            {
                // 子要素を持つ場合、再帰的に探索
                foreach (IElement e in (IEnumerable<IElement>)element)
                {
                    MediaWikiLink interlanguage = this.GetInterlanguage(code, e);
                    if (interlanguage != null)
                    {
                        return interlanguage;
                    }
                }
            }

            // 未発見の場合null
            return null;
        }

        /// <summary>
        /// 渡されたTemplate:Documentationの呼び出しから、指定された言語コードへの言語間リンクを返す。
        /// </summary>
        /// <param name="template">テンプレート呼び出しのリンク。</param>
        /// <param name="code">言語コード。</param>
        /// <returns>言語間リンク。見つからない場合またはパラメータが対象外の場合は<c>null</c>。</returns>
        /// <remarks>言語間リンクが複数存在する場合は、先に発見したものを返す。</remarks>
        private MediaWikiLink GetDocumentationInterlanguage(MediaWikiTemplate template, string code)
        {
            // Documentationテンプレートのリンクかを確認
            if (!this.IsDocumentationTemplate(template.Title))
            {
                return null;
            }

            // インライン・コンテンツの可能性があるため、先にパラメータを再帰的に探索
            foreach (IElement e in template.PipeTexts)
            {
                MediaWikiLink interlanguage = this.GetInterlanguage(code, e);
                if (interlanguage != null)
                {
                    return interlanguage;
                }
            }

            // インラインでなさそうな場合、解説記事名を確認
            string subtitle = ObjectUtils.ToString(template.PipeTexts.ElementAtOrDefault(0));
            if (String.IsNullOrWhiteSpace(subtitle) || subtitle.Contains('='))
            {
                // 指定されていない場合はデフォルトのページを探索
                subtitle = this.Website.DocumentationTemplateDefaultPage;
            }

            if (String.IsNullOrEmpty(subtitle))
            {
                return null;
            }

            // ページ名を正規化しつつ、解説ページから言語間リンクを取得
            MediaWikiPage subpage = null;
            try
            {
                // ※ ページ名を正規化するのはサブページへの対処
                // ※ 本当はここでの取得状況も画面に見せたいが、今のつくりで
                //    そうするとややこしくなるので隠蔽する。
                subpage = this.Website.GetPage(this.Normalize(new MediaWikiLink(subtitle))) as MediaWikiPage;
            }
            catch (FileNotFoundException)
            {
                // 解説ページ無し
            }
            catch (Exception ex)
            {
                // 想定外の例外だが、ここではデバッグログを吐いて終了する
                // ※ 他の処理と流れが違うため、うまい処理方法が思いつかないので
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            if (subpage != null)
            {
                // サブページの言語間リンクを返す
                // ※ テンプレート呼び出しなのでincludeとして前処理を行う
                return subpage.GetInterlanguage(
                    code,
                    MediaWikiPreparser.PreprocessByInclude(subpage.Text));
            }

            // 未発見の場合null
            return null;
        }

        /// <summary>
        /// 渡されたテンプレート名がTemplate:Documentationのいずれかに該当するか？
        /// </summary>
        /// <param name="title">テンプレート名。</param>
        /// <returns>該当する場合<c>true</c>。</returns>
        private bool IsDocumentationTemplate(string title)
        {
            // Documentationテンプレートのリンクかを確認
            string lowerTitle = title.ToLower();
            foreach (string docTitle in this.Website.DocumentationTemplates)
            {
                string lowerDocTitle = docTitle.ToLower();

                // 普通にテンプレート名を比較
                if (lowerTitle == lowerDocTitle)
                {
                    return true;
                }

                // 名前空間で一致していない可能性があるので、名前空間を取ってもう一度判定
                int index = lowerDocTitle.IndexOf(':');
                if (new MediaWikiPage(this.Website, lowerDocTitle).IsTemplate()
                    && index >= 0 && index + 1 < lowerDocTitle.Length)
                {
                    lowerDocTitle = lowerDocTitle.Substring(lowerDocTitle.IndexOf(':') + 1);
                }

                if (lowerTitle == lowerDocTitle)
                {
                    return true;
                }
            }

            return false;
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
                if (!String.IsNullOrEmpty(subtitle))
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
            if (String.IsNullOrEmpty(prefix))
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
