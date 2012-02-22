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
    using NUnit.Framework;

    /// <summary>
    /// MediaWikiNowikiParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiNowikiParserTest
    {
        #region private変数

        /// <summary>
        /// 前処理・後処理で生成／解放される言語別のMediaWikiParser。
        /// </summary>
        private IDictionary<string, MediaWikiParser> mediaWikiParsers = new Dictionary<string, MediaWikiParser>();

        #endregion

        #region 前処理・後処理

        /// <summary>
        /// テストの前処理。
        /// </summary>
        [TestFixtureSetUp]
        public void SetUpBeforeClass()
        {
            // Disposeが必要なMediaWikiParserの生成／解放
            this.mediaWikiParsers["en"] = new MediaWikiParser(new MockFactory().GetMediaWiki("en"));
        }

        /// <summary>
        /// テストの後処理。
        /// </summary>
        [TestFixtureTearDown]
        public void TearDownAfterClass()
        {
            // Disposeが必要なMediaWikiParserの生成／解放
            foreach (MediaWikiParser parser in this.mediaWikiParsers.Values)
            {
                parser.Dispose();
            }

            this.mediaWikiParsers.Clear();
        }

        #endregion

        #region インスタンス実装メソッドテストケース

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            IElement element;
            MediaWikiNowikiParser parser = new MediaWikiNowikiParser(this.mediaWikiParsers["en"]);

            // nowikiは大文字小文字区別せず動作、閉じタグが無くても機能する
            // （その判断はMediaWikiNowikiParserではなくMediaWikiParserでの設定次第によるものだが）
            // 属性値などが指定されていても機能する
            // nowiki区間ではコメントも機能しない
            Assert.IsTrue(parser.TryParse("<nowiki>[[test]]</nowiki>", out element));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", element.ToString());
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
            Assert.IsFalse(parser.TryParse("<nowik>[[test]]</nowik>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<nowiki[[test]]</nowiki>", out element));
            Assert.IsNull(element);
        }

        /// <summary>
        /// TryParseメソッドテストケース（nowiki以外のタグ）。
        /// </summary>
        /// <remarks>nowiki以外の場合に変な動きをしているようなバグがあったので。</remarks>
        [Test]
        public void TestTryParseNg()
        {
            IElement element;
            MediaWikiNowikiParser parser = new MediaWikiNowikiParser(this.mediaWikiParsers["en"]);

            Assert.IsFalse(parser.TryParse("<ref name=\"oscars.org\">{{cite web |url=http://www.oscars.org/awards/academyawards/legacy/ceremony/59th-winners.html |title=The 59th Academy Awards (1987) Nominees and Winners |accessdate=2011-07-23|work=oscars.org}}</ref> | owner = [[Discovery Communications|Discovery Communications, Inc.]] | CEO = David Zaslav | headquarters = [[Silver Spring, Maryland]] | country = Worldwide | language = English | sister names = [[TLC (TV channel)|TLC]]<br>[[Animal Planet]]<br>[[OWN: Oprah Winfrey Network]]<br>[[Planet Green]]<br>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<br>[[Animal Planet]]<br>[[OWN: Oprah Winfrey Network]]<br>[[Planet Green]]", out element));
            Assert.IsNull(element);
        }

        #endregion
    }
}
