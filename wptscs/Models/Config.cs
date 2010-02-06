// ================================================================================================
// <summary>
//      アプリケーションの設定を保持するクラスソース</summary>
//
// <copyright file="Config.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// アプリケーションの設定を保持するクラスです。
    /// </summary>
    public class Config
    {
        #region 静的変数

        /// <summary>
        /// アプリケーション内でのインスタンス保持変数。
        /// </summary>
        private static Config config;

        /// <summary>
        /// <see cref="config"/>初期化時のロック用オブジェクト。
        /// </summary>
        private static object lockObj = new object();

        #endregion

        #region private変数

        /// <summary>
        /// プログラムの処理対象。
        /// </summary>
        private RunMode mode = RunMode.Wikipedia;

        /// <summary>
        /// ウェブサイト／言語の情報。
        /// </summary>
        private IDictionary<RunMode, IList<Website>> websites = new Dictionary<RunMode, IList<Website>>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <remarks>通常は<see cref="GetInstance()"/>を使用する。</remarks>
        private Config()
        {
        }

        #endregion

        #region 定数定義

        /// <summary>
        /// プログラムの処理モードを示す列挙値です。
        /// </summary>
        public enum RunMode
        {
            /// <summary>
            /// Wikipediaやその姉妹サイト等。
            /// </summary>
            /// <remarks>現時点ではWikipediaのみ。</remarks>
            Wikipedia
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// この言語の、各言語での名称。
        /// </summary>
        public RunMode Mode
        {
            get
            {
                return this.mode;
            }

            set
            {
                this.mode = value;
            }
        }

        /// <summary>
        /// ウェブサイト／言語の情報。
        /// </summary>
        public IDictionary<RunMode, IList<Website>> Websites
        {
            get
            {
                return this.websites;
            }

            set
            {
                this.websites = value;
            }
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// アプリケーションの設定を取得する。
        /// ユーザーごとの設定ファイルがあればその内容を、
        /// なければアプリケーション標準の設定ファイルの内容を
        /// 読み込んで、インスタンスを作成する。
        /// </summary>
        /// <returns>作成した／既に存在するインスタンス。</returns>
        public static Config GetInstance()
        {
            // シングルトンとするため、処理をロック
            // ※ 別オブジェクトを使っているのは、最初のnull時にロックできないため
            lock (lockObj)
            {
                // 既に作成済みのインスタンスがあればその値を使用
                if (Config.config != null)
                {
                    return Config.config;
                }

                // 無い場合はユーザーごとの設定ファイルを読み込み
                Config.config = GetInstance(Path.Combine(
                    Application.UserAppDataPath, Settings.Default.ConfigurationFile));
                if (Config.config != null)
                {
                    return Config.config;
                }

                // 無い場合は、exeと同じフォルダから初期設定ファイルを探索
                Config.config = GetInstance(Path.Combine(
                    Application.StartupPath, Settings.Default.ConfigurationFile));
                if (Config.config != null)
                {
                    return Config.config;
                }
            }

            // どちらにも無い場合は例外を投げる
            // （空でnewしてもよいが、ユーザーが勘違いすると思うので。）
            // Config.config = new Config();
            // return Config.config;
            throw new FileNotFoundException(Settings.Default.ConfigurationFile + " is not found");
        }

        #endregion

        #region インスタンスメソッド

        /// <summary>
        /// 設定をユーザーごとの設定ファイルに書き出し。
        /// </summary>
        public void Save()
        {
            // 設定ファイル名は決まっているため、ロック
            lock (Config.config)
            {
                // 最初にディレクトリの有無を確認し作成
                string path = Application.UserAppDataPath;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // 設定ファイルを出力
                using (Stream stream = new FileStream(
                    Path.Combine(path, Settings.Default.ConfigurationFile),
                    FileMode.Create))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        this.WriteXml(writer);
                    }
                }
            }
        }

        /// <summary>
        /// ウェブサイト／言語の情報から、現在の処理対象・指定された言語の情報を取得する。
        /// </summary>
        /// <param name="lang">言語コード。</param>
        /// <returns>ウェブサイト／言語の情報。存在しない場合は <c>null</c>。</returns>
        public Website GetWebsite(string lang)
        {
            if (this.websites.ContainsKey(this.Mode))
            {
                foreach (Website site in this.websites[this.Mode])
                {
                    if (site.Lang.Code == lang)
                    {
                        return site;
                    }
                }
            }

            return null;
        }

        #endregion

        #region 静的privateメソッド

        /// <summary>
        /// 指定されたファイルからアプリケーションの設定を取得する。
        /// </summary>
        /// <param name="path">設定ファイルパス</param>
        /// <returns>ファイルから作成したインスタンス。ファイルが存在しない場合は <c>null</c>。</returns>
        private static Config GetInstance(string path)
        {
            // ファイルが存在しない場合はnullを返す
            if (!File.Exists(path))
            {
                return null;
            }

            // 設定ファイルを読み込み
            Config config = new Config();
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    config.ReadXml(reader);
                }
            }

            return config;
        }

        #endregion

        #region XML入出力メソッド

        /// <summary>
        /// XMLからオブジェクトを読み込む。
        /// </summary>
        /// <param name="reader">読込元のXmlReader</param>
        private void ReadXml(XmlReader reader)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(reader);

            // ルートエレメントを取得
            XmlElement rootElement = xml.SelectSingleNode("/Config") as XmlElement;
            if (rootElement == null)
            {
                // ルートエレメントも無い場合は終了
                return;
            }

            // 処理モードごとにループ
            // ※ 以下、基本的に無かったらNGの部分はいちいちチェックしない。例外飛ばす
            foreach (XmlNode modeNode in rootElement.ChildNodes)
            {
                XmlElement modeElement = modeNode as XmlElement;
                if (modeElement == null)
                {
                    continue;
                }

                // 処理モード
                RunMode runMode;
                try
                {
                    runMode = (RunMode)Enum.Parse(typeof(RunMode), modeElement.Name);
                }
                catch (ArgumentException)
                {
                    System.Diagnostics.Debug.WriteLine("Config.ReadXml > 未対応の処理モード : " + modeElement.Name);
                    continue;
                }

                // 各処理モードのWebサイト
                IList<Website> sites = new List<Website>();
                XmlNode sitesNode = modeElement.SelectSingleNode("Websites");
                foreach (XmlNode siteNode in sitesNode.ChildNodes)
                {
                    // Webサイト
                    XmlElement siteElement = siteNode as XmlElement;
                    if (siteElement == null)
                    {
                        continue;
                    }

                    // Webサイトの言語情報
                    XmlElement langElement = siteElement.SelectSingleNode("Language") as XmlElement;
                    Language lang = new Language(langElement.GetAttribute("Code"));

                    // 言語の呼称情報
                    foreach (XmlNode nameNode in langElement.SelectNodes("Names/LanguageName"))
                    {
                        XmlElement nameElement = nameNode as XmlElement;
                        Language.LanguageName name = new Language.LanguageName();
                        XmlElement longNameElement = nameElement.SelectSingleNode("Name") as XmlElement;
                        if (longNameElement != null)
                        {
                            name.Name = longNameElement.InnerText;
                        }

                        XmlElement shortNameElement = nameElement.SelectSingleNode("ShortName") as XmlElement;
                        if (shortNameElement != null)
                        {
                            name.ShortName = shortNameElement.InnerText;
                        }

                        lang.Names[nameElement.GetAttribute("Code")] = name;
                    }

                    // Webサイトの種類に応じて取得
                    Website site = null;
                    switch (siteElement.Name)
                    {
                        case "MediaWiki":
                            site = new MediaWiki(lang);
                            break;
                    }

                    if (site == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Config.ReadXml > 未対応のWebサイト : " + siteElement.Name);
                        continue;
                    }

                    site.Location = siteElement.SelectSingleNode("Location").InnerText;

                    // MediaWiki時の情報
                    // ※ 基本的にMediaWikiのはず
                    MediaWiki wiki = site as MediaWiki;
                    if (wiki != null)
                    {
                        wiki.ExportPath = siteElement.SelectSingleNode("ExportPath").InnerText;
                        wiki.Bracket = siteElement.SelectSingleNode("Bracket").InnerText;
                        wiki.Redirect = siteElement.SelectSingleNode("Redirect").InnerText;

                        // システム定義変数
                        IList<string> variables = new List<string>();
                        foreach (XmlNode variableNode in siteElement.SelectNodes("SystemVariables/Variable"))
                        {
                            variables.Add(variableNode.InnerText);
                        }

                        wiki.SystemVariables = variables;

                        // 見出しの置き換えパターン
                        IDictionary<int, string> titleKeys = new Dictionary<int, string>();
                        foreach (XmlNode titleNode in siteElement.SelectNodes("TitleKeys/Title"))
                        {
                            XmlElement titleElement = titleNode as XmlElement;
                            titleKeys[int.Parse(titleElement.GetAttribute("no"))] = titleElement.InnerText;
                        }

                        wiki.TitleKeys = titleKeys;
                    }

                    sites.Add(site);
                }

                this.Websites[runMode] = sites;
            }
        }

        /// <summary>
        /// オブジェクトをXMLに出力する。
        /// </summary>
        /// <param name="writer">出力先のXmlWriter</param>
        private void WriteXml(XmlWriter writer)
        {
            // ルート
            writer.WriteStartElement("Config");

            // 処理モードごとにループ
            foreach (KeyValuePair<RunMode, IList<Website>> sites in this.Websites)
            {
                // 処理モード
                writer.WriteStartElement(sites.Key.ToString());

                // 各処理モードのWebサイト
                writer.WriteStartElement("Websites");
                foreach (Website site in sites.Value)
                {
                    // Webサイト
                    // ※ 基本的にMediaWikiのはず
                    MediaWiki wiki = site as MediaWiki;
                    writer.WriteStartElement(wiki != null ? "MediaWiki" : "Website");
                    writer.WriteElementString("Location", site.Location);

                    // Webサイトの言語情報
                    writer.WriteStartElement("Language");
                    writer.WriteAttributeString("Code", site.Lang.Code);

                    // 言語の呼称情報
                    writer.WriteStartElement("Names");
                    foreach (KeyValuePair<string, Language.LanguageName> name in site.Lang.Names)
                    {
                        writer.WriteStartElement("LanguageName");
                        writer.WriteAttributeString("Code", name.Key);
                        writer.WriteElementString("Name", name.Value.Name);
                        writer.WriteElementString("ShortName", name.Value.ShortName);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    // MediaWiki時の情報
                    if (wiki != null)
                    {
                        writer.WriteElementString("Xmlns", MediaWiki.Xmlns);
                        writer.WriteElementString("ExportPath", wiki.ExportPath);
                        writer.WriteElementString("TemplateNamespaceNumber", MediaWiki.TemplateNamespaceNumber.ToString());
                        writer.WriteElementString("CategoryNamespaceNumber", MediaWiki.CategoryNamespaceNumber.ToString());
                        writer.WriteElementString("ImageNamespaceNumber", MediaWiki.ImageNamespaceNumber.ToString());
                        writer.WriteElementString("DummyPage", MediaWiki.DummyPage);
                        writer.WriteElementString("Bracket", wiki.Bracket);
                        writer.WriteElementString("Redirect", wiki.Redirect);

                        // システム定義変数
                        writer.WriteStartElement("SystemVariables");
                        foreach (string variable in wiki.SystemVariables)
                        {
                            writer.WriteElementString("Variable", variable);
                        }

                        writer.WriteEndElement();

                        // 見出しの置き換えパターン
                        writer.WriteStartElement("TitleKeys");
                        foreach (KeyValuePair<int, string> title in wiki.TitleKeys)
                        {
                            writer.WriteStartElement("Title");
                            writer.WriteAttributeString("no", title.Key.ToString());
                            writer.WriteValue(title.Value);
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
