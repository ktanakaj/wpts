// ================================================================================================
// <summary>
//      Configのテストクラスソース。</summary>
//
// <copyright file="ConfigTest.cs.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
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
    using NUnit.Framework;
    using Honememo.Utilities;
    using Honememo.Wptscs.Logics;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// Configのテストクラスです。
    /// </summary>
    [TestFixture]
    public class ConfigTest
    {
        #region モッククラス

        /// <summary>
        /// Configテスト用のモッククラスです。
        /// </summary>
        /// <remarks>そのままではnewすることができないため。</remarks>
        public class DummyConfig : Config
        {
        }

        #endregion

        // TODO: テストケース全然足りない

        #region XMLシリアライズ用メソッドケース

        /// <summary>
        /// XMLデシリアライズテストケース。
        /// </summary>
        [Test]
        public void TestReadXml()
        {
            // TODO: デシリアライズでも細かい動作の差異があるので、もう少しテストケースが必要
            Config config;
            using (Stream stream = new FileStream("Data\\config.xml", FileMode.Open, FileAccess.Read))
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
            Assert.IsTrue(config.ItemTables.Count == 0);
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
            Config config = new DummyConfig();
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

            Assert.AreEqual("<Config><Translator>MediaWikiTranslator</Translator><Websites />"
                + "<ItemTables><ItemTable From=\"en\" To=\"ja\"><Item From=\"dicKey\" To=\"dicTest\" /></ItemTable></ItemTables>"
                + "<HeadingTable><Group><Word Lang=\"recordKey\">recordValue</Word></Group></HeadingTable></Config>", b.ToString());
        }

        #endregion
    }
}
