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
            Assert.IsFalse(parser.TryParse("<nowik>[[test]]</nowik>", out element));
            Assert.IsTrue(parser.TryParse("<nowiki attr=\"Value\">[[test]]</nowiki>", out element));
            Assert.AreEqual("<nowiki attr=\"Value\">[[test]]</nowiki>", element.ToString());
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
            MediaWikiNowikiParser parser = new MediaWikiNowikiParser(new MediaWikiParser(new MockFactory().GetMediaWiki("en")));

            Assert.IsFalse(parser.TryParse("<ref name=\"oscars.org\">{{cite web |url=http://www.oscars.org/awards/academyawards/legacy/ceremony/59th-winners.html |title=The 59th Academy Awards (1987) Nominees and Winners |accessdate=2011-07-23|work=oscars.org}}</ref> | owner = [[Discovery Communications|Discovery Communications, Inc.]] | CEO = David Zaslav | headquarters = [[Silver Spring, Maryland]] | country = Worldwide | language = English | sister names = [[TLC (TV channel)|TLC]]<br>[[Animal Planet]]<br>[[OWN: Oprah Winfrey Network]]<br>[[Planet Green]]<br>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse("<br>[[Animal Planet]]<br>[[OWN: Oprah Winfrey Network]]<br>[[Planet Green]]", out element));
            Assert.IsNull(element);
        }

        #endregion
    }
}
