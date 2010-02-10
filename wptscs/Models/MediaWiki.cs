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

        // ※各変数の初期値は、新しくサイトを登録する際の初期値として使用する。

        /// <summary>
        /// WikipediaのXMLの固定値の書式。
        /// </summary>
        private string xmlns = Settings.Default.MediaWikiXmlns;

        /// <summary>
        /// 名前空間情報取得用にアクセスするページ（存在不要）。
        /// </summary>
        private string dummyPage = Settings.Default.MediaWikiDummyPage;

        /// <summary>
        /// テンプレートの名前空間を示す番号。
        /// </summary>
        private int templateNamespace = Settings.Default.MediaWikiTemplateNamespace;

        /// <summary>
        /// カテゴリの名前空間を示す番号。
        /// </summary>
        private int categoryNamespace = Settings.Default.MediaWikiCategoryNamespace;

        /// <summary>
        /// 画像の名前空間を示す番号。
        /// </summary>
        private int fileNamespace = Settings.Default.MediaWikiFileNamespace;

        /// <summary>
        /// MediaWikiの名前空間の情報。
        /// </summary>
        private IDictionary<int, string> namespaces = new Dictionary<int, string>();

        /// <summary>
        /// 記事のXMLデータが存在するパス。
        /// </summary>
        private string exportPath = Settings.Default.MediaWikiExportPath;

        /// <summary>
        /// Wikipedia書式のシステム定義変数。
        /// </summary>
        /// <remarks>初期値は http://www.mediawiki.org/wiki/Help:Magic_words を参照</remarks>
        private IList<string> variables;

        /// <summary>
        /// 括弧のフォーマット。
        /// </summary>
        private string bracket = Settings.Default.MediaWikiBracket;

        /// <summary>
        /// リダイレクトの文字列。
        /// </summary>
        private string redirect = Settings.Default.MediaWikiRedirect;

        /// <summary>
        /// 見出しの定型句。
        /// </summary>
        private IDictionary<int, string> headings = new Dictionary<int, string>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ（MediaWiki全般）。
        /// </summary>
        /// <param name="lang">ウェブサイトの言語。</param>
        /// <param name="location">ウェブサイトの場所。</param>
        public MediaWiki(Language lang, string location)
        {
            // メンバ変数の初期設定
            this.Lang = lang;
            this.Location = location;

            // 処理的に変数宣言では入れられないのでここで初期化
            string[] variables = new string[Settings.Default.MediaWikiMagicWordsVariables.Count];
            Settings.Default.MediaWikiMagicWordsVariables.CopyTo(variables, 0);
            this.MagicWords = variables;
        }

        /// <summary>
        /// コンストラクタ（Wikipedia用）。
        /// </summary>
        /// <param name="lang">ウェブサイトの言語。</param>
        public MediaWiki(Language lang)
            : this(lang, String.Format(Settings.Default.WikipediaLocation, lang.Code))
        {
        }

        /// <summary>
        /// コンストラクタ（シリアライズ or 拡張用）。
        /// </summary>
        protected MediaWiki()
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// WikipediaのXMLの固定値の書式。
        /// </summary>
        public string Xmlns
        {
            get
            {
                return xmlns;
            }

            set
            {
                xmlns = value;
            }
        }

        /// <summary>
        /// 名前空間情報取得用にアクセスするページ（存在不要）。
        /// </summary>
        public string DummyPage
        {
            get
            {
                return dummyPage;
            }

            set
            {
                dummyPage = value;
            }
        }

        /// <summary>
        /// テンプレートの名前空間を示す番号。
        /// </summary>
        public int TemplateNamespace
        {
            get
            {
                return templateNamespace;
            }

            set
            {
                templateNamespace = value;
            }
        }

        /// <summary>
        /// カテゴリの名前空間を示す番号。
        /// </summary>
        public int CategoryNamespace
        {
            get
            {
                return categoryNamespace;
            }

            set
            {
                categoryNamespace = value;
            }
        }

        /// <summary>
        /// 画像の名前空間を示す番号。
        /// </summary>
        public int FileNamespace
        {
            get
            {
                return fileNamespace;
            }

            set
            {
                fileNamespace = value;
            }
        }

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
                    nsmgr.AddNamespace("ns", Xmlns);
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
        /// 記事のXMLデータが存在するパス。
        /// </summary>
        public string ExportPath
        {
            get
            {
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
                return this.redirect;
            }

            set
            {
                this.redirect = value;
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

        /// <summary>
        /// Wikipedia書式のシステム定義変数。
        /// </summary>
        public IList<string> MagicWords
        {
            get
            {
                return this.variables;
            }

            set
            {
                this.variables = value;
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
            nsmgr.AddNamespace("ns", Xmlns);
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
        /// 指定した言語での言語名称を ページ名|略称 の形式で取得。
        /// </summary>
        /// <param name="code">言語のコード。</param>
        /// <returns>ページ名|略称形式の言語名称。</returns>
        public string GetFullName(string code)
        {
            if (Lang.Names.ContainsKey(code))
            {
                Language.LanguageName name = Lang.Names[code];
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
        /// 指定された文字列がWikipediaのシステム変数に相当かを判定。
        /// </summary>
        /// <param name="text">チェックする文字列。</param>
        /// <returns><c>true</c> システム変数に相当。</returns>
        public bool ChkSystemVariable(string text)
        {
            string s = text != null ? text : String.Empty;

            // 基本は全文一致だが、定数が : で終わっている場合、textの:より前のみを比較
            // ※ {{ns:1}}みたいな場合に備えて
            foreach (string variable in this.MagicWords)
            {
                if (variable.EndsWith(":") == true)
                {
                    if (s.StartsWith(variable) == true)
                    {
                        return true;
                    }
                }
                else if (s == variable)
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

            // Webサイトの言語情報
            using (XmlReader r = XmlReader.Create(
                new StringReader(siteElement.SelectSingleNode("Language").OuterXml), reader.Settings))
            {
                this.Lang = new XmlSerializer(typeof(Language)).Deserialize(r) as Language;
            }

            this.Location = siteElement.SelectSingleNode("Location").InnerText;
            this.Xmlns = siteElement.SelectSingleNode("Xmlns").InnerText;
            this.ExportPath = siteElement.SelectSingleNode("ExportPath").InnerText;
            this.TemplateNamespace = int.Parse(siteElement.SelectSingleNode("TemplateNamespace").InnerText);
            this.CategoryNamespace = int.Parse(siteElement.SelectSingleNode("CategoryNamespace").InnerText);
            this.FileNamespace = int.Parse(siteElement.SelectSingleNode("FileNamespace").InnerText);
            this.DummyPage = siteElement.SelectSingleNode("DummyPage").InnerText;
            this.Bracket = siteElement.SelectSingleNode("Bracket").InnerText;
            this.Redirect = siteElement.SelectSingleNode("Redirect").InnerText;

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

            // Webサイトの言語情報
            new XmlSerializer(typeof(Language)).Serialize(writer, this.Lang);

            // MediaWiki固有の情報
            // ※ 定数値については、将来的に変数にする予定のため出力
            writer.WriteElementString("Xmlns", this.Xmlns);
            writer.WriteElementString("ExportPath", this.ExportPath);
            writer.WriteElementString("TemplateNamespace", this.TemplateNamespace.ToString());
            writer.WriteElementString("CategoryNamespace", this.CategoryNamespace.ToString());
            writer.WriteElementString("FileNamespace", this.FileNamespace.ToString());
            writer.WriteElementString("DummyPage", this.DummyPage);
            writer.WriteElementString("Bracket", this.Bracket);
            writer.WriteElementString("Redirect", this.Redirect);

            // システム定義変数
            writer.WriteStartElement("MagicWords");
            foreach (string variable in this.MagicWords)
            {
                writer.WriteElementString("Variable", variable);
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
