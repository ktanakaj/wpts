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
        // TODO: いっぱい足りない

        #region インスタンス実装メソッドテストケース

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            IElement element;
            ListElement list;
            MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en"));

            Assert.IsTrue(parser.TryParse("'''Article Name''' is [[xxx]]\r\n==test head==\r\n<p>test</p><nowiki>[[test]]</nowiki><!--comment-->{{reflist}}", out element));
            Assert.IsInstanceOf(typeof(ListElement), element);
            list = (ListElement)element;
            Assert.AreEqual(8, list.Count);
            Assert.AreEqual("'''Article Name''' is ", list[0].ToString());
            Assert.AreEqual("[[xxx]]", list[1].ToString());
            Assert.AreEqual("\r\n", list[2].ToString());
            Assert.AreEqual("==test head==", list[3].ToString());
            Assert.AreEqual("\r\n<p>test</p>", list[4].ToString());
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", list[5].ToString());
            Assert.AreEqual("<!--comment-->", list[6].ToString());
            Assert.AreEqual("{{reflist}}", list[7].ToString());
        }

        #endregion
    }
}
