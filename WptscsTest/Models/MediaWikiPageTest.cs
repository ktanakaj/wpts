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

        #region 公開静的メソッドテストケース

        /// <summary>
        /// TryParseCommentメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseComment()
        {
            string comment;
            Assert.IsTrue(MediaWikiPage.TryParseComment("<!--test-->", out comment));
            Assert.AreEqual("<!--test-->", comment);
            Assert.IsTrue(MediaWikiPage.TryParseComment("<!-- test -->", out comment));
            Assert.AreEqual("<!-- test -->", comment);
            Assert.IsTrue(MediaWikiPage.TryParseComment("<!--test-->-->", out comment));
            Assert.AreEqual("<!--test-->", comment);
            Assert.IsTrue(MediaWikiPage.TryParseComment("<!--test--", out comment));
            Assert.AreEqual("<!--test--", comment);
            Assert.IsTrue(MediaWikiPage.TryParseComment("<!--\n\ntest\r\n-->", out comment));
            Assert.AreEqual("<!--\n\ntest\r\n-->", comment);
            Assert.IsFalse(MediaWikiPage.TryParseComment("<--test-->", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(MediaWikiPage.TryParseComment("<%--test--%>", out comment));
            Assert.IsNull(comment);
            Assert.IsFalse(MediaWikiPage.TryParseComment("<! --test-->", out comment));
            Assert.IsNull(comment);
        }

        /// <summary>
        /// ChkCommentメソッドテストケース。
        /// </summary>
        [Test]
        public void TestChkComment()
        {
            // TryParseComment互換用の旧メソッド
            string comment;
            Assert.AreEqual(14, MediaWikiPage.ChkComment(out comment, "ab<!-- test -->cd", 2));
            Assert.AreEqual("<!-- test -->", comment);
            Assert.AreEqual(15, MediaWikiPage.ChkComment(out comment, "ab<!-- test --cd", 2));
            Assert.AreEqual("<!-- test --cd", comment);
            Assert.AreEqual(-1, MediaWikiPage.ChkComment(out comment, "ab<!-- test -->cd", 1));
            Assert.IsEmpty(comment);
            Assert.AreEqual(-1, MediaWikiPage.ChkComment(out comment, "ab<!-- test -->cd", 3));
            Assert.IsEmpty(comment);
        }

        /// <summary>
        /// TryParseNowikiメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseNowiki()
        {
            string nowiki;
            Assert.IsTrue(MediaWikiPage.TryParseNowiki("<nowiki>[[test]]</nowiki>", out nowiki));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", nowiki);
            Assert.IsTrue(MediaWikiPage.TryParseNowiki("<NOWIKI>[[test]]</NOWIKI>", out nowiki));
            Assert.AreEqual("<NOWIKI>[[test]]</NOWIKI>", nowiki);
            Assert.IsTrue(MediaWikiPage.TryParseNowiki("<Nowiki>[[test]]</noWiki>", out nowiki));
            Assert.AreEqual("<Nowiki>[[test]]</noWiki>", nowiki);
            Assert.IsTrue(MediaWikiPage.TryParseNowiki("<nowiki>[[test]]</nowiki></nowiki>", out nowiki));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", nowiki);
            Assert.IsTrue(MediaWikiPage.TryParseNowiki("<nowiki>[[test]]nowiki", out nowiki));
            Assert.AreEqual("<nowiki>[[test]]nowiki", nowiki);
            Assert.IsTrue(MediaWikiPage.TryParseNowiki("<nowiki>\n\n[[test]]\r\n</nowiki>", out nowiki));
            Assert.AreEqual("<nowiki>\n\n[[test]]\r\n</nowiki>", nowiki);
            Assert.IsTrue(MediaWikiPage.TryParseNowiki("<nowiki><!--[[test]]--></nowiki>", out nowiki));
            Assert.AreEqual("<nowiki><!--[[test]]--></nowiki>", nowiki);
            Assert.IsTrue(MediaWikiPage.TryParseNowiki("<nowiki><!--<nowiki>[[test]]</nowiki>--></nowiki>", out nowiki));
            Assert.AreEqual("<nowiki><!--<nowiki>[[test]]</nowiki>--></nowiki>", nowiki);
            Assert.IsTrue(MediaWikiPage.TryParseNowiki("<nowiki><!--[[test]]", out nowiki));
            Assert.AreEqual("<nowiki><!--[[test]]", nowiki);
            Assert.IsFalse(MediaWikiPage.TryParseNowiki("<nowik>[[test]]</nowik>", out nowiki));
            Assert.IsNull(nowiki);
            Assert.IsFalse(MediaWikiPage.TryParseNowiki("<nowiki[[test]]</nowiki>", out nowiki));
            Assert.IsNull(nowiki);
        }

        /// <summary>
        /// ChkNowikiメソッドテストケース。
        /// </summary>
        [Test]
        public void TestChkNowiki()
        {
            // TryParseNowiki互換用の旧メソッド
            string nowiki;
            Assert.AreEqual(26, MediaWikiPage.ChkNowiki(out nowiki, "ab<nowiki>[[test]]</nowiki>cd", 2));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", nowiki);
            Assert.AreEqual(27, MediaWikiPage.ChkNowiki(out nowiki, "ab<nowiki>[[test]]</nowikicd", 2));
            Assert.AreEqual("<nowiki>[[test]]</nowikicd", nowiki);
            Assert.AreEqual(-1, MediaWikiPage.ChkNowiki(out nowiki, "ab<nowiki>[[test]]</nowiki>cd", 1));
            Assert.IsEmpty(nowiki);
            Assert.AreEqual(-1, MediaWikiPage.ChkNowiki(out nowiki, "ab<nowiki>[[test]]</nowiki>cd", 3));
            Assert.IsEmpty(nowiki);
        }

        #endregion

        #region 公開インスタンスメソッドテストケース

        /// <summary>
        /// GetInterWikiメソッドテストケース（通常ページ）。
        /// </summary>
        [Test]
        public void TestGetInterWiki()
        {
            // 普通のページ
            MediaWikiPage page = new MediaWikiPage(new MediaWiki(new Language("en")), "TestTitle", "TestText\n"
                + " [[ja:テストページ]]<nowiki>[[zh:試験]]</nowiki><!--[[ru:test]]-->[[fr:Test_Fr]]");
            Assert.AreEqual("テストページ", page.GetInterWiki("ja"));
            Assert.AreEqual("Test_Fr", page.GetInterWiki("fr"));
            Assert.IsEmpty(page.GetInterWiki("de"));
            Assert.IsEmpty(page.GetInterWiki("ru"));
            Assert.IsEmpty(page.GetInterWiki("zh"));
        }

        /// <summary>
        /// GetInterWikiメソッドテストケース（Template:Documentation使用ページ）。
        /// </summary>
        [Test]
        public void TestGetInterWikiDocumentation()
        {
            // Template:Documentation を使ってるページ
            MediaWiki site = new MediaWiki(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(site, "Template:Test", "TestText{{Documentation}}");

            // TODO: 作成中

            Assert.AreEqual("テストページ", page.GetInterWiki("ja"));
            Assert.AreEqual("Test_Fr", page.GetInterWiki("fr"));
            Assert.IsEmpty(page.GetInterWiki("de"));
            Assert.IsEmpty(page.GetInterWiki("ru"));
            Assert.IsEmpty(page.GetInterWiki("zh"));
        }

        /// <summary>
        /// IsRedirectメソッドテストケース。
        /// </summary>
        [Test]
        public void TestIsRedirect()
        {
            MediaWiki site = new MediaWiki(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(site, "TestTitle", "#REDIRECT [[Test Redirect]]");
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual("Test Redirect", page.Redirect.Title);

            page = new MediaWikiPage(site, "TestTitle", "#転送 [[Test Redirect2]]");
            Assert.IsFalse(page.IsRedirect());

            site.Redirect = "#転送";
            page = new MediaWikiPage(site, "TestTitle", "#転送 [[Test Redirect2]]");
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual("Test Redirect2", page.Redirect.Title);

            page = new MediaWikiPage(site, "TestTitle", "#REDIRECT [[Test Redirect3]]");
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual("Test Redirect3", page.Redirect.Title);

            page = new MediaWikiPage(site, "TestTitle", "#redirect [[Test Redirect4]]");
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual("Test Redirect4", page.Redirect.Title);
        }

        #endregion

        #region 内部処理用インスタンスメソッドテストケース

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
