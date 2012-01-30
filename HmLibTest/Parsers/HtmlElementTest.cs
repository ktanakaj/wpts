// ================================================================================================
// <summary>
//      HtmlElementのテストクラスソース。</summary>
//
// <copyright file="HtmlElementTest.cs" company="honeplusのメモ帳">
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
    /// HtmlElementのテストクラスです。
    /// </summary>
    [TestFixture]
    public class HtmlElementTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            HtmlElement element = new HtmlElement("testname1");
            Assert.AreEqual("testname1", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);
            Assert.AreEqual(0, element.Count);

            element = new HtmlElement("testname2", "testvalue");
            Assert.AreEqual("testname2", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);
            Assert.AreEqual(1, element.Count);
            Assert.IsInstanceOf(typeof(TextElement), element[0]);
            Assert.AreEqual("testvalue", element[0].ToString());

            IDictionary<string, string> attribute = new Dictionary<string, string>();
            attribute.Add("testattr1", "testattrvalue1");
            ICollection<IElement> collection = new List<IElement>();
            collection.Add(new XmlCommentElement("testcomment"));
            element = new HtmlElement("testname3", attribute, collection);
            Assert.AreEqual("testname3", element.Name);
            Assert.AreEqual("testattrvalue1", element.Attributes["testattr1"]);
            Assert.AreEqual("testcomment", ((XmlCommentElement)element[0]).Text);
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            HtmlElement element = new HtmlElement("form");
            Assert.AreEqual("<form>", element.ToString());
            element.Attributes.Add("action", "/test.html");
            Assert.AreEqual("<form action=\"/test.html\">", element.ToString());
            element.Attributes.Add("disabled", "");
            Assert.AreEqual("<form action=\"/test.html\" disabled=\"\">", element.ToString());
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
