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
    using Honememo.Parsers;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Websites;
    using NUnit.Framework;

    /// <summary>
    /// <see cref="MediaWikiRedirectParser"/>のテストクラスです。
    /// </summary>
    [TestFixture]
    class MediaWikiRedirectParserTest
    {
        #region ITextParserインタフェース実装メソッド

        /// <summary>
        /// <see cref="MediaWikiRedirectParser.TryParseToEndCondition"/>メソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseToEndCondition()
        {
            IElement element;
            MediaWikiLink link;
            using (MediaWikiRedirectParser parser = new MediaWikiRedirectParser(new MockFactory().GetMediaWiki("en")))
            {
                // 通常のリダイレクト
                Assert.IsTrue(parser.TryParseToEndCondition("#redirect [[Test]]", null, out element));
                Assert.IsInstanceOf(typeof(MediaWikiLink), element);
                link = (MediaWikiLink)element;
                Assert.AreEqual("Test", link.Title);
                Assert.IsNull(link.Section);

                // セクション指定付きのリダイレクト
                Assert.IsTrue(parser.TryParseToEndCondition("#redirect [[Test#Section]]", null, out element));
                Assert.IsInstanceOf(typeof(MediaWikiLink), element);
                link = (MediaWikiLink)element;
                Assert.AreEqual("Test", link.Title);
                Assert.AreEqual("Section", link.Section);

                // 普通の記事
                Assert.IsFalse(parser.TryParseToEndCondition("'''Example''' may refer to:", null, out element));
                Assert.IsNull(element);

                // enで日本語の転送書式
                Assert.IsFalse(parser.TryParseToEndCondition("#転送 [[Test]]", null, out element));
                Assert.IsNull(element);

                // 空文字列・null
                Assert.IsFalse(parser.TryParseToEndCondition(String.Empty, null, out element));
                Assert.IsNull(element);
                Assert.IsFalse(parser.TryParseToEndCondition(null, null, out element));
                Assert.IsNull(element);
            }

            using (MediaWikiRedirectParser parser = new MediaWikiRedirectParser(new MockFactory().GetMediaWiki("ja")))
            {
                // jaで日本語の転送書式
                Assert.IsTrue(parser.TryParseToEndCondition("#転送 [[Test]]", null, out element));
                Assert.IsInstanceOf(typeof(MediaWikiLink), element);
                link = (MediaWikiLink)element;
                Assert.AreEqual("Test", link.Title);
            }
        }

        /// <summary>
        /// <see cref="MediaWikiRedirectParser.TryParseToEndCondition"/>
        /// メソッドテストケース（Dispose）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestTryParseToEndConditionDispose()
        {
            MediaWikiRedirectParser parser = new MediaWikiRedirectParser(new MockFactory().GetMediaWiki("en"));
            parser.Dispose();
            IElement result;
            parser.TryParseToEndCondition(String.Empty, null, out result);
        }

        #endregion
    }
}
