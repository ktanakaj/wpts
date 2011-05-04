// ================================================================================================
// <summary>
//      MediaWikiNowikiParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiNowikiParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using NUnit.Framework;
    using Honememo.Parsers;
    using Honememo.Wptscs.Models;

    /// <summary>
    /// MediaWikiNowikiParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiNowikiParserTest
    {
        #region インスタンス実装メソッドテストケース

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            IElement element;
            MediaWikiNowikiParser parser = new MediaWikiNowikiParser(new MediaWikiParser(new MockFactory().GetMediaWiki("en")));

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
            Assert.AreEqual("<nowiki><!--<nowiki>[[test]]</nowiki>--></nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParse("<nowiki><!--[[test]]", out element));
            Assert.AreEqual("<nowiki><!--[[test]]", element.ToString());
            Assert.IsFalse(parser.TryParse("<nowik>[[test]]</nowik>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<nowiki[[test]]</nowiki>", out element));
            Assert.IsNull(element);
        }

        #endregion
    }
}
