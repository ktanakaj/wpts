// ================================================================================================
// <summary>
//      Configのテストクラスソース。</summary>
//
// <copyright file="ConfigTest.cs.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using NUnit.Framework;
    using Honememo.Utilities;
    using Honememo.Wptscs.Logics;

    /// <summary>
    /// Configのテストクラスです。
    /// </summary>
    [TestFixture]
    public class ConfigTest
    {
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
            Assert.AreEqual(typeof(TranslateMediaWiki), config.Engine);
            Assert.IsTrue(config.Websites.Count > 0);
            Website en = config.GetWebsite("en");
            Assert.IsNotNull(en);
            Assert.AreEqual("http://en.wikipedia.org", en.Location);
            Assert.IsTrue(en.Language.Names.ContainsKey("ja"));
            // TODO: この辺も、内容の確認が必要
            Assert.IsTrue(config.ItemTables.Count == 0);
            Assert.IsTrue(config.HeadingTables.Count == 0);
        }

        /// <summary>
        /// XMLシリアライズテストケース。
        /// </summary>
        [Test]
        public void TestWriteXml()
        {
            // TODO: シリアライズでも細かい動作の差異があるので、もう少しテストケースが必要
            Config config = new TestingConfig();
            config.Engine = typeof(TranslateMediaWiki);
            // TODO: 全然未実装
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            StringBuilder b = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b, settings))
            {
                new XmlSerializer(typeof(Config)).Serialize(w, config);
            }

            Assert.AreEqual("<Config><Engine>TranslateMediaWiki</Engine><Websites /><ItemTables /><HeadingTables /></Config>", b.ToString());
        }

        #endregion
    }
}
