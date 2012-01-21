// ================================================================================================
// <summary>
//      MediaWikiRedirectParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiRedirectParserTest.cs" company="honeplusのメモ帳">
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
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// MediaWikiRedirectParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiRedirectParserTest
    {
        #region インスタンス実装メソッドテストケース

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            IElement element;
            MediaWikiLink link;
            MediaWikiRedirectParser parser = new MediaWikiRedirectParser(new MockFactory().GetMediaWiki("en"));

            // 通常のリダイレクト
            Assert.IsTrue(parser.TryParse("#redirect [[Test]]", out element));
            Assert.IsInstanceOf(typeof(MediaWikiLink), element);
            link = (MediaWikiLink) element;
            Assert.AreEqual("Test", link.Title);
            Assert.IsNull(link.Section);

            // セクション指定付きのリダイレクト
            Assert.IsTrue(parser.TryParse("#redirect [[Test#Section]]", out element));
            Assert.IsInstanceOf(typeof(MediaWikiLink), element);
            link = (MediaWikiLink)element;
            Assert.AreEqual("Test", link.Title);
            Assert.AreEqual("Section", link.Section);

            // 普通の記事
            Assert.IsFalse(parser.TryParse("'''Example''' may refer to:", out element));
            Assert.IsNull(element);

            // enで日本語の転送書式
            Assert.IsFalse(parser.TryParse("#転送 [[Test]]", out element));
            Assert.IsNull(element);

            // jaで日本語の転送書式
            parser = new MediaWikiRedirectParser(new MockFactory().GetMediaWiki("ja"));
            Assert.IsTrue(parser.TryParse("#転送 [[Test]]", out element));
            Assert.IsInstanceOf(typeof(MediaWikiLink), element);
            link = (MediaWikiLink)element;
            Assert.AreEqual("Test", link.Title);
        }

        #endregion
    }
}
