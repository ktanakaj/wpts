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
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Parsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MediaWikiPage"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class MediaWikiPageTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [TestMethod]
        public void TestConstructorWebsiteTitleTextTimestamp()
        {
            DateTime t = DateTime.Now;
            Uri uri = new Uri("http://wikipedia.example");
            MediaWiki s = new MediaWiki(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(s, "TestTitle", "TestText", t, uri);
            Assert.AreSame(s, page.Website);
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
            MediaWiki s = new MediaWiki(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(s, "TestTitle", "TestText");
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
            MediaWiki s = new MediaWiki(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(s, "TestTitle");
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
            new MediaWikiPage(null, "TestTitle");
        }

        /// <summary>
        /// コンストラクタテストケース（タイトルが空）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorTitleBlank()
        {
            new MediaWikiPage(new MediaWiki(new Language("en")), "  ");
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// <see cref="MediaWikiPage.Redirect"/>プロパティテストケース（正常系）。
        /// </summary>
        [TestMethod]
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
        /// <see cref="MediaWikiPage.Redirect"/>プロパティテストケース（Text未設定）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRedirectTextNull()
        {
            MediaWikiLink dummy = new MediaWikiPage(new MockFactory().GetMediaWiki("en"), "TestTitle").Redirect;
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiPage.GetInterlanguage"/>メソッドテストケース（通常ページ）。
        /// </summary>
        [TestMethod]
        public void TestGetInterlanguage()
        {
            // 普通のページ
            MediaWikiPage page = new MediaWikiPage(
                new MockFactory().GetMediaWiki("en"),
                "TestTitle",
                "TestText\n [[ja:テストページ]]<nowiki>[[zh:試験]]</nowiki><!--[[ru:test]]-->[[fr:Test_Fr]]");
            Assert.AreEqual("[[ja:テストページ]]", page.GetInterlanguage("ja").ToString());
            Assert.AreEqual("[[fr:Test_Fr]]", page.GetInterlanguage("fr").ToString());
            Assert.IsNull(page.GetInterlanguage("de"));
            Assert.IsNull(page.GetInterlanguage("ru"));
            Assert.IsNull(page.GetInterlanguage("zh"));
        }

        /// <summary>
        /// <see cref="MediaWikiPage.GetInterlanguage"/>メソッドテストケース（通常ページ実データ使用）。
        /// </summary>
        [TestMethod, Timeout(20000)]
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
        /// <see cref="MediaWikiPage.GetInterlanguage"/>メソッドテストケース（テンプレートページ実データ使用）。
        /// </summary>
        [TestMethod, Timeout(20000)]
        public void TestGetInterlanguagePlanetboxBegin()
        {
            MediaWikiPage page = (MediaWikiPage)new MockFactory().GetMediaWiki("en").GetPage("Template:Planetbox begin");
            Assert.AreEqual("[[ja:Template:Planetbox begin]]", page.GetInterlanguage("ja").ToString());
            Assert.AreEqual("[[ru:Шаблон:Planetbox begin]]", page.GetInterlanguage("ru").ToString());
            Assert.IsNull(page.GetInterlanguage("zh"));
        }

        /// <summary>
        /// <see cref="MediaWikiPage.GetInterlanguage"/>メソッドテストケース（Template:Documentation使用ページ）。
        /// </summary>
        [TestMethod]
        public void TestGetInterlanguageDocumentation()
        {
            // Template:Documentation を使ってるページ
            MediaWiki site = new DummySite(new Language("en"));
            new MockFactory().SetMockConfig(site);
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
        /// <see cref="MediaWikiPage.GetInterlanguage"/>メソッドテストケース（Template:Documentationにnoincludeで囲まれた言語間リンクが存在）。
        /// </summary>
        [TestMethod]
        public void TestGetInterlanguagePartial()
        {
            MediaWikiPage page = (MediaWikiPage)new MockFactory().GetMediaWiki("en").GetPage("Template:Partial");
            Assert.AreEqual("[[ja:Template:Partial]]", page.GetInterlanguage("ja").ToString());
            Assert.IsNull(page.GetInterlanguage("ru"));
        }

        /// <summary>
        /// <see cref="MediaWikiPage.IsRedirect"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// <see cref="MediaWikiPage.Normalize"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestNormalize()
        {
            MediaWiki site = new MockFactory().GetMediaWiki("en");
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
        /// <see cref="MediaWikiPage.ValidateIncomplete"/>メソッドテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestValidateIncomplete()
        {
            // Textが空の場合例外発生、正常系は例外が発生しなければOK
            MediaWikiPageMock page = new MediaWikiPageMock(new MediaWiki(new Language("en")), "TestTitle");
            page.Text = "TestText";
            page.ValidateIncomplete();
        }

        /// <summary>
        /// <see cref="MediaWikiPage.ValidateIncomplete"/>メソッドテストケース（異常系）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestValidateIncompleteNg()
        {
            // Textが空の場合例外発生
            new MediaWikiPageMock(new MediaWiki(new Language("en")), "TestTitle").ValidateIncomplete();
        }

        #endregion

        #region モッククラス

        /// <summary>
        /// <see cref="MediaWikiPage"/>テスト用のモッククラスです。
        /// </summary>
        private class DummySite : MediaWiki
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
            /// ページを取得。<paramref name="title"/>に応じてテスト用の結果を返す。
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

        /// <summary>
        /// <see cref="MediaWikiPage"/>テスト用のモッククラスです。
        /// </summary>
        private class MediaWikiPageMock : MediaWikiPage
        {
            #region コンストラクタ

            /// <summary>
            /// コンストラクタ。
            /// ページの本文, タイムスタンプには<c>null</c>を設定。
            /// </summary>
            /// <param name="website">ページが所属するウェブサイト。</param>
            /// <param name="title">ページタイトル。</param>
            public MediaWikiPageMock(MediaWiki website, string title)
                : base(website, title)
            {
            }

            #endregion

            #region 非公開プロパティテスト用のオーラーライドプロパティ

            /// <summary>
            /// ページの本文。
            /// </summary>
            public new string Text
            {
                get
                {
                    return base.Text;
                }

                set
                {
                    base.Text = value;
                }
            }

            #endregion

            #region 非公開メソッドテスト用のオーラーライドメソッド

            /// <summary>
            /// オブジェクトがメソッドの実行に不完全な状態でないか検証する。
            /// 不完全な場合、例外をスローする。
            /// </summary>
            /// <exception cref="InvalidOperationException">オブジェクトは不完全。</exception>
            public new void ValidateIncomplete()
            {
                base.ValidateIncomplete();
            }

            #endregion
        }

        #endregion
    }
}
