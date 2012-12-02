// ================================================================================================
// <summary>
//      XmlElementのテストクラスソース。</summary>
//
// <copyright file="XmlElementTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="XmlElement"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class XmlElementTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            XmlElement element = new XmlElement("testname1");
            Assert.AreEqual("testname1", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);
            Assert.AreEqual(0, element.Count);

            element = new XmlElement("testname2", "testvalue");
            Assert.AreEqual("testname2", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);
            Assert.AreEqual(1, element.Count);
            Assert.IsInstanceOfType(element[0], typeof(TextElement));
            Assert.AreEqual("testvalue", element[0].ToString());

            IDictionary<string, string> attribute = new Dictionary<string, string>();
            attribute.Add("testattr1", "testattrvalue1");
            ICollection<IElement> collection = new List<IElement>();
            collection.Add(new XmlCommentElement("testcomment"));
            element = new XmlElement("testname3", attribute, collection);
            Assert.AreEqual("testname3", element.Name);
            Assert.AreEqual("testattrvalue1", element.Attributes["testattr1"]);
            Assert.AreEqual("testcomment", ((XmlCommentElement)element[0]).Text);
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Nameプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestName()
        {
            XmlElement element = new XmlElement("testname1");
            Assert.AreEqual("testname1", element.Name);
            element.Name = "testname2";
            Assert.AreEqual("testname2", element.Name);
        }

        /// <summary>
        /// Nameプロパティテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNameNull()
        {
            XmlElement element = new XmlElement(null);
        }

        /// <summary>
        /// Nameプロパティテストケース（空文字列）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNameEmpty()
        {
            XmlElement element = new XmlElement(string.Empty);
        }

        /// <summary>
        /// Nameプロパティテストケース（空白文字列）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNameBlank()
        {
            XmlElement element = new XmlElement(" ");
        }

        /// <summary>
        /// Attributesプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestAttributes()
        {
            XmlElement element = new XmlElement("testname");
            Assert.IsNotNull(element.Attributes);
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            XmlElement element = new XmlElement("form");
            Assert.AreEqual("<form />", element.ToString());
            element.Attributes.Add("action", "/test.html");
            Assert.AreEqual("<form action=\"/test.html\" />", element.ToString());
            element.Attributes.Add("disabled", string.Empty);
            Assert.AreEqual("<form action=\"/test.html\" disabled=\"\" />", element.ToString());
            element.Add(new TextElement("フォーム内のテキスト"));
            Assert.AreEqual("<form action=\"/test.html\" disabled=\"\">フォーム内のテキスト</form>", element.ToString());
            element.Add(new XmlCommentElement("コメント"));
            Assert.AreEqual("<form action=\"/test.html\" disabled=\"\">フォーム内のテキスト<!--コメント--></form>", element.ToString());
            element.Attributes.Add("test_attr", "&<>\"");
            Assert.AreEqual("<form action=\"/test.html\" disabled=\"\" test_attr=\"&amp;&lt;&gt;&quot;\">フォーム内のテキスト<!--コメント--></form>", element.ToString());
        }

        #endregion
    }
}
