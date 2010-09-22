// ================================================================================================
// <summary>
//      Pageのテストクラスソース。</summary>
//
// <copyright file="PageTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Honememo.Tests;
    using Honememo.Utilities;

    /// <summary>
    /// Pageのテストクラスです。
    /// </summary>
    [TestFixture]
    public class PageTest
    {
        /// <summary>
        /// Pageテスト用のモッククラスです。
        /// </summary>
        public class DummySite : Website
        {
            #region ダミーメソッド

            /// <summary>
            /// ページを取得。
            /// </summary>
            /// <param name="title">ページタイトル。</param>
            /// <returns>取得したページ。</returns>
            /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
            public override Page GetPage(string title)
            {
                return null;
            }

            #endregion
        }

        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [Test]
        public void TestConstructorWebsiteTitleTextTimestamp()
        {
            DateTime t = DateTime.Now;
            Website s = new DummySite();
            Page page = new Page(s, "TestTitle", "TestText", t);
            Assert.AreEqual(s, page.Website);
            Assert.AreEqual("TestTitle", page.Title);
            Assert.AreEqual("TestText", page.Text);
            Assert.AreEqual(t, page.Timestamp);
        }

        /// <summary>
        /// コンストラクタテストケース（タイムスタンプ無し）。
        /// </summary>
        [Test]
        public void TestConstructorWebsiteTitleText()
        {
            Website s = new DummySite();
            Page page = new Page(s, "TestTitle", "TestText");
            Assert.AreEqual(s, page.Website);
            Assert.AreEqual("TestTitle", page.Title);
            Assert.AreEqual("TestText", page.Text);
            Assert.IsNull(page.Timestamp);
        }

        /// <summary>
        /// コンストラクタテストケース（本文・タイムスタンプ無し）。
        /// </summary>
        [Test]
        public void TestConstructorWebsiteTitle()
        {
            Website s = new DummySite();
            Page page = new Page(s, "TestTitle");
            Assert.AreEqual(s, page.Website);
            Assert.AreEqual("TestTitle", page.Title);
            Assert.IsNull(page.Text);
            Assert.IsNull(page.Timestamp);
        }

        /// <summary>
        /// コンストラクタテストケース（ウェブサイトがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorWebsiteNull()
        {
            new Page(null, "TestTitle");
        }

        /// <summary>
        /// コンストラクタテストケース（タイトルが空）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorTitleBlank()
        {
            Page page = new Page(new DummySite(), "  ");
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// Websiteプロパティテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestWebsite()
        {
            Page page = new Page(new DummySite(), "TestTitle");
            Website s = new DummySite();
            PrivateAccessor<Page> acc = new PrivateAccessor<Page>(page);
            acc.SetProperty("Website", s);
            Assert.AreEqual(s, page.Website);
        }

        /// <summary>
        /// Websiteプロパティテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWebsiteNull()
        {
            new PrivateAccessor<Page>(new Page(new DummySite(), "TestTitle"))
                .SetProperty("Website", null);
        }

        /// <summary>
        /// Titleプロパティテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestTitle()
        {
            Page page = new Page(new DummySite(), "TestTitle");
            PrivateAccessor<Page> acc = new PrivateAccessor<Page>(page);
            acc.SetProperty("Title", "ChangeTitle");
            Assert.AreEqual("ChangeTitle", page.Title);
        }

        /// <summary>
        /// Titleプロパティテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTitleNull()
        {
            new PrivateAccessor<Page>(new Page(new DummySite(), "TestTitle"))
                .SetProperty("Title", null);
        }

        /// <summary>
        /// Titleプロパティテストケース（空）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestTitleBlank()
        {
            new PrivateAccessor<Page>(new Page(new DummySite(), "TestTitle"))
                .SetProperty("Title", "  ");
        }

        /// <summary>
        /// Textプロパティテストケース。
        /// </summary>
        [Test]
        public void TestText()
        {
            Page page = new Page(new DummySite(), "TestTitle");
            PrivateAccessor<Page> acc = new PrivateAccessor<Page>(page);
            acc.SetProperty("Text", "TestText");
            Assert.AreEqual("TestText", page.Text);
        }

        /// <summary>
        /// Timestampプロパティテストケース。
        /// </summary>
        [Test]
        public void TestTimestamp()
        {
            Page page = new Page(new DummySite(), "TestTitle");
            PrivateAccessor<Page> acc = new PrivateAccessor<Page>(page);
            DateTime t = DateTime.Now;
            acc.SetProperty("Timestamp", t);
            Assert.AreEqual(t, page.Timestamp);
        }

        #endregion

        #region 公開静的メソッドテストケース

        /// <summary>
        /// TryParseCommentメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseComment()
        {
            string comment;
            Assert.IsTrue(Page.TryParseComment("<!--test-->", out comment));
            Assert.AreEqual("<!--test-->", comment);
            Assert.IsTrue(Page.TryParseComment("<!-- test -->", out comment));
            Assert.AreEqual("<!-- test -->", comment);
            Assert.IsTrue(Page.TryParseComment("<!--test-->-->", out comment));
            Assert.AreEqual("<!--test-->", comment);
            Assert.IsTrue(Page.TryParseComment("<!--test--", out comment));
            Assert.AreEqual("<!--test--", comment);
            Assert.IsTrue(Page.TryParseComment("<!--->", out comment));
            Assert.AreEqual("<!--->", comment);
            Assert.IsTrue(Page.TryParseComment("<!--\n\ntest\r\n-->", out comment));
            Assert.AreEqual("<!--\n\ntest\r\n-->", comment);
            Assert.IsFalse(Page.TryParseComment("<--test-->", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(Page.TryParseComment("<%--test--%>", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(Page.TryParseComment("<! --test-->", out comment));
            Assert.IsNull(comment);
        }

        #endregion

    }
}
