// ================================================================================================
// <summary>
//      Websiteのテストクラスソース。</summary>
//
// <copyright file="WebsiteTest.cs.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using NUnit.Framework;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;

    /// <summary>
    /// Websiteのテストクラスです。
    /// </summary>
    [TestFixture]
    public class WebsiteTest
    {
        #region モッククラス

        /// <summary>
        /// Websiteテスト用のモッククラスです。
        /// </summary>
        public class DummySite : Website, IXmlSerializable
        {
            #region テスト用プロパティ

            /// <summary>
            /// ウェブサイトの場所。
            /// </summary>
            /// <remarks>動作確認はhttpとfileスキームのみ。</remarks>
            public new string Location
            {
                get
                {
                    return base.Location;
                }

                set
                {
                    base.Location = value;
                }
            }

            /// <summary>
            /// ウェブサイトの言語。
            /// </summary>
            public new Language Language
            {
                get
                {
                    return base.Language;
                }

                set
                {
                    base.Language = value;
                }
            }

            #endregion

            #region ダミーメソッド

            /// <summary>
            /// ページを取得。
            /// </summary>
            /// <param name="title">ページタイトル。</param>
            /// <returns>取得したページ。</returns>
            /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
            public override Page GetPage(string title)
            {
                return null;
            }

            #endregion

            #region テスト用XMLシリアライズ用メソッド

            /// <summary>
            /// シリアライズするXMLのスキーマ定義を返す。
            /// </summary>
            /// <returns>XML表現を記述するXmlSchema。</returns>
            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            /// <summary>
            /// XMLからオブジェクトをデシリアライズする。
            /// </summary>
            /// <param name="reader">デシリアライズ元のXmlReader</param>
            public void ReadXml(XmlReader reader)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(reader);

                // Webサイト
                // ※ 以下、基本的に無かったらNGの部分はいちいちチェックしない。例外飛ばす
                XmlElement siteElement = xml.SelectSingleNode("DummySite") as XmlElement;
                this.Location = siteElement.SelectSingleNode("Location").InnerText;

                using (XmlReader r = XmlReader.Create(
                    new StringReader(siteElement.SelectSingleNode("Language").OuterXml), reader.Settings))
                {
                    this.Language = new XmlSerializer(typeof(Language)).Deserialize(r) as Language;
                }
            }

            /// <summary>
            /// オブジェクトをXMLにシリアライズする。
            /// </summary>
            /// <param name="writer">シリアライズ先のXmlWriter</param>
            public void WriteXml(XmlWriter writer)
            {
                writer.WriteElementString("Location", this.Location);
                new XmlSerializer(typeof(Language)).Serialize(writer, this.Language);
            }

            #endregion
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Locationプロパティテストケース。
        /// </summary>
        [Test]
        public void TestLocation()
        {
            DummySite site = new DummySite();
            site.Location = "test";
            Assert.AreEqual("test", site.Location);
        }

        /// <summary>
        /// Locationプロパティテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLocationNull()
        {
            new DummySite().Location = null;
        }

        /// <summary>
        /// Locationプロパティテストケース（空）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLocationBlank()
        {
            new DummySite().Location = " ";
        }

        /// <summary>
        /// Languageプロパティテストケース。
        /// </summary>
        [Test]
        public void TestLanguage()
        {
            DummySite site = new DummySite();
            site.Language = new Language("ja");
            Assert.AreEqual("ja", site.Language.Code);
        }

        /// <summary>
        /// Languageプロパティテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLanguageNull()
        {
            new DummySite().Language = null;
        }

        #endregion

        #region メソッドテストケース

        /// <summary>
        /// XMLデシリアライズテストケース。
        /// </summary>
        [Test]
        public void TestReadXml()
        {
            DummySite site;
            using (XmlReader r = XmlReader.Create(
                new StringReader("<DummySite><Location>http://ja.wikipedia.org</Location>"
                    + "<Language Code=\"ja\"><Names /></Language></DummySite>")))
            {
                site = new XmlSerializer(typeof(DummySite)).Deserialize(r) as DummySite;
            }

            Assert.IsNotNull(site);
            Assert.AreEqual("http://ja.wikipedia.org", site.Location);
            Assert.AreEqual("ja", site.Language.Code);
        }

        /// <summary>
        /// XMLシリアライズテストケース。
        /// </summary>
        [Test]
        public void TestWriteXml()
        {
            Language lang = new Language("ja");
            DummySite site = new DummySite();
            site.Location = "http://ja.wikipedia.org";
            site.Language = lang;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            StringBuilder b = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b, settings))
            {
                new XmlSerializer(typeof(DummySite)).Serialize(w, site);
            }

            Assert.AreEqual("<DummySite><Location>http://ja.wikipedia.org</Location><Language Code=\"ja\"><Names /><Bracket /></Language></DummySite>", b.ToString());
        }

        #endregion
    }
}
