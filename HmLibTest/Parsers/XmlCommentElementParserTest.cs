// ================================================================================================
// <summary>
//      XmlCommentElementParserのテストクラスソース。</summary>
//
// <copyright file="XmlCommentElementParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// XmlCommentElementParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class XmlCommentElementParserTest
    {
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            XmlCommentElementParser parser = new XmlCommentElementParser();
            IElement comment;
            Assert.IsTrue(parser.TryParse("<!--test-->", out comment));
            Assert.AreEqual("<!--test-->", comment.ToString());
            Assert.IsTrue(parser.TryParse("<!-- test -->", out comment));
            Assert.AreEqual("<!-- test -->", comment.ToString());
            Assert.IsTrue(parser.TryParse("<!--test-->-->", out comment));
            Assert.AreEqual("<!--test-->", comment.ToString());
            Assert.IsTrue(parser.TryParse("<!--test--", out comment));
            Assert.AreEqual("<!--test--", comment.ToString());
            Assert.IsTrue(parser.TryParse("<!--->", out comment));
            Assert.AreEqual("<!--->", comment.ToString());
            Assert.IsTrue(parser.TryParse("<!--->-->", out comment));
            Assert.AreEqual("<!--->-->", comment.ToString());
            Assert.IsTrue(parser.TryParse("<!--\n\ntest\r\n-->", out comment));
            Assert.AreEqual("<!--\n\ntest\r\n-->", comment.ToString());
            Assert.IsFalse(parser.TryParse("<--test-->", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(parser.TryParse("<%--test--%>", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(parser.TryParse("<! --test-->", out comment));
            Assert.IsNull(comment);
        }

        /// <summary>
        /// IsPossibleParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestIsPossibleParse()
        {
            XmlCommentElementParser parser = new XmlCommentElementParser();
            Assert.IsTrue(parser.IsPossibleParse('<'));
            Assert.IsFalse(parser.IsPossibleParse('['));
            Assert.IsFalse(parser.IsPossibleParse('-'));
            Assert.IsFalse(parser.IsPossibleParse('/'));
            Assert.IsFalse(parser.IsPossibleParse('#'));
        }

        #endregion
    }
}
