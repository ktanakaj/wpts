// ================================================================================================
// <summary>
//      MediaWikiのウェブサイト（システム）をあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWiki.cs" company="honeplusのメモ帳">
//      Copyright (C) 2014 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Honememo.Models;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;
    using Honememo.Wptscs.Utilities;

    /// <summary>
    /// MediaWikiのウェブサイト（システム）をあらわすモデルクラスです。
    /// </summary>
    public class MediaWiki : Website, IXmlSerializable
    {
        #region private変数
        
        /// <summary>
        /// 名前空間情報取得用にアクセスするAPI。
        /// </summary>
        private string metaApi;

        /// <summary>
        /// MediaWiki記事データ取得用にアクセスするAPI。
        /// </summary>
        private string contentApi;

        /// <summary>
        /// MediaWiki言語間リンク取得用にアクセスするAPI。
        /// </summary>
        private string interlanguageApi;

        /// <summary>
        /// テンプレートの名前空間を示す番号。
        /// </summary>
        private int? templateNamespace;

        /// <summary>
        /// カテゴリの名前空間を示す番号。
        /// </summary>
        private int? categoryNamespace;

        /// <summary>
        /// 画像の名前空間を示す番号。
        /// </summary>
        private int? fileNamespace;

        /// <summary>
        /// MediaWiki書式のシステム定義変数。
        /// </summary>
        private ISet<string> magicWords;

        /// <summary>
        /// MediaWikiの名前空間の情報。
        /// </summary>
        private IDictionary<int, IgnoreCaseSet> namespaces;

        /// <summary>
        /// MediaWikiのウィキ間リンクのプレフィックス情報。
        /// </summary>
        private IgnoreCaseSet interwikiPrefixs;

        /// <summary>
        /// MediaWikiのウィキ間リンクのプレフィックス情報（APIから取得した値と設定値の集合）。
        /// </summary>
        private IgnoreCaseSet interwikiPrefixCaches;

        /// <summary>
        /// <see cref="InitializeByMetaApi"/>同期用ロックオブジェクト。
        /// </summary>
        private object lockLoadMetaApi = new object();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定された言語, サーバーのMediaWikiを表すインスタンスを作成。
        /// </summary>
        /// <param name="language">ウェブサイトの言語。</param>
        /// <param name="location">ウェブサイトの場所。</param>
        /// <exception cref="ArgumentNullException"><paramref name="language"/>または<paramref name="location"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentException"><paramref name="location"/>が空の文字列の場合。</exception>
        public MediaWiki(Language language, string location)
            : base(language, location)
        {
        }

        /// <summary>
        /// 指定された言語のWikipediaを表すインスタンスを作成。
        /// </summary>
        /// <param name="language">ウェブサイトの言語。</param>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        public MediaWiki(Language language)
        {
            // 親で初期化していないのは、languageのnullチェックの前にnull参照でエラーになってしまうから
            this.Language = language;
            this.Location = string.Format(Settings.Default.WikipediaLocation, language.Code);
        }

        /// <summary>
        /// 空のインスタンスを作成（シリアライズ or 拡張用）。
        /// </summary>
        protected MediaWiki()
        {
        }

        #endregion

        #region 設定ファイルに初期値を持つプロパティ
        
        /// <summary>
        /// MediaWikiメタ情報取得用にアクセスするAPI。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
        public string MetaApi
        {
            get
            {
                if (string.IsNullOrEmpty(this.metaApi))
                {
                    return Settings.Default.MediaWikiMetaApi;
                }

                return this.metaApi;
            }

            set
            {
                this.metaApi = value;
            }
        }

        /// <summary>
        /// MediaWiki記事データ取得用にアクセスするAPI。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
        public string ContentApi
        {
            get
            {
                if (string.IsNullOrEmpty(this.contentApi))
                {
                    return Settings.Default.MediaWikiContentApi;
                }

                return this.contentApi;
            }

            set
            {
                this.contentApi = value;
            }
        }

        /// <summary>
        /// MediaWiki言語間リンク取得用にアクセスするAPI。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
        public string InterlanguageApi
        {
            get
            {
                if (string.IsNullOrEmpty(this.interlanguageApi))
                {
                    return Settings.Default.MediaWikiInterlanguageApi;
                }

                return this.interlanguageApi;
            }

            set
            {
                this.interlanguageApi = value;
            }
        }

        /// <summary>
        /// テンプレートの名前空間を示す番号。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
        public int TemplateNamespace
        {
            get
            {
                return this.templateNamespace ?? Settings.Default.MediaWikiTemplateNamespace;
            }

            set
            {
                this.templateNamespace = value;
            }
        }

        /// <summary>
        /// カテゴリの名前空間を示す番号。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
        public int CategoryNamespace
        {
            get
            {
                return this.categoryNamespace ?? Settings.Default.MediaWikiCategoryNamespace;
            }

            set
            {
                this.categoryNamespace = value;
            }
        }

        /// <summary>
        /// 画像の名前空間を示す番号。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
        public int FileNamespace
        {
            get
            {
                return this.fileNamespace ?? Settings.Default.MediaWikiFileNamespace;
            }

            set
            {
                this.fileNamespace = value;
            }
        }

        /// <summary>
        /// MediaWiki書式のシステム定義変数。
        /// </summary>
        /// <remarks>
        /// 値が指定されていない場合、デフォルト値を返す。
        /// 大文字小文字を区別する。
        /// </remarks>
        public ISet<string> MagicWords
        {
            get
            {
                if (this.magicWords == null)
                {
                    // ※ 初期値は http://www.mediawiki.org/wiki/Help:Magic_words 等を参考に設定。
                    //    APIからも取得できるが、2012年2月現在 #expr でなければ認識されないものが
                    //    exprで返ってきたりとアプリで使うには情報が足りないため人力で対応。
                    return new HashSet<string>(Settings.Default.MediaWikiMagicWords.Cast<string>());
                }

                return this.magicWords;
            }

            set
            {
                this.magicWords = value;
            }
        }

        #endregion

        #region サーバーから値を取得するプロパティ

        /// <summary>
        /// MediaWikiの名前空間の情報。
        /// </summary>
        /// <remarks>
        /// サーバーから情報を取得。大文字小文字を区別しない。
        /// </remarks>
        public IDictionary<int, IgnoreCaseSet> Namespaces
        {
            get
            {
                // 値が設定されていない場合、サーバーから取得して初期化する
                // ※ コンストラクタ等で初期化していないのは、通信の準備が整うまで行えないため
                // ※ 余計なロック・通信をしないよう、ロックの前後に値のチェックを行う
                if (this.namespaces != null)
                {
                    return this.namespaces;
                }

                lock (this.lockLoadMetaApi)
                {
                    if (this.namespaces != null)
                    {
                        return this.namespaces;
                    }

                    this.InitializeByMetaApi();
                }

                return this.namespaces;
            }

            protected set
            {
                this.namespaces = value;
            }
        }

        /// <summary>
        /// MediaWikiのウィキ間リンクのプレフィックス情報。
        /// </summary>
        /// <remarks>
        /// 値が設定されていない場合デフォルト値とサーバーから、
        /// 設定されている場合その内容とサーバーから取得した情報を使用する。
        /// 大文字小文字を区別しない。
        /// </remarks>
        public IgnoreCaseSet InterwikiPrefixs
        {
            get
            {
                // 値が準備されていない場合、サーバーと設定ファイルから取得して初期化する
                // ※ コンストラクタ等で初期化していないのは、通信の準備が整うまで行えないため
                // ※ 余計なロック・通信をしないよう、ロックの前後に値のチェックを行う
                if (this.interwikiPrefixCaches != null)
                {
                    return this.interwikiPrefixCaches;
                }

                lock (this.lockLoadMetaApi)
                {
                    if (this.interwikiPrefixCaches != null)
                    {
                        return this.interwikiPrefixCaches;
                    }

                    this.InitializeByMetaApi();
                }

                return this.interwikiPrefixCaches;
            }

            set
            {
                // 値を代入しキャッシュを消去
                this.interwikiPrefixs = value;
                this.interwikiPrefixCaches = null;
            }
        }

        #endregion

        #region それ以外のプロパティ

        /// <summary>
        /// Template:仮リンク（他言語へのリンク）で書式化するためのフォーマット。
        /// </summary>
        /// <remarks>空の場合、その言語版にはこれに相当する機能は無いor使用しないものとして扱う。</remarks>
        public string LinkInterwikiFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Template:Langで書式化するためのフォーマット。
        /// </summary>
        /// <remarks>空の場合、その言語版にはこれに相当する機能は無いor使用しないものとして扱う。</remarks>
        public string LangFormat
        {
            get;
            set;
        }

        /// <summary>
        /// 言語名の記事が存在するようなサイトか？
        /// </summary>
        /// <remarks>
        /// そうした記事が存在する場合<c>true</c>。
        /// Wikipediaには[[日本語]]といった記事が存在するが、Wikitravelであればそうした記事は存在し得ない。
        /// 言語名をリンクにするかといった判断に用いる。
        /// </remarks>
        public bool HasLanguagePage
        {
            get;
            set;
        }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// ページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したページ。</returns>
        /// <exception cref="InvalidDataException">APIから取得したデータが想定外。</exception>
        /// <exception cref="NullReferenceException">APIから取得したデータが想定外。</exception>
        /// <exception cref="FileNotFoundException">ページが存在しない場合。</exception>
        /// <remarks>
        /// ページの取得に失敗した場合（通信エラーなど）は、その状況に応じた例外を投げる。
        /// このメソッドでは記事本文や日時といった情報は取得しない。
        /// そうした情報は、実際にアクセスされたタイミングで動的に取得する。
        /// </remarks>
        public override Page GetPage(string title)
        {
            // 言語間リンク取得用のURIを生成
            Uri uri = this.CreateUri(this.InterlanguageApi, title);

            // ページの言語間リンク情報XMLデータをMediaWikiサーバーから取得
            XElement doc;
            using (Stream reader = this.WebProxy.GetStream(uri))
            {
                doc = XElement.Load(reader);
            }

            // クエリーエレメントを取得
            // ※ エレメントは常に1件
            XElement qe;
            try
            {
                qe = (from n in doc.Elements("query")
                      select n).First();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("parse failed : api/query element is not found");
            }

            // クエリーからページ情報を読み込み返す
            // ※ ページが無い場合などは、例外が投げられる
            return MediaWikiPage.GetFromQuery(this, uri, qe);
        }

        /// <summary>
        /// ページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したページ。</returns>
        /// <exception cref="InvalidDataException">APIから取得したデータが想定外。</exception>
        /// <exception cref="NullReferenceException">APIから取得したデータが想定外。</exception>
        /// <exception cref="FileNotFoundException">ページが存在しない場合。</exception>
        /// <remarks>ページの取得に失敗した場合（通信エラーなど）は、その状況に応じた例外を投げる。</remarks>
        public Page GetPageBodyAndTimestamp(string title)
        {
            // 記事データ取得用のURIを生成
            Uri uri = this.CreateUri(this.ContentApi, title);

            // ページのXMLデータをMediaWikiサーバーから取得
            XElement doc;
            using (Stream reader = this.WebProxy.GetStream(uri))
            {
                doc = XElement.Load(reader);
            }

            // ページエレメントを取得
            // ※ この問い合わせでは、ページが無い場合も要素自体は毎回ある模様
            //    一件しか返らないはずなので先頭データを対象とする
            XElement pe;
            try
            {
                pe = (from query in doc.Elements("query")
                      from pages in query.Elements("pages")
                      from n in pages.Elements("page")
                      select n).First();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("parse failed : query/pages/page element is not found");
            }

            // ページの解析
            if (pe.Attribute("missing") != null)
            {
                // missing属性が存在する場合、ページ無し
                throw new FileNotFoundException("page not found");
            }

            // ページ名、ページ本文、最終更新日時
            var re = (from revisions in pe.Elements("revisions")
                      from n in revisions.Elements("rev")
                      select n).First();

            // ページ情報を作成して返す
            return new MediaWikiPage(
                this,
                XmlUtils.Value(pe.Attribute("title"), title),
                re.Value,
                new DateTime?(DateTime.Parse(re.Attribute("timestamp").Value)),
                uri);
        }

        /// <summary>
        /// 指定された文字列がMediaWikiのシステム変数に相当かを判定。
        /// </summary>
        /// <param name="text">チェックする文字列。</param>
        /// <returns>システム変数に相当する場合<c>true</c>。</returns>
        /// <remarks>大文字小文字は区別する。</remarks>
        public bool IsMagicWord(string text)
        {
            // {{CURRENTYEAR}}や{{ns:1}}みたいなパターンがある
            string s = StringUtils.DefaultString(text);
            foreach (string variable in this.MagicWords)
            {
                if (s == variable || s.StartsWith(variable + ":"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 指定されたリンク文字列がMediaWikiのウィキ間リンクかを判定。
        /// </summary>
        /// <param name="link">チェックするリンク文字列。</param>
        /// <returns>ウィキ間リンクに該当する場合<c>true</c>。</returns>
        /// <remarks>大文字小文字は区別しない。</remarks>
        public bool IsInterwiki(string link)
        {
            // ※ ウィキ間リンクには入れ子もあるが、ここでは意識する必要はない
            string s = StringUtils.DefaultString(link);

            // 名前空間と被る場合はそちらが優先、ウィキ間リンクと判定しない
            if (this.IsNamespace(link))
            {
                return false;
            }

            // 文字列がいずれかのウィキ間リンクのプレフィックスで始まるか
            int index = s.IndexOf(':');
            if (index < 0)
            {
                return false;
            }

            return this.InterwikiPrefixs.Contains(s.Remove(index));
        }

        /// <summary>
        /// 指定されたリンク文字列がMediaWikiのいずれかの名前空間に属すかを判定。
        /// </summary>
        /// <param name="link">チェックするリンク文字列。</param>
        /// <returns>いずれかの名前空間に該当する場合<c>true</c>。</returns>
        /// <remarks>大文字小文字は区別しない。</remarks>
        public bool IsNamespace(string link)
        {
            // 文字列がいずれかの名前空間のプレフィックスで始まるか
            string s = StringUtils.DefaultString(link);
            int index = s.IndexOf(':');
            if (index < 0)
            {
                return false;
            }

            string prefix = s.Remove(index);
            foreach (IgnoreCaseSet prefixes in this.Namespaces.Values)
            {
                if (prefixes.Contains(prefix))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// <see cref="LinkInterwikiFormat"/> を渡された記事名, 言語, 他言語版記事名, 表示名で書式化した文字列を返す。
        /// </summary>
        /// <param name="title">記事名。</param>
        /// <param name="lang">言語。</param>
        /// <param name="langTitle">他言語版記事名。</param>
        /// <param name="label">表示名。</param>
        /// <returns>書式化した文字列。<see cref="LinkInterwikiFormat"/>が未設定の場合<c>null</c>。</returns>
        public string FormatLinkInterwiki(string title, string lang, string langTitle, string label)
        {
            if (string.IsNullOrEmpty(this.LinkInterwikiFormat))
            {
                return null;
            }

            return StringUtils.FormatDollarVariable(this.LinkInterwikiFormat, title, lang, langTitle, label);
        }

        /// <summary>
        /// <see cref="LangFormat"/> を渡された言語, 文字列で書式化した文字列を返す。
        /// </summary>
        /// <param name="lang">言語。</param>
        /// <param name="text">文字列。</param>
        /// <returns>書式化した文字列。<see cref="LangFormat"/>が未設定の場合<c>null</c>。</returns>
        /// <remarks>
        /// この<paramref name="lang"/>と<see cref="Language"/>のコードは、厳密には一致しないケースがあるが
        /// （例、simple→en）、2012年2月現在の実装ではそこまで正確さは要求していない。
        /// </remarks>
        public string FormatLang(string lang, string text)
        {
            if (string.IsNullOrEmpty(this.LangFormat))
            {
                return null;
            }

            return StringUtils.FormatDollarVariable(this.LangFormat, lang, text);
        }
        
        #endregion

        #region XMLシリアライズ用メソッド

        /// <summary>
        /// シリアライズするXMLのスキーマ定義を返す。
        /// </summary>
        /// <returns>XML表現を記述する<see cref="System.Xml.Schema.XmlSchema"/>。</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// XMLからオブジェクトをデシリアライズする。
        /// </summary>
        /// <param name="reader">デシリアライズ元の<see cref="XmlReader"/></param>
        public void ReadXml(XmlReader reader)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(reader);

            // Webサイト
            // ※ 以下、基本的に無かったらNGの部分はいちいちチェックしない。例外飛ばす
            XmlElement siteElement = xml.DocumentElement;
            this.Location = siteElement.SelectSingleNode("Location").InnerText;

            using (XmlReader r = XmlReader.Create(
                new StringReader(siteElement.SelectSingleNode("Language").OuterXml), reader.Settings))
            {
                this.Language = new XmlSerializer(typeof(Language)).Deserialize(r) as Language;
            }

            this.MetaApi = XmlUtils.InnerText(siteElement.SelectSingleNode("MetaApi"));
            this.ContentApi = XmlUtils.InnerText(siteElement.SelectSingleNode("ContentApi"));
            this.InterlanguageApi = XmlUtils.InnerText(siteElement.SelectSingleNode("InterlanguageApi"));

            int namespaceId;
            if (int.TryParse(XmlUtils.InnerText(siteElement.SelectSingleNode("TemplateNamespace")), out namespaceId))
            {
                this.TemplateNamespace = namespaceId;
            }

            if (int.TryParse(XmlUtils.InnerText(siteElement.SelectSingleNode("CategoryNamespace")), out namespaceId))
            {
                this.CategoryNamespace = namespaceId;
            }

            if (int.TryParse(XmlUtils.InnerText(siteElement.SelectSingleNode("FileNamespace")), out namespaceId))
            {
                this.FileNamespace = namespaceId;
            }

            // システム定義変数
            ISet<string> variables = new HashSet<string>();
            foreach (XmlNode variableNode in siteElement.SelectNodes("MagicWords/Variable"))
            {
                variables.Add(variableNode.InnerText);
            }

            if (variables.Count > 0)
            {
                // 初期値の都合上、値がある場合のみ
                this.MagicWords = variables;
            }

            // ウィキ間リンク
            IgnoreCaseSet prefixs = new IgnoreCaseSet();
            foreach (XmlNode prefixNode in siteElement.SelectNodes("InterwikiPrefixs/Prefix"))
            {
                prefixs.Add(prefixNode.InnerText);
            }

            if (prefixs.Count > 0)
            {
                // 初期値の都合上、値がある場合のみ
                this.InterwikiPrefixs = prefixs;
            }

            this.LinkInterwikiFormat = XmlUtils.InnerText(siteElement.SelectSingleNode("LinkInterwikiFormat"));
            this.LangFormat = XmlUtils.InnerText(siteElement.SelectSingleNode("LangFormat"));
            bool hasLanguagePage;
            if (bool.TryParse(XmlUtils.InnerText(siteElement.SelectSingleNode("HasLanguagePage")), out hasLanguagePage))
            {
                this.HasLanguagePage = hasLanguagePage;
            }
        }

        /// <summary>
        /// オブジェクトをXMLにシリアライズする。
        /// </summary>
        /// <param name="writer">シリアライズ先の<see cref="XmlWriter"/></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Location", this.Location);
            new XmlSerializer(this.Language.GetType()).Serialize(writer, this.Language);

            // MediaWiki固有の情報
            // ※ 設定ファイルに初期値を持つものは、プロパティではなく値から出力
            writer.WriteElementString("MetaApi", this.metaApi);
            writer.WriteElementString("ContentApi", this.contentApi);
            writer.WriteElementString("InterlanguageApi", this.interlanguageApi);
            writer.WriteElementString(
                "TemplateNamespace",
                this.templateNamespace.HasValue ? this.templateNamespace.ToString() : string.Empty);
            writer.WriteElementString(
                "CategoryNamespace",
                this.templateNamespace.HasValue ? this.categoryNamespace.ToString() : string.Empty);
            writer.WriteElementString(
                "FileNamespace",
                this.templateNamespace.HasValue ? this.fileNamespace.ToString() : string.Empty);

            // システム定義変数
            writer.WriteStartElement("MagicWords");
            if (this.magicWords != null)
            {
                foreach (string variable in this.magicWords)
                {
                    writer.WriteElementString("Variable", variable);
                }
            }

            // ウィキ間リンク
            writer.WriteEndElement();
            writer.WriteStartElement("InterwikiPrefixs");
            if (this.interwikiPrefixs != null)
            {
                foreach (string prefix in this.interwikiPrefixs)
                {
                    writer.WriteElementString("Prefix", prefix);
                }
            }

            writer.WriteEndElement();
            writer.WriteElementString("LinkInterwikiFormat", this.LinkInterwikiFormat);
            writer.WriteElementString("LangFormat", this.LangFormat);
            writer.WriteElementString("HasLanguagePage", this.HasLanguagePage.ToString());
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// <see cref="MetaApi"/>を使用してサーバーからメタ情報を取得する。
        /// </summary>
        /// <exception cref="System.Net.WebException">通信エラー等が発生した場合。</exception>
        /// <exception cref="InvalidDataException">APIから取得した情報が想定外のフォーマットの場合。</exception>
        private void InitializeByMetaApi()
        {
            // APIのXMLデータをMediaWikiサーバーから取得
            XmlDocument xml = new XmlDocument();
            using (Stream reader = this.WebProxy.GetStream(new Uri(new Uri(this.Location), this.MetaApi)))
            {
                xml.Load(reader);
            }

            // ルートエレメントまで取得し、フォーマットをチェック
            XmlElement rootElement = xml["api"];
            if (rootElement == null)
            {
                // XMLは取得できたが空 or フォーマットが想定外
                throw new InvalidDataException("parse failed : api element is not found");
            }

            // クエリーを取得
            XmlElement queryElement = rootElement["query"];
            if (queryElement == null)
            {
                // フォーマットが想定外
                throw new InvalidDataException("parse failed : query element is not found");
            }

            // クエリー内のネームスペース・ネームスペースエイリアス、ウィキ間リンクを読み込み
            this.namespaces = this.LoadNamespacesElement(queryElement);
            this.interwikiPrefixCaches = this.LoadInterwikimapElement(queryElement);

            // ウィキ間リンクは読み込んだ後に設定ファイルorプロパティの分をマージ
            // ※ 設定ファイルの初期値は下記より作成。
            //    http://svn.wikimedia.org/viewvc/mediawiki/trunk/phase3/maintenance/interwiki.sql?view=markup
            //    APIに加えて設定ファイルも持っているのは、2012年2月現在APIから返ってこない
            //    項目（wikipediaとかcommonsとか）が存在するため。
            this.interwikiPrefixCaches.UnionWith(
                this.interwikiPrefixs == null
                ? Settings.Default.MediaWikiInterwikiPrefixs.Cast<string>()
                : this.interwikiPrefixs);
        }

        /// <summary>
        /// <see cref="MetaApi"/>から取得したXMLのうち、ネームスペースに関する部分を読み込む。
        /// </summary>
        /// <param name="queryElement">APIから取得したXML要素のうち、api→query部分のエレメント。</param>
        /// <returns>読み込んだネームスペース情報。</returns>
        /// <exception cref="InvalidDataException">namespacesエレメントが存在しない場合。</exception>
        private IDictionary<int, IgnoreCaseSet> LoadNamespacesElement(XmlElement queryElement)
        {
            // ネームスペースブロックを取得、ネームスペースブロックまでは必須
            XmlElement namespacesElement = queryElement["namespaces"];
            if (namespacesElement == null)
            {
                // フォーマットが想定外
                throw new InvalidDataException("parse failed : namespaces element is not found");
            }

            // ネームスペースを取得
            IDictionary<int, IgnoreCaseSet> namespaces = new Dictionary<int, IgnoreCaseSet>();
            foreach (XmlNode node in namespacesElement.ChildNodes)
            {
                XmlElement namespaceElement = node as XmlElement;
                if (namespaceElement != null)
                {
                    try
                    {
                        int id = decimal.ToInt16(decimal.Parse(namespaceElement.GetAttribute("id")));
                        IgnoreCaseSet values = new IgnoreCaseSet();
                        values.Add(namespaceElement.InnerText);
                        namespaces[id] = values;

                        // あれば標準名も設定
                        string canonical = namespaceElement.GetAttribute("canonical");
                        if (!string.IsNullOrEmpty(canonical))
                        {
                            values.Add(canonical);
                        }
                    }
                    catch (Exception e)
                    {
                        // キャッチしているのは、万が一想定外の書式が返された場合に、完璧に動かなくなるのを防ぐため
                        System.Diagnostics.Debug.WriteLine("MediaWiki.LoadNamespacesElement > 例外発生 : " + e);
                    }
                }
            }

            // ネームスペースエイリアスブロックを取得、無い場合も想定
            XmlElement aliasesElement = queryElement["namespacealiases"];
            if (aliasesElement != null)
            {
                // ネームスペースエイリアスを取得
                foreach (XmlNode node in aliasesElement.ChildNodes)
                {
                    XmlElement namespaceElement = node as XmlElement;
                    if (namespaceElement != null)
                    {
                        try
                        {
                            int id = decimal.ToInt16(decimal.Parse(namespaceElement.GetAttribute("id")));
                            ISet<string> values = new HashSet<string>();
                            if (namespaces.ContainsKey(id))
                            {
                                values = namespaces[id];
                            }

                            values.Add(namespaceElement.InnerText);
                        }
                        catch (Exception e)
                        {
                            // キャッチしているのは、万が一想定外の書式が返された場合に、完璧に動かなくなるのを防ぐため
                            System.Diagnostics.Debug.WriteLine("MediaWiki.LoadNamespacesElement > 例外発生 : " + e);
                        }
                    }
                }
            }

            return namespaces;
        }

        /// <summary>
        /// <see cref="MetaApi"/>から取得したXMLのうち、ウィキ間リンクに関する部分を読み込む。
        /// </summary>
        /// <param name="queryElement">APIから取得したXML要素のうち、api→query部分のエレメント。</param>
        /// <returns>読み込んだウィキ間リンク情報。</returns>
        /// <exception cref="InvalidDataException">interwikimapエレメントが存在しない場合。</exception>
        private IgnoreCaseSet LoadInterwikimapElement(XmlElement queryElement)
        {
            // ウィキ間リンクブロックを取得、ウィキ間リンクブロックまでは必須
            XmlElement interwikimapElement = queryElement["interwikimap"];
            if (interwikimapElement == null)
            {
                // フォーマットが想定外
                throw new InvalidDataException("parse failed : interwikimap element is not found");
            }

            // ウィキ間リンクを取得
            IgnoreCaseSet interwikiPrefixs = new IgnoreCaseSet();
            foreach (XmlNode node in interwikimapElement.ChildNodes)
            {
                XmlElement interwikiElement = node as XmlElement;
                if (interwikiElement != null)
                {
                    string prefix = interwikiElement.GetAttribute("prefix");
                    if (!string.IsNullOrWhiteSpace(prefix))
                    {
                        interwikiPrefixs.Add(prefix);
                    }
                }
            }

            return interwikiPrefixs;
        }

        /// <summary>
        /// URI用のフォーマットと記事名からURIを生成する。
        /// </summary>
        /// <param name="format">URI用のフォーマット。</param>
        /// <param name="title">フォーマットに埋め込む記事名。</param>
        /// <returns>生成されたURI。</returns>
        /// <remarks>記事名は必要に応じてエスケープされる。</remarks>
        private Uri CreateUri(string format, string title)
        {
            // ※ 仕組み的な処理はWebsite側に置きたいが、向こうではタイトルだけを抽出できないので。
            //    Uriだけでも自動的にエスケープされるが、その場合+など一部の文字が対象にならなかったため
            //    明示的に記事名だけエスケープする。
            return new Uri(new Uri(this.Location), StringUtils.FormatDollarVariable(format, this.EscapeString(title)));
        }

        /// <summary>
        /// 接続に用いるスキームに応じて、文字列をエスケープする。
        /// </summary>
        /// <param name="str">エスケープする文字列。</param>
        /// <returns>エスケープされた文字列。</returns>
        private string EscapeString(string str)
        {
            if (new Uri(this.Location).IsFile)
            {
                // fileスキームの場合、ファイルで使えない文字をエスケープ
                return FormUtils.ReplaceInvalidFileNameChars(str);
            }
            else
            {
                // それ以外はhttp等のURL用のエスケープ
                return Uri.EscapeDataString(str);
            }
        }

        #endregion
    }
}
