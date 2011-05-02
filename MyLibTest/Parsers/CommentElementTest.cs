// ================================================================================================
// <summary>
//      CommentElementのテストクラスソース。</summary>
//
// <copyright file="CommentElementTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// CommentElementのテストクラスです。
    /// </summary>
    [TestFixture]
    public class CommentElementTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            CommentElement comment = new CommentElement();
            Assert.IsNull(comment.Text);

            comment = new CommentElement("test");
            Assert.AreEqual("test", comment.Text);
        }

        #endregion

        #region 静的メソッドテストケース

        /// <summary>
        /// ParseLazyメソッドテストケース。
        /// </summary>
        [Test]
        public void TestParseLazy()
        {
            // ※ 構文についてはTryParseLazyのテストケースで確認
            Assert.AreEqual("<!--test-->", CommentElement.ParseLazy("<!--test-->").ToString());
        }

        /// <summary>
        /// ParseLazyメソッドテストケース。
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void TestParseLazyNg()
        {
            CommentElement.ParseLazy("<--test-->");
        }

        /// <summary>
        /// TryParseLazyメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseLazy()
        {
            CommentElement comment;
            Assert.IsTrue(CommentElement.TryParseLazy("<!--test-->", out comment));
            Assert.AreEqual("<!--test-->", comment.ToString());
            Assert.IsTrue(CommentElement.TryParseLazy("<!-- test -->", out comment));
            Assert.AreEqual("<!-- test -->", comment.ToString());
            Assert.IsTrue(CommentElement.TryParseLazy("<!--test-->-->", out comment));
            Assert.AreEqual("<!--test-->", comment.ToString());
            Assert.IsTrue(CommentElement.TryParseLazy("<!--test--", out comment));
            Assert.AreEqual("<!--test--", comment.ToString());
            Assert.IsTrue(CommentElement.TryParseLazy("<!--->", out comment));
            Assert.AreEqual("<!--->", comment.ToString());
            Assert.IsTrue(CommentElement.TryParseLazy("<!--->-->", out comment));
            Assert.AreEqual("<!--->-->", comment.ToString());
            Assert.IsTrue(CommentElement.TryParseLazy("<!--\n\ntest\r\n-->", out comment));
            Assert.AreEqual("<!--\n\ntest\r\n-->", comment.ToString());
            Assert.IsFalse(CommentElement.TryParseLazy("<--test-->", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(CommentElement.TryParseLazy("<%--test--%>", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(CommentElement.TryParseLazy("<! --test-->", out comment));
            Assert.IsNull(comment);
        }

        /// <summary>
        /// IsElementPossibleメソッドテストケース。
        /// </summary>
        [Test]
        public void TestIsElementPossible()
        {
            Assert.IsTrue(CommentElement.IsElementPossible('<'));
            Assert.IsFalse(CommentElement.IsElementPossible('['));
            Assert.IsFalse(CommentElement.IsElementPossible('-'));
            Assert.IsFalse(CommentElement.IsElementPossible('/'));
            Assert.IsFalse(CommentElement.IsElementPossible('#'));
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            CommentElement comment = new CommentElement();
            Assert.AreEqual("<!---->", comment.ToString());

            comment.Text = "test";
            Assert.AreEqual("<!--test-->", comment.ToString());

            // parseで生成した場合、元の不正構文を保持
            CommentElement.TryParseLazy("<!--test--", out comment);
            Assert.AreEqual("<!--test--", comment.ToString());

            // 一度でも更新されれば解除
            comment.Text = "test2";
            Assert.AreEqual("<!--test2-->", comment.ToString());
        }

        #endregion
    }
}
