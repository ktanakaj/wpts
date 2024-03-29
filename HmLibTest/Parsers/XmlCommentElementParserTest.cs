﻿// ================================================================================================
// <summary>
//      XmlCommentElementParserのテストクラスソース。</summary>
//
// <copyright file="XmlCommentElementParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="XmlCommentElementParser"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class XmlCommentElementParserTest
    {
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="XmlCommentElementParser.TryParse"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
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
            Assert.IsFalse(parser.TryParse(string.Empty, out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(parser.TryParse(null, out comment));
            Assert.IsNull(comment);
        }

        /// <summary>
        /// <see cref="XmlCommentElementParser.IsPossibleParse"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
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
