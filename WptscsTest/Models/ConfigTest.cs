// ================================================================================================
// <summary>
//      Configのテストクラスソース。</summary>
//
// <copyright file="ConfigTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Honememo.Utilities;
    using Honememo.Wptscs.Logics;
    using Honememo.Wptscs.Websites;
    using NUnit.Framework;

    /// <summary>
    /// Configのテストクラスです。
    /// </summary>
    [TestFixture]
    public class ConfigTest
    {
        #region 定数

        /// <summary>
        /// テスト結果が格納されているフォルダパス。
        /// </summary>
        private static readonly string resultXml = Path.Combine(MockFactory.TestMediaWikiDir, "result\\config.xml");

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Fileプロパティテストケース。
        /// </summary>
        [Test]
        public void TestFile()
        {
            // 初期状態ではnull、設定すれば設定した値が返る
            Config config = new Config();
            Assert.IsNull(config.File);
            config.File = "text.xml";
            Assert.AreEqual("text.xml", config.File);
            config.File = null;
            Assert.IsNull(config.File);
        }

        /// <summary>
        /// Translatorプロパティテストケース。
        /// </summary>
        [Test]
        public void TestTranslator()
        {
            // 初期状態ではnull、設定すれば設定した値が返る
            Config config = new Config();
            Assert.IsNull(config.Translator);
            config.Translator = typeof(Translator);
            Assert.AreEqual(typeof(Translator), config.Translator);
            config.Translator = null;
            Assert.IsNull(config.Translator);
        }

        /// <summary>
        /// Websitesプロパティテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestWebsites()
        {
            // 初期状態では空のリスト、設定すれば設定した値が返る
            Config config = new Config();
            Assert.AreEqual(0, config.Websites.Count);
            config.Websites.Add(new MediaWiki(new Language("en")));
            Assert.AreEqual(1, config.Websites.Count);
            IList<Website> list = new Website[] { new MediaWiki(new Language("ja")) };
            config.Websites = list;
            Assert.AreSame(list, config.Websites);
        }

        /// <summary>
        /// Websitesプロパティテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWebsitesNull()
        {
            new Config() { Websites = null };
        }

        /// <summary>
        /// ItemTablesプロパティテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestItemTables()
        {
            // 初期状態では空のリスト、設定すれば設定した値が返る
            Config config = new Config();
            Assert.AreEqual(0, config.ItemTables.Count);
            config.ItemTables.Add(new TranslationDictionary("en", "ja"));
            Assert.AreEqual(1, config.ItemTables.Count);
            IList<TranslationDictionary> list = new TranslationDictionary[] { new TranslationDictionary("ja", "en") };
            config.ItemTables = list;
            Assert.AreSame(list, config.ItemTables);
        }

        /// <summary>
        /// ItemTablesプロパティテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestItemTablesNull()
        {
            new Config() { ItemTables = null };
        }

        /// <summary>
        /// HeadingTableプロパティテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestHeadingTable()
        {
            // 初期状態では空のオブジェクト、設定すれば設定した値が返る
            Config config = new Config();
            Assert.AreEqual(0, config.HeadingTable.Count);
            config.HeadingTable.Add(new Dictionary<string, string>());
            Assert.AreEqual(1, config.HeadingTable.Count);
            TranslationTable table = new TranslationTable();
            config.HeadingTable = table;
            Assert.AreSame(table, config.HeadingTable);
        }

        /// <summary>
        /// HeadingTableプロパティテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestHeadingTableNull()
        {
            new Config() { HeadingTable = null };
        }

        #endregion

        //// TODO: メソッドのテストケースが未実装

        #region XMLシリアライズ用メソッドケース

        /// <summary>
        /// XMLデシリアライズテストケース。
        /// </summary>
        [Test]
        public void TestReadXml()
        {
            // TODO: デシリアライズでも細かい動作の差異があるので、もう少しテストケースが必要
            Config config;
            using (Stream stream = new FileStream(MockFactory.TestConfigXml, FileMode.Open, FileAccess.Read))
            {
                config = new XmlSerializer(typeof(Config)).Deserialize(stream) as Config;
            }

            Assert.AreEqual(typeof(MediaWikiTranslator), config.Translator);
            Assert.IsTrue(config.Websites.Count > 0);
            Website en = config.GetWebsite("en");
            Assert.IsNotNull(en);
            Assert.AreEqual("http://en.wikipedia.org", en.Location);
            Assert.IsTrue(en.Language.Names.ContainsKey("ja"));

            // TODO: この辺も、内容の確認が必要
            Assert.IsTrue(config.ItemTables.Count > 0);
            Assert.IsTrue(config.HeadingTable.Count > 0);
            Assert.AreEqual("関連項目", config.HeadingTable.GetWord("en", "ja", "See Also"));
        }

        /// <summary>
        /// XMLシリアライズテストケース。
        /// </summary>
        [Test]
        public void TestWriteXml()
        {
            // TODO: シリアライズでも細かい動作の差異があるので、もう少しテストケースが必要
            Config config = new Config();
            config.Translator = typeof(MediaWikiTranslator);
            TranslationDictionary dic = new TranslationDictionary("en", "ja");
            dic.Add("dicKey", new TranslationDictionary.Item { Word = "dicTest" });
            config.ItemTables.Add(dic);
            config.HeadingTable = new TranslationTable();
            IDictionary<string, string> record = new SortedDictionary<string, string>();
            record["recordKey"] = "recordValue";
            config.HeadingTable.Add(record);

            // TODO: 全然未実装
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            StringBuilder b = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b, settings))
            {
                new XmlSerializer(typeof(Config)).Serialize(w, config);
            }

            Assert.AreEqual(
                "<Config><Translator>MediaWikiTranslator</Translator><Websites />"
                + "<ItemTables><ItemTable From=\"en\" To=\"ja\"><Item From=\"dicKey\" To=\"dicTest\" /></ItemTable></ItemTables>"
                + "<HeadingTable><Group><Word Lang=\"recordKey\">recordValue</Word></Group></HeadingTable></Config>",
                b.ToString());
        }

        /// <summary>
        /// XMLデシリアライズ→シリアライズの通しのテストケース。
        /// </summary>
        [Test]
        public void TestReadXmlToWriteXml()
        {
            Config config;
            using (Stream stream = new FileStream(MockFactory.TestConfigXml, FileMode.Open, FileAccess.Read))
            {
                config = new XmlSerializer(typeof(Config)).Deserialize(stream) as Config;
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            StringBuilder b = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b, settings))
            {
                new XmlSerializer(typeof(Config)).Serialize(w, config);
            }

            Assert.AreEqual(File.ReadAllText(resultXml), b.ToString());
        }

        #endregion
    }
}
