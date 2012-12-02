// ================================================================================================
// <summary>
//      MediaWikiNowikiParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiNowikiParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using Honememo.Parsers;
    using Honememo.Wptscs.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MediaWikiNowikiParser"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class MediaWikiNowikiParserTest
    {
        #region private変数

        /// <summary>
        /// 前処理・後処理で生成／解放される<see cref="XmlParser"/>。
        /// </summary>
        private XmlParser xmlParser;

        #endregion

        #region 前処理・後処理

        /// <summary>
        /// テストの前処理。
        /// </summary>
        /// <remarks><see cref="XmlParser.Dispose"/>が必要な<see cref="XmlParser"/>の生成。</remarks>
        [TestInitialize]
        public void SetUp()
        {
            this.xmlParser = new XmlParser();
        }

        /// <summary>
        /// テストの後処理。
        /// </summary>
        /// <remarks><see cref="XmlParser.Dispose"/>が必要な<see cref="XmlParser"/>の解放。</remarks>
        [TestCleanup]
        public void TearDown()
        {
            this.xmlParser.Dispose();
        }

        #endregion

        #region インスタンス実装メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiNowikiParser.TryParse"/>メソッドテストケース（OKケース）。
        /// </summary>
        [TestMethod]
        public void TestTryParse()
        {
            IElement element;
            XmlElement xml;
            MediaWikiNowikiParser parser = new MediaWikiNowikiParser(this.xmlParser);

            // 基本動作、nowiki区間は再帰的に処理されない
            Assert.IsTrue(parser.TryParse("<nowiki>[[test]]</nowiki>", out element));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", element.ToString());
            Assert.IsInstanceOfType(element, typeof(XmlElement));
            xml = (XmlElement)element;
            Assert.IsInstanceOfType(xml[0], typeof(XmlTextElement));
            Assert.AreEqual("[[test]]", xml[0].ToString());
            Assert.AreEqual(1, xml.Count);

            Assert.IsTrue(parser.TryParse("<noWiki>{{!}}<nowiki>nowikiサンプルのつもり</nowiki>{{!}}</nowiki>", out element));
            Assert.AreEqual("<noWiki>{{!}}<nowiki>nowikiサンプルのつもり</nowiki>", element.ToString());
            Assert.IsInstanceOfType(element, typeof(XmlElement));
            xml = (XmlElement)element;
            Assert.IsInstanceOfType(xml[0], typeof(XmlTextElement));
            Assert.AreEqual("{{!}}<nowiki>nowikiサンプルのつもり", xml[0].ToString());
            Assert.AreEqual(1, xml.Count);

            Assert.IsTrue(parser.TryParse("<nowiki>{{!}}&lt;nowiki&gt;nowikiサンプル&lt;/nowiki&gt;{{!}}</nowiki>", out element));
            Assert.AreEqual("<nowiki>{{!}}&lt;nowiki&gt;nowikiサンプル&lt;/nowiki&gt;{{!}}</nowiki>", element.ToString());
            Assert.IsInstanceOfType(element, typeof(XmlElement));
            xml = (XmlElement)element;
            Assert.IsInstanceOfType(xml[0], typeof(XmlTextElement));
            Assert.AreEqual("{{!}}&lt;nowiki&gt;nowikiサンプル&lt;/nowiki&gt;{{!}}", xml[0].ToString());
            Assert.AreEqual(1, xml.Count);

            // nowikiは大文字小文字区別せず動作、閉じタグが無くても機能する
            // （その判断はMediaWikiNowikiParserではなくMediaWikiParserでの設定次第によるものだが）
            // 属性値などが指定されていても機能する
            // nowiki区間ではコメントも機能しない
            Assert.IsTrue(parser.TryParse("<NOWIKI>[[test]]</NOWIKI>", out element));
            Assert.AreEqual("<NOWIKI>[[test]]</NOWIKI>", element.ToString());

            Assert.IsTrue(parser.TryParse("<Nowiki>[[test]]</noWiki>", out element));
            Assert.AreEqual("<Nowiki>[[test]]</noWiki>", element.ToString());

            Assert.IsTrue(parser.TryParse("<nowiki>[[test]]</nowiki></nowiki>", out element));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", element.ToString());

            Assert.IsTrue(parser.TryParse("<nowiki>[[test]]nowiki", out element));
            Assert.AreEqual("<nowiki>[[test]]nowiki", element.ToString());

            Assert.IsTrue(parser.TryParse("<nowiki>\n\n[[test]]\r\n</nowiki>", out element));
            Assert.AreEqual("<nowiki>\n\n[[test]]\r\n</nowiki>", element.ToString());

            Assert.IsTrue(parser.TryParse("<nowiki><!--[[test]]--></nowiki>", out element));
            Assert.AreEqual("<nowiki><!--[[test]]--></nowiki>", element.ToString());

            Assert.IsTrue(parser.TryParse("<nowiki><!--<nowiki>[[test]]</nowiki>--></nowiki>", out element));
            Assert.AreEqual("<nowiki><!--<nowiki>[[test]]</nowiki>", element.ToString());

            Assert.IsTrue(parser.TryParse("<nowiki><!--[[test]]", out element));
            Assert.AreEqual("<nowiki><!--[[test]]", element.ToString());

            Assert.IsTrue(parser.TryParse("<nowiki attr=\"Value\">[[test]]</nowiki>", out element));
            Assert.AreEqual("<nowiki attr=\"Value\">[[test]]</nowiki>", element.ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiNowikiParser.TryParse"/>メソッドテストケース（NGケース）。
        /// </summary>
        /// <remarks>nowiki以外のタグの場合に変な動きをしているようなバグがあったためその確認も行う。</remarks>
        [TestMethod]
        public void TestTryParseNg()
        {
            IElement element;
            MediaWikiNowikiParser parser = new MediaWikiNowikiParser(this.xmlParser);

            Assert.IsFalse(parser.TryParse("<nowik>[[test]]</nowik>", out element));
            Assert.IsNull(element);

            Assert.IsFalse(parser.TryParse("<nowiki[[test]]</nowiki>", out element));
            Assert.IsNull(element);

            Assert.IsFalse(parser.TryParse(string.Empty, out element));
            Assert.IsNull(element);

            Assert.IsFalse(parser.TryParse(null, out element));
            Assert.IsNull(element);

            Assert.IsFalse(parser.TryParse("<ref name=\"oscars.org\">{{cite web |url=http://www.oscars.org/awards/academyawards/legacy/ceremony/59th-winners.html |title=The 59th Academy Awards (1987) Nominees and Winners |accessdate=2011-07-23|work=oscars.org}}</ref> | owner = [[Discovery Communications|Discovery Communications, Inc.]] | CEO = David Zaslav | headquarters = [[Silver Spring, Maryland]] | country = Worldwide | language = English | sister names = [[TLC (TV channel)|TLC]]<br>[[Animal Planet]]<br>[[OWN: Oprah Winfrey Network]]<br>[[Planet Green]]<br>", out element));
            Assert.IsNull(element);

            Assert.IsFalse(parser.TryParse("<br>[[Animal Planet]]<br>[[OWN: Oprah Winfrey Network]]<br>[[Planet Green]]", out element));
            Assert.IsNull(element);
        }

        #endregion
    }
}
