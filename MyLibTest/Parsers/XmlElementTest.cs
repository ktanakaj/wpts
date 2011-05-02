// ================================================================================================
// <summary>
//      XmlElementのテストクラスソース。</summary>
//
// <copyright file="XmlElementTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// XmlElementのテストクラスです。
    /// </summary>
    [TestFixture]
    public class XmlElementTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [Test]
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
            Assert.IsInstanceOf(typeof(TextElement), element[0]);
            Assert.AreEqual("testvalue", element[0].ToString());

            IDictionary<string, string> attribute = new Dictionary<string, string>();
            attribute.Add("testattr1", "testattrvalue1");
            ICollection<IElement> collection = new List<IElement>();
            collection.Add(new CommentElement("testcomment"));
            element = new XmlElement("testname3", attribute, collection);
            Assert.AreEqual("testname3", element.Name);
            Assert.AreEqual("testattrvalue1", element.Attributes["testattr1"]);
            Assert.AreEqual("testcomment", ((CommentElement)element[0]).Text);
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Nameプロパティテストケース。
        /// </summary>
        [Test]
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
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNameNull()
        {
            XmlElement element = new XmlElement(null);
        }

        /// <summary>
        /// Nameプロパティテストケース（空文字列）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNameEmpty()
        {
            XmlElement element = new XmlElement(String.Empty);
        }

        /// <summary>
        /// Nameプロパティテストケース（空白文字列）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNameBlank()
        {
            XmlElement element = new XmlElement(" ");
        }

        /// <summary>
        /// Attributesプロパティテストケース。
        /// </summary>
        [Test]
        public void TestAttributes()
        {
            XmlElement element = new XmlElement("testname");
            Assert.IsNotNull(element.Attributes);
        }

        #endregion

        #region 静的メソッドテストケース

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            // 解析処理の詳細はXmlParser側のテストケースで試験
            XmlElement element;
            XmlParser parser = new XmlParser();
            Assert.IsTrue(XmlElement.TryParse("<h1>test</H1>", parser, out element));
            Assert.AreEqual("h1", element.Name);
            Assert.AreEqual("test", element[0].ToString());
            parser.IgnoreCase = false;
            Assert.IsTrue(XmlElement.TryParse("<h1>test</H1>", parser, out element));
            Assert.AreEqual("test</H1>", element[0].ToString());
        }

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseLazy()
        {
            // XmlParserが初期設定で動いていれば、大文字小文字違いを閉じタグと判定する
            XmlElement element;
            Assert.IsTrue(XmlElement.TryParseLazy("<h1>test</H1>", out element));
            Assert.AreEqual("h1", element.Name);
            Assert.AreEqual("test", element[0].ToString());
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            XmlElement element = new XmlElement("form");
            Assert.AreEqual("<form />", element.ToString());
            element.Attributes.Add("action", "/test.html");
            Assert.AreEqual("<form action=\"/test.html\" />", element.ToString());
            element.Attributes.Add("disabled", "");
            Assert.AreEqual("<form action=\"/test.html\" disabled=\"\" />", element.ToString());
            element.Add(new TextElement("フォーム内のテキスト"));
            Assert.AreEqual("<form action=\"/test.html\" disabled=\"\">フォーム内のテキスト</form>", element.ToString());
            element.Add(new CommentElement("コメント"));
            Assert.AreEqual("<form action=\"/test.html\" disabled=\"\">フォーム内のテキスト<!--コメント--></form>", element.ToString());
            element.Attributes.Add("test_attr", "&<>\"");
            Assert.AreEqual("<form action=\"/test.html\" disabled=\"\" test_attr=\"&amp;&lt;&gt;&quot;\">フォーム内のテキスト<!--コメント--></form>", element.ToString());
        }

        #endregion
    }
}
