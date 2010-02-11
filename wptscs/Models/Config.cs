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
    public class Config : IXmlSerializable
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
        /// 言語に関する情報。
        /// </summary>
        private IList<Language> languages = new List<Language>();

        /// <summary>
        /// 処理対象ごとの設定。
        /// </summary>
        private IDictionary<RunMode, ModeConfig> configs = new Dictionary<RunMode, ModeConfig>();

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
        /// プログラムの処理対象。
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
        /// 言語に関する情報。
        /// </summary>
        public IList<Language> Languages
        {
            get
            {
                return this.languages;
            }

            set
            {
                this.languages = value;
            }
        }

        /// <summary>
        /// 処理対象ごとの設定。
        /// </summary>
        public IDictionary<RunMode, ModeConfig> Configs
        {
            get
            {
                return this.configs;
            }

            set
            {
                this.configs = value;
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

                // 無い場合はユーザーごと・または初期設定用の設定ファイルを読み込み
                string path = FormUtils.SearchUserAppData(Settings.Default.ConfigurationFile, "0.80.0.0");
                if (String.IsNullOrEmpty(path))
                {
                    // どこにも無い場合は例外を投げる
                    // （空でnewしてもよいが、ユーザーが勘違いすると思うので。）
                    throw new FileNotFoundException(Settings.Default.ConfigurationFile + " is not found");
                }

                // 設定ファイルを読み込み
                System.Diagnostics.Debug.WriteLine("Config.GetInstance > " + path + " を読み込み");
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    Config.config = new XmlSerializer(typeof(Config)).Deserialize(stream) as Config;
                }
            }

            return Config.config;
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
                    new XmlSerializer(typeof(Config)).Serialize(stream, this);
                }
            }
        }

        /// <summary>
        /// ウェブサイトから、指定された言語の情報を取得する。
        /// </summary>
        /// <param name="code">言語コード。</param>
        /// <returns>言語の情報。存在しない場合は <c>null</c>。</returns>
        public Language GetLanguage(string code)
        {
            foreach (Language lang in this.Languages)
            {
                if (lang.Code == code)
                {
                    return lang;
                }
            }

            return null;
        }

        /// <summary>
        /// ウェブサイトから、現在の処理対象・指定された言語の情報を取得する。
        /// </summary>
        /// <param name="lang">言語コード。</param>
        /// <returns>ウェブサイトの情報。存在しない場合は <c>null</c>。</returns>
        public Website GetWebsite(string lang)
        {
            if (this.Configs.ContainsKey(this.Mode))
            {
                foreach (Website site in this.Configs[this.Mode].Websites)
                {
                    if (site.Language == lang)
                    {
                        return site;
                    }
                }
            }

            return null;
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
        /// XMLからオブジェクトを読み込む。
        /// </summary>
        /// <param name="reader">読込元のXmlReader</param>
        public void ReadXml(XmlReader reader)
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

            // 言語情報
            foreach (XmlNode langNode in rootElement.SelectNodes("Languages/Language"))
            {
                using (XmlReader r = XmlReader.Create(
                    new StringReader(langNode.OuterXml), reader.Settings))
                {
                    this.Languages.Add(new XmlSerializer(typeof(Language)).Deserialize(r) as Language);
                }
            }
            
            // 処理モードごとにループ
            // ※ 以下、基本的に無かったらNGの部分はいちいちチェックしない。例外飛ばす
            foreach (XmlNode modeNode in rootElement.ChildNodes)
            {
                XmlElement modeElement = modeNode as XmlElement;
                if (modeElement == null || modeElement.Name == "Languages")
                {
                    // エレメント以外や処理済の言語情報は除外
                    continue;
                }

                // 処理モード
                XmlSerializer serializer = null;
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
                ModeConfig config = new ModeConfig();
                XmlNode sitesNode = modeElement.SelectSingleNode("Websites");
                foreach (XmlNode siteNode in sitesNode.ChildNodes)
                {
                    // Webサイト
                    switch (siteNode.Name)
                    {
                        // 現時点ではMediaWikiのみ対応
                        case "MediaWiki":
                            serializer = new XmlSerializer(typeof(MediaWiki));
                            break;
                    }

                    if (serializer == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Config.ReadXml > 未対応のWebサイト : " + siteNode.Name);
                        continue;
                    }

                    using (XmlReader r = XmlReader.Create(new StringReader(siteNode.OuterXml), reader.Settings))
                    {
                        config.Websites.Add(serializer.Deserialize(r) as Website);
                    }
                }

                // 各処理モードの項目の対訳表
                serializer = new XmlSerializer(typeof(Translation));
                XmlNode itemsNode = modeElement.SelectSingleNode("ItemTables");
                foreach (XmlNode itemNode in itemsNode.ChildNodes)
                {
                    using (XmlReader r = XmlReader.Create(new StringReader(itemNode.OuterXml), reader.Settings))
                    {
                        config.ItemTables.Add(serializer.Deserialize(r) as Translation);
                    }
                }

                // 各処理モードの見出しの対訳表
                XmlNode headingsNode = modeElement.SelectSingleNode("HeadingTables");
                foreach (XmlNode headingNode in headingsNode.ChildNodes)
                {
                    using (XmlReader r = XmlReader.Create(new StringReader(headingNode.OuterXml), reader.Settings))
                    {
                        config.HeadingTables.Add(serializer.Deserialize(r) as Translation);
                    }
                }

                this.Configs[runMode] = config;
            }
        }

        /// <summary>
        /// オブジェクトをXMLに出力する。
        /// </summary>
        /// <param name="writer">出力先のXmlWriter</param>
        public void WriteXml(XmlWriter writer)
        {
            // 言語情報ごとにループ
            writer.WriteStartElement("Languages");
            foreach (Language lang in this.Languages)
            {
                new XmlSerializer(typeof(Language)).Serialize(writer, lang);
            }

            writer.WriteEndElement();

            // 処理モードごとにループ
            foreach (KeyValuePair<RunMode, ModeConfig> config in this.Configs)
            {
                // 処理モード
                writer.WriteStartElement(config.Key.ToString());

                // 各処理モードのWebサイト
                writer.WriteStartElement("Websites");
                foreach (Website site in config.Value.Websites)
                {
                    // 現時点ではMediaWikiのみ対応
                    if (site as MediaWiki != null)
                    {
                        new XmlSerializer(typeof(MediaWiki)).Serialize(writer, site);
                    }
                }

                writer.WriteEndElement();

                // 各処理モードの項目の対訳表
                writer.WriteStartElement("ItemTables");
                foreach (Translation trans in config.Value.ItemTables)
                {
                    new XmlSerializer(typeof(Translation)).Serialize(writer, trans);
                }

                writer.WriteEndElement();

                // 各処理モードの見出しの対訳表
                writer.WriteStartElement("HeadingTables");
                foreach (Translation trans in config.Value.HeadingTables)
                {
                    new XmlSerializer(typeof(Translation)).Serialize(writer, trans);
                }

                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion

        #region 内部クラス

        /// <summary>
        /// 処理対象ごとの設定を格納するための内部クラスです。
        /// </summary>
        public class ModeConfig
        {
            #region private変数

            /// <summary>
            /// ウェブサイトの情報。
            /// </summary>
            private IList<Website> websites = new List<Website>();

            /// <summary>
            /// 言語間の項目の対訳表。
            /// </summary>
            private IList<Translation> itemTables = new List<Translation>();

            /// <summary>
            /// 言語間の見出しの対訳表。
            /// </summary>
            private IList<Translation> headingTables = new List<Translation>();

            #endregion
            
            #region プロパティ

            /// <summary>
            /// ウェブサイトの情報。
            /// </summary>
            public IList<Website> Websites
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

            /// <summary>
            /// 言語間の項目の対訳表。
            /// </summary>
            public IList<Translation> ItemTables
            {
                get
                {
                    return this.itemTables;
                }

                set
                {
                    this.itemTables = value;
                }
            }

            /// <summary>
            /// 言語間の見出しの対訳表。
            /// </summary>
            public IList<Translation> HeadingTables
            {
                get
                {
                    return this.headingTables;
                }

                set
                {
                    this.headingTables = value;
                }
            }

            #endregion
        }

        #endregion
    }
}
