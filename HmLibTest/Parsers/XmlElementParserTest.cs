// ================================================================================================
// <summary>
//      XmlElementParserのテストクラスソース。</summary>
//
// <copyright file="XmlElementParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// <see cref="XmlElementParser"/>のテストクラスです。
    /// </summary>
    [TestFixture]
    internal class XmlElementParserTest
    {
        #region private変数

        /// <summary>
        /// 前処理・後処理で毎回生成／解放される<see cref="XmlParser"/>。
        /// </summary>
        private XmlParser xmlParser;

        #endregion

        #region 前処理・後処理

        /// <summary>
        /// テストの前処理。
        /// </summary>
        /// <remarks><see cref="XmlParser.Dispose"/>が必要な<see cref="XmlParser"/>の生成。</remarks>
        [SetUp]
        public void SetUp()
        {
            this.xmlParser = new XmlParser();
        }

        /// <summary>
        /// テストの後処理。
        /// </summary>
        /// <remarks><see cref="XmlParser.Dispose"/>が必要な<see cref="XmlParser"/>の解放。</remarks>
        [TearDown]
        public void TearDown()
        {
            this.xmlParser.Dispose();
        }

        #endregion

        #region 公開プロパティテストケース

        /// <summary>
        /// <see cref="XmlElementParser.Targets"/>プロパティテストケース。
        /// </summary>
        [Test]
        public void TestTargets()
        {
            XmlElementParser parser = new XmlElementParser(this.xmlParser);

            // 初期状態でオブジェクトが存在すること
            Assert.IsNotNull(parser.Targets);
            Assert.AreEqual(0, parser.Targets.Count);
            parser.Targets.Add("span");
            Assert.AreEqual("span", parser.Targets[0]);

            // 設定すればそのオブジェクトが入ること
            string[] targets = new string[] { "div", "p" };
            parser.Targets = targets;
            Assert.AreSame(targets, parser.Targets);
        }

        /// <summary>
        /// <see cref="XmlElementParser.Targets"/>プロパティテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTargetsNull()
        {
            new XmlElementParser(this.xmlParser).Targets = null;
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="XmlElementParser.TryParse"/>メソッドテストケース（実例）。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            IElement element;
            XmlElement xmlElement;
            XmlElementParser parser = new XmlElementParser(this.xmlParser);

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

            this.xmlParser.IsHtml = true;
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
            Assert.AreEqual(1, xmlElement.Attributes.Count);
            Assert.AreEqual("outer", xmlElement.Attributes["id"]);
        }

        /// <summary>
        /// <see cref="XmlElementParser.TryParse"/>メソッドテストケース（基本形）。
        /// </summary>
        [Test]
        public void TestTryParseNormal()
        {
            IElement element;
            XmlElement xmlElement;
            XmlElementParser parser = new XmlElementParser(this.xmlParser);

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

            Assert.IsTrue(parser.TryParse("<testtag5 testattr2='testvalue2'>testbody</testtag5 >testend", out element));
            xmlElement = (XmlElement)element;
            Assert.IsInstanceOf(typeof(XmlTextElement), xmlElement[0]);
            Assert.AreEqual("<testtag5 testattr2='testvalue2'>testbody</testtag5 >", xmlElement.ToString());
            Assert.AreEqual("testtag5", xmlElement.Name);
            Assert.AreEqual(1, xmlElement.Attributes.Count);
            Assert.AreEqual("testvalue2", xmlElement.Attributes["testattr2"]);
        }

        /// <summary>
        /// <see cref="XmlElementParser.TryParse"/>メソッドテストケース（普通でNGパターン）。
        /// </summary>
        [Test]
        public void TestTryParseNormalNg()
        {
            IElement element;
            XmlElementParser parser = new XmlElementParser(this.xmlParser);

            Assert.IsFalse(parser.TryParse(" <testtag></testtag>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<!-- comment -->", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse(String.Empty, out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse(null, out element));
            Assert.IsNull(element);
        }

        /// <summary>
        /// <see cref="XmlElementParser.TryParse"/>メソッドテストケース（単一のパターン）。
        /// </summary>
        [Test]
        public void TestTryParseSingle()
        {
            IElement element;
            XmlElement xmlElement;
            XmlElementParser parser = new XmlElementParser(this.xmlParser);

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
        /// <see cref="XmlElementParser.TryParse"/>メソッドテストケース（不正な構文）。
        /// </summary>
        [Test]
        public void TestTryParseLazy()
        {
            IElement element;
            XmlElement xmlElement;
            XmlElementParser parser = new XmlElementParser(this.xmlParser);

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
        /// <see cref="XmlElementParser.TryParse"/>メソッドテストケース（不正でNG）。
        /// </summary>
        [Test]
        public void TestTryParseLazyNg()
        {
            IElement element;
            XmlElementParser parser = new XmlElementParser(this.xmlParser);

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
        /// <see cref="XmlElementParser.TryParse"/>メソッドテストケース（HTML）。
        /// </summary>
        [Test]
        public void TestTryParseHtml()
        {
            IElement element;
            HtmlElement htmlElement;
            this.xmlParser.IsHtml = true;
            XmlElementParser parser = new XmlElementParser(this.xmlParser);

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
        /// <see cref="XmlElementParser.TryParse"/>メソッドテストケース（大文字小文字）。
        /// </summary>
        [Test]
        public void TestTryParseIgnoreCase()
        {
            IElement element;
            XmlElement xmlElement;
            this.xmlParser.IgnoreCase = false;
            XmlElementParser parser = new XmlElementParser(this.xmlParser);

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

            this.xmlParser.IgnoreCase = true;
            Assert.IsTrue(parser.TryParse("<testtag></Testtag></testtag>", out element));
            xmlElement = (XmlElement)element;
            Assert.AreEqual("<testtag></Testtag>", xmlElement.ToString());
            Assert.AreEqual("testtag", xmlElement.Name);
            Assert.AreEqual(0, xmlElement.Attributes.Count);
        }

        /// <summary>
        /// <see cref="XmlElementParser.TryParse"/>メソッドテストケース（タグ限定）。
        /// </summary>
        [Test]
        public void TestTryParseTargets()
        {
            IElement element;
            XmlElementParser parser = new XmlElementParser(this.xmlParser);

            // 特定のタグのみを処理対象とするよう指定する
            parser.Targets = new string[] { "div", "span" };
            Assert.IsFalse(parser.TryParse("<h1>test</h1>", out element));
            Assert.IsFalse(parser.TryParse("<br />", out element));
            Assert.IsTrue(parser.TryParse("<div>test</div>", out element));
            Assert.AreEqual("<div>test</div>", element.ToString());

            // XmlParserに大文字小文字無視が指定されている場合、ここも無視する
            Assert.IsTrue(parser.TryParse("<sPan>test</span>", out element));
            Assert.AreEqual("<sPan>test</span>", element.ToString());

            // 指定されていない場合、区別する
            this.xmlParser.IgnoreCase = false;
            Assert.IsFalse(parser.TryParse("<sPan>test</span>", out element));
            Assert.IsTrue(parser.TryParse("<span>test</span>", out element));
            Assert.AreEqual("<span>test</span>", element.ToString());
        }

        /// <summary>
        /// <see cref="XmlElementParser.IsPossibleParse"/>メソッドテストケース。
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
