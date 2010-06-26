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
        /// <remarks>空でもオブジェクトは存在。</remarks>
        public IList<Language> Languages
        {
            get
            {
                return this.languages;
            }

            set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (value == null)
                {
                    throw new ArgumentNullException("languages");
                }

                this.languages = value;
            }
        }

        /// <summary>
        /// 処理対象ごとの設定。
        /// </summary>
        /// <remarks>空でもオブジェクトは存在。</remarks>
        public IDictionary<RunMode, ModeConfig> Configs
        {
            get
            {
                return this.configs;
            }

            set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (value == null)
                {
                    throw new ArgumentNullException("configs");
                }

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
                // （設定ファイルのタイムスタンプとか確認して再読み込みした方がよい？）
                if (Config.config != null)
                {
                    return Config.config;
                }

                // 無い場合はユーザーごと・または初期設定用の設定ファイルを読み込み
                string path = FormUtils.SearchUserAppData(
                    Settings.Default.ConfigurationFile,
                    Settings.Default.ConfigurationCompatible);
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
        
        #endregion

        #region 設定値取得用インスタンスメソッド

        /// <summary>
        /// 設定から、指定された言語の情報を取得する。
        /// </summary>
        /// <param name="code">言語コード。</param>
        /// <returns>言語の情報。存在しない場合は自動生成した情報を返す。</returns>
        public Language GetLanguage(string code)
        {
            // 設定が存在すれば取得した値を返す
            foreach (Language l in this.Languages)
            {
                if (l.Code == code)
                {
                    return l;
                }
            }

            // 存在しない場合自動生成した値を返す
            Language lang = new Language(code);
            this.Languages.Add(lang);
            return lang;
        }

        /// <summary>
        /// 設定から、指定された処理対象ごとの設定情報を取得する。
        /// </summary>
        /// <param name="mode">プログラムの処理対象。</param>
        /// <returns>処理対象ごとの設定情報。存在しない場合は可能であれば自動生成した情報を返す。</returns>
        public ModeConfig GetModeConfig(RunMode mode)
        {
            // 設定が存在すれば取得した値を返す
            if (this.Configs.ContainsKey(mode))
            {
                return this.Configs[mode];
            }

            // 存在しない場合自動生成した値を返す
            ModeConfig config = new ModeConfig();
            this.Configs[mode] = config;
            return config;
        }

        /// <summary>
        /// 設定から、現在の処理対象ごとの設定情報を取得する。
        /// </summary>
        /// <returns>処理対象ごとの設定情報。存在しない場合は可能であれば自動生成した情報を返す。</returns>
        public ModeConfig GetModeConfig()
        {
            // 現在の処理対象から、オーバーロードメソッドをコール
            return this.GetModeConfig(this.Mode);
        }

        /// <summary>
        /// 設定から、指定された処理対象・言語のウェブサイトを取得する。
        /// </summary>
        /// <param name="mode">プログラムの処理対象。</param>
        /// <param name="lang">言語コード。</param>
        /// <returns>ウェブサイトの情報。存在しない場合は可能であれば自動生成した情報を、できなければ<c>null</c>返す。</returns>
        public Website GetWebsite(RunMode mode, string lang)
        {
            // 設定が存在すれば取得した値を返す
            ModeConfig config = this.GetModeConfig(mode);
            foreach (Website s in config.Websites)
            {
                if (s.Language == lang)
                {
                    return s;
                }
            }

            // 存在しない場合自動生成した値を返す
            // （自動生成できない場合はnull）
            Website site = null;
            if (mode == RunMode.Wikipedia)
            {
                // 現時点ではMediaWikiのみ対応
                site = new MediaWiki(lang);
                config.Websites.Add(site);
            }

            return site;
        }

        /// <summary>
        /// 設定から、現在の処理対象・指定された言語のウェブサイトを取得する。
        /// </summary>
        /// <param name="lang">言語コード。</param>
        /// <returns>ウェブサイトの情報。存在しない場合は可能であれば自動生成した情報を、できなければ<c>null</c>返す。</returns>
        public Website GetWebsite(string lang)
        {
            // 現在の処理対象から、オーバーロードメソッドをコール
            return this.GetWebsite(this.Mode, lang);
        }

        /// <summary>
        /// 設定から、指定された処理対象・言語の対訳表（項目）を取得する。
        /// </summary>
        /// <param name="mode">プログラムの処理対象。</param>
        /// <param name="from">翻訳元言語。</param>
        /// <param name="to">翻訳先言語。</param>
        /// <returns>対訳表の情報。存在しない場合は自動生成した情報を返す。</returns>
        public Translation GetItemTable(RunMode mode, string from, string to)
        {
            // 設定が存在すれば取得した値を返す
            ModeConfig config = this.GetModeConfig(mode);
            foreach (Translation t in config.ItemTables)
            {
                if (t.From == from && t.To == to)
                {
                    return t;
                }
            }

            // 存在しない場合自動生成した値を返す
            // （自動生成できない場合はnull）
            Translation table = new Translation(from, to);
            config.ItemTables.Add(table);
            return table;
        }

        /// <summary>
        /// 設定から、現在の処理対象・指定された言語の対訳表（項目）を取得する。
        /// </summary>
        /// <param name="from">翻訳元言語。</param>
        /// <param name="to">翻訳先言語。</param>
        /// <returns>対訳表の情報。存在しない場合は自動生成した情報を返す。</returns>
        public Translation GetItemTable(string from, string to)
        {
            // 現在の処理対象から、オーバーロードメソッドをコール
            return this.GetItemTable(this.Mode, from, to);
        }

        /// <summary>
        /// 設定から、指定された処理対象・言語の対訳表（見出し）を取得する。
        /// </summary>
        /// <param name="mode">プログラムの処理対象。</param>
        /// <param name="from">翻訳元言語。</param>
        /// <param name="to">翻訳先言語。</param>
        /// <returns>対訳表の情報。存在しない場合は自動生成した情報を返す。</returns>
        public Translation GetHeadingTable(RunMode mode, string from, string to)
        {
            // 設定が存在すれば取得した値を返す
            ModeConfig config = this.GetModeConfig(mode);
            foreach (Translation t in config.HeadingTables)
            {
                if (t.From == from && t.To == to)
                {
                    return t;
                }
            }

            // 存在しない場合自動生成した値を返す
            // （自動生成できない場合はnull）
            Translation table = new Translation(from, to);
            config.HeadingTables.Add(table);
            return table;
        }

        /// <summary>
        /// 設定から、現在の処理対象・指定された言語の対訳表（見出し）を取得する。
        /// </summary>
        /// <param name="from">翻訳元言語。</param>
        /// <param name="to">翻訳先言語。</param>
        /// <returns>対訳表の情報。存在しない場合は自動生成した情報を返す。</returns>
        public Translation GetHeadingTable(string from, string to)
        {
            // 現在の処理対象から、オーバーロードメソッドをコール
            return this.GetHeadingTable(this.Mode, from, to);
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
            // ※ 以下、基本的に無かったらNGの部分はいちいちチェックしない。例外飛ばす
            XmlElement rootElement = xml.SelectSingleNode("/Config") as XmlElement;

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
            /// <remarks>空でもオブジェクトは存在。</remarks>
            public IList<Website> Websites
            {
                get
                {
                    return this.websites;
                }

                set
                {
                    // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                    if (value == null)
                    {
                        throw new ArgumentNullException("websites");
                    }

                    this.websites = value;
                }
            }

            /// <summary>
            /// 言語間の項目の対訳表。
            /// </summary>
            /// <remarks>空でもオブジェクトは存在。</remarks>
            public IList<Translation> ItemTables
            {
                get
                {
                    return this.itemTables;
                }

                set
                {
                    // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                    if (value == null)
                    {
                        throw new ArgumentNullException("itemTables");
                    }

                    this.itemTables = value;
                }
            }

            /// <summary>
            /// 言語間の見出しの対訳表。
            /// </summary>
            /// <remarks>空でもオブジェクトは存在。</remarks>
            public IList<Translation> HeadingTables
            {
                get
                {
                    return this.headingTables;
                }

                set
                {
                    // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                    if (value == null)
                    {
                        throw new ArgumentNullException("headingTables");
                    }

                    this.headingTables = value;
                }
            }

            #endregion
        }

        #endregion
    }
}
