// ================================================================================================
// <summary>
//      MediaWikiPageのテストクラスソース。</summary>
//
// <copyright file="MediaWikiPageTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2013 Honeplus. All rights reserved.</copyright>
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
        #region 公開メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiPage.GetInterlanguage"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestGetInterlanguage()
        {
            MediaWikiPageMock page = new MediaWikiPageMock(
                new MockFactory().GetMediaWiki("en"),
                "TestTitle");
            page.Interlanguages.Add("ja", "テストページ");
            page.Interlanguages.Add("fr", "Test_Fr");
            Assert.AreEqual("テストページ", page.GetInterlanguage("ja"));
            Assert.AreEqual("Test_Fr", page.GetInterlanguage("fr"));
            Assert.IsNull(page.GetInterlanguage("de"));
            Assert.IsNull(page.GetInterlanguage("ru"));
            Assert.IsNull(page.GetInterlanguage("zh"));
        }

        /// <summary>
        /// <see cref="MediaWikiPage.IsRedirect"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestIsRedirect()
        {
            MediaWiki site = new MediaWiki(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(site, "TestTitle");
            Assert.IsFalse(page.IsRedirect());
            Assert.IsNull(page.Redirect);

            page.Redirect = "Test Redirect";
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual("Test Redirect", page.Redirect);

            page.Redirect = string.Empty;
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual(string.Empty, page.Redirect);

            page.Redirect = null;
            Assert.IsFalse(page.IsRedirect());
            Assert.IsNull(page.Redirect);
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

            /// <summary>
            /// 言語間リンクの対応表。
            /// </summary>
            public new IDictionary<string, string> Interlanguages
            {
                get
                {
                    return base.Interlanguages;
                }
            }

            #endregion
        }

        #endregion
    }
}
