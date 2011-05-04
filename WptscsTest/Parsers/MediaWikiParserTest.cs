// ================================================================================================
// <summary>
//      MediaWikiParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiParserTest.cs" company="honeplusのメモ帳">
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
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// MediaWikiParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiParserTest
    {
        #region 公開メソッドテストケース

        /// <summary>
        /// TryParseNowikiメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseNowiki()
        {
            XmlElement element;
            MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en"));

            Assert.IsTrue(parser.TryParseNowiki("<nowiki>[[test]]</nowiki>", out element));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<NOWIKI>[[test]]</NOWIKI>", out element));
            Assert.AreEqual("<NOWIKI>[[test]]</NOWIKI>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<Nowiki>[[test]]</noWiki>", out element));
            Assert.AreEqual("<Nowiki>[[test]]</noWiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki>[[test]]</nowiki></nowiki>", out element));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki>[[test]]nowiki", out element));
            Assert.AreEqual("<nowiki>[[test]]nowiki", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki>\n\n[[test]]\r\n</nowiki>", out element));
            Assert.AreEqual("<nowiki>\n\n[[test]]\r\n</nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki><!--[[test]]--></nowiki>", out element));
            Assert.AreEqual("<nowiki><!--[[test]]--></nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki><!--<nowiki>[[test]]</nowiki>--></nowiki>", out element));
            Assert.AreEqual("<nowiki><!--<nowiki>[[test]]</nowiki>--></nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki><!--[[test]]", out element));
            Assert.AreEqual("<nowiki><!--[[test]]", element.ToString());
            Assert.IsFalse(parser.TryParseNowiki("<nowik>[[test]]</nowik>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseNowiki("<nowiki[[test]]</nowiki>", out element));
            Assert.IsNull(element);
        }

        #endregion
    }
}
