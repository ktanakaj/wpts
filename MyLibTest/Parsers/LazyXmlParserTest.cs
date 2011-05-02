﻿// ================================================================================================
// <summary>
//      LazyXmlParserのテストクラスソース。</summary>
//
// <copyright file="LazyXmlParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// LazyXmlParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class LazyXmlParserTest
    {
        #region 公開メソッドテストケース
        
        /// <summary>
        /// TryParseメソッドテストケース（実例）。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            XmlElement element;
            LazyXmlParser parser = new LazyXmlParser();
            Assert.IsTrue(parser.TryParseTag("<h1>test</h1>", out element));
            //Assert.AreEqual("<h1>test</h1>", element.OuterXml);
            Assert.AreEqual("h1", element.Name);
            //Assert.AreEqual("test", element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<br /><br />test<br />", out element));
            //Assert.AreEqual("<br />", element.OuterXml);
            Assert.AreEqual("br", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<div id=\"testid\" name=\"testname\"><!--<div id=\"indiv\">test</div>--></div><br />", out element));
            //Assert.AreEqual("<div id=\"testid\" name=\"testname\"><!--<div id=\"indiv\">test</div>--></div>", element.OuterXml);
            Assert.AreEqual("div", element.Name);
            //Assert.AreEqual("<!--<div id=\"indiv\">test</div>-->", element.InnerXml);
            Assert.AreEqual(2, element.Attributes.Count);
            //Assert.AreEqual("testid", element.GetAttribute("id"));
            //Assert.AreEqual("testname", element.GetAttribute("name"));

            parser.IsHtml = true;
            Assert.IsTrue(parser.TryParseTag("<p>段落1<p>段落2", out element));
            //Assert.AreEqual("<p>", element.OuterXml);
            Assert.AreEqual("p", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<input type=\"checkbox\" name=\"param\" value=\"test\" checked><label for=\"param\">チェック</label>", out element));
            //Assert.AreEqual("<input type=\"checkbox\" name=\"param\" value=\"test\" checked>", element.OuterXml);
            Assert.AreEqual("input", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(4, element.Attributes.Count);
            //Assert.AreEqual("checkbox", element.GetAttribute("type"));
            //Assert.AreEqual("param", element.GetAttribute("name"));
            //Assert.AreEqual("test", element.GetAttribute("value"));
            //Assert.IsEmpty(element.GetAttribute("checked"));
        }

        /// <summary>
        /// TryParseメソッドテストケース（基本形）。
        /// </summary>
        [Test]
        public void TestTryParseNormal()
        {
            XmlElement element;
            LazyXmlParser parser = new LazyXmlParser();
            Assert.IsTrue(parser.TryParseTag("<testtag></testtag>", out element));
            //Assert.AreEqual("<testtag></testtag>", element.OuterXml);
            Assert.AreEqual("testtag", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag2>test value</testtag2>", out element));
            //Assert.AreEqual("<testtag2>test value</testtag2>", element.OuterXml);
            Assert.AreEqual("testtag2", element.Name);
            //Assert.AreEqual("test value", element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag3> test value2 </testtag3>testend", out element));
            //Assert.AreEqual("<testtag3> test value2 </testtag3>", element.OuterXml);
            Assert.AreEqual("testtag3", element.Name);
            //Assert.AreEqual(" test value2 ", element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag4 testattr=\"testvalue\"> test<!-- </testtag4> --> value3 </testtag4>testend", out element));
            //Assert.AreEqual("<testtag4 testattr=\"testvalue\"> test<!-- </testtag4> --> value3 </testtag4>", element.OuterXml);
            Assert.AreEqual("testtag4", element.Name);
            //Assert.AreEqual(" test<!-- </testtag4> --> value3 ", element.InnerXml);
            Assert.AreEqual(1, element.Attributes.Count);
            //Assert.AreEqual("testvalue", element.GetAttribute("testattr"));

            Assert.IsTrue(parser.TryParseTag("<testtag5 testattr2='testvalue2'><testbody></testtag5 >testend", out element));
            //Assert.AreEqual("<testtag5 testattr2='testvalue2'><testbody></testtag5 >", element.OuterXml);
            Assert.AreEqual("testtag5", element.Name);
            //Assert.AreEqual("<testbody>", element.InnerXml);
            Assert.AreEqual(1, element.Attributes.Count);
            //Assert.AreEqual("testvalue2", element.GetAttribute("testattr2"));
        }

        /// <summary>
        /// TryParseメソッドテストケース（普通でNGパターン）。
        /// </summary>
        [Test]
        public void TestTryParseNormalNg()
        {
            XmlElement element;
            LazyXmlParser parser = new LazyXmlParser();
            Assert.IsFalse(parser.TryParseTag(" <testtag></testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseTag("<!-- comment -->", out element));
            Assert.IsNull(element);
        }

        /// <summary>
        /// TryParseメソッドテストケース（単一のパターン）。
        /// </summary>
        [Test]
        public void TestTryParseSingle()
        {
            XmlElement element;
            LazyXmlParser parser = new LazyXmlParser();
            Assert.IsTrue(parser.TryParseTag("<testtag />", out element));
            //Assert.AreEqual("<testtag />", element.OuterXml);
            Assert.AreEqual("testtag", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag2/>", out element));
            //Assert.AreEqual("<testtag2/>", element.OuterXml);
            Assert.AreEqual("testtag2", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag3   />testtag4 />", out element));
            //Assert.AreEqual("<testtag3   />", element.OuterXml);
            Assert.AreEqual("testtag3", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag5 testattr=\"testvalue\" />/>", out element));
            //Assert.AreEqual("<testtag5 testattr=\"testvalue\" />", element.OuterXml);
            Assert.AreEqual("testtag5", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(1, element.Attributes.Count);
            //Assert.AreEqual("testvalue", element.GetAttribute("testattr"));

            Assert.IsTrue(parser.TryParseTag("<testtag6 testattr1=\"testvalue1\" testattr2=\"testvalue2\"/>/>", out element));
            //Assert.AreEqual("<testtag6 testattr1=\"testvalue1\" testattr2=\"testvalue2\"/>", element.OuterXml);
            Assert.AreEqual("testtag6", element.Name);
            Assert.AreEqual(2, element.Attributes.Count);
            //Assert.AreEqual("testvalue1", element.GetAttribute("testattr1"));
            //Assert.AreEqual("testvalue2", element.GetAttribute("testattr2"));
        }

        /// <summary>
        /// TryParseメソッドテストケース（不正な構文）。
        /// </summary>
        [Test]
        public void TestTryParseLazy()
        {
            XmlElement element;
            LazyXmlParser parser = new LazyXmlParser();
            Assert.IsTrue(parser.TryParseTag("<p>", out element));
            //Assert.AreEqual("<p>", element.OuterXml);
            Assert.AreEqual("p", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag>test value", out element));
            //Assert.AreEqual("<testtag>test value", element.OuterXml);
            Assert.AreEqual("testtag", element.Name);
            //Assert.AreEqual("test value", element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag2 testattr=test value>test value</testtag2>", out element));
            //Assert.AreEqual("<testtag2 testattr=test value>test value</testtag2>", element.OuterXml);
            Assert.AreEqual("testtag2", element.Name);
            //Assert.AreEqual("test value", element.InnerXml);
            Assert.AreEqual(2, element.Attributes.Count);
            //Assert.AreEqual("test", element.GetAttribute("testattr"));
            //Assert.IsEmpty(element.GetAttribute("value"));

            Assert.IsTrue(parser.TryParseTag("<testtag3>test value2</ testtag3>testend", out element));
            //Assert.AreEqual("<testtag3>test value2</ testtag3>testend", element.OuterXml);
            Assert.AreEqual("testtag3", element.Name);
            //Assert.AreEqual("test value2</ testtag3>testend", element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);
        }


        /// <summary>
        /// TryParseメソッドテストケース（不正でNG）。
        /// </summary>
        [Test]
        public void TestTryParseLazyNg()
        {
            XmlElement element;
            LazyXmlParser parser = new LazyXmlParser();
            Assert.IsFalse(parser.TryParseTag("< testtag></testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseTag("<testtag", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseTag("<testtag ", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseTag("<testtag /", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseTag("<testtag </testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseTag("<testtag testattr=\"testvalue'></testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseTag("<sub((>4</sub>", out element));
            Assert.IsNull(element);
        }

        /// <summary>
        /// TryParseメソッドテストケース（HTML）。
        /// </summary>
        [Test]
        public void TestTryParseHtml()
        {
            XmlElement element;
            LazyXmlParser parser = new LazyXmlParser { IsHtml = true };
            Assert.IsTrue(parser.TryParseTag("<testtag />", out element));
            //Assert.AreEqual("<testtag />", element.OuterXml);
            Assert.AreEqual("testtag", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<p>", out element));
            //Assert.AreEqual("<p>", element.OuterXml);
            Assert.AreEqual("p", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag2 />testtag3 />", out element));
            //Assert.AreEqual("<testtag2 />", element.OuterXml);
            Assert.AreEqual("testtag2", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag4></testtag5>", out element));
            //Assert.AreEqual("<testtag4>", element.OuterXml);
            Assert.AreEqual("testtag4", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag6 testattr=\"testvalue\">>", out element));
            //Assert.AreEqual("<testtag6 testattr=\"testvalue\">", element.OuterXml);
            Assert.AreEqual("testtag6", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(1, element.Attributes.Count);
            //Assert.AreEqual("testvalue", element.GetAttribute("testattr"));

            Assert.IsTrue(parser.TryParseTag("<testtag7 testattr1=\"testvalue1\" testattr2=\"testvalue2\">test</testtag7>", out element));
            //Assert.AreEqual("<testtag7 testattr1=\"testvalue1\" testattr2=\"testvalue2\">test</testtag7>", element.OuterXml);
            Assert.AreEqual("testtag7", element.Name);
            Assert.AreEqual(2, element.Attributes.Count);
            //Assert.AreEqual("testvalue1", element.GetAttribute("testattr1"));
            //Assert.AreEqual("testvalue2", element.GetAttribute("testattr2"));
        }

        /// <summary>
        /// TryParseメソッドテストケース（大文字小文字）。
        /// </summary>
        [Test]
        public void TestTryParseIgnoreCase()
        {
            XmlElement element;
            LazyXmlParser parser = new LazyXmlParser { IgnoreCase = false };
            Assert.IsTrue(parser.TryParseTag("<testtag></testtag></Testtag>", out element));
            //Assert.AreEqual("<testtag></testtag>", element.OuterXml);
            Assert.AreEqual("testtag", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            Assert.IsTrue(parser.TryParseTag("<testtag></Testtag></testtag>", out element));
            //Assert.AreEqual("<testtag></Testtag></testtag>", element.OuterXml);
            Assert.AreEqual("testtag", element.Name);
            //Assert.AreEqual("</Testtag>", element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);

            parser.IgnoreCase = true;
            Assert.IsTrue(parser.TryParseTag("<testtag></Testtag></testtag>", out element));
            //Assert.AreEqual("<testtag></Testtag>", element.OuterXml);
            Assert.AreEqual("testtag", element.Name);
            //Assert.IsEmpty(element.InnerXml);
            Assert.AreEqual(0, element.Attributes.Count);
        }

        #endregion
    }
}