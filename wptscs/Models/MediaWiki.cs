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
    public class MediaWiki : Website
    {
        #region 定数定義

        // TODO: ここで定義している定数は、すべて可変のプロパティにする。
        //       （サーバーごとに変えたいかもしれないので。）

        /// <summary>
        /// WikipediaのXMLの固定値の書式。
        /// </summary>
        public static readonly string Xmlns = "http://www.mediawiki.org/xml/export-0.4/";

        /// <summary>
        /// 名前空間情報取得用にアクセスするページ（存在不要）。
        /// </summary>
        public static readonly string DummyPage = "Main_Page";

        /// <summary>
        /// テンプレートの名前空間を示す番号。
        /// </summary>
        public static readonly int TemplateNamespaceNumber = 10;

        /// <summary>
        /// カテゴリの名前空間を示す番号。
        /// </summary>
        public static readonly int CategoryNamespaceNumber = 14;

        /// <summary>
        /// 画像の名前空間を示す番号。
        /// </summary>
        public static readonly int ImageNamespaceNumber = 6;

        #endregion

        // ※各変数の初期値は2006年9月時点のWikipedia英語版より

        /// <summary>
        /// Wikipedia書式のシステム定義変数。
        /// </summary>
        [XmlArrayItem("Variable")]
        public string[] SystemVariables = new string[]
        {
                "CURRENTMONTH",
                "CURRENTMONTHNAME",
                "CURRENTDAY",
                "CURRENTDAYNAME",
                "CURRENTYEAR",
                "CURRENTTIME",
                "NUMBEROFARTICLES",
                "SITENAME",
                "SERVER",
                "NAMESPACE",
                "PAGENAME",
                "ns:",
                "localurl:",
                "fullurl:",
                "#if:"
        };

        /// <summary>
        /// 括弧のフォーマット。
        /// </summary>
        public string Bracket = " ({0}) ";

        /// <summary>
        /// リダイレクトの文字列。
        /// </summary>
        public string Redirect = "#REDIRECT";

        /// <summary>
        /// 見出しの定型句。
        /// </summary>
        [XmlArrayItem("Title")]
        public string[] TitleKeys = new string[0];

        #region private変数

        /// <summary>
        /// MediaWikiの名前空間の情報。
        /// </summary>
        [XmlIgnoreAttribute()]
        private IDictionary<int, string> namespaces;

        /// <summary>
        /// 記事のXMLデータが存在するパス。
        /// </summary>
        private string exportPath = "wiki/Special:Export/{0}";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ（シリアライズ用）。
        /// </summary>
        public MediaWiki()
            : this(new Language("unknown"))
        {
            // 適当な値で通常のコンストラクタを実行
            System.Diagnostics.Debug.WriteLine("MediaWiki.MediaWiki > 推奨されないコンストラクタを使用しています");
        }

        /// <summary>
        /// コンストラクタ（Wikipedia用）。
        /// </summary>
        /// <param name="lang">ウェブサイトの言語。</param>
        public MediaWiki(Language lang)
            : base(lang)
        {
            // メンバ変数の初期設定
            this.Location = String.Format(Settings.Default.WikipediaLocation, lang.Code);
        }

        #endregion

        #region プロパティ

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
                    if (this.namespaces != null)
                    {
                        return this.namespaces;
                    }

                    // 適当なページのXMLデータをMediaWikiサーバーから取得
                    XmlDocument xml = this.GetXml(MediaWiki.DummyPage);

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
                    this.namespaces = new Dictionary<int, string>();
                    XmlNodeList nodeList = rootElement.SelectNodes("ns:siteinfo/ns:namespaces/ns:namespace", nsmgr);
                    foreach (XmlNode node in nodeList)
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

        #endregion

        #region メソッド

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
            foreach (string variable in this.SystemVariables)
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
