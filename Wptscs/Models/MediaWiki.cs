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
    using Honememo.Utilities;
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
        /// 名前空間情報取得用にアクセスするAPI。
        /// </summary>
        private string namespacePath;

        /// <summary>
        /// 記事のXMLデータが存在するパス。
        /// </summary>
        private string exportPath;

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
        private IDictionary<int, IList<string>> namespaces = new Dictionary<int, IList<string>>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ（MediaWiki全般）。
        /// </summary>
        /// <param name="language">ウェブサイトの言語。</param>
        /// <param name="location">ウェブサイトの場所。</param>
        public MediaWiki(Language language, string location)
        {
            // メンバ変数の初期設定
            this.Language = language;
            this.Location = location;
        }

        /// <summary>
        /// コンストラクタ（Wikipedia用）。
        /// </summary>
        /// <param name="language">ウェブサイトの言語。</param>
        public MediaWiki(Language language)
        {
            // メンバ変数の初期設定
            // ※ オーバーロードメソッドを呼んでいないのは、languageがnullのときに先にエラーになるから
            this.Language = language;
            this.Location = String.Format(Settings.Default.WikipediaLocation, language.Code);
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
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
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
        /// 名前空間情報取得用にアクセスするAPI。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
        public string NamespacePath
        {
            get
            {
                if (String.IsNullOrEmpty(this.namespacePath))
                {
                    return Settings.Default.MediaWikiNamespacePath;
                }

                return this.namespacePath;
            }

            set
            {
                this.namespacePath = value;
            }
        }

        /// <summary>
        /// 記事のXMLデータが存在するパス。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
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
        /// リダイレクトの文字列。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
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
        /// Wikipedia書式のシステム定義変数。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
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
        /// <remarks>値が指定されていない場合、サーバーから情報を取得。</remarks>
        public IDictionary<int, IList<string>> Namespaces
        {
            get
            {
                lock (this.namespaces)
                {
                    // 値が設定されていない場合、サーバーから取得して初期化する
                    // ※ コンストラクタ等で初期化していないのは、通信の準備が整うまで行えないため
                    // ※ MagicWordsがnullでこちらが空で若干条件が違うのは、あちらは設定ファイルに
                    //    保存する設定だが、こちらは設定ファイルに保存しない基本的に読み込み用の設定だから。
                    if (this.namespaces.Count > 0)
                    {
                        return this.namespaces;
                    }

                    // APIのXMLデータをMediaWikiサーバーから取得
                    XmlDocument xml = new XmlDocument();
                    using (Stream reader = this.GetStream(new Uri(new Uri(this.Location), this.NamespacePath)))
                    {
                        xml.Load(reader);
                    }

                    // ネームスペース情報を取得し、フォーマットをチェック
                    XmlElement namespacesNode = xml.SelectSingleNode("/api/query/namespaces") as XmlElement;
                    if (namespacesNode == null)
                    {
                        // XMLは取得できたが空 or フォーマットが想定外
                        throw new InvalidDataException("parse failed");
                    }

                    // ネームスペースを取得
                    foreach (XmlNode node in namespacesNode.SelectNodes("ns"))
                    {
                        XmlElement namespaceElement = node as XmlElement;
                        if (namespaceElement != null)
                        {
                            try
                            {
                                int id = Decimal.ToInt16(Decimal.Parse(namespaceElement.GetAttribute("id")));
                                IList<string> values = new List<string>();
                                values.Add(namespaceElement.InnerText);
                                this.namespaces[id] = values;

                                // あればシステム名？も設定
                                string canonical = namespaceElement.GetAttribute("canonical");
                                if (!String.IsNullOrEmpty(canonical))
                                {
                                    values.Add(canonical);
                                }
                            }
                            catch (Exception e)
                            {
                                // キャッチしているのは、万が一想定外の書式が返された場合に、完璧に動かなくなるのを防ぐため
                                System.Diagnostics.Debug.WriteLine("MediaWiki.Namespaces > 例外発生 : " + e);
                            }
                        }
                    }

                    // ネームスペースエイリアスを取得
                    XmlNodeList aliaseNodes = xml.SelectNodes("/api/query/namespacealiases/ns");
                    if (aliaseNodes != null)
                    {
                        foreach (XmlNode node in aliaseNodes)
                        {
                            XmlElement namespaceElement = node as XmlElement;
                            if (namespaceElement != null)
                            {
                                try
                                {
                                    int id = Decimal.ToInt16(Decimal.Parse(namespaceElement.GetAttribute("id")));
                                    IList<string> values = new List<string>();
                                    if (this.namespaces.ContainsKey(id))
                                    {
                                        values = this.namespaces[id];
                                    }

                                    values.Add(namespaceElement.InnerText);
                                }
                                catch (Exception e)
                                {
                                    // キャッチしているのは、万が一想定外の書式が返された場合に、完璧に動かなくなるのを防ぐため
                                    System.Diagnostics.Debug.WriteLine("MediaWiki.Namespaces > 例外発生 : " + e);
                                }
                            }
                        }
                    }
                }

                return this.namespaces;
            }

            set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (value == null)
                {
                    throw new ArgumentNullException("namespaces");
                }

                this.namespaces = value;
            }
        }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// ページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したページ。</returns>
        /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
        public override Page GetPage(string title)
        {
            // fileスキームの場合、記事名からファイルに使えない文字をエスケープ
            // ※ 仕組み的な処理はWebsite側に置きたいが、向こうではタイトルだけを抽出できないので
            string escapeTitle = title;
            if (new Uri(this.Location).Scheme == "file")
            {
                escapeTitle = FormUtils.ReplaceInvalidFileNameChars(title);
            }

            // ページのXMLデータをMediaWikiサーバーから取得
            XmlDocument xml = new XmlDocument();
            using (Stream reader = this.GetStream(
                new Uri(new Uri(this.Location), String.Format(this.ExportPath, escapeTitle))))
            {
                xml.Load(reader);
            }

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
                timeElement != null ? new DateTime?(DateTime.Parse(timeElement.InnerText)) : null);
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
            // ※ 以下、基本的に無かったらNGの部分はいちいちチェックしない。例外飛ばす
            XmlElement siteElement = xml.SelectSingleNode("MediaWiki") as XmlElement;
            this.Location = siteElement.SelectSingleNode("Location").InnerText;

            using (XmlReader r = XmlReader.Create(
                new StringReader(siteElement.SelectSingleNode("Language").OuterXml), reader.Settings))
            {
                this.Language = new XmlSerializer(typeof(Language)).Deserialize(r) as Language;
            }

            this.Xmlns = XmlUtils.InnerText(siteElement.SelectSingleNode("Xmlns"));
            this.NamespacePath = XmlUtils.InnerText(siteElement.SelectSingleNode("NamespacePath"));
            this.ExportPath = XmlUtils.InnerText(siteElement.SelectSingleNode("ExportPath"));
            this.Redirect = XmlUtils.InnerText(siteElement.SelectSingleNode("Redirect"));

            string text = XmlUtils.InnerText(siteElement.SelectSingleNode("TemplateNamespace"));
            if (!String.IsNullOrEmpty(text))
            {
                this.TemplateNamespace = int.Parse(text);
            }

            text = XmlUtils.InnerText(siteElement.SelectSingleNode("CategoryNamespace"));
            if (!String.IsNullOrEmpty(text))
            {
                this.CategoryNamespace = int.Parse(text);
            }

            text = XmlUtils.InnerText(siteElement.SelectSingleNode("FileNamespace"));
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
        }

        /// <summary>
        /// オブジェクトをXMLにシリアライズする。
        /// </summary>
        /// <param name="writer">シリアライズ先のXmlWriter</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Location", this.Location);
            new XmlSerializer(typeof(Language)).Serialize(writer, this.Language);

            // MediaWiki固有の情報
            // ※ 設定ファイルに初期値を持つものは、プロパティではなく値から出力
            writer.WriteElementString("Xmlns", this.xmlns);
            writer.WriteElementString("NamespacePath", this.namespacePath);
            writer.WriteElementString("ExportPath", this.exportPath);
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
        }

        #endregion
    }
}
