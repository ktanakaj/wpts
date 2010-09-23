// ================================================================================================
// <summary>
//      LazyXmlParserのテストクラスソース。</summary>
//
// <copyright file="LazyXmlParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// LazyXmlParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class LazyXmlParserTest
    {
        // TODO:いっぱい足りない

        #region 公開静的メソッドテストケース

        /// <summary>
        /// TryParseCommentメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseComment()
        {
            string comment;
            Assert.IsTrue(LazyXmlParser.TryParseComment("<!--test-->", out comment));
            Assert.AreEqual("<!--test-->", comment);
            Assert.IsTrue(LazyXmlParser.TryParseComment("<!-- test -->", out comment));
            Assert.AreEqual("<!-- test -->", comment);
            Assert.IsTrue(LazyXmlParser.TryParseComment("<!--test-->-->", out comment));
            Assert.AreEqual("<!--test-->", comment);
            Assert.IsTrue(LazyXmlParser.TryParseComment("<!--test--", out comment));
            Assert.AreEqual("<!--test--", comment);
            Assert.IsTrue(LazyXmlParser.TryParseComment("<!--->", out comment));
            Assert.AreEqual("<!--->", comment);
            Assert.IsTrue(LazyXmlParser.TryParseComment("<!--\n\ntest\r\n-->", out comment));
            Assert.AreEqual("<!--\n\ntest\r\n-->", comment);
            Assert.IsFalse(LazyXmlParser.TryParseComment("<--test-->", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(LazyXmlParser.TryParseComment("<%--test--%>", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(LazyXmlParser.TryParseComment("<! --test-->", out comment));
            Assert.IsNull(comment);
        }

        #endregion
    }
}
