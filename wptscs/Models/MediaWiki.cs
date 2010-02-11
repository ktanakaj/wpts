// ================================================================================================
// <summary>
//      MediaWikiのウェブサイト（システム）をあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWiki.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// MediaWikiのウェブサイト（システム）をあらわすモデルクラスです。
    /// </summary>
    public class MediaWiki : Website, IXmlSerializable
    {
        #region private変数

        /// <summary>
        /// WikipediaのXMLの固定値の書式。
        /// </summary>
        private string xmlns;

        /// <summary>
        /// 名前空間情報取得用にアクセスするページ。
        /// </summary>
        /// <remarks>ページが存在する必要はない。</remarks>
        private string dummyPage;

        /// <summary>
        /// 記事のXMLデータが存在するパス。
        /// </summary>
        private string exportPath;

        /// <summary>
        /// 括弧のフォーマット。
        /// </summary>
        private string bracket;

        /// <summary>
        /// リダイレクトの文字列。
        /// </summary>
        private string redirect;

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
        /// Wikipedia書式のシステム定義変数。
        /// </summary>
        /// <remarks>初期値は http://www.mediawiki.org/wiki/Help:Magic_words を参照</remarks>
        private IList<string> magicWords;

        /// <summary>
        /// MediaWikiの名前空間の情報。
        /// </summary>
        private IDictionary<int, string> namespaces = new Dictionary<int, string>();

        /// <summary>
        /// 見出しの定型句。
        /// </summary>
        private IDictionary<int, string> headings = new Dictionary<int, string>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ（MediaWiki全般）。
        /// </summary>
        /// <param name="language">ウェブサイトの言語。</param>
        /// <param name="location">ウェブサイトの場所。</param>
        public MediaWiki(string language, string location)
        {
            // メンバ変数の初期設定
            this.Language = language;
            this.Location = location;
        }

        /// <summary>
        /// コンストラクタ（Wikipedia用）。
        /// </summary>
        /// <param name="language">ウェブサイトの言語。</param>
        public MediaWiki(string language)
            : this(language, String.Format(Settings.Default.WikipediaLocation, language))
        {
        }

        /// <summary>
        /// コンストラクタ（シリアライズ or 拡張用）。
        /// </summary>
        protected MediaWiki()
        {
        }

        #endregion

        #region 設定ファイルに初期値を持つプロパティ

        /// <summary>
        /// WikipediaのXMLの固定値の書式。
        /// </summary>
        public string Xmlns
        {
            get
            {
                if (String.IsNullOrEmpty(this.xmlns))
                {
                    return Settings.Default.MediaWikiXmlns;
                }

                return this.xmlns;
            }

            set
            {
                this.xmlns = value;
            }
        }

        /// <summary>
        /// 名前空間情報取得用にアクセスするページ。
        /// </summary>
        /// <remarks>ページが存在する必要はない。</remarks>
        public string DummyPage
        {
            get
            {
                if (String.IsNullOrEmpty(this.dummyPage))
                {
                    return Settings.Default.MediaWikiDummyPage;
                }

                return this.dummyPage;
            }

            set
            {
                this.dummyPage = value;
            }
        }

        /// <summary>
        /// 記事のXMLデータが存在するパス。
        /// </summary>
        public string ExportPath
        {
            get
            {
                if (String.IsNullOrEmpty(this.exportPath))
                {
                    return Settings.Default.MediaWikiExportPath;
                }

                return this.exportPath;
            }

            set
            {
                this.exportPath = value;
            }
        }

        /// <summary>
        /// 括弧のフォーマット。
        /// </summary>
        public string Bracket
        {
            get
            {
                if (String.IsNullOrEmpty(this.bracket))
                {
                    return Settings.Default.MediaWikiBracket;
                }

                return this.bracket;
            }

            set
            {
                this.bracket = value;
            }
        }

        /// <summary>
        /// リダイレクトの文字列。
        /// </summary>
        public string Redirect
        {
            get
            {
                if (String.IsNullOrEmpty(this.redirect))
                {
                    return Settings.Default.MediaWikiRedirect;
                }

                return this.redirect;
            }

            set
            {
                this.redirect = value;
            }
        }

        /// <summary>
        /// テンプレートの名前空間を示す番号。
        /// </summary>
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
        /// Wikipedia書式のシステム定義変数。
        /// </summary>
        public IList<string> MagicWords
        {
            get
            {
                if (this.magicWords == null)
                {
                    string[] magicWords = new string[Settings.Default.MediaWikiMagicWords.Count];
                    Settings.Default.MediaWikiMagicWords.CopyTo(magicWords, 0);
                    return magicWords;
                }

                return this.magicWords;
            }

            set
            {
                this.magicWords = value;
            }
        }

        #endregion

        #region それ以外のプロパティ

        /// <summary>
        /// MediaWikiの名前空間の情報。
        /// </summary>
        public IDictionary<int, string> Namespaces
        {
            get
            {
                lock (this.namespaces)
                {
                    // 値が設定されていない場合、サーバーから取得して初期化する
                    // ※ コンストラクタ等で初期化していないのは、通信の準備が整うまで行えないため
                    if (this.namespaces.Count > 0)
                    {
                        return this.namespaces;
                    }

                    // 適当なページのXMLデータをMediaWikiサーバーから取得
                    XmlDocument xml = this.GetXml(this.DummyPage);

                    // ルートエレメントまで取得し、フォーマットをチェック
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
                    nsmgr.AddNamespace("ns", this.Xmlns);
                    XmlElement rootElement = xml.SelectSingleNode("/ns:mediawiki", nsmgr) as XmlElement;
                    if (rootElement == null)
                    {
                        // XMLは取得できたが空 or フォーマットが想定外
                        throw new InvalidDataException("parse failed");
                    }

                    // ネームスペースを取得
                    foreach (XmlNode node in rootElement.SelectNodes("ns:siteinfo/ns:namespaces/ns:namespace", nsmgr))
                    {
                        XmlElement namespaceElement = node as XmlElement;
                        if (namespaceElement != null)
                        {
                            try
                            {
                                this.namespaces[Decimal.ToInt16(Decimal.Parse(namespaceElement.GetAttribute("key")))] = namespaceElement.InnerText;
                            }
                            catch (Exception e)
                            {
                                // キャッチしているのは、万が一想定外の書式が返された場合に、完璧に動かなくなるのを防ぐため
                                System.Diagnostics.Debug.WriteLine("MediaWiki.Namespaces > 例外発生 : " + e);
                            }
                        }
                    }
                }

                return this.namespaces;
            }

            set
            {
                this.namespaces = value;
            }
        }

        /// <summary>
        /// 見出しの定型句。
        /// </summary>
        public IDictionary<int, string> Headings
        {
            get
            {
                return this.headings;
            }

            set
            {
                this.headings = value;
            }
        }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// ページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したページ。ページが存在しない場合は <c>null</c> を返す。</returns>
        /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
        public override Page GetPage(string title)
        {
            // ページのXMLデータをMediaWikiサーバーから取得
            XmlDocument xml = this.GetXml(title);

            // ルートエレメントまで取得し、フォーマットをチェック
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("ns", this.Xmlns);
            XmlElement rootElement = xml.SelectSingleNode("/ns:mediawiki", nsmgr) as XmlElement;
            if (rootElement == null)
            {
                // XMLは取得できたが空 or フォーマットが想定外
                throw new InvalidDataException("parse failed");
            }

            // ページの解析
            XmlElement pageElement = rootElement.SelectSingleNode("ns:page", nsmgr) as XmlElement;
            if (pageElement == null)
            {
                // ページ無し
                throw new FileNotFoundException("page not found");
            }

            // ページ名、ページ本文、最終更新日時
            XmlElement titleElement = pageElement.SelectSingleNode("ns:title", nsmgr) as XmlElement;
            XmlElement textElement = pageElement.SelectSingleNode("ns:revision/ns:text", nsmgr) as XmlElement;
            XmlElement timeElement = pageElement.SelectSingleNode("ns:revision/ns:timestamp", nsmgr) as XmlElement;

            // ページ情報を作成して返す
            // ※ 一応、各項目が無くても動作するようにする
            return new MediaWikiPage(
                this,
                titleElement != null ? titleElement.InnerText : title,
                textElement != null ? textElement.InnerText : null,
                timeElement != null ? DateTime.Parse(timeElement.InnerText) : DateTime.UtcNow);
        }

        /// <summary>
        /// 指定された文字列がWikipediaのシステム変数に相当かを判定。
        /// </summary>
        /// <param name="text">チェックする文字列。</param>
        /// <returns><c>true</c> システム変数に相当。</returns>
        public bool IsMagicWord(string text)
        {
            string s = text != null ? text : String.Empty;

            // {{CURRENTYEAR}}や{{ns:1}}みたいなパターンがある
            foreach (string variable in this.MagicWords)
            {
                if (s == variable || s.StartsWith(variable + ":"))
                {
                    return true;
                }
            }

            return false;
        }
        
        #endregion

        #region XMLシリアライズ用メソッド

        /// <summary>
        /// シリアライズするXMLのスキーマ定義を返す。
        /// </summary>
        /// <returns>XML表現を記述するXmlSchema。</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// XMLからオブジェクトをデシリアライズする。
        /// </summary>
        /// <param name="reader">デシリアライズ元のXmlReader</param>
        public void ReadXml(XmlReader reader)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(reader);

            // Webサイト
            XmlElement siteElement = xml.SelectSingleNode("MediaWiki") as XmlElement;
            if (siteElement == null)
            {
                return;
            }

            this.Location = siteElement.SelectSingleNode("Location").InnerText;
            this.Language = siteElement.SelectSingleNode("Language").InnerText;
            this.Xmlns = siteElement.SelectSingleNode("Xmlns").InnerText;
            this.ExportPath = siteElement.SelectSingleNode("ExportPath").InnerText;
            this.DummyPage = siteElement.SelectSingleNode("DummyPage").InnerText;
            this.Bracket = siteElement.SelectSingleNode("Bracket").InnerText;
            this.Redirect = siteElement.SelectSingleNode("Redirect").InnerText;

            string text = siteElement.SelectSingleNode("TemplateNamespace").InnerText;
            if (!String.IsNullOrEmpty(text))
            {
                this.TemplateNamespace = int.Parse(text);
            }

            text = siteElement.SelectSingleNode("CategoryNamespace").InnerText;
            if (!String.IsNullOrEmpty(text))
            {
                this.CategoryNamespace = int.Parse(text);
            }

            text = siteElement.SelectSingleNode("FileNamespace").InnerText;
            if (!String.IsNullOrEmpty(text))
            {
                this.FileNamespace = int.Parse(text);
            }

            // システム定義変数
            IList<string> variables = new List<string>();
            foreach (XmlNode variableNode in siteElement.SelectNodes("MagicWords/Variable"))
            {
                variables.Add(variableNode.InnerText);
            }

            this.MagicWords = variables;

            // 見出しの置き換えパターン
            IDictionary<int, string> headings = new Dictionary<int, string>();
            foreach (XmlNode headingNode in siteElement.SelectNodes("Headings/Heading"))
            {
                XmlElement headingElement = headingNode as XmlElement;
                headings[int.Parse(headingElement.GetAttribute("no"))] = headingElement.InnerText;
            }

            this.Headings = headings;
        }

        /// <summary>
        /// オブジェクトをXMLにシリアライズする。
        /// </summary>
        /// <param name="writer">シリアライズ先のXmlWriter</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Location", this.Location);
            writer.WriteElementString("Language", this.Language);

            // MediaWiki固有の情報
            // ※ 設定ファイルに初期値を持つものは、プロパティではなく値から出力
            writer.WriteElementString("Xmlns", this.xmlns);
            writer.WriteElementString("DummyPage", this.dummyPage);
            writer.WriteElementString("ExportPath", this.exportPath);
            writer.WriteElementString("Bracket", this.bracket);
            writer.WriteElementString("Redirect", this.redirect);
            writer.WriteElementString(
                "TemplateNamespace",
                this.templateNamespace.HasValue ? this.templateNamespace.ToString() : String.Empty);
            writer.WriteElementString(
                "CategoryNamespace",
                this.templateNamespace.HasValue ? this.categoryNamespace.ToString() : String.Empty);
            writer.WriteElementString(
                "FileNamespace",
                this.templateNamespace.HasValue ? this.fileNamespace.ToString() : String.Empty);

            // システム定義変数
            writer.WriteStartElement("MagicWords");
            if (this.magicWords != null)
            {
                foreach (string variable in this.magicWords)
                {
                    writer.WriteElementString("Variable", variable);
                }
            }

            writer.WriteEndElement();

            // 見出しの置き換えパターン
            writer.WriteStartElement("Headings");
            foreach (KeyValuePair<int, string> heading in this.Headings)
            {
                writer.WriteStartElement("Heading");
                writer.WriteAttributeString("no", heading.Key.ToString());
                writer.WriteValue(heading.Value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion

        #region 内部処理メソッド

        /// <summary>
        /// MediaWikiから指定されたページのXMLを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したXML。</returns>
        /// <remarks>
        /// 取得できない場合（通信エラーなど）は例外を投げる。
        /// ただし取得したXMLのチェックはしないので、ページ情報等が入っていない可能性がある。
        /// </remarks>
        protected XmlDocument GetXml(string title)
        {
            // ページのXMLデータをMediaWikiサーバーから取得
            XmlDocument xml = new XmlDocument();
            UriBuilder uri = new UriBuilder(this.Location);
            uri.Path = String.Format(this.exportPath, title);
            using (Stream reader = this.GetStream(uri.Uri))
            {
                xml.Load(reader);
            }

            return xml;
        }

        #endregion
    }
}
