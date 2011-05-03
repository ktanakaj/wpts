// ================================================================================================
// <summary>
//      XmlCommentElementのテストクラスソース。</summary>
//
// <copyright file="XmlCommentElementTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// XmlCommentElementのテストクラスです。
    /// </summary>
    [TestFixture]
    public class XmlCommentElementTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            XmlCommentElement comment = new XmlCommentElement();
            Assert.IsNull(comment.Text);

            comment = new XmlCommentElement("test");
            Assert.AreEqual("test", comment.Text);
        }

        #endregion

        #region 静的メソッドテストケース

        /// <summary>
        /// Parseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestParse()
        {
            // ※ 構文についてはTryParseのテストケースで確認
            Assert.AreEqual("<!--test-->", XmlCommentElement.Parse("<!--test-->").ToString());
        }

        /// <summary>
        /// Parseメソッドテストケース。
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void TestParseNg()
        {
            XmlCommentElement.Parse("<--test-->");
        }

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            XmlCommentElement comment;
            Assert.IsTrue(XmlCommentElement.TryParse("<!--test-->", out comment));
            Assert.AreEqual("<!--test-->", comment.ToString());
            Assert.IsTrue(XmlCommentElement.TryParse("<!-- test -->", out comment));
            Assert.AreEqual("<!-- test -->", comment.ToString());
            Assert.IsTrue(XmlCommentElement.TryParse("<!--test-->-->", out comment));
            Assert.AreEqual("<!--test-->", comment.ToString());
            Assert.IsFalse(XmlCommentElement.TryParse("<!--test--", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(XmlCommentElement.TryParse("<!--->", out comment));
            Assert.IsNull(comment);
            Assert.IsTrue(XmlCommentElement.TryParse("<!--->-->", out comment));
            Assert.AreEqual("<!--->-->", comment.ToString());
            Assert.IsTrue(XmlCommentElement.TryParse("<!--\n\ntest\r\n-->", out comment));
            Assert.AreEqual("<!--\n\ntest\r\n-->", comment.ToString());
            Assert.IsFalse(XmlCommentElement.TryParse("<--test-->", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(XmlCommentElement.TryParse("<%--test--%>", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(XmlCommentElement.TryParse("<! --test-->", out comment));
            Assert.IsNull(comment);
        }

        /// <summary>
        /// TryParseLazyメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseLazy()
        {
            XmlCommentElement comment;
            Assert.IsTrue(XmlCommentElement.TryParseLazy("<!--test-->", out comment));
            Assert.AreEqual("<!--test-->", comment.ToString());
            Assert.IsTrue(XmlCommentElement.TryParseLazy("<!-- test -->", out comment));
            Assert.AreEqual("<!-- test -->", comment.ToString());
            Assert.IsTrue(XmlCommentElement.TryParseLazy("<!--test-->-->", out comment));
            Assert.AreEqual("<!--test-->", comment.ToString());
            Assert.IsTrue(XmlCommentElement.TryParseLazy("<!--test--", out comment));
            Assert.AreEqual("<!--test--", comment.ToString());
            Assert.IsTrue(XmlCommentElement.TryParseLazy("<!--->", out comment));
            Assert.AreEqual("<!--->", comment.ToString());
            Assert.IsTrue(XmlCommentElement.TryParseLazy("<!--->-->", out comment));
            Assert.AreEqual("<!--->-->", comment.ToString());
            Assert.IsTrue(XmlCommentElement.TryParseLazy("<!--\n\ntest\r\n-->", out comment));
            Assert.AreEqual("<!--\n\ntest\r\n-->", comment.ToString());
            Assert.IsFalse(XmlCommentElement.TryParseLazy("<--test-->", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(XmlCommentElement.TryParseLazy("<%--test--%>", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(XmlCommentElement.TryParseLazy("<! --test-->", out comment));
            Assert.IsNull(comment);
        }

        /// <summary>
        /// IsElementPossibleメソッドテストケース。
        /// </summary>
        [Test]
        public void TestIsElementPossible()
        {
            Assert.IsTrue(XmlCommentElement.IsElementPossible('<'));
            Assert.IsFalse(XmlCommentElement.IsElementPossible('['));
            Assert.IsFalse(XmlCommentElement.IsElementPossible('-'));
            Assert.IsFalse(XmlCommentElement.IsElementPossible('/'));
            Assert.IsFalse(XmlCommentElement.IsElementPossible('#'));
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            XmlCommentElement comment = new XmlCommentElement();
            Assert.AreEqual("<!---->", comment.ToString());

            comment.Text = "test";
            Assert.AreEqual("<!--test-->", comment.ToString());

            // parseで生成した場合、元の不正構文を保持
            XmlCommentElement.TryParseLazy("<!--test--", out comment);
            Assert.AreEqual("<!--test--", comment.ToString());
        }

        #endregion
    }
}
