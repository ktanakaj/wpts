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

    /// <summary>
    /// MediaWikiParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiParserTest
    {
        #region 公開静的メソッドテストケース
        
        /// <summary>
        /// TryParseNowikiメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseNowiki()
        {
            string nowiki;
            Assert.IsTrue(MediaWikiParser.TryParseNowiki("<nowiki>[[test]]</nowiki>", out nowiki));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", nowiki);
            Assert.IsTrue(MediaWikiParser.TryParseNowiki("<NOWIKI>[[test]]</NOWIKI>", out nowiki));
            Assert.AreEqual("<NOWIKI>[[test]]</NOWIKI>", nowiki);
            Assert.IsTrue(MediaWikiParser.TryParseNowiki("<Nowiki>[[test]]</noWiki>", out nowiki));
            Assert.AreEqual("<Nowiki>[[test]]</noWiki>", nowiki);
            Assert.IsTrue(MediaWikiParser.TryParseNowiki("<nowiki>[[test]]</nowiki></nowiki>", out nowiki));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", nowiki);
            Assert.IsTrue(MediaWikiParser.TryParseNowiki("<nowiki>[[test]]nowiki", out nowiki));
            Assert.AreEqual("<nowiki>[[test]]nowiki", nowiki);
            Assert.IsTrue(MediaWikiParser.TryParseNowiki("<nowiki>\n\n[[test]]\r\n</nowiki>", out nowiki));
            Assert.AreEqual("<nowiki>\n\n[[test]]\r\n</nowiki>", nowiki);
            Assert.IsTrue(MediaWikiParser.TryParseNowiki("<nowiki><!--[[test]]--></nowiki>", out nowiki));
            Assert.AreEqual("<nowiki><!--[[test]]--></nowiki>", nowiki);
            Assert.IsTrue(MediaWikiParser.TryParseNowiki("<nowiki><!--<nowiki>[[test]]</nowiki>--></nowiki>", out nowiki));
            Assert.AreEqual("<nowiki><!--<nowiki>[[test]]</nowiki>--></nowiki>", nowiki);
            Assert.IsTrue(MediaWikiParser.TryParseNowiki("<nowiki><!--[[test]]", out nowiki));
            Assert.AreEqual("<nowiki><!--[[test]]", nowiki);
            Assert.IsFalse(MediaWikiParser.TryParseNowiki("<nowik>[[test]]</nowik>", out nowiki));
            Assert.IsNull(nowiki);
            Assert.IsFalse(MediaWikiParser.TryParseNowiki("<nowiki[[test]]</nowiki>", out nowiki));
            Assert.IsNull(nowiki);
        }

        #endregion
    }
}
