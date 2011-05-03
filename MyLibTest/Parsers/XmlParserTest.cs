// ================================================================================================
// <summary>
//      XmlParserのテストクラスソース。</summary>
//
// <copyright file="XmlParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    /// <summary>
    /// XmlParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class XmlParserTest
    {
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// Parseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestParse()
        {
            // ※ 現状解析が失敗するパターンは無い
            XmlParser parser = new XmlParser();

            Assert.AreEqual("test", parser.Parse("test").ToString());

            IElement element = parser.Parse("testbefore<p>testinner</p><!--comment-->testafter");
            Assert.IsInstanceOf(typeof(ICollection<IElement>), element);
            ICollection<IElement> collection = (ICollection<IElement>)element;
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual("testbefore", collection.ElementAt(0).ToString());
            Assert.IsInstanceOf(typeof(XmlElement), collection.ElementAt(1));
            Assert.AreEqual("<p>testinner</p>", collection.ElementAt(1).ToString());
            Assert.IsInstanceOf(typeof(XmlCommentElement), collection.ElementAt(2));
            Assert.AreEqual("<!--comment-->", collection.ElementAt(2).ToString());
            Assert.AreEqual("testafter", collection.ElementAt(3).ToString());
        }

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            // ※ 現状解析が失敗するパターンは無い
            IElement element;
            XmlParser parser = new XmlParser();

            Assert.IsTrue(parser.TryParse("test", out element));
            Assert.IsInstanceOf(typeof(TextElement), element);
            Assert.AreEqual("test", element.ToString());

            Assert.IsTrue(parser.TryParse("testbefore<p>testinner</p><!--comment-->testafter", out element));
            Assert.IsInstanceOf(typeof(ICollection<IElement>), element);
            ICollection<IElement> collection = (ICollection<IElement>)element;
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual("testbefore", collection.ElementAt(0).ToString());
            Assert.IsInstanceOf(typeof(XmlElement), collection.ElementAt(1));
            Assert.AreEqual("<p>testinner</p>", collection.ElementAt(1).ToString());
            Assert.IsInstanceOf(typeof(XmlCommentElement), collection.ElementAt(2));
            Assert.AreEqual("<!--comment-->", collection.ElementAt(2).ToString());
            Assert.AreEqual("testafter", collection.ElementAt(3).ToString());
        }

        #endregion

        #region 静的メソッドテストケース

        /// <summary>
        /// IsXmlElementPossibleメソッドテストケース。
        /// </summary>
        [Test]
        public void TestIsXmlElementPossible()
        {
            XmlParser parser = new XmlParser();
            Assert.IsTrue(parser.IsXmlElementPossible('<'));
            Assert.IsFalse(parser.IsXmlElementPossible('['));
            Assert.IsFalse(parser.IsXmlElementPossible('-'));
            Assert.IsFalse(parser.IsXmlElementPossible('/'));
            Assert.IsFalse(parser.IsXmlElementPossible('#'));
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// TryParseXmlElementメソッドテストケース（実例）。
        /// </summary>
        [Test]
        public void TestTryParseXmlElement()
        {
            XmlElement element;
            XmlParser parser = new XmlParser();
            Assert.IsTrue(parser.TryParseXmlElement("<h1>test</h1>", out element));
            Assert.AreEqual("<h1>test</h1>", element.ToString());
            Assert.AreEqual("h1", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<br /><br />test<br />", out element));
            Assert.AreEqual("<br />", element.ToString());
            Assert.AreEqual("br", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<div id=\"testid\" name=\"testname\"><!--<div id=\"indiv\">test</div>--></div><br />", out element));
            Assert.AreEqual("<div id=\"testid\" name=\"testname\"><!--<div id=\"indiv\">test</div>--></div>", element.ToString());
            Assert.AreEqual("div", element.Name);
            Assert.AreEqual(2, element.Attributes.Count);
            Assert.AreEqual("testid", element.Attributes["id"]);
            Assert.AreEqual("testname", element.Attributes["name"]);

            parser.IsHtml = true;
            Assert.IsTrue(parser.TryParseXmlElement("<p>段落1<p>段落2", out element));
            Assert.AreEqual("<p>", element.ToString());
            Assert.AreEqual("p", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<input type=\"checkbox\" name=\"param\" value=\"test\" checked><label for=\"param\">チェック</label>", out element));
            Assert.AreEqual("<input type=\"checkbox\" name=\"param\" value=\"test\" checked>", element.ToString());
            Assert.AreEqual("input", element.Name);
            Assert.AreEqual(4, element.Attributes.Count);
            Assert.AreEqual("checkbox", element.Attributes["type"]);
            Assert.AreEqual("param", element.Attributes["name"]);
            Assert.AreEqual("test", element.Attributes["value"]);
            Assert.IsEmpty(element.Attributes["checked"]);
        }

        /// <summary>
        /// TryParseXmlElementメソッドテストケース（基本形）。
        /// </summary>
        [Test]
        public void TestTryParseXmlElementNormal()
        {
            XmlElement element;
            XmlParser parser = new XmlParser();
            Assert.IsTrue(parser.TryParseXmlElement("<testtag></testtag>", out element));
            Assert.AreEqual("<testtag></testtag>", element.ToString());
            Assert.AreEqual("testtag", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag2>test value</testtag2>", out element));
            Assert.AreEqual("<testtag2>test value</testtag2>", element.ToString());
            Assert.AreEqual("testtag2", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag3> test value2 </testtag3>testend", out element));
            Assert.AreEqual("<testtag3> test value2 </testtag3>", element.ToString());
            Assert.AreEqual("testtag3", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag4 testattr=\"testvalue\"> test<!-- </testtag4> --> value3 </testtag4>testend", out element));
            Assert.AreEqual("<testtag4 testattr=\"testvalue\"> test<!-- </testtag4> --> value3 </testtag4>", element.ToString());
            Assert.AreEqual("testtag4", element.Name);
            Assert.AreEqual(1, element.Attributes.Count);
            Assert.AreEqual("testvalue", element.Attributes["testattr"]);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag5 testattr2='testvalue2'><testbody></testtag5 >testend", out element));
            Assert.IsInstanceOf(typeof(XmlElement), element[0]);
            Assert.AreEqual("<testtag5 testattr2='testvalue2'><testbody></testtag5 >", element.ToString());
            Assert.AreEqual("testtag5", element.Name);
            Assert.AreEqual(1, element.Attributes.Count);
            Assert.AreEqual("testvalue2", element.Attributes["testattr2"]);
        }

        /// <summary>
        /// TryParseXmlElementメソッドテストケース（普通でNGパターン）。
        /// </summary>
        [Test]
        public void TestTryParseXmlElementNormalNg()
        {
            XmlElement element;
            XmlParser parser = new XmlParser();
            Assert.IsFalse(parser.TryParseXmlElement(" <testtag></testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseXmlElement("<!-- comment -->", out element));
            Assert.IsNull(element);
        }

        /// <summary>
        /// TryParseXmlElementメソッドテストケース（単一のパターン）。
        /// </summary>
        [Test]
        public void TestTryParseXmlElementSingle()
        {
            XmlElement element;
            XmlParser parser = new XmlParser();
            Assert.IsTrue(parser.TryParseXmlElement("<testtag />", out element));
            Assert.AreEqual("<testtag />", element.ToString());
            Assert.AreEqual("testtag", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag2/>", out element));
            Assert.AreEqual("<testtag2/>", element.ToString());
            Assert.AreEqual("testtag2", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag3   />testtag4 />", out element));
            Assert.AreEqual("<testtag3   />", element.ToString());
            Assert.AreEqual("testtag3", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag5 testattr=\"testvalue\" />/>", out element));
            Assert.AreEqual("<testtag5 testattr=\"testvalue\" />", element.ToString());
            Assert.AreEqual("testtag5", element.Name);
            Assert.AreEqual(1, element.Attributes.Count);
            Assert.AreEqual("testvalue", element.Attributes["testattr"]);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag6 testattr1=\"testvalue1\" testattr2=\"testvalue2\"/>/>", out element));
            Assert.AreEqual("<testtag6 testattr1=\"testvalue1\" testattr2=\"testvalue2\"/>", element.ToString());
            Assert.AreEqual("testtag6", element.Name);
            Assert.AreEqual(2, element.Attributes.Count);
            Assert.AreEqual("testvalue1", element.Attributes["testattr1"]);
            Assert.AreEqual("testvalue2", element.Attributes["testattr2"]);
        }

        /// <summary>
        /// TryParseXmlElementメソッドテストケース（不正な構文）。
        /// </summary>
        [Test]
        public void TestTryParseXmlElementLazy()
        {
            XmlElement element;
            XmlParser parser = new XmlParser();
            Assert.IsTrue(parser.TryParseXmlElement("<p>", out element));
            Assert.AreEqual("<p>", element.ToString());
            Assert.AreEqual("p", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag>test value", out element));
            Assert.AreEqual("<testtag>test value", element.ToString());
            Assert.AreEqual("testtag", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag2 testattr=test value>test value</testtag2>", out element));
            Assert.AreEqual("<testtag2 testattr=test value>test value</testtag2>", element.ToString());
            Assert.AreEqual("testtag2", element.Name);
            Assert.AreEqual(2, element.Attributes.Count);
            Assert.AreEqual("test", element.Attributes["testattr"]);
            Assert.IsEmpty(element.Attributes["value"]);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag3>test value2</ testtag3>testend", out element));
            Assert.AreEqual("<testtag3>test value2</ testtag3>testend", element.ToString());
            Assert.AreEqual("testtag3", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);
        }


        /// <summary>
        /// TryParseXmlElementメソッドテストケース（不正でNG）。
        /// </summary>
        [Test]
        public void TestTryParseXmlElementLazyNg()
        {
            XmlElement element;
            XmlParser parser = new XmlParser();
            Assert.IsFalse(parser.TryParseXmlElement("< testtag></testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseXmlElement("<testtag", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseXmlElement("<testtag ", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseXmlElement("<testtag /", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseXmlElement("<testtag </testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseXmlElement("<testtag testattr=\"testvalue'></testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseXmlElement("<sub((>4</sub>", out element));
            Assert.IsNull(element);
        }

        /// <summary>
        /// TryParseXmlElementメソッドテストケース（HTML）。
        /// </summary>
        [Test]
        public void TestTryParseXmlElementHtml()
        {
            XmlElement element;
            XmlParser parser = new XmlParser { IsHtml = true };
            Assert.IsTrue(parser.TryParseXmlElement("<testtag />", out element));
            Assert.AreEqual("<testtag />", element.ToString());
            Assert.AreEqual("testtag", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<p>", out element));
            Assert.AreEqual("<p>", element.ToString());
            Assert.AreEqual("p", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag2 />testtag3 />", out element));
            Assert.AreEqual("<testtag2 />", element.ToString());
            Assert.AreEqual("testtag2", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag4></testtag5>", out element));
            Assert.AreEqual("<testtag4>", element.ToString());
            Assert.AreEqual("testtag4", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag6 testattr=\"testvalue\">>", out element));
            Assert.AreEqual("<testtag6 testattr=\"testvalue\">", element.ToString());
            Assert.AreEqual("testtag6", element.Name);
            Assert.AreEqual(1, element.Attributes.Count);
            Assert.AreEqual("testvalue", element.Attributes["testattr"]);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag7 testattr1=\"testvalue1\" testattr2=\"testvalue2\">test</testtag7>", out element));
            Assert.AreEqual("<testtag7 testattr1=\"testvalue1\" testattr2=\"testvalue2\">test</testtag7>", element.ToString());
            Assert.AreEqual("testtag7", element.Name);
            Assert.AreEqual(2, element.Attributes.Count);
            Assert.AreEqual("testvalue1", element.Attributes["testattr1"]);
            Assert.AreEqual("testvalue2", element.Attributes["testattr2"]);
        }

        /// <summary>
        /// TryParseXmlElementメソッドテストケース（大文字小文字）。
        /// </summary>
        [Test]
        public void TestTryParseXmlElementIgnoreCase()
        {
            XmlElement element;
            XmlParser parser = new XmlParser { IgnoreCase = false };
            Assert.IsTrue(parser.TryParseXmlElement("<testtag></testtag></Testtag>", out element));
            Assert.AreEqual("<testtag></testtag>", element.ToString());
            Assert.AreEqual("testtag", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseXmlElement("<testtag></Testtag></testtag>", out element));
            Assert.AreEqual("<testtag></Testtag></testtag>", element.ToString());
            Assert.AreEqual("testtag", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);

            parser.IgnoreCase = true;
            Assert.IsTrue(parser.TryParseXmlElement("<testtag></Testtag></testtag>", out element));
            Assert.AreEqual("<testtag></Testtag>", element.ToString());
            Assert.AreEqual("testtag", element.Name);
            Assert.AreEqual(0, element.Attributes.Count);
        }

        #endregion
    }
}
