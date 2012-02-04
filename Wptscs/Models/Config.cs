// ================================================================================================
// <summary>
//      アプリケーションの設定を保持するクラスソース</summary>
//
// <copyright file="Config.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
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
    using Honememo.Wptscs.Utilities;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// アプリケーションの設定を保持するクラスです。
    /// </summary>
    public class Config : IXmlSerializable
    {
        #region private変数

        /// <summary>
        /// ウェブサイトの情報。
        /// </summary>
        private IList<Website> websites = new List<Website>();

        /// <summary>
        /// 言語間の項目の対訳表。
        /// </summary>
        private IList<TranslationDictionary> itemTables = new List<TranslationDictionary>();

        /// <summary>
        /// 言語間の見出しの対訳表。
        /// </summary>
        private TranslationTable headingTable = new TranslationTable();

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
        /// 設定ファイルと紐付いている場合のファイル名。
        /// </summary>
        /// <remarks>ファイルと紐付いていない場合は空。</remarks>
        public string File
        {
            get;
            set;
        }

        /// <summary>
        /// 翻訳支援処理で使用するロジッククラス名。
        /// </summary>
        public Type Translator
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
        public IList<TranslationDictionary> ItemTables
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
        public TranslationTable HeadingTable
        {
            get
            {
                return this.headingTable;
            }

            set
            {
                // ※必須な情報が設定されていない場合、例外を返す
                this.headingTable = Validate.NotNull(value, "headingTable");
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
        /// <returns>作成したインスタンス。</returns>
        public static Config GetInstance(string file)
        {
            // ユーザーごと・または初期設定用の設定ファイルを読み込み
            string path = FormUtils.SearchUserAppData(file, Settings.Default.ConfigurationCompatible);
            if (String.IsNullOrEmpty(path))
            {
                // どこにも無い場合は例外を投げる
                // （空でnewしてもよいが、ユーザーが勘違いすると思うので。）
                throw new FileNotFoundException(file + " is not found");
            }

            // 設定ファイルを読み込み、読み込み元のファイル名は記録しておく
            System.Diagnostics.Debug.WriteLine("Config.GetInstance > " + path + " を読み込み");
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Config config = new XmlSerializer(typeof(Config)).Deserialize(stream) as Config;
                config.File = file;
                return config;
            }
        }

        #endregion

        #region インスタンスメソッド

        /// <summary>
        /// 設定をユーザーごとの設定ファイルに書き出し。
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="File"/>にこのインスタンスと紐付くファイル名が指定されていない場合。
        /// </exception>
        public void Save()
        {
            // このインスタンスとファイルが紐付いていない場合、実行不可
            if (String.IsNullOrWhiteSpace(this.File))
            {
                throw new InvalidOperationException("file is empty");
            }

            // 最初にディレクトリの有無を確認し作成
            string path = Application.UserAppDataPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // 設定ファイルを出力
            using (Stream stream = new FileStream(Path.Combine(path, this.File), FileMode.Create))
            {
                new XmlSerializer(typeof(Config)).Serialize(stream, this);
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
            // ※ こっちがNeedCreateじゃないのは、何をnewすればいいのか判らないため
            return null;
        }
        
        /// <summary>
        /// 設定から、現在の処理対象・指定された言語の対訳表（項目）を取得する。
        /// </summary>
        /// <param name="from">翻訳元言語。</param>
        /// <param name="to">翻訳先言語。</param>
        /// <returns>対訳表の情報。存在しない場合は新たに作成した対訳表を返す。</returns>
        public TranslationDictionary GetItemTableNeedCreate(string from, string to)
        {
            // オブジェクトに用意されている共通メソッドをコール
            return TranslationDictionary.GetDictionaryNeedCreate(this.ItemTables, from, to);
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
            XmlElement rootElement = xml.DocumentElement;

            // ロジッククラス
            this.Translator = this.ParseTranslator(rootElement.SelectSingleNode("Translator").InnerText);

            // Webサイト
            foreach (XmlNode siteNode in rootElement.SelectSingleNode("Websites").ChildNodes)
            {
                // ノードに指定された内容に応じたインスタンスを取得する
                this.Websites.Add(this.ParseWebsite(siteNode, reader.Settings));
            }

            // 項目の対訳表
            XmlSerializer serializer = new XmlSerializer(typeof(TranslationDictionary), new XmlRootAttribute("ItemTable"));
            foreach (XmlNode itemNode in rootElement.SelectSingleNode("ItemTables").ChildNodes)
            {
                using (XmlReader r = XmlReader.Create(new StringReader(itemNode.OuterXml), reader.Settings))
                {
                    this.ItemTables.Add(serializer.Deserialize(r) as TranslationDictionary);
                }
            }

            // 見出しの対訳表
            using (XmlReader r = XmlReader.Create(
                new StringReader(rootElement.SelectSingleNode("HeadingTable").OuterXml),
                reader.Settings))
            {
                this.HeadingTable = new XmlSerializer(typeof(TranslationTable), new XmlRootAttribute("HeadingTable"))
                    .Deserialize(r) as TranslationTable;
            }
        }

        /// <summary>
        /// オブジェクトをXMLに出力する。
        /// </summary>
        /// <param name="writer">出力先のXmlWriter</param>
        public void WriteXml(XmlWriter writer)
        {
            // ロジッククラス
            string translator = this.Translator.FullName;
            if (translator.StartsWith(typeof(Translator).Namespace))
            {
                // 自前のエンジンの場合、クラス名だけを出力
                translator = this.Translator.Name;
            }

            writer.WriteElementString("Translator", translator);

            // 各処理モードのWebサイト
            writer.WriteStartElement("Websites");
            foreach (Website site in this.Websites)
            {
                // 通常はサイトのパッケージ名も含めたフル名を要素名とする
                string siteName = site.GetType().FullName;
                if (siteName.StartsWith(typeof(Website).Namespace))
                {
                    // 自前のサイトの場合、クラス名だけを出力
                    siteName = site.GetType().Name;
                }

                new XmlSerializer(site.GetType(), new XmlRootAttribute(siteName)).Serialize(writer, site);
            }

            writer.WriteEndElement();

            // 項目の対訳表
            XmlSerializer serializer = new XmlSerializer(typeof(TranslationDictionary), new XmlRootAttribute("ItemTable"));
            writer.WriteStartElement("ItemTables");
            foreach (TranslationDictionary trans in this.ItemTables)
            {
                serializer.Serialize(writer, trans);
            }

            writer.WriteEndElement();

            // 見出しの対訳表
            new XmlSerializer(this.HeadingTable.GetType(), new XmlRootAttribute("HeadingTable"))
                .Serialize(writer, this.HeadingTable);
        }

        /// <summary>
        /// 指定されたXML値からTranslatorのクラスを取得するる。
        /// </summary>
        /// <param name="name">XMLのクラス名情報。</param>
        /// <returns>Translatorクラス。</returns>
        /// <remarks>クラスは動的に判定する。クラスが存在しない場合などは随時状況に応じた例外を投げる。</remarks>
        private Type ParseTranslator(string name)
        {
            // Translateと同じパッケージに指定された名前のクラスがあるかを探す
            Type type = Type.GetType(typeof(Translator).Namespace + "." + name, false, true);
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
