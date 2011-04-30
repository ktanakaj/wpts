// ================================================================================================
// <summary>
//      MediaWikiPageのテストクラスソース。</summary>
//
// <copyright file="MediaWikiPageTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Honememo.Tests;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;

    /// <summary>
    /// MediaWikiPageのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiPageTest
    {
        #region モッククラス

        /// <summary>
        /// Websiteテスト用のモッククラスです。
        /// </summary>
        public class DummySite : MediaWiki
        {
            #region コンストラクタ

            /// <summary>
            /// コンストラクタ。
            /// </summary>
            /// <param name="lang">ウェブサイトの言語。</param>
            public DummySite(Language lang)
                : base(lang)
            {
            }

            #endregion

            #region ダミーメソッド

            /// <summary>
            /// ページを取得。
            /// </summary>
            /// <param name="title">ページタイトル。</param>
            /// <returns>取得したページ。</returns>
            /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
            public override Page GetPage(string title)
            {
                System.Diagnostics.Debug.WriteLine(title);
                if (title == "Template:Test/doc")
                {
                    return new MediaWikiPage(
                        this,
                        title,
                        "[[ja:テストページ]]<nowiki>[[zh:試験]]</nowiki><!--[[ru:test]]-->[[fr:Test_Fr]]");
                }

                return base.GetPage(title);
            }

            #endregion
        }

        #endregion

        // TODO: いっぱい足らない

        #region 公開静的メソッドテストケース
        
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
            MediaWiki site = new DummySite(new Language("en"));
            site.DocumentationTemplate = "Template:Documentation";
            site.DocumentationTemplateDefaultPage = "/doc";
            MediaWikiPage page = new MediaWikiPage(site, "Template:Test", "TestText{{Documentation}}");

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
