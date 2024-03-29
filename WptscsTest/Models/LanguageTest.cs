﻿// ================================================================================================
// <summary>
//      Languageのテストクラスソース。</summary>
//
// <copyright file="LanguageTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
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
    using Honememo.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="Language"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class LanguageTest
    {
        #region プロパティテストケース

        /// <summary>
        /// Codeプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestCode()
        {
            Language lang = new Language("en");
            Assert.AreEqual("en", lang.Code);
        }

        /// <summary>
        /// Codeプロパティテストケース（コードがnull）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCodeNull()
        {
            Language lang = new Language(null);
        }

        /// <summary>
        /// Codeプロパティテストケース（コードが空）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCodeBlank()
        {
            Language lang = new Language(" ");
        }

        /// <summary>
        /// Namesプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestNames()
        {
            Language lang = new Language("en");

            // 初期状態で空のディクショナリーを作成
            Assert.IsNotNull(lang.Names);
            Assert.AreEqual(0, lang.Names.Count);

            Language.LanguageName name = new Language.LanguageName { Name = "テスト", ShortName = "テ" };
            lang.Names.Add("ja", name);
            Assert.IsTrue(lang.Names.ContainsKey("ja"));
            Assert.AreEqual(name, lang.Names["ja"]);
            Assert.IsFalse(lang.Names.ContainsKey("en"));
        }

        /// <summary>
        /// Namesプロパティテストケース（オブジェクトがnull）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNamesNull()
        {
            new Language("en").Names = null;
        }

        /// <summary>
        /// Bracketプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestBracket()
        {
            Language lang = new Language("en");

            // 設定ファイルからデフォルト値が設定されていること
            Assert.AreEqual(" ($1) ", lang.Bracket);

            // 設定後はそちらが有効になること
            lang.Bracket = "test";
            Assert.AreEqual("test", lang.Bracket);

            // 消すとデフォルトが有効になること
            lang.Bracket = null;
            Assert.AreEqual(" ($1) ", lang.Bracket);
            lang.Bracket = string.Empty;
            Assert.AreEqual(" ($1) ", lang.Bracket);
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// FormatLinkInterwikiメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestFormatLinkInterwiki()
        {
            Language lang = new Language("en");

            // パラメータを埋め込んで書式化される
            Assert.AreEqual(" (値1) ", lang.FormatBracket("値1"));
            lang.Bracket = " {$1} ";
            Assert.AreEqual(" {値2} ", lang.FormatBracket("値2"));
            lang.Bracket = "xxx";
            Assert.AreEqual("xxx", lang.FormatBracket("値3"));

            // 値がnull等でも特に制限はない
            lang.Bracket = null;
            Assert.AreEqual(" () ", lang.FormatBracket(null));
        }

        #endregion

        #region XMLシリアライズ用メソッドテストケース

        /// <summary>
        /// XMLデシリアライズテストケース。
        /// </summary>
        [TestMethod]
        public void TestReadXml()
        {
            Language lang;
            using (XmlReader r = XmlReader.Create(
                new StringReader(
                    "<Language Code=\"ja\"><Names>"
                    + "<LanguageName Code=\"en\"><Name>Japanese language</Name><ShortName>Japanese</ShortName></LanguageName>"
                    + "<LanguageName Code=\"zh\"><Name>日语</Name><ShortName /></LanguageName>"
                    + "</Names><Bracket>（{0}）</Bracket></Language>")))
            {
                lang = new XmlSerializer(typeof(Language)).Deserialize(r) as Language;
            }

            Assert.IsNotNull(lang);
            Assert.AreEqual("ja", lang.Code);
            Assert.AreEqual("Japanese language", lang.Names["en"].Name);
            Assert.AreEqual("Japanese", lang.Names["en"].ShortName);
            Assert.AreEqual("日语", lang.Names["zh"].Name);
            Assert.AreEqual(string.Empty, lang.Names["zh"].ShortName);
            Assert.IsFalse(lang.Names.ContainsKey("ja"));
            Assert.AreEqual("（{0}）", lang.Bracket);
        }

        /// <summary>
        /// XMLシリアライズテストケース。
        /// </summary>
        [TestMethod]
        public void TestWriteXml()
        {
            Language lang = new Language("ja");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            StringBuilder b = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b, settings))
            {
                new XmlSerializer(typeof(Language)).Serialize(w, lang);
            }

            Assert.AreEqual("<Language Code=\"ja\"><Names /><Bracket /></Language>", b.ToString());

            lang.Names.Add("en", new Language.LanguageName { Name = "Japanese language", ShortName = "Japanese" });
            lang.Names.Add("zh", new Language.LanguageName { Name = "日语" });
            lang.Bracket = "（{0}）";

            StringBuilder b2 = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b2, settings))
            {
                new XmlSerializer(typeof(Language)).Serialize(w, lang);
            }

            Assert.AreEqual(
                "<Language Code=\"ja\"><Names>"
                + "<LanguageName Code=\"en\"><Name>Japanese language</Name><ShortName>Japanese</ShortName></LanguageName>"
                + "<LanguageName Code=\"zh\"><Name>日语</Name><ShortName /></LanguageName>"
                + "</Names><Bracket>（{0}）</Bracket></Language>",
                b2.ToString());
        }

        #endregion
    }
}
