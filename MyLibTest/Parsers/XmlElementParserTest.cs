// ================================================================================================
// <summary>
//      XmlElementParserのテストクラスソース。</summary>
//
// <copyright file="XmlElementParserTest.cs" company="honeplusのメモ帳">
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
    /// XmlElementParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class XmlElementParserTest
    {
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// TryParseメソッドテストケース（実例）。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            IElement element;
            XmlElement xmlElement;
            XmlParser xmlParser = new XmlParser();
            XmlElementParser parser = new XmlElementParser(xmlParser);

            Assert.IsTrue(parser.TryParse("<h1>test</h1>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<h1>test</h1>", xmlElement.ToString());
            Assert.AreEqual("h1", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<br /><br />test<br />", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<br />", xmlElement.ToString());
            Assert.AreEqual("br", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<div id=\"testid\" name=\"testname\"><!--<div id=\"indiv\">test</div>--></div><br />", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<div id=\"testid\" name=\"testname\"><!--<div id=\"indiv\">test</div>--></div>", xmlElement.ToString());
            Assert.AreEqual("div", xmlElement.Name);
            Assert.AreEqual(2, xmlElement.Attributes.Count);
            Assert.AreEqual("testid", xmlElement.Attributes["id"]);
            Assert.AreEqual("testname", xmlElement.Attributes["name"]);

            xmlParser.IsHtml = true;
            Assert.IsTrue(parser.TryParse("<p>段落1<p>段落2", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<p>", xmlElement.ToString());
            Assert.AreEqual("p", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<input type=\"checkbox\" name=\"param\" value=\"test\" checked><label for=\"param\">チェック</label>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<input type=\"checkbox\" name=\"param\" value=\"test\" checked>", xmlElement.ToString());
            Assert.AreEqual("input", xmlElement.Name);
            Assert.AreEqual(4, xmlElement.Attributes.Count);
            Assert.AreEqual("checkbox", xmlElement.Attributes["type"]);
            Assert.AreEqual("param", xmlElement.Attributes["name"]);
            Assert.AreEqual("test", xmlElement.Attributes["value"]);
            Assert.IsEmpty(xmlElement.Attributes["checked"]);

            Assert.IsTrue(parser.TryParse("<div id=\"outer\">outertext<div id=\"inner\">innertext</div></div>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<div id=\"outer\">outertext<div id=\"inner\">innertext</div></div>", xmlElement.ToString());
            Assert.AreEqual("div", xmlElement.Name);
            Assert.AreEqual(2, xmlElement.Attributes.Count);
            Assert.AreEqual("outer", xmlElement.Attributes["id"]);
        }

        /// <summary>
        /// TryParseメソッドテストケース（基本形）。
        /// </summary>
        [Test]
        public void TestTryParseNormal()
        {
            IElement element;
            XmlElement xmlElement;
            XmlElementParser parser = new XmlElementParser(new XmlParser());

            Assert.IsTrue(parser.TryParse("<testtag></testtag>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag></testtag>", xmlElement.ToString());
            Assert.AreEqual("testtag", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag2>test value</testtag2>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag2>test value</testtag2>", xmlElement.ToString());
            Assert.AreEqual("testtag2", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag3> test value2 </testtag3>testend", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag3> test value2 </testtag3>", xmlElement.ToString());
            Assert.AreEqual("testtag3", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag4 testattr=\"testvalue\"> test<!-- </testtag4> --> value3 </testtag4>testend", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag4 testattr=\"testvalue\"> test<!-- </testtag4> --> value3 </testtag4>", xmlElement.ToString());
            Assert.AreEqual("testtag4", xmlElement.Name);
            Assert.AreEqual(1, xmlElement.Attributes.Count);
            Assert.AreEqual("testvalue", xmlElement.Attributes["testattr"]);

            Assert.IsTrue(parser.TryParse("<testtag5 testattr2='testvalue2'><testbody></testtag5 >testend", out element));
            xmlElement = (XmlElement)element;
            Assert.IsInstanceOf(typeof(XmlElement), xmlElement[0]);
            Assert.AreEqual("<testtag5 testattr2='testvalue2'><testbody></testtag5 >", xmlElement.ToString());
            Assert.AreEqual("testtag5", xmlElement.Name);
            Assert.AreEqual(1, xmlElement.Attributes.Count);
            Assert.AreEqual("testvalue2", xmlElement.Attributes["testattr2"]);
        }

        /// <summary>
        /// TryParseメソッドテストケース（普通でNGパターン）。
        /// </summary>
        [Test]
        public void TestTryParseNormalNg()
        {
            IElement element;
            XmlElementParser parser = new XmlElementParser(new XmlParser());

            Assert.IsFalse(parser.TryParse(" <testtag></testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<!-- comment -->", out element));
            Assert.IsNull(element);
        }

        /// <summary>
        /// TryParseメソッドテストケース（単一のパターン）。
        /// </summary>
        [Test]
        public void TestTryParseSingle()
        {
            IElement element;
            XmlElement xmlElement;
            XmlElementParser parser = new XmlElementParser(new XmlParser());

            Assert.IsTrue(parser.TryParse("<testtag />", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag />", xmlElement.ToString());
            Assert.AreEqual("testtag", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag2/>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag2/>", xmlElement.ToString());
            Assert.AreEqual("testtag2", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag3   />testtag4 />", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag3   />", xmlElement.ToString());
            Assert.AreEqual("testtag3", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag5 testattr=\"testvalue\" />/>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag5 testattr=\"testvalue\" />", xmlElement.ToString());
            Assert.AreEqual("testtag5", xmlElement.Name);
            Assert.AreEqual(1, xmlElement.Attributes.Count);
            Assert.AreEqual("testvalue", xmlElement.Attributes["testattr"]);

            Assert.IsTrue(parser.TryParse("<testtag6 testattr1=\"testvalue1\" testattr2=\"testvalue2\"/>/>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag6 testattr1=\"testvalue1\" testattr2=\"testvalue2\"/>", xmlElement.ToString());
            Assert.AreEqual("testtag6", xmlElement.Name);
            Assert.AreEqual(2, xmlElement.Attributes.Count);
            Assert.AreEqual("testvalue1", xmlElement.Attributes["testattr1"]);
            Assert.AreEqual("testvalue2", xmlElement.Attributes["testattr2"]);
        }

        /// <summary>
        /// TryParseメソッドテストケース（不正な構文）。
        /// </summary>
        [Test]
        public void TestTryParseLazy()
        {
            IElement element;
            XmlElement xmlElement;
            XmlElementParser parser = new XmlElementParser(new XmlParser());

            Assert.IsTrue(parser.TryParse("<p>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<p>", xmlElement.ToString());
            Assert.AreEqual("p", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag>test value", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag>test value", xmlElement.ToString());
            Assert.AreEqual("testtag", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag2 testattr=test value>test value</testtag2>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag2 testattr=test value>test value</testtag2>", xmlElement.ToString());
            Assert.AreEqual("testtag2", xmlElement.Name);
            Assert.AreEqual(2, xmlElement.Attributes.Count);
            Assert.AreEqual("test", xmlElement.Attributes["testattr"]);
            Assert.IsEmpty(xmlElement.Attributes["value"]);

            Assert.IsTrue(parser.TryParse("<testtag3>test value2</ testtag3>testend", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag3>test value2</ testtag3>testend", xmlElement.ToString());
            Assert.AreEqual("testtag3", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);
        }


        /// <summary>
        /// TryParseメソッドテストケース（不正でNG）。
        /// </summary>
        [Test]
        public void TestTryParseLazyNg()
        {
            IElement element;
            XmlElementParser parser = new XmlElementParser(new XmlParser());

            Assert.IsFalse(parser.TryParse("< testtag></testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<testtag", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<testtag ", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<testtag /", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<testtag </testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<testtag testattr=\"testvalue'></testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<sub((>4</sub>", out element));
            Assert.IsNull(element);
        }

        /// <summary>
        /// TryParseメソッドテストケース（HTML）。
        /// </summary>
        [Test]
        public void TestTryParseHtml()
        {
            IElement element;
            HtmlElement htmlElement;
            XmlElementParser parser = new XmlElementParser(new XmlParser { IsHtml = true });

            Assert.IsTrue(parser.TryParse("<testtag />", out element));
            htmlElement = (HtmlElement)element;
            Assert.AreEqual("<testtag />", htmlElement.ToString());
            Assert.AreEqual("testtag", htmlElement.Name);
            Assert.AreEqual(0, htmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<p>", out element));
            htmlElement = (HtmlElement)element;
            Assert.AreEqual("<p>", htmlElement.ToString());
            Assert.AreEqual("p", htmlElement.Name);
            Assert.AreEqual(0, htmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag2 />testtag3 />", out element));
            htmlElement = (HtmlElement)element;
            Assert.AreEqual("<testtag2 />", htmlElement.ToString());
            Assert.AreEqual("testtag2", htmlElement.Name);
            Assert.AreEqual(0, htmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag4></testtag5>", out element));
            htmlElement = (HtmlElement)element;
            Assert.AreEqual("<testtag4>", htmlElement.ToString());
            Assert.AreEqual("testtag4", htmlElement.Name);
            Assert.AreEqual(0, htmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag6 testattr=\"testvalue\">>", out element));
            htmlElement = (HtmlElement)element;
            Assert.AreEqual("<testtag6 testattr=\"testvalue\">", htmlElement.ToString());
            Assert.AreEqual("testtag6", htmlElement.Name);
            Assert.AreEqual(1, htmlElement.Attributes.Count);
            Assert.AreEqual("testvalue", htmlElement.Attributes["testattr"]);

            Assert.IsTrue(parser.TryParse("<testtag7 testattr1=\"testvalue1\" testattr2=\"testvalue2\">test</testtag7>", out element));
            htmlElement = (HtmlElement)element;
            Assert.AreEqual("<testtag7 testattr1=\"testvalue1\" testattr2=\"testvalue2\">test</testtag7>", htmlElement.ToString());
            Assert.AreEqual("testtag7", htmlElement.Name);
            Assert.AreEqual(2, htmlElement.Attributes.Count);
            Assert.AreEqual("testvalue1", htmlElement.Attributes["testattr1"]);
            Assert.AreEqual("testvalue2", htmlElement.Attributes["testattr2"]);
        }

        /// <summary>
        /// TryParseメソッドテストケース（大文字小文字）。
        /// </summary>
        [Test]
        public void TestTryParseIgnoreCase()
        {
            IElement element;
            XmlElement xmlElement;
            XmlParser xmlParser = new XmlParser { IgnoreCase = false };
            XmlElementParser parser = new XmlElementParser(xmlParser);

            Assert.IsTrue(parser.TryParse("<testtag></testtag></Testtag>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag></testtag>", xmlElement.ToString());
            Assert.AreEqual("testtag", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            Assert.IsTrue(parser.TryParse("<testtag></Testtag></testtag>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag></Testtag></testtag>", xmlElement.ToString());
            Assert.AreEqual("testtag", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);

            xmlParser.IgnoreCase = true;
            Assert.IsTrue(parser.TryParse("<testtag></Testtag></testtag>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag></Testtag>", xmlElement.ToString());
            Assert.AreEqual("testtag", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// IsElementPossibleメソッドテストケース。
        /// </summary>
        [Test]
        public void TestIsElementPossible()
        {
            XmlElementParser parser = new XmlElementParser(new XmlParser());
            Assert.IsTrue(parser.IsPossibleParse('<'));
            Assert.IsFalse(parser.IsPossibleParse('['));
            Assert.IsFalse(parser.IsPossibleParse('-'));
            Assert.IsFalse(parser.IsPossibleParse('/'));
            Assert.IsFalse(parser.IsPossibleParse('#'));
        }

        #endregion
    }
}
