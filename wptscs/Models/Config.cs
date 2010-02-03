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
    using System.Xml.Serialization;
    using Honememo.Utilities;
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
        private IDictionary<RunMode, IList<Website>> websites = new SerializableDictionary<RunMode, IList<Website>>();

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
            /// <remarks>MediaWikiのサイトすべてを共通にするかは要検討。</remarks>
            [XmlEnum(Name = "Wikipedia")]
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
            Config.config = new Config();
            return Config.config;
            // throw new FileNotFoundException(Settings.Default.ConfigurationFile + " is not found");
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

                // 設定ファイルをシリアライズ
                using (Stream stream = new FileStream(Path.Combine(
                    path, Settings.Default.ConfigurationFile), FileMode.Create))
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

            // 設定ファイルからデシリアライズ
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            using (Stream reader = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return serializer.Deserialize(reader) as Config;
            }
        }

        #endregion

        #region privateメソッド

        /// <summary>
        /// XMLからオブジェクトをデシリアライズする。
        /// </summary>
        /// <param name="reader">デシリアライズ元のXmlReader</param>
        private void ReadXml(XmlReader reader)
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(KeyValue));
            //reader.Read();
            //while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            //{
            //    KeyValue kv = serializer.Deserialize(reader) as KeyValue;
            //    if (kv != null)
            //    {
            //        Add(kv.Key, kv.Value);
            //    }
            //}

            //reader.Read();
        }

        /// <summary>
        /// オブジェクトをXMLにシリアライズする。
        /// </summary>
        /// <param name="writer">シリアライズ先のXmlWriter</param>
        private void WriteXml(XmlWriter writer)
        {
            XmlDocument xml = new XmlDocument();

            // ルート
            XmlNode rootElement = xml.CreateElement("Config");
            xml.AppendChild(rootElement);

            // 処理モード
            foreach (KeyValuePair<RunMode, IList<Website>> sites in this.Websites)
            {
                XmlNode sitesElement = xml.CreateElement("Websites");
                rootElement.AppendChild(sitesElement);
                XmlAttribute modeAttribute = xml.CreateAttribute("RunMode");
                modeAttribute.Value = sites.Key.ToString();
                sitesElement.Attributes.Append(modeAttribute);

                // 各処理モードのWebサイト
                foreach (Website site in sites.Value)
                {
                    XmlNode siteElement = xml.CreateElement("Website");
                    sitesElement.AppendChild(siteElement);
                    XmlNode locationNode = xml.CreateElement("Location");
                    locationNode.InnerText = site.Location;
                    siteElement.AppendChild(locationNode);

                    // 言語情報
                    XmlNode langElement = xml.CreateElement("Language");
                    siteElement.AppendChild(langElement);
                    XmlAttribute codeAttribute = xml.CreateAttribute("Code");
                    codeAttribute.Value = site.Lang.Code;
                    langElement.Attributes.Append(codeAttribute);

                    // 言語の呼称情報
                    XmlNode namesElement = xml.CreateElement("Names");
                    langElement.AppendChild(namesElement);
                    foreach (KeyValuePair<string, Language.LanguageName> name in site.Lang.Names)
                    {
                        XmlNode nameElement = xml.CreateElement("LanguageName");
                        namesElement.AppendChild(nameElement);
                        XmlNode longNameElement = xml.CreateElement("Name");
                        longNameElement.InnerText = name.Value.Name;
                        nameElement.AppendChild(longNameElement);
                        XmlNode shortNameElement = xml.CreateElement("ShortName");
                        shortNameElement.InnerText = name.Value.ShortName;
                        nameElement.AppendChild(shortNameElement);
                    }

                    // MediaWiki時の情報
                    if (site as MediaWiki != null)
                    {
                        MediaWiki wiki = site as MediaWiki;
                        XmlNode exportPathElement = xml.CreateElement("ExportPath");
                        exportPathElement.InnerText = wiki.ExportPath;
                        siteElement.AppendChild(exportPathElement);
                    }
                }
            }
            xml.Save(writer);
        }

        #endregion
    }
}
