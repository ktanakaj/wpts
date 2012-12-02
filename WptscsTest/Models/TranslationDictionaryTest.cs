// ================================================================================================
// <summary>
//      TranslationDictionaryのテストクラスソース。</summary>
//
// <copyright file="TranslationDictionaryTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="TranslationDictionary"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class TranslationDictionaryTest
    {
        #region XMLシリアライズ用メソッドテストケース

        /// <summary>
        /// XMLデシリアライズテストケース。
        /// </summary>
        [TestMethod]
        public void TestReadXml()
        {
            // TODO: リダイレクトとタイムスタンプも
            TranslationDictionary dic;
            using (XmlReader r = XmlReader.Create(
                new StringReader(
                    "<TranslationDictionary From=\"en\" To=\"ja\"><Item From=\".example\" To=\"。さんぷる\" />"
                    + "<Item From=\"Template:Disambig\" To=\"Template:曖昧さ回避\" /></TranslationDictionary>")))
            {
                dic = new XmlSerializer(typeof(TranslationDictionary)).Deserialize(r) as TranslationDictionary;
            }

            Assert.AreEqual(2, dic.Count);
            Assert.AreEqual("。さんぷる", dic[".example"].Word);
            Assert.AreEqual("Template:曖昧さ回避", dic["Template:Disambig"].Word);
            Assert.IsFalse(dic.ContainsKey("。さんぷる"));
        }

        /// <summary>
        /// XMLシリアライズテストケース。
        /// </summary>
        [TestMethod]
        public void TestWriteXml()
        {
            // TODO: リダイレクトとタイムスタンプも
            TranslationDictionary dic = new TranslationDictionary("en", "ja");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            StringBuilder b = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b, settings))
            {
                new XmlSerializer(typeof(TranslationDictionary)).Serialize(w, dic);
            }

            Assert.AreEqual("<TranslationDictionary From=\"en\" To=\"ja\" />", b.ToString());

            dic[".example"] = new TranslationDictionary.Item { Word = "。さんぷる" };
            dic["Template:Disambig"] = new TranslationDictionary.Item { Word = "Template:曖昧さ回避" };

            StringBuilder b2 = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b2, settings))
            {
                new XmlSerializer(typeof(TranslationDictionary)).Serialize(w, dic);
            }

            Assert.AreEqual(
                "<TranslationDictionary From=\"en\" To=\"ja\"><Item From=\".example\" To=\"。さんぷる\" />"
                + "<Item From=\"Template:Disambig\" To=\"Template:曖昧さ回避\" /></TranslationDictionary>",
                b2.ToString());
        }

        #endregion
    }
}
