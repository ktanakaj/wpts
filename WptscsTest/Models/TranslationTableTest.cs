// ================================================================================================
// <summary>
//      TranslationTableのテストクラスソース。</summary>
//
// <copyright file="TranslationTableTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using NUnit.Framework;

    /// <summary>
    /// TranslationTableのテストクラスです。
    /// </summary>
    [TestFixture]
    public class TranslationTableTest
    {
        #region 公開メソッドテストケース

        /// <summary>
        /// GetWordメソッドテストケース。
        /// </summary>
        [Test]
        public void TestGetWord()
        {
            TranslationTable table = new TranslationTable();
            IDictionary<string, string[]> record = new Dictionary<string, string[]>();
            record["en"] = new string[] { "See also", "See" };
            record["ja"] = new string[] { "関連項目" };
            table.Add(record);
            table.From = "en";
            table.To = "ja";
            Assert.AreEqual("関連項目", table.GetWord("See also"));
            Assert.AreEqual("関連項目", table.GetWord("see also"));
            Assert.AreEqual("関連項目", table.GetWord("see"));
            Assert.IsNull(table.GetWord("test"));
            Assert.IsNull(table.GetWord(String.Empty));
            Assert.AreEqual("See also", table.GetWord("ja", "en", "関連項目"));
            Assert.IsNull(table.GetWord("ja", "en", String.Empty));
            Assert.IsNull(table.GetWord("ja", String.Empty, String.Empty));
            Assert.IsNull(table.GetWord(String.Empty, String.Empty, String.Empty));
        }

        /// <summary>
        /// GetWordメソッドテストケース（fromがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetWordFromNull()
        {
            TranslationTable table = new TranslationTable();
            table.GetWord(null, "en", "関連項目");
        }

        /// <summary>
        /// GetWordメソッドテストケース（toがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetWordToNull()
        {
            TranslationTable table = new TranslationTable();
            table.GetWord("ja", null, "関連項目");
        }

        /// <summary>
        /// GetWordメソッドテストケース（wordがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetWordWordNull()
        {
            TranslationTable table = new TranslationTable();
            table.GetWord("ja", "en", null);
        }

        /// <summary>
        /// GetWordメソッドテストケース（プロパティがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetWordPropertyNull()
        {
            TranslationTable table = new TranslationTable();
            table.GetWord("関連項目");
        }

        #endregion

        #region XMLシリアライズ用メソッドテストケース

        /// <summary>
        /// XMLデシリアライズテストケース。
        /// </summary>
        [Test]
        public void TestReadXml()
        {
            TranslationTable table;
            using (XmlReader r = XmlReader.Create(
                new StringReader(
                    "<TranslationTable><Group><Word Lang=\"en\">See</Word><Word Lang=\"en\" Head=\"true\">See Also</Word><Word Lang=\"ja\">関連項目</Word></Group>"
                    + "<Group><Word Lang=\"en\">History</Word><Word Lang=\"fr\">Histoire</Word></Group></TranslationTable>")))
            {
                table = new XmlSerializer(typeof(TranslationTable)).Deserialize(r) as TranslationTable;
            }

            Assert.AreEqual(2, table.Count);
            Assert.AreEqual("See Also", table[0]["en"].FirstOrDefault());
            Assert.AreEqual("See", table[0]["en"].ElementAtOrDefault(1));
            Assert.AreEqual("関連項目", table[0]["ja"].FirstOrDefault());
            Assert.IsFalse(table[0].ContainsKey("fr"));
            Assert.AreEqual("History", table[1]["en"].FirstOrDefault());
            Assert.AreEqual("Histoire", table[1]["fr"].FirstOrDefault());
            Assert.IsFalse(table[1].ContainsKey("ja"));
        }

        /// <summary>
        /// XMLシリアライズテストケース。
        /// </summary>
        [Test]
        public void TestWriteXml()
        {
            TranslationTable table = new TranslationTable();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            StringBuilder b = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b, settings))
            {
                new XmlSerializer(typeof(TranslationTable)).Serialize(w, table);
            }

            Assert.AreEqual("<TranslationTable />", b.ToString());

            IDictionary<string, string[]> record = new SortedDictionary<string, string[]>();
            record["en"] = new string[] { "See Also", "See" };
            record["ja"] = new string[] { "関連項目" };
            table.Add(record);
            record = new SortedDictionary<string, string[]>();
            record["en"] = new string[] { "History" };
            record["fr"] = new string[] { "Histoire" };
            table.Add(record);

            StringBuilder b2 = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b2, settings))
            {
                new XmlSerializer(typeof(TranslationTable)).Serialize(w, table);
            }

            Assert.AreEqual(
                "<TranslationTable><Group><Word Lang=\"en\" Head=\"True\">See Also</Word><Word Lang=\"en\">See</Word><Word Lang=\"ja\">関連項目</Word></Group>"
                + "<Group><Word Lang=\"en\">History</Word><Word Lang=\"fr\">Histoire</Word></Group></TranslationTable>",
                b2.ToString());
        }

        #endregion
    }
}
