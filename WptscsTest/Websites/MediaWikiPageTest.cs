// ================================================================================================
// <summary>
//      MediaWikiPageのテストクラスソース。</summary>
//
// <copyright file="MediaWikiPageTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using Honememo.Parsers;
    using Honememo.Tests;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Parsers;
    using NUnit.Framework;

    /// <summary>
    /// MediaWikiPageのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiPageTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [Test]
        public void TestConstructorWebsiteTitleTextTimestamp()
        {
            DateTime t = DateTime.Now;
            MediaWiki s = new DummySite(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(s, "TestTitle", "TestText", t);
            Assert.AreSame(s, page.Website);
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
            MediaWiki s = new DummySite(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(s, "TestTitle", "TestText");
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
            MediaWiki s = new DummySite(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(s, "TestTitle");
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
            new MediaWikiPage(null, "TestTitle");
        }

        /// <summary>
        /// コンストラクタテストケース（タイトルが空）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorTitleBlank()
        {
            new MediaWikiPage(new DummySite(new Language("en")), "  ");
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Redirectプロパティテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestRedirect()
        {
            MediaWiki site;

            site = new MockFactory().GetMediaWiki("en");
            Assert.IsNull(new MediaWikiPage(site, "TestTitle", "[[TestLink]]").Redirect);
            Assert.IsNotNull(new MediaWikiPage(site, "TestTitle", "#redirect [[TestLink]]").Redirect);
            Assert.AreEqual("[[TestLink]]", new MediaWikiPage(site, "TestTitle", "#redirect [[TestLink]]").Redirect.ToString());
            Assert.IsNull(new MediaWikiPage(site, "TestTitle", "#転送 [[TestLink]]").Redirect);

            site = new MockFactory().GetMediaWiki("ja");
            Assert.IsNull(new MediaWikiPage(site, "TestTitle", "[[TestLink]]").Redirect);
            Assert.IsNotNull(new MediaWikiPage(site, "TestTitle", "#redirect [[TestLink]]").Redirect);
            Assert.AreEqual("[[TestLink]]", new MediaWikiPage(site, "TestTitle", "#redirect [[TestLink]]").Redirect.ToString());
            Assert.IsNotNull(new MediaWikiPage(site, "TestTitle", "#転送 [[TestLink]]").Redirect);
            Assert.AreEqual("[[TestLink]]", new MediaWikiPage(site, "TestTitle", "#転送 [[TestLink]]").Redirect.ToString());
        }

        /// <summary>
        /// Redirectプロパティテストケース（Text未設定）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRedirectTextNull()
        {
            MediaWikiLink dummy = new MediaWikiPage(new MockFactory().GetMediaWiki("en"), "TestTitle").Redirect;
        }

        /// <summary>
        /// Elementプロパティテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestElement()
        {
            IElement element = new MediaWikiPage(new MockFactory().GetMediaWiki("en"), "TestTitle", "'''Title''' is [[xxx]].").Element;
            Assert.IsNotNull(element);
            Assert.AreEqual("'''Title''' is [[xxx]].", element.ToString());
            Assert.IsInstanceOf(typeof(ListElement), element);
            ListElement list = (ListElement)element;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("'''Title''' is ", list[0].ToString());
            Assert.AreEqual("[[xxx]]", list[1].ToString());
            Assert.IsInstanceOf(typeof(MediaWikiLink), list[1]);
            Assert.AreEqual(".", list[2].ToString());
        }

        /// <summary>
        /// Elementプロパティテストケース（Text未設定）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestElementTextNull()
        {
            IElement dummy = new MediaWikiPage(new MockFactory().GetMediaWiki("en"), "TestTitle").Element;
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// GetInterlanguageメソッドテストケース（通常ページ）。
        /// </summary>
        [Test]
        public void TestGetInterlanguage()
        {
            // 普通のページ
            MediaWikiPage page = new MediaWikiPage(
                new DummySite(new Language("en")),
                "TestTitle",
                "TestText\n [[ja:テストページ]]<nowiki>[[zh:試験]]</nowiki><!--[[ru:test]]-->[[fr:Test_Fr]]");
            Assert.AreEqual("[[ja:テストページ]]", page.GetInterlanguage("ja").ToString());
            Assert.AreEqual("[[fr:Test_Fr]]", page.GetInterlanguage("fr").ToString());
            Assert.IsNull(page.GetInterlanguage("de"));
            Assert.IsNull(page.GetInterlanguage("ru"));
            Assert.IsNull(page.GetInterlanguage("zh"));
        }

        /// <summary>
        /// GetInterlanguageメソッドテストケース（通常ページ実データ使用）。
        /// </summary>
        [Test, Timeout(20000)]
        public void TestGetInterlanguageDiscoveryChannel()
        {
            MediaWikiPage page = (MediaWikiPage)new MockFactory().GetMediaWiki("en").GetPage("Discovery Channel");
            Assert.AreEqual("[[ja:ディスカバリーチャンネル]]", page.GetInterlanguage("ja").ToString());
            Assert.AreEqual("[[it:Discovery Channel (Italia)]]", page.GetInterlanguage("it").ToString());
            Assert.AreEqual("[[simple:Discovery Channel]]", page.GetInterlanguage("simple").ToString());
            Assert.AreEqual("[[ru:Discovery (телеканал)]]", page.GetInterlanguage("ru").ToString());
            Assert.IsNull(page.GetInterlanguage("io"));
        }

        /// <summary>
        /// GetInterlanguageメソッドテストケース（テンプレートページ実データ使用）。
        /// </summary>
        [Test, Timeout(20000)]
        public void TestGetInterlanguagePlanetboxBegin()
        {
            MediaWikiPage page = (MediaWikiPage)new MockFactory().GetMediaWiki("en").GetPage("Template:Planetbox begin");
            Assert.AreEqual("[[ja:Template:Planetbox begin]]", page.GetInterlanguage("ja").ToString());
            Assert.AreEqual("[[ru:Шаблон:Planetbox begin]]", page.GetInterlanguage("ru").ToString());
            Assert.IsNull(page.GetInterlanguage("zh"));
        }

        /// <summary>
        /// GetInterlanguageメソッドテストケース（Template:Documentation使用ページ）。
        /// </summary>
        [Test]
        public void TestGetInterlanguageDocumentation()
        {
            // Template:Documentation を使ってるページ
            MediaWiki site = new DummySite(new Language("en"));
            site.DocumentationTemplates.Add("Template:Documentation");
            site.DocumentationTemplateDefaultPage = "/doc";
            MediaWikiPage page = new MediaWikiPage(site, "Template:Test", "TestText{{Documentation}}");

            Assert.AreEqual("[[ja:テストページ]]", page.GetInterlanguage("ja").ToString());
            Assert.AreEqual("[[fr:Test_Fr]]", page.GetInterlanguage("fr").ToString());
            Assert.IsNull(page.GetInterlanguage("de"));
            Assert.IsNull(page.GetInterlanguage("ru"));
            Assert.IsNull(page.GetInterlanguage("zh"));
        }

        /// <summary>
        /// IsRedirectメソッドテストケース。
        /// </summary>
        [Test]
        public void TestIsRedirect()
        {
            MediaWiki site = new DummySite(new Language("en"));
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

        /// <summary>
        /// Normalizeメソッドテストケース。
        /// </summary>
        [Test]
        public void TestNormalize()
        {
            MediaWiki site = new DummySite(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(site, "A/b/c");

            // サブページの正規化
            Assert.AreEqual("Normal page", page.Normalize(new MediaWikiLink("Normal page")));
            Assert.AreEqual("A/b/c/s", page.Normalize(new MediaWikiLink("/s")));
            Assert.AreEqual("A/b/c/s", page.Normalize(new MediaWikiLink("/s/")));
            Assert.AreEqual("A/b", page.Normalize(new MediaWikiLink("../")));
            Assert.AreEqual("A", page.Normalize(new MediaWikiLink("../../")));
            Assert.AreEqual("A/b/s", page.Normalize(new MediaWikiLink("../s")));
            Assert.AreEqual("A/b/s", page.Normalize(new MediaWikiLink("../s/")));
            Assert.AreEqual("A/s", page.Normalize(new MediaWikiLink("../../s")));
            Assert.AreEqual("A/s", page.Normalize(new MediaWikiLink("../../s/")));

            // テンプレートの正規化
            Assert.AreEqual("Template:Template page", page.Normalize(new MediaWikiTemplate("Template page")));
            Assert.AreEqual("Normal page", page.Normalize(new MediaWikiTemplate("Normal page") { IsColon = true }));
            Assert.AreEqual("Wikipedia:Help page", page.Normalize(new MediaWikiTemplate("Wikipedia:Help page")));
            Assert.AreEqual("template:Template page", page.Normalize(new MediaWikiTemplate("template:Template page")));
            Assert.AreEqual("A/b/c/Doc", page.Normalize(new MediaWikiTemplate("/Doc")));
            Assert.AreEqual("CURRENTYEAR", page.Normalize(new MediaWikiTemplate("CURRENTYEAR")));
        }

        #endregion

        #region 内部処理用メソッドテストケース

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

        #region モッククラス

        /// <summary>
        /// MediaWikiテスト用のモッククラスです。
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
    }
}
