// ================================================================================================
// <summary>
//      Pageのテストクラスソース。</summary>
//
// <copyright file="PageTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using Honememo.Tests;
    using Honememo.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="Page"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class PageTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [TestMethod]
        public void TestConstructorWebsiteTitleTextTimestamp()
        {
            DateTime t = DateTime.Now;
            Website s = new DummySite();
            Uri uri = new Uri("http://example.com/TestTitle");
            Page page = new Page(s, "TestTitle", "TestText", t, uri);
            Assert.AreEqual(s, page.Website);
            Assert.AreEqual("TestTitle", page.Title);
            Assert.AreEqual("TestText", page.Text);
            Assert.AreEqual(t, page.Timestamp);
            Assert.AreSame(uri, page.Uri);
        }

        /// <summary>
        /// コンストラクタテストケース（タイムスタンプ無し）。
        /// </summary>
        [TestMethod]
        public void TestConstructorWebsiteTitleText()
        {
            Website s = new DummySite();
            Page page = new Page(s, "TestTitle", "TestText");
            Assert.AreEqual(s, page.Website);
            Assert.AreEqual("TestTitle", page.Title);
            Assert.AreEqual("TestText", page.Text);
            Assert.IsNull(page.Timestamp);
            Assert.IsNull(page.Uri);
        }

        /// <summary>
        /// コンストラクタテストケース（本文・タイムスタンプ無し）。
        /// </summary>
        [TestMethod]
        public void TestConstructorWebsiteTitle()
        {
            Website s = new DummySite();
            Page page = new Page(s, "TestTitle");
            Assert.AreEqual(s, page.Website);
            Assert.AreEqual("TestTitle", page.Title);
            Assert.IsNull(page.Text);
            Assert.IsNull(page.Timestamp);
            Assert.IsNull(page.Uri);
        }

        /// <summary>
        /// コンストラクタテストケース（ウェブサイトがnull）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorWebsiteNull()
        {
            new Page(null, "TestTitle");
        }

        /// <summary>
        /// コンストラクタテストケース（タイトルが空）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorTitleBlank()
        {
            new Page(new DummySite(), "  ");
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// <see cref="Page.Website"/>プロパティテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestWebsite()
        {
            Page page = new Page(new DummySite(), "TestTitle");
            Website s = new DummySite();
            PrivateAccessor<Page> acc = new PrivateAccessor<Page>(page);
            acc.SetProperty("Website", s);
            Assert.AreEqual(s, page.Website);
        }

        /// <summary>
        /// <see cref="Page.Website"/>プロパティテストケース（null値）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWebsiteNull()
        {
            new PrivateAccessor<Page>(new Page(new DummySite(), "TestTitle"))
                .SetProperty("Website", null);
        }

        /// <summary>
        /// <see cref="Page.Title"/>プロパティテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestTitle()
        {
            Page page = new Page(new DummySite(), "TestTitle");
            PrivateAccessor<Page> acc = new PrivateAccessor<Page>(page);
            acc.SetProperty("Title", "ChangeTitle");
            Assert.AreEqual("ChangeTitle", page.Title);
        }

        /// <summary>
        /// <see cref="Page.Title"/>プロパティテストケース（null値）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTitleNull()
        {
            new PrivateAccessor<Page>(new Page(new DummySite(), "TestTitle"))
                .SetProperty("Title", null);
        }

        /// <summary>
        /// <see cref="Page.Title"/>プロパティテストケース（空）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestTitleBlank()
        {
            new PrivateAccessor<Page>(new Page(new DummySite(), "TestTitle"))
                .SetProperty("Title", "  ");
        }

        /// <summary>
        /// <see cref="Page.Text"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestText()
        {
            Page page = new Page(new DummySite(), "TestTitle");
            PrivateAccessor<Page> acc = new PrivateAccessor<Page>(page);
            acc.SetProperty("Text", "TestText");
            Assert.AreEqual("TestText", page.Text);
        }

        /// <summary>
        /// <see cref="Page.Timestamp"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestTimestamp()
        {
            Page page = new Page(new DummySite(), "TestTitle");
            PrivateAccessor<Page> acc = new PrivateAccessor<Page>(page);
            DateTime t = DateTime.Now;
            acc.SetProperty("Timestamp", t);
            Assert.AreEqual(t, page.Timestamp);
        }

        /// <summary>
        /// <see cref="Page.Uri"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestUri()
        {
            Page page = new Page(new DummySite(), "TestTitle");
            PrivateAccessor<Page> acc = new PrivateAccessor<Page>(page);
            Uri uri = new Uri("http://example.com/TestTitle");
            acc.SetProperty("Uri", uri);
            Assert.AreEqual(uri, page.Uri);
        }

        #endregion

        #region モッククラス

        /// <summary>
        /// <see cref="Page"/>テスト用のモッククラスです。
        /// </summary>
        private class DummySite : Website
        {
            #region ダミーメソッド

            /// <summary>
            /// ページを取得。空実装。
            /// </summary>
            /// <param name="title">ページタイトル。</param>
            /// <returns><c>null</c>。</returns>
            public override Page GetPage(string title)
            {
                return null;
            }

            #endregion
        }

        #endregion
    }
}
