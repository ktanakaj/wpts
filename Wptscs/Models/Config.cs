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
    using Honememo.Wptscs.Logics;
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
        protected Config()
        {
        }

        #endregion
        
        #region プロパティ

        /// <summary>
        /// 翻訳支援処理で使用するロジッククラス名。
        /// </summary>
        public Type Engine
        {
            get;
            set;
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
        /// 設定から、現在の処理対象・指定された言語のウェブサイトを取得する。
        /// </summary>
        /// <param name="lang">言語コード。</param>
        /// <returns>ウェブサイトの情報。存在しない場合は<c>null</c>返す。</returns>
        public Website GetWebsite(string lang)
        {
            // 設定が存在すれば取得した値を返す
            foreach (Website s in this.Websites)
            {
                if (s.Language.Code == lang)
                {
                    return s;
                }
            }

            // 存在しない場合、nullを返す
            return null;
        }
        
        /// <summary>
        /// 設定から、現在の処理対象・指定された言語の対訳表（項目）を取得する。
        /// </summary>
        /// <param name="from">翻訳元言語。</param>
        /// <param name="to">翻訳先言語。</param>
        /// <returns>対訳表の情報。存在しない場合は<c>null</c>返す。</returns>
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

            // 存在しない場合、nullを返す
            return null;
        }

        /// <summary>
        /// 設定から、現在の処理対象・指定された言語の対訳表（見出し）を取得する。
        /// </summary>
        /// <param name="from">翻訳元言語。</param>
        /// <param name="to">翻訳先言語。</param>
        /// <returns>対訳表の情報。存在しない場合は<c>null</c>返す。</returns>
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

            // 存在しない場合、nullを返す
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
            // ※ 以下、基本的に無かったらNGの部分はいちいちチェックしない。例外飛ばす
            XmlElement rootElement = xml.SelectSingleNode("/Config") as XmlElement;

            // ロジッククラス
            this.Engine = this.ParseEngine(rootElement.SelectSingleNode("Engine").InnerText);

            // Webサイト
            XmlSerializer serializer = null;
            XmlNode sitesNode = rootElement.SelectSingleNode("Websites");
            foreach (XmlNode siteNode in sitesNode.ChildNodes)
            {
                // ノードに指定された内容に応じたインスタンスを取得する
                this.Websites.Add(this.ParseWebsite(siteNode, reader.Settings));
            }

            // 対訳表
            serializer = new XmlSerializer(typeof(Translation));
            XmlNode itemsNode = rootElement.SelectSingleNode("ItemTables");
            foreach (XmlNode itemNode in itemsNode.ChildNodes)
            {
                using (XmlReader r = XmlReader.Create(new StringReader(itemNode.OuterXml), reader.Settings))
                {
                    this.ItemTables.Add(serializer.Deserialize(r) as Translation);
                }
            }

            // 対訳表
            XmlNode headingsNode = rootElement.SelectSingleNode("HeadingTables");
            foreach (XmlNode headingNode in headingsNode.ChildNodes)
            {
                using (XmlReader r = XmlReader.Create(new StringReader(headingNode.OuterXml), reader.Settings))
                {
                    this.HeadingTables.Add(serializer.Deserialize(r) as Translation);
                }
            }
        }

        /// <summary>
        /// オブジェクトをXMLに出力する。
        /// </summary>
        /// <param name="writer">出力先のXmlWriter</param>
        public void WriteXml(XmlWriter writer)
        {
            // ロジッククラス
            string engine = this.Engine.FullName;
            if (engine.StartsWith(typeof(Translate).Namespace))
            {
                // 自前のエンジンの場合、クラス名だけを出力
                engine = this.Engine.Name;
            }

            writer.WriteElementString("Engine", engine);

            // 各処理モードのWebサイト
            writer.WriteStartElement("Websites");
            foreach (Website site in this.Websites)
            {
                // TODO: 自分のパッケージ以外の場合、パッケージ名も含めて出すようにしたい
                new XmlSerializer(site.GetType()).Serialize(writer, site);
            }

            writer.WriteEndElement();

            // 項目の対訳表
            writer.WriteStartElement("ItemTables");
            foreach (Translation trans in this.ItemTables)
            {
                new XmlSerializer(trans.GetType()).Serialize(writer, trans);
            }

            writer.WriteEndElement();

            // 見出しの対訳表
            writer.WriteStartElement("HeadingTables");
            foreach (Translation trans in this.HeadingTables)
            {
                new XmlSerializer(trans.GetType()).Serialize(writer, trans);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// 指定されたXML値からEngineのクラスを取得するる。
        /// </summary>
        /// <param name="name">XMLのクラス名情報。</param>
        /// <returns>Engineクラス。</returns>
        /// <remarks>クラスは動的に判定する。クラスが存在しない場合などは随時状況に応じた例外を投げる。</remarks>
        private Type ParseEngine(string name)
        {
            // Translateと同じパッケージに指定された名前のクラスがあるかを探す
            Type type = Type.GetType(typeof(Translate).Namespace + "." + name, false, true);
            if (type == null)
            {
                // 存在しない場合、そのままの名前でクラスを探索、無ければ例外スロー
                type = Type.GetType(name, true, true);
            }

            return type;
        }

        /// <summary>
        /// XMLノードからWebSiteインスタンスをデシリアライズする。
        /// </summary>
        /// <param name="node">WebSiteをシリアライズしたノード。</param>
        /// <param name="setting">XML読み込み時の設定。</param>
        /// <returns>デシリアライズしたインスタンス。</returns>
        /// <remarks>クラスはノード名から動的に判定する。クラスが存在しない場合などは随時状況に応じた例外を投げる。</remarks>
        private Website ParseWebsite(XmlNode node, XmlReaderSettings setting)
        {
            // WebSiteと同じパッケージに指定された名前のクラスがあるかを探す
            Type type = Type.GetType(typeof(Website).Namespace + "." + node.Name, false, true);
            if (type == null)
            {
                // 存在しない場合、そのままの名前でクラスを探索、無ければ例外スロー
                type = Type.GetType(node.Name, true, true);
            }

            using (XmlReader r = XmlReader.Create(new StringReader(node.OuterXml), setting))
            {
                return new XmlSerializer(type).Deserialize(r) as Website;
            }
        }

        #endregion
    }
}
