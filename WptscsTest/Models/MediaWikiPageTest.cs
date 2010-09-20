// ================================================================================================
// <summary>
//      MediaWikiPageのテストクラスソース。</summary>
//
// <copyright file="MediaWikiPageTest.cs" company="honeplusのメモ帳">
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
    /// MediaWikiPageのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiPageTest
    {
        // TODO: いっぱい足らない

        #region インスタンスメソッドテストケース

        /// <summary>
        /// IsRedirectメソッドテストケース。
        /// </summary>
        [Test]
        public void TestIsRedirect()
        {
            MediaWiki site = new MediaWiki(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(site, "TestTitle", "#REDIRECT [[Test Redirect]]");
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual("Test Redirect", page.Redirect);

            page = new MediaWikiPage(site, "TestTitle", "#転送 [[Test Redirect2]]");
            Assert.IsFalse(page.IsRedirect());

            site.Redirect = "#転送";
            page = new MediaWikiPage(site, "TestTitle", "#転送 [[Test Redirect2]]");
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual("Test Redirect2", page.Redirect);

            page = new MediaWikiPage(site, "TestTitle", "#REDIRECT [[Test Redirect3]]");
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual("Test Redirect3", page.Redirect);

            page = new MediaWikiPage(site, "TestTitle", "#redirect [[Test Redirect4]]");
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual("Test Redirect4", page.Redirect);
        }

        #endregion

        #region 非公開メソッドテストケース

        // 非公開メソッドについてはprotected以上、またはやりたい部分だけ実施

        /// <summary>
        /// ValidateIncompleteメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestValidateIncomplete()
        {
            // 正常系は例外が発生しなければOK
            PrivateAccessor<MediaWikiPage> acc = new PrivateAccessor<MediaWikiPage>(
                new MediaWikiPage(
                    new MediaWiki(new Language("en")),
                    "TestTitle",
                    "TestText"));
            acc.SetMethod("ValidateIncomplete", new Type[0]);
            acc.Invoke(new object[0]);
        }

        /// <summary>
        /// ValidateIncompleteメソッドテストケース（異常系）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestValidateIncompleteNg()
        {
            // 正常系は例外が発生しなければOK
            PrivateAccessor<MediaWikiPage> acc = new PrivateAccessor<MediaWikiPage>(
                new MediaWikiPage(new MediaWiki(new Language("en")), "TestTitle"));
            acc.SetMethod("ValidateIncomplete");
            acc.Invoke();
        }

        #endregion
    }
}
