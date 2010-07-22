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
        private static IDictionary<string, Config> configs = new Dictionary<string, Config>();

        #endregion

        #region private変数

        /// <summary>
        /// 言語に関する情報。
        /// </summary>
        private IList<Language> languages = new List<Language>();

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

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <remarks>通常は<see cref="GetInstance(string)"/>を使用する。</remarks>
        private Config()
        {
        }

        #endregion
        
        #region プロパティ

        /// <summary>
        /// 翻訳支援処理で使用するロジッククラス名。
        /// </summary>
        public string Engine
        {
            get;
            private set;
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
                // ※必須な情報が設定されていない場合、例外を返す
                this.languages = Validate.NotNull(value, "languages");
            }
        }

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
                // ※必須な情報が設定されていない場合、例外を返す
                this.websites = Validate.NotNull(value, "websites");
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
                // ※必須な情報が設定されていない場合、例外を返す
                this.itemTables = Validate.NotNull(value, "itemTables");
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
                // ※必須な情報が設定されていない場合、例外を返す
                this.headingTables = Validate.NotNull(value, "headingTables");
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
        /// <param name="file">設定ファイル名。</param>
        /// <returns>作成した／既に存在するインスタンス。</returns>
        public static Config GetInstance(string file)
        {
            // シングルトンとするため、処理をロック
            lock (configs)
            {
                // 既に作成済みのインスタンスがあればその値を使用
                // （設定ファイルのタイムスタンプとか確認して再読み込みした方がよい？）
                if (Config.configs.ContainsKey(file))
                {
                    return Config.configs[file];
                }

                // 無い場合はユーザーごと・または初期設定用の設定ファイルを読み込み
                string path = FormUtils.SearchUserAppData(file, Settings.Default.ConfigurationCompatible);
                if (String.IsNullOrEmpty(path))
                {
                    // どこにも無い場合は例外を投げる
                    // （空でnewしてもよいが、ユーザーが勘違いすると思うので。）
                    throw new FileNotFoundException(file + " is not found");
                }

                // 設定ファイルを読み込み
                System.Diagnostics.Debug.WriteLine("Config.GetInstance > " + path + " を読み込み");
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    Config.configs[file] = new XmlSerializer(typeof(Config)).Deserialize(stream) as Config;
                }
            }

            return Config.configs[file];
        }

        /// <summary>
        /// 指定された設定ファイルからアプリケーションの設定を読み込む。
        /// </summary>
        /// <param name="path">設定ファイルパス。</param>
        /// <remarks>主にテスト用。</remarks>
        public static void SetInstance(string path)
        {
            // 設定ファイルを読み込み
            System.Diagnostics.Debug.WriteLine("Config.SetInstance > " + path + " を読み込み");
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Config.configs[Path.GetFileName(path)] = new XmlSerializer(typeof(Config)).Deserialize(stream) as Config;
            }
        }

        #endregion

        #region インスタンスメソッド

        /// <summary>
        /// 設定をユーザーごとの設定ファイルに書き出し。
        /// </summary>
        /// <param name="file">設定ファイル名。</param>
        public void Save(string file)
        {
            // ファイル出力のため、競合しないよう一応ロック
            lock (Config.configs)
            {
                // 最初にディレクトリの有無を確認し作成
                string path = Application.UserAppDataPath;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // 設定ファイルを出力
                using (Stream stream = new FileStream(
                    Path.Combine(path, file),
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
        /// 設定から、現在の処理対象・指定された言語のウェブサイトを取得する。
        /// </summary>
        /// <param name="lang">言語コード。</param>
        /// <returns>ウェブサイトの情報。存在しない場合は可能であれば自動生成した情報を、できなければ<c>null</c>返す。</returns>
        public Website GetWebsite(string lang)
        {
            // 設定が存在すれば取得した値を返す
            foreach (Website s in this.Websites)
            {
                if (s.Language == lang)
                {
                    return s;
                }
            }

            // 存在しない場合自動生成した値を返す
            // （自動生成できない場合はnull）
            //TODO: 設定ファイルにクラス名も入れる
            Website site = null;
            if (mode == RunMode.Wikipedia)
            {
                // 現時点ではMediaWikiのみ対応
                site = new MediaWiki(lang);
                this.Websites.Add(site);
            }

            return site;
        }
        
        /// <summary>
        /// 設定から、現在の処理対象・指定された言語の対訳表（項目）を取得する。
        /// </summary>
        /// <param name="from">翻訳元言語。</param>
        /// <param name="to">翻訳先言語。</param>
        /// <returns>対訳表の情報。存在しない場合は自動生成した情報を返す。</returns>
        public Translation GetItemTable(string from, string to)
        {
            // 設定が存在すれば取得した値を返す
            foreach (Translation t in this.ItemTables)
            {
                if (t.From == from && t.To == to)
                {
                    return t;
                }
            }

            // 存在しない場合自動生成した値を返す
            // （自動生成できない場合はnull）
            Translation table = new Translation(from, to);
            this.ItemTables.Add(table);
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
            // 設定が存在すれば取得した値を返す
            foreach (Translation t in this.HeadingTables)
            {
                if (t.From == from && t.To == to)
                {
                    return t;
                }
            }

            // 存在しない場合自動生成した値を返す
            // （自動生成できない場合はnull）
            Translation table = new Translation(from, to);
            this.HeadingTables.Add(table);
            return table;
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

            //TODO: 要修正
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
    }
}
