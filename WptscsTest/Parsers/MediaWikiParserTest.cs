// ================================================================================================
// <summary>
//      MediaWikiParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using Honememo.Parsers;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Websites;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MediaWikiParser"/>のテストクラスです。
    /// </summary>
    [TestClass]
    internal class MediaWikiParserTest
    {
        #region 公開プロパティテストケース

        /// <summary>
        /// <see cref="MediaWikiParser.Website"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestWebsite()
        {
            MediaWiki site = new MediaWiki(new Language("en"));
            using (MediaWikiParser parser = new MediaWikiParser(site))
            {
                // コンストラクタで指定したオブジェクトが格納されていること
                Assert.AreSame(site, parser.Website);

                // 設定すればそのオブジェクトが入ること
                site = new MediaWiki(new Language("ja"));
                parser.Website = site;
                Assert.AreSame(site, parser.Website);
            }
        }

        /// <summary>
        /// <see cref="MediaWikiParser.Website"/>プロパティテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWebsiteNull()
        {
            using (MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en")))
            {
                parser.Website = null;
            }
        }

        #endregion

        #region ITextParserインタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiParser.TryParseToEndCondition"/>
        /// メソッドテストケース（実際のデータを想定）。
        /// </summary>
        [TestMethod]
        public void TestTryParseToEndCondition()
        {
            string text = "'''Article Name''' is [[xxx]]\r\n==test head==\r\n<p>test</p><nowiki>[[TestMethod]]</nowiki><!--comment-->{{reflist}}";
            IElement element;
            using (MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en")))
            {
                // 実際に想定されるようなデータ
                Assert.IsTrue(parser.TryParseToEndCondition(text, null, out element));
            }

            Assert.AreEqual(text, element.ToString());
            Assert.IsInstanceOfType(element, typeof(ListElement));
            ListElement list = (ListElement)element;
            Assert.AreEqual(8, list.Count);
            Assert.AreEqual("'''Article Name''' is ", list[0].ToString());
            Assert.AreEqual("[[xxx]]", list[1].ToString());
            Assert.AreEqual("\r\n", list[2].ToString());
            Assert.AreEqual("==test head==", list[3].ToString());
            Assert.AreEqual("\r\n<p>test</p>", list[4].ToString());
            Assert.AreEqual("<nowiki>[[TestMethod]]</nowiki>", list[5].ToString());
            Assert.AreEqual("<!--comment-->", list[6].ToString());
            Assert.AreEqual("{{reflist}}", list[7].ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiParser.TryParseToEndCondition"/>
        /// メソッドテストケース（その他のケース）。
        /// </summary>
        [TestMethod]
        public void TestTryParseToEndConditionEmpty()
        {
            IElement element;
            using (MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en")))
            {
                // 空文字列、一応解析成功となる
                Assert.IsTrue(parser.TryParseToEndCondition(string.Empty, null, out element));
                Assert.AreEqual(string.Empty, element.ToString());
                Assert.IsInstanceOfType(element, typeof(TextElement));

                // nullは解析失敗
                Assert.IsFalse(parser.TryParseToEndCondition(null, null, out element));
                Assert.IsNull(element);
            }
        }

        /// <summary>
        /// <see cref="MediaWikiParser.TryParseToEndCondition"/>
        /// メソッドテストケース（終了条件）。
        /// </summary>
        [TestMethod]
        public void TestTryParseToEndConditionCondition()
        {
            // 親クラスにあった終了条件で停止する動作が継承先でも動作していること
            string text = "'''Article Name''' is [[xxx]]\r\n==test head==\r\n<p>test</p><nowiki>[[TestMethod]]</nowiki><!--comment-->{{reflist}}";
            IElement element;
            using (MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en")))
            {
                Assert.IsTrue(parser.TryParseToEndCondition(text, (string s, int index) => s[index] == '/', out element));
            }

            Assert.AreEqual("'''Article Name''' is [[xxx]]\r\n==test head==\r\n<p>test<", element.ToString());
            Assert.IsInstanceOfType(element, typeof(ListElement));
            ListElement list = (ListElement)element;
            Assert.AreEqual(5, list.Count);
        }

        /// <summary>
        /// <see cref="MediaWikiParser.TryParseToEndCondition"/>
        /// メソッドテストケース（テンプレートページ実データ使用）。
        /// </summary>
        /// <remarks>
        /// Ver 1.11にて解析失敗時のリトライにより極端に時間がかかっていたデータ。
        /// 中身についてはほぼ処理できない類のものだが、現実的な時間で解析が終わることだけ検証。
        /// </remarks>
        [TestMethod, Timeout(20000)]
        public void TestTryParseToEndConditionTemplateContext()
        {
            IElement element;
            using (MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en")))
            {
                string text = parser.Website.GetPage("Template:context").Text;
                Assert.IsTrue(parser.TryParseToEndCondition(text, null, out element));
                Assert.IsInstanceOfType(element, typeof(ListElement));
                Assert.AreEqual(text, element.ToString());
            }
        }

        /// <summary>
        /// <see cref="MediaWikiParser.TryParseToEndCondition"/>
        /// メソッドテストケース（Dispose）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestTryParseToEndConditionDispose()
        {
            MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en"));
            parser.Dispose();
            IElement result;
            parser.TryParseToEndCondition(string.Empty, null, out result);
        }

        #endregion
    }
}
